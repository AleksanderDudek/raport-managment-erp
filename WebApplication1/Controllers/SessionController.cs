using FileHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;
using WebApplication1.DatabaseFiles;
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

        //tutaj zwraca liste na podstawie obiektow z formularza
        //chce jeszcze przekazac info o userze
        public ActionResult JsonResult()
        {
            


            var query = from u in db.AspNetUsers
                        join up in db.UserPlace on u.Id equals up.usersId
                        join tp in db.TrashPlaces on up.placesId equals tp.Id
                        where u.Email == User.Identity.Name
                        select tp.nameOfThePlace;//or more data if you need to.

            var field = query.FirstOrDefault();


            //to zapewnia odpowiednie info do raportu 
            ViewBag.Username = User.Identity.Name;
            try {
                ViewBag.Fieldname = field.ToString();
            }
            catch
            {
                ViewBag.Fieldname = "";
            }
           
            ViewBag.Datename = DateTime.Now.ToString();

            //tutaj bylo tylko view model, ale teraz chce zapisac do bazy
            List<TrashViewModel> json = (List<TrashViewModel>)TempData["jsonList"];


            return View(json);
        }

        [HttpPost]
        public ActionResult neededPass (IEnumerable<TrashViewModel> json)
        {

            TempData["toBeSaved"] = json;

            try
            {

                //in a real world, here will be multiple database calls - or others
                return Json(new { ok = true, newurl = Url.Action("saveToDb", "Session") });
            }
            catch (Exception ex)
            {
                //TODO: log
                return Json(new { ok = false, message = ex.Message });
            }
        }


        
        public ActionResult saveToDb ()
        {



            //raport_id nie moze zawierac niczego poza cyframi i literami reszta BAD
            //powoduje 404 error

            //najpierw raport
            Raport newRaport = new Raport();


            //potem numer tygodnia
            DateTime time = DateTime.Today;

            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            var weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            //rok
            var datePart = DateTime.Now.Year.ToString().Replace(".", "").Replace(":","").Replace(" ","");

            //login bez domeny
            var namePart = User.Identity.Name.ToString();

            //usuwanie @grupatom.pl - 12 znakow
            var nameShort = namePart.Remove((namePart.Length)-12, 12);



            var queryr = from iter in db.IteratorTable select iter;


            IQueryable<AspNetUsers> userId;

            try
            {
                userId = from ccc in db.AspNetUsers
                         where ccc.UserName == User.Identity.Name
                         select ccc;
            }
            catch
            {
                TempData["errorMessage"] = "Skontaktuj się z działem IT - sprawdź ustawienia placu dla użytkownika \n";
                db.Dispose();

                return RedirectToAction("Error", "Home");
            }

            IQueryable<UserPlace> placeId;

            try
            {
                placeId = from hhh in db.UserPlace
                          where hhh.usersId == userId.FirstOrDefault().Id
                          select hhh;
            }
            catch
            {
                TempData["errorMessage"] = "Skontaktuj się z działem IT - sprawdź ustawienia placu dla użytkownika \n";
                db.Dispose();

                return RedirectToAction("Error", "Home");

            }

            //unikalny ID dla danego tygodnia, roku, osoby i rodzaju danych
            //newRaport.raport_id = weekNumber + "I" + datePart + "I" + nameShort + "I" + "stan";
            newRaport.raport_id = queryr.FirstOrDefault().C1 + "I" + datePart + "I" + nameShort + "I" + "stan" + "I" + placeId.FirstOrDefault().placesId;


            var query = from us in db.AspNetUsers
                               where us.Email == User.Identity.Name
                               select us.Id;

            newRaport.userID = query.FirstOrDefault();

            newRaport.creation_time = DateTime.UtcNow;

            newRaport.last_modyfication = DateTime.UtcNow;

            newRaport.isMinus = 0;

        

            newRaport.place_Id = placeId.FirstOrDefault().placesId;
            //zapis do bazy 

            db.Raport.Add(newRaport);

            try
            {
                //dodaj nowy raport do bazy
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Nie mozesz dodac drugiego raportu w tym okresie. Możesz: \n"
                    +"1. Edytować utworzony raport jeśli nie minął termin dostarczenia w danym okresie. \n"
                    +"2. Jeśli powinieneś mieć możliwość dodania raportu, a nie możesz tego"
                     +"zrobić skontaktuj się z działem IT - sprawdź ustawienia placu \n";
                db.Dispose();

                return RedirectToAction("Error", "Home");
            }


            //potem wszystkie dane sprzezone z raportem

            //kontener na rekord z rap
            ThrashType ctnr = new ThrashType();

            //to ma przechowywac kolejne kontenery
            List<ThrashType> ctnrList = new List<ThrashType>(); 

            //zbiera z viewmodelu rekordy
            List<TrashViewModel> listTemp = TempData["toBeSaved"] as List<TrashViewModel>;

            foreach (var item in listTemp)
            {
                //ctnr.IdTrash = (int)item.Id;
                ctnr.raport_id = newRaport.raport_id;
                ctnr.ClassName = item.someOption;
                ctnr.isNegative = false;
                ctnr.Quantity = Double.Parse(item.someNumber);
                ctnr.Information = item.someText;
                ctnr.isSend = false;
                //zapis do bazy 
                db.ThrashType.Add(ctnr);
                db.SaveChanges();

            }



            
        
            try
            {
                //zapisz zmianę po dodaniu rekordów
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Nie mozesz dodac drugiego raportu w tym okresie. Możesz: \n"
                    + " 1. Edytować utworzony raport jeśli nie minął termin dostarczenia w danym okresie. \n"
                    + " 2. Jeśli powinieneś mieć możliwość dodania raportu, a nie możesz tego"
                     + "zrobić skontaktuj się z działem IT. \n";
                db.Dispose();

                return RedirectToAction("Error", "Home");
            }


            //po zakonczeniu edycji odłącz się od bazy
            db.Dispose();
            return RedirectToAction("Index", "Home");
        }

        //[HttpPost]
        //public ActionResult Index(IEnumerable<TrashViewModel> trashes ,StoreTest model)
        //{
        //    StoreTest _objInfo = model;
             

        //    Session["objInfo"] = _objInfo;

        //    //fajnie byloby zapisac tutaj info o raporcie

        //    //tutaj zapisuje info o skladowych raportu

        //    return RedirectToAction("Index", "Home");
        //}

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
            db.Dispose();

            ViewData["list"] = list;
            return PartialView("_SingleTrash", new TrashViewModel());
        }


        //zbedna metoda - potrzebny byl 1razowy seed, bo sql wizard sie wysypywal
        public ActionResult seedTable()
        {
            var pathStr = HttpContext.Server.MapPath("~/App_Data/KlasyPlanowanie.csv");
            var engine = new FileHelperEngine<MappingClass>();

            // To Read Use:
            var result = engine.ReadFile(pathStr);

            // To Write Use:
            //List<string> parsed = new List<string>();

            foreach (MappingClass item in result)
            {
                TrashClassTable elem = new TrashClassTable();

                elem.customId = item.customId;
                elem.className = item.className;
                elem.groupOfClass = item.groupOfClass;
                elem.referenceName = item.referenceName;
                elem.isActive = item.isActive;
                elem.isTerminal = item.isTerminal;

                db.TrashClassTable.Add(elem);
                db.SaveChanges();

                //parsed.Add(item.customId);
            }

            db.Dispose();

            return RedirectToAction("Index", "Home");
        }

    }
}

