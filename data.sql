USE QuanLiKhiThai

--DELETE FROM Users
--DBCC CHECKIDENT('Users', RESEED, 0)

-- SQL script to insert dummy data into Users table(Owner role)
INSERT INTO Users (FullName, Email, Password, Role, Phone, Address)
VALUES 
    ('John Smith', 'john.smith@example.com', 'hashed_password_123', 'Owner', '(555) 111-2222', '123 Oak Street, Apartment 4B, Springfield, IL 62701'),
    ('Maria Rodriguez', 'maria.rodriguez@example.com', 'hashed_password_456', 'Owner', '(555) 222-3333', '456 Maple Avenue, Suite 200, Riverside, CA 92501'),
    ('Ahmed Khan', 'ahmed.khan@example.com', 'hashed_password_789', 'Owner', '(555) 333-4444', '789 Pine Road, Unit 15, Lakeside, NY 10001'),
    ('Sarah Johnson', 'sarah.johnson@example.com', 'hashed_password_abc', 'Owner', '(555) 444-5555', '101 Cedar Lane, Townhouse 7, Mountain View, CO 80201'),
    ('Wei Zhang', 'wei.zhang@example.com', 'hashed_password_def', 'Owner', '(555) 555-6666', '202 Birch Boulevard, Floor 3, Oceanside, FL 33101'),
    ('Emily Davis', 'emily.davis@example.com', 'hashed_password_ghi', 'Owner', '(555) 666-7777', '303 Redwood Drive, Building C, Westfield, TX 75001'),
    ('Carlos Mendez', 'carlos.mendez@example.com', 'hashed_password_jkl', 'Owner', '(555) 777-8888', '404 Spruce Court, Unit 22A, Eastville, WA 98101'),
    ('Aisha Patel', 'aisha.patel@example.com', 'hashed_password_mno', 'Owner', '(555) 888-9999', '505 Elm Street, House 10, Northtown, GA 30301'),
    ('Robert Kim', 'robert.kim@example.com', 'hashed_password_pqr', 'Owner', '(555) 999-0000', '606 Willow Way, Apartment 5D, Southfield, MI 48075'),
    ('Olivia Wilson', 'olivia.wilson@example.com', 'hashed_password_stu', 'Owner', '(555) 000-1111', '707 Aspen Avenue, Suite 305, Centreville, PA 19101');

-- SQL script to insert dummy data into Users table (Inspector role)
INSERT INTO Users (FullName, Email, Password, Role, Phone, Address)
VALUES 
    ('Michael Thompson', 'michael.thompson@inspection.gov', 'hashed_password_insp1', 'Inspector', '(555) 123-4567', '123 Certification Lane, Suite 101, Inspection City, CA 94105'),
    ('Jennifer Wilson', 'jennifer.wilson@inspection.gov', 'hashed_password_insp2', 'Inspector', '(555) 234-5678', '456 Validation Road, Floor 3, Standards Town, NY 10001'),
    ('David Martinez', 'david.martinez@inspection.gov', 'hashed_password_insp3', 'Inspector', '(555) 345-6789', '789 Compliance Avenue, Unit B, Regulation City, TX 75001'),
    ('Lisa Anderson', 'lisa.anderson@inspection.gov', 'hashed_password_insp4', 'Inspector', '(555) 456-7890', '101 Quality Control Street, Building 4, Safety Harbor, FL 33755'),
    ('James Taylor', 'james.taylor@inspection.gov', 'hashed_password_insp5', 'Inspector', '(555) 567-8901', '202 Verification Drive, Suite 220, Assessment Village, IL 60601'),
    ('Sophia Nguyen', 'sophia.nguyen@inspection.gov', 'hashed_password_insp6', 'Inspector', '(555) 678-9012', '303 Testing Blvd, Apartment 3C, Evaluation Heights, WA 98101'),
    ('Robert Johnson', 'robert.johnson@inspection.gov', 'hashed_password_insp7', 'Inspector', '(555) 789-0123', '404 Audit Court, Office 505, Examination Park, CO 80202'),
    ('Emma Garcia', 'emma.garcia@inspection.gov', 'hashed_password_insp8', 'Inspector', '(555) 890-1234', '505 Standard Street, Suite 15, Certification Springs, MI 48226'),
    ('Daniel Lee', 'daniel.lee@inspection.gov', 'hashed_password_insp9', 'Inspector', '(555) 901-2345', '606 Protocol Place, Unit 7D, Compliance Falls, GA 30303'),
    ('Olivia Brown', 'olivia.brown@inspection.gov', 'hashed_password_insp10', 'Inspector', '(555) 012-3456', '707 Regulation Road, Building A, Inspection Valley, PA 19103');

