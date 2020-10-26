using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace HangfireWorker.SQLDatabase
{
    public interface ISqlDatabaseConnection
    {
        SqlConnection CreateConnection();
    }
}
