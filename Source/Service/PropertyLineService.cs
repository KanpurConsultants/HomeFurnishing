using System.Collections.Generic;
using System.Linq;
using System;
using Model;
using System.Data.SqlClient;
using System.Configuration;
using Components.Logging;
using AutoMapper;
using System.Xml.Linq;
using Model.Models;
using Model.ViewModels;
using Data.Infrastructure;
using Model.ViewModel;
using Model.PropertyTax.Models;
using Service;
using Core.Common;

namespace Services.PropertyTax
{
    public interface IPropertyLineService : IDisposable
    {
        ProductBuyer Create(ProductBuyer s);
        PropertyLineViewModel Create(PropertyLineViewModel svm, string UserName);
        void Delete(int id);
        void Delete(ProductBuyer s);
        void Delete(PropertyLineViewModel vm, string UserName);
        PropertyLineViewModel GetProductBuyer(int id);
        ProductBuyer Find(int id);
        void Update(ProductBuyer s);
        void Update(PropertyLineViewModel svm, string UserName);
        IQueryable<PropertyLineViewModel> GetProductBuyerListForIndex(int StockHeaderId);
        IEnumerable<PropertyLineViewModel> GetProductBuyerforDelete(int headerid);
        IEnumerable<ComboBoxList> GetProductHelpList(int Id, string term);
        IEnumerable<ProductBuyer> GetProductBuyerListForHeader(int HeaderId);
        bool IsDuplicateLine(int PersonId, int ProductId, int Dimension1Id, int ProductBuyerId);
        DiscountType GetDicountTypeDetail(int DiscountTypeId);
        Decimal FGetARV(int Ward, int RoadType, int ConstructionType, int PropertyType, int DiscountType,
            decimal CoveredArea, decimal BalconyArea, decimal GarageArea, bool IsRented, DateTime DateOfConstruction, DateTime WEF);

        #region Helper Methods
        ProductViewModel GetProduct(int ProductId);

        #endregion
    }

    public class PropertyLineService : IPropertyLineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Person> _PersonRepository;
        private readonly IRepository<ProductBuyer> _ProductBuyerRepository;
        private readonly IRepository<ProductBuyerExtended> _ProductBuyerExtendedRepository;
        private readonly IRepository<ProductBuyerLog> _ProductBuyerLogRepository;
        private readonly ILogger _logger;
        private readonly IModificationCheck _modificationCheck;
        //private readonly IJobReceiveSettingsService _jobreceiveSettingsService;

        ActiivtyLogViewModel LogVm = new ActiivtyLogViewModel();

        public PropertyLineService(IUnitOfWork unitOfWork, IRepository<ProductBuyer> ProductBuyerRepo, IRepository<ProductBuyerExtended> ProductBuyerExtendedRepo,
            IRepository<ProductBuyerLog> ProductBuyerLogRepo,
            IRepository<Person> PersonRepo, 
             ILogger log,
            IModificationCheck modificationCheck
            //, IJobReceiveSettingsService JobReceiveSettingsServ
            )
        {
            _unitOfWork = unitOfWork;
            _PersonRepository = PersonRepo;
            _ProductBuyerRepository = ProductBuyerRepo;
            _ProductBuyerExtendedRepository = ProductBuyerExtendedRepo;
            _ProductBuyerLogRepository = ProductBuyerLogRepo;
            _modificationCheck = modificationCheck;
            _logger = log;
            // _jobreceiveSettingsService = JobReceiveSettingsServ;

            //Log Initialization
            LogVm.SessionId = 0;
            LogVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            LogVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            LogVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
        }      

        public ProductBuyer Create(ProductBuyer S)
        {
            S.ObjectState = ObjectState.Added;
            _ProductBuyerRepository.Add(S);
            return S;
        }

        public void Delete(int id)
        {
            _ProductBuyerRepository.Delete(id);
        }

        public void Delete(ProductBuyer s)
        {
            _ProductBuyerRepository.Delete(s);
        }

        public void Update(ProductBuyer s)
        {
            s.ObjectState = ObjectState.Modified;
            _ProductBuyerRepository.Update(s);
        }

        public ProductViewModel GetProduct(int ProductId)
        {
            return new ProductService(_unitOfWork).GetProduct(ProductId);
        }


