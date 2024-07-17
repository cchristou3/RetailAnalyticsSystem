from typing import List

import pandas as pd
from mlxtend.frequent_patterns import fpmax, association_rules
from mlxtend.preprocessing import TransactionEncoder


# TODO: Using this article https://dataaspirant.com/association-rule-analysis/#:~:text=Before%20performing%20association%20rule%20analysis%2C%20it%20is%20necessary,or%20irrelevant%20data%20Handling%20missing%20or%20incomplete%20data
#  Implement FPM, and then specialize it to use the data of a specified group.
#  E.g. Give me the ARs of the most frequent clients (use RFM model)


def mine_rules(data: pd.DataFrame):

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
                        print(f'EXCEPTION: Perform Frequent Mining on: {len(filtered_data)} records ({invoice_year}, {customer_category}, {package_type})')
                        raise
