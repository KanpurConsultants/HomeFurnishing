using Data.Infrastructure;
using Data.Models;
using Model;
using Model.Models;
using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.ViewModels;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Common;
using Model.ViewModel;
using System.Data.Entity.SqlServer;

namespace Service
{
    public interface ICustomHeaderService : IDisposable
    {
        CustomHeader Create(CustomHeader s);
        void Delete(int id);
        void Delete(CustomHeader s);
        //CustomHeaderDetail GetCustomHeaderDetail(int id);

        //CustomHeaderIndexViewModel GetCustomHeaderVM(int id);
        //CustomHeaderDetail Find(int id);
        //IQueryable<CustomHeaderIndexViewModel> GetCustomHeaderList(int id, string Uname);
        //IQueryable<CustomHeaderIndexViewModel> GetCustomHeaderListPendingToSubmit(int id, string Uname);
        //IQueryable<CustomHeaderIndexViewModel> GetCustomHeaderListPendingToReview(int id, string Uname);

        void Update(CustomHeader s);
        //string GetMaxDocNo();
        //CustomHeader FindByDocNo(string Docno);
        //IEnumerable<SaleInvoicePrintViewModel> FGetPrintData(int Id);
        //IEnumerable<SaleInvoicePrintViewModel> FGetPrintInvoiceData(int Id);
        //IEnumerable<SaleInvoicePrintViewModel> FGetPrintInvoiceWithCollectionData(int Id);
        //IEnumerable<MasterKeyPrintViewModel> FGetPrintMasterKeyData(int Id);
        //IEnumerable<SaleInvoicePrintViewModel> FGetPrintPackingListData(int Id);
        //IEnumerable<SaleInvoicePrintViewModel> FGetPrintPackingListWithCollectionData(int Id);
        //IEnumerable<CustomHeader> GetSaleInvoiceListForReport(int BuyerId);

