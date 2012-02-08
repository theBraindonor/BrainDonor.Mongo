using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Example.Web.Data;
using Example.Web.Areas.Admin.Models;

namespace Example.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class RoleController : Controller
    {
        public ActionResult Index()
        {
            var roles = (from r in Role.AsQueryable() select r);

            return View(roles);
        }

        [HttpGet]
        public ActionResult Display(RoleQueryModel query_model)
        {
            Role model = Role.AsQueryable().Where(x => x._id == query_model.Id).FirstOrDefault();

            if (model != null)
            {
                return View(model);
            }
            return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Edit(RoleQueryModel query_model)
        {
            Role role = Role.AsQueryable().Where(x => x._id == query_model.Id).FirstOrDefault();

            if (role != null)
            {
                RoleEditModel model = new RoleEditModel();
                model.Id = role._id;
                model.Name = role.Name;
                model.Description = role.Description;

                return View(model);
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RoleEditModel model)
        {
            if (ModelState.IsValid)
            {
                Role role = Role.AsQueryable().Where(x => x._id == model.Id).FirstOrDefault();

                if (role == null) return HttpNotFound();

                if (Role.AsQueryable().Where(x => x._id != model.Id && x.Name == model.Name).Count() > 0)
                {
                    ModelState.AddModelError("Name", "The role name is already in use.");
                    return View(model);
                }

                role.Name = model.Name;
                role.Description = model.Description;

                role.Save();

                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleEditModel model)
        {
            if (ModelState.IsValid)
            {
                if (Role.AsQueryable().Where(x => x.Name == model.Name).Count() > 0)
                {
                    ModelState.AddModelError("Name", "The role name is already in use.");
                }
                else
                {
                    Role role = new Role();
                    role.Name = model.Name;
                    role.Description = model.Description;
                    role.Save();

                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Delete(RoleQueryModel query_model)
        {
            Role role = Role.AsQueryable().Where(x => x._id == query_model.Id).FirstOrDefault();

            if (role != null)
            {
                RoleDeleteModel model = new RoleDeleteModel();
                model.Id = role._id;
                model.Name = role.Name;

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
                Role role = Role.AsQueryable().Where(x => x._id == model.Id).FirstOrDefault();

                if (role == null) return HttpNotFound();

                role.Delete();

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
