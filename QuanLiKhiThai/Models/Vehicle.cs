using System;
using System.Collections.Generic;


public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int OwnerId { get; set; }

    public string PlateNumber { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int ManufactureYear { get; set; }

    public string EngineNumber { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<InspectionAppointment> InspectionAppointments { get; set; } = new List<InspectionAppointment>();

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<ViolationRecord> ViolationRecords { get; set; } = new List<ViolationRecord>();
}
