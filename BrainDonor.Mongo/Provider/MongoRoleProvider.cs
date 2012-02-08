using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace BrainDonor.Mongo.Provider
{
    public class MongoRoleProvider<U, R> : System.Web.Security.RoleProvider
        where U : MongoUser<U>
        where R : MongoRole<R>
    {
        protected string default_admin_username = string.Empty;
        protected string default_admin_role = string.Empty;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            if (config.AllKeys.Contains("DefaultAdminUser"))
            {
                default_admin_username = config["DefaultAdminUser"];
            }
            if (config.AllKeys.Contains("DefaultAdminRole"))
            {
                default_admin_role = config["DefaultAdminRole"];
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] role_names)
        {
            foreach (string username in usernames)
            {
                if (string.IsNullOrEmpty(username)) throw new ArgumentException();

                U user = MongoUser<U>.AsQueryable().Where(x => x.Username == username).FirstOrDefault();

                if (user == null) throw new ProviderException();

                foreach (string role_name in role_names)
                {
                    if (string.IsNullOrEmpty(role_name)) throw new ArgumentException();

                    R role = MongoRole<R>.AsQueryable().Where(x => x.Name == role_name).FirstOrDefault();

                    if (role == null) throw new ArgumentException();

                    if (user.RoleAssignment.Where(x => x.Name == role_name).Count() == 0)
                    {
                        user.RoleAssignment.Add(new MongoUserRoleAssignment() { Name = role_name, AssignedDate = DateTime.Now });
                    }

                    user.Save();
                }
            }
        }

        public override string ApplicationName
        {
            get
            {
                return "/";
            }
            set
            {
            }
        }

        public override void CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentException();

            if (MongoRole<R>.AsQueryable().Where(x => x.Name == roleName).Count() > 0) throw new ProviderException();

            R role = Activator.CreateInstance<R>();

            role.Name = roleName;
            role.Description = string.Empty;

            role.Save();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentException();

            R role = MongoRole<R>.AsQueryable().Where(x => x.Name == roleName).FirstOrDefault();

            if (role == null) throw new ProviderException();

            if (throwOnPopulatedRole)
            {
                if ((from u in MongoUser<U>.AsQueryable() where u.RoleAssignment.Where(x => x.Name == roleName).Count() > 0 select u).Count() > 0)
                {
                    throw new ProviderException();
                }
            }

            foreach (U user in (from u in MongoUser<U>.AsQueryable() where u.RoleAssignment.Where(x => x.Name == roleName).Count() > 0 select u))
            {
                user.RoleAssignment.RemoveAll(x => x.Name == roleName);

                user.Save();
            }

            role.Delete();

            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return GetUsersInRole(roleName);
        }

        public override string[] GetAllRoles()
        {
            return (from r in MongoRole<R>.AsQueryable() select r.Name).ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentException();

            if (!string.IsNullOrEmpty(default_admin_username))
            {
                if (default_admin_username == username)
                {
                    return new string[] { default_admin_role };
                }
            }

            U user = MongoUser<U>.AsQueryable().Where(x => x.Username == username).FirstOrDefault();

            if (user == null) throw new ProviderException();

            return (from r in user.RoleAssignment select r.Name).ToArray();
        }

        public override string[] GetUsersInRole(string role_name)
        {
            if (string.IsNullOrEmpty(role_name)) throw new ArgumentException();

            R role = MongoRole<R>.AsQueryable().Where(x => x.Name == role_name).FirstOrDefault();

            if (role == null) throw new ProviderException();

            return (from u in MongoUser<U>.AsQueryable() where u.RoleAssignment.Where(x => x.Name == role_name).Count() > 0 select u.Username).ToArray();
        }

        public override bool IsUserInRole(string username, string role_name)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentException();

            if (string.IsNullOrEmpty(role_name)) throw new ArgumentException();

            if (!string.IsNullOrEmpty(default_admin_username) && !string.IsNullOrEmpty(default_admin_role))
            {
                if (username == default_admin_username && role_name == default_admin_role)
                {
                    return true;
                }
            }

            U user = MongoUser<U>.AsQueryable().Where(x => x.Username == username).FirstOrDefault();

            if (user == null) throw new ProviderException();

            R role = MongoRole<R>.AsQueryable().Where(x => x.Name == role_name).FirstOrDefault();

            if (role == null) throw new ProviderException();

            return user.RoleAssignment.Where(x => x.Name == role_name).Count() > 0;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] role_names)
        {
            foreach (string username in usernames)
            {
                if (string.IsNullOrEmpty(username)) throw new ArgumentException();

                U user = MongoUser<U>.AsQueryable().Where(x => x.Username == username).FirstOrDefault();

                if (user == null) throw new ProviderException();

                foreach (string role_name in role_names)
                {
                    if (string.IsNullOrEmpty(role_name)) throw new ArgumentException();

                    R role = MongoRole<R>.AsQueryable().Where(x => x.Name == role_name).FirstOrDefault();

                    if (role == null) throw new ArgumentException();

                    if (user.RoleAssignment.Where(x => x.Name == role_name).Count() > 0)
                    {
                        user.RoleAssignment.RemoveAll(x => x.Name == role_name);
                    }

                    user.Save();
                }
            }
        }

        public override bool RoleExists(string roleName)
        {
            return MongoRole<R>.AsQueryable().Where(x => x.Name == roleName).Count() > 0;
        }
    }
}
