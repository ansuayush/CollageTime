CREATE TABLE [dbo].[ddlSalaryComponents] (
    [salaryComponentID]     INT     IDENTITY (1, 1) NOT NULL,
    [description]           VARCHAR (50) NOT NULL,
    [code]                  VARCHAR (10) NOT NULL,
    [salaryComponentTypeID] INT     NOT NULL,
    [payTypeID]             INT     NULL,
    [payFrequencyCode]      VARCHAR (10) NOT NULL,
    [benefits]              BIT          NOT NULL,
    [Base]                  BIT          NOT NULL,
    [total]                 BIT          NOT NULL,
    [active]                BIT          CONSTRAINT [DF_vSalaryComponents_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vSalaryComponents] PRIMARY KEY CLUSTERED ([salaryComponentID] ASC)
);

