

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'IsPrimaryPhone' AND Object_ID = Object_ID(N'dbo.PersonPhoneNumbers'))
BEGIN
    ALTER TABLE PersonPhoneNumbers ADD [IsPrimaryPhone]  BIT  NOT NULL DEFAULT 0;
    
END

GO