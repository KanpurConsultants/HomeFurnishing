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
    public interface ICasteService : IDisposable
    {
        Caste Create(Caste pt);
        void Delete(int id);
        void Delete(Caste pt);
        Caste Find(string Name);
        Caste Find(int id);
        IEnumerable<Caste> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Caste pt);
        Caste Add(Caste pt);
        IEnumerable<Caste> GetCasteList();
        IQueryable<Caste> GetCasteListForIndex();
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

    public class CasteService : ICasteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Caste> _CasteRepository;
        public CasteService(IUnitOfWork unitOfWork, IRepository<Caste> CasteRepo)
        {
            _unitOfWork = unitOfWork;
            _CasteRepository = CasteRepo;
        }
        public CasteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _CasteRepository = unitOfWork.Repository<Caste>();
        }

        public Caste Find(string Name)
        {
            return _CasteRepository.Query().Get().Where(i => i.CasteName == Name).FirstOrDefault();
        }


        public Caste Find(int id)
        {
            return _unitOfWork.Repository<Caste>().Find(id);
        }

        public Caste Create(Caste pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Caste>().Add(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Caste>().Delete(id);
        }

        public void Delete(Caste pt)
        {
            _unitOfWork.Repository<Caste>().Delete(pt);
        }

        public void Update(Caste pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Caste>().Update(pt);
        }

        public IEnumerable<Caste> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Caste>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.CasteName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Caste> GetCasteList()
        {
            var pt = _unitOfWork.Repository<Caste>().Query().Get().OrderBy(m => m.CasteName);

            return pt;
        }

        public IQueryable<Caste> GetCasteListForIndex()
        {
            var pt = _unitOfWork.Repository<Caste>().Query().Get().OrderBy(m => m.CasteName);

            return pt;
        }

        public Caste Add(Caste pt)
        {
            _unitOfWork.Repository<Caste>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in _CasteRepository.Instance
                        orderby p.CasteName
                        select p.CasteId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in _CasteRepository.Instance
                        orderby p.CasteName
                        select p.CasteId).FirstOrDefault();
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

                temp = (from p in _CasteRepository.Instance
                        orderby p.CasteName
                        select p.CasteId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in _CasteRepository.Instance
                        orderby p.CasteName
                        select p.CasteId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public ComboBoxPagedResult GetList(string searchTerm, int pageSize, int pageNum)
        {
            var list = (from pr in _CasteRepository.Instance
                        where (string.IsNullOrEmpty(searchTerm) ? 1 == 1 : (pr.CasteName.ToLower().Contains(searchTerm.ToLower())))
                        orderby pr.CasteName
                        select new ComboBoxResult
                        {
                            text = pr.CasteName,
                            id = pr.CasteId.ToString()
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

            IEnumerable<Caste> Castes = from pr in _CasteRepository.Instance
                                            where pr.CasteId == Id
                                            select pr;

            ProductJson.id = Castes.FirstOrDefault().CasteId.ToString();
            ProductJson.text = Castes.FirstOrDefault().CasteName;

            return ProductJson;
        }

        public List<ComboBoxResult> GetListCsv(string Ids)
        {
            string[] subStr = Ids.Split(',');
            List<ComboBoxResult> ProductJson = new List<ComboBoxResult>();
            for (int i = 0; i < subStr.Length; i++)
            {
                int temp = Convert.ToInt32(subStr[i]);
                IEnumerable<Caste> Castes = from pr in _CasteRepository.Instance
                                                where pr.CasteId == temp
                                                select pr;
                ProductJson.Add(new ComboBoxResult()
                {
                    id = Castes.FirstOrDefault().CasteId.ToString(),
                    text = Castes.FirstOrDefault().CasteName
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

            var temp = (from pr in _CasteRepository.Instance
                        where pr.CasteName == Name
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

            var temp = (from pr in _CasteRepository.Instance
                        where pr.CasteName == Name && pr.CasteId != Id
                        select pr).FirstOrDefault();
            if (temp == null)
                return false;
            else
                return true;
        }
       
    }
}
