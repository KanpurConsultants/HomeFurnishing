﻿using System.Collections.Generic;
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
            var temp = (from p in db.SalaryLine
                        where p.SalaryLineId == id
                        select new SalaryLineViewModel
                        {
                            Remark = p.Remark,
                            SalaryLineId = p.SalaryLineId,
                            LockReason = p.LockReason,
                        }).FirstOrDefault();

            return temp;
        }


        public SalaryLine Find(int id)
        {
            return _unitOfWork.Repository<SalaryLine>().Find(id);
        }



        public IQueryable<SalaryLineViewModel> GetSalaryLineListForIndex(int SalaryHeaderId)
        {
            var temp = from p in db.SalaryLine
                       where p.SalaryHeaderId == SalaryHeaderId
                       orderby p.Sr
                       select new SalaryLineViewModel
                       {
                           EmployeeName = p.Employee.Person.Name,
                           NetSalary = p.NetSalary,
                           Remark = p.Remark,
                           SalaryHeaderId = p.SalaryHeaderId,
                           SalaryLineId = p.SalaryLineId,
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