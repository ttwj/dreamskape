using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Users;

namespace dreamskape.Modules
{
    public class HelpManager
    {
		public Dictionary<string, CommandExecutor> HelpDict;
		public Client client;
		public HelpManager(Client client)
        {
			this.client = client;
			HelpDict = new Dictionary<string, CommandExecutor> ();
        }
		public void registerHelp(CommandExecutor command)
        {
			HelpDict.Add(command.Name.ToUpper(), command);
        }
        public void showHelp(User user) 
        {

			client.noticeUser(user, Convert.ToChar(2) + "------ " + client.nickname + " Help ------");
			client.noticeUser(user, "For information on a command type " + Convert.ToChar(2) + "/msg " + client.nickname + " help <command>");
			client.noticeUser(user, " ");
			client.noticeUser(user, "The follow commands are avaliable: ");
			foreach (KeyValuePair<string, CommandExecutor> key in HelpDict)
            {
				client.noticeUser(user, Convert.ToChar(2) + key.Key + Convert.ToChar(2) + "        " + key.Value.HelpInfo);
            }
			client.noticeUser(user, Convert.ToChar(2) + "------ End of Help ------");
        }
		public void showCommandHelp(User user, String command) {
			if (HelpDict.ContainsKey (command)) {
				if (!HelpDict [command].onHelpCommand(user)) {
					client.noticeUser (user, "No help for " + command);
				}
			}
		}


    }
}
