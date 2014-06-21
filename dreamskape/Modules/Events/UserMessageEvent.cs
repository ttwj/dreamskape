﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Users;

namespace dreamskape.Modules.Events
{
    public class UserMessageEvent : Event
    {
        public string message;
        public User sendee;
        public User sender;
        public UserMessageEvent(User sender, User sendee, string message)
        {
            this.message = message;
            this.sender = sender;
            this.sendee = sendee;
        }

    }
}
