USE [master]
-------------------------------------Create database
CREATE DATABASE [VitaLabDB];
GO

USE [VitaLabDB]
GO

-------------------------------------Create tables
CREATE TABLE [Product]
(
 [ProductId]   int NOT NULL IDENTITY(1,1),
 [ProductName] nvarchar(50) NOT NULL ,
 [Price]	   decimal NOT NULL ,
 
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductId] ASC)
);
GO

CREATE TABLE [UserRole]
(
 [UserRoleId] int NOT NULL IDENTITY(1,1),
 [RoleName]   nvarchar(50) NOT NULL ,
 
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserRoleId] ASC)
);
GO

CREATE TABLE [User]
(
 [UserId]   int NOT NULL IDENTITY(1,1),
 [UserName] nvarchar(50) NOT NULL ,
 [Login]	nvarchar(50) NOT NULL ,
 [Password] nvarchar(84) NOT NULL ,

 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC)
);
GO

CREATE TABLE [UsersRoles]
(
 [Id]     int NOT NULL IDENTITY(1,1),
 [UserId] int NOT NULL,
 [RoleId] int NOT NULL,
 
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([Id] ASC),
 CONSTRAINT [FK_UsersRoles_UserId] FOREIGN KEY ([UserId])  REFERENCES [User]([UserId]),
 CONSTRAINT [FK_UsersRoles_RoleId] FOREIGN KEY ([RoleId])  REFERENCES [UserRole]([UserRoleId]),
);

CREATE TABLE [Order]
(
 [OrderId]	  int NOT NULL IDENTITY(1,1),
 [UserId]     int NOT NULL,
 [OrderDate]  Datetime NOT NULL,
 [TotalPrice] decimal NOT NULL,

  CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([OrderId] ASC),
  CONSTRAINT [FK_Order_UserId] FOREIGN KEY ([UserId])  REFERENCES [User]([UserId]),
);
GO

CREATE TABLE [OrderData]
(
 [OrderDataId]     int NOT NULL IDENTITY(1,1),
 [OrderId]	       int NOT NULL,
 [ProductId]	   int NOT NULL,
 [ProductQuantity] int NOT NULL,
 [TotalPrice]      decimal NOT NULL,

  CONSTRAINT [PK_OrderData] PRIMARY KEY CLUSTERED ([OrderDataId] ASC),
  CONSTRAINT [FK_OrderData_OrderId] FOREIGN KEY ([OrderId])  REFERENCES [Order]([OrderId]),
  CONSTRAINT [FK_OrderData_ProductId] FOREIGN KEY ([ProductId])  REFERENCES [Product](ProductId),
);
GO

USE [VitaLabDB]
GO
INSERT INTO UserRole (RoleName)
VALUES ('User')

INSERT INTO UserRole (RoleName)
VALUES ('ProductManager')

INSERT INTO UserRole (RoleName)
VALUES ('Administrator')

SELECT * FROM [UserRole]

INSERT INTO [User] ([UserName], [Login], [Password]) 
VALUES ('Pavel Evstigneev', 'Lucky', 'AQAAAAIAAYagAAAAEHupyoidfcYu7Aptzd/vU+cJjVr4nBApZQekJV/TrLmC/HOHPhSOoQ3gBOx0qqn2hA==')

INSERT INTO [User] ([UserName], [Login], [Password]) 
VALUES ('Mark Olegovich', 'Mark', 'AQAAAAIAAYagAAAAEHupyoidfcYu7Aptzd/vU+cJjVr4nBApZQekJV/TrLmC/HOHPhSOoQ3gBOx0qqn2hA==')

INSERT INTO UsersRoles (UserId, RoleId)
VALUES (1, 1)

INSERT INTO UsersRoles (UserId, RoleId)
VALUES (1, 2)

INSERT INTO UsersRoles (UserId, RoleId)
VALUES (1, 3)

INSERT INTO Product (ProductName, Price)
VALUES ('Ибупрофен', 50.0)

INSERT INTO Product (ProductName, Price)
VALUES ('Валерьянка', 20.0)

INSERT INTO Product (ProductName, Price)
VALUES ('Железа фумарат', 250.0)

SELECT * FROM [User]
SELECT * FROM [UsersRoles]
SELECT * FROM [Product]

