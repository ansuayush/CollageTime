select * from Contracts


EXEC sp_rename 'Contracts.Semister', 'SemisterId';  

alter table Contracts
alter column SemisterId int
 
drop table DDlsemisters

CREATE TABLE [dbo].[ddlSemisters](
	[SemisterID] int IDENTITY(1,1) primary key NOT NULL,
	[code] [char](10) NULL,
	[description] [varchar](50) NULL,
	[Active] [bit] NULL,
 )



