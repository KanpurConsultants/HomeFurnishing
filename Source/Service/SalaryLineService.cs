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
using Model.ViewModel;
using System.Data.SqlClient;
using System.Configuration;


namespace Service
{
    public interface ISalaryLineService : IDisposable
    {
        SalaryLine Create(SalaryLine s);
        void Delete(int id);
        void Delete(SalaryLine s);
        SalaryLineViewModel GetSalaryLine(int id);
        SalaryLine Find(int id);
        void Update(SalaryLine s);
        IQueryable<SalaryLineViewModel> GetSalaryLineListForIndex(int SalaryHeaderId);
        IEnumerable<SalaryLineViewModel> GetSalaryLineforDelete(int headerid);
        IQueryable<SalaryLineReferenceIndexViewModel> GetSalaryLineReferenceList(int SalaryLineId);

        int GetMaxSr(int id);

    }

    public class SalaryLineService : ISalaryLineService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;

        public SalaryLineService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public SalaryLine Create(SalaryLine S)
        {
            S.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SalaryLine>().Insert(S);
            return S;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SalaryLine>().Delete(id);
        }

        public void Delete(SalaryLine s)
        {
            _unitOfWork.Repository<SalaryLine>().Delete(s);
        }

        public void Update(SalaryLine s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SalaryLine>().Update(s);
        }


        public SalaryLineViewModel GetSalaryLine(int id)
        {
            var temp = (from S in db.SalaryLine
                        join P in db.Persons on S.EmployeeId equals P.PersonID into PTable
                        from PTab in PTable.DefaultIfEmpty()
                        where S.SalaryLineId == id
                        select new SalaryLineViewModel
                        {
                            SalaryLineId = S.SalaryLineId,
                            SalaryHeaderId = S.SalaryHeaderId,
                            EmployeeId = S.EmployeeId,
                            EmployeeName = PTab.Name,
                            Days = S.Days,
                            OtherAddition = S.OtherAddition,
                            OtherDeduction = S.OtherDeduction,
                            LoanEMI = S.LoanEMI,
                            Advance = S.Advance,
                            NetPayable = S.NetPayable,
                            Remark = S.Remark,
                            LockReason = S.LockReason,
                        }).FirstOrDefault();

            return temp;
        }


        public SalaryLine Find(int id)
        {
            return _unitOfWork.Repository<SalaryLine>().Find(id);
        }



        public IQueryable<SalaryLineViewModel> GetSalaryLineListForIndex(int SalaryHeaderId)
        {
            var temp = from S in db.SalaryLine
                       join P in db.Persons on S.EmployeeId equals P.PersonID into PTable
                       from PTab in PTable.DefaultIfEmpty()
                       where S.SalaryHeaderId == SalaryHeaderId
                       orderby S.Sr
                       select new SalaryLineViewModel
                       {
                           EmployeeName = PTab.Name,
                           Days = S.Days,
                           BasicSalary=S.BasicSalary,
                           OtherAddition = S.OtherAddition,
                           OtherDeduction = S.OtherDeduction,
                           LoanEMI = S.LoanEMI,
                           Advance = S.Advance,
                           NetPayable = S.NetPayable,
                           Remark = S.Remark,
                           SalaryHeaderId = S.SalaryHeaderId,
                           SalaryLineId = S.SalaryLineId,
                       };
            return temp;
        }

        public IQueryable<SalaryLineReferenceIndexViewModel> GetSalaryLineReferenceList( int SalaryLineId)
        {

            var temp = from S in db.SalaryLineReference
                       join L in db.SalaryLine on S.SalaryLineId equals L.SalaryLineId into LTable
                       from LTab in LTable.DefaultIfEmpty()
                       join H in db.SalaryHeader on LTab.SalaryHeaderId equals H.SalaryHeaderId into HTable
                       from HTab in HTable.DefaultIfEmpty()
                       join P in db.Persons on LTab.EmployeeId equals P.PersonID into PTable
                       from PTab in PTable.DefaultIfEmpty()
                       join DT in db.DocumentType  on S.ReferenceDocTypeId equals DT.DocumentTypeId into DTTable
                       from DTTab in DTTable.DefaultIfEmpty()
                       join LL in db.LedgerLine on S.ReferenceDocLineId equals LL.LedgerLineId into LLTable
                       from LLTab in LLTable.DefaultIfEmpty()
                       join LH in db.LedgerHeader on S.ReferenceDocId equals LH.LedgerHeaderId into LHTable
                       from LHTab in LHTable.DefaultIfEmpty()
                       where S.SalaryLineId == SalaryLineId 
                       select new SalaryLineReferenceIndexViewModel
                       {
                           ReferenceDocLineId  = S.ReferenceDocLineId,
                           ReferenceDocId = S.ReferenceDocId,
                           ReferenceDocTypeId = S.ReferenceDocTypeId,
                           SalaryLineId = S.SalaryLineId,
                            DocumentTypeName=DTTab.DocumentTypeName,
                            PersonName=PTab.Name,
                            SalaryHeaderDocNo=HTab.DocNo,
                            DocumentNo =LHTab.DocNo,

                       };

                return temp;

        }


        public IEnumerable<SalaryLineViewModel> GetSalaryLineforDelete(int headerid)
        {
            return (from p in db.SalaryLine
                    where p.SalaryHeaderId == headerid
                    select new SalaryLineViewModel
                    {
                        SalaryLineId = p.SalaryLineId,
                    });
        }

        public int GetMaxSr(int id)
        {
            var Max = (from p in db.SalaryLine
                       where p.SalaryHeaderId == id
                       select p.Sr
                        );

            if (Max.Count() > 0)
                return Max.Max(m => m) + 1;
            else
                return (1);
        }

        public void Dispose()
        {
        }
    }


}
