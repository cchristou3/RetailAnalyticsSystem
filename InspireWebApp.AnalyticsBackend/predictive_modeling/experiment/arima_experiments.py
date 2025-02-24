import matplotlib.pyplot as plt
import pandas as pd
import statsmodels.api as sm
from sklearn.metrics import mean_absolute_error
from sklearn.metrics import mean_squared_error
from sklearn.model_selection import ParameterGrid
from sklearn.model_selection import train_test_split
from statsmodels.tsa.arima.model import ARIMA
from statsmodels.tsa.holtwinters import ExponentialSmoothing
from statsmodels.tsa.stattools import adfuller

from predictive_modeling.constants import DATA_FILE_PATH


def daily_sales_sarimax_forecast():
    # Load the CSV file
    file_path: str = DATA_FILE_PATH
    data: pd.DataFrame = pd.read_csv(file_path, low_memory=False)

    df = pd.DataFrame(data)
    df['InvoiceDate'] = pd.to_datetime(df['InvoiceDate'])
    df.set_index('InvoiceDate', inplace=True)
    df = df.resample('D').sum().fillna(0)  # Resample to daily frequency

    # Train-test split
    train, test = train_test_split(df, test_size=0.2, shuffle=False)

    # Fit a time series model
    model = sm.tsa.statespace.SARIMAX(df['Quantity'],
                                      order=(1, 1, 1),
                                      seasonal_order=(1, 1, 1, 7))  # Adjust the order as needed
    results = model.fit()

    # Forecast the next 10 days
    forecast = results.get_forecast(steps=10)
    forecast_index = pd.date_range(start=df.index[-1] + pd.Timedelta(days=1), periods=10, freq='D')
    forecast_series = pd.Series(forecast.predicted_mean, index=forecast_index)

    # Evaluation
    mse = mean_squared_error(test, forecast_series)
    mae = mean_absolute_error(test, forecast)
    print(f'MSE: {mse}, MAE: {mae}')


def demand_and_sales_forecast_exponential_smoothing():
    # Load the CSV file
    file_path: str = DATA_FILE_PATH
    data: pd.DataFrame = pd.read_csv(file_path, low_memory=False)

    df = pd.DataFrame(data)
    df['InvoiceDate'] = pd.to_datetime(df['InvoiceDate'])
    df.set_index('InvoiceDate', inplace=True)

    # Calculate total sales
    df['TotalSales'] = df['Quantity'] * df['UnitPrice']

    # Resample the data to weekly frequency for demand
    weekly_demand = df['Quantity'].resample('W').sum()

    # Fit the Holt-Winters model with weekly seasonality
    weekly_model = ExponentialSmoothing(weekly_demand, seasonal='add', seasonal_periods=52)
    weekly_fit = weekly_model.fit()

    # Forecast future demand
    weekly_forecast_periods = 12  # Forecasting for the next 12 weeks
    weekly_forecast = weekly_fit.forecast(weekly_forecast_periods)

    # Plot the results
    plt.figure(figsize=(12, 6))
    plt.plot(weekly_demand.index, weekly_demand, label='Historical Demand')
    plt.plot(weekly_forecast.index, weekly_forecast, label='Forecasted Demand', color='red')
    plt.xlabel('Date')
    plt.ylabel('Quantity')
    plt.title('Weekly Demand Forecasting')
    plt.legend()
    plt.show()

    # Resample the data to monthly frequency for total sales
    monthly_sales = df['TotalSales'].resample('M').sum()

    # Fit the Holt-Winters model with annual seasonality
    annual_sales_model = ExponentialSmoothing(monthly_sales, seasonal='add', seasonal_periods=12)
    annual_sales_fit = annual_sales_model.fit()

    # Forecast future sales
    annual_sales_forecast = annual_sales_fit.forecast(5)

    # Plot the results
    plt.figure(figsize=(12, 6))
    plt.plot(monthly_sales.index, monthly_sales, label='Historical Sales')
    plt.plot(annual_sales_forecast.index, annual_sales_forecast, label='Forecasted Sales', color='red')
    plt.xlabel('Date')
    plt.ylabel('Sales Revenue')
    plt.title('Annual Sales Forecasting')
    plt.legend()
    plt.show()


