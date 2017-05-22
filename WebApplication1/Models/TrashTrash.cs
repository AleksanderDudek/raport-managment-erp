using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Serializable]
    public class TrashTrash
    {
        [Key] //po to, zeby tabela miala klucz glowny
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //w sumie nie, bo samo sie moze generowac ID
        public int IdTrash { get; set; }
        public string raport_id { get; set; } //unikalny id raportu,
        public string ClassName { get; set; } //pole z nazwa zlomu

        public bool isNegative { get; set; }
        public double Quantity { get; set; } //wprowadzana przez uzytkownika ilosc

        public string Information { get; set; } //uwagi co do zlomu
    }
}