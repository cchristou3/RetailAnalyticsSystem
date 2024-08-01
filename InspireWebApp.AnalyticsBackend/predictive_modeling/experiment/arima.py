from typing import Tuple

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import pmdarima as pm
from sklearn.metrics import mean_squared_error, mean_absolute_error
from sklearn.model_selection import train_test_split
from statsmodels.tsa.arima.model import ARIMA
from statsmodels.tsa.base.prediction import PredictionResults

from sql_client import SQLClient


def run():
    # TODO: Revisit when doing predictive modeling

    # Load the CSV file
    file_path: str = '../../Sales Data Online Shop.csv'
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


def run2():
    import pandas as pd
    import matplotlib.pyplot as plt
    from sklearn.metrics import mean_squared_error, mean_absolute_error
    from statsmodels.tsa.arima.model import ARIMA
    from sklearn.model_selection import train_test_split

    # Load the CSV file
    file_path: str = '../../Sales Data Online Shop.csv'
    data: pd.DataFrame = pd.read_csv(file_path, low_memory=False)

    # Data Preprocessing
    data['InvoiceDate'] = pd.to_datetime(data['InvoiceDate'])
    data['Date'] = pd.to_datetime(data['InvoiceDate'].dt.date)

    # Sort data by date
    data.sort_values(by='Date', inplace=True)

    # Feature Engineering
    data['DayOfWeek'] = data['Date'].dt.dayofweek
    data['Month'] = data['Date'].dt.month
    data['Year'] = data['Date'].dt.year

    # Aggregation and Forecasting Function
    def forecast_level(level: str):
        if level == 'daily':
            resample_rule = 'D'
        elif level == 'weekly':
            resample_rule = 'W'
        elif level == 'monthly':
            resample_rule = 'M'
        elif level == 'yearly':
            resample_rule = 'Y'
        else:
            raise ValueError("Level must be one of 'daily', 'weekly', 'monthly', or 'yearly'")

        # Aggregate data
        aggregated_sales = data.groupby('Date')['Quantity'].sum().resample(resample_rule).sum().reset_index()

        # Train-test split
        train, test = train_test_split(aggregated_sales, test_size=0.2, shuffle=False)

        # Model Selection and Training - ARIMA
        train_sales = train.set_index('Date')['Quantity']
        test_sales = test.set_index('Date')['Quantity']

        model = ARIMA(train_sales, order=(5, 1, 5))
        model_fit = model.fit()

        print('Summary: ')
        print(model_fit.summary())

        # Forecasting
        forecast = model_fit.forecast(steps=len(test_sales))

        # Evaluation
        mse = mean_squared_error(test_sales, forecast)
        mae = mean_absolute_error(test_sales, forecast)
        print(f'{level.capitalize()} - MSE: {mse}, MAE: {mae}')

        # Plotting the results
        plt.figure(figsize=(12, 6))
        plt.plot(train_sales, label='Train')
        plt.plot(test_sales.index, test_sales, label='Test')
        plt.plot(test_sales.index, forecast, label='Forecast')
        plt.title(f'{level.capitalize()} Forecast')
        plt.legend()
        plt.show()

    # Run forecasting for each level
    for level in ['daily', 'weekly', 'monthly', 'yearly']:
        forecast_level(level)


def fit_arima(order, train, test):
    model = ARIMA(train, order=order)
    model_fit = model.fit()
    return get_mean_squared_error(model_fit, train, test), model_fit


def evaluate_model(fitted, train, test):

    print(f'Summary: {fitted.summary()}')

    result = fitted.get_forecast(len(test), alpha=0.05)  # 95% conf

    fc_series, lower_series, upper_series = extract_forecast_intervals(result, test)

    plt.figure(figsize=(12, 5), dpi=100)
    plt.plot(train, label='training')
    plt.plot(test, label='actual')
    plt.plot(fc_series, label='forecast')
    plt.fill_between(lower_series.index, lower_series, upper_series,
                     color='k', alpha=.15)
    plt.title('Forecast vs Actuals')
    plt.legend(loc='upper left', fontsize=8)
    plt.show()

    fitted.plot_diagnostics(figsize=(7, 5))
    plt.show()