        public PropertyLineViewModel GetProductBuyer(int id)
        {
            var temp = (from p in _ProductBuyerRepository.Instance
                        join Se in _unitOfWork.Repository<ProductBuyerExtended>().Instance on p.ProductBuyerId equals Se.ProductBuyerId into ProductBuyerExtendedTable
                        from ProductBuyerExtendedTab in ProductBuyerExtendedTable.DefaultIfEmpty()
                        where p.ProductBuyerId == id
                        select new PropertyLineViewModel
                        {
                            ProductBuyerId = p.ProductBuyerId,
                            PersonId = p.BuyerId,
                            DateOfConsutruction = ProductBuyerExtendedTab.DateOfConsutruction,
                            WEF = ProductBuyerExtendedTab.WEF,
                            ProductId = p.ProductId,
                            Dimension1Id = (int)p.Dimension1Id,
                            DiscountTypeId = ProductBuyerExtendedTab.DiscountTypeId,
                            PropertyArea = ProductBuyerExtendedTab.PropertyArea,
                            TaxableArea = ProductBuyerExtendedTab.TaxableArea,
                            ARV = ProductBuyerExtendedTab.ARV,
                            TenantName = ProductBuyerExtendedTab.TenantName,
                            BillingType = ProductBuyerExtendedTab.BillingType,
                            Description = ProductBuyerExtendedTab.Description,
                            CoveredArea = ProductBuyerExtendedTab.CoveredArea,
                            GarageArea = ProductBuyerExtendedTab.GarageArea,
                            BalconyArea = ProductBuyerExtendedTab.BalconyArea,
                            IsRented = ProductBuyerExtendedTab.IsRented ?? false,
                            TaxPercentage = ProductBuyerExtendedTab.TaxPercentage,
                            TaxAmount = ProductBuyerExtendedTab.TaxAmount,
                            WaterTaxPercentage = ProductBuyerExtendedTab.WaterTaxPercentage,
                            WaterTaxAmount = ProductBuyerExtendedTab.WaterTaxAmount,
                            OldARV = ProductBuyerExtendedTab.ARV,
                        }).FirstOrDefault();

            //var ProductBuyerLog = (from p in _ProductBuyerLogRepository.Instance
            //                       where p.ProductBuyerId == id
            //                       orderby p.ProductBuyerLogId descending
            //                      select p).FirstOrDefault();

            //if (ProductBuyerLog != null)
            //{
            //    temp.NewWEF = ProductBuyerLog.NewWEF;
            //    temp.ModifyRemark = ProductBuyerLog.ModifyRemark;
            //}

            return temp;
        }
        public ProductBuyer Find(int id)
        {
            return _ProductBuyerRepository.Find(id);
        }



        public IQueryable<PropertyLineViewModel> GetProductBuyerListForIndex(int PersonId)
        {
            var temp = from p in _ProductBuyerRepository.Instance
                       join Se in _unitOfWork.Repository<ProductBuyerExtended>().Instance on p.ProductBuyerId equals Se.ProductBuyerId into ProductBuyerExtendedTable
                       from ProductBuyerExtendedTab in ProductBuyerExtendedTable.DefaultIfEmpty()
                       where p.BuyerId == PersonId
                       select new PropertyLineViewModel
                       {
                           ProductBuyerId = p.ProductBuyerId,
                           DateOfConsutruction = ProductBuyerExtendedTab.DateOfConsutruction,
                           ProductName = p.Product.ProductName,
                           Dimension1Name = p.Dimension1.Dimension1Name,
                           DiscountTypeName = ProductBuyerExtendedTab.DiscountType.DiscountTypeName,
                           PropertyArea = ProductBuyerExtendedTab.PropertyArea,
                           TaxableArea = ProductBuyerExtendedTab.TaxableArea,
                           ARV = ProductBuyerExtendedTab.ARV,
                           TenantName = ProductBuyerExtendedTab.TenantName,
                       };
            return temp;
        }

        public IEnumerable<PropertyLineViewModel> GetProductBuyerforDelete(int PersonId)
        {
            return (from p in _ProductBuyerRepository.Instance
                    where p.BuyerId == PersonId
                    select new PropertyLineViewModel
                    {
                        ProductBuyerId = p.ProductBuyerId,
                    });
        }


        
        public IEnumerable<ComboBoxList> GetProductHelpList(int Id, string term)
        {


            var list = (from p in _unitOfWork.Repository<Product>().Instance
                        where (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductName.ToLower().Contains(term.ToLower()))
                        group new { p } by p.ProductId into g
                        select new ComboBoxList
                        {
                            PropFirst = g.Max(m => m.p.ProductName),
                            Id = g.Key,
                        }
                          ).Take(20);

            return list.ToList();
        }



