namespace InspireWebApp.SpaBackend.Features.Dashboard;

public class DemoChartTileOptions
{
    public DemoChartType Type { get; set; }
}

public enum DemoChartType
{
    Bar,
    Bubble,
    Bullet,
    HorizontalBar,
    Line,
    Partition,
    Pie,
    Sankey
}