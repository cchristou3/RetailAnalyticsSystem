from typing import List

import pandas as pd
from mlxtend.frequent_patterns import fpmax, association_rules, fpgrowth
from mlxtend.preprocessing import TransactionEncoder
from pandas import DataFrame

from services.utilities import to_excel
from sql_client import SQLClient


# TODO: Using this article https://dataaspirant.com/association-rule-analysis/#:~:text=Before%20performing%20association%20rule%20analysis%2C%20it%20is%20necessary,or%20irrelevant%20data%20Handling%20missing%20or%20incomplete%20data
#  Implement FPM, and then specialize it to use the data of a specified group.
#  E.g. Give me the ARs of the most frequent clients (use RFM model)


def mine_rules():
    # Define the file path to the CSV file containing sales data
    file_path: str = './../Sales Data Online Shop.csv'

    # Load the CSV file into a pandas DataFrame
    # Setting low_memory=False to prevent dtype guessing and reduce memory usage
    data: DataFrame = pd.read_csv(file_path, low_memory=False)

    data['Sales'] = data['Quantity'] * data['UnitPrice']

    # Get the most profitable customers
    customer_category_performance: pd.DataFrame = data.groupby('CustomerId')['Sales'].sum().sort_values(ascending=False)
    print('Customer Performance by Category:')
    print(customer_category_performance)
    print()

    # Get the first 20% of the rows
    n_rows = int(len(customer_category_performance) * 0.3)
    first_20_percent = customer_category_performance.head(n_rows)

    # Display the first 20% of the filtered rows
    print(first_20_percent.index.values)
    top_20_customers = first_20_percent.index.values

    data = data[data['CustomerId'].isin(top_20_customers)]

    data['InvoiceDate'] = pd.to_datetime(data['InvoiceDate'])
    data['InvoiceYear'] = data['InvoiceDate'].dt.year
    data['InvoiceMonth'] = data['InvoiceDate'].dt.to_period('M')

    invoice_years: List[int] = data['InvoiceYear'].unique()
    invoice_months: List[str] = data['InvoiceMonth'].unique()
    customer_categories: List[str] = data['CustomerCategory'].unique()
    package_types: List[str] = data['PackageType'].unique()

    for invoice_year in invoice_years:
        for customer_category in customer_categories:
            for package_type in package_types:
                for invoice_month in invoice_months:

                    # Step 2: Filter the data based on the criteria
                    filtered_data = data[(data['InvoiceYear'] == invoice_year) &
                                         (data['CustomerCategory'] == customer_category) &
                                         (data['PackageType'] == package_type) &
                                         (data['InvoiceMonth'] == invoice_month)]

                    try:

                        if len(filtered_data) <= 1:
                            continue

                        # Transform the transactions into the below format
                        # dataset = [['beer', 'nuts', 'diapers'],
                        #            ['beer', 'cheese', 'diapers'],
                        #            ['beer', 'cheese', 'nuts'],
                        #            ['cheese', 'nuts']]

                        # Step 2: Preprocess the data
                        # Group by InvoiceID and aggregate ProductName into lists
                        grouped = filtered_data.groupby('InvoiceID')['ProductName'].apply(list).reset_index()

                        if len(grouped) <= 4:
                            continue

                        # Step 3: Transform the data into the desired format
                        dataset = grouped['ProductName'].tolist()

                        # Convert dataset to one-hot encoded DataFrame
                        te = TransactionEncoder()
                        te_ary = te.fit(dataset).transform(dataset)
                        filtered_data = pd.DataFrame(te_ary, columns=te.columns_)

                        # Apply FP-max algorithm with min_support = 0.5
                        frequent_item_sets = fpmax(filtered_data, min_support=0.5, use_colnames=True)

                        if len(frequent_item_sets) == 0:
                            continue

                        print(
                            f'Perform Frequent Mining on: {len(filtered_data)} records ({invoice_year}, {customer_category}, {package_type})')

                        # Print the frequent items ets
                        # print(frequent_item_sets)

                        # Generate association rules
                        rules = association_rules(frequent_item_sets, metric="lift", min_threshold=0.1)

                        if len(rules) == 0:
                            continue

                        # Print frequent item sets and association rules
                        print("Frequent Item sets:")
                        print(frequent_item_sets)
                        print("\nAssociation Rules:")
                        print(rules)
                    except Exception as ex:
                        print(
                            f'EXCEPTION: Perform Frequent Mining on: {len(filtered_data)} records ({invoice_year}, {customer_category}, {package_type})')
                        raise


