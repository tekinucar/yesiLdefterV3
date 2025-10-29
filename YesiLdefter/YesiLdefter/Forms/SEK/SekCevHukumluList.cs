using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_ToolBox;

namespace YesiLdefter.Forms.SEK
{
    public partial class SekCevHukumluList : Form
    {
        tToolBox t = new tToolBox();
        CihazLogs cl = new CihazLogs();

        DevExpress.XtraGrid.Views.Tile.TileView newUserTileView = null;
        DevExpress.XtraGrid.Views.Tile.TileView oldUserTileView = null;
        DevExpress.XtraGrid.Views.Tile.TileView tahliyeTileView = null;


        public SekCevHukumluList()
        {
            InitializeComponent();
        }

        private void SekCevHukumluList_Shown(object sender, EventArgs e)
        {
            preparingPage();
        }

        private void preparingPage()
        {
            Control viewcntrl = null;
            string ipCode = "";

            ipCode = "SEK/CEV/prcCihazLogGetIcmal.Icmal_L02";
            viewcntrl = t.Find_Control_View(this, ipCode);
            newUserTileView = ((DevExpress.XtraGrid.GridControl)viewcntrl).MainView as DevExpress.XtraGrid.Views.Tile.TileView;
            newUserTileView.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(cl.cihazTileView_ItemClick);

            ipCode = "SEK/CEV/prcCihazLogGetIcmal.Icmal_L03";
            viewcntrl = t.Find_Control_View(this, ipCode);
            oldUserTileView = ((DevExpress.XtraGrid.GridControl)viewcntrl).MainView as DevExpress.XtraGrid.Views.Tile.TileView;
            oldUserTileView.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(cl.cihazTileView_ItemClick);
            
            ipCode = "SEK/CEV/prcCihazLogGetIcmal.Icmal_L04";
            viewcntrl = t.Find_Control_View(this, ipCode);
            tahliyeTileView = ((DevExpress.XtraGrid.GridControl)viewcntrl).MainView as DevExpress.XtraGrid.Views.Tile.TileView;
            tahliyeTileView.ItemClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(cl.cihazTileView_ItemClick);
            
        }

        /*
        private void tileView_ItemClick(object sender, DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventArgs e)
        {
            //MessageBox.Show(e.Item.Text);
            
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
        */

    }
}
