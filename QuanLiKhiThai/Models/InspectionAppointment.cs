using System;
using System.Collections.Generic;

public partial class InspectionAppointment
{
    public int AppointmentId { get; set; }

    public int VehicleId { get; set; }

    public int StationId { get; set; }

    public DateTime ScheduledDateTime { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual User Station { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
