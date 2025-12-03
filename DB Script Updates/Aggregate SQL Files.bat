REM ========================================================================================================================
REM =	Created by  : Santhosh Kaparthi.
REM =	Date	    : 10/17/2017
REM =	Description : This batch file will aggregate all the update scripts in the right order
REM =		             Put that file in the same folder than the update scripts and run it.
REM = 		             It will create file DatabaseUpgradeScript.sql. Just Open it in SQL server and run it!
REM ========================================================================================================================
pushd \\pspl-hyd-srv2\g$\Projects\ExecViewPILTDGit\DB Script Updates
del DatabaseUpgradeScript.7z 
del DatabaseUpgradeScript.sql

COPY /b table_*.sql + go.sql + type_*.sql + go.sql + tr_*.sql + go.sql + func_*.sql + go.sql + view_*.sql + go.sql  + sp_*.sql + go.sql DatabaseUpgradeScript.sql

7za.exe a -t7z DatabaseUpgradeScript.7z DatabaseUpgradeScript.sql

for %%G in (DatabaseUpgradeScript.sql) do sqlcmd -U sa -P sa!2015 -S epmonline.us\SQL2014 /d ExecViewHrkClientTemplate  -i"%%G"


