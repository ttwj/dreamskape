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
        public bool registered = false;
        public Dictionary<string, User> Users;
        public int TS;
        public Channel(string name, int TS = 0)
        {
            this.name = name;
            this.TS = TS;
            Users = new Dictionary<string, User>();
            Program.Channels.Add(name.ToLower(), this);
        }
        public void updateTS(int TS)
        {
            this.TS = TS;
        }
        public void addToChannel(User user)
        {
            Users.Add(user.UID, user);
        }
        public void removeFromChannel(User user)
        {
            if (Users.ContainsValue(user))
            {
                Users.Remove(user.UID);
                return;
            }
            Console.WriteLine("Attempted to remove non-existant user from channel " + this.name);
        }
        public bool containsUser(User user)
        {
            if (Users.ContainsValue(user))
            {
                return true;
            }
            return false;
        }

    }
}
