using System;
using dreamskape;
using dreamskape.Users;
using dreamskape.Channels;
using dreamskape.Databases;
using dreamskape.Modules;
using dreamskape.Modules.Events;
using System.Collections;
using System.Collections.Generic;

namespace dreamskape.Chanserv
{
    public class ChanServ : ModulePlugin
    {
        public static Client cs;
        public override void Initialize()
        {
            this.registerHook(Hooks.SERVER_BURST_START);
            this.registerHook(Hooks.SERVER_BURST_END);
            this.registerHook(Hooks.USER_MESSAGE_CLIENT);

            //help
            Help.initHelp();
            Help.registerHelp("HELP", "Shows help, duh");
            Help.registerHelp("REGISTER", "<channel> <pass> - Registers a channel");
            Help.registerHelp("ACCESS", "Controls the channel access list");
            Help.registerHelp("LOGIN", "Gives you founder-level access to a channel");
        }
        public override void onServerBurstStart()
        {
            cs = new Client("ChanServ", "ChanServ", "S", "Chan.Serv", "Channel Management Services", generateUID());
            cs.introduce();
            this.registerClient(cs);
            
        }
        public override void onServerBurstEnd()
        {
            foreach (KeyValuePair<string, Channel> chan in Program.Channels)
            {
                cs.joinChannelMode(chan.Value, cs, "+o");
            }
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
                    case "ACCESS":
                        {
                            if (messageArray.Length < 3)
                            {
                                cs.noticeUser(user, "Invalid syntax");
                                string help;
                                Help.HelpDict.TryGetValue("ACCESS", out help);
                                cs.noticeUser(user, help);
                            }
                            switch (messageArray[2].ToUpper())
                            {
                                case "LIST":
                                    {
                                        try
                                        {
                                            ChannelAccount ca = ChannelDatabase.getChannelAccountFromChannel(getChannelFromName(messageArray[1]));
                                            if (ca.Access == null)
                                            {
                                                cs.noticeUser(user, Chars.bold + "Unknown channel " + messageArray[1]);
                                            }
                                            cs.noticeUser(user, "Access listing for channel " + messageArray[1]);
                                            foreach (KeyValuePair<Account, int> access in ca.Access)
                                            {
                                                cs.noticeUser(user, access.Key.User + "                " + access.Value);
                                            }
                                            cs.noticeUser(user, "");
                                            cs.noticeUser(user, "Channel listing for " + messageArray[1] + " complete");
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Source);
                                            Console.WriteLine(e.ToString());
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        cs.noticeUser(user, "/msg ChanServ ACCESS <CHANNEL> <LIST|ADD|DEL> <USER>");
                                        break;
                                    }
                            }
                            break;
                        }
                    case "LOGIN":
                        {
                            if (messageArray.Length < 3)
                            {
                                cs.noticeUser(user, "Invalid syntax");
                                string help;
                                Help.HelpDict.TryGetValue("LOGIN", out help);
                                cs.noticeUser(user, help);
                            }
                            Account a = NickDatabase.getAccountFromUser(user);
                            ChannelAccount ca = ChannelDatabase.getChannelAccountFromChannel(getChannelFromName(messageArray[1]));
                            if (!user.loggedIn) {
                               cs.noticeUser(user, Convert.ToChar(2) + "Please login first.");
                                return;
                            }
                            else if (ca == null)
                            {
                                cs.noticeUser(user, Convert.ToChar(2) + messageArray[1] + " is not registered.");
                                return;
                            }
                            else
                            {
                                ChannelAccountLoginEvent cle;
                                if (ca.Password == Database.sha256(messageArray[2]))
                                {
                                    ca.setAccess(a, 5);
                                    cs.noticeUser(user, "You now have founder-level accesss to " + messageArray[2]);
                                    cle = new ChannelAccountLoginEvent(ca, true);
                                }
                                else
                                {
                                    cs.noticeUser(user, "Invalid password for " + messageArray[2]);
                                    cle = new ChannelAccountLoginEvent(ca, false);
                                }
                                Module.callHook(Hooks.CHANNEL_LOGIN, null, cle);
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