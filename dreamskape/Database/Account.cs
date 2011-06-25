using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using dreamskape.Modules;
using dreamskape.Modules.Events;
using dreamskape.Databases;
using dreamskape.Users;
using System.Security.Cryptography;
using System.Runtime.Serialization;


namespace dreamskape.Databases
{
    public enum AccountEvent
    {
    }
    [Serializable]
    public class Account
    {
        public string User;
        public string Password;
        public int registerDate;
        public string Email;
        public bool show;
        public string[] Groups;
        public string Vhost;
        [NonSerialized]
        public User user;
        public static Client ns;
        public Account(User user)
        {
            this.user = user;
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
            try
            {
                Console.WriteLine(password);
                if (NickDatabase.sha256(password) == Password)
                {
                    foreach (KeyValuePair<string, Client> k in Program.Clients)
                    {
                        if (k.Key.ToLower() == "nickserv")
                        {
                            ns = k.Value;
                        }
                    }
                    Console.WriteLine("login sucessful");
                    ns.noticeUser(this.user, "Login sucessful");
                    this.user.loggedIn = true;
                    UserEvent e = new UserEvent(this.user);
                    Module.callHook(Hooks.USER_IDENTIFY, null, e);
                }
                else
                {
                    Console.WriteLine(this.user.nickname + " logged in unsucessfully.");
                    ns.noticeUser(this.user, Convert.ToChar(2) + "Invalid password.");
                    UserEvent e = new UserEvent(this.user);
                    if (user.loginAttempts == 0)
                    {
                        user.loginAttempts = 1;
                    }
                    else
                    {
                        this.user.loginAttempts++;
                    }
                    Module.callHook(Hooks.USER_IDENTIFY_FAIL, null, e);

                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.GetType());
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Data);
                
            }
        }

        public void register(string password)
        {
            this.Password = sha256(password);
            this.User = this.user.nickname.ToLower();
            Console.WriteLine(this.User + "BLAH");
            ns.noticeUser(this.user, "Registration sucessful.");
            UserEvent e = new UserEvent(this.user);
            Module.callHook(Hooks.USER_REGISTER, null, e);
            NickDatabase.createAccount(this);
        }
        public void drop(string password)
        {
            string hashedpass = sha256(password);
            if (this.Password != hashedpass)
            {
                ns.noticeUser(this.user, Chars.bold + "Error: Invalid password");
                return;
            }
            //NickDatabase.drop(this, hashedpass);
            ns.noticeUser(this.user, "Your account has been dropped sucessfully.");
        }
    }
}
