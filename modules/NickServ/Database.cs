using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Databases;
using dreamskape.Channels;
using dreamskape.Users;
using System.Data;
using System.Security.Cryptography;

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
            try
            {
                string account = user.ToLower();
                string hashedpass = sha256(password + "salt");
                DataTable userTable = Database.ExecuteQuery("INSERT INTO users VALUES (" + account + ", " + hashedpass + ")");
                NickAuths.Add(account, hashedpass);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(user + " registered despite having registered.");
                Console.WriteLine(e.ToString());
            }
        }
        public static void drop(string user, string password)
        {
            DataTable userTable = Database.ExecuteQuery("DELETE FROM users WHERE name = '" + sha256(password) + "';");
        }
    }
}
