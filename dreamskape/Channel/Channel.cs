using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using dreamskape.Users;
using dreamskape.Proto;

namespace dreamskape.Channels
{
    public class Channel
    {
        public string name;
        public ArrayList users;
        public Channel(string name, ArrayList users)
        {
            this.name = name;
            this.users = users;
            Program.Channels.Add(name, this);
        }

        public void Kick(User kicker, User kickee, string reason)
        {
            
        }
        public void Kick(User kicked, string reason)
        {
        }
    }
}
