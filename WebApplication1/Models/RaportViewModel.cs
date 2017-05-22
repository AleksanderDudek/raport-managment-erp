using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class RaportViewModel
    {
        public string whoCreated { get; set; }
        public string kindOfRaport { get; set; }
        public bool isMinus { get; set; }
        public string raportId { get; set; }
        public DateTime creationTime { get; set; }
        public DateTime modificationTime { get; set; }

    }
}