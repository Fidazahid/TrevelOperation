-- Add CreatedByEmail and CreatedByName columns to Notifications table
-- Run this script on your SQLite database

-- Add CreatedByEmail column
ALTER TABLE Notifications ADD COLUMN CreatedByEmail TEXT;

-- Add CreatedByName column
ALTER TABLE Notifications ADD COLUMN CreatedByName TEXT;

-- Verify the columns were added
PRAGMA table_info(Notifications);

-- Success message
SELECT 'Notifications table updated successfully. CreatedByEmail and CreatedByName columns added.' AS Result;
