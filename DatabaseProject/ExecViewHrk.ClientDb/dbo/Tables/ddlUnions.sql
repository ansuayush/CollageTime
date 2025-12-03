CREATE TABLE [dbo].[ddlUnions] (
    [unionID]     SMALLINT     IDENTITY (1, 1) NOT NULL,
    [description] VARCHAR (50) NOT NULL,
    [code]        VARCHAR (10) NOT NULL,
    [active]      BIT          CONSTRAINT [DF_vUnions_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vUnions] PRIMARY KEY CLUSTERED ([unionID] ASC)
);

