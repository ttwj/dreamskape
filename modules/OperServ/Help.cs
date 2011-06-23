using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Users;

namespace dreamskape.Operserv
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
           OperServ.os.noticeUser(user, Convert.ToChar(2) + "------ OperServ Help ------");
           OperServ.os.noticeUser(user, "For information on a command type " + Convert.ToChar(2) + "/msg OperServ help <command>");
           OperServ.os.noticeUser(user, " ");
           OperServ.os.noticeUser(user, "The follow commands are avaliable: ");
            foreach (KeyValuePair<string, string> key in HelpDict)
            {
                OperServ.os.noticeUser(user, Convert.ToChar(2) + key.Key + Convert.ToChar(2) + "        " + key.Value);
            }
            OperServ.os.noticeUser(user, Convert.ToChar(2) + "------ End of Help ------");
        }
    }
}
