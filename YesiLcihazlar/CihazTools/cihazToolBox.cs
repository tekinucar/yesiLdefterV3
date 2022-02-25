using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Drawing;
using System.Management;

namespace YesiLcihazlar
{
    public class tToolBox : tBase
    {
        #region Form View

        public void ChildForm_View(Form tForm, Form tMdiForm, FormWindowState state)
        {
            if (tMdiForm.IsMdiContainer)
                tForm.MdiParent = tMdiForm;

            tForm.WindowState = state;
            tForm.Show();
            //ScreenSize(tForm);
        }

        public void NormalForm_View(Form tForm, FormWindowState state)
        {
            tForm.StartPosition = FormStartPosition.CenterScreen;
            tForm.WindowState = state;
            tForm.Show();
        }


        public void DialogForm_View(Form tForm, FormWindowState state)
        {
            tForm.TopMost = true;
            tForm.StartPosition = FormStartPosition.CenterScreen;
            tForm.WindowState = state;
            tForm.ShowDialog();
        }

        #endregion Form View


        #region Db_Open

        /// <summary>
        /// MSSQL Bağlantısı
        /// </summary>
        public Boolean Db_Open(SqlConnection VTbaglanti)
        {

            #region Closed ise
            if (VTbaglanti.State == ConnectionState.Closed)
            {
                byte i = 0;

                //WaitFormOpen(v.mainForm, "[ MSSQL ] " + v.Wait_Desc_DBBaglanti);

                try
                {
                    VTbaglanti.Open();
                    v.Onay = true;
                    v.SP_ConnBool_Manager = true;

                    if (v.SP_OpenApplication == false)
                    {
                        //Thread.Sleep(500);
                        //SplashScreenManager.CloseForm(false);
                    }
                    else
                    {
                        //WaitFormOpen(v.mainForm, v.Wait_Desc_ProgramYukDevam);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("HATA : MSSQL Database bağlantısı açılmadı ... " + v.ENTER2 +
                        "*  Bunun çeşitli sebepleri olabilir." + v.ENTER2 +
                        "1. Database in bulunduğu bilgisayar ( SERVER ) kapalı olabilir ..." + v.ENTER +
                        "2. MSSQL Server kapalı olabilir ..." + v.ENTER +
                        "3. Network bağlantınızda sorun olabilir ..." + v.ENTER +
                        "4. Database bağlantı tanımlarında sorun olabilir ..." + v.ENTER2 +
                        "   Bu nedenle size sorulan soruları kontrol edin, yine olmaz ise yardım isteyin ..."
                        + v.ENTER2
                        + e.Message.ToString());

                    if (VTbaglanti == v.SP_Conn_Master_MSSQL) i = 1;
                    if (VTbaglanti == v.active_DB.managerMSSQLConn) i = 2;
                    if (VTbaglanti == v.active_DB.projectMSSQLConn) i = 4;

                    if (VTbaglanti == v.active_DB.firmMainMSSQLConn) i = 5;
                    if (VTbaglanti == v.active_DB.firmPeriodMSSQLConn) i = 6;

                    //Xml_.Database_Ondegerleri_Yenile();

                    try
                    {
                        if (i == 1) VTbaglanti = new SqlConnection(v.SP_Conn_Text_Master);
                        if (i == 3) VTbaglanti = new SqlConnection(v.SP_Conn_Text_MainManager);

                        if (i == 2) VTbaglanti = new SqlConnection(v.active_DB.managerConnectionText);
                        if (i == 4) VTbaglanti = new SqlConnection(v.active_DB.projectConnectionText);

                        if (i == 5) VTbaglanti = new SqlConnection(v.active_DB.firmMainConnectionText);
                        if (i == 6) VTbaglanti = new SqlConnection(v.active_DB.firmPeriodConnectionText);

                        // Burası OLMADI iyice araştır
                        VTbaglanti.Open();
                        v.Onay = true;
                        v.SP_ConnBool_Manager = true;
                    }
                    catch (Exception e2)
                    {
                        v.Onay = false;
                        v.SP_ConnBool_Manager = false;

                        MessageBox.Show(e2.Message.ToString());

                        Application.Exit();
                    }
                }
            }
            else
            {
                v.Onay = true;
            }
            #endregion Closed ise

            return v.Onay;
        }

        #endregion DB_Open

        #region Data_Read_Execute

        public void Preparing_DataSet(DataSet dsData, vTable vt)
        {
            // Gelen DataSet üzerinden gerekli bilgilere ulaşımaya çalışılacak;
            if (dsData != null)
            {
                string myProp = "";
                                
                if (IsNotNull(dsData.Namespace))
                    myProp = dsData.Namespace.ToString();

                Preparing_DataSet_(myProp, vt, dsData.Tables.Count);
            }
        }

        public void Preparing_DataSet_(string myProp, vTable vt, int tableCount)
        {
            if (IsNotNull(myProp) == false)
            {
                vt.TableName = "TABLE1";
                vt.TableCount = 255;
            }
            if (IsNotNull(myProp))
            {
                byte dbaseNo_ = Set(MyProperties_Get(myProp, "=DBaseNo:"), "", (byte)0);

                if (dbaseNo_ == 1) vt.DBaseNo = v.dBaseNo.Master;
                if (dbaseNo_ == 2) vt.DBaseNo = v.dBaseNo.Manager;
                if (dbaseNo_ == 4) vt.DBaseNo = v.dBaseNo.Project;
                if (dbaseNo_ == 5) vt.DBaseNo = v.dBaseNo.FirmMainDB;
                if (dbaseNo_ == 6) vt.DBaseNo = v.dBaseNo.FirmPeriodDB;

                vt.DBaseName = Find_dBLongName(vt.DBaseNo.ToString());
                vt.TableType = Set(MyProperties_Get(myProp, "=TableType:"), "", (byte)0);
                vt.TableName = Set(MyProperties_Get(myProp, "=TableName:"), "TABLE1", "TABLE1");
                vt.TableIPCode = Set(MyProperties_Get(myProp, "=TableIPCode:"), "", "");
                vt.KeyId_FName = Set(MyProperties_Get(myProp, "=KeyFName:"), "", "");
                vt.Cargo = Set(MyProperties_Get(myProp, "=Cargo:"), "", ""); // cargo = data, param, report

                string softwareCode = "";
                string projectCode = "";
                string TableCode = "";
                string IPCode = "";
                TableIPCode_Get(vt.TableIPCode, ref softwareCode, ref projectCode, ref TableCode, ref IPCode);
                vt.TableCode = TableCode;
                vt.IPCode = IPCode;
                vt.SoftwareCode = softwareCode;
                vt.ProjectCode = projectCode;
                vt.TableCount = (byte)tableCount; 

                if (myProp.IndexOf("Prop_Runtime:True") > 0)
                    vt.RunTime = true;
            }

            ///'TABLE_TYPE',  
            /// 1, 'Table'
            /// 2, 'View'
            /// 3, 'Stored Procedure'
            /// 4, 'Function'
            /// 5, 'Trigger'
            /// 6, 'Select'

            ///public enum dBName : byte
            ///    None,
            ///    Master,
            ///    Manager,
            ///    MainManager,
            ///    Project

            if (vt.DBaseNo == v.dBaseNo.Master)
            {
                vt.DBaseType = v.dBaseType.MSSQL;
                vt.msSqlConnection = v.SP_Conn_Master_MSSQL;
            }
            if (vt.DBaseNo == v.dBaseNo.Manager)
            {
                vt.DBaseType = v.dBaseType.MSSQL;
                vt.msSqlConnection = v.active_DB.managerMSSQLConn;
            }
            if ((vt.DBaseNo == v.dBaseNo.Project) && (v.active_DB.projectDBType == v.dBaseType.MSSQL))
            {
                vt.DBaseType = v.dBaseType.MSSQL;
                vt.msSqlConnection = v.active_DB.projectMSSQLConn;
            }
            if ((vt.DBaseNo == v.dBaseNo.Project) && (v.active_DB.projectDBType == v.dBaseType.MySQL))
            {
                vt.DBaseType = v.dBaseType.MySQL;
                vt.msSqlConnection = null;
            }
            // firma databaseleri 
            if ((vt.DBaseNo == v.dBaseNo.FirmMainDB) && (v.active_DB.firmMainDBType == v.dBaseType.MSSQL))
            {
                //WaitFormOpen(v.mainForm, "Firma MainDB MSSQL Connection...");
                Db_Open(v.active_DB.firmMainMSSQLConn);
                v.IsWaitOpen = false;
                //WaitFormClose();

                vt.DBaseType = v.dBaseType.MSSQL;
                vt.msSqlConnection = v.active_DB.firmMainMSSQLConn;
            }
            if ((vt.DBaseNo == v.dBaseNo.FirmPeriodDB) && (v.active_DB.firmPeriodDBType == v.dBaseType.MSSQL))
            {
                //WaitFormOpen(v.mainForm, "Firma Period MSSQL Connection...");
                Db_Open(v.active_DB.firmPeriodMSSQLConn);
                v.IsWaitOpen = false;
                //WaitFormClose();

                vt.DBaseType = v.dBaseType.MSSQL;
                vt.msSqlConnection = v.active_DB.firmPeriodMSSQLConn;
                //vt.mySqlConnection = null;
            } 
        }

