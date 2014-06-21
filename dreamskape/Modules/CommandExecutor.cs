using System;
using dreamskape.Modules.Events;
using dreamskape.Users;

namespace dreamskape.Modules
{
	public class CommandExecutor
	{
		public virtual String Name { get; set; }
		public virtual String HelpInfo { get; set; }

		public virtual bool onUserCommand(String Command, string[] args, UserMessageEvent ev) {
			return false;
		}
		public virtual bool onChannelCommand (String Command, string[] args, ChannelMessageEvent ev) {
			return false;
		}
		public virtual bool onHelpCommand(User user) {
			return false;
		}
	}
}

