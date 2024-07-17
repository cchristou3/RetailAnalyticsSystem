from datetime import datetime

import pandas as pd
from pandas import DataFrame

from predictive_modeling.association_rule_mining import mine_rules
from services.description import describe
from services.exploration import explore

if __name__ == '__main__':

    # TODO: Revisit when doing predictive modeling

    # Load the CSV file
    file_path: str = 'Sales Data Online Shop.csv'
    data: DataFrame = pd.read_csv(file_path, low_memory=False)

    # explore(data, ignore_visalizations=False)
    # describe(data)
    mine_rules(data)
