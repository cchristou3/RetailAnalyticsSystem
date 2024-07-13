namespace InspireWebApp.SpaBackend.Features.Dashboard;

public class PredefinedTableTileOptions
{
    public PredefinedTableType Type { get; set; }
}

// Underlying values are persisted to the database, hence must never change
public enum PredefinedTableType
{
    AssociationRules = 0
}