using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [DelimitedRecord(";")]
    public class MappingClass
    { 
        public string customId { get; set; }
        public string className { get; set; }
        public string referenceName { get; set; }
        public string groupOfClass { get; set; }
        public bool isActive { get; set; }
        public bool isTerminal { get; set; }
    }
}