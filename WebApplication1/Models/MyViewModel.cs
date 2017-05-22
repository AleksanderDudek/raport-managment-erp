using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class MyViewModel
    {
  
        //viewmodel do edytowania roli administratora
        public string Id { get; set; }
        public string Name { get; set; }
        public string RoleId { get; set; }

        public string PlacId { get; set; }
    }
}