using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Localization;
using Microsoft.Framework.Localization;

namespace InternationalizedStarterWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHtmlLocalizer _localizer;

        public HomeController(IHtmlLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Message = _localizer["Your application description page."];

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = _localizer["Your contact page."];

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}