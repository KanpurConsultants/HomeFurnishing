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

namespace Service
{
    public interface ISalarySettingsService : IDisposable
    {
        SalarySettings Create(SalarySettings pt);
        void Delete(int id);
        void Delete(SalarySettings pt);
        SalarySettings Find(int id);
        IEnumerable<SalarySettings> GetPagedList(int pageNumber, int pageSize, out int totalRecords);
        void Update(SalarySettings pt);
        SalarySettings Add(SalarySettings pt);
        SalarySettings GetSalarySettingsForDocument(int DocTypeId,int DivisionId,int SiteId);
        IEnumerable<SalarySettingsViewModel> GetSalarySettingsList();
        Task<IEquatable<SalarySettings>> GetAsync();
        Task<SalarySettings> FindAsync(int id);        
        int NextId(int id);
        int PrevId(int id);
    }

    public class SalarySettingsService : ISalarySettingsService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWorkForService _unitOfWork;
        private readonly Repository<SalarySettings> _SalarySettingsRepository;
        RepositoryQuery<SalarySettings> SalarySettingsRepository;
        public SalarySettingsService(IUnitOfWorkForService unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _SalarySettingsRepository = new Repository<SalarySettings>(db);
            SalarySettingsRepository = new RepositoryQuery<SalarySettings>(_SalarySettingsRepository);
        }

        public SalarySettings Find(int id)
        {
            return _unitOfWork.Repository<SalarySettings>().Find(id);
        }

        public SalarySettings GetSalarySettingsForDocument(int DocTypeId,int DivisionId,int SiteId)
        {
            return (from p in db.SalarySettings
                    where p.DocTypeId == DocTypeId && p.DivisionId == DivisionId && p.SiteId == SiteId
                    select p
                        ).FirstOrDefault();


        }
        public SalarySettings Create(SalarySettings pt)
        {
            pt.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<SalarySettings>().Insert(pt);
            return pt;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<SalarySettings>().Delete(id);
        }

        public void Delete(SalarySettings pt)
        {
            _unitOfWork.Repository<SalarySettings>().Delete(pt);
        }

        public void Update(SalarySettings pt)
        {
            pt.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<SalarySettings>().Update(pt);
        }

        public IEnumerable<SalarySettings> GetPagedList(int pageNumber, int pageSize, out int totalRecords)
        {
            var so = _unitOfWork.Repository<SalarySettings>()
                .Query()
                .OrderBy(q => q.OrderBy(c => c.SalarySettingsId))                
                .GetPage(pageNumber, pageSize, out totalRecords);

            return so;
        }

        public IEnumerable<SalarySettingsViewModel> GetSalarySettingsList()
        {

             var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];

            var pt = (from p in db.SalarySettings
                      orderby p.SalarySettingsId
                      where p.SiteId == SiteId && p.DivisionId == DivisionId
                      select new SalarySettingsViewModel
                      {
                          DocTypeName=p.DocType.DocumentTypeName,
                          DivisionName=p.Division.DivisionName,
                          SiteName=p.Site.SiteName,
                          SalarySettingsId=p.SalarySettingsId,
                      }
                          ).ToList();

            return pt;
        }

        public SalarySettings Add(SalarySettings pt)
        {
            _unitOfWork.Repository<SalarySettings>().Insert(pt);
            return pt;
        }

        public int NextId(int id)
        {
            int temp = 0;
            if (id != 0)
            {
                temp = (from p in db.SalarySettings
                        orderby p.SalarySettingsId
                        select p.SalarySettingsId).AsEnumerable().SkipWhile(p => p != id).Skip(1).FirstOrDefault();
            }
            else
            {
                temp = (from p in db.SalarySettings
                        orderby p.SalarySettingsId
                        select p.SalarySettingsId).FirstOrDefault();
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

                temp = (from p in db.SalarySettings
                        orderby p.SalarySettingsId
                        select p.SalarySettingsId).AsEnumerable().TakeWhile(p => p != id).LastOrDefault();
            }
            else
            {
                temp = (from p in db.SalarySettings
                        orderby p.SalarySettingsId
                        select p.SalarySettingsId).AsEnumerable().LastOrDefault();
            }
            if (temp != 0)
                return temp;
            else
                return id;
        }



        public void Dispose()
        {
        }


        public Task<IEquatable<SalarySettings>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SalarySettings> FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
