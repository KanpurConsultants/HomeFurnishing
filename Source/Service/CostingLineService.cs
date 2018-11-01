using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;
using Model.ViewModels;
using Core.Common;
using System;
using Model;
using System.Threading.Tasks;
using Data.Models;


namespace Service
{
    public interface ICostingLineService : IDisposable
    {
        CostingLine Create(CostingLine s);
        void Delete(int id);
        void Delete(CostingLine s);
        CostingLine GetCostingLine(int id);
        CostingLineViewModel GetCostingLineModel(int id);
        CostingLine Find(int id);
        void Update(CostingLine s);
        IEnumerable<CostingLineIndexViewModel> GetCostingLineList(int CostingHeaderId);
        bool CheckForProductExists(int ProductId, int CostingHEaderId, int CostingLineId);
        bool CheckForProductExists(int ProductId, int CostingHEaderId);
        CostingLineBalance GetCosting(int LineId);
        IQueryable<ComboBoxResult> GetCustomProductGroups(int Id, string term);
        IEnumerable<CostingLineDetail> GetProductDetail(int ProductId);
        IQueryable<CostingLineIndexViewModel> GetCostingLineListForIndex(int CostingHeaderId);
    }

    public class CostingLineService : ICostingLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public CostingLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CostingLine Create(CostingLine S)
        {
            S.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<CostingLine>().Insert(S);
            return S;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<CostingLine>().Delete(id);
        }

        public void Delete(CostingLine s)
        {
            _unitOfWork.Repository<CostingLine>().Delete(s);
        }

        public void Update(CostingLine s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<CostingLine>().Update(s);
        }

        public IQueryable<CostingLineIndexViewModel> GetCostingLineListForIndex(int CostingHeaderId)
        {

            var temp = from p in db.CostingLine
                       join t1 in db.CostingHeader on p.CostingHeaderId equals t1.CostingHeaderId into table1
                       from tab1 in table1.DefaultIfEmpty()
                       orderby p.CostingLineId
                       where p.CostingHeaderId == CostingHeaderId
                       select new CostingLineIndexViewModel
                       {
                           PileWeight = p.PileWeight,
                           ProductGroupName = p.ProductGroup.ProductGroupName,
                           ColourName = p.Colour.ColourName,
                           SizeName = p.Size.SizeName,
                           Qty = p.Qty,
                           CostingHeaderId = p.CostingHeaderId,
                           CostingLineId = p.CostingLineId,
                           Remark = p.Remark,
                       };
            return temp;
        }

        public CostingLine GetCostingLine(int id)
        {
            return _unitOfWork.Repository<CostingLine>().Query().Get().Where(m => m.CostingLineId == id).FirstOrDefault();
            //return (from p in db.CostingLine
            //        join t in db.Products on p.ProductId equals t.ProductId into table from tab in table.DefaultIfEmpty()
            //        where p.CostingLineId == id
            //        select new CostingLineViewModel
            //        {
            //            Amount=p.Amount,
            //            CreatedBy=p.CreatedBy,
            //            CreatedDate=p.CreatedDate,
            //            DeliveryQty=p.DeliveryQty,
            //            DeliveryUnitId=p.DeliveryUnitId,
            //            DueDate=p.DueDate,
            //            ModifiedBy=p.ModifiedBy,
            //            ModifiedDate=p.ModifiedDate,
            //            Qty=p.Qty,
            //            Rate=p.Rate,
            //            Remark=p.Remark,
            //            CostingHeaderId=p.CostingHeaderId,
            //            CostingLineId=p.CostingLineId,
            //            Specification=p.Specification,
            //            Product=tab.ProductName,
            //        }

            //            ).FirstOrDefault();
        }
        public CostingLineViewModel GetCostingLineModel(int id)
        {
            //return _unitOfWork.Repository<CostingLine>().Query().Get().Where(m => m.CostingLineId == id).FirstOrDefault();
            return (from p in db.CostingLine
                    join t in db.ProductGroups on p.ProductGroupId equals t.ProductGroupId into table
                    from tab in table.DefaultIfEmpty()
                    where p.CostingLineId == id
                    select new CostingLineViewModel
                    {
                        CreatedBy = p.CreatedBy,
                        CreatedDate = p.CreatedDate,
                        PileWeight = p.PileWeight,
                        ModifiedBy = p.ModifiedBy,
                        ModifiedDate = p.ModifiedDate,
                        Qty = p.Qty,
                        CostingHeaderId = p.CostingHeaderId,
                        CostingLineId = p.CostingLineId,
                        ProductGroupName = tab.ProductGroupName,
                    }

                        ).FirstOrDefault();
        }
        public CostingLine Find(int id)
        {
            return _unitOfWork.Repository<CostingLine>().Find(id);
        }


        public IEnumerable<CostingLineIndexViewModel> GetCostingLineList(int CostingHeaderId)
        {
            //return _unitOfWork.Repository<CostingLine>().Query().Include(m => m.Product).Include(m=>m.CostingHeader).Get().Where(m => m.CostingHeaderId == CostingHeaderId).ToList();

            return (from p in db.CostingLine
                    where p.CostingHeaderId == CostingHeaderId
                    select new CostingLineIndexViewModel
                    {
                        PileWeight = p.PileWeight,
                        ProductGroupName = p.ProductGroup.ProductGroupName,
                        Qty = p.Qty,
                        CostingHeaderId = p.CostingHeaderId,
                        CostingLineId = p.CostingLineId,
                        CostingHeaderDocNo=p.CostingHeader.DocNo
                    }

                );


        }


