CREATE TABLE [dbo].[ddlFLSAs] (
    [FLSAID]      SMALLINT     IDENTITY (1, 1) NOT NULL,
    [description] VARCHAR (50) NOT NULL,
    [code]        VARCHAR (10) NOT NULL,
    [active]      BIT          CONSTRAINT [DF_ddlFLSA_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vFLSA] PRIMARY KEY CLUSTERED ([FLSAID] ASC)
);

