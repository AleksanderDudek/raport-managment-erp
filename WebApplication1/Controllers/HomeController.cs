using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Text.RegularExpressions;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication1.Models;
using FileHelpers;
using WebApplication1.DatabaseFiles;
using System.IO;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private MyModel tbls = new MyModel();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        //to dziala
        public ActionResult Records()
        {




            //var users = tbls.AspNetUsers;
            //var roles = tbls.AspNetUserRoles;

            //var result = from user in users.AsEnumerable()
            //             join role in roles.AsEnumerable()
            //             on user.Id equals role.UserId
            //             into g
            //             from role in g.DefaultIfEmpty()
            //             select new MyViewModel
            //             {
            //                 Id = user.Id,
            //                 Name = user.UserName,
            //                 RoleId = role.RoleId,
            //                 PlacId = ""
            //             };

            //var all = from a in tbls.AspNetUsers
            //          join b in tbls.AspNetUserRoles on a.Id equals b.UserId
            //          select new { a.Id, a.UserName, b.RoleId };


            var all = from users in tbls.AspNetUsers
                      select users;

            List < MyViewModel > result = new List<MyViewModel>();
            string alt = "brak";

            foreach (var item in all)
            {
                MyViewModel ctnr = new MyViewModel();
                var name = "";
                var role = "";


                var hasAssign = from a in tbls.UserPlace
                                 where a.usersId == item.Id
                                 select a;
                //id miejsca
                 /*= hasAssign == null ? alt : hasAssign.FirstOrDefault().placesId;*/

             
                    try
                    {
                        var checker = hasAssign.FirstOrDefault().placesId;
                        var placeName = from place in tbls.TrashPlaces
                                        where place.Id == checker
                                        select place;
                        //nazwa
                        name = placeName == null ? alt : placeName.FirstOrDefault().nameOfThePlace;
                    }
                    catch 
                    {
                        name = "brak";
                    }
                    //nazwa miejsca            
                   
               
                    try //jak ma role 
                {

                    var userRoleId = from a in tbls.UserFrontInfo
                                     where a.userName == item.UserName
                                     join b in tbls.AspNetUserRoles
                                     on a.userId equals b.UserId
                                     select b;


                    var this_name = userRoleId.FirstOrDefault().RoleId ;
                    //rollname daje null'a

                    var roleName = from roles in tbls.AspNetRoles
                                   where roles.Id == this_name
                                   select roles;


                   
                    role = roleName == null ? alt : roleName.FirstOrDefault().Name;

                }
                catch
                {
                    role = "Nieaktywny";
                }


               

                //if (hasAssign == null)
                //{
                //    ctnr.Id = item.Id;
                //    ctnr.Name = item.UserName;
                //    ctnr.RoleId = item.RoleId;
                //    ctnr.PlacId = "brak";
                //    result.Add(ctnr);
                //}
                //else
                //{
                ctnr.Id = item.Id;
                    ctnr.Name = item.UserName;
                    ctnr.RoleId = role;
                ctnr.PlacId = name;
                    result.Add(ctnr);

                //}

            }



 
            //jak zrobic, zeby te dane takze pobieralo do zmiennej 
            //typu model i przekazac IDki tak, zeby moc potem wiedziec, ktory sie edytuje 


            //tu NIBY jest problem, w praktyce do RoleId zamiast RoleId daje Id...
            return View(result);
        }


        [HttpGet]
        public ActionResult Edit (string userId)
        {
            //czyli id przekazywane jest jednak NULL
            AspNetUserRoles personRole;
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ///tutaj wychodzi blad przy AspNetUserRoles
            ///

            //wybiera rekord dla danego uzytkownika
            var query = from roles in tbls.AspNetUserRoles
                        where roles.UserId == userId
                        select roles;

            personRole = query.FirstOrDefault();             //ma problem z odnalezieniem, ale 


            if (personRole == null)
            {
                return HttpNotFound();
            }
            return View(personRole);
        }


        public ActionResult EditPlac ()
        {

            string userId = Request.QueryString["userId"];

            //to zwraca nam nazwe placu, do ktorego przypisany jest user o zadanym id
            var query = from qu in tbls.UserPlace
                        join du in tbls.TrashPlaces
                        on qu.placesId equals du.Id
                        where qu.usersId == userId
                        select du.nameOfThePlace;

            string infStr;

            if(query.FirstOrDefault()==null)
            {
                infStr = "Jeszcze nie przypisano placu";
                TempData["userPlace"] = infStr;
            }
            else
            {
                TempData["userPlace"] = query.FirstOrDefault();

            }

            //to zwraca wszystkie place, do ktorych mozna przypisac usera



            var result = from ddd in tbls.TrashPlaces
                         select ddd;

            List <string> parsed = new List<string>();
            string holder;


            foreach (var item in result)
            {
                holder = item.nameOfThePlace;
                parsed.Add(holder);
            }

            TempData["listForPlaces"] = parsed;

            TempData["stringId"] = userId;

            //przekaze userId i uzyje w hidden input 
            return View();
        }

        [HttpPost]
        public ActionResult EditPlac(UserIdPlaceName form)
        {

            //wyciaga ID placu, ktory teraz user bedzie mial+
            var query = from plac in tbls.TrashPlaces
                        where plac.nameOfThePlace == form.namePlace
                        select plac;

            //znajduje rekord user-plac jesli istnieje 
            var ifAlr = from places in tbls.UserPlace
                        where places.usersId == form.nameId
                        select places;

            var item = ifAlr.FirstOrDefault();

            if(item == null)
            {
                //zapisz nowy
                UserPlace newest = new UserPlace();
                newest.placesId = query.FirstOrDefault().Id;
                newest.usersId = form.nameId;

                tbls.UserPlace.Add(newest);
                tbls.SaveChanges();
            }
            else
            {

                //nie moge update'a zrobic, ale moge usunac stary i dodac nowy


                //stary

                tbls.UserPlace.Remove(ifAlr.FirstOrDefault());
                tbls.SaveChanges();
                //nowy
                UserPlace newest = new UserPlace();
                newest.placesId = query.FirstOrDefault().Id;
                newest.usersId = form.nameId;

                tbls.UserPlace.Add(newest);
                tbls.SaveChanges();

            }

            tbls.Dispose();

            //pokazuje uzytkownikow
            return RedirectToAction("Records");
        }

        [HttpGet]
        public ActionResult partial_angus(int id)
        {


            //////tu z pliku na serwerze

            //var pathStr = HttpContext.Server.MapPath("~/App_Data/KlasyPlanowanie.csv");
            //var engine = new FileHelperEngine<MappingClass>();

            //// To Read Use:
            //var result = engine.ReadFile(pathStr);
            //// result is now an array of Customer

            //// To Write Use:
            //List<string> parsed = new List<string> ();

            //foreach (MappingClass item in result)
            //{
            //    parsed.Add(item.customId);
            //}


            //////////////////tutaj z bazy
            var classQuery = from cls in tbls.TrashClassTable
                             //where cls.groupOfClass == "ZC" && cls.isActive == true
                             select cls;

            List<string> parsed_db = new List<string>();

            foreach (var item in classQuery)
            {
                parsed_db.Add(item.customId);
            }
            parsed_db.Sort();


            //plik
            //TempData["listForSelect"] = parsed;

            //baza danych
            TempData["listForSelect"] = parsed_db;

            ViewBag.IdNotId = id;
            tbls.Dispose();

            return PartialView("partial_angus");
        }


        [HttpGet]
        public ActionResult angus()
        {




            //////tu z pliku na serwerze
            
            //var pathStr = HttpContext.Server.MapPath("~/App_Data/KlasyPlanowanie.csv");
            //var engine = new FileHelperEngine<MappingClass>();

            //// To Read Use:
            //var result = engine.ReadFile(pathStr);
            //// result is now an array of Customer

            //// To Write Use:
            //List<string> parsed = new List<string> ();

            //foreach (MappingClass item in result)
            //{
            //    parsed.Add(item.customId);
            //}


            //////////////////tutaj z bazy
            var classQuery = from cls in tbls.TrashClassTable
                             //where cls.groupOfClass == "ZC" && cls.isActive == true
                             select cls;

            List<string> parsed_db = new List<string>();

            foreach (var item in classQuery)
            {
                parsed_db.Add(item.customId);
            }

            parsed_db.Sort();

             //plik
             //TempData["listForSelect"] = parsed;
             
            //baza danych
            TempData["listForSelect"] = parsed_db;
            tbls.Dispose();

            return View();
        }

        [HttpPost]
        public ActionResult angus(IEnumerable<TrashViewModel> json)
        {
           

            
            TempData["jsonList"] = json;
           
            //na razie id ich niech sobie beda od 1 do n
            //przekazuje do widoku z edytorem i tam przypisuje raport_id do widoku
            //temp ma jsona a controller zwroci raport
            //raport id to username+datetime
            


            try
            {
                var line = Json(new { ok = true, newurl = Url.Action("JsonResult", "Session") });
                //in a real world, here will be multiple database calls - or others
                return line;
            }
            catch (Exception ex)
            {
                //TODO: log
                return Json(new { ok = false, message = ex.Message });
            }


        }



        //tutaj podaje konto i token np. "?account=odudek&token=e7ff25f2fe59d798849076e42f829dca"
        //w oryginale Witek generuje linki zawierajace nazwe konta i dany token
        //wazne!!! poki co generuje sie 1 token / dzien!!
        public ActionResult TestAuto()
        {
            //pobiera parametr nazwy konta
            string name = Request.QueryString["account"];

            //data jako jeden z elementow szyfru
            string date = DateTime.Today.ToString();

            //domieszka, ale tylko w formie stringa
            string secret = "szyfrator";
            MD5 haszer = MD5.Create();

            //polaczony string
            StringBuilder builder = new StringBuilder();
            builder.Append(name);
            builder.Append(date);
            builder.Append(secret);

            string input = builder.ToString();

            //zamiana na strumien danych przy haszowaniu
            byte[] data = haszer.ComputeHash(Encoding.UTF8.GetBytes(input));

            //zamiana na hexy
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            //przypisanie do ViewBag'a dla danego 
            ViewBag.Message = sBuilder.ToString();

            string ctner = Request.QueryString["token"];
            if (ctner != null)
            {
                if (String.Equals(ctner, ViewBag.Message))
                {
                    /* tutaj bedzie pewnie redirect i nadanie uprawnien*/

                    //nadaj uprawnienia, loguje uzytkownika


                    ViewBag.Message = "Nadano uprawnienia: " + ctner;

                    //moge nadac uprawnienia robiac tak, ze wysylam dane w niewidowcznym formularzu
                    //moge przekierowac link dodajac nazwe uzytkownika, aby tam go zwalidowal
                    // link np. 
                    
                    StringBuilder login = new StringBuilder();

                    login.Append(Request.QueryString["account"]);
                    login.Append("@grupatom.pl");

                        // 1 param = akcja, 2 param = kontroler, 3 param = automatyczne 
                        //zapytanie czyli new { account = login } daje ?account=login na koncu url
                    return RedirectToAction("Login", "Account", new { account = login });
                }
                else
                {
                    /* redirect do bledu, potem bedzie jakas obsluga i komunikat */
                    return RedirectToAction("ErrorAction");
                }
            }
            else
            {


                //takie rozwiazanie powoduje nieodnalezienie zasobu
                //URL odwoluje sie do zasobu, ktory nie istnieje



                //return RedirectToRoute(new
                //{
                //    controller = "Account",
                //    action = "Login",
                //    id = "account=test@test.pl"
                //}
                //    );

                //string host = Request.Url.Host;
                //string port = Request.Url.Port.ToString();

                //string url = host+":"+port+"/Account/Login?account=test@test.pl";

                //Uri myUri = new Uri(url, UriKind.);

                //return RedirectToRoute(myUri);


                return RedirectToAction("Error");
            }


            //przekazuje do widoku

        }

        public ActionResult Error()
        {
            return View("Error");
        }


        //tutaj podaje parametr zwracajacy mi token np. "?account=odudek"
        //to jest stworzone na potrzeby testowania


        public ActionResult Autoryzacja()
        {

            string name = Request.QueryString["account"];

            //data jako jeden z elementow szyfru
            string date = DateTime.Today.ToString();

            //domieszka, ale tylko w formie stringa
            string secret = "szyfrator";
            MD5 haszer = MD5.Create();

            //polaczony string
            StringBuilder builder = new StringBuilder();
            builder.Append(name);
            builder.Append(date);
            builder.Append(secret);

            string input = builder.ToString();

            //zamiana na strumien danych przy haszowaniu
            byte[] data = haszer.ComputeHash(Encoding.UTF8.GetBytes(input));

            //zamiana na hexy
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            //przypisanie do ViewBag'a dla danego 
            ViewBag.Message = sBuilder.ToString();

            return View();
        }

        public ActionResult KlasyPlanowanie ()
        {
            //wszystkei klasy i dane chcemy 
            var query = from list in tbls.TrashClassTable
                        select list;

            return View(query as IEnumerable<TrashClassTable>);
        }

        public ActionResult CreateClass ()
        {

            return View();
        }
        [HttpPost]
        public ActionResult CreateClass(TrashClassTable newClass)
        {

            tbls.TrashClassTable.Add(newClass);
            tbls.SaveChanges();
            tbls.Dispose();

            return RedirectToAction("KlasyPlanowanie");
        }

        public ActionResult EditClass(string id)

            //zapomnialem, ze nie odczyta id, ktore zawiera znaki specjalne 
        {
            var query = from tst in tbls.TrashClassTable
                        where tst.referenceName == id
                        select tst;

            MappingClass mapped = new MappingClass();

            mapped.referenceName = query.FirstOrDefault().referenceName;
            mapped.className = query.FirstOrDefault().className;
            mapped.groupOfClass = query.FirstOrDefault().groupOfClass;
            mapped.isActive = query.FirstOrDefault().isActive;
            mapped.isTerminal = query.FirstOrDefault().isTerminal;
            mapped.customId = query.FirstOrDefault().customId;


            return View(mapped);
        }

        [HttpPost]
        public ActionResult EditClass(TrashClassTable updatedClass)
        {
            var query = from cls in tbls.TrashClassTable
                        where cls.customId == updatedClass.customId
                        select cls;

            query.FirstOrDefault().customId = updatedClass.customId;
            query.FirstOrDefault().className = updatedClass.className;
            query.FirstOrDefault().groupOfClass = updatedClass.groupOfClass;
            query.FirstOrDefault().referenceName = updatedClass.referenceName;
            query.FirstOrDefault().isActive = updatedClass.isActive;
            query.FirstOrDefault().isTerminal = updatedClass.isTerminal;

            tbls.SaveChanges();
            tbls.Dispose();
            return RedirectToAction("KlasyPlanowanie");
        }
    }
}