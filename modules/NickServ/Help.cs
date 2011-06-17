using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Users;

namespace dreamskape.Modules
{
    public static class Help
    {
        private static Dictionary<string, string> HelpDict;
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
            NickServ.nickserv.noticeUser(user, Convert.ToChar(2) + "------ NickServ Help ------");
            NickServ.nickserv.noticeUser(user, "For information on a command type " + Convert.ToChar(2) + "/msg NickServ help <command>");
            NickServ.nickserv.noticeUser(user, " ");
            NickServ.nickserv.noticeUser(user, "The follow commands are avaliable: ");
            foreach (KeyValuePair<string, string> key in HelpDict)
            {
                NickServ.nickserv.noticeUser(user, Convert.ToChar(2) + key.Key + Convert.ToChar(2) + "        " + key.Value);
            }
            NickServ.nickserv.noticeUser(user, Convert.ToChar(2) + "------ End of Help ------");
        }
    }
}
