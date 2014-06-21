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
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using dreamskape.Nickserv;


namespace dreamskape.Databases
{
    public enum ChannelAccountResult
    {
        USER_NOT_LOGGED_IN,
        REGISTER_SUCESS,
        REGISTER_ALREADY_REGISTERED,

        DROP_SUCCESS,
        DROP_NOPERM,
        DROP_USER_NOT_LOGGED_IN,

        ACCESS_SUCESS,
        ACCESS_NOPERM,

        UNKNOWN,
    }
		
	public class ChannelEntry {
		public string Name { get; set; }
		public int registerDate { get; set; }
		public Dictionary<string, int> Access { get; set; }

		public ChannelEntry() {
			//this.Access = new Dictionary<Account, int>();
		}
	}

	public class ChannelManager
	{
        public Channel channel;
		public ModulePlugin plugin;
		public ChannelManager(Channel channel, ModulePlugin plugin)
        {
            this.channel = channel;
			this.plugin = plugin;
        }

		public ChannelAccountResult register(string password, User user)
        {
            try
            {	
				/*NickManager instance = new NickManager(user.nickname, this.plugin);
				if (!instance.Registered) {
                    return ChannelAccountResult.USER_NOT_LOGGED_IN;
                }*/
				/*if (ChannelDatabase.ChannelAccounts.ContainsKey(Name.ToLower()))
                {
                    return ChannelAccountResult.REGISTER_ALREADY_REGISTERED;
                }*/
                Console.WriteLine("1");

                Console.WriteLine("channel-name " + this.channel.name);

				var channelEntry = new ChannelEntry {
					Name = this.channel.name.ToLower(),
					/*Access = {
						{user.user.nickname, 5}
					},*/
				};
				return ChannelAccountResult.REGISTER_SUCESS;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.ToString());
            }
            return ChannelAccountResult.UNKNOWN;
        }
		/*public ChannelAccountResult drop(string password, Account user)
        {
            if (!user.user.loggedIn)
            {
                return ChannelAccountResult.USER_NOT_LOGGED_IN;
            }
            if (getAccess(user) != 5)
            {
                return ChannelAccountResult.DROP_NOPERM;
            }
            //bye channel :(
            ChannelDatabase.destroyChannel(this);
            return ChannelAccountResult.DROP_SUCCESS;
        }
        public ChannelAccountResult AccountSetAccess(Account user, int access)
        {
            if (Access.ContainsKey(user))
            {
                if (!(int.Parse(Access[user].ToString()) > 3))
                {
                    return ChannelAccountResult.ACCESS_NOPERM;
                }
                Access.Remove(user);
            }
            Access.Add(user, access);
            ChannelDatabase.updateChannel(this);
            return ChannelAccountResult.ACCESS_SUCESS;
        }
        public void setAccess(Account user, int access)
        {
            if (Access.ContainsKey(user))
            {
                Access.Remove(user);
            }
            Access.Add(user, access);
            ChannelDatabase.updateChannel(this);
        }
        public int getAccess(Account user)
        {
            int level;
            Access.TryGetValue(user, out level);
            return level;
        }*/
    }
}
