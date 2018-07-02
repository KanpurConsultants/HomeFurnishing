using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation.ViewModels;
using Presentation;
using Model.ViewModels;

namespace Jobs.Controllers
{
    [Authorize]
    public class TaxReturnController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        IUnitOfWork _unitOfWork;
        public TaxReturnController(IUnitOfWork work)
        {
            _unitOfWork = work;
        }



        [HttpGet]
        public ActionResult TaxReturn()
        {
            List<SelectListItem> TempItemList = new List<SelectListItem>();
            TempItemList.Add(new SelectListItem { Text = "GST 3B", Value = "GST 3B" });
            TempItemList.Add(new SelectListItem { Text = "GSTR1", Value = "GSTR1" });

            ViewBag.FormatList = TempItemList;
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
