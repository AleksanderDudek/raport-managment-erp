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




            var users = tbls.AspNetUsers;
            var roles = tbls.AspNetUserRoles;

            var result = from user in users.AsEnumerable()
                         join role in roles.AsEnumerable() 
                         on user.Id equals role.UserId 
                         into g
                         from role in g.DefaultIfEmpty()
                         select new MyViewModel
                         {
                             Id = user.Id,
                             Name = user.UserName,
                             RoleId = role.RoleId
                         };

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
            personRole = tbls.AspNetUserRoles.Single(m => m.UserId == userId);                //ma problem z odnalezieniem, ale 


            if (personRole == null)
            {
                return HttpNotFound();
            }
            return View(personRole);
        }


        [HttpGet]
        public ActionResult angus()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult angus(IEnumerable<TrashViewModel> json)
        {
            //tu mamy stringa, ktorego trzeba parsowac

            //List<TrashViewModel> jlist = new List<TrashViewModel> ();
            //TrashViewModel ctnr = new TrashViewModel();


            //    ctnr.Id = ;
            //    ctnr.someOption = 3;
            //    ctnr.someNumber = 3;
            //    ctnr.someText = 3;

            //    jlist.Add(ctnr);

            TempData["jsonList"] = json;
            //var check = json.ToString() == null;

            //Regex.Replace(json, @"", "");

            //string signer =  json.Replace(@"\", "");



            try
            {

                //in a real world, here will be multiple database calls - or others
                return Json(new { ok = true, newurl = Url.Action( "JsonResult","Session")});
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
    }
}