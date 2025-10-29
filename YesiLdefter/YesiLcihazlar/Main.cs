using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace YesiLcihazlar
{


    public partial class Main : Form
    {
        tToolBox t = new tToolBox();
        cihazSqls Sql = new cihazSqls();

        fingerProcedures fP = new fingerProcedures();

        public Main()
        {

            InitializeComponent();
            
            myInitialize();

            #region UserLOGIN

            using (cihazStarter s = new cihazStarter())
            {
                s.InitStart();
            }
            
            if (v.SP_UserLOGIN)
            {
                // login işlemleri
                Login();

                // datasetleri hazırla
                createDataSets();

                OpenZkTeco50Finger();
            }

            #endregion

        }

        #region Login

        void Login()
        {
            if (v.SP_UserLOGIN)
            {
                //setMenuItems();

                //timer_Kullaniciya_Mesaj_Varmi.Enabled = true;

                //t.WaitFormOpen(v.mainForm, "ManagerDB Connection...");
                t.Db_Open(v.active_DB.managerMSSQLConn);

                if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                {
                    //t.WaitFormOpen(v.mainForm, "ProjectDB MSSQL Connection...");
                    t.Db_Open(v.active_DB.projectMSSQLConn);
                }
                
                /*
                barMSConn.Caption = v.active_DB.managerDBName;
                barPrjConn.Caption = v.active_DB.projectDBName;
                */
                //t.WaitFormOpen(v.mainForm, "SysTypes Load ...");
                //t.SYS_Types_Read();
                
                //versionChecked();

                setMainFormCaption();
            }
        }

        void setMainFormCaption()
        {
            // ---
            this.Text = "Üstad'ın yeşiL cihazı   Ver : " +
                v.tExeAbout.activeVersionNo.Substring(2, 6) + "." +
                v.tExeAbout.activeVersionNo.Substring(9, 4) +
                "    [ " + v.SP_FIRM_ID.ToString() + " : " + v.SP_FIRM_NAME + " ] ";
        }

        #endregion

        private void createDataSets()
        {
            v.dsCihazList = new DataSet();
            v.dsCihazLogIcmal1 = new DataSet();
            v.dsCihazLogIcmal2 = new DataSet();
            v.dsCihazLogDetay = new DataSet();
            v.dsCihazEmirYeniList = new DataSet();
            v.dsCihazEmirEskiList = new DataSet();
            v.dsCihazTest = new DataSet();
    }

        
        private void BtnGercekKisi_Click(object sender, EventArgs e)
        {
            Form tForm = new YesiLcihazlar.frmGercekKisi();
            t.ChildForm_View(tForm, this, FormWindowState.Normal);//  Maximized);
        }

        private void BtnZkTeco_Click(object sender, EventArgs e)
        {
            OpenZkTeco50Finger();
        }
        
        private void OpenZkTeco50Finger()
        {
            Form tForm = new YesiLcihazlar.Forms.frmZkTecoK50Finger();
            t.ChildForm_View(tForm, this, FormWindowState.Maximized);
        }


        private void myInitialize()
        {
            DataTable dtUserInfo = new DataTable("dsUserInfo");
            dtUserInfo.Columns.Add("Id", System.Type.GetType("System.String"));
            dtUserInfo.Columns.Add("MachineNumber", System.Type.GetType("System.Int32"));
            dtUserInfo.Columns.Add("EnrollNumber", System.Type.GetType("System.String")); // Enroll Number : userId : kayıt Numarası 
            dtUserInfo.Columns.Add("Name", System.Type.GetType("System.String"));
            dtUserInfo.Columns.Add("FingerIndex", System.Type.GetType("System.Int32"));
            dtUserInfo.Columns.Add("TmpData", System.Type.GetType("System.String"));
            dtUserInfo.Columns.Add("Privelage", System.Type.GetType("System.Int32"));
            dtUserInfo.Columns.Add("Password", System.Type.GetType("System.String"));
            dtUserInfo.Columns.Add("Enabled", System.Type.GetType("System.Boolean"));
            dtUserInfo.Columns.Add("iFlag", System.Type.GetType("System.String"));
            dtUserInfo.Columns.Add("CardNumber", System.Type.GetType("System.String"));

            v.dsUserInfo = new DataSet();
            v.dsUserInfo.Tables.Add(dtUserInfo);


            DataTable dtFingerLogData = new DataTable("dsFingerLogData");
            //dtFingerLogData.Columns.Add("Id", System.Type.GetType("System.Int32"));
            dtFingerLogData.Columns.Add("MachineNumber", System.Type.GetType("System.Int32"));
            dtFingerLogData.Columns.Add("UserID", System.Type.GetType("System.String"));
            dtFingerLogData.Columns.Add("VerifyDate", System.Type.GetType("System.DateTime"));
            //dtFingerLogData.Columns.Add("VerifyType", System.Type.GetType("System.Int32"));
            //dtFingerLogData.Columns.Add("VerifyState", System.Type.GetType("System.Int32"));
            //dtFingerLogData.Columns.Add("WorkCode", System.Type.GetType("System.Int32"));

            v.dsFingerLogData = new DataSet();
            v.dsFingerLogData.Tables.Add(dtFingerLogData);


            /*
            v.dsFingerLogData.Columns.Add("Id", System.Type.GetType("System.Int32"));
            v.dsFingerLogData.Columns.Add("MachineNumber", System.Type.GetType("System.Int32"));
            v.dsFingerLogData.Columns.Add("IndRegID", System.Type.GetType("System.Int32"));
            v.dsFingerLogData.Columns.Add("DateOnlyRecord", System.Type.GetType("System.DateTime"));
            v.dsFingerLogData.Columns.Add("TimeOnlyRecord", System.Type.GetType("System.DateTime"));
            */
            //grid.DataSource = dsUserInfo;

            panel1.Controls.Add(v.mesajLabel1);
            panel1.Controls.Add(v.progressBar1);
            v.mesajLabel1.Dock = DockStyle.Left;
            v.mesajLabel1.Width = (this.Width / 2);
            v.progressBar1.Dock = DockStyle.Right;
            v.progressBar1.Width = (this.Width / 2);


        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenZkTeco50Finger();
        }

        /*
         * cihazId
         * eylemId
         * tarihsaat
         * 
         * 
         * 
        private void createSDK()
        {
            var th = new Thread();
            th.Start();
            Thread.Sleep(1000);
        }
    

        private void createSDK_()
        {
            SDK = new fingerK50_SDKHelper(fP.RaiseDeviceEvent);
        }
        */


    }
}
