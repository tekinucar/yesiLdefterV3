using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tkn_Forms
{
    class tForms:tBase
    {
        // kullanmak istedğin formları burada tek tek tanıtman gerekiyor
                
        public Form Get_Form(string FormName)
        {
            Form tForm = null;
            /*
            if (FormName == "ms_Form") tForm = new YesiLdefter.ms_Form();
            if (FormName == "ms_DBList") tForm = new YesiLdefter.ms_DBList();
            if (FormName == "ms_IPList") tForm = new YesiLdefter.ms_IPList();
            if (FormName == "ms_Layout") tForm = new YesiLdefter.ms_Layout();
            if (FormName == "ms_Menu") tForm = new YesiLdefter.ms_Menu();
            if (FormName == "ms_Pictures") tForm = new YesiLdefter.ms_Pictures();
            if (FormName == "ms_Computer") tForm = new YesiLdefter.ms_Computer();
            if (FormName == "ms_User") tForm = new YesiLdefter.ms_User();

            if (FormName == "ajn_Aranan") tForm = new YesiLdefter.ajn_Aranan();

            if (FormName == "hp_Musteri") tForm = new YesiLdefter.hp_Musteri();
            //if (FormName == "hp_CariList") tForm = new YesiLdefter.hp_CariList();
            //if (FormName == "hp_PersonelList") tForm = new YesiLdefter.hp_PersonelList();
            if (FormName == "fns_FisCari") tForm = new YesiLdefter.fns_FisCari();
            if (FormName == "fns_Fis") tForm = new YesiLdefter.fns_Fis();
            if (FormName == "fns_FinansFisleri") tForm = new YesiLdefter.fns_FinansFisleri();
            */

            //if (FormName == "MSReportDesigner") tForm = new YesiLdefter.MSReportDesigner();
            //if (FormName == "MSReportDetail") tForm = new YesiLdefter.MSReportDetail();
            //if (FormName == "MSReportParam") tForm = new YesiLdefter.MSReportParam();
            //if (FormName == "MSReportView") tForm = new YesiLdefter.MSReportView();

            //if (FormName == "TestFormu1") tForm = new YesiLdefter.TestForm1();

            //MessageBox.Show("DİKKAT : Aranan form bulunamadı ... [ " + FormName + " ] ");

            return tForm; 
        }

        public Form Get_Form(string FormName, Form SourceForm)
        {
            //if (FormName == "tn_SMSGonder") return new Tabim.MTSK.tn_SMSGonder(SourceForm);
            //if (FormName == "tn_SMSSablonuTanimla") return new Tabim.MTSK.tn_SMSSablonuTanimla(SourceForm);

            return null;
        }

    }
}


