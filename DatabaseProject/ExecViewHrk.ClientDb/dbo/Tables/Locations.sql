CREATE TABLE [dbo].[Locations] (
    [LocationId]          INT     IDENTITY (1, 1) NOT NULL,
    [LocationCode]        VARCHAR (10) NOT NULL,
    [LocationDescription] VARCHAR (50) NOT NULL,
    [Active]              BIT          DEFAULT ((1)) NULL,
    CONSTRAINT [PK_vLocations] PRIMARY KEY CLUSTERED ([LocationId] ASC)
);

