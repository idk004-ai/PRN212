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
    ('Richard Anderson', 'richard.anderson@stationmanager.com', 'hashed_password_station1', 'Station', '(555) 111-2233', '123 Station Plaza, Suite 101, Central City, NY 10001'),
    ('Patricia Williams', 'patricia.williams@stationmanager.com', 'hashed_password_station2', 'Station', '(555) 222-3344', '456 Inspection Boulevard, Office 205, Metro Heights, CA 94103'),
    ('Thomas Johnson', 'thomas.johnson@stationmanager.com', 'hashed_password_station3', 'Station', '(555) 333-4455', '789 Certification Way, Building B, Quality Town, TX 75001'),
    ('Jessica Martinez', 'jessica.martinez@stationmanager.com', 'hashed_password_station4', 'Station', '(555) 444-5566', '101 Testing Road, Floor 3, Safety City, FL 33130'),
    ('William Thompson', 'william.thompson@stationmanager.com', 'hashed_password_station5', 'Station', '(555) 555-6677', '202 Standards Avenue, Suite 300, Compliance Valley, IL 60601'),
    ('Nancy Garcia', 'nancy.garcia@stationmanager.com', 'hashed_password_station6', 'Station', '(555) 666-7788', '303 Regulation Street, Unit 15C, Inspection Heights, WA 98101'),
    ('Steven Rodriguez', 'steven.rodriguez@stationmanager.com', 'hashed_password_station7', 'Station', '(555) 777-8899', '404 Audit Lane, Building D, Assessment Park, CO 80202'),
    ('Lauren Wilson', 'lauren.wilson@stationmanager.com', 'hashed_password_station8', 'Station', '(555) 888-9900', '505 Verification Court, Office 405, Standard City, MI 48226'),
    ('Michael Brown', 'michael.brown@stationmanager.com', 'hashed_password_station9', 'Station', '(555) 999-0011', '606 Quality Control Drive, Suite 250, Protocol Falls, GA 30303'),
    ('Amanda Lee', 'amanda.lee@stationmanager.com', 'hashed_password_station10', 'Station', '(555) 000-1122', '707 Certification Avenue, Floor 5, Evaluation Valley, PA 19103');

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

--DELETE FROM InspectionStations
--DBCC CHECKIDENT ('InspectionStations', RESEED, 0)

-- SQL script to insert dummy data into InspectionStations table
/*
INSERT INTO InspectionStations (Name, Address, Phone, Email)
VALUES 
    ('Central Vehicle Inspection', '123 Main Street, Downtown, City Center 10001', '(555) 123-4567', 'info@centralinspection.com'),
    ('Highway Safety Center', '456 Boulevard Ave, West District, Metro City 20002', '(555) 234-5678', 'contact@highwaysafety.org'),
    ('Quality Auto Inspection', '789 Industrial Road, North Zone, Tech City 30003', '(555) 345-6789', 'service@qualityauto.net'),
    ('Express Vehicle Check', '321 Fast Lane, East Side, Speed Town 40004', '(555) 456-7890', 'appointments@expresscheck.com'),
    ('Reliable Inspection Services', '654 Trust Street, South Quarter, Faith City 50005', '(555) 567-8901', 'info@reliableinspect.com'),
    ('Metro Vehicle Testing', '987 Urban Drive, Downtown, Capital City 60006', '(555) 678-9012', 'support@metrotesting.org'),
    ('Countryside Inspection Station', '246 Rural Route, Green Valley, Nature County 70007', '(555) 789-0123', 'hello@countrysideinspect.com'),
    ('TechSafe Inspection Center', '135 Innovation Park, Tech District, Future City 80008', '(555) 890-1234', 'service@techsafeinspect.net'),
    ('Harbor Vehicle Inspection', '579 Port Road, Waterfront, Ocean City 90009', '(555) 901-2345', 'appointments@harborinspect.com'),
    ('Mountain View Auto Check', '864 Summit Way, Highland District, Peak Town 10010', '(555) 012-3456', 'info@mountainviewcheck.org');
*/
--DELETE FROM InspectionRecords
--DBCC CHECKIDENT ('InspectionRecords', RESEED, 0)