def extract_forecast_intervals(result: PredictionResults, test: pd.Series) -> Tuple[pd.Series, pd.Series, pd.Series]:
    # Get the forecast values
    forecast_values = result.predicted_mean

    # Get the confidence intervals
    confidence_intervals = result.conf_int(alpha=0.05)  # 95% confidence intervals

    # Extract lower and upper bounds
    lower_bound = confidence_intervals['lower TotalSales']
    upper_bound = confidence_intervals['upper TotalSales']

    # Make as pandas series
    fc_series = pd.Series(forecast_values, index=test.index)
    lower_series = pd.Series(lower_bound, index=test.index)
    upper_series = pd.Series(upper_bound, index=test.index)

    print('Other metrics:')
    print(forecast_accuracy(forecast_values.values, test.values))

    return fc_series, lower_series, upper_series


def get_mean_squared_error(fitted, train, test):
    forecast = fitted.predict(start=len(train), end=len(train) + len(test) - 1, dynamic=False)
    mse = mean_squared_error(test, forecast)
    return mse


def forecast_accuracy(forecast, actual):
    mape = np.mean(np.abs(forecast - actual) / np.abs(actual))  # MAPE
    me = np.mean(forecast - actual)  # ME
    mae = np.mean(np.abs(forecast - actual))  # MAE
    mpe = np.mean((forecast - actual) / actual)  # MPE
    rmse = np.mean((forecast - actual) ** 2) ** .5  # RMSE
    corr = np.corrcoef(forecast, actual)[0, 1]  # corr
    mins = np.amin(np.hstack([forecast[:, None],
                              actual[:, None]]), axis=1)
    maxs = np.amax(np.hstack([forecast[:, None],
                              actual[:, None]]), axis=1)
    minmax = 1 - np.mean(mins / maxs)  # minmax

    mse = mean_squared_error(actual, forecast)

    return ({'Mean Absolute Percentage Error (MAPE)': mape,
             'Mean Error (ME)': me,
             'Mean Absolute Error (MAE)': mae,
             'Mean Percentage Error (MPE)': mpe,
             'Root Mean Squared Error (RMSE)': rmse,
             'Mean Squared Error (MSE)': mse,
             'Correlation between the Actual and the Forecast (corr)': corr,
             'Min-Max Error (minmax)': minmax})


