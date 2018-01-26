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
using Model.Models;
using Data.Infrastructure;
using Model.ViewModels;

namespace Services.PropertyTax
{
    public interface IDiscountTypeService : IDisposable
    {
        DiscountType Create(DiscountType pt);
        void Delete(int id);
        void Delete(DiscountType pt);
        DiscountType Find(string Name);
        DiscountType Find(int id);
        IEnumerable<DiscountType> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(DiscountType pt);
        DiscountType Add(DiscountType pt);
        IEnumerable<DiscountType> GetDiscountTypeList();
        IQueryable<DiscountType> GetDiscountTypeListForIndex();
        int NextId(int id);
        int PrevId(int id);
        bool CheckForNameExists(string Name);
        bool CheckForNameExists(string Name, int Id);

        #region HelpList Getter
        /// <summary>
        /// *General Function*
        /// This function will create the help list for Projects
        /// </summary>
        /// <param name="searchTerm">user search term</param>
        /// <param name="pageSize">no of records to fetch for each page</param>
        /// <param name="pageNum">current page size </param>
        /// <returns>ComboBoxPagedResult</returns>
        ComboBoxPagedResult GetList(string searchTerm, int pageSize, int pageNum);
        #endregion

        #region HelpList Setters
        /// <summary>
        /// *General Function*
        /// This function will return the object in (Id,Text) format based on the Id
        /// </summary>
        /// <param name="Id">Primarykey of the record</param>
        /// <returns>ComboBoxResult</returns>
        ComboBoxResult GetValue(int Id);

        /// <summary>
        /// *General Function*
        /// This function will return list of object in (Id,Text) format based on the Ids
        /// </summary>
        /// <param name="Id">PrimaryKey of the record(',' seperated string)</param>
        /// <returns>List<ComboBoxResult></returns>
        List<ComboBoxResult> GetListCsv(string Id);
        #endregion
    }

    public class DiscountTypeService : IDiscountTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<DiscountType> _DiscountTypeRepository;
        public DiscountTypeService(IUnitOfWork unitOfWork, IRepository<DiscountType> DiscountTypeRepo)
        {
            _unitOfWork = unitOfWork;
            _DiscountTypeRepository = DiscountTypeRepo;
        }
        public DiscountTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _DiscountTypeRepository = unitOfWork.Repository<DiscountType>();
        }

        public DiscountType Find(string Name)
        {
            return _DiscountTypeRepository.Query().Get().Where(i => i.DiscountTypeName == Name).FirstOrDefault();
        }


        public DiscountType Find(int id)
        {
            return _unitOfWork.Repository<DiscountType>().Find(id);
        }

        public DiscountType Create(DiscountType pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<DiscountType>().Add(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<DiscountType>().Delete(id);
        }

        public void Delete(DiscountType pt)
        {
            _unitOfWork.Repository<DiscountType>().Delete(pt);
        }

        public void Update(DiscountType pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<DiscountType>().Update(pt);
        }

        public IEnumerable<DiscountType> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<DiscountType>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.DiscountTypeName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<DiscountType> GetDiscountTypeList()
        {
            var pt = _unitOfWork.Repository<DiscountType>().Query().Get().OrderBy(m => m.DiscountTypeName);

            return pt;
        }

        public IQueryable<DiscountType> GetDiscountTypeListForIndex()
        {
            var pt = _unitOfWork.Repository<DiscountType>().Query().Get().OrderBy(m => m.DiscountTypeName);

            return pt;
        }

        public DiscountType Add(DiscountType pt)
        {
            _unitOfWork.Repository<DiscountType>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in _DiscountTypeRepository.Instance
                        orderby p.DiscountTypeName
                        select p.DiscountTypeId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in _DiscountTypeRepository.Instance
                        orderby p.DiscountTypeName
                        select p.DiscountTypeId).FirstOrDefault();
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

                temp = (from p in _DiscountTypeRepository.Instance
                        orderby p.DiscountTypeName
                        select p.DiscountTypeId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in _DiscountTypeRepository.Instance
                        orderby p.DiscountTypeName
                        select p.DiscountTypeId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public ComboBoxPagedResult GetList(string searchTerm, int pageSize, int pageNum)
        {
            var list = (from pr in _DiscountTypeRepository.Instance
                        where (string.IsNullOrEmpty(searchTerm) ? 1 == 1 : (pr.DiscountTypeName.ToLower().Contains(searchTerm.ToLower())))
                        orderby pr.DiscountTypeName
                        select new ComboBoxResult
                        {
                            text = pr.DiscountTypeName,
                            id = pr.DiscountTypeId.ToString()
                        }
              );

            var temp = list
               .Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();

            var count = list.Count();

            ComboBoxPagedResult Data = new ComboBoxPagedResult();
            Data.Results = temp;
            Data.Total = count;

            return Data;
        }

        public ComboBoxResult GetValue(int Id)
        {
            ComboBoxResult ProductJson = new ComboBoxResult();

            IEnumerable<DiscountType> DiscountTypes = from pr in _DiscountTypeRepository.Instance
                                            where pr.DiscountTypeId == Id
                                            select pr;

            ProductJson.id = DiscountTypes.FirstOrDefault().DiscountTypeId.ToString();
            ProductJson.text = DiscountTypes.FirstOrDefault().DiscountTypeName;

            return ProductJson;
        }

        public List<ComboBoxResult> GetListCsv(string Ids)
        {
            string[] subStr = Ids.Split(',');
            List<ComboBoxResult> ProductJson = new List<ComboBoxResult>();
            for (int i = 0; i < subStr.Length; i++)
            {
                int temp = Convert.ToInt32(subStr[i]);
                IEnumerable<DiscountType> DiscountTypes = from pr in _DiscountTypeRepository.Instance
                                                where pr.DiscountTypeId == temp
                                                select pr;
                ProductJson.Add(new ComboBoxResult()
                {
                    id = DiscountTypes.FirstOrDefault().DiscountTypeId.ToString(),
                    text = DiscountTypes.FirstOrDefault().DiscountTypeName
                });
            }
            return ProductJson;
        }
        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public bool CheckForNameExists(string Name)
        {
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var temp = (from pr in _DiscountTypeRepository.Instance
                        where pr.DiscountTypeName == Name
                        select pr).FirstOrDefault();
            if (temp == null)
                return false;
            else
                return true;

        }
        public bool CheckForNameExists(string Name, int Id)
        {
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var temp = (from pr in _DiscountTypeRepository.Instance
                        where pr.DiscountTypeName == Name && pr.DiscountTypeId != Id
                        select pr).FirstOrDefault();
            if (temp == null)
                return false;
            else
                return true;
        }
       
    }
}
