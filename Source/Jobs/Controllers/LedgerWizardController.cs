using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Core.Common;
using Model.Models;
using Data.Models;
using Service;
using Data.Infrastructure;
using System.Configuration;
using Model.ViewModel;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using Model.DatabaseViews;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Jobs.Helpers;
using System.Text;
using Model.ViewModels;
using System.Web.Script.Serialization;

namespace Jobs.Controllers
{
    [Authorize]
    public class LedgerWizardController : System.Web.Mvc.Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        protected string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
        ILedgerWizardService _LedgerWizardService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;
        List<string> UserRoles = new List<string>();


        public LedgerWizardController(ILedgerWizardService LedgerWizardService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _LedgerWizardService = LedgerWizardService;
            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            _exception = exec;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult LedgerWizard(int id, int? LedgerHeaderId)
        {
            LedgerWizardViewModel vm = new LedgerWizardViewModel();
            vm.DocDate = DateTime.Now;
            vm.DocTypeId = id;

            List<SelectListItem> WagesPayTypeList = new List<SelectListItem>();
            WagesPayTypeList.Add(new SelectListItem { Text = "Daily", Value = "Daily" });
            WagesPayTypeList.Add(new SelectListItem { Text = "Monthly", Value = "Monthly" });
            ViewBag.WagesPayTypeList = WagesPayTypeList;

            vm.LedgerHeaderId = LedgerHeaderId ?? 0;
            if (vm.LedgerHeaderId != 0)
            {
                LedgerHeader Header = db.LedgerHeader.Find(vm.LedgerHeaderId);
                vm.DocDate = Header.DocDate;
                vm.Remark = Header.Remark;
            }




            return View(vm);
        }

        public JsonResult LedgerWizardFill(LedgerWizardViewModel vm)
        {
            IEnumerable<LedgerWizardResultViewModel> LedgerWizardResultViewModel = _LedgerWizardService.GetLedgerDetail(vm);

            JsonResult json = Json(new { Success = true, Data = LedgerWizardResultViewModel }, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public string Post(List<LedgerWizardResultViewModel> LedgerDataList)
        {
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            if (LedgerDataList.Count > 0)
            {
                if (LedgerDataList.FirstOrDefault().LedgerHeaderId != null && LedgerDataList.FirstOrDefault().LedgerHeaderId > 0)
                {
                    var HeaderRecord = db.LedgerHeader.Find(LedgerDataList.FirstOrDefault().LedgerHeaderId);

                    var LineRecords = (from L in db.LedgerLine where L.LedgerHeaderId == HeaderRecord.LedgerHeaderId select L).ToList();

                    foreach (var item in LineRecords)
                    {
                        item.ObjectState = Model.ObjectState.Deleted;
                        db.LedgerLine.Remove(item);
                    }

                    HeaderRecord.ObjectState = Model.ObjectState.Deleted;
                    db.LedgerHeader.Remove(HeaderRecord);
                }

                DateTime DocDate  = Convert.ToDateTime(LedgerDataList.FirstOrDefault().DocDate);
                int DocTypeId = LedgerDataList.FirstOrDefault().DocTypeId;
                string DocNo = new DocumentTypeService(_unitOfWork).FGetNewDocNo("DocNo", ConfigurationManager.AppSettings["DataBaseSchema"] + ".LedgerHeaders", DocTypeId, DocDate, DivisionId, SiteId);
                int HeaderLedgerAccountId = LedgerDataList.FirstOrDefault().HeaderLedgerAccountId;
                string Remark = LedgerDataList.FirstOrDefault().HeaderRemark;

                DocumentType DocType = new DocumentTypeService(_unitOfWork).Find(DocTypeId);

                LedgerHeader Header = new LedgerHeader();
                Header.DocDate = DocDate;
                Header.DocTypeId = DocTypeId;
                Header.DocNo = DocNo;
                Header.DivisionId = DivisionId;
                Header.SiteId = SiteId;
                Header.LedgerAccountId = HeaderLedgerAccountId;
                Header.DrCr = DocType.Nature;
                Header.Remark = Remark;
                Header.CreatedDate = DateTime.Now;
                Header.CreatedBy = User.Identity.Name;
                Header.ModifiedDate = DateTime.Now;
                Header.ModifiedBy = User.Identity.Name;
                Header.ObjectState = Model.ObjectState.Added;
                db.LedgerHeader.Add(Header);

                int i = 0;
                foreach(var LedgerData in LedgerDataList)
                {
                    if (LedgerData.Amount > 0)
                    {
                        LedgerLine Line = new LedgerLine();
                        Line.LedgerLineId = i--;
                        Line.LedgerHeaderId = Header.LedgerHeaderId;
                        Line.LedgerAccountId = LedgerData.LedgerAccountId;
                        //Line.DrCr = (Header.DrCr == NatureConstants.Debit ? NatureConstants.Credit : NatureConstants.Debit);
                        Line.Amount = LedgerData.Amount;
                        Line.CreatedDate = DateTime.Now;
                        Line.CreatedBy = User.Identity.Name;
                        Line.ModifiedDate = DateTime.Now;
                        Line.ModifiedBy = User.Identity.Name;
                        Line.ObjectState = Model.ObjectState.Added;

                        db.LedgerLine.Add(Line);

                        #region LedgerSave
                        Ledger Ledger = new Ledger();

                        if (Header.DrCr == NatureConstants.Credit)
                            Ledger.AmtDr = Line.Amount;
                        else if (Header.DrCr == NatureConstants.Debit)
                            Ledger.AmtCr = Line.Amount;
                        Ledger.ChqNo = Line.ChqNo;
                        Ledger.ChqDate = Line.ChqDate;
                        Ledger.ContraLedgerAccountId = Header.LedgerAccountId;
                        Ledger.CostCenterId = Line.CostCenterId;
                        Ledger.DueDate = Line.ChqDate;
                        Ledger.LedgerAccountId = Line.LedgerAccountId;
                        Ledger.LedgerHeaderId = Line.LedgerHeaderId;
                        Ledger.LedgerLineId = Line.LedgerLineId;
                        Ledger.ProductUidId = Line.ProductUidId;
                        Ledger.Narration = Header.Narration + Line.Remark;
                        Ledger.ObjectState = Model.ObjectState.Added;
                        Ledger.LedgerId = 1;
                        db.Ledger.Add(Ledger);

                        #endregion

                        #region ContraLedgerSave
                        Ledger ContraLedger = new Ledger();
                        if (Header.DrCr == NatureConstants.Credit)
                            ContraLedger.AmtCr = Line.Amount;
                        else if (Header.DrCr == NatureConstants.Debit)
                            ContraLedger.AmtDr = Line.Amount;
                        ContraLedger.LedgerHeaderId = Header.LedgerHeaderId;
                        ContraLedger.CostCenterId = Header.CostCenterId;
                        ContraLedger.LedgerLineId = Line.LedgerLineId;
                        ContraLedger.LedgerAccountId = Header.LedgerAccountId.Value;
                        ContraLedger.ContraLedgerAccountId = Line.LedgerAccountId;
                        ContraLedger.ChqNo = Line.ChqNo;
                        ContraLedger.ChqDate = Line.ChqDate;
                        ContraLedger.ObjectState = Model.ObjectState.Added;
                        db.Ledger.Add(ContraLedger);
                        #endregion
                    }
                }


                try
                {
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXC"] += message;
                }


                //return Json(new { success = true, Url = "/LedgerHeader/Submit/" + LedgerHeader.LedgerHeaderId });
                //return Json(new { success = true, Url = "/LedgerHeader/Submit/" + Header.LedgerHeaderId });
                return (string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/LedgerHeader/Submit/" + Header.LedgerHeaderId;
            }
            return null;
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
