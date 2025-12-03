

ALTER procedure [dbo].[spImportEmployeeManager]
@PersonInfo dbo.DtoPerson readonly
as
/*
Changes
	Date		Author		TicketNo	Details
	07-Mar-18	Chandu		DU-961		Changes made for Import Employee/Import Managers(performace issue from front end)
	17-Mar-18   Salman					Changes made for UserName login 
	27-Mar-18   Chandu					Changes made for Manger multiple departments insertion issue
*/  
begin

	IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('TempPerson_Table'))
	BEGIN
		drop table TempPerson_Table
	END


	IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('Temp_ManagerDepartment'))
	BEGIN
		drop table Temp_ManagerDepartment
	END

	declare @PersonTable as table
	(
		RowNumber int,
		AspNetUserId nvarchar(256),
		PersonId int,
		LastName varchar(50),
		FirstName varchar(50),
		PasswordHash nvarchar(max),
		SecurityStamp nvarchar(max),
		EMail varchar(100)
	)

	insert into @PersonTable 
	(
		RowNumber,
		AspNetUserId,
		PersonId,
		LastName,
		FirstName,
		PasswordHash,
		SecurityStamp,
		EMail
	)
	select 
		row_number() over(order by PersonId),
		AspNetUserId,
		PersonId,
		LastName,
		FirstName,
		PasswordHash,
		SecurityStamp,
		EMail
	from 
		@PersonInfo

select * into TempPerson_Table from @PersonTable

declare
	@TempManagerDepartment as table(RowNo int identity(1,1),ReportToPersonId int,PersonId int,DepartmentId int,Email varchar(100),CompanyCodeId int)
	declare @ManagerEmail varchar(100)
	insert into @TempManagerDepartment
	select 
		isnull(ee.ReportsToID,0),
		p.PersonId,
		ee.DepartmentId,p.Email,
		emp.CompanyCodeId
	from 
		dbo.E_Positions ee
		inner join Employees emp on ee.EmployeeId = emp.EmployeeId
		inner join @PersonTable p			
		on emp.PersonId=p.PersonId 

select * into Temp_ManagerDepartment from @TempManagerDepartment	

end