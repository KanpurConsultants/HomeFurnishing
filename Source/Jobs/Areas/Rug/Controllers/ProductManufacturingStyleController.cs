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
using Jobs.Helpers;

namespace Jobs.Areas.Rug.Controllers
{
    [Authorize]
    public class ProductManufacturingStyleController : System.Web.Mvc.Controller
    {
          private ApplicationDbContext db = new ApplicationDbContext();

          List<string> UserRoles = new List<string>();
          IProductManufacturingStyleService _ProductManufacturingStyleService;
          IUnitOfWork _unitOfWork;
          public ProductManufacturingStyleController(IProductManufacturingStyleService ProductManufacturingStyleService, IUnitOfWork unitOfWork)
          {
              UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

              _ProductManufacturingStyleService = ProductManufacturingStyleService;
              _unitOfWork = unitOfWork;
          }
        // GET: /ProductMaster/
          public ActionResult Index()
          { 
              var ProductManufacturingStyle = _ProductManufacturingStyleService.GetProductManufacturingStyleList().ToList();
              return View(ProductManufacturingStyle);
          }

        // GET: /ProductMaster/Create
          public ActionResult Create()
          {
              var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ProductManufacturingStyle);
              int DocTypeId = 0;

              if (DocType != null)
                  DocTypeId = DocType.DocumentTypeId;
              else
                  return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.ProductManufacturingStyle + " is not defined in database.");

              if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
              {
                  return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
              }

              return View();
          }

        // POST: /ProductMaster/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
          public ActionResult Create(ProductStyle pt)
          {
              if (ModelState.IsValid)
              {
                  pt.CreatedDate = DateTime.Now;
                  pt.ModifiedDate = DateTime.Now;
                  pt.CreatedBy = User.Identity.Name;
                  pt.ModifiedBy = User.Identity.Name;
                  pt.ObjectState = Model.ObjectState.Added;
                  _ProductManufacturingStyleService.Create(pt);
                  _unitOfWork.Save();
                  return RedirectToAction("Index");
              }

              return View(pt);
          }


        // GET: /ProductMaster/Edit/5
        public ActionResult Edit(int id)
        {
            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ProductManufacturingStyle);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.ProductManufacturingStyle + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            ProductStyle pt = _ProductManufacturingStyleService.GetProductManufacturingStyle(id);
            if (pt == null)
            {
                return HttpNotFound();
            }
            return View(pt);
        }

        // POST: /ProductMaster/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductStyle pt)
        {           

            if (ModelState.IsValid)
            {
                pt.ModifiedDate = DateTime.Now;
                pt.ModifiedBy = User.Identity.Name;
                pt.ObjectState = Model.ObjectState.Modified;
                _ProductManufacturingStyleService.Update(pt);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(pt);
        }

        // GET: /ProductMaster/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.ProductManufacturingStyle);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.ProductManufacturingStyle + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Delete") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            ProductStyle ProductManufacturingStyle = db.ProductStyle.Find(id);
            if (ProductManufacturingStyle == null)
            {
                return HttpNotFound();
            }
            return View(ProductManufacturingStyle);
        }

        // POST: /ProductMaster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductStyle ProductManufacturingStyle = db.ProductStyle.Find(id);
            db.ProductStyle.Remove(ProductManufacturingStyle);
            db.SaveChanges();
            return RedirectToAction("Index");
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
