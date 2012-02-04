using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using BrainDonor.Mongo.Provider;

using Example.Web.Data;
using Example.Web.Areas.Admin.Models;

namespace Example.Web.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            var users = (from u in Example.Web.Data.User.AsQueryable() select u);

            return View(users);
        }

        public ActionResult Display(UserQueryModel query_model)
        {
            var user = Example.Web.Data.User.AsQueryable().Where(x => x._id == query_model.Id).FirstOrDefault();

            if (user != null)
            {
                var model = new UserEditModel();

                model.Id = user._id;
                model.Username = user.Username;
                model.Email = user.Email;
                model.Comment = user.Comment;
                model.IsApproved = user.IsApproved;
                model.IsLockedOut = user.IsLockedOut;
                model.CreationDate = user.CreationDate;
                model.LastLoginDate = user.LastLoginDate;
                model.LastActivityDate = user.LastActivityDate;
                model.LastPasswordChangedDate = user.LastPasswordChangedDate;
                model.LastLockedOutDate = user.LastLockedOutDate;

                model.Roles = new List<string>();

                if (user.RoleAssignment != null)
                {
                    foreach (var role in user.RoleAssignment)
                    {
                        model.Roles.Add(role.Name);
                    }
                }

                return View(model);
            }

            return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Edit(UserQueryModel query_model)
        {
            var user = Example.Web.Data.User.AsQueryable().Where(x => x._id == query_model.Id).FirstOrDefault();

            if (user != null)
            {
                var model = new UserEditModel();

                model.Id = user._id;
                model.Username = user.Username;
                model.Email = user.Email;
                model.Comment = user.Comment;
                model.IsApproved = user.IsApproved;
                model.IsLockedOut = user.IsLockedOut;
                model.CreationDate = user.CreationDate;
                model.LastLoginDate = user.LastLoginDate;
                model.LastActivityDate = user.LastActivityDate;
                model.LastPasswordChangedDate = user.LastPasswordChangedDate;
                model.LastLockedOutDate = user.LastLockedOutDate;

                model.Roles = new List<string>();

                if (user.RoleAssignment != null)
                {
                    foreach (var role in user.RoleAssignment)
                    {
                        model.Roles.Add(role.Name);
                    }
                }

                return View(model);
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditModel model)
        {
            if (ModelState.IsValid)
            {
                User user = Example.Web.Data.User.AsQueryable().Where(x => x._id == model.Id).FirstOrDefault();

                if (user == null) return HttpNotFound();

                if (Example.Web.Data.User.AsQueryable().Where(x => x._id != model.Id && x.Username == model.Username).Count() > 0)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                }
                if (Example.Web.Data.User.AsQueryable().Where(x => x._id != model.Id && x.Email == model.Email).Count() > 0)
                {
                    ModelState.AddModelError("Email", "Email alread exists.");
                }

                if (ModelState.IsValid)
                {
                    user.Username = model.Username;
                    user.Email = model.Email;
                    user.Comment = model.Comment;
                    user.IsApproved = model.IsApproved;
                    user.IsLockedOut = model.IsLockedOut;

                    if (user.RoleAssignment == null)
                    {
                        user.RoleAssignment = new List<MongoUserRoleAssignment>();
                    }
                                        
                    foreach (MongoUserRoleAssignment assignment in user.RoleAssignment.ToArray())
                    {
                        if (model.Roles == null || !model.Roles.Contains(assignment.Name))
                        {
                            user.RoleAssignment.RemoveAll(x => x.Name == assignment.Name);
                        }
                    }

                    foreach (string role_name in model.Roles)
                    {
                        Role role = Role.AsQueryable().Where(x => x.Name == role_name).FirstOrDefault();

                        if (role != null)
                        {
                            if (user.RoleAssignment.Where(x => x.Name == role.Name).Count() == 0)
                            {
                                user.RoleAssignment.Add(new MongoUserRoleAssignment()
                                {
                                    Name = role.Name,
                                    AssignedBy = Request.LogonUserIdentity.Name,
                                    AssignedDate = DateTime.Now
                                });
                            }
                        }
                    }

                    user.Save();

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {
                if (Example.Web.Data.User.AsQueryable().Where(x => x.Username == model.Username).Count() > 0)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                }
                if (Example.Web.Data.User.AsQueryable().Where(x => x.Email == model.Email).Count() > 0)
                {
                    ModelState.AddModelError("Email", "Email alread exists.");
                }

                if (ModelState.IsValid)
                {
                    User user = new User();

                    user.Username = model.Username;
                    user.Email = model.Email;
                    user.PasswordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "sha1");
                    user.PasswordQuestion = string.Empty;
                    user.PasswordAnswer = string.Empty;
                    user.Comment = model.Comment;
                    user.IsApproved = model.IsApproved;
                    user.IsLockedOut = false;
                    user.CreationDate = DateTime.Now;
                    user.LastLoginDate = DateTime.Now;
                    user.LastActivityDate = DateTime.Now;
                    user.LastPasswordChangedDate = DateTime.Now;
                    user.LastLockedOutDate = DateTime.Now;
                    user.RoleAssignment = new List<MongoUserRoleAssignment>();

                    foreach (string role_name in model.Roles)
                    {
                        Role role = Role.AsQueryable().Where(x => x.Name == role_name).FirstOrDefault();

                        if (role != null)
                        {
                            user.RoleAssignment.Add(new MongoUserRoleAssignment()
                            {
                                Name = role.Name,
                                AssignedBy = Request.LogonUserIdentity.Name,
                                AssignedDate = DateTime.Now
                            });
                        }
                    }                    

                    user.Save();

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(UserQueryModel query_model)
        {
            User user = Example.Web.Data.User.AsQueryable().Where(x => x._id == query_model.Id).FirstOrDefault();

            if (user != null)
            {
                UserDeleteModel model = new UserDeleteModel();
                model.Id = user._id;
                model.Username = user.Username;
                return View(model);
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(RoleDeleteModel model)
        {
            if (model.Confirmation == true && ModelState.IsValid)
            {
                User user = Example.Web.Data.User.AsQueryable().Where(x => x._id == model.Id).FirstOrDefault();

                if (user == null) return HttpNotFound();

                user.Delete();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Password(UserQueryModel query_model)
        {
            User user = Example.Web.Data.User.AsQueryable().Where(x => x._id == query_model.Id).FirstOrDefault();

            if (user != null)
            {
                UserPasswordModel model = new UserPasswordModel();

                model.Id = user._id;
                model.Username = user.Username;

                return View(model);
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Password(UserPasswordModel model)
        {
            User user = Example.Web.Data.User.AsQueryable().Where(x => x._id == model.Id).FirstOrDefault();

            if (user != null)
            {
                user.PasswordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "sha1");

                user.Save();

                return RedirectToAction("Index");
            }

            return HttpNotFound();
        }
    }
}