-- SQL script to insert dummy data into InspectionRecords table
/*
INSERT INTO InspectionRecords (VehicleID, StationID, InspectorID, InspectionDate, Result, CO2Emission, HCEmission, Comments)
VALUES 
    -- January 2025 inspections
    (1, 1, 11, '2025-01-05 09:30:00', 'Pass', 142.35, 0.82, 'Vehicle in excellent condition. All emissions within acceptable range.'),
    (2, 2, 12, '2025-01-07 14:15:00', 'Pass', 138.75, 0.91, 'Minor exhaust system wear but emissions acceptable.'),
    (3, 3, 13, '2025-01-10 11:45:00', 'Fail', 198.62, 1.35, 'Excessive CO2 and HC emissions. Recommend catalytic converter replacement.'),
    (4, 4, 14, '2025-01-12 10:00:00', 'Pass', 145.20, 0.88, 'Vehicle passed with emissions slightly above average. Recommend tune-up soon.'),
    (5, 5, 15, '2025-01-15 13:30:00', 'Pass', 132.45, 0.76, 'Excellent emission readings. Vehicle well maintained.'),
    
    -- February 2025 inspections
    (6, 6, 16, '2025-02-03 09:15:00', 'Fail', 212.78, 1.42, 'Failed emission test. Fuel system needs maintenance. Exhaust leak detected.'),
    (7, 7, 17, '2025-02-05 11:00:00', 'Pass', 149.35, 0.95, 'Passed inspection. Recommend air filter replacement for optimal performance.'),
    (8, 8, 18, '2025-02-08 14:30:00', 'Pass', 140.82, 0.87, 'Vehicle in good condition. All systems functioning properly.'),
    (9, 9, 19, '2025-02-12 10:45:00', 'Pass', 135.65, 0.79, 'Premium vehicle with excellent emission control systems.'),
    (10, 10, 20, '2025-02-15 13:00:00', 'Fail', 205.48, 1.38, 'Emission control system malfunction. Check engine light on. Needs service.'),
    
    -- Re-inspections after repairs
    (3, 3, 13, '2025-01-24 09:30:00', 'Pass', 145.30, 0.89, 'Re-inspection after repairs. Catalytic converter replaced. Now within limits.'),
    (6, 6, 16, '2025-02-17 14:15:00', 'Pass', 147.85, 0.93, 'Re-inspection after fuel system repair. Exhaust leak fixed. Now passing.'),
    (10, 10, 11, '2025-02-26 11:30:00', 'Pass', 149.42, 0.94, 'Re-inspection successful after emission control system repairs.'),
    
    -- Recent inspections
    (1, 5, 12, '2025-02-20 10:00:00', 'Pass', 141.75, 0.81, 'Annual inspection complete. Vehicle maintains excellent condition.'),
    (2, 4, 14, '2025-02-22 13:45:00', 'Pass', 139.20, 0.86, 'Vehicle passed with minor suggestions for future maintenance.'),
    (4, 3, 15, '2025-02-24 09:15:00', 'Pass', 144.65, 0.87, 'All systems in good working order. Emissions stable compared to previous test.'),
    (5, 2, 17, '2025-02-25 15:30:00', 'Pass', 133.10, 0.77, 'Vehicle continues to perform well. No issues detected.'),
    (7, 1, 18, '2025-02-27 08:00:00', 'Pass', 148.90, 0.94, 'Regular maintenance keeping vehicle in good condition. Passed all tests.'),
    (8, 8, 19, '2025-02-27 10:15:00', 'Pass', 141.25, 0.86, 'Vehicle passed inspection with no significant concerns.'),
    (9, 7, 20, '2025-02-27 13:30:00', 'Pass', 136.10, 0.80, 'Luxury vehicle with excellent emission performance. All systems optimal.'),
    
    -- Today's inspections (current timestamp date)
    (1, 1, 13, '2025-02-27 07:00:00', 'Pass', 141.50, 0.81, 'Follow-up inspection after maintenance. All systems optimal.'),
    (3, 3, 15, '2025-02-27 07:30:00', 'Pass', 144.85, 0.88, 'Continued good performance after earlier repair.'),
    (5, 5, 17, '2025-02-27 07:35:00', 'Pass', 132.20, 0.75, 'Vehicle maintained in excellent condition.'),
    (7, 7, 19, '2025-02-27 07:35:15', 'Pass', 148.75, 0.93, 'Recent inspection confirms continued compliance with emission standards.'),

    -- Inspections with "Testing" status from today
    (2, 2, 11, '2025-02-28 09:15:00', 'Testing', 140.25, 0.85, 'Initial testing phase. Vehicle undergoing extended emission analysis.'),
    (4, 4, 13, '2025-02-28 10:30:00', 'Testing', 146.80, 0.92, 'Testing after engine modifications. Additional diagnostics in progress.'),
    (6, 6, 15, '2025-02-28 11:45:00', 'Testing', 150.40, 0.98, 'Post-repair testing with new catalytic converter. Extended monitoring required.'),
    
    -- Testing process for vehicles with previous failures
    (10, 3, 17, '2025-02-28 13:20:00', 'Testing', 175.30, 1.10, 'Intermediate testing after partial repairs. Emission levels improving but monitoring continues.'),
    
    -- Advanced testing for research purposes
    (8, 10, 19, '2025-02-28 14:45:00', 'Testing', 142.60, 0.88, 'Special testing protocol for research on emission patterns in this vehicle model.'),
    (9, 9, 20, '2025-02-28 15:30:00', 'Testing', 137.75, 0.82, 'Extended testing cycle to gather data for manufacturer specifications.'),
    
    -- Scheduled for completion later today
    (3, 5, 12, '2025-02-28 16:00:00', 'Testing', 147.50, 0.90, 'Mid-testing evaluation. Initial readings promising but test still in progress.'),
    (5, 7, 14, '2025-02-28 16:30:00', 'Testing', 135.90, 0.79, 'Comprehensive testing with additional sensors. Final results pending.'),
    
    -- Testing new inspection equipment
    (1, 8, 16, '2025-02-28 17:00:00', 'Testing', 143.15, 0.84, 'Using new calibration equipment. Test extended for verification purposes.'),
    (7, 2, 18, '2025-02-28 17:45:00', 'Testing', 149.60, 0.95, 'Testing with both standard and new equipment for comparison study.')
	*/

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
    @Comments = 'Vehicle in excellent condition. All emissions within acceptable range.';

