CREATE TABLE [dbo].[ddlSalaryGradeBands] (
    [id]          INT     IDENTITY (1, 1) NOT NULL,
    [description] VARCHAR (50) NOT NULL,
    [code]        VARCHAR (10) NOT NULL,
    [active]      BIT          CONSTRAINT [DF_vSalaryGradeBands_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vSalaryGradeBands] PRIMARY KEY CLUSTERED ([id] ASC)
);

