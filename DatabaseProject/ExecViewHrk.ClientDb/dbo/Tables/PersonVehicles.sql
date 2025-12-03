CREATE TABLE [dbo].[PersonVehicles] (
    [PersonVehicleId] INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]        INT           NOT NULL,
    [LicenseNumber]   VARCHAR (50)  NOT NULL,
    [Make]            VARCHAR (50)  NULL,
    [Model]           VARCHAR (50)  NULL,
    [Color]           VARCHAR (50)  NULL,
    [StateId]         INT       NULL,
    [AcquisitionDate] SMALLDATETIME NULL,
    [SoldDate]        SMALLDATETIME NULL,
    [Notes]           TEXT          NULL,
    [EnteredBy]       VARCHAR (50)  NOT NULL,
    [EnteredDate]     SMALLDATETIME NOT NULL,
    [ModifiedBy]      VARCHAR (50)  NULL,
    [ModifiedDate]    SMALLDATETIME NULL,
    CONSTRAINT [PK_pVehicles] PRIMARY KEY CLUSTERED ([PersonVehicleId] ASC),
    CONSTRAINT [FK_PersonVehicles_DdlStates] FOREIGN KEY ([StateId]) REFERENCES [dbo].[DdlStates] ([StateId]),
    CONSTRAINT [FK_PersonVehicles_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);

