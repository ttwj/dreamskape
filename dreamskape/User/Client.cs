using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dreamskape.Users
{
    public class Client : User
    {
        public Client (string nickname, string username, string modes, string hostname, string gecos, string UID, bool introduced = false): base(nickname, username, modes,hostname, gecos, UID, introduced = false)
        {
        }
        public void kill(User killee, string reason)
        {
            //client kill user
            //problem?
        }
    }
}
