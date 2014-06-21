using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Modules.Events;
using dreamskape.Channels;
using dreamskape.Modules;
using dreamskape.Users;
using dreamskape.Proto;
using dreamskape;
using System.Net;
using System.Data;
using dreamskape.Databases;
using ServiceStack.OrmLite;


namespace dreamskape.Nickserv
{
    public static class webstuff
    {
        public static string meme()
        {
            using (WebClient client = new WebClient())
            {
                client.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.X.Y.Z Safari/525.13.";
                string value = client.DownloadString("http://api.autome.me/text?lines=1");

                // Write values.
                return value;
            }
        }
    }
    public class NickServ : ModulePlugin
    {
		/*database stuff*/
		public static NickEntry getNickEntry(User user) {
			NickEntry nickEntry = NickServ.getInstance().getDB().Single<NickEntry> (x => x.Nick == user.nickname.ToLower ());
			return nickEntry;
		}



        public static Client nickserv;
		public override string Name {
			get {
				return "NickServ";
			}
		}
        public override void Initialize()
        {
			base.Initialize ();

			nickserv = new Client("NickServ", "NickServ", "S", "Nick.Serv", "Nickname Management Services", generateUID());
			this.registerClient(nickserv);

            this.registerHook(Hooks.SERVER_BURST_START);
            this.registerHook(Hooks.USER_MESSAGE_CLIENT);
            this.registerHook(Hooks.USER_CONNECT);
            this.registerHook(Hooks.USER_BURST_CONNECT);
            this.registerHook(Hooks.USER_NICKCHANGE);
			Console.WriteLine ("trying to initialize");
			Register registerCommand = new Register ();
			Console.WriteLine ("initialized new command");
			this.registerCommand ((CommandExecutor)registerCommand);

            //help
			/*HelpManager.initHelp();
            HelpManager.registerHelp("HELP", "Shows help, duh");
            HelpManager.registerHelp("MEME <channel>", "Shows random MEME");
            HelpManager.registerHelp("REGISTER", "<password> <email>, Registers an account");
            HelpManager.registerHelp("LOGIN", "<password> Logs you in to your account");
            HelpManager.registerHelp("DROP", "<password> Drops your account, NOT " + Convert.ToChar(2) + "DEGROUP");

		
			this.registerCommand("HELP", new Func<string[], UserMessageEvent, bool>( (args, ev) => {
				HelpManager.showHelp(ev.sender);
				return true;
			}));
			this.registerCommand("REGISTER", new Func<string[], UserMessageEvent, bool>( (args, ev) => {
				User user = ev.sender;

				return true;
			}));*/
        }



        public override void onServerBurstStart()
        {
            nickserv.introduce();
        }


		public override void loadDatabase(IDbConnection db) {
			Console.WriteLine ("nickserv load database wow!");
		}

		public override void initDatabase(IDbConnection db) {
			Console.WriteLine ("trying to initialize db..");
			db.CreateTable<NickEntry> ();
		}


        public override void onUserMessageClient(UserMessageEvent ev)
        {
			base.onUserMessageClient (ev);
        }
        public override void onUserNickChange(UserNickChangeEvent ev)
        {
			promptLogin (ev.user);
        }
		private void promptLogin(User user) {
			debugServices ("Promting " + user.nickname + " to login");
			if (getNickEntry(user) != null)
			{
				nickserv.noticeUser(user, "This account is registered, type");
				nickserv.noticeUser(user, Chars.bold + "/msg NickServ LOGIN <password>" + Chars.bold + " to login");
			}
		}
        public override void onUserBurstConnect(UserEvent ev)
        {
			promptLogin (ev.user);

        }
        public override void onUserConnect(UserEvent ev)
        {
			promptLogin (ev.user);
        }

    }

}
