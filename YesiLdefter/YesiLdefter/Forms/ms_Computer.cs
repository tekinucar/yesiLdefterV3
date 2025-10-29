using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Windows.Forms;

using Tkn_Events;
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

        private void ms_Computer_Shown(object sender, EventArgs e)
        {
            //this.Activated += new System.EventHandler(this.ms_Computer_Activated);
            //this.Deactivate += new System.EventHandler(this.ms_Computer_Deactivate);
            //this.Enter += new System.EventHandler(this.ms_Computer_Enter);
            //this.Leave += new System.EventHandler(this.ms_Computer_Leave);
            //this.Validated += new System.EventHandler(this.ms_Computer_Validated);

            //this.Text = this.Text + "s";
            PreparingComputerValues();

            // burada daha firmanın lisans adet kontrolu yapılacak
            // eğer sınır tamamlandıysa mevcut aktif computerlerin listesi gelecek 
            // bu isteden istediği bilgisayarın IsActivesini kapatıp yerine yeni computeri aktif edebilecek
            // tabiki bu kaydet butonuna basılınca gerçekleşecek
        }

        void PreparingComputerValues()
        {
            bool IsKart1 = true;
            TableIPCode = "UST/CRM/UstadComputers.Kart_F01";
            t.Find_DataSet(this, ref ds, ref dN, TableIPCode);

            if (ds == null)
            {
                TableIPCode = "UST/CRM/UstadComputers.Kart_F02";
                t.Find_DataSet(this, ref ds, ref dN, TableIPCode);
                IsKart1 = false;
            }


            if (t.IsNotNull(ds))
            {
                if (ds.Tables[0].Rows[dN.Position]["NetworkMacAddress"].ToString() == "")
                {
                    ds.Tables[0].Rows[dN.Position]["IsActive"] = 1;
                    ds.Tables[0].Rows[dN.Position]["FirmId"] = v.tMainFirm.FirmId;
                    ds.Tables[0].Rows[dN.Position]["FirmGUID"] = v.tUser.UserFirmGUID;
                    ds.Tables[0].Rows[dN.Position]["SystemName"] = v.tComputer.PcName?.ToString();
                    ds.Tables[0].Rows[dN.Position]["NetworkMacAddress"] = v.tComputer.Network_MACAddress?.ToString();
                    ds.Tables[0].Rows[dN.Position]["ProcessorName"] = v.tComputer.Processor_Name?.ToString();
                    ds.Tables[0].Rows[dN.Position]["ProcessorId"] = v.tComputer.Processor_Id?.ToString();
                    ds.Tables[0].Rows[dN.Position]["DiskModel"] = v.tComputer.DiskDrive_Model?.ToString();
                    ds.Tables[0].Rows[dN.Position]["DiskSerialNumber"] = v.tComputer.DiskDrive_SerialNumber?.ToString();
                    ds.Tables[0].Rows[dN.Position]["OperatingSystem"] = v.tComputer.OperatingSystem?.ToString();

                    Application.DoEvents();
                    // kaydı aç
                    ds.Tables[0].CaseSensitive = false;
                    dN.Tag = dN.Position;
                    NavigatorButton btnEnd = dN.Buttons.EndEdit;
                    dN.Buttons.DoClick(btnEnd);

                    if (IsKart1)
                        this.Close();
                }
            }
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
            v.tComp.SP_COMP_FIRM_GUID = ds.Tables[0].Rows[0]["FirmGUID"].ToString();

            /// compter yeni kayıt sırasında aktif kabul edilecek
            /// 
            v.tComp.SP_COMP_ISACTIVE = true;

            //MessageBox.Show("ms_Computer_FormClosing : "  + v.SP_COMP_FIRM_GUID);
        }

        private void ms_Computer_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MessageBox.Show("ms_Computer_FormClosed");
        }
    }
}
