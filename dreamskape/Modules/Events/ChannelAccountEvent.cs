using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Databases;

namespace dreamskape.Modules.Events
{
    public class ChannelAccountModuleEvent : Event
    {
        public ChannelAccount channel;
        public ChannelAccountModuleEvent(ChannelAccount channel)
        {
            this.channel = channel;
        }
    }
    public class ChannelAccountLoginEvent : ChannelAccountModuleEvent
    {
        public bool LoginSuccess;
        public ChannelAccountLoginEvent(ChannelAccount channel, bool LoginSuccess)
            : base(channel)
        {
            this.LoginSuccess = LoginSuccess;
        }
    }
}