EXEC sp_AddInspectionRecord
    @VehicleID = 2,
    @StationID = 22,
    @InspectorID = 12,
    @InspectionDate = '2025-01-07 14:15:00',
    @Result = 'Pass',
    @CO2Emission = 138.75,
    @HCEmission = 0.91,
    @Comments = 'Minor exhaust system wear but emissions acceptable.';

EXEC sp_AddInspectionRecord
    @VehicleID = 3,
    @StationID = 23,
    @InspectorID = 13,
    @InspectionDate = '2025-01-10 11:45:00',
    @Result = 'Fail',
    @CO2Emission = 198.62,
    @HCEmission = 1.35,
    @Comments = 'Excessive CO2 and HC emissions. Recommend catalytic converter replacement.';

EXEC sp_AddInspectionRecord
    @VehicleID = 4,
    @StationID = 24,
    @InspectorID = 14,
    @InspectionDate = '2025-01-12 10:00:00',
    @Result = 'Pass',
    @CO2Emission = 145.20,
    @HCEmission = 0.88,
    @Comments = 'Vehicle passed with emissions slightly above average. Recommend tune-up soon.';

EXEC sp_AddInspectionRecord
    @VehicleID = 5,
    @StationID = 25,
    @InspectorID = 15,
    @InspectionDate = '2025-01-15 13:30:00',
    @Result = 'Pass',
    @CO2Emission = 132.45,
    @HCEmission = 0.76,
    @Comments = 'Excellent emission readings. Vehicle well maintained.';

-- February 2025 inspections
EXEC sp_AddInspectionRecord
    @VehicleID = 6,
    @StationID = 26,
    @InspectorID = 16,
    @InspectionDate = '2025-02-03 09:15:00',
    @Result = 'Fail',
    @CO2Emission = 212.78,
    @HCEmission = 1.42,
    @Comments = 'Failed emission test. Fuel system needs maintenance. Exhaust leak detected.';

EXEC sp_AddInspectionRecord
    @VehicleID = 7,
    @StationID = 27,
    @InspectorID = 17,
    @InspectionDate = '2025-02-05 11:00:00',
    @Result = 'Pass',
    @CO2Emission = 149.35,
    @HCEmission = 0.95,
    @Comments = 'Passed inspection. Recommend air filter replacement for optimal performance.';

EXEC sp_AddInspectionRecord
    @VehicleID = 8,
    @StationID = 28,
    @InspectorID = 18,
    @InspectionDate = '2025-02-08 14:30:00',
    @Result = 'Pass',
    @CO2Emission = 140.82,
    @HCEmission = 0.87,
    @Comments = 'Vehicle in good condition. All systems functioning properly.';

EXEC sp_AddInspectionRecord
    @VehicleID = 9,
    @StationID = 29,
    @InspectorID = 19,
    @InspectionDate = '2025-02-12 10:45:00',
    @Result = 'Pass',
    @CO2Emission = 135.65,
    @HCEmission = 0.79,
    @Comments = 'Premium vehicle with excellent emission control systems.';

