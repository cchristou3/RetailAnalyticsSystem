import matplotlib.pyplot as plt
import seaborn as sns


class GraphHelper:

    def __init__(self):
        pass
    
    def plot_histograms(self, dataset, columns):
        sns.set(style="whitegrid")

        # Assuming 'columns' is a list of column names you want to plot
        # and 'dataset' is your DataFrame
        total_columns = len(columns)
        n_cols = 3
        n_rows = total_columns // n_cols + (1 if total_columns % n_cols > 0 else 0)

        # Create the subplot grid
        fig, axes = plt.subplots(n_rows, n_cols, figsize=(20, 4 * n_rows))
        fig.tight_layout(pad=5.0)  # Adds spacing between plots

        # Flatten the axes array for easy iteration
        axes_flat = axes.flatten()

        # Loop through the specified columns and create a histogram for each
        for i, col in enumerate(columns):
            if col in dataset.columns:
                sns.histplot(data=dataset, x=col, ax=axes_flat[i], kde=True)  # 'kde' adds a density curve
                axes_flat[i].set_title(f'Histogram of {col}')

        # Hide any unused subplots
        for j in range(i + 1, n_rows * n_cols):
            fig.delaxes(axes_flat[j])

        plt.show()

    def plot_boxplot(self, dataset, categorical, numeric):
        sns.boxplot(x=categorical, y=numeric, data=dataset)

        plt.title(f'Boxplot of {numeric} by {categorical}')
        plt.xlabel(f'{categorical}')  # Adjust as necessary
        plt.ylabel(f'{numeric}')  # Adjust as necessary
        plt.show()

    def plot_boxplot_single_instance(self, data, column):
        sns.boxplot(y=data[f'{column}'])
        plt.title(f'Boxplot of {column}')
        plt.show()

    def view_distributions(self, data, columns):
        # View distributions after standardization
        for i in range(0, len(columns), 6):
            set_of_columns = columns[i:i + 6]
            self.plot_histograms(data, set_of_columns)

    def visualize_pruning_process(self, ccp_alphas, train_scores, test_scores, best_alpha, classifier=''):
        # see: https://scikit-learn.org/stable/auto_examples/tree/plot_cost_complexity_pruning.html
        # Visualize Pruning Process
        fig, ax = plt.subplots()
        ax.set_xlabel("alpha")
        ax.set_ylabel("accuracy")
        ax.set_title(f"Accuracy vs alpha for training and testing sets ({classifier})")
        ax.plot(ccp_alphas, train_scores, marker="o", label="train", drawstyle="steps-post")
        ax.plot(ccp_alphas, test_scores, marker="o", label="test", drawstyle="steps-post")
        ax.legend()

        ax.axvline(x=best_alpha, color='r', linestyle='--', lw=2, label='best alpha')
        ax.legend()
        plt.show()

    def visualize_single_instance_pruning_process(self, ccp_alphas, scores, classifier=''):
        plt.figure(figsize=(10, 6))
        plt.plot(ccp_alphas, scores, marker='o', drawstyle="steps-post")
        plt.xlabel('alpha')
        plt.ylabel('accuracy')
        plt.title(f'Accuracy vs alpha for training and testing sets ({classifier})')
        plt.show()

    def visualize_pruning_details(self, ccp_alphas, clfs, best_alpha, classifier=''):
        node_counts = [clf.tree_.node_count for clf in clfs]
        depth = [clf.tree_.max_depth for clf in clfs]
        fig, ax = plt.subplots(2, 1)
        ax[0].plot(ccp_alphas, node_counts, marker="o", drawstyle="steps-post")
        ax[0].set_xlabel("alpha")
        ax[0].set_ylabel("number of nodes")
        ax[0].set_title("Number of nodes vs alpha")
        ax[0].axvline(x=best_alpha, color='r', linestyle='--', lw=2, label='best alpha')
        ax[1].plot(ccp_alphas, depth, marker="o", drawstyle="steps-post")
        ax[1].set_xlabel("alpha")
        ax[1].set_ylabel("depth of tree")
        ax[1].set_title(f"Depth vs alpha - {classifier}")
        ax[1].axvline(x=best_alpha, color='r', linestyle='--', lw=2, label='best alpha')
        fig.tight_layout()