CREATE TABLE [dbo].[DdlProfessionalBodies] (
    [ProfessionalBodyId] INT      IDENTITY (1, 1) NOT NULL,
    [Description]        VARCHAR (50)  NOT NULL,
    [Code]               VARCHAR (10)  NOT NULL,
    [WebAddress]         VARCHAR (100) NULL,
    [Active]             BIT           CONSTRAINT [DF_ddlProfessionalBodies_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vProfessionalBodies] PRIMARY KEY CLUSTERED ([ProfessionalBodyId] ASC)
);

