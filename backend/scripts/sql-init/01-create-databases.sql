-- ToyStore SQL Server Database Initialization
-- Bu script otomatik olarak veritabanlarını oluşturur

USE master;
GO

-- ToyStore Identity Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ToyStoreIdentity')
BEGIN
    CREATE DATABASE ToyStoreIdentity;
    PRINT 'ToyStoreIdentity database created.';
END
GO

-- ToyStore Products Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ToyStoreProducts')
BEGIN
    CREATE DATABASE ToyStoreProducts;
    PRINT 'ToyStoreProducts database created.';
END
GO

-- ToyStore Orders Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ToyStoreOrders')
BEGIN
    CREATE DATABASE ToyStoreOrders;
    PRINT 'ToyStoreOrders database created.';
END
GO

-- ToyStore Users Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ToyStoreUsers')
BEGIN
    CREATE DATABASE ToyStoreUsers;
    PRINT 'ToyStoreUsers database created.';
END
GO

PRINT 'All ToyStore databases initialized successfully!';
GO
