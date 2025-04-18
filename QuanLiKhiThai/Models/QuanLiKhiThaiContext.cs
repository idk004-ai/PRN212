﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public partial class QuanLiKhiThaiContext : DbContext
{
    public QuanLiKhiThaiContext()
    {
    }

    public QuanLiKhiThaiContext(DbContextOptions<QuanLiKhiThaiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<InspectionAppointment> InspectionAppointments { get; set; }

    public virtual DbSet<InspectionRecord> InspectionRecords { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<StationInspector> StationInspectors { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<ViolationRecord> ViolationRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;uid=sa;password=123;database=QuanLiKhiThai;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InspectionAppointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Inspecti__8ECDFCA219870EA0");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ScheduledDateTime).HasColumnType("datetime");
            entity.Property(e => e.StationId).HasColumnName("StationID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");

            entity.HasOne(d => d.Station).WithMany(p => p.InspectionAppointments)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Stations");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.InspectionAppointments)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Vehicles");
        });

        modelBuilder.Entity<InspectionRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__Inspecti__FBDF78C99D69050F");

            entity.Property(e => e.RecordId).HasColumnName("RecordID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.Co2emission)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("CO2Emission");
            entity.Property(e => e.Hcemission)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("HCEmission");
            entity.Property(e => e.InspectionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InspectorId).HasColumnName("InspectorID");
            entity.Property(e => e.Result).HasMaxLength(10);
            entity.Property(e => e.StationId).HasColumnName("StationID");
            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.InspectionRecords)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InspectionRecords_Appointments");

            entity.HasOne(d => d.Inspector).WithMany(p => p.InspectionRecordInspectors)
                .HasForeignKey(d => d.InspectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InspectionRecords_Users");

            entity.HasOne(d => d.Station).WithMany(p => p.InspectionRecordStations)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InspectionRecords_Stations");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.InspectionRecords)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InspectionRecords_Vehicles");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Logs__5E5499A88C9856F4");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logs_Users");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E329B67B96C");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<StationInspector>(entity =>
        {
            entity.HasKey(e => e.StationInspectorId).HasName("PK__StationI__E88862A04AA0BBB5");

            entity.HasIndex(e => new { e.StationId, e.InspectorId }, "UQ_StationInspector").IsUnique();

            entity.Property(e => e.StationInspectorId).HasColumnName("StationInspectorID");
            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InspectorId).HasColumnName("InspectorID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StationId).HasColumnName("StationID");

            entity.HasOne(d => d.Inspector).WithMany(p => p.StationInspectorInspectors)
                .HasForeignKey(d => d.InspectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StationInspectors_Inspector");

            entity.HasOne(d => d.Station).WithMany(p => p.StationInspectorStations)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StationInspectors_Station");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC4EFDBA02");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534913E9E1C").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.TokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.VerificationToken).HasMaxLength(255);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicles__476B54B21137E6CE");

            entity.HasIndex(e => e.PlateNumber, "UQ__Vehicles__03692624B2242CE5").IsUnique();

            entity.Property(e => e.VehicleId).HasColumnName("VehicleID");
            entity.Property(e => e.Brand).HasMaxLength(50);
            entity.Property(e => e.EngineNumber).HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.PlateNumber).HasMaxLength(15);

            entity.HasOne(d => d.Owner).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_Users");
        });

        modelBuilder.Entity<ViolationRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Violatio__3214EC07616866BE");

            entity.ToTable(tb => tb.HasTrigger("TR_ViolationRecords_OfficerRole"));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FineAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IssueDate).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.ViolationType).HasMaxLength(100);

            entity.HasOne(d => d.Officer).WithMany(p => p.ViolationRecords)
                .HasForeignKey(d => d.OfficerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ViolationRecords_Officers");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.ViolationRecords)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ViolationRecords_Vehicles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
