using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Infrastructure;
using Model.Models;

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
    public interface IEmployeeLineChargeService : IDisposable
    {
        EmployeeLineCharge Create(EmployeeLineCharge pt);
        void Delete(int id);
        void Delete(EmployeeLineCharge pt);
        EmployeeLineCharge Find(int id);
        IEnumerable<EmployeeLineCharge> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(EmployeeLineCharge pt);
        EmployeeLineCharge Add(EmployeeLineCharge pt);
        IEnumerable<EmployeeLineCharge> GetEmployeeLineChargeList();

        // IEnumerable<EmployeeLineCharge> GetEmployeeLineChargeList(int buyerId);
        Task<IEquatable<EmployeeLineCharge>> GetAsync();
        Task<EmployeeLineCharge> FindAsync(int id);
        IEnumerable<LineChargeViewModel> GetCalculationProductList(int LineTableId);
        IEnumerable<LineChargeViewModel> GetCalculationProductListSProc(int LineTableId, string LineTableName);
        int NextId(int id);
        int PrevId(int id);
        int GetMaxProductCharge(int HeaderId, string LineTableName, string HeaderFieldName, string LineTableKeyField);
    }

    public class EmployeeLineChargeService : IEmployeeLineChargeService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<EmployeeLineCharge> _EmployeeLineChargeRepository;
        RepositoryQuery<EmployeeLineCharge> EmployeeLineChargeRepository;
        public EmployeeLineChargeService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public EmployeeLineCharge Find(int id)
        {
            return (_unitOfWork.Repository<EmployeeLineCharge>().Find(id));
        }

        public EmployeeLineCharge Create(EmployeeLineCharge pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<EmployeeLineCharge>().Insert(pt);

            return pt;
        }
        public IEnumerable<LineChargeViewModel> GetCalculationProductList(int LineTableId)
        {
            return (from p in db.EmployeeLineCharge
                    where p.LineTableId== LineTableId
                    orderby p.Sr
                    select new LineChargeViewModel
                    {
                        AddDeduct = p.AddDeduct,
                        AffectCost = p.AffectCost,
                        CalculateOnId = p.CalculateOnId,
                        CalculateOnName = p.CalculateOn.ChargeName,
                        CalculateOnCode = p.CalculateOn.ChargeCode,                       
                        LineTableId=p.LineTableId,
                        Id = p.Id,
                        ChargeId = p.ChargeId,
                        ChargeName = p.Charge.ChargeName,
                        ChargeCode = p.Charge.ChargeCode,
                        ChargeTypeId = p.ChargeTypeId,
                        ChargeTypeName = p.ChargeType.ChargeTypeName,
                        CostCenterId = p.CostCenterId,
                        CostCenterName = p.CostCenter.CostCenterName,
                        IncludedInBase = p.IncludedInBase,
                        LedgerAccountCrId = p.LedgerAccountCrId,
                        LedgerAccountCrName = p.LedgerAccountCr.LedgerAccountName,
                        LedgerAccountDrId = p.LedgerAccountDrId,
                        LedgerAccountDrName = p.LedgerAccountDr.LedgerAccountName,
                        Rate = p.Rate,
                        Sr = p.Sr,
                        RateType = p.RateType,
                        IsVisible = p.IsVisible,
                        Amount = p.Amount,
                        ParentChargeId = p.ParentChargeId,                      

                    }
                      );
        }

        public int GetMaxProductCharge(int HeaderId,string LineTableName,string HeaderFieldName,string LineTableKeyField)
        {
            //var temp=from p in db.EmployeeLineCharge
            //        where p.LineTableId == (from p1 in db.EmployeeLine
            //                                orderby p1.EmployeeLineId descending
            //                                    where p1.JobOrderHeaderId==HeaderId
            //                                    select p1.EmployeeLineId
            //                                    ).FirstOrDefault()
            //        orderby p.Sr
            //        select new LineChargeViewModel
            //        {
            //            AddDeduct = p.AddDeduct,
            //            AffectCost = p.AffectCost,
            //            CalculateOnId = p.CalculateOnId,
            //            CalculateOnName = p.CalculateOn.ChargeName,
            //            CalculateOnCode = p.CalculateOn.ChargeCode,
            //            LineTableId = p.LineTableId,
            //            Id = p.Id,
            //            ChargeId = p.ChargeId,
            //            ChargeName = p.Charge.ChargeName,
            //            ChargeCode = p.Charge.ChargeCode,
            //            ChargeTypeId = p.ChargeTypeId,
            //            ChargeTypeName = p.ChargeType.ChargeTypeName,
            //            CostCenterId = p.CostCenterId,
            //            CostCenterName = p.CostCenter.CostCenterName,
            //            IncludedInBase = p.IncludedInBase,
            //            LedgerAccountCrId = p.LedgerAccountCrId,
            //            LedgerAccountCrName = p.LedgerAccountCr.LedgerAccountName,
            //            LedgerAccountDrId = p.LedgerAccountDrId,
            //            LedgerAccountDrName = p.LedgerAccountDr.LedgerAccountName,
            //            Rate = p.Rate,
            //            Sr = p.Sr,
            //            RateType = p.RateType,
            //            IsVisible = p.IsVisible,
            //            Amount = 0,
            //            ParentChargeId = p.ParentChargeId,
            //        };
            //return temp;

            SqlParameter SqlParameterHeaderTableId = new SqlParameter("@HeaderTableKeyValue", HeaderId);
            SqlParameter SqlParameterLineTableName = new SqlParameter("@LineTableName", LineTableName);
            SqlParameter SqlParameterHeaderFieldName = new SqlParameter("@HeaderFieldName", HeaderFieldName);
            SqlParameter SqlParameterLineTableFieldId = new SqlParameter("@LineTableKeyField", LineTableKeyField);

            int ? CalculationLineList = Convert.ToInt32(db.Database.SqlQuery<int ?>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".procGetCalculationMaxLineId @HeaderTableKeyValue, @LineTableName,@HeaderFieldName,@LineTableKeyField", SqlParameterHeaderTableId, SqlParameterLineTableName, SqlParameterHeaderFieldName, SqlParameterLineTableFieldId).FirstOrDefault());

            return CalculationLineList??0;

        }

        public IEnumerable<LineChargeViewModel> GetCalculationProductListSProc(int LineTableId,string LineTableName)
        {
            SqlParameter SqlParameterLineTableId = new SqlParameter("@LineTableld", LineTableId);
            SqlParameter SqlParameterLineTableName = new SqlParameter("@LineTableName", LineTableName);

            IEnumerable<LineChargeViewModel> CalculationLineList = db.Database.SqlQuery<LineChargeViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".CalculationLineCharge @LineTableld, @LineTableName", SqlParameterLineTableId, SqlParameterLineTableName).ToList();

            return CalculationLineList;
        }
        public void Delete(int id)
        {
            _unitOfWork.Repository<EmployeeLineCharge>().Delete(id);
        }

        public void Delete(EmployeeLineCharge pt)
        {
            _unitOfWork.Repository<EmployeeLineCharge>().Delete(pt);
        }

        public void Update(EmployeeLineCharge pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<EmployeeLineCharge>().Update(pt);
        }

        public IEnumerable<EmployeeLineCharge> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<EmployeeLineCharge>()
                .Query()               
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<EmployeeLineCharge> GetEmployeeLineChargeList()
        {
            var pt = _unitOfWork.Repository<EmployeeLineCharge>().Query().Get().OrderBy(m=>m.LineTableId);

            return pt;
        }

        public EmployeeLineCharge Add(EmployeeLineCharge pt)
        {
            _unitOfWork.Repository<EmployeeLineCharge>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.EmployeeLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.EmployeeLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).FirstOrDefault();
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

                temp = (from p in db.EmployeeLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.EmployeeLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<EmployeeLineCharge>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeLineCharge> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
