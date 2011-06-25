using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Channels;
using dreamskape.Users;
using System.Data;
using System.Security.Cryptography;
using System.Data.SQLite;
using System.IO;

namespace dreamskape.Databases
{

    
    public class NickDatabase : Database
    {
        public static Dictionary<string, Account> NickAccounts;
        public static Dictionary<string, Account> NickGroups;

        public static void loadRegistered()
        {
            NickAccounts = new Dictionary<string, Account>();
            DataTable userTable = ExecuteQuery("SELECT * FROM users");
            int count = 0;
            foreach (DataRow row in userTable.Rows)
            {
                //get the object, durr
                Account acc = (Account)BinaryDeserialize(row["data"].ToString());
                NickAccounts.Add(acc.User, acc);
                Console.WriteLine(row["user"].ToString().ToLower() + " :  " + row["data"].ToString());
                count++;
            }
            Console.WriteLine("Loaded " + count + " account entries!");
        }
        public static void createAccount(Account account)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
                {
                    using (SQLiteCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO users (user,data) VALUES(@user, @data);";
                        cmd.Parameters.AddWithValue("@user", account.User);
                        cmd.Parameters.AddWithValue("@data", BinarySerialize(account));
                        Console.WriteLine("data: " + BinarySerialize(account));
                        NickAccounts.Add(account.User, account);
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem: " + e.ToString());
            }
        }
        public static void updateAccount(Account account)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
                {
                    using (SQLiteCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE users SET data=@data WHERE user=@user;";
                        cmd.Parameters.AddWithValue("@user", account.User);
                        cmd.Parameters.AddWithValue("@data", BinarySerialize(account));
                        Console.WriteLine("data: " + BinarySerialize(account));
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem: " + e.ToString());
            }
        }
        
        public static Account getAccountFromUser(User user)
        {
            Account account;
            NickAccounts.TryGetValue(user.nickname.ToLower(), out account);
            return account;
        }
        public static bool isRegistered(User user)
        {
            return NickAccounts.ContainsKey(user.nickname.ToLower());
        }
      
    }
}
