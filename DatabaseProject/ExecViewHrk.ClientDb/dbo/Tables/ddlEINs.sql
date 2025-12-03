CREATE TABLE [dbo].[ddlEINs] (
    [FedralEINNbr]    INT          IDENTITY (1, 1) NOT NULL,
    [EIN]             VARCHAR (10) NOT NULL,
    [description]     VARCHAR (50) NOT NULL,
    [addressLineOne]  VARCHAR (50) NULL,
    [addressLineTwo]  VARCHAR (50) NULL,
    [city]            VARCHAR (50) NULL,
    [stateID]         INT      NULL,
    [zipCode]         VARCHAR (9)  NULL,
    [countryID]       INT     NULL,
    [phoneNumber]     VARCHAR (10) NULL,
    [faxNumber]       VARCHAR (10) NULL,
    [EEOFileStatusID] TINYINT      NULL,
    [notes]           TEXT         NULL,
    [active]          BIT          CONSTRAINT [DF_ddlEINs_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vFederalIDs] PRIMARY KEY CLUSTERED ([FedralEINNbr] ASC),
    CONSTRAINT [FK_ddlEINs_DdlCountries] FOREIGN KEY ([countryID]) REFERENCES [dbo].[DdlCountries] ([CountryId]),
    CONSTRAINT [FK_ddlEINs_DdlStates] FOREIGN KEY ([stateID]) REFERENCES [dbo].[DdlStates] ([StateId])
);

