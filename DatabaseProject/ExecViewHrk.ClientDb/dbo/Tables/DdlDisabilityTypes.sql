CREATE TABLE [dbo].[DdlDisabilityTypes] (
    [DisabilityTypeId] int     IDENTITY (1, 1) NOT NULL,
    [Description]      VARCHAR (50) NOT NULL,
    [Code]             VARCHAR (10) NOT NULL,
    [Active]           BIT          CONSTRAINT [DF_ddlDisabilityTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vDisabilities] PRIMARY KEY CLUSTERED ([DisabilityTypeId] ASC)
);

