
CREATE  PROCEDURE [dbo].[uspOpenPositionReport]   
   
AS 
BEGIN  

SET NOCOUNT ON;   
DECLARE @Report TABLE   ( PositionCode VARCHAR(20),PositionTitle VARCHAR(150), TotalSlots INT, SlotsFilled INT, SlotsLeft INT )  

INSERT INTO @Report(PositionCode,PositionTitle,TotalSlots,SlotsFilled,SlotsLeft)  
SELECT PositionCode ,PositionDescription,
CASE WHEN TotalSlots IS NULL THEN 
(
(select Count(1) from E_Positions  join employees on employees.EmployeeId = E_Positions.EmployeeId   
join ddlEmploymentStatuses es on es.EmploymentStatusId = employees.EmploymentStatusId   
 where E_Positions.PositionId = Positions.PositionId and E_Positions.actualEndDate is null and es.Code = 'T' )
 +
 (select Count(1) from E_Positions  join employees on employees.EmployeeId = E_Positions.EmployeeId   
join ddlEmploymentStatuses es on es.EmploymentStatusId = employees.EmploymentStatusId   
 where E_Positions.PositionId = Positions.PositionId and E_Positions.actualEndDate is null and es.Code = 'A' )
) 
ELSE TotalSlots END AS TotalSlots, 
(select Count(1) from E_Positions  join employees on employees.EmployeeId = E_Positions.EmployeeId   
join ddlEmploymentStatuses es on es.EmploymentStatusId = employees.EmploymentStatusId   
 where E_Positions.PositionId = Positions.PositionId and E_Positions.actualEndDate is null and es.Code = 'A' ) as  SlotsFilled ,  
0 as SlotsLeft 
from positions ORDER BY Positions.Title  


SELECT PositionCode,PositionTitle,TotalSlots,SlotsFilled,
(TotalSlots-SlotsFilled)AS SlotsLeft
FROM @Report



END

