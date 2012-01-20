using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using MongoDB.Bson;

namespace BrainDonor.Mongo.Data
{
    public class MvcBinders
    {
        public static void RegisterBinders()
        {
            ModelBinders.Binders.Add(typeof(ObjectId), new ObjectIdBinder());
        }
    }
}
