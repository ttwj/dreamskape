using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using dreamskape.Channels;
using dreamskape.Users;
using System.Net.Sockets;

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
                if (!hasBurst)
                {
                }
                while (true)
                {
                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        parseLine(inputLine);
                        Console.WriteLine("< " + inputLine);
                    }
                }

            }
            catch
            {
            }
        }
        public User getUserFromUID(string UID)
        {
            if (Program.Users.ContainsKey(UID)) {
                User user;
                Program.Users.TryGetValue(UID, out user);
                return user;
            }
            return null;
        }
        public void parseLine(string line)
        {
            string[] lineArray = line.Split(' ');
            if (!hasBurst)
            {
                burst();
                
            }
            string lineStart = lineArray[0];
            if (lineStart == "PING")
            {
                Send("PONG " + lineArray[1]);
            }

            else
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
                            break;
                        }
                    case "UID":
                        {
                            //create new user
                            string user_nickname = lineArray[2];
                            string user_modes = lineArray[5];
                            string user_username = lineArray[6];
                            string user_host = lineArray[7];
                            string user_UID = lineArray[9];
                            string user_realname = lineArray[11].Remove(0, 1);
                            User newuser = new User(user_nickname, user_username, user_modes, user_host, user_realname, user_UID);
                            Console.WriteLine("Initiated new user " + newuser.nickname);
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
            //create user.
            User derp = new User("nyan", "nyan", "i", "nyan.cat", "nyansall", SID + generateSID());
            derp.introduce();
            Channel lol = new Channel("#lol");
            derp.joinChannel(lol);
            return;
        }
        public override void introduceUser(string nickname, string username, string modes, string hostname, string gecos, string UID)
        {
            //plz dun fuck up plz :(
            Send(":" + SID + " UID " + nickname + " 1 " + getTimeStamp() + " +" + modes + " " + username + " " + hostname + " 127.0.0.1 " + UID + " :" + gecos);
        }
        public override void msgUser(User sender, User sendee, string message)
        {
            Send(":" + SID + " PRIVMSG " + sendee.UID + " :" + message);
        }
        public override void noticeUser(User sender, User sendee, string message)
        {
            Send(":" + SID + " NOTICE " + sendee.UID + " :" + message);
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
        
    }
}
