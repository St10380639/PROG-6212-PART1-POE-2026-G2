--Code written by Claude AI (Anthropic, 2026).
IF DB_ID('RaceDayPOE2026') IS NOT NULL
BEGIN
    ALTER DATABASE RaceDayPOE2026 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RaceDayPOE2026;
END
GO
CREATE DATABASE RaceDayPOE2026;
GO
USE RaceDayPOE2026;
GO

--Code written by Claude AI (Anthropic, 2026).
-- 1. ROLE
CREATE TABLE Role (
    role_id   INT IDENTITY(1,1) PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL UNIQUE
);
GO
--Code written by Claude AI (Anthropic, 2026).
-- 2. USERS
CREATE TABLE Users (
    user_id       INT IDENTITY(1,1) PRIMARY KEY,
    role_id       INT NOT NULL,
    email         VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    first_name    VARCHAR(100),
    last_name     VARCHAR(100),
    phone_number  VARCHAR(20),
    created_at    DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    updated_at    DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Users_Role FOREIGN KEY (role_id) REFERENCES Role(role_id)
);
GO
--Code written by Claude AI (Anthropic, 2026).
-- Keeps updated_at current when a profile is edited
CREATE TRIGGER trg_Users_UpdatedAt
ON Users
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE u
    SET updated_at = SYSDATETIME()
    FROM Users u
    INNER JOIN inserted i ON u.user_id = i.user_id;
END;
GO
--Code written by Claude AI (Anthropic, 2026).
-- 3. EVENT
CREATE TABLE Event (
    event_id     INT IDENTITY(1,1) PRIMARY KEY,
    organiser_id INT NOT NULL,
    name         VARCHAR(150) NOT NULL,
    description  VARCHAR(MAX),
    event_date   DATE NOT NULL,
    location     VARCHAR(255) NOT NULL,
    distance     DECIMAL(5,2),
    event_type   VARCHAR(10) NOT NULL,
    CONSTRAINT FK_Event_Organiser FOREIGN KEY (organiser_id) REFERENCES Users(user_id),
    CONSTRAINT CK_Event_Type CHECK (event_type IN ('run','walk','cycle')),
    CONSTRAINT CK_Event_Distance CHECK (distance IS NULL OR distance > 0)
);
GO
--Code written by Claude AI (Anthropic, 2026).
-- 4. CATEGORY
CREATE TABLE Category (
    category_id   INT IDENTITY(1,1) PRIMARY KEY,
    event_id      INT NOT NULL,
    category_name VARCHAR(100) NOT NULL,
    category_type VARCHAR(10),
    CONSTRAINT UQ_Category_CategoryEvent UNIQUE (category_id, event_id),
    CONSTRAINT CK_Category_Type CHECK (category_type IS NULL OR category_type IN ('age','distance')),
    CONSTRAINT FK_Category_Event FOREIGN KEY (event_id)
        REFERENCES Event(event_id) ON DELETE CASCADE
);
GO
--Code written by Claude AI (Anthropic, 2026).
-- 5. ENROLMENT
CREATE TABLE Enrolment (
    enrolment_id   INT IDENTITY(1,1) PRIMARY KEY,
    participant_id INT NOT NULL,
    event_id       INT NOT NULL,
    category_id    INT NOT NULL,
    enrolment_date DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT UQ_Enrolment_ParticipantEvent UNIQUE (participant_id, event_id),
    CONSTRAINT FK_Enrolment_Participant FOREIGN KEY (participant_id)
        REFERENCES Users(user_id),
    CONSTRAINT FK_Enrolment_Event FOREIGN KEY (event_id)
        REFERENCES Event(event_id) ON DELETE CASCADE,
    -- Composite FK: category must belong to the same event.
    -- NO ACTION here avoids SQL Server's "multiple cascade paths" error.
    CONSTRAINT FK_Enrolment_Category FOREIGN KEY (category_id, event_id)
        REFERENCES Category(category_id, event_id)
);
GO
--Code written by Claude AI (Anthropic, 2026).
-- 6. RESULT
CREATE TABLE Result (
    result_id       INT IDENTITY(1,1) PRIMARY KEY,
    enrolment_id    INT NOT NULL UNIQUE,
    finish_time     TIME(0),
    finish_position INT,
    CONSTRAINT CK_Result_Position CHECK (finish_position IS NULL OR finish_position > 0),
    CONSTRAINT FK_Result_Enrolment FOREIGN KEY (enrolment_id)
        REFERENCES Enrolment(enrolment_id) ON DELETE CASCADE
);
GO
--Code written by Claude AI (Anthropic, 2026).
-- Seed the two roles
INSERT INTO Role (role_name) VALUES ('Organiser'), ('Participant');
GO
--Code written by Claude AI (Anthropic, 2026).
SELECT DB_NAME() AS CurrentDatabase;
SELECT name FROM sys.databases;
--Code written by Claude AI (Anthropic, 2026).
SELECT user_id, role_id, email, password_hash, created_at
FROM Users;