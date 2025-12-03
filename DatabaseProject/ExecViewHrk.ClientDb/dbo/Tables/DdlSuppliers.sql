CREATE TABLE [dbo].[DdlSuppliers] (
    [SupplierId]    INT           IDENTITY (1, 1) NOT NULL,
    [Code]          VARCHAR (10)  NOT NULL,
    [Title]         VARCHAR (50)  NOT NULL,
    [AddressLine1]  VARCHAR (50)  NULL,
    [AddressLine2]  VARCHAR (50)  NULL,
    [City]          VARCHAR (50)  NULL,
    [StateId]       INT       NULL,
    [ZipCode]       VARCHAR (9)   NULL,
    [CountryId]     INT      NULL,
    [Phone]         VARCHAR (10)  NULL,
    [Fax]           VARCHAR (10)  NULL,
    [Contact]       VARCHAR (50)  NULL,
    [WebPage]       VARCHAR (100) NULL,
    [AccountNumber] VARCHAR (50)  NULL,
    [Notes]         TEXT          NULL,
    CONSTRAINT [PK_vSuppliers] PRIMARY KEY CLUSTERED ([SupplierId] ASC),
    CONSTRAINT [FK_DdlSuppliers_DdlCountries] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[DdlCountries] ([CountryId]),
    CONSTRAINT [FK_DdlSuppliers_DdlStates] FOREIGN KEY ([StateId]) REFERENCES [dbo].[DdlStates] ([StateId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_DdlSuppliers]
    ON [dbo].[DdlSuppliers]([Code] ASC);

