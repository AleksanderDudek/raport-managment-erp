using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.DatabaseFiles
{
    public class TrashPlaces
    {
        [Key]
        public string Id { get; set; }

       
        public string nameOfThePlace { get; set; }

        public virtual ICollection<UserPlace> UserPlace { get; set; }
    }
}