using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace HangfireWorker.SQLDatabase
{
    public class SqlDatabaseConnection : ISqlDatabaseConnection
    {
        public SqlConnection CreateConnection()
        {
            SqlConnection connection = new SqlConnection("Server = database; Database = HospitalBDatabase; User = sa; Password = Pa&&word2020");
            connection.Open();
            return connection;
        }
    }
}
