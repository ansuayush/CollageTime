CREATE TABLE [dbo].[DdlInnoculationTypes] (
    [InnoculationTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]        VARCHAR (50) NOT NULL,
    [Code]               VARCHAR (10) NOT NULL,
    [Active]             BIT          CONSTRAINT [DF_vInnoculationTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vInnoculationTypes] PRIMARY KEY CLUSTERED ([InnoculationTypeId] ASC)
);

