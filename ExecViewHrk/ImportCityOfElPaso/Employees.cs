using System;
using System.Data;
using System.Configuration;
using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ImportAlicePeck
{

    /// <summary>
    /// Summary description for BenefitGroups
    /// </summary>
    public class Employees
    {
        private SqlConnection m_Connection;

        public Employees(SqlConnection connection)
        {
            m_Connection = connection;
        }
        #region employeeUpdate
        public string UpdateEmployee(int EID, int PersonID, string CCode, string FileNum, string Rate, string RateType, string EmpNumber, int EmployeeTypeID,
                 string HRdate, string TRdate, string PayFrequencyCode, string HomeDept, string HomeJob, int MaritalStatus,
                int EmploymentStatusID, string WorkedStateTaxCode, string FedExemptions, string WorkedLocalTaxCode, string SUI, string Hours, string ActssDate, string custArea4, string strUpdateBy)
        {
            String Error = "";
            if (MaritalStatus == 0)
            {
                MaritalStatus = Convert.ToInt32("0");
            }

            int checkInt = 0;

            int employmentCount = 0;
            SqlCommand cmdSelect = new SqlCommand(" " +
            "Select id, hireDate, terminationDate from EMPLOYEES WHERE id <> " + EID + " AND personID = " + PersonID, m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);
            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");
            DataTable dt = ds.Tables["Employees"];
            int index = 0;

            for (index = 0; index <= dt.Rows.Count - 1; index++)
            {
                DateTime hireDate = Convert.ToDateTime(HRdate);
                DateTime CheckHDate = Convert.ToDateTime(dt.Rows[index]["hireDate"]);
                if (hireDate <= CheckHDate)
                {


                    Error = "You cannot enter employment records that occur previous to current employment records.";
                    checkInt = 1;


                }
                else if (ReferenceEquals(dt.Rows[index]["terminationDate"], System.DBNull.Value))
                {
                    Error = "You cannot enter an employment record unless previous records have a termination date.";
                    checkInt = 1;


                }
                else if (CheckHDate <= hireDate & hireDate <= Convert.ToDateTime(dt.Rows[index]["terminationDate"]))
                {
                    Error = "New employment cannot fall between dates of an existing employment, current or planned.";
                    checkInt = 1;
                }


                employmentCount += 1;

            }
            if (checkInt == 0)
            {
                SqlCommand cmdUpdate = m_Connection.CreateCommand();
                cmdUpdate.CommandType = CommandType.Text;

                cmdUpdate.CommandText = "UPDATE Employees" +
                    " SET " +
                    " CompanyCode=@CompanyCode, " +
                    " Rate=@Rate,RateType=@RateType, " +
                    " FileNumber=@FileNumber,employmentNumber=@EmploymentNumber, " +
                    " employmentTypeID=@EmploymentTypeID,hireDate=@HireDate, " +
                    " terminationDate=@TerminationDate,payFrequencyCode=@PayFrequencyCode, " +
                    " HomeJob=@HomeJob,HomeDept=@HomeDept, " +
                    " MaritalStatusID=@MaritalStatusID, " +
                    " employmentStatusID=@EmploymentStatusID,Hours=@Hours, " +
                    " custArea4=@CustArea4,WorkedLocalTaxCode=@WorkedLocalTaxCode, " +
                    " SUI=@SUI,WorkedStateTaxCode=@WorkedStateTaxCode, " +
                    " FedExemptions=@FedExemptions,actualServiceStartDate=@ActualServiceStartDate, " +
                    " enteredBy=@EnteredBy,enteredDate=@EnteredDate " +
                    " WHERE id = @ID";


                cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
                cmdUpdate.Parameters.Add(new SqlParameter("@Rate", SqlDbType.Decimal));
                cmdUpdate.Parameters.Add(new SqlParameter("@RateType", SqlDbType.VarChar, 10));
                cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
                cmdUpdate.Parameters.Add(new SqlParameter("@EmploymentNumber", SqlDbType.TinyInt));
                cmdUpdate.Parameters.Add(new SqlParameter("@EmploymentTypeID", SqlDbType.TinyInt));

                cmdUpdate.Parameters.Add(new SqlParameter("@HireDate", SqlDbType.SmallDateTime));
                cmdUpdate.Parameters.Add(new SqlParameter("@TerminationDate", SqlDbType.SmallDateTime));
                cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
                cmdUpdate.Parameters.Add(new SqlParameter("@HomeJob", SqlDbType.VarChar, 10));
                cmdUpdate.Parameters.Add(new SqlParameter("@HomeDept", SqlDbType.VarChar, 10));
                cmdUpdate.Parameters.Add(new SqlParameter("@MaritalStatusID", SqlDbType.TinyInt));

                cmdUpdate.Parameters.Add(new SqlParameter("@EmploymentStatusID", SqlDbType.TinyInt));
                cmdUpdate.Parameters.Add(new SqlParameter("@Hours", SqlDbType.Decimal));
                cmdUpdate.Parameters.Add(new SqlParameter("@CustArea4", SqlDbType.VarChar, 2));
                cmdUpdate.Parameters.Add(new SqlParameter("@WorkedLocalTaxCode", SqlDbType.VarChar, 2));
                cmdUpdate.Parameters.Add(new SqlParameter("@SUI", SqlDbType.VarChar, 10));
                cmdUpdate.Parameters.Add(new SqlParameter("@WorkedStateTaxCode", SqlDbType.VarChar, 2));
                cmdUpdate.Parameters.Add(new SqlParameter("@FedExemptions", SqlDbType.Char, 2));
                cmdUpdate.Parameters.Add(new SqlParameter("@ActualServiceStartDate", SqlDbType.SmallDateTime));

                cmdUpdate.Parameters.Add(new SqlParameter("@EnteredBy", SqlDbType.VarChar, 50));
                cmdUpdate.Parameters.Add(new SqlParameter("@EnteredDate", SqlDbType.SmallDateTime));
                cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

                cmdUpdate.Parameters["@CompanyCode"].Value = CCode;
                if (Rate != "")
                {
                    cmdUpdate.Parameters["@Rate"].Value = (Convert.ToDecimal(Rate));
                }
                else
                {
                    cmdUpdate.Parameters["@Rate"].Value = DBNull.Value;
                }

                cmdUpdate.Parameters["@RateType"].Value = RateType;
                cmdUpdate.Parameters["@FileNumber"].Value = FileNum;
                cmdUpdate.Parameters["@EmploymentNumber"].Value = (Convert.ToInt16(EmpNumber));
                cmdUpdate.Parameters["@EmploymentTypeID"].Value = (Convert.ToInt16(EmployeeTypeID));

                cmdUpdate.Parameters["@HireDate"].Value = Convert.ToDateTime(HRdate);

                if (TRdate != "")
                {
                    cmdUpdate.Parameters["@TerminationDate"].Value = Convert.ToDateTime(TRdate);
                }
                else
                {
                    cmdUpdate.Parameters["@TerminationDate"].Value = DBNull.Value;
                }
                cmdUpdate.Parameters["@PayFrequencyCode"].Value = PayFrequencyCode;
                cmdUpdate.Parameters["@HomeJob"].Value = HomeJob;
                cmdUpdate.Parameters["@HomeDept"].Value = HomeDept;
                cmdUpdate.Parameters["@MaritalStatusID"].Value = (Convert.ToInt16(MaritalStatus));

                cmdUpdate.Parameters["@EmploymentStatusID"].Value = (Convert.ToInt16(EmploymentStatusID));
                if (Hours != "")
                {
                    cmdUpdate.Parameters["@Hours"].Value = (Convert.ToDecimal(Hours));
                }
                else
                {
                    cmdUpdate.Parameters["@Hours"].Value = DBNull.Value;
                }
                cmdUpdate.Parameters["@CustArea4"].Value = custArea4;
                cmdUpdate.Parameters["@WorkedLocalTaxCode"].Value = WorkedLocalTaxCode;
                cmdUpdate.Parameters["@SUI"].Value = SUI;
                cmdUpdate.Parameters["@WorkedStateTaxCode"].Value = WorkedStateTaxCode;
                cmdUpdate.Parameters["@FedExemptions"].Value = FedExemptions;
                if (ActssDate != "")
                {
                    cmdUpdate.Parameters["@ActualServiceStartDate"].Value = Convert.ToDateTime(ActssDate);
                }
                else
                {
                    cmdUpdate.Parameters["@ActualServiceStartDate"].Value = DBNull.Value;
                }
                cmdUpdate.Parameters["@EnteredBy"].Value = strUpdateBy;
                cmdUpdate.Parameters["@EnteredDate"].Value = DateTime.Now;
                cmdUpdate.Parameters["@ID"].Value = EID;

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
            }
            return Error;
        }


        #endregion

        public string insertEmployee(int personID, DateTime hireDate, string enteredBy, string strCC, string strFN)
        {


            String Error = "";
            int checkInt = 0;
            int employmentCount = 1;
            SqlCommand cmdSelect = new SqlCommand(" " +
            "Select id, hireDate, terminationDate from EMPLOYEES WHERE personID = " + personID, m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);
            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");
            DataTable dt = ds.Tables["Employees"];
            int index = 0;

            for (index = 0; index <= dt.Rows.Count - 1; index++)
            {

                DateTime CheckHDate = Convert.ToDateTime(dt.Rows[index]["hireDate"]);
                if (hireDate <= CheckHDate)
                {


                    Error = "You cannot enter employment records that occur previous to current employment records.";
                    checkInt = 1;


                }
                else if (ReferenceEquals(dt.Rows[index]["terminationDate"], System.DBNull.Value))
                {
                    Error = "You cannot enter an employment record unless previous records have a termination date.";
                    checkInt = 1;


                }
                else if (CheckHDate <= hireDate & hireDate <= Convert.ToDateTime(dt.Rows[index]["terminationDate"]))
                {
                    Error = "New employment cannot fall between dates of an existing employment, current or planned.";
                    checkInt = 1;
                }


                employmentCount += 1;

            }


            if (checkInt == 0)
            {

                SqlCommand CmsInsert = m_Connection.CreateCommand();
                CmsInsert.CommandType = CommandType.Text;
                CmsInsert.CommandText = "INSERT INTO Employees(personID,CompanyCode,FileNumber,employmentNumber,employmentStatusID,hireDate,plannedServiceStartDate," +
                                        "seniorityDate,enteredBy,enteredDate,actualServiceStartDate)" +
                                        "VALUES(@personID,@CompanyCode,@FileNumber,@employmentNumber,@employmentStatusID,@hireDate,@plannedServiceStartDate," +
                                        "@seniorityDate,@enteredBy,@enteredDate,@actualServiceStartDate)";

                CmsInsert.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
                CmsInsert.Parameters.Add(new SqlParameter("@personID", SqlDbType.Int));
                CmsInsert.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
                CmsInsert.Parameters.Add(new SqlParameter("@employmentNumber", SqlDbType.TinyInt));
                CmsInsert.Parameters.Add(new SqlParameter("@employmentStatusID", SqlDbType.TinyInt));
                CmsInsert.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));
                CmsInsert.Parameters.Add(new SqlParameter("@plannedServiceStartDate", SqlDbType.SmallDateTime));
                CmsInsert.Parameters.Add(new SqlParameter("@actualServiceStartDate", SqlDbType.SmallDateTime));
                CmsInsert.Parameters.Add(new SqlParameter("@seniorityDate", SqlDbType.SmallDateTime));
                CmsInsert.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50));
                CmsInsert.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));

                CmsInsert.Parameters["@personID"].Value = personID;
                CmsInsert.Parameters["@employmentStatusID"].Value = 1;
                CmsInsert.Parameters["@employmentNumber"].Value = employmentCount;
                CmsInsert.Parameters["@hireDate"].Value = hireDate;
                CmsInsert.Parameters["@plannedServiceStartDate"].Value = hireDate;
                CmsInsert.Parameters["@actualServiceStartDate"].Value = hireDate;
                CmsInsert.Parameters["@seniorityDate"].Value = hireDate;
                CmsInsert.Parameters["@enteredBy"].Value = enteredBy;
                CmsInsert.Parameters["@enteredDate"].Value = DateTime.Now;


                if (!string.IsNullOrEmpty(strCC))
                {
                    CmsInsert.Parameters["@CompanyCode"].Value = strCC;
                }

                if (!string.IsNullOrEmpty(strFN))
                {
                    CmsInsert.Parameters["@FileNumber"].Value = strFN;

                }

                try
                {
                    m_Connection.Open();
                    CmsInsert.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);

                }
                finally
                {
                    m_Connection.Close();
                }
            }
            return Error;
        }

        public int InsertAvant(int nPersonId, string strCompanyCode, string strFileNumber, string strHomeClientCode, string strHomeClientDescription
                                        , DateTime hireDate, string strEnteredBy
                                        , DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO Employees " +
                "(PersonId, CompanyCode, FileNumber, homeClientCode, homeClientCodeDescription, enteredBy, enteredDate, hireDate) " +
                "VALUES(@PersonId, @CompanyCode, @FileNumber,  @homeClientCode, @homeClientCodeDescription, @enteredBy, @enteredDate, @hireDate) " +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add(new SqlParameter("@PersonId", SqlDbType.Int, 4, "@PersonId"));
            cmdInsert.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdInsert.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50, "@FileNumber"));
            cmdInsert.Parameters.Add(new SqlParameter("@homeClientCode", SqlDbType.VarChar, 50, "@homeClientCode"));
            cmdInsert.Parameters.Add(new SqlParameter("@homeClientCodeDescription", SqlDbType.VarChar, 100, "@homeClientCodeDescription"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@PersonId"].Value = nPersonId;
            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;
            cmdInsert.Parameters["@homeClientCode"].Value = strHomeClientCode;
            cmdInsert.Parameters["@homeClientCodeDescription"].Value = strHomeClientDescription;

            cmdInsert.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@enteredDate"].Value = dateEnteredDate;
            cmdInsert.Parameters["@hireDate"].Value = hireDate;

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

        public int InsertAvant(int nPersonId, string strCompanyCode, string strFileNumber
                                           , DateTime hireDate, string strEnteredBy
                                           , DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO Employees " +
                "(PersonId, CompanyCode, FileNumber, enteredBy, enteredDate, hireDate) " +
                "VALUES(@PersonId, @CompanyCode, @FileNumber, @enteredBy, @enteredDate, @hireDate) " +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add(new SqlParameter("@PersonId", SqlDbType.Int, 4, "@PersonId"));
            cmdInsert.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdInsert.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50, "@FileNumber"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@PersonId"].Value = nPersonId;
            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;

            cmdInsert.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@enteredDate"].Value = dateEnteredDate;
            cmdInsert.Parameters["@hireDate"].Value = hireDate;

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

        public int InsertYmca(int nPersonId, string strCompanyCode, string strFileNumber
                                               , DateTime hireDate, DateTime? terminationDate, string strEnteredBy
                                               , DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO Employees " +
                "(PersonId, CompanyCode, FileNumber, enteredBy, enteredDate, hireDate, terminationDate, EmploymentStatusId) " +
                "VALUES(@PersonId, @CompanyCode, @FileNumber, @enteredBy, @enteredDate, @hireDate, @TerminationDate, @EmploymentStatusId) " +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add(new SqlParameter("@PersonId", SqlDbType.Int, 4, "@PersonId"));
            cmdInsert.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdInsert.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50, "@FileNumber"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@terminationDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@EmploymentStatusId", SqlDbType.Int, 4));

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@PersonId"].Value = nPersonId;
            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;

            cmdInsert.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@enteredDate"].Value = dateEnteredDate;
            cmdInsert.Parameters["@hireDate"].Value = hireDate;

            if (terminationDate == null)
            {
                cmdInsert.Parameters["@terminationDate"].Value = System.DBNull.Value;
                cmdInsert.Parameters["@employmentStatusId"].Value = 1;
            }
            else
            {
                cmdInsert.Parameters["@terminationDate"].Value = terminationDate;
                cmdInsert.Parameters["@employmentStatusId"].Value = 2;
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

        public int InsertPhoenix(int nPersonId, string strCompanyCode, string strFileNumber, int nEmploymentStatusId, string strHomeDepartment
                                                   , DateTime hireDate, DateTime? terminationDate, string strEnteredBy
                                                   , DateTime dateEnteredDate
                                                    , string strEmployeeType)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO Employees " +
                "(PersonId, CompanyCode, FileNumber, enteredBy, enteredDate, hireDate, terminationDate, EmploymentStatusId, HomeDept, EmployeeType) " +
                "VALUES(@PersonId, @CompanyCode, @FileNumber, @enteredBy, @enteredDate, @hireDate, @TerminationDate, @EmploymentStatusId, @HomeDept, @EmployeeType) " +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add(new SqlParameter("@PersonId", SqlDbType.Int, 4, "@PersonId"));
            cmdInsert.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdInsert.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50, "@FileNumber"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@terminationDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@EmploymentStatusId", SqlDbType.Int, 4));
            cmdInsert.Parameters.Add(new SqlParameter("@HomeDept", SqlDbType.VarChar, 10));
            cmdInsert.Parameters.Add(new SqlParameter("@EmployeeType", SqlDbType.VarChar, 100));

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@PersonId"].Value = nPersonId;
            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;

            cmdInsert.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@enteredDate"].Value = dateEnteredDate;
            cmdInsert.Parameters["@hireDate"].Value = hireDate;

            if (terminationDate == null)
            {
                cmdInsert.Parameters["@terminationDate"].Value = System.DBNull.Value;

            }
            else
            {
                cmdInsert.Parameters["@terminationDate"].Value = terminationDate;
            }

            cmdInsert.Parameters["@employmentStatusId"].Value = nEmploymentStatusId;
            cmdInsert.Parameters["@HomeDept"].Value = strHomeDepartment;
            cmdInsert.Parameters["@EmployeeType"].Value = strEmployeeType;

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

        public int InsertLutheran(int nPersonId, string strCompanyCode, string strFileNumber
                                                   , DateTime hireDate, DateTime? terminationDate, string strEnteredBy
                                                   , DateTime dateEnteredDate, int nEmploymentNumber)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO Employees " +
                "(PersonId, CompanyCode, FileNumber, enteredBy, enteredDate, hireDate, terminationDate, EmploymentStatusId, EmploymentNumber) " +
                "VALUES(@PersonId, @CompanyCode, @FileNumber, @enteredBy, @enteredDate, @hireDate, @TerminationDate, @EmploymentStatusId, @EmploymentNumber) " +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add(new SqlParameter("@PersonId", SqlDbType.Int, 4, "@PersonId"));
            cmdInsert.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdInsert.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50, "@FileNumber"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@terminationDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@EmploymentStatusId", SqlDbType.Int, 4));
            cmdInsert.Parameters.Add(new SqlParameter("@EmploymentNumber", SqlDbType.TinyInt));

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@PersonId"].Value = nPersonId;
            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;

            cmdInsert.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@enteredDate"].Value = dateEnteredDate;
            cmdInsert.Parameters["@hireDate"].Value = hireDate;

            if (terminationDate == null)
            {
                cmdInsert.Parameters["@terminationDate"].Value = System.DBNull.Value;
                cmdInsert.Parameters["@employmentStatusId"].Value = 1;
            }
            else
            {
                cmdInsert.Parameters["@terminationDate"].Value = terminationDate;
                cmdInsert.Parameters["@employmentStatusId"].Value = 2;
            }

            cmdInsert.Parameters["@EmploymentNumber"].Value = nEmploymentNumber;

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

        public int InsertEmployeesForPinhurst(int nPersonId, string strCompanyCode, string strFileNumber
                                                 , DateTime hireDate, DateTime? terminationDate, string strEnteredBy
                                                 , DateTime dateEnteredDate, int nEmploymentNumber, string strReportsToFileNumber)
        {

            int nRtn = -1;

            SqlCommand cmdInsert = m_Connection.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;

            cmdInsert.CommandText = "INSERT INTO Employees " +
                "(PersonId, CompanyCode, FileNumber, enteredBy, enteredDate, hireDate, terminationDate, EmploymentStatusId, EmploymentNumber,ReportsToFileNumber) " +
                "VALUES(@PersonId, @CompanyCode, @FileNumber, @enteredBy, @enteredDate, @hireDate, @TerminationDate, @EmploymentStatusId, @EmploymentNumber,@ReportsToFileNumber) " +
                "SELECT @ID =@@IDENTITY";

            cmdInsert.Parameters.Add(new SqlParameter("@PersonId", SqlDbType.Int, 4, "@PersonId"));
            cmdInsert.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdInsert.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50, "@FileNumber"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdInsert.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@terminationDate", SqlDbType.SmallDateTime));
            cmdInsert.Parameters.Add(new SqlParameter("@EmploymentStatusId", SqlDbType.Int, 4));
            cmdInsert.Parameters.Add(new SqlParameter("@EmploymentNumber", SqlDbType.TinyInt));
            //cmdInsert.Parameters.Add(new SqlParameter("@LocationDescription", SqlDbType.VarChar,20, "@LocationDescription"));
            cmdInsert.Parameters.Add(new SqlParameter("@ReportsToFileNumber", SqlDbType.VarChar, 20, "@ReportsToFileNumber"));

            cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

            cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

            cmdInsert.Parameters["@PersonId"].Value = nPersonId;
            cmdInsert.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdInsert.Parameters["@FileNumber"].Value = strFileNumber;

            cmdInsert.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdInsert.Parameters["@enteredDate"].Value = dateEnteredDate;
            cmdInsert.Parameters["@hireDate"].Value = hireDate;

            if (terminationDate == null)
            {
                cmdInsert.Parameters["@terminationDate"].Value = System.DBNull.Value;
                cmdInsert.Parameters["@employmentStatusId"].Value = 1;
            }
            else
            {
                cmdInsert.Parameters["@terminationDate"].Value = terminationDate;
                cmdInsert.Parameters["@employmentStatusId"].Value = 2;
            }

            cmdInsert.Parameters["@EmploymentNumber"].Value = nEmploymentNumber;
            //cmdInsert.Parameters["@LocationDescription"].Value = strLocationDescription;
            cmdInsert.Parameters["@ReportsToFileNumber"].Value = strReportsToFileNumber;

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

        public int UpdateEmployeeStatus(string strCC, string strFileNum, int nEmploymentStatusId, string strReportsToFileNumber)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                " EmploymentStatusId=@EmploymentStatusId, " +
                " ReportsToFileNumber=@ReportsToFileNumber " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmploymentStatusID", SqlDbType.TinyInt));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@ReportsToFileNumber", SqlDbType.VarChar, 50));


            if (nEmploymentStatusId != -1)
                cmdUpdate.Parameters["@EmploymentStatusID"].Value = nEmploymentStatusId;
            else
                cmdUpdate.Parameters["@EmploymentStatusID"].Value = System.DBNull.Value;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;
            cmdUpdate.Parameters["@ReportsToFileNumber"].Value = strReportsToFileNumber;

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

        public int UpdateEmployeeAvant(int nEmployeeId, string strCC, string strFileNum, string strEnteredBy, DateTime dateEntered)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   CompanyCode=@CompanyCode, " +
                "   FileNumber=@FileNumber, " +
                "   EnteredDate=@EnteredDate, " +
                "   EnteredBy = @EnteredBy " +
                " WHERE Id=@Id ";

            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));


            cmdUpdate.Parameters["@ID"].Value = nEmployeeId;
            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEntered;

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

        public int UpdateEmployeeYmca(int nEmployeeId, string strCC, string strFileNum, DateTime hireDate, DateTime? terminationDate
            , string strEnteredBy, DateTime dateEntered
            )
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   CompanyCode=@CompanyCode, " +
                "   FileNumber=@FileNumber, " +
                "   hireDate=@hireDate, " +
                "   terminationDate=@terminationDate, " +
                "   employmentStatusID=@employmentStatusID, " +
                "   EnteredDate=@EnteredDate, " +
                "   EnteredBy = @EnteredBy " +
                " WHERE Id=@Id ";

            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@terminationDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@employmentStatusID", SqlDbType.Int));


            cmdUpdate.Parameters["@ID"].Value = nEmployeeId;
            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEntered;
            cmdUpdate.Parameters["@hireDate"].Value = hireDate;

            if (terminationDate == null)
            {
                cmdUpdate.Parameters["@terminationDate"].Value = System.DBNull.Value;
                cmdUpdate.Parameters["@employmentStatusId"].Value = 1;
            }
            else
            {
                cmdUpdate.Parameters["@terminationDate"].Value = terminationDate;
                cmdUpdate.Parameters["@employmentStatusId"].Value = 2;
            }

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

        public int UpdateEmployeePhoenix(int nEmployeeId, string strCC, string strFileNum, DateTime hireDate, DateTime? terminationDate
                , string strHomeDepartment, int nEmployeeStatusId, string strEnteredBy, DateTime dateEntered, string strEmployeeType
                )
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   CompanyCode=@CompanyCode, " +
                "   FileNumber=@FileNumber, " +
                "   hireDate=@hireDate, " +
                "   terminationDate=@terminationDate, " +
                "   HomeDept=@HomeDepartment, " +
                "   employmentStatusID=@employmentStatusID, " +
                "   EnteredDate=@EnteredDate, " +
                "   EnteredBy = @EnteredBy, " +
                "   EmployeeType = @EmployeeType " +
                " WHERE Id=@Id ";

            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@terminationDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@employmentStatusID", SqlDbType.Int));
            cmdUpdate.Parameters.Add(new SqlParameter("@HomeDepartment", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeType", SqlDbType.VarChar, 100));

            cmdUpdate.Parameters["@ID"].Value = nEmployeeId;
            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEntered;
            cmdUpdate.Parameters["@hireDate"].Value = hireDate;

            if (terminationDate == null)
                cmdUpdate.Parameters["@terminationDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@terminationDate"].Value = terminationDate;


            cmdUpdate.Parameters["@employmentStatusId"].Value = nEmployeeStatusId;
            cmdUpdate.Parameters["@HomeDepartment"].Value = strHomeDepartment;
            cmdUpdate.Parameters["@EmployeeType"].Value = strEmployeeType;

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


        public int UpdateEmployeeLutheran(int nEmployeeId, string strCC, string strFileNum, DateTime hireDate, DateTime? terminationDate
               , string strEnteredBy, DateTime dateEntered
               )
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   CompanyCode=@CompanyCode, " +
                "   FileNumber=@FileNumber, " +
                "   hireDate=@hireDate, " +
                "   terminationDate=@terminationDate, " +
                "   employmentStatusID=@employmentStatusID, " +
                "   EnteredDate=@EnteredDate, " +
                "   EnteredBy = @EnteredBy " +
                " WHERE Id=@Id ";

            cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@hireDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@terminationDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@employmentStatusID", SqlDbType.Int));


            cmdUpdate.Parameters["@ID"].Value = nEmployeeId;
            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEntered;
            cmdUpdate.Parameters["@hireDate"].Value = hireDate;

            if (terminationDate == null)
            {
                cmdUpdate.Parameters["@terminationDate"].Value = System.DBNull.Value;
                cmdUpdate.Parameters["@employmentStatusId"].Value = 1;
            }
            else
            {
                cmdUpdate.Parameters["@terminationDate"].Value = terminationDate;
                cmdUpdate.Parameters["@employmentStatusId"].Value = 2;
            }

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

        public int UpdateEmployeeStatusFromAdpWebServices(string strCC, string strFileNum,
                                    int nEmployeeTypeID, int nEmployeeStatusId, DateTime startDate,
                                    DateTime endDate, string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   EmploymentTypeID=@EmploymentTypeID, " +
                "   EmploymentStatusId=@EmploymentStatusId, " +
                "   actualServiceStartDate=@actualServiceStartDate, " +
                "   TerminationDate=@TerminationDate, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmploymentTypeID", SqlDbType.SmallInt));
            cmdUpdate.Parameters.Add(new SqlParameter("@EmploymentStatusID", SqlDbType.TinyInt));
            cmdUpdate.Parameters.Add(new SqlParameter("@ActualServiceStartDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@TerminationDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));

            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            if (nEmployeeTypeID != -1)
                cmdUpdate.Parameters["@EmploymentTypeID"].Value = nEmployeeTypeID;
            else
                cmdUpdate.Parameters["@EmploymentTypeID"].Value = System.DBNull.Value;

            if (nEmployeeStatusId != -1)
                cmdUpdate.Parameters["@EmploymentStatusID"].Value = nEmployeeStatusId;
            else
                cmdUpdate.Parameters["@EmploymentStatusID"].Value = System.DBNull.Value;

            if (startDate == DateTime.MaxValue)
                cmdUpdate.Parameters["@ActualServiceStartDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@ActualServiceStartDate"].Value = startDate;

            if (endDate == DateTime.MaxValue)
                cmdUpdate.Parameters["@TerminationDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@TerminationDate"].Value = startDate;

            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateEmployeeWorkInfoFromAdpWebServices(string strCC, string strFileNum,
                                        DateTime hireDate, string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   HireDate=@HireDate, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@HireDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            if (hireDate == DateTime.MaxValue)
                cmdUpdate.Parameters["@HireDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@HireDate"].Value = hireDate;

            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateEmployeeTermDateAndStatusEffectiveDateFromAdpWebServices(string strCC, string strFileNum,
                                            DateTime terminationDate, DateTime statusEffectiveDate, string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   TerminationDate=@TerminationDate, " +
                "   StatusEffectiveDate=@StatusEffectiveDate, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@TerminationDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@StatusEffectiveDate", SqlDbType.Date));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            if (terminationDate == DateTime.MaxValue)
                cmdUpdate.Parameters["@TerminationDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@TerminationDate"].Value = terminationDate;

            if (statusEffectiveDate == DateTime.MaxValue)
                cmdUpdate.Parameters["@StatusEffectiveDate"].Value = System.DBNull.Value;
            else
                cmdUpdate.Parameters["@StatusEffectiveDate"].Value = statusEffectiveDate;


            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdatePayFequencyCode(string strCC, string strFileNum, string strPayFrequencyCode,
                                            string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   PayFrequencyCode=@PayFrequencyCode, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@PayFrequencyCode", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@PayFrequencyCode"].Value = strPayFrequencyCode;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateLocation(string strCC, string strFileNum, string strLocation,
                                   string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   Location_LOC_NAME=@Location_LOC_NAME, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@Location_LOC_NAME", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@Location_LOC_NAME"].Value = strLocation;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateDivision(string strCC, string strFileNum, string strDivision,
                                       string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   Division=@Division, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@Division", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@Division"].Value = strDivision;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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


        public int UpdateHomeClientCode(int nEmployeeId, string strHomeClientCode, string strHomeClientCodeDescription
                                           , string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   HomeClientCode=@HomeClientCode, " +
                "   HomeClientCodeDescription=@HomeClientCodeDescription, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE id=@id ";

            cmdUpdate.Parameters.Add(new SqlParameter("@HomeClientCode", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@HomeClientCodeDescription", SqlDbType.VarChar, 100));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@HomeClientCode"].Value = strHomeClientCode;
            cmdUpdate.Parameters["@HomeClientCodeDescription"].Value = strHomeClientCodeDescription;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;
            cmdUpdate.Parameters["@Id"].Value = nEmployeeId;

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

        public int UpdateHours(string strCC, string strFileNum, decimal dHours,
                                       string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   Hours=@Hours, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@Hours", SqlDbType.Decimal, 9));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@Hours"].Value = dHours;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateEmployeeTypeId(string strCC, string strFileNum, string strEmployeeTypeId,
                                        string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   EmployeeTypeId=@EmployeeTypeId, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@EmployeeTypeId", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@EmployeeTypeId"].Value = strEmployeeTypeId;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateSuiFedExemptionsStateTaxCode(string strCC, string strFileNum, string strSuiSdiTaxCode,
                                                        string strFedExemptions, string strWorkedStateTaxCode, string strEnteredBy,
                                                        DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   SUI=@SUI, " +
                "   WorkedStateTaxCode = @WorkedStateTaxCode, " +
                "   FedExemptions = @FedExemptions, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@SUI", SqlDbType.VarChar, 10));
            cmdUpdate.Parameters.Add(new SqlParameter("@WorkedStateTaxCode", SqlDbType.Char, 2));
            cmdUpdate.Parameters.Add(new SqlParameter("@FedExemptions", SqlDbType.Char, 2));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@SUI"].Value = strSuiSdiTaxCode;
            cmdUpdate.Parameters["@WorkedStateTaxCode"].Value = strWorkedStateTaxCode;
            cmdUpdate.Parameters["@FedExemptions"].Value = strFedExemptions;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public DataTable GetAllEmployeeJobBusinessUnitPositionDetailsRockland()
        {
            SqlCommand cmdSelect = new SqlCommand(" select e.filenumber as 'File Number' ,p.lastname + ',' + p.firstname as 'Name' , " +
                                                  " po.title as 'Position Title' , j.code as 'Job Code', " +
                                                  " j.title as 'Job Title' ," +
                                                  " h.code as 'HR Businesslevel Code' , " +
                                                  " h.title as 'HRBusinesslevel Title'  " +
                                                  " from persons p inner join employees e " +
                                                  " on p.id=e.personid " +
                                                  " inner join epositions ep " +
                                                  " on e.id=ep.employeeid " +
                                                  " inner join positions po " +
                                                  " on po.id=ep.positionid " +
                                                  " inner join jobs j " +
                                                  " on j.id= po.jobid " +
                                                  " inner join hrbusinesslevels h " +
                                                  " on po.businesslevelId=h.id " +
                                                  " order by p.lastname asc ", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int UpdateEmployeeClassCode(string strCC, string strFileNum, string strClassCode, string strEnteredBy, DateTime dateEnteredDate
                                                        )
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   ClassCode=@ClassCode, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@ClassCOde", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@ClassCode"].Value = strClassCode;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateEmployeeRetirementDate(string strCC, string strFileNum, DateTime retirementDate, string strEnteredBy, DateTime dateEnteredDate)
        {
            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   RetirementDate=@RetirementDate, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@RetirementDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@RetirementDate"].Value = retirementDate;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateEmployeeHours(string strCC, string strFileNum, Decimal dHours, string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   Hours=@Hours, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@Hours", SqlDbType.Decimal, 9));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@Hours"].Value = dHours;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateEmployeeRateAndRateType(string strCC, string strFileNum, Decimal dRate, string strRateType, string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   Rate=@Rate, " +
                "   RateType=@RateType, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@Rate", SqlDbType.Decimal, 9));
            cmdUpdate.Parameters.Add(new SqlParameter("@RateType", SqlDbType.VarChar, 10, "@RateType"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@Rate"].Value = dRate;
            cmdUpdate.Parameters["@RateType"].Value = strRateType;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public int UpdateEmployeeReportsTo(string strCC, string strFileNum, string strReportsToCompanyCode, string strReportsToFileNumber, string strEnteredBy, DateTime dateEnteredDate)
        {

            int nRtn = -1;

            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                "   ReportsToCompanyCode=@ReportsToCompanyCode, " +
                "   ReportsToFileNumber=@ReportsToFileNumber, " +
                "   enteredBy=@EnteredBy, enteredDate=@EnteredDate " +
                " WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ";

            cmdUpdate.Parameters.Add(new SqlParameter("@ReportsToCompanyCode", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@ReportsToFileNumber", SqlDbType.VarChar, 50));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredBy", SqlDbType.VarChar, 50, "@enteredBy"));
            cmdUpdate.Parameters.Add(new SqlParameter("@enteredDate", SqlDbType.SmallDateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50, "@CompanyCode"));
            cmdUpdate.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));


            cmdUpdate.Parameters["@ReportsToCompanyCode"].Value = strReportsToCompanyCode;
            cmdUpdate.Parameters["@ReportsToFileNumber"].Value = strReportsToFileNumber;
            cmdUpdate.Parameters["@enteredBy"].Value = strEnteredBy;
            cmdUpdate.Parameters["@enteredDate"].Value = dateEnteredDate;

            cmdUpdate.Parameters["@CompanyCode"].Value = strCC;
            cmdUpdate.Parameters["@FileNumber"].Value = strFileNum;

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

        public void Delete(int nEmployeeID)
        {
            SqlCommand cmdDelete = m_Connection.CreateCommand();
            cmdDelete.CommandType = CommandType.Text;
            cmdDelete.CommandText = "DELETE FROM Employees " +
                "WHERE ID = @ID ";

            cmdDelete.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
            cmdDelete.Parameters["@ID"].Value = nEmployeeID;

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
        public int GetMaxid()
        {
            int count = -1;

            try
            {

                m_Connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT MAX(Persons.ID) FROM Persons " +
                      " INNER JOIN Employees " +
                         "  ON Persons.ID = Employees.PersonID ", m_Connection);
                cmd.CommandType = CommandType.Text;

                count = (int)cmd.ExecuteScalar();
            }
            finally
            {

                m_Connection.Close();
            }
            return count;
        }

        public DataTable GetCode()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ID,code FROM hrBUSINESSLEVELS ORDER BY code ASC", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "hrBUSINESSLEVELS");

            return ds.Tables["hrBUSINESSLEVELS"];
        }

        public DataTable GetDataFromEmployeeinfo(int nPersonID)
        {

            SqlCommand cmdselect = new SqlCommand("SELECT Employees.id, LastName+', '+FirstName as PersonName," +
                     "Employees.personID, Employees.CompanyCode," +
                     "Employees.Rate, Employees.RateType," +
                     "Employees.FileNumber," +
                     "Employees.employmentNumber," +
                     "Employees.employmentStatusID," +
                     "Employees.hireDate," +
                     "Employees.terminationDate," +
                     "Employees.employmentTypeID," +
                     "Employees.payFrequencyCode," +
                     "Employees.ADPAnnualSalary," +
                     "Employees.HomeDept," +
                     "Employees.HomeJob," +
                     "Employees.FedExemptions," +
                     "Employees.WorkedStateTaxCode," +
                     "Employees.SUI, Employees.WorkedLocalTaxCode," +
                     "Employees.Hours, Employees.MaritalStatusID," +
                     "Employees.actualServiceStartDate," +
                     "Employees.Location_LOC_NAME as Location," +
                     "Employees.custArea4, Employees.employmentTypeID" +
                     "       FROM Employees" +
                     "           INNER JOIN Persons ON Persons.ID = Employees.PersonID" +
                     " WHERE PersonID = " + nPersonID + " " +
                     "     ORDER BY PersonName", m_Connection);
            cmdselect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdselect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllEmployeeWithENumber(int nEmployeeID, int nEmployeementNumber, int nPersonID)
        {


            SqlCommand cmdselect = new SqlCommand("SELECT Employees.id, LastName+', '+FirstName as PersonName," +
                     "Employees.personID, Employees.CompanyCode," +
                     "Employees.Rate, Employees.RateType," +
                     "Employees.FileNumber," +
                     "Employees.employmentNumber," +
                     "Employees.employmentStatusID," +
                     "Employees.hireDate," +
                     "Employees.terminationDate," +
                     "Employees.employmentTypeID," +
                     "Employees.payFrequencyCode," +
                     "Employees.ADPAnnualSalary," +
                     "Employees.HomeDept," +
                     "Employees.HomeJob," +
                     "Employees.FedExemptions," +
                     "Employees.WorkedStateTaxCode," +
                     "Employees.SUI, Employees.WorkedLocalTaxCode," +
                     "Employees.Hours, Employees.MaritalStatusID," +
                     "Employees.actualServiceStartDate," +
                     "Employees.custArea4, Employees.employmentTypeID" +
                     "       FROM Employees" +
                     "           INNER JOIN Persons ON Persons.ID = Employees.PersonID" +
                     " WHERE Employees.id = " + nEmployeeID + " AND Employees.employmentNumber = " + nEmployeementNumber + "  " + " AND Employees.personID = " + nPersonID + "  " +
                     "     ORDER BY PersonName", m_Connection);
            cmdselect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdselect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }


        public DataTable GetDataSelectedEmployeeNumber(int nEID, int EmpNo)
        {

            SqlCommand cmdselect = new SqlCommand("SELECT Employees.id, LastName+', '+FirstName as PersonName," +
                     "Employees.personID, Employees.CompanyCode," +
                     "Employees.Rate, Employees.RateType," +
                     "Employees.FileNumber," +
                     "Employees.employmentNumber," +
                     "Employees.employmentStatusID," +
                     "Employees.hireDate," +
                     "Employees.terminationDate," +
                     "Employees.employmentTypeID," +
                     "Employees.payFrequencyCode," +
                     "Employees.ADPAnnualSalary," +
                     "Employees.HomeDept," +
                     "Employees.HomeJob," +
                     "Employees.FedExemptions," +
                     "Employees.WorkedStateTaxCode," +
                     "Employees.SUI, Employees.WorkedLocalTaxCode," +
                     "Employees.Hours, Employees.MaritalStatusID," +
                     "Employees.actualServiceStartDate," +
                     "Employees.custArea4, Employees.employmentTypeID" +
                     "       FROM Employees" +
                     "           INNER JOIN Persons ON Persons.ID = Employees.PersonID" +
                     " WHERE Employees.personID = " + nEID + " AND Employees.employmentNumber = " + EmpNo + "  " +
                     "     ORDER BY PersonName", m_Connection);
            cmdselect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdselect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }


        public void DeleteAll()
        {
            SqlCommand cmdDelete = m_Connection.CreateCommand();
            cmdDelete.CommandType = CommandType.Text;
            cmdDelete.CommandText = "DELETE FROM Employees ";

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

        public DataTable GetAll()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM Employees", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllForAvantEmployeeImportGrid()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Employees.id as EmployeeId, LastName, FirstName, CompanyCode, FileNumber " +
            "FROM Employees Join Persons On Persons.Id = Employees.PersonId Order By LastName, FirstName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public int GetEmployeeId(string fileNumber)
        {
            if (fileNumber.Length == 5)
                fileNumber = "0" + fileNumber;
            else if (fileNumber.Length == 4)
                fileNumber = "00" + fileNumber;
            else if (fileNumber.Length == 3)
                fileNumber = "000" + fileNumber;
            else if (fileNumber.Length == 2)
                fileNumber = "0000" + fileNumber;

            SqlCommand cmdSelect = new SqlCommand("SELECT id FROM Employees where filenumber ='" + fileNumber + "'", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataTable dt = new DataTable();
            //DataSet ds = new DataSet();
            da.Fill(dt);

            if (dt.Rows.Count != 0)
                return Convert.ToInt32(dt.Rows[0]["id"]);
            return 0;
        }

        public DataTable GetAllBranchDepartments()
        {
            SqlCommand cmdSelect = new SqlCommand("select branch, homeDept, Branch+'/'+HomeDept as BranchHomeDept " +
                "FROM Employees " +
                "Group by branch, homeDept " +
                "ORDER BY Branch, HomeDept", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllDistinctCompanyCodes()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT DISTINCT CompanyCode FROM Employees", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllDistinctDepartments()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT DISTINCT CompanyCode, HomeDept, HomeDeptDesc  FROM Employees", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllDistinctJobs()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT DISTINCT CompanyCode, HomeJob, HomeJobDesc FROM Employees", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllIDsCCAndHireDate()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Persons.LastName+', '+Persons.firstname as preferredName, Persons.DOB, Employees.ADPAnnualSalary, " +
                "Employees.PayFrequencyCode, Employees.ID, Employees.CompanyCode, Employees.HireDate " +
                "FROM Employees INNER JOIN Persons ON Employees.PersonID = Persons.ID ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAll_ID_CC_FN_Salary_PFreq(string strCC)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ID, CompanyCode, FileNumber, ADPAnnualSalary,  PayFrequencyCode " +
                                                    "FROM Employees WHERE CompanyCode = '" + strCC + "'", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataRow GetCC(int nEmployeeID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ID, CompanyCode, FileNumber, ADPAnnualSalary,  PayFrequencyCode " +
                                                    "FROM Employees WHERE ID = @ID ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

            cmdSelect.Parameters["@ID"].Value = nEmployeeID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataRow GetMaxEmploymentNumberForPersonId(int nPersonId)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT MAX(EmploymentNumber) MaxEmploymentNumber " +
                                                    "FROM Employees WHERE personId = @PersonId ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@PersonId", SqlDbType.Int, 4));

            cmdSelect.Parameters["@PersonId"].Value = nPersonId;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataTable GetAllHRPExportInfo(string strCC)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT  Persons.ADP_PersonID, " +
                    "Employees.EmploymentNumber, " +
                    "ePosSalaries.BU_Code, " +
                    "ePosSalaries.JOBCODE, " +
                    "ePosSalaries.PositionCode, " +
                    "Persons.SSN, " +
                    "ddlEmploymentStatuses.code as EmploymentStatusCode, " + //"Employees.employmentStatusID,  1 = A, 2 = T  
                    "ddlEmployeeTypes.code as EmployeeType, " + //Employees.EmployeeTypeID, "+ // ddlEmployeeTypes.ID to get    // BLANK
                    "Employees.HireDate, " +
                    "convert(varchar, Employees.TerminationDate, 1) as TerminationDate, " +                                       //BLANK
                    "ePosSalaries.payFrequencyCode as ePosPayFrequencyCode, " +
                    "ePosSalaries.rate as ePosRate, " +
                    "ePosSalaries.rateTypeCode as ePosRateTypeCode, " +
                    "ePosSalaries.daysPerPayPeriod as eDaysPerPayPeriod, " +
                    "ePosSalaries.hoursPerPayPeriod as eHoursPerPayPeriod, " +
                    "convert(varchar, ePosSalaries.startDate, 1) as StartDate, " +
                    "convert(varchar, ePosSalaries.effectiveDate, 1) as effectiveDate, " +
                    "ePosSalaries.hoursPerPayPeriod / 2 as weeklyHours " +
                "FROM Employees " +
                "       INNER JOIN Persons " +
                "           ON Employees.PersonID = Persons.ID " +
                "       LEFT JOIN ddlEmployeeTypes " +
                "           ON employees.employmentTypeID = ddlEmployeeTypes.id " +
                "       LEFT JOIN ddlEmploymentStatuses " +
                "           ON  Employees.employmentStatusID = ddlEmploymentStatuses.ID " +
                "       INNER JOIN  " +
                "            (	SELECT x.ePositionID, ePositionSalaryHistory.RateTypeCode,  " +
                "                       ePositionSalaryHistory.daysPerPayPeriod, ePositionSalaryHistory.hoursPerPayPeriod,  " +
                "						ePositionSalaryHistory.Rate, ePositions.PayFrequencyCode, employeeID, " +
                "                       Jobs.Code as JobCode, Positions.code as PositionCode, hrBusinessLevels.Code as BU_Code, " +
                "                       ePositions.startDate, ePositionSalaryHistory.effectiveDate, ePositionSalaryHistory.ImportedRecord " +
                "				FROM " +
                "					(SELECT 	MAX(EffectiveDate) as effectiveDate , ePositionID " +
                "					 FROM ePositions " +
                "						INNER JOIN ePositionSalaryHistory  " +
                "							ON ePositions.id = ePositionSalaryHistory.ePositionID " +
                "					 WHERE primaryPosition = 1 " +
                "					 GROUP BY ePositionID) x  " +
                "						INNER JOIN ePositionSalaryHistory  " +
                "							ON x.ePositionID = ePositionSalaryHistory.ePositionID  " +
                "								and x.effectiveDate = ePositionSalaryHistory.effectiveDate  " +
                "						INNER JOIN ePositions  " +
                "							ON ePositions.ID = x.ePositionID " +
                "						INNER JOIN Employees  " +
                "							ON Employees.id = ePositions.employeeID	 " +
                "                       INNER JOIN Positions " +
                "                           ON Positions.id = ePositions.PositionID " +
                "                       INNER JOIN Jobs " +
                "                           ON Jobs.ID = Positions.JobID " +
                "                       INNER JOIN hrBusinessLevels " +
                "                           ON hrBusinessLevels.ID = Positions.BusinessLevelID " +
                "             ) ePosSalaries  " +
                "       ON Employees.ID = ePosSalaries.employeeID " +
                "WHERE Employees.CompanyCode = '" + strCC + "' AND " +
                "   employees.EmploymentStatusID = 1 AND (ePosSalaries.ImportedRecord = 0 or ePosSalaries.ImportedRecord IS NULL ) ", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllTrainingExportInfoHRB(string strCC, DateTime dateStart, DateTime dateEnd)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT   " +
                    "Employees.FileNumber, " +
                    "convert(varchar, personTraining.completedDate,101) as completedDate, " +
                    "ddlTrainingCourses.Code +' - '+ddlTrainingCourses.description as CodeAndDescription, " +
                    "convert(varchar, personTraining.startDate,101) as startDate, " +
                    "convert(varchar, personTraining.scheduledDate,101) as scheduledDate, " +
                    "personTraining.notes " +
                "FROM Persons " +
                "       INNER JOIN Employees " +
                "           ON Employees.PersonID = Persons.ID " +
                "       INNER JOIN personTraining " +
                "           ON personTraining.personID = Persons.ID " +
                "       INNER JOIN ddlTrainingCoursesSchedule " +
                "           ON ddlTrainingCoursesSchedule.id = personTraining.CourseScheduleID " +
                "       INNER JOIN ddlTrainingCourses " +
                "           ON ddlTrainingCourses.id = ddlTrainingCoursesSchedule.CourseID " +
                "WHERE Employees.CompanyCode = @CompanyCode " +
                "   AND	@StartDate <= (CAST( FLOOR( CAST( personTraining.enteredDate AS FLOAT ) ) AS DATETIME )) " +
                "   AND @EndDate >= (CAST( FLOOR( CAST( personTraining.enteredDate AS FLOAT ) ) AS DATETIME )) "
                , m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmdSelect.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));


            cmdSelect.Parameters["@CompanyCode"].Value = strCC;
            cmdSelect.Parameters["@StartDate"].Value = dateStart;
            cmdSelect.Parameters["@EndDate"].Value = dateEnd;


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetEmployeePositionTitle(int nEmployeeID)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "Select lastname, firstname, employees.id as employeeID,  Title " +
                "FROM Positions " +
                "   INNER JOIN epositions On Positions.id = ePositions.PositionID " +
                "   INNER JOIN Employees On Employees.id = ePositions.employeeID " +
                "   INNER JOIN Persons On Persons.ID = Employees.PersonID " +
                "WHERE ePositions.PrimaryPosition = 1 AND employees.id = @employeeID "
                , m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int, 4));

            cmdSelect.Parameters["@EmployeeID"].Value = nEmployeeID;


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetEmployeeNamePositionId()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "   Select lastname+','+ firstname as name, Positions.id as positionId " +
                "   FROM Positions" +
                "   INNER JOIN epositions On Positions.id = ePositions.PositionID " +
                "   INNER JOIN Employees On Employees.id = ePositions.employeeID " +
                "   INNER JOIN Persons On Persons.ID = Employees.PersonID" +
                "   ORDER BY name", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetEmployeePositionIdBaseOnLastName(string strLastName)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "Select lastname, firstname, employees.id as employeeID,  Positions.Title as PositionTitle " +
                "   , Positions.Id as PositionId " +
                "FROM Positions " +
                "   INNER JOIN epositions On Positions.id = ePositions.PositionID " +
                "   INNER JOIN Employees On Employees.id = ePositions.employeeID " +
                "   INNER JOIN Persons On Persons.ID = Employees.PersonID " +
                "WHERE ePositions.PrimaryPosition = 1 AND Persons.LastName = @Lastname AND ePositions.ActualEndDate is Null " +
                "ORDER BY ePositions.StartDate DESC "
                , m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@Lastname", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@LastName"].Value = strLastName;


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetEmployeePositionIdBaseOnCoCodeFileNumber(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "Select lastname, firstname, employees.id as employeeID,  Positions.Title as PositionTitle " +
                "   , Positions.Id as PositionId " +
                "FROM Positions " +
                "   INNER JOIN epositions On Positions.id = ePositions.PositionID " +
                "   INNER JOIN Employees On Employees.id = ePositions.employeeID " +
                "   INNER JOIN Persons On Persons.ID = Employees.PersonID " +
                "WHERE ePositions.PrimaryPosition = 1 AND Employees.CompanyCode = @CompanyCode " +
                "   AND Employees.FileNumber = @FileNumber AND ePositions.ActualEndDate is Null " +
                "ORDER BY ePositions.StartDate DESC "
                , m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllMasterFileInfo(string strCC)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT  Employees.ID, " +
                    "Employees.CompanyCode, " +
                    "Employees.FileNumber, " +
                    "Persons.FirstName, " +
                    "Persons.LastName, " +
                    "Persons.SSN, " +
                    "ddlEmploymentStatuses.code as EmploymentStatusCode, " +//"Employees.employmentStatusID, "+ // 1 = A, 2 = T  
                    "Persons.Gender, " + // false = M, true = F                          BLANK
                    "ddlActualMaritalStatuses.Code as MaritalStatusCode, " + // Persons.ActualMaritalStatusesID BLANK
                    "Employees.FedExemptions, " + //                                          BLANK
                    "Employees.WorkedStateTaxCode, " +
                    "Employees.SUI, " +
                    "Employees.WorkedLocalTaxCode, " + //                                       BLANK
                    "ddlEmployeeTypes.code as EmployeeType, " + //Employees.EmployeeTypeID, "+ // ddlEmployeeTypes.ID to get    // BLANK
                    "Employees.HomeDept, " +                                             //BLANK
                    "Employees.PayFrequencyCode, " +                                     //BLANK
                    "Employees.Rate, " +
                    "Employees.RateType, " +
                    "Persons.DOB, " +
                    "Employees.HireDate, " +
                    "Employees.TerminationDate, " +                                       //BLANK
                    "ePosSalaries.payFrequencyCode as ePosPayFrequencyCode, " +
                    "ePosSalaries.rate as ePosRate, " +
                    "ePosSalaries.rateTypeCode as ePosRateTypeCode, " +
                    "ePosSalaries.daysPerPayPeriod as eDaysPerPayPeriod, " +
                    "ePosSalaries.hoursPerPayPeriod as eHoursPerPayPeriod, " +
                    "ePosSalaries.effectiveDate " +
                "FROM Employees " +
                "       INNER JOIN Persons " +
                "           ON Employees.PersonID = Persons.ID " +
                "       LEFT JOIN ddlEmployeeTypes " +
                "           ON employees.employmentTypeID = ddlEmployeeTypes.id " +
                "       LEFT JOIN ddlEmploymentStatuses " +
                "           ON  Employees.employmentStatusID = ddlEmploymentStatuses.ID " +
                "       LEFT JOIN ddlActualMaritalStatuses " +
                "           ON Employees.MaritalStatusID = ddlActualMaritalStatuses.ID " +

                "       LEFT JOIN  " +
                "            (	SELECT x.effectiveDate, x.ePositionID, ePositionSalaryHistory.RateTypeCode,  " +
                "                       ePositionSalaryHistory.daysPerPayPeriod, ePositionSalaryHistory.hoursPerPayPeriod,  " +
                "						ePositionSalaryHistory.Rate, ePositions.PayFrequencyCode, employeeID " +
                "				FROM " +
                "					(SELECT 	MAX(EffectiveDate) as effectiveDate , ePositionID " +
                "					 FROM ePositions " +
                "						INNER JOIN ePositionSalaryHistory  " +
                "							ON ePositions.id = ePositionSalaryHistory.ePositionID " +
                "					 WHERE primaryPosition = 1   AND actualEndDate is null " +
                "					 GROUP BY ePositionID) x  " +
                "						INNER JOIN ePositionSalaryHistory  " +
                "							ON x.ePositionID = ePositionSalaryHistory.ePositionID  " +
                "								and x.effectiveDate = ePositionSalaryHistory.effectiveDate  " +
                "						INNER JOIN ePositions  " +
                "							ON ePositions.ID = x.ePositionID " +
                "						INNER JOIN Employees  " +
                "							ON Employees.id = ePositions.employeeID	 " +
                "             ) ePosSalaries  " +
                "       ON Employees.ID = ePosSalaries.employeeID " +
                "WHERE Employees.CompanyCode = '" + strCC + "'", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllAdditionalInfoSpringfield()
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT  Employees.ID, " +
                "Employees.CompanyCode, " +
                "Employees.FileNumber, " +
                "Persons.FirstName, " +
                "Persons.LastName, " +
                "Persons.SSN, " +
                "COALESCE(	(SELECT Top 1 PersonAddresses.AddressLineOne  " +
                "	        FROM PersonAddresses  " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as AddressLine1, " +
                "COALESCE(	(SELECT Top 1 PersonAddresses.AddressLineTwo  " +
                "	        FROM PersonAddresses  " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as AddressLine2, " +
                "COALESCE(	(SELECT Top 1 PersonAddresses.City  " +
                "	        FROM PersonAddresses  " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as City, " +
                "COALESCE(	(SELECT Top 1 ddlStates.Code " +
                "	        FROM PersonAddresses INNER JOIN ddlStates ON ddlStates.ID = PersonAddresses.StateID " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as State, " +
                "COALESCE(	(SELECT Top 1 PersonAddresses.ZipCode " +
                "	        FROM PersonAddresses  " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as ZipCode, " +
                "Employees.FedExemptions, " +
                "Employees.WorkedStateTaxCode, " +
                "Employees.SUI, " +
                "Employees.WorkedLocalTaxCode " +
                "FROM Employees " +
                "       INNER JOIN Persons " +
                "           ON Employees.PersonID = Persons.ID " +
                "       LEFT JOIN ddlEmployeeTypes " +
                "           ON employees.employmentTypeID = ddlEmployeeTypes.id " +
                "       LEFT JOIN ddlEmploymentStatuses " +
                "           ON  Employees.employmentStatusID = ddlEmploymentStatuses.ID " +
                "       LEFT JOIN ddlActualMaritalStatuses " +
                "           ON Employees.MaritalStatusID = ddlActualMaritalStatuses.ID "

                , m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllCarrierFeedMedFileInfoBCBSRockland(string strCC)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT  " +
                "Employees.ID as EmployeeID, [Record Type] = 'S', persons.ADP_PersonID as [Subscriber ID], persons.ADP_PersonID as SSN, Persons.FirstName as [First Name], " +
                "Persons.LastName as [Last Name], " +
                "COALESCE(Persons.MiddleName, '') as Middle,  " +
                "Gender	=	CASE " +
                "		        WHEN persons.Gender = 1 THEN 'F' " +
                "		        ELSE 'M' " +
                "	        END , " +
                "[Rel Code] = '01',  " +
                "convert(varchar, Persons.DOB, 112) as Birthdate, " +
                "COALESCE(	(SELECT Top 1 PersonAddresses.AddressLineOne  " +
                "	        FROM PersonAddresses  " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as [Address 1], " +
                "COALESCE(	(SELECT Top 1 PersonAddresses.AddressLineTwo  " +
                "	        FROM PersonAddresses  " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as [Address 2], " +
                "COALESCE(	(SELECT Top 1 PersonAddresses.City  " +
                "	        FROM PersonAddresses  " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as City, " +
                "COALESCE(	(SELECT Top 1 ddlStates.Code " +
                "	        FROM PersonAddresses INNER JOIN ddlStates ON ddlStates.ID = PersonAddresses.StateID " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as [State Code], " +
                "COALESCE(	(SELECT Top 1 PersonAddresses.ZipCode " +
                "	        FROM PersonAddresses  " +
                "	        WHERE PersonAddresses.addressTypeID = 5 AND PersonAddresses.PersonID = Persons.ID), '') as [Zip Code], " +
                "[Zip 4] = '',  " +
                "convert(varchar, employeeBenefitSelections.EffectiveStartDate, 112) as [Effective Date], " +
                "employeeBenefitSelections.EffectiveStartDate as [Effective Date Normal Format], " +
                "convert(varchar, employeeBenefitSelections.OverrideEffectiveDate, 112) as [Override Effective Date], " +
                "employeeBenefitSelections.OverrideEffectiveDate as [Override Effective Date Normal Format], " +
                "[Medical Group] = 'MED', " +
                "[Coverage Code] = 	CASE " +
                "				        WHEN BenefitPlans.BenefitOption = 'FTF'		THEN '127' " +
                "				        WHEN BenefitPlans.BenefitOption = 'FTFO'	THEN '127' " +
                "				        WHEN BenefitPlans.BenefitOption = 'FTI'		THEN '101' " +
                "				        WHEN BenefitPlans.BenefitOption = 'FTIO'	THEN '101' " +
                "				        WHEN BenefitPlans.BenefitOption = 'PTF'		THEN '127' " +
                "				        WHEN BenefitPlans.BenefitOption = 'PTFO'	THEN '127' " +
                "				        WHEN BenefitPlans.BenefitOption = 'PTI'		THEN '101' " +
                "				        WHEN BenefitPlans.BenefitOption = 'PTIO'	THEN '101' " +
                "				        ELSE '' " +
                "			        END , " +
                "employeeBenefitSelections.IsCurrentPatient as [Existing Patient Indicator], " +
                "[PCP Plan Code] = COALESCE (SUBSTRING(employeeBenefitSelections.PCPID,1,3), ''), " +
                "[PCP Number] = CASE " +
                "				        WHEN LEN(employeeBenefitSelections.PCPID) < 3 THEN '' " +
                "				        WHEN SUBSTRING(employeeBenefitSelections.PCPID,1,3) = '700'	THEN  " +
                "					        'MA PCP-'+SUBSTRING(employeeBenefitSelections.PCPID,4,LEN(employeeBenefitSelections.PCPID)-3) " +
                "				        WHEN SUBSTRING(employeeBenefitSelections.PCPID,1,3) = '770'	THEN " +
                "					        'NH PCP-'+SUBSTRING(employeeBenefitSelections.PCPID,4,LEN(employeeBenefitSelections.PCPID)-3) " +
                "				        WHEN SUBSTRING(employeeBenefitSelections.PCPID,1,3) = '870'	THEN  " +
                "					        'RI PCP-'+SUBSTRING(employeeBenefitSelections.PCPID,4,LEN(employeeBenefitSelections.PCPID)-3) " +
                "				        WHEN SUBSTRING(employeeBenefitSelections.PCPID,1,3) = '915'	THEN  " +
                "					        'VT PCP-'+SUBSTRING(employeeBenefitSelections.PCPID,4,LEN(employeeBenefitSelections.PCPID)-3) " +
                "				        WHEN SUBSTRING(employeeBenefitSelections.PCPID,1,3) = '060'	THEN " +
                "					        'CT-'+SUBSTRING(employeeBenefitSelections.PCPID,4,LEN(employeeBenefitSelections.PCPID)-3) " +
                "				        WHEN SUBSTRING(employeeBenefitSelections.PCPID,1,3) = '680'	THEN  " +
                "					        'ME-'+SUBSTRING(employeeBenefitSelections.PCPID,4,LEN(employeeBenefitSelections.PCPID)-3) " +
                "				        ELSE '' " +
                "			        END  " +
            "FROM Persons " +
            "        INNER JOIN Employees ON Persons.ID = Employees.PersonID " +
            "        INNER JOIN employeeBenefitSelections employeeBenefitSelections ON employeeBenefitSelections.EmployeeID = employees.ID " +
            "        INNER JOIN BenefitPlanDetails On BenefitPlanDetails.ID = employeeBenefitSelections.BenefitPlanDetailID " +
            "        INNER JOIN BenefitPlans ON BenefitPlans.CompanyCode = BenefitPlanDetails.CompanyCode AND  " +
            "			        BenefitPlans.Code = BenefitPlanDetails.BenefitPlanCode " +
            "WHERE employeeBenefitSelections.PlanStatus = 'current' " +
            "    AND employeeBenefitSelections.BenefitPlanDetailID is not null " +
            "    AND employeeBenefitSelections.BenefitPlanDetailID > 0 " +
            "    AND employeeBenefitSelections.BenefitGroupCode = 'MED' " +
            " " +
            "UNION ALL " +
            " " +
            "SELECT Employees.ID as EmployeeID, [Record Type] = 'M', " +
            "    (SELECT ADP_PersonID FROM Persons INNER JOIN Employees e2 ON e2.PersonID = Persons.ID WHERE e2.id = Employees.ID) as [Subscriber ID], " +
            "    persons.SSN, Persons.FirstName as [First Name], " +
            "    Persons.LastName as [Last Name], " +
            "    COALESCE(Persons.MiddleName, '') as Middle, " +
            "    Gender	=	CASE " +
            "			        WHEN persons.Gender = 1 THEN 'F' " +
            "			        ELSE 'M' " +
            "		        END , " +
            "    [Rel Code] = CASE " +
            "			        WHEN ddlRelationshipTypes.Description = 'spouse' THEN '02' " +
            "			        WHEN ddlRelationshipTypes.Description = 'child' THEN '03' " +
            "			        WHEN ddlRelationshipTypes.Description = 'dependent child' THEN '03' " +
            "			        WHEN ddlRelationshipTypes.Description = 'handicapped dependent' THEN '04' " +
            "			        WHEN ddlRelationshipTypes.Description = 'student dependent' THEN '05' " +
            "			        WHEN ddlRelationshipTypes.Description = 'domestic partner' THEN '06' " +
            "			        WHEN ddlRelationshipTypes.Description = 'divorced spouse' THEN '07' " +
            "			        ELSE '' " +
            "		        END ,  " +
            "    convert(varchar, Persons.DOB, 112) as Birthdate, " +
            "    COALESCE(	(SELECT Top 1 PersonAddresses.AddressLineOne  " +
            "		        FROM Employees e2 INNER JOIN Persons ON e2.PersonID = Persons.ID  " +
            "			        INNER JOIN PersonAddresses ON PersonAddresses.PersonID = Persons.ID  " +
            "		        WHERE PersonAddresses.addressTypeID = 5 AND e2.ID = Employees.ID), '') as [Address 1], " +
            "    COALESCE(	(SELECT Top 1 PersonAddresses.AddressLineTwo " +
            "		        FROM Employees e2 INNER JOIN Persons ON e2.PersonID = Persons.ID " +
            "			        INNER JOIN PersonAddresses ON PersonAddresses.PersonID = Persons.ID " +
            "		        WHERE PersonAddresses.addressTypeID = 5 AND e2.ID = Employees.ID), '') as [Address 2], " +
            "    COALESCE(	(SELECT Top 1 PersonAddresses.City " +
            "		        FROM Employees e2 INNER JOIN Persons ON e2.PersonID = Persons.ID " +
            "			        INNER JOIN PersonAddresses ON PersonAddresses.PersonID = Persons.ID  " +
            "		        WHERE PersonAddresses.addressTypeID = 5 AND e2.ID = Employees.ID), '') as City, " +
            "    COALESCE(	(SELECT Top 1 ddlStates.Code  " +
            "		        FROM Employees e2 INNER JOIN Persons ON e2.PersonID = Persons.ID " +
            "			        INNER JOIN PersonAddresses ON PersonAddresses.PersonID = Persons.ID " +
            "			        INNER JOIN ddlStates ON ddlStates.ID = PersonAddresses.StateID  " +
            "		        WHERE PersonAddresses.addressTypeID = 5 AND e2.ID = Employees.ID), '') as [State Code], " +
            "    COALESCE(	(SELECT Top 1 PersonAddresses.ZipCode " +
            "		        FROM Employees e2 INNER JOIN Persons ON e2.PersonID = Persons.ID  " +
            "			        INNER JOIN PersonAddresses ON PersonAddresses.PersonID = Persons.ID  " +
            "		        WHERE PersonAddresses.addressTypeID = 5 AND e2.ID = Employees.ID), '') as [Zip Code], " +
            "    [Zip 4] = '', " +
            "    convert(varchar, employeeBenefitSelections.EffectiveStartDate, 112) as [Effective Date], " +
            "    employeeBenefitSelections.EffectiveStartDate as [Effective Date Normal Format], " +
            "    convert(varchar, employeeBenefitSelections.OverrideEffectiveDate, 112) as [Override Effective Date], " +
            "    employeeBenefitSelections.OverrideEffectiveDate as [Override Effective Date Normal Format], " +
            "    [Medical Group] = 'MED', " +
            "    [Coverage Code] = 	CASE " +
            "					        WHEN BenefitPlans.BenefitOption = 'FTF'		THEN '127' " +
            "					        WHEN BenefitPlans.BenefitOption = 'FTFO'	THEN '127' " +
            "					        WHEN BenefitPlans.BenefitOption = 'FTI'		THEN '101' " +
            "					        WHEN BenefitPlans.BenefitOption = 'FTIO'	THEN '101' " +
            "					        WHEN BenefitPlans.BenefitOption = 'PTF'		THEN '127' " +
            "					        WHEN BenefitPlans.BenefitOption = 'PTFO'	THEN '127' " +
            "					        WHEN BenefitPlans.BenefitOption = 'PTI'		THEN '101' " +
            "					        WHEN BenefitPlans.BenefitOption = 'PTIO'	THEN '101' " +
            "					        ELSE '' " +
            "				        END , " +
            "    empBenPlanSelectionsDependants.IsCurrentPatient as [Existing Patient Indicator], " +
            "    [PCP Plan Code] = COALESCE (SUBSTRING(empBenPlanSelectionsDependants.PCPID,1,3), ''), " +
            "    [PCP Number] = CASE " +
            "					        WHEN LEN(empBenPlanSelectionsDependants.PCPID) < 3 THEN '' " +
            "					        WHEN SUBSTRING(empBenPlanSelectionsDependants.PCPID,1,3) = '700'	THEN  " +
            "						        'MA PCP-'+SUBSTRING(empBenPlanSelectionsDependants.PCPID,4,LEN(employeeBenefitSelections.PCPID)-3) " +
            "					        WHEN SUBSTRING(empBenPlanSelectionsDependants.PCPID,1,3) = '770'	THEN  " +
            "						        'NH PCP-'+SUBSTRING(empBenPlanSelectionsDependants.PCPID,4,LEN(empBenPlanSelectionsDependants.PCPID)-3) " +
            "					        WHEN SUBSTRING(empBenPlanSelectionsDependants.PCPID,1,3) = '870'	THEN  " +
            "						        'RI PCP-'+SUBSTRING(empBenPlanSelectionsDependants.PCPID,4,LEN(empBenPlanSelectionsDependants.PCPID)-3) " +
            "					        WHEN SUBSTRING(empBenPlanSelectionsDependants.PCPID,1,3) = '915'	THEN " +
            "						        'VT PCP-'+SUBSTRING(empBenPlanSelectionsDependants.PCPID,4,LEN(empBenPlanSelectionsDependants.PCPID)-3) " +
            "					        WHEN SUBSTRING(empBenPlanSelectionsDependants.PCPID,1,3) = '060'	THEN  " +
            "						        'CT PCP-'+SUBSTRING(empBenPlanSelectionsDependants.PCPID,4,LEN(empBenPlanSelectionsDependants.PCPID)-3) " +
            "					        WHEN SUBSTRING(empBenPlanSelectionsDependants.PCPID,1,3) = '680'	THEN  " +
            "						        'ME PCP-'+SUBSTRING(empBenPlanSelectionsDependants.PCPID,4,LEN(empBenPlanSelectionsDependants.PCPID)-3) " +
            "					        ELSE '' " +
            "				        END  " +
            "FROM empBenPlanSelectionsDependants  " +
            "        INNER JOIN employeeBenefitSelections " +
            "	        ON employeeBenefitSelections.ID = empBenPlanSelectionsDependants.employeeBenefitSelectionID " +
            "        INNER JOIN Employees ON Employees.ID = employeeBenefitSelections.EmployeeID " +
            "        INNER JOIN BenefitPlanDetails On BenefitPlanDetails.ID = employeeBenefitSelections.BenefitPlanDetailID " +
            "        INNER JOIN BenefitPlans ON BenefitPlans.CompanyCode = BenefitPlanDetails.CompanyCode AND " +
            "			        BenefitPlans.Code = BenefitPlanDetails.BenefitPlanCode " +
            "        INNER JOIN PersonRelationships " +
            "	        ON EmpBenPlanSelectionsDependants.PersonRelationshipsID = PersonRelationships.ID  " +
            "        INNER JOIN Persons ON Persons.ID = PersonRelationships.RelationID  " +
            "        INNER JOIN ddlRelationShipTypes ON ddlRelationshipTypes.ID = PersonRelationships.RelationshipTypeID " +
            "WHERE employeeBenefitSelections.PlanStatus = 'current' " +
            "    AND employeeBenefitSelections.BenefitPlanDetailID is not null " +
            "    AND employeeBenefitSelections.BenefitPlanDetailID > 0 " +
            "    AND employeeBenefitSelections.BenefitGroupCode = 'MED' " +
            "ORDER BY Employees.ID, [Record Type] DESC, Lastname DESC ", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            //cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 10));
            //cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            //cmdSelect.Parameters["@CompanyCode"].Value = strCC;
            //cmdSelect.Parameters["@FileNumber"].Value = strFN;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllCarrierFeedDentalFileInfoBCBSRockland(string coCode, string tsql)
        {
            SqlCommand cmdSelect = new SqlCommand(tsql, m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            //cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 10));
            //cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            //cmdSelect.Parameters["@CompanyCode"].Value = strCC;
            //cmdSelect.Parameters["@FileNumber"].Value = strFN;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetHomeAddress(string strCC, int nEmpID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT  " +

                    "PersonAddresses.AddressLineOne, " + // ddlAddressTypes.code = H
                    "PersonAddresses.AddressLineTwo, " +
                    "PersonAddresses.City, " +
                    "ddlStates.Code as StateCode, " + //personAddresses.stateid
                    "PersonAddresses.ZipCode " +
                "FROM Employees " +
                        "INNER JOIN Persons " +
                "           ON Employees.PersonID = Persons.ID " +
                "       INNER JOIN PersonAddresses " +
                "           ON Persons.ID = PersonAddresses.PersonID " +
                "       INNER JOIN ddlStates " +
                "           ON PersonAddresses.StateID = ddlStates.ID " +
                "WHERE Employees.CompanyCode = '" + strCC + "' AND Employees.ID = " + nEmpID + " AND checkPayrollAddress=1", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAll_ID_PersonID_CC_FN(string strCC)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ID, PersonID, CompanyCode, FileNumber " +
                                                    "FROM Employees WHERE CompanyCode = '" + strCC + "'", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllByCompanyCodeForDDL(string strCC)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Employees.ID as EmployeeID, Employees.FileNumber, LastName+', '+FirstName as PersonName " +
                "FROM Employees " +
                "   INNER JOIN Persons ON Persons.ID = Employees.PersonID " +
                "WHERE CompanyCode = '" + strCC + "' ORDER BY PersonName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllByEmployeeIdCoCodeFileNumberForDDL()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Employees.ID as EmployeeID, Employees.FileNumber, Employees.FileNumber, LastName+', '+FirstName as PersonName " +
                "   ,LastName+', '+FirstName + ' - '+companyCode+' - '+filenumber as PersonNameWithCoCodeFn " +
                "   ,Convert(varchar(10),Employees.Id) + '!' + employees.CompanyCode +'!' + Employees.FileNumber as EmpIdCoCodeFileNumber " +
                "FROM Employees " +
                "   INNER JOIN Persons ON Persons.ID = Employees.PersonID " +
                "ORDER BY LastName, FirstName "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllByEmployeeIdCoCodeFileNumberForDDLNonTerminated()
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Employees.ID as EmployeeID, Employees.FileNumber, Employees.FileNumber, LastName+', '+FirstName as PersonName " +
                "   ,LastName+', '+FirstName + ' - '+companyCode+' - '+filenumber as PersonNameWithCoCodeFn " +
                "   ,Convert(varchar(10),Employees.Id) + '!' + employees.CompanyCode +'!' + Employees.FileNumber as EmpIdCoCodeFileNumber " +
                "FROM Employees " +
                "   INNER JOIN Persons ON Persons.ID = Employees.PersonID " +
                "WHERE Employees.TerminationDate is null " +
                "ORDER BY LastName, FirstName "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllByEmployeeIdCoCodeFileNumberForDDL(string strBranch, string strDept)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Employees.ID as EmployeeID, Employees.FileNumber, Employees.FileNumber, LastName+', '+FirstName as PersonName " +
                "   ,LastName+', '+FirstName + ' - '+companyCode+' - '+filenumber as PersonNameWithCoCodeFn " +
                "   ,Convert(varchar(10),Employees.Id) + '!' + employees.CompanyCode +'!' + Employees.FileNumber as EmpIdCoCodeFileNumber " +
                "FROM Employees " +
                "   INNER JOIN Persons ON Persons.ID = Employees.PersonID " +
                "WHERE Branch = @Branch AND HomeDept = @HomeDept " +
                "ORDER BY LastName, FirstName "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@Branch", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@HomeDept", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@Branch"].Value = strBranch;
            cmdSelect.Parameters["@HomeDept"].Value = strDept;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllByEmployeeIdCoCodeFileNumberForDDLNonTerminated(string strBranch, string strDept)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Employees.ID as EmployeeID, Employees.FileNumber, Employees.FileNumber, LastName+', '+FirstName as PersonName " +
                "   ,LastName+', '+FirstName + ' - '+companyCode+' - '+filenumber as PersonNameWithCoCodeFn " +
                "   ,Convert(varchar(10),Employees.Id) + '!' + employees.CompanyCode +'!' + Employees.FileNumber as EmpIdCoCodeFileNumber " +
                "FROM Employees " +
                "   INNER JOIN Persons ON Persons.ID = Employees.PersonID " +
                "WHERE Branch = @Branch AND HomeDept = @HomeDept " +
                "   AND Employees.TerminationDate is null " +
                "ORDER BY LastName, FirstName "
                , m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@Branch", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@HomeDept", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@Branch"].Value = strBranch;
            cmdSelect.Parameters["@HomeDept"].Value = strDept;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetAllForRocklandWebtimeLogin(string strLastName, string strSsnLastFour)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Employees.ID as EmployeeID, Employees.FileNumber, LastName+', '+FirstName as PersonName, Persons.Id as PersonId " +
                "   ,Employees.CompanyCode " +
                "FROM Employees " +
                "   INNER JOIN Persons ON Persons.ID = Employees.PersonID " +
                "WHERE LastName = '" + strLastName + "' AND SSN = '" + strSsnLastFour + "' ORDER BY PersonName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable getUserTypeOnly(int groupIDOne, int groupIDTwo, int groupIDThree)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Employees.ID as EmployeeID, Employees.FileNumber, LastName+', '+FirstName as PersonName " +
                "FROM Employees " +
                "   INNER JOIN Persons ON Persons.ID = Employees.PersonID " +
                " INNER JOIN PersonsGroups on PersonsGroups.PersonID = Persons.ID WHERE PersonsGroups.GroupID = " + groupIDOne +
                " or PersonsGroups.GroupID = " + groupIDTwo + " or PersonsGroups.GroupID = " + groupIDThree +
                " order by Employees.ID", m_Connection);
            //"WHERE LastName = '" + strLastName + "' AND Employees.Filenumber = '" + strFileNumber + "' ORDER BY PersonName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }
        /*
         * Coded by: Ranjan Dahal
         * Gets the data for Manager and Administrators users only for Rockland Webtime as per their
         * requirements.
         * */
        public DataTable getManagerForRockland(string strLastName, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Employees.ID as EmployeeID, AdminUsers.FileNumber, LastName+', '+FirstName as PersonName, Persons.Id as PersonId  " +
                "   ,Employees.CompanyCode " +
                "FROM Employees " +
                "   INNER JOIN Persons ON Persons.ID = Employees.PersonID " +
                " INNER JOIN AdminUsers on AdminUsers.employeeId = Employees.ID " +
                "WHERE LastName = '" + strLastName + "' AND AdminUsers.Filenumber = '" + strFileNumber + "' ORDER BY PersonName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        /*
         * Checks whether the employee user exists or not 
         * 
         * */
        public bool checkUserExistence(string fileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT FileNumber FROM AdminUsers WHERE Filenumber = '" + fileNumber + "'", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet dt = new DataSet();
            da.Fill(dt, "Employees");

            if (dt.Tables["Employees"].Rows.Count == 0)
                return false;
            return true;
        }
        /// <summary>
        /// returns filenumber of passed employeeid as a parameter
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>Filenumber String: returns filenumber of passed employee</returns>
        public string getFilenumber(string employeeId)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT FileNumber FROM Employees WHERE id = " + Convert.ToInt32(employeeId), m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet dt = new DataSet();
            da.Fill(dt, "Employees");
            return dt.Tables["Employees"].Rows[0][0].ToString();
        }

        /// <summary>
        /// It returns the datable of entire employee who has the passed lastname and filenumber.
        /// Since filenumber is a unique identifier to identify employee identity, most of the
        /// time we get only one record. If we get multiple record then we have to handle this 
        /// to the place where we call this method.
        /// </summary>
        /// <param name="strLastName">Lastname : String</param>
        /// <param name="strFileNumber">Filenumber: String</param>
        /// <returns></returns>
        public DataTable GetAllForRocklandWebtimeLoginUsingFileNumber(string strLastName, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT Employees.ID as EmployeeID, Employees.FileNumber, LastName+', '+FirstName as PersonName, Persons.Id as PersonId " +
                "   ,Employees.CompanyCode " +
                "FROM Employees " +
                "   INNER JOIN Persons ON Persons.ID = Employees.PersonID " +
                "WHERE LastName = '" + strLastName + "' AND Employees.Filenumber = '" + strFileNumber + "' ORDER BY PersonName", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataRow GetRecordForID(int nID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM Employees WHERE ID=@ID", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@ID"].Value = nID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecordForLocation(string strCompanyCode, string strFileNumber)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM Employees WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCompanyCode;
            cmdSelect.Parameters["@FileNumber"].Value = strFileNumber;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataRow GetHireDateRecordForADP_PersonID(string strADP_PersonID)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT HireDate " +
                "FROM Employees " +
                "   INNER JOIN Persons On Persons.ID = Employees.PersonID " +
                "WHERE ADP_PersonID=@ADPPersonID", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@ADPPersonID", SqlDbType.VarChar, 9));
            cmdSelect.Parameters["@ADPPersonID"].Value = strADP_PersonID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecordForCC_FN(string strCC, string strFN)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Employees.*, ddlEmploymentStatuses.Code as EmployeeStatusCode, " +
                "Persons.id as PersonId, Persons.LastName, Persons.FirstName " +
                "FROM Employees " +
                "   INNER JOIN Persons On Persons.ID = Employees.PersonID " +
                "   LEFT JOIN ddlEmploymentStatuses On ddlEmploymentStatuses.id = employees.employmentStatusId " +
                "WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 10));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCC;
            cmdSelect.Parameters["@FileNumber"].Value = strFN;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataTable GetAllForCC_FN(string strCC, string strFN)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Employees.*, ddlEmploymentStatuses.Code as EmployeeStatusCode, " +
                "Persons.id as PersonId, Persons.LastName, Persons.FirstName " +
                "FROM Employees " +
                "   INNER JOIN Persons On Persons.ID = Employees.PersonID " +
                "   LEFT JOIN ddlEmploymentStatuses On ddlEmploymentStatuses.id = employees.employmentStatusId " +
                "WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 50));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCC;
            cmdSelect.Parameters["@FileNumber"].Value = strFN;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataRow GetEmployeeIdAndStatusForCC_FN(string strCC, string strFN)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Employees.id, ddlEmploymentStatuses.code, ddlEmploymentStatuses.description " +
                "FROM Employees " +
                "   Left Join ddlEmploymentStatuses on ddlEmploymentStatuses.id = employees.employmentStatusId " +
                "WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 10));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCC;
            cmdSelect.Parameters["@FileNumber"].Value = strFN;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecordForCC_FN_ForFordClassCodeCalc(string strCC, string strFN)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "select employees.companycode, employees.filenumber, retirementDate, classcode, employees.hours as weeklyhours " +
                "   , ddlEmployeeTypes.description as employeeType, ePositions.PayFrequencyCode " +
                "   , ( Select top 1 hoursperpayperiod from ePositionSalaryHistory " +
                "       where ePositionsalaryhistory.ePositionId = epositions.id order by startDate desc) as hoursPerPayPeriod " +
                "   ,ddlEmploymentStatuses.Description as EmploymentStatus, DOB, hireDate " +
                "from employees " +
                "   INNER JOIN Persons On Persons.Id = employees.PersonId " +
                "   Left Join ePositions on ePositions.employeeId = employees.id " +
                "   Left Join ddlEmployeeTypes on ddlEmployeeTypes.id =  employees.employmentTypeId " +
                "   Left Join ddlEmploymentstatuses on ddlEmploymentStatuses.id =  employees.employmentStatusId " +
                "WHERE employees.companyCode = @companyCode and employees.filenumber = @Filenumber", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 10));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCC;
            cmdSelect.Parameters["@FileNumber"].Value = strFN;


            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataRow GetEmployeeStatusRecordForCC_FN(string strCC, string strFN)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT ddlEmploymentStatuses.Code, ddlEmploymentStatuses.Description, employees.id as employeeId FROM Employees " +
                "   INNER JOIN ddlEmploymentStatuses On ddlEmploymentStatuses.id = Employees.employmentStatusID " +
                "WHERE CompanyCode=@CompanyCode AND FileNumber=@FileNumber", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 10));
            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@CompanyCode"].Value = strCC;
            cmdSelect.Parameters["@FileNumber"].Value = strFN;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecordForFileNumber(string strFN)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM Employees " +
                "WHERE FileNumber=@FileNumber", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@FileNumber", SqlDbType.VarChar, 50));

            cmdSelect.Parameters["@FileNumber"].Value = strFN;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataRow GetHighestFNForCC(string strCC)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT MAX(FileNumber) as FileNumber FROM Employees " +
                "WHERE CompanyCode=@CompanyCode ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@CompanyCode", SqlDbType.VarChar, 10));

            cmdSelect.Parameters["@CompanyCode"].Value = strCC;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employee");

            if (ds.Tables["Employee"].Rows.Count > 0)
                return ds.Tables["Employee"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecordForIDFromPersonIDWithPersonInfo(int nID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Employees.ID, Persons.LastName, Persons.Firstname, Persons.email FROM Employees " +
            "       INNER JOIN Persons ON Employees.PersonID = Persons.ID " +
            "WHERE Persons.ID=@ID ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@ID"].Value = nID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Person");

            if (ds.Tables["Person"].Rows.Count > 0)
                return ds.Tables["Person"].Rows[0];
            else
                return null;
        }

        public DataRow GetRecordForIDFromPersonIDWithPersonInfoAndIsPrimary(int nID)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT Employees.ID, Persons.LastName, Persons.Firstname, Persons.email FROM Employees " +
            "       INNER JOIN Persons " +
            "           ON Employees.PersonID = Persons.ID " +
            "       INNER JOIN ePositions " +
            "           ON ePositions.employeeID = employees.id " +
            "WHERE Persons.ID=@ID AND ePositions.primaryPosition = 1 ", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
            cmdSelect.Parameters["@ID"].Value = nID;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Person");

            if (ds.Tables["Person"].Rows.Count > 0)
                return ds.Tables["Person"].Rows[0];
            else
                return null;
        }

        // Added while working for Rockland. Date:- 7/26/2012

        public string GetJobCodeForID(int id)
        {
            SqlCommand cmdSelect = new SqlCommand("Select HomeDept FROM Employees where id=" + id, m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"].Rows[0][0].ToString();
            //return ds.Tables["Employees"];
        }

        public DataTable GetEmployeesForDepartment(string code)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM Employees where HomeDept='" + code + "'", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public DataTable GetNewEmployeesToExportRockland()
        {
            //SqlCommand cmdSelect = new SqlCommand("SELECT Employees.ID, Persons.LastName, Persons.firstname , Employees.filenumber, Persons.DOB, Employees.ADPAnnualSalary, " +
            //   " Persons.SSN, Employees.PayFrequencyCode,  Employees.CompanyCode, Employees.HireDate, " +
            //   " Employees.Rate, Employees.RateTYpe " +
            //   "FROM Employees INNER JOIN Persons ON Employees.PersonID = Persons.ID where Employees.AddtoADP=1 ", m_Connection);

            //SqlCommand cmdSelect = new SqlCommand("SELECT Employees.ID, Persons.LastName, Persons.firstname , Employees.filenumber, Persons.DOB, Employees.ADPAnnualSalary, " +
            //   " Persons.SSN, Employees.PayFrequencyCode,  Employees.CompanyCode, Employees.HireDate, " +
            //   " Employees.Rate, Employees.RateTYpe, Employees.TerminationDate, Employees.FedExemptions, " +
            //   " Employees.WorkedStateTaxCode, Employees.WorkedLocalTaxCode, Employees.HomeDept, Persons.Gender, " +
            //   "FROM Employees INNER JOIN Persons ON Employees.PersonID = Persons.ID where Employees.AddtoADP=1 ", m_Connection);

            SqlCommand cmdSelect = new SqlCommand("SELECT  Employees.ID, " +
                    "Employees.CompanyCode, " +
                    "Employees.FileNumber, " +
                    "Persons.FirstName, " +
                    "Persons.LastName, " +
                    "Persons.SSN, " +
                    "ddlEmploymentStatuses.code as EmploymentStatusCode, " +//"Employees.employmentStatusID, "+ // 1 = A, 2 = T  
                    "Persons.Gender, " + // false = M, true = F                          BLANK
                    "ddlActualMaritalStatuses.Code as MaritalStatusCode, " + // Persons.ActualMaritalStatusesID BLANK
                    "Employees.FedExemptions, " + //                                          BLANK
                    "Employees.WorkedStateTaxCode, " +
                    "Employees.SUI, " +
                    "Employees.WorkedLocalTaxCode, " + //                                       BLANK
                    "ddlEmployeeTypes.code as EmployeeType, " + //Employees.EmployeeTypeID, "+ // ddlEmployeeTypes.ID to get    // BLANK
                    "Employees.HomeDept, " +                                             //BLANK
                    "Employees.PayFrequencyCode, " +                                     //BLANK
                    "Employees.Rate, " +
                    "Employees.RateType, " +
                    "Persons.DOB, " +
                    "Employees.HireDate, " +
                    "Employees.TerminationDate, " +                                       //BLANK
                    "ePosSalaries.effectiveDate " +
                "FROM Employees " +
                "       INNER JOIN Persons " +
                "           ON Employees.PersonID = Persons.ID " +
                "       LEFT JOIN ddlEmployeeTypes " +
                "           ON employees.employmentTypeID = ddlEmployeeTypes.id " +
                "       LEFT JOIN ddlEmploymentStatuses " +
                "           ON  Employees.employmentStatusID = ddlEmploymentStatuses.ID " +
                "       LEFT JOIN ddlActualMaritalStatuses " +
                "           ON Employees.MaritalStatusID = ddlActualMaritalStatuses.ID " +

                "       LEFT JOIN  " +
                "            (	SELECT x.effectiveDate, x.ePositionID, ePositionSalaryHistory.RateTypeCode,  " +
                "                       ePositionSalaryHistory.daysPerPayPeriod, ePositionSalaryHistory.hoursPerPayPeriod,  " +
                "						ePositionSalaryHistory.Rate, ePositions.PayFrequencyCode, employeeID " +
                "				FROM " +
                "					(SELECT 	MAX(EffectiveDate) as effectiveDate , ePositionID " +
                "					 FROM ePositions " +
                "						INNER JOIN ePositionSalaryHistory  " +
                "							ON ePositions.id = ePositionSalaryHistory.ePositionID " +
                "					 WHERE primaryPosition = 1   AND actualEndDate is null " +
                "					 GROUP BY ePositionID) x  " +
                "						INNER JOIN ePositionSalaryHistory  " +
                "							ON x.ePositionID = ePositionSalaryHistory.ePositionID  " +
                "								and x.effectiveDate = ePositionSalaryHistory.effectiveDate  " +
                "						INNER JOIN ePositions  " +
                "							ON ePositions.ID = x.ePositionID " +
                "						INNER JOIN Employees  " +
                "							ON Employees.id = ePositions.employeeID	 " +
                "             ) ePosSalaries  " +
                "       ON Employees.ID = ePosSalaries.employeeID " +
                "WHERE Employees.AddtoADP=1", m_Connection);

            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return ds.Tables["Employees"];
        }

        public int GetADPStatusByFileNumber(String fileno)
        {
            SqlCommand cmdSelect = new SqlCommand("Select AddToADP from Employees where FileNumber= '" + fileno + "'", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataSet ds = new DataSet();
            da.Fill(ds, "Employees");

            return Convert.ToInt32(ds.Tables["Employees"].Rows[0][0]);
        }

        public DataTable GetAllHudsonFteReport()
        {
            SqlCommand cmdSelect = m_Connection.CreateCommand();
            cmdSelect.CommandText = "uspFteReport";
            cmdSelect.CommandType = CommandType.StoredProcedure;

            //cmdSelect.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int, 4));

            //cmdSelect.Parameters["@Year"].Value = nYear;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        public DataTable GetAllHudsonFteVerticalReport()
        {
            SqlCommand cmdSelect = m_Connection.CreateCommand();
            cmdSelect.CommandText = "uspFteVerticalReport";
            cmdSelect.CommandType = CommandType.StoredProcedure;

            //cmdSelect.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int, 4));

            //cmdSelect.Parameters["@Year"].Value = nYear;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        public DataTable GetAllEmployeePositionsActualEndDateIsNull(int nEmployeeId)
        {
            SqlCommand cmdSelect = new SqlCommand(" " +
                "SELECT * " +
                "FROM ePositions " +
                "WHERE EmployeeId = @EmployeeId AND ActualEndDate is NULL", m_Connection);
            cmdSelect.CommandType = CommandType.Text;

            cmdSelect.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int, 4));

            cmdSelect.Parameters["@EmployeeId"].Value = nEmployeeId;

            SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        public void UpdateEPositionActualEndDate(int nId, DateTime actualEndDate)
        {
            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE ePositions " +
                " SET " +
                " ActualEndDate=@ActualEndDate  " +
                " WHERE id = @Id";

            cmdUpdate.Parameters.Add(new SqlParameter("@ActualEndDate", SqlDbType.DateTime));
            cmdUpdate.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int, 4));

            cmdUpdate.Parameters["@Id"].Value = nId;
            cmdUpdate.Parameters["@ActualEndDate"].Value = actualEndDate;

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
        }

        public void UpdateNewEmployeeAddToADPStatusRockland(String str)
        {
            SqlCommand cmdUpdate = m_Connection.CreateCommand();
            cmdUpdate.CommandType = CommandType.Text;

            cmdUpdate.CommandText = "UPDATE Employees " +
                " SET " +
                " AddToADP=0  " +
                " WHERE id in " + str;

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
        }

    }
}

