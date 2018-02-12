using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Components.ExceptionHandlers;
using Service;
using Services.PropertyTax;
using Model.ViewModels;
using Model.Models;
using Model.ViewModel;
using Core.Common;
using Reports.Controllers;

namespace Jobs.Areas.PropertyTax.Controllers
{

    [Authorize]
    public class PropertyLineController : System.Web.Mvc.Controller
    {
        List<string> UserRoles = new List<string>();

        IPropertyLineService _PropertyLineService;
        IPropertyHeaderService _PropertyHeaderService;
        IJobOrderSettingsService _jobOrderSettingsService;
        IDiscountTypeService _DiscountTypeService;
        IExceptionHandler _exception;

        bool TimePlanValidation = true;
        string ExceptionMsg = "";
        bool Continue = true;

        public PropertyLineController(IPropertyLineService PropertyLineService, IExceptionHandler exec, IPropertyHeaderService PropertyHeaderService, IJobOrderSettingsService jobOrderSettingsServ, IDiscountTypeService DiscountTypeService)
        {
            _PropertyLineService = PropertyLineService;
            _PropertyHeaderService = PropertyHeaderService;
            _jobOrderSettingsService = jobOrderSettingsServ;
            _DiscountTypeService = DiscountTypeService;
            _exception = exec;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
        }



        [HttpGet]
        public JsonResult Index(int id)
        {
            var p = _PropertyLineService.GetProductBuyerListForIndex(id).ToList();
            return Json(p, JsonRequestBehavior.AllowGet);

        }


        private void PrepareViewBag(PropertyLineViewModel vm)
        {
            if (vm != null)
            {
                PropertyHeaderViewModel H = _PropertyHeaderService.GetPropertyHeader(vm.PersonId);
                ViewBag.DocNo = H.Code;
            }
        }

        [HttpGet]
        public ActionResult CreateLine(int id, bool? IsProdBased)
        {
            return _Create(id, null, IsProdBased);
        }

        [HttpGet]
        public ActionResult CreateLineAfter_Submit(int id, bool? IsProdBased)
        {
            return _Create(id, null, IsProdBased);
        }

        [HttpGet]
        public ActionResult CreateLineAfter_Approve(int id, bool? IsProdBased)
        {
            return _Create(id, null, IsProdBased);
        }

        public ActionResult _Create(int Id, DateTime? date, bool? IsProdBased) //Id ==>Sale Order Header Id
        {
            PropertyHeaderViewModel H = _PropertyHeaderService.GetPropertyHeader(Id);
            PropertyLineViewModel s = new PropertyLineViewModel();
            s.DateOfConsutruction = DateTime.Now.Date;
            s.WEF = DateTime.Now.Date;
            s.PersonId = H.PersonID;
            ViewBag.Status = H.Status;

            DiscountType D = _DiscountTypeService.Find("NA");
            s.DiscountTypeId = D.DiscountTypeId;
            s.DiscountRate = D.Rate;


            PrepareViewBag(s);
            ViewBag.LineMode = "Create";

            return PartialView("_Create", s);
        }






        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _CreatePost(PropertyLineViewModel svm, string BulkMode)
        {
            bool BeforeSave = true;
            Person temp = _PropertyHeaderService.Find(svm.PersonId);

            if (svm.ARV == 0)
                ModelState.AddModelError("ARV", "ARV is required.");

            if (svm.ARV != svm.OldARV && svm.ProductBuyerId != 0)
            {
                if (svm.NewWEF == null)
                    ModelState.AddModelError("NewWEF", "New ARV Effective from date is required.");

                if (svm.ModifyRemark == null)
                    ModelState.AddModelError("ModifyRemark", "Modify Remark is required.");

                if (svm.NewWEF != null && svm.NewWEF < svm.WEF)
                    ModelState.AddModelError("NewWEF", "New Effective from date can not be greater then Current Effective Date.");
            }

            if (svm.Dimension1Id == null || svm.Dimension1Id == 0)
                ModelState.AddModelError("Dimension1Id", "Property Type is required.");
                


            //if (_PropertyLineService.IsDuplicateLine(svm.PersonId,svm.ProductId, svm.Dimension1Id, svm.ProductBuyerId))
            //    ModelState.AddModelError("ProductId", "Product is already entered in Property.");

            ViewBag.Status = temp.Status;


            if (svm.ProductBuyerId <= 0)
            {
                ViewBag.LineMode = "Create";
            }
            else
            {
                ViewBag.LineMode = "Edit";
            }

            if (ModelState.IsValid && BeforeSave)
            {

                if (svm.ProductBuyerId <= 0)
                {
                    try
                    {
                        _PropertyLineService.Create(svm, User.Identity.Name);
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        PrepareViewBag(svm);

                        return PartialView("_Create", svm);
                    }

                    return RedirectToAction("_Create", new { id = svm.PersonId });

                }


                else
                {

                    try
                    {
                        _PropertyLineService.Update(svm, User.Identity.Name);
                    }

                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        TempData["CSEXCL"] += message;
                        PrepareViewBag(svm);
                        return PartialView("_Create", svm);
                    }

                    return Json(new { success = true });
                }

            }
            PrepareViewBag(svm);
            return PartialView("_Create", svm);
        }



