using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson;

using BrainDonor.Mongo.Data;

namespace BrainDonor.Mongo.Provider
{
    [Serializable]
    public abstract class MongoRole<I> : Item<I> where I : MongoRole<I>
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