EXEC sp_AddInspectionRecord
    @VehicleID = 10,
    @StationID = 30,
    @InspectorID = 20,
    @InspectionDate = '2025-02-15 13:00:00',
    @Result = 'Fail',
    @CO2Emission = 205.48,
    @HCEmission = 1.38,
    @Comments = 'Emission control system malfunction. Check engine light on. Needs service.';

-- Re-inspections after repairs
EXEC sp_AddInspectionRecord
    @VehicleID = 3,
    @StationID = 23,
    @InspectorID = 13,
    @InspectionDate = '2025-01-24 09:30:00',
    @Result = 'Pass',
    @CO2Emission = 145.30,
    @HCEmission = 0.89,
    @Comments = 'Re-inspection after repairs. Catalytic converter replaced. Now within limits.';

EXEC sp_AddInspectionRecord
    @VehicleID = 6,
    @StationID = 26,
    @InspectorID = 16,
    @InspectionDate = '2025-02-17 14:15:00',
    @Result = 'Pass',
    @CO2Emission = 147.85,
    @HCEmission = 0.93,
    @Comments = 'Re-inspection after fuel system repair. Exhaust leak fixed. Now passing.';

EXEC sp_AddInspectionRecord
    @VehicleID = 10,
    @StationID = 30,
    @InspectorID = 11,
    @InspectionDate = '2025-02-26 11:30:00',
    @Result = 'Pass',
    @CO2Emission = 149.42,
    @HCEmission = 0.94,
    @Comments = 'Re-inspection successful after emission control system repairs.';

-- Recent inspections
EXEC sp_AddInspectionRecord
    @VehicleID = 1,
    @StationID = 25,
    @InspectorID = 12,
    @InspectionDate = '2025-02-20 10:00:00',
    @Result = 'Pass',
    @CO2Emission = 141.75,
    @HCEmission = 0.81,
    @Comments = 'Annual inspection complete. Vehicle maintains excellent condition.';

EXEC sp_AddInspectionRecord
    @VehicleID = 2,
    @StationID = 24,
    @InspectorID = 14,
    @InspectionDate = '2025-02-22 13:45:00',
    @Result = 'Pass',
    @CO2Emission = 139.20,
    @HCEmission = 0.86,
    @Comments = 'Vehicle passed with minor suggestions for future maintenance.';

EXEC sp_AddInspectionRecord
    @VehicleID = 4,
    @StationID = 23,
    @InspectorID = 15,
    @InspectionDate = '2025-02-24 09:15:00',
    @Result = 'Pass',
    @CO2Emission = 144.65,
    @HCEmission = 0.87,
    @Comments = 'All systems in good working order. Emissions stable compared to previous test.';

EXEC sp_AddInspectionRecord
    @VehicleID = 5,
    @StationID = 22,
    @InspectorID = 17,
    @InspectionDate = '2025-02-25 15:30:00',
    @Result = 'Pass',
    @CO2Emission = 133.10,
    @HCEmission = 0.77,
    @Comments = 'Vehicle continues to perform well. No issues detected.';

EXEC sp_AddInspectionRecord
    @VehicleID = 7,
    @StationID = 21,
    @InspectorID = 18,
    @InspectionDate = '2025-02-27 08:00:00',
    @Result = 'Pass',
    @CO2Emission = 148.90,
    @HCEmission = 0.94,
    @Comments = 'Regular maintenance keeping vehicle in good condition. Passed all tests.';

EXEC sp_AddInspectionRecord
    @VehicleID = 8,
    @StationID = 28,
    @InspectorID = 19,
    @InspectionDate = '2025-02-27 10:15:00',
    @Result = 'Pass',
    @CO2Emission = 141.25,
    @HCEmission = 0.86,
    @Comments = 'Vehicle passed inspection with no significant concerns.';

EXEC sp_AddInspectionRecord
    @VehicleID = 9,
    @StationID = 27,
    @InspectorID = 20,
    @InspectionDate = '2025-02-27 13:30:00',
    @Result = 'Pass',
    @CO2Emission = 136.10,
    @HCEmission = 0.80,
    @Comments = 'Luxury vehicle with excellent emission performance. All systems optimal.';

-- Today's inspections
EXEC sp_AddInspectionRecord
    @VehicleID = 1,
    @StationID = 21,
    @InspectorID = 13,
    @InspectionDate = '2025-02-27 07:00:00',
    @Result = 'Pass',
    @CO2Emission = 141.50,
    @HCEmission = 0.81,
    @Comments = 'Follow-up inspection after maintenance. All systems optimal.';

