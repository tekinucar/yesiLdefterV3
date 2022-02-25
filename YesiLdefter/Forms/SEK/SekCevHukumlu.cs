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
    public partial class SekCevHukumlu : Form
    {
        tToolBox t = new tToolBox();
        CihazLogs cl = new CihazLogs();

        string menuName = "MENU_SEK/CEV/CAR/HUKUMLUKARTI";
        string buttonChzGonder = "item_3095";
        string buttonChzBioAl = "item_3096";
        string buttonChzSayGonder = "item_3097";

        public SekCevHukumlu()
        {
            InitializeComponent();
        }

        private void SekCevHukumlu_Shown(object sender, EventArgs e)
        {
            //MessageBox.Show("bingo ...");

            t.Find_Button_AddClick(this, menuName, buttonChzGonder, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonChzBioAl, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonChzSayGonder, myNavElementClick);
        }

        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonChzGonder) chzGonder();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonChzBioAl) chzBioAl();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonChzSayGonder) chzSayGonder();
            }
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonChzGonder) chzGonder();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonChzBioAl) chzBioAl();
                if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonChzSayGonder) chzSayGonder();
            }
        }

        private void chzGonder()
        {
            //MessageBox.Show("bingo1 ...");
            cihazEmirSet("SetNewUser");
        }

        private void chzBioAl()
        {
            //MessageBox.Show("bingo2 ...");
            cihazEmirSet("GetNewUserFP");
        }

        private void chzSayGonder()
        {
            //MessageBox.Show("bingo3 ...");
            cihazEmirSet("DelNewUser");
        }

        public void cihazEmirSet(string param)
        {
            string soru = "Cihaz işlemleri başlayacak, Onaylıyor musunuz ?";
            DialogResult cevap = t.mySoru(soru);
            if (DialogResult.Yes == cevap)
            {
                cl.prc_CihazEmir(0, param);
            }
        }

    }
}
