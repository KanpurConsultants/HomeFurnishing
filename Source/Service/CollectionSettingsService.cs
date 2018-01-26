using System.Collections.Generic;
using System.Linq;
using System;
using Model;
using System.Threading.Tasks;
using Data.Infrastructure;
using Model.Models;

namespace Services.PropertyTax
{
    public interface ICollectionSettingsService : IDisposable
    {
        CollectionSettings Create(CollectionSettings pt);
        void Delete(int id);
        void Delete(CollectionSettings pt);
        CollectionSettings Find(int id);
        IEnumerable<CollectionSettings> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(CollectionSettings pt);
        CollectionSettings Add(CollectionSettings pt);
        CollectionSettings GetCollectionSettingsForDocument(int DocTypeId);
        Task<IEquatable<CollectionSettings>> GetAsync();
        Task<CollectionSettings> FindAsync(int id);
        int NextId(int id);
        int PrevId(int id);
    }

    public class CollectionSettingsService : ICollectionSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CollectionSettings> _CollectionSettingsRepository;
        public CollectionSettingsService(IUnitOfWork unitOfWork, IRepository<CollectionSettings> CollectionsettingsRepo)
        {
            _unitOfWork = unitOfWork;
            _CollectionSettingsRepository = CollectionsettingsRepo;
        }
        public CollectionSettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _CollectionSettingsRepository = unitOfWork.Repository<CollectionSettings>();
        }

        public CollectionSettings Find(int id)
        {
            return _unitOfWork.Repository<CollectionSettings>().Find(id);
        }

        public CollectionSettings GetCollectionSettingsForDocument(int DocTypeId)
        {
            return (from p in _CollectionSettingsRepository.Instance
                    where p.DocTypeId == DocTypeId 
                    select p
                        ).FirstOrDefault();


        }
        public CollectionSettings Create(CollectionSettings pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<CollectionSettings>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<CollectionSettings>().Delete(id);
        }

        public void Delete(CollectionSettings pt)
        {
            _unitOfWork.Repository<CollectionSettings>().Delete(pt);
        }

        public void Update(CollectionSettings pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<CollectionSettings>().Update(pt);
        }

        public IEnumerable<CollectionSettings> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<CollectionSettings>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.CollectionSettingsId))
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public CollectionSettings Add(CollectionSettings pt)
        {
            _unitOfWork.Repository<CollectionSettings>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in _CollectionSettingsRepository.Instance
                        orderby p.CollectionSettingsId
                        select p.CollectionSettingsId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in _CollectionSettingsRepository.Instance
                        orderby p.CollectionSettingsId
                        select p.CollectionSettingsId).FirstOrDefault();
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

                temp = (from p in _CollectionSettingsRepository.Instance
                        orderby p.CollectionSettingsId
                        select p.CollectionSettingsId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in _CollectionSettingsRepository.Instance
                        orderby p.CollectionSettingsId
                        select p.CollectionSettingsId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }


        public void Dispose()
        {
            _unitOfWork.Dispose();
        }


        public Task<IEquatable<CollectionSettings>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CollectionSettings> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
