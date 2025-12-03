CREATE TABLE [dbo].[UserDefinedSegment1s] (
    [UserDefinedSegment1Id]          INT     IDENTITY (1, 1) NOT NULL,
    [UserDefinedSegment1Code]        VARCHAR (10) NOT NULL,
    [UserDefinedSegment1Description] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_vUserDefinedSegment1s] PRIMARY KEY CLUSTERED ([UserDefinedSegment1Id] ASC)
);

