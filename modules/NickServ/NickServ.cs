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
        public static Client nickserv;
        public override void Initialize()
        {
            this.registerHook(Hooks.SERVER_BURST_START);
            this.registerHook(Hooks.USER_MESSAGE_CLIENT);
            this.registerHook(Hooks.USER_CONNECT);
            this.registerHook(Hooks.USER_BURST_CONNECT);
            this.registerHook(Hooks.USER_NICKCHANGE);
            //help
            Help.initHelp();
            Help.registerHelp("HELP", "Shows help, duh");
            Help.registerHelp("MEME <channel>", "Shows random MEME");
            Help.registerHelp("REGISTER", "<password> <email>, Registers an account");
            Help.registerHelp("LOGIN", "<password> Logs you in to your account");
            Help.registerHelp("DROP", "<password> Drops your account, NOT " + Convert.ToChar(2) + "DEGROUP");
        }
        public override void onServerBurstStart()
        {
            nickserv = new Client("NickServ", "NickServ", "S", "Nick.Serv", "Nickname Management Services", generateUID());
            nickserv.introduce();
            this.registerClient(nickserv);
        }

        public override void onUserMessageClient(UserMessageEvent ev)
        {
            User user = ev.sender; 
            string message = ev.message;
            string[] messageArray = message.Split(' ');
            try
            {
                switch (messageArray[0].ToUpper())
                {
                    case "HELP":
                        {
                            Help.showHelp(user);
                            break;
                        }
                    case "MEME":
                        {
                            if (messageArray[1].Length > 1)
                            {
                                Channel chan = getChannelFromName(messageArray[1]);
                                if (chan != null)
                                {
                                    nickserv.noticeUser(user, Convert.ToChar(2) + "NO SHITZ HERE FOOL!");
                                    return;
                                }
                            }
                            nickserv.noticeUser(user, "MEME: " + Convert.ToChar(2) + webstuff.meme());
                            break;
                        }
                    case "REGISTER":
                        {
                            if (messageArray.Length < 3)
                            {
                                Console.WriteLine("wtf register fail?");
                                nickserv.noticeUser(user, "Invalid syntax");
                                string help;

                                Help.HelpDict.TryGetValue("REGISTER", out help);
                                nickserv.noticeUser(user, help);
                                return;
                            }
                            else if (NickDatabase.isRegistered(user))
                            {
                                nickserv.noticeUser(user, Convert.ToChar(2) + "Error: This nickname is already registered.");
                            }
                            else
                            {
                                Account account = new Account(user);
                                account.register(messageArray[1]);
                            }
                            break;
                        }
                    case "LOGIN":
                        {
                            if (messageArray.Length < 2)
                            {
                                nickserv.noticeUser(user, "Invalid syntax");
                                string help;
                                Help.HelpDict.TryGetValue("LOGIN", out help);
                                nickserv.noticeUser(user, help);
                                return;
                            }
                            else if (!NickDatabase.isRegistered(user)) {
                                nickserv.noticeUser(user, Chars.bold + "Error: This nickname is not registered");
                                nickserv.noticeUser(user, "Use /msg NickServ REGISTER to register");
                                return;
                            }
                            else if (user.loggedIn)
                            {
                                nickserv.noticeUser(user, Chars.bold + "You are already logged in.");
                            }
                            else
                            {
                                Account account = NickDatabase.getAccountFromUser(user);
                                Console.WriteLine(account.Password + " durr");
                                account.user = user;
                                account.login(messageArray[1]);
                            }
                            break;
                        }
                    case "DROP":
                        {
                            if (messageArray.Length < 3)
                            {
                                nickserv.noticeUser(user, "Not implemented yet");
                            }
                            break;
                        }
                    case "VHOST":
                        {
                            switch (messageArray[1])
                            {
                                case "REQUEST":
                                    {
                                        break;
                                    }
                                case "ON":
                                    {
                                        break;
                                    }
                                case "OFF":
                                    {
                                        break;
                                    }
                                default:
                                    {
                                        nickserv.noticeUser(user, "Unknown command, " + Chars.bold + "/msg NickServ HELP VHOST" + Chars.bold + " for a listing of commands.");
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            nickserv.noticeUser(user, "Unknown command, " + Chars.bold + "/msg NickServ HELP" + Chars.bold + " for a listing of commands.");
                            break;
                        }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("FAIL " + e.ToString());  
            }
            
        }
        public override void onUserNickChange(UserNickChangeEvent ev)
        {
            User user = ev.user;
            if (NickDatabase.isRegistered(user))
            {
                nickserv.noticeUser(user, "This account is registered, type");
                nickserv.noticeUser(user , Chars.bold + "/msg NickServ LOGIN <password>" + Chars.bold + " to login");
                Account account = NickDatabase.getAccountFromUser(user);
                account.user = user;
            }
        }
        public override void onUserBurstConnect(UserEvent ev)
        {
            User user = ev.user;
            if (NickDatabase.isRegistered(user))
            {
                nickserv.noticeUser(user, "This account is registered, type");
                nickserv.noticeUser(user, Chars.bold + "/msg NickServ LOGIN <password>" + Chars.bold + " to login");
                Account account = NickDatabase.getAccountFromUser(user);
                account.user = user;
            }
        }
        public override void onUserConnect(UserEvent ev)
        {
            User user = ev.user;
            if (NickDatabase.isRegistered(user))
            {
                Account account = NickDatabase.getAccountFromUser(user);
                account.user = user;
                Console.WriteLine(account.user.nickname + "HIZ");
                nickserv.noticeUser(user, "This account is registered, type");
                nickserv.noticeUser(user, Chars.bold + "/msg NickServ LOGIN <password>" + Chars.bold + " to login");
                
            }
        }
    }
}