        [HttpGet]
        public ActionResult _ModifyLine(int id)
        {
            return _Modify(id);
        }

        [HttpGet]
        public ActionResult _ModifyLineAfterSubmit(int id)
        {
            return _Modify(id);
        }

        [HttpGet]
        public ActionResult _ModifyLineAfterApprove(int id)
        {
            return _Modify(id);
        }

        private ActionResult _Modify(int id)
        {
            PropertyLineViewModel temp = _PropertyLineService.GetProductBuyer(id);

            PropertyHeaderViewModel H = _PropertyHeaderService.GetPropertyHeader(temp.PersonId);

            //Getting Settings

            if (temp == null)
            {
                return HttpNotFound();
            }
            PrepareViewBag(temp);

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocumentLine(new DocumentUniqueId { LockReason = "" }, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXCL"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXCL"] += ExceptionMsg;
            #endregion

            if ((TimePlanValidation || Continue))
                ViewBag.LineMode = "Edit";


            return PartialView("_Create", temp);
        }


        [HttpGet]
        public ActionResult _DeleteLine(int id)
        {
            return _Delete(id);
        }
        [HttpGet]
        public ActionResult _DeleteLine_AfterSubmit(int id)
        {
            return _Delete(id);
        }

        [HttpGet]
        public ActionResult _DeleteLine_AfterApprove(int id)
        {
            return _Delete(id);
        }

        private ActionResult _Delete(int id)
        {
            PropertyLineViewModel temp = _PropertyLineService.GetProductBuyer(id);

            Person H = _PropertyHeaderService.Find(temp.PersonId);

            //Getting Settings

            if (temp == null)
            {
                return HttpNotFound();
            }
            PrepareViewBag(temp);
            //ViewBag.LineMode = "Delete";

            #region DocTypeTimeLineValidation
            try
            {

                TimePlanValidation = DocumentValidation.ValidateDocumentLine(new DocumentUniqueId { LockReason = "" }, User.Identity.Name, out ExceptionMsg, out Continue);

            }
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                TempData["CSEXCL"] += message;
                TimePlanValidation = false;
            }

            if (!TimePlanValidation)
                TempData["CSEXCL"] += ExceptionMsg;
            #endregion

            if ((TimePlanValidation || Continue))
                ViewBag.LineMode = "Delete";

            return PartialView("_Create", temp);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeletePost(PropertyLineViewModel vm)
        {
            bool BeforeSave = true;

            if (BeforeSave)
            {
                try
                {
                    _PropertyLineService.Delete(vm, User.Identity.Name);
                }

                catch (Exception ex)
                {
                    string message = _exception.HandleException(ex);
                    TempData["CSEXCL"] += message;
                    PrepareViewBag(vm);
                    ViewBag.LineMode = "Delete";
                    return PartialView("_Create", vm);

                }
            }

            return Json(new { success = true });

        }


        public JsonResult GetProductDetailJson(int ProductId)
        {
            ProductViewModel product = _PropertyLineService.GetProduct(ProductId);

            return Json(new
            {
                ProductId = product.ProductId,
                UnitId = product.UnitId,
                Specification = product.ProductSpecification,
                StandardCost = product.StandardCost
            });
        }


        public JsonResult GetDiscountTypeDetailJson(int DiscountTypeId)
        {
            var temp = _PropertyLineService.GetDicountTypeDetail(DiscountTypeId);

            if (temp != null)
            {
                return Json(temp);
            }
            else
            {
                return null;
            }
        }



        public JsonResult GetCustomProducts(int id, string term)//Indent Header ID
        {
            return Json(_PropertyLineService.GetProductHelpList(id, term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetARVJson(int Ward, int RoadType, int ConstructionType, int PropertyType, int DiscountType,
    decimal? CoveredArea, decimal? BalconyArea, decimal? GarageArea, bool IsRented, DateTime DateOfConstruction, DateTime WEF)
        {
            var temp = _PropertyLineService.FGetARV(Ward, RoadType, ConstructionType, PropertyType, DiscountType, CoveredArea ?? 0, BalconyArea ?? 0, GarageArea ?? 0, IsRented, DateOfConstruction, WEF);

            if (temp != null)
            {
                return Json(temp);
            }
            else
            {
                return null;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty((string)TempData["CSEXC"]))
            {
                //GenerateCookie.CreateNotificationCookie(NotificationTypeConstants.Danger, (string)TempData["CSEXC"]);
                TempData.Remove("CSEXC");
            }
            if (disposing)
            {
                _PropertyLineService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