        public CostingLineBalance GetCosting(int LineId)
        {
            //var temp = _unitOfWork.Repository<CostingLine>().Query()
            //    .Include(m => m.CostingHeader)
            //    .Include(m => m.Product)
            //    .Get().Where(m => m.ProductId == ProductId);

            //List<CostingLineBalance> CostingLineBalance = new List<CostingLineBalance>();
            //foreach (var item in temp)
            //{
            //    CostingLineBalance.Add(new CostingLineBalance
            //    {
            //        CostingLineId = item.CostingLineId,
            //        CostingDocNo = item.CostingHeader.DocNo
            //    });
            //}

            return (from p in db.CostingLine
                    join t in db.CostingHeader on p.CostingHeaderId equals t.CostingHeaderId into table from tab in table
                    where p.CostingLineId == LineId
                    select new CostingLineBalance
                    {
                        CostingLineId = p.CostingLineId,
                        CostingDocNo = tab.DocNo
                    }


                ).FirstOrDefault();

        }

        public bool CheckForProductExists(int ProductId, int CostingHeaderId, int CostingLineId)
        {

            CostingLine temp = (from p in db.CostingLine
                                  where p.ProductGroupId   == ProductId && p.CostingHeaderId == CostingHeaderId &&p.CostingLineId!=CostingLineId
                                  select p).FirstOrDefault();
            if (temp != null)
                return true;
            else return false;
        }
        public bool CheckForProductExists(int ProductId, int CostingHeaderId)
        {

            CostingLine temp = (from p in db.CostingLine
                                  where p.ProductGroupId == ProductId && p.CostingHeaderId == CostingHeaderId
                                  select p).FirstOrDefault();
            if (temp != null)
                return true;
            else return false;
        }

        public IEnumerable<CostingLineDetail> GetProductDetail(int ProductId)
        {
            string mQry = @"SELECT isnull(P.GrossWeight,P.StandardWeight) AS PileWeight , PG.ProductGroupId, PG.ProductGroupName, C.ColourId, C.ColourName, VRS.ManufaturingSizeID AS SizeId, VRS.ManufaturingSizeName  SizeName  
                            FROM web.Products P WITH(Nolock)
                            LEFT JOIN web.ProductGroups PG WITH(Nolock) ON PG.ProductGroupId = P.ProductGroupId
                            LEFT JOIN web.FinishedProduct FP WITH(Nolock) ON FP.ProductId = P.ProductId
                            LEFT JOIN web.Colours C WITH(Nolock) ON C.ColourId = FP.ColourId
                            LEFT JOIN web.ViewRugSize VRS WITH(Nolock) ON VRS.ProductId = P.ProductId
                            WHERE P.ProductId = " + ProductId;
            IEnumerable<CostingLineDetail> CostingLineDetail = db.Database.SqlQuery<CostingLineDetail>(mQry).ToList();
            return CostingLineDetail;
        }

        public IQueryable<ComboBoxResult> GetCustomProductGroups(int Id, string term)
        {

            var Costing = new CostingHeaderService(_unitOfWork).Find(Id);

            //var settings = new CostingSettingsService(_unitOfWork).GetCostingSettings(Costing.DocTypeId, Costing.DivisionId, Costing.SiteId);

            //string[] ProductTypes = null;
            //if (!string.IsNullOrEmpty(settings.filterProductTypes)) { ProductTypes = settings.filterProductTypes.Split(",".ToCharArray()); }
            //else { ProductTypes = new string[] { "NA" }; }

            //string[] Products = null;
            //if (!string.IsNullOrEmpty(settings.filterProducts)) { Products = settings.filterProducts.Split(",".ToCharArray()); }
            //else { Products = new string[] { "NA" }; }

            //string[] ProductGroups = null;
            //if (!string.IsNullOrEmpty(settings.filterProductGroups)) { ProductGroups = settings.filterProductGroups.Split(",".ToCharArray()); }
            //else { ProductGroups = new string[] { "NA" }; }

            //return (from p in db.Product
            //        where (string.IsNullOrEmpty(settings.filterProductTypes) ? 1 == 1 : ProductTypes.Contains(p.ProductGroup.ProductTypeId.ToString()))
            //        && (string.IsNullOrEmpty(settings.filterProducts) ? 1 == 1 : Products.Contains(p.ProductId.ToString()))
            //        && (string.IsNullOrEmpty(settings.filterProductGroups) ? 1 == 1 : ProductGroups.Contains(p.ProductGroupId.ToString()))
            //        && (string.IsNullOrEmpty(term) ? 1 == 1 : p.ProductName.ToLower().Contains(term.ToLower()))
            //        orderby p.ProductName
            //        select new ComboBoxResult
            //        {
            //            id = p.ProductId.ToString(),
            //            text = p.ProductName,
            //        });

            IQueryable<ProductGroup> ProductGrouplist = new ProductGroupService(_unitOfWork).GetProductGroupList(1);

            return (from PG in db.ProductGroups
                    join P in db.Product  on PG.ProductGroupId  equals P.ProductGroupId into Ptable
                    from Ptab in Ptable
                    where 1==1 && PG.ProductTypeId ==1
                    && (string.IsNullOrEmpty(term) ? 1 == 1 : Ptab.ProductName.ToLower().Contains(term.ToLower()))
                    orderby Ptab.ProductName
                    select new ComboBoxResult
                    {
                        id = Ptab.ProductId.ToString(),
                        text = Ptab.ProductName,
                        TextProp1= PG.ProductGroupName,
                    });

        }

        public void Dispose()
        {
        }
    }
}
