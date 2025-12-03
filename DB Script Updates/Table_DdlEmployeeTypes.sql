
if not exists (select 1 from sys.objects where object_id = object_id(N'DdlEmployeeTypes'))
begin
CREATE TABLE [dbo].[DdlEmployeeTypes](
	[EmployeeTypeId] [smallint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_prsEmployeeTypes] PRIMARY KEY CLUSTERED 
(
	[EmployeeTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[DF_DdlEmployeeTypes_Active]'))
BEGIN
	ALTER TABLE [dbo].[DdlEmployeeTypes] ADD  CONSTRAINT [DF_DdlEmployeeTypes_Active]  DEFAULT ((1)) FOR [Active]
END
GO

if not exists ( select 1 from DdlEmployeeTypes where code='F')
begin
	SET IDENTITY_INSERT [dbo].[DdlEmployeeTypes] ON 

	INSERT [dbo].[DdlEmployeeTypes] ([EmployeeTypeId], [Description], [Code], [Active]) VALUES (1, N'Full Time', N'F', 1)
	
	SET IDENTITY_INSERT [dbo].[DdlEmployeeTypes] OFF
end
go

if not exists ( select 1 from DdlEmployeeTypes where code='P')
begin
	SET IDENTITY_INSERT [dbo].[DdlEmployeeTypes] ON 

	INSERT [dbo].[DdlEmployeeTypes] ([EmployeeTypeId], [Description], [Code], [Active]) VALUES (2, N'Part Time', N'P', 1)
	
	SET IDENTITY_INSERT [dbo].[DdlEmployeeTypes] OFF
end
go