using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Modules;
using dreamskape.Modules.Events;
using dreamskape.Channels;
using dreamskape.Users;
using dreamskape.Proto;

namespace NickServ
{
    public class NickServ : ModulePlugin
    {
        public Client nickserv;
        public override void Initialize()
        {
            nickserv = new Client("NickServ", "NickServ", "a", "Nick.Serv", "Nickname management service", ProtocolPlugin.generateUID());
            nickserv.introduce();
            this.registerClient(nickserv);
            this.registerHook(Hooks.USER_CHANNEL_PRIVMSG);
        }
        
        public override void onUserMessageClient(UserMessageEvent ev)
        {
            User user = ev.sendee; 
            string message = ev.message;
            string[] messageArray = message.Split(' ');
            if (messageArray[0].ToLower() == "help")
            {
                nickserv.noticeUser(user, Convert.ToChar(2) + "NO HALP 4 U FAG! >:D");
            }
        }
    }
}
