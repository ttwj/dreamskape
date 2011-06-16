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
        public Dictionary<string, User> Users;
        public Channel(string name)
        {
            this.name = name;
            Users = new Dictionary<string, User>();
            Program.Channels.Add(name, this);
        }
        public void addChannel(User user)
        {
            Users.Add(user.UID, user);
        }
        public void removeChannel(User user)
        {
            if (Users.ContainsValue(user))
            {
                Users.Remove(user.UID);
                return;
            }
            Console.WriteLine("Attempted to remove non-existant user from channel " + this.name);
        }
        public void Kick(User kicker, User kickee, string reason)
        {
            Protocol.protocolPlugin.kickUser(kicker, kickee, this, reason);
        }
        public void Kick(User kickee, string reason)
        {
            Protocol.protocolPlugin.kickUser(null, kickee, this, reason);
        }
    }
}
