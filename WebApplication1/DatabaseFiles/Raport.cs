using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WebApplication1.DatabaseFiles;
using WebApplication1.Models;

namespace WebApplication1.Klasy_Raport
{


    //co ma generalnie robic czlowiek? czy tylko wprowadzac informacje o stanie?
    //czy tez zmiany tzn. spadki ilosci itd. jak te raporty wygladaja?

    //jesli chce powolac do zycia to musze zrobic WebAppInitializer: ... entity
    //genralnie zrobic seed'a dla bazy albo samo dropcreatedatabase 

    public class Raport
    {

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public Raport()
        //{
        //    ThrashType = new HashSet<ThrashType>();
        //    //Raport = new HashSet<Raport>();
        //}

        //chce robic custom made raporty id, wiec to zostaje
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string raport_id { get; set; } //id raportu
        public string userID { get; set; }

        public DateTime creation_time { get; set; }

        public int isMinus { get; set; }

        public DateTime last_modyfication { get; set; }

        //zamiast skladowania sumy po prostu bede ja obliczal za pomoca
        //linq i zwracal tempem do widoku
        //public float sum_of_records { get; set; }

        public string place_Id { get; set; }


        public virtual ICollection<ThrashType> ThrashType { get; set; } //rekordy zlomu

        public virtual ICollection<SendRecordType> SendRecordType { get; set; }
        public virtual UserFrontInfo UserFrontInfo { get; set; } //info o pracowniku
    }
}