import matplotlib.pyplot as plt
import numpy as np
import seaborn as sns


class Helper:

    def __init__(self):
        print('Helper Instantiated!')

    def print_column_stats(self, dataset, column):
        print(f"{column}: {dataset[column].dtype}")

        print('*********************************************')
        print(f'START Calculating stats for column {column}')

        # <editor-fold desc="Calculations">
        count_nulls = dataset[column].isnull().sum()
        count_zeros = (dataset[column] == 0).sum()
        mean = dataset[column].mean()
        median = dataset[column].median()
        mode = dataset[column].mode()[0]
        max = dataset[column].max()
        min = dataset[column].min()
        q1 = np.percentile(dataset[column], 25)
        q3 = np.percentile(dataset[column], 75)
        # </editor-fold>
        print(f"Printing stats for column {column}")
        print(f"Count Nulls: {count_nulls}")
        print(f"Count 0s: {count_zeros}")
        print(f"Mean: {mean}")

        print(f"Mode: {mode}")
        print(f"Min value: {min}")
        print(f"Q1 value: {q1}")
        print(f"Median: {median}")
        print(f"Q3 value: {q3}")
        print(f"Max value: {max}")
        print('*********************************************')
        print()
        print()

        return {
            'Column': column,
            'Min': min,
            'Q1': q1,
            'Median': median,
            'Mean': mean,
            'Q3': q3,
            'Max': max,
        }

    def find_outliers(self, dataset, column):
        q1 = dataset[column].quantile(0.25)
        q3 = dataset[column].quantile(0.75)
        iQR = q3 - q1
        lower_bound = q1 - 1.5 * iQR
        upper_bound = q3 + 1.5 * iQR

        outliers = dataset[(dataset[column] < lower_bound) | (dataset[column] > upper_bound)]
        return outliers

    def visualize_outliers(self, dataset, columns):
        sns.set(style="whitegrid")

        # Calculate the number of rows and columns for the subplot grid
        total_columns = len(columns)
        n_cols = 3
        n_rows = total_columns // n_cols + (1 if total_columns % n_cols > 0 else 0)

        # Create the subplot grid
        fig, axes = plt.subplots(n_rows, n_cols, figsize=(20, 4 * n_rows))
        fig.tight_layout(pad=5.0)  # Adds spacing between plots

        # Flatten the axes array for easy iteration
        axes_flat = axes.flatten()

        # Loop through the specified columns and create a boxplot for each
        for i, col in enumerate(columns):
            if col in dataset.columns:
                sns.boxplot(x=dataset[col], ax=axes_flat[i])
                axes_flat[i].set_title(f'Boxplot of {col}')
            else:
                print(f"Column '{col}' not found in dataset.")

        # Hide any unused subplots
        for j in range(i + 1, n_rows * n_cols):
            fig.delaxes(axes_flat[j])

        plt.show()

    def visualize_correlations(self, dataset):
        corr = dataset.corr()

        THRESHOLD = 0

        mask = (np.abs(corr) < THRESHOLD)

        # Create a heatmap with the mask
        plt.figure(figsize=(10, 8))
        sns.heatmap(corr, mask=mask, cmap='coolwarm_r', annot=True, linewidths=1, fmt=".2f",
                    cbar_kws={'shrink': .5}, vmin=-1.0, vmax=1.0)
        plt.title(f'Correlation Heatmap (|correlation| >= {THRESHOLD})')
        plt.show()

    def print_correlations(self, dataset):
        corr = dataset.corr()

        print(corr)
