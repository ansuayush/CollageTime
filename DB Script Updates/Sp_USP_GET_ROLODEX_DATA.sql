IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('USP_GET_ROLODEX_DATA'))
BEGIN
    DROP PROCEDURE USP_GET_ROLODEX_DATA
END
GO

CREATE PROC USP_GET_ROLODEX_DATA 
@PersonId INT
AS 
BEGIN
SELECT TOP 1
Persons.PersonId
,Positions.PositionDescription
,Persons.Lastname
,Persons.Firstname
,Employees.CompanyCodeId as CompanyCode
,Employees.HireDate
,Persons.eMail
,Employees.FileNumber
,PersonPhoneNumbers.PhoneNumber
from Persons 
LEFT OUTER JOIN Employees ON Persons.PersonId = Employees.PersonId
LEFT OUTER JOIN [E_Positions] ON Employees.EmployeeId = [E_Positions].EmployeeId
LEFT OUTER JOIN Positions ON [E_Positions].PositionId = Positions.PositionId
LEFT OUTER JOIN 
(SELECT TOP 1 PersonPhoneNumberId,PersonId,PhoneTypeId,PhoneNumber FROM PersonPhoneNumbers WHERE PersonId = @PersonId ORDER BY PersonPhoneNumberId DESC) PersonPhoneNumbers 
ON Persons.PersonId = PersonPhoneNumbers.PersonId
WHERE Persons.PersonId =  @PersonId
ORDER BY Employees.EmployeeId DESC

END


GO


