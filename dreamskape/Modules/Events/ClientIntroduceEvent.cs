using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Users;

namespace dreamskape.Modules.Events
{
    public class ClientIntroduceEvent : Event
    {
        public Client client;
        public ClientIntroduceEvent(Client client)
        {
            this.client = client;
        }
    }
}
