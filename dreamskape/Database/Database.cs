using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace dreamskape.Databases
{
    public class Database
    {
        public static void Init()
        {
            if (!File.Exists("services.db"))
            {
                SQLiteConnection.CreateFile("services.db");

                using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
                {
                    using (SQLiteCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "CREATE TABLE users (user TEXT, data TEXT); CREATE TABLE channels (channel TEXT, data TEXT);";
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                    }
                }
            }
        }
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
        public static string BinarySerialize(object item)
        {
            if (item != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, item);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            return null;
        }

        public static object BinaryDeserialize(string item)
        {
            if (item != null && item.Length > 0)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(item);
                if (buffer != null && buffer.Length > 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(item)))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        return binaryFormatter.Deserialize(memoryStream);
                    }
                }
            }
            return null;
        }
        public static string sha256(string password)
        {
            SHA256Managed crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(password), 0, Encoding.ASCII.GetByteCount(password));
            foreach (byte bit in crypto)
            {
                hash += bit.ToString("x2");
            }
            return hash;
        }
    }
}