-- SQL script to insert dummy data into Users table (Station role)
INSERT INTO Users (FullName, Email, Password, Role, Phone, Address)
VALUES 
    ('Central Vehicle Inspection', 'info@centralinspection.com', 'hashed_password_station1', 'Station', '(555) 123-4567', '123 Main Street, Downtown, City Center 10001'),
    ('Highway Safety Center', 'contact@highwaysafety.org', 'hashed_password_station2', 'Station', '(555) 234-5678', '456 Boulevard Ave, West District, Metro City 20002'),
    ('Quality Auto Inspection', 'service@qualityauto.net', 'hashed_password_station3', 'Station', '(555) 345-6789', '789 Industrial Road, North Zone, Tech City 30003'),
    ('Express Vehicle Check', 'appointments@expresscheck.com', 'hashed_password_station4', 'Station', '(555) 456-7890', '321 Fast Lane, East Side, Speed Town 40004'),
    ('Reliable Inspection Services', 'info@reliableinspect.com', 'hashed_password_station5', 'Station', '(555) 567-8901', '654 Trust Street, South Quarter, Faith City 50005'),
    ('Metro Vehicle Testing', 'support@metrotesting.org', 'hashed_password_station6', 'Station', '(555) 678-9012', '987 Urban Drive, Downtown, Capital City 60006'),
    ('Countryside Inspection Station', 'hello@countrysideinspect.com', 'hashed_password_station7', 'Station', '(555) 789-0123', '246 Rural Route, Green Valley, Nature County 70007'),
    ('TechSafe Inspection Center', 'service@techsafeinspect.net', 'hashed_password_station8', 'Station', '(555) 890-1234', '135 Innovation Park, Tech District, Future City 80008'),
    ('Harbor Vehicle Inspection', 'appointments@harborinspect.com', 'hashed_password_station9', 'Station', '(555) 901-2345', '579 Port Road, Waterfront, Ocean City 90009'),
    ('Mountain View Auto Check', 'info@mountainviewcheck.org', 'hashed_password_station10', 'Station', '(555) 012-3456', '864 Summit Way, Highland District, Peak Town 10010');

--DELETE FROM Vehicles
--DBCC CHECKIDENT ('Vehicles', RESEED, 0)

-- SQL script to insert dummy data into Vehicles table
INSERT INTO Vehicles (OwnerID, PlateNumber, Brand, Model, ManufactureYear, EngineNumber)
VALUES 
    (1, 'ABC-1234', 'Toyota', 'Camry', 2022, 'ENG-TOY-22-78945612'),
    (2, 'XYZ-5678', 'Honda', 'Civic', 2021, 'ENG-HON-21-32165498'),
    (3, 'DEF-9012', 'Ford', 'F-150', 2023, 'ENG-FRD-23-45612378'),
    (4, 'GHI-3456', 'Chevrolet', 'Malibu', 2020, 'ENG-CHV-20-98765432'),
    (5, 'JKL-7890', 'Nissan', 'Altima', 2022, 'ENG-NSN-22-12378945'),
    (6, 'MNO-1357', 'Hyundai', 'Sonata', 2021, 'ENG-HYD-21-65432198'),
    (7, 'PQR-2468', 'Kia', 'Optima', 2023, 'ENG-KIA-23-78912345'),
    (8, 'STU-3690', 'Volkswagen', 'Jetta', 2020, 'ENG-VLK-20-36925814'),
    (9, 'VWX-4712', 'BMW', '3 Series', 2022, 'ENG-BMW-22-15948726'),
    (10, 'YZA-5824', 'Mercedes-Benz', 'C-Class', 2021, 'ENG-MRC-21-95175386');

