CREATE TABLE [dbo].[UserDefinedSegment2s] (
    [UserDefinedSegment2Id]          INT     IDENTITY (1, 1) NOT NULL,
    [UserDefinedSegment2Code]        VARCHAR (10) NOT NULL,
    [UserDefinedSegment2Description] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_vUserDefinedSegment2s] PRIMARY KEY CLUSTERED ([UserDefinedSegment2Id] ASC)
);

