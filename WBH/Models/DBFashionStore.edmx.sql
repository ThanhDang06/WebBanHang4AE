
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/23/2025 16:15:14
-- Generated from EDMX file: D:\WEB BAN HANG\WebBanHang4AE\WBH\Models\DBFashionStore.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DBFashionStore];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK__Cart__IDColor__29221CFB]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cart] DROP CONSTRAINT [FK__Cart__IDColor__29221CFB];
GO
IF OBJECT_ID(N'[dbo].[FK__Cart__IDCus__01142BA1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cart] DROP CONSTRAINT [FK__Cart__IDCus__01142BA1];
GO
IF OBJECT_ID(N'[dbo].[FK__Cart__IDProduct__2739D489]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cart] DROP CONSTRAINT [FK__Cart__IDProduct__2739D489];
GO
IF OBJECT_ID(N'[dbo].[FK__Cart__IDSize__282DF8C2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cart] DROP CONSTRAINT [FK__Cart__IDSize__282DF8C2];
GO
IF OBJECT_ID(N'[dbo].[FK__OrderDeta__IDCol__2CF2ADDF]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [FK__OrderDeta__IDCol__2CF2ADDF];
GO
IF OBJECT_ID(N'[dbo].[FK__OrderDeta__IDOrd__5FB337D6]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [FK__OrderDeta__IDOrd__5FB337D6];
GO
IF OBJECT_ID(N'[dbo].[FK__OrderDeta__IDPro__2C3393D0]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [FK__OrderDeta__IDPro__2C3393D0];
GO
IF OBJECT_ID(N'[dbo].[FK__OrderDeta__IDSiz__2BFE89A6]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [FK__OrderDeta__IDSiz__2BFE89A6];
GO
IF OBJECT_ID(N'[dbo].[FK__Orders__IDCus__60A75C0F]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK__Orders__IDCus__60A75C0F];
GO
IF OBJECT_ID(N'[dbo].[FK__ProductCo__IDPro__36B12243]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductColor] DROP CONSTRAINT [FK__ProductCo__IDPro__36B12243];
GO
IF OBJECT_ID(N'[dbo].[FK__ProductSi__IDPro__398D8EEE]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductSize] DROP CONSTRAINT [FK__ProductSi__IDPro__398D8EEE];
GO
IF OBJECT_ID(N'[dbo].[FK__Sales__IDProduct__5165187F]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sales] DROP CONSTRAINT [FK__Sales__IDProduct__5165187F];
GO
IF OBJECT_ID(N'[dbo].[FK_Customers_Accounts]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Customers] DROP CONSTRAINT [FK_Customers_Accounts];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderDetails_ProductColor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [FK_OrderDetails_ProductColor];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderDetails_ProductSize]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [FK_OrderDetails_ProductSize];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductVariants_ProductColor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductVariants] DROP CONSTRAINT [FK_ProductVariants_ProductColor];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductVariants_Products]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductVariants] DROP CONSTRAINT [FK_ProductVariants_Products];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductVariants_ProductSize]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductVariants] DROP CONSTRAINT [FK_ProductVariants_ProductSize];
GO
IF OBJECT_ID(N'[dbo].[FK_Sales_Products]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sales] DROP CONSTRAINT [FK_Sales_Products];
GO
IF OBJECT_ID(N'[dbo].[FK_Voucher_Customers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Voucher] DROP CONSTRAINT [FK_Voucher_Customers];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Accounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Accounts];
GO
IF OBJECT_ID(N'[dbo].[Cart]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Cart];
GO
IF OBJECT_ID(N'[dbo].[Customers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customers];
GO
IF OBJECT_ID(N'[dbo].[OrderDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderDetails];
GO
IF OBJECT_ID(N'[dbo].[Orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Orders];
GO
IF OBJECT_ID(N'[dbo].[ProductColor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductColor];
GO
IF OBJECT_ID(N'[dbo].[Products]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Products];
GO
IF OBJECT_ID(N'[dbo].[ProductSize]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductSize];
GO
IF OBJECT_ID(N'[dbo].[ProductVariants]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductVariants];
GO
IF OBJECT_ID(N'[dbo].[Sales]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sales];
GO
IF OBJECT_ID(N'[dbo].[Voucher]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Voucher];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Accounts'
CREATE TABLE [dbo].[Accounts] (
    [IDAcc] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(50)  NULL,
    [Password] nvarchar(100)  NULL,
    [Role] nvarchar(20)  NULL
);
GO

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [IDCus] int IDENTITY(1,1) NOT NULL,
    [FullName] nvarchar(100)  NULL,
    [Email] nvarchar(100)  NULL,
    [Password] nvarchar(100)  NULL,
    [Phone] varchar(15)  NULL,
    [Address] nvarchar(200)  NULL,
    [IDAcc] int  NOT NULL
);
GO

-- Creating table 'OrderDetails'
CREATE TABLE [dbo].[OrderDetails] (
    [IDDetail] int IDENTITY(1,1) NOT NULL,
    [IDOrder] int  NULL,
    [IDProduct] int  NULL,
    [Quantity] int  NULL,
    [Price] decimal(18,2)  NULL,
    [IDSize] int  NULL,
    [IDColor] int  NULL
);
GO

-- Creating table 'Orders'
CREATE TABLE [dbo].[Orders] (
    [IDOrder] int IDENTITY(1,1) NOT NULL,
    [IDCus] int  NULL,
    [DateOrder] datetime  NULL,
    [AddressDelivery] nvarchar(200)  NULL,
    [FullName] nvarchar(100)  NULL,
    [Phone] nvarchar(15)  NULL,
    [PaymentMethod] nvarchar(50)  NULL,
    [Note] nvarchar(255)  NULL,
    [Status] nvarchar(50)  NULL,
    [Total] decimal(18,2)  NULL,
    [VoucherCode] nvarchar(50)  NULL,
    [Discount] decimal(18,2)  NULL
);
GO

-- Creating table 'Products'
CREATE TABLE [dbo].[Products] (
    [IDProduct] int IDENTITY(1,1) NOT NULL,
    [ProductName] nvarchar(100)  NULL,
    [Description] nvarchar(255)  NULL,
    [Price] decimal(18,2)  NULL,
    [Quantity] int  NULL,
    [Image] nvarchar(200)  NULL,
    [Category] nvarchar(50)  NULL,
    [OldPrice] decimal(18,2)  NULL,
    [IsSale] bit  NOT NULL
);
GO

-- Creating table 'ProductSizes'

-- Creating table 'ProductVariants'
CREATE TABLE [dbo].[ProductVariants] (
    [IDVariant] int IDENTITY(1,1) NOT NULL,
    [IDProduct] int  NOT NULL,
    [IDSize] int  NOT NULL,
    [IDColor] int  NOT NULL,
    [Quantity] int  NOT NULL
);
GO

-- Creating table 'Sales'
CREATE TABLE [dbo].[Sales] (
    [IDSale] int IDENTITY(1,1) NOT NULL,
    [IDProduct] int  NULL,
    [Category] nvarchar(50)  NULL,
    [DiscountPercent] decimal(5,2)  NULL,
    [StartDate] datetime  NULL,
    [EndDate] datetime  NULL,
    [Active] bit  NULL
);
GO


-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [IDAcc] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [PK_Accounts]
    PRIMARY KEY CLUSTERED ([IDAcc] ASC);
GO

-- Creating primary key on [IDCart] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [PK_Carts]
    PRIMARY KEY CLUSTERED ([IDCart] ASC);
GO

-- Creating primary key on [IDCus] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([IDCus] ASC);
GO

-- Creating primary key on [IDDetail] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [PK_OrderDetails]
    PRIMARY KEY CLUSTERED ([IDDetail] ASC);
GO

-- Creating primary key on [IDOrder] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [PK_Orders]
    PRIMARY KEY CLUSTERED ([IDOrder] ASC);
GO

-- Creating primary key on [IDColor] in table 'ProductColors'
ALTER TABLE [dbo].[ProductColors]
ADD CONSTRAINT [PK_ProductColors]
    PRIMARY KEY CLUSTERED ([IDColor] ASC);
GO

-- Creating primary key on [IDProduct] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [PK_Products]
    PRIMARY KEY CLUSTERED ([IDProduct] ASC);
GO

-- Creating primary key on [IDSize] in table 'ProductSizes'
ALTER TABLE [dbo].[ProductSizes]
ADD CONSTRAINT [PK_ProductSizes]
    PRIMARY KEY CLUSTERED ([IDSize] ASC);
GO

-- Creating primary key on [IDVariant] in table 'ProductVariants'
ALTER TABLE [dbo].[ProductVariants]
ADD CONSTRAINT [PK_ProductVariants]
    PRIMARY KEY CLUSTERED ([IDVariant] ASC);
GO

-- Creating primary key on [IDSale] in table 'Sales'
ALTER TABLE [dbo].[Sales]
ADD CONSTRAINT [PK_Sales]
    PRIMARY KEY CLUSTERED ([IDSale] ASC);
GO

-- Creating primary key on [IDVoucher] in table 'Vouchers'
ALTER TABLE [dbo].[Vouchers]
ADD CONSTRAINT [PK_Vouchers]
    PRIMARY KEY CLUSTERED ([IDVoucher] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [IDAcc] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [FK_Customers_Accounts]
    FOREIGN KEY ([IDAcc])
    REFERENCES [dbo].[Accounts]
        ([IDAcc])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Customers_Accounts'
CREATE INDEX [IX_FK_Customers_Accounts]
ON [dbo].[Customers]
    ([IDAcc]);
GO

-- Creating foreign key on [IDColor] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [FK__Cart__IDColor__29221CFB]
    FOREIGN KEY ([IDColor])
    REFERENCES [dbo].[ProductColors]
        ([IDColor])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [IDCus] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [FK__Cart__IDCus__01142BA1]
    FOREIGN KEY ([IDCus])
    REFERENCES [dbo].[Customers]
        ([IDCus])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO


-- Creating foreign key on [IDProduct] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [FK__Cart__IDProduct__2739D489]
    FOREIGN KEY ([IDProduct])
    REFERENCES [dbo].[Products]
        ([IDProduct])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [IDSize] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [FK__Cart__IDSize__282DF8C2]
    FOREIGN KEY ([IDSize])
    REFERENCES [dbo].[ProductSizes]
        ([IDSize])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [IDCus] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK__Orders__IDCus__60A75C0F]
    FOREIGN KEY ([IDCus])
    REFERENCES [dbo].[Customers]
        ([IDCus])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Orders__IDCus__60A75C0F'
CREATE INDEX [IX_FK__Orders__IDCus__60A75C0F]
ON [dbo].[Orders]
    ([IDCus]);
GO

-- Creating foreign key on [IDOrder] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK__OrderDeta__IDOrd__5FB337D6]
    FOREIGN KEY ([IDOrder])
    REFERENCES [dbo].[Orders]
        ([IDOrder])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__OrderDeta__IDOrd__5FB337D6'
CREATE INDEX [IX_FK__OrderDeta__IDOrd__5FB337D6]
ON [dbo].[OrderDetails]
    ([IDOrder]);
GO

-- Creating foreign key on [IDProduct] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK__OrderDeta__IDPro__2C3393D0]
    FOREIGN KEY ([IDProduct])
    REFERENCES [dbo].[Products]
        ([IDProduct])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__OrderDeta__IDPro__2C3393D0'
CREATE INDEX [IX_FK__OrderDeta__IDPro__2C3393D0]
ON [dbo].[OrderDetails]
    ([IDProduct]);
GO

-- Creating foreign key on [IDProduct] in table 'ProductColors'
ALTER TABLE [dbo].[ProductColors]
ADD CONSTRAINT [FK__ProductCo__IDPro__36B12243]
    FOREIGN KEY ([IDProduct])
    REFERENCES [dbo].[Products]
        ([IDProduct])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__ProductCo__IDPro__36B12243'
CREATE INDEX [IX_FK__ProductCo__IDPro__36B12243]
ON [dbo].[ProductColors]
    ([IDProduct]);
GO

-- Creating foreign key on [IDColor] in table 'ProductVariants'
ALTER TABLE [dbo].[ProductVariants]
ADD CONSTRAINT [FK_ProductVariants_ProductColor]
    FOREIGN KEY ([IDColor])
    REFERENCES [dbo].[ProductColors]
        ([IDColor])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductVariants_ProductColor'
CREATE INDEX [IX_FK_ProductVariants_ProductColor]
ON [dbo].[ProductVariants]
    ([IDColor]);
GO

-- Creating foreign key on [IDProduct] in table 'ProductSizes'
ALTER TABLE [dbo].[ProductSizes]
ADD CONSTRAINT [FK__ProductSi__IDPro__398D8EEE]
    FOREIGN KEY ([IDProduct])
    REFERENCES [dbo].[Products]
        ([IDProduct])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__ProductSi__IDPro__398D8EEE'
CREATE INDEX [IX_FK__ProductSi__IDPro__398D8EEE]
ON [dbo].[ProductSizes]
    ([IDProduct]);
GO

-- Creating foreign key on [IDProduct] in table 'Sales'
ALTER TABLE [dbo].[Sales]
ADD CONSTRAINT [FK__Sales__IDProduct__5165187F]
    FOREIGN KEY ([IDProduct])
    REFERENCES [dbo].[Products]
        ([IDProduct])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__Sales__IDProduct__5165187F'
CREATE INDEX [IX_FK__Sales__IDProduct__5165187F]
ON [dbo].[Sales]
    ([IDProduct]);
GO

-- Creating foreign key on [IDProduct] in table 'ProductVariants'
ALTER TABLE [dbo].[ProductVariants]
ADD CONSTRAINT [FK_ProductVariants_Products]
    FOREIGN KEY ([IDProduct])
    REFERENCES [dbo].[Products]
        ([IDProduct])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductVariants_Products'
CREATE INDEX [IX_FK_ProductVariants_Products]
ON [dbo].[ProductVariants]
    ([IDProduct]);
GO

-- Creating foreign key on [IDProduct] in table 'Sales'
ALTER TABLE [dbo].[Sales]
ADD CONSTRAINT [FK_Sales_Products]
    FOREIGN KEY ([IDProduct])
    REFERENCES [dbo].[Products]
        ([IDProduct])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sales_Products'
CREATE INDEX [IX_FK_Sales_Products]
ON [dbo].[Sales]
    ([IDProduct]);
GO

-- Creating foreign key on [IDSize] in table 'ProductVariants'
ALTER TABLE [dbo].[ProductVariants]
ADD CONSTRAINT [FK_ProductVariants_ProductSize]
    FOREIGN KEY ([IDSize])
    REFERENCES [dbo].[ProductSizes]
        ([IDSize])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductVariants_ProductSize'
CREATE INDEX [IX_FK_ProductVariants_ProductSize]
ON [dbo].[ProductVariants]
    ([IDSize]);
GO

-- Creating foreign key on [IDColor] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK_OrderDetails_ProductColor]
    FOREIGN KEY ([IDColor])
    REFERENCES [dbo].[ProductColors]
        ([IDColor])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderDetails_ProductColor'
CREATE INDEX [IX_FK_OrderDetails_ProductColor]
ON [dbo].[OrderDetails]
    ([IDColor]);
GO

-- Creating foreign key on [IDSize] in table 'OrderDetails'
ALTER TABLE [dbo].[OrderDetails]
ADD CONSTRAINT [FK_OrderDetails_ProductSize]
    FOREIGN KEY ([IDSize])
    REFERENCES [dbo].[ProductSizes]
        ([IDSize])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderDetails_ProductSize'
CREATE INDEX [IX_FK_OrderDetails_ProductSize]
ON [dbo].[OrderDetails]
    ([IDSize]);
GO

-- Creating foreign key on [IDCus] in table 'Vouchers'
ALTER TABLE [dbo].[Vouchers]
ADD CONSTRAINT [FK_Voucher_Customers]
    FOREIGN KEY ([IDCus])
    REFERENCES [dbo].[Customers]
        ([IDCus])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Voucher_Customers'
CREATE INDEX [IX_FK_Voucher_Customers]
ON [dbo].[Vouchers]
    ([IDCus]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------