        IQueryable<CustomHeaderIndexViewModel> GetCustomHeaderIndex(int id, string Uname);
        int NextId(int id);
        int PrevId(int id);
        CustomHeader FindCustomHeader(int id);
        //IQueryable<ComboBoxResult> GetCustomPerson(int Id, string term);
        IEnumerable<DocumentTypeHeaderAttributeViewModel> GetDocumentHeaderAttribute(int id);
        //string GetNarration(int CustomHeaderId);
    }
    public class CustomHeaderService : ICustomHeaderService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public CustomHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }


        public IQueryable<CustomHeaderIndexViewModel> GetCustomHeaderIndex(int id, string Uname)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


            var temp = from p in db.ViewCustomHeaderAttribute
                       join c in db.CustomHeader on p.CustomHeaderId equals c.CustomHeaderId into cTable
                       from cTab in cTable.DefaultIfEmpty()
                       where p.DivisionId == DivisionId && p.SiteId == SiteId && p.DocTypeId == id
                       orderby p.CustomHeaderId descending
                       select new CustomHeaderIndexViewModel
                       {
                           CustomHeaderId = p.CustomHeaderId,
                           DocNo = p.DocNo,
                           DocDate = p.DocDate,
                           ModifiedBy=cTab.ModifiedBy,
                           ModifiedDate =cTab.ModifiedDate,
                           Remark = p.Remark
                       };
            return temp;
        }

        public CustomHeader Create(CustomHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<CustomHeader>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<CustomHeader>().Delete(id);
        }
        public void Delete(CustomHeader s)
        {
            _unitOfWork.Repository<CustomHeader>().Delete(s);
        }
        public void Update(CustomHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<CustomHeader>().Update(s);
        }

        //public CustomHeaderDetail GetCustomHeaderDetail(int id)
        //{
        //    //return _unitOfWork.Repository<CustomHeaderDetail>().Query().Get().Where(m => m.CustomHeaderId == id).FirstOrDefault();
        //    return (from H in db.CustomHeaderDetail where H.CustomHeaderId == id select H).FirstOrDefault();
        //}

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.CustomHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.CustomHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();


            }
            else
            {
                temp = (from p in db.CustomHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.CustomHeaderId).FirstOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public int PrevId(int id)
        {

            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.CustomHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.CustomHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.CustomHeader
                        orderby p.DocDate descending, p.DocNo descending
                        select p.CustomHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        //public IQueryable<CustomHeaderIndexViewModel> GetSaleInvoiceListForCustomIndex(int id, string Uname)
        //{
        //    var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
        //    var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];


        //    var temp = from p in db.ViewSaleInvoiceWithCustomAttribute
        //               join Pe in db.CustomHeader on p.CustomHeaderId equals Pe.CustomHeaderId into PeTable
        //               from PeTab in PeTable.DefaultIfEmpty()
        //               join t2 in db.Persons on PeTab.BillToBuyerId equals t2.PersonID into table2
        //               from tab2 in table2.DefaultIfEmpty()
        //               where PeTab.DivisionId == DivisionId && PeTab.SiteId == SiteId && PeTab.DocTypeId == id
        //               orderby p.CustomHeaderId
        //               select new CustomHeaderIndexViewModel
        //               {
        //                   CustomHeaderId=p.CustomHeaderId,
        //                   DocNo=p.DocNo,
        //                   DocDate=p.DocDate,
        //                   SaleToBuyerName= tab2.Name,
        //                   BillToBuyerName = tab2.Name,
        //               };
        //    return temp;
        //}

        //public IQueryable<ComboBoxResult> GetPendingSaleInvoiceToCustomAttribute(string term)
        //{
        //    var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
        //    var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

        //    var list = (from p in db.ViewSaleInvoiceBalanceForCustomAttribute
        //                join b in db.Persons on p.SaleToBuyerId equals b.PersonID into bTable
        //                from bTab in bTable.DefaultIfEmpty()
        //                where 1 == 1 
        //                && ((string.IsNullOrEmpty(term) ? 1 == 1 : p.DocNo.ToLower().Contains(term.ToLower()))
        //                || (string.IsNullOrEmpty(term) ? 1 == 1 : bTab.Code.Contains(term.ToLower())))
        //                orderby p.DocDate, p.DocNo
        //                select new ComboBoxResult
        //                {
        //                    text = p.DocNo,
        //                    id = p.CustomHeaderId.ToString(),
        //                    TextProp1 = bTab.Code
        //                });
        //    return list;
        //}
        //public CustomHeaderIndexViewModel GetCustomHeaderVM(int id)
        //{
        //    CustomHeaderIndexViewModel temp = (from p in db.CustomHeader
        //                                            join t2 in db.Persons on p.BillToBuyerId equals t2.PersonID into table2
        //                                            from tab2 in table2.DefaultIfEmpty()
        //                                            where p.CustomHeaderId == id
        //                                            select new CustomHeaderIndexViewModel
        //                                            {
        //                                                BillToBuyerName = tab2.Name,
        //                                                CreatedBy = p.CreatedBy,
        //                                                CreatedDate = p.CreatedDate,
        //                                                DivisionName = p.Division.DivisionName,
        //                                                DocDate = p.DocDate,
        //                                                DocTypeId=p.DocTypeId,
        //                                                SiteId=p.SiteId,
        //                                                DivisionId=p.DivisionId,
        //                                                DocNo = p.DocNo,
        //                                                ModifiedBy = p.ModifiedBy,
        //                                                ModifiedDate = p.ModifiedDate,
        //                                                Remark = p.Remark,
        //                                                CustomHeaderId = p.CustomHeaderId,
        //                                                SiteName = p.Site.SiteName,
        //                                                Status = p.Status,
        //                                                DocumentTypeName = p.DocType.DocumentTypeName,
        //                                                CurrencyName = p.Currency.Name,
        //                                                LockReason=p.LockReason,
        //                                            }

        //        ).FirstOrDefault();

        //    return temp;
        //}

        //public CustomHeaderDetail Find(int id)
        //{
        //    return _unitOfWork.Repository<CustomHeaderDetail>().Find(id);
        //}

        public CustomHeader FindCustomHeader(int id)
        {
            return _unitOfWork.Repository<CustomHeader>().Find(id);
        }
        //public IQueryable<CustomHeaderIndexViewModel> GetCustomHeaderList(int id, string Uname)
        //{
        //    var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
        //    var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

        //    int AdditionalChargesProductNatureId = 0;
        //    var ProductNature = (from Pt in db.ProductNature where Pt.ProductNatureName == ProductNatureConstants.AdditionalCharges select Pt).FirstOrDefault();
        //    if (ProductNature != null)
        //        AdditionalChargesProductNatureId = ProductNature.ProductNatureId;

        //    var TempCustomHeaderCharges = from H in db.CustomHeader
        //                                      join Hc in db.CustomHeaderCharge on H.CustomHeaderId equals Hc.HeaderTableId into CustomHeaderChargesTable
        //                                      from CustomHeaderChargesTab in CustomHeaderChargesTable.DefaultIfEmpty()
        //                                      join C in db.Charge on CustomHeaderChargesTab.ChargeId equals C.ChargeId into ChargeTable
        //                                      from ChargeTab in ChargeTable.DefaultIfEmpty()
        //                                      where H.SiteId == SiteId && H.DivisionId == DivisionId && H.DocTypeId == id && ChargeTab.ChargeName == "Net Amount"
        //                                      select new
        //                                      {
        //                                          CustomHeaderId = H.CustomHeaderId,
        //                                          NetAmount = CustomHeaderChargesTab.Amount
        //                                      };

        //    var SaleInvoiceCancelDetail = from Sih in db.CustomHeader
        //               join Sil in db.SaleInvoiceLine on Sih.CustomHeaderId equals Sil.CustomHeaderId into SaleInvoiceLineTable from SaleInvoiceLineTab in SaleInvoiceLineTable.DefaultIfEmpty()
        //               join L in db.SaleInvoiceReturnLine on SaleInvoiceLineTab.SaleInvoiceLineId equals L.SaleInvoiceLineId into SaleInvoiceReturnLineTable from SaleInvoiceReturnLineTab in SaleInvoiceReturnLineTable.DefaultIfEmpty()
        //               join H in db.SaleInvoiceReturnHeader on SaleInvoiceReturnLineTab.SaleInvoiceReturnHeaderId equals H.SaleInvoiceReturnHeaderId into SaleInvoiceReturnHeaderTable from SaleInvoiceReturnHeaderTab in SaleInvoiceReturnHeaderTable.DefaultIfEmpty()
        //               join Hc in db.SaleInvoiceReturnHeaderCharge on SaleInvoiceReturnHeaderTab.SaleInvoiceReturnHeaderId equals Hc.HeaderTableId into SaleInvoiceReturnHeaderChargeTable
        //               from SaleInvoiceReturnHeaderChargeTab in SaleInvoiceReturnHeaderChargeTable.DefaultIfEmpty()
        //               join C in db.Charge on SaleInvoiceReturnHeaderChargeTab.ChargeId equals C.ChargeId into ChargeTable
        //               from ChargeTab in ChargeTable.DefaultIfEmpty()
        //               join D in db.DocumentType on SaleInvoiceReturnHeaderTab.DocTypeId equals D.DocumentTypeId into DocumentTypeTable
        //               from DocumentTypeTab in DocumentTypeTable.DefaultIfEmpty()
        //               where DocumentTypeTab.Nature == TransactionNatureConstants.Return && ChargeTab.ChargeName == "Net Amount"
        //               select new
        //               {
        //                   CustomHeaderId = Sih.CustomHeaderId,
        //                   CancelNetAmount = SaleInvoiceReturnHeaderChargeTab.Amount
        //               };



        //    var TempProductDetail = from H in db.CustomHeader
        //                            join L in db.SaleInvoiceLine on H.CustomHeaderId equals L.CustomHeaderId into SaleInvoiceLineTable
        //                            from SaleInvoiceLineTab in SaleInvoiceLineTable.DefaultIfEmpty()
        //                            where H.DivisionId == DivisionId && H.SiteId == SiteId && H.DocTypeId == id && SaleInvoiceLineTab.SaleDispatchLine.PackingLine.Product.ProductGroup.ProductType.ProductNatureId != AdditionalChargesProductNatureId && SaleInvoiceLineTab.Qty != 0
        //                            group new { SaleInvoiceLineTab } by new { SaleInvoiceLineTab.CustomHeaderId } into Result
        //                            select new
        //                            {
        //                                CustomHeaderId = Result.Key.CustomHeaderId,
        //                                ProductUidName = Result.Max(i => i.SaleInvoiceLineTab.SaleDispatchLine.PackingLine.ProductUid.ProductUidName),
        //                                ProductName = Result.Max(i => i.SaleInvoiceLineTab.SaleDispatchLine.PackingLine.Product.ProductName),
        //                                ProductGroupName = Result.Max(i => i.SaleInvoiceLineTab.SaleDispatchLine.PackingLine.Product.ProductGroup.ProductGroupName),
        //                            };


        //    var temp = from p in db.CustomHeader
        //               join t in db.Persons on p.BillToBuyerId equals t.PersonID
        //               join Hc in TempCustomHeaderCharges on p.CustomHeaderId equals Hc.CustomHeaderId into CustomHeaderChargesTable
        //               from CustomHeaderChargesTab in CustomHeaderChargesTable.DefaultIfEmpty()
        //               join Tp in TempProductDetail on p.CustomHeaderId equals Tp.CustomHeaderId into TempProductDetailTable
        //               from TempProductDetailTab in TempProductDetailTable.DefaultIfEmpty()
        //               join Sc in SaleInvoiceCancelDetail on p.CustomHeaderId equals Sc.CustomHeaderId into SaleInvoiceCancelDetailTable from SaleInvoiceCancelDetailTab in SaleInvoiceCancelDetailTable.DefaultIfEmpty()
        //               orderby p.DocDate descending, p.DocNo descending
        //               where p.DivisionId == DivisionId && p.SiteId == SiteId && p.DocTypeId == id
        //               select new CustomHeaderIndexViewModel
        //               {
        //                   //Remark = p.Remark,
        //                   Remark = (CustomHeaderChargesTab.NetAmount == (SaleInvoiceCancelDetailTab.CancelNetAmount ?? 0) ? "Cancelled." : ((SaleInvoiceCancelDetailTab.CancelNetAmount ?? 0) != 0 ? "Partially Cancelled." : "")) + p.Remark ,
        //                   DocDate = p.DocDate,
        //                   CustomHeaderId = p.CustomHeaderId,
        //                   DocNo = p.DocNo,
        //                   BillToBuyerName = t.Name + ", " + t.Suffix + " [" + t.Code + "]",
        //                   Status = p.Status,
        //                   ModifiedBy = p.ModifiedBy,
        //                   ReviewCount = p.ReviewCount,
        //                   ReviewBy = p.ReviewBy,                                              
        //                   Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
        //                   GatePassDocNo = p.SaleDispatchHeader.GatePassHeader.DocNo,
        //                   GatePassHeaderId = p.SaleDispatchHeader.GatePassHeader.GatePassHeaderId,
        //                   GatePassDocDate = p.SaleDispatchHeader.GatePassHeader.DocDate,
        //                   GatePassStatus = (p.SaleDispatchHeader.GatePassHeader.Status != null ? p.SaleDispatchHeader.GatePassHeader.Status : 0),
        //                   TotalQty = p.SaleInvoiceLines.Sum(m => m.Qty),
        //                   TotalAmount = CustomHeaderChargesTab.NetAmount ?? (p.SaleInvoiceLines.Sum(m => m.Amount)),

        //                   ProductUidName = TempProductDetailTab.ProductUidName,
        //                   ProductName = TempProductDetailTab.ProductName,
        //                   ProductGroupName = TempProductDetailTab.ProductGroupName,


        //                   DecimalPlaces = (from o in p.SaleInvoiceLines
        //                                    join rl in db.SaleDispatchLine on o.SaleDispatchLineId equals rl.SaleDispatchLineId
        //                                    join ol in db.PackingLine on rl.PackingLineId equals ol.PackingLineId
        //                                    join prod in db.Product on ol.ProductId equals prod.ProductId
        //                                    join u in db.Units on prod.UnitId equals u.UnitId
        //                                    select u.DecimalPlaces).Max(),

        //               };
        //    return temp;
        //}

        //public IQueryable<CustomHeaderIndexViewModel> GetCustomHeaderListPendingToSubmit(int id, string Uname)
        //{
        //    List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
        //    var LedgerHeader = GetCustomHeaderList(id, Uname).AsQueryable();

        //    var PendingToSubmit = from p in LedgerHeader
        //                          where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
        //                          select p;
        //    return PendingToSubmit;
        //}

        //public IQueryable<CustomHeaderIndexViewModel> GetCustomHeaderListPendingToReview(int id, string Uname)
        //{
        //    List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
        //    var LedgerHeader = GetCustomHeaderList(id, Uname).AsQueryable();

        //    var PendingToReview = from p in LedgerHeader
        //                          where p.Status == (int)StatusConstants.Submitted && (SqlFunctions.CharIndex(Uname, (p.ReviewBy ?? "")) == 0)
        //                          select p;
        //    return PendingToReview;
        //}

        //public CustomHeader FindByDocNo(string Docno)
        //{
        //    return _unitOfWork.Repository<CustomHeader>().Query().Get().Where(m => m.DocNo == Docno).FirstOrDefault();
        //}

        //public string GetMaxDocNo()
        //{
        //    int x;
        //    var maxVal = _unitOfWork.Repository<CustomHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
        //    return (maxVal + 1).ToString();
        //}

        public void Dispose()
        {
        }

        public IEnumerable<SaleInvoicePrintViewModel> FGetPrintData(int Id)
        {
            IEnumerable<SaleInvoicePrintViewModel> SaleInvoiceprintviewmodel = db.Database.SqlQuery<SaleInvoicePrintViewModel>("Web.ProcSaleInvoicePrint @Id", new SqlParameter("@Id", Id)).ToList();
            return SaleInvoiceprintviewmodel;
        }

        public IEnumerable<SaleInvoicePrintViewModel> FGetPrintInvoiceData(int Id)
        {
            IEnumerable<SaleInvoicePrintViewModel> SaleInvoiceprintviewmodel = db.Database.SqlQuery<SaleInvoicePrintViewModel>("Web.ProcSaleInvoicePrint_ForInvoice @Id", new SqlParameter("@Id", Id)).ToList();
            return SaleInvoiceprintviewmodel;
        }

        public IEnumerable<SaleInvoicePrintViewModel> FGetPrintInvoiceWithCollectionData(int Id)
        {
            IEnumerable<SaleInvoicePrintViewModel> SaleInvoiceprintviewmodel = db.Database.SqlQuery<SaleInvoicePrintViewModel>("Web.ProcSaleInvoicePrint_ForInvoice_WithCollection @Id", new SqlParameter("@Id", Id)).ToList();
            return SaleInvoiceprintviewmodel;
        }

        public IEnumerable<MasterKeyPrintViewModel> FGetPrintMasterKeyData(int Id)
        {
            IEnumerable<MasterKeyPrintViewModel> SaleInvoiceprintviewmodel = db.Database.SqlQuery<MasterKeyPrintViewModel>("Web.ProcSaleInvoicePrint_ForMasterKey @Id", new SqlParameter("@Id", Id)).ToList();
            return SaleInvoiceprintviewmodel;
        }

        public IEnumerable<SaleInvoicePrintViewModel> FGetPrintPackingListData(int Id)
        {
            IEnumerable<SaleInvoicePrintViewModel> SaleInvoiceprintviewmodel = db.Database.SqlQuery<SaleInvoicePrintViewModel>("Web.ProcSalePackingListPrint @Id", new SqlParameter("@Id", Id)).ToList();
            return SaleInvoiceprintviewmodel;
        }

        public IEnumerable<SaleInvoicePrintViewModel> FGetPrintPackingListWithCollectionData(int Id)
        {
            IEnumerable<SaleInvoicePrintViewModel> SaleInvoiceprintviewmodel = db.Database.SqlQuery<SaleInvoicePrintViewModel>("Web.ProcSalePackingListPrint @Id", new SqlParameter("@Id", Id)).ToList();
            return SaleInvoiceprintviewmodel;
        }

        //public IEnumerable<CustomHeader> GetSaleInvoiceListForReport(int BuyerId)
        //{
        //    return _unitOfWork.Repository<CustomHeader>().Query().Include(m => m.DocType).Get().Where(m => m.BillToBuyerId == BuyerId);
        //}

        //public IEnumerable<SaleInvoiceListViewModel> GetPendingInvoices(int id, int SaleInvoiceReturnHeaderId)
        //{

        //    var SaleInvoiceReturnHeader = new SaleInvoiceReturnHeaderService(_unitOfWork).Find(SaleInvoiceReturnHeaderId);

        //    var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(SaleInvoiceReturnHeader.DocTypeId, SaleInvoiceReturnHeader.DivisionId, SaleInvoiceReturnHeader.SiteId);

        //    string[] contraDocTypes = null;
        //    if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
        //    else { contraDocTypes = new string[] { "NA" }; }

        //    string[] contraSites = null;
        //    if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
        //    else { contraSites = new string[] { "NA" }; }

        //    string[] contraDivisions = null;
        //    if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
        //    else { contraDivisions = new string[] { "NA" }; }

        //    int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
        //    int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


        //    return (from p in db.ViewSaleInvoiceBalance
        //            join t in db.CustomHeader on p.CustomHeaderId equals t.CustomHeaderId into table
        //            from tab in table.DefaultIfEmpty()
        //            join t1 in db.SaleDispatchLine on p.SaleDispatchLineId equals t1.SaleDispatchLineId into table1
        //            from tab1 in table1.DefaultIfEmpty()
        //            join t2 in db.SaleInvoiceLine on p.SaleDispatchLineId equals t2.SaleDispatchLineId into InvoiceTable
        //            from InvTab in InvoiceTable.DefaultIfEmpty()
        //            where InvTab.ProductId == id && tab.SaleToBuyerId == SaleInvoiceReturnHeader.BuyerId && p.BalanceQty > 0
        //            && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
        //            && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
        //            select new SaleInvoiceListViewModel
        //            {
        //                SaleInvoiceLineId = p.SaleInvoiceLineId,
        //                CustomHeaderId = p.CustomHeaderId,
        //                DocNo = tab.DocNo,
        //                Dimension1Name = InvTab.Dimension1.Dimension1Name,
        //                Dimension2Name = InvTab.Dimension2.Dimension2Name,
        //            }
        //                );
        //}

        //public IEnumerable<SaleInvoiceListViewModel> GetPendingInvoicesWithterm(int id, int SaleInvoiceReturnHeaderId, string term)
        //{

        //    var SaleInvoiceReturnHeader = new SaleInvoiceReturnHeaderService(_unitOfWork).Find(SaleInvoiceReturnHeaderId);

        //    var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(SaleInvoiceReturnHeader.DocTypeId, SaleInvoiceReturnHeader.DivisionId, SaleInvoiceReturnHeader.SiteId);

        //    string[] contraDocTypes = null;
        //    if (!string.IsNullOrEmpty(settings.filterContraDocTypes)) { contraDocTypes = settings.filterContraDocTypes.Split(",".ToCharArray()); }
        //    else { contraDocTypes = new string[] { "NA" }; }

        //    string[] contraSites = null;
        //    if (!string.IsNullOrEmpty(settings.filterContraSites)) { contraSites = settings.filterContraSites.Split(",".ToCharArray()); }
        //    else { contraSites = new string[] { "NA" }; }

        //    string[] contraDivisions = null;
        //    if (!string.IsNullOrEmpty(settings.filterContraDivisions)) { contraDivisions = settings.filterContraDivisions.Split(",".ToCharArray()); }
        //    else { contraDivisions = new string[] { "NA" }; }

        //    int CurrentSiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
        //    int CurrentDivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];


        //    return (from p in db.ViewSaleInvoiceBalance
        //            join t in db.CustomHeader on p.CustomHeaderId equals t.CustomHeaderId into table
        //            from tab in table.DefaultIfEmpty()
        //            join t1 in db.SaleDispatchLine on p.SaleDispatchLineId equals t1.SaleDispatchLineId into table1
        //            from tab1 in table1.DefaultIfEmpty()
        //            join t2 in db.SaleInvoiceLine on p.SaleDispatchLineId equals t2.SaleDispatchLineId into InvoiceTable
        //            from InvTab in InvoiceTable.DefaultIfEmpty()
        //            where InvTab.ProductId == id && tab.SaleToBuyerId == SaleInvoiceReturnHeader.BuyerId && p.BalanceQty > 0
        //            && (string.IsNullOrEmpty(settings.filterContraSites) ? p.SiteId == CurrentSiteId : contraSites.Contains(p.SiteId.ToString()))
        //            && (string.IsNullOrEmpty(settings.filterContraDivisions) ? p.DivisionId == CurrentDivisionId : contraDivisions.Contains(p.DivisionId.ToString()))
        //            && (string.IsNullOrEmpty(term) ? 1 == 1 : p.SaleInvoiceNo.ToLower().Contains(term.ToLower()))
        //            select new SaleInvoiceListViewModel
        //            {
        //                SaleInvoiceLineId = p.SaleInvoiceLineId,
        //                CustomHeaderId = p.CustomHeaderId,
        //                DocNo = tab.DocNo,
        //                Dimension1Name = InvTab.Dimension1.Dimension1Name,
        //                Dimension2Name = InvTab.Dimension2.Dimension2Name,
        //            }
        //                );
        //}

        public IQueryable<ComboBoxResult> GetCustomPerson(int Id, string term)
        {
            int DocTypeId = Id;
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var DocTypeName = db.DocumentType.Find(DocTypeId).DocumentTypeName;

            var settings = new SaleInvoiceSettingService(_unitOfWork).GetSaleInvoiceSettingForDocument(DocTypeId, DivisionId, SiteId);

            string[] PersonRoles = null;
            if (!string.IsNullOrEmpty(settings.filterPersonRoles)) { PersonRoles = settings.filterPersonRoles.Split(",".ToCharArray()); }
            else { PersonRoles = new string[] { "NA" }; }

            string DivIdStr = "|" + DivisionId.ToString() + "|";
            string SiteIdStr = "|" + SiteId.ToString() + "|";

            var list = (from p in db.Persons
                        join bus in db.BusinessEntity on p.PersonID equals bus.PersonID into BusinessEntityTable
                        from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                        join pp in db.PersonProcess on p.PersonID equals pp.PersonId into PersonProcessTable
                        from PersonProcessTab in PersonProcessTable.DefaultIfEmpty()
                        join pr in db.PersonRole on p.PersonID equals pr.PersonId into PersonRoleTable
                        from PersonRoleTab in PersonRoleTable.DefaultIfEmpty()
                        where PersonProcessTab.ProcessId == settings.ProcessId
                        && (string.IsNullOrEmpty(term) ? 1 == 1 : (p.Name.ToLower().Contains(term.ToLower()) || p.Code.ToLower().Contains(term.ToLower())))
                        && (string.IsNullOrEmpty(settings.filterPersonRoles) ? 1 == 1 : PersonRoles.Contains(PersonRoleTab.RoleDocTypeId.ToString()))
                        && BusinessEntityTab.DivisionIds.IndexOf(DivIdStr) != -1
                        && BusinessEntityTab.SiteIds.IndexOf(SiteIdStr) != -1
                        && (p.IsActive == null ? 1 == 1 : p.IsActive == true)
                        group new { p } by new { p.PersonID } into Result
                        orderby Result.Max(m => m.p.Name)
                        select new ComboBoxResult
                        {
                            id = Result.Key.PersonID.ToString(),
                            text = Result.Max(m => m.p.Name + ", " + m.p.Suffix + " [" + m.p.Code + "]"),
                        }
              );

            return list;
        }


        public IEnumerable<DocumentTypeHeaderAttributeViewModel> GetDocumentHeaderAttribute(int id)
        {
            var Header = db.CustomHeader.Find(id);

            var temp = from Dta in db.DocumentTypeHeaderAttribute
                       join Ha in db.CustomHeaderAttributes on Dta.DocumentTypeHeaderAttributeId equals Ha.DocumentTypeHeaderAttributeId into HeaderAttributeTable
                       from HeaderAttributeTab in HeaderAttributeTable.Where(m => m.HeaderTableId == id).DefaultIfEmpty()
                       where (Dta.DocumentTypeId == Header.DocTypeId)
                       orderby Dta.Sr
                       select new DocumentTypeHeaderAttributeViewModel
                       {
                           ListItem = Dta.ListItem,
                           DataType = Dta.DataType,
                           Value = HeaderAttributeTab.Value,
                           Name = Dta.Name,
                           DocumentTypeHeaderAttributeId = Dta.DocumentTypeHeaderAttributeId,
                       };

            return temp;
        }

        //public string GetNarration(int CustomHeaderId)
        //{
        //    string Narration = "";

        //    var CustomHeader_Data = (from H in db.CustomHeader
        //                                  join L in db.SaleInvoiceLine on H.CustomHeaderId equals L.CustomHeaderId into SaleInvoiceLineTable
        //                                  from SaleInvoiceLineTab in SaleInvoiceLineTable.DefaultIfEmpty()
        //                                  where H.CustomHeaderId == CustomHeaderId
        //                                  select new
        //                                  {
        //                                      DocTypeId = H.DocTypeId,
        //                                      SaleToBuyerName = H.SaleToBuyer.Name,
        //                                      ProductName = SaleInvoiceLineTab.SaleDispatchLine.PackingLine.Product.ProductName,
        //                                      ProductUidName = SaleInvoiceLineTab.SaleDispatchLine.PackingLine.ProductUid.ProductUidName,
        //                                      ProductGroupName = SaleInvoiceLineTab.SaleDispatchLine.PackingLine.Product.ProductGroup.ProductGroupName,
        //                                      Qty = SaleInvoiceLineTab.Qty,
        //                                      Rate = SaleInvoiceLineTab.Rate,
        //                                      Amount = SaleInvoiceLineTab.Amount,
        //                                  }).FirstOrDefault();

        //    var CustomHeaderCharges_Data = (from Hc in db.CustomHeaderCharge
        //                                         where Hc.HeaderTableId == CustomHeaderId
        //                                         && Hc.Charge.ChargeName == "Net Amount"
        //                                         select new
        //                                         {
        //                                             NetAmount = Hc.Amount
        //                                         }).FirstOrDefault();



        //    if (CustomHeader_Data != null)
        //    {
        //        var Narration_Temp = (from H in db.Narration where H.DocTypeId == CustomHeader_Data.DocTypeId select new { Narration = H.NarrationName }).FirstOrDefault();

        //        if (Narration_Temp != null)
        //            Narration = Narration_Temp.Narration.Replace("<CustomerName>", CustomHeader_Data.SaleToBuyerName)
        //                                    .Replace("<ProductName>", CustomHeader_Data.ProductName)
        //                                    .Replace("<ProductGroupName>", CustomHeader_Data.ProductGroupName)
        //                                    .Replace("<ProductUidName>", CustomHeader_Data.ProductUidName)
        //                                    .Replace("<Qty>", CustomHeader_Data.Qty.ToString())
        //                                    .Replace("<Rate>", CustomHeader_Data.Rate.ToString())
        //                                    .Replace("<Amount>", CustomHeader_Data.Amount.ToString())
        //                                    .Replace("<NetAmount>", CustomHeaderCharges_Data.NetAmount.ToString());
        //    }
        //    return Narration;
        //}

    }
}
