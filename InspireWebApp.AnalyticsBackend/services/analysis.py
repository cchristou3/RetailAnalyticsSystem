import pandas as pd
from pandas import DataFrame

from predictive_modeling.constants import DATA_FILE_PATH
from services.description import describe
from services.exploration import explore

"""
    The file was used to perform the initial data exploratory analysis 
    as well as generating descriptive analytics
"""
if __name__ == '__main__':
    # Define the file path to the CSV file containing sales data
    file_path: str = DATA_FILE_PATH

    # Load the CSV file into a pandas DataFrame
    # Setting low_memory=False to prevent dtype guessing and reduce memory usage
    data: DataFrame = pd.read_csv(file_path, low_memory=False)

    # Perform exploratory data analysis on the loaded DataFrame
    explore(data)

    # Generate descriptive statistics for the loaded DataFrame
    describe()