def frozenset_to_string(frozen_set):
    return ' & '.join(sorted(frozen_set))


def mine_rules_with_rfm_segments():
    client = SQLClient()

    data: pd.DataFrame = client.get_fpm_prerequisite_data()

    customer_segments: List[str] = data['CustomerSegment'].unique()

    all_rules: pd.DataFrame = pd.DataFrame()

    for customer_segment in customer_segments:

        # Step 2: Filter the data based on the criteria
        filtered_data = data[data['CustomerSegment'] == customer_segment]

        # Check the size of the filtered data
        print(f'Number of transactions after filtering: {len(filtered_data)}')

        if len(filtered_data) <= 1:
            print('Filtered data has 1 or fewer transactions. Exiting.')
            continue

        try:
            # Group by InvoiceID and aggregate ProductName into lists
            grouped = filtered_data.groupby('InvoiceId')['ProductName'].apply(list).reset_index()

            # Check the size of the grouped data
            print(f'Number of grouped transactions: {len(grouped)}')

            if len(grouped) <= 4:
                print('Grouped data has 4 or fewer transactions. Exiting.')
                continue

            # Step 3: Transform the data into the desired format
            dataset = grouped['ProductName'].tolist()

            # Convert dataset to one-hot encoded DataFrame
            te = TransactionEncoder()
            te_ary = te.fit(dataset).transform(dataset)
            one_hot_data = pd.DataFrame(te_ary, columns=te.columns_)

            # Analyze item frequencies
            item_frequencies = one_hot_data.sum(axis=0).sort_values(ascending=False)
            print(f'Item frequencies:\n{item_frequencies}')

            # Apply FP-growth algorithm with lower min_support = 0.01
            frequent_item_sets = fpgrowth(one_hot_data, min_support=0.00125, use_colnames=True)

            if frequent_item_sets.empty:
                print('No frequent item sets found. Exiting.')
                continue

            print(f'Perform Frequent Mining on: {len(one_hot_data)} records')

            # Generate association rules
            rules = association_rules(frequent_item_sets, metric="confidence", min_threshold=0.8)

            if rules.empty:
                print('No association rules found. Exiting.')
                continue

            # Print frequent item sets and association rules
            print(f"Association Rules ({len(te_ary)}):")
            # Extract the required columns
            columns_to_extract = ['antecedents', 'consequents', 'support', 'confidence', 'lift']
            df_extracted: pd.DataFrame = rules[columns_to_extract]

            df_extracted.loc[:, 'Segment'] = customer_segment
            all_rules = pd.concat([all_rules, df_extracted], ignore_index=True)

        except Exception as e:
            print(f'EXCEPTION: An error occurred during the mining process: {e}')
            raise

    all_rules['antecedents'] = all_rules['antecedents'].apply(frozenset_to_string)
    all_rules['consequents'] = all_rules['consequents'].apply(frozenset_to_string)

    all_rules = all_rules.rename(columns={
        'antecedents': 'LeftHand',
        'consequents': 'RightHand',
        'support': 'Support',
        'confidence': 'Confidence',
        'lift': 'Lift'
    })

    all_rules['Support'] = all_rules['Support'].round(5)
    all_rules['Lift'] = all_rules['Lift'].round(5)
    all_rules['Confidence'] = all_rules['Confidence'].round(5)

    to_excel(all_rules, 'rules.xlsx')

    all_rules_dict = all_rules.to_dict(orient='records')
    print(all_rules_dict)

    client.insert_rules(all_rules_dict)


mine_rules_with_rfm_segments()
