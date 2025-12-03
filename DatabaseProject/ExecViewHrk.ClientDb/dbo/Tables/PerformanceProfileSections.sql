CREATE TABLE [dbo].[PerformanceProfileSections] (
    [ID]            INT             IDENTITY (1, 1) NOT NULL,
    [SectionName]   VARCHAR (50)    NOT NULL,
    [Header]        VARCHAR (200)   NOT NULL,
    [PerProfileID]  INT             NOT NULL,
    [NumRows]       INT             CONSTRAINT [DF_PerformanceProfileSections_NumRows] DEFAULT ((5)) NOT NULL,
    [MaxCharacters] INT             CONSTRAINT [DF_PerformanceProfileSections_MaxCharacters] DEFAULT ((200)) NOT NULL,
    [Weight]        DECIMAL (18, 2) CONSTRAINT [DF_PerformanceProfileSections_Weight] DEFAULT ((100.0)) NOT NULL,
    [Position]      INT             NOT NULL,
    [EnteredBy]     VARCHAR (50)    NULL,
    [EnteredDate]   SMALLDATETIME   NULL,
    [ModifiedBy]    VARCHAR (50)    NULL,
    [ModifiedDate]  SMALLDATETIME   NULL,
    CONSTRAINT [PK_PerformanceProfileSections] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PerformanceProfileSections_PerformanceProfile] FOREIGN KEY ([PerProfileID]) REFERENCES [dbo].[PerformanceProfiles] ([PerProfileID]),
    CONSTRAINT [IX_PerformanceProfileSections] UNIQUE NONCLUSTERED ([PerProfileID] ASC, [SectionName] ASC)
);

