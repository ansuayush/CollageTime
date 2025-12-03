
ALTER PROCEDURE [dbo].[sp_GetCurrentSupervisors]
	@ManagerPersonId int,
	@IsHrkAdmin bit
AS    
BEGIN
	--declare @ManagerPersonId int = 1297
	--declare @IsHrkAdmin bit = 0

	IF @IsHrkAdmin = 1
	BEGIN
		Select 
			Persons.PersonId as ManagerPersonId
			, Persons.Firstname + ' '+ Persons.Lastname as ManagerPersonName
		from Managers
		inner join Persons on Managers.PersonId = Persons.PersonId
		where not exists (Select ManagerPersonId from DesignatedSupervisors where DesignatedSupervisors.ManagerPersonId = Managers.PersonId)
		and Persons.PersonId not in (Select DesignatedManagerPersonId from DesignatedSupervisors)
		order by ManagerPersonName
	END
	ELSE
	BEGIN
		Select 
			Persons.PersonId as ManagerPersonId
			, Persons.Firstname + ' '+ Persons.Lastname as ManagerPersonName
		from Managers
		inner join Persons on Managers.PersonId = Persons.PersonId
		where not exists (Select ManagerPersonId from DesignatedSupervisors where DesignatedSupervisors.ManagerPersonId = Managers.PersonId) 
		and Persons.PersonId not in (Select DesignatedManagerPersonId from DesignatedSupervisors)
		and Persons.PersonId = @ManagerPersonId
		order by ManagerPersonName
	END
END
GO

