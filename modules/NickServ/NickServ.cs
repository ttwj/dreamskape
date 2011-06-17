using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Modules.Events;
using dreamskape.Channels;
using dreamskape.Users;
using dreamskape.Proto;
using dreamskape;
using System.Net;

namespace dreamskape.Modules
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
            nickserv = new Client("NickServ", "NickServ", "a", "Nick.Serv", "Nickname management service", generateUID());
            nickserv.introduce();
            this.registerClient(nickserv);
            this.registerHook(Hooks.USER_MESSAGE_CLIENT);
            //help
            Help.initHelp();
            Help.registerHelp("HELP", "Shows help, duh");
            Help.registerHelp("MEME <channel>", "Shows random MEME");
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
        public override void onUserMessageClient(UserMessageEvent ev)
        {
            User user = ev.sender; 
            string message = ev.message;
            string[] messageArray = message.Split(' ');
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
            }
            
        }
    }
}
