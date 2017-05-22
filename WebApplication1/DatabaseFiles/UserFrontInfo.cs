using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApplication1.Klasy_Raport;

namespace WebApplication1.DatabaseFiles
{
    public class UserFrontInfo

    {
        // tutaj dojdzie do redundancji, ale zycie :
        // nie chce grzebac w tabeli domyslnej
        // podczas zapisu do bazy skopiuje informacje o uzytkowniku

        //generalnie tez chce, zeby bylo generowane custom 
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string userId { get; set; } //to pobierze z idenity
        public string userName { get; set; } //to pobierze z idenity
        //idenity

        public virtual ICollection<Raport> Raport { get; set; } //to wiadomo 
        public virtual ICollection<UserPlace> UserPlace { get; set; }

        
    }
}