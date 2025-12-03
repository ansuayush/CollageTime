    
 Alter PROCEDURE [dbo].[sp_GetTimeCardApprovalReportList]        
 @compCodeId int,                        
 @deptId int,                        
 @payPeriodId int                  
 AS                        
BEGIN                         
            
select t.EmployeeId, sum(Isnull(t.DailyHours,0)) as RegularHours,  OverTime = case when (sum(Isnull(t.DailyHours,0))) > 80 Then                      
((sum(Isnull(t.DailyHours,0)))-80) else 0 end, CodedHours=0, Approved =1        
from TimeCards t, PayPeriods p, Employees e    
where    
e.EmployeeId in (t.EmployeeId) and    
e.Isstudent =1 and  t.IsDeleted = 0 and  
t.CompanyCodeId = @compCodeId and    
--t.DepartmentId = @deptId and    
t.IsApproved=1 and    
p.PayPeriodId = @payPeriodId And     
t.ActualDate between p.StartDate AND p.EndDate    
group by t.EmployeeId    
end 