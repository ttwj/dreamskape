using System;
using ServiceStack.OrmLite;
using dreamskape.Modules;

namespace dreamskape.Nickserv
{
	public class NickEntry {
		public String Nick { get; set; }
		public String Email { get; set; }
		private String _Password;
		public String Password { 
			get {
				return _Password;
			}
			set { 
				_Password = Databases.Database.sha256(value);
			}
		}
	}
}

