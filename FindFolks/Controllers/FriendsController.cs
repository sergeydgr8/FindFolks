using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FindFolks.EF;
using FindFolks.Models;

namespace FindFolks.Controllers
{
    public class FriendsController : Controller
    {
        private FFContext ffContext = new FFContext();

        // GET: Friends
        public ActionResult Index()
        {
            return View();
        }

        
    }
}
