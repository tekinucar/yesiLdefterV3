using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_Report
{
    class tReport
    {
        tToolBox t = new tToolBox();


        Form tFormReports = null;

        //private DataSet dsData = null;

        // Declare a base report variable.
        private ReportDesignTool designTool = null;
        private ReportPrintTool printTool = null;
        //private DocumentViewer documentViewer = null;

        // XtraReport report = new XtraReport();
        //private XtraReport db_Report;
        private XtraReport xtraReport1;


        public void ReportDocumentViewer(Form tForm, DataSet dsMsReports, DataNavigator dNMsReports, string sourceFormCodeAndName, ref DocumentViewer documentViewer1)
        {
            t.WaitFormOpen(v.mainForm, "Rapor hazırlanıyor ...");
            v.SP_OpenApplication = true;

            // Raporların olduğu form
            tFormReports = tForm;

            // Raporlar Formunu çağıran formun tespiti
            string formName = getFormName(sourceFormCodeAndName);
            Form tFormSource = Application.OpenForms[formName];

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

            v.IsWaitOpen = false;
            t.WaitFormClose();
        }

        public void tShowReportDesigner(Form tForm, DataSet dsMsReports, DataNavigator dNMsReports, string sourceFormCodeAndName)
        {
            //t.WaitFormOpen(v.mainForm, "Rapor dizayn hazırlanıyor ...");
            //v.SP_OpenApplication = true;
            t.AlertMessage("Rapor Dizaynı yükleniyor...", "Lütfen bekleyin.");

            // Raporların olduğu form
            tFormReports = tForm;

            // Raporlar Formunu çağıran formun tespiti
            string formName = getFormName(sourceFormCodeAndName);
            Form tFormSource = Application.OpenForms[formName];

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

            // Report Design Formu oluşturuloyor
            //
            ReportDesignTool designTool = new ReportDesignTool(xtraReport1);
            designTool.ShowRibbonDesignerDialog();

            // Design Formunda iş bittikten sonra işlem yapılan raporun database kaydedilmesi
            //
            string soru = "Rapor kayıt edilecek, onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                dataBaseSaveReport(tFormReports, dsMsReports, dNMsReports, xtraReport1);
            }

            //v.IsWaitOpen = false;
            //t.WaitFormClose();
        }

        private string getFormName(string sourceFormCodeAndName)
        {
            string formCode = "";
            string formName = "";

            if (sourceFormCodeAndName.IndexOf("||") > -1)
            {
                formCode = t.Get_And_Clear(ref sourceFormCodeAndName, "||");
                formName = t.Get_And_Clear(ref sourceFormCodeAndName, "||");
            }
            return formName;
        }

        private void dbLoadReport(DataSet dsReports, DataNavigator dNReports, ref XtraReport xtraReport1)
        {
            // Rapor tasarımın text dosyasından alınması
            // xtraReport1.LoadLayoutFromXml("XtraReport.repx");
            //
            // Rapor tasarımın databaseden okunması
            //
            string temp = dsReports.Tables[0].Rows[dNReports.Position]["ReportTemp"].ToString();

            using (StreamWriter sw = new StreamWriter(new MemoryStream()))
            {
                sw.Write(temp);
                sw.Flush();
                xtraReport1 = XtraReport.FromStream(sw.BaseStream, true);
            }

            if (xtraReport1 == null)
                xtraReport1 = new XtraReport();
        }

        private void preparingReportData(Form tFormSource, ref DataSet reportDS, string sourceFormCodeAndName)
        {
            // raporun içinde kullanılacak olan data hazırlanıyor
            //
            reportDS.Namespace = "Tablolar";

            Control cntrl = null;
            string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };

            List<string> list = new List<string>();
            t.Find_DataNavigator_List(tFormSource, ref list);

            string tableIPCode = "";
            foreach (string value in list)
            {
                cntrl = t.Find_Control(tFormSource, value, "", controls);
                if (cntrl != null)
                {
                    if (((DevExpress.XtraEditors.DataNavigator)cntrl).AccessibleName != null)
                    {
                        tableIPCode = ((DevExpress.XtraEditors.DataNavigator)cntrl).AccessibleName;
                        tableIPCode = tableIPCode.Replace(".", "_");

                        object tDataTable = ((DevExpress.XtraEditors.DataNavigator)cntrl).DataSource;

                        DataSet dsData = ((DataTable)tDataTable).DataSet;
                        DataTable dataTable = dsData.Tables[0].Copy();

                        dataTable.TableName = tableIPCode;

                        if (dsData != null)
                        {
                            reportDS.Tables.Add(dataTable);
                        }
                    }
                } // if cntrl != null
            }
        }

        private void dataBaseSaveReport(Form tFormReports, DataSet dsMsReports, DataNavigator dNMsReports, XtraReport xtraReport1)
        {
            tSave sv = new tSave();

            // Save the report to a stream.
            MemoryStream stream = new MemoryStream();
            xtraReport1.SaveLayout(stream);
                       
            // rapor tasarımın text dosyasına yazılması
            // xtraReport1.SaveLayoutToXml("XtraReport.repx");
            
            // Prepare the stream for reading.
            stream.Position = 0;

            // Insert the report to a database.
            using (StreamReader sr = new StreamReader(stream))
            {
                try
                {
                    // Read the report from the stream to a string variable.
                    string temp = sr.ReadToEnd();

                    dsMsReports.Tables[0].Rows[dNMsReports.Position]["ReportTemp"] = temp;

                    // DB Records
                    sv.tDataSave(tFormReports, dsMsReports, dNMsReports, dNMsReports.Position);
                }
                finally
                {
                    t.FlyoutMessage("Raporun tasarım kaydı", "İşlem tamamlandı.");
                }

                // Add a row to a table.
                // DataTable dt = dataSet1.Tables["Records"];
                // DataRow row = dt.NewRow();
                // row["Report"] = s;
                // dt.Rows.Add(row);
                // adapter.Update(dataSet1.Tables[0]);
                // dataSet1.AcceptChanges();
            }
        }


        //--------------------

        public void reportDesigner_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            xtraReport1 = new XtraReport();

            //createReportDesigner(xtraReport1);

            createReportPreview(xtraReport1);

            //createReportDocumentViewer(xtraReport1);
        }

        private void createReportDesigner(XtraReport xtraReport1)
        {
            t.WaitFormOpen(v.mainForm, "Rapor dizayn hazırlanıyor ...");
            v.SP_OpenApplication = true;
                        
            designTool = new ReportDesignTool(xtraReport1);
            designTool.ShowRibbonDesignerDialog();

            v.IsWaitOpen = false;
            t.WaitFormClose();
        }
        private void createReportPreview(XtraReport xtraReport1)
        {
            // bağımsız form oluşturuyor  ReportDesign gibi
            // 
            t.WaitFormOpen(v.mainForm, "Rapor hazırlanıyor ...");
            v.SP_OpenApplication = true;

            printTool = new ReportPrintTool(xtraReport1);
            printTool.ShowRibbonPreview();

            v.IsWaitOpen = false;
            t.WaitFormClose();
        }

        private void createReportDocumentViewer(DocumentViewer documentViewer, XtraReport xtraReport1)
        {
            t.WaitFormOpen(v.mainForm, "Rapor hazırlanıyor ...");
            v.SP_OpenApplication = true;

            documentViewer = new DevExpress.XtraPrinting.Preview.DocumentViewer();
            documentViewer.DocumentSource = xtraReport1;

            v.IsWaitOpen = false;
            t.WaitFormClose();
        }

    }
}
