
if not exists ( select 1 from dbo.ddlWorkersCompensations where code='8811')
begin
	insert into ddlWorkersCompensations(code,description,Active) values('8811','881',1)
end
go	
 
 if not exists ( select 1 from dbo.ddlJobClasses where code='Office')
begin
	insert into ddlJobClasses(code,description,Active) values('Office','Office',1)
end
go	

 if not exists ( select 1 from dbo.ddlEEOJobCodes where code='1')
begin
	insert into ddlEEOJobCodes(code,description,eeoFileStatusID,Active) values ('1','test',1,1)
end
go	

if not exists ( select 1 from dbo.ddlFLSAs where code='NE')
begin
	insert into ddlFLSAs(description,code,Active) values ('Non Exempt','NE',1)
end
go	

if not exists ( select 1 from dbo.ddlEEOJobTrainingStatuses where code='1')
begin
	insert into ddlEEOJobTrainingStatuses(description,code,Active) values ('New',1,1)
end
go

if not exists ( select 1 from dbo.ddlUnions where code='CDD')
begin
	insert into ddlUnions(description,code,Active) values ('CDD','CDD',1)
end
go

if not exists ( select 1 from dbo.ddlJobFamilys where code='U')
begin
	insert into ddlJobFamilys(description,code,Active) values ('Uniform','U',1)
end
go


if not exists ( select 1 from dbo.ddlBusinessLevelTypes where code='BP')
begin
	insert into ddlBusinessLevelTypes(code,description,Active) values ('BP','Business plans for startups',1)
end
go
if not exists ( select 1 from dbo.ddlEEOFileStatuses where code='EA')
begin
	insert into ddlEEOFileStatuses(code,description,Active) values ('EA','employer accountable',1)
end
go


if not exists ( select 1 from dbo.ddlEINs where EIN='123456789')
begin
	insert into ddlEINs values ('123456789','123456789','addressLineOne','addressLineTwo','city',2,'2','2','umber','faxNer',1,'notes',	1)
end
go

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'BusinessLevelCode' AND Object_ID = Object_ID(N'dbo.PositionBusinessLevels'))
BEGIN
    ALTER TABLE PositionBusinessLevels ADD BusinessLevelCode nvarchar(1000) not null
END

GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'BusinessLevelNotes' AND Object_ID = Object_ID(N'dbo.PositionBusinessLevels'))
BEGIN
    ALTER TABLE PositionBusinessLevels ADD BusinessLevelNotes nvarchar(700) null
END

GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Active' AND Object_ID = Object_ID(N'dbo.ddlEEOFileStatuses'))
BEGIN
    ALTER TABLE ddlEEOFileStatuses ADD [Active] [bit]   DEFAULT ((1));
    Update ddlEEOFileStatuses SET [Active] = '1'
END

GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Active' AND Object_ID = Object_ID(N'dbo.DdlEeoTypes'))
BEGIN
    ALTER TABLE DdlEeoTypes ADD [Active] [bit]   DEFAULT ((1));
    Update DdlEeoTypes SET [Active] = '1';
END

GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Active' AND Object_ID = Object_ID(N'dbo.ddlEEOJobTrainingStatuses'))
BEGIN
    ALTER TABLE ddlEEOJobTrainingStatuses ADD [Active] [bit]   DEFAULT ((1));
    Update ddlEEOJobTrainingStatuses SET [Active] = '1';
END

GO

GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'projectedEndDate' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[projectedEndDate] [smalldatetime] NULL
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'actualEndDate' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD  [actualEndDate] [smalldatetime] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'positionCategoryID' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[positionCategoryID] [smallint] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'positionTypeID' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[positionTypeID] [smallint] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'positionGradeID' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[positionGradeID] [smallint] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'scheduledDays' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[scheduledDays] [varchar](9) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'weeklyHours' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[weeklyHours] [varchar](9) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'salary' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[salary] [money] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'determinedStatus' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[determinedStatus] [varchar](50) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'headcount' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[headcount] [decimal](18, 0) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'FTE' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[FTE] [decimal](18, 0) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'homeDepartmentID' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[homeDepartmentID] [int] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'homeLocationID' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[homeLocationID] [int] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'percentage' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[percentage] [decimal](18, 2) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'payRate' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[payRate] [money] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'rateTypeCode' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[rateTypeCode] [varchar](1) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'overtimeMultiplier' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[overtimeMultiplier] [decimal](18, 2) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'shift' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[shift] [tinyint] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'clockNumber' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[clockNumber] [varchar](50) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'workersCompensationID' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[workersCompensationID] [tinyint] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'FLSAID' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[FLSAID] [tinyint] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'companyID' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[companyID] [int] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'payFrequencyCode' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[payFrequencyCode] [varchar](10) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'mustAllocate' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[mustAllocate] [bit] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Class' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[Class] [varchar](10) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'ReportsToID' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[ReportsToID] [varchar](9) NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'GoalAmount' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[GoalAmount] [money] NULL
END
GO

	IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'spaceNumber' AND Object_ID = Object_ID(N'dbo.E_Positions'))
