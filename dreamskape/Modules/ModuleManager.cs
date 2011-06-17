using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using dreamskape.Channels;
using dreamskape.Users;
using dreamskape.Modules.Events;

namespace dreamskape.Modules
{
    public class Module
    {
        public static ArrayList moduleList;
        public static void loadPlugins()
        {
            moduleList = new ArrayList();
            try
            {
                Console.WriteLine("Loading modules..");
                foreach (string str in Directory.GetFiles(@"modules\"))
                {
                    FileInfo info = new FileInfo(str);
                    if (info.Name == Program.protocol)
                    {
                        Console.WriteLine("Loading module " + info.Name);
                        Assembly assembly = Assembly.LoadFrom(info.FullName);
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (!type.IsAbstract && (type.BaseType == typeof(ModulePlugin)))
                            {
                                ModulePlugin plugin = (ModulePlugin)Activator.CreateInstance(type);
                                plugin.Initialize();
                                moduleList.Add(plugin);
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine("Protocol plugin loaded!");
            }
            catch (ReflectionTypeLoadException exception)
            {
                Console.WriteLine(exception.ToString());
                foreach (Exception exception2 in exception.LoaderExceptions)
                {
                    Console.WriteLine(exception2.ToString());
                }
                Console.WriteLine("Problem loading modules!");
            }
        }
        public static void callHook(Hooks hook, User client, Event ev = null)
        {
            foreach (ModulePlugin module in moduleList)
            {
                try
                {
                    if (module.moduleHooks.Contains(hook))
                    {
                        switch (hook)
                        {
                            case Hooks.USER_CHANNEL_PRIVMSG:
                                {
                                    ChannelMessageEvent me = (ChannelMessageEvent)ev;
                                    if (me.channel.containsUser(me.user))
                                    {
                                        module.onUserMessageChannel(me);
                                    }
                                    break;
                                }
                            case Hooks.USER_MESSAGE_CLIENT:
                                {
                                    UserMessageEvent me = (UserMessageEvent)ev;
                                    if (me.sender == client)
                                    {
                                        module.onUserMessageClient(me);
                                    }
                                    break;
                                }
                        }
                    }
                }
                catch
                {
                }
            }
        }

    }
}
