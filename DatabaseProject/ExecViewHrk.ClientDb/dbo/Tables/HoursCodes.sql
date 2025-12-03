CREATE TABLE [dbo].[HoursCodes] (
    [HoursCodeId]          INT           IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]        INT      NOT NULL,
    [HoursCodeCode]        VARCHAR (10)  NOT NULL,
    [HoursCodeDescription] NVARCHAR (50) NOT NULL,
    [ADPFieldMappingId]    INT       NOT NULL,
    [ADPAccNumberId]       INT       NULL,
    [RateOverride]         DECIMAL (18)  NULL,
    [RateMultiplier]       DECIMAL (18)  NULL,
    [ExcludeFromOT]        BIT           NOT NULL,
    [SubtractFromRegular]  BIT           NOT NULL,
    [NonPayCode]           BIT           NOT NULL,
    [IsHoursCodeActive]    BIT           CONSTRAINT [DF_vHoursCodes_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_HoursCodes] PRIMARY KEY CLUSTERED ([HoursCodeId] ASC),
    CONSTRAINT [FK_HoursCodes_ADPAccNumbers] FOREIGN KEY ([ADPAccNumberId]) REFERENCES [dbo].[ADPAccNumbers] ([ADPAccNumberId]),
    CONSTRAINT [FK_HoursCodes_ADPFieldMappings] FOREIGN KEY ([ADPFieldMappingId]) REFERENCES [dbo].[ADPFieldMappings] ([ADPFieldMappingId]),
    CONSTRAINT [FK_HoursCodes_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [uq_HoursCodes] UNIQUE NONCLUSTERED ([CompanyCodeId] ASC, [HoursCodeCode] ASC)
);

