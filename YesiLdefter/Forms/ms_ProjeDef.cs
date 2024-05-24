using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Tkn_Events;
using Tkn_InputPanel;
using Tkn_ToolBox;

namespace YesiLdefter
{

    public partial class ms_ProjeDef : DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();

        DataSet ds = null;
        DataNavigator dN = null;

        string TableIPCode = string.Empty;
        string menuName = "MENU_" + "PRJ_DEF";
        string buttonChild = "item_2592";
        string buttonNormal = "item_2593";
        string buttonDialog = "item_2594";
        string buttonIPTesti = "item_2595";
        string buttonTableFields = "item_2596";


        public ms_ProjeDef()
        {
            InitializeComponent();
        }

        private void ms_ProjeDef_Shown(object sender, EventArgs e)
        {
            //((DevExpress.XtraBars.Navigation.AccordionControlElementBase)sender).Click

            //DevExpress.XtraBars.Navigation.AccordionControlElementEventArgs

            t.Find_Button_AddClick_(this, menuName, buttonDialog, myAccordionControlElementClick);
            t.Find_Button_AddClick_(this, menuName, buttonNormal, myAccordionControlElementClick);
            t.Find_Button_AddClick_(this, menuName, buttonChild, myAccordionControlElementClick);

            t.Find_Button_AddClick_(this, menuName, buttonIPTesti, myAccordionControlElementClick);
            t.Find_Button_AddClick_(this, menuName, buttonTableFields, myAccordionControlElementClick);

            if (ds == null)
            {
                TableIPCode = "SYSPROJ.SYSPROJ_L01";
                t.Find_DataSet(this, ref ds, ref dN, TableIPCode);
            }
        }
        //private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        private void myAccordionControlElementClick(object sender, EventArgs e)
        {
            if (((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Name == buttonDialog) TestDialog();
            if (((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Name == buttonNormal) TestNormal();
            if (((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Name == buttonChild) TestChild();
            if (((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Name == buttonIPTesti) TestIP();
            if (((DevExpress.XtraBars.Navigation.AccordionControlElement)sender).Name == buttonTableFields) TestTableFields();
        }

        private void TestDialog()
        {
            string FormCode = "DNM_01";

            if (ds != null)
            {
                if (dN.Position > -1)
                    FormCode = ds.Tables[0].Rows[dN.Position]["LAYOUT_CODE"].ToString();
            }

            string Prop_Navigator = @"
            0=FORMNAME:null;
            0=FORMCODE:" + FormCode + @";
            0=FORMTYPE:DIALOG;
            0=FORMSTATE:NORMAL;
            ";

            t.OpenForm(new ms_Form(), Prop_Navigator);
        }

        private void TestNormal()
        {
            string FormCode = "DNM_01";

            if (ds != null)
            {
                if (dN.Position > -1)
                    FormCode = ds.Tables[0].Rows[dN.Position]["LAYOUT_CODE"].ToString();
            }

            string Prop_Navigator = @"
            0=FORMNAME:null;
            0=FORMCODE:" + FormCode + @";
            0=FORMTYPE:NORMAL;
            0=FORMSTATE:NORMAL;
            ";

            t.OpenForm(new ms_Form(), Prop_Navigator);
        }

        private void TestChild()
        {
            string FormCode = "DNM_01";

            if (ds != null)
            {
                if (dN.Position > -1)
                    FormCode = ds.Tables[0].Rows[dN.Position]["LAYOUT_CODE"].ToString();
            }

            string Prop_Navigator = @"
            0=FORMNAME:null;
            0=FORMCODE:" + FormCode + @";
            0=FORMTYPE:CHILD;
            0=FORMSTATE:NORMAL;
            ";

            t.OpenForm(new ms_Form(), Prop_Navigator);
        }

        private void TestIP()
        {
            string TableIPCode = "";
            string FormWidth = "0";
            string FormHeight = "0";

            if (ds != null)
            {
                if (dN.Position > -1)
                {
                    TableIPCode = ds.Tables[0].Rows[dN.Position]["TABLE_CODE"].ToString();
                    FormWidth = ds.Tables[0].Rows[dN.Position]["CMP_WIDTH"].ToString();
                    FormHeight = ds.Tables[0].Rows[dN.Position]["CMP_HEIGHT"].ToString();
                }
                else return;
            }

            tToolBox t = new tToolBox();
            tInputPanel ip = new tInputPanel();

            string Caption = "Test InputPanel";

            #region Create tForm 

            int width = 0;
            int height = 0;
            if (FormWidth != "MAX") width = t.myInt32(FormWidth);
            if (FormHeight != "MAX") height = t.myInt32(FormHeight);

            if (width == 0) width = 800;
            if (height == 0) height = 600;

            ///
            /// New Form
            ///
            tEventsForm evf = new tEventsForm();
            Form tTestForm = new Form();

            tTestForm.Size = new Size(width, height);
            evf.myFormEventsAdd(tTestForm);

            ///
            /// groupControl
            ///
            DevExpress.XtraEditors.GroupControl groupControl = new GroupControl();
            groupControl.Text = Caption;
            groupControl.Dock = DockStyle.Fill;

            if (t.IsNotNull(TableIPCode))
                ip.Create_InputPanel(tTestForm, groupControl, TableIPCode, 1, true);

            tTestForm.Controls.Add(groupControl);

            /// Open SearchForm
            /// 
            t.DialogForm_View(tTestForm, FormWindowState.Normal);

            #endregion Create tTestForm 

        }

        private void TestTableFields()
        {
            //
        }

    }
}
