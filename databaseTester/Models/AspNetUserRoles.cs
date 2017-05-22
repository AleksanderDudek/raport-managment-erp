namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AspNetUserRoles
    {
        //to daje mozliwosci zwiazane z rolami, ale nie pozwala przydzielac klucza roli to bazy
        //[Key]
        //[Column(Order = 0)]
        //public string UserId { get; set; }

        //[Key]
        //[Column(Order = 1)]
        //public string RoleId { get; set; }
        //public virtual AspNetUsers AspNetUsers { get; set; }

        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string RoleId { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }

    }
}
