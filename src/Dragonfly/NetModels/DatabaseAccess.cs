namespace Dragonfly.NetModels
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;

    public class DatabaseAccess
    {
        private const string ThisClassName = "Dragonfly.NetModels.DatabaseAccess";

        #region Private Variables
        SqlConnection ThisConnection = null;
        bool IsConnectionError = false;

        #endregion

        #region Custom Enums



        #endregion

        #region Properties



        #endregion

        #region Init, etc. Methods

        public DatabaseAccess(string ConnectionStringName)
        {
            string ConnString = "";
            try
            {
                ConnString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            }
            catch (Exception exConnStr)
            {
                this.IsConnectionError = true;
                //TODO: Update using new code pattern:
                //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                //var msg = string.Format("");
                //Info.LogException("irisCommon.SupplementalDB.Init", exConnStr, "ConnectionStringName=" + ConnectionStringName);
            }

            try
            {
                this.ThisConnection = new SqlConnection(ConnString);
                this.ThisConnection.Open();
            }
            catch (Exception ex)
            {
                this.IsConnectionError = true;
                //TODO: Update using new code pattern:
                //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                //var msg = string.Format("");
                //Info.LogException("SupplementalDB.Init", ex, "ConnString=" + ConnString);
            }

        }

        public bool ConnectionError()
        {
            return this.IsConnectionError;
        }

        public int RunUpdateCommand(string UpdateSQLStatement)
        {

            int RecordsAffected = 0;
            SqlCommand cmd = null;
            SqlDataReader rdr = null;

            try
            {
                cmd = new SqlCommand(UpdateSQLStatement, this.ThisConnection);
                RecordsAffected = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //TODO: Update using new code pattern:
                //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                //var msg = string.Format("");
                //Info.LogException("SupplementalDB.Init", ex, "UpdateSQLStatement=" + UpdateSQLStatement);
            }
            finally
            {
                if (cmd != null)
                { cmd.Dispose(); }

                if (rdr != null)
                { rdr.Dispose(); }
            }

            return RecordsAffected;
        }

        public string GetData(string SQLStatement)
        {
            string Result = "to do - not yet implemented";

            SqlCommand cmd = null;
            SqlDataReader rdr = null;

            try
            {
                cmd = new SqlCommand(SQLStatement, this.ThisConnection);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    // Operate on fetched data
                }
            }
            catch (Exception ex)
            {
                //TODO: Update using new code pattern:
                //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                //var msg = string.Format("");
                //Info.LogException("SupplementalDB.Init", ex, "UpdateSQLStatement=" + SQLStatement);
            }
            finally
            {
                if (cmd != null)
                { cmd.Dispose(); }

                if (rdr != null)
                { rdr.Dispose(); }
            }

            return Result;
        }

        public void CloseConnection()
        {
            if (this.ThisConnection != null)
            {
                this.ThisConnection.Close();
            }
        }

        #endregion
    }
}