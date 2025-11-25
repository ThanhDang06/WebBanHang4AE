IF NOT EXISTS(SELECT name FROM sys.databases WHERE name = N'DBFashionStore')
BEGIN
    CREATE DATABASE DBFashionStore
    ON (FILENAME = 'App_Data\DBFashionStore.mdf')
    FOR ATTACH_REBUILD_LOG;
END
