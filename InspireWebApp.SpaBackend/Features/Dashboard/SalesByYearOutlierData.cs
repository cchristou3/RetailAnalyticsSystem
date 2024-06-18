﻿using InspireWebApp.SpaBackend.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.Dashboard;

public class SalesByYearOutlierData
{
    public int Year { get; set; }
    public double Value { get; set; }
}

public class SalesByYearOutlierDataEntityConfiguration : IEntityTypeConfiguration<SalesByYearOutlierData>
{
    public void Configure(EntityTypeBuilder<SalesByYearOutlierData> builder)
    {
        builder.QueryResultOnly();
    }
}