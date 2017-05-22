using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Klasy_Raport
{
    public class ThrashType
    { //to bedzie pojedyncza instancja rekordu dla kazdego zlomu

        [Key] //po to, zeby tabela miala klucz glowny
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //w sumie nie, bo samo sie moze generowac ID
        public int IdTrash { get; set; }
        public string raport_id { get; set; } //unikalny id raportu,
        public string ClassName { get; set; } //pole z nazwa zlomu

        public bool isNegative { get; set; }
        public double Quantity { get; set; } //wprowadzana przez uzytkownika ilosc
       
        public string Information { get; set; } //uwagi co do zlomu

        public bool isSend { get; set; }
        //przenioslem te dane na Raport
        //public string CreatorsName { get; set; } //tu bedzie, kto to stworzyl

        //jeden zlom do jednego raportu
        public virtual Raport Raport { get; set; }   

    
        // zeby bylo wiadomo, do ktorego raportu nalezy dany rekord
        //raport zawiera info gdzie i kto i kiedy - to daje nam mozliwosc polaczenai sie z tamta tabela
    }
}