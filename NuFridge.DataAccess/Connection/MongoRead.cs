using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace NuFridge.DataAccess.Connection
{
    public class MongoRead
    {
        MongoServer Server;

        public MongoRead(MongoServer server)
        {
            Server = server;
        }

        public string FullConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["MongoDB_ConnectionStringDatabase"].ConnectionString;
            }
        }


        public MongoDatabase Database
        {
            get { return Server.GetDatabase(ConfigurationManager.AppSettings["MongoDB_DatabaseName"]); }
        }

        public MongoCollection Logs
        {
            get { return Database.GetCollection("logs"); }
        }

        private static MongoRead _instance = null;

        public static MongoRead Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = RegisterMongoDb();

                }

                return _instance;
            }

        }

        private static MongoRead RegisterMongoDb()
        {
            MongoClient client = new MongoClient(ConfigurationManager.AppSettings["MongoDB_ConnectionString"]);
            var readServer = client.GetServer();
            var read = new MongoRead(readServer);
            return read;
        }

    }
}
