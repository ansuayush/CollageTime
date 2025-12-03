CREATE TABLE [dbo].[ManagersPositions] (
    [ID]         INT      IDENTITY (1, 1) NOT NULL,
    [ManagerID]  INT      NOT NULL,
    [PositionID] INT NOT NULL,
    CONSTRAINT [PK_ManagersPositions] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ManagersPositions_Managers] FOREIGN KEY ([ManagerID]) REFERENCES [dbo].[Managers] ([ManagerId]),
    CONSTRAINT [FK_ManagersPositions_Positions] FOREIGN KEY ([PositionID]) REFERENCES [dbo].[Positions] ([PositionId]),
    CONSTRAINT [IX_ManagersPositions] UNIQUE NONCLUSTERED ([ManagerID] ASC, [PositionID] ASC)
);

