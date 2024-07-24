import numpy as np
import pandas as pd
from scipy.stats import boxcox
from sklearn.decomposition import PCA
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.preprocessing import StandardScaler

from services.graph_helper import GraphHelper


# from imblearn.over_sampling import
class SMOTE:
    pass


def prepare(data, ignore_visalizations):
    graph_helper = GraphHelper()

    # Drop Unique Identifier
    data = data.drop(['id'], axis=1)

    ### TODO: Create new variables? If value is above mean, then add 1 or 0

    # (1) Remove Highly Correlate Features
    high_correlated_features = ['texture_worst', 'perimeter_mean', 'area_mean', 'concave_points_mean',
                                'radius_worst', 'perimeter_worst', 'area_worst', 'smoothness_worst', 'compactness_mean',
                                'concavity_worst', 'concave_points_worst', 'fractal_dimension_worst', 'perimeter_se',
                                'area_se', 'concavity_se', 'fractal_dimension_se']

    data = data.drop(high_correlated_features, axis=1)

    # Get the column names of remaining independent variables
    remaining_x_columns = [column for column in data.columns
                           if column not in high_correlated_features and column != 'diagnosis']

    x = data.iloc[:, 1:]  # Exclude the first column which is the target variable
    y = data.iloc[:, 0]  # Select only the first column for the target variable

    # (2) Encoding categorical data (D.V) - Label Encoding
    label_encoder = LabelEncoder()
    y = label_encoder.fit_transform(y)
    diagnosis_m_count = (y == 1).sum()
    diagnosis_b_count = (y == 0).sum()
    print(f"Count Occurrence of value M (1): {diagnosis_m_count}")
    print(f"Count Occurrence of value B (0): {diagnosis_b_count}")

    # (3) Split dataset
    x_train, x_test, y_train, y_test = train_test_split(x, y, test_size=0.3, random_state=1)

    # (4) Over-sample using SMOTE
    smote = SMOTE(random_state=1)

    # print the number of instances for each class
    print("Before OverSampling, counts of label '1': {}".format(sum(y_train == 1)))
    print("Before OverSampling, counts of label '0': {} \n".format(sum(y_train == 0)))

    # Over-sample the minority class (in this case, the positive class) only on the training data
    x_train, y_train = smote.fit_resample(x_train, y_train)

    print("After OverSampling, counts of label '1': {}".format(sum(y_train == 1)))
    print("After OverSampling, counts of label '0': {} \n".format(sum(y_train == 0)))

    # (5) Transformation (smoothing skewness)
    x_train, x_test = transform(x_train, x_test, remaining_x_columns, ignore_visalizations)

    # (6) Standardization (smooth outliers scaling)
    sc = StandardScaler()
    x_train_scaled = sc.fit_transform(x_train)
    x_test_scaled = sc.transform(x_test)

    # View distributions after standardization
    if ignore_visalizations is False:
        # Convert the scaled arrays back to DataFrames
        x_train_scaled_df = pd.DataFrame(x_train_scaled, columns=x.columns)
        graph_helper.view_distributions(x_train_scaled_df, remaining_x_columns)

    return x_train_scaled, x_test_scaled, y_train, y_test, remaining_x_columns


def transform(x_train, x_test, remaining_x_columns, ignore_visalizations=False):
    graph_helper = GraphHelper()

    # View distributions prior transformation
    if ignore_visalizations is False:
        graph_helper.view_distributions(x_train, remaining_x_columns)

    # (4) Transformation (smoothing skewness)
    step = 0.001  # Adding 0.001 to handle zero values
    for column in remaining_x_columns:
        train_has_negative_values = x_train[column].lt(0).any()  # .lt => less than
        test_has_negative_values = x_test[column].lt(0).any()

        if train_has_negative_values or test_has_negative_values:
            # Perform Square Root transformation
            x_train[column] = np.sqrt(x_train[column] + step)
            x_test[column] = np.sqrt(x_test[column] + step)
        else:
            # Perform Box-Cox transformation (Does not support negative valued)
            x_train[column], lambda_value = boxcox(x_train[column] + step)
            # Use the lambda value from training data
            x_test[column] = boxcox(x_test[column] + step, lmbda=lambda_value)

    # View distributions after transformation
    if ignore_visalizations is False:
        graph_helper.view_distributions(x_train, remaining_x_columns)

    return x_train, x_test


def reduce_dimensions(x_train, x_test, to_n_dimensions):
    pca = PCA(n_components=to_n_dimensions, random_state=1)
    x_train = pca.fit_transform(x_train)
    x_test = pca.transform(x_test)

    return x_train, x_test

