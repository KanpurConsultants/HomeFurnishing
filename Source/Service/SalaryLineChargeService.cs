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
    public interface ISalaryLineChargeService : IDisposable
    {
        SalaryLineCharge Create(SalaryLineCharge pt);
        void Delete(int id);
        void Delete(SalaryLineCharge pt);
        SalaryLineCharge Find(int id);
        IEnumerable<SalaryLineCharge> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(SalaryLineCharge pt);
        SalaryLineCharge Add(SalaryLineCharge pt);
        IEnumerable<SalaryLineCharge> GetSalaryLineChargeList();

        // IEnumerable<SalaryLineCharge> GetSalaryLineChargeList(int buyerId);
        Task<IEquatable<SalaryLineCharge>> GetAsync();
        Task<SalaryLineCharge> FindAsync(int id);
        IEnumerable<LineChargeViewModel> GetCalculationProductList(int LineTableId);
        IEnumerable<LineChargeViewModel> GetCalculationProductListSProc(int LineTableId, string LineTableName);
        int NextId(int id);
        int PrevId(int id);
        int GetMaxProductCharge(int HeaderId, string LineTableName, string HeaderFieldName, string LineTableKeyField);
    }

    public class SalaryLineChargeService : ISalaryLineChargeService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<SalaryLineCharge> _SalaryLineChargeRepository;
        RepositoryQuery<SalaryLineCharge> SalaryLineChargeRepository;
        public SalaryLineChargeService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public SalaryLineCharge Find(int id)
        {
            return (_unitOfWork.Repository<SalaryLineCharge>().Find(id));
        }

        public SalaryLineCharge Create(SalaryLineCharge pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SalaryLineCharge>().Insert(pt);

            return pt;
        }
        public IEnumerable<LineChargeViewModel> GetCalculationProductList(int LineTableId)
        {
            return (from p in db.SalaryLineCharge
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
            //var temp=from p in db.SalaryLineCharge
            //        where p.LineTableId == (from p1 in db.SalaryLine
            //                                orderby p1.SalaryLineId descending
            //                                    where p1.SalaryHeaderId==HeaderId
            //                                    select p1.SalaryLineId
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
            _unitOfWork.Repository<SalaryLineCharge>().Delete(id);
        }

        public void Delete(SalaryLineCharge pt)
        {
            _unitOfWork.Repository<SalaryLineCharge>().Delete(pt);
        }

        public void Update(SalaryLineCharge pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SalaryLineCharge>().Update(pt);
        }

        public IEnumerable<SalaryLineCharge> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<SalaryLineCharge>()
                .Query()               
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<SalaryLineCharge> GetSalaryLineChargeList()
        {
            var pt = _unitOfWork.Repository<SalaryLineCharge>().Query().Get().OrderBy(m=>m.LineTableId);

            return pt;
        }

        public SalaryLineCharge Add(SalaryLineCharge pt)
        {
            _unitOfWork.Repository<SalaryLineCharge>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.SalaryLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.SalaryLineCharge
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

                temp = (from p in db.SalaryLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.SalaryLineCharge
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


        public Task<IEquatable<SalaryLineCharge>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SalaryLineCharge> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
