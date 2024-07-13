using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InspireWebApp.SpaBackend.Features.Customers;
using InspireWebApp.SpaBackend.Features.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InspireWebApp.SpaBackend.Features.Invoices;

public record InvoiceIdentifier
{
    public required int Id { get; init; }

    public static implicit operator InvoiceIdentifier(Invoice invoice)
    {
        return new InvoiceIdentifier
        {
            Id = invoice.Id
        };
    }
}

public class Invoice
{
    public Invoice()
    {
    }

    public Invoice(InvoiceIdentifier identifier)
    {
        Id = identifier.Id;
    }

    public int Id { get; set; }

    public required DateTime Date { get; set; }

    public required Customer Customer { get; set; }
    public required int CustomerId { get; set; }

    public required Employee Employee { get; set; }
    public required int EmployeeId { get; set; }

    public ICollection<InvoiceDetail>? InvoiceDetails { get; set; }
}