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
using Jobs.Constants.Menu;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Model.ViewModels;

namespace Service
{
    public class GridCustomReportService 
    {
        ApplicationDbContext db = new ApplicationDbContext();
        string mQry = "";

        public string ProductUidCaption = "ProductUid";
        public string Dimension1Caption = "Dimension1";
        public string Dimension2Caption = "Dimension2";
        public string Dimension3Caption = "Dimension3";
        public string Dimension4Caption = "Dimension4";

        public GridCustomReportService()
        {
        }
        public void GetGridCustomReportByName(string MenuName, ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            switch (MenuName)
            {
                case "Job Invoice Report":
                {
                    JobInvoiceReport(ref ReportHeader, ref ReportLineList, form);
                    return;
                }
                case "Job Order Report":
                    {
                        JobOrderReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Job Receive Report":
                    {
                        JobReceiveReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Job Order Cancel Report":
                    {
                        JobOrderCancelReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Job Invoice Return Report":
                    {
                        JobInvoiceReturnReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Job Return Report":
                    {
                        JobReturnReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Sale Order Report":
                    {
                        SaleOrderReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Sale Cancel Report":
                    {
                        SaleOrderCancelReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Sale Invoice Report":
                    {
                        SaleInvoiceReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Packing Report":
                    {
                        PackingReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Sale Order Amendment Report":
                    {
                        SaleOrderAmendmentReport(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                case "Store Issue Report":
                    {
                        StoreReport(ref ReportHeader, ref ReportLineList, form, "Store Issue Report");
                        return;
                    }
                case "Store Receive Report":
                    {
                        StoreReport(ref ReportHeader, ref ReportLineList, form, "Store Receive Report");
                        return;
                    }
                case "Stock Transfer Report":
                    {
                        StoreReport(ref ReportHeader, ref ReportLineList, form, "Stock Transfer Report");
                        return;
                    }
                case "Job Invoice Summary Report":
                    {
                        JobInvoiceSummary(ref ReportHeader, ref ReportLineList, form);
                        return;
                    }
                default:
                {
                    ReportHeader = null;
                    ReportLineList = null;
                    return;
                }
                        
            }
        }

        #region JobGridCustomReport
        private void JobInvoiceReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Job Invoice Report";

            if (form != null)
            {
                //
                mQry = @" SELECT H.JobInvoiceHeaderId AS DocId, H.DocTypeId, 
                        format(H.DocDate, 'dd/MMM/yy') As InvoiceDate, D.DocumentTypeShortName + '-' + H.DocNo AS InvoiceNo, Pr.ProcessName, 
                        Jw.Name AS Party, 
                        H.JobWorkerDocNo AS PartyDocNo, format(H.JobWorkerDocDate,'dd/MMM/yy') AS PartyDocDate, 
                        Cgp.ChargeGroupPersonName AS SalesTaxGroupPerson, 
                        Fn.Name AS Financier, H.CreditDays, C.Name AS Currency, H.GovtInvoiceNo, H.Remark,
                        Pu.ProductUidName AS " + ProductUidCaption + @", 
                        P.ProductName, 
                        D1.Dimension1Name AS " + Dimension1Caption + @", 
                        D2.Dimension2Name AS " + Dimension2Caption + @", 
                        D3.Dimension3Name AS " + Dimension3Caption + @", 
                        D4.Dimension4Name AS " + Dimension4Caption + @", 
                        Rd.DocumentTypeShortName + '-' + Jrh.DocNo AS ReceiveNo, 
                        Od.DocumentTypeShortName + '-' + Joh.DocNo AS OrderNo, 
                        Cpt.ChargeGroupProductName AS SalesTaxGroupProduct,
                        L.Qty, U.UnitName AS Unit, 
                        L.DealQty, Du.UnitName AS DealUnit, 
                        L.Rate, L.Amount, L.Remark AS LineRemark, 
                        L.IncentiveAmt, L.IncentiveRate, L.RateDiscountPer, L.RateDiscountAmt
                        FROM Web.JobInvoiceHeaders H 
                        LEFT JOIN Web.JobInvoiceLines L ON H.JobInvoiceHeaderId = L.JobInvoiceHeaderId
                        LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId
                        LEFT JOIN Web.Processes Pr ON H.ProcessId = Pr.ProcessId
                        LEFT JOIN Web.People Jw ON H.JobWorkerId = Jw.PersonID
                        LEFT JOIN Web.People Fn ON H.FinancierId = Fn.PersonID
                        LEFT JOIN Web.ChargeGroupPersons Cgp ON H.SalesTaxGroupPersonId = Cgp.ChargeGroupPersonId
                        LEFT JOIN Web.Currencies C ON H.CurrencyId = C.ID
                        LEFT JOIN Web.JobReceiveLines Jrl ON L.JobReceiveLineId = Jrl.JobReceiveLineId
                        LEFT JOIN Web.JobReceiveHeaders Jrh ON Jrl.JobReceiveHeaderId = Jrh.JobReceiveHeaderId
                        LEFT JOIN Web.DocumentTypes Rd ON Jrh.DocTypeId = Rd.DocumentTypeId
                        LEFT JOIN Web.Units Du ON L.DealUnitId = Du.UnitId
                        LEFT JOIN Web.JobOrderLines Jol ON Jrl.JobOrderLineId = Jol.JobOrderLineId
                        LEFT JOIN Web.JobOrderHeaders Joh ON Jol.JobOrderHeaderId = Joh.JobOrderHeaderId
                        LEFT JOIN Web.DocumentTypes Od ON Joh.DocTypeId = D.DocumentTypeId
                        LEFT JOIN Web.ChargeGroupProducts Cpt ON L.SalesTaxGroupProductId = Cpt.ChargeGroupProductId
                        LEFT JOIN Web.Products P WITH (nolock) ON P.ProductId = JRL.ProductId 
                        LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = P.ProductGroupId
                        LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON Pg.ProductTypeId = Pt.ProductTypeId
                        LEFT JOIN Web.ProductUids Pu WITH (nolock) ON Pu.ProductUidId = JRL.ProductUidId 
                        LEFT JOIN Web.Units U WITH (nolock) ON U.UnitId = JOL.UnitId 
                        LEFT JOIN Web.Dimension1 D1 WITH (nolock) ON D1.Dimension1Id = JRL.Dimension1Id  
                        LEFT JOIN Web.Dimension2 D2 WITH (nolock) ON D2.Dimension2Id = JRL.Dimension2Id 
                        LEFT JOIN Web.Dimension3 D3 WITH (nolock) ON D3.Dimension3Id = JRL.Dimension3Id  
                        LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id = JRL.Dimension4Id 
                        WHERE 1 = 1 " +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +                        
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["JobWorker"].ToString() != null && form["JobWorker"].ToString() != "" ? " AND H.JobWorkerId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND P.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND P.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY H.DocDate,H.DocNo,L.JobInvoiceLineId ";


            }

            ReportHeader.SqlProc = mQry;

            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);            
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Job Worker", "JobWorker", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "1.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);
        }

        private void JobReceiveReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Job Receive Report";

            if (form != null)
            {
                mQry = @" SELECT  H.JobReceiveHeaderId AS DocId, H.DocTypeId, 
                                DT.DocumentTypeName,format(H.DocDate,'dd/MMM/yy') AS ReceiveDate,H.DocNo AS ReceiveNo,
                                PS.Name AS JobWorkerName,WDT.DocumentTypeShortName + '-' + JOH.DocNo AS OrderNo,
                                P.ProductName AS Product,L.LotNo,
                                D1.Dimension1Name AS " + Dimension1Caption + @", 
                                D2.Dimension2Name AS " + Dimension2Caption + @", 
                                D3.Dimension3Name AS " + Dimension3Caption + @", 
                                D4.Dimension4Name AS " + Dimension4Caption + @", 
                                round(L.Qty,isnull(U.DecimalPlaces,0)) as Qty,
                                U.UnitName as Unit, round(isnull(L.LossQty,0),isnull(U.DecimalPlaces,0)) AS LossQty,
                                 round(isnull(L.PassQty,0),isnull(U.DecimalPlaces,0)) AS PassQty, L.Remark AS LineRemark, H.Remark AS HeaderRemark
                                FROM  
                                (
                                SELECT * FROM web._JobReceiveHeaders H WITH (nolock) WHERE 1=1
                                )  H
                                LEFT JOIN [Web].DocumentTypes DT WITH (nolock) ON DT.DocumentTypeId = H.DocTypeId 
                                left Join Web.DocumentTypeSettings DTS With (Nolock) On DTS.DocumentTypeId=H.DocTypeId
                                LEFT JOIN   Web._JobReceiveLines L WITH (nolock) ON L.JobReceiveHeaderId=H.JobReceiveHeaderId
                                LEFT JOIN [Web]._People PS WITH (nolock) ON PS.PersonID = H.JobWorkerId 
                                LEFT JOIN  web.JobOrderLines JOL WITH (nolock) ON JOL.JobOrderLineId=L.JobOrderLineId
                                LEFT JOIN Web.products P WITH (nolock) ON JOL.ProductId=P.ProductId
                                LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = P.ProductGroupId
                                LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON Pg.ProductTypeId = Pt.ProductTypeId  
                                LEFT JOIN Web.Dimension1Types Dt1 WITH (nolock) ON Pt.Dimension1TypeId = Dt1.Dimension1TypeId
                                LEFT JOIN Web.Dimension2Types Dt2 WITH (nolock) ON Pt.Dimension2TypeId = Dt2.Dimension2TypeId
                                LEFT JOIN [Web].Units U WITH (nolock) ON U.UnitId = JOL.UnitId   
                                LEFT JOIN Web.Dimension1 D1 WITH (nolock) ON D1.Dimension1Id = JOL.Dimension1Id  
                                LEFT JOIN Web.Dimension2 D2 WITH (nolock) ON D2.Dimension2Id = JOL.Dimension2Id 
                                LEFT JOIN Web.Dimension3 D3 WITH (nolock) ON D3.Dimension3Id = JOL.Dimension3Id  
                                LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id = JOL.Dimension4Id 
                                LEFT JOIN web.Sites SI WITH (nolock) ON SI.SiteId = H.SiteId
                                LEFT JOIN web.Divisions DI WITH (nolock) ON DI.DivisionId  = H.DivisionId
                                LEFT JOIN web._JobOrderHeaders JOH WITH (nolock) ON JOL.JobOrderHeaderId = JOH.JobOrderHeaderId
                                LEFT JOIN Web.DocumentTypes WDT WITH (nolock) ON JOH.DocTypeId=WDT.DocumentTypeId
                                LEFT JOIN Web.Processes WPS WITH (nolock) ON H.ProcessId=WPS.ProcessId
                                LEFT JOIN Web.Godowns WG WITH (nolock) ON WG.GodownId=H.GodownId
                                LEFT JOIN (
			                                SELECT ProductId FROM Web.ProductCustomGroupLines
			                                WHERE 1=1 
			                                GROUP BY ProductId
			                                ) AS PCG  ON  JOL.ProductId=PCG.ProductId 
                                WHERE 1=1 AND JOL.ProductId IS NOT NULL    " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +                        
                        (form["JobWorker"].ToString() != null && form["JobWorker"].ToString() != "" ? " AND H.JobWorkerId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND P.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND P.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY H.DocNo,DT.DocumentTypeShortName,L.JobReceiveLineId ";


            }

            ReportHeader.SqlProc = mQry;

            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Job Worker", "JobWorker", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "1.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }

        private void JobOrderReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Job Order Report";

            if (form != null)
            {
                mQry = @" SELECT H.JobOrderHeaderId AS DocId, H.DocTypeId,
                            format(H.DocDate, 'dd/MMM/yy') As DocDate, 		                    
		                    H.DocNo AS DocNo, format(H.DueDate, 'dd/MMM/yy') as DueDate,
		                    PS.Name AS Supplier, DTPO.DocumentTypeShortName + '-' +POH.DocNo AS PlanNo, 
		                    P.ProductName, L.Specification,
                            D1.Dimension1Name AS " + Dimension1Caption + @", 
                            D2.Dimension2Name AS " + Dimension2Caption + @", 
                            D3.Dimension3Name AS " + Dimension3Caption + @", 
                            D4.Dimension4Name AS " + Dimension4Caption + @", 
                            round(L.Qty,isnull(U.DecimalPlaces,0)) as Qty,U.UnitName as Unit,  
                            round(L.DealQty,isnull(AU.DecimalPlaces,0)) AS DealQty,AU.UnitName AS DealUnit,
		                    convert(float,L.Rate) AS Rate,L.Amount AS Amount, 
		                    L.LotNo,OBI.Name AS IssueBy, L.Remark AS LineRemark,H.Remark AS HeaderRemark
                    FROM  
                    ( 
                    SELECT * FROM [Web]._JobOrderHeaders H WITH (Nolock) WHERE 1=1
                    ) H 
                    LEFT JOIN [Web].DocumentTypes DT WITH (nolock) ON DT.DocumentTypeId = H.DocTypeId 
                    left Join Web.DocumentTypeSettings DTS With (Nolock) On DTS.DocumentTypeId=H.DocTypeId
                    LEFT JOIN [Web].JobOrderLines L WITH (nolock) ON L.JobOrderHeaderId = H.JobOrderHeaderId 
                    LEFT JOIN [Web]._People PS WITH (nolock) ON PS.PersonID = H.JobWorkerId  
                    LEFT JOIN [Web].People OBI WITH (nolock) ON OBI.PersonID =H.OrderById 
                    LEFT JOIN [Web].Products P WITH (nolock) ON P.ProductId = L.ProductId 
                    LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = P.ProductGroupId
                    LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON Pg.ProductTypeId = Pt.ProductTypeId  
                    LEFT JOIN Web.Dimension1Types Dt1 WITH (nolock) ON Pt.Dimension1TypeId = Dt1.Dimension1TypeId
                    LEFT JOIN Web.Dimension2Types Dt2 WITH (nolock) ON Pt.Dimension2TypeId = Dt2.Dimension2TypeId
                    LEFT JOIN [Web].Units U WITH (nolock) ON U.UnitId = L.UnitId  
                    LEFT JOIN [Web].Units AU WITH (nolock) ON AU.UnitId = L.DealUnitId
                    LEFT JOIN Web.Dimension1 D1 WITH (nolock) ON D1.Dimension1Id = L.Dimension1Id  
                    LEFT JOIN Web.Dimension2 D2 WITH (nolock) ON D2.Dimension2Id = L.Dimension2Id 
                    Left Join Web.Dimension3 D3 with (Nolock) On D3.Dimension3Id=L.Dimension3Id
                    Left Join Web.Dimension4 D4 with (Nolock) On D4.Dimension4Id=L.Dimension4Id
                    LEFT JOIN Web.Processes PR WITH (nolock) ON PR.ProcessId = H.ProcessId
                    LEFT JOIN web.Sites SI WITH (nolock) ON SI.SiteId = H.SiteId
                    LEFT JOIN web.Divisions DI WITH (nolock) ON DI.DivisionId  = H.DivisionId
                    LEFT JOIN Web.ProdOrderLines POL WITH (nolock) ON L.ProdOrderLineId = POL.ProdOrderLineId 
                    LEFT JOIN Web.ProdOrderHeaders POH WITH (nolock) ON POL.ProdOrderHeaderId  = POH.ProdOrderHeaderId
                    LEFT JOIN [Web].DocumentTypes DTPO WITH (nolock) ON DTPO.DocumentTypeId = POH.DocTypeId
                    LEFT JOIN web.ViewRugSize VRS with (nolock) on VRS.ProductId=P.ProductId
                    WHERE 1=1 AND L.ProductId IS NOT NULL " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["JobWorker"].ToString() != null && form["JobWorker"].ToString() != "" ? " AND H.JobWorkerId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND P.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND P.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        
                " ORDER BY H.DocDate,H.DocNo,L.JoborderlineId ";


            }

            ReportHeader.SqlProc = mQry;
            
            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Job Worker", "JobWorker", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "1.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);            
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);
            
        }

        private void JobOrderCancelReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Job Order Cancel Report";

            if (form != null)
            {
                mQry = @" SELECT  
                             H.JobOrderCancelHeaderId AS DocId, H.DocTypeId,
                            DT.DocumentTypeName,  
                            format(H.DocDate, 'dd/MMM/yy') AS CancelDate, 
		                    DT.DocumentTypeShortName + '-' + H.DocNo AS CancelDocNo, 
		                    PS.Name AS JobWorkerName,
		                    DTPI.DocumentTypeShortName + '-' + JOH.DocNo AS OrderNo,
		                    P.ProductName AS Product,
		                    JOL.Specification, 
                            D1.Dimension1Name AS " + Dimension1Caption + @", 
                            D2.Dimension2Name AS " + Dimension2Caption + @", 
                            D3.Dimension3Name AS " + Dimension3Caption + @", 
                            D4.Dimension4Name AS " + Dimension4Caption + @", 
                            round(L.Qty,isnull(U.DecimalPlaces,0)) as Qty,
		                    U.UnitName as Unit,
		                    R.ReasonName,
		                    L.Remark AS LineRemark, 
		                    H.Remark AS HeaderRemark
                    FROM  
                    ( 
                    SELECT * FROM [Web]._JobOrderCancelHeaders H WITH (nolock) WHERE 1=1
                    ) H 
                    LEFT JOIN [Web].DocumentTypes DT WITH (nolock) ON DT.DocumentTypeId = H.DocTypeId 
                    left Join Web.DocumentTypeSettings DTS With (Nolock) On DTS.DocumentTypeId=H.DocTypeId
                    LEFT JOIN [Web]._JobOrderCancelLines L WITH (nolock) ON L.JobOrderCancelHeaderId = H.JobOrderCancelHeaderId 
                    LEFT JOIN [Web]._People PS WITH (nolock) ON PS.PersonID = H.JobWorkerId  
                    LEFT JOIN Web.JobOrderLines JOl WITH (nolock) ON L.JobOrderLineId = JOL.JobOrderLineId
                    LEFT JOIN Web.JobOrderHeaders JOH WITH (nolock) ON JOH.JobOrderHeaderId = JOL.JobOrderHeaderId
                    LEFT JOIN web.Processes PR WITH (nolock) ON PR.ProcessId = JOH.ProcessId 
                    LEFT JOIN [Web].Products P WITH (nolock) ON P.ProductId = JOL.ProductId 
                    LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = P.ProductGroupId
                    LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON Pg.ProductTypeId = Pt.ProductTypeId 
                    LEFT JOIN Web.Dimension1Types Dt1 WITH (nolock)  ON Pt.Dimension1TypeId = Dt1.Dimension1TypeId
                    LEFT JOIN Web.Dimension2Types Dt2 WITH (nolock)  ON Pt.Dimension2TypeId = Dt2.Dimension2TypeId
                    LEFT JOIN [Web].Units U WITH (nolock) ON U.UnitId = P.UnitId   
                    LEFT JOIN Web.Dimension1 D1 WITH (nolock) ON D1.Dimension1Id = JOL.Dimension1Id  
                    LEFT JOIN Web.Dimension2 D2 WITH (nolock) ON D2.Dimension2Id = JOL.Dimension2Id 
                    LEFT JOIN Web.Dimension3 D3 WITH (nolock) ON D3.Dimension3Id = JOL.Dimension3Id  
                    LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id = JOL.Dimension4Id
                    LEFT JOIN web.Sites SI WITH (nolock) ON SI.SiteId = H.SiteId
                    LEFT JOIN web.Divisions DI WITH (nolock) ON DI.DivisionId  = H.DivisionId
                    LEFT JOIN [Web].DocumentTypes DTPI WITH (nolock) ON DTPI.DocumentTypeId = JOH.DocTypeId 
                    LEFT JOIN web.Reasons R WITH (nolock) ON R.ReasonId = H.ReasonId
                    LEFT JOIN (
			                    SELECT ProductId FROM Web.ProductCustomGroupLines
			                    WHERE 1=1 		  
			                    GROUP BY ProductId
			                    ) AS PCG  ON  JOL.ProductId =PCG.ProductId
                    WHERE 1=1 AND JOL.ProductId IS NOT NULL 
                    " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["JobWorker"].ToString() != null && form["JobWorker"].ToString() != "" ? " AND H.JobWorkerId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND P.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND P.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY H.JobOrderCancelHeaderId,L.JobOrderLineId,JOL.JobOrderLineId ";


            }

            ReportHeader.SqlProc = mQry;

            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Job Worker", "JobWorker", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "1.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }

        private void JobInvoiceReturnReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Job Invoice Return Report";

            if (form != null)
            {
                mQry = @" SELECT H.JobInvoiceReturnHeaderId AS DocId, H.DocTypeId,                               
                                format(H.DocDate,'dd/MMM/yy') as DocDate,
                                H.DocNo AS DocNo,
                                P.Name AS Name,G.GodownName AS GodownName,JIH.DocNo AS InvoiceNo,
                                format(JIH.DocDate, 'dd/MMM/yy') AS InvoiceDate,
                                JOH.DocNo AS OrderNo,                                
                                format(JOH.DocDate,'dd/MMM/yy') AS OrderDate,
                                PD.ProductName AS Product,                               
                                D1.Dimension1Name as Dimension1,D2.Dimension2Name as Dimension2,D3.Dimension3Name as Dimension3,D4.Dimension4Name as Dimension4,
                                (CASE WHEN H.Nature='Return' THEN round(L.Qty,isnull(U.DecimalPlaces,0)) ELSE 0 END) AS Qty, 
                                (CASE WHEN H.Nature='Return' THEN  U.UnitName ELSE NULL END) AS Unit, 
                                (CASE WHEN H.Nature='Return' THEN  round(isnull(L.DealQty,0),isnull(AU.DecimalPlaces,0)) ELSE 0 END) AS DealQty, 
                                (CASE WHEN H.Nature='Return' THEN  AU.UnitName ELSE NULL END) AS DealUnit, 
                                convert(float,L.Rate) AS Rate,
                                isnull(L.Amount,0) AS Amount
                                 FROM Web.JobInvoiceReturnHeaders H WITH (nolock)
                                LEFT JOIN web.JobInvoiceReturnLines L WITH (Nolock) ON L.JobInvoiceReturnHeaderId=H.JobInvoiceReturnHeaderId
                                LEFT JOIN Web.DocumentTypes DT WITH (Nolock) ON DT.DocumentTypeId=H.DocTypeId
                                LEFT JOIN Web.JobReturnHeaders JRH WITH (Nolock) ON JRH.JobReturnHeaderId=H.JobReturnHeaderId
                                LEFT JOIN Web.Godowns G WITH (Nolock) ON G.GodownId=JRH.GodownId
                                LEFT JOIN Web.JobInvoiceLines JIL WITH (Nolock) ON JIL.JobInvoiceLineId=L.JobInvoiceLineId
                                LEFT JOIN Web.JobInvoiceHeaders JIH WITH (Nolock) ON JIH.JobInvoiceHeaderId=JIL.JobInvoiceHeaderId
                                LEFT JOIN Web.Reasons R WITH (Nolock) ON R.ReasonId=H.ReasonId
                                LEFT JOIN Web._People P WITH (Nolock) ON P.PersonID=H.JobWorkerId
                                LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=H.ProcessId
                                LEFT JOIN Web.Units AU WITH (Nolock) ON AU.UnitId=L.DealUnitId
                                LEFT JOIN web.JobReceiveLines JRL WITH (Nolock) ON JRL.JobReceiveLineId=JIL.JobReceiveLineId
                                LEFT JOIN Web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId=JRL.JobOrderLineId
                                LEFT JOIN Web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderId=JOL.JobOrderHeaderId
                                LEFT JOIN Web.Products PD WITH (Nolock) ON PD.ProductId=JOL.ProductId
                                LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = PD.ProductGroupId
                                LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON Pg.ProductTypeId = Pt.ProductTypeId 
                                LEFT JOIN Web.ViewRugSize VRS WITH (Nolock) ON VRS.ProductId=JOL.ProductId
                                LEFT JOIN Web.Dimension1 D1 WITH (Nolock) ON D1.Dimension1Id=JRL.Dimension1Id
                                LEFT JOIN Web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id=JRL.Dimension2Id
                                LEFT JOIN Web.Dimension3 D3 WITH (Nolock) ON D3.Dimension3Id=JRL.Dimension3Id
                                LEFT JOIN Web.Dimension4 D4 WITH (Nolock) ON D4.Dimension4Id=JRL.Dimension4Id
                                LEFT JOIN Web.Units U WITH (Nolock) ON  U.UnitId=JOL.UnitId Where 1=1 " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["JobWorker"].ToString() != null && form["JobWorker"].ToString() != "" ? " AND H.JobWorkerId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND PD.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND PD.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY L.Sr ";


            }

            ReportHeader.SqlProc = mQry;

            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Job Worker", "JobWorker", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "1.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }

        private void JobReturnReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Job Return Report";

            if (form != null)
            {
                mQry = @" SELECT H.JobReturnHeaderId AS DocId, H.DocTypeId,
                                format(max(H.DocDate),'dd/MMM/yy') AS RetDate,
                                max(H.DocNo) AS RetNo,
                                max(P.Name) AS Name,
                                max(R.reasonName) AS Reason,
                                max(JRH.DocNo) AS ReceiveNo,
                                max(JOH.DocNo) AS OrderNo,
                                max(PD.ProductName) AS Product,
                                sum(round(L.Qty,isnull(U.DecimalPlaces,0))) AS Qty, 
                                max(U.UnitName) AS Unit,
                                sum(round(L.DealQty,isnull(AU.DecimalPlaces,0))) AS DealQty, 
                                max(Au.UnitName) AS DealUnit,                               
                                (
                                SELECT PU.ProductUidName +',' FROM Web.JobReturnLines L WITH (Nolock)
                                LEFT JOIN Web.JobReceiveLines JRL WITH (Nolock) ON  JRL.JobReceiveLineId=L.JobReceiveLineId
                                LEFT JOIN Web.ProductUids PU WITH (Nolock) ON PU.ProductUIDId=JRL.ProductUidId
                                LEFT JOIN Web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId=JRL.JobOrderLineId
                                WHERE JOL.JobOrderHeaderId=JOH.JobOrderHeaderId AND JOL.ProductId=PD.ProductId
                                AND JRL.JobReceiveHeaderId=JRH.JobReceiveHeaderId AND L.JobReturnHeaderId=H.JobReturnHeaderId 
                                ORDER BY L.Sr FOR XML path ('')
                                ) AS Barcode,
                                max(H.Remark) AS HeaderRemark

                                FROM 
                                (
                                SELECT  * FROM Web.JobReturnHeaders H WITH (Nolock) WHERE 1=1
                                ) H
                                LEFT JOIN Web.DocumentTypes DT WITH (Nolock) ON DT.DocumentTypeId=H.DocTypeId
                                LEFT JOIN Web.People P WITH (Nolock) ON P.PersonID=H.JobWorkerId
                                LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=H.ProcessId
                                LEFT JOIN Web.Reasons R WITH (Nolock) ON R.ReasonId=H.ReasonId
                                LEFT JOIN Web.People Pp WITH (Nolock) ON Pp.PersonID=H.OrderById
                                LEFT JOIN Web.JobReturnLines L WITH (Nolock) ON H.JobReturnHeaderId=L.JobReturnHeaderId
                                LEFT JOIN Web.JobReceiveLines JRL WITH (Nolock) ON JRL.JobReceiveLineId=L.JobReceiveLineId
                                LEFT JOIN Web.JobReceiveHeaders JRH WITH (Nolock) ON JRH.JobReceiveHeaderId=JRL.JobReceiveHeaderId
                                LEFT JOIN Web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId=JRL.JobOrderLineId
                                LEFT JOIN Web.JobOrderHeaders JOH WITH (Nolock) ON JOH.JobOrderHeaderId=JOL.JobOrderHeaderId
                                LEFT JOIN Web.Products PD WITH (Nolock) ON PD.ProductId=JOL.ProductId
                                LEFT JOIN Web.Units U WITH (Nolock) ON U.UnitId=Jol.UnitId
                                LEFT JOIN Web.Units AU WITH (Nolock) ON AU.UnitId=L.DealUnitId
                                LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = PD.ProductGroupId
                                LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON Pg.ProductTypeId = Pt.ProductTypeId 
                                WHERE 1=1 
                                AND JOL.ProductId IS NOT NULL   " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["JobWorker"].ToString() != null && form["JobWorker"].ToString() != "" ? " AND H.JobWorkerId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND PD.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND PD.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        "GROUP BY H.JobReturnHeaderId,JOH.JobOrderHeaderId,PD.ProductId,JRH.JobReceiveHeaderId  ORDER BY  H.JobReturnHeaderId ";


            }

            ReportHeader.SqlProc = mQry;

            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Job Worker", "JobWorker", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "1.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }

        #endregion JobGridCustomReport

        #region SaleGridCustomReport
        private void SaleOrderReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Sale Order Report";

            if (form != null)
            {
                mQry = @" SELECT    
                             H.SaleOrderHeaderId AS DocId, H.DocTypeId,
                            format(H.DocDate, 'dd/MMM/yy') As DocDate, 		                    
		                    H.DocNo AS DocNo,format(H.DueDate,'dd/MMM/yy') as DueDate,
		                    PS.Name AS Buyer,
		                    P.ProductName, L.Specification,
                            D1.Dimension1Name AS " + Dimension1Caption + @", 
                            D2.Dimension2Name AS " + Dimension2Caption + @", 
                            D3.Dimension3Name AS " + Dimension3Caption + @", 
                            D4.Dimension4Name AS " + Dimension4Caption + @", 
                            round(L.Qty,isnull(U.DecimalPlaces,0)) as Qty,U.UnitName as Unit,  
                            round(L.DealQty,isnull(AU.DecimalPlaces,0)) AS DealQty,AU.UnitName AS DealUnit,
		                    convert(float,L.Rate) AS Rate,L.Amount AS Amount, 
		                    L.Remark AS LineRemark,H.Remark AS HeaderRemark
                    FROM  
                    ( 
                         SELECT * FROM Web.SaleOrderheaders H WITH (Nolock) WHERE 1=1
                    ) H 
                    LEFT JOIN [Web].DocumentTypes DT WITH (nolock) ON DT.DocumentTypeId = H.DocTypeId 
                    left Join Web.DocumentTypeSettings DTS With (Nolock) On DTS.DocumentTypeId=H.DocTypeId
                    LEFT JOIN Web.SaleOrderLines L WITH (nolock) ON L.SaleOrderHeaderId = H.SaleOrderHeaderId 
                    LEFT JOIN [Web]._People PS WITH (nolock) ON PS.PersonID = H.SaleToBuyerId
                    LEFT JOIN [Web].Products P WITH (nolock) ON P.ProductId = L.ProductId 
                    LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = P.ProductGroupId
                    LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON Pg.ProductTypeId = Pt.ProductTypeId  
                    LEFT JOIN [Web].Units U WITH (nolock) ON U.UnitId=P.UnitId
                    LEFT JOIN [Web].Units AU WITH (nolock) ON AU.UnitId = L.DealUnitId
                    LEFT JOIN Web.Dimension1 D1 WITH (nolock) ON D1.Dimension1Id = L.Dimension1Id  
                    LEFT JOIN Web.Dimension2 D2 WITH (nolock) ON D2.Dimension2Id = L.Dimension2Id 
                    Left Join Web.Dimension3 D3 with (Nolock) On D3.Dimension3Id=L.Dimension3Id
                    Left Join Web.Dimension4 D4 with (Nolock) On D4.Dimension4Id=L.Dimension4Id                 
                    LEFT JOIN web.Sites SI WITH (nolock) ON SI.SiteId = H.SiteId
                    LEFT JOIN web.Divisions DI WITH (nolock) ON DI.DivisionId  = H.DivisionId
                    LEFT JOIN web.ViewRugSize VRS with (nolock) on VRS.ProductId=P.ProductId
                    WHERE 1=1 AND L.ProductId IS NOT NULL   " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["SaleToBuyer"].ToString() != null && form["SaleToBuyer"].ToString() != "" ? " AND H.SaleToBuyerId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND P.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND P.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY H.DocDate,H.DocNo,L.SaleOrderLineId ";


            }

            ReportHeader.SqlProc = mQry;

            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Sale To Buyer", "SaleToBuyer", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "1.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }

        private void SaleOrderCancelReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Sale Cancel Report";

            if (form != null)
            {
                mQry = @" SELECT 
                                        H.SaleOrderCancelHeaderId AS DocId, H.DocTypeId,
                                        DT.DocumentTypeName,  
                                        format(H.DocDate, 'dd/MMM/yy') AS CancelDate, 
		                                DT.DocumentTypeShortName + '-' + H.DocNo AS CancelDocNo, 
		                                PS.Name AS JobWorkerName,
		                                DTPI.DocumentTypeShortName + '-' + Soh.DocNo AS OrderNo,
		                                P.ProductName AS Product,
		                                Sol.Specification, 
                                        D1.Dimension1Name AS " + Dimension1Caption + @", 
                                        D2.Dimension2Name AS " + Dimension2Caption + @", 
                                        D3.Dimension3Name AS " + Dimension3Caption + @", 
                                        D4.Dimension4Name AS " + Dimension4Caption + @", 
                                        round(L.Qty,isnull(U.DecimalPlaces,0)) as Qty,
		                                U.UnitName as Unit,
		                                R.ReasonName,
		                                L.Remark AS LineRemark, 
		                                H.Remark AS HeaderRemark
                                FROM  
                                ( 
                                SELECT * FROM Web.SaleOrderCancelHeaders H WITH (nolock) WHERE 1=1
                                ) H 
                                LEFT JOIN [Web].DocumentTypes DT WITH (nolock) ON DT.DocumentTypeId = H.DocTypeId 
                                left Join Web.DocumentTypeSettings DTS With (Nolock) On DTS.DocumentTypeId=H.DocTypeId
                                LEFT JOIN Web.SaleOrderCancelLines L WITH (nolock) ON L.SaleOrderCancelHeaderId = H.SaleOrderCancelHeaderId
                                LEFT JOIN [Web]._People PS WITH (nolock) ON PS.PersonID = H.BuyerId  
                                LEFT JOIN Web.SaleOrderLines Sol WITH (nolock) ON L.SaleOrderLineId=Sol.SaleOrderLineId                    
                                LEFT JOIN Web.SaleOrderHeaders Soh WITH (nolock) ON Soh.SaleOrderHeaderId=Sol.SaleOrderHeaderId
                                LEFT JOIN [Web].Products P WITH (nolock) ON P.ProductId = Sol.ProductId 
                                LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = P.ProductGroupId
                                LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON Pg.ProductTypeId = Pt.ProductTypeId 
                                LEFT JOIN [Web].Units U WITH (nolock) ON U.UnitId = P.UnitId   
                                LEFT JOIN Web.Dimension1 D1 WITH (nolock) ON D1.Dimension1Id = Sol.Dimension1Id  
                                LEFT JOIN Web.Dimension2 D2 WITH (nolock) ON D2.Dimension2Id = Sol.Dimension2Id 
                                LEFT JOIN Web.Dimension3 D3 WITH (nolock) ON D3.Dimension3Id = Sol.Dimension3Id  
                                LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id = Sol.Dimension4Id
                                LEFT JOIN web.Sites SI WITH (nolock) ON SI.SiteId = H.SiteId
                                LEFT JOIN web.Divisions DI WITH (nolock) ON DI.DivisionId  = H.DivisionId
                                LEFT JOIN [Web].DocumentTypes DTPI WITH (nolock) ON DTPI.DocumentTypeId = Soh.DocTypeId 
                                LEFT JOIN web.Reasons R WITH (nolock) ON R.ReasonId = H.ReasonId
                                LEFT JOIN (
			                                SELECT ProductId FROM Web.ProductCustomGroupLines
			                                WHERE 1=1 		  
			                                GROUP BY ProductId
			                                ) AS PCG  ON  Sol.ProductId =PCG.ProductId
                                WHERE 1=1 AND Sol.ProductId IS NOT NULL   " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["Buyer"].ToString() != null && form["Buyer"].ToString() != "" ? " AND H.BuyerId  IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND P.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND P.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY H.DocDate,H.DocNo,L.SaleOrderCancelLineId ";


            }

            ReportHeader.SqlProc = mQry;
            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Buyer", "Buyer", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "1.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }

