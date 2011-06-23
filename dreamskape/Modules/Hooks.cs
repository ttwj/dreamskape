using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dreamskape.Modules
{
    public enum Hooks
    {
        USER_CHANNEL_PRIVMSG,
        USER_CHANNEL_NOTICE,
        USER_MESSAGE_CLIENT,
        USER_NOTICE_CLIENT,
        USER_NICKCHANGE,
        USER_IDENTIFY,
        USER_IDENTIFY_FAIL,
        USER_REGISTER,
        USER_BURST_CONNECT,
        CHANNEL_LOG,
        CHANNEL_REGISTER,
        CHANNEL_DROP,
        USER_LOGOUT,
        USER_CONNECT,
        CLIENT_KILLED,
        CLIENT_INTRO,
        SERVER_BURST_START,
        SERVER_BURST_END,

        USER_VHOST_REQUEST,
        USER_VHOST_REJECT,
        USER_VHOST_ACTIVATE,

    }
}
