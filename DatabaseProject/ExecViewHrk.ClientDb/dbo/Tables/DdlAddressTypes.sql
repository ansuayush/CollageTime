CREATE TABLE [dbo].[DdlAddressTypes] (
    [AddressTypeId] INT      IDENTITY (1, 1) NOT NULL,
    [Description]   VARCHAR (50) NOT NULL,
    [Code]          VARCHAR (10) NOT NULL,
    [Active]        BIT          CONSTRAINT [DF_vAddressTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vAddressTypes] PRIMARY KEY CLUSTERED ([AddressTypeId] ASC)
);

