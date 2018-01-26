using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity.SqlServer;
using Components.Logging;
using AutoMapper;
using System.Xml.Linq;
using System.Data;
using Data.Infrastructure;
using Model.Models;

namespace Service
{
    public interface IDimension1ExtendedService : IDisposable
    {
        Dimension1Extended Create(Dimension1Extended pt);
        void Delete(int id);
        void Delete(Dimension1Extended pt);
        Dimension1Extended Find(int id);
        IEnumerable<Dimension1Extended> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Dimension1Extended pt);
        Dimension1Extended Add(Dimension1Extended pt);
        IEnumerable<Dimension1Extended> GetDimension1ExtendedList();
        IQueryable<Dimension1Extended> GetDimension1ExtendedListForIndex();

        
    }

    public class Dimension1ExtendedService : IDimension1ExtendedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Dimension1Extended> _Dimension1ExtendedRepository;
        public Dimension1ExtendedService(IUnitOfWork unitOfWork, IRepository<Dimension1Extended> Dimension1ExtendedRepo)
        {
            _unitOfWork = unitOfWork;
            _Dimension1ExtendedRepository = Dimension1ExtendedRepo;
        }
        public Dimension1ExtendedService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _Dimension1ExtendedRepository = unitOfWork.Repository<Dimension1Extended>();
        }


        public Dimension1Extended Find(int id)
        {
            return _unitOfWork.Repository<Dimension1Extended>().Find(id);
        }

        public Dimension1Extended Create(Dimension1Extended pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Dimension1Extended>().Add(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Dimension1Extended>().Delete(id);
        }

        public void Delete(Dimension1Extended pt)
        {
            _unitOfWork.Repository<Dimension1Extended>().Delete(pt);
        }

        public void Update(Dimension1Extended pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Dimension1Extended>().Update(pt);
        }

        public IEnumerable<Dimension1Extended> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Dimension1Extended>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.Dimension1Id))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Dimension1Extended> GetDimension1ExtendedList()
        {
            var pt = _unitOfWork.Repository<Dimension1Extended>().Query().Get().OrderBy(m => m.Dimension1Id);

            return pt;
        }

        public IQueryable<Dimension1Extended> GetDimension1ExtendedListForIndex()
        {
            var pt = _unitOfWork.Repository<Dimension1Extended>().Query().Get().OrderBy(m => m.Dimension1Id);

            return pt;
        }

        public Dimension1Extended Add(Dimension1Extended pt)
        {
            _unitOfWork.Repository<Dimension1Extended>().Insert(pt);
            return pt;
        }






        public void Dispose()
        {
            _unitOfWork.Dispose();
        }


       
    }
}
