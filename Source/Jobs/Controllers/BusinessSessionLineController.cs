using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using Presentation.ViewModels;
using Presentation;
using Core.Common;
using Model.ViewModel;
using AutoMapper;
using System.Xml.Linq;
using Model.ViewModels;
using Jobs.Helpers;
using System.Data.SqlClient;

namespace Jobs.Controllers
{
    [Authorize]
    public class BusinessSessionLineController : System.Web.Mvc.Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        public BusinessSessionLineController(IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _unitOfWork = unitOfWork;
            _exception = exec;

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }
        // GET: /BusinessSessionLineMaster/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult FillRecords()
        {
            string mQry = @"SELECT IsNull(Bl.BusinessSessionLineId,0) As BusinessSessionLineId, Bs.BusinessSessionId, Bs.BusinessSessionName, S.SiteId, S.SiteName, D.DivisionId, D.DivisionName, 
                        Bl.OpeningStockValue AS OpeningStockValue, Bl.ClosingStockValue AS ClosingStockValue           
                        FROM Web.BusinessSessions Bs                   
                        LEFT JOIN Web.Sites S ON 1 = 1                  
                        LEFT JOIN Web.Divisions D ON 1 = 1 
                        LEFT JOIN Web.BusinessSessionLines Bl ON Bs.BusinessSessionId = Bl.BusinessSessionId
		                AND S.SiteId = Bl.SiteId
		                AND D.DivisionId = Bl.DivisionId
                        Order By Bs.BusinessSessionId Desc ";
            IEnumerable<BusinessSessionLineViewModel> BusinessSessionLineList = db.Database.SqlQuery<BusinessSessionLineViewModel>(mQry).ToList();

            if (BusinessSessionLineList != null)
            {
                JsonResult json = Json(new { Success = true, Data = BusinessSessionLineList }, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public void Post(string ColumnName, int BusinessSessionLineId,
            int BusinessSessionId, int SiteId, int DivisionId,
            decimal? newValue)
        {
            if (BusinessSessionLineId == 0)
            {
                var BusinessSessionLine = (from L in db.BusinessSessionLine
                                           where L.BusinessSessionId == BusinessSessionId &&
                                           L.SiteId == SiteId && L.DivisionId == DivisionId
                                           select L).FirstOrDefault();

                if (BusinessSessionLine != null)
                    BusinessSessionLineId = BusinessSessionLine.BusinessSessionLineId;
            }


            if (BusinessSessionLineId != 0)
            {
                BusinessSessionLine Line = db.BusinessSessionLine.Find(BusinessSessionLineId);
                if (ColumnName == "OpeningStockValue")
                    Line.OpeningStockValue = newValue;
                if (ColumnName == "ClosingStockValue")
                    Line.ClosingStockValue = newValue;
                Line.ModifiedDate = DateTime.Now;
                Line.ModifiedBy = User.Identity.Name;
                Line.ObjectState = Model.ObjectState.Modified;
                db.BusinessSessionLine.Add(Line);
            }
            else
            {
                BusinessSessionLine Line = new BusinessSessionLine();
                Line.BusinessSessionId = BusinessSessionId;
                Line.SiteId = SiteId;
                Line.DivisionId = DivisionId;
                if (ColumnName == "OpeningStockValue")
                    Line.OpeningStockValue = newValue;
                if (ColumnName == "ClosingStockValue")
                    Line.ClosingStockValue = newValue;
                Line.CreatedDate = DateTime.Now;
                Line.ModifiedDate = DateTime.Now;
                Line.CreatedBy = User.Identity.Name;
                Line.ModifiedBy = User.Identity.Name;
                Line.ObjectState = Model.ObjectState.Added;
                db.BusinessSessionLine.Add(Line);
            }

            try
            {
                db.SaveChanges();
            }

            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
            }
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

    public class BusinessSessionLineViewModel
    {
        public int BusinessSessionLineId { get; set; }
        public int BusinessSessionId { get; set; }
        public string BusinessSessionName { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
        public decimal? OpeningStockValue { get; set; }
        public decimal? ClosingStockValue { get; set; }
    }
}


