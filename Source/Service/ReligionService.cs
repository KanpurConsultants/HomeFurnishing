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
    public interface IReligionService : IDisposable
    {
        Religion Create(Religion pt);
        void Delete(int id);
        void Delete(Religion pt);
        Religion Find(string Name);
        Religion Find(int id);
        IEnumerable<Religion> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(Religion pt);
        Religion Add(Religion pt);
        IEnumerable<Religion> GetReligionList();
        IQueryable<Religion> GetReligionListForIndex();
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

    public class ReligionService : IReligionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Religion> _ReligionRepository;
        public ReligionService(IUnitOfWork unitOfWork, IRepository<Religion> ReligionRepo)
        {
            _unitOfWork = unitOfWork;
            _ReligionRepository = ReligionRepo;
        }
        public ReligionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _ReligionRepository = unitOfWork.Repository<Religion>();
        }

        public Religion Find(string Name)
        {
            return _ReligionRepository.Query().Get().Where(i => i.ReligionName == Name).FirstOrDefault();
        }


        public Religion Find(int id)
        {
            return _unitOfWork.Repository<Religion>().Find(id);
        }

        public Religion Create(Religion pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<Religion>().Add(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<Religion>().Delete(id);
        }

        public void Delete(Religion pt)
        {
            _unitOfWork.Repository<Religion>().Delete(pt);
        }

        public void Update(Religion pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<Religion>().Update(pt);
        }

        public IEnumerable<Religion> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<Religion>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.ReligionName))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<Religion> GetReligionList()
        {
            var pt = _unitOfWork.Repository<Religion>().Query().Get().OrderBy(m => m.ReligionName);

            return pt;
        }

        public IQueryable<Religion> GetReligionListForIndex()
        {
            var pt = _unitOfWork.Repository<Religion>().Query().Get().OrderBy(m => m.ReligionName);

            return pt;
        }

        public Religion Add(Religion pt)
        {
            _unitOfWork.Repository<Religion>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in _ReligionRepository.Instance
                        orderby p.ReligionName
                        select p.ReligionId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in _ReligionRepository.Instance
                        orderby p.ReligionName
                        select p.ReligionId).FirstOrDefault();
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

                temp = (from p in _ReligionRepository.Instance
                        orderby p.ReligionName
                        select p.ReligionId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in _ReligionRepository.Instance
                        orderby p.ReligionName
                        select p.ReligionId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }

        public ComboBoxPagedResult GetList(string searchTerm, int pageSize, int pageNum)
        {
            var list = (from pr in _ReligionRepository.Instance
                        where (string.IsNullOrEmpty(searchTerm) ? 1 == 1 : (pr.ReligionName.ToLower().Contains(searchTerm.ToLower())))
                        orderby pr.ReligionName
                        select new ComboBoxResult
                        {
                            text = pr.ReligionName,
                            id = pr.ReligionId.ToString()
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

            IEnumerable<Religion> Religions = from pr in _ReligionRepository.Instance
                                            where pr.ReligionId == Id
                                            select pr;

            ProductJson.id = Religions.FirstOrDefault().ReligionId.ToString();
            ProductJson.text = Religions.FirstOrDefault().ReligionName;

            return ProductJson;
        }

        public List<ComboBoxResult> GetListCsv(string Ids)
        {
            string[] subStr = Ids.Split(',');
            List<ComboBoxResult> ProductJson = new List<ComboBoxResult>();
            for (int i = 0; i < subStr.Length; i++)
            {
                int temp = Convert.ToInt32(subStr[i]);
                IEnumerable<Religion> Religions = from pr in _ReligionRepository.Instance
                                                where pr.ReligionId == temp
                                                select pr;
                ProductJson.Add(new ComboBoxResult()
                {
                    id = Religions.FirstOrDefault().ReligionId.ToString(),
                    text = Religions.FirstOrDefault().ReligionName
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

            var temp = (from pr in _ReligionRepository.Instance
                        where pr.ReligionName == Name
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

            var temp = (from pr in _ReligionRepository.Instance
                        where pr.ReligionName == Name && pr.ReligionId != Id
                        select pr).FirstOrDefault();
            if (temp == null)
                return false;
            else
                return true;
        }
       
    }
}
