using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.DatabaseFiles
{
    public class TrashClassTable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public string referenceName { get; set; }
        public string customId { get; set; }
        public string className { get; set; }
    
        public string groupOfClass { get; set; }
        public bool isActive { get; set; }
        public bool isTerminal { get; set; }

    }
}