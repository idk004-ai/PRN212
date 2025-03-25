using System;
using System.Collections.Generic;

public partial class ViolationRecord
{
    public int Id { get; set; }

    public int VehicleId { get; set; }

    public int OfficerId { get; set; }

    public DateTime IssueDate { get; set; }

    public string Location { get; set; } = null!;

    public string ViolationType { get; set; } = null!;

    public decimal FineAmount { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Officer { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
