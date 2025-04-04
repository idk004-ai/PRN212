﻿

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    Phone NVARCHAR(15) NOT NULL,
    Address NVARCHAR(MAX) NOT NULL,
	IsEnabled BIT NOT NULL DEFAULT 0,
	VerificationToken NVARCHAR(255) NULL,
    TokenExpiry DATETIME NULL,
    CONSTRAINT CHK_UserRole CHECK (Role IN ('Owner', 'Inspector', 'Station', 'Police'))
);

CREATE TABLE Vehicles (
    VehicleID INT PRIMARY KEY IDENTITY(1,1),
    OwnerID INT NOT NULL,
    PlateNumber NVARCHAR(15) NOT NULL UNIQUE,
    Brand NVARCHAR(50) NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    ManufactureYear INT NOT NULL,
    EngineNumber NVARCHAR(100) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Vehicles_Users FOREIGN KEY (OwnerID) REFERENCES Users(UserID)
);

CREATE TABLE InspectionAppointments (
	AppointmentID INT PRIMARY KEY IDENTITY(1,1),
	VehicleID INT NOT NULL,
	StationID INT NOT NULL,
	ScheduledDateTime DATETIME NOT NULL,
	Status NVARCHAR(20) DEFAULT 'Pending' NOT NULL,
	CreatedAt DATETIME DEFAULT GETDATE(),
	CONSTRAINT CHK_AppointmentStatus CHECK (Status IN ('Pending', 'Assigned', 'Completed', 'Cancelled')),
    CONSTRAINT FK_Appointments_Vehicles FOREIGN KEY (VehicleID) REFERENCES Vehicles(VehicleID),
    CONSTRAINT FK_Appointments_Stations FOREIGN KEY (StationID) REFERENCES Users(UserID)
)

CREATE TABLE InspectionRecords (
    RecordID INT PRIMARY KEY IDENTITY(1,1),
    VehicleID INT NOT NULL,
    StationID INT NOT NULL,
    InspectorID INT NOT NULL,
	AppointmentID INT NOT NULL, 
    InspectionDate DATETIME DEFAULT GETDATE(),
    Result NVARCHAR(10) NOT NULL,
    CO2Emission DECIMAL(5,2) NOT NULL,
    HCEmission DECIMAL(5,2) NOT NULL,
    Comments NVARCHAR(MAX),
    CONSTRAINT CHK_InspectionResult CHECK (Result IN ('Pass', 'Fail', 'Testing', 'Cancelled')),
    CONSTRAINT FK_InspectionRecords_Vehicles FOREIGN KEY (VehicleID) REFERENCES Vehicles(VehicleID),
    CONSTRAINT FK_InspectionRecords_Stations FOREIGN KEY (StationID) REFERENCES Users(UserID),
    CONSTRAINT FK_InspectionRecords_Users FOREIGN KEY (InspectorID) REFERENCES Users(UserID),
	CONSTRAINT FK_InspectionRecords_Appointments FOREIGN KEY (AppointmentID) REFERENCES InspectionAppointments(AppointmentID)
);

GO

CREATE TABLE StationInspectors (
    StationInspectorID INT PRIMARY KEY IDENTITY(1,1),
    StationID INT NOT NULL,
    InspectorID INT NOT NULL,
    AssignedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    Notes NVARCHAR(MAX),
    
    -- Tham chiếu khóa ngoại bình thường
    CONSTRAINT FK_StationInspectors_Station FOREIGN KEY (StationID) 
        REFERENCES Users(UserID),
    CONSTRAINT FK_StationInspectors_Inspector FOREIGN KEY (InspectorID) 
        REFERENCES Users(UserID),
    
    -- Đảm bảo mỗi Inspector chỉ được liên kết với mỗi Station một lần
    CONSTRAINT UQ_StationInspector UNIQUE (StationID, InspectorID)
);

GO

CREATE PROCEDURE sp_AddInspectionRecord 
	@VehicleID INT,
	@StationID INT,
	@InspectorID INT,
	@AppointmentID INT,
	@InspectionDate DATETIME,
	@Result NVARCHAR(10),
	@CO2Emission DECIMAL(5,2),
	@HCEmission DECIMAL(5,2),
	@Comments NVARCHAR(MAX)
AS
BEGIN
	-- Check StationID
	IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @StationID AND Role = 'Station')
	BEGIN 
		THROW 50000, 'StationID must reference a User with the Station role', 1;
		RETURN;
	END

	-- Check InspectorID
	IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @InspectorID AND Role = 'Inspector')
    BEGIN
        THROW 50001, 'InspectorID must reference a User with the Inspector role', 1;
        RETURN;
    END

	INSERT INTO InspectionRecords (VehicleID, StationID, InspectorID, AppointmentID, InspectionDate, Result, CO2Emission, HCEmission, Comments)
	VALUES (@VehicleID, @StationID, @InspectorID, @AppointmentID, @InspectionDate, @Result, @CO2Emission, @HCEmission, @Comments);
END

GO

CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    SentDate DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE Logs (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Action NVARCHAR(100) NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Logs_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE ViolationRecords (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VehicleId INT NOT NULL,
    OfficerId INT NOT NULL,
    IssueDate DATETIME NOT NULL,
    Location NVARCHAR(255) NOT NULL,
    ViolationType NVARCHAR(100) NOT NULL,
    FineAmount DECIMAL(10, 2) NOT NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_ViolationRecords_Vehicles FOREIGN KEY (VehicleId) 
        REFERENCES Vehicles(VehicleId),
    
    CONSTRAINT FK_ViolationRecords_Officers FOREIGN KEY (OfficerId) 
        REFERENCES Users(UserID)
);

GO

CREATE TRIGGER TR_ViolationRecords_OfficerRole
ON ViolationRecords
AFTER INSERT, UPDATE
AS
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM inserted i
        LEFT JOIN Users u ON i.OfficerId = u.UserID
        WHERE u.Role <> 'Police' OR u.Role IS NULL
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50000, 'OfficerId must reference a User with the Police role.', 1;
    END
END;