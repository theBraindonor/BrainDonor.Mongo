using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MongoDB.Bson;

using BrainDonor.Mongo.Provider;

using DataAnnotationsExtensions;

using Example.Web.Data;

namespace Example.Web.Areas.Admin.Models
{
    public class MandatoryAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            return (!(value is bool) || (bool)value);
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "mandatory";
            yield return rule;
        }
    }

    public class UserQueryModel
    {
        public ObjectId Id { get; set; }
    }

    public class UserDeleteModel
    {
        [Display(Name = "Id")]
        public ObjectId Id { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Confirmation")]
        [Mandatory(ErrorMessage = "Required")]
        public bool Confirmation { get; set; }
    }

    public class UserEditModel
    {
        [Display(Name = "Id")]
        public ObjectId Id { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is Required")]
        public string Username { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "An email address is required.")]
        [Email(ErrorMessage = "An email address is required.")]
        public string Email { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Is Locked Out")]
        public bool IsLockedOut { get; set; }

        [Display(Name = "Created")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Last Login")]
        public DateTime LastLoginDate { get; set; }

        [Display(Name = "Last Activity")]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "Password Changed")]
        public DateTime LastPasswordChangedDate { get; set; }

        [Display(Name = "Locked Out")]
        public DateTime LastLockedOutDate { get; set; }

        [Display(Name = "Roles")]
        public List<string> Roles { get; set; }
    }

    public class UserCreateModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "An email address is required.")]
        [Email(ErrorMessage = "An email address is required.")]
        public string Email { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "Repeat password")]
        [EqualTo("Password", ErrorMessage = "You must repeat the password.")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Roles")]
        public string[] Roles { get; set; }
    }

    public class UserPasswordModel
    {
        [Display(Name = "Id")]
        public ObjectId Id { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "Repeat password")]
        [EqualTo("Password", ErrorMessage = "You must repeat the password.")]
        public string PasswordConfirm { get; set; }
    }
}