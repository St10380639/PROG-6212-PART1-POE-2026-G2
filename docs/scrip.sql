Create database RaceDay;

use RaceDay;

CREATE TABLE Organiser(
Organiser_ID int identity(1,1) primary key,
Organiser_Name varchar(50) not null,
Organiser_Email varchar(50) not null
);

CREATE TABLE Events(
Events_ID int identity(1001,1) primary key,
Events_Name varchar(50) not null,
Event_Date varchar(50) not null
);

CREATE TABLE Category(
Category_ID int identity(2001,1) primary key,
Category_Name varchar(50) not null,
Category_Desciption varchar(50) not null
);

CREATE TABLE Enrolments(
Enrolments_ID int identity(3001,1) primary key,
Enrolments_Day varchar(50) not null,
Enrolments_Time char(7) not null
);

CREATE TABLE Participants(
Participants_ID int identity(3001,1) primary key,
Participants_Name varchar(50) not null,
Participants_Surname varchar(50) not null
);

CREATE TABLE results(
Result_ID int identity(4001,1) primary key,
Participants_ID int,
Events_ID int,
Category_ID int,
Enrolments_ID int,

FOREIGN KEY(Participants_ID) REFERENCES Participants(Participants_ID),
FOREIGN KEY(Enrolments_ID) REFERENCES Enrolments(Enrolments_ID),
FOREIGN KEY(Events_ID) REFERENCES Events(Events_ID),
FOREIGN KEY(Category_ID) REFERENCES Category(Category_ID)
);

INSERT INTO Organiser(Organiser_Name, Organiser_Email)
VALUES
('Kulan', 'kulan@gmail.com'),
('Temen','temen@gmail.com'),
('Lebadeleda','Lebadeleda@gmail.com');

INSERT INTO Participants(Participants_Name, Participants_Surname)
VALUES
('Kuman','Zam'),
('Libano','Rexama'),
('Pumeni','Weza');

INSERT INTO Events(Events_Name,Event_Date)
VALUES
('Cycle Mzansi', '4 December 2026'),
('Santon Run', '3O October 2026'),
('CapeTown Walk','17 November 2026');

INSERT INTO Category(Category_Name, Category_Desciption)
VALUES
('Cycle','Bicycle or montain biking'),
('Walk','Walking around a comunity or city'),
('Run','Running short or long maratons');


INSERT INTO Enrolments(Enrolments_Day, Enrolments_Time)
VALUES
('4 December 2026','05:00'),
('3O October 2026','06:30'),
('17 November 2026','08:00');

SELECT * FROM Organiser;
SELECT * FROM Participants;
SELECT * FROM Events;
SELECT * FROM Category;
SELECT * FROM Enrolments;
SELECT * FROM results;


































































































































































































































































































































