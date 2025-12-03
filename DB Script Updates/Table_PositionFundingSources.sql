
if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.PositionFundingSources') and type in (N'U'))
begin
create table PositionFundingSources(
PositionFundingSourceID [int] IDENTITY(1,1) NOT NULL,
EffectiveDate [smalldatetime] NULL,
FundCodeID [tinyint] NOT NULL,
Percentage [tinyint] NULL,
)
end

go


if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.PositionFundingSourceHistories') and type in (N'U'))
begin
create table PositionFundingSourceHistories(
FundingSourceHistoriesID [int] IDENTITY(1,1) NOT NULL,
EffectiveDate [smalldatetime] NULL,
FundCodeID [tinyint] NOT NULL,
Percentage [tinyint] NULL,
ChangeEffectiveDate [smalldatetime] NULL,
PositionFundingSourceID  [tinyint] NOT NULL
)
end

go



if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.PositionSalaryGradeSourceHistories') and type in (N'U'))
begin
create table PositionSalaryGradeSourceHistories(
PositionSalaryGradeHistoriesID int IDENTITY(1,1) NOT NULL,
SalaryGradeID tinyint NOT NULL,
ValidDate smalldatetime NULL,
salaryMinimum money NOT NULL,
salaryMidpoint money NOT NULL,
salaryMaximum money NOT NULL,
ChangeEffectiveDate smalldatetime NULL,
DdlSalaryGradeHistoriesID tinyint NOT NULL
)

end

go