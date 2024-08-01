import ast
from typing import List

import pandas as pd
from matplotlib import pyplot as plt
from pandas import DataFrame
from pandas.core.dtypes.common import is_numeric_dtype
import seaborn as sns

from services.graph_helper import GraphHelper
from services.helper import Helper
from services.utilities import to_excel

import scipy.stats as stats


def explore(data: DataFrame):

    helper = Helper()

    # Display the first few rows of the dataset to get an idea of its structure
    print(data.head())

    columns: List[str] = [
        "InvoiceID", "InvoiceDate", "InvoiceTime", "IsCreditNote", "CustomerId", "CustomerCategory",
        "CityName", "EmployeeId", "Quantity", "UnitPrice", "ProductName", "Weight", "Tags", "PackageType"
    ]

    for column in columns:
        print(f"====================================\n{column}\n")
        print(data[column].value_counts())

    # Check the data types of the numeric columns
    data_types = data[columns].dtypes
    print(data_types)

    # Discard identifiers
    data = data.drop("InvoiceID", axis=1)
    data = data.drop("CustomerId", axis=1)
    data = data.drop("EmployeeId", axis=1)

    numeric_columns: List[str] = [col for col in data.columns if is_numeric_dtype(data[col])]

    column_stats = [helper.print_column_stats(data, col) for col in numeric_columns]

    # Output results in an Excel file
    to_excel(column_stats, "column_stats.xlsx")

    # Column IsCreditNote has only 0s, it does not provide any information, so we discard it
    data = data.drop('IsCreditNote', axis=1)
    numeric_columns.remove('IsCreditNote')

    # Tags Analysis
    print()
    print("*********************************************")

    # Convert the 'Tags' column from string representation to a list of tags
    data['Tags'] = data['Tags'].apply(lambda x: ast.literal_eval(x))

    # Extract all unique tags
    unique_tags = set(tag for tags_list in data['Tags'] for tag in tags_list)

    # Create binary columns for each unique tag
    for tag in unique_tags:
        data[tag] = data['Tags'].apply(lambda tags_list: 1 if tag in tags_list else 0)

    for tag in unique_tags:
        data[f'{tag} (tag)'] = data[tag]
        del data[tag]

    unique_tags = [f'{tag} (tag)' for tag in unique_tags]

    # Now data contains binary columns for each unique tag
    # You can use these columns for further analysis or modeling

    # Example usage:
    # Summarize count of occurrences of each tag
    tag_counts = data[list(unique_tags)].sum()

    # Print out the counts of each tag
    print(tag_counts)

    # Perform Similar Descriptive Analytics
    # graph_helper.plot_boxplot(data, 'diagnosis', 'radius_mean')
    # graph_helper.plot_boxplot(data, 'diagnosis', 'texture_mean')
    # graph_helper.plot_boxplot(data, 'diagnosis', 'perimeter_mean')
    # graph_helper.plot_boxplot(data, 'diagnosis', 'area_mean')
    #
    helper.visualize_outliers(data, numeric_columns)
    #
    # helper.print_correlations(data[numeric_columns])
    # helper.visualize_correlations(data[numeric_columns])

    graph_helper = GraphHelper()
    # graph_helper.plot_histograms(data, numeric_columns)

    sns.set(style="whitegrid")
    # Create a figure with one row and three columns
    fig, axes = plt.subplots(1, 3, figsize=(15, 5))

    # Plot the histogram for the 'Weight' column
    axes[0].hist(data['Weight'], bins=10, color='blue', edgecolor='black')
    axes[0].set_title('Weight Distribution')
    axes[0].set_xlabel('Weight')
    axes[0].set_ylabel('Frequency')

    # Plot the histogram for the 'UnitPrice' column
    axes[1].hist(data['UnitPrice'], bins=50, color='green', edgecolor='black')
    axes[1].set_title('UnitPrice Distribution')
    axes[1].set_xlabel('UnitPrice')
    axes[1].set_ylabel('Frequency')

    # Plot the histogram for the 'Quantity' column
    axes[2].hist(data['Quantity'], bins=50, color='red', edgecolor='black')
    axes[2].set_title('Quantity Distribution')
    axes[2].set_xlabel('Quantity')
    axes[2].set_ylabel('Frequency')

    # Adjust layout to prevent overlap
    plt.tight_layout()

    # Show the plot
    plt.show()

    p_value_cut_off = 0.05

    categorical_columns = [column for column in data.columns if
                           column not in numeric_columns and column not in ["InvoiceDate", "InvoiceTime", "ProductName",
                                                                            "CityName", "Tags"]]

    correlation_stats = []
    for categorical_column in categorical_columns:
        for numeric_column in numeric_columns:

            samples = []
            for value in data[categorical_column].unique():
                samples.append(data[data[categorical_column] == value][numeric_column])

            kruskal_result = stats.kruskal(
                *samples
            )

            if kruskal_result.pvalue < p_value_cut_off:
                # 1) A higher H-statistic indicates a greater difference between the groups' distributions. It measures the degree to which the groups differ.
                # 2) p-value < 0.05: There is sufficient evidence to suggest that at least one group median is different from the others.
                # p-value >= 0.05: There is not sufficient evidence to suggest a significant difference between the groups.
                print(
                    f'{categorical_column} <-> {numeric_column} | Kruskal-Wallis H-statistic: {kruskal_result.statistic}, p-value: {kruskal_result.pvalue}')

                correlation_stats.append({
                    'Categorical Feature': categorical_column,
                    'Numeric Feature': numeric_column,
                    'H-statistic': kruskal_result.statistic,
                    'P-value': kruskal_result.pvalue
                })
                # plt.figure(figsize=(10, 6))
                # sns.boxplot(x=categorical_column, y=numeric_column, data=data)
                # plt.title(f'Violin Plot of {numeric_column} by {categorical_column}')
                # plt.show()

    # Output correlation_stats in an Excel file
    to_excel(correlation_stats, "correlation_stats.xlsx")
