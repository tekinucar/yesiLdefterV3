using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;
using Tkn_DataCopy;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_DBList : DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();

        DataSet dsTables = null;
        DataNavigator tDataNavigator_Tables = null;

        string TableIPCode = string.Empty;

        public ms_DBList()
        {
            InitializeComponent();
        }

        private void ms_DBList_Shown(object sender, EventArgs e)
        {
            // Tables List
            TableIPCode = "UST/T01/3S_TBL.3S_TBL_01";

            t.Find_DataSet(this, ref dsTables, ref tDataNavigator_Tables, TableIPCode);

            //TableIPCode = "3S_TBL.3S_TBL_01";
            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = t.Find_Control(this, "simpleButton_ek1", TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Width = 105;
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Text = "Tabloyu Kopyala";
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Click += new System.EventHandler(btn_TabloyuKopyalar_Click);
            }

        }

        private void btn_TabloyuKopyalar_Click(object sender, EventArgs e)
        {
            Dosya_Kopyala();
        }

        private void Dosya_Kopyala()
        {
            //MessageBox.Show("ok");

            if (t.IsNotNull(dsTables) == false) return;

            string table_name = dsTables.Tables[0].Rows[tDataNavigator_Tables.Position]["tableName"].ToString();

            string s =
                String.Format(" Select * from [{0}].[dbo].[MS_TABLES] " +
                              " where TABLE_NAME = '{1}'", v.active_DB.managerDBName, table_name);

            Boolean sonuc = t.Find_Record(v.SP_Conn_Manager_MSSQL, s);

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