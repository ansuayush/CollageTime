CREATE TABLE [dbo].[DdlI9DocumentTypes] (
    [I9DocumentTypeId] TINYINT      IDENTITY (1, 1) NOT NULL,
    [Description]      VARCHAR (50) NOT NULL,
    [Code]             VARCHAR (10) NOT NULL,
    [Active]           BIT          CONSTRAINT [DF_vI9DocumentTypes_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vI9Documents] PRIMARY KEY CLUSTERED ([I9DocumentTypeId] ASC)
);

