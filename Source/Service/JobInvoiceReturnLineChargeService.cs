﻿using System.Collections.Generic;
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
    public interface IJobInvoiceReturnLineChargeService : IDisposable
    {
        JobInvoiceReturnLineCharge Create(JobInvoiceReturnLineCharge pt);
        void Delete(int id);
        void Delete(JobInvoiceReturnLineCharge pt);
        JobInvoiceReturnLineCharge Find(int id);
        void Update(JobInvoiceReturnLineCharge pt);
        // IEnumerable<JobInvoiceReturnLineCharge> GetJobInvoiceReturnLineChargeList(int buyerId);
        Task<IEquatable<JobInvoiceReturnLineCharge>> GetAsync();
        Task<JobInvoiceReturnLineCharge> FindAsync(int id);
        IEnumerable<LineChargeViewModel> GetCalculationProductList(int LineTableId);
        IEnumerable<LineChargeViewModel> GetCalculationProductListSProc(int LineTableId, string LineTableName);
        int NextId(int id);
        int PrevId(int id);
        int GetMaxProductCharge(int HeaderId, string LineTableName, string HeaderFieldName, string LineTableKeyField);
    }

    public class JobInvoiceReturnLineChargeService : IJobInvoiceReturnLineChargeService
    {
        ApplicationDbContext db;
        public JobInvoiceReturnLineChargeService(ApplicationDbContext db)
        {
            this.db = db;
        }


        public JobInvoiceReturnLineCharge Find(int id)
        {
            return (db.JobInvoiceReturnLineCharge.Find(id));
        }

        public JobInvoiceReturnLineCharge Create(JobInvoiceReturnLineCharge pt)
        {
            pt.ObjectState = ObjectState.Added;
            db.JobInvoiceReturnLineCharge.Add(pt);

            return pt;
        }
        public IEnumerable<LineChargeViewModel> GetCalculationProductList(int LineTableId)
        {
            return (from p in db.JobInvoiceReturnLineCharge
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
            JobInvoiceReturnLineCharge temp = db.JobInvoiceReturnLineCharge.Find(id);
            temp.ObjectState = Model.ObjectState.Deleted;
            db.JobInvoiceReturnLineCharge.Remove(temp);
        }

        public void Delete(JobInvoiceReturnLineCharge pt)
        {
            pt.ObjectState = Model.ObjectState.Deleted;
            db.JobInvoiceReturnLineCharge.Remove(pt);
        }

        public void Update(JobInvoiceReturnLineCharge pt)
        {
            pt.ObjectState = ObjectState.Modified;
            db.JobInvoiceReturnLineCharge.Add(pt);
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.JobInvoiceReturnLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.JobInvoiceReturnLineCharge
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

                temp = (from p in db.JobInvoiceReturnLineCharge
                        orderby p.LineTableId
                        select p.LineTableId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.JobInvoiceReturnLineCharge
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


        public Task<IEquatable<JobInvoiceReturnLineCharge>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<JobInvoiceReturnLineCharge> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
