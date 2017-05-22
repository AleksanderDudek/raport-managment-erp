using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
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

        [Key]
        public int raport_id { get; set; } //unikalny zeby baza danych byla zadowolona
        //public string who_did { get; set; }
        //kto pisal, to olac, bo to przechodzi na UserID
        public string where_from { get; set; } //z jakiego miejsca, placu, to moze 
        // bedzie trzeba rozbic na 2, albo wrzucic, ze user ma swoj plac powiazany z idenity
        

        public DateTime creation_time { get; set; }
        public DateTime last_modyfication { get; set; }


        //jakas kolekcja przechowujaca kazdy osobny typ zlomu
        // przechowuje wiele instancji zlomu
        public virtual ICollection<ThrashType> ThrashType { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; } 
    }
}