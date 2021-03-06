﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Channels;
using dreamskape.Proto;
using dreamskape.Modules;
using dreamskape.Modules.Events;

namespace dreamskape.Users
{
    public class Client : User
    {
        public Client(string nickname, string username, string modes, string hostname, string gecos, string UID, bool introduced = false)
            : base(nickname, username, modes, hostname, gecos, UID, introduced = false)
        {
           
            Program.Clients.Add(nickname.ToLower(), this);
        }
        public void introduce()
        {
            try
            {
                Protocol.protocolPlugin.introduceUser(nickname, username, modes, hostname, gecos, UID);
                ClientIntroduceEvent ev = new ClientIntroduceEvent(this);
                ModuleManager.callHook(Hooks.CLIENT_INTRO, this, ev);
                Console.WriteLine("derp");
                this.isIntroduced = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("fail" + e.ToString());
            }
            
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
            Console.WriteLine(this.UID);
            Console.WriteLine(user.UID);
            Protocol.protocolPlugin.noticeUser(this, user, message);
        }
        public void messageUser(User user, string message = "hi")
        {
            Protocol.protocolPlugin.msgUser(this, user, message);
        }
        public void messageChannel(Channel channel, string message = "hi")
        {
            Protocol.protocolPlugin.msgChannel(this, channel, message);
        }
        public void modeChannel(Channel channel, string mode)
        {
            Protocol.protocolPlugin.chanMode(this, channel, mode);
        }
        public void modeChannel(Channel channel, User dest, string mode)
        {
            Protocol.protocolPlugin.chanMode(this, channel, dest, mode);
        }
        public void joinChannel(Channel chan)
        {
            Protocol.protocolPlugin.joinUser(this, chan);
        }
        public void joinChannelMode(Channel channel, string mode)
        {
            Protocol.protocolPlugin.joinChannelMode(this, channel, mode);
        }
        public void joinChannelMode(Channel channel, User dest, string mode)
        {
            Protocol.protocolPlugin.joinChannelMode(this, channel, dest, mode);
        }
        
    }
}
