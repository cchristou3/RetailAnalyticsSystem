from matplotlib import pyplot as plt


def exponential_smoothing():
    import pandas as pd

    # Load the data into a DataFrame
    file_path: str = '../../Sales Data Online Shop.csv'
    data: pd.DataFrame = pd.read_csv(file_path, low_memory=False)

    # Combine InvoiceDate and InvoiceTime into a single datetime column
    data['InvoiceDatetime'] = pd.to_datetime(data['InvoiceDate'] + ' ' + data['InvoiceTime'])

    # Aggregate the sales data on a daily basis
    data['TotalSales'] = data['Quantity'] * data['UnitPrice']
    daily_sales = data.resample('D', on='InvoiceDatetime')['TotalSales'].sum().reset_index()

    # Handle missing values by forward filling
    daily_sales['TotalSales'].fillna(method='ffill', inplace=True)

    # Display the pre-processed data
    print(daily_sales.head())

    # Split the data into training and testing sets
    train_size = int(len(daily_sales) * 0.8)
    train, test = daily_sales[:train_size], daily_sales[train_size:]

    train.shape, test.shape

    from statsmodels.tsa.holtwinters import ExponentialSmoothing
    from sklearn.metrics import mean_squared_error
    import numpy as np

    # Define a parameter grid for Exponential Smoothing
    param_grid_es = {
        'trend': [None, 'add', 'mul'],
        'seasonal': [None, 'add', 'mul'],
        'seasonal_periods': [7, 30, 365]
    }

    # Custom function to fit and score an Exponential Smoothing model
    def fit_exponential_smoothing(params, train, test):
        trend = params['trend']
        seasonal = params['seasonal']
        seasonal_periods = params['seasonal_periods']

        model = ExponentialSmoothing(train['TotalSales'], trend=trend, seasonal=seasonal,
                                     seasonal_periods=seasonal_periods)
        fit_model = model.fit()
        forecast = fit_model.forecast(len(test))

        mse = mean_squared_error(test['TotalSales'], forecast)
        return mse

    # Perform grid search for Exponential Smoothing
    best_score_es, best_params_es = float('inf'), None
    for trend in param_grid_es['trend']:
        for seasonal in param_grid_es['seasonal']:
            for seasonal_periods in param_grid_es['seasonal_periods']:
                params = {'trend': trend, 'seasonal': seasonal, 'seasonal_periods': seasonal_periods}
                try:
                    score = fit_exponential_smoothing(params, train, test)
                    if score < best_score_es:
                        best_score_es, best_params_es = score, params
                except:
                    continue

    print('Best Exponential Smoothing Score:', best_score_es)
    print('Best Exponential Smoothing Params:', best_params_es)

    # Fit the best Exponential Smoothing model
    best_model_es = ExponentialSmoothing(train['TotalSales'],
                                         trend=best_params_es['trend'],
                                         seasonal=best_params_es['seasonal'],
                                         seasonal_periods=best_params_es['seasonal_periods'])
    fit_best_model_es = best_model_es.fit()

    # Forecast future sales
    forecast_es = fit_best_model_es.forecast(len(test))

    # Plot the results
    plt.figure(figsize=(12, 6))
    plt.plot(train['InvoiceDatetime'], train['TotalSales'], label='Train')
    plt.plot(test['InvoiceDatetime'], test['TotalSales'], label='Test')
    plt.plot(test['InvoiceDatetime'], forecast_es, label='Forecast')
    plt.title('Exponential Smoothing Forecast')
    plt.xlabel('Date')
    plt.ylabel('Total Sales')
    plt.legend()
    plt.show()