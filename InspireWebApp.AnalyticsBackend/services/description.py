import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import seaborn as sns

from sql_client import SQLClient


def describe():
    data = SQLClient().get_describe_prerequisite_data()

    data['Sales'] = data['Quantity'] * data['UnitPrice']

    # Identify Top Customers:

    # Customer segmentation by total transaction amount:
    customer_totals = data.groupby('CustomerId')['UnitPrice'].sum().sort_values(ascending=False)
    top_customers = customer_totals.head(10)  # Top 10 customers by total spending
    print(top_customers)

    # Analyze Customer Behavior:
    # Customer behavior analysis
    customer_behavior = data.groupby('CustomerId').agg({
        'InvoiceId': 'count',  # Count of invoices per customer
        'Quantity': 'sum'  # Average transaction amount per customer
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

    # Convert InvoiceDate and InvoiceTime to datetime
    data['InvoiceDate'] = pd.to_datetime(data['InvoiceDate'])

    # Most profitable days
    daily_sales = data.groupby(data['InvoiceDate'].dt.date).agg({
        'Quantity': 'sum',  # Total sales per day
        'InvoiceId': 'count'  # Number of transactions per day
    }).reset_index()

    daily_sales.columns = ['InvoiceDate', 'AverageSales', 'TransactionCount']

    # Product Performance by Package Type:
    product_package_type_performance = data.groupby('PackageType')['Quantity'].sum().sort_values(ascending=False)
    print('Product Performance by Package Type:')
    print(product_package_type_performance)
    print()

    # Customer Performance by Category:
    customer_category_performance = data.groupby('CustomerCategory')['Quantity'].sum().sort_values(ascending=False)
    print('Customer Performance by Category:')
    print(customer_category_performance)
    print()

    # Regional Sales Analysis:
    region_sales = data.groupby('CityName')['Quantity'].sum().sort_values(ascending=False)
    print('Regional Sales Analysis:')
    print(region_sales.head())
    print()

    # Customer distribution across regions:
    customer_region_distribution = data['CityName'].value_counts()
    print('Customer distribution across regions:')
    print(customer_region_distribution.head())
    print()

    # Invoice metrics:
    invoice_metrics = data.groupby('InvoiceId').agg({
        'Quantity': 'sum',  # Total quantity per invoice
        'Quantity': 'sum'  # Average unit price per invoice
    })
    print('Invoice metrics')
    print(invoice_metrics.head())
    print()

    df = data

    # Combine InvoiceDate and InvoiceTime into a single datetime column
    df['InvoiceDateTime'] = df['InvoiceDate']

    # Display the first few rows to verify
    print(df.head())

    # Aggregate sales by day
    daily_sales = df.groupby(df['InvoiceDate'].dt.date).agg({
        'Quantity': 'sum',  # Total sales per day
        'InvoiceId': 'count'  # Number of transactions per day
    }).reset_index()

    daily_sales.columns = ['InvoiceDate', 'AverageSales', 'TransactionCount']

    daily_sales = daily_sales.sort_values(by='AverageSales', ascending=False)

    print(daily_sales.head())

    # Aggregate sales by week
    df['Week'] = df['InvoiceDate'].dt.isocalendar().week
    weekly_sales = df.groupby(df['Week']).agg({
        'Quantity': 'sum',
        'InvoiceId': 'count'
    }).reset_index()

    weekly_sales.columns = ['Week', 'AverageSales', 'TransactionCount']

    print(weekly_sales.head())

    # Aggregate sales by month
    df['Month'] = df['InvoiceDate'].dt.to_period('M')
    monthly_sales = df.groupby(df['Month']).agg({
        'Quantity': 'sum',
        'InvoiceId': 'count'
    }).reset_index()

    monthly_sales.columns = ['Month', 'AverageSales', 'TransactionCount']

    print(monthly_sales.head())
    # Convert the 'Month' column to string
    monthly_sales['Month'] = monthly_sales['Month'].dt.strftime('%Y%m')

    # Aggregate sales by quarter
    df['Quarter'] = df['InvoiceDate'].dt.to_period('Q')

    quarterly_sales = df[df['Quarter'] != '2016Q2'].groupby(df['Quarter']).agg({
        'Quantity': 'sum',
        'InvoiceId': 'count'
    }).reset_index()

    quarterly_sales.columns = ['Quarter', 'AverageSales', 'TransactionCount']

    # Convert the 'Quarter' column to the desired string format
    quarterly_sales['Quarter'] = quarterly_sales['Quarter'].dt.strftime('%YQ%q')

    print(quarterly_sales.head())

    # Aggregate sales by hour
    df['Hour'] = df['InvoiceDateTime'].dt.hour
    hourly_sales = df.groupby(df['Hour']).agg({
        'Quantity': 'mean',
        'InvoiceId': 'count'
    }).reset_index()

    hourly_sales.columns = ['Hour', 'AverageSales', 'TransactionCount']

    print(hourly_sales.head())

    # Plot hourly sales trends
    plt.figure(figsize=(12, 6))
    sns.lineplot(data=hourly_sales, x='Hour', y='AverageSales', marker='o', label='Hourly Sales')
    plt.title('Hourly Sales Analysis: Average Quantity Sold by Hour')
    plt.xlabel('Hour of the Day')
    plt.ylabel('Total Sales')
    # Rotate x-axis labels for better readability
    plt.xticks(rotation=45, ticks=range(int(hourly_sales['Hour'].min()), int(hourly_sales['Hour'].max()) + 1))
    plt.yticks(ticks=range(int(hourly_sales['AverageSales'].min() * 0.8), int(hourly_sales['AverageSales'].max()) + 1))
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
    plt.title('Monthly Sales Summary: Total Quantity Sold by Month')
    plt.xlabel('Month')
    plt.ylabel('Total Sales')
    plt.xticks(rotation=45)  # Rotate x-axis labels for better readability
    plt.legend()
    plt.show()

    # Plot quarterly sales trends
    plt.figure(figsize=(12, 6))
    sns.lineplot(data=quarterly_sales, x='Quarter', y='AverageSales', marker='o', label='Quarterly Sales')
    plt.title('Quarterly Sales Report: Total Quantity Sold per Quarter')
    plt.xlabel('Quarter')
    plt.ylabel('Total Sales')
    plt.xticks(rotation=45)  # Rotate x-axis labels for better readability
    plt.legend()
    plt.show()

    top_revenue_generating_cities = SQLClient().get_top_revenue_generating_cities()

    plt.figure(figsize=(10, 6))
    plt.bar(top_revenue_generating_cities['CityName'], top_revenue_generating_cities['Value'])
    plt.xlabel('City Name')
    plt.ylabel('Total Value')
    plt.title('Top 10 Cities by Total Invoice Value')
    plt.xticks(rotation=45)
    plt.show()

    df = SQLClient().get_sales_by_customer_category()

    plt.figure(figsize=(10, 6))

    pie_slices = plt.pie(df['Value'], labels=df['CustomerCategoryName'], autopct='%1.1f%%', startangle=140)

    # Add a legend with the customer category names
    plt.legend(pie_slices[0], df['CustomerCategoryName'], title="Customer Categories",
               loc="best", bbox_to_anchor=(1, 0, 0.5, 1))

    plt.title('Visualization of Sales by Customer Category: Total Value per Category')
    plt.show()

    df = SQLClient().get_customer_distribution_by_customer_category()

    plt.figure(figsize=(10, 6))

    pie_slices = plt.pie(df['NumberOfCustomers'], labels=df['CustomerCategoryName'], autopct='%1.1f%%', startangle=140)

    # Add a legend with the customer category names
    plt.legend(pie_slices[0], df['CustomerCategoryName'], title="Customer Categories",
               loc="best", bbox_to_anchor=(1, 0, 0.5, 1))

    plt.title('Visualization of Customer Distribution by Customer Category')
    plt.show()

    df = SQLClient().get_customer_distribution_by_customer_segment()

    plt.figure(figsize=(10, 6))

    pie_slices = plt.pie(df['NumberOfCustomers'], labels=df['SegmentName'], autopct='%1.1f%%', startangle=140)

    # Add a legend with the customer category names
    plt.legend(pie_slices[0], df['SegmentName'], title="Customer Segments",
               loc="best", bbox_to_anchor=(1, 0, 0.5, 1))

    plt.title('Visualization of Customer Distribution by Customer Segment')
    plt.show()

    top_revenue_generating_products = SQLClient().get_top_revenue_generating_products()

    plt.figure(figsize=(10, 6))
    plt.bar(top_revenue_generating_products['ProductName'], top_revenue_generating_products['Value'])
    plt.xlabel('Product Name')
    plt.ylabel('Total Value')
    plt.title('Top 5 Products by Total Invoice Value')
    plt.xticks(rotation=7)
    plt.show()

    df = SQLClient().get_sales_by_product_pack_type()

    plt.figure(figsize=(10, 6))

    # Create pie chart with adjusted distances
    pie_slices, texts, autotexts = plt.pie(
        df['Volume'],
        labels=df['ProductPackTypeName'],
        autopct='%1.1f%%',
        startangle=140,
        pctdistance=1.1,  # Position the percentage labels closer to the center
        labeldistance=1.2  # Position the labels closer to the center
    )

    # Loop over the pie slices and the corresponding percentage texts to draw the lines
    for pie_slice, autotext in zip(pie_slices, autotexts):
        # Get the start and end angle of the pie slice
        theta1, theta2 = pie_slice.theta1, pie_slice.theta2

        # Calculate the angle at the center of the pie slice
        angle = np.deg2rad((theta1 + theta2) / 2)

        # Calculate the endpoint of the line for the percentage text (outer radius)
        x_text, y_text = autotext.get_position()

        # Calculate the starting point (x, y) of the line, slightly inside the outer edge of the slice
        x = 0.9 * np.cos(angle)
        y = 0.9 * np.sin(angle)

        # Draw a line from the edge of the pie slice to the percentage text
        plt.annotate(
            '', xy=(x_text, y_text), xytext=(x, y),  # Draw line from slice to percentage text
            arrowprops=dict(arrowstyle='-')  # Use a simple line style
        )

    # Create legend labels with both product pack types and corresponding volume values
    legend_labels = [f"{row['ProductPackTypeName']} (Volume: {row['Volume']})" for _, row in df.iterrows()]
    plt.legend(pie_slices, legend_labels, title="Product Pack Types",
               loc="best", bbox_to_anchor=(1, 0, 0.5, 1))

    plt.title('Visualization of Sales by Product Pack Type: Total Value per Pack Type')
    plt.show()

    df = SQLClient().get_sales_by_product_tag()

    plt.figure(figsize=(10, 6))

    # Create pie chart with adjusted distances
    pie_slices, texts, autotexts = plt.pie(
        df['Volume'],
        labels=df['ProductTagName'],
        autopct='%1.1f%%',
        startangle=140,
        pctdistance=1.1,  # Position the percentage labels closer to the center
        labeldistance=1.2  # Position the labels closer to the center
    )

    # Loop over the pie slices and the corresponding percentage texts to draw the lines
    for pie_slice, autotext in zip(pie_slices, autotexts):
        # Get the start and end angle of the pie slice
        theta1, theta2 = pie_slice.theta1, pie_slice.theta2

        # Calculate the angle at the center of the pie slice
        angle = np.deg2rad((theta1 + theta2) / 2)

        # Calculate the endpoint of the line for the percentage text (outer radius)
        x_text, y_text = autotext.get_position()

        # Calculate the starting point (x, y) of the line, slightly inside the outer edge of the slice
        x = 0.9 * np.cos(angle)
        y = 0.9 * np.sin(angle)

        # Draw a line from the edge of the pie slice to the percentage text
        plt.annotate(
            '', xy=(x_text, y_text), xytext=(x, y),  # Draw line from slice to percentage text
            arrowprops=dict(arrowstyle='-')  # Use a simple line style
        )

    # Create legend labels with both product pack types and corresponding volume values
    legend_labels = [f"{row['ProductTagName']} (Volume: {row['Volume']})" for _, row in df.iterrows()]
    plt.legend(pie_slices, legend_labels, title="Product Tags",
               loc="best", bbox_to_anchor=(1, 0, 0.5, 1))

    plt.title('Visualization of Sales by Product Tag: Total Value per Tag')
    plt.show()

    # Observations:
    # Hourly: 11:00, 14:00, and 18:00 are the hours with the least sales on average,
    # whereas 6:00, 10:00, 12:00, 19:00, and 22:00 are the hours with the most sales.
    # The retailer achieves the most sales during 22:00.
    # Quarterly: The retail company earns most of its annual profits in the second, third, and fourth quarter.
    # The first quarter is the least profitable.