GO

INSERT INTO InspectionAppointments (VehicleID, StationID, ScheduledDateTime, Status, CreatedAt)
VALUES
    -- January 2025 appointments
    (1, 21, '2025-01-05 09:00:00', 'Completed', '2025-01-01 10:15:00'),
    (2, 22, '2025-01-07 14:00:00', 'Completed', '2025-01-03 09:30:00'),
    (3, 23, '2025-01-10 11:30:00', 'Completed', '2025-01-05 16:45:00'),
    (4, 24, '2025-01-12 09:45:00', 'Completed', '2025-01-07 13:20:00'),
    (5, 25, '2025-01-15 13:15:00', 'Completed', '2025-01-10 11:10:00'),
    
    -- Re-inspection appointments
    (3, 23, '2025-01-24 09:15:00', 'Completed', '2025-01-15 14:30:00'),
    
    -- February 2025 appointments
    (6, 26, '2025-02-03 09:00:00', 'Completed', '2025-01-28 10:45:00'),
    (7, 27, '2025-02-05 10:45:00', 'Completed', '2025-01-30 09:15:00'),
    (8, 28, '2025-02-08 14:15:00', 'Completed', '2025-02-03 11:30:00'),
    (9, 29, '2025-02-12 10:30:00', 'Completed', '2025-02-07 15:45:00'),
    (10, 30, '2025-02-15 12:45:00', 'Completed', '2025-02-10 09:30:00'),
    
    -- Re-inspection appointments
    (6, 26, '2025-02-17 14:00:00', 'Completed', '2025-02-16 09:15:00'),
    (10, 30, '2025-02-26 11:15:00', 'Completed', '2025-02-20 14:30:00'),
    
    -- Recent appointments
    (1, 25, '2025-02-20 09:45:00', 'Completed', '2025-02-15 10:30:00'),
    (2, 24, '2025-02-22 13:30:00', 'Completed', '2025-02-17 11:15:00'),
    (4, 23, '2025-02-24 09:00:00', 'Completed', '2025-02-19 14:45:00'),
    (5, 22, '2025-02-25 15:15:00', 'Completed', '2025-02-20 09:30:00'),
    (7, 21, '2025-02-27 07:45:00', 'Completed', '2025-02-22 13:20:00'),
    (8, 28, '2025-02-27 10:00:00', 'Completed', '2025-02-22 16:45:00'),
    (9, 27, '2025-02-27 13:15:00', 'Completed', '2025-02-23 10:30:00'),
    
    -- Today's appointments
    (1, 21, '2025-02-27 06:45:00', 'Completed', '2025-02-23 11:30:00'),
    (3, 23, '2025-02-27 07:15:00', 'Completed', '2025-02-23 15:45:00'),
    (5, 25, '2025-02-27 07:20:00', 'Completed', '2025-02-24 09:20:00'),
    (7, 27, '2025-02-27 07:20:00', 'Completed', '2025-02-24 13:35:00'),
    
    -- Testing appointments
    (2, 22, '2025-02-28 09:00:00', 'Assigned', '2025-02-25 10:15:00'),
    (4, 24, '2025-02-28 10:15:00', 'Assigned', '2025-02-25 14:30:00'),
    (6, 26, '2025-02-28 11:30:00', 'Assigned', '2025-02-26 09:45:00'),
    (10, 23, '2025-02-28 13:00:00', 'Assigned', '2025-02-26 15:20:00'),
    (8, 30, '2025-02-28 14:30:00', 'Assigned', '2025-02-27 10:15:00'),
    (9, 29, '2025-02-28 15:15:00', 'Assigned', '2025-02-27 11:30:00'),
    (3, 25, '2025-02-28 15:45:00', 'Assigned', '2025-02-27 13:45:00'),
    (5, 27, '2025-02-28 16:15:00', 'Assigned', '2025-02-27 16:00:00'),
    (1, 28, '2025-02-28 16:45:00', 'Assigned', '2025-02-27 17:30:00');


GO

