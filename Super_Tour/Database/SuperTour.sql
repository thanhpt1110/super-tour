CREATE DATABASE Super_Tour;
USE  Super_Tour;
-- Create Table
CREATE TABLE ACCOUNT
(
	Id_Account int auto_increment primary key,
    Username varchar(255) unique,
    Password varchar(1000),
	Account_Name nvarchar(255),
    Service nvarchar(255),
    Priority INT,
    Email varchar(255) unique
) auto_increment = 1000;
CREATE TABLE TYPE_PACKAGE(
	Id_Type_Package int auto_increment primary key,
    Name_Type nvarchar(255),
    Description nvarchar(255)
) AUTO_INCREMENT = 10000;
CREATE TABLE PACKAGE
( 
	Id_Package int auto_increment primary key,
	Name_Package nvarchar(255),
    Id_Type_Package int,
    Id_Province varchar(255),
    Id_District varchar(255),
	Image_Package text,
    Description_Package text,
	Price decimal
) auto_increment = 100000;
CREATE TABLE TOUR
(
	Id_Tour int auto_increment primary key  ,
    Name_Tour text,
    TotalDay int,
    TotalNight int,
    PlaceOfTour text,
    Status_Tour nvarchar(50)
) auto_increment = 100000;
CREATE TABLE TOUR_DETAILS(
	Id_TourDetails int auto_increment primary key,
    Id_Tour int,
    Id_Package int,
    Session nvarchar(255),
    Date_Order_Package int,
	Start_Time_Package time
) auto_increment = 100000;
CREATE TABLE TRAVEL(
	Id_Travel int auto_increment primary key,
    Id_Tour int,
    StartLocation nvarchar(255),
    StartDateTimeTravel datetime,
	RemainingTicket int,
    Discount int,
    MaxTicket int
) auto_increment=100000;
CREATE TABLE CUSTOMER(
	Id_Customer int auto_increment primary key,
    Name_Customer nvarchar(255),
    IdNumber nvarchar(15),
    PhoneNumber nvarchar(15),
    Address nvarchar(255)
) auto_increment=100000;
CREATE TABLE BOOKING(
	Id_Booking int auto_increment primary key,
    Id_Customer_Booking int,
    Id_Travel int,
    Booking_Date Date,
    Status varchar(50)
)auto_increment=100000;
CREATE TABLE TOURIST(
	Id_Tourist int auto_increment primary key,
    Id_Booking int,
    Name_Tourist nvarchar(255)
) auto_increment=100000;
    
CREATE TABLE TICKET(
	Id_Ticket int auto_increment primary key,
    Id_Tourist int,
    Status nvarchar(100)
) auto_increment=100000;


 -- FOREIGN KEY
 ALTER TABLE TOURIST ADD FOREIGN KEY(Id_Booking) references BOOKING(Id_booking);
 ALTER TABLE PACKAGE ADD FOREIGN KEY (Id_Type_Package) REFERENCES TYPE_PACKAGE(Id_Type_Package);
 ALTER TABLE TOUR_DETAILS ADD FOREIGN KEY(Id_Tour) references TOUR(Id_Tour);
 ALTER TABLE TOUR_DETAILS add foreign key(Id_Package) references PACKAGE(Id_Package);
 ALTER TABLE TRAVEL add foreign key(Id_Tour) references TOUR(Id_Tour);
 ALTER TABLE BOOKING add foreign key(Id_Customer_Booking) references CUSTOMER(Id_Customer);
 ALTER TABLE BOOKING add foreign key(Id_Travel) references TRAVEL(Id_Travel);
 ALTER TABLE TICKET add foreign key(Id_Tourist) references TOURIST(Id_Tourist);

-- ADD DEFAULT ACCOUNT
INSERT INTO ACCOUNT(Username,Password,Account_Name,Service,Priority,Email) VALUES ('admin', '81dc9bdb52d04dc20036dbd8313ed055', 'Trùm cuối', 'Admin', '1', 'trumcuoi@gmail.com');
