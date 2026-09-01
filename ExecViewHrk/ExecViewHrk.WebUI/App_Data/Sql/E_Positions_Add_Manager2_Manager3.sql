IF OBJECT_ID(N'dbo.E_Positions', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'dbo.E_Positions', N'Manager2ID') IS NULL
    BEGIN
        ALTER TABLE dbo.E_Positions
        ADD Manager2ID INT NULL;
    END;

    IF COL_LENGTH(N'dbo.E_Positions', N'Manager3ID') IS NULL
    BEGIN
        ALTER TABLE dbo.E_Positions
        ADD Manager3ID INT NULL;
    END;
END;
