USE [QuanLiKhiThai]
GO


-- SQL script to insert dummy data into Users table
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

-- SQL script to insert dummy data into Vehicles table
INSERT INTO Vehicles (OwnerID, PlateNumber, Brand, Model, ManufactureYear, EngineNumber)
VALUES 
    (8, 'ABC-1234', 'Toyota', 'Camry', 2022, 'ENG-TOY-22-78945612'),
    (9, 'XYZ-5678', 'Honda', 'Civic', 2021, 'ENG-HON-21-32165498'),
    (10, 'DEF-9012', 'Ford', 'F-150', 2023, 'ENG-FRD-23-45612378'),
    (11, 'GHI-3456', 'Chevrolet', 'Malibu', 2020, 'ENG-CHV-20-98765432'),
    (12, 'JKL-7890', 'Nissan', 'Altima', 2022, 'ENG-NSN-22-12378945'),
    (13, 'MNO-1357', 'Hyundai', 'Sonata', 2021, 'ENG-HYD-21-65432198'),
    (14, 'PQR-2468', 'Kia', 'Optima', 2023, 'ENG-KIA-23-78912345'),
    (15, 'STU-3690', 'Volkswagen', 'Jetta', 2020, 'ENG-VLK-20-36925814'),
    (16, 'VWX-4712', 'BMW', '3 Series', 2022, 'ENG-BMW-22-15948726'),
    (17, 'YZA-5824', 'Mercedes-Benz', 'C-Class', 2021, 'ENG-MRC-21-95175386');


-- SQL script to insert dummy data into InspectionStations table
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
GO


