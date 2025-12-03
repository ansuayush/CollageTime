GO

/****** Object:  StoredProcedure [dbo].[sp_GetPayPeriodList]    Script Date: 5/28/2018 4:22:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_GetPayPeriodList]        
AS        
BEGIN         
select p.PayPeriodId ,p.PayFrequencyId,p.StartDate,p.EndDate,p.CompanyCodeId,cc.CompanyCodeCode as CompanyCode,ddl.Description as PayFrequencyName,p.IsPayPeriodActive,p.IsPayPeriodActive,p.LockoutEmployees,p.LockoutManagers,p.IsArchived,        
p.PayGroupCode,p.PayPeriodNumber,dp.Description,      
        
((SELECT  Convert( Varchar(20),COUNT(*)) FROM ManagerLockouts WHERE PayPeriodID =p.PayPeriodId) +'/'+        
            
 (select Convert( Varchar(20),COUNT(*)) FROM Persons         
             INNER JOIN Employees on Employees.PersonID = Persons.PersonId         
                INNER JOIN E_Positions On E_Positions.EmployeeID = Employees.EmployeeId         
    INNER JOIN UserNamesPersons as us on Persons.PersonId = us.PersonID        
                INNER JOIN Positions On Positions.PositionId = E_Positions.[PositionId]         
                WHERE Positions.PositionId IN (Select reportsToPositionID FROM Positions))) as ManagersCount,      
  PayPeriodStartDate = convert(varchar(20),p.StartDate,112),      
  PayPeriodEndDate = convert(varchar(20),p.EndDate,112),
  p.IsDeleted
  from PayPeriods p        
join DdlPayFrequencies ddl on p.PayFrequencyId=ddl.PayFrequencyId        
join CompanyCodes cc on cc.CompanyCodeId= p.CompanyCodeId       
join DdlPayGroups dp on dp.Code= p.PayGroupCode
where p.IsArchived=0 and p.IsDeleted = 0
        
End
GO


