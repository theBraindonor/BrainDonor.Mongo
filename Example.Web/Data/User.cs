using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BrainDonor.Mongo.Provider;

namespace Example.Web.Data
{
    public class UserProvider : MongoMembershipProvider<User, Role>
    {
    }

    public class User : MongoUser<User>
    {
    }
}