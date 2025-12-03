create table Treatylimithistories
(
         Id int identity(1,1) primary key,
          EmployeeId int,
          CompanyCodeId int, 
          TreatyLimit decimal(18,2), 
          UsedAmount decimal(18,2),
          RemainingAmount decimal(18,2),
          CreatedBy varchar(50),
          CreatedDate DateTime,
		  EmployeeName varchar(50),
		  CompanyCodeDescription varchar(50)
    )