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

namespace Service
{
    public interface IPaymentTermsService : IDisposable
    {
        PaymentTerms Create(PaymentTerms pt);
        void Delete(int id);
        void Delete(PaymentTerms pt);
        PaymentTerms Find(string Name);
        PaymentTerms Find(int id);
        IEnumerable<PaymentTerms> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(PaymentTerms pt);
        PaymentTerms Add(PaymentTerms pt);
        IEnumerable<PaymentTerms> GetPaymentTermsList();

        // IEnumerable<PaymentTerms> GetPaymentTermsList(int buyerId);
        Task<IEquatable<PaymentTerms>> GetAsync();
        Task<PaymentTerms> FindAsync(int id);
        PaymentTerms GetPaymentTermsByName(string terms);
        int NextId(int id);
        int PrevId(int id);
    }

    public class PaymentTermsService : IPaymentTermsService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<PaymentTerms> _PaymentTermsRepository;
        RepositoryQuery<PaymentTerms> PaymentTermsRepository;
        public PaymentTermsService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _PaymentTermsRepository = new Repository<PaymentTerms>(db);
            PaymentTermsRepository = new RepositoryQuery<PaymentTerms>(_PaymentTermsRepository);
        }
        public PaymentTerms GetPaymentTermsByName(string terms)
        {
            return (from p in db.PaymentTerms
                    where p.PaymentTermsName == terms
                    select p).FirstOrDefault();
        }

        public PaymentTerms Find(string Name)
        {
            return PaymentTermsRepository.Get().Where(i => i.PaymentTermsName == Name).FirstOrDefault();
        }


        public PaymentTerms Find(int id)
        {
            return _unitOfWork.Repository<PaymentTerms>().Find(id);
        }

        public PaymentTerms Create(PaymentTerms pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<PaymentTerms>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<PaymentTerms>().Delete(id);
        }

        public void Delete(PaymentTerms pt)
        {
            _unitOfWork.Repository<PaymentTerms>().Delete(pt);
        }

        public void Update(PaymentTerms pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<PaymentTerms>().Update(pt);
        }

        public IEnumerable<PaymentTerms> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<PaymentTerms>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.PaymentTermsName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<PaymentTerms> GetPaymentTermsList()
        {
            var pt = _unitOfWork.Repository<PaymentTerms>().Query().Get().OrderBy(m=>m.PaymentTermsName);

            return pt;
        }

        public PaymentTerms Add(PaymentTerms pt)
        {
            _unitOfWork.Repository<PaymentTerms>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.PaymentTerms
                        orderby p.PaymentTermsName
                        select p.PaymentTermsId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.PaymentTerms
                        orderby p.PaymentTermsName
                        select p.PaymentTermsId).FirstOrDefault();
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

                temp = (from p in db.PaymentTerms
                        orderby p.PaymentTermsName
                        select p.PaymentTermsId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.PaymentTerms
                        orderby p.PaymentTermsName
                        select p.PaymentTermsId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public void Dispose()
        {
        }


        public Task<IEquatable<PaymentTerms>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PaymentTerms> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
