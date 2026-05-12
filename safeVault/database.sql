-- database.sql
CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(100),
    Email VARCHAR(100)
);

ALTER TABLE Users
ADD Password VARCHAR(255) NOT NULL;

ALTER TABLE Users
MODIFY Role ENUM('admin', 'user', 'manager') NOT NULL DEFAULT 'user';
