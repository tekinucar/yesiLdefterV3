using DevExpress.CodeParser;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class CihazLogs : tBase
    {
        tToolBox t = new tToolBox();

        Form tMForm = null;
        System.Windows.Forms.Timer timerCihazLogGetIcmal = null;
        DataSet dsCihazLogIcmal = null;
        DataNavigator dNCihazLogIcmal = null;
        bool cihazLogIcmalRefresh = false;
        int cihazLogIcmalPos = 0;

        SqlConnection baglanti;
        SqlCommand cmd;
        SqlDependency dependency;
        
        #region Dependency

        void Dependency()
        {
            dependency = new SqlDependency(cmd);

            SqlDependency.Stop(v.active_DB.projectConnectionText);
            SqlDependency.Start(v.active_DB.projectConnectionText);

            dependency.OnChange += Dependency_OnChange;
        }
        void EylemPlaniGetir()
        {
            try
            {
                baglanti = new SqlConnection(v.active_DB.projectConnectionText);
                cmd = new SqlCommand(@" Select 
                 [Id]
                ,[FirmId]
                ,[Tarih]
                ,[IsActive]
                ,[IsCancel]
                ,[EylemTipiId]
                ,[LokalId]
                ,[CariId]
                ,[BaslamaTarihSaat]
                ,[BitisTarihSaat]
                ,[VadeGun]
                ,[Aciklama]
                ,[GirisCihazKaydiId]
                ,[CikisCihazKaydiId]
                ,[Gun]
                ,[BaslamaSaati]
                ,[BitisSaati]
                ,[Sure]
                ,[GirisTarihi]
                ,[CikisTarihi]    
                From [dbo].[EylemPlani] Where FirmId = " + v.SP_FIRM_ID + @" And   Convert(date,Tarih,104) = Convert(date,getDate(),104) ", baglanti);

                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                Dependency();

                //SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //DataTable Personeller = new DataTable();
                //Personeller.Load(dr);

                //dgvPersoneller.DataSource = null;
                //dgvPersoneller.DataSource = Personeller;

                baglanti.Dispose();
                cmd.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                throw;
            }

        }
        
        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //  MessageBox.Show("Veritabanı güncellenmiştir. " + e.Info.ToString());
            //  barMesajlar.Caption = DateTime.Now.ToLongTimeString();
            dependency.OnChange -= Dependency_OnChange;
            EylemPlaniGetir();
        }

        #endregion Dependency


        /// CihazLogs 
        /// 
        public void CihazLog(Form tForm, Timer timerCihazLogGetIcmal_)//, DataSet dsCihazLogIcmal, DataNavigator dNCihazLogIcmal)
        {
            tMForm = tForm;
            timerCihazLogGetIcmal = timerCihazLogGetIcmal_;

            string ipCode = "SEK/CEV/prcCihazLogGetIcmal.Icmal_L01";
            using (tMainForm f = new tMainForm())
            {
                f.preparingDockPanel(tForm, ipCode);
            }
            
            Control viewcntrl = t.Find_Control_View(tForm, ipCode);

            DevExpress.XtraGrid.Views.Tile.TileView cihazTileView =
                ((DevExpress.XtraGrid.GridControl)viewcntrl).MainView as DevExpress.XtraGrid.Views.Tile.TileView;

            cihazTileView.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(cihazTileView_ItemClick);

            t.Find_DataSet(tForm, ref dsCihazLogIcmal, ref dNCihazLogIcmal, ipCode);
            dNCihazLogIcmal.PositionChanged += new System.EventHandler(dNCihazLogIcmal_PositionChanged);

            cihazLogIcmalPos = dNCihazLogIcmal.Position;

            //timerCihazLogGetIcmal = new System.Windows.Forms.Timer(this.components);
            timerCihazLogGetIcmal.Interval = 10000;
            timerCihazLogGetIcmal.Tick += new System.EventHandler(timerCihazLogGetIcmal_Tick);
            timerCihazLogGetIcmal.Enabled = true;
        }

        private void timerCihazLogGetIcmal_Tick(object sender, EventArgs e)
        {
            CihazLogGetIcmal();
        }

        private void CihazLogGetIcmal()
        {
            cihazLogIcmalRefresh = true;
            t.TableRefresh(tMForm, dsCihazLogIcmal);
            cihazLogIcmalRefresh = false;

            if (dNCihazLogIcmal.Position != cihazLogIcmalPos)
                dNCihazLogIcmal.Position = cihazLogIcmalPos;

            //barMesajlar.Caption = DateTime.Now.ToLongTimeString() + " ; " + cihazLogIcmalPos;
        }

        public void dNCihazLogIcmal_PositionChanged(object sender, EventArgs e)
        {
            if (cihazLogIcmalRefresh == false)
                cihazLogIcmalPos = ((DevExpress.XtraEditors.DataNavigator)sender).Position;

            //barMesajlar.Caption = DateTime.Now.ToLongTimeString() + " ; " + cihazLogIcmalPos;
        }

        public void cihazTileView_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            //MessageBox.Show("aaa"+ e.Item.Text);

            //string param = dsCihazLogIcmal.Tables[0].Rows[dNCihazLogIcmal.Position]["Param"].ToString();
            //string adet = dsCihazLogIcmal.Tables[0].Rows[dNCihazLogIcmal.Position]["Adet"].ToString();

            string param = e.Item.Text;
            if (param != "")
            { 
                string soru = "Cihaz işlemleri başlayacak, Onaylıyor musunuz ?";
                DialogResult cevap = t.mySoru(soru);
                if (DialogResult.Yes == cevap)
                {
                    //MessageBox.Show(param);
                    prc_CihazEmir(0, param);
                }
            }
        }

        public void prc_CihazEmir(int Id, string Param)
        {
            string sql =
        @" 
DECLARE @Id int
DECLARE @FirmId int
DECLARE @TalepTipiId smallint

Set @Id = :CihazEmirId
Set @FirmId = :FIRM_ID
Set @TalepTipiId = ':TalepTipiId'
EXECUTE [dbo].[prc_CihazEmir] @Id, @FirmId, @TalepTipiId
        ";

            // burda Id her zaman = 0 geliyor bu sayede her zaman insert çalışmakta
            sql = sql.Replace(":CihazEmirId", Id.ToString());
            sql = sql.Replace(":TalepTipiId", TalepTipiIdGet(Param).ToString());

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazEmir");
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");

            DataSet ds = new DataSet();
            ds.Namespace = myProp;

            try
            {
                t.Data_Read_Execute(null, ds, ref sql, "CihazEmir", null);
                MessageBox.Show("Talebiniz iletilmiştir...");
            }
            catch (Exception)
            {
                throw;
            }
            ds.Dispose();
        }

        private Int16 TalepTipiIdGet(string name)
        {
            Int16 talep = 0;

            if (name == "GetAllUserAndFPs") talep = (Int16)v.cihazTalepTipi.chGetAllUserAndFPs;
            if (name == "GetAllLogs") talep = (Int16)v.cihazTalepTipi.chGetAllLogs;
            if (name == "SetAllUserAndFPs") talep = (Int16)v.cihazTalepTipi.chSetAllUserAndFPs;

            if (name == "SetNewUser")      talep = (Int16)v.cihazTalepTipi.chSetNewUser;
            if (name == "GetNewUserFP")    talep = (Int16)v.cihazTalepTipi.chGetNewUserFP;
            if (name == "SetNewUserSayim") talep = (Int16)v.cihazTalepTipi.chSetNewUserSayim;

            if (name == "SetOldUser")      talep = (Int16)v.cihazTalepTipi.chSetOldUser;
            if (name == "GetOldUserFP")    talep = (Int16)v.cihazTalepTipi.chGetOldUserFP;
            if (name == "SetOldUserSayim") talep = (Int16)v.cihazTalepTipi.chSetOldUserSayim;

            if (name == "SetSayim") talep = (Int16)v.cihazTalepTipi.chSetSayim;
            if (name == "SetTahliye") talep = (Int16)v.cihazTalepTipi.chSetTahliye;
            if (name == "SetGorev") talep = (Int16)v.cihazTalepTipi.chSetGorev;
            if (name == "SetGorus") talep = (Int16)v.cihazTalepTipi.chSetGorus;
            if (name == "GetGorev") talep = (Int16)v.cihazTalepTipi.chGetGorev;
            if (name == "GetGorus") talep = (Int16)v.cihazTalepTipi.chGetGorus;
            if (name == "GetSayim") talep = (Int16)v.cihazTalepTipi.chGetSayim;
            if (name == "DelGorev") talep = (Int16)v.cihazTalepTipi.chDelGorev;
            if (name == "DelGorus") talep = (Int16)v.cihazTalepTipi.chDelGorus;
            if (name == "DelTahliye") talep = (Int16)v.cihazTalepTipi.chDelTahliye;

            if (name == "Icmal")        talep = (Int16)v.cihazTalepTipi.chIcmal;
            if (name == "IcmalNewUser") talep = (Int16)v.cihazTalepTipi.chIcmalNewUser;
            if (name == "IcmalOldUser") talep = (Int16)v.cihazTalepTipi.chIcmalOldUser;
            if (name == "IcmalTahliye") talep = (Int16)v.cihazTalepTipi.chIcmalTahliye;

            return talep;
        }

    }

}
