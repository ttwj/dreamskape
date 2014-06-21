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
        REGISTER_ALREADY_REGISTERED,
        REGISTER_SUCCESS,
        REGISTER_INVALID_EMAIL,

        LOGIN_INVALID,
        LOGIN_NOT_REGISTERED,
        LOGIN_SUCCESS,

        UNKNOWN,
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
        public AccountEvent login(string password)
        {
            try
            {
                Console.WriteLine(password);
                if (!NickDatabase.isRegistered(this.user))
                {
                    return AccountEvent.LOGIN_NOT_REGISTERED;
                }
                else if (NickDatabase.sha256(password) == Password)
                {
                    Console.WriteLine("login sucessful");
                    this.user.loggedIn = true;
                    UserEvent e = new UserEvent(this.user);
                    Module.callHook(Hooks.USER_IDENTIFY, null, e);
                    return AccountEvent.LOGIN_SUCCESS;
                }
                else
                {
                    Console.WriteLine(this.user.nickname + " logged in unsucessfully.");
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
                    return AccountEvent.LOGIN_INVALID;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem: " + e.ToString());
                
            }
            return AccountEvent.UNKNOWN;
        }

        public AccountEvent register(string password)
        {
            try
            {
                if (NickDatabase.isRegistered(user))
                {
                    return AccountEvent.REGISTER_ALREADY_REGISTERED;
                }
                this.Password = sha256(password);
                this.User = this.user.nickname.ToLower();
                Console.WriteLine(this.User + "BLAH");
                UserEvent e = new UserEvent(this.user);
                Module.callHook(Hooks.USER_REGISTER, null, e);
                NickDatabase.createAccount(this);
                return AccountEvent.REGISTER_SUCCESS;
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem: " + e.ToString());
            }
            return AccountEvent.UNKNOWN;
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
