using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data;

public record CustomerDistributionRecord
{
    public required int NumberOfCustomers { get; init; }
}