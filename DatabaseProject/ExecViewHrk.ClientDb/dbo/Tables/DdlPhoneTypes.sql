CREATE TABLE [dbo].[DdlPhoneTypes] (
    [PhoneTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        VARCHAR (10) NOT NULL,
    [Active]      BIT          CONSTRAINT [DF_vPhoneTypes_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vPhoneTypes] PRIMARY KEY CLUSTERED ([PhoneTypeId] ASC)
);

