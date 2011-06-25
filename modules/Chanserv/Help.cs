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
            NickServ.cs.noticeUser(user, Convert.ToChar(2) + "------ ChanServ Help ------");
            NickServ.cs.noticeUser(user, "For information on a command type " + Convert.ToChar(2) + "/msg ChanServ help <command>");
            NickServ.cs.noticeUser(user, " ");
            NickServ.cs.noticeUser(user, "The follow commands are avaliable: ");
            foreach (KeyValuePair<string, string> key in HelpDict)
            {
                NickServ.cs.noticeUser(user, Convert.ToChar(2) + key.Key + Convert.ToChar(2) + "        " + key.Value);
            }
            NickServ.cs.noticeUser(user, Convert.ToChar(2) + "------ End of Help ------");
        }
    }
}
