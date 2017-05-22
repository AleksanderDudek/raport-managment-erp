using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace WebApplication1.Models
{

    //viewmodel do przechowywania danych z pojedynczej
    //linii formularza 

    [Serializable]
    public class TrashViewModel
    {
        public string Id { get; set; }
        public string someOption { get; set; }
        public string someText { get; set; }
        public string someNumber { get; set; }

    }
}