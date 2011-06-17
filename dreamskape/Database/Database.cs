using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace dreamskape.Databases
{
    public static class Database
    {
        public static DataTable ExecuteQuery(string sql)
        {
            // Validate SQL
            if (string.IsNullOrWhiteSpace(sql))
            {
                return null;
            }
            else
            {
                if (!sql.EndsWith(";"))
                {
                    sql += ";";
                }
                SQLiteConnection connection = new SQLiteConnection("Data Source=services.db");
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(connection);
                cmd.CommandText = sql;
                DataTable dt = new DataTable();
                SQLiteDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                connection.Close();
                return dt;
            }
        }
    }
}
