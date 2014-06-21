using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Users;

namespace dreamskape.Modules.Events
{
    public class UserEvent : Event
    {
        public User user;
        public UserEvent(User user)
        {
            this.user = user;
        }
    }
    public class UserConnectEvent : UserEvent
    {
        public UserConnectEvent(User user) : base(user) { }
    }
}
