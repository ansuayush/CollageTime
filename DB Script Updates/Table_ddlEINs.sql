
if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlEINs'))
begin
CREATE TABLE [dbo].[ddlEINs](
	[FedralEINNbr] [int] IDENTITY(1,1) NOT NULL,
	[EIN] [varchar](10) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[addressLineOne] [varchar](50) NULL,
	[addressLineTwo] [varchar](50) NULL,
	[city] [varchar](50) NULL,
	[stateID] [tinyint] NULL,
	[zipCode] [varchar](9) NULL,
	[countryID] [smallint] NULL,
	[phoneNumber] [varchar](10) NULL,
	[faxNumber] [varchar](10) NULL,
	[EEOFileStatusID] [tinyint] NULL,
	[notes] [text] NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK_vFederalIDs] PRIMARY KEY CLUSTERED 
(
	[FedralEINNbr] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[DF_ddlEINs_active]'))
BEGIN
	ALTER TABLE [dbo].[ddlEINs]  ADD  CONSTRAINT [DF_ddlEINs_active]  DEFAULT ((1)) FOR [active]
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_ddlEINs_DdlCountries]'))
BEGIN
	ALTER TABLE [dbo].[ddlEINs]  WITH CHECK ADD  CONSTRAINT [FK_ddlEINs_DdlCountries] FOREIGN KEY([countryID])
	REFERENCES [dbo].[DdlCountries] ([CountryId])
END
GO



IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_ddlEINs_DdlStates]'))
BEGIN
	ALTER TABLE [dbo].[ddlEINs]  WITH CHECK ADD  CONSTRAINT [FK_ddlEINs_DdlStates] FOREIGN KEY([stateID])
	REFERENCES [dbo].[DdlStates] ([StateId])
END
GO





