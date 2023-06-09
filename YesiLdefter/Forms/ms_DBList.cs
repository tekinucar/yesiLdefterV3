using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;
using Tkn_DataCopy;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_DBList : DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();
        

        DataSet dsTables = null;
        DataNavigator dNTables = null;
        DataSet ds_DataList = null;
        DataNavigator dN_DataList = null;

        string tableListTableIPCode = "UST/T01/3S_TBL.3S_TBL_01";
        string dataListTableIPCode = "UST/T01/3S_DATA.List_L01";

        string menuName = "MENU_" + "UST/PMS/PMS/Database";
        string buttonInsertPaketOlustur = "ButtonPaketOlustur";
        string cumleData = "";
        public ms_DBList()
        {
            InitializeComponent();
        }

        private void ms_DBList_Shown(object sender, EventArgs e)
        {
            // Tables List
            t.Find_DataSet(this, ref dsTables, ref dNTables, tableListTableIPCode);
            t.Find_DataSet(this, ref ds_DataList, ref dN_DataList, dataListTableIPCode);

            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = t.Find_Control(this, "simpleButton_ek1", tableListTableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Width = 105;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Text = "Tabloyu Kopyala";
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_TabloyuKopyalar_Click);
            }


            t.Find_Button_AddClick(this, menuName, buttonInsertPaketOlustur, myNavElementClick);
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            /*
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonTest) Test();
            }
            */
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonInsertPaketOlustur) InsertPaketOlustur();
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonPaketiGonder) PaketiGonder();
            }
        }

        private void InsertPaketOlustur()
        {
            if (t.IsNotNull(ds_DataList) == false) return;

            string databaseName = dsTables.Tables[0].Rows[dNTables.Position]["databaseName"].ToString();
            string schemaName = dsTables.Tables[0].Rows[dNTables.Position]["schemaName"].ToString();
            string tableName = dsTables.Tables[0].Rows[dNTables.Position]["tableName"].ToString();

            string soru = databaseName + "." + schemaName + "." + tableName + " tablosunun datası için INSERT paketi oluşturulacak, Onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                vScripts scripts = new vScripts();
                scripts.SourceDBaseName = databaseName;
                scripts.SchemaName = schemaName;
                scripts.SourceTableName = tableName;
                scripts.IdentityInsertOnOff = true;

                cumleData = "";
                cumleData = t.preparingInsertScript(scripts);

                PaketiGonder();
            }
        }

        private void PaketiGonder()
        {
            /*
            string cevap = "";
            string dBaseNo = "";

            tToolBox t = new tToolBox();
            vUserInputBox iBox = new vUserInputBox();

            // SERVERNAME
            if (tSql != "")
            {
                iBox.title = "Hazırlanacak data hangi database türüne insert edilecek ?";
                iBox.promptText = "1. Manager, 2. Project, 3. Ustad CRM";
                iBox.value = "2";
                iBox.displayFormat = "";
                iBox.fieldType = 0;
                if (t.UserInpuBox(iBox) == DialogResult.OK)
                {
                    cevap = iBox.value;
                    if (cevap == "1") dBaseNo = Convert.ToString((byte)v.dBaseNo.Manager);

                }
            }

            */

            v.SQLSave = "";
            v.SQLSave = cumleData;

            if (cumleData != "")
                t.runScript(v.dBaseNo.WebManager, cumleData);

            t.FlyoutMessage("Web Manager Database Update", "Insert paketler gönderildi...");
        }





        private void btn_TabloyuKopyalar_Click(object sender, EventArgs e)
        {
            Dosya_Kopyala();
        }

        private void Dosya_Kopyala()
        {
            //MessageBox.Show("ok");

            if (t.IsNotNull(dsTables) == false) return;

            string table_name = dsTables.Tables[0].Rows[dNTables.Position]["tableName"].ToString();

            string s =
                String.Format(" Select * from [{0}].[dbo].[MS_TABLES] " +
                              " where TABLE_NAME = '{1}'", v.active_DB.managerDBName, table_name);

            Boolean sonuc = t.Find_Record(v.active_DB.managerMSSQLConn, s);

            if (sonuc)
            {
                MessageBox.Show("Aynı isimde kayıtlı tablo mevcut ... " + table_name);
                return;
            }
            else
            {

                //xtraTabPage_MS.Show();

                tDataCopy dc = new tDataCopy();

                // Tabloyu kopyala, onay true dönerse filedleride kopyala
                string DataCopyCode = "DC_MSTABLE01";
                if (dc.tDC_Run(this, DataCopyCode))
                {
                    DataCopyCode = "DC_MSFIELD01";
                    dc.tDC_Run(this, DataCopyCode);
                }
                /*
                if (dc.tDC_Run(this, v.SP_Conn_MainManager_MSSQL, "DC_MSTABLE01"))
                {
                    // fieldleri kopyala
                    dc.tDC_Run(this, v.SP_Conn_MainManager_MSSQL, "DC_MSFIELD01");
                }
                */
            }


        }


    }
}