        private void Preparing_TableFields(DataSet dsData, vTable vt)
        {
            //if (vt.TableName.IndexOf("SNL_") > -1) return;
            //if (vt.TableName.IndexOf("MS_TABLES") > -1)
            //    MessageBox.Show("");

            //if (dsData.Tables.Count > 1) return;
            if (dsData.Tables.Count != 1) return;

            if ((dsData.Tables[0].Rows.Count == 1) &&
                (dsData.Tables[0].Columns.Count == 1) &&
                (dsData.Tables[0].Columns[0].Caption == "@@VERSION")) return;

            // zaten bu koşul yoksa buraya gelmiyor
            // if (vt.Cargo != "data") return;

            // gelen DataSet üzerine yüklenen Table ve StoredProcedures değilse geri dönsün  
            if ((vt.TableType != 1) && // table
                (vt.TableType != 3) && // storedprocedure
                (vt.TableType != 6)    // select
                ) return;

            string fields = "_FIELDS";
            string sqlA = string.Empty;
            string sqlB = string.Empty;

            Preparing_TableFields_SQL(vt, ref sqlA, ref sqlB);

            if (vt.DBaseType == v.dBaseType.MSSQL)
            {
                SqlDataAdapter msSqlAdapter = null;

                if (sqlA != string.Empty)
                {
                    msSqlAdapter = new SqlDataAdapter(sqlA, vt.msSqlConnection);
                    msSqlAdapter.Fill(dsData, vt.TableName + fields);
                }

                if (sqlB != string.Empty)
                {
                    //= TableIPCode:3S_MSTBL.3S_MSTBL_02;

                    //if (vt.TableIPCode.IndexOf("3S_") == -1)
                    //    msSqlAdapter = new SqlDataAdapter(sqlB, v.SP_Conn_Manager_MSSQL);
                    //else
                    //    msSqlAdapter = new SqlDataAdapter(sqlB, v.SP_Conn_MainManager_MSSQL);

                    msSqlAdapter = new SqlDataAdapter(sqlB, v.active_DB.managerMSSQLConn);

                    msSqlAdapter.Fill(dsData, vt.TableName + fields + "2");

                }

                msSqlAdapter.Dispose();
            }

            /*
            if (vt.DBaseType == v.dBaseType.MySQL)
            {
                MySqlDataAdapter mySqlAdapter = null;

                if (sqlA != string.Empty)
                {
                    mySqlAdapter = new MySqlDataAdapter(sqlA, vt.mySqlConnection);
                    mySqlAdapter.Fill(dsData, vt.TableName + fields);
                }

                /// database ler şimdilik her halükarda MSSQL üzerinde olduğu için
                /// MSSQL den okunayor mecburen
                /// MSV3DFTR veya SystemMS 
                if (sqlB != string.Empty)
                {
                    //= TableIPCode:3S_MSTBL.3S_MSTBL_02;
                    SqlDataAdapter msSqlAdapter = null;

                    //if (vt.TableIPCode.IndexOf("3S_") == -1)
                    //    msSqlAdapter = new SqlDataAdapter(sqlB, v.SP_Conn_Manager_MSSQL);
                    //else msSqlAdapter = new SqlDataAdapter(sqlB, v.SP_Conn_MainManager_MSSQL);



                    msSqlAdapter = new SqlDataAdapter(sqlB, v.active_DB.managerMSSQLConn);

                    msSqlAdapter.Fill(dsData, vt.TableName + fields + "2");

                    msSqlAdapter.Dispose();
                }
                mySqlAdapter.Dispose();
            }*/
        }

        private void Preparing_TableFields_SQL(vTable vt,
               ref string sqlA, ref string sqlB)
        {
            if (vt.DBaseType == v.dBaseType.MSSQL)
                sqlA =
            @" Select  
               a.column_id, a.name 
             , convert(smallInt, a.system_type_id) system_type_id 
             , convert(smallInt, a.user_type_id)   user_type_id 
             , a.max_length, a.precision, a.scale, a.is_nullable, a.is_identity  
             from sys.columns a 
               left outer join sys.tables b on (a.object_id = b.object_id ) 
             where b.name = '" + vt.TableName + @"' 
             order by a.column_id ";

            if (vt.DBaseType == v.dBaseType.MySQL)
                sqlA =
            @" Select COLUMN_NAME, ORDINAL_POSITION, DATA_TYPE, COLUMN_TYPE, EXTRA 
               From INFORMATION_SCHEMA.COLUMNS
               Where table_name = '" + vt.TableName + "' ";

            /// FFOREING Bit                 NULL,
            /// FTRIGGER Bit                 NULL,
            /// PROP_EXPRESSION VarChar(4000)          NULL,              
            /// VALIDATION_INSERT Bit                NULL,
            /// XML_FIELD_NAME VarChar(30)            NULL,
            /// CMP_DISPLAY_FORMAT VarChar(50) 		NULL,

            sqlB =
            @" select  
               d.FIELD_NO
             , d.FIELD_NAME
             , d.FFOREING 
             , d.FTRIGGER 
             , d.PROP_EXPRESSION 
             , e.VALIDATION_INSERT
             , e.XML_FIELD_NAME
             , e.CMP_DISPLAY_FORMAT

             from dbo.MS_TABLES c 
               left outer join dbo.MS_FIELDS d on ( c.TABLE_CODE = d.TABLE_CODE 
                    and c.SOFTWARE_CODE = d.SOFTWARE_CODE
                    and c.PROJECT_CODE = d.PROJECT_CODE
                 )
               left outer join dbo.MS_FIELDS_IP e on (
                     d.TABLE_CODE = e.TABLE_CODE 
                 and d.SOFTWARE_CODE = e.SOFTWARE_CODE
                 and d.PROJECT_CODE = e.PROJECT_CODE
                 and d.FIELD_NO  = e.FIELD_NO
                 and e.IP_CODE   = '" + vt.IPCode + @"' )  
             where c.TABLE_NAME  = '" + vt.TableName + @"' 
             and c.SOFTWARE_CODE = '" + vt.SoftwareCode + @"'
             and c.PROJECT_CODE  = '" + vt.ProjectCode + @"'
             order by d.FIELD_NO ";

            if ((vt.TableName.IndexOf("_KIST") > -1) ||
                (vt.TableName.IndexOf("_FULL") > -1))
                MessageBox.Show("Preparing_Fields_SQL : Table_Name.IndexOf(_KIST) / (_FULL) : konusu vardı.");
        }


        public Boolean Sql_ExecuteNon(SqlCommand SqlComm, vTable vt)
        {
            Boolean sonuc = false;

            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            SqlConnection msSqlConn = vt.msSqlConnection;

            Db_Open(msSqlConn);

            try
            {
                SqlComm.Connection = msSqlConn;
                SqlComm.ExecuteNonQuery();
                sonuc = true;
            }
            catch (Exception e)
            {
                sonuc = false;
                MessageBox.Show("HATALI İŞLEM : " + v.ENTER2 + e.Message, "Sql_ExecuteNon [ " + vt.TableName + " ]");
            }

            if (sonuc == true)
            {
                try
                {
                    SqlComm.ExecuteNonQuery().ToString();
                }
                catch
                {
                    // 
                }
            }

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            return sonuc;
        }


        public Boolean Sql_ExecuteNon(DataSet dsData, ref string SQL, vTable vt)
        {
            Boolean sonuc = false;

            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            SqlConnection msSqlConn = vt.msSqlConnection;
            //MySqlConnection mySqlConn = vt.mySqlConnection;

            if (vt.DBaseType == v.dBaseType.MSSQL)
                Db_Open(msSqlConn);
            //if (vt.DBaseType == v.dBaseType.MySQL)
            //    Db_Open(mySqlConn);

            SqlCommand SqlComm = null;
            //MySqlCommand MySqlComm = null;

            try
            {
                SQL = SQLPreparing(SQL, vt);

                if (vt.DBaseType == v.dBaseType.MSSQL)
                {
                    SqlComm = new SqlCommand(SQL, msSqlConn);
                    SqlComm.ExecuteNonQuery();
                }
                //if (vt.DBaseType == v.dBaseType.MySQL)
                //{
                //    MySqlComm = new MySqlCommand(SQL, mySqlConn);
                //    MySqlComm.ExecuteNonQuery();
                //}
                sonuc = true;
            }
            catch (Exception e)
            {
                sonuc = false;
                MessageBox.Show("HATALI İŞLEM : " + v.ENTER2 + e.Message, "Sql_ExecuteNon [ " + vt.TableName + " ]");
            }

            if (sonuc == true)
            {
                try
                {
                    string Adet = "";

                    if (vt.DBaseType == v.dBaseType.MSSQL)
                        SqlComm.ExecuteNonQuery().ToString();

                    //if (vt.DBaseType == v.dBaseType.MySQL)
                    //    MySqlComm.ExecuteNonQuery().ToString();

                    if ((Adet != "-1") && (Adet != "0"))
                        v.Kullaniciya_Mesaj_Var = Adet + " adet kayıt üzerinde işlem gerçekleşti ...";
                }
                catch
                {
                    // 
                }
            }

            if (vt.DBaseType == v.dBaseType.MSSQL)
                SqlComm.Dispose();

            //if (vt.DBaseType == v.dBaseType.MySQL)
            //    MySqlComm.Dispose();

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            return sonuc;
        }

        private Boolean Sql_Execute(DataSet dsData, ref string SQL, vTable vt)
        {
            Boolean onay = false;

            SqlConnection msSqlConn = vt.msSqlConnection;
            //MySqlConnection mySqlConn = vt.mySqlConnection;

            if (vt.DBaseType == v.dBaseType.MSSQL)
                Db_Open(msSqlConn);
            //if (vt.DBaseType == v.dBaseType.MySQL)
            //    Db_Open(mySqlConn);

            SqlDataAdapter msSqlAdapter = null;
            //MySqlDataAdapter mySqlAdapter = null;

            #region 1. adımda DATA dolduruluyor
            try
            {
                if ((SQL.IndexOf("MS_") == -1) &&
                    (SQL.IndexOf("3S_") == -1))
                    v.SQL = v.ENTER2 + SQL + v.SQL;

                /// sql execute oluyor
                if (vt.DBaseType == v.dBaseType.MSSQL)
                    msSqlAdapter = new SqlDataAdapter(SQL, msSqlConn);
                //if (vt.DBaseType == v.dBaseType.MySQL)
                //    mySqlAdapter = new MySqlDataAdapter(SQL, mySqlConn);

                if (vt.Cargo == "data")
                {
                    if (vt.TableCount > 0)
                    {
                        bool uValue = v.con_PositionChange;
                        v.con_PositionChange = true;
                        dsData.Tables[0].Clear();
                        dsData.Tables[0].Columns.Clear();
                        v.con_PositionChange = uValue;
                    }
                    else dsData.Clear();
                }

                /// data dolduruluyor
                if (vt.DBaseType == v.dBaseType.MSSQL)
                    msSqlAdapter.Fill(dsData, vt.TableName);
                //if (vt.DBaseType == v.dBaseType.MySQL)
                //    mySqlAdapter.Fill(dsData, vt.TableName);

                if ((vt.Cargo == "data") &&
                    (SQL.IndexOf("Select @@VERSION") == -1))
                {
                    tSqlSecond_Set(ref dsData, SQL);
                }

                /// bunu daha sonra parametreye bağla 
                /// 
                if ((vt.TableName.IndexOf("GROUPS") == -1) &&
                    (vt.TableName.IndexOf("MS_FIELDS_IP") == -1) &&
                    (vt.TableName.IndexOf("MS_TABLES_IP") == -1))
                {
                    //v.SQL =
                    //    v.ENTER2 + "[ Data_Read_Execute : " +
                    //    vt.TableName + " / " + vt.TableIPCode + " / " + vt.functionName +
                    //    " ] { " + v.ENTER2 + SQL + v.ENTER2 + " } " + v.SQL;
                }

                onay = true;

            }
            catch (Exception e)
            {
                /// uzak bağlantıda ilk insert denemesinde hata veriyor
                /// o hata oluştursa aynı sql bir daha çalışınca
                /// işlem bu sefer gerçekleşiyor
                /// bu geçici çözümdür
                ///

                //if (vt.DBaseType == v.dBaseType.MySQL)
                //{
                //    if (mySqlConn.State == System.Data.ConnectionState.Closed)
                //    {
                //        v.SP_ConnBool_Project = false;
                //        v.Kullaniciya_Mesaj_Var = "DİKKAT : MySQL Database bağlantısı koptu...";
                //    }
                //}

                if ((vt.DBaseType == v.dBaseType.MySQL) &&
                    (e.Message == "Fatal error encountered during command execution."))
                {
                    //Db_Open(mySqlConn);
                    //mySqlAdapter.Fill(dsData, vt.TableName);

                    //v.SQL =
                    //    v.ENTER2 + "[ 2. Data_Read_Execute : " +
                    //    vt.TableName + " / " + vt.TableIPCode + " / " + vt.functionName +
                    //    " ] { " + v.ENTER2 + SQL + v.ENTER2 + " } " + v.SQL;
                }
                else
                {
                    onay = false;
                    Cursor.Current = Cursors.Default;

                    v.SQL = v.ENTER2 +
                        "   HATALI SQL :  [ " + e.Message.ToString() + v.ENTER2 +
                        vt.TableName + " / " + vt.TableIPCode + " / " + vt.functionName +
                        " ] { " + v.ENTER2 + SQL + v.ENTER2 + " } " + v.SQL;

                    //MessageBox.Show("HATALI SQL : " + v.ENTER2 + e.Message.ToString() +
                    //    v.ENTER2 + vt.TableName + v.ENTER2 + SQL, "Data_Read_Execute");
                }
            }
            #endregion 1.adım

            if (msSqlAdapter != null) msSqlAdapter.Dispose();
            //if (mySqlAdapter != null) mySqlAdapter.Dispose();

            return onay;
        }

