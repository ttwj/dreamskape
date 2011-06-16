using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Proto;
using dreamskape.Channels;

namespace dreamskape.Users
{
    public class User
    {
        public string nickname;
        public string username;
        public string hostname;
        public string gecos;
        public string UID;
        public string modes = "";
        public User(string nickname, string username, string modes, string hostname, string gecos, string UID)
        {
            this.nickname = nickname;
            this.username = username;
            this.hostname = hostname;
            this.gecos = gecos;
            this.modes = modes;
            this.UID = UID;
            Program.Users.Add(UID, this);
        }
        public void introduce()
        {
            Protocol.protocolPlugin.introduceUser(nickname, username, modes, hostname, gecos, UID);
        }
        public void joinChannel(Channel chan)
        {
            Protocol.protocolPlugin.joinUser(this, chan);
        }
    }
}