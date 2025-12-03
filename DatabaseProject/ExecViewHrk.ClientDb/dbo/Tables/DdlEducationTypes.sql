CREATE TABLE [dbo].[DdlEducationTypes] (
    [EducationTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]     VARCHAR (50) NOT NULL,
    [Code]            VARCHAR (10) NOT NULL,
    [Active]          BIT          CONSTRAINT [DF_vEducationTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vEducationTypes] PRIMARY KEY CLUSTERED ([EducationTypeId] ASC)
);

