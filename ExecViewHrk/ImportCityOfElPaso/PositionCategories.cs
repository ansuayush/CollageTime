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
/// Summary description for PositionCategories
/// </summary>
public class PositionCategories
{
		private SqlConnection m_Connection;

		public PositionCategories(SqlConnection connection)
		{
			m_Connection = connection;
		}

    public int Insert(   string strCode, string strDescription, bool bIsActive)
    {

        int nRtn = -1;

        SqlCommand cmdInsert = m_Connection.CreateCommand();
        cmdInsert.CommandType = CommandType.Text;

        cmdInsert.CommandText = "INSERT INTO ddlPositionCategories " +
            "(Code, Description, Active) " +
            "VALUES(@Code, @Description, @Active)" +
            "SELECT @ID =@@IDENTITY";

        cmdInsert.Parameters.Add("@Code", SqlDbType.VarChar,10);
        cmdInsert.Parameters.Add("@Description", SqlDbType.VarChar, 50);
        cmdInsert.Parameters.Add("@Active", SqlDbType.Bit, 1);
        
        cmdInsert.Parameters.Add("@ID", SqlDbType.Int);

        cmdInsert.Parameters["@ID"].Direction = ParameterDirection.Output;

        cmdInsert.Parameters["@Code"].Value             = strCode;
        cmdInsert.Parameters["@Description"].Value      = strDescription;
        cmdInsert.Parameters["@Active"].Value    = bIsActive;

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

    public void Update( int nID, string strCode, string strDescription, bool bIsActive)
    {

        SqlCommand cmdUpdate = m_Connection.CreateCommand();
        cmdUpdate.CommandType = CommandType.Text;
        cmdUpdate.CommandText = "UPDATE ddlPositionCategories " +
            "SET Code = @Code, " +
            "Description = @Description," +
            "Active = @Active "+
            "WHERE ID = @ID";

        cmdUpdate.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar, 10));
        cmdUpdate.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar, 50));
        cmdUpdate.Parameters.Add(new SqlParameter("@Active", SqlDbType.Bit, 1));
        cmdUpdate.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));

        cmdUpdate.Parameters["@Code"].Value = strCode;
        cmdUpdate.Parameters["@Description"].Value = strDescription;
        cmdUpdate.Parameters["@Active"].Value = bIsActive;
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
        cmdDelete.CommandText = "DELETE FROM ddlPositionCategories " +
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

    public DataTable GetAll()
    {
        SqlCommand cmdSelect = new SqlCommand("SELECT * FROM ddlPositionCategories", m_Connection);
        cmdSelect.CommandType = CommandType.Text;

        SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

        DataSet ds = new DataSet();
        da.Fill(ds, "PositionCategories");

        return ds.Tables["PositionCategories"];
    }


    public DataRow GetRecordForID(int nID)
    {
        SqlCommand cmdSelect = new SqlCommand("SELECT * FROM ddlPositionCategories WHERE ID=@ID", m_Connection);
        cmdSelect.CommandType = CommandType.Text;

        cmdSelect.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4));
        cmdSelect.Parameters["@ID"].Value = nID;

        SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

        DataSet ds = new DataSet();
        da.Fill(ds, "PositionCategory");

        if (ds.Tables["PositionCategory"].Rows.Count > 0)
            return ds.Tables["PositionCategory"].Rows[0];
        else
            return null;
    }

    public DataRow GetRecordForCode(string strCode)
    {
        SqlCommand cmdSelect = new SqlCommand("SELECT * FROM ddlPositionCategories WHERE Code=@Code", m_Connection);
        cmdSelect.CommandType = CommandType.Text;

        cmdSelect.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar, 10));
        cmdSelect.Parameters["@Code"].Value = strCode;

        SqlDataAdapter da = new SqlDataAdapter(cmdSelect);

        DataSet ds = new DataSet();
        da.Fill(ds, "PositionCategory");

        if (ds.Tables["PositionCategory"].Rows.Count > 0)
            return ds.Tables["PositionCategory"].Rows[0];
        else
            return null;
    }
       
	}
}

