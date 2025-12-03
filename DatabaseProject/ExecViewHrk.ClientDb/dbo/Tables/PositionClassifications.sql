CREATE TABLE [dbo].[PositionClassifications] (
    [ClassificationCriteria] VARCHAR (50) NOT NULL,
    [IsCriteriaApplicable]   BIT          CONSTRAINT [DF_PositionClassifications_IsCriteriaApplicable] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_vPositionClassifications] PRIMARY KEY CLUSTERED ([ClassificationCriteria] ASC)
);

