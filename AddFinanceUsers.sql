-- Add Finance team members to Employees table
INSERT OR IGNORE INTO Employees (EmployeeId, Email, FirstName, LastName, Department, IsActive, CostCenter, CreatedAt)
VALUES 
('FIN001', 'martina.popinsk@wsc.com', 'Martina', 'Popinsk', 'Finance', 1, 'FIN-001', datetime('now')),
('FIN002', 'maayan.chesler@wsc.com', 'Maayan', 'Chesler', 'Finance', 1, 'FIN-002', datetime('now'));

-- Remove or deactivate old finance@company.com if it exists
UPDATE Employees SET IsActive = 0 WHERE Email = 'finance@company.com';

-- Verify
SELECT Email, Department, IsActive FROM Employees WHERE Department = 'Finance';
