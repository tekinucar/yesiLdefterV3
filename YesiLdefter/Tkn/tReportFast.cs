using DevExpress.XtraEditors;
using FastReport;
using FastReport.Data;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_Report
{
    class tReportFast
    {
        tToolBox t = new tToolBox();
        
        Form tFormReports = null;
        private DataSet dataSet;


        public void ReportDocumentViewer(Form tForm, DataSet dsMsReports, DataNavigator dNMsReports, List<string> dataSetList, string sourceFormCodeAndName, ref FastReport.Preview.PreviewControl documentViewer1)

        {
            t.WaitFormOpen(v.mainForm, "Rapor hazırlanıyor ...");
            v.SP_OpenApplication = true;

            // Raporların olduğu form
            tFormReports = tForm;

            // Raporlar Formunu çağıran formun tespiti
            string formName = t.getFormName(sourceFormCodeAndName);
            Form tFormSource = Application.OpenForms[formName];

            string reportCode = dsMsReports.Tables[0].Rows[dNMsReports.Position]["ReportCode"].ToString();
            string reportName = dsMsReports.Tables[0].Rows[dNMsReports.Position]["ReportName"].ToString();
            string reportFileName = "";

            t.Str_Replace(ref reportCode, ".", "_");

            if (t.IsNotNull(reportName) == false)
                reportName = reportCode;

            reportFileName = v.EXE_FastReportsPath + reportName + ".fr3";

            if (File.Exists(@"" + reportFileName))
            {

            }
            else
            {
                /// Auto FastReport Page Create
                ///
                Report report = new Report();
                ReportPage page1 = new ReportPage();
                ReportTitleBand reportTitleBand1 = new ReportTitleBand();
                //PageHeaderBand pageHeaderBand1 = new PageHeaderBand();
                DataBand dataBand1 = new DataBand();
                //ReportSummaryBand reportSummaryBand1 = new ReportSummaryBand();
                PageFooterBand pageFooterBand1 = new PageFooterBand();

                page1.Name = "Page1";
                reportTitleBand1.Name = "ReportTitleBand1";
                //pageHeaderBand1.Name = "PageHeaderBand1";
                dataBand1.Name = "DataBand1";
                pageFooterBand1.Name = "PageFooterBand1";

                reportTitleBand1.Height = Units.Centimeters * (float)1.0;
                //pageHeaderBand1.Height = Units.Centimeters * (float)0.75;
                dataBand1.Height = Units.Centimeters * (float)2.0;
                pageFooterBand1.Height = Units.Centimeters * (float)0.5;

                page1.Bands.Add(reportTitleBand1);
                //page1.Bands.Add(pageHeaderBand1);
                page1.Bands.Add(dataBand1);
                //page1.Bands.Add(reportSummaryBand1);
                page1.Bands.Add(pageFooterBand1);

                report.Pages.Add(page1);

                report.Save(reportFileName);
            }

            //dataSet = dsMsReports;

                try 
            {
                //  previewControl1
                if (documentViewer1.Report != null)
                {
                    documentViewer1.Clear();
                    documentViewer1.Report.Dispose();
                }
                try
                {
                    Report FReport = new Report();

                    FReport.Preview = documentViewer1; // previewControl1;

                    FReport.Load(reportFileName);

                    Report report = FReport.Report;

                    PageCollection page = report.Pages;

                    PageBase page1 = page[0];

                    ReportSummaryBand reportSummaryBand1 = new ReportSummaryBand();
                    reportSummaryBand1.Name = "UstadYazilim";
                    reportSummaryBand1.Height = Units.Centimeters * (float)2.0;

                    DataBand dataBand2 = new DataBand();
                    dataBand2.Name = "DataBand2";
                    dataBand2.Height = Units.Centimeters * (float)4.0;

                    PageHeaderBand pageHeaderBand1 = new PageHeaderBand();
                    pageHeaderBand1.Name = "PageHeaderBand1";
                    pageHeaderBand1.Height = Units.Centimeters * (float)4.0;

                    FastReport.TextObject textObject = new TextObject();
                    textObject.Name = "text1";
                    textObject.Text = "Üstad Yazılım";
                    textObject.Left = Units.Centimeters * (float)0;
                    textObject.Height = Units.Centimeters * (float)0.5;
                    textObject.HorzAlign = HorzAlign.Center;
                    textObject.Dock = DockStyle.Fill;

                    pageHeaderBand1.AddChild(textObject);

                    report.BeginInit();
                    ((ReportPage)(report.Pages[0])).Bands.Add(pageHeaderBand1);

                    report.EndInit();
                    report.Refresh();
                    report.Save(reportFileName);

                    //((ReportPage)page1).Bands.Add(reportSummaryBand1);

                    //((ReportPage)((PageBase)((PageCollection)report.Pages)[0])).Bands.Add(reportSummaryBand1);


                    //string tableName = dsMsReports.Tables[0].TableName.ToString();
                    //FReport.RegisterData(dsMsReports);
                    //FReport.GetDataSource(tableName).Enabled = true;

                    allRegisterData(tForm, FReport, dataSetList);

                    FReport.Show();

                    //PreviewReport(FReport, tableName);

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
            finally
            {
                //lockSelect = false;
            }

            v.IsWaitOpen = false;
            t.WaitFormClose();
        }

        private void allRegisterData(Form tForm, Report FReport, List<string> dataSetList)
        {
            DataSet dsData = null;
            DataNavigator dN = null;
            object tDataTable = null;
            string tableName = "";

            Control cntrl = new Control();
            string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };
            foreach (string value in dataSetList)
            {
                cntrl = t.Find_Control(tForm, value, "", controls);
                if (cntrl != null)
                {
                    dN = (DevExpress.XtraEditors.DataNavigator)cntrl;
                    if (dN.DataSource != null)
                    {
                        tDataTable = dN.DataSource;
                        tableName = ((DataTable)tDataTable).TableName;
                        dsData = ((DataTable)tDataTable).DataSet;
                        
                        DataTable newDataTable = ((DataTable)tDataTable).Clone();
                        DataSet newDataSet = new DataSet();
                        newDataSet.Tables.Add(newDataTable);

                        FReport.RegisterData(dsData);
                        //FReport.RegisterData(newDataSet);
                        FReport.GetDataSource(tableName).Enabled = true;
                    }
                }
            }
        }

        private void PreviewReport(Report FReport, string tableName)
        {
            //RegisterData(FReport, tableName);
            FReport.Show();
        }

        private void RegisterData(Report FReport, string tableName)
        {
            FReport.RegisterData(dataSet, tableName);

            foreach (DataSourceBase source in FReport.Dictionary.DataSources)
            {
                source.Enabled = true;
            }
        }

    }
}
