from flask import Flask

from predictive_modeling.forecasting import perform_forecast
from services.rfm import segments_descriptions

app = Flask(__name__)

@app.route('/segment-details', methods=['GET'])
def sales_by_hour():
    return segments_descriptions

@app.route('/sales/forecasting', methods=['GET'])
def sales_forecasting():
    return perform_forecast()


# Run the Flask application
if __name__ == '__main__':
    app.run(debug=False)
