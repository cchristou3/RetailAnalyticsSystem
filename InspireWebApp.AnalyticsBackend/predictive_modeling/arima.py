import pandas as pd

from sklearn.model_selection import train_test_split
from statsmodels.tsa.arima.model import ARIMA
from sklearn.metrics import mean_squared_error, mean_absolute_error
import matplotlib.pyplot as plt


def run():
    # TODO: Revisit when doing predictive modeling

    # Load the CSV file
    file_path: str = 'Sales Data Online Shop.csv'
    data: pd.DataFrame = pd.read_csv(file_path, low_memory=False)

    # Data Preprocessing
    data['InvoiceDate'] = pd.to_datetime(data['InvoiceDate'])
    data['Date'] = data['InvoiceDate'].dt.date
    data['Date'] = pd.to_datetime(data['Date'])

    # latest_year = data['Date'].max().year
    # data = data[data['Date'].dt.year == latest_year]

    data.sort_values(by='Date', inplace=True)

    # Feature Engineering
    data['DayOfWeek'] = data['Date'].dt.dayofweek
    data['Month'] = data['Date'].dt.month
    data['Year'] = data['Date'].dt.year

    # Aggregate data to daily sales
    daily_sales = data.groupby('Date')['Quantity'].sum().reset_index()

    # Train-test split
    train, test = train_test_split(daily_sales, test_size=0.2, shuffle=False)

    # Model Selection and Training - ARIMA
    train_sales = train.set_index('Date')['Quantity']
    test_sales = test.set_index('Date')['Quantity']

    model = ARIMA(train_sales, order=(5, 1, 5))
    model_fit = model.fit()

    # Forecasting
    forecast = model_fit.forecast(steps=len(test_sales))

    # Evaluation
    mse = mean_squared_error(test_sales, forecast)
    mae = mean_absolute_error(test_sales, forecast)
    print(f'MSE: {mse}, MAE: {mae}')

    # Plotting the results
    plt.figure(figsize=(10, 5))
    plt.plot(train_sales, label='Train')
    plt.plot(test_sales.index, test_sales, label='Test')
    plt.plot(test_sales.index, forecast, label='Forecast')
    plt.legend()
    plt.show()
