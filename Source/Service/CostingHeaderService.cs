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
    public interface ICostingHeaderService : IDisposable
    {
        CostingHeader Create(CostingHeader s);
        void Delete(int id);
        void Delete(CostingHeader s);
        CostingHeader GetCostingHeader(int id);

        CostingHeaderIndexViewModel GetCostingHeaderVM(int id);
        CostingHeader Find(int id);
        IQueryable<CostingHeaderIndexViewModel> GetCostingHeaderList(int id, string Uname);
        IQueryable<CostingHeaderIndexViewModel> GetCostingHeaderListPendingToSubmit(int id, string Uname);
        IQueryable<CostingHeaderIndexViewModel> GetCostingHeaderListPendingToReview(int id, string Uname);
        void Update(CostingHeader s);
        string GetMaxDocNo();
        CostingHeader FindByDocNo(string Docno);
        IEnumerable<CostingHeader> GetCostingListForReport(int BuyerId);
        int NextId(int id);
        int PrevId(int id);
        //IEnumerable<CostingLineListViewModel> GetCostingsForDocumentType(int HeaderId, string term);//DoctypeIds
        IQueryable<ComboBoxResult> GetCustomPerson(int Id, string term);
    }
    public class CostingHeaderService : ICostingHeaderService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public CostingHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

        public CostingHeader Create(CostingHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<CostingHeader>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<CostingHeader>().Delete(id);
        }
        public void Delete(CostingHeader s)
        {
            _unitOfWork.Repository<CostingHeader>().Delete(s);
        }
        public void Update(CostingHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<CostingHeader>().Update(s);
        }

        public CostingHeader GetCostingHeader(int id)
        {
            return _unitOfWork.Repository<CostingHeader>().Query().Get().Where(m => m.CostingHeaderId == id).FirstOrDefault();
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {

                temp = (from p in db.CostingHeader
                        join t in db.Persons on p.PersonId equals t.PersonID
                        orderby p.DocDate descending, p.DocNo descending
                        select p.CostingHeaderId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();


            }
            else
            {
                temp = (from p in db.CostingHeader
                        join t in db.Persons on p.PersonId equals t.PersonID
                        orderby p.DocDate descending, p.DocNo descending
                        select p.CostingHeaderId).FirstOrDefault();
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

                temp = (from p in db.CostingHeader
                        join t in db.Persons on p.PersonId equals t.PersonID
                        orderby p.DocDate descending, p.DocNo descending
                        select p.CostingHeaderId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.CostingHeader
                        join t in db.Persons on p.PersonId equals t.PersonID
                        orderby p.DocDate descending, p.DocNo descending
                        select p.CostingHeaderId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public CostingHeaderIndexViewModel GetCostingHeaderVM(int id)
        {

            CostingHeaderIndexViewModel temp = (from p in db.CostingHeader
                                                  join t in db.Persons on p.PersonId equals t.PersonID into table
                                                  from tab in table.DefaultIfEmpty()
                                                  where p.CostingHeaderId == id
                                                  select new CostingHeaderIndexViewModel
                                                  {
                                                      DocTypeId = p.DocTypeId,
                                                      CreatedBy = p.CreatedBy,
                                                      CreatedDate = p.CreatedDate,
                                                      DivisionName = p.Division.DivisionName,
                                                      DocDate = p.DocDate,
                                                      DocNo = p.DocNo,
                                                      ModifiedBy = p.ModifiedBy,
                                                      ModifiedDate = p.ModifiedDate,
                                                      Remark = p.Remark,
                                                      CostingHeaderId = p.CostingHeaderId,
                                                      PersonName = tab.Name,
                                                      SiteName = p.Site.SiteName,
                                                      SiteId = p.SiteId,
                                                      DivisionId = p.DivisionId,
                                                      Status = p.Status,
                                                      DocumentTypeName = p.DocType.DocumentTypeName,

                                                  }

                ).FirstOrDefault();
            return temp;
        }

        public CostingHeader Find(int id)
        {
            return _unitOfWork.Repository<CostingHeader>().Find(id);
        }
        

        public IQueryable<CostingHeaderIndexViewModel> GetCostingHeaderList(int id, string Uname)
        {
            int divisionid = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            int siteid = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var temp = from p in db.CostingHeader
                       join t in db.Persons on p.PersonId equals t.PersonID
                       orderby p.DocDate descending, p.DocNo descending
                       where p.DivisionId == divisionid && p.SiteId == siteid && p.DocTypeId == id
                       select new CostingHeaderIndexViewModel
                       {
                           Remark = p.Remark,
                           DocDate = p.DocDate,
                           CostingHeaderId = p.CostingHeaderId,
                           DocNo = p.DocNo,
                           PersonName=t.Name,
                           Status = p.Status,
                           ModifiedBy = p.ModifiedBy,
                           ReviewCount = p.ReviewCount,
                           ReviewBy = p.ReviewBy,
                           Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
                       };
            return temp;
        }

        public IQueryable<CostingHeaderIndexViewModel> GetCostingHeaderListPendingToSubmit(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var LedgerHeader = GetCostingHeaderList(id, Uname).AsQueryable();

            var PendingToSubmit = from p in LedgerHeader
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Import || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }

        public IQueryable<CostingHeaderIndexViewModel> GetCostingHeaderListPendingToReview(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var LedgerHeader = GetCostingHeaderList(id, Uname).AsQueryable();

            var PendingToReview = from p in LedgerHeader
                                  where p.Status == (int)StatusConstants.Submitted && (SqlFunctions.CharIndex(Uname, (p.ReviewBy ?? "")) == 0)
                                  select p;
            return PendingToReview;

        }


        //public IEnumerable<CostingLineListViewModel> GetCostings(int ProductId, int BuyerId)//Product Id
        //{
        //    var tem = from p in db.CostingHeader
        //              join t in db.CostingLine on p.CostingHeaderId equals t.CostingHeaderId into table
        //              from tab in table.DefaultIfEmpty()

        //              where tab.ProductId == ProductId && p.PersonId == BuyerId
        //              orderby p.DocNo
        //              select new CostingLineListViewModel
        //              {
        //                  DocNo = p.DocNo,
        //                  CostingLineId = tab.CostingLineId,
        //              };

        //    return (tem);
        //}

        public CostingHeader FindByDocNo(string Docno)
        {
            return _unitOfWork.Repository<CostingHeader>().Query().Get().Where(m => m.DocNo == Docno).FirstOrDefault();

        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<CostingHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }
        public void Dispose()
        {
        }

        //public IEnumerable<CostingPrintViewModel> FGetPrintData(int Id)
        //{
        //    ApplicationDbContext Db = new ApplicationDbContext();
        //    IEnumerable<CostingPrintViewModel> Costingprintviewmodel = db.Database.SqlQuery<CostingPrintViewModel>(Db.strSchemaName + ".ProcCostingPrint @Id", new SqlParameter("@Id", Id)).ToList();
        //    return Costingprintviewmodel;
        //}

        public IEnumerable<CostingHeader> GetCostingListForReport(int BuyerId)
        {
            return _unitOfWork.Repository<CostingHeader>().Query().Include(m => m.DocType).Get().Where(m => m.PersonId == BuyerId);
        }

        public IEnumerable<CostingHeader> GetCostingListFromIds(String StrCostingIdsList)
        {
            string[] strarr = StrCostingIdsList.Split(',');
            int[] CostingListArr = Array.ConvertAll(strarr, s => int.Parse(s));

            var p = (from H in db.CostingHeader where CostingListArr.Contains(H.CostingHeaderId) select H).ToList();

            return p;


            //return _unitOfWork.Repository<CostingHeader>().Query().Get().Where(CostingListArr.Contains(m => m.CostingHeaderId));
        }


        //public IEnumerable<CostingLineListViewModel> GetCostingsForDocumentType(int HeaderId, string term)
        //{
            //return (from p in db.CostingHeader
            //        where DocTypeIds.Contains(p.DocTypeId.ToString())
            //        orderby p.DocDate descending, p.DocNo descending
            //        select new CostingLineListViewModel
            //        {
            //            DocNo = p.DocNo,
            //            CostingHeaderId = p.CostingHeaderId
            //        }
            //            );

            //var Header = new MaterialPlanHeaderService(_unitOfWork).Find(HeaderId);

            //var Settings = new MaterialPlanSettingsService(_unitOfWork).GetMaterialPlanSettingsForDocument(Header.DocTypeId, Header.DivisionId, Header.SiteId);

            //SqlParameter SqlParameterDocType = new SqlParameter("@PlanningDocumentType", Header.DocTypeId);
            //SqlParameter SqlParameterSite = new SqlParameter("@Site", Header.SiteId);
            //SqlParameter SqlParameterDivision = new SqlParameter("@Division", Header.DivisionId);
            //SqlParameter SqlParameterBuyer = new SqlParameter("@BuyerId", Header.BuyerId.HasValue ? Header.BuyerId : (object)DBNull.Value);

            //string ProcName = Settings.PendingProdOrderList;
            //if (string.IsNullOrEmpty(ProcName))
            //    throw new Exception("Pending ProdOrders not configured");

            //IEnumerable<PendingCostingFromProc> CalculationLineList = db.Database.SqlQuery<PendingCostingFromProc>("" + ProcName + " @PlanningDocumentType, @Site, @Division, @BuyerId", SqlParameterDocType, SqlParameterSite, SqlParameterDivision, SqlParameterBuyer).ToList();

            //var list = (from p in CalculationLineList
            //            where p.CostingNo.ToLower().Contains(term.ToLower())
            //            group new { p } by p.CostingHeaderId into g
            //            select new CostingLineListViewModel
            //            {
            //                DocNo = g.Max(m => m.p.CostingNo),
            //                CostingHeaderId = g.Key
            //            }
            //              );

        //    return list.ToList();
        //}

        public IQueryable<ComboBoxResult> GetCustomPerson(int Id, string term)
        {
            int DocTypeId = Id;
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            //var settings = new CostingSettingsService(_unitOfWork).GetCostingSettingsForDocument(DocTypeId, DivisionId, SiteId);

            //string[] PersonRoles = null;
            //if (!string.IsNullOrEmpty(settings.filterPersonRoles)) { PersonRoles = settings.filterPersonRoles.Split(",".ToCharArray()); }
            //else { PersonRoles = new string[] { "NA" }; }

            string DivIdStr = "|" + DivisionId.ToString() + "|";
            string SiteIdStr = "|" + SiteId.ToString() + "|";

            var list = (from p in db.Persons
                        join bus in db.BusinessEntity on p.PersonID equals bus.PersonID into BusinessEntityTable
                        from BusinessEntityTab in BusinessEntityTable.DefaultIfEmpty()
                        join pp in db.PersonProcess on p.PersonID equals pp.PersonId into PersonProcessTable
                        from PersonProcessTab in PersonProcessTable.DefaultIfEmpty()
                        join pr in db.PersonRole on p.PersonID equals pr.PersonId into PersonRoleTable
                        from PersonRoleTab in PersonRoleTable.DefaultIfEmpty()
                        where 1 == 1
                        && (string.IsNullOrEmpty(term) ? 1 == 1 : (p.Name.ToLower().Contains(term.ToLower()) || p.Code.ToLower().Contains(term.ToLower())))                        
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
    }
}
