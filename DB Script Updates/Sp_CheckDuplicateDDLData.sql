IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('CheckDuplicateDDLData'))
BEGIN
    DROP PROCEDURE [CheckDuplicateDDLData]
END
GO


CREATE proc [dbo].[CheckDuplicateDDLData]
@Code varchar (50),  
@Description varchar (50),
@TblName varchar(50)  
as  
  
DECLARE @execquery AS NVARCHAR(MAX)  
DECLARE @tablename AS NVARCHAR(128)  
declare  @rowcnt INT  
declare  @message AS NVARCHAR(128)  
set @execquery= N'SELECT * FROM ' + QUOTENAME(@TblName) + ' WHERE Code='''+ @Code +''' OR Description ='''+ @Description + ''' '  
EXEC Sp_executesql @execquery  
SELECT @rowcnt = @@ROWCOUNT  
IF @rowcnt > 0  
BEGIN  
  RaisError('Duplicate.', 16, -1)
return 
END 

GO


