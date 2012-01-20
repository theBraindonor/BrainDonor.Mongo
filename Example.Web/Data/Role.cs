using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BrainDonor.Mongo.Provider;

namespace Example.Web.Data
{
    public class RoleProviders : MongoRoleProvider<User, Role>
    {
    }

    public class Role : MongoRole<Role>
    {
    }
}