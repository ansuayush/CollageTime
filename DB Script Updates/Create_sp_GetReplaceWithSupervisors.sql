
CREATE PROCEDURE [dbo].[sp_GetReplaceWithSupervisors]

AS    
BEGIN

	Select 
		Persons.PersonId as ManagerPersonId
		,Persons.Firstname + ' '+ Persons.Lastname as ManagerPersonName
	from Managers
	inner join Persons on Managers.PersonId = Persons.PersonId
	order by ManagerPersonName
	
END