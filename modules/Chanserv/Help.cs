using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Users;

namespace dreamskape.Chanserv
{
    public static class Help
    {
        public static Dictionary<string, string> HelpDict;
        public static Dictionary<string, string> SubHelpDict;
        public static void initHelp()
        {
            HelpDict = new Dictionary<string, string>();
        }
        public static void registerHelp(string command, string help)
        {
            HelpDict.Add(command.ToUpper(), help);
        }
        public static void showHelp(User user)
        {
            ChanServ.cs.noticeUser(user, Convert.ToChar(2) + "------ ChanServ Help ------");
            ChanServ.cs.noticeUser(user, "For information on a command type " + Convert.ToChar(2) + "/msg ChanServ help <command>");
            ChanServ.cs.noticeUser(user, " ");
            ChanServ.cs.noticeUser(user, "The follow commands are avaliable: ");
            foreach (KeyValuePair<string, string> key in HelpDict)
            {
                ChanServ.cs.noticeUser(user, Convert.ToChar(2) + key.Key + Convert.ToChar(2) + "        " + key.Value);
            }
            ChanServ.cs.noticeUser(user, Convert.ToChar(2) + "------ End of Help ------");
        }
        public static void registerSubHelp(string sub, string command, string help)
        {
            
        }
    }
}
