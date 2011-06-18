using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using dreamskape.Databases;

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
        public Account(string User, string Password, int registerDate, string Email, string[] Groups, string Vhost)
        {
            this.User = User;
            this.Password = Password;
            this.registerDate = registerDate;
            this.Email = Email;
            this.Groups = Groups;
            this.Vhost = Vhost;
            this.register();
        }
        public void register()
        {
            NickDatabase.register(this.User, this.Password);
        }
        public void drop()
        {
            
        }
    }
}
