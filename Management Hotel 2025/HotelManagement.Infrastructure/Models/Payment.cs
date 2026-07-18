using System;
using System.Collections.Generic;

namespace Mydata.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? TransactionId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int BookingId { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
