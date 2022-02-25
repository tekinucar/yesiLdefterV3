using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

using Tkn_CreateObject;
using Tkn_Events;
using Tkn_InputPanel;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_SubView
{
    public class tSubViews : tBase 
    {
        /// <summary>
        ///  şimdilik iptal 
        ///  
        /// </summary>
        /// <param name="tForm"></param>
        /// <param name="Prop_Navigator"></param>
        /// <param name="selectItemValue"></param>
        /// <param name="caption"></param>
        public void tSubView_(Form tForm, string Prop_Navigator, string selectItemValue, string caption)
        {
            tToolBox t = new tToolBox();

            #region 
            
            /// 0=ROW_PROP_NAVIGATOR:0;
            /// 0=CAPTION:null;
            /// 0=BUTTONTYPE:null;
            /// 0=TABLEIPCODE_LIST:TABLEIPCODE_LIST={
            /// 1=ROW_TABLEIPCODE_LIST:1;
            /// 1=CAPTION:Detail Group Bilgileri;
            ///
            /// 1=TABLEIPCODE:FNLNVOL.FNLNVOL_KSL01;
            /// 1=TABLEALIAS:PARAM_SET;
            /// 1=KEYFNAME:AND ( [FNLNVOL].BHESAP_ID = {0} OR [FNLNVOL].AHESAP_ID = {0} );
            ///
            /// 1=RTABLEIPCODE:LKP_FULL_NAME;
            /// 1=RKEYFNAME:LKP_REF_ID;
            /// 1=MSETVALUE:null;
            /// 1=WORKTYPE:READ;
            /// 1=CONTROLNAME:null;
            /// 1=ROWE_TABLEIPCODE_LIST:1;
            /// TABLEIPCODE_LIST=};
            ///
            /// 0=FORMNAME:null;
            /// 0=FORMCODE:null;
            /// 0=FORMTYPE:null;
            /// 0=FORMSTATE:null;
            /// 0=CHC_IPCODE:HPBNK.HPBNK_02;
            /// 0=CHC_FNAME:LKP_FNS_TIPI;
            /// 0=CHC_VALUE:2020;
            /// 0=CHC_OPERAND:=;
            /// 0=ROWE_PROP_NAVIGATOR:0;                    
            
            #endregion

            string TableIPCode = t.MyProperties_Get(Prop_Navigator, "=TABLEIPCODE:");
            string TableAlias = t.MyProperties_Get(Prop_Navigator, "=TABLEALIAS:");
            string KeyFName = t.MyProperties_Get(Prop_Navigator, "=KEYFNAME:");

            //-----
            // 1. Tablo
            string myFormLoadValue = string.Empty;
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "BEGIN", string.Empty);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetTableIPCode", TableIPCode);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetTableAlias", TableAlias);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetFieldName", KeyFName);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetValue", selectItemValue);
            // 2. Tablo
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondTableIPCode", TableIPCode);
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondTableAlias", TableAlias);
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondFieldName", "AHESAP_ID");
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondValue", "1");
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "END", string.Empty);

            v.con_FormLoadValue = myFormLoadValue;

            tSubView_Preparing(tForm, "TabPage", "", TableIPCode, selectItemValue, caption, "");
        }

        public void tSubView_Preparing(Form tForm, 
            string ViewType, string Form_Code, string TableIPCode,
            string ReadValue, string ReadCaption, string SM_PageName)
        {
            tToolBox t = new tToolBox();

            #region TabPage ise
            if ((ViewType == "TabPage") ||
                (ViewType == "TabPage2"))
            {
                tInputPanel ip = new tInputPanel();
                tCreateObject co = new tCreateObject();
                tEvents ev = new tEvents();

                // yok ise yeni TabPage oluşturulacak
                // var ise Show etmesi sağlanacak
                string TabControlName = "tabControl_SUBVIEW";
                ev.CreateOrSelect_Page(tForm, TabControlName, TableIPCode, ReadValue);

                // yukarıda oluşturulan TabPage aranıyor
                Control cntrl = null;
                string[] controls = new string[] {
                    "DevExpress.XtraTab.XtraTabPage",
                    "DevExpress.XtraBars.Navigation.TabNavigationPage" };

                cntrl = t.Find_Control(tForm, "tTabPage_" + t.AntiStr_Dot(TableIPCode + ReadValue), "", controls);

                // TabPage bulunduysa üzerinde istenen IP (InputPanel) oluşturuluyor
                if (cntrl != null)
                {
                    // TabPage nin içi boş ise InputPanelin içi hazırlanması gerekiyor
                    //if (((DevExpress.XtraTab.XtraTabPage)cntrl).Controls.Count == 0)
                    if (cntrl.Controls.Count == 0)
                    {
                        if (ViewType == "TabPage")
                        {
                            ip.Create_InputPanel(tForm, cntrl, TableIPCode, ReadValue, 1);

                            if (t.IsNotNull(ReadCaption))
                                cntrl.Text = " [ " + ReadCaption + " ].[ " + ReadValue + " ] ";

                            /// 'DEFAULT_TYPE', 22, '', 'Source ParentControl.Tag READ'); 
                            ///  Viewin içinde bulunduğu control ün tag ındaki value yi okumak için
                            /// 
                            cntrl.Tag = ReadValue;

                            //tTabPage_FNLNVOL_FNLNVOL_BNL0123
                            //FNLNVOL.FNLNVOL_BNL01|23   grid.
                        }

                        if (ViewType == "TabPage2")
                        {
                            SplitContainer splitContainer1 = new System.Windows.Forms.SplitContainer();
                            // 
                            // splitContainer1
                            // 
                            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
                            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
                            splitContainer1.Location = new System.Drawing.Point(0, 0);
                            splitContainer1.Name = "splitContainer1";
                            splitContainer1.Size = new System.Drawing.Size(958, 308);
                            splitContainer1.SplitterDistance = 195;
                            splitContainer1.TabIndex = 0;

                            ((DevExpress.XtraTab.XtraTabPage)cntrl).Controls.Add(splitContainer1);

                            ip.Create_InputPanel(tForm, splitContainer1.Panel1, TableIPCode, 2);
                            ip.Create_InputPanel(tForm, splitContainer1.Panel2, TableIPCode, 1);

                            ((DevExpress.XtraTab.XtraTabPage)cntrl).Text = splitContainer1.Panel2.Text;
                        }

                    }
                }

            }
            #endregion TabPage

            #region dockPanel ise
            if (ViewType == "dockPanel")
            {
                // yukarıda oluşturulan TabPage aranıyor
                Control cntrl = null;
                string[] controls = new string[] { "DevExpress.XtraBars.Docking.DockPanel" };
                cntrl = t.Find_Control(tForm, Form_Code, "", controls);

                // TabPage bulunduysa üzerinde istenen IP (InputPanel) oluşturuluyor
                if (cntrl != null)
                {
                    // dockPanel boş ise InputPanelin içi hazırlanması gerekiyor
                    if (((DevExpress.XtraBars.Docking.DockPanel)cntrl).Controls.Count == 1)
                    {
                        tInputPanel ip = new tInputPanel();
                        ip.Create_InputPanel(tForm, ((DevExpress.XtraBars.Docking.DockPanel)cntrl), TableIPCode, 1);
                    }
                }
            }
            #endregion dockPanel ise

            // PageName var ise bir menunün (Ribbon un) page ismidir ve bu page show edilececek
            #region Menu Page Show
            if (t.IsNotNull(SM_PageName))
            {
                /*
                Form tForm = t.Find_Form(sender);

                Control c = t.Find_Control(tForm, "MENU_VZ_BNK_TP");

                if (c != null)
                {

                }
                */
            }
            #endregion Menu Page Show
        }
        
    }
}