        public PropertyLineViewModel Create(PropertyLineViewModel svm, string UserName)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            ProductBuyer productbuyer = Mapper.Map<PropertyLineViewModel, ProductBuyer>(svm);
            ProductBuyerExtended productbuyerextended = Mapper.Map<PropertyLineViewModel, ProductBuyerExtended>(svm);


            Person temp = _unitOfWork.Repository<Person>().Find(svm.PersonId);

            productbuyer.BuyerId = svm.PersonId;
            productbuyer.CreatedDate = DateTime.Now;
            productbuyer.ModifiedDate = DateTime.Now;
            productbuyer.CreatedBy = UserName;
            productbuyer.ModifiedBy = UserName;
            productbuyer.ObjectState = Model.ObjectState.Added;

            
            Create(productbuyer);



            productbuyerextended.ObjectState = Model.ObjectState.Added;
            _ProductBuyerExtendedRepository.Add(productbuyerextended);


            PersonExtended personextended = _unitOfWork.Repository<PersonExtended>().Find(svm.PersonId);
            personextended.TotalPropertyArea = (personextended.TotalPropertyArea ?? 0) + (productbuyerextended.PropertyArea ?? 0);
            personextended.TotalTaxableArea = (personextended.TotalTaxableArea ?? 0) + (productbuyerextended.TaxableArea ?? 0);
            personextended.TotalARV = (personextended.TotalARV ?? 0) + (productbuyerextended.ARV ?? 0);
            personextended.ObjectState = Model.ObjectState.Modified;
            _unitOfWork.Repository<PersonExtended>().Add(personextended);

            

            _unitOfWork.Save();

            if (svm != null)
            {
                SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", svm.PersonId);
                _unitOfWork.SqlQuery<PropertyLineViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_CalculatePropertyTax @PersonId", SqlParameterPersonId).ToList();
            }

            svm.ProductBuyerId = productbuyer.ProductBuyerId;

