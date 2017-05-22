using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using WebApplication1.Klasy_Raport;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SessionController : Controller
    {

        MyModel db = new MyModel();
        // GET: Session
        public ActionResult Index()
        {
            //tutaj zmien na tabele zawierajaca slownik zlomow
            var cont = db.AspNetUsers.ToList();
            var list = new List<SelectListItem> ();
           

            foreach (var item in cont)
            {
                SelectListItem ctr = new SelectListItem();
                ctr.Text = item.Email;
                ctr.Value = item.Email;
                list.Add(ctr);
            }
            
           ViewData["list"] = list;

            var initialData = new[] {
                new TrashViewModel { someOption ="", someNumber="0", someText="" },
                new TrashViewModel { someOption ="", someNumber="0", someText="" }
            };

            return View(initialData);
        }

        [HttpGet]
        [WebMethod]
        public JsonResult GetKinds()
        {

            JavaScriptSerializer TheSerializer = new JavaScriptSerializer();
            //tutaj po prostu przekaze rodzaje do listy
            List<TrashTypeModel> list = new List<TrashTypeModel>();
            list.Add(new TrashTypeModel { Id = 1, Name = "Pierwszy" });
            list.Add(new TrashTypeModel { Id = 2, Name = "Dwa" });
            list.Add(new TrashTypeModel { Id = 3, Name = "Trzy" });
            list.Add(new TrashTypeModel { Id = 4, Name = "434" });
            list.Add(new TrashTypeModel { Id = 5, Name = "dad123" });




            return (Json(TheSerializer.Serialize(list), JsonRequestBehavior.AllowGet));
        }

        
        public ActionResult JsonResult()
        {

            List<TrashViewModel> json = (List<TrashViewModel>)TempData["jsonList"];



            return View(json);
        }

        [HttpPost]
        public ActionResult Index(IEnumerable<TrashViewModel> trashes ,StoreTest model)
        {
            StoreTest _objInfo = model;
             

            Session["objInfo"] = _objInfo;

            //fajnie byloby zapisac tutaj info o raporcie

            //tutaj zapisuje info o skladowych raportu

            return RedirectToAction("Index", "Home");
        }

        public PartialViewResult Add()
        {
            var cont = db.AspNetUsers.ToList();
            var list = new List<SelectListItem>();


            foreach (var item in cont)
            {
                SelectListItem ctr = new SelectListItem();
                ctr.Text = item.Email;
                ctr.Value = item.Email;
                list.Add(ctr);
            }

            ViewData["list"] = list;
            return PartialView("_SingleTrash", new TrashViewModel());
        }
    }
}

