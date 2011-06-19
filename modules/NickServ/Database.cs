using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Channels;
using dreamskape.Users;
using dreamskape.Databases;
using System.Data;
using System.Security.Cryptography;
using System.Data.SQLite;

namespace dreamskape.Nickserv
{
    public class NickDatabase 
    {
        public static Dictionary<string, string> NickAuths;
        public static void loadRegistered()
        {
            NickAuths = new Dictionary<string, string>();
            DataTable userTable = Database.ExecuteQuery("SELECT * FROM users");
            int count = 0;
            foreach (DataRow row in userTable.Rows)
            {
                NickAuths.Add(row["name"].ToString().ToLower(), row["password"].ToString());
                Console.WriteLine(row["name"].ToString().ToLower() + " :  " + row["password"].ToString());
                count++;
            }
            Console.WriteLine("Loaded " + count + " account entries!");
        }
        public static bool isRegistered(string user)
        {
            return NickAuths.ContainsKey(user);
        }
        static string sha256(string password)
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
        public static void register(string user, string password)
        {
            using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO users (name,password) VALUES(@username, @pass)";
                    cmd.Parameters.AddWithValue("@username", user.ToLower());
                    cmd.Parameters.AddWithValue("@pass", sha256(password));

                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                }
            }
        }
        public static void drop(string user, string password)
        {
            using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM users (name,password) WHERE user = '@user' AND password = '@password'";
                    cmd.Parameters.AddWithValue("@user", user.ToLower());
                    cmd.Parameters.AddWithValue("@password", sha256(password));
                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                }
            }
        }
    }
}
