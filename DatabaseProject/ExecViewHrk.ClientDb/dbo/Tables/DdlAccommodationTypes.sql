CREATE TABLE [dbo].[DdlAccommodationTypes] (
    [AccommodationTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]         VARCHAR (50) NOT NULL,
    [Code]                VARCHAR (10) NOT NULL,
    [Active]              BIT          CONSTRAINT [DF_vAccomodationTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vAccomodations] PRIMARY KEY CLUSTERED ([AccommodationTypeId] ASC)
);

