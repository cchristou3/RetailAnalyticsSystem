using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Data;

public record FinancialRecord
{
    public required decimal Value { get; init; }
    public required int Volume { get; init; }
}