using Dapper;
using ExecViewHrk.Domain.Interface;
using ExecViewHrk.EfClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ExecViewHrk.EfAdmin;

namespace ExecViewHrk.Domain.Repositories
{
    public abstract class RepositoryBase : IBaseRepository
    {
        protected IDbConnection _connection;
       public int _timeout = 60;
        protected ClientDbContext _context;

        protected AdminDbContext _contextAdmin;

        protected RepositoryBase()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["execView1"].ConnectionString;           
            _context = new ClientDbContext(connectionString);
            _connection = new SqlConnection(connectionString);
            _contextAdmin = new AdminDbContext();

            string timeout = ConfigurationManager.AppSettings["SqlCommandTimeout"];
            if (!string.IsNullOrEmpty(timeout))
            {
                if (!int.TryParse(timeout, out _timeout))
                {
                    _timeout = 60;
                }
            }
        }

        public bool CanDispose { get; set; }

        #region Protected Methods

        protected List<T> Query<T>(string storedProcName)
        {
            return Query<T>(storedProcName, null);
        }

        protected List<T> Query<T>(string storedProcName, object parameters)
        {
            try
            {
                return _connection.Query<T>(storedProcName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: _timeout).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this.Dispose();
            }
        }

        protected void Execute(string storedProcName)
        {
            Execute(storedProcName, null);
        }

        /// <summary>
        /// Wraps Dapper extension that takes any prepared sql.
        /// </summary>
        /// <param name="sql">Can be stored procedure or any prepared sql.</param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        protected void Execute(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                _connection.Execute(
                    sql, parameters, commandType: commandType,
                    commandTimeout: _timeout);
            }
            catch (Exception exception)
            {
                exception.Data.Add("sql", sql);
                throw;
            }
            finally
            {
                this.Dispose();
            }
        }

        protected void Execute(string storedProcName, object parameters, IDbTransaction transaction)
        {
            try
            {
                _connection.Execute(storedProcName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure, commandTimeout: _timeout);
            }
            catch (Exception ex) { throw; }
            finally
            {
                this.Dispose();
            }
        }

        protected object ExecuteScalar(string storedProcName)
        {
            return ExecuteScalar(storedProcName, null);
        }

        protected object ExecuteScalar(string storedProcName, object parameters)
        {
            try
            {
                return _connection.ExecuteScalar(storedProcName, parameters, commandType: CommandType.StoredProcedure, commandTimeout: _timeout);
            }
            catch (Exception ex) { throw; }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// Executes <paramref name="query"/> and returns a <typeparamref name="TScalar"/>.
        /// </summary>
        /// <typeparam name="TScalar"></typeparam>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public TScalar ExecuteScalar<TScalar>(string query, object param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                return _connection.ExecuteScalar<TScalar>(query, param, commandType: commandType);
            }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// Executes <paramref name="query"/> and returns a list of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected List<T> QueryTableView<T>(string query, object parameters = null, CommandType? commandType = null)
        {
            try
            {
                return _connection.Query<T>(query, parameters, commandType: commandType, commandTimeout: _timeout).ToList();
            }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// Executes <paramref name="sql"/> that returns multiple sets of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public List<List<T>> QueryMultiple<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using (var multi = _connection.QueryMultiple(sql, parameters, commandType: commandType, commandTimeout: _timeout))
            {
                var results = new List<List<T>>();
                while (!multi.IsConsumed)
                    results.Add(multi.Read<T>().ToList());

                return results;
            }
        }

        /// <summary>
        /// Executes <paramref name="sql"/> for multiple result sets and gives the resulting <see cref="SqlMapper.GridReader"/> to <paramref name="act"/>.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="act"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        public void QueryMultipleGridReader(string sql, Action<SqlMapper.GridReader> act, object parameters = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            using (var multi = _connection.QueryMultiple(sql, parameters, commandType: commandType, commandTimeout: _timeout))
            {
                act(multi);
            }
        }

        /// <summary>
        /// For custom reports that have different output lists.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected IEnumerable<dynamic> QueryDynamic(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            return _connection.Query(sql, parameters, commandType: commandType, commandTimeout: _timeout).ToArray();
        }

        #endregion Protected Methods

        #region Protected Methods For Policy Application Related

        protected void InsertUpdateDeleteSave(string storedProcName, List<KeyValuePair<string, string>> parameters)
        {
            try
            {
                SqlCommand SqlC = new SqlCommand
                {
                    Connection = (SqlConnection)_connection,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = storedProcName,
                    CommandTimeout = _timeout
                };

                for (int i = 0; i < parameters.Count(); i++)
                {
                    SqlC.Parameters.Add(new SqlParameter(parameters[i].Key, parameters[i].Value));
                }

                SqlC.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this.Dispose();
            }
        }

        protected DataTable GetDataTable(string sqlQuery)
        {
            try
            {
                if (string.IsNullOrEmpty(sqlQuery)) return new DataTable();

                string sqlText = string.Empty;
                SqlCommand SqlC = new SqlCommand
                {
                    Connection = (SqlConnection)_connection,
                    CommandType = CommandType.Text,
                    CommandTimeout = _timeout,
                    CommandText = sqlQuery
                };
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(SqlC);
                da.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this.Dispose();
            }
        }

        #endregion Protected Methods For Policy Application Related

        #region IDisposable

        public void Dispose(bool force)
        {
            this.CanDispose = true;
            Dispose();
        }

        public void Dispose()
        {
            if (_connection != null && this.CanDispose)
                _connection.Dispose();
        }

        #endregion IDisposable

        #region Private Methods/Functions

        private string SQLValue(string strValue)
        {
            strValue = "'" + strValue.Replace("'", "''") + "'";
            return strValue;
        }

        #endregion Private Methods/Functions
    }
}