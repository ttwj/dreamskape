using System;
using System.Collections;
using System.Collections.Generic;
using ServiceStack.OrmLite;
using dreamskape;
using dreamskape.Channels;
using dreamskape.Databases;
using dreamskape.Modules;
using dreamskape.Modules.Events;
using dreamskape.Users;
using System.Data;

namespace dreamskape.Chanserv
{
    public class ChanServ : ModulePlugin
    {
        public static Client cs;
		public override string Name {
			get {
				return "ChanServ";
			}
		}
        public override void Initialize()
        {

            this.registerHook(Hooks.SERVER_BURST_START);
            this.registerHook(Hooks.SERVER_BURST_END);
            this.registerHook(Hooks.USER_MESSAGE_CLIENT);

		
			//this.registerCommand ("HELP", helpCommand);
		
            //help
			/*Help.initHelp();
            Help.registerHelp("HELP", "Shows help, duh");
            Help.registerHelp("REGISTER", "<channel> <pass> - Registers a channel");
            Help.registerHelp("ACCESS", "Controls the channel access list");
            Help.registerHelp("LOGIN", "Gives you founder-level access to a channel");*/
        }

		private bool helpCommand(string[] commandArray, ChannelMessageEvent ev) {
			Help.showHelp (ev.user);
			return true;
		}

		public override void loadDatabase(IDbConnection db) {
			Console.WriteLine ("chanserv load database wow!");
		}

		public override void initDatabase(IDbConnection db) {
			db.CreateTable<ChannelEntry> ();
		}


        public override void onServerBurstStart()
        {
            cs = new Client("ChanServ", "ChanServ", "S", "Chan.Serv", "Channel Management Services", generateUID());
            cs.introduce();
            this.registerClient(cs);
            
        }
        public override void onServerBurstEnd()
        {
            foreach (KeyValuePair<string, Channel> chan in Program.Channels)
            {
                cs.joinChannelMode(chan.Value, cs, "+o");
            }
        }
        public override void onUserMessageClient(UserMessageEvent ev)
        {
			base.onUserMessageClient (ev);

        }
    }
}