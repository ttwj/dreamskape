using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using dreamskape.Databases;
using dreamskape.Users;
using System.Security.Cryptography;

namespace dreamskape.Nickserv
{
    public class Account : User
    {
        public string User;
        public string Password;
        public int registerDate;
        public string Email;
        public bool show;
        public string[] Groups;
        public string Vhost;
        public bool LoggedIn;
        public Client ns = NickServ.nickserv;
        public Account(User user) : base (user.nickname, user.username, user.modes, user.hostname, user.gecos, user.UID)
        {
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
        public void login(string password)
        {
            string dbpass;
            NickDatabase.NickAuths.TryGetValue(this.nickname.ToLower(), out dbpass);
            Console.WriteLine(password);
            if (NickDatabase.sha256(password) == dbpass)
            {
                ns.noticeUser((User)this, "Login sucessful");
            }
            else
            {
                ns.noticeUser((User)this, Convert.ToChar(2) + "Invalid password.");
            }
        }
        public void register(string password)
        {
            this.Password = sha256(password);
            NickDatabase.register(this, this.Password);
        }
        public void drop(string password)
        {
            string hashedpass = sha256(password);
            if (this.Password != hashedpass)
            {
                ns.noticeUser((User)this, Chars.bold + "Error: Invalid password");
                return;
            }
            NickDatabase.drop(this, hashedpass);
            ns.noticeUser((User)this, "Your account has been dropped sucessfully.");
        }
    }
}