-- January 2025 inspections
EXEC sp_AddInspectionRecord
    @VehicleID = 1,
    @StationID = 21,
    @InspectorID = 11,
    @InspectionDate = '2025-01-05 09:30:00',
    @Result = 'Pass',
    @CO2Emission = 142.35,
    @HCEmission = 0.82,
    @Comments = 'Vehicle in excellent condition. All emissions within acceptable range.',
    @AppointmentID = 1;

EXEC sp_AddInspectionRecord
    @VehicleID = 2,
    @StationID = 22,
    @InspectorID = 12,
    @InspectionDate = '2025-01-07 14:15:00',
    @Result = 'Pass',
    @CO2Emission = 138.75,
    @HCEmission = 0.91,
    @Comments = 'Minor exhaust system wear but emissions acceptable.',
    @AppointmentID = 2;

EXEC sp_AddInspectionRecord
    @VehicleID = 3,
    @StationID = 23,
    @InspectorID = 13,
    @InspectionDate = '2025-01-10 11:45:00',
    @Result = 'Fail',
    @CO2Emission = 198.62,
    @HCEmission = 1.35,
    @Comments = 'Excessive CO2 and HC emissions. Recommend catalytic converter replacement.',
    @AppointmentID = 3;

EXEC sp_AddInspectionRecord
    @VehicleID = 4,
    @StationID = 24,
    @InspectorID = 14,
    @InspectionDate = '2025-01-12 10:00:00',
    @Result = 'Pass',
    @CO2Emission = 145.20,
    @HCEmission = 0.88,
    @Comments = 'Vehicle passed with emissions slightly above average. Recommend tune-up soon.',
    @AppointmentID = 4;

EXEC sp_AddInspectionRecord
    @VehicleID = 5,
    @StationID = 25,
    @InspectorID = 15,
    @InspectionDate = '2025-01-15 13:30:00',
    @Result = 'Pass',
    @CO2Emission = 132.45,
    @HCEmission = 0.76,
    @Comments = 'Excellent emission readings. Vehicle well maintained.',
    @AppointmentID = 5;

-- February 2025 inspections
EXEC sp_AddInspectionRecord
    @VehicleID = 6,
    @StationID = 26,
    @InspectorID = 16,
    @InspectionDate = '2025-02-03 09:15:00',
    @Result = 'Fail',
    @CO2Emission = 212.78,
    @HCEmission = 1.42,
    @Comments = 'Failed emission test. Fuel system needs maintenance. Exhaust leak detected.',
    @AppointmentID = 7;

EXEC sp_AddInspectionRecord
    @VehicleID = 7,
    @StationID = 27,
    @InspectorID = 17,
    @InspectionDate = '2025-02-05 11:00:00',
    @Result = 'Pass',
    @CO2Emission = 149.35,
    @HCEmission = 0.95,
    @Comments = 'Passed inspection. Recommend air filter replacement for optimal performance.',
    @AppointmentID = 8;

-- Re-inspections after repairs
EXEC sp_AddInspectionRecord
    @VehicleID = 3,
    @StationID = 23,
    @InspectorID = 13,
    @InspectionDate = '2025-01-24 09:30:00',
    @Result = 'Pass',
    @CO2Emission = 145.30,
    @HCEmission = 0.89,
    @Comments = 'Re-inspection after repairs. Catalytic converter replaced. Now within limits.',
    @AppointmentID = 6;

-- Testing status inspections
EXEC sp_AddInspectionRecord
    @VehicleID = 2,
    @StationID = 22,
    @InspectorID = 11,
    @InspectionDate = '2025-02-28 09:15:00',
    @Result = 'Testing',
    @CO2Emission = 140.25,
    @HCEmission = 0.85,
    @Comments = 'Initial testing phase. Vehicle undergoing extended emission analysis.',
    @AppointmentID = 25;

EXEC sp_AddInspectionRecord
    @VehicleID = 4,
    @StationID = 24,
    @InspectorID = 13,
    @InspectionDate = '2025-02-28 10:30:00',
    @Result = 'Testing',
    @CO2Emission = 146.80,
    @HCEmission = 0.92,
    @Comments = 'Testing after engine modifications. Additional diagnostics in progress.',
    @AppointmentID = 26;