CREATE DATABASE DBFashionStore;
GO
USE DBFashionStore;
GO
--Bảng Khách Hàng
CREATE TABLE Customers (
    IDCus INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone VARCHAR(15),
    Address NVARCHAR(200)
);
--Bảng Sản Phẩm
CREATE TABLE Products (
    IDProduct INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100),
    Description NVARCHAR(255),
    Price DECIMAL(18,2),
    Quantity INT,
    Image NVARCHAR(200),
    Category NVARCHAR(50)
);
--Bảng Đơn Hàng
CREATE TABLE Orders (
    IDOrder INT IDENTITY(1,1) PRIMARY KEY,
    IDCus INT,
    DateOrder DATE,
    AddressDelivery NVARCHAR(200),
    Total DECIMAL(18,2),
    FOREIGN KEY (IDCus) REFERENCES Customers(IDCus)
);
--Bảng Chi Tiết Đơn Hàng
CREATE TABLE OrderDetails (
    IDDetail INT IDENTITY(1,1) PRIMARY KEY,
    IDOrder INT,
    IDProduct INT,
    Quantity INT,
    UnitPrice DECIMAL(18,2),
    FOREIGN KEY (IDOrder) REFERENCES Orders(IDOrder),
    FOREIGN KEY (IDProduct) REFERENCES Products(IDProduct)
);
--Bảng Tài Khoản
CREATE TABLE Accounts (
    IDAcc INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50),
    Password NVARCHAR(100),
    Role NVARCHAR(20)  -- 'Admin' hoặc 'Customer'
);
-- Bảng giỏ hàng
CREATE TABLE Cart (
    IDCart INT IDENTITY(1,1) PRIMARY KEY,
    IDCus INT,
    IDProduct INT,
    Quantity INT,
    DateAdded DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IDCus) REFERENCES Customers(IDCus),
    FOREIGN KEY (IDProduct) REFERENCES Products(IDProduct)
);
