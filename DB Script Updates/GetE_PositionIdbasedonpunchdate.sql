CREATE proc GetE_PositionIdbasedonpunchdate      
@EmployeeId int,      
@positionId int,      
@Companycodeid int      
as      
begin      
select E_PositionId      
From e_positions ee      
inner join  positions ep on ee.PositionId = ep.PositionId      
inner join Employees em on ee.EmployeeId=em.EmployeeId      
where ee.employeeid =@EmployeeId and ee.PositionId=@positionId and em.companycodeid=@Companycodeid   
--and isnull(ee.actualEndDate,GetDate())>=convert(date,GetDate())      
end