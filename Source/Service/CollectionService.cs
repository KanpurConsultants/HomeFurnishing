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
using Model.ViewModel;
using Service;
using Core.Common;
using Presentation.ViewModels;
using Model.ViewModels;
using Model.PropertyTax.Models;
using Model.DatabaseViews;

namespace Services.PropertyTax
{
    public interface ICollectionService : IDisposable
    {
        LedgerHeader Create(LedgerHeader s);
        CollectionViewModel Create(CollectionViewModel vmCollection, string UserName);
        void Delete(int id);
        void Delete(LedgerHeader s);
        void Delete(ReasonViewModel vm, string UserName);
        CollectionViewModel GetCollection(int id);
        LedgerHeader Find(int id);
        IQueryable<CollectionViewModel> GetCollectionList(int DocumentTypeId, string Uname);
        IQueryable<CollectionViewModel> GetCollectionListPendingToSubmit(int DocumentTypeId, string Uname);
        IQueryable<CollectionViewModel> GetCollectionListPendingToReview(int DocumentTypeId, string Uname);
        void Update(LedgerHeader s);
        void Update(CollectionViewModel vmCollection, string UserName);
        string GetMaxDocNo();
        DateTime AddDueDate(DateTime Base, int DueDays);
        void Submit(int Id, string UserName, string GenGatePass, string UserRemark);
        void Review(int Id, string UserName, string UserRemark);
        int NextPrevId(int DocId, int DocTypeId, string UserName, string PrevNextConstants);
        //byte[] GetReport(string Ids, int DocTypeId, string UserName);

        ComboBoxPagedResult GetReasonAccount(string searchTerm, int pageSize, int pageNum);



        #region Helper Methods
        void LogDetailInfo(CollectionViewModel vm);
        _Menu GetMenu(int Id);
        _Menu GetMenu(string Name);
        _ReportHeader GetReportHeader(string MenuName);
        _ReportLine GetReportLine(string Name, int ReportHeaderId);
        bool CheckForDocNoExists(string docno, int DocTypeId);
        bool CheckForDocNoExists(string docno, int headerid, int DocTypeId);
        #endregion

    }
    public class CollectionService : ICollectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<LedgerHeader> _CollectionRepository;
        private readonly ILogger _logger;
        private readonly IModificationCheck _modificationCheck;
        private readonly IDocumentTypeService _DocumentTypeService;

        private ActiivtyLogViewModel logVm = new ActiivtyLogViewModel();

        public CollectionService(IUnitOfWork unit, IRepository<LedgerHeader> CollectionRepo,
            IDocumentTypeService DocumentTypeService,
            ILogger log, IModificationCheck modificationCheck)
        {
            _unitOfWork = unit;
            _CollectionRepository = CollectionRepo;
            _DocumentTypeService = DocumentTypeService;
            _logger = log;
            _modificationCheck = modificationCheck;

            //Log Initialization
            logVm.SessionId = 0;
            logVm.ControllerName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            logVm.ActionName = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            logVm.User = System.Web.HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;

        }

        public LedgerHeader Create(LedgerHeader s)
        {
            s.ObjectState = ObjectState.Added;
            _unitOfWork.Repository<LedgerHeader>().Insert(s);
            return s;
        }

        public void Delete(int id)
        {
            _unitOfWork.Repository<LedgerHeader>().Delete(id);
        }
        public void Delete(LedgerHeader s)
        {
            _unitOfWork.Repository<LedgerHeader>().Delete(s);
        }
        public void Update(LedgerHeader s)
        {
            s.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<LedgerHeader>().Update(s);
        }


        public LedgerHeader Find(int id)
        {
            return _unitOfWork.Repository<LedgerHeader>().Find(id);
        }

        public string GetMaxDocNo()
        {
            int x;
            var maxVal = _unitOfWork.Repository<LedgerHeader>().Query().Get().Select(i => i.DocNo ).DefaultIfEmpty().ToList().Select(sx => int.TryParse(sx, out x) ? x : 0).Max();
            return (maxVal + 1).ToString();
        }


        public IQueryable<CollectionViewModel> GetCollectionList(int DocumentTypeId, string Uname)
        {
            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];

