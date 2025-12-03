CREATE TABLE [dbo].[Funds] (
    [ID]                 INT             IDENTITY (1, 1) NOT NULL,
    [Description]        VARCHAR (50)    NOT NULL,
    [code]               VARCHAR (10)    NOT NULL,
    [Active]             BIT             CONSTRAINT [DF_Funds_Active] DEFAULT ((1)) NOT NULL,
    [Amount]             DECIMAL (18, 2) NULL,
    [FTE]                DECIMAL (18, 2) NULL,
    [EffectiveStartDate] DATETIME        NULL,
    [EffectiveEndDate]   DATETIME        NULL,
    CONSTRAINT [PK_Funds] PRIMARY KEY CLUSTERED ([ID] ASC)
);

