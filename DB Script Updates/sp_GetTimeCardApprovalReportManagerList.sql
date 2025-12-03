    
                         
Alter PROCEDURE [dbo].[sp_GetTimeCardApprovalReportManagerList]                
 @compCodeId int,                                      
 @deptId int,                                      
 @payPeriodId int,                    
 @email varchar(100)                    
 AS              
BEGIN              
--Select t.EmployeeId, sum(Isnull(t.DailyHours,0)) as RegularHours,  OverTime = case when (sum(Isnull(t.DailyHours,0))) > 80 Then                              
--((sum(Isnull(t.DailyHours,0)))-80) else 0 end, CodedHours=0, Approved =1              
--from TimeCards t, PayPeriods p, Persons pe, E_Positions ep,Employees e      
--where       
--e.EmployeeId in (t.EmployeeId) and    
--e.Isstudent =1 and           
--t.CompanyCodeId = @compCodeId and                 
----t.DepartmentId = @deptId and                
--t.IsApproved=1 and                
--p.PayPeriodId = @payPeriodId And      
--pe.eMail = @email And      
--ep.ReportsToID = pe.PersonId And      
--t.ActualDate between p.StartDate AND p.EndDate And      
----and t.ApprovedBy=@email      
--t.EmployeeId in (ep.EmployeeId) And      
--t.PositionId in (ep.PositionId)      
--group by t.EmployeeId     
         
        
declare @PersonId int=  ( Select PersonId from Persons where Email = @email  );     
declare @designPersonemail varchar(100) = (sELECT top 1 Persons.eMail FROM DesignatedSupervisors     
join Persons on Persons.PersonId = DesignatedSupervisors.ManagerPersonId    
where DesignatedSupervisors.DesignatedManagerPersonId in (@PersonId))     
    
    
Select     
 t.EmployeeId, sum(Isnull(t.DailyHours,0)) as RegularHours    
 ,  OverTime = case when (sum(Isnull(t.DailyHours,0))) > 80 Then  ((sum(Isnull(t.DailyHours,0)))-80) else 0 end    
 , CodedHours=0    
 , Approved =1              
from TimeCards t, PayPeriods p, Persons pe, E_Positions ep,Employees e      
where       
e.EmployeeId in (t.EmployeeId) and    
e.Isstudent =1 and  t.IsDeleted = 0 and         
t.CompanyCodeId = @compCodeId and                 
--t.DepartmentId = @deptId and                
t.IsApproved=1 and                
p.PayPeriodId = @payPeriodId And ( pe.eMail = @email or pe.eMail = @designPersonemail)    
And      
ep.ReportsToID = pe.PersonId And      
t.ActualDate between p.StartDate AND p.EndDate And      
--and t.ApprovedBy=@email      
t.EmployeeId in (ep.EmployeeId) And      
t.PositionId in (ep.PositionId)      
group by t.EmployeeId            
    
    
    
    
    
    
    
    
    
    
    
    
    
           
End 