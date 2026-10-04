-- 1. Retrieve all equipment.
SELECT Id, Name, IsAvailable
FROM Equipment;

-- 2. Retrieve currently available equipment.
SELECT Id, Name, IsAvailable
FROM Equipment
WHERE IsAvailable = 1
ORDER BY Name;

-- 3. Active borrowing records with student and equipment details.
-- BorrowingStatus.Active is stored as 0 by the EF Core enum conversion.
SELECT s.Name AS Student,
       e.Name AS Equipment,
       b.DateBorrowed,
       b.ExpectedReturnDate,
       b.Status
FROM Borrowings AS b
INNER JOIN Students AS s ON s.Id = b.StudentId
INNER JOIN Equipment AS e ON e.Id = b.EquipmentId
WHERE b.Status = 0
ORDER BY b.ExpectedReturnDate;

-- 4. Count active borrowings for each student.
SELECT s.Id,
       s.Name,
       COUNT(b.Id) AS ActiveBorrowings
FROM Students AS s
LEFT JOIN Borrowings AS b
    ON b.StudentId = s.Id AND b.Status = 0
GROUP BY s.Id, s.Name
ORDER BY s.Name;

-- 5. Example update: mark a student as eligible to borrow.
UPDATE Students
SET IsAllowedToBorrow = 1
WHERE Id = 3;
