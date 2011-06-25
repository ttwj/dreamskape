using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Modules;
using dreamskape.Modules.Events;
using dreamskape.Users;
using dreamskape.Channels;
using dreamskape.Proto;
using System.Text.RegularExpressions;

namespace dreamskape.Operserv
{
    public class OperServ : ModulePlugin
    {
        public static Client os;
        public string channel = "#services";
        public Channel services;
        public Client ns;
        //slaving another client ftw.
        public override void Initialize()
        {
            
            this.registerHook(Hooks.SERVER_BURST_START);
            this.registerHook(Hooks.SERVER_BURST_END);
            this.registerHook(Hooks.USER_MESSAGE_CLIENT);
            this.registerHook(Hooks.USER_IDENTIFY);
            this.registerHook(Hooks.USER_IDENTIFY_FAIL);
            this.registerHook(Hooks.USER_CONNECT);
            this.registerHook(Hooks.CLIENT_INTRO);
            Help.initHelp();
            Help.registerHelp("HELP", "Shows help - duh");
            Help.registerHelp("SHUTDOWN", "Shutdown services :O!");
            
            
        }
        public override void onServerBurstStart()
        {
            os = new Client("OperServ", "OperServ", "S", "Oper.Serv", "Operator Management Services", generateUID());
            os.introduce();
            this.registerClient(os);
        }
        public override void onUserConnect(UserEvent ev)
        {
            User user = ev.user;
            os.messageChannel(services, Convert.ToChar(2) + user.nickname + "(" + user.UID + ")" + Convert.ToChar(2) + " has connected");
        }
        
        public override void onUserIdentify(UserEvent ev)
        {
            User user = ev.user;
            ns.messageChannel(services, "Sucessful login by " + Convert.ToChar(2) + user.nickname + Convert.ToChar(2));
        }
        public override void onUserIdentifyFail(UserEvent ev)
        {
            User user = ev.user;
            Console.WriteLine("blah blah blah");
            ns.messageChannel(services, Convert.ToChar(2) + user.loginAttempts + Convert.ToChar(2) + " failed login attempts by " + Convert.ToChar(2) + user.nickname + Convert.ToChar(2));
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
                    case "SHUTDOWN":
                        {
                            if (user.modes.Contains('o'))
                            {
                                Protocol.protocolPlugin.Send("QUIT: bai bai");
                                System.Environment.Exit(0);
                            }
                            else
                            {
                                os.noticeUser(user, "You do not have permission to perform this task.");
                                os.messageChannel(services, "NP " + Convert.ToChar(2) + user.nickname + "SHUTDOWN");
                            }
                            break;
                        }
                    case "RAW":
                        {
                            if (user.modes.Contains('o'))
                            {
                                string raw = message.Remove(0, 4).Replace(';', ':');
                                os.messageChannel(services, user.nickname + "RAW: " + raw);
                                Protocol.protocolPlugin.Send(raw);
                                //Protocol.protocolPlugin.Send(message.Remove(0,4));
                            }
                            else
                            {
                                os.noticeUser(user, "You do not have permission to perform this task.");
                                os.messageChannel(services, "NP " + Convert.ToChar(2) + user.nickname + "RAW");
                            }
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("FAIL " + e.ToString());
            }

        }
        public override void onServerBurstEnd()
        {
            services = getChannelFromName(channel);
            if (services == null)
            {
                services = new Channel(channel, getTimeStamp());
            }
            foreach (KeyValuePair<string, Client> c in Program.Clients)
            {
                c.Value.joinChannelMode(services, c.Value, "+o");
                if (c.Value.nickname.ToLower() == "nickserv")
                {
                    ns = c.Value;
                }
            }
        }
    }
}