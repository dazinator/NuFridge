using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using NuFridge.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace NuFridge.DataAccess.Connection
{
    public class MongoRead
    {
        internal MongoServer Server { get; set; }
        internal MongoDatabase Database { get; set; }
        protected static string DatabaseName { get; set; }

        public static bool TestConnectionString(string connectionString)
        {
            try
            {
                MongoClient client = new MongoClient(connectionString);
                var server = client.GetServer();
                var result = server.DatabaseExists("NuFridge test for connection");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TestDatabaseExists(string connectionString, string databaseName)
        {
            try
            {
                MongoClient client = new MongoClient(connectionString);
                var server = client.GetServer();
               return server.DatabaseExists(databaseName);
            }
            catch
            {
                return false;
            }
        }

        public bool Connect()
        {
            if (Database == null)
            {
                try
                {
                    if (Server.DatabaseExists(DatabaseName))
                    {
                        Database = Server.GetDatabase(DatabaseName);
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public MongoRead(bool ConnectToDatabase)
        {
            MongoClient client = new MongoClient(ConfigurationManager.AppSettings["MongoDB.ConnectionString"]);
            Server = client.GetServer();

            DatabaseName = ConfigurationManager.AppSettings["MongoDB.DatabaseName"];

            if (ConnectToDatabase)
            {
                if (!Connect())
                {
                    throw new Exception("Could not connect to the database.");
                }
            }
        }

        public MongoRead()
            : this(true)
        {

        }

        public static void CreateDatabase(string connectionString, string databaseName)
        {
                MongoClient client = new MongoClient(connectionString);
                var server = client.GetServer();
                var database = server.GetDatabase(databaseName);
                database.CreateCollection(typeof(Feed).Name);
        }
    }
}