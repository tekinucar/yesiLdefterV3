using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using FastReport.Export.Dbf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Tkn_EFaturaUyum;
using Tkn_Events;
using Tkn_Images;
using Tkn_InvoiceTasks;
using Tkn_Save;
using Tkn_ToolBox;
using Tkn_Variable;

namespace YesiLdefter
{
    public partial class ms_XMLRead : Form
    {
        #region Tanımlar
        tToolBox t = new tToolBox();
        tEvents ev = new tEvents();


        string menuName = "MENU_" + "UST/PMS/HUB/XMLRead";
        string buttonXMLRead = "ButtonXMLRead";
        string buttonXMLReadFields = "ButtonXMLReadFields";
        string buttonTabloOlustur = "ButtonTabloOlustur";
        string buttonDataTransferi = "ButtonDataTransferi";

        public class FieldInfo
        {
            public string FieldName { get; set; }
            public string FieldType { get; set; }
        }

        #endregion

        public ms_XMLRead()
        {
            InitializeComponent();

            tEventsForm evf = new tEventsForm();

            this.Load += new System.EventHandler(evf.myForm_Load);
            this.Shown += new System.EventHandler(evf.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_XMLRead_Shown);

            this.KeyPreview = true;

        }

        private void ms_XMLRead_Shown(object sender, EventArgs e)
        {
            t.Find_Button_AddClick(this, menuName, buttonXMLRead, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonXMLReadFields, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonTabloOlustur, myNavElementClick);
            t.Find_Button_AddClick(this, menuName, buttonDataTransferi, myNavElementClick);

            //t.Find_NavButton_Control(this, menuName, buttonManuelSave, ref buttonManuelSaveControl);
        }
        private void myNavElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.NavButton")
            {
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonXMLRead) XMLFileRead();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonXMLReadFields) readXMLFields();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonTabloOlustur) tableCreate();
                if (((DevExpress.XtraBars.Navigation.NavButton)sender).Name == buttonDataTransferi) dataTransferi();
            }
            // SubItem butonlar
            if (sender.GetType().ToString() == "DevExpress.XtraBars.Navigation.TileNavItem")
            {
                //if (((DevExpress.XtraBars.Navigation.TileNavItem)sender).Name == buttonMsfMsfIP) CopyMsfMsfIP();
            }
        }


        private XElement xmlFileRead(ref string fileName)
        {
            //Resim seçilir
            OpenFileDialog fdialog = new OpenFileDialog();
            fdialog.Filter = "XML|*.XML";
            string filePath = string.Empty;

            if (DialogResult.OK == fdialog.ShowDialog())
            {
                filePath = fdialog.FileName;   // Seçilen resme ait tam yol.

                try
                {
                    if (filePath.ToUpper().IndexOf(".XML") > -1)
                    {
                        System.Xml.Linq.XDocument xdoc = XDocument.Load(@"" + filePath);


                        // Her bir öğe grubunu (örnek: <Employee>, <Item> vs.) bul
                        // En üst düzeydeki alt elemanları alıyoruz
                        var root = xdoc.Root;
                        fileName = Path.GetFileName(filePath);

                        if (root == null)
                        {
                            MessageBox.Show("XML dosyası boş veya hatalı.");
                            return null;
                        }
                        else return root;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu:\n" + ex.Message);
                }
            }

            fdialog.Dispose();

            return null;
        }

        private void readXMLFields()
        {
            string fileName = "";
            var root = xmlFileRead(ref fileName);


            string fieldNameList = "";



            if (root != null)
            {
                string fieldName = "";
                string fieldType = "";
                // Root içindeki her bir child (örneğin <Employee>)
                foreach (var element in root.Elements())
                {
                    string itemInfo = $"⮞ Öğenin adı: {element.Name}\n";

                    // İçindeki her alt alanı (field) dolaş
                    foreach (var field in element.Elements())
                    {
                        fieldName = field.Name.LocalName;
                        if (fieldName.IndexOf("ID") > -1)
                            fieldType = "Int";
                        else if (fieldName.IndexOf("TARIH") > -1)
                            fieldType = "Date";
                        else
                            fieldType = "Varchar(100)";

                        itemInfo += $"    {fieldName.PadRight(30)} {fieldType.PadRight(15)}  null, \n";

                        fieldNameList += fieldName + ", ";
                    }

                    // Öğede alt eleman yoksa (örneğin doğrudan <Value>123</Value> gibi)
                    if (!element.Elements().Any())
                    {
                        itemInfo += $"   Değer: {element.Value}\n";
                    }

                    // Her item sonunda göster
                    //MessageBox.Show(itemInfo, "XML Öğesi");

                    break; // Sadece ilk öğeyi göster
                }
            }

            fieldNameList = fieldNameList.TrimEnd(' ');
            fieldNameList = fieldNameList.TrimEnd(',');

            test(root, fileName, fieldNameList);

        }

        private void XMLFileRead()
        {
            string fileName = "";
            var root = xmlFileRead(ref fileName);

            if (root != null)
            {
                // Root içindeki her bir child (örneğin <Employee>)
                foreach (var element in root.Elements())
                {
                    string itemInfo = $"⮞ Öğenin adı: {element.Name}\n";

                    // İçindeki her alt alanı (field) dolaş
                    foreach (var field in element.Elements())
                    {
                        string fieldName = field.Name.LocalName;
                        string fieldValue = field.Value;
                        itemInfo += $"   {fieldName} = {fieldValue}\n";
                    }

                    // Öğede alt eleman yoksa (örneğin doğrudan <Value>123</Value> gibi)
                    if (!element.Elements().Any())
                    {
                        itemInfo += $"   Değer: {element.Value}\n";
                    }

                    // Her item sonunda göster
                    MessageBox.Show(itemInfo, "XML Öğesi");

                    //break; // Sadece ilk öğeyi göster
                }
            }


            /*
            if (DialogResult.OK == fdialog.ShowDialog())
            {
                filePath = fdialog.FileName;   // Seçilen resme ait tam yol.
                
                try
                {
                    if (filePath.ToUpper().IndexOf(".XML") > -1)
                    {
                        System.Xml.Linq.XDocument xdoc = XDocument.Load(@"" + filePath);


                        // Her bir öğe grubunu (örnek: <Employee>, <Item> vs.) bul
                        // En üst düzeydeki alt elemanları alıyoruz
                        var root = xdoc.Root;

                        if (root == null)
                        {
                            MessageBox.Show("XML dosyası boş veya hatalı.");
                            return;
                        }

                        // Root içindeki her bir child (örneğin <Employee>)
                        foreach (var element in root.Elements())
                        {
                            string itemInfo = $"⮞ Öğenin adı: {element.Name}\n";

                            // İçindeki her alt alanı (field) dolaş
                            foreach (var field in element.Elements())
                            {
                                string fieldName = field.Name.LocalName;
                                string fieldValue = field.Value;
                                itemInfo += $"   {fieldName} = {fieldValue}\n";
                            }

                            // Öğede alt eleman yoksa (örneğin doğrudan <Value>123</Value> gibi)
                            if (!element.Elements().Any())
                            {
                                itemInfo += $"   Değer: {element.Value}\n";
                            }

                            // Her item sonunda göster
                            MessageBox.Show(itemInfo, "XML Öğesi");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu:\n" + ex.Message);
                }
            }

            fdialog.Dispose();
            */


        }


        private void test(XElement root, string tableName, string fieldNameList)
        {
            var records = root.Elements().ToList();

            if (records.Count == 0)
            {
                MessageBox.Show("XML içinde kayıt bulunamadı.");
                return;
            }

            //fieldNameList = "";
            string fieldNameList_ = "";

            foreach (var record in records)
            {
                // Her bir XML öğesinin alanlarını oku
                var fields = record.Elements().ToList();

                // Alan adlarını ve değerlerini topla
                var columnNames = string.Join(", ", fields.Select(f => f.Name.LocalName));
                var paramNames = string.Join(", ", fields.Select((f, i) => $"@p{i}"));

                string sql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({paramNames})";

                fieldNameList_ = "";// columnNames;// + "\n";

                //if (fieldNameList_.Length > 0 && fieldNameList == "")
                //    fieldNameList = fieldNameList_;

                for (int i = 0; i < fields.Count; i++)
                {
                    fieldNameList_ += ", " + (object)fields[i].Name.ToString() + " = " + (object)fields[i].Value.ToString();
                }


                /*
                if (fieldNameList_ != fieldNameList)
                {
                    MessageBox.Show("Alan isimleri uyuşmuyor:\n" + fieldNameList_ + "\n---\n" + fieldNameList);
                    return;
                }
                */

                MessageBox.Show(fieldNameList_);

                /*
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    for (int i = 0; i < fields.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@p{i}", (object)fields[i].Value ?? DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();
                }
                */
            }
        }


        private string getTableName(string fileName)
        {
            string tableName = Path.GetFileNameWithoutExtension(fileName);
            tableName = "XML_" + tableName.Substring(11);
            return tableName;
        }

        private void tableCreate()
        {
            string fileName = "";
            var root = xmlFileRead(ref fileName);
            string tableName = getTableName(fileName);

            //List<FieldInfo> fields = GetFieldListFromXml(root);
            List<FieldInfo> fields = GetAllFieldNamesAndTypes(root);

            string Sql = $@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '{tableName}')
                BEGIN
                    CREATE TABLE [{tableName}] (  RefId Int IDENTITY(1,1) not null, 
                        {string.Join(", ", fields.Select(f => $"[{f.FieldName}] {f.FieldType}"))}
                    );
                END";

            //MessageBox.Show(Sql);

            vTable vt = new vTable();
            vt.DBaseNo = v.dBaseNo.Project;
            vt.DBaseType = v.dBaseType.MSSQL;
            vt.DBaseName = v.active_DB.projectDBName;
            vt.msSqlConnection = v.active_DB.projectMSSQLConn;

            DataSet ds = new DataSet();

            t.Sql_ExecuteNon(ds, ref Sql, vt);

            ds.Dispose();

            t.AlertMessage("Tablo oluşturma işlemi tamamlandı.", "Bilgi");

        }

        private List<FieldInfo> GetFieldListFromXml(XElement root)
        {
            List<FieldInfo> fieldList = new List<FieldInfo>();

            //XDocument xdoc = XDocument.Load(xmlPath);
            //var root = xdoc.Root;

            if (root == null)
                throw new Exception("XML dosyası geçersiz veya boş.");

            var firstRecord = root.Elements().LastOrDefault();
            if (firstRecord == null)
                throw new Exception("XML içinde kayıt bulunamadı.");

            foreach (var field in firstRecord.Elements())
            {
                string name = field.Name.LocalName.ToUpperInvariant();
                string type;

                if (name.Contains("ID"))
                    type = "INT";
                else if (name.Contains("TARIH"))
                    type = "DATE";
                else
                    type = "VARCHAR(100)";

                fieldList.Add(new FieldInfo
                {
                    FieldName = field.Name.LocalName,
                    FieldType = type
                });
            }

            return fieldList;
        }

        private List<FieldInfo> GetAllFieldNamesAndTypes(XElement root)
        {
            List<FieldInfo> fieldList = new List<FieldInfo>();

            if (root == null)
                throw new Exception("XML dosyası geçersiz veya boş.");

            // Bütün kayıtların altındaki tüm field adlarını toplayacağız
            HashSet<string> uniqueFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Kayıt elemanlarını (örneğin <Customer> gibi) bul
            var allRecords = root.Elements();

            // Her kayıttaki tüm field'ları dolaş
            foreach (var record in allRecords)
            {
                foreach (var field in record.Elements())
                {
                    uniqueFields.Add(field.Name.LocalName);
                }
            }

            foreach (var fieldName in uniqueFields)
            {
                string upper = fieldName.ToUpperInvariant();
                string fieldType;

                if (upper.Contains("ID"))
                    fieldType = "INT";
                else if (upper.Contains("TARIH"))
                    fieldType = "VARCHAR(30)";//"DATE";
                else
                    fieldType = "VARCHAR(100)";

                // wentec için özel notlar alanı
                if (upper == "OZEL_NOTLAR") fieldType = "VARCHAR(255)";
                if (upper == "EV_ADRESI") fieldType = "VARCHAR(255)";
                if (upper == "IS_ADRESI") fieldType = "VARCHAR(255)";
                if (upper == "SARI_NOTLAR") fieldType = "VARCHAR(255)";
                if (upper == "ACIKLAMA") fieldType = "VARCHAR(255)";

                //result.Add((fieldName, fieldType));

                fieldList.Add(new FieldInfo
                {
                    FieldName = fieldName,
                    FieldType = fieldType
                });
            }

            return fieldList;
        }


        /// XML kayıtlarını tabloya ekler
        private void dataTransferi()
        {
            string fileName = "";
            var root = xmlFileRead(ref fileName);
            string tableName = getTableName(fileName);

            //List<FieldInfo> fields = GetFieldListFromXml(root);


            if (root == null)
            {
                MessageBox.Show("XML dosyası boş veya geçersiz.");
                return;
            }
            var records = root.Elements().ToList();
            if (records.Count == 0)
            {
                MessageBox.Show("XML içinde kayıt bulunamadı.");
                return;
            }

            string connectionString = v.active_DB.projectConnectionText;

            t.WaitFormOpen(this, "Veri aktarımı yapılıyor, lütfen bekleyiniz...");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (var record in records)
                {
                    var fields_ = record.Elements().ToList();
                    string columns = string.Join(", ", fields_.Select(f => $"[{f.Name.LocalName}]"));
                    string paramNames = string.Join(", ", fields_.Select((f, i) => $"@p{i}"));

                    string sql = $"INSERT INTO [{tableName}] ({columns}) VALUES ({paramNames})";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        for (int i = 0; i < fields_.Count; i++)
                        {
                            var value = fields_[i].Value;
                            cmd.Parameters.AddWithValue($"@p{i}", (object)value ?? DBNull.Value);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            t.AlertMessage("Veri aktarımı tamamlandı.", "Bilgi");
            v.SP_OpenApplication = false;
            v.IsWaitOpen = false;
            t.WaitFormClose();

        }

    }


}