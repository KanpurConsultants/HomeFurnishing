using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity.SqlServer;
using Components.Logging;
using AutoMapper;
using System.Xml.Linq;
using System.Data;
using Model.Models;
using Model.ViewModels;
using Presentation.ViewModels;
using Core.Common;
using Service;
using Data.Infrastructure;
using Model.ViewModel;
using Model.PropertyTax.Models;
using Model.DatabaseViews;

namespace Services.PropertyTax
{
    public interface IPropertyHeaderService : IDisposable
    {
        Person Create(Person s);
        PropertyHeaderViewModel Create(PropertyHeaderViewModel vmPropertyHeader, string UserName);
        void Delete(int id);
        void Delete(Person s);
        void Delete(ReasonViewModel vm, string UserName);
        PropertyHeaderViewModel GetPropertyHeader(int id);
        Person Find(int id);
        IQueryable<PropertyHeaderViewModel> GetPropertyHeaderList(int DocumentTypeId, string Uname);
        //IQueryable<PropertyHeaderViewModel> GetPropertyHeaderList(int DocumentTypeId, int GodownId, string Uname);
        IEnumerable<PropertyHeaderViewModel> GetPropertyHeaderList(int DocumentTypeId, int GodownId, string Uname);
        IQueryable<PropertyHeaderViewModel> GetPropertyHeaderListPendingToSubmit(int DocumentTypeId, int GodownId, string Uname);
        IQueryable<PropertyHeaderViewModel> GetPropertyHeaderListPendingToReview(int DocumentTypeId, int GodownId, string Uname);
        void Update(Person s);
        void Update(PropertyHeaderViewModel vmPropertyHeader, string UserName);
        string GetMaxDocNo();
        DateTime AddDueDate(DateTime Base, int DueDays);
        void Submit(int Id, string UserName, string GenGatePass, string UserRemark);
        void Review(int Id, string UserName, string UserRemark);
        int NextPrevId(int DocId, int DocTypeId, string UserName, string PrevNextConstants);
        //byte[] GetReport(string Ids, int DocTypeId, string UserName, string SqlProc);



        ComboBoxPagedResult GetPersonWithDocType(string searchTerm, int pageSize, int pageNum, int DocTypeId);
        string FGetNewPersonCode(int SiteId, int GodownId, int? BinLocationId);
        string FGetNewHouseNo(int SiteId, int GodownId, int BinLocationId);



        Decimal GetIntrestBalance(int PersonId);
        Decimal GetArearBalance(int PersonId);
        Decimal GetExcessBalance(int PersonId);
        Decimal GetCurrentYearBalance(int PersonId);
        Decimal GetNetOutstanding(int PersonId);
        ComboBoxPagedResult GetProperty(string searchTerm, int pageSize, int pageNum);
        IEnumerable<DocumentTypeAttributeViewModel> GetAttributeForDocumentType(int DocumentTypeId);
        IEnumerable<DocumentTypeAttributeViewModel> GetAttributeForPerson(int id);
        IQueryable<WardIndexViewModel> GetGodownListForIndex(int SiteId);

        #region Helper Methods
        void LogDetailInfo(PropertyHeaderViewModel vm);
        _Menu GetMenu(int Id);
        _Menu GetMenu(string Name);
        _ReportHeader GetReportHeader(string MenuName);
        _ReportLine GetReportLine(string Name, int ReportHeaderId);
        bool CheckForDocNoExists(string docno, int DocTypeId);
        bool CheckForDocNoExists(string docno, int headerid, int DocTypeId);
        #endregion

    }
    public class PropertyHeaderService : IPropertyHeaderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Person> _PropertyRepository;
        private readonly ILogger _logger;
        private readonly IModificationCheck _modificationCheck;
        private readonly IDocumentTypeService _DocumentTypeService;

        private ActiivtyLogViewModel logVm = new ActiivtyLogViewModel();

        public PropertyHeaderService(IUnitOfWork unit, IRepository<Person> PropertyRepo,
            IDocumentTypeService DocumentTypeService,
            ILogger log, IModificationCheck modificationCheck)
        {
            _unitOfWork = unit;
            _PropertyRepository = PropertyRepo;
            _DocumentTypeService = DocumentTypeService;
            _logger = log;
            _modificationCheck = modificationCheck;

            //Log Initialization
            logVm.SessionId = 0;
            logVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            logVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            logVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;

        }

        public Person Create(Person s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Person>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Person>().Delete(id);
        }
        public void Delete(Person s)
        {
            _unitOfWork.Repository<Person>().Delete(s);
        }
        public void Update(Person s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Person>().Update(s);
        }


        public Person Find(int id)
        {
            return _unitOfWork.Repository<Person>().Find(id);
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<Person>().Query().Get().Select(i => i.Code ).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        public IQueryable<PropertyHeaderViewModel> GetPropertyHeaderList(int DocumentTypeId, string Uname)
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            return (from p in _PropertyRepository.Instance
                    join dt in _unitOfWork.Repository<DocumentType>().Instance on p.DocTypeId equals dt.DocumentTypeId
                    join pe in _unitOfWork.Repository<PersonExtended>().Instance on p.PersonID equals pe.PersonId into PersonExtendedTable
                    from PersonExtendedTab in PersonExtendedTable.DefaultIfEmpty()
                    orderby PersonExtendedTab.Godown.GodownName, PersonExtendedTab.HouseNo
                    where p.DocTypeId == DocumentTypeId
                    select new PropertyHeaderViewModel
                    {
                        DocTypeName = dt.DocumentTypeName,
                        PersonID = p.PersonID,
                        Code = p.Code,
                        Name = p.Name,
                        FatherName = PersonExtendedTab.FatherName,
                        GodownName = PersonExtendedTab.Godown.GodownName,
                        BinLocationName = PersonExtendedTab.BinLocation.BinLocationName,
                        HouseNo = PersonExtendedTab.HouseNo,
                        AreaName = PersonExtendedTab.Area.AreaName,
                        AadharNo = PersonExtendedTab.AadharNo,
                        OldHouseNo = PersonExtendedTab.OldHouseNo,
                        Status = p.Status ?? 0,
                        ModifiedBy = p.ModifiedBy,
                        ReviewCount = p.ReviewCount,
                        ReviewBy = p.ReviewBy,
                        Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
                    });
        }


