using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using MongoDB.Bson;

namespace BrainDonor.Mongo.Data
{
    public class ObjectIdBinder : IModelBinder
    {
        public object BindModel(ControllerContext controller_context, ModelBindingContext binding_context)
        {
            ValueProviderResult result = binding_context.ValueProvider.GetValue(binding_context.ModelName);
            return new ObjectId(result.AttemptedValue);
        }
    }
}
