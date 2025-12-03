CREATE TABLE [dbo].[DdlQualificationTypes] (
    [QualificationTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]         VARCHAR (50) NOT NULL,
    [Code]                VARCHAR (10) NOT NULL,
    [Active]              BIT          CONSTRAINT [DF_vQualificationTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vQualificationTypes] PRIMARY KEY CLUSTERED ([QualificationTypeId] ASC)
);

