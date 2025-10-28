-- Create Notifications Table
-- Run this script on your SQLite database

CREATE TABLE IF NOT EXISTS "Notifications" (
    "NotificationId" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "RecipientEmail" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "Title" TEXT NOT NULL,
    "Message" TEXT NOT NULL,
    "ActionUrl" TEXT,
    "ActionLabel" TEXT,
    "RelatedEntityId" TEXT,
    "RelatedEntityType" TEXT,
    "Priority" TEXT NOT NULL,
    "IsRead" INTEGER NOT NULL DEFAULT 0,
    "ReadAt" TEXT,
    "EmailSent" INTEGER NOT NULL DEFAULT 0,
    "EmailSentAt" TEXT,
    "ExpiresAt" TEXT,
    "CreatedAt" TEXT NOT NULL,
    "Icon" TEXT
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS "IX_Notifications_RecipientEmail_IsRead" 
ON "Notifications" ("RecipientEmail", "IsRead");

CREATE INDEX IF NOT EXISTS "IX_Notifications_CreatedAt" 
ON "Notifications" ("CreatedAt");

CREATE INDEX IF NOT EXISTS "IX_Notifications_ExpiresAt" 
ON "Notifications" ("ExpiresAt");

-- Verify table was created
SELECT 'Notifications table created successfully!' as Result;

-- Show table structure
PRAGMA table_info(Notifications);