        //public IQueryable<PropertyHeaderViewModel> GetPropertyHeaderList(int DocumentTypeId, int GodownId, string Uname)
        //{

        //    var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
        //    var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
        //    List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

        //    return (from p in _PropertyRepository.Instance
        //            join dt in _unitOfWork.Repository<DocumentType>().Instance on p.DocTypeId equals dt.DocumentTypeId
        //            join pe in _unitOfWork.Repository<PersonExtended>().Instance on p.PersonID equals pe.PersonId into PersonExtendedTable from PersonExtendedTab in PersonExtendedTable.DefaultIfEmpty()
        //            orderby PersonExtendedTab.HouseNo
        //            where p.DocTypeId == DocumentTypeId  && PersonExtendedTab.GodownId == GodownId
        //            select new PropertyHeaderViewModel
        //            {
        //                DocTypeName = dt.DocumentTypeName,
        //                PersonID = p.PersonID,
        //                Code = p.Code,
        //                Name = p.Name,
        //                FatherName = PersonExtendedTab.FatherName,
        //                GodownName = PersonExtendedTab.Godown.GodownName,
        //                BinLocationName = PersonExtendedTab.BinLocation.BinLocationName,
        //                HouseNo = PersonExtendedTab.HouseNo,
        //                AreaName = PersonExtendedTab.Area.AreaName,
        //                AadharNo = PersonExtendedTab.AadharNo,
        //                OldHouseNo = PersonExtendedTab.OldHouseNo,
        //                Status = p.Status ?? 0,
        //                ModifiedBy = p.ModifiedBy,
        //                ReviewCount = p.ReviewCount,
        //                ReviewBy = p.ReviewBy,
        //                Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
        //            });
        //}

        public IEnumerable<PropertyHeaderViewModel> GetPropertyHeaderList(int DocumentTypeId, int GodownId, string Uname)
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            var PropertyList_Temp = (from p in _PropertyRepository.Instance
                    join dt in _unitOfWork.Repository<DocumentType>().Instance on p.DocTypeId equals dt.DocumentTypeId
                    join pe in _unitOfWork.Repository<PersonExtended>().Instance on p.PersonID equals pe.PersonId into PersonExtendedTable
                    from PersonExtendedTab in PersonExtendedTable.DefaultIfEmpty()
                    orderby PersonExtendedTab.HouseNo
                    where p.DocTypeId == DocumentTypeId && PersonExtendedTab.GodownId == GodownId
                    select new PropertyHeaderViewModel
                    {
                        DocTypeName = dt.DocumentTypeName,
                        PersonID = p.PersonID,
                        Code = p.Code,
                        Name = p.Name,
                        FatherName = PersonExtendedTab.FatherName,
                        GodownName = PersonExtendedTab.Godown.GodownName,
                        BinLocationName = PersonExtendedTab.BinLocation.BinLocationName,
                        HouseNo = PersonExtendedTab.HouseNo,
                        AreaName = PersonExtendedTab.Area.AreaName,
                        AadharNo = PersonExtendedTab.AadharNo,
                        OldHouseNo = PersonExtendedTab.OldHouseNo,
                        Status = p.Status ?? 0,
                        ModifiedBy = p.ModifiedBy,
                        ReviewCount = p.ReviewCount,
                        ReviewBy = p.ReviewBy,
                        Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
                    }).ToList();

            double x = 0;
            var PropertyList = PropertyList_Temp.OrderBy(sx => double.TryParse((sx.HouseNo ?? "").Replace("-C","").Replace("-","."), out x) ? x : 0);

