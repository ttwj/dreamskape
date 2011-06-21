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
            }
        }
        public string generateUID()
        {
            return Program.SID + Proto.ProtocolPlugin.generateUID();
        }
        public abstract void Initialize();
        public virtual void onUserMessageChannel(ChannelMessageEvent ev) { }
        public virtual void onUserMessageClient(UserMessageEvent ev) { }
        public virtual void onUserPartChannel() { }
        public virtual void onClientKilled(KillEvent ev) { }
        public virtual void onClientKill(KillEvent ev) { }
        public virtual void onUserConnect(UserEvent ev) { }
        public virtual void onUserNickChange(UserNickChangeEvent ev) { }

    }
}
