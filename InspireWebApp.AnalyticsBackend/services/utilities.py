import pandas as pd


def to_excel(data, file_name):
    # Convert stats to a DataFrame
    stats_df = pd.DataFrame(data)

    # Write stats to an Excel file
    stats_df.to_excel(file_name, index=False)
