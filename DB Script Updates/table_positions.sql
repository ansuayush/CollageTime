go

IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].[positions]') AND name = 'CostNumber')
BEGIN
ALTER TABLE [dbo].[positions] add  CostNumber varchar(100)
END

go


go

IF NOT EXISTS (SELECT 1  FROM SYS.COLUMNS WHERE  
OBJECT_ID = OBJECT_ID(N'[dbo].[E_Positions]') AND name = 'ReportsToID')
BEGIN
alter table E_Positions alter column ReportsToID int
END

go

