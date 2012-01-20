using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson;

using BrainDonor.Mongo.Data;

namespace BrainDonor.Mongo.Provider
{
    [Serializable]
    public class MongoUserRoleAssignment
    {
        public string Name { get; set; }
        public DateTime AssignedDate { get; set; }
        public string AssignedBy { get; set; }
    }

    [Serializable]
    public abstract class MongoUser<I> : Item<I> where I : MongoUser<I>
    {
        public ObjectId _id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public string Comment { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public DateTime LastLockedOutDate { get; set; }
        public List<MongoUserRoleAssignment> RoleAssignment { get; set; }
    }
}
