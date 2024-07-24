import pickle

import pandas as pd
from pandas import Period
from statsmodels.tsa.arima.model import ARIMA, ARIMAResults

from sql_client import SQLClient

DEFAULT_FORECAST_STEPS = 15


def create_forecasting_model():
    data: pd.DataFrame = SQLClient().get_arima_prerequisite_data()

    data['InvoiceDate'] = pd.to_datetime(data['InvoiceDate'])

    # Extract month and year from the InvoiceDate
    data['YearMonth'] = data['InvoiceDate'].dt.to_period('M')

    # Aggregate sales by month and year
    monthly_sales = data.groupby('YearMonth')['TotalSales'].sum().reset_index()

    model = ARIMA(monthly_sales['TotalSales'], order=(6, 1, 5))
    model_fit = model.fit(method_kwargs={'maxiter':300})

    print(model_fit.summary())

    with open('predictive_modeling/arima_model_full.pkl', 'wb') as pkl_file:
        pickle.dump(model_fit, pkl_file)


def load_model() -> ARIMAResults:
    # Load the retrained ARIMA model
    with open('predictive_modeling/arima_model_full.pkl', 'rb') as pkl_file:
        return pickle.load(pkl_file)


def perform_forecast(steps=DEFAULT_FORECAST_STEPS):
    data: pd.DataFrame = SQLClient().get_arima_prerequisite_data()
    data['InvoiceDate'] = pd.to_datetime(data['InvoiceDate'])
    last_date: Period = data['InvoiceDate'].max()
    data['YearMonth'] = data['InvoiceDate'].dt.to_period('M')

    model = load_model()
    forecast_values = model.forecast(steps=steps)
    forecast_data = pd.DataFrame(forecast_values)

    forecast_data['YearMonth'] = pd.date_range(start=last_date, periods=DEFAULT_FORECAST_STEPS, freq='M').to_period('M')

    # Aggregate sales by month and year
    monthly_sales = data.groupby('YearMonth')['TotalSales'].sum().reset_index()

    # Ensure the columns align for concatenation
    forecast_data = forecast_data.rename(columns={'predicted_mean': 'TotalSales'})

    # Concatenate the existing sales data with the forecasted data
    combined_sales = pd.concat([monthly_sales, forecast_data]).reset_index(drop=True)

    combined_sales['YearMonth'] = combined_sales['YearMonth'].astype(str)
    combined_sales['TotalSales'] = combined_sales['TotalSales'].round(2).apply(lambda x: format(x, '.2f'))

    return combined_sales.to_json(orient='records')
