CREATE TABLE [dbo].[PerformanceProfiles] (
    [PerProfileID] INT          IDENTITY (1, 1) NOT NULL,
    [Code]         VARCHAR (50) NOT NULL,
    [Description]  VARCHAR (50) CONSTRAINT [DF_PerformanceProfile_Description] DEFAULT ('') NOT NULL,
    [Active]       BIT          CONSTRAINT [DF_PerformanceProfiles_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_PerformanceProfile] PRIMARY KEY CLUSTERED ([PerProfileID] ASC)
);

