using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

using MongoDB.Driver;

namespace BrainDonor.Mongo.Data
{
    public class Database
    {
        public static MongoDatabase Create(string name)
        {
            return MongoDatabase.Create(WebConfigurationManager.ConnectionStrings[name].ConnectionString);
        }

        public static MongoDatabase CreateDefault()
        {
            return Create("DefaultMongoDB");
        }
    }
}