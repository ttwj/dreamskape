using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Channels;
using dreamskape.Proto;

namespace dreamskape.Users
{
    public class Client : User
    {
        public Client (string nickname, string username, string modes, string hostname, string gecos, string UID, bool introduced = false): base(nickname, username, modes,hostname, gecos, UID, introduced = false)
        {
        }
        public void kill(User killee, string reason = "Killed by dreamskape")
        {
            //client kill user
            //problem?
            Protocol.protocolPlugin.killUser(this, killee, reason);
        }
        public void kickUser(User kickee, Channel chan, string reason = "Kicked by dreamskape")
        {
            Protocol.protocolPlugin.kickUser(this, kickee, chan, reason);
        }
        public void noticeUser(User user, string message = "hi")
        {
            Protocol.protocolPlugin.noticeUser(user, (User)this, message);
        }
    }
}
