using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace BrainDonor.Mongo.Data
{
    /*
     * We are wrapping the database context so that we can easily assume to be working with the web.config.
     * We'll default to "DefaultMongoDB" connection string unless we are passed an explicit mongo database.
     */
    public abstract class Item<T>
    {
        public static MongoCollection<T> Collection(MongoDatabase context)
        {
            return context.GetCollection<T>(typeof(T).Name);
        }

        public static MongoCollection<T> Collection()
        {
            return Item<T>.Collection(Database.CreateDefault());
        }

        public static IQueryable<T> AsQueryable(MongoDatabase context)
        {
            return Collection(context).AsQueryable<T>();
        }

        public static IQueryable<T> AsQueryable()
        {
            return AsQueryable(Database.CreateDefault());
        }

        public SafeModeResult Delete(MongoDatabase context)
        {
            string id_key = null;
            BsonValue id_value = null;

            string fallback_id_key = "_id";
            BsonValue fallback_id_value = null;

            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                if (prop.Name == fallback_id_key)
                {
                    fallback_id_value = BsonValue.Create(prop.GetValue(this, null));
                }

                object[] match = prop.GetCustomAttributes(typeof(BsonIdAttribute), true);

                if (match.Length > 0)
                {
                    id_key = prop.Name;
                    id_value = BsonValue.Create(prop.GetValue(this, null));
                }
            }

            if (id_key != null && id_value != null)
            {
                return Item<T>.Collection(context).Remove(Query.EQ(id_key, id_value));
            }

            if (fallback_id_key != null && fallback_id_value != null)
            {
                return Item<T>.Collection(context).Remove(Query.EQ(fallback_id_key, fallback_id_value));
            }

            throw new InvalidOperationException();
        }

        public SafeModeResult Delete()
        {
            return Delete(Database.CreateDefault());
        }

        public SafeModeResult Save(MongoDatabase context)
        {
            return Item<T>.Collection(context).Save(this);
        }

        public SafeModeResult Save()
        {
            return Save(Database.CreateDefault());
        }
    }
}
