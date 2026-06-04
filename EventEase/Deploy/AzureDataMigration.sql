-- EventEase Elite - Data Migration Script (LocalDB -> Azure SQL)
-- Author: Joshua Marc Lourens
-- Run AFTER `dotnet ef database update` has created the schema + seeded EventType (IDs 1-8).
-- IDENTITY_INSERT preserves the original keys so the FK links (Event->Venue, Booking->Venue/Event) stay valid.
-- Venue image URLs are left NULL on purpose; the two images are re-uploaded through the live
-- app after go-live, so the blobs are recreated in Azure Storage (not the dead Azurite URLs).

SET NOCOUNT ON;

-- Venues
SET IDENTITY_INSERT dbo.Venue ON;
INSERT INTO dbo.Venue (VenueID, VenueName, VenueLocation, VenueCapacity, ImageUrl) VALUES
    (7, N'Emeris University',   N'Westville, South Africa', 2000, NULL),
    (8, N'uShaka Marine World', N'Durban North',             500, NULL);
SET IDENTITY_INSERT dbo.Venue OFF;

-- Events  (EventTypeID NULL - the event was never classified locally)
SET IDENTITY_INSERT dbo.Event ON;
INSERT INTO dbo.Event (EventID, EventName, EventDescription, EventStartDate, EventEndDate, VenueID, EventTypeID) VALUES
    (5, N'Water Day', N'Water day at uShaka', '2026-05-08', '2026-05-08', 8, NULL);
SET IDENTITY_INSERT dbo.Event OFF;

-- Bookings
SET IDENTITY_INSERT dbo.Booking ON;
INSERT INTO dbo.Booking (BookingID, VenueID, EventID, BookingStartDate, BookingEndDate, BookingStatus) VALUES
    (17, 7, 5, '2026-05-08', '2026-05-08', NULL),
    (19, 7, 5, '2026-06-04', '2026-06-10', NULL);
SET IDENTITY_INSERT dbo.Booking OFF;

-- Verify row counts
SELECT 'Venue' AS [Table], COUNT(*) AS [Rows] FROM dbo.Venue
UNION ALL SELECT 'Event',     COUNT(*) FROM dbo.Event
UNION ALL SELECT 'Booking',   COUNT(*) FROM dbo.Booking
UNION ALL SELECT 'EventType', COUNT(*) FROM dbo.EventType;
