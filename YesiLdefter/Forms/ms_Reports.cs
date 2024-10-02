using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
using FastReport;
using FastReport.Preview;
using Tkn_Events;
using Tkn_InputPanel;
using Tkn_Report;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_Reports : Form
    {
        tToolBox t = new tToolBox();
        tReportDevEx raporDevEx = new tReportDevEx();
        tReportFast raporFast = new tReportFast();

        private DataSet dsMsReports = null;
        private DataNavigator dNMsReports = null;
        private DevExpress.XtraPrinting.Preview.DocumentViewer documentViewerDevEx = null;
        private FastReport.Preview.PreviewControl documentViewerFast = null;

        string sourceFormCodeAndName = "";

        Control cntrlReportNames = null;
        string controlNames = "controlReportNameList";

        string menuName = "MENU_" + "UST/PMS/PMS/REPORT";
        string buttonRaporYazdir = "RAPOR_YAZDIR";
        string buttonRaporOnizleme = "RAPOR_ONIZLEME";

        Int16 desingerType = 0;
        bool lockSelect = false;

        public ms_Reports()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_Reports_Shown);
            
            this.KeyPreview = true;
        }

        private void ms_Reports_Shown(object sender, EventArgs e)
        {
            string ipCodes = this.AccessibleDefaultActionDescription;
            if (this.AccessibleDescription != null)
                sourceFormCodeAndName = this.AccessibleDescription;

            cntrlReportNames = t.Find_Control(this, controlNames);

            string tableIPCode = v.con_Source_ReportTableIPCode;

            tInputPanel ip = new tInputPanel();
            ip.Create_InputPanel(this, cntrlReportNames, tableIPCode, 1, true);


            t.Find_DataSet(this, ref dsMsReports, ref dNMsReports, tableIPCode);
            dNMsReports.PositionChanged += new System.EventHandler(dNMsReports_PositionChanged);

            documentViewerDevEx = (DevExpress.XtraPrinting.Preview.DocumentViewer)t.Find_Control(this, "documentViewerDevEx");
            documentViewerFast = (FastReport.Preview.PreviewControl)t.Find_Control(this, "documentViewerFast");
            
            documentViewerFast.Buttons =
             ((FastReport.PreviewButtons)((((((((((((((((FastReport.PreviewButtons.Print 
            | FastReport.PreviewButtons.Open)
            | FastReport.PreviewButtons.Save)
            | FastReport.PreviewButtons.Email)
            | FastReport.PreviewButtons.Find)
            | FastReport.PreviewButtons.Zoom)
            | FastReport.PreviewButtons.Outline)
            | FastReport.PreviewButtons.PageSetup)
            | FastReport.PreviewButtons.Edit)
            | FastReport.PreviewButtons.Watermark)
            | FastReport.PreviewButtons.Navigator)
            | FastReport.PreviewButtons.Close)
            | FastReport.PreviewButtons.Design)
            | FastReport.PreviewButtons.CopyPage)
            | FastReport.PreviewButtons.DeletePage)
            | FastReport.PreviewButtons.About)));

            /*
            // DİKKAT : Bu atamanın yerini değiştirme
            //
            // ms_Reports u çağıaran formun FormCode si
            // hangi formun içinden çağrıldığına dair işaret
            this.AccessibleDescription = v.con_Source_FormCodeAndName;
            // this.AccessibleName ise ms_Reports un kendisi
            //
            
            // Raporları listeleyen TableIPCode nin tespiti
            string ipCodes = this.AccessibleDefaultActionDescription;
            string tableIPCode = t.Get_And_Clear(ref ipCodes, "||");

            t.Find_DataSet(this, ref dsMsReports, ref dNMsReports, tableIPCode);

            dNMsReports.PositionChanged += new System.EventHandler(dNMsReports_PositionChanged);


            documentViewer = (DocumentViewer)t.Find_Control(this, "documentViewer");

            if (this.AccessibleDescription != null)
                sourceFormCodeAndName = this.AccessibleDescription;
            */

            t.Find_Button_AddClick(this, menuName, buttonRaporYazdir, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonRaporOnizleme, myNavElementClick);
        }

        private void dNMsReports_PositionChanged(object sender, EventArgs e)
        {
            // LKP_ONAY için ise işlem olmasın
            if ((v.con_OnayChange) || (v.con_PositionChange)) return;

            if (v.con_Cancel == true)
            {
                v.con_Cancel = false;
                return;
            }

            if (t.IsNotNull(dsMsReports))
            { 
                desingerType = Convert.ToInt16(dsMsReports.Tables[0].Rows[dNMsReports.Position]["DesignerTypeId"].ToString());

                if (desingerType == (Int16)v.ReportDesignerTool.DevExpress)
                    raporDevEx.ReportDocumentViewer(this, dsMsReports, dNMsReports, sourceFormCodeAndName, ref documentViewerDevEx);
                else
                    raporFast.ReportDocumentViewer(this, dsMsReports, dNMsReports, sourceFormCodeAndName, ref documentViewerFast);

            }
            /*
            t.WaitFormOpen("Rapor hazırlanıyor ...");

            ReportToolType = t.myInt32(tMSReports.Tables["MS_REPORTS"].Rows[tDataNavigator_MSResports.Position]["REPTOOL_TYPE"].ToString());

            if (ReportToolType <= (byte)v.ReportTool.DevExpress)
            {
                dv_report = null;
                dv_report = new XtraReport();

                if (xtraTabControl_View.SelectedTabPageIndex != 0)
                {
                    xtraTabControl_View.SelectedTabPage = xtraTabPage_DevExp;

                    previewBar_FastReport.Visible = false;
                    barHeader.Caption = "DevExpress";
                    previewBar_DevExpress.Offset = 45;
                    previewBar_DevExpress.Visible = true;

                    printPreviewSubItem1.Enabled = true;
                    printPreviewSubItem2.Enabled = true;
                    printPreviewSubItem3.Enabled = true;
                }
            }

            if (ReportToolType == (byte)v.ReportTool.FastReport)
            {
                //fs_report = null;
                //fs_report = new FastReport.Report();

                if (xtraTabControl_View.SelectedTabPageIndex != 1)
                {
                    //xtraTabControl_View.SelectedTabPage = xtraTabPage_FastRep;

                    previewBar_DevExpress.Visible = false;
                    barHeader.Caption = "FastReport";
                    previewBar_FastReport.Offset = 45;
                    previewBar_FastReport.Visible = true;

                    printPreviewSubItem1.Enabled = false;
                    printPreviewSubItem2.Enabled = false;
                    printPreviewSubItem3.Enabled = false;
                }
            }

            dsData = null;
            dsKrtr = null;

            try
            {
                ReportRead();
            }
            catch (Exception)
            {
                t.WaitFormClose();
            }

            t.WaitFormClose();
            */
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonRaporYazdir) RaporYazdir();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonRaporOnizleme) RaporOnizleme();
            }
            // SubItem butonlar
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonMsfMsfIP) CopyMsfMsfIP();
            }
        }

        private void RaporYazdir()
        {
            if (documentViewerDevEx != null)
                if (documentViewerDevEx.DocumentSource != null)
                    ((XtraReport)documentViewerDevEx.DocumentSource).PrintDialog();
        }

        private void RaporOnizleme()
        {
            if (documentViewerDevEx != null)
                if (documentViewerDevEx.DocumentSource != null)
                    ((XtraReport)documentViewerDevEx.DocumentSource).ShowRibbonPreview();
        }


    }
}
