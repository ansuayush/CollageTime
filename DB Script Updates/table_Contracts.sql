go

IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'Job')
BEGIN
ALTER TABLE [dbo].[Contracts] add  Job varchar(100)
END

go

IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'Notes')
BEGIN
ALTER TABLE [dbo].[Contracts] add  Notes varchar(max)

END

go

IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'AddNewContract')
BEGIN
ALTER TABLE [dbo].[Contracts] add  AddNewContract varchar(200)
End
go

IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'Semister')
BEGIN
ALTER TABLE [dbo].[Contracts] add  Semister varchar(10)
End

go

IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].[Contracts]') AND name = 'ePositionId')
BEGIN
ALTER TABLE [dbo].[Contracts] add  ePositionId int

END

go
