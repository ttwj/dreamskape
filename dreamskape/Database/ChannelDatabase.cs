using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dreamskape.Channels;
using dreamskape.Users;
using System.Data;
using System.Security.Cryptography;
using System.Data.SQLite;
using System.IO;

namespace dreamskape.Databases
{

    
    public class ChannelDatabase : Database
    {
        public static Dictionary<string, ChannelAccount> ChannelAccounts;
        public static void loadRegistered()
        {
            ChannelAccounts = new Dictionary<string, ChannelAccount>();
            DataTable userTable = ExecuteQuery("SELECT * FROM channels");
            int count = 0;
            foreach (DataRow row in userTable.Rows)
            {
                //get the object, durr
                ChannelAccount channel = (ChannelAccount)BinaryDeserialize(row["data"].ToString());
                ChannelAccounts.Add(row["channel"].ToString().ToLower(), channel);
                count++;
            }
            Console.WriteLine("Loaded " + count + " ChannelAccount entries!");
        }
        public static void updateChannel(ChannelAccount channel)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
                {
                    using (SQLiteCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO channels (channel ,data) VALUES(@channel, @data)";
                        cmd.Parameters.AddWithValue("@channel", channel.Name);
                        cmd.Parameters.AddWithValue("@data", BinarySerialize(channel));
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem: " + e.ToString());
            }
        }
        public static void createChannel(ChannelAccount channel)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
                {
                    using (SQLiteCommand cmd = cnn.CreateCommand())
                    {
                        ChannelAccounts.Add(channel.Name, channel);
                        cmd.CommandText = "INSERT INTO channels (channel ,data) VALUES(@channel, @data)";
                        cmd.Parameters.AddWithValue("@channel", channel.Name);
                        cmd.Parameters.AddWithValue("@data", BinarySerialize(channel));
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem: " + e.ToString());
            }
        }
        public static void destroyChannel(ChannelAccount channel)
        {
            try
            {
                using (SQLiteConnection cnn = new SQLiteConnection("Data Source=services.db"))
                {
                    using (SQLiteCommand cmd = cnn.CreateCommand())
                    {
                        ChannelAccounts.Add(channel.Name, channel);
                        cmd.CommandText = "DELETE FROM channels WHERE channel=@channel";
                        cmd.Parameters.AddWithValue("@channel", channel.Name);
                        cnn.Open();
                        cmd.ExecuteNonQuery();
                        cnn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem: " + e.ToString());
            }
        }
        public static ChannelAccount getChannelAccountFromChannel(Channel channel)
        {
            ChannelAccount account;
            ChannelAccounts.TryGetValue(channel.name.ToLower(), out account);
            return account;
        }
        public static bool isRegistered(Channel channel)
        {
            return ChannelAccounts.ContainsKey(channel.name.ToLower());
        }
      
    }
}
