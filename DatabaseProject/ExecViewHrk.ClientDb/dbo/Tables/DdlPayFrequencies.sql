CREATE TABLE [dbo].[DdlPayFrequencies] (
    [PayFrequencyId] INT      IDENTITY (1, 1) NOT NULL,
    [Code]           VARCHAR (10) NOT NULL,
    [Description]    VARCHAR (50) NOT NULL,
    [Active]         BIT          CONSTRAINT [DF_DdlPayFrequencies_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vhPayFrequencies] PRIMARY KEY CLUSTERED ([PayFrequencyId] ASC)
);

