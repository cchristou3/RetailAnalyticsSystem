import pandas as pd

from services.utilities import to_excel
from sql_client import SQLClient

# https://www.datacamp.com/tutorial/introduction-customer-segmentation-python?dc_referrer=https%3A%2F%2Fwww.bing.com%2F
# https://www.optimove.com/resources/learning-center/rfm-segmentation
segments_descriptions_df = pd.DataFrame({
    'Segment': [
        'Best Customers', 'Loyal Customers', 'Potential Loyalists', 'Big Spenders',
        'New Customers', 'Promising', 'Need Attention', 'High Value',
        'At Risk', 'Lost Customers', 'Others'
    ],
    'Description': [
        'High-value customers who have purchased recently, frequently, and spend the most.',
        'Customers who purchase frequently and have a high spend but may not be recent.',
        'Customers who show high frequency and monetary value but have not purchased recently.',
        'High-value customers with significant spending but who purchase infrequently.',
        'Recently acquired customers who are making substantial purchases but have not yet established frequency.',
        'Customers who are somewhat active and have moderate spending, but with room for improvement.',
        'Customers who are still valuable but show signs of decreased frequency or spend.',
        'Customers with high spending but lower frequency and/or recency.',
        'Customers who have decreased in both recency and spend, showing potential signs of churn.',
        'Customers who have not purchased recently and show low frequency and spend.',
        'Customers who do not fit into the predefined categories or are not yet fully analyzed.'
    ],
    'Criteria': [
        'Recency: 5 (Most recent), Frequency: 5 (High frequency), Monetary: 5 (High spend)',
        'Recency: 4 or 5, Frequency: 4 or 5, Monetary: 4 or 5',
        'Recency: 3, Frequency: 4 or 5, Monetary: 3 or 4',
        'Recency: 2 or 3, Frequency: 2 or 3, Monetary: 4 or 5',
        'Recency: 5, Frequency: 1 or 2, Monetary: 4 or 5',
        'Recency: 4, Frequency: 2 or 3, Monetary: 2 or 3',
        'Recency: 3 or 4, Frequency: 2 or 3, Monetary: 1 or 2',
        'Recency: 2 or 3, Frequency: 2 or 3, Monetary: 3 or 4',
        'Recency: 2 or 3, Frequency: 1 or 2, Monetary: 1 or 2',
        'Recency: 1, Frequency: 1, Monetary: 1',
        'Varies based on data; typically residual category.'
    ],
    'TypicalActions': [
        'Reward with exclusive offers and loyalty programs.<br>Personalize communication.<br>Upsell and cross-sell premium products.',
        'Maintain engagement with regular updates.<br>Offer loyalty rewards.<br>Solicit feedback to improve experience.',
        'Re-engage with targeted marketing campaigns.<br>Offer incentives for repeat purchases.<br>Monitor for signs of churn.',
        'Offer special promotions to increase purchase frequency.<br>Provide personalized service.<br>Create high-value bundles.',
        'Welcome them with introductory offers.<br>Encourage frequent purchases through follow-up communications.<br>Track engagement closely.',
        'Nurture with targeted promotions.<br>Encourage higher spend with cross-sells.<br>Increase engagement efforts.',
        'Re-engage with special offers.<br>Assess and address potential issues.<br>Create retention strategies.',
        'Encourage repeat purchases with personalized offers.<br>Build a strong relationship through exclusive deals.<br>Monitor activity closely.',
        'Implement win-back strategies.<br>Offer reactivation incentives.<br>Investigate reasons for disengagement.',
        'Target with re-engagement campaigns.<br>Offer substantial discounts or offers.<br>Analyze reasons for churn to prevent future occurrences.',
        'Analyze further for insights.<br>Monitor for trends.<br>Include in general marketing campaigns.'
    ]
})

segments_descriptions = segments_descriptions_df.to_json(orient='records')

print(segments_descriptions)

to_excel(segments_descriptions_df, 'Segments.xlsx')


def find_most_impactful_customers(rfm_df):
    # Calculate Total Spending by quantile
    quantile_summary = rfm_df.groupby('M')['Monetary'].sum()

    # Calculate Proportion of Total Spending
    total_spending = rfm_df['Monetary'].sum()
    quantile_percentage = quantile_summary / total_spending * 100

    # Identify and Print Top Spending Quantiles
    # Sort quantiles by spending in descending order
    sorted_quantiles = quantile_percentage.sort_values(ascending=False)
    print("\nTop Spending Quantiles:")
    print(sorted_quantiles)

    # Check Contribution of Top Quantiles
    # Define the number of top quantiles you are interested in
    # For example, top 4 quantiles
    top_quantiles = sorted_quantiles.head(4)  # Adjust the number of top quantiles as needed

    # Calculate Total Contribution of Top Quantiles
    top_quantiles_contribution = top_quantiles.sum()

    print("\nTop Spending Quantiles Contribution to Total Revenue (%):")
    print(top_quantiles_contribution)

    # Check if top spending quantiles contribute around 80% of the total revenue
    if top_quantiles_contribution >= 80:
        print("\nThe top spending quantiles contribute approximately 80% of the total revenue.")
    else:
        print("\nThe top spending quantiles do not contribute approximately 80% of the total revenue.")


