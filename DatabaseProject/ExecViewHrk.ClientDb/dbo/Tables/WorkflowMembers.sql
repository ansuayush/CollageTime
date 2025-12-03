CREATE TABLE [dbo].[WorkflowMembers] (
    [WorkflowMemberId] INT            IDENTITY (1, 1) NOT NULL,
    [WorkflowId]       INT            NOT NULL,
    [IsGroup]          BIT            NOT NULL,
    [UserOrGroupName]  VARCHAR (100)  NOT NULL,
    [Position]         DECIMAL (6, 2) NOT NULL,
    CONSTRAINT [PK_WorkflowMembers] PRIMARY KEY CLUSTERED ([WorkflowMemberId] ASC),
    CONSTRAINT [FK_WorkflowMembers_Workflows] FOREIGN KEY ([WorkflowId]) REFERENCES [dbo].[Workflows] ([WorkflowId])
);