            return (from p in _CollectionRepository.Instance
                    join dt in _unitOfWork.Repository<DocumentType>().Instance on p.DocTypeId equals dt.DocumentTypeId
                    orderby p.DocDate 
                    where p.DocTypeId == DocumentTypeId
                    select new CollectionViewModel
                    {
                        LedgerHeaderId = p.LedgerHeaderId,
                        DocTypeId = p.DocTypeId,
                        DocTypeName = dt.DocumentTypeName,
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        PersonId = (int) p.LedgerAccount.PersonId,
                        Name = p.LedgerAccount.Person.Name,
                        Code = p.LedgerAccount.Person.Code,
                        Status = p.Status,
                        ModifiedBy = p.ModifiedBy,
                        ReviewCount = p.ReviewCount,
                        ReviewBy = p.ReviewBy,
                        Reviewed = (SqlFunctions.CharIndex(Uname, p.ReviewBy) > 0),
                    });
        }

        public CollectionViewModel GetCollection(int id)
        {
            return (from p in _CollectionRepository.Instance
                    join L in _unitOfWork.Repository<LedgerLine>().Instance on p.LedgerHeaderId equals L.LedgerHeaderId into LedgerLineTable from LedgerLineTab in LedgerLineTable.DefaultIfEmpty()
                    join A in _unitOfWork.Repository<LedgerAccount>().Instance on p.LedgerAccountId equals A.LedgerAccountId into LedgerAccountTable from LedgerAccountTab in LedgerAccountTable.DefaultIfEmpty()
                    join P in _unitOfWork.Repository<Person>().Instance  on LedgerAccountTab.PersonId equals P.PersonID into PersonTable from PersonTab in PersonTable.DefaultIfEmpty()
                    join Pe in _unitOfWork.Repository<PersonExtended>().Instance   on PersonTab.PersonID equals Pe.PersonId into PersonExtendedTable from PersonExtendedTab in PersonExtendedTable.DefaultIfEmpty()
                    where p.LedgerHeaderId == id
                    select new CollectionViewModel
                    {
                        LedgerHeaderId = p.LedgerHeaderId,
                        LedgerLineId = LedgerLineTab.LedgerLineId,
                        PersonId = (int)p.LedgerAccount.PersonId,
                        Code = p.LedgerAccount.Person.Code,
                        GodownId = PersonExtendedTab.GodownId,
                        BinLocationId = PersonExtendedTab.BinLocationId,
                        HouseNo = PersonExtendedTab.HouseNo,
                        AreaId = PersonExtendedTab.AreaId,
                        Name = PersonTab.Name,
                        FatherName = PersonExtendedTab.FatherName,
                        TotalPropertyArea = PersonExtendedTab.TotalPropertyArea,
                        TotalTaxableArea = PersonExtendedTab.TotalTaxableArea,
                        TotalARV = PersonExtendedTab.TotalARV,
                        TotalTax = PersonExtendedTab.TotalTax,
                        DocTypeName = p.DocType.DocumentTypeName,
                        DocDate = p.DocDate,
                        DocNo = p.DocNo,
                        PartyDocNo = p.PartyDocNo,
                        ReceivedAmount = LedgerLineTab.Amount,
                        PaymentModeId = (int)LedgerLineTab.PaymentModeId,
                        AgentId = LedgerLineTab.AgentId,
                        ReferenceLedgerAccountId = LedgerLineTab.ReferenceLedgerAccountId,
                        ChqNo = LedgerLineTab.ChqNo,
                        ChqDate = LedgerLineTab.ChqDate,
                        DiscountAmount = LedgerLineTab.DiscountAmount,
                        Status = p.Status,
                        DocTypeId = p.DocTypeId,
                        ModifiedBy = p.ModifiedBy,
                        CreatedDate = p.CreatedDate,
                    }).FirstOrDefault();
        }


        public IQueryable<CollectionViewModel> GetCollectionListPendingToSubmit(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var LedgerHeader = GetCollectionList(id, Uname).AsQueryable();

            var PendingToSubmit = from p in LedgerHeader
                                  where p.Status == (int)StatusConstants.Drafted || p.Status == (int)StatusConstants.Import || p.Status == (int)StatusConstants.Modified && (p.ModifiedBy == Uname || UserRoles.Contains("Admin"))
                                  select p;
            return PendingToSubmit;

        }


        public IQueryable<CollectionViewModel> GetCollectionListPendingToReview(int id, string Uname)
        {

            List<string> UserRoles = (List<string>)System.Web.HttpContext.Current.Session["Roles"];
            var LedgerHeader = GetCollectionList(id, Uname).AsQueryable();

            var PendingToReview = from p in LedgerHeader
                                  where p.Status == (int)StatusConstants.Submitted && (SqlFunctions.CharIndex(Uname, (p.ReviewBy ?? "")) == 0)
                                  select p;
            return PendingToReview;

        }




        public DateTime AddDueDate(DateTime Base, int DueDays)
        {
            DateTime DueDate = Base.AddDays((double)DueDays);
            if (DueDate.DayOfWeek == DayOfWeek.Sunday)
                DueDate = DueDate.AddDays(1);

            return DueDate;
        }

        public CollectionViewModel Create(CollectionViewModel vmCollection, string UserName)
        {
            LedgerHeader ledgerheader = Mapper.Map<CollectionViewModel, LedgerHeader>(vmCollection);
            LedgerLine ledgerline = Mapper.Map<CollectionViewModel, LedgerLine>(vmCollection);
            DocumentType D = _DocumentTypeService.Find(ledgerheader.DocTypeId);

            ledgerheader.CreatedDate = DateTime.Now;
            ledgerheader.ModifiedDate = DateTime.Now;
            ledgerheader.CreatedBy = UserName;
            ledgerheader.ModifiedBy = UserName;
            ledgerheader.Status = (int)StatusConstants.Drafted;
            ledgerheader.ObjectState = Model.ObjectState.Added;
            Create(ledgerheader);


            ledgerline.DiscountAmount = vmCollection.DiscountAmount;
            ledgerline.Amount = vmCollection.ReceivedAmount;
            ledgerline.CreatedDate = DateTime.Now;
            ledgerline.ModifiedDate = DateTime.Now;
            ledgerline.CreatedBy = UserName;
            ledgerline.ModifiedBy = UserName;
            ledgerline.ObjectState = Model.ObjectState.Added;
            _unitOfWork.Repository<LedgerLine>().Add(ledgerline);


            int PostingAccountId = (from P in _unitOfWork.Repository<PaymentModeLedgerAccount>().Instance
                                  where P.PaymentModeId == vmCollection.PaymentModeId && P.SiteId == ledgerheader.SiteId && P.DivisionId == ledgerheader.DivisionId
                                  select P).FirstOrDefault().LedgerAccountId;


            int OtherCollection = Core.Common.DocumentTypeIdConstants.OtherCollection;


            int LedgerDueDrId = 0;
            if (OtherCollection == vmCollection.DocTypeId)
            {
                Ledger LedgerDueDr = new Ledger();
                LedgerDueDr.LedgerId = -1;
                LedgerDueDr.LedgerHeaderId = ledgerheader.LedgerHeaderId;
                LedgerDueDr.LedgerLineId = ledgerline.LedgerLineId;
                LedgerDueDr.LedgerAccountId = (int)ledgerheader.LedgerAccountId;
                LedgerDueDr.ContraLedgerAccountId = vmCollection.ReferenceLedgerAccountId;
                LedgerDueDr.AmtDr = ledgerline.Amount;
                LedgerDueDr.AmtCr = 0;
                LedgerDueDr.ObjectState = Model.ObjectState.Added;
                _unitOfWork.Repository<Ledger>().Add(LedgerDueDr);
                LedgerDueDrId = LedgerDueDr.LedgerId;


                Ledger LedgerDueCr = new Ledger();
                LedgerDueCr.LedgerId = -2;
                LedgerDueCr.LedgerHeaderId = ledgerheader.LedgerHeaderId;
                LedgerDueCr.LedgerLineId = ledgerline.LedgerLineId;
                LedgerDueCr.LedgerAccountId = (int) vmCollection.ReferenceLedgerAccountId;
                LedgerDueCr.ContraLedgerAccountId = ledgerheader.LedgerAccountId;
                LedgerDueCr.AmtDr = 0;
                LedgerDueCr.AmtCr = ledgerline.Amount;
                LedgerDueCr.ObjectState = Model.ObjectState.Added;
                _unitOfWork.Repository<Ledger>().Add(LedgerDueCr);
            }


            Ledger LedgerDr = new Ledger();
            LedgerDr.LedgerId = -3;
            LedgerDr.LedgerHeaderId = ledgerheader.LedgerHeaderId;
            LedgerDr.LedgerLineId = ledgerline.LedgerLineId;
            LedgerDr.LedgerAccountId = PostingAccountId;
            LedgerDr.ContraLedgerAccountId = ledgerheader.LedgerAccountId;
            LedgerDr.AmtDr = ledgerline.Amount;
            LedgerDr.AmtCr = 0;
            LedgerDr.ObjectState = Model.ObjectState.Added;
            _unitOfWork.Repository<Ledger>().Add(LedgerDr);


            Ledger LedgerCr = new Ledger();
            LedgerCr.LedgerId = -4;
            LedgerCr.LedgerHeaderId = ledgerheader.LedgerHeaderId;
            LedgerCr.LedgerLineId = ledgerline.LedgerLineId;
            LedgerCr.LedgerAccountId = (int) ledgerheader.LedgerAccountId;
            LedgerCr.ContraLedgerAccountId = PostingAccountId;
            LedgerCr.AmtDr = 0;
            LedgerCr.AmtCr = ledgerline.Amount;
            LedgerCr.ObjectState = Model.ObjectState.Added;
            _unitOfWork.Repository<Ledger>().Add(LedgerCr);


            if (OtherCollection == vmCollection.DocTypeId)
            {
                LedgerAdj LedgerAdj = new LedgerAdj();
                LedgerAdj.DrLedgerId = LedgerDueDrId;
                LedgerAdj.CrLedgerId = LedgerCr.LedgerId;
                LedgerAdj.SiteId = ledgerheader.SiteId;
                LedgerAdj.Amount = ledgerline.Amount;
                LedgerAdj.Adj_Type = "Cr";
                LedgerAdj.CreatedDate = DateTime.Now;
                LedgerAdj.ModifiedDate = DateTime.Now;
                LedgerAdj.CreatedBy = UserName;
                LedgerAdj.ModifiedBy = UserName;
                LedgerAdj.ObjectState = Model.ObjectState.Added;
                _unitOfWork.Repository<LedgerAdj>().Add(LedgerAdj);
            }


            _unitOfWork.Save();

            vmCollection.LedgerHeaderId = ledgerheader.LedgerHeaderId;

            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = ledgerheader.DocTypeId,
                DocId = ledgerheader.LedgerHeaderId,
                ActivityType = (int)ActivityTypeContants.Added,
                DocNo = ledgerheader.DocNo,
                DocDate = ledgerheader.DocDate,
                DocStatus = ledgerheader.Status,
            }));


            return vmCollection;
        }


        public void Update(CollectionViewModel vmCollection, string UserName)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            LedgerHeader ledgerheader = Find(vmCollection.LedgerHeaderId);
            LedgerLine ledgerline = _unitOfWork.Repository<LedgerLine>().Find(vmCollection.LedgerLineId);

            LedgerHeader ExRec = Mapper.Map<LedgerHeader>(ledgerheader);

            DocumentType D = _DocumentTypeService.Find(ledgerheader.DocTypeId);

            int status = ledgerheader.Status;

            if (ledgerheader.Status != (int)StatusConstants.Drafted && ledgerheader.Status != (int)StatusConstants.Import)
                ledgerheader.Status = (int)StatusConstants.Modified;



            ledgerheader.DocDate = vmCollection.DocDate;
            ledgerheader.DocNo = vmCollection.DocNo;
            ledgerheader.PartyDocNo = vmCollection.PartyDocNo;
            ledgerheader.PartyDocNo = vmCollection.PartyDocNo;
            ledgerheader.ModifiedDate = DateTime.Now;
            ledgerheader.ModifiedBy = UserName;
            ledgerheader.ObjectState = Model.ObjectState.Modified;
            Update(ledgerheader);


            ledgerline.DiscountAmount = vmCollection.DiscountAmount;
            ledgerline.Amount = vmCollection.ReceivedAmount;
            ledgerline.PaymentModeId = vmCollection.PaymentModeId;
            ledgerline.AgentId = vmCollection.AgentId;
            ledgerline.ChqNo = vmCollection.ChqNo;
            ledgerline.ChqDate = vmCollection.ChqDate;
            ledgerline.ModifiedDate = DateTime.Now;
            ledgerline.ModifiedBy = UserName;
            ledgerline.Amount = vmCollection.ReceivedAmount;
            ledgerline.ObjectState = ObjectState.Modified;
            _unitOfWork.Repository<LedgerLine>().Add(ledgerline);



            LogList.Add(new LogTypeViewModel
            {
                ExObj = ExRec,
                Obj = ledgerheader,
            });



            XElement Modifications = _modificationCheck.CheckChanges(LogList);

            _unitOfWork.Save();

            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = ledgerheader.DocTypeId,
                DocId = ledgerheader.LedgerHeaderId,
                ActivityType = (int)ActivityTypeContants.Modified,
                DocNo = ledgerheader.DocNo,
                xEModifications = Modifications,
                DocDate = ledgerheader.DocDate,
                DocStatus = ledgerheader.Status,
            }));

        }

        public void Delete(ReasonViewModel vm, string UserName)
        {
            List<LogTypeViewModel> LogList = new List<LogTypeViewModel>();

            var LedgerHeader = Find(vm.id);


            LogList.Add(new LogTypeViewModel
            {
                ExObj = Mapper.Map<LedgerHeader>(LedgerHeader),
            });


            var LedgerLine = _unitOfWork.Repository<LedgerLine>().Query().Get().Where(m => m.LedgerHeaderId == LedgerHeader.LedgerHeaderId).ToList();
            var Ledger = _unitOfWork.Repository<Ledger>().Query().Get().Where(m => m.LedgerHeaderId == LedgerHeader.LedgerHeaderId).ToList();


            List<int> LedgerAdjList = new List<int>();
            foreach (var item in Ledger)
            {
                var LedgerAdj1 = _unitOfWork.Repository<LedgerAdj>().Query().Get().Where(m => m.DrLedgerId == item.LedgerId).ToList();

                foreach (var item2 in LedgerAdj1)
                {
                    if (!LedgerAdjList.Contains(item2.LedgerAdjId))
                    {
                        item2.ObjectState = Model.ObjectState.Deleted;
                        _unitOfWork.Repository<LedgerAdj>().Delete(item2);
                        LedgerAdjList.Add(item2.LedgerAdjId);
                    }
                }

                var LedgerAdj2 = _unitOfWork.Repository<LedgerAdj>().Query().Get().Where(m => m.CrLedgerId == item.LedgerId).ToList();

                foreach (var item3 in LedgerAdj2)
                {
                    if (!LedgerAdjList.Contains(item3.LedgerAdjId))
                    {
                        item3.ObjectState = Model.ObjectState.Deleted;
                        _unitOfWork.Repository<LedgerAdj>().Delete(item3);
                        LedgerAdjList.Add(item3.LedgerAdjId);
                    }
                }
            }

            foreach (var item in Ledger)
            {
                item.ObjectState = Model.ObjectState.Deleted;
                _unitOfWork.Repository<Ledger>().Delete(item);
            }

            foreach (var item in LedgerLine)
            {
                item.ObjectState = Model.ObjectState.Deleted;
                _unitOfWork.Repository<LedgerLine>().Delete(item);
            }



            LedgerHeader.ObjectState = Model.ObjectState.Deleted;
            _unitOfWork.Repository<LedgerHeader>().Delete(LedgerHeader);


            XElement Modifications = _modificationCheck.CheckChanges(LogList);

            _unitOfWork.Save();


            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = LedgerHeader.DocTypeId,
                DocId = LedgerHeader.LedgerHeaderId,
                ActivityType = (int)ActivityTypeContants.Deleted,
                UserRemark = vm.Reason,
                DocNo = LedgerHeader.DocNo,
                xEModifications = Modifications,
                DocDate = LedgerHeader.DocDate,
                DocStatus = LedgerHeader.Status,
            }));

        }


        public void Submit(int Id, string UserName, string GenGatePass, string UserRemark)
        {
            var pd = _CollectionRepository.Find(Id);

            pd.Status = (int)StatusConstants.Submitted;
            int ActivityType = (int)ActivityTypeContants.Submitted;


            pd.ReviewBy = null;
            pd.ObjectState = Model.ObjectState.Modified;
            _CollectionRepository.Update(pd);


            CollectionViewModel vmCollection = GetCollection(Id);


            int DocumentTypeId = _DocumentTypeService.Find(Core.Common.DocumentTypeIdConstants.Collection).DocumentTypeId;

            if (DocumentTypeId == vmCollection.DocTypeId)
            {
                SqlParameter SqlParameterPersonId = new SqlParameter("@PersonId", vmCollection.PersonId);

                _unitOfWork.SqlQuery<CollectionViewModel>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".sp_CollectionAdjustment @PersonId", SqlParameterPersonId).ToList();
            }


            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = pd.DocTypeId,
                DocId = pd.LedgerHeaderId,
                ActivityType = ActivityType,
                UserRemark = UserRemark,
                DocNo = pd.DocNo,
                DocDate = pd.DocDate,
                DocStatus = pd.Status,
            }));


        }

        public void Review(int Id, string UserName, string UserRemark)
        {
            var pd = Find(Id);

            pd.ReviewCount = (pd.ReviewCount ?? 0) + 1;
            pd.ReviewBy += UserName + ", ";
            pd.ObjectState = Model.ObjectState.Modified;

            Update(pd);

            _unitOfWork.Save();

            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = pd.DocTypeId,
                DocId = pd.LedgerHeaderId,
                ActivityType = (int)ActivityTypeContants.Reviewed,
                UserRemark = UserRemark,
                DocNo = pd.DocNo,
                DocDate = pd.DocDate,
                DocStatus = pd.Status,
            }));

        }

        public int NextPrevId(int DocId, int DocTypeId, string UserName, string PrevNext)
        {
            //return new NextPrevIdService(_unitOfWork).GetNextPrevId(DocId, DocTypeId, UserName, "", "Web.LedgerHeaders", "LedgerHeaderId", PrevNext);
            return GetNextPrevId(DocId, DocTypeId, UserName, "", "Web.LedgerHeaders", "LedgerHeaderId", PrevNext);
        }

        public int GetNextPrevId(int DocId, int DocTypeId, string UserName, string ForAction, string HeaderTableName, string HeaderTablePK, string NextPrev)
        {

            SqlParameter SqlParameterUserName = new SqlParameter("@UserName", UserName);
            SqlParameter SqlParameterForAction = new SqlParameter("@ForAction", ForAction);
            SqlParameter SqlParameterHeaderTableName = new SqlParameter("@HeaderTableName", HeaderTableName);
            SqlParameter SqlParameterHeaderTablePkFieldName = new SqlParameter("@HeaderTablePkFieldName", HeaderTablePK);
            SqlParameter SqlParameterDocId = new SqlParameter("@DocId", DocId);
            SqlParameter SqlParameterDocTypeId = new SqlParameter("@DocTypeId", DocTypeId);
            SqlParameter SqlParameterNextPrevious = new SqlParameter("@NextPrevious", NextPrev);

            int Id = _unitOfWork.SqlQuery<int>("" + ConfigurationManager.AppSettings["DataBaseSchema"] + ".spGetNextPreviousIdForCollection @UserName, @ForAction, @HeaderTableName, @HeaderTablePkFieldName, @DocId, @DocTypeId, @NextPrevious", SqlParameterUserName, SqlParameterForAction, SqlParameterHeaderTableName, SqlParameterHeaderTablePkFieldName, SqlParameterDocId, SqlParameterDocTypeId, SqlParameterNextPrevious).FirstOrDefault();

            if (Id == 0 || Id == null)
            {
                return DocId;
            }
            else
            {
                return Id;
            }
        }


        //public byte[] GetReport(string Ids, int DocTypeId, string UserName)
        //{

        //    int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
        //    int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

        //    var Settings = new CollectionSettingsService(_unitOfWork).GetCollectionSettingsForDocument(DocTypeId);

        //    string ReportSql = "";

        //    if (!string.IsNullOrEmpty(Settings.DocumentPrint))
        //        ReportSql = GetReportHeader(Settings.DocumentPrint).ReportSQL;


        //    List<byte[]> PdfStream = new List<byte[]>();
        //    foreach (var item in Ids.Split(',').Select(Int32.Parse))
        //    {

        //        DirectReportPrint drp = new DirectReportPrint();

        //        var pd = Find(item);

        //        byte[] Pdf;

        //        if (!string.IsNullOrEmpty(ReportSql))
        //        {
        //            Pdf = drp.rsDirectDocumentPrint(ReportSql, UserName, item);
        //            PdfStream.Add(Pdf);
        //        }
        //        else
        //        {
        //            if (pd.Status == (int)StatusConstants.Drafted || pd.Status == (int)StatusConstants.Import || pd.Status == (int)StatusConstants.Modified)
        //            {
        //                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint, UserName, item);

        //                PdfStream.Add(Pdf);
        //            }
        //            else if (pd.Status == (int)StatusConstants.Submitted || pd.Status == (int)StatusConstants.ModificationSubmitted)
        //            {
        //                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterSubmit, UserName, item);

        //                PdfStream.Add(Pdf);
        //            }
        //            else
        //            {
        //                Pdf = drp.DirectDocumentPrint(Settings.SqlProcDocumentPrint_AfterApprove, UserName, item);
        //                PdfStream.Add(Pdf);
        //            }
        //        }

        //        _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
        //        {
        //            DocTypeId = pd.DocTypeId,
        //            DocId = pd.LedgerHeaderId,
        //            ActivityType = (int)ActivityTypeContants.Print,
        //            DocNo = pd.DocNo,
        //            DocDate = pd.DocDate,
        //            DocStatus = pd.Status,
        //        }));

        //    }

        //    PdfMerger pm = new PdfMerger();

        //    byte[] Merge = pm.MergeFiles(PdfStream);

        //    return Merge;
        //}






        #region Helper Methods

        public void LogDetailInfo(CollectionViewModel vm)
        {
            _logger.LogActivityDetail(logVm.Map(new ActiivtyLogViewModel
            {
                DocTypeId = vm.DocTypeId,
                DocId = vm.LedgerHeaderId,
                ActivityType = (int)ActivityTypeContants.Detail,
                DocNo = vm.DocNo,
                DocDate = vm.DocDate,
                DocStatus = vm.Status,
            }));
        }

        public _Menu GetMenu(int Id)
        {
            return _unitOfWork.Repository<_Menu>().Find(Id);
        }

        public _Menu GetMenu(string Name)
        {
            return _unitOfWork.Repository<_Menu>().Query().Get().Where(m => m.MenuName == Name).FirstOrDefault();
        }

        public _ReportHeader GetReportHeader(string MenuName)
        {
            return _unitOfWork.Repository<_ReportHeader>().Query().Get().Where(m => m.ReportName == MenuName).FirstOrDefault();
        }
        public _ReportLine GetReportLine(string Name, int ReportHeaderId)
        {
            return _unitOfWork.Repository<_ReportLine>().Query().Get().Where(m => m.ReportHeaderId == ReportHeaderId && m.FieldName == Name).FirstOrDefault();
        }

        public bool CheckForDocNoExists(string docno, int DocTypeId)
        {
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var temp = (from pr in _CollectionRepository.Instance
                        where pr.DocNo == docno && (pr.DocTypeId == DocTypeId)
                        select pr).FirstOrDefault();
            if (temp == null)
                return false;
            else
                return true;

        }
        public bool CheckForDocNoExists(string docno, int headerid, int DocTypeId)
        {
            int SiteId = (int)System.Web.HttpContext.Current.Session["SiteId"];
            int DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];

            var temp = (from pr in _CollectionRepository.Instance
                        where pr.DocNo == docno && pr.LedgerHeaderId != headerid && (pr.DocTypeId == DocTypeId)
                        select pr).FirstOrDefault();
            if (temp == null)
                return false;
            else
                return true;
        }

        #endregion

        public ComboBoxPagedResult GetReasonAccount(string searchTerm, int pageSize, int pageNum)
        {
            var list = (from pr in _unitOfWork.Repository<LedgerAccount>().Instance
                        join Ag in _unitOfWork.Repository<LedgerAccountGroup>().Instance on pr.LedgerAccountGroupId equals Ag.LedgerAccountGroupId into LedgerAccountGroupTable from LedgerAccountGroupTab in LedgerAccountGroupTable.DefaultIfEmpty()
                        where (string.IsNullOrEmpty(searchTerm) ? 1 == 1 : (pr.LedgerAccountName.ToLower().Contains(searchTerm.ToLower()))) &&
                        LedgerAccountGroupTab.LedgerAccountGroupName == "Indirect Incomes"
                        orderby pr.LedgerAccountName
                        select new ComboBoxResult
                        {
                            text = pr.LedgerAccountName,
                            id = pr.LedgerAccountId.ToString()
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

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }




    }


}


