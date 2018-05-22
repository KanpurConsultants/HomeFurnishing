using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Model.Models;
using Model.ViewModels;
using Data.Models;
using Data.Infrastructure;
using Service;
using AutoMapper;
using Presentation.ViewModels;
using Presentation;
using Core.Common;
using System.Text;
using System.IO;
using ImageResizer;
using System.Configuration;
using Jobs.Helpers;
using Model.ViewModel;
using System.Data.SqlClient;

namespace Jobs.Controllers
{
   [Authorize]
    public class EmployeeController : System.Web.Mvc.Controller
    {
       private ApplicationDbContext db = new ApplicationDbContext();
        IEmployeeService _EmployeeService;
        IBusinessEntityService _BusinessEntityService;
        IPersonService _PersonService;
        IPersonAddressService _PersonAddressService;
        IAccountService _AccountService;
        IPersonProcessService _PersonProcessService;
        IPersonRegistrationService _PersonRegistrationService;
        IActivityLogService _ActivityLogService;
        IUnitOfWork _unitOfWork;
        IExceptionHandlingService _exception;

        List<string> UserRoles = new List<string>();

        public EmployeeController(IEmployeeService EmployeeService, IBusinessEntityService BusinessEntityService, IAccountService AccountService, IPersonService PersonService, IPersonRegistrationService PersonRegistrationService, IPersonAddressService PersonAddressService, IPersonProcessService PersonProcessService, IActivityLogService ActivityLogService, IUnitOfWork unitOfWork, IExceptionHandlingService exec)
        {
            _EmployeeService = EmployeeService;
            _PersonService = PersonService;
            _PersonAddressService = PersonAddressService;
            _BusinessEntityService = BusinessEntityService;
            _AccountService = AccountService;
            _PersonProcessService = PersonProcessService;
            _PersonRegistrationService = PersonRegistrationService;
            _ActivityLogService = ActivityLogService;
            _unitOfWork = unitOfWork;
            _exception = exec;

            UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
        }
        // GET: /Order/
        public ActionResult Index()
        {
            var Employees = _EmployeeService.GetEmployeeListForIndex();
            return View(Employees);
        }

        [HttpGet]
        public ActionResult NextPage(int id, string name)//EmployeeId
        {
            var nextId = _EmployeeService.NextId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }
        [HttpGet]
        public ActionResult PrevPage(int id, string name)//EmployeeId
        {
            var nextId = _EmployeeService.PrevId(id);
            return RedirectToAction("Edit", new { id = nextId });
        }