EXEC sp_AddInspectionRecord
    @VehicleID = 3,
    @StationID = 23,
    @InspectorID = 15,
    @InspectionDate = '2025-02-27 07:30:00',
    @Result = 'Pass',
    @CO2Emission = 144.85,
    @HCEmission = 0.88,
    @Comments = 'Continued good performance after earlier repair.';

EXEC sp_AddInspectionRecord
    @VehicleID = 5,
    @StationID = 25,
    @InspectorID = 17,
    @InspectionDate = '2025-02-27 07:35:00',
    @Result = 'Pass',
    @CO2Emission = 132.20,
    @HCEmission = 0.75,
    @Comments = 'Vehicle maintained in excellent condition.';

EXEC sp_AddInspectionRecord
    @VehicleID = 7,
    @StationID = 27,
    @InspectorID = 19,
    @InspectionDate = '2025-02-27 07:35:15',
    @Result = 'Pass',
    @CO2Emission = 148.75,
    @HCEmission = 0.93,
    @Comments = 'Recent inspection confirms continued compliance with emission standards.';

-- Inspections with "Testing" status from today
EXEC sp_AddInspectionRecord
    @VehicleID = 2,
    @StationID = 22,
    @InspectorID = 11,
    @InspectionDate = '2025-02-28 09:15:00',
    @Result = 'Testing',
    @CO2Emission = 140.25,
    @HCEmission = 0.85,
    @Comments = 'Initial testing phase. Vehicle undergoing extended emission analysis.';

EXEC sp_AddInspectionRecord
    @VehicleID = 4,
    @StationID = 24,
    @InspectorID = 13,
    @InspectionDate = '2025-02-28 10:30:00',
    @Result = 'Testing',
    @CO2Emission = 146.80,
    @HCEmission = 0.92,
    @Comments = 'Testing after engine modifications. Additional diagnostics in progress.';

EXEC sp_AddInspectionRecord
    @VehicleID = 6,
    @StationID = 26,
    @InspectorID = 15,
    @InspectionDate = '2025-02-28 11:45:00',
    @Result = 'Testing',
    @CO2Emission = 150.40,
    @HCEmission = 0.98,
    @Comments = 'Post-repair testing with new catalytic converter. Extended monitoring required.';

-- Testing process for vehicles with previous failures
EXEC sp_AddInspectionRecord
    @VehicleID = 10,
    @StationID = 23,
    @InspectorID = 17,
    @InspectionDate = '2025-02-28 13:20:00',
    @Result = 'Testing',
    @CO2Emission = 175.30,
    @HCEmission = 1.10,
    @Comments = 'Intermediate testing after partial repairs. Emission levels improving but monitoring continues.';

-- Advanced testing for research purposes
EXEC sp_AddInspectionRecord
    @VehicleID = 8,
    @StationID = 30,
    @InspectorID = 19,
    @InspectionDate = '2025-02-28 14:45:00',
    @Result = 'Testing',
    @CO2Emission = 142.60,
    @HCEmission = 0.88,
    @Comments = 'Special testing protocol for research on emission patterns in this vehicle model.';

EXEC sp_AddInspectionRecord
    @VehicleID = 9,
    @StationID = 29,
    @InspectorID = 20,
    @InspectionDate = '2025-02-28 15:30:00',
    @Result = 'Testing',
    @CO2Emission = 137.75,
    @HCEmission = 0.82,
    @Comments = 'Extended testing cycle to gather data for manufacturer specifications.';

-- Scheduled for completion later today
EXEC sp_AddInspectionRecord
    @VehicleID = 3,
    @StationID = 25,
    @InspectorID = 12,
    @InspectionDate = '2025-02-28 16:00:00',
    @Result = 'Testing',
    @CO2Emission = 147.50,
    @HCEmission = 0.90,
    @Comments = 'Mid-testing evaluation. Initial readings promising but test still in progress.';

EXEC sp_AddInspectionRecord
    @VehicleID = 5,
    @StationID = 27,
    @InspectorID = 14,
    @InspectionDate = '2025-02-28 16:30:00',
    @Result = 'Testing',
    @CO2Emission = 135.90,
    @HCEmission = 0.79,
    @Comments = 'Comprehensive testing with additional sensors. Final results pending.';

-- Testing new inspection equipment
EXEC sp_AddInspectionRecord
    @VehicleID = 1,
    @StationID = 28,
    @InspectorID = 16,
    @InspectionDate = '2025-02-28 17:00:00',
    @Result = 'Testing',
    @CO2Emission = 143.15,
    @HCEmission = 0.84,
    @Comments = 'Using new calibration equipment. Test extended for verification purposes.';
