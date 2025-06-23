-- Step 1: Create the database
CREATE DATABASE EventEDB;
GO

USE EventEDB;
GO

-- Step 2: Use the new database
USE EventEDB;
GO

-- Drop tables in order to avoid FK conflicts (if they exist)
IF OBJECT_ID('Booking', 'U') IS NOT NULL DROP TABLE Booking;
IF OBJECT_ID('Event', 'U') IS NOT NULL DROP TABLE Event;
IF OBJECT_ID('EventType', 'U') IS NOT NULL DROP TABLE EventType;
IF OBJECT_ID('Venue', 'U') IS NOT NULL DROP TABLE Venue;
GO


-- Step 3: Create Venue table
CREATE TABLE Venue (
    VenueID INT PRIMARY KEY IDENTITY(1,1),
    VenueName NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Capacity INT NOT NULL,
    ImageURL NVARCHAR(MAX) NULL
);
GO

-- Insert Venue data
INSERT INTO Venue (VenueName, Location, Capacity, ImageURL)
VALUES ('Shepstone Gardens', 'Johannesburg', 250, 'https://www.wheresmywedding.co.za/blog/gauteng-wedding-venue-shepstone-gardens');
GO

SELECT * FROM Venue

CREATE TABLE EventType(
	EventTypeID INT IDENTITY(1,1) PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL
);

INSERT INTO EventType (Name)
VALUES
('Conference'),
('Wedding'),
('Naming'),
('Birthday'),
('Concert');

SELECT * FROM EventType

-- Step 5: Create Event table
CREATE TABLE Event (
    EventID INT PRIMARY KEY IDENTITY(1,1),
    EventName NVARCHAR(100) NOT NULL,
    Date DATETIME NOT NULL,
    Description NVARCHAR(MAX) NULL,
    ImageURL NVARCHAR(MAX) NULL,
    VenueID INT NULL,
    EventTypeID INT NULL,
    FOREIGN KEY (VenueID) REFERENCES Venue(VenueID) ON DELETE SET NULL,
    FOREIGN KEY (EventTypeID) REFERENCES EventType(EventTypeID) ON DELETE SET NULL
);
GO


-- Insert Event data
INSERT INTO Event (EventName, Date, Description, ImageURL, VenueID, EventTypeID)
VALUES (
    'Balcony Mix Africa', 
    '2025-02-26 13:09', 
    'Vibrant Music Festival', 
    'https://howler-production.s3.eu-west-1.amazonaws.com/uploads/organiser/organiser_logo/10196/Logos_The_Balcony_Mix_Africa_colored_black.png', 
    1,  -- VenueID (Shepstone Gardens)
    5   -- EventTypeID (Concert)
);
GO

SELECT * FROM Event


-- Step 6: Create Booking table
CREATE TABLE Booking (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    BookingDate DATETIME NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    CustomerEmail NVARCHAR(100) NOT NULL,
    NumberOfGuests INT NOT NULL,
    EventID INT NOT NULL,
    VenueID INT NOT NULL,
    FOREIGN KEY (EventID) REFERENCES Event(EventID) ON DELETE CASCADE,
    FOREIGN KEY (VenueID) REFERENCES Venue(VenueID)
);
GO

-- Insert Booking data
INSERT INTO Booking (BookingDate, CustomerName, CustomerEmail, NumberOfGuests, EventID, VenueID)
VALUES ('2025-07-01 14:30', 'Lily Bloom', 'lilybloom@gmail.com', 25, 1, 1);
GO


SELECT * FROM Booking

	ALTER TABLE Booking
ADD CONSTRAINT FK_Booking_EventID FOREIGN KEY (EventID) REFERENCES Event(EventID) ON DELETE CASCADE;

INSERT INTO Booking (BookingDate, CustomerName, CustomerEmail, NumberOfGuests, EventID, VenueID)
VALUES ('2025-07-01 14:30', 'Lily Bloom', 'lilybloom@gmail.com', 25, 1, 1);

SELECT * 
