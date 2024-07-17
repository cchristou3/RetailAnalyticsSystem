from datetime import datetime

import pandas as pd
from pandas import DataFrame

from services.graph_helper import GraphHelper
from services.helper import Helper


def describe(data: DataFrame):

    data['Sales'] = data['Quantity'] * data['UnitPrice']

    # Identify Top Customers:

    # Customer segmentation by total transaction amount:
    customer_totals = data.groupby('CustomerId')['UnitPrice'].sum().sort_values(ascending=False)
    top_customers = customer_totals.head(10)  # Top 10 customers by total spending
    print(top_customers)

    # Analyze Customer Behavior:
    # Customer behavior analysis
    customer_behavior = data.groupby('CustomerId').agg({
        'InvoiceID': 'count',            # Count of invoices per customer
        'UnitPrice': 'mean'              # Average transaction amount per customer
    })
    print(customer_behavior.head())

    # Identify Top-Selling Products:
    # Product analysis
    top_products = data.groupby('ProductName')['Quantity'].sum().sort_values(ascending=False).head(10)
    print('Top-Selling Products:')
    print(top_products)
    print()

    top_products = data.groupby('ProductName')['Sales'].sum().sort_values(ascending=False).head(10)
    print('Top-Profitable Products:')
    print(top_products)
    print()

    # Most profitable days
    daily_sales = data.groupby(data['InvoiceDate'].dt.date).agg({
        'UnitPrice': 'mean',  # Total sales per day
        'InvoiceID': 'count'  # Number of transactions per day
    }).reset_index()

    daily_sales.columns = ['InvoiceDate', 'AverageSales', 'TransactionCount']

    # Product Performance by Package Type:
    product_package_type_performance = data.groupby('PackageType')['UnitPrice'].sum().sort_values(ascending=False)
    print('Product Performance by Package Type:')
    print(product_package_type_performance)
    print()

    # Customer Performance by Category:
    customer_category_performance = data.groupby('CustomerCategory')['UnitPrice'].sum().sort_values(ascending=False)
    print('Customer Performance by Category:')
    print(customer_category_performance)
    print()

    # Regional Sales Analysis:
    region_sales = data.groupby('CityName')['UnitPrice'].sum().sort_values(ascending=False)
    print('Regional Sales Analysis:')
    print(region_sales.head())
    print()

    # Customer distribution across regions:
    customer_region_distribution = data['CityName'].value_counts()
    print('Customer distribution across regions:')
    print(customer_region_distribution.head())
    print()

    # Invoice metrics:
    invoice_metrics = data.groupby('InvoiceID').agg({
        'Quantity': 'sum',  # Total quantity per invoice
        'UnitPrice': 'mean'  # Average unit price per invoice
    })
    print('Invoice metrics')
    print(invoice_metrics.head())
    print()

    df = data
    # Convert InvoiceDate and InvoiceTime to datetime
    df['InvoiceDate'] = pd.to_datetime(df['InvoiceDate'])
    df['InvoiceTime'] = pd.to_datetime(df['InvoiceTime'], format='%H:%M:%S').dt.time

    # Combine InvoiceDate and InvoiceTime into a single datetime column
    df['InvoiceDateTime'] = df.apply(lambda row: datetime.combine(row['InvoiceDate'], row['InvoiceTime']), axis=1)

    # Display the first few rows to verify
    print(df.head())

    # Aggregate sales by day
    daily_sales = df.groupby(df['InvoiceDate'].dt.date).agg({
        'UnitPrice': 'mean',  # Total sales per day
        'InvoiceID': 'count'  # Number of transactions per day
    }).reset_index()

    daily_sales.columns = ['InvoiceDate', 'AverageSales', 'TransactionCount']

    daily_sales = daily_sales.sort_values(by='AverageSales', ascending=False)

    print(daily_sales.head())

    # Aggregate sales by week
    df['Week'] = df['InvoiceDate'].dt.isocalendar().week
    weekly_sales = df.groupby(df['Week']).agg({
        'UnitPrice': 'mean',
        'InvoiceID': 'count'
    }).reset_index()

    weekly_sales.columns = ['Week', 'AverageSales', 'TransactionCount']

    print(weekly_sales.head())

    # Aggregate sales by month
    df['Month'] = df['InvoiceDate'].dt.to_period('M')
    monthly_sales = df.groupby(df['Month']).agg({
        'UnitPrice': 'mean',
        'InvoiceID': 'count'
    }).reset_index()

    monthly_sales.columns = ['Month', 'AverageSales', 'TransactionCount']

    print(monthly_sales.head())
    # Convert the 'Month' column to string
    monthly_sales['Month'] = monthly_sales['Month'].dt.strftime('%Y%m')

    # Aggregate sales by quarter
    df['Quarter'] = df['InvoiceDate'].dt.to_period('Q')

    quarterly_sales = df[df['Quarter'] != '2016Q2'].groupby(df['Quarter']).agg({
        'UnitPrice': 'mean',
        'InvoiceID': 'count'
    }).reset_index()

    quarterly_sales.columns = ['Quarter', 'AverageSales', 'TransactionCount']

    # Convert the 'Quarter' column to the desired string format
    quarterly_sales['Quarter'] = quarterly_sales['Quarter'].dt.strftime('%YQ%q')

    print(quarterly_sales.head())

    # Aggregate sales by hour
    df['Hour'] = df['InvoiceDateTime'].dt.hour
    hourly_sales = df.groupby(df['Hour']).agg({
        'UnitPrice': 'mean',
        'InvoiceID': 'count'
    }).reset_index()

    hourly_sales.columns = ['Hour', 'AverageSales', 'TransactionCount']

    print(hourly_sales.head())

    import matplotlib.pyplot as plt
    import seaborn as sns

    # Plot hourly sales trends
    plt.figure(figsize=(12, 6))
    sns.lineplot(data=hourly_sales, x='Hour', y='AverageSales', marker='o', label='Hourly Sales')
    plt.title('Hourly Sales Trends')
    plt.xlabel('Hour of the Day')
    plt.ylabel('Total Sales')
    # Rotate x-axis labels for better readability
    plt.xticks(rotation=45, ticks=range(int(hourly_sales['Hour'].min()), int(hourly_sales['Hour'].max()) + 1))
    plt.legend()
    plt.show()

    # Plot daily sales trends
    plt.figure(figsize=(12, 6))
    sns.lineplot(data=daily_sales, x='InvoiceDate', y='AverageSales', marker='o', label='Daily Sales')
    plt.title('Daily Sales Trends')
    plt.xlabel('Date')
    plt.ylabel('Total Sales')
    plt.xticks(rotation=45)  # Rotate x-axis labels for better readability
    plt.legend()
    plt.show()

    # Plot weekly sales trends
    plt.figure(figsize=(12, 6))
    sns.lineplot(data=weekly_sales, x='Week', y='AverageSales', marker='o', label='Weekly Sales')
    plt.title('Weekly Sales Trends')
    plt.xlabel('Week')
    plt.ylabel('Total Sales')
    plt.xticks(rotation=45)  # Rotate x-axis labels for better readability
    plt.legend()
    plt.show()

    # Plot monthly sales trends
    plt.figure(figsize=(12, 6))
    sns.lineplot(data=monthly_sales, x='Month', y='AverageSales', marker='o', label='Monthly Sales')
    plt.title('Monthly Sales Trends')
    plt.xlabel('Month')
    plt.ylabel('Total Sales')
    plt.xticks(rotation=45)  # Rotate x-axis labels for better readability
    plt.legend()
    plt.show()

    # Plot quarterly sales trends
    plt.figure(figsize=(12, 6))
    sns.lineplot(data=quarterly_sales, x='Quarter', y='AverageSales', marker='o', label='Quarterly Sales')
    plt.title('Quarterly Sales Trends')
    plt.xlabel('Quarter')
    plt.ylabel('Total Sales')
    plt.xticks(rotation=45)  # Rotate x-axis labels for better readability
    plt.legend()
    plt.show()

    # Observations:
    # Hourly: 11:00, 14:00, and 18:00 are the hours with the least sales on average,
    # whereas 6:00, 10:00, 12:00, 19:00, and 22:00 are the hours with the most sales.
    # The retailer achieves the most sales during 22:00.
    # Quarterly: The retail company earns most of its annual profits in the second, third, and fourth quarter.
    # The first quarter is the least profitable.

