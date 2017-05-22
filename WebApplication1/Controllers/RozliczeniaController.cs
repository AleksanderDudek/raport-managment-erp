using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.DatabaseFiles;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    [Authorize(Roles ="Administrator, Nadzorca")]
    public class RozliczeniaController : Controller
    {
        MyModel db = new MyModel();
        // GET: Rozliczenia

        public ActionResult Blokowanie ()
        {
            //po prostu zwraca widok z przyciskami
           var timer = db.IteratorTable.FirstOrDefault();
            db.Dispose();

            return View(timer.C1);
        }


        public ActionResult PodsumowanieZPlacu ()
        {
            ////wszystkie place
            //var classQuery = from cls in db.UserPlace
            //                 join ccs in db.TrashPlaces
            //                 on cls.placesId equals ccs.Id
            //                 select ccs;

            //List<string> parsed_db = new List<string>();

            //foreach (var item in classQuery)
            //{
            //    parsed_db.Add(item.nameOfThePlace);
            //}
            //parsed_db.Sort();
            //TempData["listForPlaces"] = parsed_db;

            ////wszystkie place posortowane i dodane do tempdata





            return View();
        }

        public ActionResult Podsumowanie ()
        {
            //id okresu rozliczeniowego
            var biezacyOkres = from iter in db.IteratorTable
                               select iter;
            //wybieramy nasz schemat poszukiwan
            var strMatch = (biezacyOkres.FirstOrDefault().C1).ToString();

            //wybieramy wszystkie raporty wysylkowe +++ nalezace do DANEGO PLACU, 
            //czyli trzeba znalezc userId i sprzezony z nim plac
            var sendRaports = from raps in db.SendRecordType
                              select raps;

            //sposrd raport_id wszystkich naszych rekordow wybieramy te, ktorych id zaczyna sie od strMatch i jego dlugosci
            //

            List<SendSend> listOfSendrecords = new List<SendSend>();
            //List<string> recip = new List<string>();
            //List<string> clas = new List<string>();

            foreach (SendRecordType item in sendRaports)
            {

                SendSend ctnr = new SendSend();
                var length = strMatch.Length;
                
                //to sprawdza czy biezacy raport_id rekordu ma prefix tej samej dlugosci
                //wiedzac, ze na okreslonej pozycji dla zadanej dlugosci musi znajdowac sie duze I
                if (item.raport_id.ElementAt(length) == 'I')
                {
                    //prefix tej samej dlugosci
                    if(item.raport_id.StartsWith(strMatch))
                    {
                        ctnr.send_time = item.send_time;
                        ctnr.recipient = item.recipient;
                        ctnr.trash_class = item.trash_class;
                        ctnr.weight = item.weight;
                        ctnr.isSend = item.isSend;
                        ctnr.raport_id = item.raport_id;

                        //recip.Add(item.recipient);
                        //clas.Add(item.trash_class);
                        //nasz rekord
                        listOfSendrecords.Add(ctnr);
                    }

                }
                else
                {//nie zgadzaja sie 
                    continue;
                }
            }


            //unikalni odbiorcy dla danego ok. rozliczeniowego
            List<string> uniqRec = (from recip in listOfSendrecords
                                    select recip.recipient)
                                    .Distinct()
                                    .ToList();

            
            //unikalne klasy dla danego okresu rozliczeniowego
            List<string> uniqClass = (from recip in listOfSendrecords
                                      select recip.trash_class)
                                    .Distinct()
                                    .ToList();
                                 

            //ile unikalnych wystapien, wymiary macierzy tabeli
            int m = uniqRec.Count, n = uniqClass.Count;

            //tablica wartosci

            Array arr = Array.CreateInstance(typeof(Double), m, n);

           
            //wiersze odbiorcow
            for (int i=0; i<m;i++)
            {
                //kolumny klas
                for(int j=0; j<n; j++)
                {
                    var query = from records in listOfSendrecords
                                where records.recipient == uniqRec.ElementAt(i)
                                 && records.trash_class == uniqClass.ElementAt(j)
                                select records.weight;
                    try
                    {
                        arr.SetValue(query.Sum(), i, j);
                    }
                    catch
                    {//w razie gdyby brak rekordow dal mi null exception po Sum()
                        arr.SetValue(0, i, j);

                    }
                }
            }

            //wektor sum dla kazdej klasy
            Array wektorSum = Array.CreateInstance(typeof(Double), n); 

            for(int q=0; q<n; q++)
            {
                double holder = new double();

                for (int p=0; p<m; p++)
                {
                    holder += (double)arr.GetValue(p, q);
                }
                wektorSum.SetValue(holder, q);
                holder = 0;
            }
            //suma sum wektora sum

            double sumaSum = new double();

            for(int y=0; y<n; y++)
            {
                sumaSum += (double)wektorSum.GetValue(y);
            }

            TempData["sumaSumWektoraSum"] = sumaSum;

            Array tabela = Array.CreateInstance(typeof(string), m + 2, n + 1);

            //pierwsza kolumna 
            for(int i=0; i<m+2; i++)
            {
                if(i==0)
                {
                    tabela.SetValue("ODBIORCA\\KLASA", i, 0);
                    continue;
                }
                if(i==m+1)
                {
                    tabela.SetValue("SUMA", i, 0);
                    continue;
                }
                tabela.SetValue(uniqRec.ElementAt(i - 1), i, 0);
            }

            //pierwszy wiersz
            for(int j=1; j<n+1; j++)
            {
                tabela.SetValue(uniqClass.ElementAt(j - 1), 0, j);
            }

            //ostatni wiersz 
            for (int j = 1; j < n + 1; j++)
            {
                tabela.SetValue(wektorSum.GetValue(j - 1).ToString(), m+1, j);
            }

            //przepisanie wartosci 
            for (int i=1; i<m+1; i++)
            {
                for(int j=1; j<n+1; j++)
                {
                    tabela.SetValue(" "+arr.GetValue(i - 1, j - 1).ToString()+" " , i, j);
                }
            }

            ViewData["podsumowanie"] = tabela;
            TempData["wysokosc"] = m + 2;
            TempData["szerokosc"] = n + 1;

            //posiadamy wszystkie rekordy z tego okresu rozliczeniowego
            //teraz chcemy je przetworzyc na tabele 
            TempData["czas"]= DateTime.Today.ToString();

            //wszystkie place
            var classQuery = from cls in db.TrashPlaces
                             select cls;

            List<string> parsed_db = new List<string>();

            foreach (var item in classQuery)
            {
                parsed_db.Add(item.Id.ToString());
            }
            parsed_db.Sort();
            TempData["listForPlaces"] = parsed_db;

            //wszystkie place posortowane i dodane do tempdata



            db.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult _Podsumowanie (string placeName)
        {

            TempData["placeName"] = placeName;

            if (placeName == null)
            {
                ViewBag.Message = "Pusty json";
                return RedirectToAction("Error","Home");
            }

            //id okresu rozliczeniowego
            var biezacyOkres = from iter in db.IteratorTable
                               select iter;
            //wybieramy nasz schemat poszukiwan
            var strMatch = (biezacyOkres.FirstOrDefault().C1).ToString();

            //stad bedzie userId
            var pattern = from sss in db.UserPlace
                          where sss.placesId == placeName
                          select sss;

            //wybieramy wszystkie raporty wysylkowe +++ nalezace do DANEGO PLACU
            var chosenRaps = from rrr in db.Raport
                             where rrr.place_Id == placeName
                             && rrr.isMinus == 1
                             select rrr;
            //teoretycznie powinien byc 1, wiec moge zaryzykowac

        
            var sendRaports = from raps in db.SendRecordType
                              where raps.raport_id == chosenRaps.FirstOrDefault().raport_id
                              select raps;

            //sposrd raport_id wszystkich naszych rekordow wybieramy te, ktorych id zaczyna sie od strMatch i jego dlugosci
            //

            List<SendSend> listOfSendrecords = new List<SendSend>();
            //List<string> recip = new List<string>();
            //List<string> clas = new List<string>();

            foreach (SendRecordType item in sendRaports)
            {

                SendSend ctnr = new SendSend();
                var length = strMatch.Length;

                //to sprawdza czy biezacy raport_id rekordu ma prefix tej samej dlugosci
                //wiedzac, ze na okreslonej pozycji dla zadanej dlugosci musi znajdowac sie duze I
                if (item.raport_id.ElementAt(length) == 'I')
                {
                    //prefix tej samej dlugosci
                    if (item.raport_id.StartsWith(strMatch))
                    {
                        ctnr.send_time = item.send_time;
                        ctnr.recipient = item.recipient;
                        ctnr.trash_class = item.trash_class;
                        ctnr.weight = item.weight;
                        ctnr.isSend = item.isSend;
                        ctnr.raport_id = item.raport_id;

                        //recip.Add(item.recipient);
                        //clas.Add(item.trash_class);
                        //nasz rekord
                        listOfSendrecords.Add(ctnr);
                    }

                }
                else
                {//nie zgadzaja sie 
                    continue;
                }
            }


            //unikalni odbiorcy dla danego ok. rozliczeniowego
            List<string> uniqRec = (from recip in listOfSendrecords
                                    select recip.recipient)
                                    .Distinct()
                                    .ToList();


            //unikalne klasy dla danego okresu rozliczeniowego
            List<string> uniqClass = (from recip in listOfSendrecords
                                      select recip.trash_class)
                                    .Distinct()
                                    .ToList();


            //ile unikalnych wystapien, wymiary macierzy tabeli
            int m = uniqRec.Count, n = uniqClass.Count;

            //tablica wartosci

            Array arr = Array.CreateInstance(typeof(Double), m, n);


            //wiersze odbiorcow
            for (int i = 0; i < m; i++)
            {
                //kolumny klas
                for (int j = 0; j < n; j++)
                {
                    var query = from records in listOfSendrecords
                                where records.recipient == uniqRec.ElementAt(i)
                                 && records.trash_class == uniqClass.ElementAt(j)
                                select records.weight;
                    try
                    {
                        arr.SetValue(query.Sum(), i, j);
                    }
                    catch
                    {//w razie gdyby brak rekordow dal mi null exception po Sum()
                        arr.SetValue(0, i, j);

                    }
                }
            }

            //wektor sum dla kazdej klasy
            Array wektorSum = Array.CreateInstance(typeof(Double), n);

            for (int q = 0; q < n; q++)
            {
                double holder = new double();

                for (int p = 0; p < m; p++)
                {
                    holder += (double)arr.GetValue(p, q);
                }
                wektorSum.SetValue(holder, q);
                holder = 0;
            }
            //suma sum wektora sum

            double sumaSum = new double();

            for (int y = 0; y < n; y++)
            {
                sumaSum += (double)wektorSum.GetValue(y);
            }

            TempData["sumsumsum"] = sumaSum;

            Array tabela = Array.CreateInstance(typeof(string), m + 2, n + 1);

            //pierwsza kolumna 
            for (int i = 0; i < m + 2; i++)
            {
                if (i == 0)
                {
                    tabela.SetValue("ODBIORCA\\KLASA", i, 0);
                    continue;
                }
                if (i == m + 1)
                {
                    tabela.SetValue("SUMA", i, 0);
                    continue;
                }
                tabela.SetValue(uniqRec.ElementAt(i - 1), i, 0);
            }

            //pierwszy wiersz
            for (int j = 1; j < n + 1; j++)
            {
                tabela.SetValue(uniqClass.ElementAt(j - 1), 0, j);
            }

            //ostatni wiersz 
            for (int j = 1; j < n + 1; j++)
            {
                tabela.SetValue(wektorSum.GetValue(j - 1).ToString(), m + 1, j);
            }

            //przepisanie wartosci 
            for (int i = 1; i < m + 1; i++)
            {
                for (int j = 1; j < n + 1; j++)
                {
                    tabela.SetValue(" " + arr.GetValue(i - 1, j - 1).ToString() + " ", i, j);
                }
            }

            ViewData["podsum"] = tabela;
            TempData["wys"] = m + 2;
            TempData["szer"] = n + 1;

            //posiadamy wszystkie rekordy z tego okresu rozliczeniowego
            //teraz chcemy je przetworzyc na tabele 
         



            
            return PartialView("_Podsumowanie");
        }

        [HttpPost]
        public ActionResult ZablokujRaportowanie()
        {

            //wybiera wszystkich aktywnych kierownikow (tzn. takich
            //co moga tworzyc raporty) 

            //var Kierownicy = from kierownicy in db.AspNetUserRoles
            //                 where kierownicy.RoleId == "3"
            //                 select kierownicy;

            ////to nie zadzialalo, bo zmiana klucza w tabeli UserRoles jest niedozwolona
            ////zmienia ich role na KierownikNotActive = nie moze wysylac, tworzyc czy edytowac raportow
            //foreach (AspNetUserRoles item in Kierownicy)
            //{
            //    item.RoleId = "4";
            //}

            //w druga strone, zmienimy w tabeli Roles wartosc pola dla danego ID 

            var zmianaRoli = from role in db.AspNetRoles
                             where role.Id == "3"
                             select role;

            zmianaRoli.FirstOrDefault().Name = "KierownikNotActive";

            db.SaveChanges();
            db.Dispose();



            return Json("success");
        }

        [HttpPost]
        public ActionResult OdblokujRaportowanie()
        {

            //wybiera wszystkich aktywnych kierownikow (tzn. takich
            //co nie moga tworzyc raportow)

            //var Kierownicy = from kierownicy in db.AspNetUserRoles
            //                 where kierownicy.RoleId == "3"
            //                 select kierownicy;

            ////to nie zadzialalo, bo zmiana klucza w tabeli UserRoles jest niedozwolona
            ////zmienia ich role na KierownikActive =  moze wysylac, tworzyc czy edytowac raporty
            //foreach (AspNetUserRoles item in Kierownicy)
            //{
            //    item.RoleId = "3";
            //}

            var zmianaRoli = from role in db.AspNetRoles
                             where role.Id == "3"
                             select role;

            zmianaRoli.FirstOrDefault().Name = "KierownikActive";

            db.SaveChanges();
            db.Dispose();

            return Json("success");
        }


        public ActionResult ZakonczOkres()
        {

            var query = db.IteratorTable.FirstOrDefault();
            query.C1 += 1;

            db.SaveChanges();

            var current = db.IteratorTable.FirstOrDefault().C1;

            db.Dispose();


            return Json(current);
           
        }

        
        public ActionResult PowrotDoPrzeszlosci()
        {

            var query = db.IteratorTable.FirstOrDefault();
            query.C1 -= 1;

            db.SaveChanges();

            var current = db.IteratorTable.FirstOrDefault().C1;

            ViewBag.Message = "PAMIĘTAJ ABY WRÓCIĆ DO OBOWIĄZUJĄCEGO OKRESU ROZLICZENIOWEGO! Zrobisz to klikając Zakończ okres, aż powrócisz to przyszłości.";

            return Json(current);
        }
        public ActionResult TabelaRozliczeniowa ()
        {
            //zakladam, ze chcemy dane z 

            //unikalne klasy
            var uniqWeekClass = from classes in db.SendRecordType
                                 select classes;

            //unikalni odbiorcy
            var uniqWeekRecipient = from classes in db.SendRecordType
                                    select classes;


            // nr i rok 

            //potem numer tygodnia
            DateTime time = DateTime.Today, time2 = new DateTime(2016,9,30), time3 = new DateTime(2016, 10, 1), 
                time4 = new DateTime(2016, 10, 3), time5 = new DateTime(2016,10, 5), time6 = new DateTime(2016, 10, 7), timer;

            List<DateTime> listaCzasu = new List<DateTime>();
            List<int> listaIntow = new List<int>();

            listaCzasu.Add(time);
            listaCzasu.Add(time2);
            listaCzasu.Add(time3);
            listaCzasu.Add(time4);
            listaCzasu.Add(time5);
            listaCzasu.Add(time6);


            for (int i=0; i< listaCzasu.Count; i++)
            {
                DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(timer=listaCzasu.ElementAt(i));
                if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                {
                   timer = listaCzasu.ElementAt(i).AddDays(3);
                }
                var weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(timer, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                listaIntow.Add(weekNumber);
            }
           

            //rok
            var datePart = DateTime.Now.Year.ToString().Replace(".", "").Replace(":", "").Replace(" ", "");



            return View(listaIntow);
        }

    }
}