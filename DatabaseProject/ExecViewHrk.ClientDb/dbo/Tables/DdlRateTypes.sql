CREATE TABLE [dbo].[DdlRateTypes] (
    [RateTypeId]  INT          IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        CHAR (1)     NOT NULL,
    [Active]      BIT          CONSTRAINT [DF_DdlRateTypes_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_DdlRateTypes] PRIMARY KEY CLUSTERED ([RateTypeId] ASC)
);

