using System;
using System.Data;

using System.Data.SqlClient;

namespace ImportCityOfAlPaso
{

    /// <summary>
    /// Summary description for EmployeeActuals
    /// </summary>
    public class EmployeeActuals
    {
        private SqlConnection m_Connection;

        public EmployeeActuals()
        {
            string strConn = System.Configuration.ConfigurationManager.AppSettings["dbExecViewConnectString"];
            m_Connection = new System.Data.SqlClient.SqlConnection(strConn);
        }

        public EmployeeActuals(SqlConnection connection)
        {
            m_Connection = connection;
        }

       
        public int Insert(int nEmployeeID, int nPositionID, DateTime datePayPeriod, Decimal dActualPay)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO EmployeeActuals " +
                "(EmployeeID, PositionID, PayPeriodDate, ActualPay) " +
                "VALUES(@EmployeeID, @PositionID, @PayPeriodDate, @ActualPay)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PayPeriodDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);
            
            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PayPeriodDate"].Value = datePayPeriod;
            cmdInsert.Parameters["@ActualPay"].Value = dActualPay;

            //cmdInsert.Parameters["@UserID"].SourceVersion = DataRowVersion.Original;

            try
            {
                m_Connection.Open();
                cmdInsert.ExecuteNonQuery();
                nRtn = (int)cmdInsert.Parameters["@ID"].Value;
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }

        public int InsertStJoseph(int nEmployeeID, int nPositionID, DateTime datePayPeriod,  DateTime payDate, Decimal dActualPay
            , string strWorkedInDepartmentCode, string strCompanyCode, string strFileNumber)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO EmployeeActuals " +
                "(EmployeeID, PositionID, PayPeriodEndDate, PayDate, ActualPay, WorkedInDepartmentCode, CompanyCode, FileNumber) " +
                "VALUES(@EmployeeID, @PositionID, @PayPeriodEndDate, @PayDate, @ActualPay, @WorkedInDepartmentCode, @CompanyCode, @FileNumber)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PayPeriodEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@PayDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@WorkedInDepartmentCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@CompanyCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@FileNumber", SqlDbType.VarChar, 50);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PayPeriodEndDate"].Value = datePayPeriod;
            cmdInsert.Parameters["@PayDate"].Value = payDate;
            cmdInsert.Parameters["@ActualPay"].Value = dActualPay;
            cmdInsert.Parameters["@WorkedInDepartmentCode"].Value = strWorkedInDepartmentCode;
            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;

            //cmdInsert.Parameters["@UserID"].SourceVersion = DataRowVersion.Original;

            try
            {
                m_Connection.Open();
                cmdInsert.ExecuteNonQuery();
                nRtn = (int)cmdInsert.Parameters["@ID"].Value;
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }

        public int InsertPinehurst(int nEmployeeID, int nPositionID, DateTime datePayPeriod, DateTime payDate, Decimal dActualPay
           , string strCompanyCode, string strFileNumber)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO EmployeeActuals " +
                "(EmployeeID, PositionID, PayPeriodDate, PayCheckEndDate, ActualPay, CompanyCode, FileNumber) " +
                "VALUES(@EmployeeID, @PositionID, @PayPeriodDate, @PayCheckEndDate, @ActualPay, @CompanyCode, @FileNumber)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PayPeriodDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@PayCheckEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);
           // cmdInsert.Parameters.Add("@WorkedInDepartmentCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@CompanyCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@FileNumber", SqlDbType.VarChar, 50);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PayPeriodDate"].Value = datePayPeriod;
            cmdInsert.Parameters["@PayCheckEndDate"].Value = payDate;
            cmdInsert.Parameters["@ActualPay"].Value = dActualPay;
           // cmdInsert.Parameters["@WorkedInDepartmentCode"].Value = strWorkedInDepartmentCode;
            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;

            //cmdInsert.Parameters["@UserID"].SourceVersion = DataRowVersion.Original;

            try
            {
                m_Connection.Open();
                cmdInsert.ExecuteNonQuery();
                nRtn = (int)cmdInsert.Parameters["@ID"].Value;
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }

        public int UpdateEmployeeActualsGrossPayPinehurst(int nEmployeeID, int nPositionID, DateTime datePayPeriod, DateTime payDate, Decimal dActualPay
           , string strCompanyCode, string strFileNumber)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE EmployeeActuals SET ActualPay=@ActualPay WHERE FileNumber=@FileNumber " +
                                     " AND EmployeeID=@EmployeeID AND PositionID=@PositionID AND CompanyCode=@CompanyCode " +
                                     " AND PayPeriodDate=@PayPeriodDate";

