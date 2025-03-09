using System;
using System.Collections.Generic;


public partial class StationInspector
{
    public int StationInspectorId { get; set; }

    public int StationId { get; set; }

    public int InspectorId { get; set; }

    public DateTime? AssignedDate { get; set; }

    public bool? IsActive { get; set; }

    public string? Notes { get; set; }

    public virtual User Inspector { get; set; } = null!;

    public virtual User Station { get; set; } = null!;
}
