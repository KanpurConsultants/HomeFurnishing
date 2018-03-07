using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Core.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Net;

namespace Service
{
    public class ReportGenerateService_New
    {
        string ReportName = "";
        string ReportTitle = "";
        string SubReportTitle = "";

        string SiteName = "";
        string DivisionName = "";
        string FileType = "";
        string CompanyName = "";
        string CompanyAddress = "";
        string CompanyCity = "";
        int SubReportDataIndex = 0;
        string PrintedBy = "";

        DataTable SubReportData;
        List<DataTable> ListSubReportData;
        List<string> ListSubReportName;

        public byte[] ReportGenerateNew(List<ListofDataTable> DataTableList, out string mimeType, string ReportFormatType = "PDF", List<string> ParaList = null, string BaseDirectoryPath = null, string UserName = null)
        {
            int c = 0;
            DataTable Dt = new DataTable();
            Dt = DataTableList[0].DataTable;
            List <ReportDataSource> reportdatasourceList = new List<ReportDataSource>();

            foreach (ListofDataTable DataTable in DataTableList)
            {
                c = c + 1;
                ReportDataSource reportdatasource = new ReportDataSource("Ds" + DataTable.DataTableName.Replace("Query",""), DataTable.DataTable);
                reportdatasourceList.Add(reportdatasource);
            }

            ReportViewer reportViewer = new ReportViewer();
            mimeType = "";
            byte[] Bytes;
            //string SubReportName;

            List<string> SubReportNameList = new List<string>();
            PrintedBy = UserName;
            
            FileType = ReportFormatType;


            if (Dt.Columns.Contains("ReportName"))
            {
                ReportName = Dt.Rows[0]["ReportName"].ToString();
            }
            else
            {
                ReportName = "";
                Bytes = new byte[1];
                Bytes[0] = 0;
                return Bytes;

            }

            if (Dt.Columns.Contains("ReportTitle"))
            {
                ReportTitle = Dt.Rows[0]["ReportTitle"].ToString();
            }
            else
            {
                ReportTitle = "";
                Bytes = new byte[2];
                Bytes[0] = 0;
                return Bytes;

            }



            if ((Dt.Columns.Contains("SiteName")) && (Dt.Columns.Contains("DivisionName")) && (Dt.Columns.Contains("CompanyName")) && (Dt.Columns.Contains("CompanyAddress")) && (Dt.Columns.Contains("CompanyCity")))
            {
                SiteName = Dt.Rows[0]["SiteName"].ToString();
                DivisionName = Dt.Rows[0]["DivisionName"].ToString();
                CompanyName = Dt.Rows[0]["CompanyName"].ToString();
                CompanyAddress = Dt.Rows[0]["CompanyAddress"].ToString();
                CompanyCity = Dt.Rows[0]["CompanyCity"].ToString();


            }
            if (Dt.Columns.Contains("SiteId"))
            {
                String queryCompanyDetail = "";
                SqlConnection Con = new SqlConnection((string)System.Web.HttpContext.Current.Session["DefaultConnectionString"]);

                if (Dt.Columns.Contains("DivisionId"))                {

                    if (Dt.Rows[0]["DivisionId"].ToString() == "" && Dt.Rows[0]["SiteId"].ToString() == "")
                        queryCompanyDetail = "Web.ProcCompanyDetail '" + (int)System.Web.HttpContext.Current.Session[SessionNameConstants.LoginSiteId] + "'";
                    else if (Dt.Rows[0]["DivisionId"].ToString() != "" && Dt.Rows[0]["SiteId"].ToString() == "")
                        if (Dt.Columns.Contains("DocDate"))
                        {
                            queryCompanyDetail = "Web.ProcCompanyDetail NULL , '" + Dt.Rows[0]["DivisionId"].ToString() + "', '" + Dt.Rows[0]["DocDate"].ToString() + "'";
                        }
                        else
                        {
                            queryCompanyDetail = "Web.ProcCompanyDetail NULL , '" + Dt.Rows[0]["DivisionId"].ToString() + "'";
                        }
                    else
                        if (Dt.Columns.Contains("DocDate"))
                    {
                        queryCompanyDetail = "Web.ProcCompanyDetail '" + Dt.Rows[0]["SiteId"].ToString() + "', '" + Dt.Rows[0]["DivisionId"].ToString() + "', '" + Dt.Rows[0]["DocDate"].ToString() + "'";
                    }
                    else
                    {
                        queryCompanyDetail = "Web.ProcCompanyDetail '" + Dt.Rows[0]["SiteId"].ToString() + "', '" + Dt.Rows[0]["DivisionId"].ToString() + "'";
                    }

                }
                else
                {
                    queryCompanyDetail = "Web.ProcCompanyDetail '" + Dt.Rows[0]["SiteId"].ToString() + "'";
                }

                DataTable CompanyDetailData = new DataTable();


                SqlDataAdapter CompanyDetailAapter = new SqlDataAdapter(queryCompanyDetail.ToString(), Con);
                CompanyDetailAapter.Fill(CompanyDetailData);

                ListSubReportData = new List<DataTable>();
                ListSubReportData.Add(CompanyDetailData);
                ReportDataSource reportdatasource = new ReportDataSource("DsMain", CompanyDetailData);
                reportdatasourceList.Add(reportdatasource);
                SubReportNameList.Add("CompanyDetail");


                SiteName = CompanyDetailData.Rows[0]["SiteName"].ToString();
                DivisionName = CompanyDetailData.Rows[0]["DivisionName"].ToString();
                CompanyName = CompanyDetailData.Rows[0]["CompanyName"].ToString();
                CompanyAddress = CompanyDetailData.Rows[0]["CompanyAddress"].ToString();
                CompanyCity = CompanyDetailData.Rows[0]["CompanyCity"].ToString();

                CompanyDetailData = null;
            }
            else
            {
                ReportTitle = "";
                Bytes = new byte[2];
                Bytes[0] = 0;
                return Bytes;

            }

            ListSubReportName = SubReportNameList;
            //ListSubReportData = DataTableList;


            string path = "";

            if (ReportName.Contains("."))
                path = ConfigurationManager.AppSettings["PhysicalRDLCPath"] + ConfigurationManager.AppSettings["ReportsPathFromService"] + ReportName;
            else
                path = ConfigurationManager.AppSettings["PhysicalRDLCPath"] + ConfigurationManager.AppSettings["ReportsPathFromService"] + ReportName + ".rdlc";


            if (BaseDirectoryPath != null)
            {
                if (ReportName.Contains("."))
                    path = BaseDirectoryPath + ConfigurationManager.AppSettings["ReportsPathFromService"] + ReportName;
                else
                    path = BaseDirectoryPath + ConfigurationManager.AppSettings["ReportsPathFromService"] + ReportName + ".rdlc";
            }



            reportViewer.LocalReport.ReportPath = path;

            foreach (ReportDataSource reportdatasour in reportdatasourceList)
            {
                reportViewer.LocalReport.DataSources.Add(reportdatasour);
            }

            SetReportAttibutes_New(reportViewer);





            reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(MySubreportEventHandler);




            string FilterStr = "FilterStr";
            int i = 1;

            if (ParaList != null)
            {
                if (ParaList.Count > 0)
                {
                    foreach (var item in ParaList)
                    {
                        reportViewer.LocalReport.SetParameters(new ReportParameter(FilterStr + i.ToString(), item));
                        i++;
                    }
                }
            }


            string encoding;
            string fileNameExtension;

            string deviceinfo =
                "<DeviceInfo>" +
                "   <OutputFormat>" + ReportFormatType + "</OutputFormat>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;


            Bytes = reportViewer.LocalReport.Render(
                    ReportFormatType,
                    deviceinfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);



            return Bytes;
        }


