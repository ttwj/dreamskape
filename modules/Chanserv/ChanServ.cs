using System;
using dreamskape;
using dreamskape.Users;
using dreamskape.Channels;
using dreamskape.Databases;
using dreamskape.Modules;
using dreamskape.Modules.Events;

namespace dreamskape.Chanserv
{
    public class NickServ : ModulePlugin
    {
        public static Client cs;
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
            Help.registerHelp("REGISTER", "<channel> <pass> - Registers a channel");
        }
        public override void onServerBurstStart()
        {
            cs = new Client("ChanServ", "ChanServ", "S", "Chan.Serv", "Channel Management Services", generateUID());
            cs.introduce();
            this.registerClient(cs);
            
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
                    case "REGISTER":
                        {
                            if (messageArray.Length < 3)
                            {
                                Console.WriteLine("wtf register fail?");
                                cs.noticeUser(user, "Invalid syntax");
                                string help;

                                Help.HelpDict.TryGetValue("REGISTER", out help);
                                cs.noticeUser(user, help);
                                return;
                            }
                            else
                            {
                                Channel channel = getChannelFromName(messageArray[1]);
                                Console.WriteLine("channel " + messageArray[1]);
                                Console.WriteLine("channelname " + channel.name);
                                Account account = NickDatabase.getAccountFromUser(user);
                                if (account == null) {
                                    cs.noticeUser(user, "You are not registered.");
                                    return;
                                }
                                ChannelAccount chan = new ChannelAccount(channel);
                                ChannelAccountEvent cae = chan.register(messageArray[2], account);
                                switch (cae)
                                {
                                    case ChannelAccountEvent.REGISTER_ALREADY_REGISTERED:
                                        {
                                            cs.noticeUser(user, "Channel " + messageArray[1] + " is already registered.");
                                            return;
                                        }
                                    case ChannelAccountEvent.REGISTER_SUCESS:
                                        {
                                            cs.noticeUser(user, messageArray[1] + " is now registered to you.");
                                            return;
                                        }
                                    default:
                                        {
                                            cs.noticeUser(user, "An unexpected error occured..");
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    default:
                        {
                            cs.noticeUser(user, "Unknown command, " + Chars.bold + "/msg ChanServ HELP" + Chars.bold + " for a listing of commands.");
                            break;
                        }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("FAIL " + e.ToString());
            }

        }
    }
}