        public Boolean Data_Read_Execute(DataSet dsData,
                                         ref string SQL, string TableName,
                                         Control cntrl)
        {
            Boolean onay = false;

            // -99 var ise SQL çalışmasın 
            int Eksi99 = SQL.IndexOf("-99");

            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            // Gerekli olan verileri topla
            vTable vt = new vTable();
            Preparing_DataSet(dsData, vt);

            if ((SQL != "") &&
                (Eksi99 == -1))
            {
                SQL = SQLPreparing(SQL, vt);

                // 1. adım
                onay = Sql_Execute(dsData, ref SQL, vt);

            }

            #region NotExecute
            if ((SQL != "") &&
                (Eksi99 > -1))
            {
                SQL = SQLPreparing(SQL, vt);

                tSqlSecond_Set(ref dsData, SQL);

                if ((vt.TableName.IndexOf("SNL_") > -1) ||
                    (vt.TableName.IndexOf("3S_") > -1)
                    || (vt.TableType == 3) // stored procedure 
                    )
                    SQL = "Select @@VERSION ";

                // 1. adım
                onay = Sql_Execute(dsData, ref SQL, vt);
                if (IsNotNull(dsData))
                    SQL = dsData.Tables[0].Rows.Count.ToString() + " adet kayıt ... [NotExecute]";

            }
            #endregion NotExecute

            #region 2. adımda Tablonun Fields bilgileri geliyor
            /// tablo ilk defa okunuyorsa ve
            /// data tablosuyla tablonun fields listesi hazırlanıyor 
            if ((vt.TableCount == 0) &&
                (vt.Cargo == "data"))
            {
                Preparing_TableFields(dsData, vt);
            }
            #endregion 2. adım

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            return onay;
        }

        #endregion Data_Read_Execute

        #region SQL_Read_Execute
        public Boolean SQL_Read_Execute(v.dBaseNo dBNo, DataSet dsData,
               ref string SQL, string tableName, string function_name)
        {
            Boolean onay = false;
            string TableIPCode = string.Empty;

            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            // Gerekli olan verileri topla
            vTable vt = new vTable();

            vt.functionName = function_name;

            vt.DBaseNo = dBNo;
            //if (v.dBaseNo.Master == dBNo)
            //    vt.DBaseNo = (byte)1;
            //if (v.dBaseNo.Manager == dBNo)
            //    vt.DBaseNo = (byte)2;
            //if (v.dBaseNo.MainManager == dBNo)
            //    vt.DBaseNo = (byte)3;
            //if (v.dBaseNo.Project == dBNo)
            //    vt.DBaseNo = (byte)4;

            Preparing_DataSet(dsData, vt);

            if (SQL != "")
            {

                if ((vt.TableName == "TABLE1") &&
                    (tableName != ""))
                    vt.TableName = tableName;

                SQL = SQLPreparing(SQL, vt);

                if ((tableName != "GROUPS") &&
                    (SQL.IndexOf("[Lkp]") == -1))
                {
                    TableRemove(dsData);
                }

                // 1. adım
                onay = Sql_Execute(dsData, ref SQL, vt);

                tSqlSecond_Set(ref dsData, SQL);

                if (IsNotNull(vt.TableIPCode))
                    dsData.DataSetName = vt.TableIPCode;

            }


            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            return onay;



            /*
            SqlConnection conn = new SqlConnection(sqlConnectionString); 
            Server server = new Server(new ServerConnection(conn)); 
            server.ConnectionContext.ExecuteNonQuery(script);
            */
            /*
            conn = new SqlConnection("Server=(local);DataBase=master;Integrated Security=SSPI"); 
            conn.Open(); 
            SqlCommand cmd = new SqlCommand("dbo.test", conn);
            cmd.CommandType = CommandType.Text;
            rdr = cmd.ExecuteReader(); 
            */


        }
        #endregion SQL_Read_Execute

        #region TriggerOnOff