def daily_sales_forecast_sarimax_example():
    # Step 1: Collecting Data

    # Load the CSV file
    file_path: str = DATA_FILE_PATH
    data: pd.DataFrame = pd.read_csv(file_path, low_memory=False)

    # Display the first few rows of the dataframe
    print(data.head())

    # Step 2: Data Pre-Processing

    # Combine InvoiceDate and InvoiceTime into a single datetime column
    data['InvoiceDatetime'] = pd.to_datetime(data['InvoiceDate'] + ' ' + data['InvoiceTime'])

    # Aggregate the sales data on a daily basis
    data['TotalSales'] = data['Quantity'] * data['UnitPrice']
    daily_sales = data.resample('D', on='InvoiceDatetime')['TotalSales'].sum().reset_index()

    # Check for missing values
    daily_sales.isnull().sum()

    # Handle missing values by forward filling
    daily_sales['TotalSales'].fillna(method='ffill', inplace=True)

    # Display the pre-processed data
    print(daily_sales.head())

    # Step 3: Data Analysis

    # Plot the daily sales
    plt.figure(figsize=(12, 6))
    plt.plot(daily_sales['InvoiceDatetime'], daily_sales['TotalSales'])
    plt.title('Daily Sales Over Time')
    plt.xlabel('Date')
    plt.ylabel('Total Sales')
    plt.show()

    # Check for stationarity using the Augmented Dickey-Fuller test

    result = adfuller(daily_sales['TotalSales'])
    print('ADF Statistic:', result[0])
    print('p-value:', result[1])

    # Step 4: Ensure Stationarity
    # Apply differencing if data is not stationary
    if result[1] > 0.05:
        daily_sales['TotalSales'] = daily_sales['TotalSales'].diff().dropna()

    # Recheck for stationarity after differencing
    result = adfuller(daily_sales['TotalSales'].dropna())
    print('ADF Statistic after differencing:', result[0])
    print('p-value after differencing:', result[1])

    # Step 5: Split Data into Training and Test Sets
    # Define the split point, let's say we use the last 20% of data for testing
    split_point = int(len(daily_sales) * 0.8)
    train, test = daily_sales[:split_point], daily_sales[split_point:]

    # Step 6: Model Selection and Training

    # Define the ARIMA model for training data
    model = ARIMA(train['TotalSales'], order=(1, 1, 1), enforce_stationarity=False, enforce_invertibility=False)

    # Fit the model
    results = model.fit()

    # Display model summary
    print(results.summary())

    # Step 7: Model Evaluation

    # Make predictions on the test set
    test['Forecast'] = results.predict(start=len(train), end=len(train) + len(test) - 1, dynamic=False)

    # Plot the results
    recent_days = 30
    plt.figure(figsize=(12, 6))
    plt.plot(train['InvoiceDatetime'].iloc[-recent_days:], train['TotalSales'].iloc[-recent_days:], label='Train Sales')
    plt.plot(test['InvoiceDatetime'], test['TotalSales'], label='Actual Sales')
    plt.plot(test['InvoiceDatetime'], test['Forecast'], label='Forecasted Sales', color='red')
    plt.title('Actual vs Forecasted Sales (Recent Days)')
    plt.xlabel('Date')
    plt.ylabel('Total Sales')
    plt.legend()
    plt.show()

    # Calculate evaluation metrics
    mse = mean_squared_error(test['TotalSales'], test['Forecast'])
    mae = mean_absolute_error(test['TotalSales'], test['Forecast'])
    rmse = mean_squared_error(test['TotalSales'], test['Forecast'], squared=False)

    print(f'MSE: {mse}')
    print(f'MAE: {mae}')
    print(f'RMSE: {rmse}')

    # Step 8: Model Parameters Tuning

    # Define a more extensive parameter grid
    param_grid = {
        'order': [(p, d, q) for p in range(0, 4) for d in range(0, 2) for q in range(0, 4)]
    }

    # Custom function to fit and score an ARIMA model
    def fit_arima(params, train, test):
        order = params['order']
        model = ARIMA(train['TotalSales'], order=order, enforce_stationarity=False, enforce_invertibility=False)
        results = model.fit(low_memory=False)
        forecast = results.predict(start=len(train), end=len(train) + len(test) - 1, dynamic=False)
        mse = mean_squared_error(test['TotalSales'], forecast)
        return mse

    # Perform grid search
    best_score, best_params = float('inf'), None
    for params in ParameterGrid(param_grid):
        score = fit_arima(params, train, test)
        if score < best_score:
            best_score, best_params = score, params

    print('Best Score:', best_score)
    print('Best Params:', best_params)


