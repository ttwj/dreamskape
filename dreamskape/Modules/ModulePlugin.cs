using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using dreamskape.Modules.Events;
using dreamskape.Users;
using dreamskape.Channels;

namespace dreamskape.Modules
{
    public abstract class ModulePlugin
    {
        public ArrayList moduleHooks = new ArrayList();
        public User moduleClient;
        public void registerClient(User client)
        {
            if (moduleClient == null)
            {
                this.moduleClient = client;
            }
        }
        public void registerHook(Hooks hook)
        {
            if (!moduleHooks.Contains(hook))
            {
                moduleHooks.Add(hook);
                Console.WriteLine(hook + " added");
            }
        }
        public static int getTimeStamp()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            int timestamp = (int)t.TotalSeconds;
            return timestamp;
        }
        public string generateUID()
        {
            return Program.SID + Proto.ProtocolPlugin.generateUID();
        }
        public User getUserFromUID(string UID)
        {
            if (Program.Users.ContainsKey(UID))
            {
                User user;
                Program.Users.TryGetValue(UID, out user);
                return user;
            }
            return null;
        }
        public Channel getChannelFromName(string channel)
        {
            if (Program.Channels.ContainsKey(channel.ToLower()))
            {
                Channel chan;
                Program.Channels.TryGetValue(channel.ToLower(), out chan);
                return chan;
            }
            return null;
        }
        public abstract void Initialize();
        public virtual void onUserMessageChannel(ChannelMessageEvent ev) { }
        public virtual void onUserMessageClient(UserMessageEvent ev) { }
        public virtual void onUserPartChannel() { }
        public virtual void onClientKilled(KillEvent ev) { }
        public virtual void onClientKill(KillEvent ev) { }
        public virtual void onUserConnect(UserEvent ev) { }
        public virtual void onUserNickChange(UserNickChangeEvent ev) { }
        public virtual void onChannelLog(ChannelLogEvent ev) { }
        public virtual void onClientIntroduce(ClientIntroduceEvent ev) { }
        public virtual void onServerBurstStart() { }
        public virtual void onServerBurstEnd() { }

        public virtual void onUserIdentify(UserEvent ev) { }
        public virtual void onUserIdentifyFail(UserEvent ev) { }

        public virtual void onUserBurstConnect(UserEvent ev) { }
    }
}
