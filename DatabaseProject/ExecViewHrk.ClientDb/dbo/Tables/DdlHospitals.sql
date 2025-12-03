CREATE TABLE [dbo].[DdlHospitals] (
    [HospitalId]     INT           IDENTITY (1, 1) NOT NULL,
    [Description]    VARCHAR (50)  NOT NULL,
    [Code]           VARCHAR (10)  NOT NULL,
    [AddressLineOne] VARCHAR (50)  NULL,
    [AddressLineTwo] VARCHAR (50)  NULL,
    [City]           VARCHAR (50)  NULL,
    [StateId]        TINYINT       CONSTRAINT [DF_vHospitals_stateID] DEFAULT ((0)) NOT NULL,
    [ZipCode]        VARCHAR (9)   NULL,
    [CountryId]      SMALLINT      CONSTRAINT [DF_vHospitals_countryID] DEFAULT ((0)) NOT NULL,
    [PhoneNumber]    VARCHAR (10)  NULL,
    [FaxNumber]      VARCHAR (10)  NULL,
    [Contact]        VARCHAR (50)  NULL,
    [WebAddress]     VARCHAR (100) NULL,
    [Notes]          TEXT          NULL,
    [Active]         BIT           CONSTRAINT [DF_vHospitals_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vHospitals] PRIMARY KEY CLUSTERED ([HospitalId] ASC),
    CONSTRAINT [FK_DdlHospitals_DdlHospitals] FOREIGN KEY ([HospitalId]) REFERENCES [dbo].[DdlHospitals] ([HospitalId])
);

