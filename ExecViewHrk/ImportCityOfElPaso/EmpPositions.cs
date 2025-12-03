using System;
using System.Data;
//using System.Configuration;
//using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
//using System.Collections.Generic;
using System.Data.SqlClient;

namespace ImportCityOfAlPaso
{

    /// <summary>
    /// Summary description for ePositions
    /// </summary>
    public class EmpPositions
    {
        private SqlConnection m_Connection;

        public EmpPositions(SqlConnection connection)
        {
            m_Connection = connection;
        }

        public int Insert(int nEmployeeID, int nPositionID, int nPosCatID)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PositionCategoryID) " +
                "VALUES(@EmployeeID, @PositionID, @PositionCategoryID)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionCategoryID", SqlDbType.Int, 4);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PositionCategoryID"].Value = nPosCatID;

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

        public int InsertYmca(int nEmployeeID, int nPositionID, int nDepartmentId, int nLocationId, int nAccountId
            ,int nPcsCodeId, int nSupervisorId, DateTime? startDate, DateTime? endDate)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, DepartmentId, LocationId, AccountId, PcsCodeId, SupervisorId, StartDate, ActualEndDate) " +
                "VALUES(@EmployeeID, @PositionID, @DepartmentId, @LocationId, @AccountId, @PcsCodeId, @SupervisorId, @StartDate, @EndDate)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@DepartmentID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@LocationID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@AccountID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PcsCodeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@SupervisorID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.Date);
            cmdInsert.Parameters.Add("@EndDate", SqlDbType.Date);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@DepartmentID"].Value = nDepartmentId;
            cmdInsert.Parameters["@LocationID"].Value = nLocationId;
            cmdInsert.Parameters["@AccountID"].Value = nAccountId;

            if (nPcsCodeId == 0)
                cmdInsert.Parameters["@PcsCodeID"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@PcsCodeID"].Value = nPcsCodeId;

            cmdInsert.Parameters["@SupervisorID"].Value = nSupervisorId;
            cmdInsert.Parameters["@StartDate"].Value = startDate;
            
            if (endDate == null)
                cmdInsert.Parameters["@EndDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@EndDate"].Value = endDate;

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

        public int InsertForHRP(int nEmployeeID, int nPositionID, int nPosCatID, string strPayFreqCode, bool bIsPrimaryPosition,
            DateTime dateStart, string strRateTypeCode)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PositionCategoryID, payFrequencyCode, PrimaryPosition, StartDate, RateTypeCode) " +
                "VALUES(@EmployeeID, @PositionID, @PositionCategoryID, @PayFrequencyCode, @PrimaryPosition, @StartDate, @RateTypeCode)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionCategoryID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PayFrequencyCode", SqlDbType.VarChar, 10);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@RateTypeCode", SqlDbType.VarChar, 1);


            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PositionCategoryID"].Value = nPosCatID;
            cmdInsert.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;

            if (dateStart == DateTime.MaxValue)
                cmdInsert.Parameters["@StartDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@StartDate"].Value = dateStart;

            cmdInsert.Parameters["@RateTypeCode"].Value = strRateTypeCode;
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

        public int InsertForHRB(int nEmployeeID, int nPositionID, int nPosCatID, bool bIsPrimaryPosition, string strPayFreqCode,
                                DateTime dateStart)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PositionCategoryID, PrimaryPosition, PayFrequencyCode, startDate) " +
                "VALUES(@EmployeeID, @PositionID, @PositionCategoryID, @PrimaryPosition, @PayFrequencyCode, @StartDate)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionCategoryID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@PayFrequencyCode", SqlDbType.VarChar, 10);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);


            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PositionCategoryID"].Value = nPosCatID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;

            if (strPayFreqCode.ToUpper() == "B" || strPayFreqCode.ToUpper() == "W" || strPayFreqCode.ToUpper() == "M")
                cmdInsert.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            else
                cmdInsert.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;

            cmdInsert.Parameters["@StartDate"].Value = dateStart;

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

        public int InsertForHudsonWebServices(int nEmployeeID, int nPositionID, int nPosCatID, bool bIsPrimaryPosition, string strPayFreqCode,
                                DateTime dateStart, int nBudgetCode, Decimal dFte, DateTime actualEndDate)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PositionCategoryID, PrimaryPosition, PayFrequencyCode, startDate, BudgetCode, Fte, ActualEndDate) " +
                "VALUES(@EmployeeID, @PositionID, @PositionCategoryID, @PrimaryPosition, @PayFrequencyCode, @StartDate, @BudgetCode, @Fte, @ActualEndDate)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionCategoryID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@PayFrequencyCode", SqlDbType.VarChar, 10);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@BudgetCode", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@Fte", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);


            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PositionCategoryID"].Value = nPosCatID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;

            if (strPayFreqCode.ToUpper() == "B" || strPayFreqCode.ToUpper() == "W" || strPayFreqCode.ToUpper() == "M")
                cmdInsert.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            else
                cmdInsert.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;

            cmdInsert.Parameters["@StartDate"].Value = dateStart;
            cmdInsert.Parameters["@BudgetCode"].Value = nBudgetCode;
            cmdInsert.Parameters["@Fte"].Value = dFte;

            if (actualEndDate == DateTime.MaxValue)
                cmdInsert.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ActualEndDate"].Value = actualEndDate;

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

        public int InsertForHRBCardinalHill(int nEmployeeID, int nPositionID, int nPosCatID, bool bIsPrimaryPosition, string strPayFreqCode,
                                DateTime dateStart, DateTime dateEnd)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PositionCategoryID, PrimaryPosition, PayFrequencyCode, startDate, actualEndDate) " +
                "VALUES(@EmployeeID, @PositionID, @PositionCategoryID, @PrimaryPosition, @PayFrequencyCode, @StartDate, @ActualEndDate)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionCategoryID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@PayFrequencyCode", SqlDbType.VarChar, 10);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);


            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PositionCategoryID"].Value = nPosCatID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;

            if (strPayFreqCode.ToUpper() == "B" || strPayFreqCode.ToUpper() == "W" || strPayFreqCode.ToUpper() == "M")
                cmdInsert.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            else
                cmdInsert.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;

            if (dateStart == DateTime.MaxValue)
                cmdInsert.Parameters["@StartDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@StartDate"].Value = dateStart;

            if (dateEnd == DateTime.MaxValue)
                cmdInsert.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;

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

        public int InsertForAdpWebSevicesImport(int nEmployeeID, int nPositionID, bool bIsPrimaryPosition,
            DateTime dateStart, DateTime dateEnd, string strEnteredBy, DateTime dateEntered, DateTime importedDate,string FileNumber,string StrNewPositionId)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PrimaryPosition, startDate, actualEndDate, EnteredBy, EnteredDate, ImportedDate,fileNumber,NewPositionId) " +
                "VALUES(@EmployeeID, @PositionID, @PrimaryPosition, @StartDate, @actualEndDate, @EnteredBy, @EnteredDate, @ImportedDate,@fileNumber,@NewPositionId)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@EnteredDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ImportedDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@fileNumber", SqlDbType.VarChar,10);
            cmdInsert.Parameters.Add("@NewPositionId", SqlDbType.VarChar, 10);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdInsert.Parameters["@StartDate"].Value = dateStart;
            cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;
            cmdInsert.Parameters["@EnteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@EnteredDate"].Value = dateEntered;
            cmdInsert.Parameters["@ImportedDate"].Value = importedDate;
            cmdInsert.Parameters["@fileNumber"].Value = FileNumber;
            cmdInsert.Parameters["@NewPositionId"].Value = StrNewPositionId;

            if (dateEnd == DateTime.MaxValue)
                cmdInsert.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;

            cmdInsert.Parameters["@StartDate"].Value = dateStart;

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

        public int InsertForAdpNational(int nEmployeeID, int nPositionID, bool bIsPrimaryPosition,
            DateTime dateStart, DateTime dateEnd, string strEnteredBy, DateTime dateEntered, DateTime importedDate
            , string strRateTypeCode, decimal? dRate, string strPayFrequencyCode, string strImportedRateTypeCode, decimal? dImportedRate)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PrimaryPosition, startDate, actualEndDate, EnteredBy, EnteredDate, ImportedDate, payRate, RateTypeCode, PayFrequencyCode, ImportedRateTypeCode, ImportedPayRate) " +
                "VALUES(@EmployeeID, @PositionID, @PrimaryPosition, @StartDate, @actualEndDate, @EnteredBy, @EnteredDate, @ImportedDate, @payRate, @RateTypeCode, @PayFrequencyCode, @ImportedRateTypeCode, @ImportedPayRate)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@EnteredDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ImportedDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@PayRate", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@RateTypeCode", SqlDbType.VarChar, 1);
            cmdInsert.Parameters.Add("@PayFrequencyCode", SqlDbType.VarChar, 10);
            cmdInsert.Parameters.Add("@ImportedPayRate", SqlDbType.Decimal, 9);
            cmdInsert.Parameters.Add("@ImportedRateTypeCode", SqlDbType.VarChar, 10);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdInsert.Parameters["@StartDate"].Value = dateStart;
            cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;
            cmdInsert.Parameters["@EnteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@EnteredDate"].Value = dateEntered;
            cmdInsert.Parameters["@ImportedDate"].Value = importedDate;

            if (dateEnd == DateTime.MaxValue)
                cmdInsert.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;
            

            cmdInsert.Parameters["@StartDate"].Value = dateStart;

            if (dRate == null)
                cmdInsert.Parameters["@PayRate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@PayRate"].Value = dRate;

            if (strRateTypeCode == null)
                cmdInsert.Parameters["@RateTypeCode"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@RateTypeCode"].Value = strRateTypeCode;

            if (strPayFrequencyCode == null)
                cmdInsert.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@PayFrequencyCode"].Value = strPayFrequencyCode;

            if (dImportedRate == null)
                cmdInsert.Parameters["@ImportedPayRate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ImportedPayRate"].Value = dImportedRate;

            if (strImportedRateTypeCode == null)
                cmdInsert.Parameters["@ImportedRateTypeCode"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ImportedRateTypeCode"].Value = strImportedRateTypeCode;

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

        public int InsertPhoenix(int nEmployeeID, int nPositionID, bool bIsPrimaryPosition,
            DateTime dateStart, DateTime? dateEnd, string strEnteredBy, DateTime dateEntered, int? nAdpPositionNumber)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PrimaryPosition, startDate, actualEndDate, EnteredBy, EnteredDate, AdpPositionNumber) " +
                "VALUES(@EmployeeID, @PositionID, @PrimaryPosition, @StartDate, @actualEndDate, @EnteredBy, @EnteredDate, @AdpPositionNumber)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@EnteredDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@AdpPositionNumber", SqlDbType.Int, 4);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdInsert.Parameters["@StartDate"].Value = dateStart;
            cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;
            cmdInsert.Parameters["@EnteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@EnteredDate"].Value = dateEntered;
            


            if (dateEnd == null)
                cmdInsert.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;

            if (nAdpPositionNumber == null)
                cmdInsert.Parameters["@AdpPositionNumber"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@AdpPositionNumber"].Value = nAdpPositionNumber;

            cmdInsert.Parameters["@StartDate"].Value = dateStart;

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

        public int InsertAdpNational(int nEmployeeID, int nPositionID, bool bIsPrimaryPosition,
            DateTime dateStart, DateTime? dateEnd, string strEnteredBy, DateTime dateEntered, 
            string strRateTypeCode, Decimal? dRate, string strPayFrequencyCode)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PrimaryPosition, startDate, actualEndDate, EnteredBy, EnteredDate, RateTypeCode, PayRate, PayFrequencyCode ) " +
                "VALUES(@EmployeeID, @PositionID, @PrimaryPosition, @StartDate, @actualEndDate, @EnteredBy, @EnteredDate, @RateTypeCode, @PayRate, @PayFrequencyCode)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@EnteredDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add(new SqlParameter("@RateTypeCode", SqlDbType.VarChar, 1));
            cmdInsert.Parameters.Add(new SqlParameter("@PayRate", SqlDbType.Decimal, 9));
            cmdInsert.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdInsert.Parameters["@StartDate"].Value = dateStart;
            cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;
            cmdInsert.Parameters["@EnteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@EnteredDate"].Value = dateEntered;



            if (dateEnd == null)
                cmdInsert.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;


            if (dRate == null)
                cmdInsert.Parameters["@PayRate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@PayRate"].Value = dRate;

            cmdInsert.Parameters["@RateTypeCode"].Value = strRateTypeCode;
            cmdInsert.Parameters["@PayFrequencyCode"].Value = strPayFrequencyCode;



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

        public int InsertHudson(int nEmployeeID, int nPositionID, bool bIsPrimaryPosition,
            DateTime dateStart, DateTime? dateEnd, string strEnteredBy, DateTime dateEntered,
            int? nBudgetCode)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PrimaryPosition, startDate, actualEndDate, EnteredBy, EnteredDate, BudgetCode ) " +
                "VALUES(@EmployeeID, @PositionID, @PrimaryPosition, @StartDate, @actualEndDate, @EnteredBy, @EnteredDate, @BudgetCode)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@EnteredDate", SqlDbType.DateTime);
            //cmdInsert.Parameters.Add(new SqlParameter("@RateTypeCode", SqlDbType.VarChar, 1));
            //cmdInsert.Parameters.Add(new SqlParameter("@PayRate", SqlDbType.Decimal, 9));
            //cmdInsert.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdInsert.Parameters.Add("@BudgetCode", SqlDbType.Int, 4);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdInsert.Parameters["@StartDate"].Value = dateStart;
            cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;
            cmdInsert.Parameters["@EnteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@EnteredDate"].Value = dateEntered;



            if (dateEnd == null)
                cmdInsert.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;


            //if (dRate == null)
            //    cmdInsert.Parameters["@PayRate"].Value = System.DBNull.Value;
            //else
            //    cmdInsert.Parameters["@PayRate"].Value = dRate;

            //cmdInsert.Parameters["@RateTypeCode"].Value = strRateTypeCode;
            //cmdInsert.Parameters["@PayFrequencyCode"].Value = strPayFrequencyCode;

            if (nBudgetCode == null)
                cmdInsert.Parameters["@BudgetCode"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@BudgetCode"].Value = nBudgetCode;



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

        public int InsertForPositions(string nEmployeeID, string nPositionID, string nsalary)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, salary) " +
                "VALUES(@EmployeeID, @PositionID, @salary)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);

            cmdInsert.Parameters.Add("@salary", SqlDbType.Money);



            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = Convert.ToInt32(nEmployeeID);
            cmdInsert.Parameters["@PositionID"].Value = Convert.ToInt32(nPositionID);
            if (nsalary != "")
            {
                cmdInsert.Parameters["@salary"].Value = Convert.ToDecimal(nsalary);
            }
            else
            {

                cmdInsert.Parameters["@salary"].Value = DBNull.Value;
            }


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

        public int EPositionSalHistoryInsert(int neposid, string nratetype, string nrate, string ndayperiod, string nhoursperiod, string neffectivedate, decimal nannsalary)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositionSalaryHistory " +
                "(ePositionID,RateTypeCode, Rate, DaysPerPayPeriod,HoursPerPayPeriod,effectiveDate,AnnualSalary) " +
                "VALUES(@ePositionID,@RateTypeCode, @Rate, @DaysPerPayPeriod,@HoursPerPayPeriod,@effectiveDate,@AnnualSalary)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@ePositionID", SqlDbType.Int);
            cmdInsert.Parameters.Add("@RateTypeCode", SqlDbType.Char);
            cmdInsert.Parameters.Add("@Rate", SqlDbType.Decimal);
            cmdInsert.Parameters.Add("@DaysPerPayPeriod", SqlDbType.Decimal);
            cmdInsert.Parameters.Add("@HoursPerPayPeriod", SqlDbType.Decimal);
            cmdInsert.Parameters.Add("@effectiveDate", SqlDbType.SmallDateTime);
            cmdInsert.Parameters.Add("@AnnualSalary", SqlDbType.Decimal);

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;
            cmdInsert.Parameters["@ePositionID"].Value = neposid;
            cmdInsert.Parameters["@RateTypeCode"].Value = nratetype;
            cmdInsert.Parameters["@Rate"].Value = Convert.ToDecimal(nrate);
            cmdInsert.Parameters["@DaysPerPayPeriod"].Value = Convert.ToDecimal(ndayperiod);
            cmdInsert.Parameters["@HoursPerPayPeriod"].Value = Convert.ToDecimal(nhoursperiod);
            cmdInsert.Parameters["@effectiveDate"].Value = Convert.ToDateTime(neffectivedate);
            cmdInsert.Parameters["@AnnualSalary"].Value = Convert.ToDecimal(nannsalary);
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

        public int InsertForAdpWebSevicesImportSpringfield(int nEmployeeID, int nPositionID, bool bIsPrimaryPosition,
            DateTime dateStart, DateTime dateEnd, string strEnteredBy, DateTime dateEntered, string strCompanyCode, string strFileNumber)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PrimaryPosition, startDate, actualEndDate, EnteredBy, EnteredDate, CompanyCode, FileNumber) " +
                "VALUES(@EmployeeID, @PositionID, @PrimaryPosition, @StartDate, @actualEndDate, @EnteredBy, @EnteredDate, @CompanyCode, @FileNumber)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@EnteredDate", SqlDbType.DateTime);
            cmdInsert.Parameters.Add("@CompanyCode", SqlDbType.VarChar, 50);
            cmdInsert.Parameters.Add("@FileNumber", SqlDbType.VarChar, 50);


            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdInsert.Parameters["@StartDate"].Value = dateStart;
            cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;
            cmdInsert.Parameters["@EnteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@EnteredDate"].Value = dateEntered;

            if (dateEnd == DateTime.MaxValue)
                cmdInsert.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@ActualEndDate"].Value = dateEnd;

            cmdInsert.Parameters["@StartDate"].Value = dateStart;

            if (string.IsNullOrEmpty(strCompanyCode))
                cmdInsert.Parameters["@CompanyCode"].Value = System.DBNull.Value;
            else
                cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;

            if (string.IsNullOrEmpty(strFileNumber))
                cmdInsert.Parameters["@FileNumber"].Value = System.DBNull.Value;
            else
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

        public void EPositionSalHistoryUpdate(int nID, string nratetype, string nrate, string ndayperiod, string nhoursperiod, string neffectivedate, decimal nannsalary)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositionSalaryHistory " +
                "SET RateTypeCode = @RateTypeCode,Rate=@Rate,DaysPerPayPeriod=@DaysPerPayPeriod,HoursPerPayPeriod=@HoursPerPayPeriod,effectiveDate=@effectiveDate,AnnualSalary=@AnnualSalary WHERE ID = @ID";


            cmdUpdate.Parameters.Add(new SqlParameter("@RateTypeCode", SqlDbType.Char));
            cmdUpdate.Parameters.Add(new SqlParameter("@Rate", SqlDbType.Decimal));
            cmdUpdate.Parameters.Add(new SqlParameter("@DaysPerPayPeriod", SqlDbType.Decimal));
            cmdUpdate.Parameters.Add(new SqlParameter("@HoursPerPayPeriod", SqlDbType.Decimal));
            cmdUpdate.Parameters.Add(new SqlParameter("@effectiveDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@AnnualSalary", SqlDbType.Decimal));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@RateTypeCode"].Value = nratetype;
            cmdUpdate.Parameters["@Rate"].Value = Convert.ToDecimal(nrate);
            cmdUpdate.Parameters["@DaysPerPayPeriod"].Value = ndayperiod;
            cmdUpdate.Parameters["@HoursPerPayPeriod"].Value = nhoursperiod;
            cmdUpdate.Parameters["@effectiveDate"].Value = neffectivedate;
            cmdUpdate.Parameters["@AnnualSalary"].Value = nannsalary;
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

        public int UpdateForAdpWebSevicesImport(int ePositionId, bool bIsPrimaryPosition,
            DateTime dateStart, DateTime dateEnd, string strEnteredBy, DateTime dateEntered, DateTime importedDate,string FileNumber,string NewPositionId)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET PrimaryPosition = @PrimaryPosition, " +
                "startDate = @StartDate, " +
                "actualEndDate = @actualEndDate, " +
                "EnteredBy = @EnteredBy, " +
                "EnteredDate =  @EnteredDate, " +
                "ImportedDate = @ImportedDate, " +
                     "fileNumber = @fileNumber, " +
                        "NewPositionId = @NewPositionId " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdUpdate.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@EnteredDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@ImportedDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@fileNumber", SqlDbType.VarChar,10);
            cmdUpdate.Parameters.Add("@NewPositionId", SqlDbType.VarChar,10);
            cmdUpdate.Parameters.Add("@ID", SqlDbType.Int);

            cmdUpdate.Parameters["@ID"].Value = ePositionId;
            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdUpdate.Parameters["@StartDate"].Value = dateStart;
            cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;
            cmdUpdate.Parameters["@EnteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@EnteredDate"].Value = dateEntered;
            cmdUpdate.Parameters["@ImportedDate"].Value = importedDate;
            cmdUpdate.Parameters["@fileNumber"].Value = FileNumber;
            cmdUpdate.Parameters["@NewPositionId"].Value = NewPositionId;

            if (dateEnd == DateTime.MaxValue)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;

            cmdUpdate.Parameters["@StartDate"].Value = dateStart;


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

        public int UpdateForAdpNational(int ePositionId, bool bIsPrimaryPosition,
            DateTime dateStart, DateTime dateEnd, string strEnteredBy, DateTime dateEntered, DateTime importedDate
            ,string strRateTypeCode, decimal? dRate, string strPayFrequencyCode, string strImportedRateTypeCode, decimal? dImportedRate)
        {
            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET PrimaryPosition = @PrimaryPosition, " +
                "startDate = @StartDate, " +
                "actualEndDate = @actualEndDate, " +
                "EnteredBy = @EnteredBy, " +
                "EnteredDate =  @EnteredDate, " +
                "ImportedDate = @ImportedDate, " +
                "PayRate = @PayRate, "+ 
                "RateTypeCode =@RateTypeCode, "+
                "PayFrequencyCode = @PayFrequencyCode, "+
                "ImportedPayRate = @ImportedPayRate, " +
                "ImportedRateTypeCode =@ImportedRateTypeCode " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdUpdate.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@EnteredDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@ImportedDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@PayRate", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@RateTypeCode", SqlDbType.VarChar, 1);
            cmdUpdate.Parameters.Add("@PayFrequencyCode", SqlDbType.VarChar);
            cmdUpdate.Parameters.Add("@ImportedPayRate", SqlDbType.Decimal, 9);
            cmdUpdate.Parameters.Add("@ImportedRateTypeCode", SqlDbType.VarChar, 1);
            cmdUpdate.Parameters.Add("@ID", SqlDbType.Int);

            cmdUpdate.Parameters["@ID"].Value = ePositionId;
            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdUpdate.Parameters["@StartDate"].Value = dateStart;
            cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;
            cmdUpdate.Parameters["@EnteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@EnteredDate"].Value = dateEntered;
            cmdUpdate.Parameters["@ImportedDate"].Value = importedDate;

            if (dateEnd == DateTime.MaxValue)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;

            cmdUpdate.Parameters["@StartDate"].Value = dateStart;

            if (dRate == null)
                cmdUpdate.Parameters["@PayRate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@PayRate"].Value = dRate;

            if (strRateTypeCode == null)
                cmdUpdate.Parameters["@RateTypeCode"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@RateTypeCode"].Value = strRateTypeCode;

            if (strPayFrequencyCode == null)
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFrequencyCode;

            if (dImportedRate == null)
                cmdUpdate.Parameters["@ImportedPayRate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ImportedPayRate"].Value = dImportedRate;

            if (strImportedRateTypeCode == null)
                cmdUpdate.Parameters["@ImportedRateTypeCode"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ImportedRateTypeCode"].Value = strImportedRateTypeCode;

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


        public int UpdateForAdpWebSevicesImportSpringfield(int ePositionId, bool bIsPrimaryPosition,
                DateTime dateStart, DateTime dateEnd, string strEnteredBy, DateTime dateEntered, string strCompanyCode, string strFileNumber)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET PrimaryPosition = @PrimaryPosition, " +
                "startDate = @StartDate, " +
                "actualEndDate = @actualEndDate, " +
                "EnteredBy = @EnteredBy, " +
                "EnteredDate =  @EnteredDate, " +
                "CompanyCode = @CompanyCode, " +
                "FileNumber = @FileNumber " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdUpdate.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@ActualEndDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@EnteredBy", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@EnteredDate", SqlDbType.DateTime);
            cmdUpdate.Parameters.Add("@CompanyCode", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@FileNumber", SqlDbType.VarChar, 50);
            cmdUpdate.Parameters.Add("@ID", SqlDbType.Int);

            cmdUpdate.Parameters["@ID"].Value = ePositionId;
            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdUpdate.Parameters["@StartDate"].Value = dateStart;
            cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;
            cmdUpdate.Parameters["@EnteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@EnteredDate"].Value = dateEntered;

            if (dateEnd == DateTime.MaxValue)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;

            if (string.IsNullOrEmpty(strCompanyCode))
                cmdUpdate.Parameters["@CompanyCode"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@CompanyCode"].Value = strCompanyCode;

            if (string.IsNullOrEmpty(strFileNumber))
                cmdUpdate.Parameters["@FileNumber"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@FileNumber"].Value = strFileNumber;

            cmdUpdate.Parameters["@StartDate"].Value = dateStart;


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

        public void Update(int nID, int nEmployeeID, int nPositionID)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EmployeeID = @EmployeeID," +
                "PositionID = @PositionID " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
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

        public void UpdatePhoenix(int nID, int nEmployeeID, int nPositionID, DateTime startDate, DateTime? endDate, int? nAdpPositionNumber)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EmployeeID = @EmployeeID," +
                "PositionID = @PositionID, " +
                "StartDate=@StartDate, "+
                "ActualEndDate=@ActualEndDate, "+
                "AdpPositionNumber = @AdpPositionNumber "+
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ActualEndDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@AdpPositionNumber", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@StartDate"].Value = startDate;

            if (endDate == null)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = endDate;

            if (nAdpPositionNumber == null)
                cmdUpdate.Parameters["@AdpPositionNumber"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@AdpPositionNumber"].Value = nAdpPositionNumber;

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

        public void UpdateAdpNational(int nID, int nEmployeeID, int nPositionID, DateTime startDate, DateTime? endDate
            ,string strRateTypeCode, Decimal? dRate, string strPayFrequencyCode)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EmployeeID = @EmployeeID," +
                "PositionID = @PositionID, " +
                "StartDate=@StartDate, " +
                "ActualEndDate=@ActualEndDate, " +
                "RateTypeCode=@RateTypeCode," +
                "PayRate=@PayRate, "+
                "PayFrequencyCode=@PayFrequencyCode "+
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ActualEndDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@RateTypeCode", SqlDbType.VarChar,1));
            cmdUpdate.Parameters.Add(new SqlParameter("@PayRate", SqlDbType.Decimal,9));
            cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@StartDate"].Value = startDate;

            if (endDate == null)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = endDate;

            if (dRate == null)
                cmdUpdate.Parameters["@PayRate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@PayRate"].Value = dRate;

            cmdUpdate.Parameters["@RateTypeCode"].Value = strRateTypeCode;
            cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFrequencyCode;

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

        public void UpdateHudson(int nID, int nEmployeeID, int nPositionID, DateTime startDate, DateTime? endDate
            , int? nBudgetCode)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EmployeeID = @EmployeeID," +
                "PositionID = @PositionID, " +
                "StartDate=@StartDate, " +
                "ActualEndDate=@ActualEndDate, " +
                //"RateTypeCode=@RateTypeCode," +
                //"PayRate=@PayRate, " +
                //"PayFrequencyCode=@PayFrequencyCode " +
                "BudgetCode = @BudgetCode "+
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ActualEndDate", SqlDbType.DateTime));
            //cmdUpdate.Parameters.Add(new SqlParameter("@RateTypeCode", SqlDbType.VarChar, 1));
            //cmdUpdate.Parameters.Add(new SqlParameter("@PayRate", SqlDbType.Decimal, 9));
            //cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@BudgetCode", SqlDbType.Int));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@StartDate"].Value = startDate;

            if (endDate == null)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = endDate;

            //if (dRate == null)
            //    cmdUpdate.Parameters["@PayRate"].Value = System.DBNull.Value;
            //else
            //    cmdUpdate.Parameters["@PayRate"].Value = dRate;

            //cmdUpdate.Parameters["@RateTypeCode"].Value = strRateTypeCode;
            //cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFrequencyCode;

            if (nBudgetCode == null)
                cmdUpdate.Parameters["@BudgetCode"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@BudgetCode"].Value = nBudgetCode;
            

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

        public void UpdateAdpPositionNumberPhoenix(int nID,  int? nAdpPositionNumber)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET AdpPositionNumber = @AdpPositionNumber " +
                "WHERE ID = @ID";

            
            cmdUpdate.Parameters.Add(new SqlParameter("@AdpPositionNumber", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));


            if (nAdpPositionNumber == null)
                cmdUpdate.Parameters["@AdpPositionNumber"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@AdpPositionNumber"].Value = nAdpPositionNumber;

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

        public void Update(int nID, int nSalaryGradeID)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET PositionGradeID = @PositionGradeID " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@PositionGradeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            if (nSalaryGradeID == -1)
                cmdUpdate.Parameters["@PositionGradeID"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@PositionGradeID"].Value = nSalaryGradeID;

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

        public void UpdateTermedIsPrimary(int nID, DateTime dateTermed, bool bIsPrimary)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET PrimaryPosition = @PrimaryPosition, " +
                "ActualEndDate = @ActualEndDate " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@PrimaryPosition", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@ActualEndDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimary;
            cmdUpdate.Parameters["@ActualEndDate"].Value = dateTermed;

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

        public void UpdateIsPrimary(int nID, bool bIsPrimary)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET PrimaryPosition = @PrimaryPosition " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@PrimaryPosition", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimary;

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

        public void UpdateImportedFromFile(int nID, bool bImportedFromFile, DateTime importDate)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET ImportedFromFile = @ImportedFromFile, " +
                "   ImportedDate = @ImportedDate "+
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@ImportedFromFile", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@ImportedDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@ImportedFromFile"].Value = bImportedFromFile;
            cmdUpdate.Parameters["@ImportedDate"].Value = importDate;


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

        public void UpdateEndDate(int nID, DateTime dateEnd)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET actualEndDate = @ActualEndDate " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@ActualEndDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            if (dateEnd == DateTime.MaxValue)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;

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

        public void UpdateEnteredBy(int nId, string strEnteredBy, DateTime dateEntered)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EnteredBy = @EnteredBy, " +
                "   EnteredDate = @EnteredDate "+
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@EnteredBy", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@EnteredDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EnteredBy"].Value = strEnteredBy;
            if (dateEntered == DateTime.MaxValue)
                cmdUpdate.Parameters["@EnteredDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@EnteredDate"].Value = dateEntered;

            cmdUpdate.Parameters["@ID"].Value = nId;

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

        public void UpdateEndDateForAllWithoutEndDate(int nEmployeeId, int nPositionId, DateTime dateEnd)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET actualEndDate = @ActualEndDate " +
                "WHERE employeeID = @EmployeeID AND PositionId = @PositionID AND ActualEndDate is NULL";

            cmdUpdate.Parameters.Add(new SqlParameter("@ActualEndDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));

            if (dateEnd == DateTime.MaxValue)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeId;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionId;

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

        public void UpdateEndDateForAllWithoutEndDateExceptThisEmployeeId(int nEmployeeId, int nPositionId, DateTime dateEnd)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET actualEndDate = @ActualEndDate " +
                "WHERE employeeID <> @EmployeeID AND PositionId = @PositionID AND ActualEndDate is NULL";

            cmdUpdate.Parameters.Add(new SqlParameter("@ActualEndDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));

            if (dateEnd == DateTime.MaxValue)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeId;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionId;

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

        public void UpdateFTE(int nID, decimal dFTE)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET FTE = @FTE " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@FTE", SqlDbType.Decimal, 9));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@FTE"].Value = dFTE;

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

        public void UpdatePayFrequencyCode(int nID, string strPayFrequecyCode)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET PayFrequencyCode = @PayFrequencyCode " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFrequecyCode;

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

        public void UpdateCompanyCode(int nID, string strCompanyCode)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET CompanyCode = @CompanyCode " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@CompanyCode"].Value = strCompanyCode;

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

        public void UpdateMonetaryCountryCode(int nID, string strMonetaryCountryCode)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET MonetaryCountryCode = @MonetaryCountryCode " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@MonetaryCountryCode", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@MonetaryCountryCode"].Value = strMonetaryCountryCode;

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


        public void UpdateRateTypeCode(int nID, string strRateTypeCode)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET RateTypeCode = @RateTypeCode " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@RateTypeCode", SqlDbType.VarChar, 1));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@RateTypeCode"].Value = strRateTypeCode;

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


        public void UpdateAllPrimary(int nEmployeeID, bool bIsPrimary)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET PrimaryPosition = @PrimaryPosition " +
                "WHERE EmployeeID = @EmployeeID";

            cmdUpdate.Parameters.Add(new SqlParameter("@PrimaryPosition", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimary;

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;

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

        public void UpdateActualEndDateWhereEnteredDateNotEqualToImportDateAndActualEndDateIsNull(DateTime importDate)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions SET actualEndDate = EnteredDate " +
                "WHERE ePositions.ID In " +
                "   (  " +
                "       SELECT e2.Id  " +
                "       FROM ePositions e2 " +
                "       WHERE (importedDate Is null AND EnteredDate is not null) " +
                "           OR (Cast(Convert(Varchar(30), enteredDate, 101) as DateTime) > Cast(Convert(Varchar(30), ImportedDate, 101) as DateTime))  " +
                "   ) AND ePositions.ActualEndDate IS NULL ";


            cmdUpdate.Parameters.Add(new SqlParameter("@ImportDate", SqlDbType.DateTime));

            cmdUpdate.Parameters["@ImportDate"].Value = importDate;

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


        public void UpdateForHRP(int nID, int nEmployeeID, int nPositionID, string strPayFreqCode,
                                    bool bIsPrimaryPosition, string strRateTypeCode)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EmployeeID = @EmployeeID," +
                "PositionID = @PositionID, " +
                "PayFrequencyCode = @PayFrequencyCode, " +
                "PrimaryPosition = @PrimaryPosition, " +
                "RateTypeCode = @RateTypeCode " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@PrimaryPosition", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@RateTypeCode", SqlDbType.VarChar, 1));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdUpdate.Parameters["@RateTypeCode"].Value = strRateTypeCode;

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

        public void UpdateForHRB(int nID, int nEmployeeID, int nPositionID, bool bIsPrimaryPosition,
                                    string strPayFreqCode, DateTime dateStart)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EmployeeID = @EmployeeID," +
                "PositionID = @PositionID, " +
                "PrimaryPosition = @PrimaryPosition, " +
                "PayFrequencyCode = @PayFrequencyCode, " +
                "StartDate = @StartDate " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PrimaryPosition", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;

            if (strPayFreqCode.ToUpper() == "B" || strPayFreqCode.ToUpper() == "W" || strPayFreqCode.ToUpper() == "M" || strPayFreqCode.ToUpper() == "S")
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            else
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;

            cmdUpdate.Parameters["@StartDate"].Value = dateStart;

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

        public void UpdateForHudsonWebServicesPrimaryEndDate(int nID, bool bIsPrimaryPosition,
                                    DateTime actualEndDate)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET "+
                "PrimaryPosition = @PrimaryPosition, " +
                "ActualEndDate = @EndDate " +
                "WHERE ID = @ID";

            
            cmdUpdate.Parameters.Add(new SqlParameter("@PrimaryPosition", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            
            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;
            cmdUpdate.Parameters["@EndDate"].Value = actualEndDate;

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

        


        public void UpdateForHRBWithStartDateCardinalHill(int nID, int nEmployeeID, int nPositionID, bool bIsPrimaryPosition,
                                    string strPayFreqCode, DateTime dateStart)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EmployeeID = @EmployeeID," +
                "PositionID = @PositionID, " +
                "PrimaryPosition = @PrimaryPosition, " +
                "PayFrequencyCode = @PayFrequencyCode, " +
                "StartDate = @StartDate " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PrimaryPosition", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;

            if (strPayFreqCode.ToUpper() == "B" || strPayFreqCode.ToUpper() == "W" || strPayFreqCode.ToUpper() == "M" || strPayFreqCode.ToUpper() == "S")
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            else
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;

            if (dateStart == DateTime.MaxValue)
                cmdUpdate.Parameters["@StartDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@StartDate"].Value = dateStart;

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

        public void EPositionUpdate(int nID, string nstartDate, string nprojectedEndDate, string nactualEndDate, string nposgrade, string npayfrequency, string nheadcount, string nFTE, string ndeterminedStatus, string nPoscategory, string nPostype, string nspaceNumber)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET startDate = @startDate,projectedEndDate=@projectedEndDate,actualEndDate=@actualEndDate,positionCategoryID=@positionCategoryID,positionTypeID=@positionTypeID,positionGradeID=@positionGradeID,determinedStatus=@determinedStatus,headcount=@headcount,FTE=@FTE,payFrequencyCode=@payFrequencyCode,spaceNumber=@spaceNumber WHERE ID = @ID";


            cmdUpdate.Parameters.Add(new SqlParameter("@startDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@projectedEndDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@actualEndDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@positionCategoryID", SqlDbType.SmallInt));
            cmdUpdate.Parameters.Add(new SqlParameter("@positionTypeID", SqlDbType.SmallInt));
            cmdUpdate.Parameters.Add(new SqlParameter("@positionGradeID", SqlDbType.SmallInt));
            cmdUpdate.Parameters.Add(new SqlParameter("@determinedStatus", SqlDbType.VarChar));
            cmdUpdate.Parameters.Add(new SqlParameter("@headcount", SqlDbType.Decimal));
            cmdUpdate.Parameters.Add(new SqlParameter("@FTE", SqlDbType.Decimal));
            cmdUpdate.Parameters.Add(new SqlParameter("@payFrequencyCode", SqlDbType.VarChar));
            cmdUpdate.Parameters.Add(new SqlParameter("@spaceNumber", SqlDbType.VarChar));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@startDate"].Value = Convert.ToDateTime(nstartDate);
            cmdUpdate.Parameters["@projectedEndDate"].Value = Convert.ToDateTime(nprojectedEndDate);
            cmdUpdate.Parameters["@actualEndDate"].Value = Convert.ToDateTime(nactualEndDate);
            cmdUpdate.Parameters["@positionCategoryID"].Value = Convert.ToInt32(nPoscategory);
            cmdUpdate.Parameters["@positionTypeID"].Value = Convert.ToInt32(nPostype);
            cmdUpdate.Parameters["@positionGradeID"].Value = Convert.ToInt32(nposgrade);
            cmdUpdate.Parameters["@determinedStatus"].Value = ndeterminedStatus;
            cmdUpdate.Parameters["@headcount"].Value = Convert.ToDecimal(nheadcount);
            cmdUpdate.Parameters["@FTE"].Value = Convert.ToDecimal(nFTE);
            cmdUpdate.Parameters["@payFrequencyCode"].Value = npayfrequency;
            cmdUpdate.Parameters["@spaceNumber"].Value = nspaceNumber;
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

        public void UpdateForHRBWithEndDateCardinalHill(int nID, int nEmployeeID, int nPositionID, bool bIsPrimaryPosition,
                                    string strPayFreqCode, DateTime dateEnd)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EmployeeID = @EmployeeID," +
                "PositionID = @PositionID, " +
                "PrimaryPosition = @PrimaryPosition, " +
                "PayFrequencyCode = @PayFrequencyCode, " +
                "ActualEndDate = @ActualEndDate " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PrimaryPosition", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@ActualEndDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;

            if (strPayFreqCode.ToUpper() == "B" || strPayFreqCode.ToUpper() == "W" || strPayFreqCode.ToUpper() == "M" || strPayFreqCode.ToUpper() == "S")
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            else
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;

            if (dateEnd == DateTime.MaxValue)
                cmdUpdate.Parameters["@ActualEndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualEndDate"].Value = dateEnd;

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

        public void UpdateProjectedOtHoursAndIncreaseForWea(int nID, Decimal dProjectedOtHours, decimal dProjectedIncreasePercent)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET  ProjectedOtHours = @ProjectedOtHours," +
                "ProjectedIncreasePercent = @ProjectedIncreasePercent " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@ProjectedOtHours", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@ProjectedIncreasePercent", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@ProjectedOtHours"].Value = dProjectedOtHours;
            cmdUpdate.Parameters["@ProjectedIncreasePercent"].Value = dProjectedIncreasePercent;
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

        public int UpdateYmca(int nId, int nEmployeeID, int nPositionID, int nDepartmentId, int nLocationId, int nAccountId
            , int nPcsCodeId, int nSupervisorId, DateTime? startDate, DateTime? endDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "Update ePositions " +
                "SET "+ 
                
                "   EmployeeID=@EmployeeId, "+ 
                "   PositionID=@PositionId, "+
                "   DepartmentId=@DepartmentId, "+
                "   LocationId=@LocationId, "+
                "   AccountId=@AccountId, "+
                "   PcsCodeId=@PcsCodeId, "+
                "   SupervisorId=@SupervisorId, "+
                "   StartDate=@StartDate, "+
                "   ActualEndDate=@EndDate " +
                "WHERE ID =@ID";

            cmdUpdate.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@DepartmentID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@LocationID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@AccountID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@PcsCodeID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@SupervisorID", SqlDbType.Int, 4);
            cmdUpdate.Parameters.Add("@StartDate", SqlDbType.Date);
            cmdUpdate.Parameters.Add("@EndDate", SqlDbType.Date);

            cmdUpdate.Parameters.Add("@ID", SqlDbType.Int,4);

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@DepartmentID"].Value = nDepartmentId;
            cmdUpdate.Parameters["@LocationID"].Value = nLocationId;
            cmdUpdate.Parameters["@AccountID"].Value = nAccountId;

            if (nPcsCodeId == 0)
                cmdUpdate.Parameters["@PcsCodeID"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@PcsCodeID"].Value = nPcsCodeId;

            cmdUpdate.Parameters["@SupervisorID"].Value = nSupervisorId;
            cmdUpdate.Parameters["@StartDate"].Value = startDate;

            if (endDate == null)
                cmdUpdate.Parameters["@EndDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@EndDate"].Value = endDate;

            cmdUpdate.Parameters["@ID"].Value = nId;

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

        public void UpdateEJobCodeAndDescription(int nID, string strEJobCode, string strEJobDescription)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET eJobCode = @eJobCode " +
                "   ,eJobDescription = @eJobDescription " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@eJobCode", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@eJobDescription", SqlDbType.VarChar, 100));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@eJobCode"].Value = strEJobCode;
            cmdUpdate.Parameters["@eJobDescription"].Value = strEJobDescription;

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

        public void UpdateUnionCodeAndDescription(int nID, string strUnionCode, string strUnionDescription)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET UnionCode = @UnionCode " +
                "   ,UnionDescription = @UnionDescription " +
                "WHERE ID = @ID";

            cmdUpdate.Parameters.Add(new SqlParameter("@UnionCode", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@UnionDescription", SqlDbType.VarChar, 100));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@UnionCode"].Value = strUnionCode;
            cmdUpdate.Parameters["@UnionDescription"].Value = strUnionDescription;

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
            cmdDelete.CommandText = "DELETE FROM ePositions " +
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

        public void Delete_AndAllRelatedTableRecs(int nID)
        {
            SqlTransaction sqlTransaction = null;
            try
            {
                m_Connection.ConnectionString = m_Connection.ConnectionString + ";";

                m_Connection.Open();
                sqlTransaction = m_Connection.BeginTransaction();

                SqlCommand cmdDelete = m_Connection.CreateCommand();
                cmdDelete.Transaction = sqlTransaction;


                cmdDelete.CommandType = CommandType.Text;
                cmdDelete.CommandText = "DELETE FROM empAppraisalSections " +
                    "WHERE empAppraisalID IN (Select empAppraisalID From employeeAppraisals WHERE ePositionID = @ePositionID) ";
                cmdDelete.Parameters.Add(new SqlParameter("@ePositionID", SqlDbType.Int, 4));
                cmdDelete.Parameters["@ePositionID"].Value = nID;
                cmdDelete.ExecuteNonQuery();


                cmdDelete.CommandText = "DELETE FROM employeeAppraisals " +
                    "WHERE ePositionID = @ePositionID ";
                cmdDelete.Parameters.Clear();
                cmdDelete.Parameters.Add(new SqlParameter("@ePositionID", SqlDbType.Int, 4));
                cmdDelete.Parameters["@ePositionID"].Value = nID;
                cmdDelete.ExecuteNonQuery();


                cmdDelete.CommandText = "DELETE  FROM ePositionSalaryHistory " +
                                        "WHERE ePositionID = @ePositionID ";
                cmdDelete.Parameters.Clear();
                cmdDelete.Parameters.Add(new SqlParameter("@ePositionID", SqlDbType.Int, 4));
                cmdDelete.Parameters["@ePositionID"].Value = nID;
                cmdDelete.ExecuteNonQuery();

                cmdDelete.CommandText = "DELETE FROM ePositions " +
                    "WHERE ID = @ID ";
                cmdDelete.Parameters.Clear();
                cmdDelete.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
                cmdDelete.Parameters["@ID"].Value = nID;
                cmdDelete.ExecuteNonQuery();


                sqlTransaction.Commit();


            }
            catch
            {
                sqlTransaction.Rollback();
                throw;
            }

            finally
            {
                m_Connection.Close();
            }
        }
        public DataTable GetAllEposition(int nempid)
        {
            //SqlCommand cmdSelect = new SqlCommand("SELECT * FROM ePositions", m_Connection);

            SqlCommand cmdSelect = new SqlCommand("select epositions.id as eposid,epositions.payFrequencyCode as payfrequency,Positions.id as posid,Positions.title as postitle,hrBUSINESSLEVELS.id as hrbusid,hrBUSINESSLEVELS.title as businesslevel,jobs.id as jobid,jobs.title as jobtitle,ddlPayFrequencies.id as payfrequencyid,ddlPayFrequencies.description as payfrequency,ddlPositionCategories.id as poscateid,ddlPositionCategories.description as poscategory,ddlPositionTypes.id as postypeid,ddlPositionTypes.description as postype,epositions.* from epositions " +
            "LEFT JOIN Positions on epositions.positionID=Positions.id LEFT JOIN hrBUSINESSLEVELS on Positions.businessLevelID=hrBUSINESSLEVELS.id " +
            "LEFT JOIN ddlPayFrequencies on ePositions.payFrequencyCode=ddlPayFrequencies.code " +
            "LEFT JOIN ddlPositionCategories on epositions.positionCategoryID=ddlPositionCategories.id " +
            "LEFT JOIN ddlPositionTypes on epositions.positionTypeID=ddlPositionTypes.id " + 
            "LEFT JOIN jobs on Positions.jobID=jobs.id where epositions.employeeID='" + nempid + "'", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllSalHistory(int neposid)
        {
            SqlCommand cmdSelect = new SqlCommand("select * from ePositionSalaryHistory where ePositionID='" + neposid + "'", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositionSalaryHistory");

            return ds.Tables["ePositionSalaryHistory"];
        }

        public DataTable GetAll()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM ePositions", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllWithPositionCode(int employeeId)
        {
            SqlCommand cmdSelect = new SqlCommand(" "+
                "SELECT ePositions.*, Positions.Code as PositionCode "+
                "FROM ePositions "+
                "   Join Positions on Positions.Id = ePositions.PositionId "+
                "WHERE epositions.employeeId = @EmployeeId "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int, 4));

            cmdSelect.Parameters["@EmployeeId"].Value = employeeId;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPhoeinx()
        {
            SqlCommand cmdSelect = new SqlCommand(" "+
                "SELECT ePositions.Id as ePostionId, ePositions.EmployeeId, ePositions.PositionId, ePositions.EndDate, AdpPositionNumber "+
                "FROM ePositions "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllYmca()
        {
            SqlCommand cmdSelect = new SqlCommand(" "+
                "SELECT ePositions.Id "+
	            "    ,Employees.id as EmployeeId, Persons.LastName+', '+Persons.FirstName as EmployeeName "+
	            "    ,Positions.Id as PositionId, Positions.Code as PositionCode, Positions.title as PositionTitle "+
	            "    ,ePositions.startDate, ePositions.ActualEndDate "+
	            "    ,LocationsYmca.id as locationId, LocationsYmca.branchNumber+'-'+LocationsYmca.Description as Location "+
	            "    ,DepartmentsYmca.id as DepartmentId, DepartmentsYmca.DepartmentNumber+'-'+LocationsYmca.Description as Department "+
	            "    ,AccountNumbersYmca.id as AccountId, AccountNumbersYmca.AccountNumber+'-'+AccountNumbersYmca.Description as Account "+
	            "    ,PcsCodesYmca.id as PcsCodeId, PcsCodesYmca.code+'-'+PcsCodesYmca.Description as PcsCode "+
	            "    ,e2.id as SupervisorId, p2.LastName+', '+P2.firstname as Supervisor "+
                "FROM ePositions "+
	            "    join Employees on employees.Id = ePositions.employeeId "+
	            "    join Persons on Persons.id = employees.personid "+
	            "    join Positions on Positions.id = ePositions.positionID "+
	            "    join DepartmentsYmca on DepartmentsYmca.id = ePositions.departmentId "+
	            "    join LocationsYmca on LocationsYmca.id = epositions.LocationId "+
	            "    join AccountNumbersYmca on AccountNumbersYmca.Id = ePositions.AccountId "+
	            "    join Employees e2 on e2.id = ePositions.SupervisorId "+
	            "    join Persons p2 on p2.id = e2.personid "+
                "    left join PcsCodesYmca on PcsCodesYmca.Id = ePositions.PcsCodeId " +
                "order by Persons.lastname, Persons.firstname", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPhoenix()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT ePositions.Id " +
                "    ,Employees.id as EmployeeId, Persons.LastName+', '+Persons.FirstName as EmployeeName " +
                "    ,Positions.Id as PositionId, Positions.Code as PositionCode, Positions.title as PositionTitle " +
                "    ,ePositions.startDate, ePositions.ActualEndDate, ePositions.AdpPositionNumber " +
                "FROM ePositions " +
                "    join Employees on employees.Id = ePositions.employeeId " +
                "    join Persons on Persons.id = employees.personid " +
                "    join Positions on Positions.id = ePositions.positionID " +
                "order by Persons.lastname, Persons.firstname", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPhoenixForEpositionAssigmentGridFilterByCoCodeAssignments(int nPersonId)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT ePositions.Id  " +
                "    ,Employees.id as EmployeeId, Persons.LastName+', '+Persons.FirstName as EmployeeName " + 
                "    ,Positions.Id as PositionId, Positions.Code as PositionCode, Positions.title as PositionTitle " + 
                "    ,ePositions.startDate, ePositions.ActualEndDate, ePositions.AdpPositionNumber " +
                "    ,positions.psCompany, employees.CompanyCode, Employees.FileNumber " + 
                "FROM ePositions  " +
                "    join Employees on employees.Id = ePositions.employeeId " + 
                "    join Persons on Persons.id = employees.personid  " +
                "    join Positions on Positions.id = ePositions.positionID " +
                "WHERE Positions.psCompany in (select companyCode from PersonCompanyCodeAssignments pcca where PersonId = "+nPersonId+")  " +
	            "    AND employees.CompanyCode in (select companyCode from PersonCompanyCodeAssignments pcca where PersonId = "+nPersonId+") " +
                "order by Persons.lastname, Persons.firstname", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllForAdpNational()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT ePositions.Id  " +
                "    ,Employees.id as EmployeeId, Persons.LastName+', '+Persons.FirstName as EmployeeName " +
                "    ,Positions.Id as PositionId, Positions.Code as PositionCode, Positions.title as PositionTitle " +
                "    ,ePositions.startDate, ePositions.ActualEndDate, ePositions.PayRate, ePositions.RateTypeCode, ePositions.PayFrequencyCode " +
                "    ,employees.CompanyCode, Employees.FileNumber, es.Description as employeeStatus " +
                "FROM ePositions  " +
                "    join Employees on employees.Id = ePositions.employeeId " +
                "    join Persons on Persons.id = employees.personid  " +
                "    join Positions on Positions.id = ePositions.positionID " +
                "   left join ddlEmploymentStatuses es on es.id = employees.employmentStatusId "+
                "order by Persons.lastname, Persons.firstname", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllForHudson()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT ePositions.Id  " +
                "    ,Employees.id as EmployeeId, Persons.LastName+', '+Persons.FirstName as EmployeeName " +
                "    ,Positions.Id as PositionId, Positions.Code as PositionCode, Positions.title as PositionTitle " +
                "    ,ePositions.startDate, ePositions.ActualEndDate, ePositions.PayRate, ePositions.RateTypeCode, ePositions.PayFrequencyCode " +
                "    ,employees.CompanyCode, Employees.FileNumber, es.Description as employeeStatus " +
                "   ,ePositions.BudgetCode "+
                "FROM ePositions  " +
                "    join Employees on employees.Id = ePositions.employeeId " +
                "    join Persons on Persons.id = employees.personid  " +
                "    join Positions on Positions.id = ePositions.positionID " +
                "   left join ddlEmploymentStatuses es on es.id = employees.employmentStatusId " +
                "WHERE Positions.Status = 1 "+
                "order by Persons.lastname, Persons.firstname", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPhoeinxMinimalFieldsNeededToAdjustAdpPositionNumbers()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT ePositions.Id as ePositionId, ePositions.EmployeeId, ePositions.PositionId, ePositions.ActualEndDate, AdpPositionNumber " +
                "FROM ePositions "+
                "   join Employees on employees.id = ePositions.employeeID "+
	            "   join ddlEmploymentStatuses es on es.id = employees.employmentStatusID "+
                "where es.code = 'a' and AdpPositionNumber is not null "+
                "ORDER BY employeeid, AdpPositionNumber "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPhoeinxUniqueEmployeeIds()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Distinct Employees.id as EmployeeId " +
                "FROM ePositions " +
                "   join Employees on employees.id = ePositions.employeeID " +
                "   join ddlEmploymentStatuses es on es.id = employees.employmentStatusID " +
                "where es.code = 'a' "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPhoeinxMinimalFieldsNeededToAdjustAdpPositionNumbers(int nEmployeeId)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT ePositions.Id as ePostionId, ePositions.EmployeeId, ePositions.PositionId, ePositions.EndDate, AdpPositionNumber " +
                "FROM ePositions "+
                "   join Employees on employees.id = ePositions.employeeID " +
                "   join ddlEmploymentStatuses es on es.id = employees.employmentStatusID " +
                "WHERE employeeId = @EmployeeId  and es.code = 'a' "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int, 4));

            cmdSelect.Parameters["@EmployeeId"].Value = nEmployeeId;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAll(int nPositionID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Persons.preferredName, Persons.LastName+', '+Persons.FirstName As LastFirst " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "WHERE ePositions.PositionID = " + nPositionID + " AND ePositions.StartDate is NOT NULL AND ePositions.actualEndDate is Null " +
                "ORDER BY LastFirst "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }


        public DataTable GetFillPosition()
        {

            SqlCommand cmdSelect = new SqlCommand(" " +

            "Select POSITIONS.*, hrBUSINESSLEVELS.title as businessLevelTitle, hrBusinessLevels.Code as BUCode, jobs.title as jobTitle, jobs.Code as JobCode from POSITIONS, hrBUSINESSLEVELS, jobs where POSITIONS.businessLevelID = hrBUSINESSLEVELS.id and POSITIONS.jobID = jobs.id order by hrBusinessLevels.title, Positions.title", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "POSITIONS");

            return ds.Tables["POSITIONS"];
        }


        public DataTable GetAllActiveNoEndDate(int nPositionID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Persons.preferredName, Persons.LastName+', '+Persons.FirstName As LastFirst " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "   INNER JOIN ddlEmploymentStatuses on ddlEmploymentStatuses.id = Employees.employmentStatusId " +
                "WHERE ePositions.PositionID = " + nPositionID + " " +
                "   AND ePositions.StartDate is NOT NULL AND ePositions.actualEndDate is Null AND ddlEmploymentStatuses.code = 'a' " +
                "ORDER BY LastFirst "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAll(int nPositionID, int EmployeeID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Persons.preferredName, Persons.LastName+', '+Persons.FirstName As LastFirst " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "WHERE ePositions.PositionID = " + nPositionID + " AND ePositions.EmployeeID= " + EmployeeID + "  AND ePositions.StartDate is NOT NULL AND ePositions.actualEndDate is Null " +
                "ORDER BY LastFirst "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }
        public DataTable GetAllDistinctCompanyCodes(string strPayFrequencyCode)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "select DISTINCT CompanyCode " +
                "from ePositions " +
                "WHERE CompanyCode IS NOT NULL AND ePositions.PayFrequencyCode = @PayFrequencyCode AND epositions.RateTypeCode = 'S' "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));

            cmdSelect.Parameters["@PayFrequencyCode"].Value = strPayFrequencyCode;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllDistinctCompanyCodes()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "select DISTINCT CompanyCode " +
                "from ePositions " +
                "WHERE CompanyCode IS NOT NULL " +
                "ORDER BY CompanyCode "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllDistinctPayFrequencies()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "select DISTINCT PayFrequencyCode " +
                "    , CASE PayFrequencyCOde " +
                "        WHEN 'W' THEN 'Weekly' " +
                "        WHEN 'B' THEN 'Bi-Weekly' " +
                "        WHEN 'S' THEN 'Semi-Monthly' " +
                "        WHEN 'M' THEN 'Monthly' " +
                "        ELSE 'Unknow Pay Frequency Code' " +
                "    END as PayFrequencyDescription " +
                "from ePositions " +
                "WHERE PayFrequencyCode IS NOT NULL "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEpositionsForPositionID(int nPositionID, bool bIsPrimary)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Persons.preferredName, Persons.LastName+', '+Persons.FirstName As LastFirst, Employees.FileNumber, " +
                    " Employees.EmploymentStatusID " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "WHERE ePositions.PositionID = @PositionID  AND ePositions.primaryPosition = @IsPrimary " +
                "ORDER BY LastFirst "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@IsPrimary", SqlDbType.Bit));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@IsPrimary"].Value = bIsPrimary;

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEpositionAccountsForPositionID(int nPositionID, bool bIsPrimary)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositionAccountActuals.*, Persons.preferredName, Persons.LastName+', '+Persons.FirstName As LastFirst " +
                "FROM Positions " +
                "   INNER JOIN ePositionAccountActuals " +
                "       ON ePositionAccountActuals.AccountNumber = Positions.AccountNumber " +
                "   INNER JOIN ePositions " +
                "       ON ePositions.ID = ePositionAccountActuals.ePositionID " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "WHERE Positions.ID = @PositionID " +
                "ORDER BY LastFirst "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            //cmdSelect.Parameters.Add(new SqlParameter("@IsPrimary", SqlDbType.Bit));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            //cmdSelect.Parameters["@IsPrimary"].Value = bIsPrimary;

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPrimary(int nPositionID)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT ePositions.*, Persons.preferredName, Persons.LastName+', '+Persons.FirstName As LastFirst " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "WHERE ePositions.PositionID = " + nPositionID + " AND ePositions.primaryPosition=1 " +
                "ORDER BY LastFirst "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPrimary(int nPositionID, DateTime dateEffectiveSalary, DateTime dateEffectiveHourly, int nRateType)
        {
            string strRateTypeFilter = "";
            if (nRateType == 0)
                strRateTypeFilter = " And employees.RateType = 's' ";
            else if (nRateType == 1)
                strRateTypeFilter = " AND employees.RateType = 'h' ";

            SqlCommand cmdSelect = new SqlCommand(" " +
            "SELECT	ePositions.id as EPositionID, ePositions.EmployeeID, Persons.LastName+', '+Persons.FirstName As LastFirst, " +
            "   employees.HireDate,  Positions.Title as PositionTitle,  " +
            "   ePositions.PositionGradeID,  " +
            "   subString(hrBusinessLevels.Code,LEN(hrBusinessLevels.Code)-2,3 ) as BusinessCodeLast3, " +
            " " +
            "   (   SELECT ddlSalaryGrades.Code " +
            "       FROM ddlSalaryGrades " +
            "       WHERE ePositions.PositionGradeID = ddlSalaryGrades.ID ) as ePositionGradeCode,  " +
            "   (	SELECT Top(1) Code  From ePositionSalaryHistory  " +
            "           INNER JOIN ddlSalaryGradeHistory ON " +
            "               ddlSalaryGradeHistory.id = ePositionSalaryHistory.salGradeHistoryID " +
            "           INNER JOIN ddlSalaryGrades " +
            "               ON ddlSalaryGrades.id = ddlSalaryGradeHistory.salaryGradeID " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as oldGradeCode, " +
            " " +
            " " +
            "   (	SELECT Top(1) Rate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as Rate, " +
            " " +
            "   (	SELECT Top(1) HoursPerPayPeriod From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as HoursPerPayPeriod, " +
            " " +
            "   (	SELECT Top(1) effectiveDate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as effectiveDate, " +
            " " +
            "   (	SELECT Top(1) salaryMinimum " +
            "	FROM ePositionSalaryHistory " +
            "		LEFT JOIN ddlSalaryGradeHistory " +
            "            ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))   " +
            "	ORDER BY EffectiveDate DESC) as salaryMinimum, " +
            " " +
            "   (	SELECT Top(1) salaryMidpoint  " +
            "	FROM ePositionSalaryHistory  " +
            "		LEFT JOIN ddlSalaryGradeHistory  " +
            "            ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as salaryMidpoint, " +
            " " +
            "(	SELECT Top(1) salaryMaximum  " +
            "	FROM ePositionSalaryHistory  " +
            "		LEFT JOIN ddlSalaryGradeHistory  " +
            "            ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as salaryMaximum, " +
            " " +
            "(	SELECT Top(1) compRatio From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as compRatio, " +
            " " +
            "(	SELECT Top(1) increaseAmount From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as increaseAmount, " +
            " " +
            "(	SELECT Top(1) increasePercent From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as increasePercent, " +
            " " +
            "(	SELECT Top(1) rateTypeCode From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as rateTypeCode, " +
            " " +
            "(	SELECT Top(1) rating From employeeRatings " +
            "	WHERE fileNumber = employees.fileNumber  " +
            "		AND ((rateType = 's' AND employeeRatings.year < DATEPART(year,@EffectiveDateSalary)) OR (rateType = 'h' AND employeeRatings.year < DATEPART(year,@EffectiveDateHourly))) " +
            "	ORDER BY Year DESC) as PreviousRating, " +
            " " +
            "(	SELECT Top(1) rating From employeeRatings  " +
            "	WHERE fileNumber = employees.fileNumber  " +
            "		AND ((rateType = 's' AND employeeRatings.year = DATEPART(year,@EffectiveDateSalary)) OR (rateType = 'h' AND employeeRatings.year = DATEPART(year,@EffectiveDateHourly)))  " +
            "	ORDER BY Year DESC) as CurrentRating, " +
            " " +
            "(	SELECT IncreaseAmount From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)))  as CurrentIncAmnt, " +
            " " +
            "(	SELECT IncreasePercent From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as CurrentIncPcnt, " +
            " " +
            "(	SELECT Rate From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as CurrentRate, " +
            " " +
            "(	SELECT CompRatio From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as NewCompRatio, " +
            " " +
            "(	SELECT EXCLO From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as EXCLO, " +
            " " +
            "(	SELECT EXCHI From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as EXCHI, " +
            " " +
            "(	SELECT Reason From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as Reason, " +

            "(	SELECT updatedBy From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as UpdatedBy, " +
            " " +
            "   coalesce((	SELECT editLocked From ePositionSalaryHistory  " +
            "           	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as editLocked, " +
            " " +
            "coalesce((	SELECT increaseApproved From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as IncreaseApproved, " +
            " " +
            "SalaryMinimumNew = " +
            "   CASE " +
            "       WHEN (	SELECT salaryMinimum  " +
            "	            FROM ePositionSalaryHistory  " +
            "		            LEFT JOIN ddlSalaryGradeHistory  " +
            "                       ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
            "	            WHERE ePositionID = ePositions.id   " +
            "		            AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) " +
            "                   OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ) IS NULL " +
            "           THEN (	COALESCE((SELECT Top (1) salaryMinimum  " +
            "	                FROM ePositions ep2  " +
            "                       LEFT JOIN ddlSalaryGrades " +
            "                           ON ddlSalaryGrades.id = ePositions.PositionGradeID " +
            "		                LEFT JOIN ddlSalaryGradeHistory  " +
            "                           ON ddlSalaryGradeHistory.SalaryGradeID = ddlSalaryGrades.ID " +
            "	                WHERE ep2.ID = ePositions.id   " +
            "		                AND ((rateTypeCode = 's' AND validFrom <= @EffectiveDateSalary) " +
            "                       OR (rateTypeCode = 'h' AND validFrom <= @EffectiveDateHourly)) " +
            "                   ORDER BY validFrom DESC), 0.0) ) " +
            "       ELSE (	SELECT salaryMinimum  " +
            "	            FROM ePositionSalaryHistory  " +
            "		            LEFT JOIN ddlSalaryGradeHistory  " +
            "                       ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
            "	                WHERE ePositionID = ePositions.id   " +
            "		                AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) " +
            "                       OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ) " +
            "   END , " +
            " " +
            "SalaryMidpointNew = " +
            "   CASE " +
            "       WHEN (	SELECT salaryMidpoint  " +
            "	            FROM ePositionSalaryHistory  " +
            "		            LEFT JOIN ddlSalaryGradeHistory  " +
            "                       ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
            "	            WHERE ePositionID = ePositions.id   " +
            "		            AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) " +
            "                   OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ) IS NULL " +
            "           THEN (	COALESCE((SELECT Top(1) salaryMidpoint  " +
            "	                FROM ePositions ep2  " +
            "                       LEFT JOIN ddlSalaryGrades " +
            "                           ON ddlSalaryGrades.id = ePositions.PositionGradeID " +
            "		                LEFT JOIN ddlSalaryGradeHistory  " +
            "                           ON ddlSalaryGradeHistory.SalaryGradeID = ddlSalaryGrades.ID " +
            "	                WHERE ep2.ID = ePositions.id   " +
            "		                AND ((rateTypeCode = 's' AND validFrom <= @EffectiveDateSalary) " +
            "                       OR (rateTypeCode = 'h' AND validFrom <= @EffectiveDateHourly)) " +
             "                   ORDER BY validFrom DESC), 0.0) ) " +
            "       ELSE (	SELECT salaryMidpoint  " +
            "	            FROM ePositionSalaryHistory  " +
            "		            LEFT JOIN ddlSalaryGradeHistory  " +
            "                       ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
            "	                WHERE ePositionID = ePositions.id   " +
            "		                AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) " +
            "                       OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ) " +
            "   END , " +
            " " +
            "SalaryMaximumNew = " +
            "   CASE " +
            "       WHEN (	SELECT salaryMaximum  " +
            "	            FROM ePositionSalaryHistory  " +
            "		            LEFT JOIN ddlSalaryGradeHistory  " +
            "                       ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
            "	            WHERE ePositionID = ePositions.id   " +
            "		            AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) " +
            "                   OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ) IS NULL " +
            "           THEN (	COALESCE((SELECT Top(1) salaryMaximum  " +
            "	                FROM ePositions ep2  " +
            "                       LEFT JOIN ddlSalaryGrades " +
            "                           ON ddlSalaryGrades.id = ePositions.PositionGradeID " +
            "		                LEFT JOIN ddlSalaryGradeHistory  " +
            "                           ON ddlSalaryGradeHistory.SalaryGradeID = ddlSalaryGrades.ID " +
            "	                WHERE ep2.ID = ePositions.id   " +
            "		                AND ((rateTypeCode = 's' AND validFrom <= @EffectiveDateSalary) " +
            "                       OR (rateTypeCode = 'h' AND validFrom <= @EffectiveDateHourly)) " +
            "                   ORDER BY validFrom DESC), 0.0) ) " +
            "       ELSE (	SELECT salaryMaximum  " +
            "	            FROM ePositionSalaryHistory  " +
            "		            LEFT JOIN ddlSalaryGradeHistory  " +
            "                       ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
            "	                WHERE ePositionID = ePositions.id   " +
            "		                AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) " +
            "                       OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ) " +
            "   END  " +
            " " +
            "FROM ePositions   " +
            "   INNER JOIN Employees " +
            "       ON Employees.ID = ePositions.EmployeeID  " +
            "   INNER JOIN Persons   " +
            "       ON Persons.ID = Employees.PersonID " +
            "   INNER JOIN Positions  " +
            "       ON Positions.id = ePositions.PositionID  " +
            "   INNER JOIN hrBusinessLevels  " +
            "       ON hrBusinessLevels.ID = Positions.BusinessLevelID  " +
            "WHERE ePositions.PositionID = @PositionID AND ePositions.primaryPosition=1 AND employees.employmentStatusID = 1 " +
            "   AND (employees.RateType = 'h' or employees.RateType = 's') " +
            " " + strRateTypeFilter +
            "ORDER BY LastFirst "
            , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateHourly", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateSalary", SqlDbType.DateTime));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@EffectiveDateHourly"].Value = dateEffectiveHourly;
            cmdSelect.Parameters["@EffectiveDateSalary"].Value = dateEffectiveSalary;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPrimaryDecisionResources(int nPositionID, DateTime dateEffectiveSalary, DateTime dateEffectiveHourly, int nRateType)
        {
            string strRateTypeFilter = "";
            if (nRateType == 0)
                strRateTypeFilter = " And employees.RateType = 's' ";
            else if (nRateType == 1)
                strRateTypeFilter = " AND employees.RateType = 'h' ";

            SqlCommand cmdSelect = new SqlCommand(" " +
            "SELECT	ePositions.id as EPositionID, ePositions.EmployeeID, " +
            "   Persons.LastName+', '+Persons.FirstName As LastFirst, " +
            "   hrBusinessLevels.Code as BusinessLevelCode, " +
            "   Employees.CompanyCode+' : '+Employees.FileNumber as CoCodeFileNumber, " +
            "   COALESCE(Positions.Code, '') as JobCategory, " +
            "   JobCodeHierachy = '', " +
            "   employees.HireDate,  " +
            "   Positions.Title as OldPositionTitle,  " +
            " " +
            "coalesce((	SELECT NewPositionTitle From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), '') " +
            "   as NewPositionTitle, " +
            " " +
            "(	SELECT Reason From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewReason, " +
            " " +
            "   (	SELECT Top(1) Rate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as OldRate, " +
            " " +
            "   (	SELECT Top(1) AnnualSalary  From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as OldAnnualSalary, " +
            " " +
            "   (	SELECT Top(1) HoursPerPayPeriod From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldHoursPerPayPeriod, " +
            " " +
            "   (	SELECT Top(1) effectiveDate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldEffectiveDate, " +
            " " +
            "(	SELECT Top(1) increaseAmount From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldIncreaseAmount, " +
            " " +
            "(	SELECT Top(1) increasePercent From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldIncreasePercent, " +
            " " +
            "(	SELECT Top(1) rateTypeCode From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as OldRateTypeCode, " +
            " " +
            "(	SELECT IncreaseAmount From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewIncreaseAmount, " +
            " " +
            "(	SELECT IncreasePercent From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewIncreasePercent, " +
            " " +
                    " " +
            "(	SELECT BonusIncreaseAmount From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewBonusIncreaseAmount, " +
            " " +
            "(	SELECT BonusIncreasePercent From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewBonusIncreasePercent, " +
            " " +
            "(	SELECT Rate From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewRate, " +
            " " +
            "'NewBiWeeklyRate' = " +
            "	CASE " +
            "		WHEN (	SELECT Top(1) AnnualSalary  " +
            "				From ePositionSalaryHistory  " +
            "				WHERE ePositionID = ePositions.id " +
            "					AND effectiveDate = @EffectiveDateSalary " +
            "					AND AnnualSalary IS NOT NULL) IS NOT NULL		THEN 	(SELECT AnnualSalary/26 From ePositionSalaryHistory  " +
            "																										WHERE ePositionID = ePositions.id   " +
            "																											AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "		ELSE NULL " +
            "	END, " +
            " " +
            "(SELECT AnnualSalary From ePositionSalaryHistory  " +
            "			WHERE ePositionID = ePositions.id  " +
            "				AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)))  " +
            "   as NewAnnualSalary,  " +
            " " +
            "(	SELECT updatedBy From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as UpdatedBy, " +
            " " +
            "   coalesce((	SELECT editLocked From ePositionSalaryHistory  " +
            "           	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as editLocked  " +
            " " +
            ",coalesce((	SELECT increaseApproved From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as IncreaseApproved " +
                "   ,(SELECT Top 1 statusClassification FROM employeeStatusMMC WHERE employeeStatusMMC.CompanyCode = Employees.CompanyCode " +
                "        and  employeeStatusMMC.FileNumber = Employees.FileNumber ) as StatusClassification " +
            " " +
            " " +
            "FROM ePositions   " +
            "   INNER JOIN Employees " +
            "       ON Employees.ID = ePositions.EmployeeID  " +
            "   INNER JOIN Persons   " +
            "       ON Persons.ID = Employees.PersonID " +
            "   INNER JOIN Positions  " +
            "       ON Positions.id = ePositions.PositionID  " +
            "   INNER JOIN hrBusinessLevels  " +
            "       ON hrBusinessLevels.ID = Positions.BusinessLevelID  " +
            "WHERE ePositions.PositionID = @PositionID AND ePositions.primaryPosition=1 AND employees.employmentStatusID = 1 " +
            " " + strRateTypeFilter +
            "ORDER BY LastFirst "
            , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateHourly", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateSalary", SqlDbType.DateTime));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@EffectiveDateHourly"].Value = dateEffectiveHourly;
            cmdSelect.Parameters["@EffectiveDateSalary"].Value = dateEffectiveSalary;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPrimaryV3(int nPositionID, DateTime dateEffectiveSalary, DateTime dateEffectiveHourly, int nRateType)
        {
            string strRateTypeFilter = "";
            if (nRateType == 0)
                strRateTypeFilter = " And epsh.RateTypeCode = 's' ";
            else if (nRateType == 1)
                strRateTypeFilter = " AND epsh.RateTypeCode = 'h' ";

            SqlCommand cmdSelect = new SqlCommand(" " +
            "SELECT ePositions.id as EPositionID, ePositions.EmployeeID, Persons.LastName+', '+Persons.FirstName As LastFirst, " +
            "   employees.HireDate,  Positions.Title as PositionTitle,  " +
            "   ePositions.PositionGradeID,  " +
            "   subString(hrBusinessLevels.Code,LEN(hrBusinessLevels.Code)-2,3 ) as BusinessCodeLast3,  " +
            "   epsh.Rate, epsh.HoursPerPayPeriod, epsh.effectiveDate,  " +
            "   epsh.salaryMinimum, epsh.salaryMidpoint, epsh.salaryMaximum, epsh.compRatio,  " +
            "   epsh.IncreaseAmount, epsh.IncreasePercent, epsh.RateTypeCode, er.Rating as PreviousRating,    " +
            "   erCurrent.Rating as CurrentRating,  " +
            "   epshCurrent.IncreaseAmount as currentIncAmnt, epshCurrent.IncreasePercent as currentIncPcnt,  " +
            "   epshCurrent.Rate as currentRate, epshCurrent.CompRatio as NewCompRatio,  " +
            "   epshCurrent.EXCLO, epshCurrent.EXCHI, epshCurrent.Reason, epshCurrent.UpdatedBy,  " +
            "   coalesce(epshCurrent.EditLocked, 'false') as editLocked,  " +
            "   coalesce(epshCurrent.IncreaseApproved, 'false') as IncreaseApproved  " +
            "FROM ePositions    " +
            "   INNER JOIN Employees   " +
            "       ON Employees.ID = ePositions.EmployeeID   " +
            "   INNER JOIN Persons   " +
            "       ON Persons.ID = Employees.PersonID   " +
            "   INNER JOIN Positions   " +
            "       ON Positions.id = ePositions.PositionID   " +
            "   INNER JOIN hrBusinessLevels   " +
            "       ON hrBusinessLevels.ID = Positions.BusinessLevelID   " +
            "   LEFT JOIN    " +
            "       (	SELECT	Rate, HoursPerPayPeriod, ePositionSalaryHistory.effectiveDate, ePositionSalaryHistory.ePositionID, rateTypeCode,    " +
            "               salaryMinimum, salaryMidpoint, salaryMaximum, compRatio, IncreaseAmount, IncreasePercent   " +
            "	        FROM ePositionSalaryHistory   " +
            "   		    INNER JOIN	(SELECT ePositionID , MAX(EffectiveDate) as effectiveDate  " +
            "   					    FROM ePositionSalaryHistory   " +
            "   					    WHERE (rateTypeCode = 'h' and effectiveDate < @effectiveDateHourly) OR (rateTypeCode='s' AND effectiveDate < @effectiveDateSalary)   " +
            "   					    GROUP BY ePositionID ) topPrevSH  " +
            "   			    ON ePositionSalaryHistory.ePositionID = topPrevSH.ePositionID " +
            "               LEFT JOIN ddlSalaryGradeHistory  " +
            "                   ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID  " +
            "   	    WHERE ePositionSalaryHistory.effectiveDate = topPrevSH.EffectiveDate ) epsh  " +
            "       ON epsh.ePositionID = ePositions.ID  " +
            "   LEFT JOIN (	SELECT EmployeeRatings.*  " +
            "       		FROM EmployeeRatings " +
            "       			INNER JOIN Employees ON EmployeeRatings.FileNumber = Employees.FileNumber " +
            "       		WHERE (rateType='s' AND year-1 = DATEPART(year, @effectiveDateSalary)) OR (rateType='h' AND year-1 = DATEPART(year, @effectiveDateHourly)) ) er  " +
            "       ON er.FileNumber = Employees.FileNumber  " +
            "   LEFT JOIN (	SELECT EmployeeRatings.*  " +
            "       		FROM EmployeeRatings " +
            "       			INNER JOIN Employees ON EmployeeRatings.FileNumber = Employees.FileNumber " +
            "       		WHERE (rateType='s' AND Year = DATEPART(year, @effectiveDateSalary)) OR (rateType='h' AND year = DATEPART(year, @effectiveDateHourly)) ) erCurrent  " +
            "       ON erCurrent.FileNumber = Employees.FileNumber  " +
            "LEFT JOIN   " +
            "       (	SELECT	Rate, HoursPerPayPeriod, effectiveDate, ePositionID,  " +
            "               salaryMinimum, salaryMidpoint, salaryMaximum, compRatio, IncreaseAmount, IncreasePercent, " +
            "               EXCLO, EXCHI, Reason, updatedBy, editLocked, increaseApproved  " +
            "           FROM ePositionSalaryHistory  " +
            "               LEFT JOIN ddlSalaryGradeHistory  " +
            "                   ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID  " +
            "           WHERE (rateTypeCode = 'h' and ePositionSalaryHistory.effectiveDate = @effectiveDateHourly)  " +
            "               OR (rateTypeCode = 's' and ePositionSalaryHistory.effectiveDate = @effectiveDateSalary)) epshCurrent  " +
            "   ON epshCurrent.ePositionID = ePositions.ID " +
            "WHERE ePositions.PositionID = @PositionID AND  ePositions.primaryPosition=1 AND employees.employmentStatusID = 1 " +
            " " + strRateTypeFilter +
            "ORDER BY LastFirst "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateHourly", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateSalary", SqlDbType.DateTime));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@EffectiveDateHourly"].Value = dateEffectiveHourly;
            cmdSelect.Parameters["@EffectiveDateSalary"].Value = dateEffectiveSalary;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPrimaryOld(int nPositionID, int nYear, int nRateType) // this version only looked at current year/ prev year as opposed to effectiveDate and last raise
        {
            string strRateTypeFilter = "";
            if (nRateType == 0)
                strRateTypeFilter = " And epsh.RateTypeCode = 's' ";
            else if (nRateType == 1)
                strRateTypeFilter = " AND epsh.RateTypeCode = 'h' ";

            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT ePositions.id as EPositionID, ePositions.EmployeeID, Persons.LastName+', '+Persons.FirstName As LastFirst, " +
                "       employees.HireDate,  Positions.Title as PositionTitle, " +
                "       ePositions.PositionGradeID, " +
                "       subString(hrBusinessLevels.Code,LEN(hrBusinessLevels.Code)-2,3 ) as BusinessCodeLast3, " +
                "       epsh.Rate, epsh.HoursPerPayPeriod, epsh.effectiveDate,  " +
                "       epsh.salaryMinimum, epsh.salaryMidpoint, epsh.salaryMaximum, epsh.compRatio, " +
                "       epsh.IncreaseAmount, epsh.IncreasePercent, epsh.RateTypeCode, er.Rating as PreviousRating, " +
                "       erCurrent.Rating as CurrentRating, " +
                "       epshCurrent.IncreaseAmount as currentIncAmnt, epshCurrent.IncreasePercent as currentIncPcnt, " +
                "       epshCurrent.Rate as currentRate, epshCurrent.CompRatio as NewCompRatio, " +
                "       epshCurrent.EXCLO, epshCurrent.EXCHI, epshCurrent.Reason, epshCurrent.UpdatedBy, " +
                "       coalesce(epshCurrent.EditLocked, 'false') as editLocked, " +
                "       coalesce(epshCurrent.IncreaseApproved, 'false') as IncreaseApproved " +
                "FROM ePositions  " +
                "   INNER JOIN Employees  " +
                "       ON Employees.ID = ePositions.EmployeeID  " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID  " +
                "   INNER JOIN Positions " +
                "       ON Positions.id = ePositions.PositionID " +
                "   INNER JOIN hrBusinessLevels " +
                "       ON hrBusinessLevels.ID = Positions.BusinessLevelID " +
                "   LEFT JOIN " +
                "       (	SELECT	Rate, HoursPerPayPeriod, effectiveDate, ePositionID, rateTypeCode, " +
                "		            salaryMinimum, salaryMidpoint, salaryMaximum, compRatio, IncreaseAmount, IncreasePercent " +
                "           FROM ePositionSalaryHistory " +
                "	            LEFT JOIN ddlSalaryGradeHistory " +
                "		            ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
                "           WHERE DATEPART(year, ePositionSalaryHistory.effectiveDate) = @Year-1 ) epsh " +
                "       ON epsh.ePositionID = ePositions.ID " +
                "   LEFT JOIN (SELECT * FROM EmployeeRatings WHERE Year=@Year-1) er " +
                "       ON er.FileNumber = Employees.FileNumber " +
                "   LEFT JOIN (SELECT * FROM EmployeeRatings WHERE Year=@Year) erCurrent " +
                "       ON erCurrent.FileNumber = Employees.FileNumber " +
                "   LEFT JOIN " +
                "       (	SELECT	Rate, HoursPerPayPeriod, effectiveDate, ePositionID,  " +
                "		            salaryMinimum, salaryMidpoint, salaryMaximum, compRatio, IncreaseAmount, IncreasePercent, " +
                "                   EXCLO, EXCHI, Reason, updatedBy, editLocked, increaseApproved " +
                "           FROM ePositionSalaryHistory " +
                "	            LEFT JOIN ddlSalaryGradeHistory " +
                "		            ON ddlSalaryGradeHistory.ID = ePositionSalaryHistory.salGradeHistoryID " +
                "           WHERE DATEPART(year, ePositionSalaryHistory.effectiveDate) = @Year ) epshCurrent " +
                "       ON epshCurrent.ePositionID = ePositions.ID " +
                "WHERE ePositions.PositionID = @PositionID AND ePositions.primaryPosition=1 AND employees.employmentStatusID = 1 " +
                strRateTypeFilter +
                "ORDER BY LastFirst "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@Year"].Value = nYear;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPrimary()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.ID as PositionID, Positions.ReportsToPositionID, Persons.ID as PersonID, Persons.preferredName, Persons.LastName+', '+Persons.FirstName As LastFirst " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Persons " +
                "       ON Persons.ID = Employees.PersonID " +
                "   INNER JOIN Positions " +
                "       ON ePositions.PositionID = Positions.ID " +
                "WHERE ePositions.primaryPosition=1 " +
                "ORDER BY LastFirst "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllForPersonID(int nPersonID)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                        "SELECT Positions.ID,  Persons.LastName+', '+Persons.FirstName as PersonName " +
                        "FROM Persons   " +
                        "   INNER JOIN Employees " +
                        "       ON Employees.PersonID = Persons.ID " +
                        "   INNER JOIN ePositions " +
                        "       ON ePositions.EmployeeID = employees.ID " +
                        "   INNER JOIN Positions " +
                        "       ON Positions.ID = ePositions.PositionID " +
                        "WHERE PersonID = " + nPersonID
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllForPositionIdWithNoEndDate(int nPositionId, int nEmployeeId)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                        "SELECT * " +
                        "FROM ePositions   " +
                        "WHERE positionID = @PositionId  and actualEndDate is null and employeeId <> @EmployeeId"
                    , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int, 4));

            cmdSelect.Parameters["@PositionId"].Value = nPositionId;
            cmdSelect.Parameters["@EmployeeId"].Value = nEmployeeId;


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllForEmployeeWithApdPositionNumberWithNoEndDateExceptId(int nId, int nEmployeeId, int nAdpPositionNumber)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                        "SELECT * " +
                        "FROM ePositions   " +
                        "WHERE id<>@Id  AND EmployeeId=@EmployeeId and AdpPositionNumber=@AdpPositionNumber and actualEndDate is null "
                    , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@AdpPositionNumber", SqlDbType.Int, 4));

            cmdSelect.Parameters["@Id"].Value = nId;
            cmdSelect.Parameters["@EmployeeId"].Value = nEmployeeId;
            cmdSelect.Parameters["@AdpPositionNumber"].Value = nAdpPositionNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllForEmployeeWithApdPositionNumberWithNoEndDate(int nEmployeeId, int nAdpPositionNumber)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                        "SELECT * " +
                        "FROM ePositions   " +
                        "WHERE EmployeeId=@EmployeeId and AdpPositionNumber=@AdpPositionNumber and actualEndDate is null "
                    , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@AdpPositionNumber", SqlDbType.Int, 4));

            cmdSelect.Parameters["@EmployeeId"].Value = nEmployeeId;
            cmdSelect.Parameters["@AdpPositionNumber"].Value = nAdpPositionNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllPrimaryForPersonID(int nPersonID)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                        "SELECT Positions.ID,  Persons.LastName+', '+Persons.FirstName as PersonName, ePositions.ID as ePositionId " +
                        "FROM Persons   " +
                        "   INNER JOIN Employees " +
                        "       ON Employees.PersonID = Persons.ID " +
                        "   INNER JOIN ePositions " +
                        "       ON ePositions.EmployeeID = employees.ID " +
                        "   INNER JOIN Positions " +
                        "       ON Positions.ID = ePositions.PositionID " +
                        "WHERE PersonID = " + nPersonID + " AND ePositions.primaryPosition = 1 "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataRow GetPrimaryForPersonID(int nPersonID)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                        "SELECT Positions.ID as PositionID,  Persons.LastName+', '+Persons.FirstName as PersonName " +
                        "FROM Persons   " +
                        "   INNER JOIN Employees " +
                        "       ON Employees.PersonID = Persons.ID " +
                        "   INNER JOIN ePositions " +
                        "       ON ePositions.EmployeeID = employees.ID " +
                        "   INNER JOIN Positions " +
                        "       ON Positions.ID = ePositions.PositionID " +
                        "WHERE PersonID = @PersonID AND ePositions.PrimaryPosition = 1 "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PersonID", SqlDbType.Int, 4));

            cmdSelect.Parameters["@PersonID"].Value = nPersonID;


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);
            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            if (ds.Tables["ePositions"].Rows.Count == 1)
                return ds.Tables["ePositions"].Rows[0];
            else if (ds.Tables["ePositions"].Rows.Count > 1)
                throw new Exception("More than one primary ePosition for PersonID.");
            else
                return null;
        }

        public DataTable GetAllEmployeePositions(int nEmployeeID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.* " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "WHERE ePositions.EmployeeID = " + nEmployeeID + " AND ePositions.StartDate is NOT NULL " +
                "   AND ePositions.actualEndDate is Null AND ePositions.primaryPosition = 1 "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsRegardlessOfStatus(int nEmployeeID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.* " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "WHERE ePositions.EmployeeID = " + nEmployeeID
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsRegardlessOfStatusWithPositionInfo(int nEmployeeID)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT LastName, FirstName, Positions.code as PositionCode, Positions.Title as PositionTitle, "+
	            "    ePositions.*  "+
                "FROM ePositions  "+
                "   INNER JOIN Employees  " +
                "       ON Employees.ID = ePositions.EmployeeID  "+
	            "    INNER JOIN Persons On Persons.Id = employees.personId "+
	            "    INNER JOIN Positions On Positions.Id = ePositions.PositionId "+
	            "    INNER JOIN hrBusinessLevels on hrBusinessLevels.id = positions.businessLevelid "+
	            "    INNER JOIN Jobs on Jobs.Id = Positions.JobId "+
                "WHERE ePositions.EmployeeID =@EmployeeId "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int, 4));
            cmdSelect.Parameters["@EmployeeId"].Value = nEmployeeID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositions(string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.* " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "WHERE Employees.FileNumber = @FileNumber "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsPrimary(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.* " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "WHERE Employees.FileNumber = @FileNumber And Employees.CompanyCode=@CompanyCode and ePositions.primaryPosition = 1"
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }


        public DataTable GetAllEmployeePositionsPrimaryWithFteTotal(string strCompanyCode, string strFileNumber, DateTime payPeriodDate)
        {

            //SqlCommand cmdSelect = new SqlCommand(" "+
            //    "SELECT employees.filenumber, ePositions.* "+ 
            //    "    ,(	SELECT  SUM (fte) "+ 
            //    "        FROM ePositions ep2 "+
            //    "           INNER JOIN Employees e2 "+
            //    "               ON E2.ID = ep2.EmployeeID  "+
            //    "        WHERE E2.FileNumber = @FileNumber And E2.CompanyCode=@CompanyCode and ep2.primaryPosition = 1) as FteTotal "+
            //    "FROM ePositions  "+
            //    "   INNER JOIN Employees  "+
            //    "       ON Employees.ID = ePositions.EmployeeID  "+
            //    "WHERE Employees.FileNumber = @FileNumber And Employees.CompanyCode=@CompanyCode and ePositions.primaryPosition = 1 "
            //    , m_Connection);

            SqlCommand cmdSelect = new SqlCommand(" "+
                "SELECT employees.filenumber, ePositions.*  "+
                "    ,(	SELECT  SUM (fte)    "+
                "        FROM ePositions ep2  "+
                "           INNER JOIN Employees e2  "+
                "               ON E2.ID = ep2.EmployeeID  "+ 
                "        WHERE E2.FileNumber = @FileNumber And E2.CompanyCode=@CompanyCode  "+
                "        and ((@PayPeriodDate between eP2.startDate and eP2.actualEndDate  "+
			    "	            and EP2.startDate is not null and eP2.actualEndDate is not NULL) "+
	            "    OR	(@PayPeriodDate >= eP2.startDate  "+ 
			    "	            and EP2.startDate is not null and eP2.actualEndDate is NULL))) as FteTotal  "+
                "FROM ePositions  "+ 
                "   INNER JOIN Employees "+  
                "       ON Employees.ID = ePositions.EmployeeID "+  
                "WHERE Employees.FileNumber = @FileNumber And Employees.CompanyCode=@CompanyCode  "+ 
	            "    and ((@PayPeriodDate between ePositions.startDate and ePositions.actualEndDate  "+
			    "	            and EPositions.startDate is not null and ePositions.actualEndDate is not NULL) "+
	            "    OR	(@PayPeriodDate >= ePositions.startDate  "+ 
			    "	            and EPositions.startDate is not null and ePositions.actualEndDate is NULL)) "
                , m_Connection);


            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;
            cmdSelect.Parameters["@PayPeriodDate"].Value = payPeriodDate;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }
       

        public DataTable GetAllEmployee_ePositionsWithPositionID(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.Id as PositionID " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions ON Positions.ID = ePositions.PositionID " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber AND ePositions.PrimaryPosition = 1"
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployee_ePositionsWithPositionIDHavingActiveStatusAndIsPrimary(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.Id as PositionID " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions ON Positions.ID = ePositions.PositionID " +
                "   INNER JOIN ddlEmploymentStatuses On ddlEmploymentStatuses.id = Employees.EmploymentStatusId " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber AND ePositions.PrimaryPosition = 1 " +
                "   AND ddlEmploymentStatuses.Code = 'A' "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployee_ePositionsWithStartDateLessThanEntAllergy(string strCompanyCode, string strFileNumber, DateTime startDate)
        {
            SqlCommand cmdSelect = new SqlCommand(" "+
                "SELECT ePositions.id as ePosId, Positions.Id as PosId, Positions.Code as PositionCode, Positions.id as PosId " +
	            "    , Employees.id as EmpId, Employees.FileNumber, Employees.CompanyCode "+
                "FROM ePositions JOIN Employees On Employees.id = ePositions.employeeID "+
	            "    JOIN Positions on Positions.id = ePositions.PositionId "+
                "WHERE employees.CompanyCode = @CompanyCode and employees.FileNumber = @FileNumber "+
	            "    AND ePositions.startDate < @StartDate "+
                "ORDER BY ePositions.startDate DESC "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;
            cmdSelect.Parameters["@StartDate"].Value = startDate;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployee_ePositionsWithCodeForStJoseph(string strCompanyCode, string strFileNumber, string strCode)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "select positions.code, substring(positions.code,6,3)  as deptCode, employees.companyCode, employees.filenumber, ePositions.* "+
                "   ,Positions.id as posId, ePositions.id as ePosId, employees.id as empId "+
                "from ePositions "+
	            "   join positions on positions.id = ePositions.positionid  "+
	            "   join employees on employees.id = epositions.employeeID "+
                "where substring(positions.code,6,3) = @PositionCode  "+
	            "   and employees.companyCode = @CompanyCode and employees.filenumber = @FileNumber  "+
                "ORDER BY ePositions.ActualEndDate "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@PositionCode", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;
            cmdSelect.Parameters["@PositionCode"].Value = strCode;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }


        public DataTable GetAllEmployee_ExistingActaulsForPinehurst(string strCompanyCode, string strFileNumber, int nEmployeeID, int nPositionID, DateTime datePayPeriod)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                " Select * from EmployeeActuals where FileNumber=@FileNumber AND CompanyCode=@CompanyCode " +
                " AND EmployeeID=@EmployeeID AND PositionID=@PositionID AND  PayPeriodDate=@PayPeriodDate ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;
            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@PayPeriodDate"].Value = datePayPeriod;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "EmployeeActuals");

            return ds.Tables["EmployeeActuals"];
        }


        public DataTable GetAllEmployee_ePositionsWithCodeForPinehurst(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "select positions.code, substring(positions.code,6,3)  as deptCode, employees.companyCode, employees.filenumber, ePositions.* " +
                "   ,Positions.id as posId, ePositions.id as ePosId, employees.id as empId " +
                "from ePositions " +
                "   join positions on positions.id = ePositions.positionid  " +
                "   join employees on employees.id = epositions.employeeID " +
                "where employees.companyCode = @CompanyCode and employees.filenumber = @FileNumber  " +
                "ORDER BY ePositions.ActualEndDate "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
           // cmdSelect.Parameters.Add(new SqlParameter("@PositionCode", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;
           // cmdSelect.Parameters["@PositionCode"].Value = strCode;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployee_ePositionsWithFullCodeForStJoseph(string strCompanyCode, string strFileNumber, string strCode)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "select positions.code,  employees.companyCode, employees.filenumber, ePositions.* " +
                "   ,Positions.id as posId, ePositions.id as ePosId, employees.id as empId " +
                "from ePositions " +
                "   join positions on positions.id = ePositions.positionid  " +
                "   join employees on employees.id = epositions.employeeID " +
                "where Positions.code = @PositionCode  " +
                "   and employees.companyCode = @CompanyCode and employees.filenumber = @FileNumber  " +
                "ORDER BY ePositions.ActualEndDate "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@PositionCode", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;
            cmdSelect.Parameters["@PositionCode"].Value = strCode;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployee_ePositionsStJoseph(int nEmployeeId, int nPositionID,
                                DateTime positionEffectiveDate)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "select id "+
                "from ePositions " +
                "where EmployeeId = @EmployeeId   " +
                "   and positionId=@PositionId and startDate=@StartDate  " 
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@PositionId", SqlDbType.Int,4));
            cmdSelect.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));

            cmdSelect.Parameters["@EmployeeId"].Value = nEmployeeId;
            cmdSelect.Parameters["@positionId"].Value = nPositionID;
            cmdSelect.Parameters["@StartDate"].Value = positionEffectiveDate;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }


        public DataTable GetAllEmployee_ePositionsWithPositionIDHavingOrderByPrimaryThenStartDateDesc(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.Id as PositionID " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions ON Positions.ID = ePositions.PositionID " +
                "   INNER JOIN ddlEmploymentStatuses On ddlEmploymentStatuses.id = Employees.EmploymentStatusId " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber  " +
                "ORDER BY PrimaryPosition DESC, ePositions.StartDate DESC "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }


        public DataTable GetAllEmployee_ePositionsWithPositionIDOrderByEpositionId(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.Id as PositionID " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions ON Positions.ID = ePositions.PositionID " +
                "   INNER JOIN ddlEmploymentStatuses On ddlEmploymentStatuses.id = Employees.EmploymentStatusId " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber " +
                "ORDER BY ePositions.id DESC"
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsUsingCompanyCodeFileNumber(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.Title as PositionTitle, hrBusinessLevels.Code as BUCode, HrBusinessLevels.Title as BUTitle, " +
                "jobs.Code as jobCode, jobs.Title as jobTitle, Positions.Code as PositionCode " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions On Positions.ID = ePositions.PositionId " +
                "   INNER JOIN HrBusinessLevels On HrBusinessLevels.Id = Positions.businessLevelId " +
                "   INNER JOIN Jobs On Jobs.Id = Positions.JobId " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsUsingCompanyCodeFileNumberOrderByStartDateDesc(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.Title as PositionTitle, "+
                "   hrBusinessLevels.Code as BUCode, HrBusinessLevels.Title as BUTitle, " +
                "   jobs.Code as jobCode, jobs.Title as jobTitle, Positions.Code as PositionCode " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions On Positions.ID = ePositions.PositionId " +
                "   INNER JOIN HrBusinessLevels On HrBusinessLevels.Id = Positions.businessLevelId " +
                "   INNER JOIN Jobs On Jobs.Id = Positions.JobId " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber "+
                "ORDER BY ePositions.StartDate DESC "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }



        public DataTable GetLatestUsingCompanyCodeFileNumber(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("select Top 1 Positions.title as PositionTitle "+
                "FROM Persons join Employees on employees.personID = persons.id  "+
	            "    join ePositions on epositions.employeeid = employees.id "+
	            "    join positions on positions.id = ePositions.positionID "+
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber "+
                "order by ePositions.startDate DESC "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetForPayPeriodUsingCompanyCodeFileNumberAndCostCenter(DateTime payPeriodDate
            , string strCompanyCode, string strFileNumber, string strCostCenter)
        {
            SqlCommand cmdSelect = new SqlCommand(" "+
                "select Positions.Title as PositionTitle, AdpPositionNumber "+
                "FROM ePositions join positions on positions.id = ePositions.positionID "+
	            "    join Employees on employees.id = ePositions.employeeId "+
                "where positions.psInfo = @CostNumber "+ 
	            "    and ePositions.ActualEndDate is not NULL  "+
	            "    AND @PayPeriodDate between ePositions.startDate and ePositions.actualEndDate "+
	            "    AND employees.CompanyCode = @CompanyCode and employees.FileNumber = @FileNumber "+
                "UNION "+
                "select Positions.Title as PositionTitle, AdpPositionNumber " +
                "FROM ePositions join positions on positions.id = ePositions.positionID "+
	            "    join Employees on employees.id = ePositions.employeeId "+
                "where  "+
	            "    positions.psInfo = @CostNumber  "+
	            "    AND ePositions.actualEndDate is NULL  "+
	            "    AND @PayPeriodDate >= ePositions.startDate "+
	            "    AND employees.CompanyCode = @CompanyCode and employees.FileNumber = @FileNumber "+ 
                "ORDER BY adpPositionNumber "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@CostNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@PayPeriodDate", SqlDbType.DateTime));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;
            cmdSelect.Parameters["@CostNumber"].Value = strCostCenter;
            cmdSelect.Parameters["@PayPeriodDate"].Value = payPeriodDate;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsUsingCompanyCodeFileNumberDescendByStartDate(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.Title as PositionTitle, hrBusinessLevels.Code as BUCode, HrBusinessLevels.Title as BUTitle, " +
                "jobs.Code as jobCode, jobs.Title as jobTitle " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions On Positions.ID = ePositions.PositionId " +
                "   INNER JOIN HrBusinessLevels On HrBusinessLevels.Id = Positions.businessLevelId " +
                "   INNER JOIN Jobs On Jobs.Id = Positions.JobId " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber " +
                "ORDER BY ePositions.StartDate DESC "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsUsingCompanyCodeFileNumberBudgetCodeDescendByStartDateWith(string strCompanyCode, string strFileNumber, int nBudgetCode)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.Title as PositionTitle, hrBusinessLevels.Code as BUCode, HrBusinessLevels.Title as BUTitle, " +
                "jobs.Code as jobCode, jobs.Title as jobTitle, Positions.code as PositionCode " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions On Positions.ID = ePositions.PositionId " +
                "   INNER JOIN HrBusinessLevels On HrBusinessLevels.Id = Positions.businessLevelId " +
                "   INNER JOIN Jobs On Jobs.Id = Positions.JobId " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber AND ePositions.BudgetCode=@BudgetCode " +
                "ORDER BY ePositions.StartDate DESC "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@BudgetCode", SqlDbType.Int, 4));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;
            cmdSelect.Parameters["@BudgetCode"].Value = nBudgetCode;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsUsingCompanyCodeFileNumberDescendByEPositionId(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*, Positions.Title as PositionTitle, hrBusinessLevels.Code as BUCode, HrBusinessLevels.Title as BUTitle, " +
                "jobs.Code as jobCode, jobs.Title as jobTitle " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions On Positions.ID = ePositions.PositionId " +
                "   INNER JOIN HrBusinessLevels On HrBusinessLevels.Id = Positions.businessLevelId " +
                "   INNER JOIN Jobs On Jobs.Id = Positions.JobId " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber " +
                "ORDER BY ePositions.Id DESC "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsUsingCompanyCodeFileNumberPrimary(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.* " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber AND ePositions.PrimaryPosition = 1 "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsForMonetaryConversionReport()
        {
            SqlCommand cmdSelect = new SqlCommand(" "+
                "Select * "+
	            "    ,	Case "+
			    "            WHEN rateTypeCode = 'S' then Rate "+
			    "            WHEN rateTypeCode = 'H' then HoursPerPayPeriod * rate "+ 
		        "        END as PayPeriodAmount "+
	            "    ,	Case "+
			    "            WHEN rateTypeCode = 'S' then Rate * ConversionRate "+
			    "            WHEN rateTypeCode = 'H' then (HoursPerPayPeriod * rate) * ConversionRate "+ 
		        "        END as PayPeriodAmountConversion "+
                "FROM "+
                "    ( "+
                "    Select lastName+', '+ FirstName as PersonName, Employees.companyCode, Employees.FileNumber "+
	            "        , Positions.Title as PositionTitle, ePositions.PayFrequencyCode, MonetaryCountryCode "+
	            "        , (	SELECT Top 1 RateTypeCode From ePositionSalaryHistory Where ePositionSalaryHistory.epositionId = epositions.id "+ 
		        "            ORDER BY EffectiveDate DESC) as RateTypeCode "+ 
	            "        , (	SELECT Top 1 Rate From ePositionSalaryHistory Where ePositionSalaryHistory.epositionId = epositions.id "+ 
		        "            ORDER BY EffectiveDate DESC) as Rate "+ 
	            "        , (	SELECT Top 1 HoursPerPayPeriod From ePositionSalaryHistory Where ePositionSalaryHistory.epositionId = epositions.id  "+
		        "            ORDER BY EffectiveDate DESC) as HoursPerPayPeriod "+ 
	            "        , (	SELECT Top 1 CONVERT(VARCHAR(10),EffectiveDate,101) From ePositionSalaryHistory Where ePositionSalaryHistory.epositionId = epositions.id "+ 
		        "            ORDER BY EffectiveDate DESC) as effectiveDate "+
	            "        , (	SELECT Top 1 ConversionRate From MonetaryConversionRates "+ 
		        "            Where MonetaryConversionRates.CountryCode = epositions.MonetaryCountryCode "+ 
		        "            ORDER BY EffectiveDate DESC) as ConversionRate "+
                "        , ePositions.id as ePositionId "+
                "    FROM Persons "+
	            "        INNER JOIN Employees On Employees.PersonId = Persons.Id "+
	            "        INNER JOIN ePositions on ePositions.EmployeeId = Employees.Id "+
	            "        INNER JOIN Positions on Positions.Id = ePositions.Id "+
                "    WHERE ePositions.PrimaryPosition = 1 and Employees.EmploymentStatusId = 1 "+
                "    ) t1 " +
                "ORDER BY PersonName  "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            //cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            //cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            //cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            //cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsUsingCompanyCodeFileNumberPrimaryDecending(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.* " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "WHERE Employees.CompanyCode = @CompanyCode AND Employees.FileNumber = @FileNumber AND ePositions.PrimaryPosition = 1 " +
                "ORDER BY ePositions.ID DESC "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllEmployeePositionsForPositionCode(string strFileNumber, string strPositionCode)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.* " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions " +
                "       ON Positions.id = ePositions.PositionID " +
                "WHERE Employees.FileNumber = @FileNumber AND Positions.Code = @PositionCode "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@PositionCode", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;
            cmdSelect.Parameters["@PositionCode"].Value = strPositionCode;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllCurrentEmployeePositionsWithPositionName(int nEmployeeID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ePositions.*,  Positions.Title " +
                "FROM ePositions " +
                "   INNER JOIN Employees " +
                "       ON Employees.ID = ePositions.EmployeeID " +
                "   INNER JOIN Positions " +
                "       ON ePositions.PositionID = Positions.ID " +
                "WHERE ePositions.EmployeeID = " + nEmployeeID + " AND ePositions.StartDate is NOT NULL AND ePositions.actualEndDate is Null "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAllWithBusJobPositionTitlesEmployeeNameUserNamePrimaryNotTerminatedHasSubordinatePositions()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "Select Positions.ID as PositionID, hrBusinessLevels.Title as BusinessTitle, Jobs.Title as JobTitle, Jobs.Code as JobCode, " +
                "    Positions.Title as PositionTitle, Positions.closedDate, reportsToPositionID, ePositions.ID as ePositionID, " +
                "    lastname, firstname, username, email " +
                "FROM Positions " +
                "    INNER JOIN Jobs ON Jobs.ID = Positions.JobID " +
                "    INNER JOIN hrBusinessLevels ON hrBusinessLevels.id = Positions.BusinessLevelID " +
                "    INNER JOIN ePositions On ePositions.PositionID = Positions.ID " +
                "    INNER JOIN employees ON employees.id = ePositions.EmployeeID " +
                "    INNER JOIN Persons ON persons.ID = employees.PersonID " +
                "    INNER JOIN UserNamesPersons on UsernamesPersons.PersonID = persons.id " +
                "WHERE (Positions.closedDate Is NULL OR Positions.ClosedDate < GetDate() ) " +
                "    AND Positions.ID IN (Select reportsToPositionID FROM Positions WHERE reportsToPositionID IS NOT NULL) " +
                "    AND ePositions.PrimaryPosition = 1 " +
                "   AND Employees.EmploymentStatusID = 1 "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataRow GetRecordForID(int nID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM ePositions WHERE ID=@ID", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@ID"].Value = nID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            if (ds.Tables["ePositions"].Rows.Count > 0)
                return ds.Tables["ePositions"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecordForID_WITH_CC_FILENO(int nID)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT ePositions.*, Employees.CompanyCode, Employees.FileNumber " +
                "FROM ePositions " +
                "   INNER JOIN Employees ON ePositions.EmployeeID = Employees.ID " +
                "WHERE ePositions.ID=@ID "
                , m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@ID"].Value = nID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            if (ds.Tables["ePositions"].Rows.Count > 0)
                return ds.Tables["ePositions"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecordForEmpID_PositionID(int nEmpID, int nPositionID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM ePositions WHERE EmployeeID=@EmployeeID AND PositionID=@PositionID", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));

            cmdSelect.Parameters["@EmployeeID"].Value = nEmpID;
            cmdSelect.Parameters["@PositionID"].Value = nPositionID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            if (ds.Tables["ePositions"].Rows.Count == 1)
                return ds.Tables["ePositions"].Rows[0];
            else if (ds.Tables["ePositions"].Rows.Count > 1)
                throw new Exception("More than one ePosition for EmployeeID PositionID combination");
            else
                return null;
        }

        public DataRow GetRecordForEmpID_PositionIDCompanyCode(int nEmpID, int nPositionID, string strCompanyCode)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM ePositions " +
                "WHERE EmployeeID=@EmployeeID AND PositionID=@PositionID AND CompanyCode = @CompanyCode", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@EmployeeID"].Value = nEmpID;
            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            if (ds.Tables["ePositions"].Rows.Count == 1)
                return ds.Tables["ePositions"].Rows[0];
            else if (ds.Tables["ePositions"].Rows.Count > 1)
                throw new Exception("More than one ePosition for EmployeeID PositionID combination");
            else
                return null;
        }


        public DataRow GetPerfProfileID(int nID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Positions.PerfProfileID " +
                "FROM ePositions " +
                "       INNER JOIN Positions " +
                "           ON ePositions.PositionID = Positions.ID " +
                "WHERE ePositions.ID=@ID", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@ID"].Value = nID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            if (ds.Tables["ePositions"].Rows.Count > 0)
                return ds.Tables["ePositions"].Rows[0];
            else
                return null;
        }

        public DataTable GetAllPrimaryDecisionResourcesNw(int nPositionID, DateTime dateEffectiveSalary, DateTime dateEffectiveHourly, int nRateType)
        {
            string strRateTypeFilter = "";
            if (nRateType == 0)
                strRateTypeFilter = " And employees.RateType = 's' ";
            else if (nRateType == 1)
                strRateTypeFilter = " AND employees.RateType = 'h' ";

            SqlCommand cmdSelect = new SqlCommand(" " +
            "SELECT	ePositions.id as EPositionID,ePositions.EmployeeID, " +
            "   Persons.LastName+', '+Persons.FirstName As LastFirst, " +
            "   hrBusinessLevels.Code as BusinessLevelCode, " +
            "   Employees.CompanyCode+' : '+Employees.FileNumber as CoCodeFileNumber, " +
            "   COALESCE(Positions.Code, '') as JobCategory, " +
            "   JobCodeHierachy = '', " +
            "   employees.HireDate,  " +
            "   Positions.Title as OldPositionTitle,  " +
            " " +
            "coalesce((	SELECT NewPositionTitle From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), '') " +
            "   as NewPositionTitle, " +
            " " +
            "(	SELECT Reason From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewReason, " +
            " " +
            "   (	SELECT Top(1) Rate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's') OR (rateTypeCode = 'h'))  " +
            "	ORDER BY EffectiveDate DESC) as OldRate, " +
            " " +
            "   (	SELECT Top(1) AnnualSalary  From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's') OR (rateTypeCode = 'h'))  " +
            "	ORDER BY EffectiveDate DESC) as OldAnnualSalary, " +
             "   (	SELECT Top(1) HoursPerPayPeriod From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's') OR (rateTypeCode = 'h')) " +
            "	ORDER BY EffectiveDate DESC) as OldHoursPerPayPeriod, " +
            " " +
            "   (	SELECT Top(1) effectiveDate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's') OR (rateTypeCode = 'h')) " +
            "	ORDER BY EffectiveDate DESC) as OldEffectiveDate, " +
            " " +
            "(	SELECT Top(1) increaseAmount From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's') OR (rateTypeCode = 'h')) " +
            "	ORDER BY EffectiveDate DESC) as OldIncreaseAmount, " +
            " " +
            "(	SELECT Top(1) increasePercent From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate > @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate > @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldIncreasePercent, " +
            " " +
            "(	SELECT Top(1) rateTypeCode From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's') OR (rateTypeCode = 'h'))  " +
            "	ORDER BY EffectiveDate DESC) as OldRateTypeCode, " +
            " " +
            "(	SELECT IncreaseAmount From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewIncreaseAmount, " +
            " " +
            "(	SELECT IncreasePercent From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewIncreasePercent, " +
            " " +
                    " " +
            "(	SELECT BonusIncreaseAmount From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewBonusIncreaseAmount, " +
            " " +
            "(	SELECT BonusIncreasePercent From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewBonusIncreasePercent, " +
            " " +
            "(	SELECT Rate From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewRate, " +
            " " +
            "(	SELECT Promotion From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as Promotion, " +
            " " +
            "(	SELECT MarketAdjustment From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as MarketAdjustment, " +
            " " +
            "'NewBiWeeklyRate' = " +
            "	CASE " +
            "		WHEN (	SELECT AnnualSalary  " +
            "				From ePositionSalaryHistory  " +
            "				WHERE ePositionID = ePositions.id " +
            "					AND effectiveDate = @EffectiveDateSalary " +
            "					AND AnnualSalary IS NOT NULL) IS NOT NULL		THEN 	(SELECT AnnualSalary/26 From ePositionSalaryHistory  " +
            "																										WHERE ePositionID = ePositions.id   " +
            "																											AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "		ELSE NULL " +
            "	END, " +
            " " +
            "(SELECT AnnualSalary From ePositionSalaryHistory  " +
            "			WHERE ePositionID = ePositions.id  " +
            "				AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)))  " +
            "   as NewAnnualSalary,  " +
            " " +
            "(	SELECT updatedBy From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as UpdatedBy, " +
            " " +
            "   coalesce((	SELECT editLocked From ePositionSalaryHistory  " +
            "           	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as editLocked, " +
            " " +
            "coalesce((	SELECT increaseApproved From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as IncreaseApproved " +
            " " +
            " " +
            "FROM ePositions   " +
            "   INNER JOIN Employees " +
            "       ON Employees.ID = ePositions.EmployeeID  " +
            "   INNER JOIN Persons   " +
            "       ON Persons.ID = Employees.PersonID " +
            "   INNER JOIN Positions  " +
            "       ON Positions.id = ePositions.PositionID  " +
            "   INNER JOIN hrBusinessLevels  " +
            "       ON hrBusinessLevels.ID = Positions.BusinessLevelID  " +
            "WHERE ePositions.PositionID = @PositionID AND ePositions.primaryPosition=1 AND employees.employmentStatusID = 1 " +
                //"   AND (employees.RateType = 'h' or employees.RateType = 's') " +
            " " + strRateTypeFilter +
            "ORDER BY LastFirst "
            , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateHourly", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateSalary", SqlDbType.DateTime));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@EffectiveDateHourly"].Value = dateEffectiveHourly;
            cmdSelect.Parameters["@EffectiveDateSalary"].Value = dateEffectiveSalary;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }
        public DataTable GetAllPrimaryRangeResources(int nPositionID, DateTime dateEffectiveSalary, DateTime dateEffectiveHourly, int nRateType)
        {
            string strRateTypeFilter = "";
            if (nRateType == 0)
                strRateTypeFilter = " And employees.RateType = 's' ";
            else if (nRateType == 1)
                strRateTypeFilter = " AND employees.RateType = 'h' ";

            SqlCommand cmdSelect = new SqlCommand(" " +
            "SELECT	ePositions.id as EPositionID,ePositions.PositionID as PositionID, ePositions.EmployeeID, " +
            "   fillerOne = '    ', fillerTwo = '     ', "+
            "   Persons.LastName+', '+Persons.FirstName As LastFirst, " +
            "   hrBusinessLevels.Code as BusinessLevelCode, " +
            "   Employees.CompanyCode+' : '+Employees.FileNumber as CoCodeFileNumber, " +
            "   COALESCE(Positions.Code, '') as JobCategory, " +
            "   JobCodeHierachy = '', " +
            "   employees.HireDate,  " +
            "   Positions.Title as OldPositionTitle,  " +
            "   RIGHT(Positions.title, LEN(Positions.title)-Convert(TinyInt,CHARINDEX(':', Positions.title))-1) as getTitle," +
            "   UploadSalaryPlaning.*, "+
            "   '_PerformanceRating'=case when UploadSalaryPlaning.PerformanceRating is null then  '0' ELSE UploadSalaryPlaning.PerformanceRating end,  " +
            "   PerformanceText = case "+
            "                       when UploadSalaryPlaning.PerformanceRating is null then  'Too New' "+
            "                       when UploadSalaryPlaning.PerformanceRating = '0' then  'Too New' "+
            "                       when UploadSalaryPlaning.PerformanceRating = '1' then  'Unsatisfactory' " +
            "                       when UploadSalaryPlaning.PerformanceRating = '2' then  'Needs Improvement' " +
            "                       when UploadSalaryPlaning.PerformanceRating = '3' then  'Fully Successful' " +
            "                       when UploadSalaryPlaning.PerformanceRating = '4' then  'Superior' " +
            "                       when UploadSalaryPlaning.PerformanceRating = '5' then  'Distinguished' " +
            "                       else '' "+
            "                       end, "+
            " " +
            "coalesce((	SELECT NewPositionTitle From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), '') " +
            "   as NewPositionTitle, " +
            " " +
            "   'NewReason'=CASE WHEN (	SELECT PerfomanceScore From ePositionSalaryHistory 	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateHourly) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) is null  " +
            "   THEN  " +
            "       0  " +
            "   ELSE  " +
            "   (	SELECT PerfomanceScore From ePositionSalaryHistory 	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateHourly) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) END, " +

            " " +
            "   (	SELECT Top(1) Rate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as OldRate, " +
            " " +
            "   (	SELECT Top(1) AnnualSalary  From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as OldAnnualSalary, " +
            " " +
            "   (	SELECT Top(1) HoursPerPayPeriod From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldHoursPerPayPeriod, " +
            " " +
            "   (	SELECT Top(1) effectiveDate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldEffectiveDate, " +
            " " +
            "(	SELECT Top(1) increaseAmount From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldIncreaseAmount, " +
            " " +
            "(	SELECT Top(1) increasePercent From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldIncreasePercent, " +
            " " +
            "(	SELECT Top(1) rateTypeCode From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as OldRateTypeCode, " +
            " " +
            "(	SELECT IncreaseAmount From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewIncreaseAmount, " +
            " " +
            "(	SELECT IncreasePercent From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewIncreasePercent, " +
            " " +
            " " +
            "(	SELECT BonusIncreaseAmount From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewBonusIncreaseAmount, " +
            " " +
            "(	SELECT BonusIncreasePercent From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewBonusIncreasePercent, " +
            " " +
            "(	SELECT Rate From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewRate, " +
            " " +
            "'NewBiWeeklyRate' = " +
            "	CASE " +
            "		WHEN (	SELECT Top(1) AnnualSalary  " +
            "				From ePositionSalaryHistory  " +
            "				WHERE ePositionID = ePositions.id " +
            "					AND effectiveDate = @EffectiveDateSalary " +
            "					AND AnnualSalary IS NOT NULL) IS NOT NULL		THEN 	(SELECT AnnualSalary/26 From ePositionSalaryHistory  " +
            "																										WHERE ePositionID = ePositions.id   " +
            "																											AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "		ELSE NULL " +
            "	END, " +
            " " +
            "(SELECT AnnualSalary From ePositionSalaryHistory  " +
            "			WHERE ePositionID = ePositions.id  " +
            "				AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)))  " +
            "   as NewAnnualSalary,  " +
            " " +
            "(	SELECT updatedBy From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as UpdatedBy, " +
            " " +
            "   coalesce((	SELECT editLocked From ePositionSalaryHistory  " +
            "           	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as editLocked  " +
            " " +
            ",coalesce((	SELECT increaseApproved From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as IncreaseApproved, " +
 
            "	(	SELECT PoolAmount From PositionPool	WHERE PositionID = ePositions.PositionID AND (PoolYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBasicPayTotal,   " +
            "	(	SELECT PoolTempAmt From PositionPool	WHERE PositionID = ePositions.PositionID AND (PoolYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBasicPayAval, " +

            "	(	SELECT BonusAmount From PositionBonusAllocation	WHERE PositionID = ePositions.PositionID AND (BonusYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBonusPayTotal,   " +
            "	(	SELECT BonusTempAmt From PositionBonusAllocation	WHERE PositionID = ePositions.PositionID AND (BonusYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBonusPayAval, " +

            "	(	SELECT EquityAmount From PositionEquity	WHERE PositionID = ePositions.PositionID AND (EYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidEquityPayTotal,   " +
            "	(	SELECT EquityTempAmt From PositionEquity WHERE PositionID = ePositions.PositionID AND (EYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidEquityPayAval, " +

            "	(	SELECT BasicIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BasicIncreaseAmount,    " +
            "	(	SELECT BasicIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BasicIncreasePercent,    " +

            "	(	SELECT BonusAllowIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BonusAllowIncreaseAmount,    " +
            "	(	SELECT BonusAllowIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BonusAllowIncreasePercent,    " +

            "	(	SELECT EquityIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as EquityIncreaseAmount,    " +
            "	(	SELECT EquityIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as EquityIncreasePercent    " +

            " " +
            " " +
            "FROM ePositions   " +
            "   INNER JOIN Employees " +
            "       ON Employees.ID = ePositions.EmployeeID  " +
            "   INNER JOIN Persons   " +
            "       ON Persons.ID = Employees.PersonID " +
            "   INNER JOIN Positions  " +
            "       ON Positions.id = ePositions.PositionID  " +
            "   INNER JOIN hrBusinessLevels  " +
            "       ON hrBusinessLevels.ID = Positions.BusinessLevelID  " +
            "   Left JOIN UploadSalaryPlaning " +
            "       ON UploadSalaryPlaning.CompanyCode = Employees.CompanyCode and  UploadSalaryPlaning.Filenumber=Employees.Filenumber " +
            "WHERE ePositions.PositionID = @PositionID AND ePositions.primaryPosition=1 AND employees.employmentStatusID = 1 " +
            " " + strRateTypeFilter +
            "ORDER BY LastFirst "
            , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateHourly", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateSalary", SqlDbType.DateTime));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@EffectiveDateHourly"].Value = dateEffectiveHourly;
            cmdSelect.Parameters["@EffectiveDateSalary"].Value = dateEffectiveSalary;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        //public DataTable GetEmployeeSalaryInfoRangeResourcesForReport(int nEmployeeId, DateTime dateEffective )
        //{
        //    string strRateTypeFilter = "";
        //    //if (nRateType == 0)
        //    //    strRateTypeFilter = " And employees.RateType = 's' ";
        //    //else if (nRateType == 1)
        //    //    strRateTypeFilter = " AND employees.RateType = 'h' ";

        //    SqlCommand cmdSelect = new SqlCommand(" " +
        //    "SELECT	ePositions.id as EPositionID,ePositions.PositionID as PositionID, ePositions.EmployeeID, " +
        //    "   fillerOne = '    ', fillerTwo = '     ', " +
        //    "   Persons.LastName+', '+Persons.FirstName As LastFirst, " +
        //    "   hrBusinessLevels.Code as BusinessLevelCode, " +
        //    "   Employees.CompanyCode+' : '+Employees.FileNumber as CoCodeFileNumber, " +
        //    "   COALESCE(Positions.Code, '') as JobCategory, " +
        //    "   JobCodeHierachy = '', " +
        //    "   employees.HireDate,  " +
        //    "   Positions.Title as OldPositionTitle,  " +
        //    "   RIGHT(Positions.title, LEN(Positions.title)-Convert(TinyInt,CHARINDEX(':', Positions.title))-1) as getTitle," +
        //    "   UploadSalaryPlaning.*, " +
        //    "   '_PerformanceRating'=case when UploadSalaryPlaning.PerformanceRating is null then  '0' ELSE UploadSalaryPlaning.PerformanceRating end,  " +
        //    "   PerformanceText = case " +
        //    "                       when UploadSalaryPlaning.PerformanceRating is null then  'Too New' " +
        //    "                       when UploadSalaryPlaning.PerformanceRating = '0' then  'Too New' " +
        //    "                       when UploadSalaryPlaning.PerformanceRating = '1' then  'Unsatisfactory' " +
        //    "                       when UploadSalaryPlaning.PerformanceRating = '2' then  'Needs Improvement' " +
        //    "                       when UploadSalaryPlaning.PerformanceRating = '3' then  'Fully Successful' " +
        //    "                       when UploadSalaryPlaning.PerformanceRating = '4' then  'Superior' " +
        //    "                       when UploadSalaryPlaning.PerformanceRating = '5' then  'Distinguished' " +
        //    "                       else '' " +
        //    "                       end, " +
        //    " " +
        //    "coalesce((	SELECT NewPositionTitle From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id  " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), '') " +
        //    "   as NewPositionTitle, " +
        //    " " +
        //    "   'NewReason'=CASE WHEN (	SELECT PerfomanceScore From ePositionSalaryHistory 	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateHourly) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) is null  " +
        //    "   THEN  " +
        //    "       0  " +
        //    "   ELSE  " +
        //    "   (	SELECT PerfomanceScore From ePositionSalaryHistory 	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateHourly) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) END, " +

        //    " " +
        //    "   (	SELECT Top(1) Rate From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id  " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
        //    "	ORDER BY EffectiveDate DESC) as OldRate, " +
        //    " " +
        //    "   (	SELECT Top(1) AnnualSalary  From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id  " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
        //    "	ORDER BY EffectiveDate DESC) as OldAnnualSalary, " +
        //    " " +
        //    "   (	SELECT Top(1) HoursPerPayPeriod From ePositionSalaryHistory " +
        //    "	WHERE ePositionID = ePositions.id " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
        //    "	ORDER BY EffectiveDate DESC) as OldHoursPerPayPeriod, " +
        //    " " +
        //    "   (	SELECT Top(1) effectiveDate From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id   " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
        //    "	ORDER BY EffectiveDate DESC) as OldEffectiveDate, " +
        //    " " +
        //    "(	SELECT Top(1) increaseAmount From ePositionSalaryHistory " +
        //    "	WHERE ePositionID = ePositions.id   " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
        //    "	ORDER BY EffectiveDate DESC) as OldIncreaseAmount, " +
        //    " " +
        //    "(	SELECT Top(1) increasePercent From ePositionSalaryHistory " +
        //    "	WHERE ePositionID = ePositions.id   " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
        //    "	ORDER BY EffectiveDate DESC) as OldIncreasePercent, " +
        //    " " +
        //    "(	SELECT Top(1) rateTypeCode From ePositionSalaryHistory " +
        //    "	WHERE ePositionID = ePositions.id " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
        //    "	ORDER BY EffectiveDate DESC) as OldRateTypeCode, " +
        //    " " +
        //    "(	SELECT IncreaseAmount From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
        //    "   as NewIncreaseAmount, " +
        //    " " +
        //    "(	SELECT IncreasePercent From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id  " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
        //    "   as NewIncreasePercent, " +
        //    " " +
        //    " " +
        //    "(	SELECT BonusIncreaseAmount From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
        //    "   as NewBonusIncreaseAmount, " +
        //    " " +
        //    "(	SELECT BonusIncreasePercent From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id  " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
        //    "   as NewBonusIncreasePercent, " +
        //    " " +
        //    "(	SELECT Rate From ePositionSalaryHistory " +
        //    "	WHERE ePositionID = ePositions.id  " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
        //    "   as NewRate, " +
        //    " " +
        //    "'NewBiWeeklyRate' = " +
        //    "	CASE " +
        //    "		WHEN (	SELECT Top(1) AnnualSalary  " +
        //    "				From ePositionSalaryHistory  " +
        //    "				WHERE ePositionID = ePositions.id " +
        //    "					AND effectiveDate = @EffectiveDateSalary " +
        //    "					AND AnnualSalary IS NOT NULL) IS NOT NULL		THEN 	(SELECT AnnualSalary/26 From ePositionSalaryHistory  " +
        //    "																										WHERE ePositionID = ePositions.id   " +
        //    "																											AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
        //    "		ELSE NULL " +
        //    "	END, " +
        //    " " +
        //    "(SELECT AnnualSalary From ePositionSalaryHistory  " +
        //    "			WHERE ePositionID = ePositions.id  " +
        //    "				AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)))  " +
        //    "   as NewAnnualSalary,  " +
        //    " " +
        //    "(	SELECT updatedBy From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id  " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as UpdatedBy, " +
        //    " " +
        //    "   coalesce((	SELECT editLocked From ePositionSalaryHistory  " +
        //    "           	WHERE ePositionID = ePositions.id  " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as editLocked  " +
        //    " " +
        //    ",coalesce((	SELECT increaseApproved From ePositionSalaryHistory  " +
        //    "	WHERE ePositionID = ePositions.id  " +
        //    "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as IncreaseApproved, " +

        //    "	(	SELECT PoolAmount From PositionPool	WHERE PositionID = ePositions.PositionID AND (PoolYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBasicPayTotal,   " +
        //    "	(	SELECT PoolTempAmt From PositionPool	WHERE PositionID = ePositions.PositionID AND (PoolYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBasicPayAval, " +

        //    "	(	SELECT BonusAmount From PositionBonusAllocation	WHERE PositionID = ePositions.PositionID AND (BonusYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBonusPayTotal,   " +
        //    "	(	SELECT BonusTempAmt From PositionBonusAllocation	WHERE PositionID = ePositions.PositionID AND (BonusYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBonusPayAval, " +

        //    "	(	SELECT EquityAmount From PositionEquity	WHERE PositionID = ePositions.PositionID AND (EYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidEquityPayTotal,   " +
        //    "	(	SELECT EquityTempAmt From PositionEquity WHERE PositionID = ePositions.PositionID AND (EYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidEquityPayAval, " +

        //    "	(	SELECT BasicIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BasicIncreaseAmount,    " +
        //    "	(	SELECT BasicIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BasicIncreasePercent,    " +

        //    "	(	SELECT BonusAllowIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BonusAllowIncreaseAmount,    " +
        //    "	(	SELECT BonusAllowIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BonusAllowIncreasePercent,    " +

        //    "	(	SELECT EquityIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as EquityIncreaseAmount,    " +
        //    "	(	SELECT EquityIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as EquityIncreasePercent    " +

        //    " " +
        //    " " +
        //    "FROM ePositions   " +
        //    "   INNER JOIN Employees " +
        //    "       ON Employees.ID = ePositions.EmployeeID  " +
        //    "   INNER JOIN Persons   " +
        //    "       ON Persons.ID = Employees.PersonID " +
        //    "   INNER JOIN Positions  " +
        //    "       ON Positions.id = ePositions.PositionID  " +
        //    "   INNER JOIN hrBusinessLevels  " +
        //    "       ON hrBusinessLevels.ID = Positions.BusinessLevelID  " +
        //    "   Left JOIN UploadSalaryPlaning " +
        //    "       ON UploadSalaryPlaning.CompanyCode = Employees.CompanyCode and  UploadSalaryPlaning.Filenumber=Employees.Filenumber " +
        //    "WHERE ePositions.PositionID = @PositionID AND ePositions.primaryPosition=1 AND employees.employmentStatusID = 1 " +
        //    " " + strRateTypeFilter +
        //    "ORDER BY LastFirst "
        //    , m_Connection);
        //    cmdSelect.CommandType = CommandType.Text;

        //    cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
        //    cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateHourly", SqlDbType.DateTime));
        //    cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateSalary", SqlDbType.DateTime));

        //    cmdSelect.Parameters["@PositionID"].Value = nPositionID;
        //    cmdSelect.Parameters["@EffectiveDateHourly"].Value = dateEffectiveHourly;
        //    cmdSelect.Parameters["@EffectiveDateSalary"].Value = dateEffectiveSalary;

        //    SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

        //    DataSet ds = new DataSet();
        //    da.Fill(ds, "ePositions");

        //    return ds.Tables["ePositions"];
        //}
        public DataTable SpringFCollExport(string CoCode, ref String ErrMsg)
        {

            string tsql;

            tsql = "";
            tsql = tsql + "SELECT dbo.ePositions.CompanyCode, dbo.ePositions.FileNumber, dbo.Persons.lastname, dbo.Persons.firstname, dbo.Persons.SSN, " + Environment.NewLine;
            tsql = tsql + "                      dbo.personAddresses.addressLineOne, dbo.personAddresses.addressLineTwo, dbo.personAddresses.city, dbo.ddlStates.code, " + Environment.NewLine;
            tsql = tsql + "                      dbo.personAddresses.zipCode, dbo.ddlEmploymentStatuses.code AS [Employee Status], " + Environment.NewLine;
            tsql = tsql + "                      CASE WHEN dbo.Persons.gender = 1 THEN 'F' ELSE 'M' END AS Gender, dbo.ddlActualMaritalStatuses.code AS MaritalStatus, " + Environment.NewLine;
            tsql = tsql + "                      dbo.Employees.FedExemptions, dbo.Employees.WorkedStateTaxCode, dbo.Employees.SUI, NULL AS phone, " + Environment.NewLine;
            tsql = tsql + "                      dbo.ddlEmployeeTypes.code AS [Employee Type], dbo.Positions.FLSA, dbo.Employees.HomeDept, dbo.Employees.payFrequencyCode, " + Environment.NewLine;
            tsql = tsql + "                      ePositionSalaryHistory_1.Rate AS [Rate 1 Amount], NULL AS [Rate 2 Amount], NULL AS [Rate 3 Amount], " + Environment.NewLine;
            tsql = tsql + "                      ePositionSalaryHistory_1.RateTypeCode AS [Rate Type], ePositionSalaryHistory_1.DaysPerPayPeriod AS [Standard Hours], NULL " + Environment.NewLine;
            tsql = tsql + "                      AS [Primary Rate Effective Date], CONVERT(varchar(10), dbo.Employees.hireDate, 101) AS Expr1, CONVERT(varchar(10), dbo.Persons.DOB, 101) " + Environment.NewLine;
            tsql = tsql + "                      AS Expr2, dbo.Employees.WorkedLocalTaxCode, NULL AS [Do not calculate local tax]" + Environment.NewLine;
            tsql = tsql + "FROM         dbo.ePositionSalaryHistory AS ePositionSalaryHistory_1 RIGHT OUTER JOIN" + Environment.NewLine;
            tsql = tsql + "                          (SELECT     MAX(ID) AS id, ePositionID" + Environment.NewLine;
            tsql = tsql + "                            FROM          dbo.ePositionSalaryHistory" + Environment.NewLine;
            tsql = tsql + "                            GROUP BY ePositionID) AS salhist ON ePositionSalaryHistory_1.ID = salhist.id RIGHT OUTER JOIN" + Environment.NewLine;
            tsql = tsql + "                      dbo.Persons INNER JOIN" + Environment.NewLine;
            tsql = tsql + "                      dbo.Employees ON dbo.Persons.id = dbo.Employees.personID LEFT OUTER JOIN" + Environment.NewLine;
            tsql = tsql + "                      dbo.ddlEmploymentStatuses ON dbo.Employees.employmentStatusID = dbo.ddlEmploymentStatuses.id LEFT OUTER JOIN" + Environment.NewLine;
            tsql = tsql + "                      dbo.Positions INNER JOIN" + Environment.NewLine;
            tsql = tsql + "                      dbo.ePositions ON dbo.Positions.id = dbo.ePositions.positionID ON dbo.Employees.id = dbo.ePositions.employeeID LEFT OUTER JOIN" + Environment.NewLine;
            tsql = tsql + "                      dbo.ddlActualMaritalStatuses ON dbo.Employees.MaritalStatusID = dbo.ddlActualMaritalStatuses.id LEFT OUTER JOIN" + Environment.NewLine;
            tsql = tsql + "                      dbo.ddlStates INNER JOIN" + Environment.NewLine;
            tsql = tsql + "                      dbo.personAddresses ON dbo.ddlStates.id = dbo.personAddresses.stateID ON dbo.Persons.id = dbo.personAddresses.personID LEFT OUTER JOIN" + Environment.NewLine;
            tsql = tsql + "                      dbo.ddlEmployeeTypes ON dbo.Employees.FedExemptions = dbo.ddlEmployeeTypes.id ON salhist.ePositionID = dbo.ePositions.id" + Environment.NewLine;
            tsql = tsql + "WHERE     (dbo.ePositions.ExportFlag = 1) AND (dbo.ePositions.CompanyCode = '?CoCode?')" + Environment.NewLine;
            tsql = tsql + "ORDER BY dbo.Persons.lastname, dbo.Persons.firstname" + Environment.NewLine;
            tsql = tsql.Replace("?CoCode?", CoCode);

            SqlCommand cmdSelect = new SqlCommand(tsql, m_Connection);
            cmdSelect.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositionsExport");

            return ds.Tables["ePositionsExport"];

        }

        public void DontExport(int nPositionID, ref string ErrMsg)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "Update ePositions Set ExportFlag = 0 where id = @id";

            cmdUpdate.Parameters.Add("@ID", SqlDbType.Int, 4);
            cmdUpdate.Parameters["@ID"].Value = nPositionID;

            try
            {
                m_Connection.Open();
                cmdUpdate.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                ErrMsg = err.Message;
            }
            finally
            {
                m_Connection.Close();
            }
        }

        public DataTable GetAllPrimaryRangeResourcesDPR(int nPositionID, DateTime dateEffectiveSalary, DateTime dateEffectiveHourly, int nRateType)
        {
            string strRateTypeFilter = "";
            if (nRateType == 0)
                strRateTypeFilter = " And employees.RateType = 's' ";
            else if (nRateType == 1)
                strRateTypeFilter = " AND employees.RateType = 'h' ";

            SqlCommand cmdSelect = new SqlCommand(" " +
            "SELECT	ePositions.id as EPositionID,ePositions.PositionID as PositionID, ePositions.EmployeeID, " +
            "   Persons.LastName+', '+Persons.FirstName As LastFirst, " +
            "   hrBusinessLevels.Code as BusinessLevelCode, " +
            "   Employees.CompanyCode+' : '+Employees.FileNumber as CoCodeFileNumber, " +
            "   COALESCE(Positions.Code, '') as JobCategory, " +
            "   JobCodeHierachy = '', " +
            "   employees.HireDate,  " +
            "   Positions.Title as OldPositionTitle,  " +
            "   UploadSalaryPlaning.*,'_PerformanceRating'=case when UploadSalaryPlaning.PerformanceRating is null then  '0' ELSE UploadSalaryPlaning.PerformanceRating end,  " +
            " " +
            "coalesce((	SELECT NewPositionTitle From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), '') " +
            "   as NewPositionTitle, " +
            " " +
                //"(	SELECT Reason From ePositionSalaryHistory " +
                //"	WHERE ePositionID = ePositions.id  " +
                //"		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
                //"   as NewReason, " +
            "   'NewReason'=CASE WHEN (	SELECT PerfomanceScore From ePositionSalaryHistory 	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateHourly) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) is null  " +
            "   THEN  " +
            "       0  " +
            "   ELSE  " +
            "   (	SELECT PerfomanceScore From ePositionSalaryHistory 	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateHourly) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) END, " +

            " " +
            "   (	SELECT Top(1) Rate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as OldRate, " +
            " " +
            "   (	SELECT Top(1) AnnualSalary  From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as OldAnnualSalary, " +
            " " +
            "   (	SELECT Top(1) HoursPerPayPeriod From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldHoursPerPayPeriod, " +
            " " +
            "   (	SELECT Top(1) effectiveDate From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldEffectiveDate, " +
            " " +
            "(	SELECT Top(1) increaseAmount From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldIncreaseAmount, " +
            " " +
            "(	SELECT Top(1) increasePercent From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id   " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly)) " +
            "	ORDER BY EffectiveDate DESC) as OldIncreasePercent, " +
            " " +
            "(	SELECT Top(1) rateTypeCode From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate < @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate < @EffectiveDateHourly))  " +
            "	ORDER BY EffectiveDate DESC) as OldRateTypeCode, " +
            " " +
            "(	SELECT IncreaseAmount From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewIncreaseAmount, " +
            " " +
            "(	SELECT IncreasePercent From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewIncreasePercent, " +
            " " +
                    " " +
            "(	SELECT BonusIncreaseAmount From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewBonusIncreaseAmount, " +
            " " +
            "(	SELECT BonusIncreasePercent From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewBonusIncreasePercent, " +
            " " +
            "(	SELECT Rate From ePositionSalaryHistory " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "   as NewRate, " +
            " " +
            "'NewBiWeeklyRate' = " +
            "	CASE " +
            "		WHEN (	SELECT Top(1) AnnualSalary  " +
            "				From ePositionSalaryHistory  " +
            "				WHERE ePositionID = ePositions.id " +
            "					AND effectiveDate = @EffectiveDateSalary " +
            "					AND AnnualSalary IS NOT NULL) IS NOT NULL		THEN 	(SELECT AnnualSalary/26 From ePositionSalaryHistory  " +
            "																										WHERE ePositionID = ePositions.id   " +
            "																											AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) " +
            "		ELSE NULL " +
            "	END, " +
            " " +
            "(SELECT AnnualSalary From ePositionSalaryHistory  " +
            "			WHERE ePositionID = ePositions.id  " +
            "				AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)))  " +
            "   as NewAnnualSalary,  " +
            " " +
            "(	SELECT updatedBy From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly))) as UpdatedBy, " +
            " " +
            "   coalesce((	SELECT editLocked From ePositionSalaryHistory  " +
            "           	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as editLocked  " +
            " " +
            ",coalesce((	SELECT increaseApproved From ePositionSalaryHistory  " +
            "	WHERE ePositionID = ePositions.id  " +
            "		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate = @EffectiveDateHourly)) ), 'false') as IncreaseApproved, " +
                //"   ,(SELECT Top 1 statusClassification FROM employeeStatusMMC WHERE employeeStatusMMC.CompanyCode = Employees.CompanyCode "+
                //"        and  employeeStatusMMC.FileNumber = Employees.FileNumber ) as StatusClassification "+ 


            "	(	SELECT PoolAmount From PositionPool	WHERE PositionID = ePositions.PositionID AND (PoolYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBasicPayTotal,   " +
            "	(	SELECT PoolTempAmt From PositionPool	WHERE PositionID = ePositions.PositionID AND (PoolYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBasicPayAval, " +

            "	(	SELECT BonusAmount From PositionBonusAllocation	WHERE PositionID = ePositions.PositionID AND (BonusYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBonusPayTotal,   " +
            "	(	SELECT BonusTempAmt From PositionBonusAllocation	WHERE PositionID = ePositions.PositionID AND (BonusYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidBonusPayAval, " +

            "	(	SELECT EquityAmount From PositionEquity	WHERE PositionID = ePositions.PositionID AND (EYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidEquityPayTotal,   " +
            "	(	SELECT EquityTempAmt From PositionEquity WHERE PositionID = ePositions.PositionID AND (EYear = DATENAME(yyyy, @EffectiveDateSalary)))    as HidEquityPayAval, " +

            "	(	SELECT BasicIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BasicIncreaseAmount,    " +
            "	(	SELECT BasicIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BasicIncreasePercent,    " +

            "	(	SELECT BonusAllowIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BonusAllowIncreaseAmount,    " +
            "	(	SELECT BonusAllowIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as BonusAllowIncreasePercent,    " +

            "	(	SELECT EquityIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as EquityIncreaseAmount,    " +
            "	(	SELECT EquityIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as EquityIncreasePercent    " +

           // "	(	SELECT EquityAllowIncreaseAmount From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as EquityAllowIncreaseAmount,    " +
                // "	(	SELECT EquityAllowIncreasePercent From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as EquityAllowIncreasePercent    " +

            //"	(	SELECT PerfomanceScore From ePositionSalaryHistory  	WHERE ePositionID = ePositions.id  		AND ((rateTypeCode = 's' AND effectiveDate = @EffectiveDateSalary) OR (rateTypeCode = 'h' AND effectiveDate =  @EffectiveDateHourly)))    as PerfomanceScore    " +


            " " +
            " " +
            "FROM ePositions   " +
            "   INNER JOIN Employees " +
            "       ON Employees.ID = ePositions.EmployeeID  " +
            "   INNER JOIN Persons   " +
            "       ON Persons.ID = Employees.PersonID " +
            "   INNER JOIN Positions  " +
            "       ON Positions.id = ePositions.PositionID  " +
            "   INNER JOIN hrBusinessLevels  " +
            "       ON hrBusinessLevels.ID = Positions.BusinessLevelID  " +
            //"   LEFT JOIN UploadSalaryPlaning " +
            //"       ON UploadSalaryPlaning.Employeeid = Employees.ID  " +
            "	LEFT JOIN UploadSalaryPlaning on UploadSalaryPlaning.companycode = employees.companycode "+
            "       and UploadSalaryPlaning.filenumber = employees.filenumber "+

            "WHERE ePositions.PositionID = @PositionID AND ePositions.primaryPosition=1 AND employees.employmentStatusID = 1 " +
            " " + strRateTypeFilter +
            "ORDER BY LastFirst "
            , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateHourly", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@EffectiveDateSalary", SqlDbType.DateTime));

            cmdSelect.Parameters["@PositionID"].Value = nPositionID;
            cmdSelect.Parameters["@EffectiveDateHourly"].Value = dateEffectiveHourly;
            cmdSelect.Parameters["@EffectiveDateSalary"].Value = dateEffectiveSalary;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public int InsertForDidlake(int nEmployeeID, int nPositionID, int nPosCatID, int nPosTypID, bool bIsPrimaryPosition, string strPayFreqCode,
                                DateTime dateStart)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO ePositions " +
                "(EmployeeID, PositionID, PositionCategoryID, positionTypeID, PrimaryPosition, PayFrequencyCode, startDate) " +
                "VALUES(@EmployeeID, @PositionID, @PositionCategoryID, @positionTypeID, @PrimaryPosition, @PayFrequencyCode, @StartDate)" +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add("@EmployeeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PositionCategoryID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@positionTypeID", SqlDbType.Int, 4);
            cmdInsert.Parameters.Add("@PrimaryPosition", SqlDbType.Bit);
            cmdInsert.Parameters.Add("@PayFrequencyCode", SqlDbType.VarChar, 10);
            cmdInsert.Parameters.Add("@StartDate", SqlDbType.DateTime);


            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdInsert.Parameters["@PositionID"].Value = nPositionID;
            cmdInsert.Parameters["@PositionCategoryID"].Value = nPosCatID;
            cmdInsert.Parameters["@positionTypeID"].Value = nPosTypID;
            cmdInsert.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;

            if (strPayFreqCode.ToUpper() == "B" || strPayFreqCode.ToUpper() == "W" || strPayFreqCode.ToUpper() == "M")
                cmdInsert.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            else
                cmdInsert.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;

            cmdInsert.Parameters["@StartDate"].Value = dateStart;

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

        public void UpdateForDidlake(int nID, int nEmployeeID, int nPositionID, int nPosCatID, int nPosTypID, bool bIsPrimaryPosition,
                                    string strPayFreqCode, DateTime dateStart)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET EmployeeID = @EmployeeID," +
                "PositionID = @PositionID, " +
                "PositionCategoryID = @PositionCategoryID, " +
                "positionTypeID = @positionTypeID, " +
                "PrimaryPosition = @PrimaryPosition, " +
                "PayFrequencyCode = @PayFrequencyCode, " +
                "StartDate = @StartDate " +
                "WHERE ID = @ID";
            
            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PositionCategoryID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@positionTypeID", SqlDbType.Int, 4));
            cmdUpdate.Parameters.Add(new SqlParameter("@PrimaryPosition", SqlDbType.Bit));
            cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@EmployeeID"].Value = nEmployeeID;
            cmdUpdate.Parameters["@PositionID"].Value = nPositionID;
            cmdUpdate.Parameters["@PositionCategoryID"].Value = nPosCatID;
            cmdUpdate.Parameters["@positionTypeID"].Value = nPosTypID;
            cmdUpdate.Parameters["@PrimaryPosition"].Value = bIsPrimaryPosition;

            if (strPayFreqCode.ToUpper() == "B" || strPayFreqCode.ToUpper() == "W" || strPayFreqCode.ToUpper() == "M" || strPayFreqCode.ToUpper() == "S")
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFreqCode;
            else
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = System.DBNull.Value;

            cmdUpdate.Parameters["@StartDate"].Value = dateStart;

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

        public void UpdateReportsTo(int nID, int nReportsToID)
        {

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;
            cmdUpdate.CommandText = "UPDATE ePositions " +
                "SET ReportsToID = @ReportsToID " +
                " WHERE ID = @ID";


            cmdUpdate.Parameters.Add(new SqlParameter("@ReportsToID", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@ReportsToID"].Value = nReportsToID.ToString();
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

        public DataTable GetPositionInfoFromWebServiceImportMerrimackCollege()
        {
            SqlCommand cmdSelect = m_Connection.CreateCommand();
            cmdSelect.CommandType = CommandType.StoredProcedure;
            cmdSelect.CommandText = "uspGetWebServiceEmployeePositionInfo";

            //cmdSelect.Parameters.Add(new SqlParameter("@BudgetYear", SqlDbType.Int, 4));
            //cmdSelect.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int, 4));
            //cmdSelect.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            //cmdSelect.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));

            //cmdSelect.Parameters["@BudgetYear"].Value = nBudgetYear;
            //cmdSelect.Parameters["@Month"].Value = nMonthsToSum;
            //cmdSelect.Parameters["@StartDate"].Value = startDate;
            //cmdSelect.Parameters["@EndDate"].Value = endDate;


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public DataTable GetAlaReportPhoenix()
        {
            SqlCommand cmdSelect = m_Connection.CreateCommand();
            cmdSelect.CommandType = CommandType.StoredProcedure;
            cmdSelect.CommandText = "uspAlaReport";

            


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "ePositions");

            return ds.Tables["ePositions"];
        }

        public int? GetMaxAdpPositionNumber(int nEmployeeId)
        {
            int? count = 0;

            try
            {

                m_Connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COALESCE(MAX(AdpPositionNumber), 0) FROM ePositions " +
                      "WHERE EmployeeId = @EmployeeId ", m_Connection);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));

                cmd.Parameters["@EmployeeId"].Value = nEmployeeId;

                count = (int)cmd.ExecuteScalar();

                if (count == null) count = 0;
            }
            catch (Exception err)
            {
                throw;
            }
            finally
            {

                m_Connection.Close();
            }
            return count;
        }

    }
}

