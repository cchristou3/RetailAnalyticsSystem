﻿using System.Collections.Generic;

namespace InspireWebApp.SpaBackend.DataVisualization;

public static class DataVisualizationHelpers
{
    /// <summary>
    ///     Wraps a single dataset into a key -> dataset dictionary, using the default key.
    ///     See common-viz-data.ts for more info.
    /// </summary>
    public static Dictionary<string, object> WrapDefaultDataset(object dataset)
    {
        return new Dictionary<string, object>
        {
            [DataVisualizationConstants.DefaultDatasetKey] = dataset
        };
    }
}