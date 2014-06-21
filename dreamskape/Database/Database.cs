using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using ServiceStack.OrmLite;
using dreamskape.Modules;

namespace dreamskape.Databases
{
    public class Database
    {
		public static OrmLiteConnectionFactory dbFactory;

        public static void Init()
        {


			dbFactory = new OrmLiteConnectionFactory (@"services.db", SqliteDialect.Provider);

			if (!File.Exists ("services.db")) {
				Console.WriteLine ("calling init db");
				ModuleManager.callInitDb (dbFactory);
			}

			ModuleManager.callLoadDb (dbFactory);

        }
       
        public static string sha256(string password)
        {
            SHA256Managed crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(password), 0, Encoding.ASCII.GetByteCount(password));
            foreach (byte bit in crypto)
            {
                hash += bit.ToString("x2");
            }
            return hash;
        }
    }
}
