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

namespace dreamskape.Nickserv
{
    public class Account 
    {
        public string User;
        public string Password;
        public int registerDate;
        public string Email;
        public bool show;
        public string[] Groups;
        public string Vhost;
        public User user;
        public static Client ns = NickServ.nickserv;
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
            Console.WriteLine(this.user.nickname + " LLLLL");
            Console.WriteLine(ns.nickname + "LLLOL");
            try
            {
                Console.WriteLine(password);
                if (NickDatabase.sha256(password) == Password)
                {
                    Console.WriteLine("login sucessful");
                    ns.noticeUser(this.user, "Login sucessful");
                    this.user.loggedIn = true;
                    UserEvent e = new UserEvent(this.user);
                    Module.callHook(Hooks.USER_IDENTIFY, null, e);
                }
                else
                {
                    Console.WriteLine("login unsucessful");
                    ns.noticeUser(this.user, Convert.ToChar(2) + "Invalid password.");
                    UserEvent e = new UserEvent(this.user);
                    Module.callHook(Hooks.USER_IDENTIFY_FAIL, null, e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void register(string password)
        {
            this.Password = sha256(password);
            NickDatabase.register(this, Password);
            ns.noticeUser(this.user, "Registration sucessful.");
            UserEvent e = new UserEvent(this.user);
            Module.callHook(Hooks.USER_REGISTER, null, e);
            
        }
        public void drop(string password)
        {
            string hashedpass = sha256(password);
            if (this.Password != hashedpass)
            {
                ns.noticeUser(this.user, Chars.bold + "Error: Invalid password");
                return;
            }
            NickDatabase.drop(this, hashedpass);
            ns.noticeUser(this.user, "Your account has been dropped sucessfully.");
        }
    }
}
