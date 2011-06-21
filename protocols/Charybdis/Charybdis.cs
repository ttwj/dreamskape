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
            catch (Exception e)
            {
                Console.WriteLine("FAIL");
                Console.WriteLine(e.ToString());
                
            }
        }
        
        public void parseLine(string line)
        {
            string[] lineArray = line.Split(' ');
            if (!hasBurst)
            {
                burst();
                Module.InitModules();
            }
            string lineStart = lineArray[0];
            if (lineStart == "PING")
            {
                Send("PONG " + lineArray[1]);
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
                                    Console.WriteLine("calling hooK!");
                                    UserMessageEvent ev = new UserMessageEvent(sender, user, message);
                                    Module.callHook(Hooks.USER_MESSAGE_CLIENT, user, ev);
                                }
                                else if (channel != null)
                                {
                                    //not implemented :3d
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
        public override void msgUser(User sender, User sendee, string message)
        {
            Send(":" + sender.UID + " PRIVMSG " + sendee.UID + " :" + message);
        }
        public override void noticeUser(User sender, User sendee, string message)
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
        
    }
}