            return PropertyList;
        }

        public PropertyHeaderViewModel GetPropertyHeader(int id)
        {
            return (from p in _PropertyRepository.Instance
                    join pe in _unitOfWork.Repository<PersonExtended>().Instance on p.PersonID equals pe.PersonId into PersonExtendedTable from PersonExtendedTab in PersonExtendedTable.DefaultIfEmpty()
                    join Be in _unitOfWork.Repository<BusinessEntity>().Instance on p.PersonID equals Be.PersonID into BusinessEntityTable
                    from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                    join L in _unitOfWork.Repository<LedgerAccount>().Instance on p.PersonID equals L.PersonId into LedgerAccountTable
                    from LedgerAccountTab in LedgerAccountTable.DefaultIfEmpty()
                    join pa in _unitOfWork.Repository<PersonAddress>().Instance  on p.PersonID equals pa.PersonId into PersonAddressTable 
                    from PersonAddressTab in PersonAddressTable.DefaultIfEmpty()
                    where p.PersonID == id
                    select new PropertyHeaderViewModel
                    {
                        PersonID = p.PersonID,
                        LedgerAccountId = LedgerAccountTab.LedgerAccountId,
                        DocTypeName = p.DocType.DocumentTypeName,
                        Code = p.Code,
                        Suffix = p.Suffix,
                        ParentId = BusinessEntityTab.ParentId,
                        GISId = PersonExtendedTab.GISId,
                        GodownId = PersonExtendedTab.GodownId,
                        BinLocationId = PersonExtendedTab.BinLocationId,
                        HouseNo = PersonExtendedTab.HouseNo,
                        OldHouseNo = PersonExtendedTab.OldHouseNo,
                        PersonAddressId = PersonAddressTab.PersonAddressID,
                        Address = PersonAddressTab.Address,
                        ZipCode = PersonAddressTab.Zipcode,
                        AreaId = PersonExtendedTab.AreaId,
                        Name = p.Name,
                        FatherName = PersonExtendedTab.FatherName,
                        AadharNo = PersonExtendedTab.AadharNo,
                        Mobile = p.Mobile,
                        Email = p.Email,
                        CasteId = PersonExtendedTab.CasteId,
                        ReligionId = PersonExtendedTab.ReligionId,
                        PersonRateGroupId = PersonExtendedTab.PersonRateGroupId,
                        TotalPropertyArea = PersonExtendedTab.TotalPropertyArea,
                        TotalTaxableArea = PersonExtendedTab.TotalTaxableArea,
                        TotalARV = PersonExtendedTab.TotalARV,
                        TotalTax = PersonExtendedTab.TotalTax,
                        Status = p.Status ?? 0,
                        SiteIds = BusinessEntityTab.SiteIds,
                        DivisionIds = BusinessEntityTab.DivisionIds,
                        DocTypeId = p.DocTypeId,
                        ModifiedBy = p.ModifiedBy,
                        CreatedDate = p.CreatedDate,
                    }).FirstOrDefault();
        }


        public IQueryable<PropertyHeaderViewModel> GetPropertyHeaderListPendingToSubmit(int id, int GodownId, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var Person = GetPropertyHeaderList(id, GodownId, Uname).AsQueryable();

            var PendingToSubmit = from p in Person
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Import || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }


        public IQueryable<PropertyHeaderViewModel> GetPropertyHeaderListPendingToReview(int id, int GodownId, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var Person = GetPropertyHeaderList(id, GodownId, Uname).AsQueryable();

            var PendingToReview = from p in Person
                                  where p.Status == (int)StatusConstants.Submitted && p.ReviewBy == null
                                  select p;
            return PendingToReview;

        }




        public DateTime AddDueDate(DateTime Base, int DueDays)
        {
            DateTime DueDate = Base.AddDays((double)DueDays);
            if (DueDate.DayOfWeek == DayOfWeek.Sunday)
                DueDate = DueDate.AddDays(1);

            return DueDate;
        }

        public PropertyHeaderViewModel Create(PropertyHeaderViewModel vmPropertyHeader, string UserName)
        {
            Person person = Mapper.Map<PropertyHeaderViewModel, Person>(vmPropertyHeader);
            PersonAddress personaddress = Mapper.Map<PropertyHeaderViewModel, PersonAddress>(vmPropertyHeader);
            BusinessEntity businessentity = Mapper.Map<PropertyHeaderViewModel, BusinessEntity>(vmPropertyHeader);
            LedgerAccount ledgeraccount = Mapper.Map<PropertyHeaderViewModel, LedgerAccount>(vmPropertyHeader);
            PersonExtended personextended = Mapper.Map<PropertyHeaderViewModel, PersonExtended>(vmPropertyHeader);
            DocumentType D = _DocumentTypeService.Find(person.DocTypeId);

            person.Suffix = vmPropertyHeader.Code;
            person.IsActive = true;
            person.CreatedDate = DateTime.Now;
            person.ModifiedDate = DateTime.Now;
            person.CreatedBy = UserName;
            person.ModifiedBy = UserName;
            person.Status = (int)StatusConstants.Drafted;
            person.ObjectState = Model.ObjectState.Added;
            Create(person);


            personaddress.CreatedDate = DateTime.Now;
            personaddress.ModifiedDate = DateTime.Now;
            personaddress.CreatedBy = UserName;
            personaddress.ModifiedBy = UserName;
            personaddress.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<PersonAddress>().Add(personaddress);


            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            string Divisions = vmPropertyHeader.DivisionIds;
            if (Divisions != null)
            {
                Divisions = "|" + Divisions.Replace(",", "|,|") + "|";
            }
            else
            {
                Divisions = "|" + CurrentDivisionId.ToString() + "|";
            }

            businessentity.DivisionIds = Divisions;

            string Sites = vmPropertyHeader.SiteIds;
            if (Sites != null)
            {
                Sites = "|" + Sites.Replace(",", "|,|") + "|";
            }
            else
            {
                Sites = "|" + CurrentSiteId.ToString() + "|";
            }

            businessentity.SiteIds = Sites;
            businessentity.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<BusinessEntity>().Add(businessentity);


            LedgerAccountGroup ledgeraccountgroup = (from G in _unitOfWork.Repository<LedgerAccountGroup>().Instance
                                                     where G.LedgerAccountGroupName == "Sundry Debtors"
                                                     select G).FirstOrDefault();


            
            ledgeraccount.LedgerAccountName = person.Name;
            ledgeraccount.LedgerAccountSuffix = person.Suffix;
            ledgeraccount.LedgerAccountGroupId = ledgeraccountgroup.LedgerAccountGroupId;
            ledgeraccount.IsActive = true;
            ledgeraccount.CreatedDate = DateTime.Now;
            ledgeraccount.ModifiedDate = DateTime.Now;
            ledgeraccount.CreatedBy = UserName;
            ledgeraccount.ModifiedBy = UserName;
            ledgeraccount.ObjectState = Model.ObjectState.Added;
            _unitOfWork.Repository<LedgerAccount>().Add(ledgeraccount);


            personextended.ObjectState = Model.ObjectState.Added;
            _unitOfWork.Repository<PersonExtended>().Add(personextended);


            if (vmPropertyHeader.DocumentTypeAttributes != null)
            {
                foreach (var pta in vmPropertyHeader.DocumentTypeAttributes)
                {

                    PersonAttributes PersonAttribute = (from A in _unitOfWork.Repository<PersonAttributes>().Instance
                                                         where A.PersonId == person.PersonID && A.DocumentTypeAttributeId == pta.DocumentTypeAttributeId
                                                         select A).FirstOrDefault();

                    if (PersonAttribute != null)
                    {
                        PersonAttribute.PersonAttributeValue = pta.DefaultValue;
                        PersonAttribute.ObjectState = Model.ObjectState.Modified;
                        _unitOfWork.Repository<PersonAttributes>().Add(PersonAttribute);
                    }
                    else
                    {
                        PersonAttributes pa = new PersonAttributes()
                        {
                            PersonAttributeValue = pta.DefaultValue,
                            PersonId = person.PersonID,
                            DocumentTypeAttributeId = pta.DocumentTypeAttributeId,
                            CreatedBy = UserName,
                            ModifiedBy = UserName,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now

                        };
                        pa.ObjectState = Model.ObjectState.Added;
                        _unitOfWork.Repository<PersonAttributes>().Add(pa);
                    }
                }
            }


            //End Line Save


            _unitOfWork.Save();

            vmPropertyHeader.PersonID = person.PersonID;

            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = person.DocTypeId,
                DocId = person.PersonID,
                ActivityType = (int)ActivityTypeContants.Added,
                DocNo = person.Code,
                DocDate = person.CreatedDate,
                DocStatus = person.Status ?? 0,
            }));



            return vmPropertyHeader;
        }


        public void Update(PropertyHeaderViewModel vmPropertyHeader, string UserName)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            Person person = Find(vmPropertyHeader.PersonID);
            PersonAddress personaddress = _unitOfWork.Repository<PersonAddress>().Find(vmPropertyHeader.PersonAddressId);
            PersonExtended personextended = _unitOfWork.Repository<PersonExtended>().Find(vmPropertyHeader.PersonID);
            BusinessEntity businessentity = _unitOfWork.Repository<BusinessEntity>().Find(vmPropertyHeader.PersonID);
            LedgerAccount ledgeraccount = _unitOfWork.Repository<LedgerAccount>().Find(vmPropertyHeader.LedgerAccountId); 

            Person ExRec = Mapper.Map<Person>(person);

            DocumentType D = _DocumentTypeService.Find(person.DocTypeId);

            int status = person.Status ?? 0;

            if (person.Status != (int)StatusConstants.Drafted && person.Status != (int)StatusConstants.Import)
                person.Status = (int)StatusConstants.Modified;


            person.Code = vmPropertyHeader.Code;
            person.Suffix = vmPropertyHeader.Suffix;
            person.Name = vmPropertyHeader.Name;
            person.Mobile = vmPropertyHeader.Mobile;
            person.Email = vmPropertyHeader.Email;
            person.ModifiedDate = DateTime.Now;
            person.ModifiedBy = UserName;
            person.ObjectState = Model.ObjectState.Modified;
            Update(person);


            personaddress.Address = vmPropertyHeader.Address;
            personaddress.Zipcode = vmPropertyHeader.ZipCode;
            personaddress.ModifiedDate = DateTime.Now;
            personaddress.ModifiedBy = UserName;
            personaddress.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<PersonAddress>().Add(personaddress);


            personextended.AadharNo = vmPropertyHeader.AadharNo;
            personextended.GISId = vmPropertyHeader.GISId;
            personextended.GodownId = vmPropertyHeader.GodownId;
            personextended.BinLocationId = vmPropertyHeader.BinLocationId;
            personextended.HouseNo = vmPropertyHeader.HouseNo;
            personextended.AreaId = vmPropertyHeader.AreaId;
            personextended.FatherName = vmPropertyHeader.FatherName;
            personextended.CasteId = vmPropertyHeader.CasteId;
            personextended.ReligionId = vmPropertyHeader.ReligionId;
            personextended.PersonRateGroupId = vmPropertyHeader.PersonRateGroupId;
            personextended.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<PersonExtended>().Add(personextended);


            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            string Divisions = vmPropertyHeader.DivisionIds;
            if (Divisions != null)
            {
                Divisions = "|" + Divisions.Replace(",", "|,|") + "|";
            }
            else
            {
                Divisions = "|" + CurrentDivisionId.ToString() + "|";
            }

            businessentity.DivisionIds = Divisions;

            string Sites = vmPropertyHeader.SiteIds;
            if (Sites != null)
            {
                Sites = "|" + Sites.Replace(",", "|,|") + "|";
            }
            else
            {
                Sites = "|" + CurrentSiteId.ToString() + "|";
            }

            businessentity.SiteIds = Sites;
            businessentity.ParentId = vmPropertyHeader.ParentId;
            businessentity.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<BusinessEntity>().Add(businessentity);


            ledgeraccount.LedgerAccountName = person.Name;
            ledgeraccount.LedgerAccountSuffix = person.Suffix;
            ledgeraccount.ModifiedDate = DateTime.Now;
            ledgeraccount.ModifiedBy = UserName;
            ledgeraccount.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<LedgerAccount>().Add(ledgeraccount);


            if (vmPropertyHeader.DocumentTypeAttributes != null)
            {
                foreach (var pta in vmPropertyHeader.DocumentTypeAttributes)
                {

                    PersonAttributes PersonAttribute = (from A in _unitOfWork.Repository<PersonAttributes>().Instance
                                                        where A.PersonId == person.PersonID && A.DocumentTypeAttributeId == pta.DocumentTypeAttributeId
                                                        select A).FirstOrDefault();

                    if (PersonAttribute != null)
                    {
                        PersonAttribute.PersonAttributeValue = pta.DefaultValue;
                        PersonAttribute.ObjectState = Model.ObjectState.Modified;
                        _unitOfWork.Repository<PersonAttributes>().Add(PersonAttribute);
                    }
                    else
                    {
                        PersonAttributes pa = new PersonAttributes()
                        {
                            PersonAttributeValue = pta.DefaultValue,
                            PersonId = person.PersonID,
                            DocumentTypeAttributeId = pta.DocumentTypeAttributeId,
                            CreatedBy = UserName,
                            ModifiedBy = UserName,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now

                        };
                        pa.ObjectState = Model.ObjectState.Added;
                        _unitOfWork.Repository<PersonAttributes>().Add(pa);
                    }
                }
            }



            LogList.Add(new LogTypeViewModel
            {
                ExObj = ExRec,
                Obj = person,
            });



            XElement Modifications = _modificationCheck.CheckChanges(LogList);

            _unitOfWork.Save();

            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = person.DocTypeId,
                DocId = person.PersonID,
                ActivityType = (int)ActivityTypeContants.Modified,
                DocNo = person.Code,
                xEModifications = Modifications,
                DocDate = person.CreatedDate,
                DocStatus = person.Status ?? 0,
            }));

        }

        public void Delete(ReasonViewModel vm, string UserName)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            var Person = Find(vm.id);


            LogList.Add(new LogTypeViewModel
            {
                ExObj = Mapper.Map<Person>(Person),
            });

            var PersonAddress = _unitOfWork.Repository<PersonAddress>().Query().Get().Where(m => m.PersonId == Person.PersonID).FirstOrDefault();
            var PersonExtended = _unitOfWork.Repository<PersonExtended>().Query().Get().Where(m => m.PersonId == Person.PersonID).FirstOrDefault();
            var BusinessEntity = _unitOfWork.Repository<BusinessEntity>().Query().Get().Where(m => m.PersonID == Person.PersonID).FirstOrDefault();
            var ledgeraccount = _unitOfWork.Repository<LedgerAccount>().Query().Get().Where(m => m.PersonId == Person.PersonID).FirstOrDefault();
            var ProductBuyer = _unitOfWork.Repository<ProductBuyer>().Query().Get().Where(m => m.BuyerId == Person.PersonID).ToList();

            var PersonAttribute = _unitOfWork.Repository<PersonAttributes>().Query().Get().Where(m => m.PersonId == Person.PersonID).ToList();




            foreach (var item in ProductBuyer)
            {
                List<ProductBuyerLog> productbuyerloglist = (from p in _unitOfWork.Repository<ProductBuyerLog>().Instance
                                                             where p.ProductBuyerId == item.ProductBuyerId
                                                             select p).ToList();
                if (productbuyerloglist != null)
                {
                    foreach (var productbuyerlogitem in productbuyerloglist)
                    {
                        ProductBuyerLog productbuyerlog = _unitOfWork.Repository<ProductBuyerLog>().Find(productbuyerlogitem.ProductBuyerLogId);
                        productbuyerlog.ObjectState = Model.ObjectState.Deleted;
                        _unitOfWork.Repository<ProductBuyerLog>().Delete(productbuyerlogitem);
                    }
                }


                var ProductBuyerExtended = _unitOfWork.Repository<ProductBuyerExtended>().Query().Get().Where(m => m.ProductBuyerId == item.ProductBuyerId).FirstOrDefault();
                ProductBuyerExtended.ObjectState = Model.ObjectState.Deleted;
                _unitOfWork.Repository<ProductBuyerExtended>().Delete(ProductBuyerExtended);

                var ProductBuyerLogList = _unitOfWork.Repository<ProductBuyerLog>().Query().Get().Where(m => m.ProductBuyerId == item.ProductBuyerId).ToList();
                foreach (var ProductBuyerLog in ProductBuyerLogList)
                {
                    ProductBuyerLog.ObjectState = Model.ObjectState.Deleted;
                    _unitOfWork.Repository<ProductBuyerLog>().Delete(ProductBuyerLog);
                }

                item.ObjectState = Model.ObjectState.Deleted;
                _unitOfWork.Repository<ProductBuyer>().Delete(item);
            }

            foreach (var item in PersonAttribute)
            {
                item.ObjectState = Model.ObjectState.Deleted;
                _unitOfWork.Repository<PersonAttributes>().Delete(item);
            }

            BusinessEntity.ObjectState = Model.ObjectState.Deleted;
            _unitOfWork.Repository<BusinessEntity>().Delete(BusinessEntity);

            ledgeraccount.ObjectState = Model.ObjectState.Deleted;
            _unitOfWork.Repository<LedgerAccount>().Delete(ledgeraccount);

            PersonExtended.ObjectState = Model.ObjectState.Deleted;
            _unitOfWork.Repository<PersonExtended>().Delete(PersonExtended);

            PersonAddress.ObjectState = Model.ObjectState.Deleted;
            _unitOfWork.Repository<PersonAddress>().Delete(PersonAddress);

            Person.ObjectState = Model.ObjectState.Deleted;
            _unitOfWork.Repository<Person>().Delete(Person);


            XElement Modifications = _modificationCheck.CheckChanges(LogList);

            _unitOfWork.Save();


            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = Person.DocTypeId,
                DocId = Person.PersonID,
                ActivityType = (int)ActivityTypeContants.Deleted,
                UserRemark = vm.Reason,
                DocNo = Person.Code,
                xEModifications = Modifications,
                DocDate = Person.CreatedDate,
                DocStatus = Person.Status ?? 0,
            }));

        }


        public void Submit(int Id, string UserName, string GenGatePass, string UserRemark)
        {
            var pd = _PropertyRepository.Find(Id);

            pd.Status = (int)StatusConstants.Submitted;
            int ActivityType = (int)ActivityTypeContants.Submitted;


            pd.ReviewBy = null;
            pd.ObjectState = Model.ObjectState.Modified;
            _PropertyRepository.Update(pd);


            PropertyHeaderViewModel vmPropertyHeader = GetPropertyHeader(Id);


            if (vmPropertyHeader != null)
            {
                SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", vmPropertyHeader.PersonID);
                var s = _unitOfWork.SqlQuery<object>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_CalculatePropertyTax @PersonId", SqlParameterPersonId).ToList();
            }


            var temp = (from L in _unitOfWork.Repository<Ledger>().Instance where L.LedgerAccountId == vmPropertyHeader.LedgerAccountId select L).FirstOrDefault();

            if (temp == null)
            {
                SqlParameter SqlParameterSite = new SqlParameter("@Site", vmPropertyHeader.SiteIds);
                SqlParameter SqlParameterDivision = new SqlParameter("@Division", vmPropertyHeader.DivisionIds);
                SqlParameter SqlParameterIsFromWEFDate = new SqlParameter("@IsFromWEFDate", 1);
                SqlParameter SqlParameterUserName = new SqlParameter("@UserName", UserName);
                SqlParameter SqlParameterBusinessSessionId = new SqlParameter("@BusinessSessionId", "0");
                SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", vmPropertyHeader.PersonID);
                _unitOfWork.SqlQuery<string>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_PropertyTaxDue     @Site, @Division,	@IsFromWEFDate,@UserName,@BusinessSessionId, @PersonId", SqlParameterSite, SqlParameterDivision, SqlParameterIsFromWEFDate, SqlParameterUserName, SqlParameterBusinessSessionId, SqlParameterPersonId).ToList();
            }


            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = pd.DocTypeId,
                DocId = pd.PersonID,
                ActivityType = ActivityType,
                UserRemark = UserRemark,
                DocNo = pd.Code,
                DocDate = pd.CreatedDate,
                DocStatus = pd.Status ?? 0,
            }));


        }

        public void Review(int Id, string UserName, string UserRemark)
        {
            var pd = Find(Id);

            pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
            pd.ReviewBy += UserName + ", ";
            pd.ObjectState = Model.ObjectState.Modified;

            Update(pd);

            _unitOfWork.Save();

            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = pd.DocTypeId,
                DocId = pd.PersonID,
                ActivityType = (int)ActivityTypeContants.Reviewed,
                UserRemark = UserRemark,
                DocNo = pd.Code,
                DocDate = pd.CreatedDate,
                DocStatus = pd.Status ?? 0,
            }));

        }

        public int NextPrevId(int DocId, int DocTypeId, string UserName, string PrevNext)
        {
            return GetNextPrevId(DocId, DocTypeId, UserName, "", "Web.BusinessEntities", "PersonId", PrevNext);
        }

        public int GetNextPrevId(int DocId, int DocTypeId, string UserName, string ForAction, string HeaderTableName, string HeaderTablePK, string NextPrev)
        {

            SqlParameter SqlParameterUserName = new SqlParameter("@UserName", UserName);
            SqlParameter SqlParameterForAction = new SqlParameter("@ForAction", ForAction);
            SqlParameter SqlParameterHeaderTableName = new SqlParameter("@HeaderTableName", HeaderTableName);
            SqlParameter SqlParameterHeaderTablePkFieldName = new SqlParameter("@HeaderTablePkFieldName", HeaderTablePK);
            SqlParameter SqlParameterDocId = new SqlParameter("@DocId", DocId);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", DocTypeId);
            SqlParameter SqlParameterNextPrevious = new SqlParameter("@NextPrevious", NextPrev);

            int Id = _unitOfWork.SqlQuery<int>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spGetNextPreviousIdForPerson @UserName, @ForAction, @HeaderTableName, @HeaderTablePkFieldName, @DocId, @DocTypeId, @NextPrevious", SqlParameterUserName, SqlParameterForAction, SqlParameterHeaderTableName, SqlParameterHeaderTablePkFieldName, SqlParameterDocId, SqlParameterDocTypeId, SqlParameterNextPrevious).FirstOrDefault();

            return Id;
        }

        //public byte[] GetReport(string Ids, int DocTypeId, string UserName, string SqlProc)
        //{

        //    int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
        //    int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

        //    //var Settings = new PersonSettingService(_unitOfWork).GetJobOrderSettingsForDocument(DocTypeId, DivisionId, SiteId);

        //    string DocumentPrint = "";
        //    //string SqlProcDocumentPrint = "Web.spRep_PropertyPrint";
        //    //string SqlProcDocumentPrint_AfterSubmit = "Web.spRep_PropertyPrint";
        //    //string SqlProcDocumentPrint_AfterApprove = "Web.spRep_PropertyPrint";

        //    //string SqlProcDocumentPrint = "Web.spRep_InvoicePrint";
        //    //string SqlProcDocumentPrint_AfterSubmit = "Web.spRep_InvoicePrint";
        //    //string SqlProcDocumentPrint_AfterApprove = "Web.spRep_InvoicePrint";

        //    string SqlProcDocumentPrint = SqlProc;
        //    string SqlProcDocumentPrint_AfterSubmit = SqlProc;
        //    string SqlProcDocumentPrint_AfterApprove = SqlProc;

        //    string ReportSql = "";

        //    if (!string.IsNullOrEmpty(DocumentPrint))
        //        ReportSql = GetReportHeader(DocumentPrint).ReportSQL;


        //    List<byte[]> PdfStream = new List<byte[]>();
        //    foreach (var item in Ids.Split(',').Select(Int32.Parse))
        //    {

        //        DirectReportPrint drp = new DirectReportPrint();

        //        var pd = Find(item);

        //        byte[] Pdf;

        //        if (!string.IsNullOrEmpty(ReportSql))
        //        {
        //            Pdf = drp.rsDirectDocumentPrint(ReportSql, UserName, item);
        //            PdfStream.Add(Pdf);
        //        }
        //        else
        //        {
        //            if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Import || pd.Status == (int)StatusConstants.Modified)
        //            {
        //                Pdf = drp.DirectDocumentPrint(SqlProcDocumentPrint, UserName, item);

        //                PdfStream.Add(Pdf);
        //            }
        //            else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
        //            {
        //                Pdf = drp.DirectDocumentPrint(SqlProcDocumentPrint_AfterSubmit, UserName, item);

        //                PdfStream.Add(Pdf);
        //            }
        //            else
        //            {
        //                Pdf = drp.DirectDocumentPrint(SqlProcDocumentPrint_AfterApprove, UserName, item);
        //                PdfStream.Add(Pdf);
        //            }
        //        }

        //        _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
        //        {
        //            DocTypeId = pd.DocTypeId,
        //            DocId = pd.PersonID,
        //            ActivityType = (int)ActivityTypeContants.Print,
        //            DocNo = pd.Code,
        //            DocDate = pd.CreatedDate,
        //            DocStatus = pd.Status ?? 0,
        //        }));

        //    }

        //    PdfMerger pm = new PdfMerger();

        //    byte[] Merge = pm.MergeFiles(PdfStream);

        //    return Merge;
        //}



        #region Helper Methods

        public void LogDetailInfo(PropertyHeaderViewModel vm)
        {
            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = vm.DocTypeId,
                DocId = vm.PersonID,
                ActivityType = (int)ActivityTypeContants.Detail,
                DocNo = vm.Code,
                DocDate = vm.CreatedDate,
                DocStatus = vm.Status,
            }));
        }

        public _Menu GetMenu(int Id)
        {
            return _unitOfWork.Repository<_Menu>().Find(Id);
        }

        public _Menu GetMenu(string Name)
        {
            return _unitOfWork.Repository<_Menu>().Query().Get().Where(m => m.MenuName == Name).FirstOrDefault();
        }

        public _ReportHeader GetReportHeader(string MenuName)
        {
            return _unitOfWork.Repository<_ReportHeader>().Query().Get().Where(m => m.ReportName == MenuName).FirstOrDefault();
        }
        public _ReportLine GetReportLine(string Name, int ReportHeaderId)
        {
            return _unitOfWork.Repository<_ReportLine>().Query().Get().Where(m => m.ReportHeaderId == ReportHeaderId && m.FieldName == Name).FirstOrDefault();
        }

        public bool CheckForDocNoExists(string docno, int DocTypeId)
        {
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var temp = (from pr in _PropertyRepository.Instance
                        where pr.Code == docno && (pr.DocTypeId == DocTypeId)
                        select pr).FirstOrDefault();
            if (temp == null)
                return false;
            else
                return true;

        }
        public bool CheckForDocNoExists(string docno, int headerid, int DocTypeId)
        {
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var temp = (from pr in _PropertyRepository.Instance
                        where pr.Code == docno && pr.PersonID != headerid && (pr.DocTypeId == DocTypeId)
                        select pr).FirstOrDefault();
            if (temp == null)
                return false;
            else
                return true;
        }

        #endregion

        public string FGetNewPersonCode(int SiteId, int GodownId, int? BinLocationId)
        {
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", SiteId);
            SqlParameter SqlParameterGodownId = new SqlParameter("@GodownId", GodownId);
            //SqlParameter SqlParameterBinLocationId = new SqlParameter("@BinLocationId", BinLocationId);

            NewDocNoViewModel NewDocNoViewModel = _unitOfWork.SqlQuery<NewDocNoViewModel>("" + System.Configuration.ConfigurationManager.AppSettings["DataBaseSchema"] + ".GetNewPersonCode @SiteId, @GodownId ", SqlParameterSiteId, SqlParameterGodownId).FirstOrDefault();

            if (NewDocNoViewModel != null)
            {
                return NewDocNoViewModel.NewDocNo;
            }
            else
            {
                return null;
            }
        }


        public string FGetNewHouseNo(int SiteId, int GodownId, int BinLocationId)
        {
            SqlParameter SqlParameterSiteId = new SqlParameter("@SiteId", SiteId);
            SqlParameter SqlParameterGodownId = new SqlParameter("@GodownId", GodownId);
            SqlParameter SqlParameterBinLocationId = new SqlParameter("@BinLocationId", BinLocationId);

            NewDocNoViewModel NewDocNoViewModel = _unitOfWork.SqlQuery<NewDocNoViewModel>("" + System.Configuration.ConfigurationManager.AppSettings["DataBaseSchema"] + ".GetNewHouseNo @SiteId, @GodownId, @BinLocationId ", SqlParameterSiteId, SqlParameterGodownId, SqlParameterBinLocationId).FirstOrDefault();

            if (NewDocNoViewModel != null)
            {
                return NewDocNoViewModel.NewDocNo;
            }
            else
            {
                return null;
            }
        }


        public ComboBoxPagedResult GetPersonWithDocType(string searchTerm, int pageSize, int pageNum, int DocTypeId)
        {
            return GetListWithDocType(searchTerm, pageSize, pageNum, DocTypeId);
        }


        public ComboBoxPagedResult GetListWithDocType(string searchTerm, int pageSize, int pageNum, int DocTypeId)
        {

            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            string DivId = "|" + CurrentDivisionId.ToString() + "|";
            string SiteId = "|" + CurrentSiteId.ToString() + "|";

            var list = (from b in _unitOfWork.Repository<Person>().Instance 
                        join bus in _unitOfWork.Repository<BusinessEntity>().Instance on b.PersonID equals bus.PersonID into BusinessEntityTable
                        from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                        join p in _unitOfWork.Repository<Person>().Instance on b.PersonID equals p.PersonID into PersonTable
                        from PersonTab in PersonTable.DefaultIfEmpty()
                        join pe in _unitOfWork.Repository<PersonExtended>().Instance on b.PersonID equals pe.PersonId into PersonExtendedTable
                        from PersonExtendedTab in PersonExtendedTable.DefaultIfEmpty()
                        where b.DocTypeId == DocTypeId
                        && (string.IsNullOrEmpty(searchTerm) ? 1 == 1 : (PersonTab.Name.ToLower().Contains(searchTerm.ToLower()) || PersonTab.Code.ToLower().Contains(searchTerm.ToLower())))
                        && BusinessEntityTab.DivisionIds.IndexOf(DivId) != -1
                        && (PersonTab.IsActive == null ? 1 == 1 : PersonTab.IsActive == true)
                        && BusinessEntityTab.SiteIds.IndexOf(SiteId) != -1
                        orderby PersonTab.Name
                        select new ComboBoxResult
                        {
                            id = b.PersonID.ToString(),
                            text = PersonTab.Code,
                            AProp1 = "Owner : " + PersonTab.Name,
                            AProp2 = "Father Name : " + PersonExtendedTab.FatherName,
                            TextProp1 = "Ward : " + PersonExtendedTab.Godown.GodownName + ", Chak : " + PersonExtendedTab.BinLocation.BinLocationName,
                            TextProp2  = "House No : " + PersonExtendedTab.HouseNo + ", Area : " + PersonExtendedTab.Area.AreaName
                        }
              );

            var temp = list
               .Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();

            var count = list.Count();

            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;

            return Data;
        }



        public Decimal GetIntrestBalance(int PersonId)
        {
            SqlParameter SqlPersonId = new SqlParameter("@PersonId", PersonId);

            Outstanding Outstanding = _unitOfWork.SqlQuery<Outstanding>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_GetIntrestBalance @PersonId", SqlPersonId).FirstOrDefault();
            if (Outstanding  == null)
            {
                return 0;
            }
            else{
                return Outstanding.Amount ?? 0;
            }
        }

        public Decimal GetArearBalance(int PersonId)
        {
            SqlParameter SqlPersonId = new SqlParameter("@PersonId", PersonId);

            Outstanding Outstanding = _unitOfWork.SqlQuery<Outstanding>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_GetArearBalance @PersonId", SqlPersonId).FirstOrDefault();
            if (Outstanding == null)
            {
                return 0;
            }
            else
            {
                return Outstanding.Amount ?? 0;
            }
        }

        public Decimal GetExcessBalance(int PersonId)
        {
            SqlParameter SqlPersonId = new SqlParameter("@PersonId", PersonId);

            Outstanding Outstanding = _unitOfWork.SqlQuery<Outstanding>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_GetExcessBalance @PersonId", SqlPersonId).FirstOrDefault();
            if (Outstanding == null)
            {
                return 0;
            }
            else
            {
                return Outstanding.Amount ?? 0;
            }
        }

        public Decimal GetCurrentYearBalance(int PersonId)
        {
            SqlParameter SqlPersonId = new SqlParameter("@PersonId", PersonId);

            Outstanding Outstanding = _unitOfWork.SqlQuery<Outstanding>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_GetCurrentYearBalance @PersonId", SqlPersonId).FirstOrDefault();
            if (Outstanding == null)
            {
                return 0;
            }
            else
            {
                return Outstanding.Amount ?? 0;
            }
        }

        public Decimal GetNetOutstanding(int PersonId)
        {
            SqlParameter SqlPersonId = new SqlParameter("@PersonId", PersonId);

            Outstanding Outstanding = _unitOfWork.SqlQuery<Outstanding>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_GetNetOutstanding @PersonId", SqlPersonId).FirstOrDefault();
            if (Outstanding == null)
            {
                return 0;
            }
            else
            {
                return Outstanding.Amount ?? 0;
            }
        }

        public ComboBoxPagedResult GetProperty(string searchTerm, int pageSize, int pageNum)
        {
            int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            string DivId = "|" + CurrentDivisionId.ToString() + "|";
            string SiteId = "|" + CurrentSiteId.ToString() + "|";

            var list = (from P in _PropertyRepository.Instance
                        join Pe in _unitOfWork.Repository<PersonExtended>().Instance on P.PersonID equals Pe.PersonId into PersonExtendedTable
                        from PersonExtendedTab in PersonExtendedTable.DefaultIfEmpty()
                        join bus in _unitOfWork.Repository<BusinessEntity>().Instance on P.PersonID equals bus.PersonID into BusinessEntityTable
                        from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                        where P.DocTypeId == 1867
                        && (string.IsNullOrEmpty(searchTerm) ? 1 == 1 : (P.Name.ToLower().Contains(searchTerm.ToLower()) || P.Code.ToLower().Contains(searchTerm.ToLower())))
                        && BusinessEntityTab.DivisionIds.IndexOf(DivId) != -1
                        && (P.IsActive == null ? 1 == 1 : P.IsActive == true)
                        && BusinessEntityTab.SiteIds.IndexOf(SiteId) != -1
                        orderby P.Name
                        select new ComboBoxResult
                        {
                            id = P.PersonID.ToString(),
                            text = P.Code,
                            AProp1 = "Owner : " + P.Name + ", Father/Husband Name :" + PersonExtendedTab.FatherName,
                            AProp2 = "Aadhar No : " + PersonExtendedTab.AadharNo,
                            TextProp1 = "Mobile : " + P.Mobile,
                            TextProp2 = "EMail : " + P.Email
                        }
              );

            var temp = list
               .Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();

            var count = list.Count();

            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;

            return Data;

        }

        public IEnumerable<DocumentTypeAttributeViewModel> GetAttributeForDocumentType(int DocumentTypeId)
        {
            return (from p in _unitOfWork.Repository<DocumentTypeAttribute>().Instance
                    where p.DocumentTypeId == DocumentTypeId
                    select new DocumentTypeAttributeViewModel
                    {
                        DataType = p.DataType,
                        ListItem = p.ListItem,
                        DefaultValue = p.DefaultValue,
                        Name = p.Name,
                        DocumentTypeAttributeId = p.DocumentTypeAttributeId,
                        DocumentTypeId = p.DocumentTypeId,
                    });
        }

        public IEnumerable<DocumentTypeAttributeViewModel> GetAttributeForPerson(int id)
        {
            PropertyHeaderViewModel s = GetPropertyHeader(id);

            var temp = from p in _unitOfWork.Repository<DocumentTypeAttribute>().Instance
                       join t in _unitOfWork.Repository<PersonAttributes>().Instance on p.DocumentTypeAttributeId equals t.DocumentTypeAttributeId into table
                       from tab in table.Where(m => m.PersonId == id).DefaultIfEmpty()
                       where (p.DocumentTypeId == s.DocTypeId)
                       select new DocumentTypeAttributeViewModel
                       {
                           ListItem = p.ListItem,
                           DataType = p.DataType,
                           DefaultValue = tab.PersonAttributeValue,
                           Name = p.Name,
                           DocumentTypeAttributeId = p.DocumentTypeAttributeId,
                           DocumentAttributeId = (int?)tab.PersonAttributeId ?? 0
                       };

            return temp;
        }

        public IQueryable<WardIndexViewModel> GetGodownListForIndex(int SiteId)
        {
            //var pt = _unitOfWork.Repository<Godown>().Query().Get().OrderBy(m => m.GodownName).Where(m => m.SiteId == SiteId);

            var WardProperty = from L in _unitOfWork.Repository<PersonExtended>().Instance
                               group new { L } by new { L.GodownId } into Result
                               select new
                               {
                                   GodownId = Result.Key.GodownId,
                                   Count = Result.Count()
                               };

            var pt = from H in _unitOfWork.Repository<Godown>().Instance
                     join L in WardProperty on H.GodownId equals L.GodownId into WardPropertyTable
                     from WardPropertyTab in WardPropertyTable.DefaultIfEmpty()
                     orderby H.GodownName
                     select new WardIndexViewModel
                     {
                         GodownId = H.GodownId,
                         GodownCode = H.GodownCode,
                         GodownName = H.GodownName,
                         PropertyCount = WardPropertyTab.Count
                     };



            return pt;
        }



        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }

    public class GetValue
    {
        public Decimal Value { get; set; }
    }

    public class Outstanding
    {
        public Decimal? Amount { get; set; }
    }


}