        //DISABLE TRIGGER Person.uAddress ON Person.Address;
        //ENABLE Trigger Person.uAddress ON Person.Address;
        public bool tTriggerOnOff(string TableName, string TriggerName, v.tEnabled enabled)
        {
            Boolean onay = false;
            try
            {
                SqlCommand cmd = new SqlCommand(@"@_Enabled TRIGGER @_TriggerName ON @_TableName; ");

                if (enabled == v.tEnabled.Enable)
                     cmd.Parameters.AddWithValue("@_Enabled", "ENABLE");
                else cmd.Parameters.AddWithValue("@_Enabled", "DISABLE");

                cmd.Parameters.AddWithValue("@_TriggerName", TriggerName);
                cmd.Parameters.AddWithValue("@_TableName", TableName);

                string myProp = string.Empty;
                MyProperties_Set(ref myProp, "DBaseNo", "4");
                MyProperties_Set(ref myProp, "TableName", TableName);
                MyProperties_Set(ref myProp, "SqlFirst", "null");
                MyProperties_Set(ref myProp, "SqlSecond", "null");

                vTable vt = new vTable();
                Preparing_DataSet_(myProp, vt, 0);
                Sql_ExecuteNon(cmd, vt);
                onay = true;
                cmd.Dispose();
            }
            catch (SqlException se)
            {
                throw se;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return onay;
        }
        
        #endregion TriggerOnOff


        #region IsNotNull
        public Boolean IsNotNull(string Veri)
        {
            if ((Veri != "") &&
                (Veri != "null") &&
                (Veri != null) &&
                (Veri != string.Empty))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsNotNull(DataSet dsData)
        {
            if (dsData != null)
            {
                if (dsData.Tables.Count > 0)
                {

                    if (dsData.Tables[0].Rows.Count > 0)
                    {
                        //if ((dsData.Tables[0].Rows.Count == 1) &&
                        //(dsData.Tables[0].Columns.Count == 1) &&
                        //(dsData.Tables[0].Columns[0].Caption == "@@VERSION")) return false;

                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }

        #endregion IsNotNull
        
        #region Set

        public byte Set(string Value1, string Value2, byte Default)
        {
            // Hangi değer atanır?, Öncelik sırası nedir ?
            // 1. Value1
            // 2. Value2
            // 3. Default  atanır

            // Value1 boş değilse
            if ((Value1 != null) && (Value1 != ""))
            {
                try
                {
                    return Convert.ToByte(Value1);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            if (((Value1 == null) || (Value1 == "")) &&
                ((Value2 != null) && (Value2 != "")))
            {
                try
                {
                    return Convert.ToByte(Value2);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            return Default;
        }

        public Boolean Set(string Value1, string Value2, Boolean Default)
        {
            // Hangi değer atanır?, Öncelik sırası nedir ?
            // 1. Value1
            // 2. Value2
            // 3. Default  atanır

            // Value1 boş değilse
            if ((Value1 != null) && (Value1 != ""))
            {
                if ((Value1 == "True") || (Value1 == "1"))
                    return true;
                if ((Value1 == "False") || (Value1 == "0"))
                    return false;
            }

            if (((Value1 == null) || (Value1 == "")) &&
                ((Value2 != null) && (Value2 != "")))
            {
                if ((Value2 == "True") || (Value2 == "1"))
                    return true;
                if ((Value2 == "False") || (Value2 == "0"))
                    return false;
            }

            return Default;
        }

        public Int16 Set(string Value1, string Value2, Int16 Default)
        {
            // Hangi değer atanır?, Öncelik sırası nedir ?
            // 1. Value1
            // 2. Value2
            // 3. Default  atanır

            // Value1 boş değilse
            if ((Value1 != null) && (Value1 != ""))
            {
                try
                {
                    return Convert.ToInt16(Value1);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            if (((Value1 == null) || (Value1 == "")) &&
                ((Value2 != null) && (Value2 != "")))
            {
                try
                {
                    return Convert.ToInt16(Value2);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            return Default;
        }

        public int Set(string Value1, string Value2, int Default)
        {
            // Hangi değer atanır?, Öncelik sırası nedir ?
            // 1. Value1
            // 2. Value2
            // 3. Default  atanır

            // Value1 boş değilse
            if ((Value1 != null) && (Value1 != ""))
            {
                try
                {
                    return Convert.ToInt32(Value1);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            if (((Value1 == null) || (Value1 == "")) &&
                ((Value2 != null) && (Value2 != "")))
            {
                try
                {
                    return Convert.ToInt32(Value2);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            return Default;
        }

        public float Set(string Value1, string Value2, float Default)
        {
            // Hangi değer atanır?, Öncelik sırası nedir ?
            // 1. Value1
            // 2. Value2
            // 3. Default  atanır

            // Value1 boş değilse
            if ((Value1 != null) && (Value1 != ""))
            {
                try
                {
                    return Convert.ToSingle(Value1);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            if (((Value1 == null) || (Value1 == "")) &&
                ((Value2 != null) && (Value2 != "")))
            {
                try
                {
                    return Convert.ToSingle(Value2);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            return Default;
        }

        public double Set(string Value1, string Value2, double Default)
        {
            // Hangi değer atanır?, Öncelik sırası nedir ?
            // 1. Value1
            // 2. Value2
            // 3. Default  atanır

            // Value1 boş değilse
            if ((Value1 != null) && (Value1 != ""))
            {
                try
                {
                    return Convert.ToDouble(Value1);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            if (((Value1 == null) || (Value1 == "")) &&
                ((Value2 != null) && (Value2 != "")))
            {
                try
                {
                    return Convert.ToDouble(Value2);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            return Default;
        }

        public decimal Set(string Value1, string Value2, decimal Default)
        {
            // Hangi değer atanır?, Öncelik sırası nedir ?
            // 1. Value1
            // 2. Value2
            // 3. Default  atanır

            // Value1 boş değilse
            if ((Value1 != null) && (Value1 != ""))
            {
                try
                {
                    return Convert.ToDecimal(Value1);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            if (((Value1 == null) || (Value1 == "")) &&
                ((Value2 != null) && (Value2 != "")))
            {
                try
                {
                    return Convert.ToDecimal(Value2);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            return Default;
        }

        public string Set(string Value1, string Value2, string Default)
        {
            // Hangi değer atanır?, Öncelik sırası nedir ?
            // 1. Value1
            // 2. Value2
            // 3. Default  atanır

            // Value1 boş değilse
            if ((Value1 != null) && (Value1 != "") && (Value1 != "null"))
            {
                return Value1;
            }

            if (((Value1 == null) || (Value1 == "") || (Value1 == "null")) &&
                ((Value2 != null) && (Value2 != "") && (Value2 != "null")))
            {
                return Value2;
            }

            return Default;
        }

        public DateTime Set(string Value1, string Value2, DateTime Default)
        {
            // Hangi değer atanır?, Öncelik sırası nedir ?
            // 1. Value1
            // 2. Value2
            // 3. Default  atanır

            // Value1 boş değilse
            if ((Value1 != null) && (Value1 != ""))
            {
                try
                {
                    return Convert.ToDateTime(Value1);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            if (((Value1 == null) || (Value1 == "")) &&
                ((Value2 != null) && (Value2 != "")))
            {
                try
                {
                    return Convert.ToDateTime(Value2);
                }
                catch (Exception)
                {
                    return Default;
                    throw;
                }
            }

            return Default;
        }

        #endregion Set 
        
        #region *MyProperties

        public void MyProperties_Set(ref string tProperties, ref string tProCaption, string object_name, string Caption, string Value)
        {
            if (Value == string.Empty) Value = "null";

            if ((Value != "BEGIN") && (Value != "END"))
            {
                tProperties = tProperties + object_name + "=" + Caption + ":" + Value + ";" + v.ENTER;
                tProCaption = tProCaption + object_name + "=" + Caption + ":";
            }

            if ((Value == "BEGIN") || (Value == "END"))
            {
                tProperties = tProperties + object_name + "=" + Value + ":" + v.ENTER;
            }
        }

        public void MyProperties_Set(ref string tProperties, string object_name, string Caption, string Value)
        {
            if (Value == string.Empty) Value = "null";

            if ((Value != "BEGIN") && (Value != "END"))
            {
                tProperties = tProperties + object_name + "=" + Caption + ":" + Value + ";" + v.ENTER;
            }
            if ((Value == "BEGIN") || (Value == "END"))
            {
                tProperties = tProperties + object_name + "=" + Value + ":" + object_name + ";" + v.ENTER;
            }
        }

        public void MyProperties_Set(ref string tProperties, string Caption, string Value)
        {
            if (Value == string.Empty) Value = "null";

            if ((Value != "BEGIN") && (Value != "END"))
            {
                tProperties = tProperties + "=" + Caption + ":" + Value + ";" + v.ENTER;
            }
            if ((Value == "BEGIN") || (Value == "END"))
            {
                tProperties = tProperties + "=" + Value + ":" + v.ENTER;
            }
        }

        public string MyProperties_Get(string tProperties, string Caption)
        {
            string MyValue = string.Empty;
            if (IsNotNull(tProperties) == false) return MyValue;
            int i1 = 0;

            i1 = tProperties.IndexOf(Caption);
            if (i1 > -1)
            {
                i1 = i1 + Caption.Length;
                MyValue = tProperties.Remove(0, i1);
                i1 = MyValue.IndexOf(";");
                if (i1 > -1)
                    MyValue = MyValue.Substring(0, i1);
                else
                {
                    // yoksa " çift tırnak ara
                    i1 = MyValue.IndexOf((char)34);
                    if (i1 > -1)
                        MyValue = MyValue.Substring(0, i1);
                }
            }

            if (MyValue == "null") MyValue = string.Empty;

            return MyValue;

            /*
                *   tBarButtonItem_84=Caption:button 3;
                *   tBarButtonItem_84=TableIPCode:22;
                *   tBarButtonItem_84=MasterTableIPCode1:TABLE_1.t11;
                *   tBarButtonItem_84=MasterFieldName11:F1;
                *   tBarButtonItem_84=MasterFieldName12:F2;
                *   tBarButtonItem_84=ParentTableIPCode1:TABLE_1.t11;
                *   tBarButtonItem_84=ParentFieldName11:F3;
                *   tBarButtonItem_84=ParentFieldName12:F4;
                *   tBarButtonItem_84=ParentTableIPCode2:TABLE_1.t11;
                *   tBarButtonItem_84=ParentFieldName21:F5;
                *   tBarButtonItem_84=ParentFieldName22:F6;
                *   tBarButtonItem_84=END:
                * 
                */
        }

        #endregion MyProperties
        
        #region tSqlSecond_Set
        public void tSqlSecond_Set(ref DataSet dsData, string NewSql)
        {
            if (dsData != null)
                if (dsData.Namespace != null)
                {
                    string myProp = dsData.Namespace.ToString();
                    if (IsNotNull(myProp))
                    {
                        string OldSql = MyProperties_Get(myProp, "=SqlSecond:");

                        if (OldSql != NewSql)
                        {
                            if (OldSql == "")
                                OldSql = "=SqlSecond:null;";
                            else OldSql = "=SqlSecond:" + OldSql + ";";

                            NewSql = "=SqlSecond:" + NewSql + ";";

                            Str_Replace(ref myProp, OldSql, NewSql);
                            dsData.Namespace = myProp;
                        }
                    }
                }
        }
        #endregion tSqlSecond_Set

        #region SQLPreparing

        public bool DeclarePreparingField(string cumle, string value, int ffieldtype)
        {
            bool onay = false;

            if ((cumle.IndexOf("{GUN}") > -1) &&
                (cumle.IndexOf("{gun}") > -1))
            {
                onay = true;
                v.Declare_Gun = tData_Convert(value, ffieldtype);
                return onay;
            }
            if ((cumle.IndexOf("{AY}") > -1) &&
                (cumle.IndexOf("{ay}") > -1))
            {
                onay = true;
                v.Declare_Ay = tData_Convert(value, ffieldtype);
                return onay;
            }
            if ((cumle.IndexOf("{YIL}") > -1) &&
                (cumle.IndexOf("{yil}") > -1))
            {
                onay = true;
                v.Declare_Yil = tData_Convert(value, ffieldtype);
                return onay;
            }

            if (cumle.IndexOf("{bugun}") > -1)
            {
                onay = true;
                v.Declare_bugun = tData_Convert(value, ffieldtype);
                return onay;
            }

            return onay;
        }

        public string SQLPreparing(string Sql, vTable vt)
        {

            // SQL içindeki " işaretleri ' ile değiştirilir
            //Sql = Sql.ToUpper();
            Sql = Str_AntiCheck(Sql);

            // Yukarıda toUpper yüzüunden join > JOİN şekline dönüşüyor
            //Str_Replace(ref Sql, "JOİN", "JOIN");
            //Str_Replace(ref Sql, "İSNULL", "ISNULL");
            //Str_Replace(ref Sql, "UNİON", "UNION");

            //if (vt.TableIPCode == "SYSUSER.SYSUSER_L02")
            //{
            //    v.Kullaniciya_Mesaj_Var = vt.TableIPCode;
            //}

            if (vt.DBaseType == v.dBaseType.MySQL)
            {
                //Sql = Sql.ToLower();
                Str_Replace(ref Sql, "begin transaction", "");
                Str_Replace(ref Sql, "commit transaction", "");

                // ID  Int IDENTITY(1,1) NOT NULL,
                //`ID` INTEGER NOT NULL AUTO_INCREMENT,

                Str_Replace(ref Sql, "Int IDENTITY(1,1) NOT NULL", "int not null auto_increment");

                // ENGINE = InnoDB
                // AUTO_INCREMENT = 1
                // CHARACTER SET latin5 COLLATE latin5_turkish_ci
                // ROW_FORMAT = DYNAMIC;

                Str_Replace(ref Sql, "-- mysql_add",
                    @" engine = innoDB  
  auto_increment = 1  
  character set latin5 collate latin5_turkish_ci 
  row_format = dynamic; ");

                //\r\n grant
                Str_Replace(ref Sql, "grant", "-- grant");

                //uniqueidentifier
                Str_Replace(ref Sql, "uniqueidentifier", "varchar(50)");

                Str_Replace(ref Sql, "smalldatetime", "datetime");
                Str_Replace(ref Sql, "SmallDatetime", "datetime");
                Str_Replace(ref Sql, "SmallDateTime", "datetime");
                Str_Replace(ref Sql, "SMALLDATETIME", "datetime");

            }

            Str_Replace(ref Sql, ":VT_FIRM_ID", v.SP_FIRM_ID.ToString());
            Str_Replace(ref Sql, ":FIRM_ID", v.SP_FIRM_ID.ToString());
            Str_Replace(ref Sql, ":FIRM_USERLIST", v.SP_FIRM_USERLIST);
            Str_Replace(ref Sql, ":FIRM_USER_LIST", v.SP_FIRM_USERLIST);
            Str_Replace(ref Sql, ":FIRM_FULLLIST", v.SP_FIRM_FULLLIST);
            Str_Replace(ref Sql, ":FIRM_FULL_LIST", v.SP_FIRM_FULLLIST);

            Str_Replace(ref Sql, ":VT_COMP_ID", v.tComp.SP_COMP_ID.ToString());
            Str_Replace(ref Sql, ":VT_PERIOD_ID", v.vt_PERIOD_ID.ToString());
            Str_Replace(ref Sql, ":VT_USER_ID", v.tUser.SP_USER_ID.ToString());
            Str_Replace(ref Sql, ":USER_ID", v.tUser.SP_USER_ID.ToString());

            //Str_Replace(ref Sql, "{GUN}", v.BUGUN_GUN.ToString());
            //Str_Replace(ref Sql, "{AY}", v.BUGUN_AY.ToString());
            //Str_Replace(ref Sql, "{YIL}", v.BUGUN_YIL.ToString());
            //Str_Replace(ref Sql, "{gun}", v.BUGUN_GUN.ToString());
            //Str_Replace(ref Sql, "{ay}", v.BUGUN_AY.ToString());
            //Str_Replace(ref Sql, "{yil}", v.BUGUN_YIL.ToString());
            //Str_Replace(ref Sql, "{bugun}", Tarih_Formati(v.BUGUN_TARIH));

            if (v.Declare_Gun != "")
            {
                Str_Replace(ref Sql, "{GUN}", v.Declare_Gun);
                Str_Replace(ref Sql, "{gun}", v.Declare_Gun);
                v.Declare_Gun = string.Empty;
            }
            else
            {
                Str_Replace(ref Sql, "{GUN}", v.BUGUN_GUN.ToString());
                Str_Replace(ref Sql, "{gun}", v.BUGUN_GUN.ToString());
            }

            if (v.Declare_Ay != "")
            {
                Str_Replace(ref Sql, "{AY}", v.Declare_Ay);
                Str_Replace(ref Sql, "{ay}", v.Declare_Ay);
                v.Declare_Ay = string.Empty;
            }
            else
            {
                Str_Replace(ref Sql, "{AY}", v.BUGUN_AY.ToString());
                Str_Replace(ref Sql, "{ay}", v.BUGUN_AY.ToString());
            }

            if (v.Declare_Yil != "")
            {
                Str_Replace(ref Sql, "{YIL}", v.Declare_Yil);
                Str_Replace(ref Sql, "{yil}", v.Declare_Yil);
                v.Declare_Yil = string.Empty;
            }
            else
            {
                Str_Replace(ref Sql, "{YIL}", v.BUGUN_YIL.ToString());
                Str_Replace(ref Sql, "{yil}", v.BUGUN_YIL.ToString());
            }

            if (v.Declare_bugun != "")
            {
                Str_Replace(ref Sql, "{bugun}", v.Declare_bugun);
                v.Declare_bugun = string.Empty;
            }
            else
            {
                Str_Replace(ref Sql, "{bugun}", Tarih_Formati(v.BUGUN_TARIH));
            }

            if (Sql.IndexOf("{baslamaTarihi}") > -1)
            {
                Str_Replace(ref Sql, "{baslamaTarihi}", Tarih_Formati(v.BUGUN_TARIH));
            }

            if (Sql.IndexOf("{bitisTarihi}") > -1)
            {
                Str_Replace(ref Sql, "{bitisTarihi}", Tarih_Formati(v.BUGUN_TARIH));
            }

            if (Sql.IndexOf("[MSV3]") > -1)
            {
                // buraya vt ile mssql mi mysql mi de ileride gerekir
                //

                /*
                if (dBaseNo == 2)
                    Str_Replace(ref Sql, "[MSV3]", "[" + v.active_DB.managerDBName.ToString() + "]");
                if (dBaseNo == 3)
                    Str_Replace(ref Sql, "[MSV3]", "[" + v.db_MAINMANAGER_DBNAME.ToString() + "]");
                */
                Str_Replace(ref Sql, "[MSV3]", "[" + v.active_DB.managerDBName.ToString() + "]");
            }

            //if (v.vt_USER_SHOP_LIST != "")
            //    Str_Replace(ref Sql, ":VT_USER_SHOP_LIST", v.vt_USER_SHOP_LIST);
            //if (v.vt_USER_SHOP_LIST == "")
            //    Str_Replace(ref Sql, ":VT_USER_SHOP_LIST", "-1");

            //    SQL = SQL.Replace("[", "");
            //    SQL = SQL.Replace("]", "");
            if (vt.DBaseType == v.dBaseType.MySQL)
            {
                Str_Replace(ref Sql, "[", "");
                Str_Replace(ref Sql, "]", "");
            }

            return Sql;
        }

        #endregion SQLPreparing

        #region Str_Replace ( Str Değiştirme )
        /*  (char)34 = "    (char)39 = '   */
        public string Str_Replace(ref string Veri, string mevcut_ifade, string yeni_ifade)
        {
            if (Veri.Length > 0)
                Veri = Veri.Replace(mevcut_ifade, yeni_ifade);
            return Veri;
        }

        public string Str_Replace(ref string Veri, char mevcut_ifade, char yeni_ifade)
        {
            if (Veri.Length > 0)
                Veri = Veri.Replace(mevcut_ifade, yeni_ifade);
            return Veri;
        }

        #endregion
        
        #region *Tarih Fonksiyonları

        #region Tarih_Formati

        public string Tarih_Formati(DateTime Tarih)
        {
            string s = "";
            string t = "";
            if (Tarih.Year > 0)
            {
                t = Tarih.Date.ToString().Substring(0, 10);
                /*
                s = "'" +
                    t[3].ToString() + t[4].ToString() + "." +
                    t[0].ToString() + t[1].ToString() + "." +
                    t[6].ToString() + t[7].ToString() +
                    t[8].ToString() + t[9].ToString() + "'";
                s = " Convert(Date, Convert(varchar(10)," + s + ", 101), 101) ";
                */

                //s = " Convert(Date, Convert(varchar(10),'" + t + "', 101), 101) ";

                if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                    s = " Convert(Date, '" + t + "', 103) ";
                if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                    s = " Convert('" + t + "', Date) ";
            }
            else s = "null";
            return s;
        }

        public string TarihSaat_Formati(DateTime Tarih)
        {
            string s = "";
            string t = "";
            string saat = "";

            if (Tarih.Year > 0)
            {
                t = Tarih.ToString();

                saat = Tarih.ToLongTimeString();
                /*
                s = "'" +
                    t[3].ToString() + t[4].ToString() + "." +
                    t[0].ToString() + t[1].ToString() + "." +
                    t[6].ToString() + t[7].ToString() +
                    t[8].ToString() + t[9].ToString() + " " + saat + "'";
                */

                if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                {
                    s = "'" + Tarih.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture) + " " + saat + "'";
                    // CONVERT(SMALLDATETIME, '05.29.2015 00:00:00', 101)
                    s = " CONVERT(SMALLDATETIME, " + s + ", 101) ";
                }
                if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                {
                    s = "'" + Tarih.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture) + " " + saat + "'";
                    s = " convert(" + s + ", datetime) ";
                }

            }
            else s = "null";

            return s;
        }

        public string TarihSaat_Formati_End(DateTime Tarih)
        {
            string s = "";
            string t = "";
            string saat = "";

            if (Tarih.Year > 0)
            {
                t = Tarih.ToString();

                saat = "23:59:00";

                //s = "'" +
                //    t[3].ToString() + t[4].ToString() + "." +
                //    t[0].ToString() + t[1].ToString() + "." +
                //    t[6].ToString() + t[7].ToString() +
                //    t[8].ToString() + t[9].ToString() + " " + saat + "'";

                s = "'" + Tarih.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture) + " " + saat + "'";


            }
            else s = "null";
            return s;
        }

        public DateTime Tarih_Set(string Read_Date)
        {
            DateTime Tarih = Convert.ToDateTime("01.01.0001");
            try
            {
                Tarih = Convert.ToDateTime(Read_Date);
            }
            catch (Exception)
            {
                Tarih = Convert.ToDateTime("01.01.0001");
            }

            return Tarih;
        }

        public string TarihSaat_Etiketi_Preparing()
        {
            DateTime Tarih = DateTime.Now;
            string s = Tarih.ToLongTimeString();

            string n = "";
            string t = "";

            if (Tarih.Year > 0)
            {
                t = Tarih.ToString();

                n = t[8].ToString() + t[9].ToString() + // yıl
                    t[3].ToString() + t[4].ToString() + // ay
                    t[0].ToString() + t[1].ToString() + '_' +// gun
                    s[0].ToString() + s[1].ToString() + // hh : saat
                    s[3].ToString() + s[4].ToString() + // dd :
                    s[6].ToString() + s[6].ToString();

            }
            else n = "null";
            return n;
        }

        #endregion

        #region Tarih Aralığı

        public void Tarih_Araligi_Haftalik(DateTime MasterTarih, ref DateTime BasTarih, ref DateTime BitTarih, int ArtiHafta, int KacHafta)
        {
            if (BasTarih.ToString() == "01.01.0001 00:00:00")
            {
                BasTarih = v.BU_HAFTA_BASI_TARIH.AddDays(ArtiHafta * 7);
                BitTarih = v.BU_HAFTA_BASI_TARIH.AddDays((ArtiHafta * 7) + ((KacHafta * 7) - 1));
            }
            else
            {
                BasTarih = MasterTarih.AddDays(ArtiHafta * 7);
                BitTarih = MasterTarih.AddDays((ArtiHafta * 7) + ((KacHafta * 7) - 1));
            }
        }

        public int Iki_Tarih_Arasi_Kac_Gun(DateTime Bas_Tarih, DateTime Bit_Tarih)
        {
            System.TimeSpan zaman;
            zaman = Bit_Tarih.Subtract(Bas_Tarih);
            int fark = Convert.ToInt32(zaman.TotalDays);
            return fark;
        }

        public void Tarihin_Ayin_IlkGunu_SonGunu(DateTime Tarih, ref DateTime IlkGun, ref DateTime SonGunu)
        {
            IlkGun = new DateTime(Tarih.Year, Tarih.Month, 1);
            SonGunu = IlkGun.AddMonths(1).AddDays(-1);
        }

        #endregion Tarih Aralığı

        #endregion *Tarih Fonksiyonları

        #region Str_Check

        public string AntiStr_Dot(string Veri)
        {
            // string içindeki . işaareti _ işaretile değiştiriliyor
            Str_Replace(ref Veri, (char)46, (char)95);
            return Veri;
        }


        /*  (char)34 = "    (char)39 = '   */
        public string Str_Check(string Veri)
        {
            // string içindeki ' işaareti " işaretile değiştiriliyor
            Str_Replace(ref Veri, (char)39, (char)34);
            return Veri;
            /*
            string s = "";
            for (int i = 0; i < Veri.Length; ++i)
            {
                if (Veri[i].ToString() != "'")
                { s += Veri[i].ToString(); }
                else
                { s += '"'; };
            }

            return s;
            */
        }

        public string Str_AntiCheck(string Veri)
        {
            // string içindeki " işaareti ' işaretile değiştiriliyor
            Str_Replace(ref Veri, (char)34, (char)39);
            Str_Replace(ref Veri, '½', (char)39);

            return Veri;
            /*
            string s = "";
            for (int i = 0; i < Veri.Length; ++i)
            {
                if (Veri[i] != '"')
                { s += Veri[i].ToString(); }
                else
                { s += "'"; };
            }

            return s;
            */
        }

        #endregion Str_Check
        
        #region tData_Convert
        public string tData_Convert(string fvalue, int ftype)
        {
            string sonuc = "";

            // eğer tanımsız ise default veri tipi VarChar olsun
            if (ftype == 0) ftype = 167;

            //* rakam  56, 48, 127, 52, 60, 62, 59, 108
            if ((ftype == 56) | (ftype == 48) | (ftype == 127) | (ftype == 52) |
                (ftype == 60) | (ftype == 62) | (ftype == 59) | (ftype == 108))
            {
                // Eğer veri (Rakam) yok ise Sıfır bas
                //if (fvalue == "") fvalue = "0";

                // rakam türü , ile ayrılmış ise , yerine . işareti değiştiriliyor 
                if (fvalue.IndexOf(",") > -1)
                    fvalue = fvalue.Replace(",", ".");

                sonuc = fvalue;
            }

            //* text = (char)175, (varchar)167, (ntext)99, (text)35, (nchar)239
            if ((ftype == 175) | (ftype == 167) | (ftype == 99) | (ftype == 35) | (ftype == 239))
            {
                // Eğer veri yok ise null bas
                if ((fvalue == "") | (fvalue == "null"))
                    sonuc = "null";
                else
                    sonuc = " '" + Str_Check(fvalue) + "' ";
            }

            //* bit türü 104
            if (ftype == 104)
            {
                if ((fvalue == "") | (fvalue == "0") | (fvalue == "False"))
                    sonuc = " 0 ";
                else sonuc = " 1 ";
            }

            //* datetime 58
            if ((ftype == 58) || (ftype == 581))
            {
                if (fvalue != "")
                    sonuc = " " + TarihSaat_Formati(Convert.ToDateTime(fvalue)) + " ";
                else sonuc = "";
            }

            if (ftype == 582)
            {
                if (fvalue != "")
                    sonuc = " " + TarihSaat_Formati_End(Convert.ToDateTime(fvalue)) + " ";
                else sonuc = "";
            }


            //* date türü 40
            if ((ftype == 40) || (ftype == 61))
            {
                if (fvalue != "")
                    sonuc = " " + Tarih_Formati(Convert.ToDateTime(fvalue)) + " ";
            }


            // new CheckedComboBoxEdit
            #region
            if ((ftype == 1672) || (ftype == 562))
            {
                string value = string.Empty;
                sonuc = string.Empty;
                int p1 = 0;
                int p2 = 0;

                // köşeli parentez içindeki değerler alıncak ve 
                // sonuçta bu değerler in için hazır hale getirilecek                 

                // fvalue nin gelişi

                // [SH] Saat Hesabı, [HI] Hafta İçi, [VS] Sabah Vardiyası
                //   +  Saat Hesabı, [HI] Hafta İçi, [VS] Sabah Vardiyası
                //   +  Saat Hesabı,   +  Hafta İçi, [VS] Sabah Vardiyası
                //   +  Saat Hesabı,   +  Hafta İçi,   +  Sabah Vardiyası

                // sonuc = ('SH','HI','VS')

                while (fvalue.IndexOf("[") > -1)
                {
                    value = string.Empty;
                    p1 = fvalue.IndexOf("[");
                    p2 = fvalue.IndexOf("]");

                    if ((p1 > -1) && (p2 > -1))
                    {
                        // valueyi al
                        value = fvalue.Substring(p1 + 1, (p2 - p1) - 1);
                        // valueyi listeden çıkar
                        Str_Replace(ref fvalue, "[" + value + "]", "+");

                        // string field ise
                        if (ftype == 1672) value = "'" + value + "'";
                        // rakam field ise
                        //if (ftype == 562)

                        if (sonuc.Length == 0) sonuc = value;
                        else sonuc = sonuc + "," + value;
                    }
                }

                if (IsNotNull(sonuc))
                    sonuc = "(" + sonuc + ")";
            }
            #endregion 

            return sonuc;
        }
        #endregion tData_Convert

        #region Find_dBLongName
        public string Find_dBLongName(string DatabaseName)
        {
            string s = string.Empty;

            // "master"
            if ((DatabaseName.ToUpper() == "MASTER") ||
                (DatabaseName == v.dBaseNo.Master.ToString()) ||
                (DatabaseName == "1"))
                s = v.db_MASTER_DBNAME;

            //  "MANAGERSERVER"
            if ((DatabaseName.ToUpper() == v.active_DB.managerDBName.ToUpper()) ||
                (DatabaseName == v.dBaseNo.Manager.ToString()) ||
                (DatabaseName == "2"))
                s = v.active_DB.managerDBName;

            //  "MAINMANAGERSERVER"
            if ((DatabaseName.ToUpper() == v.db_MAINMANAGER_DBNAME.ToUpper()) ||
                (DatabaseName == "3"))
                s = v.db_MAINMANAGER_DBNAME;

            //  proje adı
            if ((DatabaseName.ToUpper() == v.active_DB.projectDBName.ToUpper()) ||
                (DatabaseName == v.dBaseNo.Project.ToString()) ||
                (DatabaseName == "4") ||
                (DatabaseName == ""))
                s = v.active_DB.projectDBName;

            //  firm Main DBaseName
            if ((DatabaseName.ToUpper() == v.active_DB.firmMainDBName.ToUpper()) ||
                (DatabaseName == v.dBaseNo.FirmMainDB.ToString()) ||
                (DatabaseName == "5"))
                s = v.active_DB.firmMainDBName;

            //  firm Period DBaseName
            if ((DatabaseName.ToUpper() == v.active_DB.firmPeriodDBName.ToUpper()) ||
                (DatabaseName == v.dBaseNo.FirmPeriodDB.ToString()) ||
                (DatabaseName == "6"))
                s = v.active_DB.firmPeriodDBName;

            if (IsNotNull(s))
            {
                return s;
            }
            else
            {
                if (DatabaseName != "null")
                    MessageBox.Show("DİKKAT : [ " + DatabaseName + " ] için Database Name tespit edilemedi ...");

                return "";
            }
        }
        #endregion Find_dBLongName
   
        #region TableIPCode_Get
        public void TableIPCode_Get(string TableIPCode,
             ref string softCode, ref string projectCode,
             ref string TableCode, ref string IPCode)
        {
            /* 
            string Table_Code = string.Empty;
            string IP_Code = string.Empty;

            tToolBox t = new tToolBox();
            t.TableIPCode_Get(Table_IP_Code, ref Table_Code, ref IP_Code);
            */

            int i = 0;
            i = TableIPCode.IndexOf(".");
            if (i > 0)
            {
                TableCode = TableIPCode.Substring(0, i);

                softCode = Get_And_Clear(ref TableCode, "/");
                projectCode = Get_And_Clear(ref TableCode, "/");

                TableIPCode = TableIPCode.Remove(0, i + 1);
                IPCode = TableIPCode;
            }
            else
            {
                TableCode = "null";
                IPCode = "null";
            }
        }
        #endregion TableIPCode_Get  

        #region Get_And_Clear

        public string Get_And_Clear(ref string Veri, string Separator)
        {
            string snc = Veri;
            int i1 = Veri.IndexOf(Separator);
            if (i1 > -1)
            {
                // separator karekter sayısı 
                int i2 = Separator.Length;

                // separatörden önceki veri
                snc = Veri.Substring(0, i1);

                // separatörden sonra kalan kısım
                Veri = Veri.Remove(0, i1 + i2);
            }
            return snc;
        }

        public string BeforeGet_And_AfterClear(ref string Veri, string Separator, Boolean Separator_Dahil)
        {
            string snc = Veri;
            int i1 = Veri.IndexOf(Separator);
            if (i1 > -1)
            {
                int i2 = 0;
                int i3 = Separator.Length;
                if (Separator_Dahil) i2 = i3;

                snc = Veri.Substring(0, i1 + i2);

                if (Separator_Dahil)
                    Veri = Veri.Remove(0, i1 + i2);
                else
                {
                    Veri = Veri.Remove(0, i1 + i3);
                }
            }
            return snc;
        }

        public string AfterGet_And_BeforeClear(ref string Veri, string Separator, Boolean Separator_Dahil)
        {
            string snc = Veri;
            int i1 = Veri.IndexOf(Separator);
            if (i1 > -1)
            {
                int i2 = 0;
                int i3 = Separator.Length;
                int i4 = Veri.Length;

                if (Separator_Dahil) i2 = i3;

                snc = Veri.Substring(i1 + i2, i4 - (i1 + i2));

                Veri = Veri.Remove(i1 + i2, i4 - (i1 + i2));
            }
            return snc;
        }

        #endregion Get_And_Clear
        
        #region TableRemove
        public Boolean TableRemove(DataSet ds)
        {
            Boolean sonuc = true;

            if (ds == null) return sonuc;
            if (ds.Tables.Count == 0) return sonuc;

            try
            {
                for (int i = ds.Tables.Count - 1; i >= 0; i--)
                {
                    //ds.Tables[i].Clear();
                    ds.Tables.RemoveAt(i);
                }
            }
            catch (Exception e)
            {
                sonuc = false;
                MessageBox.Show("HATALI : " + v.ENTER2 + e.Message, "TableRemove");
            }

            return sonuc;
        }
        #endregion TableRemove

        #region MSSQL_Server_Tarihi
        public void MSSQL_Server_Tarihi()
        {
            string ySql =
              @"  select top 1 upd.*
             , GETDATE() DB_TARIH 
             , CONVERT(varchar(10), GETDATE(), 104) TARIH 
             , CONVERT(varchar, GETDATE(), 102) YILAYGUN 
             , CONVERT(varchar, GETDATE(), 8) UZUN_SAAT 
             , CONVERT(varchar, GETDATE(), 14) UZUN_SAAT2 
             , YEAR(GETDATE()) YIL 
             , MONTH(GETDATE()) AY 
             , DAY(GETDATE()) GUN 
             , DATEPART(weekday, GETDATE()) HAFTA_GUN
            from SYS_UPDATES upd order by upd.REC_DATE desc 
            " + v.ENTER;

            DataSet ds = new DataSet();

            if (SQL_Read_Execute(v.dBaseNo.Manager, ds, ref ySql, "", "MSSQL Server Tarihi"))
            {
                v.BUGUN_TARIH = Convert.ToDateTime(ds.Tables[0].Rows[0]["TARIH"].ToString());
                v.DONEM_BASI_TARIH = v.BUGUN_TARIH.AddDays(-367);
                v.DONEM_BITIS_TARIH = v.BUGUN_TARIH.AddDays(365);
                v.BU_HAFTA_BASI_TARIH = v.BUGUN_TARIH; //???
                v.BU_HAFTA_SONU_TARIH = v.BUGUN_TARIH; //???
                v.BIR_HAFTA_ONCEKI_TARIH = DateTime.Now.AddDays(-7);
                v.IKI_HAFTA_ONCEKI_TARIH = DateTime.Now.AddDays(-14);
                v.ON_GUN_ONCEKI_TARIH = DateTime.Now.AddDays(-10);
                v.GECEN_AY_BUGUN = DateTime.Now.AddMonths(-1);
                v.GELECEK_AY_BUGUN = DateTime.Now.AddMonths(1);

                v.BUGUN_GUN = v.BUGUN_TARIH.Day;
                v.BUGUN_AY = v.BUGUN_TARIH.Month;
                v.BUGUN_YIL = v.BUGUN_TARIH.Year;

                v.TARIH_SAAT = ds.Tables[0].Rows[0]["DB_TARIH"].ToString();
                v.SAAT_KISA = ds.Tables[0].Rows[0]["UZUN_SAAT"].ToString().Substring(0, 5);
                v.SAAT_UZUN = ds.Tables[0].Rows[0]["UZUN_SAAT"].ToString();
                v.SAAT_UZUN2 = ds.Tables[0].Rows[0]["UZUN_SAAT2"].ToString();

                //-- BU_HAFTA_BASI_TARIH
                string gun = v.BUGUN_TARIH.DayOfWeek.ToString();
                byte g = 0;

                if (gun == "Sunday") g = 1;
                if (gun == "Monday") g = 7;
                if (gun == "Tuesday") g = 6;
                if (gun == "Wednesday") g = 5;
                if (gun == "Thursday") g = 4;
                if (gun == "Friday") g = 3;
                if (gun == "Saturday") g = 2;

                v.BU_HAFTA_BASI_TARIH = v.BUGUN_TARIH.Date.AddDays(-1 * (7 - g));

                //-- BU_HAFTA_SONU_TARIH
                gun = v.BUGUN_TARIH.DayOfWeek.ToString();

                if (gun == "Sunday") g = 0;
                if (gun == "Monday") g = 6;
                if (gun == "Tuesday") g = 5;
                if (gun == "Wednesday") g = 4;
                if (gun == "Thursday") g = 3;
                if (gun == "Friday") g = 2;
                if (gun == "Saturday") g = 1;

                v.BU_HAFTA_SONU_TARIH = v.BUGUN_TARIH.Date.AddDays(g);

                Tarihin_Ayin_IlkGunu_SonGunu(v.BUGUN_TARIH, ref v.BU_AY_BASI_TARIH, ref v.BU_AY_SONU_TARIH);

                v.tExeAbout.ftpVersionNo = ds.Tables[0].Rows[0]["VERSION_NO"].ToString();
                v.tExeAbout.ftpExeName = ds.Tables[0].Rows[0]["EXE_NAME"].ToString();
                v.tExeAbout.ftpPacketName = ds.Tables[0].Rows[0]["PACKET_NAME"].ToString();


            }

            //MessageBox.Show(v.BUGUN_TARIH.ToString());

            ds.Dispose();
        }
        #endregion MSSQL_Server_Tarihi

        #region *InputBox
        //public DialogResult InputBox(string title, string promptText, ref string value, int ftype, string displayFormat)
        public DialogResult UserInpuBox(vUserInputBox vUIBox)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            //DevExpress.XtraEditors.TextEdit textBox = new DevExpress.XtraEditors.TextEdit();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Font = new Font("Tahoma", 10);
            form.Text = vUIBox.title;       // title;
            label.Text = vUIBox.promptText; // promptText;
            //label
            textBox.Text = vUIBox.value;    // value;

            buttonOk.Text = "Tamam";
            buttonCancel.Text = "Vazgeç";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 26);
            //textBox.SetBounds(12, 36, 372, 20);
            textBox.SetBounds(12, 60, 372, 20);
            textBox.Font = new Font("Tahoma", 12);


            /// if ((tcmp_format_type == 0) || (tcmp_format_type == 1))
            ///     {
            ///         tEdit.RightToLeft = RightToLeft.Yes;
            ///         tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            ///     }
            ///     if (tcmp_format_type == 2) tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            ///     if (tcmp_format_type == 3) tEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Custom;
            /// 
            ///     tEdit.Properties.Mask.EditMask = tdisplayformat;
            ///     tEdit.Properties.Mask.SaveLiteral = true;
            ///     tEdit.Properties.Mask.ShowPlaceHolders = true;
            ///     tEdit.Properties.Mask.UseMaskAsDisplayFormat = true;

            //* date 40
            if ((vUIBox.fieldType == 40) |
                (vUIBox.fieldType == 61))
            {
                //textBox.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            }

            //* rakam  56, 48, 127, 52, 60, 62, 59, 106, 108
            if ((vUIBox.fieldType == 56) | (vUIBox.fieldType == 48) |
                (vUIBox.fieldType == 59) | (vUIBox.fieldType == 52) |
                (vUIBox.fieldType == 60) | (vUIBox.fieldType == 62) |
                (vUIBox.fieldType == 127) | (vUIBox.fieldType == 106) |
                (vUIBox.fieldType == 108))
            {
                textBox.RightToLeft = RightToLeft.Yes;
                //textBox.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;

                if (vUIBox.value == "") textBox.Text = "0";
            }

            if ((vUIBox.displayFormat != "") ||
                (vUIBox.displayFormat != null))
            {
                /*
                textBox.Properties.Mask.EditMask = vUIBox.displayFormat;
                textBox.Properties.Mask.SaveLiteral = true;
                textBox.Properties.Mask.ShowPlaceHolders = true;
                textBox.Properties.Mask.UseMaskAsDisplayFormat = true;
                */
            }


            //buttonOk.SetBounds(228, 72, 75, 23);
            //buttonCancel.SetBounds(309, 72, 75, 23);
            buttonOk.SetBounds(228, 98, 75, 25);
            buttonCancel.SetBounds(309, 98, 75, 25);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(400, 175);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(400, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();

            vUIBox.value = textBox.Text;

            return dialogResult;
        }
        #endregion InputBox

        #region tLast_Char_Remove / En Sondaki ', ' sil
        public void tLast_Char_Remove(ref string Veri)
        {
            // En Sondaki ', ' siliniyor
            Veri = Veri.Trim();
            if (Veri.Length < 2)
                return;
            Veri = Veri.Substring(0, Veri.Length - 1);
        }
        #endregion

        #region myInt32

        public int myInt32(string Veri)
        {
            int j = 0;

            string s = "";
            for (int i = 0; i < Veri.Length; ++i)
            {
                if (char.IsDigit(Veri[i]) == true)
                {
                    s += Veri[i].ToString();
                }
                else
                {
                    if (Veri[i] == '-')
                        s += Veri[i].ToString();
                    if (Veri[i] == ',')
                        s += Veri[i].ToString();
                }
            }

            if (s == "") s = "0";

            try
            {
                j = Convert.ToInt32(s);

            }
            catch (Exception)
            {
                if (s.IndexOf(',') > -1)
                    MessageBox.Show("DİKKAT : myInt32 çevrim için gelen rakam double cinsinden !");

                j = 0;  //throw;
            }

            return j;
        }

        #endregion myInt32

        #region myInt16

        public Int16 myInt16(string Veri)
        {
            Int16 j = 0;

            string s = "";
            for (int i = 0; i < Veri.Length; ++i)
            {
                if (char.IsDigit(Veri[i]) == true)
                {
                    s += Veri[i].ToString();
                }
                else
                {
                    if (Veri[i] == '-')
                        s += Veri[i].ToString();
                }
            }

            if (s == "") s = "0";

            try
            {
                j = Convert.ToInt16(s);
            }
            catch (Exception)
            {
                j = 0;  //throw;
            }

            return j;
        }

        #endregion myInt16

        #region Firm İşlemleri

        public void firmAboutAdd(DataRow row, ref string ufl)
        {
            int fId = 0;
            string fName = "";
            string fGuid = "";
            string fusePackage = "";

            string firmDBType = "";
            string firmDBName = "";
            string firmMainDBFormat = "";
            string firmPeriodDBFormat = "";
            string firmServerType = "";
            string firmServerName = "";
            string firmAuthentication = "";
            string firmLogin = "";
            string firmPassword = "";

            string fmainDbConnText = "";
            string fperiodDbConnText = "";

            // user in tüm FIRMA ıd leri toplanıyor 
            ufl = ufl + row["ID"].ToString() + ", ";
            //
            fId = myInt32(row["ID"].ToString());
            fName = row["FIRM_NAME"].ToString();
            fGuid = row["FIRM_GUID"].ToString();
            fusePackage = row["USE_PACKAGE"].ToString();

            firmDBType = row["FIRM_DB_TYPE"].ToString();
            firmDBName = row["FIRM_DB_NAME"].ToString();
            firmMainDBFormat = row["FIRM_MAINDB_FORMAT"].ToString();
            firmPeriodDBFormat = row["FIRM_PERIODDB_FORMAT"].ToString();
            firmServerType = row["FIRM_SERVER_TYPE"].ToString();
            firmServerName = row["FIRM_SERVER_NAME"].ToString();
            firmAuthentication = row["FIRM_AUTHENTICATION"].ToString();
            firmLogin = row["FIRM_LOGIN"].ToString();
            firmPassword = row["FIRM_PASSWORD"].ToString();

            fmainDbConnText = row["FIRM_MAINDB_CONNTEXT"].ToString();
            fperiodDbConnText = row["FIRM_PERIODDB_CONNTEXT"].ToString();

            //
            FirmAbout tFirmAbout = new FirmAbout();
            tFirmAbout.FirmId = fId;
            tFirmAbout.FirmName = fName;
            tFirmAbout.FirmGuid = fGuid;
            tFirmAbout.usePackage = fusePackage;

            tFirmAbout.firmDBType = firmDBType;
            tFirmAbout.firmDBName = firmDBName;
            tFirmAbout.firmMainDBFormat = firmMainDBFormat;
            tFirmAbout.firmPeriodDBFormat = firmPeriodDBFormat;
            tFirmAbout.firmServerType = firmServerType;
            tFirmAbout.firmServerName = firmServerName;
            tFirmAbout.firmAuthentication = firmAuthentication;
            tFirmAbout.firmLogin = firmLogin;
            tFirmAbout.firmPassword = firmPassword;
            tFirmAbout.firmMainConnText = fmainDbConnText;
            tFirmAbout.firmPeriodConnText = fperiodDbConnText;

            v.tFirmUserList.Add(tFirmAbout);
        }

        public void selectFirm(int firmPos)
        {
            //foreach (FirmAbout fAbout in v.tFirmUserList)
            //foreach (FirmAbout fAbout in v.tFirmFullList)
            int pos = 0;
            foreach (FirmAbout fAbout in v.tFirmUserList)
            {
                if (pos == firmPos)
                {
                    v.SP_FIRM_ID = Convert.ToInt32(fAbout.FirmId);
                    v.SP_FIRM_NAME = fAbout.FirmName;
                    v.SP_FIRM_USE_PACKAGE = fAbout.usePackage;

                    if (IsNotNull(v.SP_FIRM_USE_PACKAGE) == false)
                        v.SP_FIRM_USE_PACKAGE = "UST/T01/ONM/KOBI";

                    /// firm tablosunda tanımlı ise
                    /// 
                    if (((fAbout.firmMainDBFormat == "") && (fAbout.firmServerName != "")) ||
                        ((fAbout.firmPeriodDBFormat == "") && (fAbout.firmServerName != "")))
                    {
                        v.active_DB.projectDBName = fAbout.firmDBName;
                        v.active_DB.projectServerName = fAbout.firmServerName; // "195.xx";
                        v.active_DB.projectUserName = fAbout.firmLogin; //"sa";

                        if (fAbout.firmPassword != "")
                            v.active_DB.projectPsw = "Password = " + fAbout.firmPassword + ";"; // Password = 1;
                        else v.active_DB.projectPsw = "";

                        v.active_DB.projectDBType = v.dBaseType.MSSQL;

                        v.active_DB.projectConnectionText =
                            string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                            v.active_DB.projectServerName,
                            v.active_DB.projectDBName,
                            v.active_DB.projectUserName,
                            v.active_DB.projectPsw);

                        v.active_DB.projectMSSQLConn = new SqlConnection(v.active_DB.projectConnectionText);
                        //v.active_DB.projectMSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateProject);
                    }

                    /// firm tablosunda YIL formatlı db bilgisi mevcut ise
                    /// 
                    if (fAbout.firmMainDBFormat != "")
                    {
                        //v.active_DB.firmMainDBName = "043_DUYSA_İNŞ_LTD_ŞTİ_GENEL";
                        //v.active_DB.firmMainDBName = "test_GENEL";
                        v.active_DB.firmMainDBName = fAbout.firmMainDBFormat.Replace("DBNAME", fAbout.firmDBName);
                        v.active_DB.firmMainDBName = v.active_DB.firmMainDBName.Replace("YYYY", v.BUGUN_YIL.ToString());

                        v.active_DB.firmMainServerName = fAbout.firmServerName; // "PCTEKIN\\SQLEXPRESS";
                        v.active_DB.firmMainUserName = fAbout.firmLogin; //"sa";

                        if (fAbout.firmPassword != "")
                            v.active_DB.firmMainPsw = "Password = " + fAbout.firmPassword + ";"; // Password = zrvsql;
                        else v.active_DB.firmMainPsw = "";

                        v.active_DB.firmMainDBType = v.dBaseType.MSSQL;

                        v.active_DB.firmMainConnectionText =
                            string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                            v.active_DB.firmMainServerName,
                            v.active_DB.firmMainDBName,
                            v.active_DB.firmMainUserName,
                            v.active_DB.firmMainPsw);

                        v.active_DB.firmMainMSSQLConn = new SqlConnection(v.active_DB.firmMainConnectionText);
                        //v.active_DB.firmMainMSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateFirmMain);
                    }

                    /// firm tablosunda YIL formatlı db bilgisi mevcut ise
                    /// 
                    if (fAbout.firmPeriodDBFormat != "")
                    {
                        //v.active_DB.firmPeriodDBName = "043_DUYSA_İNŞ_LTD_ŞTİ_2018";
                        //v.active_DB.firmPeriodDBName = "test_2018";
                        v.active_DB.firmPeriodDBName = fAbout.firmPeriodDBFormat.Replace("DBNAME", fAbout.firmDBName);
                        v.active_DB.firmPeriodDBName = v.active_DB.firmPeriodDBName.Replace("YYYY", v.BUGUN_YIL.ToString());

                        v.active_DB.firmPeriodServerName = fAbout.firmServerName; // "PCTEKIN\\SQLEXPRESS";
                        v.active_DB.firmPeriodUserName = fAbout.firmLogin; //"sa";

                        if (fAbout.firmPassword != "")
                            v.active_DB.firmPeriodPsw = "Password = " + fAbout.firmPassword + ";"; // Password = 1;
                        else v.active_DB.firmPeriodPsw = "";

                        v.active_DB.firmPeriodDBType = v.dBaseType.MSSQL;

                        v.active_DB.firmPeriodConnectionText =
                            string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                            v.active_DB.firmPeriodServerName,
                            v.active_DB.firmPeriodDBName,
                            v.active_DB.firmPeriodUserName,
                            v.active_DB.firmPeriodPsw);

                        v.active_DB.firmPeriodMSSQLConn = new SqlConnection(v.active_DB.firmPeriodConnectionText);
                        //v.active_DB.firmPeriodMSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateFirmPeriod);
                    }

                    /// firm tablosunda hiç bilgi verilmemiş ise
                    /// 
                    if ((fAbout.firmMainDBFormat == "") &&
                        (fAbout.firmServerName == "") &&
                        (fAbout.firmPeriodDBFormat == "") &&
                        (fAbout.firmServerName == ""))
                    {
                        v.active_DB.projectServerName = "94.73.170.20";
                        v.active_DB.projectDBName = "u7093888_MSV3GREENMS";
                        v.active_DB.projectUserName = "u7093888_user4601";
                        v.active_DB.projectPsw = "Password = CanBerk98;";
                        v.active_DB.projectDBType = v.dBaseType.MSSQL;
                        v.active_DB.projectConnectionText =
                            string.Format(" Data Source = {0}; Initial Catalog = {1}; User ID = {2}; {3} MultipleActiveResultSets = True ",
                            v.active_DB.projectServerName,
                            v.active_DB.projectDBName,
                            v.active_DB.projectUserName,
                            v.active_DB.projectPsw);

                        v.active_DB.projectMSSQLConn = new SqlConnection(v.active_DB.projectConnectionText);
                        //v.active_DB.projectMSSQLConn.StateChange += new StateChangeEventHandler(DBConnectStateProject);
                    }

                    break;
                }
                pos++;
            }
        }

        #region Aktif Firma Read
        public void Aktif_Firma_Read()
        {
            if ((v.SP_FIRM_ID > 0) && (IsNotNull(v.SP_FIRM_NAME) == false))
            {
                string function_name = "Aktif Kurs";
                string Sql =
                    " Select * from [" + v.active_DB.projectDBName + "].dbo.[" + v.SP_FIRM_TABLENAME + "]" +
                    " where " + v.SP_FIRM_TABLEKEYFNAME + " = " + v.SP_FIRM_ID.ToString();

                Data_Read_Execute(v.ds_Firm, ref Sql, v.SP_FIRM_TABLENAME, null);

                if (IsNotNull(v.ds_Firm))
                {
                    v.SP_FIRM_NAME = v.ds_Firm.Tables[v.SP_FIRM_TABLENAME].Rows[0][v.SP_FIRM_TABLECAPTIONFNAME].ToString();
                }
                else
                {
                    v.SP_FIRM_NAME = "DİKKAT : TANIMSIZ KURS";
                    MessageBox.Show(v.SP_FIRM_NAME, function_name);
                }

            }
        }
        #endregion Aktif Firma Read

        #endregion Firm İşlemleri



        public void ComputerAbout()
        {

            ///Win32_Processor
            ///
            ///string Name 
            ///string ProcessorId
            ///string SystemName
            ///
            ///Win32_DiskDrive
            ///
            ///string   SerialNumber;
            ///string   SystemName;

            ManagementObjectSearcher MOS = new ManagementObjectSearcher("root\\CIMV2", "Select * From Win32_Processor ");
            foreach (ManagementObject obj in MOS.Get())
            {
                v.tComputer.SystemName = Convert.ToString(obj["SystemName"]);
                v.tComputer.Processor_Name = Convert.ToString(obj["Name"]);
                v.tComputer.Processor_Id = Convert.ToString(obj["ProcessorId"]);
            }

            MOS = new ManagementObjectSearcher("root\\CIMV2", "Select * From Win32_DiskDrive ");
            foreach (ManagementObject obj in MOS.Get())
            {
                v.tComputer.DiskDrive_Name = Convert.ToString(obj["Name"]);
                v.tComputer.DiskDrive_Model = Convert.ToString(obj["Model"]);
                v.tComputer.DiskDrive_SerialNumber = Convert.ToString(obj["SerialNumber"]);
            }

            MOS.Dispose();

            //MessageBox.Show(v.tComputer.Processor_Id);
        }

    }
}