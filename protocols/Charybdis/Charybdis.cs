using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using dreamskape.Channels;
using dreamskape.Users;
using dreamskape.Modules;
using dreamskape.Modules.Events;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace dreamskape.Proto
{
    public class charybdis : ProtocolPlugin
    {
        private static StreamWriter writer;
        public override void Send(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
            Console.WriteLine("> " + message);
        }
        public override void Connect()
        {
            NetworkStream stream;
            StreamReader reader;
            TcpClient IRC;
            string inputLine;
            try
            {
                Console.WriteLine("Connecting to " + server + ":" + port);
                IRC = new TcpClient(server, port);
                stream = IRC.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
                while (true)
                {
                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        Console.WriteLine("< " + inputLine);
                        parseLine(inputLine);

                    }
                }

            }
            catch (System.Net.Sockets.SocketException e)
            {
                Console.WriteLine("A socket exception occured, reconneting in 5 seconds.");
                Console.WriteLine("Exception: " + e.ToString());
                System.Threading.Thread.Sleep(5000);
                this.Connect();
            }

            catch (Exception e)
            {
                Console.WriteLine("An unexpected error occured.");
                Console.WriteLine(e.ToString());
            }
        }

        public void parseLine(string line)
        {
            string[] lineArray = line.Split(' ');
            if (!hasBurst)
            {
                burst();
                Module.callHook(Hooks.SERVER_BURST_START, null, null);
            }
            string lineStart = lineArray[0];
            if (lineStart == "PING")
            {
                Send("PONG " + lineArray[1]);
                if (burstComplete == false)
                {
                    Console.WriteLine("calling hook burst_Complete ===");
                    Module.callHook(Hooks.SERVER_BURST_END, null, null);
                    burstComplete = true;
                }
            }
            else if (lineArray[1].Length > 1)
            {
                switch (lineArray[1])
                {
                    case "QUIT":
                        {
                            string user_UID = lineArray[0].Remove(0, 1);
                            //check if user exists.
                            User user = getUserFromUID(user_UID);
                            if (user == null)
                            {
                                Console.WriteLine("Warning : A user that wasn't introduced quit!");
                                return;
                            }
                            //get rid of this user.
                            user = null;
                            Program.Users.Remove(user_UID);
                            //call plugins :o

                            break;
                        }
                    case "SJOIN":
                        {
                            Channel channel = getChannelFromName(lineArray[3]);
                            if (channel == null)
                            {
                                channel = new Channel(lineArray[3], int.Parse(lineArray[2]));
                            }
                            //get the user..
                            Regex users = new Regex(@":(.*) SJOIN (.*):(.*)");
                            Match match = users.Match(line);
                            string[] userStringArray = match.Groups[3].Value.Split(' ');
                            foreach (string user in userStringArray)
                            {
                                string usr;
                                if (user.StartsWith("@") || user.StartsWith("+"))
                                {
                                    usr = user.Remove(0, 1);
                                }
                                else
                                {
                                    usr = user;
                                }
                                User u = getUserFromUID(usr);
                                channel.addToChannel(u);
                            }
                            break;
                        }
                    case "UID":
                        {
                            //create new user
                            foreach (string lol in lineArray)
                            {
                                Console.WriteLine(": " + lol);
                            }
                            string user_nickname = lineArray[2];
                            string user_modes = lineArray[5];
                            string user_username = lineArray[6];
                            string user_host = lineArray[7];
                            string user_UID = lineArray[9];
                            string user_realname = lineArray[10].Remove(0, 1);
                            User newuser = new User(user_nickname, user_username, user_modes, user_host, user_realname, user_UID);
                            Console.WriteLine("Initiated new user " + newuser.nickname);
                            UserEvent ev = new UserEvent(newuser);
                            Module.callHook(Hooks.USER_CONNECT, null, ev);
                            Module.callHook(Hooks.USER_BURST_CONNECT, null, ev);
                            break;
                        }
                    case "PRIVMSG":
                        {
                            Console.WriteLine("OMG PRIVMSG!!");
                            User sender = getUserFromUID(lineArray[0].Remove(0, 1));
                            string dest = lineArray[2];
                            Client user = (Client)getUserFromUID(dest);
                            Console.WriteLine(dest);
                            Console.WriteLine(sender.nickname);
                            Channel channel = getChannelFromName(dest);
                            Regex text = new Regex(@":(.*) PRIVMSG (.*) :(.*)");
                            Match match = text.Match(line);
                            if (match.Success)
                            {
                                string message = match.Groups[3].Value;
                                Console.WriteLine(message);
                                if (user != null)
                                {
                                    UserMessageEvent ev = new UserMessageEvent(sender, user, message);
                                    Module.callHook(Hooks.USER_MESSAGE_CLIENT, user, ev);
                                }
                                else if (channel != null)
                                {

                                }
                                else
                                {
                                    Console.WriteLine("An unexpected shit happened at " + line);
                                }
                            }
                            break;
                        }
                    case "NICK":
                        {
                            string senderuid = lineArray[0].Remove(0, 1);
                            User user = getUserFromUID(senderuid);
                            string oldnick = user.nickname;
                            user.nickname = lineArray[2];
                            Console.WriteLine("changed " + user.nickname);
                            UserNickChangeEvent ev = new UserNickChangeEvent(user, oldnick, lineArray[2]);
                            Console.WriteLine("calling hook!");
                            Module.callHook(Hooks.USER_NICKCHANGE, null, ev);
                            break;
                        }
                    case "MODE":
                        {
                            string senderuid = lineArray[0].Remove(0, 1);
                            User user = getUserFromUID(senderuid);
                            user.modes = user.modes + lineArray[3].Remove(0, 1);
                            break;
                        }
                }
            }

        }
        private void burst()
        {
            Send("PASS " + sendpass + " TS 6 :" + SID);
            Send("CAPAB :QS EX IE KLN UNKLN TB ESID ENCAP RSFNC EOPMOD SERVICES");
            Send("SERVER " + thisserver + " 1 :Nyan IRC Services v0.1");
            Send("SVINFO 6 3 0 :" + getTimeStamp());
            hasBurst = true;
            return;
        }
        public override void introduceUser(string nickname, string username, string modes, string hostname, string gecos, string UID)
        {
            //plz dun fuck up plz :(
            Send(":" + SID + " UID " + nickname + " 1 " + getTimeStamp() + " +" + modes + " " + username + " " + hostname + " 127.0.0.1 " + UID + " :" + gecos);
        }
        public override void msgUser(Client sender, User sendee, string message)
        {
            Send(":" + sender.UID + " PRIVMSG " + sendee.UID + " :" + message);
        }
        public override void noticeUser(Client sender, User sendee, string message)
        {
            Send(":" + sender.UID + " NOTICE " + sendee.UID + " :" + message);
        }
        public override void joinUser(User joinee, Channel channel)
        {
            Send(":" + SID + " SJOIN " + getTimeStamp() + " " + channel.name + " + :" + joinee.UID);
        }
        public override void killUser(User killee, string reason)
        {
            if (reason.Length < 1)
            {
                reason = "Killed by dreamskape";
            }
            Send(SID + " KILL " + killee.UID + " :" + reason);
        }
        public override void kickUser(Client kicker, User kickee, Channel channel, string reason = null)
        {
            Send(":" + kicker.UID + " KICK " + channel.name + " " + kickee.UID + " :" + reason);
        }
        public override void killUser(Client killer, User killee, string reason = null)
        {
            Send(":" + killer.UID + " " + killee.UID + " :" + thisserver + "!" + killer.hostname + "!" + killer.username + "!" + killer.nickname + " (" + reason + ")");
        }
        public override void msgChannel(Client sender, Channel channel, string message)
        {
            Console.WriteLine(":" + sender.UID + " PRIVMSG " + channel.name + " :" + message);
            Send(":" + sender.UID + " PRIVMSG " + channel.name + " :" + message);
        }
        public override void noticeChannel(Client sender, Channel channel, string message)
        {
            Send(":" + sender.UID + " NOTICE " + channel.name + " :" + message);
        }
        public override void chanMode(Client sender, Channel channel, string modes)
        {
            Send(":" + sender.UID + " TMODE " + getTimeStamp() + " " + channel.name + " " + modes);
        }
        public override void chanMode(Client sender, Channel channel, User dest, string modes)
        {
            Send(":" + sender.UID + " TMODE " + getTimeStamp() + " " + channel.name + " " + modes + " " + dest.UID);
        }
        public override void joinChannelMode(Client client, Channel channel, string modes)
        {
            Send(":" + SID + " SJOIN " + channel.TS + " " + channel.name + " " + modes + " :" + client.UID);
        }
        
        public override void joinChannelMode(Client client, Channel channel, User dest, string modes)
        {
            if (modes.StartsWith("+"))
            {
                if (modes.Contains('o'))
                {
                    Send("SJOIN " + channel.TS + " " + channel.name + " + :@" + client.UID);
                }
                else if (modes.Contains('v'))
                {
                    Send("SJOIN " + channel.TS + " " + channel.name + " + :+" + client.UID);
                }
            }
            else
            {
                Send(":" + client.UID + " TMODE " + getTimeStamp() + " " + channel.name + " " + modes + " " + dest.UID);
            }
            
        }
    }
}
