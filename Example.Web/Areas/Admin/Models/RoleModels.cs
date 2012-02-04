using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using MongoDB.Bson;

using Example.Web.Data;

namespace Example.Web.Areas.Admin.Models
{
    public class RoleQueryModel
    {
        public ObjectId Id { get; set; }
    }

    public class RoleDeleteModel
    {
        [Display(Name = "Id")]
        public ObjectId Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Confirmation")]
        [Mandatory(ErrorMessage = "Required")]
        public bool Confirmation { get; set; }
    }

    public class RoleEditModel
    {
        [Display(Name = "Id")]
        public ObjectId Id { get; set; }

        [Display(Name = "Name")]        
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(32)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
    }
}
