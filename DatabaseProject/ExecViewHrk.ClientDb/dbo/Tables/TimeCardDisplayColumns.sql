CREATE TABLE [dbo].[TimeCardDisplayColumns] (
    [TimeCardTypeId] INT NOT NULL,
    [Day]            BIT      DEFAULT ((1)) NOT NULL,
    [ActualDate]     BIT      DEFAULT ((1)) NOT NULL,
    [DailyHours]     BIT      DEFAULT ((1)) NOT NULL,
    [HoursCodeId]    BIT      DEFAULT ((1)) NOT NULL,
    [Hours]          BIT      DEFAULT ((1)) NOT NULL,
    [EarningsCodeId] BIT      DEFAULT ((1)) NOT NULL,
    [EarningsAmount] BIT      DEFAULT ((1)) NOT NULL,
    [TempDeptId]     BIT      DEFAULT ((1)) NOT NULL,
    [TempJobId]      BIT      DEFAULT ((1)) NOT NULL,
    [TimeIn]         BIT      DEFAULT ((1)) NOT NULL,
    [TimeOut]        BIT      DEFAULT ((1)) NOT NULL,
    [LunchOut]       BIT      DEFAULT ((1)) NOT NULL,
    [LunchBack]      BIT      DEFAULT ((1)) NOT NULL,
    [IsApproved]     BIT      DEFAULT ((1)) NOT NULL,
    [ApprovedBy]     BIT      DEFAULT ((1)) NOT NULL,
    [AutoFill]       BIT      DEFAULT ((0)) NOT NULL,
    [Total]          BIT      DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_TimeCardDisplayColumns] PRIMARY KEY CLUSTERED ([TimeCardTypeId] ASC),
    CONSTRAINT [FK_TimeCardDisplayColumns_DdlTimeCardTypes] FOREIGN KEY ([TimeCardTypeId]) REFERENCES [dbo].[DdlTimeCardTypes] ([TimeCardTypeId])
);