        void MySubreportEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            int indexOfSlash = 0;
            string reportPath = e.ReportPath;
            indexOfSlash = reportPath.LastIndexOf("\\");

            if (indexOfSlash > 0)
            {
                reportPath = reportPath.Substring(indexOfSlash + 1, reportPath.Length - (indexOfSlash + 1));
            }

            reportPath = reportPath.Replace(".rdlc", "");
            reportPath = reportPath.Replace(".rdl", "");


            if (ListSubReportData != null && ListSubReportName != null)
            {
                if (ListSubReportData.Count > 0 && ListSubReportName.Count > 0)
                {
                    for (int i = 0; i < ListSubReportName.Count; i++)
                    {
                        string Str = ListSubReportName[i];

                        Str = Str.Replace(".rdlc", "");
                        Str = Str.Replace(".rdl", "");

                        if (Str.ToUpper() == "COMPANYDETAIL" || Str.ToUpper() == "GATEPASSPRINT")
                        {
                            if (reportPath.Contains(Str))
                            {
                                e.DataSources.Add(new ReportDataSource("DsMain", (DataTable)ListSubReportData[i]));
                            }
                        }
                        else
                        {
                            if (reportPath.ToUpper() == Str.ToUpper())
                            {
                                e.DataSources.Add(new ReportDataSource("DsMain", (DataTable)ListSubReportData[i]));
                            }
                        }

                    }
                }

            }

        }


        public void SetReportAttibutes_New(ReportViewer reportViewer)
        {
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Percentage(100);
           

            string CompanyLogoPath = System.AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ImagesPathFromService"] + "Company_Logo.BMP";
            reportViewer.LocalReport.EnableExternalImages = true;
            string imagePath = new Uri(CompanyLogoPath).AbsoluteUri;
            string ImagePath = ConfigurationManager.AppSettings["ImagesPathFromService"];


            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "ImagePath").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("ImagePath", ImagePath));
            }


            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "CompanyLogoPath").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("CompanyLogoPath", imagePath));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "PrintedBy").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("PrintedBy", PrintedBy));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "ReportTitle").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("ReportTitle", ReportTitle));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "ReportSubtitle").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("ReportSubtitle", SubReportTitle));
            }


            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "CompanyName").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("CompanyName", CompanyName));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "CompanyAddress").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("CompanyAddress", CompanyAddress));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "CompanyCity").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("CompanyCity", CompanyCity));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "SiteName").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("SiteName", SiteName));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "DivisionName").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("DivisionName", DivisionName));
            }



            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "FileType").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("FileType", FileType));
            }




            reportViewer.HyperlinkTarget = "_blank";
            reportViewer.LocalReport.EnableHyperlinks = true;
        }

    }

    public class ListofDataTable
    {
        public DataTable DataTable;
        public string DataTableName;
    }
}