        [HttpGet]
        public ActionResult History()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }
        [HttpGet]
        public ActionResult Print()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }
        [HttpGet]
        public ActionResult Email()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }

        [HttpGet]
        public ActionResult Report()
        {

            DocumentType Dt = new DocumentType();
            Dt = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Employee );

            return Redirect((string)System.Configuration.ConfigurationManager.AppSettings["JobsDomain"] + "/Report_ReportPrint/ReportPrint/?MenuId=" + Dt.ReportMenuId);

        }

        [HttpGet]
        public ActionResult Remove()
        {
            //To Be Implemented
            return View("~/Views/Shared/UnderImplementation.cshtml");
        }




       public void PrepareViewBag()
        {
            ViewBag.SalesTaxGroupPartyList = new SalesTaxGroupPartyService(_unitOfWork).GetSalesTaxGroupPartyList().ToList();
            ViewBag.CityList = new CityService(_unitOfWork).GetCityList().ToList();
            ViewBag.CurrencyList = new CurrencyService(_unitOfWork).GetCurrencyList().ToList();
            ViewBag.PersonList = new PersonService(_unitOfWork).GetPersonList().ToList();
            ViewBag.TdsCategoryList = new TdsCategoryService(_unitOfWork).GetTdsCategoryList().ToList();
            ViewBag.TdsGroupList = new TdsGroupService(_unitOfWork).GetTdsGroupList().ToList();
            ViewBag.PersonRateGroupList = new PersonRateGroupService(_unitOfWork).GetPersonRateGroupList().ToList();
            ViewBag.DesignationList = new DesignationService(_unitOfWork).GetDesignationList().ToList();
            ViewBag.DepartmentList = new DepartmentService(_unitOfWork).GetDepartmentList().ToList();

            List<SelectListItem> WagesPayTypeList = new List<SelectListItem>();
            WagesPayTypeList.Add(new SelectListItem { Text = "Daily", Value = "Daily" });
            WagesPayTypeList.Add(new SelectListItem { Text = "Monthly", Value = "Monthly" });
            ViewBag.WagesPayTypeList = WagesPayTypeList;

            List<SelectListItem> PaymentTypeList = new List<SelectListItem>();
            PaymentTypeList.Add(new SelectListItem { Text = "Cash", Value = "Cash" });
            PaymentTypeList.Add(new SelectListItem { Text = "Cheque", Value = "Cheque" });
            PaymentTypeList.Add(new SelectListItem { Text = "Transfer", Value = "Transfer" });
            ViewBag.PaymentTypeList = PaymentTypeList;
        }

        public ActionResult Create()
        {
            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Employee);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.Employee + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Create") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            var settings = new PersonSettingsService(_unitOfWork).GetPersonSettingsForDocument(DocTypeId);

            if (settings == null && UserRoles.Contains("SysAdmin"))
            {
                return RedirectToAction("Create", "PersonSettings", new { id = DocTypeId }).Warning("Please create Person settings");
            }
            else if (settings == null && !UserRoles.Contains("SysAdmin"))
            {
                return View("~/Views/Shared/InValidSettings.cshtml");
            }

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            EmployeeViewModel p = new EmployeeViewModel();
            p.CalculationId = settings.CalculationId;
            p.IsActive = true;
            p.Code = new PersonService(_unitOfWork).GetMaxCode();
            p.LedgerAccountGroupId = settings.LedgerAccountGroupId;
            p.DateOfJoining = DateTime.Now;

            p.DivisionId = DivisionId;
            p.SiteId = SiteId;
            p.DocTypeId = DocTypeId;
            //p.DateOfRelieving = DateTime.Now;
            PrepareViewBag();
            return View("Create", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeViewModel EmployeeVm)
        {
            string[] ProcessIdArr;
            if (EmployeeVm.LedgerAccountGroupId == 0)
            {
                PrepareViewBag();
                return View(EmployeeVm).Danger("Account Group field is required");
            }

            if (EmployeeVm.TdsCategoryId == 0 || EmployeeVm.TdsCategoryId == null)
            {
                PrepareViewBag();
                return View(EmployeeVm).Danger("Tds Category field is required");
            }

            if (EmployeeVm.TdsGroupId == 0 || EmployeeVm.TdsGroupId == null)
            {
                PrepareViewBag();
                return View(EmployeeVm).Danger("Tds Group field is required");
            }

            if (EmployeeVm.DepartmentId == 0 || EmployeeVm.DepartmentId == null)
            {
                PrepareViewBag();
                return View(EmployeeVm).Danger("Department field is required");
            }

            if (EmployeeVm.DesignationId == 0 || EmployeeVm.DesignationId == null)
            {
                PrepareViewBag();
                return View(EmployeeVm).Danger("Designatio field is required");
            }
            //if (_PersonService.CheckDuplicate(EmployeeVm.Name, EmployeeVm.Suffix, EmployeeVm.PersonId) == true)
            //{
            //    PrepareViewBag();
            //    return View(EmployeeVm).Danger("Combination of name and sufix is duplicate");
            //}


            if (ModelState.IsValid)
            {
                if (EmployeeVm.EmployeeId == 0)
                {
                    Person person = Mapper.Map<EmployeeViewModel, Person>(EmployeeVm);
                    BusinessEntity businessentity = Mapper.Map<EmployeeViewModel, BusinessEntity>(EmployeeVm);
                    Employee Employee = Mapper.Map<EmployeeViewModel, Employee>(EmployeeVm);
                    PersonAddress personaddress = Mapper.Map<EmployeeViewModel, PersonAddress>(EmployeeVm);
                    LedgerAccount account = Mapper.Map<EmployeeViewModel, LedgerAccount>(EmployeeVm);

                    person.DocTypeId = new DocumentTypeService(_unitOfWork).Find(MasterDocTypeConstants.Employee).DocumentTypeId;
                    person.CreatedDate = DateTime.Now;
                    person.ModifiedDate = DateTime.Now;
                    person.CreatedBy = User.Identity.Name;
                    person.ModifiedBy = User.Identity.Name;
                    person.ObjectState = Model.ObjectState.Added;
                    new PersonService(_unitOfWork).Create(person);

                    string Divisions = EmployeeVm.DivisionIds;
                    if (Divisions != null)
                    { Divisions = "|" + Divisions.Replace(",", "|,|") + "|"; }

                    businessentity.DivisionIds = Divisions;

                    string Sites = EmployeeVm.SiteIds;
                    if (Sites != null)
                    { Sites = "|" + Sites.Replace(",", "|,|") + "|"; }

                    businessentity.SiteIds = Sites;

                    _BusinessEntityService.Create(businessentity);
                    _EmployeeService.Create(Employee);


                    personaddress.AddressType = AddressTypeConstants.Work;
                    personaddress.CreatedDate = DateTime.Now;
                    personaddress.ModifiedDate = DateTime.Now;
                    personaddress.CreatedBy = User.Identity.Name;
                    personaddress.ModifiedBy = User.Identity.Name;
                    personaddress.ObjectState = Model.ObjectState.Added;
                    _PersonAddressService.Create(personaddress);


                    account.LedgerAccountName = person.Name;
                    account.LedgerAccountSuffix = person.Suffix;
                    account.LedgerAccountGroupId = EmployeeVm.LedgerAccountGroupId;
                    account.CreatedDate = DateTime.Now;
                    account.ModifiedDate = DateTime.Now;
                    account.CreatedBy = User.Identity.Name;
                    account.ModifiedBy = User.Identity.Name;
                    account.ObjectState = Model.ObjectState.Added;
                    _AccountService.Create(account);

                    if (EmployeeVm.ProcessIds != null && EmployeeVm.ProcessIds != "")
                    {
                        ProcessIdArr = EmployeeVm.ProcessIds.Split(new Char[] { ',' });

                        for (int i = 0; i <= ProcessIdArr.Length - 1; i++)
                        {
                            PersonProcess personprocess = new PersonProcess();
                            personprocess.PersonId = Employee.PersonID;
                            personprocess.ProcessId = Convert.ToInt32(ProcessIdArr[i]);
                            personprocess.CreatedDate = DateTime.Now;
                            personprocess.ModifiedDate = DateTime.Now;
                            personprocess.CreatedBy = User.Identity.Name;
                            personprocess.ModifiedBy = User.Identity.Name;
                            personprocess.ObjectState = Model.ObjectState.Added;
                            _PersonProcessService.Create(personprocess);
                        }
                    }

                    if (EmployeeVm.PanNo != "" && EmployeeVm.PanNo != null)
                    {
                        PersonRegistration personregistration = new PersonRegistration();
                        personregistration.RegistrationType = PersonRegistrationType.PANNo;
                        personregistration.RegistrationNo = EmployeeVm.PanNo;
                        personregistration.CreatedDate = DateTime.Now;
                        personregistration.ModifiedDate = DateTime.Now;
                        personregistration.CreatedBy = User.Identity.Name;
                        personregistration.ModifiedBy = User.Identity.Name;
                        personregistration.ObjectState = Model.ObjectState.Added;
                        _PersonRegistrationService.Create(personregistration);
                    }


                    //var Footer = (from H in db.CalculationFooter where H.CalculationId == EmployeeVm.CalculationId select H).ToList();
                    //IEnumerable<EmployeeCharge> EmployeeChargeList = Mapper.Map < IEnumerable<CalculationFooter>, IEnumerable<EmployeeCharge>>(Footer);


                    //int cnt = 0;
                    //foreach (var item in EmployeeChargeList)
                    //{
                    //    item.Id = cnt;
                    //    item.HeaderTableId = Employee.EmployeeId;
                    //    new EmployeeChargeService(_unitOfWork).Create(item);
                    //    cnt = cnt - 1;
                    //}


                    if (EmployeeVm.linecharges != null)
                        foreach (var item in EmployeeVm.linecharges)
                        {
                            item.LineTableId = EmployeeVm.EmployeeId;
                            item.PersonID = EmployeeVm.PersonId;
                            item.DealQty = 0;
                            item.HeaderTableId = EmployeeVm.EmployeeId;
                            new EmployeeLineChargeService(_unitOfWork).Create(item);
                        }

                    if (EmployeeVm.footercharges != null)
                        foreach (var item in EmployeeVm.footercharges)
                        {
                            if (item.Id > 0)
                            {

                                var footercharge = new EmployeeChargeService(_unitOfWork).Find(item.Id);
                                footercharge.Rate = item.Rate;
                                footercharge.Amount = item.Amount;
                                new EmployeeChargeService(_unitOfWork).Update(footercharge);
                            }

                            else
                            {
                                item.HeaderTableId = EmployeeVm.EmployeeId;
                                item.PersonID = EmployeeVm.EmployeeId;
                                new EmployeeChargeService(_unitOfWork).Create(item);
                            }
                        }

                    try
                    {
                        _unitOfWork.Save();
                    }
                 
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        PrepareViewBag();
                        return View(EmployeeVm);

                    }


                    #region

                    //Saving Images if any uploaded after UnitOfWorkSave

                    if (Request.Files[0] != null && Request.Files[0].ContentLength > 0)
                    {
                        //For checking the first time if the folder exists or not-----------------------------
                        string uploadfolder;
                        int MaxLimit;
                        int.TryParse(ConfigurationManager.AppSettings["MaxFileUploadLimit"], out MaxLimit);
                        var x = (from iid in db.Counter
                                 select iid).FirstOrDefault();
                        if (x == null)
                        {

                            uploadfolder = System.Guid.NewGuid().ToString();
                            Counter img = new Counter();
                            img.ImageFolderName = uploadfolder;
                            img.ModifiedBy = User.Identity.Name;
                            img.CreatedBy = User.Identity.Name;
                            img.ModifiedDate = DateTime.Now;
                            img.CreatedDate = DateTime.Now;
                            new CounterService(_unitOfWork).Create(img);
                            _unitOfWork.Save();
                        }

                        else
                        { uploadfolder = x.ImageFolderName; }


                        //For checking if the image contents length is greater than 100 then create a new folder------------------------------------

                        if (!Directory.Exists(System.Web.HttpContext.Current.Request.MapPath("~/Uploads/" + uploadfolder))) Directory.CreateDirectory(System.Web.HttpContext.Current.Request.MapPath("~/Uploads/" + uploadfolder));

                        int count = Directory.GetFiles(System.Web.HttpContext.Current.Request.MapPath("~/Uploads/" + uploadfolder)).Length;

                        if (count >= MaxLimit)
                        {
                            uploadfolder = System.Guid.NewGuid().ToString();
                            var u = new CounterService(_unitOfWork).Find(x.CounterId);
                            u.ImageFolderName = uploadfolder;
                            new CounterService(_unitOfWork).Update(u);
                            _unitOfWork.Save();
                        }


                        //Saving Thumbnails images:
                        Dictionary<string, string> versions = new Dictionary<string, string>();

                        //Define the versions to generate
                        versions.Add("_thumb", "maxwidth=100&maxheight=100"); //Crop to square thumbnail
                        versions.Add("_medium", "maxwidth=200&maxheight=200"); //Fit inside 400x400 area, jpeg

                        string temp2 = "";
                        string filename = System.Guid.NewGuid().ToString();
                        foreach (string filekey in System.Web.HttpContext.Current.Request.Files.Keys)
                        {

                            HttpPostedFile pfile = System.Web.HttpContext.Current.Request.Files[filekey];
                            if (pfile.ContentLength <= 0) continue; //Skip unused file controls.  

                            temp2 = Path.GetExtension(pfile.FileName);

                            string uploadFolder = System.Web.HttpContext.Current.Request.MapPath("~/Uploads/" + uploadfolder);
                            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                            string filecontent = Path.Combine(uploadFolder, EmployeeVm.Name + "_" + filename);

                            //pfile.SaveAs(filecontent);
                            ImageBuilder.Current.Build(new ImageJob(pfile, filecontent, new Instructions(), false, true));


                            //Generate each version
                            foreach (string suffix in versions.Keys)
                            {
                                if (suffix == "_thumb")
                                {
                                    string tuploadFolder = System.Web.HttpContext.Current.Request.MapPath("~/Uploads/" + uploadfolder + "/Thumbs");
                                    if (!Directory.Exists(tuploadFolder)) Directory.CreateDirectory(tuploadFolder);

                                    //Generate a filename (GUIDs are best).
                                    string tfileName = Path.Combine(tuploadFolder, EmployeeVm.Name + "_" + filename);

                                    //Let the image builder add the correct extension based on the output file type
                                    //fileName = ImageBuilder.Current.Build(file, fileName, new ResizeSettings(versions[suffix]), false, true);
                                    ImageBuilder.Current.Build(new ImageJob(pfile, tfileName, new Instructions(versions[suffix]), false, true));

                                }
                                else if (suffix == "_medium")
                                {
                                    string tuploadFolder = System.Web.HttpContext.Current.Request.MapPath("~/Uploads/" + uploadfolder + "/Medium");
                                    if (!Directory.Exists(tuploadFolder)) Directory.CreateDirectory(tuploadFolder);

                                    //Generate a filename (GUIDs are best).
                                    string tfileName = Path.Combine(tuploadFolder, EmployeeVm.Name + "_" + filename);

                                    //Let the image builder add the correct extension based on the output file type
                                    //fileName = ImageBuilder.Current.Build(file, fileName, new ResizeSettings(versions[suffix]), false, true);
                                    ImageBuilder.Current.Build(new ImageJob(pfile, tfileName, new Instructions(versions[suffix]), false, true));
                                }

                            }

                            //var tempsave = _FinishedProductService.Find(pt.ProductId);

                            person.ImageFileName = EmployeeVm.Name+ "_" + filename + temp2;
                            person.ImageFolderName = uploadfolder;
                            person.ObjectState = Model.ObjectState.Modified;
                            _PersonService.Update(person);
                            _unitOfWork.Save();
                        }

                    }

                    #endregion



                    //return RedirectToAction("Create").Success("Data saved successfully");
                    return RedirectToAction("Edit", new { id = Employee.EmployeeId }).Success("Data saved Successfully");
                }
                else
                {
                    //string tempredirect = (Request["Redirect"].ToString());
                    Person person = Mapper.Map<EmployeeViewModel, Person>(EmployeeVm);
                    BusinessEntity businessentity = Mapper.Map<EmployeeViewModel, BusinessEntity>(EmployeeVm);
                    Employee Employee = Mapper.Map<EmployeeViewModel, Employee>(EmployeeVm);
                    PersonAddress personaddress = _PersonAddressService.Find(EmployeeVm.PersonAddressID);
                    LedgerAccount account = _AccountService.Find(EmployeeVm.AccountId);
                    PersonRegistration PersonPan = _PersonRegistrationService.Find(EmployeeVm.PersonRegistrationPanNoID);
                    
                    StringBuilder logstring = new StringBuilder();                 

                    person.ModifiedDate = DateTime.Now;
                    person.ModifiedBy = User.Identity.Name;
                    new PersonService(_unitOfWork).Update(person);

                    string Divisions = EmployeeVm.DivisionIds;
                    if (Divisions != null)
                    { Divisions = "|" + Divisions.Replace(",", "|,|") + "|"; }

                    businessentity.DivisionIds = Divisions;

                    string Sites = EmployeeVm.SiteIds;
                    if (Sites != null)
                    { Sites = "|" + Sites.Replace(",", "|,|") + "|"; }

                    businessentity.SiteIds = Sites;


                    _BusinessEntityService.Update(businessentity);
                    _EmployeeService.Update(Employee);

                    personaddress.Address = EmployeeVm.Address;
                    personaddress.CityId = EmployeeVm.CityId;
                    personaddress.Zipcode = EmployeeVm.Zipcode;
                    personaddress.ModifiedDate = DateTime.Now;
                    personaddress.ModifiedBy = User.Identity.Name;
                    personaddress.ObjectState = Model.ObjectState.Modified;
                    _PersonAddressService.Update(personaddress);

                    account.LedgerAccountName = person.Name;
                    account.LedgerAccountGroupId = EmployeeVm.LedgerAccountGroupId;
                    account.IsActive = person.IsActive;
                    account.LedgerAccountSuffix = person.Suffix;
                    account.ModifiedDate = DateTime.Now;
                    account.ModifiedBy = User.Identity.Name;
                    _AccountService.Update(account);


                    if (EmployeeVm.ProcessIds != "" && EmployeeVm.ProcessIds != null)
                    {

                        IEnumerable<PersonProcess> personprocesslist = _PersonProcessService.GetPersonProcessList(EmployeeVm.PersonId);

                        foreach (PersonProcess item in personprocesslist)
                        {
                            new PersonProcessService(_unitOfWork).Delete(item.PersonProcessId);
                        }



                        ProcessIdArr = EmployeeVm.ProcessIds.Split(new Char[] { ',' });

                        for (int i = 0; i <= ProcessIdArr.Length - 1; i++)
                        {
                            PersonProcess personprocess = new PersonProcess();
                            personprocess.PersonId = Employee.PersonID;
                            personprocess.ProcessId = Convert.ToInt32(ProcessIdArr[i]);
                            personprocess.CreatedDate = DateTime.Now;
                            personprocess.ModifiedDate = DateTime.Now;
                            personprocess.CreatedBy = User.Identity.Name;
                            personprocess.ModifiedBy = User.Identity.Name;
                            personprocess.ObjectState = Model.ObjectState.Added;
                            _PersonProcessService.Create(personprocess);
                        }
                    }
                 

                    if (EmployeeVm.PanNo != null && EmployeeVm.PanNo != "" )
                    {
                        if (PersonPan != null)
                        {
                            PersonPan.RegistrationNo = EmployeeVm.PanNo;
                            _PersonRegistrationService.Update(PersonPan);
                        }
                        else
                        {
                            PersonRegistration personregistration = new PersonRegistration();
                            personregistration.PersonId = EmployeeVm.PersonId;
                            personregistration.RegistrationType = PersonRegistrationType.PANNo;
                            personregistration.RegistrationNo = EmployeeVm.PanNo;
                            personregistration.CreatedDate = DateTime.Now;
                            personregistration.ModifiedDate = DateTime.Now;
                            personregistration.CreatedBy = User.Identity.Name;
                            personregistration.ModifiedBy = User.Identity.Name;
                            personregistration.ObjectState = Model.ObjectState.Added;
                            _PersonRegistrationService.Create(personregistration);
                        }
                    }


                    if (EmployeeVm.linecharges != null)
                    {
                        var ProductChargeList = (from p in db.EmployeeLineCharge
                                                 where p.LineTableId == EmployeeVm.EmployeeId
                                                 select p).ToList();

                        foreach (var item in EmployeeVm.linecharges)
                        {
                            var productcharge = (ProductChargeList.Where(m => m.Id == item.Id)).FirstOrDefault();

                            var ExProdcharge = Mapper.Map<EmployeeLineCharge>(productcharge);
                            productcharge.Rate = item.Rate ?? 0;
                            productcharge.Amount = item.Amount ?? 0;
                            productcharge.DealQty = 0;
                            new EmployeeLineChargeService(_unitOfWork).Update(productcharge);
                        }
                    }

                    if (EmployeeVm.footercharges != null)
                    {
                        var footerChargerecords = (from p in db.EmployeeCharge
                                                   where p.HeaderTableId == EmployeeVm.EmployeeId
                                                   select p).ToList();

                        foreach (var item in EmployeeVm.footercharges)
                        {
                            var footercharge = footerChargerecords.Where(m => m.Id == item.Id).FirstOrDefault();
                            var Exfootercharge = Mapper.Map<EmployeeCharge>(footercharge);
                            footercharge.Rate = item.Rate ?? 0;
                            footercharge.Amount = item.Amount ?? 0;
                            new EmployeeChargeService(_unitOfWork).Update(footercharge);
                        }
                    }



                    ////Saving Activity Log::
                    ActivityLog al = new ActivityLog()
                    {
                        ActivityType = (int)ActivityTypeContants.Modified,
                        DocId = EmployeeVm.EmployeeId,
                        Narration = logstring.ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        //DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(TransactionDocCategoryConstants.ProcessSequence).DocumentTypeId,

                    };
                    new ActivityLogService(_unitOfWork).Create(al);
                    //End of Saving ActivityLog

                    try
                    {
                        _unitOfWork.Save();
                    }
               
                    catch (Exception ex)
                    {
                        string message = _exception.HandleException(ex);
                        ModelState.AddModelError("", message);
                        PrepareViewBag();
                        return View("Create", EmployeeVm);
                    }




                    #region

                    //Saving Image if file is uploaded
                    if (Request.Files[0] != null && Request.Files[0].ContentLength > 0)
                    {
                        string uploadfolder = EmployeeVm.ImageFolderName;
                        string tempfilename = EmployeeVm.ImageFileName;
                        if (uploadfolder == null)
                        {
                            var x = (from iid in db.Counter
                                     select iid).FirstOrDefault();
                            if (x == null)
                            {

                                uploadfolder = System.Guid.NewGuid().ToString();
                                Counter img = new Counter();
                                img.ImageFolderName = uploadfolder;
                                img.ModifiedBy = User.Identity.Name;
                                img.CreatedBy = User.Identity.Name;
                                img.ModifiedDate = DateTime.Now;
                                img.CreatedDate = DateTime.Now;
                                new CounterService(_unitOfWork).Create(img);
                                _unitOfWork.Save();
                            }
                            else
                            { uploadfolder = x.ImageFolderName; }

                        }
                        //Deleting Existing Images

                        var xtemp = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + uploadfolder + "/" + tempfilename);
                        if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + uploadfolder + "/" + tempfilename)))
                        {
                            System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + uploadfolder + "/" + tempfilename));
                        }

                        //Deleting Thumbnail Image:

                        if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + uploadfolder + "/Thumbs/" + tempfilename)))
                        {
                            System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + uploadfolder + "/Thumbs/" + tempfilename));
                        }

                        //Deleting Medium Image:
                        if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + uploadfolder + "/Medium/" + tempfilename)))
                        {
                            System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + uploadfolder + "/Medium/" + tempfilename));
                        }

                        //Saving Thumbnails images:
                        Dictionary<string, string> versions = new Dictionary<string, string>();

                        //Define the versions to generate
                        versions.Add("_thumb", "maxwidth=100&maxheight=100"); //Crop to square thumbnail
                        versions.Add("_medium", "maxwidth=200&maxheight=200"); //Fit inside 400x400 area, jpeg                            

                        string temp2 = "";
                        string filename = System.Guid.NewGuid().ToString();
                        foreach (string filekey in System.Web.HttpContext.Current.Request.Files.Keys)
                        {

                            HttpPostedFile pfile = System.Web.HttpContext.Current.Request.Files[filekey];
                            if (pfile.ContentLength <= 0) continue; //Skip unused file controls.    

                            temp2 = Path.GetExtension(pfile.FileName);

                            string uploadFolder = System.Web.HttpContext.Current.Request.MapPath("~/Uploads/" + uploadfolder);
                            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                            string filecontent = Path.Combine(uploadFolder, EmployeeVm.Name + "_" + filename);

                            //pfile.SaveAs(filecontent);

                            ImageBuilder.Current.Build(new ImageJob(pfile, filecontent, new Instructions(), false, true));

                            //Generate each version
                            foreach (string suffix in versions.Keys)
                            {
                                if (suffix == "_thumb")
                                {
                                    string tuploadFolder = System.Web.HttpContext.Current.Request.MapPath("~/Uploads/" + uploadfolder + "/Thumbs");
                                    if (!Directory.Exists(tuploadFolder)) Directory.CreateDirectory(tuploadFolder);

                                    //Generate a filename (GUIDs are best).
                                    string tfileName = Path.Combine(tuploadFolder, EmployeeVm.Name + "_" + filename);

                                    //Let the image builder add the correct extension based on the output file type
                                    //fileName = ImageBuilder.Current.Build(file, fileName, new ResizeSettings(versions[suffix]), false, true);
                                    ImageBuilder.Current.Build(new ImageJob(pfile, tfileName, new Instructions(versions[suffix]), false, true));

                                }
                                else if (suffix == "_medium")
                                {
                                    string tuploadFolder = System.Web.HttpContext.Current.Request.MapPath("~/Uploads/" + uploadfolder + "/Medium");
                                    if (!Directory.Exists(tuploadFolder)) Directory.CreateDirectory(tuploadFolder);

                                    //Generate a filename (GUIDs are best).
                                    string tfileName = Path.Combine(tuploadFolder, EmployeeVm.Name + "_" + filename);

                                    //Let the image builder add the correct extension based on the output file type
                                    //fileName = ImageBuilder.Current.Build(file, fileName, new ResizeSettings(versions[suffix]), false, true);
                                    ImageBuilder.Current.Build(new ImageJob(pfile, tfileName, new Instructions(versions[suffix]), false, true));
                                }
                            }
                        }
                        var temsave = _PersonService.Find(person.PersonID);
                        person.ImageFileName = temsave.Name+ "_" + filename + temp2;
                        temsave.ImageFolderName = uploadfolder;
                        _PersonService.Update(temsave);
                        _unitOfWork.Save();
                    }

                    #endregion  






                    return RedirectToAction("Index").Success("Data saved successfully");
                }
            }
            PrepareViewBag();
            return View(EmployeeVm);
        }


        public ActionResult Edit(int id)
        {
            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Employee);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.Employee + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Edit") == false)
            {
                return View("~/Views/Shared/PermissionDenied.cshtml").Warning("You don't have permission to do this task.");
            }

            EmployeeViewModel bvm = _EmployeeService.GetEmployeeViewModel(id);
            PrepareViewBag();
            if (bvm == null)
            {
                return HttpNotFound();
            }
            return View("Create", bvm);
        }


        // GET: /ProductMaster/Delete/5
        
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var DocType = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Employee);
            int DocTypeId = 0;

            if (DocType != null)
                DocTypeId = DocType.DocumentTypeId;
            else
                return View("~/Views/Shared/InValidSettings.cshtml").Warning("Document Type named " + MasterDocTypeConstants.Employee + " is not defined in database.");

            if (new RolePermissionService(_unitOfWork).IsActionAllowed(UserRoles, DocTypeId, null, this.ControllerContext.RouteData.Values["controller"].ToString(), "Delete") == false)
            {
                return PartialView("~/Views/Shared/PermissionDenied_Modal.cshtml").Warning("You don't have permission to do this task.");
            }

            Employee Employee = _EmployeeService.GetEmployee(id);
            if (Employee == null)
            {
                return HttpNotFound();
            }

            ReasonViewModel vm = new ReasonViewModel()
            {
                id = id,
            };

            return PartialView("_Reason", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ReasonViewModel vm)
        {


            if(ModelState.IsValid)
            {
                Employee employee = new EmployeeService(_unitOfWork).Find(vm.id);
                Person person = new PersonService(_unitOfWork).Find(employee.PersonID);
                BusinessEntity businessentiry = _BusinessEntityService.Find(employee.PersonID);

                ActivityLog al = new ActivityLog()
                {
                    ActivityType = (int)ActivityTypeContants.Deleted,
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now,
                    DocId = vm.id,
                    UserRemark = vm.Reason,
                    Narration = "Employee is deleted with Name:" + person.Name,
                    DocTypeId = new DocumentTypeService(_unitOfWork).FindByName(MasterDocTypeConstants.Employee).DocumentTypeId,
                    UploadDate = DateTime.Now,

                };
                new ActivityLogService(_unitOfWork).Create(al);

                //Then find Ledger Account associated with the above Person.
                LedgerAccount ledgeraccount = _AccountService.GetLedgerAccountFromPersonId(person.PersonID);
                _AccountService.Delete(ledgeraccount.LedgerAccountId);

                //Then find all the Person Address associated with the above Person.
                PersonAddress personaddress = _PersonAddressService.GetShipAddressByPersonId(person.PersonID);
                _PersonAddressService.Delete(personaddress.PersonAddressID);


                IEnumerable<PersonContact> personcontact = new PersonContactService(_unitOfWork).GetPersonContactIdListByPersonId(person.PersonID);
                //Mark ObjectState.Delete to all the Person Contact For Above Person. 
                foreach (PersonContact item in personcontact)
                {
                    new PersonContactService(_unitOfWork).Delete(item.PersonContactID);
                }

                IEnumerable<PersonBankAccount> personbankaccount = new PersonBankAccountService(_unitOfWork).GetPersonBankAccountIdListByPersonId(person.PersonID);
                //Mark ObjectState.Delete to all the Person Contact For Above Person. 
                foreach (PersonBankAccount item in personbankaccount)
                {
                    new PersonBankAccountService(_unitOfWork).Delete(item.PersonBankAccountID);
                }


                IEnumerable<PersonProcess> personProcess = new PersonProcessService(_unitOfWork).GetPersonProcessIdListByPersonId(employee.PersonID);
                //Mark ObjectState.Delete to all the Person Process For Above Person. 
                foreach (PersonProcess item in personProcess)
                {
                    new PersonProcessService(_unitOfWork).Delete(item.PersonProcessId);
                }


                IEnumerable<PersonRegistration> personregistration = new PersonRegistrationService(_unitOfWork).GetPersonRegistrationIdListByPersonId(employee.PersonID);
                //Mark ObjectState.Delete to all the Person Registration For Above Person. 
                foreach (PersonRegistration item in personregistration)
                {
                    new PersonRegistrationService(_unitOfWork).Delete(item.PersonRegistrationID);
                }


                IEnumerable<EmployeeLineCharge> EmployeeLineChargeList = (from L in db.EmployeeLineCharge where L.LineTableId == employee.EmployeeId select L).ToList();
                foreach (var EmployeeLineCharge in EmployeeLineChargeList)
                {
                    new EmployeeLineChargeService(_unitOfWork).Delete(EmployeeLineCharge.Id);
                }

                IEnumerable<EmployeeCharge> EmployeeChargeList = (from L in db.EmployeeCharge where L.HeaderTableId == employee.EmployeeId select L).ToList();
                foreach(var EmployeeCharge in EmployeeChargeList)
                {
                    new EmployeeChargeService(_unitOfWork).Delete(EmployeeCharge.Id);
                }


            // Now delete the Parent Employee
                _EmployeeService.Delete(employee);
                _BusinessEntityService.Delete(businessentiry);
                new PersonService(_unitOfWork).Delete(person);


            try
            {
                _unitOfWork.Save();
            }
         
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return PartialView("_Reason", vm);
            }
            return Json(new { success = true });
            }
            return PartialView("_Reason", vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult AddToExisting()
        {
            return PartialView("AddToExisting");
        }

        [HttpPost]
        public ActionResult AddToExisting(AddToExistingContactViewModel svm)
        {
            Employee Employee = new Employee();
            Employee.PersonID = svm.PersonId;
            _EmployeeService.Create(Employee);

            try
            {
                _unitOfWork.Save();
            }
          
            catch (Exception ex)
            {
                string message = _exception.HandleException(ex);
                ModelState.AddModelError("", message);
                return PartialView("_Create", svm);

            }

            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult ChooseContactType()
        {
            return PartialView("ChooseContactType");
        }




    }
}
