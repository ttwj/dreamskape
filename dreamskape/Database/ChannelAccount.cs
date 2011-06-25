using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using dreamskape.Modules;
using dreamskape.Modules.Events;
using dreamskape.Databases;
using dreamskape.Channels;
using dreamskape.Users;
using System.Security.Cryptography;
using System.Runtime.Serialization;


namespace dreamskape.Databases
{
    public enum ChannelAccountEvent
    {
        USER_NOT_LOGGED_IN,
        REGISTER_SUCESS,
        REGISTER_ALREADY_REGISTERED,

        DROP_SUCCESS,
        DROP_NOPERM,
        DROP_USER_NOT_LOGGED_IN,

        ACCESS_SUCESS,
        ACCESS_NOPERM

    }

    [Serializable]
    public class ChannelAccount
    {
        public string Name;
        public string Password;
        public int registerDate;
        public SerializableDictionary<Account, int> Access;
        [NonSerialized]
        public Channel channel;
        public ChannelAccount(Channel channel)
        {
            this.channel = channel;
        }

        public ChannelAccountEvent register(string password, Account user)
        {
            if (!user.user.loggedIn)
            {
                return ChannelAccountEvent.USER_NOT_LOGGED_IN;
            }
            if (ChannelDatabase.ChannelAccounts.ContainsKey(Name.ToLower())) {
                return ChannelAccountEvent.REGISTER_ALREADY_REGISTERED;
            }
            Console.WriteLine("1");
            Console.WriteLine("channel-name " + this.channel.name);
            /*if (ChannelDatabase.isRegistered(this.channel)) 
            {
                return ChannelAccountEvent.REGISTER_ALREADY_REGISTERED;
            }*/
            Console.WriteLine("dur");
            //set the channel name, owner password etc.
            this.Name = channel.name.ToLower();
            Console.WriteLine("a");
            this.Password = Database.sha256(password);
            Console.WriteLine("ab");
            this.Access = new SerializableDictionary<Account, int>();
            //add access stuff
            Console.WriteLine("abc");
            Access.Add(user, 5);
            //register!
            Console.WriteLine("abcd");
            ChannelDatabase.createChannel(this);
            return ChannelAccountEvent.REGISTER_SUCESS;
        }
        public ChannelAccountEvent drop(string password, Account user)
        {
            if (!user.user.loggedIn)
            {
                return ChannelAccountEvent.USER_NOT_LOGGED_IN;
            }
            if (getAccess(user) != 5)
            {
                return ChannelAccountEvent.DROP_NOPERM;
            }
            //bye channel :(
            ChannelDatabase.destroyChannel(this);
            return ChannelAccountEvent.DROP_SUCCESS;
        }
        public ChannelAccountEvent access(Account user, int access)
        {
            if (Access.ContainsKey(user))
            {
                if (!(int.Parse(Access[user].ToString()) > 3))
                {
                    return ChannelAccountEvent.ACCESS_NOPERM;
                }
                Access.Remove(user);
            }
            Access.Add(user, access);
            ChannelDatabase.updateChannel(this);
            return ChannelAccountEvent.ACCESS_SUCESS;
        }
        public int getAccess(Account user)
        {
            int level;
            Access.TryGetValue(user, out level);
            return level;
        }
    }
}