BEGIN
ALTER  TABLE E_Positions
ADD	[spaceNumber] [varchar](50) NULL
END
GO  


IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Active' AND Object_ID = Object_ID(N'dbo.ddlJobFamilys'))
BEGIN
    ALTER TABLE ddlJobFamilys ADD [Active] [bit]   DEFAULT ((1));
    Update ddlJobFamilys SET [Active] = '1'
END

GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Active' AND Object_ID = Object_ID(N'dbo.ddlEEOJobCodes'))
BEGIN
    ALTER TABLE ddlEEOJobCodes ADD [Active] [bit]   DEFAULT ((1));
    Update ddlEEOJobCodes SET [Active] = '1'
END

GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Active' AND Object_ID = Object_ID(N'dbo.ddlWorkersCompensations'))
BEGIN
    ALTER TABLE ddlWorkersCompensations ADD [Active] [bit]   DEFAULT ((1));
    Update ddlWorkersCompensations SET [Active] = '1'
END

GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'PositionGradeID' AND Object_ID = Object_ID(N'dbo.Positions'))
BEGIN
    ALTER TABLE Positions ADD PositionGradeID smallint  null
END
 GO

 IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Active' AND Object_ID = Object_ID(N'dbo.Locations'))
BEGIN
    ALTER TABLE Locations ADD [Active] [bit]   DEFAULT ((1));
    Update Locations SET [Active] = '1'
END

GO



 IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME ='PositionBusinessLevels_BusinessLevelTitle')
 begin
	ALTER TABLE PositionBusinessLevels ADD CONSTRAINT PositionBusinessLevels_BusinessLevelTitle UNIQUE (BusinessLevelTitle);
end
GO
 IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME ='PositionBusinessLevels_BusinessLevelCode')
 begin
ALTER TABLE PositionBusinessLevels ADD CONSTRAINT PositionBusinessLevels_BusinessLevelCode UNIQUE (BusinessLevelCode);
end
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'IsStudent' AND Object_ID = Object_ID(N'dbo.Persons'))
BEGIN
ALTER  TABLE Persons
ADD	[IsStudent] [bit] NULL
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'IsTrainer' AND Object_ID = Object_ID(N'dbo.Persons'))
BEGIN
ALTER  TABLE Persons
ADD	[IsTrainer] [bit] NULL
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'IsApplicant' AND Object_ID = Object_ID(N'dbo.Persons'))
BEGIN
ALTER  TABLE Persons
ADD	[IsApplicant] [bit] NULL
END

GO


IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('DF_Persons_IsStudent'))
begin
ALTER TABLE Persons ADD CONSTRAINT DF_Persons_IsStudent  DEFAULT (0) FOR [IsStudent]
end
GO

IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('DF_Persons_IsTrainer'))
begin
ALTER TABLE Persons ADD CONSTRAINT DF_Persons_IsTrainer DEFAULT (0) FOR [IsTrainer]
end
GO

IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('DF_Persons_IsApplicant'))
begin
ALTER TABLE Persons ADD CONSTRAINT DF_Persons_IsApplicant  DEFAULT ((0)) FOR [IsApplicant]
end
GO

IF  EXISTS(SELECT 1 FROM sysobjects WHERE id = OBJECT_ID('FK_Employees_CompanyCodes]'))
BEGIN
	ALTER TABLE Employees DROP CONSTRAINT FK_Employees_CompanyCodes	
END

GO

ALTER TABLE PERSONS ALTER COLUMN SSN VARCHAR(9) NULL
GO


if not exists ( select 1 from dbo.PerformanceProfiles where code='QP')
begin
	insert into PerformanceProfiles values ('QP','Quarterly Performance',1)
end
go	

if not exists ( select 1 from dbo.PerformanceProfiles where code='HYP')
begin
	insert into PerformanceProfiles values ('HYP','Half Yearly Performance',1)
end
go	

if not exists ( select 1 from dbo.PerformanceProfiles where code='AP')
begin
	insert into PerformanceProfiles values ('AP','Annual Performance',1)
