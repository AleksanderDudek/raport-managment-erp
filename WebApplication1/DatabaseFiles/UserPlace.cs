using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.DatabaseFiles
{
    public class UserPlace
    {
        [Key]
        [Column(Order = 0)]
        public string usersId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string placesId { get; set; }

        public virtual TrashPlaces TrashPlaces { get; set; }
        public virtual UserFrontInfo UserFrontInfo { get; set; }
    }
}