
Create proc [dbo].[sp_Exec_TimeCardExport]
	@CompanyCodeId varchar(50),
	@PayPeriodId varchar(50),
	@FilePath varchar(200)
AS
BEGIN
DECLARE @AllApproved bit
IF EXISTS(Select NotApproved from 
	(select EmployeeId, count(ISNULL(t.IsApproved,0)) as NoOfEntries, NotApproved = COUNT(case when t.IsApproved = 0  then 1 end)
	from Timecards t, payperiods p where p.payperiodid = @PayPeriodId and (t.ActualDate >= p.StartDate And  t.ActualDate <= p.EndDate) and
	t.CompanyCodeId = p.CompanyCodeId and t.CompanyCodeId = @CompanyCodeId
	group by EmployeeId) tc where tc.NotApproved > 0)
	-- if not approved row exists
BEGIN
	SET @AllApproved = 0		
END
ELSE
BEGIN
	SET @AllApproved = 1
	dECLARE @SQL nvarchar(4000)
	set @SQL = 'SQLCMD -S RN112-PC\SQLEXPRESS2012 -d ExecViewHrkClientTemplate -s, -W -Q "Exec sp_TimeCardExport ' + @CompanyCodeId + ',' + @PayPeriodId + ' "  | findstr /V /C:"-" /B > ' + @FilePath +''
	exec master..xp_cmdshell @SQL
END

RETURN @AllApproved
END