def compare_arima_and_sarima_sales_forecasting():
    # Step 1: Collecting Data

    # Load the CSV file
    file_path: str = DATA_FILE_PATH
    data: pd.DataFrame = pd.read_csv(file_path, low_memory=False)

    # Display the first few rows of the dataframe
    print(data.head())

    # Combine InvoiceDate and InvoiceTime into a single datetime column
    data['InvoiceDatetime'] = pd.to_datetime(data['InvoiceDate'] + ' ' + data['InvoiceTime'])

    data.sort_values(by='InvoiceDatetime', inplace=True)

    # Aggregate the sales data on a daily basis
    data['TotalSales'] = data['Quantity'] * data['UnitPrice']
    daily_sales = data.resample('D', on='InvoiceDatetime')['TotalSales'].sum().reset_index()

    # Handle missing values by forward filling
    daily_sales['TotalSales'].fillna(method='ffill', inplace=True)

    # Display the pre-processed data
    print(daily_sales.head())

    # Plot the daily sales
    plt.figure(figsize=(12, 6))
    plt.plot(daily_sales['InvoiceDatetime'], daily_sales['TotalSales'])
    plt.title('Daily Sales Over Time')
    plt.xlabel('Date')
    plt.ylabel('Total Sales')
    plt.show()

    # Check for stationarity using the Augmented Dickey-Fuller test
    result = adfuller(daily_sales['TotalSales'])
    print('ADF Statistic:', result[0])
    print('p-value:', result[1])

    # Since the p-value is less than 0.05, we reject the null hypothesis, indicating that the data is stationary.
    # Given the stationarity of the data, we can proceed with the ARIMA model. If we had detected seasonality,
    # we would have considered SARIMA. Now, let's split the data into training and testing sets and
    # then implement the ARIMA model.

    # Split the data into training and testing sets
    train_size = int(len(daily_sales) * 0.8)
    train, test = daily_sales[:train_size], daily_sales[train_size:]

    # Define a more extensive parameter grid for ARIMA
    param_grid_arima = {
        'order': [(p, d, q) for p in range(0, 4) for d in range(0, 2) for q in range(0, 4)]
    }

    # Custom function to fit and score an ARIMA model
    def fit_arima(params, train, test):
        order = params['order']
        model = ARIMA(train['TotalSales'], order=order, enforce_stationarity=False, enforce_invertibility=False)
        results = model.fit()
        forecast = results.predict(start=len(train), end=len(train) + len(test) - 1, dynamic=False)
        mse = mean_squared_error(test['TotalSales'], forecast)
        return mse

    # Perform grid search for ARIMA
    best_score_arima, best_params_arima = float('inf'), None
    for params in ParameterGrid(param_grid_arima):
        score = fit_arima(params, train, test)
        if score < best_score_arima:
            best_score_arima, best_params_arima = score, params

    print('Best ARIMA Score:', best_score_arima)
    print('Best ARIMA Params:', best_params_arima)
