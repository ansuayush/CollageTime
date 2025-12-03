USE [DUBlankDB_ForNewImport318New]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetCurrentSupervisors]    Script Date: 4/27/2018 12:15:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetCurrentSupervisors]
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
		and Persons.PersonId = @ManagerPersonId
		order by ManagerPersonName
	END
END
GO