        private void SaleInvoiceReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Sale Invoice Report";

            if (form != null)
            {
                mQry = @"  SELECT H.SaleInvoiceHeaderId AS DocId, H.DocTypeId, S.SiteName,
                        format(H.DocDate, 'dd/MMM/yy') As InvoiceDate, D.DocumentTypeShortName + '-' + H.DocNo AS InvoiceNo,                        
                        Jw.Name AS Customer,                        
                        Cgp.ChargeGroupPersonName AS SalesTaxGroupPerson, 
                        C.Name AS Currency,                        	 
                        SDHD.DocumentTypeShortName + '-' + SDH.DocNo AS DispatchNo,
                        Od.DocumentTypeShortName + '-' + Soh.DocNo AS OrderNo, 
                        Cpt.ChargeGroupProductName AS SalesTaxGroupProduct,
                        Pu.ProductUidName AS " + ProductUidCaption + @", 
                        P.ProductName as Product, 
                        D1.Dimension1Name AS " + Dimension1Caption + @", 
                        D2.Dimension2Name AS " + Dimension2Caption + @", 
                        D3.Dimension3Name AS " + Dimension3Caption + @", 
                        D4.Dimension4Name AS " + Dimension4Caption + @", 
                        L.Qty, U.UnitName AS Unit, 
                        L.DealQty, Du.UnitName AS DealUnit, 
                        convert(float,L.Rate) AS Rate,
                        L.Amount, L.Remark AS LineRemark, H.Remark
                        FROM Web.SaleInvoiceHeaders  H 
                        LEFT JOIN Web.SaleInvoiceLines L ON H.SaleInvoiceHeaderId=L.SaleInvoiceHeaderId
                        LEFT JOIN Web.DocumentTypes D ON H.DocTypeId = D.DocumentTypeId                                             
                        LEFT JOIN Web.People Jw ON Jw.PersonID = H.BillToBuyerId
                        LEFT JOIN Web.ChargeGroupPersons Cgp ON H.SalesTaxGroupPersonId = Cgp.ChargeGroupPersonId
                        LEFT JOIN Web.Currencies C ON H.CurrencyId = C.ID                        
                        LEFT JOIN Web.SaleDispatchLines SDL ON SDL.SaleDispatchLineId = L.SaleDispatchLineId  
                        LEFT JOIN Web.SaleDispatchHeaders SDH ON SDH.SaleDispatchHeaderId=SDL.SaleDispatchHeaderId  
                        LEFT JOIN Web.PackingLines PL ON PL.PackingLineId  = SDL.PackingLineId                         
                        LEFT JOIN Web.DocumentTypes SDHD ON SDH.DocTypeId = SDHD.DocumentTypeId                        
                        LEFT JOIN Web.Units Du ON L.DealUnitId = Du.UnitId 
                        LEFT JOIN Web.SaleOrderLines Sol ON Sol.SaleOrderLineId=PL.SaleOrderLineId
                        LEFT JOIN Web.Saleorderheaders Soh ON Soh.SaleOrderHeaderId=Sol.SaleOrderHeaderId
                        LEFT JOIN Web.DocumentTypes Od ON Soh.DocTypeId = Od.DocumentTypeId
                        LEFT JOIN Web.ChargeGroupProducts Cpt ON L.SalesTaxGroupProductId = Cpt.ChargeGroupProductId
                        LEFT JOIN Web.Products P WITH (nolock) ON P.ProductId = Pl.ProductId 
                        LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = P.ProductGroupId
                        LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON Pg.ProductTypeId = Pt.ProductTypeId                                
                        LEFT JOIN Web.ProductUids Pu On Pl.ProductUidId = Pu.ProductUidId
                        LEFT JOIN Web.Units U WITH (nolock) ON U.UnitId = P.UnitId
                        LEFT JOIN Web.Dimension1 D1 WITH (nolock) ON D1.Dimension1Id = Pl.Dimension1Id  
                        LEFT JOIN Web.Dimension2 D2 WITH (nolock) ON D2.Dimension2Id = Pl.Dimension2Id 
                        LEFT JOIN Web.Dimension3 D3 WITH (nolock) ON D3.Dimension3Id = Pl.Dimension3Id  
                        LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id = Pl.Dimension4Id  
                        LEFT JOIN Web.Sites S On H.SiteId = S.SiteId
                          where 1=1 " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["Buyer"].ToString() != null && form["Buyer"].ToString() != "" ? " AND H.BillToBuyerId  IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                         (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND P.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND P.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY H.DocDate,H.DocNo,L.SaleInvoicelineId ";


            }

            ReportHeader.SqlProc = mQry;
            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Buyer", "Buyer", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }

        private void PackingReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Packing Report";

            if (form != null)
            {
                mQry = @"  SELECT 
                                H.SaleOrderCancelHeaderId AS DocId, H.DocTypeId,
                                format(H.DocDate, 'dd/MMM/yy') AS PackingDate,H.DocNo AS PackingNo,
                                SOH.DocNo AS OrderNo,P.ProductName AS ProductName,
                                SOL.Specification AS Specification,PG.ProductGroupName AS ProductGroupName,
                                D1.Dimension1Name AS " + Dimension1Caption + @", 
                                D2.Dimension2Name AS " + Dimension2Caption + @", 
                                D3.Dimension3Name AS " + Dimension3Caption + @", 
                                D4.Dimension4Name AS " + Dimension4Caption + @", 
                                isnull(L.Qty,0) AS Qty,U.UnitName AS Unit,L.BaleNo AS BaleNo,  
                                PU.ProductUidName AS " + ProductUidCaption + @", 
                                PS.ProcessName AS FromProcessName
                                FROM (SELECT  * FROM Web.PackingHeaders H WITH (Nolock)) H 
                                LEFT JOIN Web._DocumentTypeSettings DTS WITH (Nolock) ON DTS.DocumentTypeId=H.DocTypeId
                                LEFT JOIN Web.PackingLines L WITH (Nolock) ON L.PackingHeaderId = H.PackingHeaderId 
                                LEFT JOIN Web.Products P WITH (Nolock) ON P.ProductId = L.ProductId 
                                LEFT JOIN Web.SaleOrderLines SOL WITH (Nolock) ON SOL.SaleOrderLineId = L.SaleOrderLineId 
                                LEFT JOIN Web.SaleOrderHeaders SOH WITH (Nolock) ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId 
                                LEFT JOIN Web.ProductUids PU WITH (Nolock) ON  PU.ProductUIDId = L.ProductUidId 
                                LEFT JOIN Web.FinishedProduct FPD WITH (Nolock) ON P.ProductId = FPD.ProductId  
                                LEFT JOIN Web.Units U WITH (Nolock) ON U.UnitId = P.UnitId   
                                LEFT JOIN Web.ViewRugArea VRA ON VRA.ProductId = P.ProductId   
                                LEFT JOIN Web.ProductQualities PQ ON PQ.ProductQualityId = FPD.ProductQualityId    
                                LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = P.ProductGroupId 
                                LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON PG.ProductTypeId = PT.ProductTypeId 
                                LEFT JOIN web.Sites S ON S.SiteId = H.SiteId
                                LEFT JOIN Web.Processes PS WITH (Nolock) ON PS.ProcessId=L.FromProcessId
                                LEFT JOIN web.ViewDivisionCompany  VDC ON VDC.DivisionId = H.DivisionId
                                LEFT JOIN Web.Dimension1 D1 WITH (nolock) ON D1.Dimension1Id = L.Dimension1Id  
                                LEFT JOIN Web.Dimension2 D2 WITH (nolock) ON D2.Dimension2Id = L.Dimension2Id 
                                LEFT JOIN Web.Dimension3 D3 WITH (nolock) ON D3.Dimension3Id = L.Dimension3Id  
                                LEFT JOIN Web.Dimension4 D4 WITH (nolock) ON D4.Dimension4Id = L.Dimension4Id
                                LEFT JOIN (
			                                SELECT ProductId FROM Web.ProductCustomGroupLines
			                                WHERE 1=1 
			                                GROUP BY ProductId
			                                ) AS PCG  ON  L.ProductId=PCG.ProductId
                                WHERE 1=1  AND L.PackingLineId IS NOT NULL " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["Buyer"].ToString() != null && form["Buyer"].ToString() != "" ? " AND H.BuyerId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                         (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND P.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND P.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY H.DocDate,H.DocNo,L.PackingLineId ";


            }

            ReportHeader.SqlProc = mQry;
            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Buyer", "Buyer", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }

        private void SaleOrderAmendmentReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            GetDivisionSettings();

            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Sale Order Amendment Report";

            if (form != null)
            {
                mQry = @"  SELECT H.SaleOrderAmendmentHeaderId As DocId, H.DocTypeId, format(H.DocDate, 'dd/MMM/yy') AS DocDate,
                                    H.DocNo AS DocNo, 
                                    R.ReasonName AS ReasonName,  
                                    SOH.DocNo AS SaleOrderNo, 
                                    B.Name AS BuyerName,
                                    PD.ProductName AS ProductName, 
                                    SOL.Specification AS Specification,
                                    L.Qty AS Qty,U.UnitName AS UnitName,
                                    D1.Dimension1Name AS " + Dimension1Caption + @", 
                                    D2.Dimension2Name AS " + Dimension2Caption + @", 
                                    D3.Dimension3Name AS " + Dimension3Caption + @", 
                                    D4.Dimension4Name AS " + Dimension4Caption + @", 
                                    L.Remark AS LineRemark,
                                    H.Remark AS Remark
                                    FROM  Web.SaleOrderAmendmentHeaders H  
                                    LEFT JOIN Web.SaleOrderQtyAmendmentLines L ON L.SaleOrderAmendmentHeaderId = H.SaleOrderAmendmentHeaderId   
                                    LEFT JOIN Web.DocumentTypes Dt ON Dt.DocumentTypeId = H.DocTypeId   
                                    LEFT JOIN Web.Reasons R ON R.ReasonId = H.ReasonId   
                                    LEFT JOIN Web.SaleOrderLines SOL ON SOL.SaleOrderLineId = L.SaleOrderLineId   
                                    LEFT JOIN Web.SaleOrderHeaders SOH ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId  
                                    LEFT JOIN Web.Dimension1 D1 WITH (Nolock) ON D1.Dimension1Id=SOL.Dimension1Id
                                    LEFT JOIN Web.Dimension2 D2 WITH (Nolock) ON D2.Dimension2Id=SOL.Dimension2Id
                                    LEFT JOIN Web.Dimension3 D3 WITH (Nolock) ON D3.Dimension3Id=Sol.Dimension3Id
                                    LEFT JOIN Web.Dimension4 D4 WITH (Nolock) ON D4.Dimension4Id=Sol.Dimension4Id
                                    LEFT JOIN Web.People B ON B.PersonID = SOH.SaleToBuyerId   
                                    LEFT JOIN Web.Products PD ON PD.ProductId = SOL.ProductId   
                                    LEFT JOIN Web.FinishedProduct FPD ON PD.ProductId = FPD.ProductId   
                                    LEFT JOIN Web.Units U ON U.UnitId = PD.UnitId   
                                    LEFT JOIN Web.ViewRugSize UC ON UC.ProductId = SOL.ProductId  
                                    LEFT JOIN Web.Sizes PS ON PS.SizeId = UC.StandardSizeID
                                    LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId=PD.ProductGroupId
                                    LEFT JOIN Web.ProductTypes PT ON PT.ProductTypeId =PG.ProductTypeId
                                    LEFT JOIN (
			                                    SELECT ProductId FROM Web.ProductCustomGroupLines
			                                    WHERE 1=1 
			                                    GROUP BY ProductId
			                                    ) AS PCG  ON  SOL.ProductId =PCG.ProductId   
                                    WHERE 1=1  " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["Buyer"].ToString() != null && form["Buyer"].ToString() != "" ? " AND SOH.SaleToBuyerId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND PD.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND PD.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY H.DocDate,H.DocNo,L.SaleOrderQtyAmendmentLineId ";


            }

            ReportHeader.SqlProc = mQry;
            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Buyer", "Buyer", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }
        #endregion SaleGridCustomReport



        #region StoreReport
        private void StoreReport(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form, String ReportName)
        {
            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = ReportName;

            if (form != null)
            {
                string Select = "";
                if (ReportName == "Stock Transfer Report")
                {
                    Select = "H.StockHeaderId AS DocId, H.DocTypeId, format(H.DocDate, 'dd/MMM/yy') AS TransferDate,H.DocNo AS TransferNo,FGD.GodownName as FromGodown, TGD.GodownName as ToGodown,PD.ProductName as Item,round(L.Qty,isnull(U.DecimalPlaces,0)) as Qty, U.UnitName AS Unit,  H.Remark AS HeaderRemrk,L.Remark AS LineRemark  ";
                }
                else if (ReportName == "Store Receive Report")
                {
                    Select = @"  H.StockHeaderId AS DocId, H.DocTypeId, format(H.DocDate, 'dd/MMM/yy') AS ReceiveDate,H.DocNo AS ReceiveNo,DocumentTypeName AS Documenttypename,P.Name AS JobWorker,WP.ProcessName,PD.ProductName,
                                L.Specification,L.LotNo,round(L.Qty, isnull(U.DecimalPlaces, 0)) as Qty, U.UnitName AS Unit,  
                                isnull(L.Amount, 0) AS Amount,
                                convert(float, L.Rate) AS Rate,
                                D1.Dimension1Name AS " + Dimension1Caption + @", 
                                D2.Dimension2Name AS " + Dimension2Caption + @", 
                                D3.Dimension3Name AS " + Dimension3Caption + @", 
                                D4.Dimension4Name AS " + Dimension4Caption + @", 
                                H.Remark AS HeaderRemrk,
                                L.Remark AS LineRemark ";
                }
                else if (ReportName == "Store Issue Report")
                {
                    Select = @"  H.StockHeaderId AS DocId, H.DocTypeId, format(H.DocDate, 'dd/MMM/yy') AS IssueDate,H.DocNo AS IssueNo,DocumentTypeName AS Documenttypename,P.Name AS JobWorker,WP.ProcessName,PD.ProductName,
                                L.Specification,L.LotNo,round(L.Qty, isnull(U.DecimalPlaces, 0)) as Qty, U.UnitName AS Unit,  
                                isnull(L.Amount, 0) AS Amount,
                                convert(float, L.Rate) AS Rate,
                                D1.Dimension1Name AS " + Dimension1Caption + @", 
                                D2.Dimension2Name AS " + Dimension2Caption + @", 
                                D3.Dimension3Name AS " + Dimension3Caption + @", 
                                D4.Dimension4Name AS " + Dimension4Caption + @", 
                                H.Remark AS HeaderRemrk,
                                L.Remark AS LineRemark ";
                }

                mQry = @" SELECT 
                              	" + Select + @"
                                FROM  
                                ( 
                                SELECT * FROM Web.StockHeaders H  
                                 ) H 
                                LEFT JOIN Web.StockLines L WITH (nolock)  ON H.StockHeaderId=L.StockHeaderId 
                                LEFT JOIN Web.DocumentTypes DT WITH (nolock) ON DT.DocumentTypeId = H.DocTypeId 
                                LEFT JOIN Web._People P WITH (nolock) ON P.PersonID =H.PersonId
                                LEFT JOIN Web.Products PD WITH (nolock) ON PD.ProductId = L.ProductId 
                                LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId  = PD.ProductGroupId
                                LEFT JOIN Web.ProductTypes PT WITH (nolock)  ON PG.ProductTypeId = Pt.ProductTypeId 
                                LEFT JOIN [Web].Units U WITH (nolock) ON U.UnitId = PD.UnitId   
                                LEFT JOIN Web.Dimension1 D1 WITH (nolock) ON D1.Dimension1Id = L.Dimension1Id  
                                LEFT JOIN Web.Dimension2 D2 WITH (nolock) ON D2.Dimension2Id = L.Dimension2Id 
                                LEFT JOIN Web.Dimension3 D3 WITH (Nolock) ON D3.Dimension3Id=L.Dimension3Id
                                LEFT JOIN Web.Dimension4 D4 WITH (Nolock) ON D4.Dimension4Id=L.Dimension4Id
                                LEFT JOIN Web.Processes WP WITH (nolock) ON WP.ProcessId=H.ProcessId
                                LEFT JOIN web.Sites SI WITH (nolock) ON SI.SiteId = H.SiteId
                                LEFT JOIN web.Divisions DI WITH (nolock) ON DI.DivisionId  = H.DivisionId
                                LEFT JOIN Web.Godowns FGD WITH (nolock) ON H.FromGodownId=FGD.GodownId
                                LEFT JOIN Web.Godowns TGD WITH (nolock) ON H.GodownId=TGD.GodownId
                                LEFT JOIN (
			                                SELECT ProductId FROM Web.ProductCustomGroupLines
			                                WHERE 1=1 
		   	                                 GROUP BY ProductId
			                                ) AS PCG  ON  L.ProductId=PCG.ProductId
                                WHERE 1=1  
                                AND isnull(L.Qty,0) <> 0     " +
                        (form["DocumentType"].ToString() != null && form["DocumentType"].ToString() != "" ? " AND H.DocTypeId IN (SELECT Items FROM [dbo].[Split] (@DocumentType, ','))" : "") +
                        (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                        (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                        (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                        (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                        (form["JobWorker"].ToString() != null && form["JobWorker"].ToString() != "" ? " AND H.PersonId IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                        (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                        (form["ProductGroup"].ToString() != null && form["ProductGroup"].ToString() != "" ? " AND PD.ProductGroupId IN (SELECT Items FROM [dbo].[Split] (@ProductGroup, ','))" : "") +
                        (form["ProductCategory"].ToString() != null && form["ProductCategory"].ToString() != "" ? " AND PD.ProductCategoryId IN (SELECT Items FROM [dbo].[Split] (@ProductCategory, ','))" : "") +
                        (form["ProductNature"].ToString() != null && form["ProductNature"].ToString() != "" ? " AND Pt.ProductNatureId IN (SELECT Items FROM [dbo].[Split] (@ProductNature, ','))" : "") +
                        (form["ProductType"].ToString() != null && form["ProductType"].ToString() != "" ? " AND PG.ProductTypeId IN (SELECT Items FROM [dbo].[Split] (@ProductType, ','))" : "") +
                        " ORDER BY L.StockLineId ";


            }

            ReportHeader.SqlProc = mQry;

            AddReportParameters(ref ReportLineList, "Document Type", "DocumentType", "Multi Select", "1.Filter", "GetDocumentType", "SetDocumentType", "@DocumentType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "1.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "1.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "1.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "1.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Job Worker", "JobWorker", "Multi Select", "1.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "1.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Group", "ProductGroup", "Multi Select", "1.Filter", "GetProductGroup", "SetProductGroup", "@ProductGroup", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Category", "ProductCategory", "Multi Select", "1.Filter", "GetProductCategory", "SetProductCategory", "@ProductCategory", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Type", "ProductType", "Multi Select", "1.Filter", "GetProductType", "SetProductType", "@ProductType", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Product Nature", "ProductNature", "Multi Select", "1.Filter", "GetProductNature", "SetProductNature", "@ProductNature", null, null, true, null, false, false);

        }
        #endregion 
        #region JobInvoiceSummary
        private void JobInvoiceSummary(ref ReportHeader ReportHeader, ref List<ReportLine> ReportLineList, FormCollection form)
        {
            ReportHeader = new ReportHeader();
            ReportHeader.ReportName = "Job Invoice Summary Report";

            if (form != null)
            {
                
                   mQry = @"WITH cteViewJobInvoiceLineCharges AS
                            (
	                                SELECT L.LineTableId AS JobInvoiceLineId, 
                            Sum(CASE WHEN C.ChargeName='Gross Amount' THEN L.Amount ELSE 0 END) AS GrossAmount,
                            Sum(CASE WHEN C.ChargeName='Incentive' THEN L.Amount ELSE 0 END) AS IncentiveAmt,
                            Max(CASE WHEN C.ChargeName='Incentive' THEN L.Rate ELSE 0 END) AS IncentiveRate,
                            Sum(CASE WHEN C.ChargeName='Penalty' THEN L.Amount ELSE 0 END) AS PenaltyAmt,
                            Sum(CASE WHEN C.ChargeName='Net Amount' THEN L.Amount ELSE 0 END) AS NetAmount,
                            sum(CASE WHEN C.ChargeName='CGST' THEN L.Amount ELSE 0 END) AS CGSTAmt ,
                            sum(CASE WHEN C.ChargeName='SGST' THEN L.Amount ELSE 0 END) AS SGSTAmt,
                            max(CASE WHEN C.ChargeName='CGST' THEN L.Rate ELSE 0 END) AS CGSTPer ,
                            max(CASE WHEN C.ChargeName='SGST' THEN L.Rate ELSE 0 END) AS SGSTPer,
                            sum(CASE WHEN C.ChargeName IN ('CGST','SGST') THEN L.Amount ELSE 0 END) AS GSTAmt ,
                            --New Added
                            sum(CASE WHEN C.ChargeName IN ('Sales Taxable Amount','Sales Tax Taxable Amt','Expense') THEN L.Amount ELSE 0 END) AS TaxableAmount,
                            Sum(CASE WHEN C.ChargeName ='IGST' THEN L.Amount ELSE 0 End) AS IGSTAmt,
                            Max(CASE WHEN C.ChargeName ='IGST' THEN L.Rate ELSE 0 End) AS IGSTPer
                            FROM Web.JobInvoiceLineCharges L WITH (NoLock)
                            LEFT JOIN web.Charges C WITH (Nolock) ON C.ChargeId=L.ChargeId
                            GROUP BY L.LineTableId
                            ),
                        cteViewJobInvoiceReturnLineCharges AS
                            (
	                               SELECT L.LineTableId,
                                    Sum(CASE WHEN C.ChargeName='Gross Amount' THEN L.Amount ELSE 0 END) AS GrossAmount,
                                    Sum(CASE WHEN C.ChargeName='Incentive' THEN L.Amount ELSE 0 END) AS IncentiveAmt,
                                    Max(CASE WHEN C.ChargeName='Incentive' THEN L.Rate ELSE 0 END) AS IncentiveRate,
                                    Sum(CASE WHEN C.ChargeName='Penalty' THEN L.Amount ELSE 0 END) AS PenaltyAmt,
                                    Sum(CASE WHEN C.ChargeName='Net Amount' THEN L.Amount ELSE 0 END) AS NetAmount,
                                    sum(CASE WHEN C.ChargeName='CGST' THEN L.Amount ELSE 0 END) AS CGSTAmt ,
                                    sum(CASE WHEN C.ChargeName='SGST' THEN L.Amount ELSE 0 END) AS SGSTAmt,
                                    max(CASE WHEN C.ChargeName='CGST' THEN L.Rate ELSE 0 END) AS CGSTPer ,
                                    max(CASE WHEN C.ChargeName='SGST' THEN L.Rate ELSE 0 END) AS SGSTPer,
                                    sum(CASE WHEN C.ChargeName IN ('Sales Taxable Amount','Sales Tax Taxable Amt') THEN L.Amount ELSE 0 END) AS TaxableAmount,
                                    Sum(CASE WHEN C.ChargeName ='IGST' THEN L.Amount ELSE 0 End) AS IGSTAmt,
                                    Max(CASE WHEN C.ChargeName ='IGST' THEN L.Rate ELSE 0 End) AS IGSTPer
                                     FROM 
                                    Web.JobInvoiceReturnLineCharges L WITH (Nolock)
                                    LEFT JOIN web.Charges C WITH (Nolock) ON C.ChargeId=L.ChargeId
                                    GROUP BY L.LineTableId
                            ),
                        cteViewJobInvoiceRateAmendmentLinecharges AS
                            (
	                                        SELECT L.LineTableId,
                                Sum(CASE WHEN C.ChargeName='Gross Amount' THEN L.Amount ELSE 0 END) AS GrossAmount,
                                Sum(CASE WHEN C.ChargeName='Net Amount' THEN L.Amount ELSE 0 END) AS NetAmount,
                                Sum(CASE WHEN C.ChargeName='Incentive' THEN L.Amount ELSE 0 END) AS IncentiveAmt,
                                Max(CASE WHEN C.ChargeName='Incentive' THEN L.Rate ELSE 0 END) AS IncentiveRate,
                                Sum(CASE WHEN C.ChargeName='Penalty' THEN L.Amount ELSE 0 END) AS PenaltyAmt,
                                sum(CASE WHEN C.ChargeName='CGST' THEN L.Amount ELSE 0 END) AS CGSTAmt ,
                                max(CASE WHEN C.ChargeName='CGST' THEN L.Rate ELSE 0 END) AS CGSTRate ,
                                sum(CASE WHEN C.ChargeName='SGST' THEN L.Amount ELSE 0 END) AS SGSTAmt,
                                max(CASE WHEN C.ChargeName='SGST' THEN L.Rate ELSE 0 END) AS SGSTRate,
                                sum(CASE WHEN C.ChargeName IN ('CGST','SGST') THEN L.Amount ELSE 0 END) AS GSTAmt ,
                                sum(CASE WHEN C.ChargeName IN ('Sales Taxable Amount','Sales Tax Taxable Amt') THEN L.Amount ELSE 0 END) AS TaxableAmount
                                FROM Web.JobInvoiceRateAmendmentLinecharges L WITH (Nolock)
                                LEFT JOIN Web.Charges C WITH (Nolock) ON C.ChargeId=L.ChargeId
                                GROUP BY L.LineTableId
                            ),
                          cteJobInvoiceLineCharges AS
                            (
	                          SELECT H.JobInvoiceLineId AS JobInvoiceLineId,
                                isnull(H.GrossAmount,0)+isnull(JIRAL.GrossAmount,0) AS GrossAmount,
                                isnull(H.IncentiveAmt,0)+isnull(JIRAL.IncentiveAmt,0) AS IncentiveAmt,
                                isnull(H.IncentiveRate,0) AS IncentiveRate,
                                isnull(H.PenaltyAmt,0)+isnull(JIRAL.PenaltyAmt,0) AS PenaltyAmt,
                                isnull(H.NetAmount,0)+isnull(JIRAL.NetAmount,0) AS NetAmount,
                                isnull(H.CGSTAmt,0)+isnull(JIRAL.CGSTAmt,0) AS CGSTAmt,
                                isnull(H.SGSTAmt,0)+isnull(JIRAL.SGSTAmt,0) AS SGSTAmt,
                                H.CGSTPer AS CGSTPer,H.SGSTPer AS SGSTPer,
                                isnull(H.TaxableAmount,0)+isnull(JIRAL.TaxableAmount,0) AS TaxableAmount,
                                isnull(H.GSTAmt,0)+isnull(JIRAL.GSTAmt,0) AS GSTAmt,
                                H.IGSTAmt AS IGSTAmt,H.IGSTPer AS IGSTPer
                                  FROM cteViewJobInvoiceLineCharges H
                                LEFT JOIN
	                                (
		                                SELECT JIRAL.JobInvoiceLineId,sum(isnull(LC.GrossAmount,0)) AS GrossAmount,sum(isnull(LC.IncentiveAmt,0)) AS IncentiveAmt,
		                                sum(isnull(LC.PenaltyAmt,0)) AS PenaltyAmt,sum(isnull(LC.NetAmount,0)) AS NetAmount,
		                                sum(isnull(LC.CGSTAmt,0)) AS CGSTAmt,sum(isnull(LC.SGSTAmt,0)) AS SGSTAmt,sum(isnull(LC.TaxableAmount,0)) AS TaxableAmount,
		                                sum(isnull(LC.GSTAmt,0)) AS GSTAmt		
		                                FROM Web.JobInvoiceRateAmendmentLines JIRAL WITH (Nolock)
		                                LEFT JOIN cteViewJobInvoiceRateAmendmentLinecharges LC ON LC.LineTableId=JIRAL.JobInvoiceRateAmendmentLineId
		                                GROUP BY JIRAL.JobInvoiceLineId
	                                ) JIRAL ON JIRAL.JobInvoiceLineId=H.JobInvoiceLineId
                            ),
                          cteJobInvoiceReturnLineCharges AS
                            (
	                          SELECT L.LineTableId,
                                Sum(CASE WHEN C.ChargeName='Gross Amount' THEN L.Amount ELSE 0 END) AS GrossAmount,
                                Sum(CASE WHEN C.ChargeName='Incentive' THEN L.Amount ELSE 0 END) AS IncentiveAmt,
                                Max(CASE WHEN C.ChargeName='Incentive' THEN L.Rate ELSE 0 END) AS IncentiveRate,
                                Sum(CASE WHEN C.ChargeName='Penalty' THEN L.Amount ELSE 0 END) AS PenaltyAmt,
                                Sum(CASE WHEN C.ChargeName='Net Amount' THEN L.Amount ELSE 0 END) AS NetAmount,
                                sum(CASE WHEN C.ChargeName='CGST' THEN L.Amount ELSE 0 END) AS CGSTAmt ,
                                sum(CASE WHEN C.ChargeName='SGST' THEN L.Amount ELSE 0 END) AS SGSTAmt,
                                max(CASE WHEN C.ChargeName='CGST' THEN L.Rate ELSE 0 END) AS CGSTPer ,
                                max(CASE WHEN C.ChargeName='SGST' THEN L.Rate ELSE 0 END) AS SGSTPer,
                                --New Added
                                sum(CASE WHEN C.ChargeName IN ('Sales Taxable Amount','Sales Tax Taxable Amt') THEN L.Amount ELSE 0 END) AS TaxableAmount,
                                Sum(CASE WHEN C.ChargeName ='IGST' THEN L.Amount ELSE 0 End) AS IGSTAmt,
                                Max(CASE WHEN C.ChargeName ='IGST' THEN L.Rate ELSE 0 End) AS IGSTPer
                                 FROM 
                                Web.JobInvoiceReturnLineCharges L WITH (Nolock)
                                LEFT JOIN web.Charges C WITH (Nolock) ON C.ChargeId=L.ChargeId
                                GROUP BY L.LineTableId
                            ),
                           cteJobInvoice AS
                            (
                                            SELECT 
                                            P.Name AS Name ,
                                            H.DocDate AS DocDate,
                                            PD.ProductName AS ProductName,
                                            PG.ProductGroupName AS ProductGroupName,                                            
                                            round(L.Qty,isnull(U.DecimalPlaces,0)) as Qty,
                                            U.UnitName AS Unit,                                            
                                            round(L.DealQty,isnull(AU.DecimalPlaces,0)) AS DealQty,
                                            Au.UnitName AS DealUnit,                                           
                                            isnull(L.Amount,0) AS Amount,
                                            isnull(LC.TaxableAmount,0) AS TaxableAmount,
                                            (CASE WHEN CGP.ChargeGroupPersonName IN ('State (Registered)','Ex-State (Registered)') THEN 0 ELSE isnull(LC.IGSTAmt,0) END) AS IGST,
                                            (CASE WHEN CGP.ChargeGroupPersonName IN ('State (Registered)','Ex-State (Registered)') THEN 0 ELSE isnull(LC.CGSTAmt,0) END) AS CGST,
                                            (CASE WHEN CGP.ChargeGroupPersonName IN ('State (Registered)','Ex-State (Registered)') THEN 0 ELSE isnull(LC.SGSTAmt,0) END) AS SGST,
                                            isnull(LC.NetAmount,0) AS InvoiceAmount
                                            FROM Web.JobInvoiceHeaders H
                                            LEFT JOIN Web.JobInvoiceLines L WITH (Nolock) ON L.JobInvoiceHeaderId=H.JobInvoiceHeaderId
                                            LEFT JOIN web.People P WITH (Nolock) ON P.PersonID=isnull(H.JobWorkerId,L.JobWorkerId)
                                            LEFT JOIN web.JobReceiveLines JRL WITH (Nolock) ON JRL.JobReceiveLineId=L.JobReceiveLineId
                                            LEFT JOIN web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId=JRL.JobOrderLineId
                                            LEFT JOIN web.Products PD WITH (Nolock) ON PD.ProductId=JOL.ProductId
                                            LEFT JOIN web.Units U WITH (Nolock) ON U.UnitId=PD.UnitId
                                            LEFT JOIN web.Units AU WITH (Nolock) ON AU.UnitId=L.DealUnitId
                                            LEFT JOIN web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId=PD.ProductGroupId	
                                            LEFT JOIN web.ChargeGroupPersons CGP WITH (Nolock) ON CGP.ChargeGroupPersonId=H.SalesTaxGroupPersonId	   
                                            Left Join cteJobInvoiceLineCharges LC ON LC.JobInvoiceLineId=L.JobInvoicelineId
                                            where 1=1 " + 
                                                (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                                                (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                                                (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                                                (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                                                (form["JobWorker"].ToString() != null && form["JobWorker"].ToString() != "" ? "  AND  P.PersonID IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                                                (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                                     @"
                            ),
                           cteJobInvoiceReturn AS
                            (
                                    SELECT 
                                    P.Name AS Name,
                                    H.DocDate AS DocDate,			
                                    PD.ProductName AS ProductName,
                                    PG.ProductGroupName AS ProductGroupName,
                                    (CASE WHEN H.Nature='Return' THEN  round(L.Qty,isnull(U.DecimalPlaces,0)) ELSE 0 END) AS Qty, 
                                    U.UnitName AS Unit,
                                    (CASE WHEN H.Nature='Return' THEN round(L.DealQty,isnull(AU.DecimalPlaces,0)) ELSE 0 END) AS DealQty,
                                    AU.UnitName  AS DealUnit,
                                    isnull(L.Amount,0) AS Amount,
                                    isnull(JIRL.TaxableAmount,0) AS TaxableAmount,
                                    (CASE WHEN CGP.ChargeGroupPersonName IN ('State (Registered)','Ex-State (Registered)') THEN 0 ELSE isnull(JIRL.IGST,0) END) AS IGST,
                                    (CASE WHEN CGP.ChargeGroupPersonName IN ('State (Registered)','Ex-State (Registered)') THEN 0 ELSE isnull(JIRL.CGST,0) END) AS CGST,
                                    (CASE WHEN CGP.ChargeGroupPersonName IN ('State (Registered)','Ex-State (Registered)') THEN 0 ELSE isnull(JIRL.SGST,0) END) AS SGST,
                                    isnull(JIRL.NetAmount,0) AS InvoiceAmount,
                                     H.Nature
                                    FROM Web.JobInvoiceReturnLines L
                                    LEFT JOIN Web.JobInvoiceReturnHeaders H WITH (Nolock) ON H.JobInvoiceReturnHeaderId=L.JobInvoiceReturnHeaderId
                                    LEFT JOIN 
                                    (	SELECT 
                                    H.LineTableId,H.TaxableAmount,H.IGSTAmt AS IGST,H.IGSTPer AS IGSTRate,H.CGSTAmt AS CGST,
                                    H.CGSTPer AS CGSTRate,H.SGSTAmt AS SGST, H.SGSTPer AS SGSTRate,H.NetAmount AS NetAmount
                                    FROM cteViewJobInvoiceReturnLineCharges H
                                    ) JIRL ON JIRL.LineTableId=L.JobInvoiceReturnLineId
                                    LEFT JOIN web.People P WITH (Nolock) ON P.PersonID=H.JobWorkerId 
                                    LEFT JOIN Web.JobInvoiceLines JIL WITH (Nolock) ON JIL.JobInvoiceLineId=L.JobInvoiceLineId
                                    LEFT JOIN web.ChargeGroupProducts CGPD WITH (Nolock) ON JIL.SalesTaxGroupProductId = CGPD.ChargeGroupProductId
                                    LEFT JOIN web.JobInvoiceHeaders JIH WITH (Nolock) ON JIH.JobInvoiceHeaderId=JIL.JobInvoiceHeaderId
                                    LEFT JOIN web.ChargeGroupPersons CGP WITH (Nolock) ON CGP.ChargeGroupPersonId=JIH.SalesTaxGroupPersonId			
                                    LEFT JOIN Web.jobreceivelines JRL WITH (Nolock) ON  JRL.JobReceiveLineId=JIL.JobReceiveLineId
                                    LEFT JOIN web.JobOrderLines JOL WITH (Nolock) ON JOL.JobOrderLineId=JRL.JobOrderLineId
                                    LEFT JOIN web.Products PD WITH (Nolock) ON PD.ProductId=JOL.ProductId
                                    LEFT JOIN Web.ProductGroups PG WITH (Nolock) ON PG.ProductGroupId=PD.ProductGroupId
                                    LEFT JOIN Web.SalesTaxProductCodes STC WITH (Nolock) ON STC.SalesTaxProductCodeId= IsNull(PD.SalesTaxProductCodeId, Pg.DefaultSalesTaxProductCodeId)
                                    LEFT JOIN web.Units U WITH (Nolock) ON U.UnitId=PD.UnitId  
                                    LEFT JOIN web.Units AU WITH (Nolock) ON AU.UnitId=L.DealUnitId	                                  					
                                    WHERE 1=1 AND H.Nature IN ('Debit Note', 'Return','Credit Note') " +
                                    (form["FromDate"].ToString() != null && form["FromDate"].ToString() != "" ? " AND H.DocDate >= @FromDate " : "") +
                                    (form["ToDate"].ToString() != null && form["ToDate"].ToString() != "" ? " AND H.DocDate <= @ToDate " : "") +
                                    (form["SiteName"].ToString() != null && form["SiteName"].ToString() != "" ? " AND H.SiteId IN (SELECT Items FROM [dbo].[Split] (@Site, ','))" : "") +
                                    (form["DivisionName"].ToString() != null && form["DivisionName"].ToString() != "" ? " AND H.DivisionId IN (SELECT Items FROM [dbo].[Split] (@Division, ','))" : "") +
                                    (form["JobWorker"].ToString() != null && form["JobWorker"].ToString() != "" ? "  AND  P.PersonID IN (SELECT Items FROM [dbo].[Split] (@JobWorker, ','))" : "") +
                                    (form["Process"].ToString() != null && form["Process"].ToString() != "" ? " AND H.ProcessId IN (SELECT Items FROM [dbo].[Split] (@Process, ','))" : "") +
                                     @"
                            ),
                         cteJobInvoiceWithReturn AS
                            (
                           SELECT Name, DocDate, ProductName, ProductGroupName, Qty, Unit , DealQty, 
                                  DealUnit,Amount, TaxableAmount, IGST,CGST,SGST, InvoiceAmount from cteJobInvoice
                             Union All 
                           SELECT Name, DocDate, ProductName, ProductGroupName, -isnull(Qty,0) as Qty, Unit , -isnull(DealQty,0) as DealQty,
                                   DealUnit,-isnull(Amount,0) as Amount, -isnull(TaxableAmount,0) as TaxableAmount, -isnull(IGST,0) as IGST,-isnull(CGST,0) as CGST,-isnull(SGST,0) as SGST,-isnull(InvoiceAmount,0) as InvoiceAmount  from cteJobInvoiceReturn where 1=1 and Nature='Debit Note'
                            )";

                string CteName = string.Empty;
                string ReportType = form["ReportType"].ToString();
                string Format = form["Format"].ToString();
                if (ReportType == "Job Invoice")
                {
                    CteName = "cteJobInvoice";
                }
                else if (ReportType == "Job Invoice Return/Debit Credit")
                {
                    CteName = "cteJobInvoiceReturn";
                }
                else
                {
                    CteName = "cteJobInvoiceWithReturn";
                }
                if (Format == "Job Worker Wise Summary")
                {
                    mQry += @"    SELECT H.Name as " + "JobWorker" + @",sum(isnull(H.Qty,0)) as Qty,max(H.Unit) as Unit,sum(isnull(H.DealQty,0)) as DealQty, 
                                  max(H.DealUnit) as DealUnit,sum(isnull(H.Amount,0)) as Amount,sum(isnull(H.TaxableAmount,0)) as TaxableAmount,Sum(isnull(H.IGST,0)) as IGST,Sum(isnull(H.CGST,0)) as CGST,sum(isnull(H.SGST,0)) as SGST,
                                  Sum(isnull(H.InvoiceAmount,0)) as  InvoiceAmount
                                  from " + CteName + @" H
                                  Group By H.Name
                                  order By H.Name
                                  ";
                }
                else if (Format == "Month Wise Summary")
                {
                    mQry += @"    SELECT STUFF(CONVERT(CHAR(11),max(H.DocDate), 100), 5,5, '-') as " + "Month" + @",sum(isnull(H.Qty,0)) as Qty,max(H.Unit) as Unit,sum(isnull(H.DealQty,0)) as DealQty,  
                                 max(H.DealUnit) as DealUnit,sum(isnull(H.Amount,0)) as Amount,sum(isnull(H.TaxableAmount,0)) as TaxableAmount,Sum(isnull(H.IGST,0)) as IGST,Sum(isnull(H.CGST,0)) as CGST,sum(isnull(SGST,0)) as SGST,
                                  Sum(isnull(H.InvoiceAmount,0)) as  InvoiceAmount
                                  from " + CteName + @" H
                                  Group By Substring(convert(NVARCHAR,H.DocDate,11),0,6)
                                  order By Substring(convert(NVARCHAR,H.DocDate,11),0,6)
                                  ";
                }
                else if (Format == "Product Wise Summary")
                {
                    mQry += @"    SELECT H.ProductName as " + "Product" + @",sum(isnull(H.Qty,0)) as Qty,max(H.Unit) as Unit,sum(isnull(H.DealQty,0)) as DealQty, 
                                  max(H.DealUnit) as DealUnit,sum(isnull(H.Amount,0)) as Amount,sum(isnull(H.TaxableAmount,0)) as TaxableAmount,Sum(isnull(H.IGST,0)) as IGST,Sum(isnull(H.CGST,0)) as CGST,sum(isnull(SGST,0)) as SGST,
                                  Sum(isnull(H.InvoiceAmount,0)) as  InvoiceAmount
                                  from " + CteName + @" H
                                  Group By H.ProductName
                                  ";
                }
                else if (Format == "Product Group Wise Summary")
                {
                    mQry += @"    SELECT H.ProductGroupName as "+"ProductGroup"+@",sum(isnull(H.Qty,0)) as Qty,max(H.Unit) as Unit,sum(isnull(H.DealQty,0)) as DealQty, 
                                  max(H.DealUnit) as DealUnit,sum(isnull(H.Amount,0)) as Amount,sum(isnull(H.TaxableAmount,0)) as TaxableAmount,Sum(isnull(H.IGST,0)) as IGST,Sum(isnull(H.CGST,0)) as CGST,sum(isnull(SGST,0)) as SGST,
                                  Sum(isnull(H.InvoiceAmount,0)) as  InvoiceAmount
                                  from " + CteName + @" H
                                  Group By H.ProductGroupName
                                  ";
                }
            }

            ReportHeader.SqlProc = mQry;
            AddReportParameters(ref ReportLineList, "Format", "Format", "Single Select", "1.Format",null, null, "@Format", ReportFormatConstants.JobWorkerWise + "," + ReportFormatConstants.MonthWise+","+ ReportFormatConstants.ProductWise+","+ ReportFormatConstants.ProductGroupWise, ReportFormatConstants.JobWorkerWise, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Report Type", "ReportType", "Single Select", "1.Format",null,null, "@ReportType", ReportTypeConstants.JobInvoice+","+ ReportTypeConstants.JobInvoiceReturn, ReportTypeConstants.JobInvoice, true, null, false, false);
            AddReportParameters(ref ReportLineList, "From Date", "FromDate", "Date", "2.Filter", null, null, "@FromDate", null, null, true, GetMonthStartDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "To Date", "ToDate", "Date", "2.Filter", null, null, "@ToDate", null, null, true, GetCurrentDateQry(), false, false);
            AddReportParameters(ref ReportLineList, "Site Name", "SiteName", "Multi Select", "2.Filter", "GetSite", "SetSite", "@Site", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Division Name", "DivisionName", "Multi Select", "2.Filter", "GetDivision", "SetDivision", "@Division", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Job Worker", "JobWorker", "Multi Select", "2.Filter", "GetJobWorkers", "SetJobWorkers", "@JobWorker", null, null, true, null, false, false);
            AddReportParameters(ref ReportLineList, "Process", "Process", "Multi Select", "2.Filter", "GetProcess", "SetProcess", "@Process", null, null, true, null, false, false);
            

        }
        #endregion

        public void AddReportParameters(ref List<ReportLine> ReportLineList, string DisplayName, string FieldName, string DataType, string Type, string ServiceFuncGet,
            string ServiceFuncSet, string SqlParameter, string ListItem, string DefaultValue, bool IsVisible, string SqlProcGetSet,
            bool IsCollapse, bool IsMandatory)
        {
            ReportLine ReportLine = new ReportLine();
            ReportLine.DisplayName = DisplayName;
            ReportLine.FieldName = FieldName;
            ReportLine.DataType = DataType;
            ReportLine.Type = Type;
            ReportLine.ServiceFuncGet = ServiceFuncGet;
            ReportLine.ServiceFuncSet = ServiceFuncSet;
            ReportLine.CacheKey = null;
            ReportLine.SqlParameter = SqlParameter;
            ReportLine.ListItem = ListItem;
            ReportLine.DefaultValue = DefaultValue;
            ReportLine.IsVisible = IsVisible;
            ReportLine.SqlProcGetSet = SqlProcGetSet;
            ReportLine.IsCollapse = IsCollapse;
            ReportLine.IsMandatory = IsMandatory;


            if (ReportLineList != null)
            {
                if (ReportLineList.Count == 0)
                    ReportLine.Serial = 1;
                else
                    ReportLine.Serial = ReportLineList.Count + 1;

            }
            else
            {
                ReportLine.Serial = 1;
            }


            ReportLineList.Add(ReportLine);
        }



        public string GetCurrentDateQry()
        {
            mQry = "SELECT replace(FORMAT(Getdate(), 'dd MMMM yyyy'),' ','/') AS DATE";
            return mQry;
        }

        public string GetMonthStartDateQry()
        {
            mQry = "SELECT replace(FORMAT(Getdate(), '01 MMMM yyyy'),' ','/') AS DATE";
            return mQry;
        }

        public string GetFinancialYearStartDateQry()
        {
            mQry = @"  declare @date varchar(20)
                  declare @dat int
                  set @dat=DATEPART(mm, GETDATE())
                  if(@dat < 4 )
                  set @date='01/April/'+ convert(varchar,DATEPART(YEAR, GETDATE())-1) 
                  else 
                  set @date='01/April/'+ convert(varchar,DATEPART(YEAR, GETDATE()))
                  select @date as Date
                  end ";
             return mQry;
        }

        public class ReportGridFormat
        {
            public const string JobWorkerWise = "Job Worker Wise Summary";
            public const string MonthWise = "Month Wise Summary";
            public const string ProductWise = "Product Wise Summary";
            public const string ProductGroupWise = "Product Group Wise Summary";
        }

        public void GetDivisionSettings()
        {
            string connectionString = (string)System.Web.HttpContext.Current.Session["DefaultConnectionString"];
            DataTable DivisionSettings = new DataTable();
            int DivisionId = (int)System.Web.HttpContext.Current.Session[SessionNameConstants.LoginDivisionId];
            mQry = "SELECT * From Web.DivisionSettings Where DivisionId = " + DivisionId.ToString();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(mQry, sqlConnection);
                SqlDataAdapter sqlDataAapter = new SqlDataAdapter(cmd);
                sqlDataAapter.Fill(DivisionSettings);
            }
            if (DivisionSettings.Rows.Count > 0)
            {
                if (DivisionSettings.Rows[0]["Dimension1Caption"].ToString() != "")
                    Dimension1Caption = DivisionSettings.Rows[0]["Dimension1Caption"].ToString().Replace(" ","");
                if (DivisionSettings.Rows[0]["Dimension2Caption"].ToString() != "")
                    Dimension2Caption = DivisionSettings.Rows[0]["Dimension2Caption"].ToString().Replace(" ", "");
                if (DivisionSettings.Rows[0]["Dimension3Caption"].ToString() != "")
                    Dimension3Caption = DivisionSettings.Rows[0]["Dimension3Caption"].ToString().Replace(" ", "");
                if (DivisionSettings.Rows[0]["Dimension4Caption"].ToString() != "")
                    Dimension4Caption = DivisionSettings.Rows[0]["Dimension4Caption"].ToString().Replace(" ", "");
                if (DivisionSettings.Rows[0]["ProductUidCaption"].ToString() != "")
                    ProductUidCaption = DivisionSettings.Rows[0]["ProductUidCaption"].ToString().Replace(" ", "");
            }
        }

        public ReportHeaderCompanyDetail GetReportHeaderCompanyDetail()
        {
            ReportHeaderCompanyDetail ReportHeaderCompanyDetail = new ReportHeaderCompanyDetail();

            var DivisionId = (int)System.Web.HttpContext.Current.Session["DivisionId"];
            var CompanyId = (int)System.Web.HttpContext.Current.Session["CompanyId"];

            Company Company = db.Company.Find(CompanyId);
            Division Division = db.Divisions.Find(DivisionId);


            ReportHeaderCompanyDetail.CompanyName = Company.CompanyName.Replace(System.Environment.NewLine, " ");
            ReportHeaderCompanyDetail.Address = Company.Address.Replace(System.Environment.NewLine, " ");

            //ReportHeaderCompanyDetail.CompanyName = Company.CompanyName;
            //ReportHeaderCompanyDetail.Address = Company.Address;
            if (Company.CityId != null)
                ReportHeaderCompanyDetail.CityName = db.City.Find(Company.CityId).CityName;
            else
                ReportHeaderCompanyDetail.CityName = "";
            ReportHeaderCompanyDetail.Phone = Company.Phone;
            ReportHeaderCompanyDetail.LogoBlob = Division.LogoBlob;

            return ReportHeaderCompanyDetail;
        }

        public void Dispose()
        {
        }



    }
}