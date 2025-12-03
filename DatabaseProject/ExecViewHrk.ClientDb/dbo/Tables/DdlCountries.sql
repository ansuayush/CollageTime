CREATE TABLE [dbo].[DdlCountries] (
    [CountryId]   INT     IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        VARCHAR (10) NOT NULL,
    [Active]      BIT          CONSTRAINT [DF_DdlCountries_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vCountries] PRIMARY KEY CLUSTERED ([CountryId] ASC)
);