def r_score(x, quantiles):
    if x <= quantiles[0.25]:
        return 4
    elif x <= quantiles[0.50]:
        return 3
    elif x <= quantiles[0.75]:
        return 2
    else:
        return 1


def fm_score(x, quantiles):
    if x <= quantiles[0.25]:
        return 1
    elif x <= quantiles[0.50]:
        return 2
    elif x <= quantiles[0.75]:
        return 3
    else:
        return 4


def segment_customer(df):
    rfm_score = df['RFM_Score']
    r = int(rfm_score[0])
    f = int(rfm_score[1])
    m = int(rfm_score[2])

    if r == 4 and f == 4 and m == 4:
        return 'Best Customers'
    elif r >= 3 and f >= 3 and m >= 3:
        return 'Loyal Customers'
    elif r >= 2 and f >= 3 and m >= 2:
        return 'Potential Loyalists'
    elif r >= 3 and f <= 2 and m >= 3:
        return 'Big Spenders'
    elif r <= 2 and f >= 3 and m >= 3:
        return 'New Customers'
    elif r <= 2 and f >= 2 and m <= 2:
        return 'Promising'
    elif r >= 3 and f >= 2 and m <= 2:
        return 'Need Attention'
    elif r >= 2 and f <= 2 and m >= 3:
        return 'High Value'
    elif r <= 2 and f <= 2 and m <= 2:
        return 'At Risk'
    elif r == 1 and f == 1 and m == 1:
        return 'Lost Customers'
    else:
        return 'Others'


def generate_rfm_scores(sql_client: SQLClient) -> pd.DataFrame:
    df = sql_client.get_rfm_prerequisite_data()

    df['InvoiceDate'] = pd.to_datetime(df['InvoiceDate'])

    # Calculate the reference date (usually the most recent date in the dataset)
    reference_date = df['InvoiceDate'].max() + pd.DateOffset(1)

    # Calculate Recency, Frequency, and Monetary values
    rfm_df = df.groupby('CustomerId').agg({
        'InvoiceDate': lambda x: (reference_date - x.max()).days,
        'InvoiceId': 'nunique',
        'TotalPrice': 'sum'
    }).reset_index()

    # Rename columns to RFM
    rfm_df.columns = ['CustomerId', 'Recency', 'Frequency', 'Monetary']

    # Calculate quantiles
    r_quantiles = rfm_df['Recency'].quantile([0.25, 0.50, 0.75]).to_dict()
    f_quantiles = rfm_df['Frequency'].quantile([0.25, 0.50, 0.75]).to_dict()
    m_quantiles = rfm_df['Monetary'].quantile([0.25, 0.50, 0.75]).to_dict()

    # Apply the scoring
    rfm_df['R'] = rfm_df['Recency'].apply(r_score, args=(r_quantiles,))
    rfm_df['F'] = rfm_df['Frequency'].apply(fm_score, args=(f_quantiles,))
    rfm_df['M'] = rfm_df['Monetary'].apply(fm_score, args=(m_quantiles,))

    # Calculate the RFM Score
    rfm_df['RFM_Score'] = rfm_df['R'].map(str) + rfm_df['F'].map(str) + rfm_df['M'].map(str)
    return rfm_df


def map_to_segments(rfm_df: pd.DataFrame) -> pd.DataFrame:
    rfm_df['Segment'] = rfm_df.apply(segment_customer, axis=1)
    return rfm_df


def store_rfm_scores_to_database(rfm_df: pd.DataFrame, sql_client: SQLClient):
    rfm_df = rfm_df.rename(columns={'RFM_Score': 'RfmScore'})

    # Prepare the updates for the Customers table
    updates = rfm_df[['CustomerId', 'RfmScore', 'Segment']].to_dict(orient='records')
    sql_client.update_customers(updates)


def segment_customers():
    client = SQLClient()

    rfm_df = generate_rfm_scores(client)

    rfm_df = map_to_segments(rfm_df)

    print(rfm_df['Segment'].value_counts())
    print(rfm_df['RFM_Score'].value_counts())

    store_rfm_scores_to_database(rfm_df, client)
