
if not exists ( select 1 from dbo.ddlWorkersCompensations where code='8811')
begin
	insert into ddlWorkersCompensations values('8811','881')
end
go	
 
 if not exists ( select 1 from dbo.ddlJobClasses where code='Office')
begin
	insert into ddlJobClasses values('Office','Office',1)
end
go	

 if not exists ( select 1 from dbo.ddlEEOJobCodes where code='1')
begin
	insert into ddlEEOJobCodes values ('test','1',1)
end
go	

if not exists ( select 1 from dbo.ddlFLSAs where code='NE')
begin
	insert into ddlFLSAs values ('Non Exempt','NE',1)
end
go	

if not exists ( select 1 from dbo.ddlEEOJobTrainingStatuses where code='1')
begin
	insert into ddlEEOJobTrainingStatuses values ('New',1)
end
go

if not exists ( select 1 from dbo.ddlUnions where code='CDD')
begin
	insert into ddlUnions values ('CDD','CDD',1)
end
go

if not exists ( select 1 from dbo.ddlJobFamilys where code='U')
begin
	insert into ddlJobFamilys values ('Uniform','U')
end
go

if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlEEOJobCodes') and type in (N'U'))
CREATE TABLE dbo.ddlEEOJobCodes(
	[eeoJobCodeID] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[eeoFileStatusID] [tinyint] NOT NULL,
 CONSTRAINT [PK_vhEEOJobCodes] PRIMARY KEY CLUSTERED 
(
	[eeoJobCodeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlEEOJobTrainingStatuses') and type in (N'U'))
CREATE TABLE dbo.ddlEEOJobTrainingStatuses(
	[eeoJobTrainingStatusID] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
 CONSTRAINT [PK_vhEEOJobTrainingCodes] PRIMARY KEY CLUSTERED 
(
	[eeoJobTrainingStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


GO

if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlFLSAs') and type in (N'U'))
CREATE TABLE dbo.ddlFLSAs(
	[FLSAID] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL CONSTRAINT [DF_ddlFLSA_active]  DEFAULT (1),
 CONSTRAINT [PK_vFLSA] PRIMARY KEY CLUSTERED 
(
	[FLSAID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

go

if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlJobClasses') and type in (N'U'))
CREATE TABLE [dbo].[ddlJobClasses](
	[jobClassID] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL CONSTRAINT [DF_vJobClasses_active]  DEFAULT (1),
 CONSTRAINT [PK_vJobClasses] PRIMARY KEY CLUSTERED 
(
	[jobClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlJobFamilys') and type in (N'U'))
CREATE TABLE [dbo].[ddlJobFamilys](
	[JobFamilyId] [smallint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
 CONSTRAINT [PK_ddlJobFamily] PRIMARY KEY CLUSTERED 
(
	[JobFamilyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlUnions') and type in (N'U'))
CREATE TABLE [dbo].[ddlUnions](
	[unionID] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL CONSTRAINT [DF_vUnions_active]  DEFAULT (1),
 CONSTRAINT [PK_vUnions] PRIMARY KEY CLUSTERED 
(
	[unionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlWorkersCompensations') and type in (N'U'))
CREATE TABLE [dbo].[ddlWorkersCompensations](
	[workersCompensationID] [smallint] IDENTITY(1,1) NOT NULL,
	[code] [char](10) NULL,
	[description] [varchar](50) NULL,
 CONSTRAINT [PK_ddlHRWorkersCompensation] PRIMARY KEY CLUSTERED 
(
	[workersCompensationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

go


go


go


go

-- test Func 

go


go


go


go

--test SP 
go


go
ALTER TABLE TimeCardsArchive
ADD EmployeeId int null;
