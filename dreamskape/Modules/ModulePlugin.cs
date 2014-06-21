using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using dreamskape.Modules.Events;
using dreamskape.Users;
using dreamskape.Channels;
using System.Data;
using ServiceStack.OrmLite;


namespace dreamskape.Modules
{
    public abstract class ModulePlugin
    {
		private static ModulePlugin instance; 
		private static Channel Services;

		public static ModulePlugin getInstance() {
			return instance;
		}

		public virtual String Name {
			get {
				return "No Name Module";
			}
		}
		public HelpManager Help;

		public ArrayList moduleHooks = new ArrayList();

		private Dictionary<String, CommandExecutor> UserCommands = new Dictionary<String, CommandExecutor> ();
		private Dictionary<String, CommandExecutor> ChannelCommands = new Dictionary<String, CommandExecutor> ();
		public Client moduleClient;
		private IDbConnection db;
		public void registerClient(Client client)
        {
			if (moduleClient == null) {
				this.moduleClient = client;
				Help = new HelpManager (client);
			}
        }
        public void registerHook(Hooks hook)
        {
            if (!moduleHooks.Contains(hook))
            {
                moduleHooks.Add(hook);
                Console.WriteLine(hook + " added");
            }
        }
        public static int getTimeStamp()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            int timestamp = (int)t.TotalSeconds;
            return timestamp;
        }
        public string generateUID()
        {
            return Program.SID + Proto.ProtocolPlugin.generateUID();
        }
        public User getUserFromUID(string UID)
        {
            if (Program.Users.ContainsKey(UID))
            {
                User user;
                Program.Users.TryGetValue(UID, out user);
                return user;
            }
            return null;
        }
        public Channel getChannelFromName(string channel)
        {
            if (Program.Channels.ContainsKey(channel.ToLower()))
            {
                Channel chan;
                Program.Channels.TryGetValue(channel.ToLower(), out chan);
                return chan;
            }
            return null;
        }
		public virtual void Initialize() {
			instance = this;
			this.registerHook (Hooks.SERVER_BURST_END);
		}
		public virtual void Shutdown() {
			
		}
		public virtual void onUserMessageChannel(ChannelMessageEvent ev) { 
			Console.WriteLine ("user message channel!");
			//processCommand (ev);
		}
        public virtual void onUserMessageClient(UserMessageEvent ev) { 
			Console.WriteLine ("user message client !");
			processCommand (ev);
		}
        public virtual void onUserPartChannel() { }
        public virtual void onClientKilled(KillEvent ev) { }
        public virtual void onClientKill(KillEvent ev) { }
        public virtual void onUserConnect(UserEvent ev) { }
        public virtual void onUserNickChange(UserNickChangeEvent ev) { }
        public virtual void onChannelLog(ChannelLogEvent ev) { }
        public virtual void onClientIntroduce(ClientIntroduceEvent ev) { }
        public virtual void onServerBurstStart() { }
        public virtual void onServerBurstEnd() { 
			Services = getChannelFromName("#services");
			if (Services == null)
			{
				Services = new Channel("#services", getTimeStamp());
			}
		}

        public virtual void onUserIdentify(UserEvent ev) { }
        public virtual void onUserIdentifyFail(UserEvent ev) { }

        public virtual void onUserBurstConnect(UserEvent ev) { }

		public virtual void loadDatabase(IDbConnection db) {

		}

		public virtual void initDatabase(IDbConnection db) {
			db.Dispose ();
		}

		/*public void registerCommand(String Command, Func<string[], ChannelMessageEvent, bool> Function) {
			ChannelCommands.Add (Command.ToUpper(), Function);
		}*/

		public void registerCommand(CommandExecutor Command) {
			Help.registerHelp (Command);
			Console.WriteLine ("adding new user command " + Command.Name);
			UserCommands.Add (Command.Name.ToUpper(), Command);
			Console.WriteLine ("adding new user command ok " + Command.Name);
		}

		/*private void processCommand(ChannelMessageEvent ev) {
			Console.WriteLine ("processing channel command");
			string[] messageArray = ev.message.Split(' ');
			string command = messageArray [0].ToUpper();
			if (ChannelCommands.ContainsKey (command)) {
				bool success = (bool) ChannelCommands [command].DynamicInvoke (messageArray, ev);
			}
		}*/
		public void debugServices(String message) {

			this.moduleClient.messageChannel (Services, message);
		}
		private void processCommand(UserMessageEvent ev) {
			Console.WriteLine ("processing user command");
			Console.WriteLine (UserCommands.Keys);

			string[] messageArray = ev.message.Split(' ');
			string command = messageArray [0].ToUpper();
			if (command == "HELP") {
				Help.showHelp (ev.sender);
			} else {
				if (UserCommands.ContainsKey (command)) {
					Console.WriteLine ("executing command " + command);
					bool success = UserCommands [command].onUserCommand (command, messageArray, ev);
					if (!success) {
						Console.WriteLine ("command failed!");
						Help.showCommandHelp (ev.sender, command);
					}
				}
			}
			Console.WriteLine ("command " + command);
	

		}

		public IDbConnection getDB() {
			if (db == null) {
				db = Databases.Database.dbFactory.OpenDbConnection ();
			}
			return db;
		}
    }
}
