﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Channels;
using dreamskape.Users;

namespace dreamskape.Modules.Events
{
    public class ChannelMessageEvent : Event
    {
        public string message;
        public Channel channel;
        public User user;
        public ChannelMessageEvent(User user, string message, Channel channel) 
        {
            this.message = message;
            this.channel = channel;
        }
        public bool ContainsUser(User user)
        {
            return channel.containsUser(user);
        }
    }
}
