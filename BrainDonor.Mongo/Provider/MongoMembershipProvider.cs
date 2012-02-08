using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

using MongoDB.Bson;

namespace BrainDonor.Mongo.Provider
{
    public abstract class MongoMembershipProvider<U,R> : System.Web.Security.MembershipProvider
        where U : MongoUser<U>
        where R : MongoRole<R>
    {

        protected string default_admin_username = string.Empty;

        protected MembershipUser GetMembershipUser(U user_record)
        {
            return new MembershipUser("MongoMembershipProvider",
                user_record.Username,
                user_record._id,
                user_record.Email,
                user_record.PasswordQuestion,
                user_record.Comment,
                user_record.IsApproved,
                user_record.IsLockedOut,
                user_record.CreationDate,
                user_record.LastLoginDate,
                user_record.LastLockedOutDate,
                user_record.LastPasswordChangedDate,
                user_record.LastLockedOutDate);
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            if (config.AllKeys.Contains("DefaultAdminUser"))
            {
                default_admin_username = config["DefaultAdminUser"];
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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            string old_password_hash = FormsAuthentication.HashPasswordForStoringInConfigFile(oldPassword, "sha1");
            string new_password_hash = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword, "sha1");

            U user = (from u in MongoUser<U>.AsQueryable() where u.Username == username && u.PasswordHash == old_password_hash select u).FirstOrDefault();

            if (user != null)
            {
                user.PasswordHash = new_password_hash;
                user.Save();
                return true;
            }

            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            string password_hash = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");

            U user = (from u in MongoUser<U>.AsQueryable() where u.Username == username && u.PasswordHash == password_hash select u).FirstOrDefault();

            if (user != null)
            {
                user.PasswordQuestion = newPasswordQuestion;
                user.PasswordAnswer = newPasswordAnswer;
                user.Save();
                return true;
            }

            return false;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (MongoUser<U>.AsQueryable().Where(x => x.Username == username).Count() > 0)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            if (MongoUser<U>.AsQueryable().Where(x => x.Email.ToUpper() == email.ToUpper()).Count() > 0)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            string password_hash = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");

            U user = Activator.CreateInstance<U>();

            user.Username = username;
            user.Email = email;
            user.PasswordHash = password_hash;
            user.PasswordQuestion = passwordQuestion;
            user.PasswordAnswer = passwordAnswer;
            user.IsApproved = isApproved;
            user.Comment = string.Empty;
            user.IsLockedOut = false;
            user.CreationDate = DateTime.Now;
            user.LastLoginDate = DateTime.Now;
            user.LastActivityDate = DateTime.Now;
            user.LastLockedOutDate = DateTime.Now;
            user.LastPasswordChangedDate = DateTime.Now;

            user.Save();

            status = MembershipCreateStatus.Success;

            return GetMembershipUser(user);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            U user = (from u in MongoUser<U>.AsQueryable() where u.Username == username select u).FirstOrDefault();

            if (user != null)
            {
                user.Delete();
                return true;
            }
            return false;
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = MongoUser<U>.AsQueryable().Where(u => u.Email == emailToMatch).Count();

            MembershipUserCollection collection = new MembershipUserCollection();

            foreach (U user in (from u in MongoUser<U>.AsQueryable() where u.Email == emailToMatch select u).Skip(pageIndex * pageSize).Take(pageSize))
            {
                collection.Add(GetMembershipUser(user));
            }

            return collection;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = MongoUser<U>.AsQueryable().Where(u => u.Username == usernameToMatch).Count();

            MembershipUserCollection collection = new MembershipUserCollection();

            foreach (U user in (from u in MongoUser<U>.AsQueryable() where u.Username == usernameToMatch select u).Skip(pageIndex * pageSize).Take(pageSize))
            {
                collection.Add(GetMembershipUser(user));
            }

            return collection;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = MongoUser<U>.AsQueryable().Count();

            MembershipUserCollection collection = new MembershipUserCollection();

            foreach (U user in (from u in MongoUser<U>.AsQueryable() select u).Skip(pageIndex * pageSize).Take(pageSize))
            {
                collection.Add(GetMembershipUser(user));
            }

            return collection;
        }

        public override int GetNumberOfUsersOnline()
        {
            return (from u in MongoUser<U>.AsQueryable() where u.LastActivityDate >= DateTime.Now.AddMinutes(-15) select u).Count();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new ProviderException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            U user = (from u in MongoUser<U>.AsQueryable() where u.Username == username select u).FirstOrDefault();

            if (user != null)
            {
                if (userIsOnline)
                {
                    user.LastActivityDate = DateTime.Now;
                    user.Save();
                }

                return GetMembershipUser(user);
            }

            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            ObjectId key = (ObjectId)providerUserKey;

            U user = (from u in MongoUser<U>.AsQueryable() where u._id == key select u).FirstOrDefault();

            if (user != null)
            {
                if (userIsOnline)
                {
                    user.LastActivityDate = DateTime.Now;
                    user.Save();
                }

                return GetMembershipUser(user);
            }

            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            U user = (from u in MongoUser<U>.AsQueryable() where u.Email == email select u).FirstOrDefault();

            if (user != null) return user.Email;

            return string.Empty;
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 5; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 1; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 10; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return string.Empty; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return true; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset) throw new NotSupportedException();

            U user = (from u in MongoUser<U>.AsQueryable() where u.Username == username select u).FirstOrDefault();

            if (user != null)
            {
                if (RequiresQuestionAndAnswer)
                {
                    if (user.PasswordAnswer != answer) throw new MembershipPasswordException();
                }

                string new_password = Membership.GeneratePassword(12, 4);

                string password_hash = FormsAuthentication.HashPasswordForStoringInConfigFile(new_password, "sha1");

                user.PasswordHash = password_hash;

                user.Save();

                return new_password;
            }

            throw new MembershipPasswordException();
        }

        public override bool UnlockUser(string userName)
        {
            U user = (from u in MongoUser<U>.AsQueryable() where u.Username == userName select u).FirstOrDefault();

            if (user != null)
            {
                user.IsLockedOut = false;
                user.Save();
                return true;
            }

            return false;

            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser membership_user)
        {
            ObjectId key = (ObjectId)membership_user.ProviderUserKey;

            U user = (from u in MongoUser<U>.AsQueryable() where u._id == key select u).FirstOrDefault();

            if (user != null)
            {
                user.Username = membership_user.UserName;
                user.Email = membership_user.Email;
                user.PasswordQuestion = membership_user.PasswordQuestion;
                user.Comment = membership_user.Comment;
                user.IsApproved = membership_user.IsApproved;
                user.IsLockedOut = membership_user.IsLockedOut;
                user.CreationDate = membership_user.CreationDate;
                user.LastLoginDate = membership_user.LastLoginDate;
                user.LastActivityDate = membership_user.LastActivityDate;
                user.LastPasswordChangedDate = membership_user.LastPasswordChangedDate;
                user.LastLockedOutDate = membership_user.LastLockoutDate;
            }

            user.Save();
        }

        public override bool ValidateUser(string username, string password)
        {
            string password_hash = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");

            U user = (from u in MongoUser<U>.AsQueryable() where u.Username == username && u.PasswordHash == password_hash select u).FirstOrDefault();
            
            if (user != null) return true;

            if (FormsAuthentication.Authenticate(username, password))
            {
                return true;
            }

            return false;
        }
    }
}
