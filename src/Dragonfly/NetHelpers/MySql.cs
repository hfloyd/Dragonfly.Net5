namespace Dragonfly.NetHelpers
{
    using System;
    using System.Data;
    using System.Data.Odbc;
    using System.Data.SqlClient;
    using System.Text;

    public static class MySql
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.MySql";
        

        public static DataSet GetMySQLDataSet(string ConnectionString, string MySQLQuery)
        {
            DataSet dsReturn = new DataSet();
            int NumRowsReturned;
            
            try {
                OdbcConnection cn;
                OdbcCommand cm;
                OdbcDataReader rd;

                cn = new OdbcConnection(ConnectionString);
                cn.Open();

                cm = new OdbcCommand(MySQLQuery, cn);
                NumRowsReturned = cm.ExecuteNonQuery();

                rd = cm.ExecuteReader();

                OdbcDataAdapter da = new OdbcDataAdapter(MySQLQuery, cn);
                
                da.Fill(dsReturn, "ReturnData");

                //while (rd.Read())
                //{
                //    Response.write(rd("field1"))
                //}

                rd.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                var msg = string.Format("");
                //Info.LogException(functionName, ex);
            }

            return dsReturn;


            //Example using MySQL .Net Connector
            //MySqlConnection myConnection;
            //Dim myDataAdapter As MySqlDataAdapter
            //Dim myDataSet    As DataSet
            // 
            //Dim strSQL      As String
            //Dim iRecordCount  As Integer
            //Dim connString   As String
            //connString = ConfigurationManager.ConnectionStrings["BMREConnString"].ConnectionString
            //myConnection = new MySqlConnection(connString)
            //	 
            //'the following line works not commented out, just not reading from the web.config file.
            //	 
            //'myConnection = New MySqlConnection("Server=;Port=;Database=;Uid=;Pwd=;")
            //   strSQL = "SELECT * FROM Agents;"
            //	 
            // myDataAdapter = New MySqlDataAdapter(strSQL, myConnection)
            //	    myDataSet = New Dataset()
        }

        public static string GetLoggingMessageFromCommand(SqlCommand sqlCmd)
        {
            var SpCommandText = new StringBuilder();
            var counter = 0;
            if (sqlCmd.CommandType == CommandType.StoredProcedure)
            {
                SpCommandText.AppendFormat("EXEC {0} ", sqlCmd.CommandText);
                SpCommandText.AppendLine();
                var totalParams = sqlCmd.Parameters.Count;

                foreach (System.Data.SqlClient.SqlParameter param in sqlCmd.Parameters)
                {
                    counter++;
                    string comma = counter < totalParams ? ", " : "";
                    string value = "";
                    //if(param.DbType == )
                    value = string.Format("'{0}'", param.Value);
                    SpCommandText.AppendFormat("{0} = {1}{2}", param.ParameterName, value, comma);  
                }
            }
            else
            {
                SpCommandText.AppendFormat("[Function GetLoggingMessageFromCommand() needs to be updated to support commands of type '{0}'", sqlCmd.CommandType.ToString());
            }

            return SpCommandText.ToString();
        }
    }
}