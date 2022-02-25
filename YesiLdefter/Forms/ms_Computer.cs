using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;

using Tkn_Events;
//using Tkn_EventsForm;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_Computer : DevExpress.XtraEditors.XtraForm
    {
        tToolBox t = new tToolBox();
        tEvents ev = new tEvents();
        tEventsForm evf = new tEventsForm();

        DataSet ds = null;
        DataNavigator dN = null;

        string TableIPCode = string.Empty;

        public ms_Computer()
        {
            InitializeComponent();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);

            this.Shown += new System.EventHandler(this.ms_Computer_Shown);

            this.KeyPreview = true;

        }

        void PreparingCompterValues()
        {
            TableIPCode = "UST/T01/SYSCOMP.SYSCOMP_F01";
                      //  "UST/T01/SYSCOMP.SYSCOMP_F01"
            t.Find_DataSet(this, ref ds, ref dN, TableIPCode);

            if (t.IsNotNull(ds))
            {
                if (ds.Tables[0].Rows[dN.Position]["NETWORK_MACADDRESS"].ToString() == "")
                {
                    ds.Tables[0].Rows[dN.Position]["ISACTIVE"] = 1;
                    ds.Tables[0].Rows[dN.Position]["SYSTEM_NAME"] = v.tComputer.SystemName.ToString();
                    ds.Tables[0].Rows[dN.Position]["NETWORK_MACADDRESS"] = v.tComputer.Network_MACAddress.ToString();
                    ds.Tables[0].Rows[dN.Position]["PROCESSOR_NAME"] = v.tComputer.Processor_Name.ToString();
                    ds.Tables[0].Rows[dN.Position]["PROCESSOR_ID"] = v.tComputer.Processor_Id.ToString();

                    ds.Tables[0].Rows[dN.Position]["DISK_MODEL"] = v.tComputer.DiskDrive_Model.ToString();
                    ds.Tables[0].Rows[dN.Position]["DISK_SERIALNUMBER"] = v.tComputer.DiskDrive_SerialNumber.ToString();

                }

                /*
    SYSTEM_NAME                VarChar(50)       NULL,
	NETWORK_MACADDRESS         VarChar(30)       NULL UNIQUE,  
	PROCESSOR_NAME             VarChar(50)       NULL,
	PROCESSOR_ID               VarChar(50)       NULL,
	
	DISK_MODEL                 VarChar(50)       NULL,
	DISK_SERIALNUMBER          VarChar(50)       NULL,

                */
            }


            //string[] controls = new string[] { };
            //txt_FirmGuid = t.Find_Control(this, "Column_FIRM_GUID", TableIPCode, controls);

        }

        private void ms_Computer_Shown(object sender, EventArgs e)
        {
            //this.Activated += new System.EventHandler(this.ms_Computer_Activated);
            //this.Deactivate += new System.EventHandler(this.ms_Computer_Deactivate);
            //this.Enter += new System.EventHandler(this.ms_Computer_Enter);
            //this.Leave += new System.EventHandler(this.ms_Computer_Leave);
            //this.Validated += new System.EventHandler(this.ms_Computer_Validated);

            //this.Text = this.Text + "s";
            PreparingCompterValues();



            // burada daha firmanın lisans adet kontrolu yapılacak
            // eğer sınır tamamlandıysa mevcut aktif computerlerin listesi gelecek 
            // bu isteden istediği bilgisayarın IsActivesini kapatıp yerine yeni computeri aktif edebilecek
            // tabiki bu kaydet butonuna basılınca gerçekleşecek


        }


        private void ms_Computer_Activated(object sender, EventArgs e)
        {
            //MessageBox.Show("Form1_Activated");
            //this.Text = this.Text + "a";
            //PreparingCompterValues();
        }

        private void ms_Computer_Deactivate(object sender, EventArgs e)
        {
            //MessageBox.Show("Form1_Deactivate");
            //this.Text = this.Text + "d";
            //PreparingCompterValues();
        }

        private void ms_Computer_Enter(object sender, EventArgs e)
        {
            //MessageBox.Show("Form1_Enter");
        }

        private void ms_Computer_Leave(object sender, EventArgs e)
        {
            //MessageBox.Show("Form1_Leave");
        }

        private void ms_Computer_Validated(object sender, EventArgs e)
        {
            //MessageBox.Show("Form1_Validated");
        }

        private void ms_Computer_FormClosing(object sender, FormClosingEventArgs e)
        {
            /// kullanıcın yeni girdiği firm_guidi al
            ///
            v.tComp.SP_COMP_FIRM_GUID = ds.Tables[0].Rows[0]["COMP_FIRM_GUID"].ToString();

            /// compter yeni kayıt sırasında aktif kabul edilecek
            /// 
            v.tComp.SP_COMP_ISACTIVE = 1;

            //MessageBox.Show("ms_Computer_FormClosing : "  + v.SP_COMP_FIRM_GUID);
        }

        private void ms_Computer_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MessageBox.Show("ms_Computer_FormClosed");
        }
    }
}