            _logger.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = temp.DocTypeId,
                DocId = temp.PersonID,
                DocLineId = productbuyer.ProductBuyerId,
                ActivityType = (int)ActivityTypeContants.Added,
                DocNo = temp.Code,
                DocDate = temp.CreatedDate,
                DocStatus = temp.Status ?? 0,
            }));

            return svm;

        }

        public void Update(PropertyLineViewModel svm, string UserName)
        {
            ProductBuyer s = Mapper.Map<PropertyLineViewModel, ProductBuyer>(svm);

            Person temp = _unitOfWork.Repository<Person>().Find(svm.PersonId);

            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            ProductBuyer productbuyer = Find(s.ProductBuyerId);
            ProductBuyerExtended productbuyerextended = _unitOfWork.Repository<ProductBuyerExtended>().Find(s.ProductBuyerId);

            ProductBuyer ExTempLine = new ProductBuyer();
            ExTempLine = Mapper.Map<ProductBuyer>(productbuyer);

            if (svm.ARV != svm.OldARV)
            {
                //ProductBuyerLog productbuyerlog = Mapper.Map<ProductBuyerExtended, ProductBuyerLog>(productbuyerextended);
                ProductBuyerLog productbuyerlog = new ProductBuyerLog();


                productbuyerlog.ProductBuyerId = productbuyer.ProductBuyerId;
                productbuyerlog.ProductId = productbuyer.ProductId;
                productbuyerlog.BuyerId = productbuyer.BuyerId;
                productbuyerlog.BuyerSku = productbuyer.BuyerSku;
                productbuyerlog.BuyerProductCode = productbuyer.BuyerProductCode;
                productbuyerlog.BuyerUpcCode = productbuyer.BuyerUpcCode;
                productbuyerlog.BuyerSpecification = productbuyer.BuyerSpecification;
                productbuyerlog.BuyerSpecification1 = productbuyer.BuyerSpecification1;
                productbuyerlog.BuyerSpecification2 = productbuyer.BuyerSpecification2;
                productbuyerlog.BuyerSpecification3 = productbuyer.BuyerSpecification3;
                productbuyerlog.BuyerSpecification4 = productbuyer.BuyerSpecification4;
                productbuyerlog.BuyerSpecification5 = productbuyer.BuyerSpecification5;
                productbuyerlog.BuyerSpecification6 = productbuyer.BuyerSpecification6;


                productbuyerlog.DateOfConsutruction = productbuyerextended.DateOfConsutruction;
                productbuyerlog.DiscountTypeId = productbuyerextended.DiscountTypeId;
                productbuyerlog.PropertyArea = productbuyerextended.PropertyArea;
                productbuyerlog.TaxableArea = productbuyerextended.TaxableArea;
                productbuyerlog.ARV = productbuyerextended.ARV;
                productbuyerlog.TenantName = productbuyerextended.TenantName;
                productbuyerlog.BillingType = productbuyerextended.BillingType;
                productbuyerlog.Description = productbuyerextended.Description;
                productbuyerlog.CoveredArea = productbuyerextended.CoveredArea;
                productbuyerlog.GarageArea = productbuyerextended.GarageArea;
                productbuyerlog.BalconyArea = productbuyerextended.BalconyArea;
                productbuyerlog.IsRented = productbuyerextended.IsRented;
                productbuyerlog.TaxPercentage = productbuyerextended.TaxPercentage;
                productbuyerlog.TaxAmount = productbuyerextended.TaxAmount;
                productbuyerlog.WaterTaxPercentage = productbuyerextended.WaterTaxPercentage;
                productbuyerlog.WaterTaxAmount = productbuyerextended.WaterTaxAmount;
                productbuyerlog.WEF = svm.WEF;



                productbuyerlog.ModifyRemark = svm.ModifyRemark;
                productbuyerlog.CreatedBy = UserName;
                productbuyerlog.ModifiedBy = UserName;
                productbuyerlog.CreatedDate = DateTime.Now;
                productbuyerlog.ModifiedDate = DateTime.Now;
                productbuyerlog.ObjectState = Model.ObjectState.Added;
                _ProductBuyerLogRepository.Add(productbuyerlog);
            }

            
            productbuyer.ProductId = s.ProductId;
            productbuyer.ModifiedDate = DateTime.Now;
            productbuyer.ModifiedBy = UserName;
            productbuyer.ObjectState = Model.ObjectState.Modified;
            Update(productbuyer);


            Decimal XPropertyArea = productbuyerextended.PropertyArea ?? 0;
            Decimal XTaxableArea = productbuyerextended.TaxableArea ?? 0;
            Decimal XARV = productbuyerextended.ARV ?? 0;

            productbuyerextended.DateOfConsutruction = svm.DateOfConsutruction;
            productbuyerextended.DiscountTypeId = svm.DiscountTypeId;
            productbuyerextended.PropertyArea = svm.PropertyArea;
            productbuyerextended.TaxableArea = svm.TaxableArea;
            productbuyerextended.ARV = svm.ARV;
            productbuyerextended.TenantName = svm.TenantName;
            productbuyerextended.BillingType = svm.BillingType;
            productbuyerextended.Description = svm.Description;
            productbuyerextended.CoveredArea = svm.CoveredArea;
            productbuyerextended.GarageArea = svm.GarageArea;
            productbuyerextended.BalconyArea = svm.BalconyArea;
            productbuyerextended.IsRented = svm.IsRented;
            productbuyerextended.TaxPercentage = svm.TaxPercentage;
            productbuyerextended.TaxAmount = svm.TaxAmount;
            productbuyerextended.WaterTaxPercentage = svm.WaterTaxPercentage;
            productbuyerextended.WaterTaxAmount = svm.WaterTaxAmount;
            productbuyerextended.WEF = svm.WEF;
            if (svm.WEF != null)
                productbuyerextended.WEF = (DateTime)svm.NewWEF;
            productbuyerextended.ObjectState = Model.ObjectState.Modified;
            _ProductBuyerExtendedRepository.Update(productbuyerextended);



            int Status = 0;
            if (temp.Status != (int)StatusConstants.Drafted && temp.Status != (int)StatusConstants.Import)
            {
                Status = temp.Status ?? 0;
                temp.Status = (int)StatusConstants.Modified;
                temp.ModifiedBy = UserName;
                temp.ModifiedDate = DateTime.Now;
            }

            temp.ObjectState = Model.ObjectState.Modified;
            _PersonRepository.Update(temp);


            PersonExtended personextended = _unitOfWork.Repository<PersonExtended>().Find(svm.PersonId);
            personextended.TotalPropertyArea = (personextended.TotalPropertyArea ?? 0) - XPropertyArea + (productbuyerextended.PropertyArea ?? 0);
            personextended.TotalTaxableArea = (personextended.TotalTaxableArea ?? 0) - XTaxableArea + (productbuyerextended.TaxableArea ?? 0);
            personextended.TotalARV = (personextended.TotalARV ?? 0) - XARV + (productbuyerextended.ARV ?? 0);
            personextended.ObjectState = Model.ObjectState.Modified;
            _unitOfWork.Repository<PersonExtended>().Add(personextended);



            LogList.Add(new LogTypeViewModel
            {
                ExObj = ExTempLine,
                Obj = productbuyer
            });


            XElement Modifications = _modificationCheck.CheckChanges(LogList);

            _unitOfWork.Save();

            if (svm != null)
            {
                SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", svm.PersonId);
                _unitOfWork.SqlQuery<PropertyLineViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_CalculatePropertyTax @PersonId", SqlParameterPersonId).ToList();
            }


            if (svm.ARV != svm.OldARV)
            {
                SqlParameter SqlParameterProductBuyerId = new SqlParameter("@ProductBuyerId", svm.ProductBuyerId);
                _unitOfWork.SqlQuery<PropertyLineViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_RevisePropertyTaxDue @ProductBuyerId", SqlParameterProductBuyerId).ToList();
            }


            _logger.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = temp.DocTypeId,
                DocId = productbuyer.BuyerId,
                DocLineId = productbuyer.ProductBuyerId,
                ActivityType = (int)ActivityTypeContants.Modified,
                DocNo = temp.Code,
                xEModifications = Modifications,
                DocDate = temp.CreatedDate,
                DocStatus = temp.Status ?? 0,
            }));
        }

        public void Delete(PropertyLineViewModel vm, string UserName)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            ProductBuyer ProductBuyer = Find(vm.ProductBuyerId);
            ProductBuyerExtended ProductBuyerExtended = _ProductBuyerExtendedRepository.Find(vm.ProductBuyerId);
            Person header = _PersonRepository.Find(ProductBuyer.BuyerId);





            LogList.Add(new LogTypeViewModel
            {
                Obj = Mapper.Map<ProductBuyer>(ProductBuyer),
            });

            List<ProductBuyerLog> productbuyerloglist = (from p in _ProductBuyerLogRepository.Instance
                                                         where p.ProductBuyerId == vm.ProductBuyerId
                                                         select p).ToList();
            if (productbuyerloglist != null)
            {
                foreach(var productbuyerlogitem in productbuyerloglist)
                {
                    ProductBuyerLog productbuyerlog = _unitOfWork.Repository<ProductBuyerLog>().Find(productbuyerlogitem.ProductBuyerLogId);
                    productbuyerlog.ObjectState = Model.ObjectState.Deleted;
                    _ProductBuyerLogRepository.Delete(productbuyerlog);
                }
            }



            PersonExtended personextended = _unitOfWork.Repository<PersonExtended>().Find(header.PersonID);
            personextended.TotalPropertyArea = (personextended.TotalPropertyArea ?? 0) - (ProductBuyerExtended.PropertyArea ?? 0);
            personextended.TotalTaxableArea = (personextended.TotalTaxableArea ?? 0) - (ProductBuyerExtended.TaxableArea ?? 0);
            personextended.TotalARV = (personextended.TotalARV ?? 0) - (ProductBuyerExtended.ARV ?? 0);
            personextended.ObjectState = Model.ObjectState.Modified;
            _unitOfWork.Repository<PersonExtended>().Add(personextended);

            ProductBuyerExtended.ObjectState = Model.ObjectState.Deleted;
            _ProductBuyerExtendedRepository.Delete(ProductBuyerExtended);

            //_PropertyLineService.Delete(ProductBuyer);
            ProductBuyer.ObjectState = Model.ObjectState.Deleted;
            Delete(ProductBuyer);





            if (header.Status != (int)StatusConstants.Drafted && header.Status != (int)StatusConstants.Import)
            {
                header.Status = (int)StatusConstants.Modified;
                header.ModifiedDate = DateTime.Now;
                header.ModifiedBy = UserName;
                _PersonRepository.Update(header);
            }





            
            XElement Modifications = _modificationCheck.CheckChanges(LogList);

            _unitOfWork.Save();

            if (vm != null)
            {
                SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", vm.PersonId);
                _unitOfWork.SqlQuery<PropertyLineViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_CalculatePropertyTax @PersonId", SqlParameterPersonId).ToList();
            }

            //Saving the Activity Log

            _logger.LogActivityDetail(LogVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = header.DocTypeId,
                DocId = header.PersonID,
                DocLineId = ProductBuyer.ProductBuyerId,
                ActivityType = (int)ActivityTypeContants.Deleted,
                DocNo = header.Code,
                xEModifications = Modifications,
                DocDate = header.CreatedDate,
                DocStatus = header.Status ?? 0,
            }));

        }


        public void Dispose()
        {
            _unitOfWork.Dispose();
        }




        public IEnumerable<ProductBuyer> GetProductBuyerListForHeader(int HeaderId)
        {
            return (from p in _ProductBuyerRepository.Instance
                    where p.BuyerId == HeaderId
                    select p);
        }





        public bool IsDuplicateLine(int PersonId, int ProductId, int Dimension1Id, int ProductBuyerId)
        {
            var temp = (from L in _unitOfWork.Repository<ProductBuyer>().Instance
                        where L.BuyerId == PersonId && L.ProductId == ProductId && L.Dimension1Id == Dimension1Id && L.ProductBuyerId != ProductBuyerId
                        select L).FirstOrDefault();

            if (temp != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DiscountType GetDicountTypeDetail(int DiscountTypeId)
        {
            var temp = (from L in _unitOfWork.Repository<DiscountType>().Instance
                        where L.DiscountTypeId == DiscountTypeId
                        select L).FirstOrDefault();

            return temp;
        }

        public Decimal FGetARV(int Ward, int RoadType, int ConstructionType, int PropertyType, int DiscountType,
    decimal CoveredArea, decimal BalconyArea, decimal GarageArea, bool IsRented, DateTime DateOfConstruction, DateTime WEF)
        {
            SqlParameter SqlParameterWard = new SqlParameter("@Ward", Ward);
            SqlParameter SqlParameterRoadType = new SqlParameter("@RoadType", RoadType);
            SqlParameter SqlParameterConstructionType = new SqlParameter("@ConstructionType", ConstructionType);
            SqlParameter SqlParameterPropertyType = new SqlParameter("@PropertyType", PropertyType);
            SqlParameter SqlParameterDiscountType = new SqlParameter("@DiscountType", DiscountType);
            SqlParameter SqlParameterCoveredArea = new SqlParameter("@CoveredArea", CoveredArea);
            SqlParameter SqlParameterBalconyArea = new SqlParameter("@BalconyArea", BalconyArea);
            SqlParameter SqlParameterGarageArea = new SqlParameter("@GarageArea", GarageArea);
            SqlParameter SqlParameterIsRented = new SqlParameter("@IsRented", IsRented);
            SqlParameter SqlParameterDateOfConstruction = new SqlParameter("@DateOfConstruction", DateOfConstruction);
            SqlParameter SqlParameterWEF = new SqlParameter("@WEF", WEF);

            GetValue GetValue = _unitOfWork.SqlQuery<GetValue>("" + System.Configuration.ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_GetPropertyTaxARV @Ward, @RoadType, @ConstructionType, @PropertyType, @DiscountType, @CoveredArea,@BalconyArea, @GarageArea, @IsRented, @DateOfConstruction,@WEF",
                SqlParameterWard, SqlParameterRoadType, SqlParameterConstructionType, SqlParameterPropertyType, SqlParameterDiscountType,
                SqlParameterCoveredArea, SqlParameterBalconyArea, SqlParameterGarageArea, SqlParameterIsRented, SqlParameterDateOfConstruction, SqlParameterWEF).FirstOrDefault();

            if (GetValue != null)
            {
                return GetValue.Value;
            }
            else
            {
                return 0;
            }
        }

    }
}