end
go

ALTER TABLE Positions ALTER COLUMN PositionCode VARCHAR(15) NOT NULL

Go

Alter Table E_positions Alter column positionCategoryID int

Go



IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'EndDate' AND Object_ID = Object_ID(N'dbo.E_PositionSalaryHistories'))
BEGIN
ALTER  TABLE E_PositionSalaryHistories
ADD	[EndDate] [datetime] 
END
GO 

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'RateTypeId' AND Object_ID = Object_ID(N'dbo.E_PositionSalaryHistories'))
BEGIN
ALTER  TABLE E_PositionSalaryHistories
ADD	[RateTypeId] [int] 
END
GO 

IF NOT EXISTS (SELECT 1 
               FROM   Information_Schema.Columns 
               WHERE  Table_Name = 'PersonAddresses' 
                      AND Table_Schema = 'dbo' 
                      AND Column_Name = 'IsPrimaryAddress') 
  BEGIN 
      ALTER TABLE dbo.PersonAddresses 
        ADD IsPrimaryAddress BIT NOT NULL DEFAULT 0 
  END 
  
 GO
 
 IF NOT EXISTS (SELECT 1 
               FROM   Information_Schema.Columns 
               WHERE  Table_Name = 'DdlEmploymentStatuses' 
                      AND Table_Schema = 'dbo' 
                      AND Column_Name = 'IsDefault') 
  BEGIN 
      ALTER TABLE dbo.DdlEmploymentStatuses 
        ADD IsDefault BIT NOT NULL DEFAULT 0 
  END 
 
 GO
 
 
  ---Active
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='A' AND [Description]='Active')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('A','Active',1,1)
	END
GO
	-------On International Assignment
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='B' AND [Description]='On International Assignment')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('B','On International Assignment',1,1)
	END

GO
	--Casual
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='C' AND [Description]='Casual')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('C','Casual',1,1)
	END
GO
	--On Long Term Disability
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='D' AND [Description]='On Long Term Disability')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('D','On Long Term Disability',1,1)
	END
GO
--Sick Leave
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='E' AND [Description]='Sick Leave')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('E','Sick Leave',1,1)
	END
GO
--Surviving Spouse
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='F' AND [Description]='Surviving Spouse')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('F','Surviving Spouse',1,1)
	END
GO
--No Longer Employed
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='G' AND [Description]='No Longer Employed')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('G','No Longer Employed',1,1)
	END
GO
--Blocked
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='H' AND [Description]='Blocked')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('H','Blocked',1,1)
	END
GO
--Inactive
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='I' AND [Description]='Inactive')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('I','Inactive',1,1)
	END
GO
--Transferred
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='J' AND [Description]='Transferred')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('J','Transferred',1,1)
	END
GO
--On Sabbatical
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='K' AND [Description]='On Sabbatical')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('K','On Sabbatical',1,1)
	END
GO
--On Leave
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='L' AND [Description]='On Leave')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('L','On Leave',1,1)
	END
GO
--Multiple Positions
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='M' AND [Description]='Multiple Positions')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('M','Multiple Positions',1,1)
	END
GO
--New Employee
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='N' AND [Description]='New Employee')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('N','New Employee',1,1)
	END
GO
--Lay Off
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='O' AND [Description]='Lay Off')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('O','Lay Off',1,1)
	END
GO
--Part Time
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='P' AND [Description]='Part Time')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('P','Part Time',1,1)
	END
GO
--Deaceased
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='Q' AND [Description]='Deaceased')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('Q','Deaceased',1,1)
	END
GO
--Retired
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='R' AND [Description]='Retired')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('R','Retired',1,1)
	END
GO
--Separated
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='S' AND [Description]='Separated')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('S','Separated',1,1)
	END
GO
--Terminated
IF NOT EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE code='T' AND [Description]='Terminated')
	BEGIN
		INSERT INTO dbo.DdlEmploymentStatuses(Code,[Description],Active,IsDefault) VALUES('T','Terminated',1,1)
	END
GO


IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='A' AND [Description]='Active')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='A' AND [Description]='Active'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='B' AND [Description]='On International Assignment')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='B' AND [Description]='On International Assignment'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='C' AND [Description]='Casual')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='C' AND [Description]='Casual'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='D' AND [Description]='On Long Term Disability')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='D' AND [Description]='On Long Term Disability'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='E' AND [Description]='Sick Leave')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='E' AND [Description]='Sick Leave'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='F' AND [Description]='Surviving Spouse')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='F' AND [Description]='Surviving Spouse'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='G' AND [Description]='No Longer Employed')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='G' AND [Description]='No Longer Employed'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='H' AND [Description]='Blocked')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='H' AND [Description]='Blocked'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='I' AND [Description]='Inactive')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='I' AND [Description]='Inactive'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='J' AND [Description]='Transferred')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='J' AND [Description]='Transferred'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='K' AND [Description]='On Sabbatical')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='K' AND [Description]='On Sabbatical'
	END

IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='L' AND [Description]='On Leave')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='L' AND [Description]='On Leave'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='M' AND [Description]='Multiple Positions')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='M' AND [Description]='Multiple Positions'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='N' AND [Description]='New Employee')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='N' AND [Description]='New Employee'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='O' AND [Description]='Lay Off')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='O' AND [Description]='Lay Off'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='P' AND [Description]='Part Time')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='P' AND [Description]='Part Time'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='Q' AND [Description]='Deaceased')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='Q' AND [Description]='Deaceased'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='R' AND [Description]='Retired')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='R' AND [Description]='Retired'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='S' AND [Description]='Separated')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='S' AND [Description]='Separated'
	END
GO
IF  EXISTS ( SELECT 1 FROM dbo.DdlEmploymentStatuses WHERE Code='T' AND [Description]='Terminated')
	BEGIN
		UPDATE
			 dbo.DdlEmploymentStatuses
		SET IsDefault=1 ,Active=1
					WHERE Code='T' AND [Description]='Terminated'
	END
GO


IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'PositionId' AND Object_ID = Object_ID(N'dbo.PositionFundingSources'))
BEGIN
ALTER TABLE [PositionFundingSources] ADD [PositionId] [smallint] NULL 
END

GO


IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'FundingGroup' AND Object_ID = Object_ID(N'dbo.PositionFundingSources'))
BEGIN
ALTER TABLE [PositionFundingSources] ADD [FundingGroup] [nvarchar](50) NULL
END

GO

IF NOT EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Amount' AND Object_ID = Object_ID(N'dbo.Funds'))
BEGIN

ALTER TABLE Funds
ADD Amount DECIMAL(18,2),
FTE DECIMAL(18,2),
EffectiveStartDate DATETIME,
EffectiveEndDate DATETIME

END

GO

if not exists (select 1 from Information_Schema.Columns where Table_Name='Positions' and Table_Schema ='dbo' and Column_Name='Suffix')
begin
	alter table dbo.Positions 
	add Suffix char(2)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumberEffectiveDate')
begin
	alter table E_Positions
	add CostNumberEffectiveDate datetime
end
go
if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber1Percent')
begin
	alter table E_Positions
	add CostNumber1Percent decimal(10,2)
end
go
if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber2Account')
begin
	alter table E_Positions
	add CostNumber2Account varchar(100)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber2Percent')
begin
	alter table E_Positions
	add CostNumber2Percent decimal(10,2)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber3Account')
begin
	alter table E_Positions
	add CostNumber3Account varchar(100)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber3Percent')
begin
	alter table E_Positions
	add CostNumber3Percent decimal(10,2)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber4Account')
begin
	alter table E_Positions
	add CostNumber4Account varchar(100)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber4Percent')
begin
	alter table E_Positions
	add CostNumber4Percent decimal(10,2)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber5Account')
begin
	alter table E_Positions
	add CostNumber5Account varchar(100)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber5Percent')
begin
	alter table E_Positions
	add CostNumber5Percent decimal(10,2)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber6Account')
begin
	alter table E_Positions
	add CostNumber6Account varchar(100)
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='CostNumber6Percent')
begin
	alter table E_Positions
	add CostNumber6Percent decimal(10,2)
end
go

if exists(
SELECT 1 
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_NAME='uq_Positions' )
begin
	alter table Positions
	drop constraint uq_Positions
end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='E_Positions' and Table_Schema ='dbo' and Column_Name='EmployeeClassId')
begin
	alter table E_Positions
	add EmployeeClassId int
end
go

if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.EmployeeClass') and type in (N'U'))
begin
	CREATE TABLE [dbo].[EmployeeClass](
	[EmployeeClassId] [int] IDENTITY(1,1) NOT NULL,
	[ClassName] [varchar](20) NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL
) ON [PRIMARY]

end
go

if not exists (select 1 from Information_Schema.Columns where Table_Name='ManagerDepartments' and Table_Schema ='dbo' and Column_Name='CompanyCodeId')
begin
	alter table ManagerDepartments
	add CompanyCodeId int
end
go