using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class RaportyController : Controller
    {
        // GET: Raporty
        public ActionResult WybierzRaport()
        {
            return View();
        }

        public ActionResult StanPlacu ()
        {
            return View();
        }

        public ActionResult DaneWysylki ()
        {
            return View();
        }
    }
}