def exercise():
    data: pd.DataFrame = SQLClient().get_arima_prerequisite_data()

    data['InvoiceDate'] = pd.to_datetime(data['InvoiceDate'])

    # Extract month and year from the InvoiceDate
    data['YearMonth'] = data['InvoiceDate'].dt.to_period('M')
    # data['YearWeek'] = data['InvoiceDate'].dt.to_period('W')

    # Aggregate sales by month and year
    monthly_sales = data.groupby('YearMonth')['TotalSales'].sum().reset_index()

    show_graphs = False
    if show_graphs:
        # Plotting the data
        plt.figure(figsize=(10, 6))
        plt.plot(monthly_sales['YearMonth'].astype(str), monthly_sales['TotalSales'], marker='o')
        plt.title('Monthly Sales Over Time')
        plt.xlabel('Month-Year')
        plt.ylabel('Sales')
        plt.xticks(rotation=45)
        plt.grid(True)
        plt.tight_layout()
        plt.show()

        """ Identify the d (order of differencing) parameter for the ARIMA model """

        # Check for stationarity using the Augmented Dickey-Fuller test
        from statsmodels.tsa.stattools import adfuller

        result = adfuller(monthly_sales['TotalSales'])
        print('ADF Statistic:', result[0])
        print('p-value:', result[1])

        from statsmodels.graphics.tsaplots import plot_acf, plot_pacf

        # Original Series
        fig, axes = plt.subplots(3, 2, sharex=True)
        axes[0, 0].plot(monthly_sales['TotalSales'])
        axes[0, 0].set_title('Original Series')
        plot_acf(monthly_sales['TotalSales'], ax=axes[0, 1])

        # 1st Differencing
        axes[1, 0].plot(monthly_sales['TotalSales'].diff())
        axes[1, 0].set_title('1st Order Differencing')
        plot_acf(monthly_sales['TotalSales'].diff().dropna(), ax=axes[1, 1])

        # 2nd Differencing
        axes[2, 0].plot(monthly_sales['TotalSales'].diff().diff())
        axes[2, 0].set_title('2nd Order Differencing')
        plot_acf(monthly_sales['TotalSales'].diff().diff().dropna(), ax=axes[2, 1])

        plt.show()  # View comments_on_plot

        """ Identify the p parameter (significant lags -> AR term) for the ARIMA model """

        # Perform ADF test on the first-differenced series
        result = adfuller(monthly_sales['TotalSales'].diff().dropna())
        print('ADF Statistic:', result[0])
        print('p-value:', result[1])

        # PACF plot of 1st differenced series
        plt.rcParams.update({'figure.figsize': (9, 3), 'figure.dpi': 120})

        fig, axes = plt.subplots(1, 2, sharex=True)
        axes[0].plot(monthly_sales['TotalSales'].diff())
        axes[0].set_title('1st Differencing')
        axes[1].set(ylim=(0, 5))
        plot_pacf(monthly_sales['TotalSales'].diff().dropna(), ax=axes[1])

        plt.show()  # View comments_on_pacf_plot

        """ Identify the q parameter (significant lags -> MA term) for the ARIMA model """

        fig, axes = plt.subplots(1, 2, sharex=True)
        axes[0].plot(monthly_sales['TotalSales'].diff())
        axes[0].set_title('1st Differencing')
        axes[1].set(ylim=(0, 1.2))
        plot_acf(monthly_sales['TotalSales'].diff().dropna(), ax=axes[1])

        plt.show()  # View comments_on_q_parameter

    """ Split the data into Training set and Testing set """
    # Create Training and Test
    split_point = int(len(monthly_sales['TotalSales']) * 0.7)
    train = monthly_sales['TotalSales'][:split_point]
    test = monthly_sales['TotalSales'][split_point:]

    # 1,1,1 ARIMA Model
    model_1 = ARIMA(train, order=(1, 1, 1))
    model_1_fit = model_1.fit()
    # evaluate_model(model_1_fit, train, test)

    # 1,1,2 ARIMA Model (second iteration)
    model_2 = ARIMA(train, order=(1, 1, 2))
    model_2_fit = model_2.fit()
    # evaluate_model(model_2_fit, train, test)

    # ?,?,? ARIMA Model (third iteration): using auto_arima to find the best params
    model = pm.auto_arima(train,
                          start_p=1, start_q=1,
                          test='adf',  # use adftest to find optimal 'd'
                          max_p=7, max_q=7,  # maximum p and q
                          m=1,  # frequency of series
                          d=None,  # let model determine 'd'
                          seasonal=False,  # No Seasonality
                          start_P=0,
                          D=0,
                          trace=True,
                          error_action='ignore',
                          suppress_warnings=True,
                          stepwise=True,
                          max_order=25,
                          random_state=1)

    model_3 = ARIMA(train, order=model.order)
    model_3_fit = model_3.fit()
    # evaluate_model(model_3_fit, train, test)

    # My implementation of GridSearch
    # Step 8: Model Parameters Tuning
    from sklearn.model_selection import ParameterGrid

    d_parameters = [1]
    p_parameters = [1, 2, 6]
    q_parameters = [1, 2, 7]

    # Define a more extensive parameter grid
    param_grid = {
        'order': [(p, d, q) for p in p_parameters for d in d_parameters for q in q_parameters]
    }

    # Custom function to fit and score an ARIMA model

    # Perform grid search
    best_score, best_params = float('inf'), None
    for params in ParameterGrid(param_grid):
        try:
            print(f'training: {params['order']} =====================================')
            score, model_fit = fit_arima(params['order'], train, test)
            result = model_fit.get_forecast(len(test), alpha=0.05)  # 95% conf
            fc_series, lower_series, upper_series = extract_forecast_intervals(result, test)
            print(f'Params: {params['order']}, Score: {score}')
            if score < best_score:
                best_score, best_params = score, params
        except:
            pass

    print('Best Score:', best_score)
    print('Best Params:', best_params)

    model_4 = ARIMA(train, order=best_params['order'])
    model_4_fit = model_4.fit()
    # evaluate_model(model_4_fit, train, test)

    start = len(train)
    end = len(train) + len(test) - 1
    m1_predictions = model_1_fit.predict(start=start, end=end, dynamic=False, typ='levels').rename('Model 1 Predictions')
    m2_predictions = model_2_fit.predict(start=start, end=end, dynamic=False, typ='levels').rename('Model 2 Predictions')
    m3_predictions = model_3_fit.predict(start=start, end=end, dynamic=False, typ='levels').rename(
        'Model 3 Predictions')
    m4_predictions = model_4_fit.predict(start=start, end=end, dynamic=False, typ='levels').rename(
        'Model 4 Predictions')

    ax: plt.Axes = test.plot(legend=True, figsize=(12, 6), title='Sales, Actual v.s. Forecasted')
    m1_predictions.plot(legend=True)
    m2_predictions.plot(legend=True)
    m3_predictions.plot(legend=True)
    m4_predictions.plot(legend=True)
    ax.autoscale(axis='x', tight=True)
    ax.set(ylabel='Retail Sales Amount in MM USD')
    plt.show()


