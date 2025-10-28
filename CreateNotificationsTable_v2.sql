-- Create Notifications table for the Travel Expense Management System
-- This script can be executed manually if migrations fail

CREATE TABLE IF NOT EXISTS "Notifications" (
    "NotificationId" INTEGER NOT NULL CONSTRAINT "PK_Notifications" PRIMARY KEY AUTOINCREMENT,
    "RecipientEmail" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "Priority" TEXT NOT NULL,
    "Title" TEXT NOT NULL,
    "Message" TEXT NOT NULL,
    "ActionUrl" TEXT NULL,
    "ActionLabel" TEXT NULL,
    "RelatedEntityType" TEXT NULL,
    "RelatedEntityId" TEXT NULL,
    "Icon" TEXT NULL,
    "IsRead" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TEXT NOT NULL,
    "ReadAt" TEXT NULL,
    "ExpiresAt" TEXT NULL,
    "Metadata" TEXT NULL
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS "IX_Notifications_RecipientEmail_IsRead" ON "Notifications" ("RecipientEmail", "IsRead");
CREATE INDEX IF NOT EXISTS "IX_Notifications_CreatedAt" ON "Notifications" ("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Notifications_ExpiresAt" ON "Notifications" ("ExpiresAt");

-- Verify table was created
SELECT 'Notifications table created successfully!' as Result;
SELECT COUNT(*) as TableExists FROM sqlite_master WHERE type='table' AND name='Notifications';
