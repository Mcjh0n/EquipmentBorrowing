-- UC-01: complete equipment catalog
SELECT "EquipmentIdentifier", "Name", "Description", "IsAvailable"
FROM "Equipment"
ORDER BY "Name", "Id";

-- UC-02: available physical items
SELECT "Id", "EquipmentIdentifier", "Name", "Description"
FROM "Equipment"
WHERE "IsAvailable" = TRUE
ORDER BY "Name", "Id";

-- UC-04 / UC-09: active loans with related identities
SELECT b."Id", s."StudentNumber", s."Name" AS "Student",
       e."EquipmentIdentifier", e."Name" AS "Equipment",
       b."DateBorrowed", b."ExpectedReturnDate", b."Status"
FROM "Borrowings" AS b
JOIN "Students" AS s ON s."Id" = b."StudentId"
JOIN "Equipment" AS e ON e."Id" = b."EquipmentId"
WHERE b."Status" = 0
ORDER BY b."ExpectedReturnDate", b."Id";

-- UC-07: student borrowing history
SELECT b."Id", e."EquipmentIdentifier", e."Name", b."DateBorrowed",
       b."ExpectedReturnDate", b."ReturnedDate", b."Status"
FROM "Borrowings" AS b
JOIN "Equipment" AS e ON e."Id" = b."EquipmentId"
WHERE b."StudentId" = 1
ORDER BY b."DateBorrowed" DESC, b."Id";

-- UC-08: equipment borrowing history
SELECT b."Id", s."StudentNumber", s."Name", b."DateBorrowed",
       b."ExpectedReturnDate", b."ReturnedDate", b."Status"
FROM "Borrowings" AS b
JOIN "Students" AS s ON s."Id" = b."StudentId"
WHERE b."EquipmentId" = 1
ORDER BY b."DateBorrowed" DESC, b."Id";

-- Active item loans per student
SELECT s."Id", s."StudentNumber", s."Name", COUNT(b."Id") AS "ActiveItems"
FROM "Students" AS s
LEFT JOIN "Borrowings" AS b ON b."StudentId" = s."Id" AND b."Status" = 0
GROUP BY s."Id", s."StudentNumber", s."Name"
ORDER BY s."Name", s."Id";
