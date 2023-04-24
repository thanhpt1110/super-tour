CREATE DATABASE Super_Tour;
USE  Super_Tour;
-- Create Table
CREATE TABLE ACCOUNT
(
    Username varchar(255) primary key,
    Password varchar(1000),
	Account_Name nvarchar(255),
    Service nvarchar(255),
    Priority INT,
    Email varchar(255) unique
) auto_increment = 1000;
CREATE TABLE TYPE_PACKAGE(
	Id_Type_Package int auto_increment primary key,
    Name_Type nvarchar(255)
) AUTO_INCREMENT = 10000;
CREATE TABLE PACKAGE
( 
	Id_Package int auto_increment primary key,
	Name_Package nvarchar(255),
    Id_Type_Package int,
    Id_Province int,
    Id_District int,
	Image_Package text,
    Description_Package text,
	LengthTime_Package float
) auto_increment = 100000;
CREATE TABLE TOUR
(
	Id_Tour int auto_increment primary key  ,
    TotalDay tinyint,
    TotalNight tinyint,
    MaxTicket int,
    Status_Tour nvarchar(50)
) auto_increment = 1000000;
CREATE TABLE TOUR_DETAILS(
	Id_TourDetails int auto_increment primary key,
    Id_Tour int,
    Id_Package int,
    Session nvarchar(255),
    Date_Order_Package tinyint,
	Start_Time_Package time
) auto_increment = 1000000;
CREATE TABLE TRAVEL(
	Id_Travel int auto_increment primary key,
    Id_Tour int,
    StartLocation nvarchar(255),
    StartDateTimeTravel datetime,
	RemainingTicket int,
    Discount int
) auto_increment=1000000;
CREATE TABLE CUSTOMER(
	Id_Customer int auto_increment primary key,
    Name_Customer nvarchar(255),
    IdNumber nvarchar(15),
    PhoneNumber nvarchar(15),
    Address nvarchar(255)
) auto_increment=1000000;
CREATE TABLE BOOKING(
	Id_Booking int auto_increment primary key,
    Id_Customer_Booking int,
    Id_Travel int,
    Booking_Date Date,
    Status varchar(50)
)auto_increment=1000000;
CREATE TABLE BOOKING_DETAILS(
	Id_Booking_Details int auto_increment primary key,
    Id_Booking int,
    Id_Customer int
);
CREATE TABLE TICKET(
	Id_Ticket int auto_increment primary key,
    Id_Booking_Details int,
    Status nvarchar(100)
) auto_increment=1000000;


 -- FOREIGN KEY
 ALTER TABLE PACKAGE ADD FOREIGN KEY (Id_Type_Package) REFERENCES TYPE_PACKAGE(Id_Type_Package);
 ALTER TABLE TOUR_DETAILS ADD FOREIGN KEY(Id_Tour) references TOUR(Id_Tour);
 ALTER TABLE tour_details add foreign key(Id_Package) references package(Id_Package);
 ALTER TABLE travel add foreign key(Id_Tour) references tour(Id_Tour);
 ALTER TABLE booking add foreign key(Id_Customer_Booking) references customer(Id_Customer);
 ALTER TABLE booking add foreign key(Id_Travel) references travel(Id_Travel);
 ALTER TABLE booking_details add foreign key(Id_Booking) references booking(Id_booking);
 ALTER TABLE BOOKING_DETAILS add foreign key(Id_Customer) references customer(Id_Customer);
 ALTER TABLE ticket add foreign key(Id_Booking_Details) references booking_details(Id_Booking_Details);