            cmdUpdate.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@PayPeriodDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@CompanyCode", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@FileNumber", SqlDbType.VarChar, 50);
                        
            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@PayPeriodDate"].Value = datePayPeriod;
            cmdUpdate.Parameters["@ActualPay"].Value = dActualPay;
            cmdUpdate.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNumber;
            
            try
            {
                m_Connection.Open();
                cmdUpdate.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }

        public int InsertWea(int nEmployeeID, int nPositionID, DateTime datePayPeriod, Decimal dActualPay
           , string strCompanyCode, string strFileNumber, DateTime? payDate, Decimal? dRegularHours, Decimal? dOtHours, string strBusinessUnit
            , string strJobCode, string strJobTitle, Decimal? dFte, string strHomeDepartment)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = " " +
                "INSERT INTO EmployeeActuals " +
                "   ([EmployeeID] " +
                "   ,[PositionID] " +
                "   ,[PayPeriodDate] " +
                "   ,[ActualPay] " +
                "   ,[CompanyCode] " +
                "   ,[FileNumber] " +
                "   ,[PayDate] " +
                "   ,[RegularHours] " +
                "   ,[OvertimeHours] " +
                "   ,[BusinessUnit] " +
                "   ,[JobCode] " +
                "   ,[JobTitle] " +
                "   ,[Fte] " +
                "   ,[HomeDepartment] )" +
                "VALUES " +
                "   (@EmployeeID " +
                "   ,@PositionID " +
                "   ,@PayPeriodDate " +
                "   ,@ActualPay " +
                "   ,@CompanyCode " +
                "   ,@FileNumber " +
                "   ,@PayDate " +
                "   ,@RegularHours " +
                "   ,@OvertimeHours " +
                "   ,@BusinessUnit " +
                "   ,@JobCode " +
                "   ,@JobTitle " +
                "   ,@Fte " +
                "   ,@HomeDepartment )" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PayPeriodDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);

            cmdInsert.Parameters.Add("@CompanyCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@FileNumber", SqlDbType.VarChar, 50);


            cmdInsert.Parameters.Add("@PayDate", SqlDbType.Date);
            cmdInsert.Parameters.Add("@RegularHours", SqlDbType.Decimal,9);
            cmdInsert.Parameters.Add("@OvertimeHours", SqlDbType.Decimal,9);
            cmdInsert.Parameters.Add("@BusinessUnit", SqlDbType.VarChar, 100);
            cmdInsert.Parameters.Add("@JobCode", SqlDbType.VarChar, 100);
            cmdInsert.Parameters.Add("@JobTitle", SqlDbType.VarChar, 100); 
            cmdInsert.Parameters.Add("@Fte", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@HomeDepartment", SqlDbType.VarChar, 100);


            cmdInsert.Parameters.Add("@ID", SqlDbType.Int, 4);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PayPeriodDate"].Value = datePayPeriod;
            cmdInsert.Parameters["@ActualPay"].Value = dActualPay;

            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;

            if (payDate == null)
                cmdInsert.Parameters["@PayDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@PayDate"].Value = payDate;

            if (dRegularHours == null )
                cmdInsert.Parameters["@RegularHours"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@RegularHours"].Value = dRegularHours;

            if (dOtHours == null)
                cmdInsert.Parameters["@OvertimeHours"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@OvertimeHours"].Value = dOtHours;


            cmdInsert.Parameters["@BusinessUnit"].Value = strBusinessUnit;
            cmdInsert.Parameters["@JobCode"].Value = strJobCode;
            cmdInsert.Parameters["@JobTitle"].Value = strJobTitle;
           
            if (dFte == null)
                cmdInsert.Parameters["@Fte"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@Fte"].Value = dFte;


            cmdInsert.Parameters["@HomeDepartment"].Value = strHomeDepartment;
            

            //cmdInsert.Parameters["@UserID"].SourceVersion = DataRowVersion.Original;

            try
            {
                m_Connection.Open();
                cmdInsert.ExecuteNonQuery();
                nRtn = (int)cmdInsert.Parameters["@ID"].Value;
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }

        public int InsertSpectracare(int nEmployeeID, int nPositionID, DateTime datePayPeriod, Decimal dActualPay
            ,string strCompanyCode, string strFileNumber, string strPayrollWeekNumber, string strHomeDepartment, string strDepartmentWorked, Decimal dRegularHours
            ,Decimal dOvertimeHours, Decimal dCompHours, Decimal dWacJanitorHours, Decimal dUnspecHours
            ,Decimal dHolidayHours, Decimal dPersonalHours, Decimal dBirthdayHours
            ,Decimal dSickHours, Decimal dVacationHours, Decimal dRegularEarnings, Decimal dOvertimeEarnings
            ,Decimal dOtherStaffPay, Decimal dWacEarnings, Decimal dEmployerSuiTax
            ,Decimal dFICA, Decimal dEmployerRetirementContribution, Decimal dEmployerHealthInsuranceContribution, Decimal dEmployerDentalVisionContribution
            ,Decimal dEmployerDentalVisionOffset, Decimal dEmployerLifeInsuranceContribution, Decimal dEmployerDisibilityInsuranceContribution
            ,Decimal dWorkmenCompTotal)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = " "+
                "INSERT INTO EmployeeActuals "+
                "   ([EmployeeID] "+
                "   ,[PositionID] "+
                "   ,[PayPeriodDate] "+
                "   ,[ActualPay] "+
                "   ,[CompanyCode] "+
                "   ,[FileNumber] "+
                "   ,[PayrollWeekNumber] "+
                "   ,[HomeDepartment] "+
                "   ,[DepartmentWorked] "+
                "   ,[RegularHours] "+
                "   ,[OvertimeHours] "+
                "   ,[CompHours] "+
                "   ,[WacJanitorHours] "+
                "   ,[UnspecHours] "+
                "   ,[HolidayHours] "+
                "   ,[PersonalHours] "+
                "   ,[BirthdayHours] "+
                "   ,[SickHours] "+
                "   ,[VacationHours] "+
                "   ,[RegularEarnings] "+
                "   ,[OvertimeEarnings] "+
                "   ,[OtherStaffPay] "+
                "   ,[WacEarnings] "+
                "   ,[EmployerSuiTax] "+
                "   ,[FICA] "+
                "   ,[EmployerRetirementContribution] "+
                "   ,[EmployerHealthInsuranceContribution] "+
                "   ,[EmployerDentalVisionContribution] "+
                "   ,[EmployerDentalVisionOffset] "+
                "   ,[EmployerLifeInsuranceContribution] "+
                "   ,[EmployerDisibilityInsuranceContribution] "+
                "   ,[WorkmenCompTotal] )"+
                "VALUES "+
                "   (@EmployeeID "+
                "   ,@PositionID "+
                "   ,@PayPeriodDate "+
                "   ,@ActualPay "+
                "   ,@CompanyCode "+
                "   ,@FileNumber "+
                "   ,@PayrollWeekNumber "+
                "   ,@HomeDepartment "+
                "   ,@DepartmentWorked "+
                "   ,@RegularHours "+
                "   ,@OvertimeHours "+
                "   ,@CompHours "+
                "   ,@WacJanitorHours "+
                "   ,@UnspecHours "+
                "   ,@HolidayHours "+
                "   ,@PersonalHours  "+
                "   ,@BirthdayHours  "+
                "   ,@SickHours  "+
                "   ,@VacationHours "+
                "   ,@RegularEarnings "+
                "   ,@OvertimeEarnings "+
                "   ,@OtherStaffPay "+
                "   ,@WacEarnings "+
                "   ,@EmployerSuiTax "+
                "   ,@FICA "+
                "   ,@EmployerRetirementContribution "+
                "   ,@EmployerHealthInsuranceContribution "+
                "   ,@EmployerDentalVisionContribution "+
                "   ,@EmployerDentalVisionOffset "+
                "   ,@EmployerLifeInsuranceContribution "+
                "   ,@EmployerDisibilityInsuranceContribution "+
                "   ,@WorkmenCompTotal )" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PayPeriodDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);

            cmdInsert.Parameters.Add("@CompanyCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@FileNumber", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@PayrollWeekNumber", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@HomeDepartment", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@DepartmentWorked", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@RegularHours", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@OvertimeHours", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@CompHours", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@WacJanitorHours", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@UnspecHours", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@HolidayHours", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@PersonalHours ", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@BirthdayHours ", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@SickHours ", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@VacationHours", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@RegularEarnings", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@OvertimeEarnings", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@OtherStaffPay", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@WacEarnings", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@EmployerSuiTax", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@FICA", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@EmployerRetirementContribution", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@EmployerHealthInsuranceContribution", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@EmployerDentalVisionContribution", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@EmployerDentalVisionOffset", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@EmployerLifeInsuranceContribution", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@EmployerDisibilityInsuranceContribution", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@WorkmenCompTotal", SqlDbType.Decimal, 9);
            

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int, 4);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PayPeriodDate"].Value = datePayPeriod;
            cmdInsert.Parameters["@ActualPay"].Value = dActualPay;

            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;
            cmdInsert.Parameters["@PayrollWeekNumber"].Value =strPayrollWeekNumber;
            cmdInsert.Parameters["@HomeDepartment"].Value = strHomeDepartment;
            cmdInsert.Parameters["@DepartmentWorked"].Value = strDepartmentWorked;
            cmdInsert.Parameters["@RegularHours"].Value = dRegularHours;
            cmdInsert.Parameters["@OvertimeHours"].Value = dOvertimeHours;
            cmdInsert.Parameters["@CompHours"].Value = dCompHours;
            cmdInsert.Parameters["@WacJanitorHours"].Value = dWacJanitorHours;
            cmdInsert.Parameters["@UnspecHours"].Value = dUnspecHours;
            cmdInsert.Parameters["@HolidayHours"].Value = dHolidayHours;
            cmdInsert.Parameters["@PersonalHours "].Value = dPersonalHours;
            cmdInsert.Parameters["@BirthdayHours "].Value = dBirthdayHours;
            cmdInsert.Parameters["@SickHours "].Value = dSickHours;
            cmdInsert.Parameters["@VacationHours"].Value = dVacationHours;
            cmdInsert.Parameters["@RegularEarnings"].Value = dRegularEarnings;
            cmdInsert.Parameters["@OvertimeEarnings"].Value = dOvertimeEarnings;
            cmdInsert.Parameters["@OtherStaffPay"].Value = dOtherStaffPay;
            cmdInsert.Parameters["@WacEarnings"].Value = dWacEarnings;
            cmdInsert.Parameters["@EmployerSuiTax"].Value = dEmployerSuiTax;
            cmdInsert.Parameters["@FICA"].Value = dFICA;
            cmdInsert.Parameters["@EmployerRetirementContribution"].Value = dEmployerRetirementContribution;
            cmdInsert.Parameters["@EmployerHealthInsuranceContribution"].Value =dEmployerHealthInsuranceContribution;
            cmdInsert.Parameters["@EmployerDentalVisionContribution"].Value = dEmployerDentalVisionContribution;
            cmdInsert.Parameters["@EmployerDentalVisionOffset"].Value = dEmployerDentalVisionOffset;
            cmdInsert.Parameters["@EmployerLifeInsuranceContribution"].Value = dEmployerLifeInsuranceContribution;
            cmdInsert.Parameters["@EmployerDisibilityInsuranceContribution"].Value = dEmployerDisibilityInsuranceContribution;
            cmdInsert.Parameters["@WorkmenCompTotal"].Value = dWorkmenCompTotal;
            
            //cmdInsert.Parameters["@UserID"].SourceVersion = DataRowVersion.Original;

            try
            {
                m_Connection.Open();
                cmdInsert.ExecuteNonQuery();
                nRtn = (int)cmdInsert.Parameters["@ID"].Value;
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }


        public int InsertMerrimack(int nEmployeeID, int nPositionID, DateTime datePayPeriod, Decimal dActualPay,
            string strCompanyCode, string strFileNumber, DateTime paycheckEndDate, Decimal overtime, 
            string jobCode, string departmentCode,
            string strLocationCode, Decimal dBonus, Decimal dDetailPay, Decimal dAdditionalDuties)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO EmployeeActuals " +
                "(EmployeeID, PositionID, PayPeriodDate, ActualPay, CompanyCode, FileNumber, paycheckEndDate, overtime, jobCode, departmentCode, "+
                "    locationCode, Bonus, DetailPay, AdditionalDuties) " +
                "VALUES(@EmployeeID, @PositionID, @PayPeriodDate, @ActualPay, @CompanyCode, @FileNumber, @paycheckEndDate, @overtime, @jobCode, @departmentCode, "+
                "   @LocationCode, @Bonus, @DetailPay, @AdditionalDuties)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PayPeriodDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@CompanyCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@FileNumber", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@PayCheckEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@Overtime", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@JobCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@DepartmentCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@LocationCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@Bonus", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@DetailPay", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@AdditionalDuties", SqlDbType.Decimal, 9);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PayPeriodDate"].Value = datePayPeriod;
            cmdInsert.Parameters["@ActualPay"].Value = dActualPay;
            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;
            cmdInsert.Parameters["@PayCheckEndDate"].Value = paycheckEndDate;
            cmdInsert.Parameters["@Overtime"].Value = overtime;
            cmdInsert.Parameters["@JobCode"].Value = jobCode;
            cmdInsert.Parameters["@DepartmentCode"].Value = departmentCode;
            cmdInsert.Parameters["@LocationCode"].Value = strLocationCode;
            cmdInsert.Parameters["@Bonus"].Value = dBonus;
            cmdInsert.Parameters["@DetailPay"].Value = dDetailPay;
            cmdInsert.Parameters["@AdditionalDuties"].Value = dAdditionalDuties;

            //cmdInsert.Parameters["@UserID"].SourceVersion = DataRowVersion.Original;

            try
            {
                m_Connection.Open();
                cmdInsert.ExecuteNonQuery();
                nRtn = (int)cmdInsert.Parameters["@ID"].Value;
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }

          /*
            PayDate	datetime	Checked
            HomeDepartment	varchar(50)	Checked
            PeriodEndDate	datetime	Checked
            RateAmount	decimal(18, 4)	Checked
            GrossPay	decimal(18, 4)	Checked
            OvertimeEarnings	decimal(18, 4)	Checked
            Bonus	decimal(18, 4)	Checked
            ShiftPay	decimal(18, 4)	Checked
            CellPhone	decimal(18, 4)	Checked
            OnCall	decimal(18, 4)	Checked
            PTOHours	decimal(18, 4)	Checked
            PTODollars	decimal(18, 4)	Checked
            TotalHoursPaid	decimal(18, 4)	Checked
            OvertimeHours	decimal(18, 4)	Checked
         */
        public int InsertCardinalHill(int nEmployeeID, int nPositionID, DateTime datePayPeriod, Decimal dActualPay,
            DateTime datePay, string strHomeDepartment, DateTime datePeriodEnd, Decimal dRateAmount, Decimal dGrossPay,
            Decimal dOvertimeEarnings, Decimal dBonus, Decimal dShiftPay, Decimal dCellPhone, Decimal dOnCall, Decimal dPTOHours,
            Decimal dPTODollars, Decimal dTotalHoursPaid, Decimal dOvertimeHours, string strDepartmentWorked)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO EmployeeActuals " +
                "(  EmployeeID, PositionID, PayPeriodDate, ActualPay, "+
                "   PayDate, HomeDepartment, PeriodEndDate, RateAmount, GrossPay, "+
                "   OvertimeEarnings, Bonus, ShiftPay, CellPhone, OnCall, PTOHours, "+
                "   PTODollars, TotalHoursPaid, OvertimeHours, DepartmentWorked) " +
                "VALUES(@EmployeeID, @PositionID, @PayPeriodDate, @ActualPay, " +
                "       @PayDate, @HomeDepartment, @PeriodEndDate, @RateAmount, @GrossPay, " +
                "       @OvertimeEarnings, @Bonus, @ShiftPay, @CellPhone, @OnCall, @PTOHours, " +
                "       @PTODollars, @TotalHoursPaid, @OvertimeHours, @DepartmentWorked) " +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PayPeriodDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@PayDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@HomeDepartment", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@PeriodEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@RateAmount", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@GrossPay", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@OvertimeEarnings", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@Bonus", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@ShiftPay", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@CellPhone", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@OnCall", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@PTOHours", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@PTODollars", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@TotalHoursPaid", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@OvertimeHours", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@DepartmentWorked", SqlDbType.VarChar, 50);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PayPeriodDate"].Value = datePayPeriod;
            cmdInsert.Parameters["@ActualPay"].Value = dActualPay;
            cmdInsert.Parameters["@PayDate"].Value = datePay;
            cmdInsert.Parameters["@HomeDepartment"].Value = strHomeDepartment;
            cmdInsert.Parameters["@PeriodEndDate"].Value = datePeriodEnd;
            cmdInsert.Parameters["@RateAmount"].Value = dRateAmount;
            cmdInsert.Parameters["@GrossPay"].Value = dGrossPay;
            cmdInsert.Parameters["@OvertimeEarnings"].Value = dOvertimeEarnings;
            cmdInsert.Parameters["@Bonus"].Value = dBonus;
            cmdInsert.Parameters["@ShiftPay"].Value = dShiftPay;
            cmdInsert.Parameters["@CellPhone"].Value = dCellPhone;
            cmdInsert.Parameters["@OnCall"].Value = dOnCall;
            cmdInsert.Parameters["@PTOHours"].Value = dPTOHours;
            cmdInsert.Parameters["@PTODollars"].Value = dPTODollars;
            cmdInsert.Parameters["@TotalHoursPaid"].Value = dTotalHoursPaid;
            cmdInsert.Parameters["@OvertimeHours"].Value = dOvertimeHours;
            cmdInsert.Parameters["@DepartmentWorked"].Value = strDepartmentWorked;

            //cmdInsert.Parameters["@UserID"].SourceVersion = DataRowVersion.Original;

            try
            {
                m_Connection.Open();
                cmdInsert.ExecuteNonQuery();
                nRtn = (int)cmdInsert.Parameters["@ID"].Value;
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }

        public int UpdateSpectracare(int nId, int nEmployeeID, int nPositionID, DateTime datePayPeriod, Decimal dActualPay
            ,string strCompanyCode, string strFileNumber, string strPayrollWeekNumber, string strHomeDepartment, string strDepartmentWorked, Decimal dRegularHours
            ,Decimal dOvertimeHours, Decimal dCompHours, Decimal dWacJanitorHours, Decimal dUnspecHours
            ,Decimal dHolidayHours, Decimal dPersonalHours, Decimal dBirthdayHours
            ,Decimal dSickHours, Decimal dVacationHours, Decimal dRegularEarnings, Decimal dOvertimeEarnings
            ,Decimal dOtherStaffPay, Decimal dWacEarnings, Decimal dEmployerSuiTax
            ,Decimal dFICA, Decimal dEmployerRetirementContribution, Decimal dEmployerHealthInsuranceContribution, Decimal dEmployerDentalVisionContribution
            ,Decimal dEmployerDentalVisionOffset, Decimal dEmployerLifeInsuranceContribution, Decimal dEmployerDisibilityInsuranceContribution
            ,Decimal dWorkmenCompTotal)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = " "+
                "Update EmployeeActuals "+
                "SET "+
                "   [EmployeeID] = @EmployeeID "+
                "   ,[PositionID] = @PositionID  "+
                "   ,[PayPeriodDate] = @PayPeriodDate  "+
                "   ,[ActualPay] = @ActualPay  "+
                "   ,[CompanyCode] = @CompanyCode  "+
                "   ,[FileNumber] = @FileNumber  "+
                "   ,[PayrollWeekNumber] = @PayrollWeekNumber  "+
                "   ,[HomeDepartment] = @HomeDepartment  "+
                "   ,[DepartmentWorked] = @DepartmentWorked  "+
                "   ,[RegularHours] = @RegularHours  "+
                "   ,[OvertimeHours] = @OvertimeHours  "+
                "   ,[CompHours] = @CompHours  "+
                "   ,[WacJanitorHours] = @WacJanitorHours  "+
                "   ,[UnspecHours] = @UnspecHours  "+
                "   ,[HolidayHours] = @HolidayHours  "+
                "   ,[PersonalHours] = @PersonalHours  "+
                "   ,[BirthdayHours] = @BirthdayHours  "+
                "   ,[SickHours] = @SickHours  "+
                "   ,[VacationHours] = @VacationHours  "+
                "   ,[RegularEarnings] = @RegularEarnings  "+
                "   ,[OvertimeEarnings] = @OvertimeEarnings  "+
                "   ,[OtherStaffPay] = @OtherStaffPay  "+
                "   ,[WacEarnings] = @WacEarnings  "+
                "   ,[EmployerSuiTax] = @EmployerSuiTax  "+
                "   ,[FICA] = @FICA  "+
                "   ,[EmployerRetirementContribution] = @EmployerRetirementContribution  "+
                "   ,[EmployerHealthInsuranceContribution] = @EmployerHealthInsuranceContribution  "+
                "   ,[EmployerDentalVisionContribution] = @EmployerDentalVisionContribution  "+
                "   ,[EmployerDentalVisionOffset] = @EmployerDentalVisionOffset  "+
                "   ,[EmployerLifeInsuranceContribution] = @EmployerLifeInsuranceContribution  "+
                "   ,[EmployerDisibilityInsuranceContribution]  = @EmployerDisibilityInsuranceContribution "+
                "   ,[WorkmenCompTotal] = @WorkmenCompTotal  "+
                "WHERE ID=@ID";

            cmdUpdate.Parameters.Add("@ID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@PayPeriodDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);

            cmdUpdate.Parameters.Add("@CompanyCode", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@FileNumber", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@PayrollWeekNumber", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@HomeDepartment", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@DepartmentWorked", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@RegularHours", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@OvertimeHours", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@CompHours", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@WacJanitorHours", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@UnspecHours", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@HolidayHours", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@PersonalHours ", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@BirthdayHours ", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@SickHours ", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@VacationHours", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@RegularEarnings", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@OvertimeEarnings", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@OtherStaffPay", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@WacEarnings", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@EmployerSuiTax", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@FICA", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@EmployerRetirementContribution", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@EmployerHealthInsuranceContribution", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@EmployerDentalVisionContribution", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@EmployerDentalVisionOffset", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@EmployerLifeInsuranceContribution", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@EmployerDisibilityInsuranceContribution", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@WorkmenCompTotal", SqlDbType.Decimal, 9);

            cmdUpdate.Parameters["@ID"].Value = nId;
            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@PayPeriodDate"].Value = datePayPeriod;
            cmdUpdate.Parameters["@ActualPay"].Value = dActualPay;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNumber;
            cmdUpdate.Parameters["@PayrollWeekNumber"].Value = strPayrollWeekNumber;
            cmdUpdate.Parameters["@HomeDepartment"].Value = strHomeDepartment;
            cmdUpdate.Parameters["@DepartmentWorked"].Value = strDepartmentWorked;
            cmdUpdate.Parameters["@RegularHours"].Value = dRegularHours;
            cmdUpdate.Parameters["@OvertimeHours"].Value = dOvertimeHours;
            cmdUpdate.Parameters["@CompHours"].Value = dCompHours;
            cmdUpdate.Parameters["@WacJanitorHours"].Value = dWacJanitorHours;
            cmdUpdate.Parameters["@UnspecHours"].Value = dUnspecHours;
            cmdUpdate.Parameters["@HolidayHours"].Value = dHolidayHours;
            cmdUpdate.Parameters["@PersonalHours "].Value = dPersonalHours;
            cmdUpdate.Parameters["@BirthdayHours "].Value = dBirthdayHours;
            cmdUpdate.Parameters["@SickHours "].Value = dSickHours;
            cmdUpdate.Parameters["@VacationHours"].Value = dVacationHours;
            cmdUpdate.Parameters["@RegularEarnings"].Value = dRegularEarnings;
            cmdUpdate.Parameters["@OvertimeEarnings"].Value = dOvertimeEarnings;
            cmdUpdate.Parameters["@OtherStaffPay"].Value = dOtherStaffPay;
            cmdUpdate.Parameters["@WacEarnings"].Value = dWacEarnings;
            cmdUpdate.Parameters["@EmployerSuiTax"].Value = dEmployerSuiTax;
            cmdUpdate.Parameters["@FICA"].Value = dFICA;
            cmdUpdate.Parameters["@EmployerRetirementContribution"].Value = dEmployerRetirementContribution;
            cmdUpdate.Parameters["@EmployerHealthInsuranceContribution"].Value =dEmployerHealthInsuranceContribution;
            cmdUpdate.Parameters["@EmployerDentalVisionContribution"].Value = dEmployerDentalVisionContribution;
            cmdUpdate.Parameters["@EmployerDentalVisionOffset"].Value = dEmployerDentalVisionOffset;
            cmdUpdate.Parameters["@EmployerLifeInsuranceContribution"].Value = dEmployerLifeInsuranceContribution;
            cmdUpdate.Parameters["@EmployerDisibilityInsuranceContribution"].Value = dEmployerDisibilityInsuranceContribution;
            cmdUpdate.Parameters["@WorkmenCompTotal"].Value = dWorkmenCompTotal;

            //cmdInsert.Parameters["@UserID"].SourceVersion = DataRowVersion.Original;

            try
            {
                m_Connection.Open();
                cmdUpdate.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }

        public int UpdateCardinalHill(int nID, int nEmployeeID, int nPositionID, DateTime datePayPeriod, Decimal dActualPay,
            DateTime datePay, string strHomeDepartment, DateTime datePeriodEnd, Decimal dRateAmount, Decimal dGrossPay,
            Decimal dOvertimeEarnings, Decimal dBonus, Decimal dShiftPay, Decimal dCellPhone, Decimal dOnCall, Decimal dPTOHours,
            Decimal dPTODollars, Decimal dTotalHoursPaid, Decimal dOvertimeHours, string strDepartmentWorked)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE EmployeeActuals " +
                "SET "+
                "   ActualPay=@ActualPay, " +
                "   PayDate=@PayDate, "+
                "   HomeDepartment=@HomeDepartment, @PeriodEndDate=@PeriodEndDate, "+
                "   GrossPay=@GrossPay, " +
                "   OvertimeEarnings=@OvertimeEarnings, Bonus=@Bonus, ShiftPay=@ShiftPay, "+
                "   CellPhone=@CellPhone, OnCall=@OnCall, PTOHours=@PTOHours, " +
                "   PTODollars=@PTODollars, TotalHoursPaid=@TotalHoursPaid, OvertimeHours=@OvertimeHours, DepartmentWorked=@DepartmentWorked " +
                "WHERE ID=@ID and EmployeeID=@EmployeeID AND PositionID=@PositionID AND PayPeriodDate=@PayPeriodDate AND RateAmount=@RateAmount ";

            cmdUpdate.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@PayPeriodDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@PayDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@HomeDepartment", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@PeriodEndDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@RateAmount", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@GrossPay", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@OvertimeEarnings", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@Bonus", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@ShiftPay", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@CellPhone", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@OnCall", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@PTOHours", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@PTODollars", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@TotalHoursPaid", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@OvertimeHours", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@DepartmentWorked", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@ID", SqlDbType.Int);

            cmdUpdate.Parameters["@ID"].Value = nID;
            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@PayPeriodDate"].Value = datePayPeriod;
            cmdUpdate.Parameters["@ActualPay"].Value = dActualPay;
            cmdUpdate.Parameters["@PayDate"].Value = datePay;
            cmdUpdate.Parameters["@HomeDepartment"].Value = strHomeDepartment;
            cmdUpdate.Parameters["@PeriodEndDate"].Value = datePeriodEnd;
            cmdUpdate.Parameters["@RateAmount"].Value = dRateAmount;
            cmdUpdate.Parameters["@GrossPay"].Value = dGrossPay;
            cmdUpdate.Parameters["@OvertimeEarnings"].Value = dOvertimeEarnings;
            cmdUpdate.Parameters["@Bonus"].Value = dBonus;
            cmdUpdate.Parameters["@ShiftPay"].Value = dShiftPay;
            cmdUpdate.Parameters["@CellPhone"].Value = dCellPhone;
            cmdUpdate.Parameters["@OnCall"].Value = dOnCall;
            cmdUpdate.Parameters["@PTOHours"].Value = dPTOHours;
            cmdUpdate.Parameters["@PTODollars"].Value = dPTODollars;
            cmdUpdate.Parameters["@TotalHoursPaid"].Value = dTotalHoursPaid;
            cmdUpdate.Parameters["@OvertimeHours"].Value = dOvertimeHours;
            cmdUpdate.Parameters["@DepartmentWorked"].Value = strDepartmentWorked;

            try
            {
                m_Connection.Open();
                cmdUpdate.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                String strErr = err.Message;
                throw;
            }
            finally
            {
                m_Connection.Close();
            }
            return nRtn;
        }


        public void Update(int nID, Decimal dActualPay)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE EmployeeActuals " +
                "SET ActualPay = @ActualPay " +
                "WHERE ID = @ID";

            
            cmdUpdate.Parameters.Add("@ActualPay", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@ID", SqlDbType.Int, 4);
            
            cmdUpdate.Parameters["@ActualPay"].Value = dActualPay;
            cmdUpdate.Parameters["@ID"].Value = nID;

            try
            {
                m_Connection.Open();
                cmdUpdate.ExecuteNonQuery();
            }
            finally
            {
                m_Connection.Close();
            }
        }

        public void Delete(int nID)
        {
            SqlCommand cmdDelete = m_Connection.CreateCommand();
            cmdDelete.CommandType = CommandType.Text;
            cmdDelete.CommandText = "DELETE FROM EmployeeActuals " +
                "WHERE ID = @ID ";

            cmdDelete.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdDelete.Parameters["@ID"].Value = nID;

            try
            {
                m_Connection.Open();
                cmdDelete.ExecuteNonQuery();
            }
            finally
            {
                m_Connection.Close();
            }
        }

        public void DeleteAll()
        {
            SqlCommand cmdDelete = m_Connection.CreateCommand();
            cmdDelete.CommandType = CommandType.Text;
            cmdDelete.CommandText = "DELETE FROM EmployeeActuals ";

            try
            {
                m_Connection.Open();
                cmdDelete.ExecuteNonQuery();
            }
            finally
            {
                m_Connection.Close();
            }
        }

        public void DeleteAllForPosition(int nPositionID)
        {
            SqlCommand cmdDelete = m_Connection.CreateCommand();
            cmdDelete.CommandType = CommandType.Text;
            cmdDelete.CommandText = "DELETE FROM EmployeeActuals " +
                "WHERE PositionID = @PositionID ";

            cmdDelete.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));

            cmdDelete.Parameters["@PositionID"].Value = nPositionID;

            try
            {
                m_Connection.Open();
                cmdDelete.ExecuteNonQuery();
            }
            finally
            {
                m_Connection.Close();
            }
        }

        public void DeleteAllForDate(DateTime datePP)
        {
            SqlCommand cmdDelete = m_Connection.CreateCommand();
            cmdDelete.CommandType = CommandType.Text;
            cmdDelete.CommandText = "DELETE FROM EmployeeActuals " +
                "WHERE PayPeriodDate = @PayPeriodDate ";

            cmdDelete.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));

            cmdDelete.Parameters["@PayPeriodDate"].Value = datePP;

            try
            {
                m_Connection.Open();
                cmdDelete.ExecuteNonQuery();
            }
            finally
            {
                m_Connection.Close();
            }
        }

        public void DeleteAllForDateStJoseph(DateTime datePP)
        {
            SqlCommand cmdDelete = m_Connection.CreateCommand();
            cmdDelete.CommandType = CommandType.Text;
            cmdDelete.CommandText = "DELETE FROM EmployeeActuals " +
                "WHERE PayPeriodDate = @PayPeriodDate ";

            cmdDelete.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));

            cmdDelete.Parameters["@PayPeriodDate"].Value = datePP;

            try
            {
                m_Connection.Open();
                cmdDelete.ExecuteNonQuery();
            }
            finally
            {
                m_Connection.Close();
            }
        }

        public void DeleteAllForDateAndCoCode(DateTime datePP, string strCompanyCode)
        {
            SqlCommand cmdDelete = m_Connection.CreateCommand();
            cmdDelete.CommandType = CommandType.Text;
            cmdDelete.CommandText = "DELETE FROM EmployeeActuals " +
                "WHERE PayPeriodDate = @PayPeriodDate AND CompanyCode = @CompanyCode";

            cmdDelete.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));
            cmdDelete.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));

            cmdDelete.Parameters["@PayPeriodDate"].Value = datePP;
            cmdDelete.Parameters["@CompanyCode"].Value = strCompanyCode;

            try
            {
                m_Connection.Open();
                cmdDelete.ExecuteNonQuery();
            }
            finally
            {
                m_Connection.Close();
            }
        }

        public void DeleteAllForEmployeeID(int nEmployeeID)
        {
            SqlCommand cmdDelete = m_Connection.CreateCommand();
            cmdDelete.CommandType = CommandType.Text;
            cmdDelete.CommandText = "DELETE FROM EmployeeActuals " +
                "WHERE EmployeeID = @EmployeeID ";

            cmdDelete.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));

            cmdDelete.Parameters["@EmployeeID"].Value = nEmployeeID;

            try
            {
                m_Connection.Open();
                cmdDelete.ExecuteNonQuery();
            }
            finally
            {
                m_Connection.Close();
            }
        }

        public DataTable GetAllEmployeePosition()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ea.id,ea.PayPeriodStart,ea.PayPeriodEnd,ea.ActualPay,ea.TotalHours," + 
                                                    "p.title as posTitle, per.Lastname+','+per.firstName as Name FROM EmployeeActuals ea " +
                                                    "INNER JOIN positions p on p.id = ea.PositionId " +
                                                    "INNER JOIN employees e on e.id = ea.employeeid " +
                                                    "INNER JOIN persons per on e.personid = per.id", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }


        public DataTable GetAll()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM EmployeeActuals", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }

        public DataTable GetAllWithPersonName()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT employeeActuals.*, LastName+', '+FirstName as PersonName "+
                "FROM EmployeeActuals "+
                "   INNER JOIN Employees on Employees.Id = employeeActuals.EmployeeId "+
                "   INNER JOIN Persons on Persons.Id = Employees.PersonId", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }

        public DataTable GetAll(int nPositionID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * " +
                "FROM EmployeeActuals " +
                "WHERE PositionID = @PositionID ORDER BY PayPeriodDate Desc", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }

        public DataTable GetAll(DateTime datePayPeriod)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * " +
                "FROM EmployeeActuals " +
                "   INNER JOIN Positions "+
                "       ON Positions.id = EmployeeActuals.PositionID "+
                "   INNER JOIN Employees "+
                "       ON Employees.id = ePositions.EmployeeID "+
                "   INNER JOIN Persons "+
                "       ON Persons.ID = Employees.PersonID "+
                "WHERE PayPeriodDate = @PayPeriodDate ORDER BY LastName ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));

            cmdSelect.Parameters["@PayPeriodDate"].Value = datePayPeriod;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }
        public DataTable GetListOfPayDates()
        {
            SqlCommand cmdSelect = new SqlCommand(" "+
                "SELECT payPeriodDate, CONVERT(VARCHAR(10),payperioddate,101) as PayPeriodDateString "+ 
                "FROM "+
	            "    ( "+
		        "        SELECT DISTINCT PayPeriodDate "+ 
		        "        FROM EmployeeActuals 	"+	
	            "    ) x "+
                "ORDER BY PayPeriodDate DESC", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }

        public DataTable GetAllLatest()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Positions.title as PositionTitle, "+
                "EmployeeActuals.*, PayPeriodDate as StartDate, Persons.lastName+', '+Persons.FirstName as PersonName " +
                "FROM EmployeeActuals " +
                "   INNER JOIN Positions " +
                "       ON Positions.id = EmployeeActuals.PositionID " +
                "   INNER JOIN Employees " +
                "       ON Employees.id = EmployeeActuals.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "WHERE EmployeeActuals.PayPeriodDate = (SELECT TOP(1) PayPeriodDate FROM EmployeeActuals ORDER BY PayPeriodDate DESC) "+
                "ORDER BY Persons.LastName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }

        public DataTable GetAllByDate(DateTime datePayPeriod)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Positions.title as PositionTitle, " +
                "EmployeeActuals.*, PayPeriodDate as StartDate, Persons.lastName+', '+Persons.FirstName as PersonName " +
                "FROM EmployeeActuals " +
                "   INNER JOIN Positions " +
                "       ON Positions.id = EmployeeActuals.PositionID " +
                "   INNER JOIN Employees " +
                "       ON Employees.id = EmployeeActuals.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "WHERE EmployeeActuals.PayPeriodDate = @PayPeriodDate " +
                "ORDER BY Persons.LastName, Persons.FirstName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));

            cmdSelect.Parameters["@PayPeriodDate"].Value = datePayPeriod;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }

        public DataTable GetAllByDateForMerrimackAcutalsImportGrid(DateTime datePayPeriod)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Positions.title as PositionTitle, " +
                "EmployeeActuals.*, PayPeriodDate, Persons.lastName+', '+Persons.FirstName as PersonName " +
                "FROM EmployeeActuals " +
                "   INNER JOIN Positions " +
                "       ON Positions.id = EmployeeActuals.PositionID " +
                "   INNER JOIN Employees " +
                "       ON Employees.id = EmployeeActuals.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "WHERE EmployeeActuals.PayPeriodDate = @PayPeriodDate " +
                "ORDER BY Persons.LastName, Persons.FirstName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));

            cmdSelect.Parameters["@PayPeriodDate"].Value = datePayPeriod;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }

        public DataTable GetAllByDateForMerrimackAcutalsImportGrid()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Positions.title as PositionTitle, Positions.Code as PositionCode" +
                "   ,EmployeeActuals.*, PayPeriodDate, Persons.lastName+', '+Persons.FirstName as PersonName, Jobs.Code as JobCode " +
                "FROM EmployeeActuals " +
                "   INNER JOIN Positions " +
                "       ON Positions.id = EmployeeActuals.PositionID " +
                "   INNER JOIN Jobs on jobs.id = Positions.JobId "+
                "   INNER JOIN Employees " +
                "       ON Employees.id = EmployeeActuals.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "ORDER BY PayPeriodDate DESC, JobCode, Persons.LastName, Persons.FirstName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }


        public DataTable GetAll(int nPositionID, DateTime dateStart, DateTime dateEnd, string strSort)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * " +
                "FROM EmployeeActuals " +
                "WHERE PositionID = @PositionID AND PayPeriodDate >= @StartDate and PayPeriodDate <= @EndDate  ORDER BY @SortStartDate ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@SortStartDate", SqlDbType.VarChar, 20));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@StartDate"].Value = dateStart;
            cmdSelect.Parameters["@EndDate"].Value = dateEnd;
            cmdSelect.Parameters["@SortStartDate"].Value = strSort;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }

        public DataTable GetActualToBudgetReportPriorToFiscalYearFix(int nPositionID, DateTime dateStart, DateTime dateEnd)
        {
            SqlCommand cmdSelect = new SqlCommand("  "+
                "Select Persons.LastName+','+Persons.FirstName as PersonName, Cast(ActualPay as varchar(50)) as Actual, '' as Budgeted , '' as Variance " +
	            "FROM ePositions "+
		        "    INNER JOIN Positions "+
			    "        ON Positions.id = ePositions.positionID "+
		        "    INNER JOIN hrBusinessLevels ON hrBusinessLEvels.id = positions.businessLevelID "+
		        "    INNER JOIN Jobs ON Jobs.ID = Positions.jobID "+
		        "    INNER JOIN (Select ePositionID,   Sum(ActualPay) ActualPay  "+
			    "        FROM ePositions "+
				"            INNER JOIN Positions "+
			    "		            ON Positions.id = ePositions.positionID "+
			    "	         INNER JOIN EmployeeActuals "+
				"	            ON EmployeeActuals.ePositionID = ePositions.ID "+
				"            WHERE positions.id = @PositionID  and  "+
				"	            EmployeeActuals.StartDate >= @StartDate and EmployeeActuals.StartDate <= @EndDate "+
				"            GROUP BY ePositionID) epa "+
			    "        ON epa.ePositionID = ePositions.ID "+
		        "    INNER JOIN employees "+
			    "        ON employees.ID = ePositions.employeeID "+
		        "    INNER JOIN Persons "+ 
			    "        ON persons.id = employees.personid "+
                "Union All "+
                "select '-----------------------------------------------', '--------------', '', '' "+
                "Union All "+
                "Select 'Total', Cast(Sum(epa.ActualPay) as varchar(50)) as Actual, "+ 
		        "        CAST((Select  Cast(Sum(PositionBudgetMonths.BudgetAmount) as varchar(50) ) as Budgeted "+
                " "+		
			    "               FROM PositionBudgetMonths "+
				"	                Inner JOIN PositionBudgets on PositionBudgets.ID = PositionBudgetMonths.PositionBudgetsID "+
				"	                INNER JOIN Positions ON Positions.ID = PositionBudgets.PositionID "+
                "               WHERE Positions.ID = @PositionID AND PositionBudgets.BudgetYear = DatePart(yyyy, @StartDate)  and " + 
				"	                PositionBudgetMonths.BudgetMonth >= DatePart(m, @StartDate) "+ 
				"	                and PositionBudgetMonths.BudgetMonth <= DatePart(m, @EndDate)) as Varchar(50)) as Budgeted, "+
                " "+
			    "            CAST(((Select Sum(PositionBudgetMonths.BudgetAmount) "+
                " "+		
			    "                   FROM PositionBudgetMonths "+
				"	                    INNER JOIN PositionBudgets on PositionBudgets.ID = PositionBudgetMonths.PositionBudgetsID "+
				"	                    INNER JOIN Positions ON Positions.ID = PositionBudgets.PositionID "+
                "                   WHERE  Positions.ID = @PositionID AND PositionBudgets.BudgetYear = DatePart(yyyy, @StartDate)  and  " +
				"	                    PositionBudgetMonths.BudgetMonth >= DatePart(m, @StartDate) "+ 
				"	                    and PositionBudgetMonths.BudgetMonth <= DatePart(m, @EndDate)) - Sum(epa.ActualPay) ) as Varchar(50)) as Variance "+		
                "FROM "+ 
	            "    (Select ePositionID,   Sum(ActualPay) ActualPay "+ 
			    "            FROM ePositions "+
				"                INNER JOIN Positions "+
				"	                ON Positions.id = ePositions.positionID "+
				"                INNER JOIN EmployeeActuals "+
				"	                ON EmployeeActuals.ePositionID = ePositions.ID "+
				"                where positions.id = @PositionID and DatePart(yyyy,EmployeeActuals.StartDate) = DatePart(yyyy, @StartDate) and "+
				"	                EmployeeActuals.StartDate >= @StartDate and EmployeeActuals.StartDate <= @EndDate "+
				"                group by ePositionID) epa "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));
            
            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@StartDate"].Value = dateStart;
            cmdSelect.Parameters["@EndDate"].Value = dateEnd;
            

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }

        public DataTable GetActualToBudgetReport(int nPositionID, DateTime dateStart, DateTime dateEnd, int nFiscalYear, int nFiscalYearStartMonth)
        {
            SqlCommand cmdSelect = new SqlCommand("  "+
                "DECLARE @StartDateBudget as datetime  "+ 
                "Set @StartDateBudget = CAST(DATEPART(m,@StartDate) as varchar)+'/1/'+CAST(DATEPART(YYYY,@StartDate) as varchar) "+ 
                "DECLARE @EndDateBudget as datetime "+ 
                "Set @EndDateBudget = CAST(DATEPART(m,@EndDate) as varchar)+'/1/'+CAST(DATEPART(YYYY,@EndDate) as varchar) "+ 
                " "+
"Select '1' SortPosition, Persons.LastName+','+Persons.FirstName as PersonName,  "+ 
"    '' as Budgeted, Cast(COALESCE(epa.ActualPay,0.0) as varchar(50)) as Actual,  '' as Variance   "+
"FROM   "+
"    (  Select EmployeeID,PositionID,   COALESCE(Sum(ActualPay), 0.0) ActualPay "+    
"		FROM EmployeeActuals  "+  
"			INNER JOIN Positions "+   
"				ON Positions.id = EmployeeActuals.positionID    "+
"			INNER JOIN Employees    "+
"				ON Employees.ID = EmployeeActuals.EmployeeID    "+
"		WHERE positions.id = @PositionID  AND     "+
"			EmployeeActuals.PayPeriodDate >= @StartDate and EmployeeActuals.PayPeriodDate <= @EndDate    "+
"		GROUP BY EmployeeActuals.EmployeeID, PositionID "+
"											) epa  " +
"	LEFT JOIN Employees "+
"		ON Employees.ID = epa.EmployeeID  "+
"   LEFT JOIN Positions   "+
"       ON Positions.id = epa.positionID  "+ 
"   LEFT JOIN hrBusinessLevels ON hrBusinessLEvels.id = positions.businessLevelID  "+
"   LEFT JOIN Jobs ON Jobs.ID = Positions.jobID  "+ 
"   LEFT JOIN Persons    "+
"       ON persons.id = employees.PersonID   "+
"    WHERE Positions.ID = @PositionID  "+
" "+
"Union All  "+ 
"SELECT '2', '', '', '',''  "+
" "+
"Union All  "+ 
"Select '3', 'Total', Cast(COALESCE(Sum(PosBudgeted.Budgeted), 0.0) as varchar(50)) as Budgeted,  "+  
"    Cast(COALESCE(Sum(epa.ActualPay),0.0) as varchar(50)) as Actual,   "+
"    Cast((COALESCE(Sum(PosBudgeted.Budgeted),0.0) - COALESCE(Sum(epa.ActualPay),0.0)) as varchar(50)) as Variance   "+		
"FROM Positions  "+
"    LEFT JOIN   "+
"        (Select   Sum(ActualPay) ActualPay, Positions.ID as PositionID    "+
"                FROM Positions  "+ 
"                    LEFT JOIN EmployeeActuals   "+
"	                    ON Positions.id = EmployeeActuals.positionID   "+
"                    where positions.id = @PositionID   "+  
"	                    AND EmployeeActuals.PayPeriodDate >= @StartDate and EmployeeActuals.PayPeriodDate <= @EndDate   "+
"                    group by Positions.ID) epa  "+
"        ON Positions.ID = epa.PositionID  "+
"    LEFT JOIN (    "+
"                SELECT  Positions.ID, COALESCE(Sum(PositionBudgetMonthsWithYear.BudgetAmount), 0.0) as Budgeted  "+
"                FROM Positions  "+
"                    LEFT JOIN PositionBudgets  "+  
"	                    ON Positions.ID = PositionBudgets.PositionID  "+ 
"                    LEFT JOIN  "+    
"	                    (	SELECT PositionBudgetMonths.* , BudgetMonthDate =   "+
"					                    CASE    "+
"						                    WHEN PositionBudgetMonths.BudgetMonth >= @FiscalYearStartMonth AND PositionBudgetMonths.BudgetMonth <= 12 THEN Convert(DateTime, CAST(PositionBudgetMonths.BudgetMonth as varchar)+'/1/'+CAST(@FiscalYear as varchar), 101)  "+ 
"						                    ELSE Convert(DateTime, CAST(PositionBudgetMonths.BudgetMonth as varchar)+'/1/'+CAST((@FiscalYear+1) as varchar), 101)  "+ 
"					                    END  "+ 
"		                    FROM PositionBudgetMonths INNER JOIN PositionBudgets  ON PositionBudgetMonths.PositionBudgetsID = PositionBudgets.ID  "+
"		                    WHERE budgetYear = @FiscalYear  "+ 
"	                    ) PositionBudgetMonthsWithYear  "+ 
"		                    ON PositionBudgets.ID = PositionBudgetMonthsWithYear.PositionBudgetsID  "+ 
"                WHERE PositionBudgets.BudgetYear = @FiscalYear  and   "+  
"                    PositionBudgetMonthsWithYear.BudgetMonthDate >= @StartDateBudget  "+    
"                    AND PositionBudgetMonthsWithYear.BudgetMonthDate <= @EndDateBudget   "+
"                    AND Positions.ID = @PositionID  "+ 
"                GROUP BY Positions.ID)  as PosBudgeted  "+ 
"        ON Positions.ID = PosBudgeted.ID "+
"WHERE Positions.ID = @PositionID   "+  
"ORDER BY SortPosition, PersonName   "
                , m_Connection);


            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@FiscalYear", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@FiscalYearStartMonth", SqlDbType.Int, 4));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@StartDate"].Value = dateStart;
            cmdSelect.Parameters["@EndDate"].Value = dateEnd;
            cmdSelect.Parameters["@FiscalYear"].Value = nFiscalYear;
            cmdSelect.Parameters["@FiscalYearStartMonth"].Value = nFiscalYearStartMonth;


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }


        public DataRow GetRecord(int nEmployeeID, int nPositionID, DateTime datePayPeriod)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * " +
                "FROM EmployeeActuals " +
                "WHERE PositionID = @PositionID AND EmployeeID = @EmployeeID AND PayPeriodDate = @PayPeriodDate ", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@EmployeeID"].Value = nEmployeeID;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@PositionID"].Value = nPositionID;

            cmdSelect.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));
            cmdSelect.Parameters["@PayPeriodDate"].Value = datePayPeriod;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            if (ds.Tables["EmployeeActuals"].Rows.Count > 0)
                return ds.Tables["EmployeeActuals"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecord(int nEmployeeID, int nPositionID, DateTime datePayPeriod, Decimal dRateAmount)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * " +
                "FROM EmployeeActuals " +
                "WHERE PositionID = @PositionID AND EmployeeID = @EmployeeID AND PayPeriodDate = @PayPeriodDate AND RateAmount = @RateAmount ", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@EmployeeID"].Value = nEmployeeID;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@PositionID"].Value = nPositionID;

            cmdSelect.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));
            cmdSelect.Parameters["@PayPeriodDate"].Value = datePayPeriod;

            cmdSelect.Parameters.Add(new SqlParameter("@RateAmount", SqlDbType.Decimal, 9));
            cmdSelect.Parameters["@RateAmount"].Value = dRateAmount;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            if (ds.Tables["EmployeeActuals"].Rows.Count > 0)
                return ds.Tables["EmployeeActuals"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecord(int nEmployeeID, int nPositionID, DateTime datePayPeriod, 
            Decimal dRateAmount, string strDepartmentWorked)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * " +
                "FROM EmployeeActuals " +
                "WHERE PositionID = @PositionID AND EmployeeID = @EmployeeID AND "+
                "   PayPeriodDate = @PayPeriodDate AND RateAmount = @RateAmount "+
                "   AND DepartmentWorked=@DepartmentWorked", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@EmployeeID"].Value = nEmployeeID;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@PositionID"].Value = nPositionID;

            cmdSelect.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));
            cmdSelect.Parameters["@PayPeriodDate"].Value = datePayPeriod;

            cmdSelect.Parameters.Add(new SqlParameter("@RateAmount", SqlDbType.Decimal, 9));
            cmdSelect.Parameters["@RateAmount"].Value = dRateAmount;

            cmdSelect.Parameters.Add(new SqlParameter("@DepartmentWorked", SqlDbType.VarChar, 50));
            cmdSelect.Parameters["@DepartmentWorked"].Value = strDepartmentWorked;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            if (ds.Tables["EmployeeActuals"].Rows.Count == 1)
                return ds.Tables["EmployeeActuals"].Rows[0];
            else if (ds.Tables["EmployeeActuals"].Rows.Count == 0)
                return null;
            else
                throw new Exception("More than one record for employeeid, positionid, payperiodDate, Rate amount, Department Worked");
        }

        public DataTable GetAllWithNameEntAllergy()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT lastName, Firstname, eac.PayPeriodDate, eac.ActualPay "+
                "   , Employees.CompanyCode, employees.FileNumber " +
                "FROM EmployeeActuals eac " +
                "   JOIN Employees ON Employees.id = eac.employeeId " +
                "   JOIN Persons on persons.id = employees.personid " +
                "   JOIN Positions on Positions.Id = eac.PositionId " +
                "ORDER BY LastName, FirstName, Employees.CompanyCode, Employees.FileNumber, PayPeriodDate DESC"
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActualsByCostCenter");

            return ds.Tables["EmployeeActualsByCostCenter"];
        }

    }
}

