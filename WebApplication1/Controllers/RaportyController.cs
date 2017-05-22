using FileHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.DatabaseFiles;
using WebApplication1.Klasy_Raport;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class RaportyController : Controller
    {
        MyModel db = new MyModel();
        // GET: Raporty
        public ActionResult WybierzRaport()
        {
            return View();
        }

        public ActionResult partial_poprzedni (int id)
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
            var classQuery = from cls in db.TrashClassTable
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
            db.Dispose();

            return PartialView("partial_poprzedni");

        }

        public ActionResult PoprzedniSchematRaport()
        {
            //pobiera poprzedni raport i przekazuje TrashType z nim zwiazane 
            //do widoku jako kolekcje 

            ////najswiezszy raport

            //imie uzytkownika
            var usersName = User.Identity.Name;

            //id uzytkownika na podstawie imienia
            var idQuery = from ddd in db.UserFrontInfo
                          where ddd.userName == usersName
                          select ddd.userId;

            //wybiera wszystkie raporty uzytkownika
            var userRapQuery = from dbs in db.Raport
                               where dbs.userID == idQuery.FirstOrDefault()
                               select dbs;
            //tutaj wybiera najswiezszy raport uzytkownika typu STAN
            var newestRaport = from newest in userRapQuery
                               where newest.isMinus == 0
                               select newest;

            var answer = newestRaport.OrderByDescending(x => x.raport_id).First();

            string newestRapId;
            ////najswiezszy raport
            try
            {
                //newestRapId = newestRaport.First().raport_id;
                newestRapId = answer.raport_id;
            }
            catch
            {
                @TempData["errorMessage"] = "Nie wprowadziłeś jeszcze żadnego raportu! Spokojnie - zrób tylko pierwszy i potem możesz go wygodnie odtworzyć i edytować.";
                return RedirectToAction("Error", "Home");
            }

            //wybiera rekordy zwiazane z raportem
            var newestRecords = from trashes in db.ThrashType
                                where trashes.raport_id == newestRapId
                                select trashes;

            List<TrashViewModel> newRecList = new List<TrashViewModel>();


            //przerobienie TrashType na TrashViewModel
            foreach (var record in newestRecords)
            {
                TrashViewModel newRec = new TrashViewModel();

                newRec.Id = record.IdTrash.ToString();
                newRec.someOption = record.ClassName;
                newRec.someNumber = record.Quantity.ToString().Replace(',','.');
                newRec.someText = record.Information;

                newRecList.Add(newRec);
            }


            //sam raport idzie tak jak osatnio przez tempdata
            //albo nie idzie bo nikogo to nie obchodzi, za to musi powstac nowa instancja raportu
            //moge po zczytaniu danych utworzyc i chyba tak zrobie



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
            var classQuery = from cls in db.TrashClassTable
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
            db.Dispose();

            return View(newRecList);
        }

        public ActionResult PoprzedniSchematRaportSend()
        {
            //pobiera poprzedni raport i przekazuje TrashType z nim zwiazane 
            //do widoku jako kolekcje 

            ////najswiezszy raport
            var usersName = User.Identity.Name;

            //id uzytkownika
            var idQuery = from ddd in db.UserFrontInfo
                          where ddd.userName == usersName
                          select ddd.userId;

            //wybiera wszystkie raporty
            var userRapQuery = from dbs in db.Raport
                               where dbs.userID == idQuery.FirstOrDefault()
                               select dbs;
            //tutaj wybiera najswiezszy raport (a powinien wybierac najswiezszy typu SEND)
            var newestRaport = from newest in userRapQuery
                               where newest.isMinus == 1
                               select newest;

            var answer = newestRaport.OrderByDescending(x => x.raport_id).First();

            string newestRapId;
            ////najswiezszy raport
            try
            {
                //newestRapId = newestRaport.First().raport_id;
                newestRapId = answer.raport_id;
            }
            catch
            {
                @TempData["errorMessage"] = "Nie wprowadziłeś jeszcze żadnego raportu! Spokojnie - zrób tylko pierwszy i potem możesz go wygodnie odtworzyć i edytować.";
                return RedirectToAction("Error", "Home");
            }

            //wybiera rekordy zwiazane z raportem
            var newestRecords = from trashes in db.SendRecordType
                                where trashes.raport_id == newestRapId
                                select trashes;

            List<SendViewModel> newRecList = new List<SendViewModel>();


            //przerobienie TrashType na TrashViewModel
            foreach (var record in newestRecords)
            {
                SendViewModel newRec = new SendViewModel();

                newRec.send_time = record.send_time;
                newRec.recipient = record.recipient;
                newRec.class_name = record.trash_class;
                newRec.weight = record.weight.ToString().Replace(',','.');

                newRecList.Add(newRec);
            }


            //sam raport idzie tak jak osatnio przez tempdata
            //albo nie idzie bo nikogo to nie obchodzi, za to musi powstac nowa instancja raportu
            //moge po zczytaniu danych utworzyc i chyba tak zrobie



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
            var classQuery = from cls in db.TrashClassTable
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
            db.Dispose();

            return View(newRecList);
        }

        public ActionResult PoprzedniSchemat ()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Nadzorca")]
        public ActionResult EdytujRaport()
        {
            

            //dla admina, wybiera wszystkie raporty
            var query = from dbs in db.Raport
                        select dbs;
            //dla admina


            List<RaportViewModel> raportList = new List<RaportViewModel>();
            List<Raport> raports = new List<Raport>();

            foreach (var item in query)
            {
                Raport holder = new Raport();
                holder = item;
                raports.Add(holder);

            }

            List<UserFrontInfo> userFI = new List<UserFrontInfo>();
            var itemName = from abc in db.UserFrontInfo
                           select abc;

            foreach (var item in itemName)
            {
                UserFrontInfo holder = new UserFrontInfo();
                holder = item;
                userFI.Add(holder);
            }

            foreach (var item in raports)
            {
                RaportViewModel elem = new RaportViewModel();
                //znajduje imie na podstawie userID



                var answer = from it in userFI
                             where it.userId == item.userID
                             select it;
                elem.whoCreated = answer.FirstOrDefault().userName;

                if (item.isMinus == 0)
                {
                    elem.kindOfRaport = "STAN PLACU";
                    elem.isMinus = false;
                }
                else
                {
                    elem.kindOfRaport = "WYSYŁKA";
                    elem.isMinus = true;
                }
                elem.raportId = item.raport_id;
                elem.creationTime = item.creation_time;
                elem.modificationTime = item.last_modyfication;

                raportList.Add(elem);
            }
            return View(raportList);
        }

        public ActionResult EdytujRaportUser()
        {


            //wydobycie id na podstawie imienia
            var userId = from name in db.AspNetUsers
                         where name.UserName == User.Identity.Name
                         select name;

            //wydobycie raportow dla uzytkownika o podanym id
            var query = from dbs in db.Raport
                        where dbs.userID == userId.FirstOrDefault().Id
                        select dbs;
            //wydobycie raportow z danego okresu rozliczeniowego

            //1. wydobycie prefiksa
            var period = from ddd in db.IteratorTable
                         select ddd;
            var strMatch = (period.FirstOrDefault().C1).ToString();
            //2. wydobycie raportow

            List<RaportViewModel> listOfSendrecords = new List<RaportViewModel>();
            //List<string> recip = new List<string>();
            //List<string> clas = new List<string>();

            foreach (Raport item in query)
            {

                RaportViewModel ctnr = new RaportViewModel();
                var length = strMatch.Length;

                //to sprawdza czy biezacy raport_id rekordu ma prefix tej samej dlugosci
                //wiedzac, ze na okreslonej pozycji dla zadanej dlugosci musi znajdowac sie duze I
                if (item.raport_id.ElementAt(length) == 'I')
                {
                    //prefix tej samej dlugosci
                    if (item.raport_id.StartsWith(strMatch))
                    {
                        ctnr.whoCreated = User.Identity.Name;
                        ctnr.isMinus = item.isMinus==1;
                        ctnr.creationTime = item.creation_time;
                        ctnr.modificationTime = item.last_modyfication;
                        ctnr.raportId = item.raport_id;
                        ctnr.kindOfRaport = "tup";

                      
                        //nasz rekord
                        listOfSendrecords.Add(ctnr);
                    }

                }
                else
                {//nie zgadzaja sie 
                    continue;
                }
            }


            var nameOfTheUser = from abc in db.UserFrontInfo
                                where abc.userId == userId.FirstOrDefault().Id
                                select abc;

            ViewBag.Message = nameOfTheUser.FirstOrDefault().userName;

            return View(listOfSendrecords);
        }

        public ActionResult partial_send (int id)
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
            var classQuery = from cls in db.TrashClassTable
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
            return PartialView("partial_send");
        }

        public ActionResult partial_sendPoprzedni(int id)
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
            var classQuery = from cls in db.TrashClassTable
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
            return PartialView("partial_sendPoprzedni");
        }

        [Authorize(Roles ="KierownikActive")]
        public ActionResult OgladajRaporty ()
        {
            //wydobycie id na podstawie imienia
            var userId = from name in db.AspNetUsers
                         where name.UserName == User.Identity.Name
                         select name;

            //wydobycie raportow dla uzytkownika o podanym id
            var query = from dbs in db.Raport
                        where dbs.userID == userId.FirstOrDefault().Id
                        select dbs;


            var nameOfTheUser = from abc in db.UserFrontInfo
                                where abc.userId == userId.FirstOrDefault().Id
                                select abc;

            ViewBag.Message = nameOfTheUser.FirstOrDefault().userName;

            return View(query as IEnumerable<Raport>);
        }

        [Authorize(Roles = "Nadzorca, Administrator")]
        public ActionResult OgladajRaportyWszystkie()
        {
            ////wydobycie id na podstawie imienia
            //var userId = from name in db.AspNetUsers
            //             where name.UserName == User.Identity.Name
            //             select name;

            //wydobycie raportow wszystkich
            var query = from dbs in db.Raport
                        select dbs;

            List<RaportViewModel> raportList = new List<RaportViewModel>();
            List<Raport> raports = new List<Raport>();

            foreach (var item in query)
            {
                Raport holder = new Raport();
                holder = item;
                raports.Add(holder);

            }

            List<UserFrontInfo> userFI = new List<UserFrontInfo>();
            var itemName = from abc in db.UserFrontInfo
                           select abc;

            foreach (var item in itemName)
            {
                UserFrontInfo holder = new UserFrontInfo();
                holder = item;
                userFI.Add(holder);
            }

            foreach (var item in raports)
            {
                RaportViewModel elem = new RaportViewModel();
                //znajduje imie na podstawie userID



                var answer = from it in userFI
                                  where it.userId == item.userID
                                  select it;
                elem.whoCreated = answer.FirstOrDefault().userName;

                if (item.isMinus==0)
                {
                    elem.kindOfRaport = "STAN PLACU";
                    elem.isMinus = false;
                }
                else
                {
                    elem.kindOfRaport = "WYSYŁKA";
                    elem.isMinus = true;
                }
                elem.raportId = item.raport_id;
                elem.creationTime = item.creation_time;
                elem.modificationTime = item.last_modyfication;

                raportList.Add(elem);
            }


            return View(raportList);
        }

        [Authorize(Roles = "KierownikActive, Nadzorca, Administrator")]
        public ActionResult OgladajRekordy (string id)
        {
            //lapie raport o zadanym id
            var rapQuery = from dbsdbs in db.Raport
                           where dbsdbs.raport_id == id
                           select dbsdbs;

            //wylapuje imie osoby, ktora utworzyla ten raport
            var nameQuery = from rap in rapQuery
                            join users in db.AspNetUsers
                            on rap.userID equals users.Id
                            where users.Id == rap.userID
                            select users.UserName;




            //kontener na dany raport
            Raport raper = rapQuery.FirstOrDefault() as Raport;
            //pobranie imienia 
            TempData["userName"] = nameQuery.FirstOrDefault();

            string kind;

            if (raper.isMinus == 0)
            {
                kind = "STAN PLACU";
            }
            else
            {
                kind = "WYSYLKA";
            }
            TempData["raportKind"] = kind;

            TempData["lastModification"] = raper.last_modyfication;

            //jesli jest stan placu to odpytuje tabele rekordow stanu placu:
            if(raper.isMinus == 0)
            {
                var query = from dbs in db.ThrashType
                            where dbs.raport_id == id
                            select dbs;

                //zwraca wszystkie rekordy zwiazane z danym raportem
                return View(query as IEnumerable<ThrashType>);
            }
            else
            { //jesli jest to wysylka to odpytuje tabele rekordow wysylki:
                var query = from dbs in db.SendRecordType
                            where dbs.raport_id == id
                            select dbs;


                return View("OgladajRekordySend", query as IEnumerable<SendRecordType>);
            }
           
        }

        public ActionResult OgladajRekordySend(string id)
        {
            //lapie raport o zadanym id
            var rapQuery = from dbsdbs in db.Raport
                           where dbsdbs.raport_id == id
                           select dbsdbs;

            //wylapuje imie osoby, ktora utworzyla ten raport
            var nameQuery = from rap in rapQuery
                            join users in db.AspNetUsers
                            on rap.userID equals users.Id
                            where users.Id == rap.userID
                            select users.UserName;




            //kontener na dany raport
            Raport raper = rapQuery.FirstOrDefault() as Raport;
            //pobranie imienia 
            TempData["userName"] = nameQuery.FirstOrDefault();

            string kind;

            if (raper.isMinus == 0)
            {
                kind = "STAN PLACU";
            }
            else
            {
                kind = "WYSYLKA";
            }
            TempData["raportKind"] = kind;

            TempData["lastModification"] = raper.last_modyfication;

            //jesli jest stan placu to odpytuje tabele rekordow stanu placu:
            if (raper.isMinus == 0)
            {
                var query = from dbs in db.ThrashType
                            where dbs.raport_id == id
                            select dbs;

                //zwraca wszystkie rekordy zwiazane z danym raportem
                return View(query as IEnumerable<ThrashType>);
            }
            else
            { //jesli jest to wysylka to odpytuje tabele rekordow wysylki:
                var query = from dbs in db.SendRecordType
                            where dbs.raport_id == id
                            select dbs;


                return View("OgladajRekordySend", query as IEnumerable<SendRecordType>);
            }

        }

        //zwraca rekordy dla danego raportu
        //[HttpGet]
        public ActionResult EdytujRekordy (string id)
        {
            //tutaj jeszcze przekazuje w tempie raport informacje

            //tu w sumie wszystko wyko
            var rapQuery = from dbsdbs in db.Raport
                           where dbsdbs.raport_id == id
                           select dbsdbs;

            var nameQuery = from rap in rapQuery
                            join users in db.AspNetUsers
                            on rap.userID equals users.Id
                            where users.Id == rap.userID
                            select users.UserName;





            Raport raper = rapQuery.FirstOrDefault() as Raport;
            //konwersja 
            TempData["userName"] = nameQuery.FirstOrDefault();

            string kind;

            if(raper.isMinus==0)
            {
                kind = "STAN PLACU";
            }
            else
            {
                kind = "WYSYLKA";
            }
            TempData["raportKind"] = kind;
            TempData["raportIdik"] = id;
            TempData["lastModification"] = raper.last_modyfication;

            if(raper.isMinus == 0)
            {
                var query = from dbs in db.ThrashType
                            where dbs.raport_id == id
                            select dbs;



                //zwraca wszystkie rekordy zwiazane z danym raportem
                return View(query as IEnumerable<ThrashType>);
            }
            else
            {
                var query = from dbs in db.SendRecordType
                            where dbs.raport_id == id
                            select dbs;



                //zwraca wszystkie rekordy zwiazane z danym raportem
                return View(query as IEnumerable<SendRecordType>);
            }
            
        }

        public ActionResult EdytujRekordySend(string id)
        {
            //tutaj jeszcze przekazuje w tempie raport informacje

            //tu w sumie wszystko wyko
            var rapQuery = from dbsdbs in db.Raport
                           where dbsdbs.raport_id == id
                           select dbsdbs;

            var nameQuery = from rap in rapQuery
                            join users in db.AspNetUsers
                            on rap.userID equals users.Id
                            where users.Id == rap.userID
                            select users.UserName;





            Raport raper = rapQuery.FirstOrDefault() as Raport;
            //konwersja 
            TempData["userName"] = nameQuery.FirstOrDefault();

            string kind;

            if (raper.isMinus == 0)
            {
                kind = "STAN PLACU";
            }
            else
            {
                kind = "WYSYLKA";
            }
            TempData["raportKind"] = kind;
            TempData["raportIdik"] = id;

            TempData["lastModification"] = raper.last_modyfication;

            if (raper.isMinus == 0)
            {
                var query = from dbs in db.SendRecordType
                            where dbs.raport_id == id
                            select dbs;



                //zwraca wszystkie rekordy zwiazane z danym raportem
                return View(query as IEnumerable<ThrashType>);
            }
            else
            {
                var query = from dbs in db.SendRecordType
                            where dbs.raport_id == id
                            select dbs;



                //zwraca wszystkie rekordy zwiazane z danym raportem
                return View(query as IEnumerable<SendRecordType>);
            }

        }

        [HttpGet]
        public ActionResult DodajRekord(string id)
        {

            TempData["rekRapId"] = id;

            //////////////////tutaj z bazy
            var classQuery = from cls in db.TrashClassTable
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
            //zwraca widok jednego rekordu
            return View();
        }

        [HttpPost]
        public ActionResult DodajRekordT (IEnumerable<TrashViewModel> json)
        {


            ThrashType ctnr = new ThrashType();

            ctnr.isSend = false;
            ctnr.isNegative = false;

            var holder = json.FirstOrDefault().someNumber.ToString().Replace(".", ",");

            ctnr.Quantity = Double.Parse(holder);
            ctnr.Information = json.FirstOrDefault().someText;
            ctnr.ClassName = json.FirstOrDefault().someOption;
            ctnr.raport_id = TempData["rekRapId"] as string;

            db.ThrashType.Add(ctnr);
            db.SaveChanges();

            db.Dispose();

            //zwraca widok jednego rekordu

            if (User.IsInRole("Administrator, Nadzorca"))
            {
                return Json(new { ok = true, newurl = Url.Action("EdytujRaport", "Raporty") }, JsonRequestBehavior.AllowGet);
            }
            if (User.IsInRole("KierownikActive"))
            {
                return Json(new { ok = true, newurl = Url.Action("EdytujRaportUser", "Raporty") }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        
        public ActionResult UsunRekord(int trashId)
        {
            

            return View(trashId);
        }

        public ActionResult UsunRekordPotwierdzenie(int id)
        {

            var del = from ccc in db.ThrashType
                      where ccc.IdTrash == id
                      select ccc;

            db.ThrashType.Remove(del.FirstOrDefault());
            db.SaveChanges();
            db.Dispose();


            if (User.IsInRole("Administrator, Nadzorca"))
            {
                return RedirectToAction("EdytujRaport", "Raporty");
            }
            if (User.IsInRole("KierownikActive"))
            {
                return RedirectToAction("EdytujRaportUser", "Raporty");

            }
            return View();
        }


        public ActionResult UsunSend(int IdSendRecord)
        {


            return View(IdSendRecord);
        }

        public ActionResult UsunRekordSendPotwierdzenie(int id)
        {

            var del = from ccc in db.SendRecordType
                      where ccc.IdSendRecord == id
                      select ccc;

            db.SendRecordType.Remove(del.FirstOrDefault());
            db.SaveChanges();
            db.Dispose();

            if (User.IsInRole("Administrator, Nadzorca"))
            {
                return RedirectToAction("EdytujRaport", "Raporty");
            }
            if (User.IsInRole("KierownikActive"))
            {
                return RedirectToAction("EdytujRaportUser", "Raporty");

            }
            return View();
        }



        public ActionResult DodajRekordSend(string id)
        {
            TempData["sendRapIdik"] = id;


            //////////////////tutaj z bazy
            var classQuery = from cls in db.TrashClassTable
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
            //zwraca widok jednego rekordu
            return View();
        }
        [HttpPost]
        public ActionResult DodajRekordSendT(IEnumerable<SendViewModel> json)
        {
            SendRecordType ctnr = new SendRecordType();

            ctnr.isSend = true;
            ctnr.raport_id = TempData["sendRapIdik"] as string;
            var holder = json.FirstOrDefault().weight.ToString().Replace(".", ",");

            ctnr.weight = Double.Parse(holder);

            ctnr.recipient = json.FirstOrDefault().recipient;
            ctnr.send_time = json.FirstOrDefault().send_time;
            ctnr.trash_class = json.FirstOrDefault().class_name;
            db.SendRecordType.Add(ctnr);

            db.SaveChanges();

            db.Dispose();

            //zwraca widok jednego rekordu
            if (User.IsInRole("Administrator, Nadzorca"))
            {
                return Json(new { ok = true, newurl = Url.Action("EdytujRaport", "Raporty") });
            }
            if (User.IsInRole("KierownikActive"))
            {
                return Json(new { ok = true, newurl = Url.Action("EdytujRaportUser", "Raporty") });
            }
            return View();
        }

        public ActionResult UsunRekordSend()
        {


            //zwraca widok potwierdzenia usuwania 
            return View();
        }



        //modyfikuje pojedynczy rekord w bazie 
        public ActionResult ZmienRekord (string rapId, int trashId)
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
            var classQuery = from cls in db.TrashClassTable
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


            //trashId null???

            int checker = trashId;

            var query = from dbs in db.ThrashType
                        where dbs.IdTrash == checker
                        select dbs;

            ThrashType ctnr22 = new ThrashType();

            TrashViewModel ctnr = new TrashViewModel();

            ctnr22 = query.FirstOrDefault() as ThrashType;

            ctnr.Id = ctnr22.IdTrash.ToString();
            ctnr.someNumber = ctnr22.Quantity.ToString();
            ctnr.someOption = ctnr22.ClassName.ToString();
            try {
                ctnr.someText = ctnr22.Information.ToString();
            }
            catch {
                ctnr.someText ="brak";
            }


            TempData["rapsy"] = rapId;
            TempData["idiksy"] = checker;


            return View(ctnr);
        }

        [HttpPost]
        public ActionResult ZmienRekord(IEnumerable<TrashViewModel> json)
        {
            int idik = Int32.Parse(json.FirstOrDefault().Id);

            var change = from chan in db.ThrashType
                         where chan.IdTrash == idik
                         select chan;

            var jsonOnly = json.FirstOrDefault();

            change.FirstOrDefault().ClassName = jsonOnly.someOption;

            double nr = Double.Parse(json.FirstOrDefault().someNumber);

            change.FirstOrDefault().Quantity = nr;

            try
            {
                change.FirstOrDefault().Information = jsonOnly.someText;
            }
            catch
            {
                change.FirstOrDefault().Information = "brak";
            }

            db.SaveChanges();
            db.Dispose();

            if(User.IsInRole("Administrator, Nadzorca"))
            {
                return Json(new { ok = true, newurl = Url.Action("EdytujRaport", "Raporty") });
            }
            if (User.IsInRole("KierownikActive"))
            {
                return Json(new { ok = true, newurl = Url.Action("EdytujRaportUser", "Raporty") });
            }
            return View();
        }

        public ActionResult ZmienRekordSend(string rapId, int trashId)
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
            var classQuery = from cls in db.TrashClassTable
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


            //trashId null???

            //dobry wynik zwraca tylko, ze potem nic z tym query nie robie
            var query = from dbs in db.SendRecordType
                        where dbs.IdSendRecord == trashId
                        select dbs;

            SendRecordType ctnr = new SendRecordType();
            ctnr = query.FirstOrDefault() as SendRecordType;

            SendEditViewModel ctnr22 = new SendEditViewModel();
            ctnr22.send_id = ctnr.IdSendRecord.ToString();
            ctnr22.recipient = ctnr.recipient;
            ctnr22.class_name = ctnr.trash_class;
            ctnr22.weight = ctnr.weight.ToString();

            TempData["raport_id"] = rapId;
            TempData["IdTrash"] = trashId;


            return View(ctnr22);
        }

        [HttpPost]
        public ActionResult ZmienRekordSend (IEnumerable<SendEditViewModel> json)            
        {
            int idik = Int32.Parse(json.FirstOrDefault().send_id);

            var change = from chan in db.SendRecordType
                         where chan.IdSendRecord == idik
                         select chan;

            var jsonOnly = json.FirstOrDefault();

            change.FirstOrDefault().trash_class = jsonOnly.class_name;

            var parsed = json.FirstOrDefault().weight.ToString().Replace('.',',');

            double nr = Double.Parse(parsed);

            change.FirstOrDefault().weight = nr;

            try
            {
                change.FirstOrDefault().recipient = jsonOnly.recipient;
            }
            catch
            {
                change.FirstOrDefault().recipient = "brak";
            }

            try
            {
                change.FirstOrDefault().send_time = jsonOnly.send_time;
            }
            catch
            {
                change.FirstOrDefault().send_time = "brak";
            }

            db.SaveChanges();
            db.Dispose();


            if (User.IsInRole("Administrator, Nadzorca"))
            {
                return Json(new { ok = true, newurl = Url.Action("EdytujRaport", "Raporty") });
            }
            if (User.IsInRole("KierownikActive"))
            {
                return Json(new { ok = true, newurl = Url.Action("EdytujRaportUser", "Raporty") });
            }
            return View();
        }

        [HttpPost]
        public ActionResult dataChange(IEnumerable<TrashTrash> json)
        {
            TrashTrash myObject = new TrashTrash();
            myObject = json.First();

            TempData["saveMeToDb"] = myObject;

            try
            {

                //in a real world, here will be multiple database calls - or others
                return Json(new { ok = true, newurl = Url.Action("saveToDbAgain", "Raporty") });
            }
            catch (Exception ex)
            {
                //TODO: log
                return Json(new { ok = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult dataChangeSend(IEnumerable<SendSend> json)
        {
            SendSend myObject = new SendSend();
            myObject = json.First();

            TempData["saveMeToDbSend"] = myObject;

            try
            {

                //in a real world, here will be multiple database calls - or others
                return Json(new { ok = true, newurl = Url.Action("saveToDbAgainSend", "Raporty") });
            }
            catch (Exception ex)
            {
                //TODO: log
                return Json(new { ok = false, message = ex.Message });
            }
        }


        public ActionResult saveToDbAgain()
        {

            //najpierw raport
            Raport oldraport = new Raport();
            List<TrashTrash> record = new List<TrashTrash>();
            TrashTrash recordCtr = new TrashTrash ();
            //to sie staje nullem, bo typy sobie nieodpowiadaja
            recordCtr= TempData["saveMeToDb"] as TrashTrash;


            var query = from us in db.Raport
                        where us.raport_id == recordCtr.raport_id
                        select us;

            //tu wyskoczyl blad z niestatyczna metoda potrzebuje obiektu docelowego
            oldraport = query.FirstOrDefault();

            oldraport.last_modyfication = DateTime.Now;
            

            //zapis do bazy
           
            db.SaveChanges();

            //potem wszystkie dane sprzezone z raportem
            ThrashType ctnr = new ThrashType();

            var cQuery = from dudu in db.ThrashType
                         where dudu.IdTrash == recordCtr.IdTrash
                         select dudu;
                //ctnr.IdTrash = (int)item.Id;

                ctnr = cQuery.FirstOrDefault();
                ctnr.ClassName = recordCtr.ClassName;
                ctnr.Quantity = recordCtr.Quantity;
                ctnr.Information = recordCtr.Information;

                //zapis do bazy 

            
            db.SaveChanges();
            db.Dispose();

            //chce zobacyzc odswiezony widok
            if (User.IsInRole("KierownikActive"))
            {
                return RedirectToAction("EdytujRaportUser", "Raporty");
            }
            else
            {
                return RedirectToAction("EdytujRaport", "Raporty");

            }
        }

        public ActionResult saveToDbAgainSend()
        {

            //najpierw raport
            Raport oldraport = new Raport();
            List<SendSend> record = new List<SendSend>();
            SendSend recordCtr = new SendSend();
            //to sie staje nullem, bo typy sobie nieodpowiadaja
            recordCtr = TempData["saveMeToDbSend"] as SendSend;


            var query = from us in db.Raport
                        where us.raport_id == recordCtr.raport_id
                        select us;

            //tu wyskoczyl blad z niestatyczna metoda potrzebuje obiektu docelowego
            oldraport = query.FirstOrDefault();

            oldraport.last_modyfication = DateTime.Now;


            //zapis do bazy

            db.SaveChanges();

            //potem wszystkie dane sprzezone z raportem
            SendRecordType ctnr = new SendRecordType();

            var cQuery = from dudu in db.SendRecordType
                         where dudu.IdSendRecord == recordCtr.IdSendRecord
                         select dudu;
            //ctnr.IdTrash = (int)item.Id;

            ctnr = cQuery.FirstOrDefault();
            ctnr.send_time = recordCtr.send_time;
            ctnr.recipient = recordCtr.recipient;
            ctnr.trash_class = recordCtr.trash_class;
            ctnr.weight = recordCtr.weight;

            //zapis do bazy 


            db.SaveChanges();
            db.Dispose();

            //chce zobacyzc odswiezony widok
            if(User.IsInRole("KierownikActive"))
            {
                return RedirectToAction("EdytujRaportUser", "Raporty");
            }
            else
            {
                return RedirectToAction("EdytujRaport", "Raporty");

            }
        }
        //a to w ogole chyba mial byc angus, ale nie zmienilem 
        public ActionResult StanPlacu ()
        {
            return View();
        }


        ///to nasz uprzedni 'angus' 
        ///
        //widok taki jak dla angusa
        public ActionResult DaneWysylki ()
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
            var classQuery = from cls in db.TrashClassTable
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
            db.Dispose();

            return View();
        }

        [HttpPost]
        public JsonResult DaneWysylki(IEnumerable<SendViewModel> json)
        {
            TempData["jsonNegativeList"] = json;

            //na razie id ich niech sobie beda od 1 do n
            //przekazuje do widoku z edytorem i tam przypisuje raport_id do widoku
            //temp ma jsona a controller zwroci raport
            //raport id to username+datetime



            return Json(new { ok = true, newurl = Url.Action("JsonNegative", "Raporty") });

            //try
            //{
            //    var line = Json(new { ok = true, newurl = Url.Action("JsonNegative", "Raporty") });

            //    return line;
            //    //in a real world, here will be multiple database calls - or others
            //}
            //catch (Exception ex)
            //{
            //    //TODO: log
            //    return Json(new { ok = false, message = ex.Message });
            //}
        }
        //
        //[HttpPost]
        //public ActionResult DaneWysylki(IEnumerable<SendViewModel> json)
        //{
        //    TempData["jsonNegativeList"] = json;

        //    //na razie id ich niech sobie beda od 1 do n
        //    //przekazuje do widoku z edytorem i tam przypisuje raport_id do widoku
        //    //temp ma jsona a controller zwroci raport
        //    //raport id to username+datetime



        //    try
        //    {
        //        var line = Json(new { ok = true, newurl = Url.Action("JsonNegative", "Raporty") });

        //        return line;
        //        //in a real world, here will be multiple database calls - or others
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO: log
        //        return Json(new { ok = false, message = ex.Message });
        //    }
        //}

        public ActionResult JsonNegative ()
        {
            var query = from u in db.AspNetUsers
                        join up in db.UserPlace on u.Id equals up.usersId
                        join tp in db.TrashPlaces on up.placesId equals tp.Id
                        where u.Email == User.Identity.Name
                        select tp.nameOfThePlace;//or more data if you need to.

            var field = query.FirstOrDefault();


            //to zapewnia odpowiednie info do raportu 
            ViewBag.Username = User.Identity.Name;
            try
            {
                ViewBag.Fieldname = field.ToString();
            }
            catch
            {
                ViewBag.Fieldname = "";
            }
            ViewBag.Datename = DateTime.Now.ToString();

            //tutaj bylo tylko view model, ale teraz chce zapisac do bazy
            List<SendViewModel> json = (List<SendViewModel>)TempData["jsonNegativeList"];


            return View(json);
        }

        [HttpPost]
        public ActionResult neededPassNeg(IEnumerable<SendViewModel> json)
        {

            TempData["toBeSavedNeg"] = json;

            try
            {

                //in a real world, here will be multiple database calls - or others
                return Json(new { ok = true, newurl = Url.Action("saveToDbNeg", "Raporty") });
            }
            catch (Exception ex)
            {
                //TODO: log
                return Json(new { ok = false, message = ex.Message });
            }
        }

        public ActionResult UsunRaport (string delId)
        {
            TempData["wtf"] = delId;
            return View();
        }

        
        public ActionResult UsunPotwierdzenie(string id)
        {
            var findRaport = from raps in db.Raport
                             where raps.raport_id == id
                             select raps;
            //najpierw usuwamy rekordy zwiazane z danym raportem
            try
            {
               if(findRaport.FirstOrDefault().isMinus==0)
                {//rekordy stanu placu
                    var findRecords = from recs in db.ThrashType
                                      where recs.raport_id == findRaport.FirstOrDefault().raport_id
                                      select recs;
                    
                    foreach (var item in findRecords)
                    {
                        db.ThrashType.Remove(item);
                        db.SaveChanges();
                    }
                   

                }
                else
                {//rekordy wysylek
                    var findRecords = from recs in db.SendRecordType
                                      where recs.raport_id == findRaport.FirstOrDefault().raport_id
                                      select recs;

                    foreach (var item in findRecords)
                    {
                        db.SendRecordType.Remove(item);
                        db.SaveChanges();

                    }
                }
            }
            catch
            {

            }
           

            //potem usuwamy raport
            try
            {
                db.Raport.Remove(findRaport.FirstOrDefault());
                db.SaveChanges();

            }
            catch { }

            return RedirectToAction("EdytujRaport");
        }


        public ActionResult saveToDbNeg()
        {



            //raport_id nie moze zawierac niczego poza cyframi i literami reszta BAD
            //powoduje 404 error

            //najpierw raport
            Raport newRaport = new Raport();


            //potem numer tygodnia
            DateTime time = DateTime.Today;

            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday){
                time = time.AddDays(3); }

            var weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            //rok
            var datePart = DateTime.Now.Year.ToString().Replace(".", "").Replace(":", "").Replace(" ", "");

            //login bez domeny
            var namePart = User.Identity.Name.ToString();

            //usuwanie @grupatom.pl - 12 znakow
            var nameShort = namePart.Remove((namePart.Length) - 12, 12);


            var queryr = from iter in db.IteratorTable select iter;

            //unikalny ID dla danego tygodnia, roku, osoby i rodzaju danych
            //newRaport.raport_id = weekNumber + "I" + datePart + "I" + nameShort + "I" + "wysylka";

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

            try {
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
           



            newRaport.raport_id = queryr.FirstOrDefault().C1 + "I" + datePart + "I" + nameShort + "I" + "wysylka" + "I" + placeId.FirstOrDefault().placesId;

            var query = from us in db.AspNetUsers
                        where us.Email == User.Identity.Name
                        select us.Id;

            newRaport.userID = query.FirstOrDefault();

            newRaport.creation_time = DateTime.UtcNow;

            newRaport.last_modyfication = DateTime.UtcNow;

            newRaport.isMinus = 1;

           
            newRaport.place_Id = placeId.FirstOrDefault().placesId;
            //newRaport.placId = "kit";

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
                      + "1. Edytować utworzony raport jeśli nie minął termin dostarczenia w danym okresie. \n"
                      + "2. Jeśli powinieneś mieć możliwość dodania raportu, a nie możesz tego"
                       + "zrobić skontaktuj się z działem IT - sprawdź ustawienia placu \n";
                db.Dispose();

                return RedirectToAction("Error", "Home");
            }


            //potem wszystkie dane sprzezone z raportem

            //kontener na rekord z rap
            SendRecordType ctnr = new SendRecordType();

            //to ma przechowywac kolejne kontenery
            List<SendRecordType> ctnrList = new List<SendRecordType>();

            //zbiera z viewmodelu rekordy
            List<SendViewModel> listTemp = TempData["toBeSavedNeg"] as List<SendViewModel>;

            foreach (var item in listTemp)
            {
                //ctnr.IdTrash = (int)item.Id;
                ctnr.raport_id = newRaport.raport_id;
                ctnr.send_time = item.send_time;
                ctnr.recipient = item.recipient;
                ctnr.trash_class = item.class_name;
                ctnr.weight = Double.Parse(item.weight);
                ctnr.isSend = true;

                //zapis do bazy 
                db.SendRecordType.Add(ctnr);
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
                    + "1. Edytować utworzony raport jeśli nie minął termin dostarczenia w danym okresie. \n"
                    + "2. Jeśli powinieneś mieć możliwość dodania raportu, a nie możesz tego"
                     + "zrobić skontaktuj się z działem IT. \n";
                db.Dispose();

                return RedirectToAction("Error", "Home");
            }


            //po zakonczeniu edycji odłącz się od bazy
            db.Dispose();
            return RedirectToAction("Index", "Home");
        }
    }
}