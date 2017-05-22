using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Klasy_Raport
{
    public class ThrashType
    { //to bedzie pojedyncza instancja rekordu dla kazdego zlomu

        [Key] //po to, zeby tabela miala klucz glowny
        public int IdTrash { get; set; }
        public string ClassName { get; set; } //pole z nazwa zlomu

        public int Quantity { get; set; } //wprowadzana przez uzytkownika ilosc
       
        public string Information { get; set; } //uwagi co do zlomu

        //przenioslem te dane na Raport
        //public string CreatorsName { get; set; } //tu bedzie, kto to stworzyl

        //jeden zlom do jednego raportu
        public virtual Raport Raport { get; set; }  //unikalny id raportu, 
        // zeby bylo wiadomo, do ktorego raportu nalezy dany rekord

    }
}