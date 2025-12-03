
go

if  exists (select 1 from sys.objects where object_id = object_id(N'dbo.spImportEmployeeManager') and type in (N'P', N'PC'))
drop procedure dbo.spImportEmployeeManager
go

if  exists (select 1 from sys.objects where object_id = object_id(N'dbo.spInsertManager') and type in (N'P', N'PC'))
drop procedure dbo.spInsertManager
go

if exists (select 1 from sys.types where name = N'DtoPerson' and schema_id = schema_id('dbo'))
drop type dbo.DtoPerson
go

create type dbo.DtoPerson as table(
[AspNetUserId] [nvarchar](256) null,
[PersonId] [int] null,
[LastName] [varchar](50) null,
[FirstName] [varchar](50) null,
[PasswordHash] [nvarchar](max) null,
[SecurityStamp] [nvarchar](max) null,
[EMail] [varchar](100) null
)
go

create procedure dbo.spImportEmployeeManager
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

	
declare 
		@Count int = 1,
		@RowCount int,
		@PersonId int,
		@LastName varchar(50),
		@FirstName varchar(50),
		@FileNumber varchar(50)='',
		@EmployeeRoleId nvarchar(256),
		@UserId nvarchar(256),
		@Email nvarchar(512),
		@AspNetUserId nvarchar(256),
		@AspNetUserUpdateId nvarchar(256) ,
		@ReportToPersonId int=0,
		@ManagerRoleId nvarchar(256),
		@DepartmentId int,
		@ManagerId int
	
	select 
		@EmployeeRoleId = Id
	from
		dbo.AspNetRoles
	where
		Name='ClientEmployees'
	
	select 
		@ManagerRoleId = Id
	from
		dbo.AspNetRoles
	where
		Name='ClientManagers'
	

	select
		 @RowCount = count(1)
	from   
		@PersonTable
	/*Looping for Employees*/
	while @Count <= @RowCount
	begin
		select
			@PersonId = PersonId,
			@LastName = ltrim(rtrim(LastName)),
			@FirstName = ltrim(rtrim(FirstName)),
			@AspNetUserId = AspNetUserId,
			@Email=EMail
		from
			@PersonTable 
		where
			RowNumber = @Count;
	
		if not exists(select 1 from dbo.AspNetUsers where UserName= @Email)
		begin
			
			insert into dbo.AspNetUsers
			(
				Id,
				EmployerId,
				LastPasswordChangeDate,
				Email,
				EmailConfirmed,
				PasswordHash,
				SecurityStamp,
				UserName,
				LastName,
				FirstName,
				PhoneNumberConfirmed,
				TwoFactorEnabled,
				LockoutEnabled,
				AccessFailedCount
			)
			select
				@AspNetUserId,
				3,
				GETDATE(),
				@Email,
				0,
				PasswordHash,
				SecurityStamp,
				@Email,
				@LastName,
				@FirstName,
				0,
				0,
				1,
				0
			from
				@PersonTable
			where
				RowNumber = @Count
			if not exists(select 1 from dbo.AspNetUserRoles where UserId=@AspNetUserId)
			begin
				insert into dbo.AspNetUserRoles
				(
					UserId,
					RoleId
				)
				select
					@AspNetUserId,
					@EmployeeRoleId
			end
			
		if exists(select 1 from dbo.UserNamesPersons where UserName=@Email)
		begin
			update 
				dbo.UserNamesPersons 
			set
				PersonId=@PersonId
			where
				UserName=@Email
		end
		else
		begin
			insert into dbo.UserNamesPersons 
			(
				UserName,
				PersonID,
				CreationDate
			)
			select
				@Email,
				@PersonId,
				GETDATE()
		end
		end
		else
		begin
			select
				@AspNetUserUpdateId = Id
			from 
				dbo.AspNetUsers 
			where 
				UserName= @Email
			update 
				dbo.AspNetUsers 
			set
				UserName=@Email,
				LastName = @LastName,
				FirstName = @FirstName
			where
				Id=@AspNetUserUpdateId
			if not exists(select 1 from AspNetUserRoles where UserId=@AspNetUserUpdateId)
			begin
				insert into dbo.AspNetUserRoles
					(
						UserId,
						RoleId
					)
					select
						@AspNetUserUpdateId,
						@EmployeeRoleId
			end
			else
			begin
				update
					dbo.AspNetUserRoles
				set
					RoleId=@EmployeeRoleId
				where
					UserId=@AspNetUserUpdateId
			end
			if exists(select 1 from dbo.UserNamesPersons where UserName=@Email)
			begin
				update 
					dbo.UserNamesPersons 
				set
					PersonId=@PersonId,
					ModifiedDate=GETDATE()
				where
					UserName=@Email
			end
		end
		set 
		  @Count = @Count + 1;
	end

select
		@Count=1,
		@EMail=null,
		@PersonId =null,
		@RowCount =0

	declare
			@TempManagerDepartment as table(RowNo int identity(1,1),ReportToPersonId int,PersonId int,DepartmentId int,Email varchar(100))
	declare @ManagerEmail varchar(100)
	insert into @TempManagerDepartment
	select 
		isnull(ee.ReportsToID,0),
		p.PersonId,
		ee.DepartmentId,p.Email
	from 
		dbo.E_Positions ee
		inner join Employees emp on ee.EmployeeId = emp.EmployeeId
		inner join @PersonTable p			
		on emp.PersonId=p.PersonId 
		
	select
		@RowCount = count(1)
	from
		@TempManagerDepartment 
	

	while @Count <= @RowCount
	begin
		select 
			@ReportToPersonId = isnull(ReportToPersonId,0),
			@PersonId = PersonId,
			@DepartmentId = DepartmentId,
			@Email = Email
		from 
			@TempManagerDepartment
		where
			RowNo=@Count;

		
		if(@ReportToPersonId>0)
		begin
			
			if exists(select 1 from dbo.AspNetUsers where UserName= @EMail)
			begin
				set @ManagerId = null
				if not exists(select 1 from dbo.Managers where PersonId=@ReportToPersonId)
				begin
					
					insert into dbo.Managers(PersonId)
					select @ReportToPersonId
					set @ManagerId = scope_identity()
				end
				else
				begin
					select @ManagerId = (select top 1 ManagerId from dbo.Managers where PersonId=@ReportToPersonId)
				end
				if(@ManagerId is not null)
				begin
				
					if not exists(select 1 from dbo.ManagerDepartments where ManagerId=@ManagerId and DepartmentId = @DepartmentId)
					begin
						insert into dbo.ManagerDepartments
						(
							ManagerId,
							DepartmentId
						)
						select 
							@ManagerId,@DepartmentId
					end
				end
			
			end
			select	
				@ManagerEmail = Email
			from
				dbo.Persons
			where
				PersonId = @ReportToPersonId
			set
				@UserId = null
			if exists(select 1 from dbo.AspNetUsers where UserName= @ManagerEmail)
			begin
				select 
					@UserId = Id 
				from
					dbo.AspNetUsers
				where 
					UserName = @ManagerEmail
				update
					dbo.AspNetUserRoles
				set
					RoleId = @ManagerRoleId
				where
					UserId = @UserId
					
			end
		end
		set 
		  @Count = @Count + 1;
	end
end
	go



