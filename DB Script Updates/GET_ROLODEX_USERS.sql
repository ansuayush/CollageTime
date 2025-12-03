
CREATE PROC [dbo].[GET_ROLODEX_USERS]               
@type varchar(10) = 'EMPLOYEES',                
@search varchar(20) = ''                
AS                
BEGIN                
IF @type = 'PERSONS'                
  BEGIN                
    SELECT DISTINCT A.PersonId,Firstname,LastName,(Firstname +' '+LastName) PersonName, 
	EmployeeId=Employees.EmployeeId,           
    FileNumber = Employees.FileNumber,      
    CompanyCodeId = C.CompanyCodeId,      
    CompanyCode = C.CompanyCodeDescription,    
	BusinessLevelNbr =  EPositions.BusinessLevelNbr,
	A.eMail
    FROM Persons A                
    LEFT OUTER JOIN (
		SELECT ROW_NUMBER() OVER (PARTITION BY PersonId ORDER BY TerminationDate ASC) AS Rank, EmployeeId,FileNumber,PersonId,CompanyCode,CompanyCodeId FROM EMPLOYEES
	) AS Employees ON A.PersonId=Employees.PersonId AND Employees.Rank = 1       
	LEFT OUTER JOIN (
		SELECT 
		ROW_NUMBER() OVER (PARTITION BY EmployeeId ORDER BY E_POSITIONS.EnteredDate DESC) AS Rank , EmployeeId,E_POSITIONS.E_PositionId, BusinessLevelNbr FROM E_POSITIONS
		INNER JOIN Positions ON Positions.Positionid = E_POSITIONS.Positionid
	) AS EPositions ON Employees.EmployeeId=EPositions.EmployeeId AND EPositions.Rank = 1 
	       
    LEFT OUTER JOIN CompanyCodes C ON C.CompanyCodeId = Employees.CompanyCodeId            
    WHERE (@search = '' OR (LOWER((Firstname +' '+LastName)) LIKE '%'+@search+'%' OR FileNumber LIKE '%'+@search+'%' OR eMail LIKE '%'+@search+'%'))                
    ORDER BY Firstname               
  END                
IF @type = 'EMPLOYEES'                
  BEGIN                
    SELECT DISTINCT A.PersonId,Firstname,LastName,(Firstname +' '+LastName) PersonName,  
	EmployeeId=Employees.EmployeeId,          
    FileNumber = Employees.FileNumber,      
    CompanyCodeId = C.CompanyCodeId,      
    CompanyCode = C.CompanyCodeDescription,    
	BusinessLevelNbr =  EPositions.BusinessLevelNbr,
	A.eMail     
    FROM Persons A                  
    INNER JOIN (
		SELECT ROW_NUMBER() OVER (PARTITION BY PersonId ORDER BY TerminationDate ASC) AS Rank, EmployeeId,FileNumber,PersonId,CompanyCode,CompanyCodeId FROM EMPLOYEES
	) AS Employees ON A.PersonId=Employees.PersonId AND Employees.Rank = 1       
	LEFT OUTER JOIN (
		SELECT 
		ROW_NUMBER() OVER (PARTITION BY EmployeeId ORDER BY E_POSITIONS.EnteredDate DESC) AS Rank , EmployeeId,E_POSITIONS.E_PositionId, BusinessLevelNbr FROM E_POSITIONS
		INNER JOIN Positions ON Positions.Positionid = E_POSITIONS.Positionid
	) AS EPositions ON Employees.EmployeeId=EPositions.EmployeeId AND EPositions.Rank = 1
	          
    LEFT OUTER JOIN CompanyCodes C ON C.CompanyCodeId = Employees.CompanyCodeId       
    WHERE (@search = '' OR (LOWER((Firstname +' '+LastName)) LIKE '%'+@search+'%' OR FileNumber LIKE '%'+@search+'%' OR eMail LIKE '%'+@search+'%'))                
    ORDER BY Firstname              
  END                
IF @type = 'USERS'                
  BEGIN                
   SELECT DISTINCT A.PersonId,A.Firstname,A.LastName,(Firstname +' '+LastName) PersonName,A.SSN,A.Email,ClientDBUserName = U.UserName,          
   FileNumber = Employees.FileNumber,      
   EmployeeId=Employees.EmployeeId,
   CompanyCodeId = C.CompanyCodeId,      
   CompanyCode = C.CompanyCodeDescription,    
   BusinessLevelNbr =  EPositions.BusinessLevelNbr,
   A.eMail    
   FROM Persons A                
   LEFT OUTER JOIN UserNamesPersons U ON U.PersonID = A.PersonId          
   INNER JOIN (
		SELECT ROW_NUMBER() OVER (PARTITION BY PersonId ORDER BY TerminationDate ASC) AS Rank, EmployeeId,FileNumber,PersonId,CompanyCode,CompanyCodeId FROM EMPLOYEES
	) AS Employees ON A.PersonId=Employees.PersonId AND Employees.Rank = 1       
	LEFT OUTER JOIN (
		SELECT 
		ROW_NUMBER() OVER (PARTITION BY EmployeeId ORDER BY E_POSITIONS.EnteredDate DESC) AS Rank , EmployeeId,E_POSITIONS.E_PositionId, BusinessLevelNbr FROM E_POSITIONS
		INNER JOIN Positions ON Positions.Positionid = E_POSITIONS.Positionid
	) AS EPositions ON Employees.EmployeeId=EPositions.EmployeeId AND EPositions.Rank = 1
	         
   LEFT OUTER JOIN CompanyCodes C ON C.CompanyCodeId = Employees.CompanyCodeId       
   WHERE (@search = '' OR A.PersonId = @search)               
   ORDER BY Firstname              
  END          
END     