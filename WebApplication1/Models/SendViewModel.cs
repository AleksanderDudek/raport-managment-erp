using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class SendViewModel
    {
        public string send_time { get; set; }

        public string recipient { get; set; }
        public string class_name { get; set; }
        public string weight { get; set; }
    }
}