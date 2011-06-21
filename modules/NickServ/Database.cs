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
        public static Dictionary<string, Account> NickAccounts;
        public static void loadRegistered()
        {
            NickAccounts = new Dictionary<string, Account>();
            DataTable userTable = Database.ExecuteQuery("SELECT * FROM users");
            int count = 0;
            foreach (DataRow row in userTable.Rows)
            {
                Account account = new Account(null);
                account.Password = row["password"].ToString();
                account.User = row["name"].ToString();
                NickAccounts.Add(account.User, account);
                Console.WriteLine(row["name"].ToString().ToLower() + " :  " + row["password"].ToString());
                count++;
            }
            Console.WriteLine("Loaded " + count + " account entries!");
        }
        public static Account getAccountFromUser(User user)
        {
            if (NickAccounts.ContainsKey(user.nickname.ToLower()))
            {
                return NickAccounts[user.nickname.ToLower()];
            }
            return null;
        }
        public static bool isRegistered(User user)
        {
            return NickAccounts.ContainsKey(user.nickname.ToLower());
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
        public static void register(Account account, string hashedpass)
        {
            User usr = account.user;
            using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO users (name,password) VALUES(@username, @pass)";
                    cmd.Parameters.AddWithValue("@username", usr.nickname.ToLower());
                    cmd.Parameters.AddWithValue("@pass", hashedpass);
                    cnn.Open();
                    account.Password = hashedpass;
                    account.User = account.user.nickname.ToLower();
                    NickAccounts.Add(account.User, account);
                    
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                }
            }
        }
        public static void drop(Account usr, string hashedpass)
        {
            string user = usr.user.nickname.ToLower();
            using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM users (name,password) WHERE user = '@user' AND password = '@password'";
                    cmd.Parameters.AddWithValue("@user", user.ToLower());
                    cmd.Parameters.AddWithValue("@password", hashedpass);
                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                }
            }
        }
    }
}
