from sklearn.metrics import roc_auc_score, precision_score, recall_score, accuracy_score, confusion_matrix


def predict_and_evaluate(classifier_name, classifier, x_train, y_train, x_test, y_test):
    # Predictions
    y_pred_train = classifier.predict(x_train)
    y_pred_test = classifier.predict(x_test)

    # Evaluate the model
    roc_auc = roc_auc_score(y_test, y_pred_test)
    precision = precision_score(y_test, y_pred_test)
    recall = recall_score(y_test, y_pred_test)
    accuracy = accuracy_score(y_test, y_pred_test)
    misclassification_rate = 1 - accuracy

    # Confusion Matrix
    cm = confusion_matrix(y_test, y_pred_test)

    # Check for over-fitting or under-fitting
    train_accuracy = accuracy_score(y_train, y_pred_train)
    test_accuracy = accuracy_score(y_test, y_pred_test)

    # Return evaluation metrics
    evaluation_results = {
        'Classifier': classifier_name,
        'ROC AUC': roc_auc,
        'Precision': precision,
        'Recall': recall,
        'Accuracy': accuracy,
        'Misclassification Rate': misclassification_rate,
        'Training Accuracy': train_accuracy,
        'Testing Accuracy': test_accuracy,
        'Discrepancy in Accuracy': train_accuracy - test_accuracy
    }

    return evaluation_results