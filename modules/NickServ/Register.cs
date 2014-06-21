using System;
using dreamskape.Modules;
using dreamskape.Users;
using dreamskape.Modules.Events;
using ServiceStack.OrmLite;

namespace dreamskape.Nickserv
{
	public class Register : CommandExecutor
	{
		private static ModulePlugin instance = NickServ.getInstance();
		public static void registerAccount(User user, String password, String email) {
			NickEntry nickEntry = new NickEntry ();
			nickEntry.Nick = user.nickname.ToLower ();
			nickEntry.Email = email;
			nickEntry.Password = password;
			instance.getDB().Insert (nickEntry, selectIdentity: true);
			instance.debugServices (user.nickname + " registered account (" + email + ")");
		}
		public Register() {
			Console.WriteLine ("uh hello");
		}
		private Client nickserv = instance.moduleClient;

		public override string Name {
			get {
				return "REGISTER";
			}
		}
		public override string HelpInfo {
			get {
				return "Registers your nickname";
			}
		}
		public override bool onHelpCommand (dreamskape.Users.User user)
		{
			nickserv.noticeUser (user, "Syntax: REGISTER <password> <email>");
			return true;
		}
		public override bool onUserCommand (string Command, string[] args, UserMessageEvent ev)
		{
			User user = ev.sender;
			Console.WriteLine ("register command called!");

			if (args.Length < 3)
			{
				Console.WriteLine("wtf register fail?");
				nickserv.noticeUser(user, "Invalid syntax");
				return false;
			}
			else if (NickServ.getNickEntry(user) != null) {
				nickserv.noticeUser(user, Convert.ToChar(2) + "Error: This nickname is already registered.");
				return true;
			}
			registerAccount (user, args [1], args [2]);
			user.loggedIn = true;
			nickserv.noticeUser(user, Chars.bold + "You are now registered.");
			return true;

		}


	}
}

