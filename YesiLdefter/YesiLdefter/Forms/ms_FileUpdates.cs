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
using Tkn_SQLs;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_FileUpdates : Form
    {
        tToolBox t = new tToolBox();
        DataSet ds = null;
        DataNavigator dN = null;

        string menuName = "MENU_" + "UST/PMS/PMS/FileUpdates";
        string buttonFilePaketle = "ButtonFilePaketle";
        string buttonFileUpload = "ButtonFileUpload";
        string fileListTableIPCode = "UST/PMS/MsFileUpdates.List_L01";

        public ms_FileUpdates()
        {
            InitializeComponent();
        }
        private void ms_FileUpdates_Shown(object sender, EventArgs e)
        {
            t.Find_Button_AddClick(this, menuName, buttonFilePaketle, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonFileUpload, myNavElementClick);

            if (ds == null)
            {
                t.Find_DataSet(this, ref ds, ref dN, fileListTableIPCode);
            }
        }
        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonFilePaketle) filePaketle();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonFileUpload) fileUpload();
            }
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonInsertPaketOlustur) InsertPaketOlustur();
            }
        }

        private void filePaketle()
        {
            if (t.IsNotNull(ds))
            {
                string fileName = ds.Tables[0].Rows[dN.Position]["FileName"].ToString();
                string extension = ds.Tables[0].Rows[dN.Position]["Extension"].ToString();
                string versionNo = ds.Tables[0].Rows[dN.Position]["VersionNo"].ToString();

                v.tExeAbout.orderFileName = fileName;
                v.tExeAbout.orderFileExtension = extension;
                v.tExeAbout.orderFileVersionNo = versionNo;

                bool onay = t.CompressFile(v.tExeAbout, v.fileType.OrderFile);
                if (onay)
                {
                    ds.Tables[0].Rows[dN.Position]["PacketName"] = v.tExeAbout.newPacketName;

                    Application.DoEvents();
                    // kaydı aç
                    ds.Tables[0].CaseSensitive = false;
                    dN.Tag = dN.Position;
                    NavigatorButton btnEnd = dN.Buttons.EndEdit;
                    dN.Buttons.DoClick(btnEnd);
                }
            }
        }
        private void fileUpload()
        {
            if (t.IsNotNull(ds))
            {
                string fileName = ds.Tables[0].Rows[dN.Position]["FileName"].ToString();
                string extension = ds.Tables[0].Rows[dN.Position]["Extension"].ToString();
                string versionNo = ds.Tables[0].Rows[dN.Position]["VersionNo"].ToString();
                string packetName = ds.Tables[0].Rows[dN.Position]["PacketName"].ToString();

                v.tExeAbout.orderFileName = fileName;
                v.tExeAbout.orderFileExtension = extension;
                v.tExeAbout.orderFileVersionNo = versionNo;

                v.tExeAbout.newFileName = fileName;
                v.tExeAbout.newPacketName = packetName;

                bool onay = t.ftpUpload(v.tExeAbout);

                /// Dosya hakkındaki bilgiyi zaten biz elle giriyoruz
                /// onun için gerekyok 
                if (onay)
                {
                    //tSQLs sqls = new tSQLs();
                    //DataSet ds = new DataSet();
                    
                    //string sql = sqls.Sql_MsFileUpdates_Insert();
                    //t.SQL_Read_Execute(v.dBaseNo.publishManager, ds, ref sql, "", "MsFileUpdates");
                }

            }



        }
    }
}
