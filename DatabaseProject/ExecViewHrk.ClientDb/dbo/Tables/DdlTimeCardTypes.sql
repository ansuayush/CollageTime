CREATE TABLE [dbo].[DdlTimeCardTypes] (
    [TimeCardTypeId]          INT     IDENTITY (1, 1) NOT NULL,
    [TimeCardTypeCode]        VARCHAR (10) NOT NULL,
    [TimeCardTypeDescription] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_vTimeCardTypes] PRIMARY KEY CLUSTERED ([TimeCardTypeId] ASC)
);

