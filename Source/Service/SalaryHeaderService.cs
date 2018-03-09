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
using Model.ViewModel;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity.SqlServer;
using System.Data.Linq;
using Model.DatabaseViews;
using Model.ViewModels;
using AutoMapper;

namespace Service
{
    public interface ISalaryHeaderService : IDisposable
    {
        SalaryHeader Create(SalaryHeader s);
        void Delete(int id);
        void Delete(SalaryHeader s);
        SalaryHeaderViewModel GetSalaryHeader(int id);
        SalaryHeader Find(int id);
        IQueryable<SalaryHeaderViewModel> GetSalaryHeaderList(int DocumentTypeId, string Uname);
        IQueryable<SalaryHeaderViewModel> GetSalaryHeaderListPendingToSubmit(int DocumentTypeId, string Uname);
        IQueryable<SalaryHeaderViewModel> GetSalaryHeaderListPendingToReview(int DocumentTypeId, string Uname);
        void Update(SalaryHeader s);
        string GetMaxDocNo();
    }
    public class SalaryHeaderService : ISalaryHeaderService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public SalaryHeaderService(IUnitOfWorkForService unit)
        {
            _unitOfWork = unit;
        }

        public SalaryHeader Create(SalaryHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SalaryHeader>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SalaryHeader>().Delete(id);
        }
        public void Delete(SalaryHeader s)
        {
            _unitOfWork.Repository<SalaryHeader>().Delete(s);
        }
        public void Update(SalaryHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SalaryHeader>().Update(s);
        }


        public SalaryHeader Find(int id)
        {
            return _unitOfWork.Repository<SalaryHeader>().Find(id);
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<SalaryHeader>().Query().Get().Select(i => i.DocNo).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }

        
        public IQueryable<SalaryHeaderViewModel> GetSalaryHeaderList(int DocumentTypeId, string Uname)
        {

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];      

            
            return (from p in db.SalaryHeader
                    join dt in db.DocumentType on p.DocTypeId equals dt.DocumentTypeId
                    orderby p.DocDate descending, p.DocNo descending
                    where p.DocTypeId == DocumentTypeId && p.DivisionId == DivisionId && p.SiteId == SiteId
                    select new SalaryHeaderViewModel
                    {
                        DocTypeName = dt.DocumentTypeName,
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        Remark = p.Remark,
                        Status = p.Status,
                        SalaryHeaderId = p.SalaryHeaderId,
                        ModifiedBy = p.ModifiedBy,
                        ReviewCount = p.ReviewCount,
                        ReviewBy = p.ReviewBy,
                        Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
                    });
        }

        public SalaryHeaderViewModel GetSalaryHeader(int id)
        {
            return (from H in db.SalaryHeader
                    where H.SalaryHeaderId == id
                    select new SalaryHeaderViewModel
                    {
                        DocTypeName = H.DocType.DocumentTypeName,
                        DocDate = H.DocDate,
                        DocNo = H.DocNo,
                        Remark = H.Remark,
                        SalaryHeaderId = H.SalaryHeaderId,
                        LedgerHeaderId = H.LedgerHeaderId,
                        Status = H.Status,
                        DocTypeId = H.DocTypeId,
                        DivisionId = H.DivisionId,
                        SiteId = H.SiteId,
                        LockReason = H.LockReason,
                        ModifiedBy = H.ModifiedBy,
                        CreatedDate = H.CreatedDate
                    }).FirstOrDefault();
        }



        public IQueryable<SalaryHeaderViewModel> GetSalaryHeaderListPendingToSubmit(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var SalaryHeader = GetSalaryHeaderList(id, Uname).AsQueryable();

            var PendingToSubmit = from p in SalaryHeader
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Import || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }


        public IQueryable<SalaryHeaderViewModel> GetSalaryHeaderListPendingToReview(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var SalaryHeader = GetSalaryHeaderList(id, Uname).AsQueryable();

            var PendingToReview = from p in SalaryHeader
                                  where p.Status == (int)StatusConstants.Submitted && (SqlFunctions.CharIndex(Uname, (p.ReviewBy ?? "")) == 0)
                                  select p;
            return PendingToReview;

        }

        public void Dispose()
        {
        }
    }
}
