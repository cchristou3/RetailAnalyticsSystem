import csv
import json

import pandas as pd


def to_excel(data, file_name):
    # Convert stats to a DataFrame
    stats_df = pd.DataFrame(data)

    # Write stats to an Excel file
    stats_df.to_excel(file_name, index=False)


def csv_to_json(csv_file_path):
    # List to hold dictionaries representing each row in the CSV
    data = []

    # Open and read the CSV file
    with open(csv_file_path, mode='r', newline='', encoding='utf-8') as csv_file:
        csv_reader = csv.DictReader(csv_file)
        for row in csv_reader:
            data.append(row)

    # Convert list of dictionaries to a JSON string
    json_string = json.dumps(data, indent=4)

    return json_string
