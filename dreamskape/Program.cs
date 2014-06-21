using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using dreamskape.Channels;
using dreamskape.Users;
using dreamskape.Proto;
using dreamskape.Modules;
using System.Data;
using dreamskape.Databases;

namespace dreamskape
{
    public static class Program
    {
        public static string protocol = "Charybdis.dll";
        public static Dictionary<string,Channel> Channels;
        public static Dictionary<string, User> Users;
        public static Dictionary<string, Client> Clients;
        public static string SID = "32X";
        public static void Main(string[] args)
        {
            Users = new Dictionary<string, User>();
            Channels = new Dictionary<string, Channel>();
            Clients = new Dictionary<string, Client>();
            Protocol.loadPlugins();
            ModuleManager.loadPlugins();
            ProtocolPlugin p = Protocol.protocolPlugin;
			p.Init("127.0.0.1", "services.int", 6667, "pvps1234", SID);
            Database.Init();
            ModuleManager.InitModules();
            Protocol.protocolPlugin.Connect();
        }
        public static Client getClientFromNick(string nick)
        {
            foreach (KeyValuePair<string, Client> k in Clients)
            {
                if (nick.ToLower() == k.Key.ToLower())
                {
                    return k.Value;
                }
            }
            return null;
        }
		public static void Shutdown() {
			ModuleManager.Shutdown ();
			System.Environment.Exit(0);
		}
    }
}
