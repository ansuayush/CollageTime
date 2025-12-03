



CREATE PROCEDURE [dbo].[sp_TimeCardExport]
	@CompanyCodeId int = NULL,
	@PayPeriodId int = NULL
AS
BEGIN   
	-- Check If not approved rows exists
	--IF EXISTS(Select NotApproved from 
	--(select EmployeeId, count(ISNULL(t.IsApproved,0)) as NoOfEntries, NotApproved = COUNT(case when t.IsApproved = 0  then 1 end)
	--from Timecards t, payperiods p where p.payperiodid = @PayPeriodId and (t.ActualDate >= p.StartDate And  t.ActualDate <= p.EndDate) and
	--t.CompanyCodeId = p.CompanyCodeId and t.CompanyCodeId = @CompanyCodeId
	--group by EmployeeId) tc where tc.NotApproved > 0)
	--BEGIN  -- if not approved row exists
	--	select 'not approved'
	--END
	--ELSE
	BEGIN
		--select ' approved'
		SET NOCOUNT ON
		select h.CompanyCodeId as [Co Code], h.CompanyCodeId as [Batch ID], h.FileNumber as [File #], [Reg Hours] = case when (TotalRegularHrsPerPayPeriod + Hrs_HoursCode) > 80 Then
		TotalRegularHrsPerPayPeriod - ((TotalRegularHrsPerPayPeriod + Hrs_HoursCode)-80) else ISNULL(NULLIF(TotalRegularHrsPerPayPeriod,0),'') end,
		[O/T Hours] = case when (TotalRegularHrsPerPayPeriod + Hrs_HoursCode) > 80 Then
		((TotalRegularHrsPerPayPeriod + Hrs_HoursCode)-80) else 0 end, 
		NULL as [Hours 3 Code], NULL as [Hours 3 Amount], NULL as [Hours 4 Code], NULL as [Hours 4 Amount], NULL as  [Hours 5 Code], NULL as [Hours 5 Amount],
		NULL as [Earnings 3 Code] , NULL as [Earnings 3 Amount], NULL as [Earnings 4 Code], NULL as [Earnings 4 Amount], NULL as [Earnings 5 Code], NULL as [Earnings 5 Amount]
		from
		(select e.FileNumber, t.CompanyCodeId,
		sum(cast(Case When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NOT NULL) and (LunchBack IS NOT NULL) Then
		Convert( Varchar(5),(DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))/60) + '.' + 
		RIGHT('0' + CAST((DATEDIFF(MINUTE, TimeIn, TimeOut)-DATEDIFF(MINUTE, LunchOut, LunchBack))%60 AS varchar(2)),2)
		When (TimeIn IS NOT NULL) and (TimeOut IS NOT NULL) and (LunchOut IS NULL) and (LunchBack IS NULL) Then
		Convert( Varchar(5),DATEDIFF(MINUTE, TimeIn, TimeOut)/60) + '.'+RIGHT('0' + CAST(DATEDIFF(MINUTE, TimeIn, TimeOut) % 60 AS varchar(2)),2)
		Else NULL End as Decimal(5,2))) as TotalRegularHrsPerPayPeriod, sum(t.Hours) as Hrs_HoursCode
		from TimeCards t, PayPeriods p, Employees e
		where t.CompanyCodeId = p.CompanyCodeId and t.EmployeeId = e.EmployeeId and (t.ActualDate >= p.StartDate And  t.ActualDate <= p.EndDate)
		and p.PayPeriodId= @PayPeriodId and t.CompanyCodeId = @CompanyCodeId
		group by e.FileNumber, t.CompanyCodeId) as h
		Union
		select t.CompanyCodeId as [Co Code], t.CompanyCodeId as [Batch ID], e.FileNumber as [File #], NULL as [Reg Hours], NULL as [O/T Hours],
		[Hours 3 Code] = case when ADPFieldMappingId = 1 then h.HoursCodeCode end, [Hours 3 Amount] = case when ADPFieldMappingId = 1 then sum(t.Hours) end,
			[Hours 4 Code] = case when ADPFieldMappingId = 2 then h.HoursCodeCode end,  [Hours 4 Amount] = case when ADPFieldMappingId = 2 then sum(t.Hours) end,
			[Hours 5 Code] = case when ADPFieldMappingId =3 then h.HoursCodeCode end,  [Hours 5 Amount] = case when ADPFieldMappingId = 3 then sum(t.Hours) end,
			NULL as [Earnings 3 Code] , NULL as [Earnings 3 Amount], NULL as [Earnings 4 Code], NULL as [Earnings 4 Amount], NULL as [Earnings 5 Code], NULL as [Earnings 5 Amount]
		from TimeCards t, Employees e, PayPeriods p, HoursCodes h
		where t.EmployeeId = e.EmployeeId and e.CompanyCodeId = t.CompanyCodeId and 
		t.HoursCodeId = h.HoursCodeId and t.CompanyCodeId = h.CompanyCodeId and t.CompanyCodeId = p.CompanyCodeId
		and (t.ActualDate >= p.StartDate And  t.ActualDate <= p.EndDate) 
		and t.CompanyCodeId = @CompanyCodeId and p.payperiodid = @PayPeriodId and t.HoursCodeId IS NOT NULL
		group by t.CompanyCodeId, e.FileNumber, h.HoursCodeCode ,h.ADPFieldMappingId 
		UNION
		select t.CompanyCodeId as [Co Code], t.CompanyCodeId as [Batch ID], e.FileNumber as [File #], NULL as [Reg Hours], NULL as [O/T Hours],
		NULL as [Hours 3 Code], '' as [Hours 3 Amount], NULL as [Hours 4 Code], NULL as [Hours 4 Amount], '' as  [Hours 5 Code], NULL as [Hours 5 Amount],
		[Earnings 3 Code] = case when ea.ADPFieldMappingId = 1 then ea.EarningsCodeCode end, [Earnings 3 Amount] = case when ea.ADPFieldMappingId = 1 then sum(t.EarningsAmount) end,
		[Earnings 4 Code] = case when ea.ADPFieldMappingId = 2 then ea.EarningsCodeCode end,  [Earnings 4 Amount] = case when ADPFieldMappingId = 2 then sum(t.EarningsAmount) end,
		[Earnings 5 Code] = case when ea.ADPFieldMappingId =3 then ea.EarningsCodeCode end,  [Earnings 5 Amount] = case when ADPFieldMappingId = 3 then sum(t.EarningsAmount) end
		from TimeCards t, Employees e, PayPeriods p, EarningsCodes ea
		where t.EmployeeId = e.EmployeeId and e.CompanyCodeId = t.CompanyCodeId and 
		t.EarningsCodeId = ea.EarningsCodeId and t.CompanyCodeId = ea.CompanyCodeId and t.CompanyCodeId = p.CompanyCodeId
		and (t.ActualDate >= p.StartDate And  t.ActualDate <= p.EndDate) 
		and t.CompanyCodeId = @CompanyCodeId and p.payperiodid = @PayPeriodId and t.EarningsCodeId IS NOT NULL
		group by t.CompanyCodeId, e.FileNumber, ea.EarningsCodeCode ,ea.ADPFieldMappingId
		order by [File #]			
	END	
END


