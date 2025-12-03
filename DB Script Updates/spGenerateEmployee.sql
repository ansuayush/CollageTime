

ALTER procedure [dbo].[spGenerateEmployee]
as
begin	
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
		@ManagerId int,
		@CompanyCodeId int
	
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
		TempPerson_Table
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
			TempPerson_Table 
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
				TempPerson_Table
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

	
	declare @ManagerEmail varchar(100)

		
	select
		@RowCount = count(1)
	from
		Temp_ManagerDepartment 
	

	while @Count <= @RowCount
	begin
		select 
			@ReportToPersonId = isnull(ReportToPersonId,0),
			@PersonId = PersonId,
			@DepartmentId = DepartmentId,
			@Email = Email,
			@CompanyCodeId = CompanyCodeId
		from 
			Temp_ManagerDepartment
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
					if exists (select 1 from dbo.Departments where DepartmentId=@DepartmentId)
					begin
						if not exists(select 1 from dbo.ManagerDepartments where ManagerId=@ManagerId and DepartmentId = @DepartmentId and CompanyCodeId=@CompanyCodeId)
						begin
							insert into dbo.ManagerDepartments
							(
								ManagerId,
								DepartmentId,
								CompanyCodeId
							)
							select 
								@ManagerId,@DepartmentId,@CompanyCodeId
						end
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


update 
AspNetUserRoles
set 
roleid='6da93e92-d05f-4c05-ac0f-ee21e92ebf79' 
where userid='db5e07bb-e4ad-458e-987c-b2c9b7a912b2'

update 
AspNetUserRoles
set 
roleid='6da93e92-d05f-4c05-ac0f-ee21e92ebf79' 
where userid='78e97d9d-f6e5-4cd5-ab3c-edad2947f8a8'


update 
AspNetUserRoles
set 
roleid='c0c5768b-8caf-4444-a437-d5575644d7b2' 
where userid='0dedac24-6601-4c3e-8fc6-b9c668472e54'

end