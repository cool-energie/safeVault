SELECT UserID, Username, Email
FROM Users
WHERE (Username = @Identifier OR Email = @Identifier)
LIMIT 1;

SELECT UserID, Username, Email
FROM Users
WHERE Username LIKE CONCAT('%', @SearchTerm, '%')
LIMIT 50;
