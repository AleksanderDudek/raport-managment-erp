using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Klasy_Raport;

namespace WebApplication1.DatabaseFiles
{
    public class SendRecordType
    {
        [Key]
        public int IdSendRecord { get; set; }
        public string raport_id { get; set; } //unikalny id raportu,
        public string send_time { get; set; }
        public string recipient { get; set; }
        public string trash_class { get; set; }
        public double weight { get; set; }
        public bool isSend { get; set; }
        public virtual Raport Raport { get; set; }
    }
}