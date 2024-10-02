using DevExpress.XtraEditors;
using FastReport;
using FastReport.Data;
using System;
using System.Collections.Generic;
using System.Data;
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


        public void ReportDocumentViewer(Form tForm, DataSet dsMsReports, DataNavigator dNMsReports, string sourceFormCodeAndName, ref FastReport.Preview.PreviewControl documentViewer1)

        {
            t.WaitFormOpen(v.mainForm, "Rapor hazırlanıyor ...");
            v.SP_OpenApplication = true;

            // Raporların olduğu form
            tFormReports = tForm;

            // Raporlar Formunu çağıran formun tespiti
            string formName = t.getFormName(sourceFormCodeAndName);
            Form tFormSource = Application.OpenForms[formName];

            dataSet = dsMsReports;

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

                    string reportName = v.EXE_FastReportsPath + "FastReport1.frx";
                    FReport.Load(reportName);

                    string tableName = dsMsReports.Tables[0].TableName.ToString();
                    FReport.RegisterData(dsMsReports);
                    FReport.GetDataSource(tableName).Enabled = true;
                    
                    PreviewReport(FReport);

                    /*
                    TreeNode selectedNode = tvReports.SelectedNode;
                    if (selectedNode.Tag == null)
                    {
                        lockSelect = false;
                        tvReports.SelectedNode = e.Node.Nodes[0];
                    }
                    else
                    {
                        lastNode = selectedNode;
                        FReport.Load((string)selectedNode.Tag);
                        PreviewReport(FReport);
                    }
                    */
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


            /*
            // xtraReport1 MsReport tablosundan okunacak
            //
            XtraReport xtraReport1 = null;
            dbLoadReport(dsMsReports, dNMsReports, ref xtraReport1);

            // xtraReport1 in DataSet i hazırlanacak
            // raporun içinde kullanılacak datasetler 
            // 
            DataSet dsViewData = new DataSet();
            preparingReportData(tFormSource, ref dsViewData, sourceFormCodeAndName);
            xtraReport1.DataSource = dsViewData;

            // hazırlanan Raporun view edilmesi
            // 
            documentViewer1.DocumentSource = xtraReport1;
            xtraReport1.CreateDocument();

            //xtraReport1.ShowRibbonPreviewDialog();
            */


            v.IsWaitOpen = false;
            t.WaitFormClose();
        }

        private void PreviewReport(Report FReport)
        {
            RegisterData(FReport);
            FReport.Show();
        }

        private void RegisterData(Report FReport)
        {
            //FReport.RegisterData(dataSet, "NorthWind");
            //FReport.RegisterData(businessObject, "Categories BusinessObject");

            FReport.RegisterData(dataSet, "test");

            foreach (DataSourceBase source in FReport.Dictionary.DataSources)
            {
                source.Enabled = true;
            }
        }

    }
}