exercise()

comments_on_plot = """
Original Series
Visual Inspection: The original series shows a clear trend or variability over time, indicating non-stationarity.
ACF: The ACF of the original series shows a slow decay, which is another indication of non-stationarity.

1st Order Differencing
Visual Inspection: The 1st order differencing plot shows the differences between consecutive observations. It appears to stabilize the mean, as the variations seem more consistent around zero.
ACF: The ACF of the first-differenced series shows a more rapid decay, with fewer significant lags compared to the original series. This is a good sign that the series has become more stationary.

2nd Order Differencing
Visual Inspection: The 2nd order differencing plot shows the differences of the first-differenced series. It appears more erratic and might not be necessary if the 1st order differencing is sufficient.
ACF: The ACF of the second-differenced series shows even fewer significant lags, but it might not be needed if the 1st differencing suffices.

Conclusion: Is 1st Differencing Sufficient?
Based on the plots and the ACF:
Visual Stability: The 1st differencing plot appears to stabilize the mean, removing any apparent trend.
ACF Behavior: The ACF for the first-differenced series shows a rapid decay, indicating that the series is likely stationary.

Additional Check: Augmented Dickey-Fuller (ADF) Test
To conclusively determine stationarity, you should refer to the Augmented Dickey-Fuller (ADF) test results for the 1st differenced series. The ADF test provides a statistical test for stationarity:

ADF Statistic: If the ADF statistic is significantly lower than the critical value, it indicates stationarity.
p-value: A p-value less than 0.05 typically suggests rejecting the null hypothesis of a unit root, indicating stationarity.
If your ADF test for the first-differenced series results in a low p-value (typically <0.05), it further confirms that 1st differencing is sufficient.

The p-value for the Augmented Dickey-Fuller (ADF) Test was 0.007496603033164487, meaning that the 1st differencing makes the dataset stationary.

MAIN CONCLUSION: Use d = 1 for the ARIMA model (order of differencing)
"""

comments_on_pacf_plot = """
**** PACF Plot Explanation
X-axis (Lags):

The X-axis represents the lag order.
A lag of 1 means you are looking at the correlation between the time series and itself lagged by 1 period.
A lag of 2 means the correlation between the time series and itself lagged by 2 periods, and so on.
Each point on the X-axis corresponds to a different lag value.

Y-axis (Partial Autocorrelation):

The Y-axis shows the partial autocorrelation coefficient values.
These values measure the correlation between the time series and its lagged values, after removing the effects of intermediate lags.

Blue Region (Confidence Interval):

The blue shaded region represents the confidence interval for the partial autocorrelation coefficients.
If a bar extends beyond this region, it indicates that the partial autocorrelation at that lag is statistically significant.
Typically, the confidence interval is set at 95%, meaning any coefficient outside this range is significantly different from zero at the 5% significance level.


**** How to Interpret the PACF Plot
Significant Lags:

Look for bars that extend beyond the blue region (confidence interval). 
These bars represent lags where the partial autocorrelation is significantly different from zero.

Significant lags indicate potential autoregressive terms (AR) in the ARIMA model.

**** Conclusion:
The X-axis represents the lag order.
The Y-axis represents the partial autocorrelation values.
The blue region indicates the confidence interval.
Significant bars outside the blue region indicate potential AR terms for your model.

Based on the pacf plot the significant lags are 1,2, and 6. So, I will be using 1,2, and 6 for the p parameter (AR term)
"""

comments_on_q_parameter = """
We are using the ACF plot to try to identify the significant lags.
The plot illustrates 1,2, and 7 to be the significant lags. Thus, they will be used fo the q parameter (MA term)
"""
