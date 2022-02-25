using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Management;
using MySql.Data.MySqlClient;

using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;

using Tkn_CreateObject;
using Tkn_Variable;
using Tkn_SQLs;
using Tkn_Forms;
using Tkn_TablesRead;
using Tkn_Save;
using Tkn_Layout;
using Tkn_Ftp;

using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraWizard;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
//using System.Threading;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Globalization;
using DevExpress.XtraBars.Alerter;

namespace Tkn_ToolBox
{
    public class tToolBox : tBase
    {
        
        #region *Database İşlemleri

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

                WaitFormOpen(v.mainForm, "[ MSSQL ] " + v.Wait_Desc_DBBaglanti);

                try
                {
                    VTbaglanti.Open();
                    v.Onay = true;
                    v.SP_ConnBool_Manager = true;

                    if (v.SP_OpenApplication == false)
                    {
                        Thread.Sleep(500);
                        SplashScreenManager.CloseForm(false);
                    }
                    else
                    {
                        WaitFormOpen(v.mainForm, v.Wait_Desc_ProgramYukDevam);
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

                    if (VTbaglanti == v.SP_Conn_Master_MSQL) i = 1;
                    if (VTbaglanti == v.SP_Conn_MainManager_MSSQL) i = 3;

                    if (VTbaglanti == v.active_DB.managerMSSQLConn) i = 2;
                    if (VTbaglanti == v.active_DB.projectMSSQLConn) i = 4;

                    //Xml_.Database_Ondegerleri_Yenile();

                    try
                    {
                        if (i == 1) VTbaglanti = new SqlConnection(v.SP_Conn_Text_Master);
                        if (i == 3) VTbaglanti = new SqlConnection(v.SP_Conn_Text_MainManager);

                        if (i == 2) VTbaglanti = new SqlConnection(v.active_DB.managerConnectionText);
                        if (i == 4) VTbaglanti = new SqlConnection(v.active_DB.projectConnectionText);

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
                        //MessageBox.Show("Lütfen yardım isteyin ..." + v.ENTER2 + "Error : Database.Connection()");

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

        /// <summary>
        /// MySQL Bağlantısı
        /// </summary>
        public bool Db_Open(MySqlConnection VTbaglanti)
        {

            #region Closed ise
            bool onay = false;

            if (VTbaglanti.State == System.Data.ConnectionState.Closed)
            {
                byte i = 0;

                WaitFormOpen(v.mainForm, "[ MySQL ] " + v.Wait_Desc_DBBaglanti);

                try
                {
                    VTbaglanti.Open();
                    v.Onay = true;
                    v.SP_ConnBool_Project = true;
                    v.Kullaniciya_Mesaj_Var = "MySQL Database bağlantısı sağlandı...";

                    if (v.SP_OpenApplication == false)
                    {
                        Thread.Sleep(500);
                        SplashScreenManager.CloseForm(false);
                    }
                    else
                    {
                        WaitFormOpen(v.mainForm, v.Wait_Desc_ProgramYukDevam);
                    }

                    if ((VTbaglanti == v.SP_Conn_MySQL) &&
                        (v.SP_Connect_Source_DBType == v.db_MySQL)) v.SP_ConnectBool_Source = true;

                    if ((VTbaglanti == v.SP_Conn_MySQL) &&
                        (v.SP_Connect_Target_DBType == v.db_MySQL)) v.SP_ConnectBool_Target = true;

                }
                catch (Exception e)
                {
                    v.SP_ConnBool_Project = false;
                    v.Kullaniciya_Mesaj_Var = "DİKKAT : MySQL Database bağlantısı koptu...";

                    MessageBox.Show("HATA : MySQL Database bağlantısı açılmadı ... " + v.ENTER2 +
                        e.Message.ToString() + v.ENTER2 +
                        "*  Bunun çeşitli sebepleri olabilir." + v.ENTER2 +
                        "1. Database in bulunduğu bilgisayar ( SERVER ) kapalı olabilir ..." + v.ENTER +
                        "2. MySQL Server kapalı olabilir ..." + v.ENTER +
                        "3. Network bağlantınızda sorun olabilir ..." + v.ENTER +
                        "4. Database bağlantı tanımlarında sorun olabilir ..." + v.ENTER2 +
                        "   Bu nedenle size sorulan soruları kontrol edin, yine olmaz ise yardım isteyin ...");

                    //if (VTbaglanti == v.SP_Conn_Master_MSQL) i = 1;
                    //if (VTbaglanti == v.SP_Conn_Manager_MSSQL) i = 2;
                    //if (VTbaglanti == v.SP_Conn_MainManager_MSSQL) i = 3;
                    if (VTbaglanti == v.active_DB.projectMySQLConn) i = 4;


                    try
                    {
                        //if (i == 1) VTbaglanti = new MySqlConnection(v.SP_Conn_Text_Master_MySQL);
                        //if (i == 2) VTbaglanti = new MySqlConnection(v.SP_Conn_Text_Manager_MySQL);
                        //if (i == 3) VTbaglanti = new MySqlConnection(v.SP_Conn_Text_MainManager_MySQL);
                        if (i == 4) VTbaglanti = new MySqlConnection(v.active_DB.projectConnectionText);

                        // Burası OLMADI iyice araştır
                        VTbaglanti.Open();
                        v.Onay = true;
                        v.SP_ConnBool_Project = true;
                        v.Kullaniciya_Mesaj_Var = "MySQL Database bağlantısı sağlandı...";

                    }
                    catch (Exception e2)
                    {
                        v.Onay = false;
                        v.SP_ConnBool_Project = false;
                        v.Kullaniciya_Mesaj_Var = "DİKKAT : MySQL Database bağlantısı gerçekleşmedi...";

                        MessageBox.Show(e2.Message.ToString());
                        //MessageBox.Show("Lütfen yardım isteyin ..." + v.ENTER2 + "Error : Database.Connection()");

                        Application.Exit();
                    }

                }

            }
            else
            {
                onay = true;
            }
            #endregion Closed ise

            return onay;
        }

        #endregion DB_Open

        #region Data_Read_Execute

        public void Preparing_DataSet(DataSet dsData, vTable vt)
        {
            // Gelen DataSet üzerinden gerekli bilgilere ulaşımaya çalışılacak;

            if (dsData != null)
            {
                if (IsNotNull(dsData.Namespace) == false)
                {
                    vt.TableName = "TABLE1";
                    vt.TableCount = 255;
                }

                if (IsNotNull(dsData.Namespace))
                {
                    string myProp = dsData.Namespace.ToString();

                    byte dbaseNo_ = Set(MyProperties_Get(myProp, "=DBaseNo:"), "", (byte)0);

                    if (dbaseNo_ == 1) vt.DBaseNo = v.dBaseNo.Master;
                    if (dbaseNo_ == 2) vt.DBaseNo = v.dBaseNo.Manager;
                    if (dbaseNo_ == 4) vt.DBaseNo = v.dBaseNo.Project;

                    vt.DBaseName = Find_dBLongName(vt.DBaseNo.ToString());
                    vt.TableType = Set(MyProperties_Get(myProp, "=TableType:"), "", (byte)0);
                    vt.TableName = Set(MyProperties_Get(myProp, "=TableName:"), "TABLE1", "TABLE1");
                    vt.TableIPCode = Set(MyProperties_Get(myProp, "=TableIPCode:"), "", "");
                    vt.KeyId_FName = Set(MyProperties_Get(myProp, "=KeyFName:"), "", "");
                    vt.Cargo = Set(MyProperties_Get(myProp, "=Cargo:"), "", ""); // cargo = data, param, report

                    string TableCode = "";
                    string IPCode = "";
                    TableIPCode_Get(vt.TableIPCode, ref TableCode, ref IPCode);
                    vt.TableCode = TableCode;
                    vt.IPCode = IPCode;

                    vt.TableCount = (byte)dsData.Tables.Count;

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
                    vt.msSqlConnection = v.SP_Conn_Master_MSQL;
                    vt.mySqlConnection = null;
                }
                if ((vt.DBaseNo == v.dBaseNo.Manager) || (vt.DBaseNo == v.dBaseNo.MainManager))
                {
                    vt.DBaseType = v.dBaseType.MSSQL;
                    vt.msSqlConnection = v.active_DB.managerMSSQLConn;
                    vt.mySqlConnection = null;
                }
                //if (vt.DBaseNo == 3)
                //{
                //    vt.DBaseType = v.dBaseType.MSSQL;
                //    vt.msSqlConnection = v.SP_Conn_MainManager_MSSQL;
                //    vt.mySqlConnection = null;
                //}
                if ((vt.DBaseNo == v.dBaseNo.Project) && (v.active_DB.projectDBType == v.dBaseType.MSSQL))
                {
                    vt.DBaseType = v.dBaseType.MSSQL;
                    vt.msSqlConnection = v.active_DB.projectMSSQLConn;
                    vt.mySqlConnection = null;
                }
                if ((vt.DBaseNo == v.dBaseNo.Project) && (v.active_DB.projectDBType == v.dBaseType.MySQL))
                {
                    vt.DBaseType = v.dBaseType.MySQL;
                    vt.msSqlConnection = null;
                    vt.mySqlConnection = v.active_DB.projectMySQLConn;
                }
            }
        }

        private void Preparing_TableFields(DataSet dsData, vTable vt)
        {
            //if (vt.TableName.IndexOf("SNL_") > -1) return;
            //if (vt.TableName.IndexOf("MS_TABLES") > -1)
            //    MessageBox.Show("");

            if (dsData.Tables.Count > 1) return;

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

            }
        }

        private void Preparing_TableFields_SQL(vTable vt,
               ref string sqlA, ref string sqlB)
        {
            #region bulut öncesi yapı
            /*
            string s =
            @" select  
               a.column_id, a.name 
             , convert(smallInt, a.system_type_id) system_type_id 
             , convert(smallInt, a.user_type_id)   user_type_id 
             , a.max_length, a.precision, a.scale, a.is_nullable, a.is_identity  
             , isnull(d.FFOREING, 0) FFOREING
             , isnull(d.FTRIGGER, 0) FTRIGGER
             , isnull(e.VALIDATION_INSERT, 0) VALIDATION_INSERT
             , d.PROP_EXPRESSION 
             , e.XML_FIELD_NAME
             , e.CMP_DISPLAY_FORMAT

             from [" + DatabaseName + @"].sys.columns a 
              left outer join [" + DatabaseName + @"].sys.tables b on (a.object_id = b.object_id )
              left outer join [" + v.active_DB.managerDBName + @"].dbo.MS_TABLES c on (b.name = c.TABLE_NAME)
              left outer join [" + v.active_DB.managerDBName + @"].dbo.MS_FIELDS d on (
                 a.name = d.FIELD_NAME 
                 and c.TABLE_CODE = d.TABLE_CODE
                 and a.system_type_id = d.FIELD_TYPE
                 )
              left outer join [" + v.active_DB.managerDBName + @"].dbo.MS_FIELDS_IP e on (
                 c.TABLE_CODE = d.TABLE_CODE 
                 and d.FIELD_NO = e.FIELD_NO
                 and e.IP_CODE = '" + IPCode + @"' 
                 )  
             where 0 = 0 
             and   b.name = '" + Table_Name + @"' 
             order by a.column_id ";
            */
            #endregion

            if (vt.DBaseType == v.dBaseType.MSSQL)
                sqlA =
                @" select  
               a.column_id, a.name 
             , convert(smallInt, a.system_type_id) system_type_id 
             , convert(smallInt, a.user_type_id)   user_type_id 
             , a.max_length, a.precision, a.scale, a.is_nullable, a.is_identity  
             from sys.columns a 
               left outer join sys.tables b on (a.object_id = b.object_id ) 
             where b.name = '" + vt.TableName + @"' 
             order by a.column_id ";

            //            from[" + vt.DBaseName + @"].sys.columns a
            //             left outer join[" + vt.DBaseName + @"].sys.tables b on(a.object_id = b.object_id)

            if (vt.DBaseType == v.dBaseType.MySQL)
                sqlA =
                @" SELECT COLUMN_NAME, ORDINAL_POSITION, DATA_TYPE, COLUMN_TYPE, EXTRA 
                   FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE table_name = '" + vt.TableName + "' ";
            // -- AND table_schema = 'msv3greenblt'



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
               left outer join dbo.MS_FIELDS d on ( c.TABLE_CODE = d.TABLE_CODE )
               left outer join dbo.MS_FIELDS_IP e on (
                     c.TABLE_CODE = d.TABLE_CODE 
                 and d.FIELD_NO = e.FIELD_NO
                 and e.IP_CODE = '" + vt.IPCode + @"' 
                 )  
             where c.TABLE_NAME = '" + vt.TableName + @"' 
             order by d.FIELD_NO 
            ";

            if ((vt.TableName.IndexOf("_KIST") > -1) ||
                (vt.TableName.IndexOf("_FULL") > -1))
                MessageBox.Show("Preparing_Fields_SQL : Table_Name.IndexOf(_KIST) / (_FULL) : konusu vardı.");
            //return "param";

        }


        public Boolean Sql_ExecuteNon(DataSet dsData, ref string SQL, vTable vt)
        {
            Boolean sonuc = false;

            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            SqlConnection msSqlConn = vt.msSqlConnection;
            MySqlConnection mySqlConn = vt.mySqlConnection;

            if (vt.DBaseType == v.dBaseType.MSSQL)
                Db_Open(msSqlConn);
            if (vt.DBaseType == v.dBaseType.MySQL)
                Db_Open(mySqlConn);

            SqlCommand SqlComm = null;
            MySqlCommand MySqlComm = null;

            try
            {
                SQL = SQLPreparing(SQL, vt);

                if (vt.DBaseType == v.dBaseType.MSSQL)
                {
                    SqlComm = new SqlCommand(SQL, msSqlConn);
                    SqlComm.ExecuteNonQuery();
                }
                if (vt.DBaseType == v.dBaseType.MySQL)
                {
                    MySqlComm = new MySqlCommand(SQL, mySqlConn);
                    MySqlComm.ExecuteNonQuery();
                }
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

                    if (vt.DBaseType == v.dBaseType.MySQL)
                        MySqlComm.ExecuteNonQuery().ToString();

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

            if (vt.DBaseType == v.dBaseType.MySQL)
                MySqlComm.Dispose();

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            return sonuc;
        }

        private Boolean Sql_Execute(DataSet dsData, ref string SQL, vTable vt)
        {
            Boolean onay = false;

            SqlConnection msSqlConn = vt.msSqlConnection;
            MySqlConnection mySqlConn = vt.mySqlConnection;

            if (vt.DBaseType == v.dBaseType.MSSQL)
                Db_Open(msSqlConn);
            if (vt.DBaseType == v.dBaseType.MySQL)
                Db_Open(mySqlConn);

            SqlDataAdapter msSqlAdapter = null;
            MySqlDataAdapter mySqlAdapter = null;

            #region 1. adımda DATA dolduruluyor
            try
            {
                //if (vt.DBaseType == v.dBaseType.MySQL)
                //{
                //    SQL = SQL.Replace("[", "");
                //    SQL = SQL.Replace("]", "");
                //}

                /// sql execute oluyor
                if (vt.DBaseType == v.dBaseType.MSSQL)
                    msSqlAdapter = new SqlDataAdapter(SQL, msSqlConn);
                if (vt.DBaseType == v.dBaseType.MySQL)
                    mySqlAdapter = new MySqlDataAdapter(SQL, mySqlConn);

                if (vt.Cargo == "data")
                {
                    //if (dsData.Tables.Count > 0)
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
                if (vt.DBaseType == v.dBaseType.MySQL)
                    mySqlAdapter.Fill(dsData, vt.TableName);

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

                if (vt.DBaseType == v.dBaseType.MySQL)
                {
                    if (mySqlConn.State == System.Data.ConnectionState.Closed)
                    {
                        v.SP_ConnBool_Project = false;
                        v.Kullaniciya_Mesaj_Var = "DİKKAT : MySQL Database bağlantısı koptu...";
                    }
                }

                if ((vt.DBaseType == v.dBaseType.MySQL) &&
                    (e.Message == "Fatal error encountered during command execution."))
                {
                    Db_Open(mySqlConn);
                    mySqlAdapter.Fill(dsData, vt.TableName);

                    v.SQL =
                        v.ENTER2 + "[ 2. Data_Read_Execute : " +
                        vt.TableName + " / " + vt.TableIPCode + " / " + vt.functionName +
                        " ] { " + v.ENTER2 + SQL + v.ENTER2 + " } " + v.SQL;
                }
                else
                {
                    onay = false;
                    Cursor.Current = Cursors.Default;

                    //if (v.SP_ConnectBool)
                    //{
                    v.SQL = v.ENTER2 +
                        "   HATALI SQL :  [ " + e.Message.ToString() + v.ENTER2 +
                        vt.TableName + " / " + vt.TableIPCode + " / " + vt.functionName +
                        " ] { " + v.ENTER2 + SQL + v.ENTER2 + " } " + v.SQL;

                    MessageBox.Show("HATALI SQL : " + v.ENTER2 + e.Message.ToString() +
                        v.ENTER2 + vt.TableName + v.ENTER2 + SQL, "Data_Read_Execute");
                    //}
                }
            }
            #endregion 1.adım

            if (msSqlAdapter != null) msSqlAdapter.Dispose();
            if (mySqlAdapter != null) mySqlAdapter.Dispose();

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

                if (tableName != "GROUPS")
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

        #region SQL_ExecuteNon

        public Boolean SQL_ExecuteNon(DataSet dsData, ref string SQL, string Kim)
        {
            // SQL çalıştırır ve geriye hiç sonuç vermez

            Boolean sonuc = false;
            /*
                        #region Eksik bilgiler varsa tamamlanacak

                        SqlConnection SqlConn = null;
                        string FieldsListSQL = string.Empty;
                        string function_name = "SQL ExecuteNon"; 
                        string TableName = string.Empty;
                        int TableCount = 0;
                        byte dbName = 0;

                        // Gerekli olan verileri topla
                        Preparing_DB_Variables(dsData, ref dbName, ref TableName, ref FieldsListSQL, ref TableCount);

                        #endregion Eksik bilgiler varsa tamamlanacak

                        #region SqlConn == Empty
                        if (SqlConn.ConnectionString == string.Empty)
                        {
                            MessageBox.Show("DİKKAT : Bağlantı cümlesi yok..." + v.ENTER2 +
                                            SQL, function_name);
                            return sonuc;
                        }
                        #endregion

                        #region SQL != Empty

                        if (SQL != "")
                        {
                            Db_Open(SqlConn);

                            SQL = SQLPreparing(SQL);

                            v.SQL = v.ENTER2 + SQL + v.SQL;

                            SqlCommand SqlKomut = new SqlCommand(SQL, SqlConn);

                            try
                            {
                                SqlKomut.ExecuteNonQuery();
                                sonuc = true;
                            }
                            catch (Exception e)
                            {
                                sonuc = false;

                                if (v.SP_ConnectBool)
                                {
                                    MessageBox.Show("HATALI İŞLEM : " + v.ENTER2 + e.Message, function_name + " [ " + Kim + " ]");
                                }
                                else 
                                {
                                    if (Db_Open(SqlConn))
                                    {
                                        SqlKomut.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        MessageBox.Show("DİKKAT : Bağlantı kurulamadı...", function_name);
                                    }
                                }
                            }

                            if (sonuc == true)
                            {
                                try
                                {
                                    string Adet = SqlKomut.ExecuteNonQuery().ToString();
                                    if (Adet != "-1")
                                        v.Kullaniciya_Mesaj_Var = Adet + " adet kayıt üzerinde işlem gerçekleşti ...";
                                }
                                catch
                                {
                                    // 
                                }
                            }

                            SqlKomut.Dispose();

                        } // if (SQL != "")
                        #endregion
            */
            return sonuc;
        }

        public Boolean SQL_ExecuteNon(SqlConnection SqlConn, DataSet dsData, ref string SQL, string Kim)
        {
            // SQL çalıştırır ve geriye hiç sonuç vermez

            //#region Tanımlar
            Boolean sonuc = false;
            /*          string function_name = "SQL ExecuteNon";

                      if (SqlConn.ConnectionString == string.Empty)
                      {
                          MessageBox.Show("DİKKAT : Bağlantı cümlesi yok..." + v.ENTER2 +
                                          SQL, function_name);
                          return sonuc;
                      }
                      #endregion Tanımlar

                      #region SQL != Empty

                      if (SQL != "")
                      {
                          Db_Open(SqlConn);

                          SQL = SQLPreparing(SQL);

                          v.SQL = v.ENTER2 + SQL + v.SQL;

                          SqlCommand SqlKomut = new SqlCommand(SQL, SqlConn);

                          try
                          {
                              SqlKomut.ExecuteNonQuery();
                              sonuc = true;
                          }
                          catch (Exception e)
                          {
                              sonuc = false;

                              if (v.SP_ConnectBool)
                              {
                                  MessageBox.Show("HATALI İŞLEM : " + v.ENTER2 + e.Message, function_name + " [ " + Kim + " ]");
                              }
                              else
                              {
                                  if (Db_Open(SqlConn))
                                  {
                                      SqlKomut.ExecuteNonQuery();
                                  }
                                  else
                                  {
                                      MessageBox.Show("DİKKAT : Bağlantı kurulamadı...", function_name);
                                  }
                              }
                          }

                          if (sonuc == true)
                          {
                              try
                              {
                                  string Adet = SqlKomut.ExecuteNonQuery().ToString();
                                  if ((Adet != "-1") && (Adet != "0"))
                                      v.Kullaniciya_Mesaj_Var = Adet + " adet kayıt üzerinde işlem gerçekleşti ...";
                              }
                              catch
                              {
                                  // 
                              }
                          }

                          SqlKomut.Dispose();

                      } // if (SQL != "")
                      #endregion
          */
            return sonuc;
        }

        #endregion SQL_ExecuteNon

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

        #region TableRowDelete
        public Boolean TableRowDelete(DataSet ds, int TableNo)
        {
            Boolean sonuc = true;

            if (ds == null) return sonuc;
            if (ds.Tables.Count == 0) return sonuc;
            if (ds.Tables[TableNo].Rows.Count == 0) return sonuc;
            try
            {
                for (int i = ds.Tables[TableNo].Rows.Count - 1; i >= 0; i--)
                {
                    ds.Tables[TableNo].Rows[i].Delete();
                }
                ds.Tables[TableNo].AcceptChanges();
            }
            catch (Exception e)
            {
                sonuc = false;
                MessageBox.Show("HATALI : " + v.ENTER2 + e.Message, "TableRemove");
            }

            return sonuc;
        }
        #endregion TableRowDelete

        #region TableClean
        public Boolean TableClean(Form tForm, DataSet ds)
        {
            Boolean sonuc = true;

            // Table_Type = 1.Table olmalı
            if (ds == null) return sonuc;
            if (ds.Tables.Count == 0) return sonuc;

            string myProp = ds.Namespace.ToString();
            string TableLabel = MyProperties_Get(myProp, "TableLabel:");
            string Kisitlama = MyProperties_Get(myProp, "Kisitlama:");
            string Sql = MyProperties_Get(myProp, "SqlFirst:");

            if (IsNotNull(Kisitlama))
            {
                #region  
                int ga = Sql.IndexOf("/*KISITLAMALAR|1|*/");
                if (ga == -1)
                {
                    int i_bgn = Sql.IndexOf("/*" + TableLabel + ".DEFAULT_VALUE*/") + 20 + TableLabel.Length; //18 + 2;

                    Sql = Sql.Insert(i_bgn, Kisitlama);
                }

                try
                {
                    string TableIPCode = Set(ds.DataSetName.ToString(), "", "");

                    Control cntrl = new Control();
                    cntrl = Find_Control_View(tForm, TableIPCode);

                    Data_Read_Execute(ds, ref Sql, "", null);

                    // her halükarda viev kullanılı hale gelsin
                    if (cntrl != null) cntrl.Enabled = true;

                    sonuc = true;

                }
                catch (Exception)
                {
                    sonuc = false;
                    throw;
                }
                #endregion
            }

            return sonuc;
        }
        #endregion TableClean

        #region TableRefresh

        public Boolean TableRefresh(Form tForm, DataSet ds, string TableIPCode)
        {
            Boolean sonuc = true;

            sonuc = TableRefresh(tForm, ds);

            ButtonEnabledAll(tForm, TableIPCode, true);
                        
            return sonuc;
        }

        public Boolean TableRefresh(Form tForm, DataSet ds)
        {
            Boolean sonuc = true;

            // Table_Type = 1.Table olmalı
            if (ds == null) return sonuc;
            //if (ds.Tables.Count == 0) return sonuc;
            //if (IsNotNull(ds) == false) return sonuc;

            //string Sql = ds.Tables[0].Namespace.ToString();

            string myProp = ds.Namespace.ToString();
            string Sql = Set(MyProperties_Get(myProp, "SqlSecond:"),
                             MyProperties_Get(myProp, "SqlFirst:"), "");

            if (IsNotNull(Sql))
            {
                #region
                try
                {
                    string TableIPCode = Set(ds.DataSetName.ToString(), "", "");

                    Control cntrl = new Control();
                    cntrl = Find_Control_View(tForm, TableIPCode);

                    Data_Read_Execute(ds, ref Sql, "", cntrl);

                    sonuc = true;
                }
                catch (Exception)
                {
                    sonuc = false;
                    throw;
                }
                #endregion
            }

            v.con_Refresh = false;
            return sonuc;
        }
        
        #endregion TableRefresh

        #region TableRowGet
        public Boolean TableRowGet(Form tForm, string TableIPCode)
        {
            Boolean sonuc = true;

            v.con_DataRow = null;

            DataSet ds = null;
            DataNavigator dN = null;

            //ds = Find_DataSet(tForm, "", TableIPCode, "");
            Find_DataSet(tForm, ref ds, ref dN, TableIPCode);

            if (ds == null) return sonuc;

            //int pos = Find_DataNavigator_Position(tForm, TableIPCode);

            if (IsNotNull(ds))
            {
                #region
                try
                {
                    //v.con_DataRow = ds.Tables[0].Rows[pos];
                    v.con_DataRow = ds.Tables[0].Rows[dN.Position];
                    sonuc = true;
                }
                catch (Exception)
                {
                    v.con_DataRow = null;
                    sonuc = false;
                }
                #endregion
            }

            return sonuc;
        }
        #endregion TableRowGet

        #region TableFieldValueGet
        public string TableFieldValueGet(Form tForm, string TableIPCode, string FieldName)
        {
            string value = string.Empty;

            v.con_DataRow = null;

            DataSet ds = Find_DataSet(tForm, "", TableIPCode, "");
            if (ds == null) return value;

            int pos = Find_DataNavigator_Position(tForm, TableIPCode);

            if (IsNotNull(ds))
            {
                #region
                // fieldname boş ise row isteniyor
                if (FieldName != "")
                {
                    try
                    {
                        value = ds.Tables[0].Rows[pos][FieldName].ToString();
                    }
                    catch (Exception)
                    {
                        value = string.Empty;
                    }
                }
                #endregion
            }

            return value;
        }
        #endregion TableFieldValueGet

        #region DBConnectState

        public void DBConnectStateProject(object sender, StateChangeEventArgs e)
        {
            v.SP_ConnBool_Project = (e.CurrentState == ConnectionState.Open);

            if (v.SP_ConnBool_Project == false)
                v.Kullaniciya_Mesaj_Var = "DİKKAT : MySQL Database bağlantısı koptu...";

            //this._previousState = e.OriginalState;
            //this._connectionState = e.CurrentState;
            //log.WriteLog("Connection change status: previous " + e.OriginalState.ToString() + ", current " + e.CurrentState.ToString(), log.INFO);

            //09.10.41: Connection change status: previous Closed, current Open
            //09.10.41: Connection change status: previous Open, current Closed
            //09.10.51: Connection change status: previous Closed, current Open
            //09.10.51: Connection change status: previous Open, current Closed

            /*
            if (v.SP_ConnBool_Project != v.SP_ConnBool_Project_Old)
            {
                v.SP_Conn_Caption = " " + v.SP_SERVER_PCNAME + " . " + v.active_DB.projectDBName + " ";
                if (v.SP_ConnBool_Project)
                {
                    v.SP_Conn_Caption = "SQL : " + v.SP_Conn_Caption;
                }
                else
                {
                    v.SP_Conn_Caption = "Bağlantı Yok : " + v.SP_Conn_Caption;

                    if ((v.SP_ConnBool_Project == false) && (v.SP_ConnBool_Project_Old))
                    {
                        MessageBox.Show("DİKKAT : Server ve Database bağlantı sorunu..." + v.ENTER2 +
                          "Server Name :  " + v.SP_SERVER_PCNAME + "  " + v.ENTER +
                          "Database Name :  " + v.active_DB.projectDBName + "  " + v.ENTER  
                          , "Önemli Uyarı");

                        // ((MySqlConnection)sender).
                    }
                }

                v.SP_ConnBool_Project_Old = v.SP_ConnBool_Project;

            } // if
            */

        }

        public void DBConnectStateManager(object sender, StateChangeEventArgs e)
        {
            v.SP_ConnBool_Manager = (e.CurrentState == ConnectionState.Open);

            //this._previousState = e.OriginalState;
            //this._connectionState = e.CurrentState;
            //log.WriteLog("Connection change status: previous " + e.OriginalState.ToString() + ", current " + e.CurrentState.ToString(), log.INFO);

            //09.10.41: Connection change status: previous Closed, current Open
            //09.10.41: Connection change status: previous Open, current Closed
            //09.10.51: Connection change status: previous Closed, current Open
            //09.10.51: Connection change status: previous Open, current Closed

            if (v.SP_ConnBool_Manager != v.SP_ConnBool_Manager_Old)
            {
                v.SP_Conn_Caption = " " + v.active_DB.managerServerName + " . " + v.active_DB.managerDBName + " ";
                if (v.SP_ConnBool_Manager)
                {
                    v.SP_Conn_Caption = "SQL : " + v.SP_Conn_Caption;
                }
                else
                {
                    v.SP_Conn_Caption = "Bağlantı Yok : " + v.SP_Conn_Caption;

                    if ((v.SP_ConnBool_Manager == false) && (v.SP_ConnBool_Manager_Old))
                    {
                        MessageBox.Show("DİKKAT : Server ve Database bağlantı sorunu..." + v.ENTER2 +
                          "Server Name :  " + v.active_DB.managerServerName + "  " + v.ENTER +
                          "Database Name :  " + v.active_DB.managerDBName + "  ", "Önemli Uyarı");
                    }
                }

                v.SP_ConnBool_Manager_Old = v.SP_ConnBool_Manager;

            } // if
        }

        #endregion DBConnectState

        #region ViewControl_Enabled

        public void ViewControl_Enabled_ExtarnalIP(Form tForm, DataSet dsData)
        {
            ///= External_IP:True;
            ///= External_TableIPCode:HPFNS.HPFNS_KS02;
            ///
            string TableIPCode = "";
            string myProp = dsData.Namespace.ToString();

            /// External_IP=True
            /// bir ıp ye birden fazla extra_IP bağlı olabilir 

            if (myProp.IndexOf("External_IP:True") > -1)
            {

                while (myProp.IndexOf("External_TableIPCode:") > -1)
                {
                    TableIPCode = MyProperties_Get(myProp, "External_TableIPCode:");

                    ViewControl_Enabled(tForm, dsData, TableIPCode);

                    Get_And_Clear(ref myProp, "External_TableIPCode:");
                }

            }

        }

        public void ViewControl_Enabled(Form tForm, DataSet dsData, string TableIPCode) //Control cntrl)
        {
            // Control Enabled
            Control cntrl = new Control();
            cntrl = Find_Control_View(tForm, TableIPCode);
            if (cntrl != null)
            {
                vTableAbout vTA = new vTableAbout();
                Table_About(vTA, dsData);

                // RecordCount aslında rowCount u verir
                // rowCount varsa enabled=true  onun  harici enabled=false olacak
                if (vTA.RecordCount > 0)
                {
                    //if (TableIPCode == "HPFNS.HPFNS_BN01")
                    //    MessageBox.Show("true");

                    cntrl.Enabled = true;
                }
                else
                {
                    //if (TableIPCode == "HPFNS.HPFNS_BN01")
                    //    MessageBox.Show("false");

                    cntrl.Enabled = false;

                    Control btn = null;
                    string[] controls = new string[] { };
                    btn = Find_Control(tForm, "simpleButton_yeni_hesap", TableIPCode, controls);

                    if (btn != null)
                    {
                        if (((DevExpress.XtraEditors.SimpleButton)btn).Visible)
                        {
                            if (((DevExpress.XtraEditors.SimpleButton)btn).Tag != null)
                                ((DevExpress.XtraEditors.SimpleButton)btn).Text =
                                    ((DevExpress.XtraEditors.SimpleButton)btn).Tag.ToString();
                        }
                    }

                }
            }
        }

        #endregion


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

        #region DataRowCopy

        public DataRow DataRowCopy(DataSet dsData, int position, string Key_FieldName)
        {
            DataRow New_Row = null;
            int ColumnCount = dsData.Tables[0].Columns.Count;

            New_Row = dsData.Tables[0].NewRow();

            for (int i = 0; i < ColumnCount; i++)
            {
                if (dsData.Tables[0].Columns[i].ColumnName != Key_FieldName)
                {
                    try
                    {
                        New_Row[i] = dsData.Tables[0].Rows[position][i];
                    }
                    catch (Exception)
                    {
                        //
                    }
                }
            }

            return New_Row;
        }

        #endregion DataRowCopy

        #endregion Database İşlemleri

        #region *SQL Syntax

        #region IfExists_Preparing

        public string IfExists_Preparing(string SorguSQL, string SonucVarsa_SQL, string SonucYoksa_SQL)
        {
            string NewSql = string.Empty;

            NewSql = @"
            IF EXISTS ( 
            /*SORGU*/
            " + SorguSQL + @"
            )
            BEGIN
              use " + v.active_DB.projectDBName + @"
              /*VARSA*/ -- print 'varsa'
              " + SonucVarsa_SQL + @" 
            END ELSE BEGIN
              use " + v.active_DB.projectDBName + @"
              /*YOKSA*/ -- print 'yoksa'
              " + SonucYoksa_SQL + @" 
            END ";

            return NewSql;
        }

        #endregion IfExists_Preparing

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

            // İPTAL
            //Str_Replace(ref Sql, ":VT_SHOP_ID", v.vt_SHOP_ID.ToString());
            //Str_Replace(ref Sql, ":SHOP_ID", v.vt_SHOP_ID.ToString());

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

        #region SQL_Select_Table
        public string SQL_Select_Table(string TableName)
        {
            return " Select * from " + TableName + " where 0 = 0 ";
        }
        #endregion SQL_Select_Table

        #region SQLWhereAdd
        public string SQLWhereAdd(string Sql, string tLabel, string WhereAnd, string Header)
        {
            string tHeader = string.Empty;
            string FullHeader1 = string.Empty;
            string FullHeader2 = string.Empty;

            if (Header.IndexOf("DEFAULT") > -1) tHeader = "DEFAULT_VALUE";
            if (Header.IndexOf("KRITER") > -1) tHeader = "KRITERLER";

            FullHeader1 = "/*" + tHeader + "*/" + v.ENTER;
            FullHeader2 = "/*" + tLabel + "." + tHeader + "*/" + v.ENTER;

            if (IsNotNull(WhereAnd))
            {
                /*DEFAULT_VALUE*/
                /*KRITERLER*/
                int f1 = 0;
                int i1 = Sql.IndexOf(FullHeader1, f1);
                while ((i1 > f1) && (f1 > -1))
                {
                    Sql = Sql.Insert((i1 + FullHeader1.Length) - 2, v.ENTER + WhereAnd);
                    f1 = Sql.IndexOf(FullHeader1, i1 + FullHeader1.Length);
                    if (f1 > 0)
                    {
                        i1 = f1;
                        f1 = 0;
                    }
                }

                /*[XXX].DEFAULT_VALUE*/
                /*[XXX].KRITERLER*/
                int f2 = 0;
                int i2 = Sql.IndexOf(FullHeader2, f2);
                while ((i2 > f2) && (f2 > -1))
                {
                    Sql = Sql.Insert((i2 + FullHeader2.Length) - 2, v.ENTER + WhereAnd);
                    f2 = Sql.IndexOf(FullHeader2, i2 + FullHeader2.Length);
                    if (f2 > 0)
                    {
                        i2 = f2;
                        f2 = 0;
                    }
                }

            }

            return Sql;
        }
        #endregion SQLWhereAdd

        #endregion SQL Syntax

        #region *Form View


        public void OpenForm_JSON(Form tForm, PROP_NAVIGATOR prop_)
        {

            //'FORMTYPE',      0,  '',       'none')
            //'FORMTYPE',      0,  'CHILD',  'ChildForm')
            //'FORMTYPE',      0,  'NORMAL', 'NormalForm')
            //'FORMTYPE',      0,  'DIALOG', 'DialogForm')

            //'FORMSTATE',     0,  '',       'none')
            //'FORMSTATE',     0,  'NORMAL', 'Normal')
            //'FORMSTATE',     0,  'MAX',    'Max')
            //'FORMSTATE',     0,  'MIN',    'Min')

            string FORMNAME = Set(prop_.FORMNAME.ToString(), "", "");
            string FORMCODE = Set(prop_.FORMCODE.ToString(), "", "");
            string FORMTYPE = Set(prop_.FORMTYPE.ToString(), "", "");
            string FORMSTATE = Set(prop_.FORMSTATE.ToString(), "", "");

            /// liste ekranları üzerindeki seçilen kaydı açmak için
            /// okunması gereken liste IP leri oku ve 
            /// okunan bu value leri MSETVALUE ye set et
            /// 
            OpenForm_ReadValues_JSON(tForm, prop_.TABLEIPCODE_LIST);

            string myFormLoadValue = JsonConvert.SerializeObject(prop_.TABLEIPCODE_LIST);

            #region 
            // form code var ise boş formu kullanarak Layout dizayn et
            if ((FORMCODE != "") &&
                (FORMNAME == ""))
                FORMNAME = "ms_Form";

            tForms fr = new tForms();

            Form tNewForm = null;

            v.con_FormLoadValue = myFormLoadValue;

            if (IsNotNull(FORMNAME))
            {
                tNewForm = fr.Get_Form(FORMNAME);

                // MS_LAYOUT Preparing
                // form code var ise boş formu kullanarak Layout dizayn et
                if (FORMCODE != "")
                {
                    tLayout l = new tLayout();
                    l.Create_Layout(tNewForm, FORMCODE);
                }

                if (tNewForm != null)
                {
                    /// ?? nedir 
                    tNewForm.Opacity = 50;
                    //tNewForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

                    /// refresh özelliğini taşı
                    /// açılan newForm da kayıt gerçekleşirse on 
                    tNewForm.HelpButton = tForm.HelpButton;

                    #region
                    if (IsNotNull(FORMTYPE))
                    {
                        if (FORMTYPE == "CHILD")
                        {
                            ChildForm_View(tNewForm, Application.OpenForms[0], myFormLoadValue);
                        }
                        if (FORMTYPE == "NORMAL")
                        {
                            if (FORMSTATE == "NORMAL")
                                NormalForm_View(tNewForm, FormWindowState.Normal, myFormLoadValue);
                            if (FORMSTATE == "MAX")
                                NormalForm_View(tNewForm, FormWindowState.Maximized, myFormLoadValue);
                            if (FORMSTATE == "MIN")
                                NormalForm_View(tNewForm, FormWindowState.Minimized, myFormLoadValue);
                        }
                        if (FORMTYPE == "DIALOG")
                        {
                            if (FORMSTATE == "NORMAL")
                                DialogForm_View(tNewForm, FormWindowState.Normal, myFormLoadValue);
                            if (FORMSTATE == "MAX")
                                DialogForm_View(tNewForm, FormWindowState.Maximized, myFormLoadValue);
                            if (FORMSTATE == "MIN")
                                DialogForm_View(tNewForm, FormWindowState.Minimized, myFormLoadValue);
                        }
                    }
                    else
                    {
                        if (FORMTYPE == "CHILD")
                        {
                            ChildForm_View(tNewForm, Application.OpenForms[0], myFormLoadValue);
                        }
                    }
                    #endregion
                }
                else MessageBox.Show("Form bulunamadı...");

            }
            #endregion
        }

        private void OpenForm_ReadValues_JSON(Form tForm, List<TABLEIPCODE_LIST> TableIPCodeList)
        {
            DataSet ds = null;
            DataNavigator dN = null;

            string WORKTYPE = string.Empty;
            string RTABLEIPCODE = string.Empty;
            string RKEYFNAME = string.Empty;
            string ReadValue = string.Empty;

            foreach (var item in TableIPCodeList)
            {
                WORKTYPE = item.WORKTYPE.ToString();
                RTABLEIPCODE = Set(item.RTABLEIPCODE.ToString(), "", "");
                RKEYFNAME = Set(item.RKEYFNAME.ToString(), "", "");

                #region 1
                if (WORKTYPE == "NEW")
                {
                    ReadValue = "0";
                }
                #endregion

                #region 2
                if (IsNotNull(RTABLEIPCODE) &&
                    IsNotNull(RKEYFNAME) &&
                    ((WORKTYPE == "READ") ||
                     (WORKTYPE == "SVIEW") ||
                     (WORKTYPE == "SVIEWVALUE") ||
                     (WORKTYPE == "CREATEVIEW"))
                    )
                {
                    ds = Find_DataSet(tForm, "", RTABLEIPCODE, "");
                    Find_DataSet(tForm, ref ds, ref dN, RTABLEIPCODE);

                    if (IsNotNull(ds))
                    {
                        ReadValue = ds.Tables[0].Rows[dN.Position][RKEYFNAME].ToString();
                    }
                }
                #endregion

                #region 3
                if (IsNotNull(ReadValue))
                {
                    item.MSETVALUE = ReadValue;
                }
                #endregion
            }
        }

        public void OpenForm(Form tForm, string Prop_Navigator)
        {
            string s1 = "=ROW_PROP_NAVIGATOR:";
            string s2 = (char)34 + "TABLEIPCODE_LIST" + (char)34 + ": [";
            // "TABLEIPCODE_LIST": [

            string s3 = "=TABLEIPCODE_LIST:";
            string s4 = "=FORMCODE:";

            if (Prop_Navigator.IndexOf(s1) > -1)
                OpenForm_OLD(tForm, Prop_Navigator);
            else if (Prop_Navigator.IndexOf(s3) > -1)
                OpenForm_OLD(tForm, Prop_Navigator);
            else if (Prop_Navigator.IndexOf(s4) > -1)
                OpenForm_OLD(tForm, Prop_Navigator);
            else if (Prop_Navigator.IndexOf(s2) > -1)
            {
                Str_Remove(ref Prop_Navigator, "|ds|");

                var prop_ = JsonConvert.DeserializeObject(Prop_Navigator);

                OpenForm_JSON(tForm, (PROP_NAVIGATOR)prop_);
            } else
            {
                MessageBox.Show("DİKKAT : Tanımlayamadığım bir PROP_NAVIGATOR cümlesi geldi...");
            }
        }

        private void OpenForm_OLD(Form tForm, string Prop_Navigator)
        {
            #region örnek
            /*
            PROP_NAVIGATOR={
            1=ROW_PROP_NAVIGATOR:1;
            1=CAPTION:KART AÇ;             <<< Hangi butona basıldığına dair bilgi  
            1=BUTTONTYPE:51;
             * 
            1=TABLEIPCODE_LIST:TABLEIPCODE_LIST={   <<<<< field bloğu        <<< okunacak TABLEIPCODE ler
            1=ROW_TABLEIPCODE_LIST:1;               <<<<< row bloğu
            1=CAPTION:Kart aç;
            1=TABLEIPCODE:TSTDTL.TSTDTL_03;
            1=TABLEALIAS:[TSTDTL];
            1=KEYFNAME:MSTR_REF_ID;
            1=RTABLEIPCODE:TSTMSTR.TSTMSTR_01;
            1=RKEYFNAME:REF_ID;
            1=MSETVALUE:null;
            1=WORKTYPE:READ;
            1=ROWE_TABLEIPCODE_LIST:1;
             * 
            2=ROW_TABLEIPCODE_LIST:2;
            2=CAPTION:Cari Hesabı Oku;
            2=TABLEIPCODE:HP_CARI.HP_CARI_02;
            2=TABLEALIAS:[HPCARI];
            2=KEYFNAME:REF_ID;
            2=RTABLEIPCODE:TSTMSTR.TSTMSTR_01;
            2=RKEYFNAME:CARI_ID;
            2=MSETVALUE:null;
            2=WORKTYPE:READ;
            2=ROWE_TABLEIPCODE_LIST:2;
            TABLEIPCODE_LIST=};                 <<<<< 
             * 
            1=FORMNAME:TestFormu1;              <<< açılacak form bilgileri
            1=FORMCODE:null;
            1=FORMTYPE:DIALOG;
            1=FORMSTATE:NORMAL;
            1=ROWE_PROP_NAVIGATOR:1;
            PROP_NAVIGATOR=}
            */
            #endregion örnek

            //'FORMTYPE',      0,  '',       'none')
            //'FORMTYPE',      0,  'CHILD',  'ChildForm')
            //'FORMTYPE',      0,  'NORMAL', 'NormalForm')
            //'FORMTYPE',      0,  'DIALOG', 'DialogForm')

            //'FORMSTATE',     0,  '',       'none')
            //'FORMSTATE',     0,  'NORMAL', 'Normal')
            //'FORMSTATE',     0,  'MAX',    'Max')
            //'FORMSTATE',     0,  'MIN',    'Min')

            string FORMNAME = MyProperties_Get(Prop_Navigator, "FORMNAME:");
            string FORMCODE = MyProperties_Get(Prop_Navigator, "FORMCODE:");
            string FORMTYPE = MyProperties_Get(Prop_Navigator, "FORMTYPE:");
            string FORMSTATE = MyProperties_Get(Prop_Navigator, "FORMSTATE:");

            string TableIPCode_RowBlock = Find_Properies_Get_FieldBlock(Prop_Navigator, "TABLEIPCODE_LIST");

            TableIPCode_RowBlock = TableIPCodeList_Get_Values(tForm, TableIPCode_RowBlock);

            string myFormLoadValue = TableIPCode_RowBlock;

            // form code var ise boş formu kullanarak Layout dizayn et
            if ((FORMCODE != "") && (FORMNAME == ""))
                FORMNAME = "ms_Form";

            tForms fr = new tForms();

            Form tNewForm = null;

            v.con_FormLoadValue = myFormLoadValue;

            if (IsNotNull(FORMNAME))
            {
                tNewForm = fr.Get_Form(FORMNAME);

                // MS_LAYOUT Preparing
                // form code var ise boş formu kullanarak Layout dizayn et
                if (FORMCODE != "")
                {
                    tLayout l = new tLayout();
                    l.Create_Layout(tNewForm, FORMCODE);
                }

                if (tNewForm != null)
                {
                    tNewForm.Opacity = 50;
                    //tNewForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;

                    if (IsNotNull(FORMTYPE))
                    {
                        if (FORMTYPE == "CHILD")
                        {
                            ChildForm_View(tNewForm, Application.OpenForms[0], myFormLoadValue);
                        }
                        if (FORMTYPE == "NORMAL")
                        {
                            if (FORMSTATE == "NORMAL")
                                NormalForm_View(tNewForm, FormWindowState.Normal, myFormLoadValue);
                            if (FORMSTATE == "MAX")
                                NormalForm_View(tNewForm, FormWindowState.Maximized, myFormLoadValue);
                            if (FORMSTATE == "MIN")
                                NormalForm_View(tNewForm, FormWindowState.Minimized, myFormLoadValue);
                        }
                        if (FORMTYPE == "DIALOG")
                        {
                            if (FORMSTATE == "NORMAL")
                                DialogForm_View(tNewForm, FormWindowState.Normal, myFormLoadValue);
                            if (FORMSTATE == "MAX")
                                DialogForm_View(tNewForm, FormWindowState.Maximized, myFormLoadValue);
                            if (FORMSTATE == "MIN")
                                DialogForm_View(tNewForm, FormWindowState.Minimized, myFormLoadValue);
                        }
                    }
                    else
                    {
                        if (FORMTYPE == "CHILD")
                        {
                            ChildForm_View(tNewForm, Application.OpenForms[0], myFormLoadValue);
                        }
                    }
                }
                else MessageBox.Show("Form bulunamadı...");

            }
            //return tNewForm;
        }

        //----
        public void ChildForm_View(Form tForm, Form tMdiForm)
        {
            ChildForm_View(tForm, tMdiForm, string.Empty);
        }

        public void ChildForm_View(Form tForm, Form tMdiForm, string myFormLoadValue)
        {
            // Formun üzerinde MyFormBox isimli memo yok ise eklesin
            tCreateObject co = new tCreateObject();
            co.Create_MyFormBox(tForm, myFormLoadValue);

            tForm.Tag = "CHILD";
            if (tMdiForm.IsMdiContainer)
                tForm.MdiParent = tMdiForm;
            tForm.Show();
            ScreenSize(tForm);
        }

        public void ChildForm_View(Form tForm, Form tMdiForm, FormWindowState state)
        {
            ChildForm_View(tForm, tMdiForm, state, string.Empty);
        }

        public void ChildForm_View(Form tForm, Form tMdiForm, FormWindowState state, string myFormLoadValue)
        {
            // Formun üzerinde MyFormBox isimli memo yok ise eklesin
            tCreateObject co = new tCreateObject();
            co.Create_MyFormBox(tForm, myFormLoadValue);

            if (tMdiForm.IsMdiContainer)
                tForm.MdiParent = tMdiForm;

            tForm.Tag = "CHILD";
            tForm.WindowState = state;
            tForm.Show();
            ScreenSize(tForm);
        }
        //---
        public void NormalForm_View(Form tForm, FormWindowState state)
        {
            NormalForm_View(tForm, state, string.Empty);
        }

        public void NormalForm_View(Form tForm, FormWindowState state, string myFormLoadValue)
        {
            // Formun üzerinde MyFormBox isimli memo yok ise eklesin
            tCreateObject co = new tCreateObject();
            co.Create_MyFormBox(tForm, myFormLoadValue);

            tForm.Tag = "NORMAL";
            tForm.StartPosition = FormStartPosition.CenterScreen;
            tForm.WindowState = state;
            tForm.Show();
        }
        //---
        public void DialogForm_View(Form tForm, FormWindowState state)
        {
            DialogForm_View(tForm, state, string.Empty);
        }

        public void DialogForm_View(Form tForm, FormWindowState state, string myFormLoadValue)
        {
            // Formun üzerinde MyFormBox isimli memo yok ise eklesin
            tCreateObject co = new tCreateObject();
            co.Create_MyFormBox(tForm, myFormLoadValue);

            tForm.Tag = "DIALOG";
            tForm.TopMost = true;
            tForm.StartPosition = FormStartPosition.CenterScreen;
            tForm.WindowState = state;
            tForm.ShowDialog();
        }
        //---
        public void ScreenSize(Form tForm)
        {
            tForm.Top = 0;
            tForm.Left = 0;
            tForm.Width = v.Screen_Width;
            tForm.Height = v.Screen_Height;
        }


        #endregion Form View

        #region *String İşlemler

        #region TurkceKarakterDuzenle
        public string TurkceKarakterDuzenle(string metin)
        {
            // Artı olarak eklenmek ve replace edilmek istenen karakterler olur ise eklenebilir.

            metin = metin.Replace("Ğ", "G"); metin = metin.Replace("ğ", "g");
            metin = metin.Replace("Ü", "U"); metin = metin.Replace("ü", "u");
            metin = metin.Replace("Ş", "S"); metin = metin.Replace("ş", "s");
            metin = metin.Replace("İ", "I"); metin = metin.Replace("ı", "i");
            metin = metin.Replace("Ö", "O"); metin = metin.Replace("ö", "o");
            metin = metin.Replace("Ç", "C"); metin = metin.Replace("ç", "c");

            return metin;
        }
        #endregion

        #region myControl_Size_And_Location
        public void myControl_Size_And_Location(Control tControl,
                                                int width, int height, int left, int top)
        {
            if (width == 0) width = 250;
            if (height == 0) height = 250;

            if ((width > 0) || (height > 0))
                tControl.Size = new System.Drawing.Size(width, height);

            tControl.Location = new Point(left, top);
        }
        #endregion myControl_Size_And_Location

        #region myVisible

        public Boolean myVisible(string Veri)
        {
            Boolean onay;
            if (Veri.ToUpper() == "TRUE") onay = true;
            else onay = false;
            return onay;
        }

        #endregion 

        #region myBlock
        public string myBlock(string prp_type, string Kim)
        {
            string s = string.Empty;
            if (prp_type == "tPropertiesEdit")
            {
                //lockB
                if (Kim.ToUpper() == "BEGIN") s = "//<ALOCK_0>";
                //lockE
                if (Kim.ToUpper() == "END") s = "//<ALOCK_1>";
            }
            else
            {
                //lockB
                if (Kim.ToUpper() == "BEGIN") s = "//<BLOCK_0>";
                //lockE
                if (Kim.ToUpper() == "END") s = "//<BLOCK_1>";
            }

            return s;
        }
        #endregion myBlock


        #region myDouble

        public double myDouble(string Veri)
        {
            double j = 0.0;

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

            if (s == "") s = "0.0";

            try
            {
                j = Convert.ToDouble(s);

            }
            catch (Exception)
            {
                j = 0.0;  //throw;
            }

            return j;
        }

        #endregion myDouble

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

        #region myTamliBolen
        public int myTamliBolen(int Bolunen, int Bolen)
        {
            int kalan = 0;
            int adet = 0;
            adet = Bolunen / Bolen;
            kalan = Bolunen % Bolen;

            if (kalan > 0) adet++;

            return adet;
        }
        #endregion

        #region myFieldValue

        public string myFieldValue(DataSet ds, string GetFieldName, string FieldName, string Value)
        {
            string j = "";

            int i2 = ds.Tables[0].Rows.Count;
            for (int i = 0; i < i2; i++)
            {
                if (ds.Tables[0].Rows[i][FieldName].ToString() == Value)
                {
                    j = ds.Tables[0].Rows[i][GetFieldName].ToString();
                    break;
                }
            }

            return j;
        }

        public string myFieldValue(DataSet ds, string GetFieldName,
               string FieldName1, string Value1, string FieldName2, string Value2)
        {
            string j = "";

            int i2 = ds.Tables[0].Rows.Count;
            for (int i = 0; i < i2; i++)
            {
                if ((ds.Tables[0].Rows[i][FieldName1].ToString() == Value1) &&
                    (ds.Tables[0].Rows[i][FieldName2].ToString() == Value2))
                {
                    j = ds.Tables[0].Rows[i][GetFieldName].ToString();
                    break;
                }
            }

            return j;
        }

        #endregion myFieldValue

        #region myFieldValueCount
        public int myFieldValueCount(DataSet ds, string FieldName, string Value)
        {
            int j = 0;

            foreach (DataRow Row in ds.Tables[0].Rows)
            {
                if (Row[FieldName].ToString() == Value) j++;
            }

            return j;
        }
        #endregion myFieldValueCount

        #region myLkpOnayCount

        public int myLkpOnayCount(DataSet ds,
                                  int MinCount, string MinUyarisi,
                                  int MaxCount, string MaxUyarisi,
                                  string MaxIslemi)
        {
            int j = 0;

            foreach (DataRow Row in ds.Tables[0].Rows)
            {
                if (Row["LKP_ONAY"].ToString() == "1") j++;
            }

            if ((MinCount > -1) && (MinCount == j))
            {
                if (IsNotNull(MinUyarisi))
                {
                    MessageBox.Show(MinUyarisi);
                }
            }

            if ((MaxCount > -1) && (MaxCount < j))
            {
                if (IsNotNull(MaxUyarisi))
                {
                    MessageBox.Show(MaxUyarisi);
                }

                if (MaxIslemi == "SIL")
                {
                    foreach (DataRow Row in ds.Tables[0].Rows)
                    {
                        if (Row["LKP_ONAY"].ToString() == "1") Row["LKP_ONAY"] = 0;
                    }
                }

            }

            return j;
        }

        #endregion myLkpOnayCount

        #region myLkpOnayPositionSet

        public void myLkpOnayPosition(DataSet ds, DataNavigator tDataNavigator)
        {
            int j = 0;
            foreach (DataRow Row in ds.Tables[0].Rows)
            {
                if (Row["LKP_ONAY"].ToString() == "1")
                {
                    tDataNavigator.Position = j;
                    tDataNavigator.Tag = j;
                    break;
                }
                j++;
            }
        }

        #endregion myLkpOnayPositionSet

        #region myMaxValue

        public int myMaxValue(string Veri)
        {
            int j = 0;
            j = myInt32(Veri);

            if (j >= v.con_Value_Max)
            {
                v.con_Value_Max = j;
            }

            return v.con_Value_Max;
        }

        #endregion myMaxValue

        #region myMinValue

        public int myMinValue(string Veri)
        {
            int j = 0;
            j = myInt32(Veri);

            if (j <= v.con_Value_Min)
            {
                v.con_Value_Min = j;
            }

            return v.con_Value_Min;
        }

        #endregion myMinValue

        #region myOperandControl

        public Boolean myOperandControl(string Value1, string Value2, string OperandType)
        {
            Boolean onay = false;

            /* 'KRT_ODD_NOTLIKE_STR'
            1, '>=', '>=');
            2, '>', '>');
            3, '=', '=');
            4, '<=', '<=');
            5, '<', '<');
            8, '<>', '<>');
           18, 'in', 'in'
            */

            if (OperandType == ">=")
            {
                //if (myInt32(Value1) >= myInt32(Value2)) onay = true;
                if (myDouble(Value1) >= myDouble(Value2)) onay = true;
            }

            if (OperandType == ">")
            {
                //if (myInt32(Value1) > myInt32(Value2)) onay = true;
                if (myDouble(Value1) > myDouble(Value2)) onay = true;
            }

            if (OperandType == "=")
            {
                if (Value1 == Value2) onay = true;
            }

            if (OperandType == "<=")
            {
                //if (myInt32(Value1) <= myInt32(Value2)) onay = true;
                if (myDouble(Value1) <= myDouble(Value2)) onay = true;
            }

            if (OperandType == "<")
            {
                //if (myInt32(Value1) < myInt32(Value2)) onay = true;
                if (myDouble(Value1) < myDouble(Value2)) onay = true;
            }

            if (OperandType == "<>")
            {
                if (Value1 != Value2) onay = true;
            }

            if (OperandType == "in")
            {
                if (Value2.IndexOf(Value1) > -1) onay = true;
            }

            return onay;
        }


        #endregion myOperandControl

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

        #region Str_Remove

        public void Str_Remove(ref string Veri, string silinecek_ifade)
        {
            if (Veri.Length > 0)
            {
                int i1 = Veri.IndexOf(silinecek_ifade);
                int i2 = silinecek_ifade.Length;
                if (i1 > -1) Veri = Veri.Remove(i1, i2);
            }
        }

        #endregion Str_Remove

        #region String Parcala
        public void String_Parcala(string Veri, ref string ABlock, ref string BBlock, string Bolucu)
        {
            int i = 0;

            i = Veri.IndexOf(Bolucu);
            if (i > 0)
            {
                ABlock = Veri.Substring(0, i);
                Veri = Veri.Remove(0, i + Bolucu.Length);
                BBlock = Veri;
            }
            else
            {
                ABlock = "null";
                BBlock = "null";
            }

        }
        #endregion String Parcala

        #region IsData
        public Boolean IsData(ref string Veri, string ArananData)
        {
            Boolean onay = false;

            if (IsNotNull(Veri))
            {
                //int i1 = Veri.IndexOf("=" + ArananData + ":");
                //int i2 = Veri.IndexOf("=" + ArananData + ":FIN");
                int i1 = Veri.IndexOf(ArananData + "={");   // 0=AUTO_LST:AUTO_LST={
                int i2 = Veri.IndexOf(ArananData + "=};");  // AUTO_LST=};


                // 4 nedir  ={\r\n  ArananData sonudaki işaretler
                int i3 = ArananData.Length + 4;
                int i4 = i2 - (i1 + i3);

                if (i4 > 10)
                {
                    string s = Get_And_Clear(ref Veri, ArananData + "={");
                    s = Get_And_Clear(ref Veri, ArananData + "=}"); ;
                    Veri = s.Trim();
                    onay = true;
                }
            }

            return onay;

        }
        #endregion IsData

        #region IsFile
        public bool IsFile(string pathFileName)
        {
            bool onay = false;

            FileAttributes attributes = File.GetAttributes(pathFileName);

            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
                onay = true;

            return onay;
        }
        #endregion IsFile 

        #region IsDirectory
        public bool IsDirectory(string pathName)
        {
            bool onay = false;

            Str_Remove(ref pathName, "...");

            FileAttributes attributes = File.GetAttributes(pathName);

            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                onay = true;

            return onay;
        }
        #endregion IsDirectory


        #region UseDirectory
        public void UseDirectory(string pathName)
        {
            if (!Directory.Exists(pathName))
            {
                Directory.CreateDirectory(pathName);
            }
        }
        #endregion UseDirectory

        #region IsNull
        public string IsNull(string Veri, string Dolgu)
        {
            if ((Veri == null) || (Veri == ""))
                Veri = Dolgu;

            return Veri;
        }
        #endregion IsNull 

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

        public string Alias_Clear(string Veri)
        {
            int i = 0;
            string fname = Veri;

            i = Veri.IndexOf(".");
            if (i > 0)
            {
                fname = Veri.Substring(i + 1, Veri.Length - (i + 1));
            }

            return fname;
        }

        #region TableIPCode_Get
        public void TableIPCode_Get(string TableIPCode, ref string TableCode, ref string IPCode)
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

        #region Set_FieldName_Value

        public string Set_FieldName_Value(DataSet dsFields, string Field_Name, string fvalue, string SetType, string OperandType)
        {
            string MyStr = string.Empty;
            Int16 ftype = 0;

            foreach (DataRow Row in dsFields.Tables[0].Rows)
            {
                if (Row["LKP_FIELD_NAME"].ToString() == Field_Name)
                {
                    //ftype = Set(Row["user_type_id"].ToString(), "", (Int16)167);
                    ftype = Set(Row["LKP_FIELD_TYPE"].ToString(), "", (Int16)167);
                    break;
                }
            }

            if ((ftype == 0) && (IsNotNull(Field_Name) != false))
            {
                MessageBox.Show("DİKKAT : İşlem yapmak istediğiniz field [dsFields].[" + Field_Name + "] listesinde bulunamadı.  " + v.ENTER +
                " TableIPCode - Fields listesini kontrol ediniz. Tanımlı değildir ... ", "Set_FieldName_Value");
            }

            MyStr = Set_FieldName_Value_(ftype, Field_Name, fvalue, SetType, OperandType);
            return MyStr;
        }

        public string Set_FieldName_Value_(Int16 ftype, string Field_Name, string fvalue,
                                           string SetType, string OperandType)
        {
            string MyStr = string.Empty;
            string Operand = " = ";

            // SetType  ( and ) ise   MyStr = Field_Name + " = " + fvalue;
            // SetType  ( as  ) ise   MyStr = fvalue + " as " + Field_Name;
            // SetType  ( @   ) ise   MyStr = Field_Name + " = " + fvalue;
            // SetType  ( in  ) ise   
            //
            //   MyStr =     [K].GRUP_ID in (   
            //               / *[K].GRUP_ID_INLIST* /
            //                 2990
            //               , 2983
            //               / *[K].GRUP_ID_INEND* /
            //               )
            //

            // fvalue = -2 gelirse Tumu anlamına gelmektedir

            #region toperand_type
            /* 
                   0,  '' yani =  
                   1,  'Even (Double)'
                   2,  'Odd  (Single)'
                   3,  'Speed'
                   4,  'On/Off'
                   9,  'Visible=False'
                  11,  '>='
                  12,  '>'
                  13,  '<='
                  14,  '<'
                  15,  '<>'
                  16,  'Benzerleri (%abc%)'
                  17,  'Benzerleri (abc%)'
            */
            #endregion toperand_type

            if (OperandType == "null") Operand = "";
            if (OperandType == "=") Operand = " = ";
            if (OperandType == "0") Operand = " = ";
            if ((OperandType == "11") || (OperandType == ">=")) Operand = " >= ";
            if ((OperandType == "12") || (OperandType == ">")) Operand = " > ";
            if ((OperandType == "13") || (OperandType == "<=")) Operand = " <= ";
            if ((OperandType == "14") || (OperandType == "<")) Operand = " < ";
            if ((OperandType == "15") || (OperandType == "<>")) Operand = " <> ";

            if (SetType == "@")
            {
                // @GCB_ID   >>  GCB   oluyor
                // fieldname önünde bu işaret varsa silelim
                if (Field_Name.IndexOf("@") > -1)
                    Field_Name = Field_Name.Substring(1, Field_Name.Length - 1);
            }

            //* rakam  56, 48, 127, 52, 60, 62, 59, 108
            if ((ftype == 56) | (ftype == 48) | (ftype == 127) | (ftype == 52) |
                (ftype == 60) | (ftype == 62) | (ftype == 59) | (ftype == 108))
            {
                // Eğer veri (Rakam) yok ise Sıfır bas
                if ((fvalue == "") ||
                    (fvalue == null) ||
                    (fvalue == "null") ||
                    (fvalue == "NewRecord")) fvalue = "-1";

                if (fvalue == "first") fvalue = "-99";

                // rakam türü , ile ayrılmış ise , yerine . işareti değiştiriliyor 
                if (fvalue.IndexOf(",") > -1)
                    fvalue = fvalue.Replace(",", ".");

                // alınan değeri şekillendir
                if (SetType == "and") MyStr = Field_Name + Operand + fvalue;
                if ((SetType == "and") && (fvalue == "-2")) MyStr = Field_Name + " > " + fvalue;

                if (SetType == "as") MyStr = fvalue + " as " + Field_Name;
                if (SetType == "@") MyStr = "@" + Field_Name + Operand + fvalue;
                if (SetType == "in")
                    MyStr = Field_Name + " in ( " + v.ENTER +
                        "/*" + Field_Name + "_INLIST*/" + v.ENTER +
                        fvalue + v.ENTER +
                        "/*" + Field_Name + "_INEND*/" + v.ENTER + "  )  ";
            }

            //* text = (char)175, (varchar)167, (ntext)99, (text)35, (nchar)239
            if ((ftype == 175) | (ftype == 167) | (ftype == 99) | (ftype == 35) | (ftype == 239))
            {
                // Eğer veri yok ise null bas
                if ((fvalue == "") ||
                    (fvalue == null) ||
                    (fvalue == "NewRecord")) fvalue = "";

                //if (fvalue == "null") fvalue = "null"; ??? çözüm bul

                if (fvalue == "first") fvalue = "-99";

                if ((SetType == "and") && (fvalue != string.Empty)) MyStr = Field_Name + Operand + "'" + Str_Check(fvalue) + "' ";
                if ((SetType == "and") && (fvalue == "")) MyStr = Field_Name + Operand + " '' ";
                if ((SetType == "and") && (fvalue == "-2")) MyStr = Field_Name + " <> " + "'" + Str_Check(fvalue) + "' ";


                if (((SetType == "andlike") || (OperandType == "16")) && (fvalue != string.Empty))
                    MyStr = Field_Name + " like " + "'%" + Str_Check(fvalue) + "%' ";

                if (((SetType == "andlike") || (OperandType == "17")) && (fvalue != string.Empty))
                    MyStr = Field_Name + " like " + "'" + Str_Check(fvalue) + "%' ";

                if ((SetType == "andlike") && (fvalue == ""))
                    MyStr = Field_Name + " like '%' ";


                if ((SetType == "@") && (fvalue != string.Empty)) MyStr = "@" + Field_Name + Operand + "'" + fvalue + "' ";
                if ((SetType == "@") && (fvalue == "")) MyStr = "@" + Field_Name + Operand + " '' ";

                if (SetType == "as") MyStr = "'" + fvalue + "'" + " as " + Field_Name;

            }

            //* bit türü 104
            if (ftype == 104)
            {
                if ((fvalue == "") ||
                    (fvalue == null) ||
                    (fvalue == "null") ||
                    (fvalue == "NewRecord") ||
                    (fvalue == "0") ||
                    (fvalue == "False"))
                {
                    if (SetType == "and") MyStr = Field_Name + Operand + " 0 ";
                    if (SetType == "as") MyStr = " 0 " + " as " + Field_Name;
                    if (SetType == "@") MyStr = "@" + Field_Name + Operand + " 0 ";
                }
                else
                {
                    if (SetType == "and") MyStr = Field_Name + Operand + " 1 ";
                    if ((SetType == "and") && (fvalue == "-2")) MyStr = Field_Name + " > " + " -2 ";

                    if (SetType == "as") MyStr = " 1 " + " as " + Field_Name;
                    if (SetType == "@") MyStr = "@" + Field_Name + Operand + " 1 ";
                }
            }

            //* date 58
            if ((ftype == 58) || (ftype == 40) || (ftype == 61))
            {
                if (fvalue == "")
                    fvalue = DateTime.Now.Date.ToShortDateString();

                if ((fvalue == null) ||
                    (fvalue == "null") ||
                    (fvalue == "NewRecord") ||
                    (fvalue == "0"))
                {
                    MyStr = string.Empty;
                }
                else
                {
                    if (fvalue != "first")
                    {
                        //if (SetType == "and") MyStr = "Convert(Date, Convert(varchar(10)," + Field_Name + ", 103), 103) " + Operand + Tarih_Formati(Convert.ToDateTime(fvalue)) + " ";
                        
                        if ((v.active_DB.projectDBType == v.dBaseType.MSSQL) && (SetType == "and"))
                            MyStr = "Convert(Date, " + Field_Name + ", 103) " + Operand + Tarih_Formati(Convert.ToDateTime(fvalue)) + " ";

                        if ((v.active_DB.projectDBType == v.dBaseType.MySQL) && (SetType == "and"))
                            MyStr = "Convert(" + Field_Name + ", Date) " + Operand + Tarih_Formati(Convert.ToDateTime(fvalue)) + " ";

                        if (SetType == "as") MyStr = Tarih_Formati(Convert.ToDateTime(fvalue)) + " as " + Field_Name;
                        if (SetType == "@") MyStr = "@" + Field_Name + Operand + Tarih_Formati(Convert.ToDateTime(fvalue)) + " ";
                    }
                    if ((fvalue == "first") || (fvalue == "-99"))
                    {
                        //if (SetType == "and") MyStr = "Convert(Date, Convert(varchar(10)," + Field_Name + ", 103), 103) " + Operand + "Convert(Date,'01.01.1900', 103) ";

                        if ((v.active_DB.projectDBType == v.dBaseType.MSSQL) && (SetType == "and"))
                            MyStr = "Convert(Date, " + Field_Name + ", 103) " + Operand + "Convert(Date,'01.01.1900', 103) ";

                        if ((v.active_DB.projectDBType == v.dBaseType.MySQL) && (SetType == "and"))
                            MyStr = "Convert(" + Field_Name + ", Date) " + Operand + "Convert('01.01.1900', Date) ";

                        if (SetType == "as") MyStr = "'01.01.1900'" + " as " + Field_Name;
                        if (SetType == "@") MyStr = "@" + Field_Name + Operand + "'01.01.1900'" + " ";
                    }
                }
            }

            return MyStr;
        }

        #endregion Set_FieldName_Value

        #region Alias Control
        public void Alias_Control(ref string alias)
        {
            if (alias.IndexOf(".") == -1)
                alias = alias + ".";
        }
        #endregion Alias Control

        #region MySQL_Clear
        public string MySQL_Clear(string tSQL)
        {
            if (tSQL.IndexOf("/*KISITLAMALAR|1|*/") > -1)
            {
                int i1 = tSQL.IndexOf("/*KISITLAMALAR|1|*/");
                int i2 = (tSQL.IndexOf("/*KISITLAMALAR|2|*/") - i1) + 22;
                tSQL = tSQL.Remove(i1, i2);
            }
            return tSQL;
        }
        #endregion MySQL_Clear

        #region Get_ValueTrue

        public bool Get_ValueTrue(int ftype, string read_value)
        {
            bool onay = false;

            //* rakam  56, 48, 127, 52, 60, 62, 59, 108
            if ((ftype == 56) | (ftype == 48) | (ftype == 127) | (ftype == 52) |
                (ftype == 60) | (ftype == 62) | (ftype == 59) | (ftype == 108))
            {
                int value = myInt32(read_value);

                if (value == 0) return false;
                if (value > 0) return true;
            }

            //* text = (char)175, (varchar)167, (ntext)99, (text)35, (nchar)239
            if ((ftype == 175) | (ftype == 167) | (ftype == 99) | (ftype == 35) | (ftype == 239))
            {
                // Eğer veri yok ise null bas
                return IsNotNull(read_value);
            }

            //* bit türü 104
            if (ftype == 104)
            {
                if ((read_value == "") ||
                    (read_value == "0") ||
                    (read_value == "False")) return false;

                if ((read_value == "1") ||
                    (read_value == "True")) return true;
            }

            //* date 58
            if ((ftype == 58) || (ftype == 40) || (ftype == 61))
            {
                return IsNotNull(read_value);
            }

            return onay;
        }

        #endregion Get_ValueTrue

        #region Get_FieldTypeName

        public string Get_FieldTypeName(int ftype, string value)
        {
            string s = string.Empty;

            if ((value == "") ||
                (value == "0")) value = "100";

            if (ftype == 56) s = "int";
            if (ftype == 52) s = "smallint";
            if (ftype == 127) s = "bigint";
            if (ftype == 104) s = "bit";
            if (ftype == 108) s = "numeric(" + value + ")";
            if (ftype == 62) s = "float";
            if (ftype == 59) s = "real";
            if (ftype == 60) s = "money";
            if (ftype == 48) s = "tinyint";

            if (ftype == 167) s = "varchar(" + value + ")";
            if (ftype == 175) s = "char(" + value + ")";
            if (ftype == 35) s = "text";
            if (ftype == 99) s = "ntext";

            if (ftype == 58) s = "smalldatetime";
            if ((ftype == 40) | (ftype == 61)) s = "date";

            if (ftype == 41) s = "time";

            if (ftype == 34) s = "image";
            if (ftype == 173) s = "binary";
            if (ftype == 165) s = "varbinary";

            return s;
        }

        #endregion Get_FieldTypeName

        #region tField_Convert
        public string tField_Convert(string fname, int ftype)
        {
            // işleme uğramaz ise geldiği gibi dönsün
            string sonuc = fname;

            //* date türü 40
            if ((ftype == 40) || (ftype == 61))
            {
                if (fname != "")
                {
                    if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                        sonuc = " Convert(date, Convert(varchar(10)," + fname + ", 103), 104) ";  //  Tarih_Formati(Convert.ToDateTime(fvalue)) + " ";
                                                                                                  //and convert(date, VP.BAS_TARIH, 104)
                    if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                        sonuc = " Convert(" + fname + ", Date) ";
                }
            }

            return sonuc;
        }
        #endregion tField_Convert

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

        #region tSqlSecond_Clear
        public void tSqlSecond_Clear(ref DataSet dsData)
        {
            if (dsData != null)
                if (dsData.Namespace != null)
                {
                    string myProp = dsData.Namespace.ToString();
                    if (IsNotNull(myProp))
                    {
                        string OldSql = MyProperties_Get(myProp, "=SqlSecond:");

                        if (OldSql != "")
                        {
                            OldSql = "=SqlSecond:" + OldSql + ";";

                            Str_Replace(ref myProp, OldSql, "=SqlSecond:null;" + v.ENTER);
                            dsData.Namespace = myProp;
                        }
                    }
                }
        }
        #endregion tSqlSecond_Set

        #region tSqlParam_Replace
        public void tSqlParam_Replace(ref string Sql, string Param, string Value)
        {
            string ParamSatiri = Param + " = ";
            int i1 = Sql.IndexOf(ParamSatiri);
            int i2 = 0;

            if (i1 > -1)
            {
                for (int i = i1; i < Sql.Length; i++)
                {
                    if (Sql[i] == '\r') { i2 = i; break; }
                }

                if ((i1 > 0) && (i2 > 0))
                {
                    string OldValue = Sql.Substring(i1, (i2 - i1));
                    string NewValue = Param + " = " + Value + "  ";
                    Str_Replace(ref Sql, OldValue, NewValue);
                }
            }
        }
        #endregion tSqlParam_Replace

        #region Kriter_Ekle
        public void tKriter_Ekle(ref string aSQL, string alias, string Where_Add)
        {
            // and [VRSNL_10].{bugun} =   CONVERT(SMALLDATETIME, '06.08.2015 00:00:00', 101)  

            /*[VRSNL_10].KRITERLER*/

            if (alias.IndexOf(".") == -1) alias = alias + ".";

            int tboy = 15 + alias.Length;

            int h = 0;
            int n = 0;
            string SQLTemp = aSQL;
            int fKRITERLER = SQLTemp.IndexOf("/*" + alias + "KRITERLER*/");

            while (fKRITERLER > 0)
            {
                n = n + fKRITERLER + tboy;
                if ((fKRITERLER > tboy) && (Where_Add != ""))
                {
                    aSQL = aSQL.Insert(n, Where_Add);
                }

                h = fKRITERLER + tboy;
                n = n + Where_Add.Length;

                SQLTemp = SQLTemp.Remove(0, h);

                // birden fazla aynı alias olabilir, UNION ALL da gerekiyor

                fKRITERLER = SQLTemp.IndexOf("/*" + alias + "KRITERLER*/");
            }
        }
        #endregion Kriter_Ekle

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

        #endregion String İşlemler

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

        #region *Get Functions 

        #region <myStart> Get
        public string myStart_Get(Form tForm)
        {
            string[] controls = new string[] { };
            Control c = Find_Control(tForm, "tMyFormBox", "", controls);
            string s = string.Empty;

            if (c != null)
            {
                if (((DevExpress.XtraEditors.MemoEdit)c).EditValue != null)
                {
                    s = ((DevExpress.XtraEditors.MemoEdit)c).EditValue.ToString();

                    int i1 = s.IndexOf("//<myStart>");
                    int i2 = s.LastIndexOf("//<myEnd>");

                    if ((i1 > -1) && (i2 > -1))
                        s = s.Substring(i1, i2 - i1);
                }
            }

            return s;
        }
        #endregion <myStart> Get

        #region myFormBox_Values //  myFormListBox_Values
        public string myFormBox_Values(Form tForm, string ButtonName)
        {
            string s = string.Empty;
            string[] controls = new string[] { };
            Control c = Find_Control(tForm, "tMyFormBox", "", controls);

            // ButtonName boş ise tMyFormBox üzerindeki tüm bilgiler gönderilecek
            //            boş değilse sadece ButtonName ait bilgiler gönderilecek

            if (c != null)
            {
                if (((DevExpress.XtraEditors.MemoEdit)c).EditValue != null)
                {
                    s = ((DevExpress.XtraEditors.MemoEdit)c).EditValue.ToString();
                    if (s == "[]") s = "";

                    int i1 = 0;
                    int i2 = 0;
                    if ((ButtonName != string.Empty) || (ButtonName != ""))
                    {
                        i1 = s.IndexOf(ButtonName);
                        i2 = s.LastIndexOf(ButtonName + "=END:");

                        if ((i1 > -1) && (i2 > -1))
                            s = s.Substring(i1, i2 - i1);
                    }
                }
            }

            return s;
        }
        #endregion myFormBox_Values

        #region Find_Button_Value
        public string Find_Button_Value(Form tForm, string ButtonName, string FindCode)
        {
            // 1. formun üzerinde create sırasında atanmış olan button hakkındaki bilgiler okunur
            string snc = myFormBox_Values(tForm, ButtonName);

            // 2. butona ait bilgiler ayıklanır
            return MyProperties_Get(snc, FindCode);
        }
        #endregion Find_Button_Value

        #region Find_Control_List

        public void Find_Control_List(Form tForm, List<string> list, string[] ControlType)
        {
            // CheckedListBoxControl
            #region Control1
            string dataLayout = "DevExpress.XtraDataLayout.DataLayoutControl";

            list.Clear();

            foreach (Control c in tForm.Controls)
            {
                tList_Add(c, list, ControlType);

                if (c.ToString() == dataLayout)
                    tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c, list, ControlType);

                if (c.Controls.Count > 0)
                {
                    #region Control2
                    foreach (Control c2 in c.Controls)
                    {
                        tList_Add(c2, list, ControlType);

                        if (c2.ToString() == dataLayout)
                            tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c2, list, ControlType);

                        #region Control3
                        foreach (Control c3 in c2.Controls)
                        {
                            tList_Add(c3, list, ControlType);

                            if (c3.ToString() == dataLayout)
                                tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c3, list, ControlType);

                            if (c3.Controls.Count > 0)
                            {
                                #region Control4
                                foreach (Control c4 in c3.Controls)
                                {
                                    tList_Add(c4, list, ControlType);

                                    if (c4.ToString() == dataLayout)
                                        tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c4, list, ControlType);

                                    if (c4.Controls.Count > 0)
                                    {
                                        #region Control5
                                        foreach (Control c5 in c4.Controls)
                                        {
                                            tList_Add(c5, list, ControlType);

                                            if (c5.ToString() == dataLayout)
                                                tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c5, list, ControlType);

                                            if (c5.Controls.Count > 0)
                                            {
                                                #region Control6
                                                foreach (Control c6 in c5.Controls)
                                                {
                                                    tList_Add(c6, list, ControlType);

                                                    if (c6.ToString() == dataLayout)
                                                        tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c6, list, ControlType);

                                                    if (c6.Controls.Count > 0)
                                                    {
                                                        #region Control7
                                                        foreach (Control c7 in c6.Controls)
                                                        {
                                                            tList_Add(c7, list, ControlType);

                                                            if (c7.ToString() == dataLayout)
                                                                tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c7, list, ControlType);

                                                            if (c7.Controls.Count > 0)
                                                            {
                                                                #region Control8
                                                                foreach (Control c8 in c7.Controls)
                                                                {
                                                                    tList_Add(c8, list, ControlType);

                                                                    if (c8.ToString() == dataLayout)
                                                                        tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c8, list, ControlType);

                                                                    if (c8.Controls.Count > 0)
                                                                    {
                                                                        #region Control9
                                                                        foreach (Control c9 in c8.Controls)
                                                                        {
                                                                            tList_Add(c9, list, ControlType);

                                                                            if (c9.ToString() == dataLayout)
                                                                                tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c9, list, ControlType);

                                                                            if (c9.Controls.Count > 0)
                                                                            {
                                                                                #region Control10
                                                                                foreach (Control c10 in c9.Controls)
                                                                                {
                                                                                    tList_Add(c10, list, ControlType);

                                                                                    if (c10.ToString() == dataLayout)
                                                                                        tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c10, list, ControlType);

                                                                                    if (c10.Controls.Count > 0)
                                                                                    {
                                                                                        #region Control11
                                                                                        foreach (Control c11 in c10.Controls)
                                                                                        {
                                                                                            tList_Add(c11, list, ControlType);

                                                                                            if (c11.ToString() == dataLayout)
                                                                                                tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c11, list, ControlType);

                                                                                            if (c11.Controls.Count > 0)
                                                                                            {
                                                                                                #region Control12
                                                                                                foreach (Control c12 in c11.Controls)
                                                                                                {
                                                                                                    tList_Add(c12, list, ControlType);

                                                                                                    if (c12.ToString() == dataLayout)
                                                                                                        tLayoutControl_Add((DevExpress.XtraLayout.LayoutControl)c12, list, ControlType);

                                                                                                    //.....
                                                                                                }
                                                                                                #endregion // Control12
                                                                                            }
                                                                                        }
                                                                                        #endregion // Control11
                                                                                    }
                                                                                }
                                                                                #endregion // Control10
                                                                            }
                                                                        }
                                                                        #endregion // Control9
                                                                    }
                                                                }
                                                                #endregion // Control8
                                                            }
                                                        }
                                                        #endregion // Control7
                                                    }
                                                }
                                                #endregion // Control6
                                            }
                                        }
                                        #endregion // Control5
                                    }
                                }
                                #endregion // Control4
                            }

                        }
                        #endregion // Control3
                    }
                    #endregion // Control2
                }
            }

            list.Sort();

            #endregion // Control1


            /*
            string[] controls = new string[] { "DevExpress.XtraGrid.GridControl", 
                                               "DevExpress.XtraVerticalGrid.VGridControl",
                                               "DevExpress.XtraDataLayout.DataLayoutControl",
                                               "DevExpress.XtraTreeList.TreeList",
                                               "DevExpress.XtraEditors.DataNavigator"
                                             };
            List<string> list = new List<string>();
            t.Find_Control_List(tForm, list, controls);
            ****  * /
            
            string s = Kim + v.ENTER2;
            foreach (string value in list)
            {
                s = s + value + v.ENTER;
            }

            MessageBox.Show(s);
            */
        }

        private void tLayoutControl_Add(LayoutControl tLayoutControl, List<string> list, string[] ControlType)
        {
            //CheckedListBoxControl
            foreach (BaseLayoutItem item in ((DevExpress.XtraLayout.LayoutControl)tLayoutControl).Items)
            {
                tList_Add(item, list, ControlType);
            }
        }

        private void tList_Add(Control cntrl, List<string> list, string[] ControlType)
        {
            if (ControlType.Length == 0)
            {
                if (cntrl.Name.ToString() != string.Empty)
                    list.Add(cntrl.Name);
            }
            else
            {
                for (int t = 0; t < ControlType.Length; t++)
                {
                    if (cntrl.ToString() == ControlType[t])
                    {
                        if (cntrl.Name.ToString() != string.Empty)
                            list.Add(cntrl.Name.ToString());
                        break;
                    }
                }
            }
        }

        private void tList_Add(BaseLayoutItem cntrl, List<string> list, string[] ControlType)
        {
            if (ControlType.Length == 0)
            {
                if (cntrl.Name.ToString() != string.Empty)
                    list.Add(cntrl.Name);
            }
            else
            {
                for (int t = 0; t < ControlType.Length; t++)
                {
                    if (cntrl.ToString() == ControlType[t])
                    {
                        if (cntrl.Name.ToString() != string.Empty)
                            list.Add(cntrl.Name.ToString());
                        break;
                    }
                }
            }
        }

        #endregion Find_Control_List

        #endregion Get Functions

        #region *Diğer Yardımcı Fonksiyonlar

        

        #region Takipci
        public void Takipci(string function_name, string sub_function, char bayrak)
        {
            if ((v.Takip_Find == false) && (function_name.IndexOf("_Find") > 0)) return;

            if (v.Takip)
            {
                if (bayrak == '}') { --v.Takip_Adim; }

                if (v.Takip_Adim < 0) v.Takip_Adim = 0;

                if (v.Takip_Adim == 0) v.bosluk = "";
                if (v.Takip_Adim == 1) v.bosluk = "...";
                if (v.Takip_Adim == 2) v.bosluk = "......";
                if (v.Takip_Adim == 3) v.bosluk = ".........";
                if (v.Takip_Adim == 4) v.bosluk = "............";
                if (v.Takip_Adim == 5) v.bosluk = "...............";
                if (v.Takip_Adim == 6) v.bosluk = "..................";
                if (v.Takip_Adim == 7) v.bosluk = ".....................";
                if (v.Takip_Adim == 8) v.bosluk = "........................";
                if (v.Takip_Adim == 9) v.bosluk = "...........................";
                if (v.Takip_Adim == 10) v.bosluk = "..............................";
                if (v.Takip_Adim == 11) v.bosluk = ".................................";
                if (v.Takip_Adim == 12) v.bosluk = "....................................";


                if (bayrak == '{') { ++v.Takip_Adim; }

                v.Takip_Listesi = v.Takip_Listesi + v.ENTER + v.bosluk + function_name;

                if (sub_function != string.Empty)
                    v.Takip_Listesi = v.Takip_Listesi + ", " + sub_function;

                v.Takip_Listesi = v.Takip_Listesi + " " + bayrak;
            }
        }
        #endregion Takipci

        #region FormActiveControl
        /// parametre TableIPCode ise view nesnesi active edilecek 
        /// 
        public void tFormActiveView(Form tForm, string TableIPCode)
        {
            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = Find_Control_View(tForm, TableIPCode);

            // set et
            tFormActiveControl(tForm, cntrl);
        }
        public void tFormActiveControl(Form tForm, string controlName)
        {
            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = Find_Control(tForm, controlName, "", controls);

            // set et
            tFormActiveControl(tForm, cntrl);
        }

        /// parametre TableIPCode ve FieldName ise view nesnesi active edilecek 
        /// 
        public void tFormActiveControl(Form tForm, string TableIPCode, string columnFrontName , string fieldName)
        {
            Control cntrl = null;
            string[] controls = new string[] { };

            //controlName = "Column_"
            //cntrl = Find_Control(tForm, "Column_" + sf_FieldName, sf_TableIPCode, controls);

            cntrl = Find_Control(tForm, columnFrontName + fieldName, TableIPCode, controls);
            
            // set et
            tFormActiveControl(tForm, cntrl); 
        }
        /// parametre bir control nesnesi ise kendisi set edilecek
        ///
        public void tFormActiveControl(Form tForm, Control cntrl)
        {
            if (cntrl != null)
                tForm.ActiveControl = cntrl;
        }

        
        #endregion FormActiveControl

        #region ButtonEnabled and All

        public void ButtonEnabledAll(Form tForm, string TableIPCode, Boolean NewBool)
        {
            // 53 yeni hesap
            ButtonCaptionRefresh(tForm, TableIPCode, "simpleButton_yeni_hesap");
            // 54 yeni alt hesap
            ButtonCaptionRefresh(tForm, TableIPCode, "simpleButton_yeni_alt_hesap");

            /*
            // 53 yeni hesap
            ButtonEnabled(tForm, TableIPCode, "simpleButton_yeni_hesap", NewBool);
            // 54 yeni alt hesap
            ButtonEnabled(tForm, TableIPCode, "simpleButton_yeni_alt_hesap", NewBool);
            // 58 yeni fiş
            ButtonEnabled(tForm, TableIPCode, "simpleButton_yeni_fis", NewBool);
            */
        }

        public void ButtonCaptionRefresh(Form tForm, string TableIPCode, string ButtonName)
        {
            Control cntrl = null;
            string[] controls = new string[] { };

            cntrl = Find_Control(tForm, ButtonName, TableIPCode, controls);

            if (cntrl != null)
            {
                // sakladığın caption yeniden kullan ( Vazgeç <<< Yeni Hesap )

                if (((DevExpress.XtraEditors.SimpleButton)cntrl).Tag != null)
                {

                    ((DevExpress.XtraEditors.SimpleButton)cntrl).Text =
                        ((DevExpress.XtraEditors.SimpleButton)cntrl).Tag.ToString();

                    if (((DevExpress.XtraEditors.SimpleButton)cntrl).Name == "simpleButton_yeni_hesap")
                    {
                        if (((DevExpress.XtraEditors.SimpleButton)cntrl).Text == "Vazgeç")
                            ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = Find_Glyph("YENIVAZGEÇ16");
                        else ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = Find_Glyph("YENIHESAP16");
                    } 
                }
            }
        }
        
        public void ButtonEnabled(Form tForm, string TableIPCode, string ButtonName, Boolean NewBool)
        {
            Control cntrl = null;
            string[] controls = new string[] { };

            cntrl = Find_Control(tForm, ButtonName, TableIPCode, controls);

            if (cntrl != null)
            {
                ((DevExpress.XtraEditors.SimpleButton)cntrl).Enabled = NewBool;
            }
        }

        #endregion ButtonEnabled and All

        #region PrevPage / NextPage

        public void PrevPage(Form tForm, string TableIPCode)
        {
            PageChange(tForm, TableIPCode, "PREVIOUS"); //previous
        }

        public void NextPage(Form tForm, string TableIPCode)
        {
            PageChange(tForm, TableIPCode, "NEXT");
        }

        public Control Find_Pages_Object(Form tForm, string TableIPCode)
        {
            Control viewcntrl = null;
            Control myParent1 = null;
            //Control myParent2 = null;

            if (IsNotNull(TableIPCode))
            {
                viewcntrl = Find_Control_View(tForm, TableIPCode);

                /// TableIPCode üzerinde bazen nesne adıda olabiliyor onun için
                /// tekrar isimdede arama yapılacak
                /// xxxx.Name = v.lyt_Name + RefId.ToString();  
                ///  
                if (viewcntrl == null)
                    viewcntrl = Find_Control(tForm, TableIPCode);
            }

            if (viewcntrl != null)
            {
                myParent1 = Find_TabControl(tForm, viewcntrl);
            }
            
            return myParent1;
        }

        private Control Find_TabControl(Form tForm, Control viewControl)
        {
            /// grid buldum, gridin hangi tabControl içinde olduğunu bulmam gerekir
            ///
            Control myParent1 = viewControl;
            Control myParent2 = null;

            /// önce gelenin kendisi kontrol ediliyor
            /// eğer aran tabcontrollerden biri ise aranan bulunmuştur
            if (myParent1 == null) return null;

            if (myParent1.ToString() == "DevExpress.XtraDataLayout.DataLayoutControl")
            {
                string TableIPCode = ((DevExpress.XtraDataLayout.DataLayoutControl)myParent1).AccessibleName;

                myParent1 = Find_Control(tForm, "navigationPane_" + AntiStr_Dot(TableIPCode));
                if (myParent1 == null) return null;
            }
            
            if (myParent1.ToString() == "DevExpress.XtraBars.Navigation.TabPane") return myParent1;
            if (myParent1.ToString() == "DevExpress.XtraBars.Navigation.NavigationPane") return myParent1;
            if (myParent1.ToString() == "DevExpress.XtraBars.Ribbon.BackstageViewControl") return myParent1;
            if (myParent1.ToString() == "DevExpress.XtraTab.XtraTabControl") return myParent1;
            if (myParent1.ToString() == "DevExpress.XtraBars.Ribbon.BackstageViewClientControl")
            {
                return myParent1.Parent;
            }
            
            /// eğer aranan tabcontrol bulunmadı ise sırayla üst hesapları kontrol edilecek
            /// takii boşa düşene kadar
            while (myParent1 != null)
            {
                if (myParent1.Parent != null)
                    myParent2 = myParent1.Parent;

                if (myParent2 == null) return myParent1;
                if (myParent2.ToString() == "DevExpress.XtraBars.Navigation.TabPane") return myParent2;
                if (myParent2.ToString() == "DevExpress.XtraBars.Navigation.NavigationPane") return myParent2;
                if (myParent2.ToString() == "DevExpress.XtraBars.Ribbon.BackstageViewControl") return myParent2;
                if (myParent2.ToString() == "DevExpress.XtraTab.XtraTabControl") return myParent2;

                if ((myParent2.ToString() == "DevExpress.XtraBars.Ribbon.BackstageViewClientControl") ||
                    (myParent2.ToString() == "DevExpress.XtraBars.Navigation.TabNavigationPage") ||
                    (myParent2.ToString() == "DevExpress.XtraBars.Navigation.NavigationPage") ||
                    (myParent2.ToString() == "DevExpress.XtraTab.XtraTabPage")
                   )
                {
                    return myParent2.Parent;
                }

                myParent1 = myParent2.Parent;
            }

            return null;
        }

        public void PageChange(Form tForm, string TableIPCode, string Action)
        {
            Control myControl = null;
            
            myControl = Find_Pages_Object(tForm, TableIPCode);

            if (myControl != null)
                PageChange_(tForm, myControl, Action, TableIPCode);
        }

        private void PageChange_(Form tForm, Control myControl, string Action, string TableIPCode)
        {
            string myObjectName = string.Empty;
            string sayfa = "ortaSayfa";
            int oldIndex = -1;
            int newIndex = -1;
            int pageCount = -1;

            if (myControl != null)
                myObjectName = myControl.ToString();
            
            #region NEXT || LAST
            if ((myControl != null) && ((Action == "NEXT") || (Action == "LAST")))
            {
                if (myObjectName == "DevExpress.XtraBars.Navigation.TabPane")
                {
                    pageCount = ((DevExpress.XtraBars.Navigation.TabPane)myControl).Pages.Count;

                    if ((((DevExpress.XtraBars.Navigation.TabPane)myControl).Pages.Count - 1) >
                         ((DevExpress.XtraBars.Navigation.TabPane)myControl).SelectedPageIndex)
                    {
                        oldIndex = ((DevExpress.XtraBars.Navigation.TabPane)myControl).SelectedPageIndex;
                        // NEXT
                        newIndex = oldIndex + 1;

                        if (Action == "LAST")
                            newIndex = (pageCount - 1);

                        ((DevExpress.XtraBars.Navigation.TabPane)myControl).SelectedPageIndex = newIndex;
                    }
                }
                
                if (myObjectName == "DevExpress.XtraBars.Navigation.NavigationPane")
                {
                    pageCount = ((DevExpress.XtraBars.Navigation.NavigationPane)myControl).Pages.Count;

                    if ((((DevExpress.XtraBars.Navigation.NavigationPane)myControl).Pages.Count - 1) >
                         ((DevExpress.XtraBars.Navigation.NavigationPane)myControl).SelectedPageIndex)
                    {
                        oldIndex = ((DevExpress.XtraBars.Navigation.NavigationPane)myControl).SelectedPageIndex;
                        // NEXT
                        newIndex = oldIndex + 1;

                        if (Action == "LAST")
                            newIndex = (pageCount - 1);

                        ((DevExpress.XtraBars.Navigation.NavigationPane)myControl).SelectedPageIndex = newIndex;
                    }
                }

                if (myObjectName == "DevExpress.XtraBars.Ribbon.BackstageViewControl")
                {
                    pageCount = ((DevExpress.XtraBars.Ribbon.BackstageViewControl)myControl).Items.Count;

                    if ((((DevExpress.XtraBars.Ribbon.BackstageViewControl)myControl).Items.Count - 1) >
                         ((DevExpress.XtraBars.Ribbon.BackstageViewControl)myControl).SelectedTabIndex)
                    {
                        oldIndex = ((DevExpress.XtraBars.Ribbon.BackstageViewControl)myControl).SelectedTabIndex;
                        // NEXT
                        newIndex = oldIndex + 1;

                        if (Action == "LAST")
                            newIndex = (pageCount - 1);

                        ((DevExpress.XtraBars.Ribbon.BackstageViewControl)myControl).SelectedTabIndex = newIndex;
                    }
                }

                if (myObjectName == "DevExpress.XtraTab.XtraTabControl")
                {
                    pageCount = ((DevExpress.XtraTab.XtraTabControl)myControl).TabPages.Count;

                    if ((((DevExpress.XtraTab.XtraTabControl)myControl).TabPages.Count - 1) >
                         ((DevExpress.XtraTab.XtraTabControl)myControl).TabIndex)
                    {
                        oldIndex = ((DevExpress.XtraTab.XtraTabControl)myControl).TabIndex;
                        // NEXT
                        newIndex = oldIndex + 1;

                        if (Action == "LAST")
                            newIndex = (pageCount - 1);

                        ((DevExpress.XtraTab.XtraTabControl)myControl).TabIndex = newIndex;
                    }
                }

                if ((newIndex == (pageCount - 1)) ||
                    (newIndex == -1))
                    sayfa = "sonSayfa";

            }
            #endregion NEXT

            #region PREVIOUS || FIRST
            if ((myControl != null) && ((Action == "PREVIOUS") || (Action == "FIRST")))
            {
                if (myObjectName == "DevExpress.XtraBars.Navigation.TabPane")
                {
                    if (((DevExpress.XtraBars.Navigation.TabPane)myControl).SelectedPageIndex > 0)
                    {
                        oldIndex = ((DevExpress.XtraBars.Navigation.TabPane)myControl).SelectedPageIndex;
                        // PREVIOUS 
                        newIndex = oldIndex - 1;
                        if (Action == "FIRST") newIndex = 0;

                        ((DevExpress.XtraBars.Navigation.TabPane)myControl).SelectedPageIndex = newIndex;
                    }
                }

                if (myObjectName == "DevExpress.XtraBars.Navigation.NavigationPane")
                {
                    if (((DevExpress.XtraBars.Navigation.NavigationPane)myControl).SelectedPageIndex > 0)
                    {
                        oldIndex = ((DevExpress.XtraBars.Navigation.NavigationPane)myControl).SelectedPageIndex;
                        // PREVIOUS 
                        newIndex = oldIndex - 1;
                        if (Action == "FIRST") newIndex = 0;

                        ((DevExpress.XtraBars.Navigation.NavigationPane)myControl).SelectedPageIndex = newIndex;
                    }
                }

                if (myObjectName == "DevExpress.XtraBars.Ribbon.BackstageViewControl")
                {
                    if (((DevExpress.XtraBars.Ribbon.BackstageViewControl)myControl).SelectedTabIndex > 0)
                    {
                        oldIndex = ((DevExpress.XtraBars.Ribbon.BackstageViewControl)myControl).SelectedTabIndex;
                        // PREVIOUS 
                        newIndex = oldIndex - 1;
                        if (Action == "FIRST") newIndex = 0;

                        ((DevExpress.XtraBars.Ribbon.BackstageViewControl)myControl).SelectedTabIndex = newIndex;
                    }
                }

                if (myObjectName == "DevExpress.XtraTab.XtraTabControl")
                {
                    if (((DevExpress.XtraTab.XtraTabControl)myControl).TabIndex > 0)
                    {
                        oldIndex = ((DevExpress.XtraTab.XtraTabControl)myControl).TabIndex;
                        // PREVIOUS 
                        newIndex = oldIndex - 1;
                        if (Action == "FIRST") newIndex = 0;

                        ((DevExpress.XtraTab.XtraTabControl)myControl).TabIndex = newIndex;
                    }
                }

                if (newIndex == 0)
                    sayfa = "ilkSayfa";


            }
            #endregion PREVIOUS

            //simpleButton_sihirbaz_geri
            //simpleButton_sihirbaz_sonra
            //simpleButton_sihirbaz_devam

            Control cntrl = null;
            string[] controls = new string[] { };
            cntrl = Find_Control(tForm, "simpleButton_sihirbaz_geri", TableIPCode, controls);

            if (cntrl != null)
            {
                if (sayfa == "ilkSayfa")
                     ((DevExpress.XtraEditors.SimpleButton)cntrl).Enabled = false;
                else ((DevExpress.XtraEditors.SimpleButton)cntrl).Enabled = true;
            }

            cntrl = Find_Control(tForm, "simpleButton_sihirbaz_sonra", TableIPCode, controls);

            if (cntrl != null)
            {
                if (sayfa == "sonSayfa")
                    ((DevExpress.XtraEditors.SimpleButton)cntrl).Enabled = false;
                else ((DevExpress.XtraEditors.SimpleButton)cntrl).Enabled = true;
            }
            
            cntrl = Find_Control(tForm, "simpleButton_sihirbaz_devam", TableIPCode, controls);

            if (cntrl != null)
            {
                if (sayfa == "sonSayfa")
                {
                    if (((DevExpress.XtraEditors.SimpleButton)cntrl).Text != "Kaydet")
                    {
                        ((DevExpress.XtraEditors.SimpleButton)cntrl).Text = "Kaydet";
                        ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = Find_Glyph("KAYDET16");
                        //((DevExpress.XtraEditors.SimpleButton)cntrl).Appearance.ForeColor = Color.Red;
                    }
                }
                else
                {
                    if (((DevExpress.XtraEditors.SimpleButton)cntrl).Text != "Devam")
                    {
                        ((DevExpress.XtraEditors.SimpleButton)cntrl).Text = "Devam";
                        ((DevExpress.XtraEditors.SimpleButton)cntrl).Image = Find_Glyph("SIHIRBAZDEVAM16");
                    }
                }
            }


            /// dataWizard üzerindeki mesaj labelleri
            /// 
            #region labelControls
            if (oldIndex >= 0)
            {
                Control oldLabel = Find_Control(tForm, "labelControl_Step_" + oldIndex);
                if (oldLabel != null)
                {
                    // 
                    // old >> labelControl_Step1
                    // 
                    ((DevExpress.XtraEditors.LabelControl)oldLabel).Appearance.BackColor = System.Drawing.SystemColors.ControlLight;
                    ((DevExpress.XtraEditors.LabelControl)oldLabel).Appearance.ForeColor = System.Drawing.SystemColors.ControlDarkDark;

                    Control newLabel = Find_Control(tForm, "labelControl_Step_" + newIndex);

                    if (newLabel != null)
                    {
                        // 
                        // new >> labelControl_Step1
                        // 
                        ((DevExpress.XtraEditors.LabelControl)newLabel).Appearance.BackColor = System.Drawing.SystemColors.InactiveCaption;
                        ((DevExpress.XtraEditors.LabelControl)newLabel).Appearance.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
                    }
                    // işlemi bitir
                    oldIndex = -1;
                    newIndex = -1;
                }
            }
            #endregion labelControls
        }

        #endregion PrevPage / NextPage

        #region RibbonPagesSet
        public void RibbonPagesSet(Control ribbon, string pName)
        {
            if (pName.IndexOf("item_") == -1)
                pName = "item_" + pName;

            foreach (DevExpress.XtraBars.Ribbon.RibbonPage page in ((DevExpress.XtraBars.Ribbon.RibbonControl)ribbon).Pages)
            {
                page.Visible = (page.Name.ToString() == pName);
            }
        }
        #endregion

        #region MSSQL_Server_Tarihi
        public void MSSQL_Server_Tarihi()
        {
            string ySql =
              @" select 
               GETDATE() DB_TARIH 
             , CONVERT(varchar(10), GETDATE(), 104) TARIH 
             , CONVERT(varchar, GETDATE(), 102) YILAYGUN 
             , CONVERT(varchar, GETDATE(), 8) UZUN_SAAT 
             , CONVERT(varchar, GETDATE(), 14) UZUN_SAAT2 
             , YEAR(GETDATE()) YIL 
             , MONTH(GETDATE()) AY 
             , DAY(GETDATE()) GUN 
             , DATEPART(weekday, GETDATE()) HAFTA_GUN
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

            }

            //MessageBox.Show(v.BUGUN_TARIH.ToString());

            ds.Dispose();
        }
        #endregion MSSQL_Server_Tarihi

        #region tGotoRecord
        public Boolean tGotoRecord(Form tForm, DataSet dsData, string TableIPCode, string FieldName, string Value, int position)// string TalepEden)
        {
            Boolean snc = false;

            // Her hangi bir id ye ulaşmak için  ...
            // DataRow[] row = dsData.Tables[0].Select(FieldName + "=" + Value);
            // ----

            DataNavigator tDataNavigator = null;

            int i = 0;
            // position daha önce tespit edilmemişse 
            if (position < 0)
            {
                if (dsData == null)
                    Find_DataSet(tForm, ref dsData, ref tDataNavigator, TableIPCode); 
                    //dsData = Find_DataSet(tForm, "", TableIPCode, "");

                if (IsNotNull(dsData))
                    i = Find_GotoRecord(dsData, FieldName, Value);
            }
            else i = position;

            if (i > -1)
            {
                if (tDataNavigator == null)
                    tDataNavigator = Find_DataNavigator(tForm, TableIPCode);

                if (tDataNavigator != null)
                {
                    // burada atama yapılıyor fakat position değişmiyor
                    tDataNavigator.Position = i;
                    tDataNavigator.Tag = i;
                    v.con_SubView_FIRST_POSITION = i;
                    // ---
                    
                    v.con_GotoRecord = string.Empty;
                    v.con_GotoRecord_TableIPCode = string.Empty;
                    v.con_GotoRecord_FName = string.Empty;
                    v.con_GotoRecord_Value = string.Empty;
                    v.con_GotoRecord_Position = -1;
                    snc = true;
                }
            }
            else
            {
                // Eğer halen -1 ise form DialogForm olduğu zaman çalışmıyor
                // Onun için FormLoad sırasında bu işlemleri tekrar yapmak gerekiyor
                v.con_GotoRecord = "ON";
                v.con_GotoRecord_TableIPCode = TableIPCode;
                v.con_GotoRecord_FName = FieldName;
                v.con_GotoRecord_Value = Value;
                v.con_GotoRecord_Position = position;
                snc = false;
            }

            return snc;
        }
        #endregion GotoRecord

        #region SYS_Types_List

        public void SYS_Types_Read()
        {
            /// 1. işlem
            ///
            SYS_Types_Names_Read();

            /// 2. işlem
            /// 
            SYS_Variables_Read();

            /// 3. işlem 
            /// SYS_COMPS ile HP_COMPS
            /// SYS_USERS ile HP_USERS
            /// SYS_FIRMS ile HP_FIRM arasındaki fark gideriliyor
            HP_FIRM_Preparing();


            /// 4. İşlem 
            /// exe update varmı
            HP_UPDATE_Preparing();

        }
        
        private void SYS_Types_Names_Read()
        {
            tSQLs sql = new tSQLs();

            /// 1. adım 
            string SysTypesSql = "";
            string MsTypesSql = "";

            sql.SQL_Types_List(ref SysTypesSql, ref MsTypesSql);

            if (IsNotNull(SysTypesSql))
                SQL_Read_Execute(v.dBaseNo.Project, v.ds_TypesList, ref SysTypesSql, "SYS_TYPES_LIST", "");

            if (IsNotNull(MsTypesSql))
                SQL_Read_Execute(v.dBaseNo.Manager, v.ds_MsTypesList, ref MsTypesSql, "MS_TYPES_LIST", "");

            /// amaç : tüm types_name isimlerini 

            string rName = "";
            string tName = "";
            v.ds_TypesNames = "";
            v.ds_MsTypesNames = "";

            int i1 = 0;
            if (v.ds_TypesList != null)
            {
                if (v.ds_TypesList.Tables.Count > 0)
                    i1 = v.ds_TypesList.Tables[0].Rows.Count;

                for (int i = 0; i < i1; i++)
                {
                    rName = v.ds_TypesList.Tables[0].Rows[i]["TYPES_NAME"].ToString();
                    if (rName != tName)
                    {
                        v.ds_TypesNames = v.ds_TypesNames + rName + ";" + v.ENTER;
                        tName = rName;
                    }
                }
            }

            i1 = 0;
            if (v.ds_MsTypesList != null)
            {
                if (v.ds_MsTypesList.Tables.Count > 0)
                    i1 = v.ds_MsTypesList.Tables[0].Rows.Count;

                for (int i = 0; i < i1; i++)
                {
                    rName = v.ds_MsTypesList.Tables[0].Rows[i]["TYPES_NAME"].ToString();
                    if (rName != tName)
                    {
                        v.ds_MsTypesNames = v.ds_MsTypesNames + rName + ";" + v.ENTER;
                        tName = rName;
                    }
                }
            }

        }

        public void SYS_Variables_Read()
        {
            /// SYS_VARIABLES list
            /// 
            if (v.active_DB.managerDBName != "SystemMS")
            {
                tSQLs sql = new tSQLs();

                string Sql = sql.SQL_SYS_Variables_List(v.active_DB.projectDBType);

                if (IsNotNull(Sql))
                    SQL_Read_Execute(v.dBaseNo.Project, v.ds_Variables, ref Sql, "SYS_VARIABLES_LIST", "");
            }
        }

        public void HP_UPDATE_Preparing()
        {
            DataSet ds_Query = new DataSet();
            tSQLs sql = new tSQLs();

            string execSql = string.Empty;
            string tSql = sql.SQL_SYS_UPDATES();

            SQL_Read_Execute(v.dBaseNo.Manager, ds_Query, ref tSql, "SYS_UPDATES", "UpdateList");

            /*
            SELECT 
               [ID]
              ,[ISACTIVE]
              ,[REC_DATE]
              ,[EXE_NAME]
              ,[UPDATE_TD]
              ,[UPDATE_NO]
              ,[VERSION_NO]
              ,[PACKET_NAME]
              ,[ABOUT]
            FROM [MSV3DFTRBLT].[dbo].[SYS_UPDATES]
            */

            tSql = "";
            string s = "";
            
            foreach (DataRow row in ds_Query.Tables[0].Rows)
            {
                s = Sql_HP_UPDATES_Insert(
                    row["REC_DATE"].ToString()
                  , row["EXE_NAME"].ToString()
                  , myInt16(row["UPDATE_TD"].ToString())
                  , myInt32(row["UPDATE_NO"].ToString())
                  , row["VERSION_NO"].ToString()
                  , row["PACKET_NAME"].ToString()
                  , row["ABOUT"].ToString());
                
                // update kontrol cümlesi
                //
                execSql = execSql + s + v.ENTER;
            }
                        
            if (IsNotNull(execSql))
            {
                SQL_Read_Execute(v.dBaseNo.Project, v.ds_ExeUpdates, ref execSql, "HP_UPDATES", "HP_UPDATES_INS");

                newExeUpdate(v.ds_ExeUpdates);
            }
        }


        public void HP_FIRM_Preparing()
        {
            DataSet ds_Query = new DataSet();
            tSQLs sql = new tSQLs();

            string execSql = string.Empty;
            string tSql = sql.SQL_SYS_FIRM_List(v.SP_FIRM_GUID, v.tFirmListType.AllFirm);

            SQL_Read_Execute(v.dBaseNo.Manager, ds_Query, ref tSql, "SYS_FIRMS", "FirmList");

            /*
              Select 
              distinct f.[ID]
                      ,f.[PARENT_ID]
                      ,f.[REF_ID]
                      ,f.[PARENT_REF_ID]
                      ,f.[ISACTIVE]
                      ,f.LOCAL_TD
                      ,f.[FIRM_GUID]
                      ,f.[FIRM_CODE]
                      ,f.[FIRM_NAME]
                      ,f.[FIRM_LONG_NAME1]
                      ,f.[FIRM_LONG_NAME2]

                  from CTE c
                     left outer join SYS_FIRMS f on(c.U_ID = f.ID)
            */
            
            tSql = "";
            string s = "";
            string ffl = "";
            int fId = 0;
            string fName = "";
            string fGuid = "";
            v.SP_FIRM_FULLLIST = "";

            foreach (DataRow row in ds_Query.Tables[0].Rows)
            {    
                s = Sql_HP_FIRM_Insert(
                   myInt16(row["ISACTIVE"].ToString())
                 , row["FIRM_GUID"].ToString()
                 , row["LOCAL_TD"].ToString()   
                 , row["ID"].ToString()          // LOCAL_ID 
                 , row["PARENT_ID"].ToString()
                 , row["FIRM_NAME"].ToString());

                // firma kontrol cümlesi
                //
                execSql = execSql + s + v.ENTER;

                // Tüm grup FIRMA ıd leri toplanıyor 
                ffl = ffl + row["ID"].ToString() + ", ";

                //
                fId = myInt32(row["ID"].ToString());
                fName = row["FIRM_NAME"].ToString();
                fGuid = row["FIRM_GUID"].ToString();

                // hafızada kalsın diye bilgiler toplanıyor
                FirmAbout tFirmAbout = new FirmAbout();
                tFirmAbout.FirmId = fId;
                tFirmAbout.FirmName = fName;
                tFirmAbout.FirmGuid = fGuid;
                v.tFirmFullList.Add(tFirmAbout);
            }

            // comp / bilgisayar kontrol cumlesi
            //
            execSql = execSql + v.ENTER + Sql_HP_COMPS();

            // users kontrol cümlesi
            //
            execSql = execSql + v.ENTER + Sql_HP_USERS();


            if (IsNotNull(execSql))
            {
                // en sondaki virgülü sil
                tLast_Char_Remove(ref ffl);
                v.SP_FIRM_FULLLIST = ffl;
                                
                //execSql = " begin transaction \r\n " + execSql + " commit transaction "; 

                SQL_Read_Execute(v.dBaseNo.Project, ds_Query, ref execSql, "HP_FIRM", "HP_FIRMS_INS_UPD");
            }
        }

        private string Sql_HP_COMPS()
        {
            string s = "";

            if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                s = @"
            if ( select count(*) ADET from HP_COMPS
                 where ID > 0 
                 and  NETWORK_MACADDRESS = '" + v.tComp.SP_COMP_MACADDRESS + @"'
                 and ( ISACTIVE <> " + v.tComp.SP_COMP_ISACTIVE.ToString() + @"
                   or  SYSTEM_NAME <> '" + v.tComp.SP_COMP_SYSTEM_NAME + @"' 
                   or  COMP_FIRM_GUID <> '" + v.tComp.SP_COMP_FIRM_GUID + @"' 
            )) = 1
            begin

              UPDATE HP_COMPS
              SET ISACTIVE = " + v.tComp.SP_COMP_ISACTIVE.ToString() + @"
                 ,COMP_FIRM_GUID = '" + v.tComp.SP_COMP_FIRM_GUID + @"'
                 ,SYSTEM_NAME = '" + v.tComp.SP_COMP_SYSTEM_NAME + @"'
              WHERE NETWORK_MACADDRESS = '" + v.tComp.SP_COMP_MACADDRESS + @"'
            end  

            if ( select count(*) ADET from HP_COMPS
                 where NETWORK_MACADDRESS = '" + v.tComp.SP_COMP_MACADDRESS + @"'
            ) = 0
            begin
              INSERT INTO HP_COMPS
                (SYS_ID
                ,ISACTIVE
                ,COMP_FIRM_GUID
                ,SYSTEM_NAME
                ,NETWORK_MACADDRESS
                ,PROCESSOR_ID
                ,REC_DATE)
              VALUES
                (" + v.tComp.SP_COMP_ID.ToString() + @"
                ,1
                ,'" + v.tComp.SP_COMP_FIRM_GUID + @"'
                ,'" + v.tComp.SP_COMP_SYSTEM_NAME + @"'
                ,'" + v.tComp.SP_COMP_MACADDRESS + @"'
                ,'" + v.tComp.SP_COMP_PROCESSOR_ID + @"'
                ,getdate()
                )
            end    ";

            if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                s = "";

                return s; 
        }


        private string Sql_HP_USERS()
        {

            string s = "";

            if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                s = @"
            if ( select count(*) ADET from HP_USERS
                 where ID > 0 
                 and  USER_EMAIL = '" + v.tUser.SP_USER_EMAIL + @"'
                 and ( ISACTIVE <> " + v.tUser.SP_USER_ISACTIVE.ToString() + @"
                   or  USER_FIRM_GUID <> '" + v.tUser.SP_USER_FIRM_GUID + @"'
                   or  USER_FIRSTNAME <> '" + v.tUser.SP_USER_FIRSTNAME + @"' 
                   or  USER_LASTNAME <> '" + v.tUser.SP_USER_LASTNAME + @"'
            )) = 1
            begin
              UPDATE HP_USERS
              SET ISACTIVE = " + v.tUser.SP_USER_ISACTIVE.ToString() + @"
                 ,USER_FIRM_GUID = '" + v.tUser.SP_USER_FIRM_GUID + @"'
                 ,USER_FIRSTNAME = '" + v.tUser.SP_USER_FIRSTNAME + @"'
                 ,USER_LASTNAME = '" + v.tUser.SP_USER_LASTNAME + @"' 
              WHERE USER_EMAIL = '" + v.tUser.SP_USER_EMAIL + @"'
            end

            if ( select count(*) ADET from HP_USERS
                 where USER_EMAIL = '" + v.tUser.SP_USER_EMAIL + @"'
            ) = 0
            begin
              INSERT INTO HP_USERS
                (SYS_ID
                ,USER_FIRM_GUID
                ,ISACTIVE
                ,USER_GUID
                ,USER_FULLNAME
                ,USER_FIRSTNAME
                ,USER_LASTNAME
                ,USER_EMAIL
                ,REC_DATE)
              VALUES
                (" + v.tUser.SP_USER_ID.ToString() + @"
                ,'" + v.tUser.SP_USER_FIRM_GUID + @"'
                ,1
                ,'" + v.tUser.SP_USER_GUID + @"'
                ,'" + v.tUser.SP_USER_FULLNAME + @"'
                ,'" + v.tUser.SP_USER_FIRSTNAME + @"'
                ,'" + v.tUser.SP_USER_LASTNAME + @"'
                ,'" + v.tUser.SP_USER_EMAIL + @"'
                ,getdate()
                )
            end     ";

            if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                s = "";

            return s;
        }

        private string Sql_HP_FIRM_Insert(
            Int16  isActive,
            string myGuid,
            string localTd, 
            string firmId,
            string parentId,
            string firmName )
        {
            string s = "";

            if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
            s = @"
                if ( Select count(*) ADET from [HP_FIRMS] where 0 = 0 
                     and [FIRM_GUID] = '" + myGuid + @"' 
                ) = 0 
                begin 
                  insert into [HP_FIRMS] ( 
                       ISACTIVE, 
                       REC_DATE, 
                       FIRM_GUID, 
                       LOCAL_TD, 
                       LOCAL_ID, 
                       PARENT_ID, 
                       FIRM_NAME,
                       HP_NAME
                       ) values ( 1 , getdate() 
                      , '" + myGuid + @"'
                      , " + localTd + @"
                      , " + firmId + @"
                      , " + parentId + @"
                      , '" + firmName + @"'
                      , '" + firmName + @"' ) 
                end  

                if ( select count(*) ADET from HP_FIRMS
                     where ID > 0 
                     and FIRM_GUID = '" + myGuid + @"'
                     and ( ISACTIVE <> " + isActive.ToString() + @"
                       or  LOCAL_TD <> " + localTd + @" 
                       or  PARENT_ID <> " + parentId + @"
                       or  FIRM_NAME <> '" + firmName + @"' 
                )) = 1
                begin
                  UPDATE HP_FIRMS
                    SET ISACTIVE = " + isActive.ToString() + @"
                       ,LOCAL_TD = " + localTd + @"
                       ,PARENT_ID = " + parentId + @"
                       ,FIRM_NAME = '" + firmName + @"'
                  WHERE FIRM_GUID = '" + myGuid + @"'
                end  ";

            if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                s = @"
                  insert into HP_FIRMS ( 
                       ISACTIVE, 
                       REC_DATE, 
                       FIRM_GUID, 
                       LOCAL_TD, 
                       LOCAL_ID, 
                       PARENT_ID, 
                       FIRM_NAME,
                       HP_NAME
                       ) 
                   select 1 , curdate() 
                      , '" + myGuid + @"'
                      ,  " + localTd + @"
                      ,  " + firmId + @"
                      ,  " + parentId + @"
                      , '" + firmName + @"'
                      , '" + firmName + @"' 
                   from HP_FIRMS   
                   where not exists 
                   ( Select count(*) ADET from [HP_FIRMS] where 0 = 0 
                     and [FIRM_GUID] = '" + myGuid + @"' )  
                   limit 1;
            ";

            ///*       -- update
                      

            //    if ( select count(*) ADET from HP_FIRMS
            //         where ID > 0 
            //         and FIRM_GUID = '" + myGuid + @"'
            //         and ( ISACTIVE <> " + isActive.ToString() + @"
            //           or  LOCAL_TD <> " + localTd + @" 
            //           or  PARENT_ID <> " + parentId + @"
            //           or  FIRM_NAME <> '" + firmName + @"' 
            //    )) = 1
            //    begin
            //      UPDATE HP_FIRMS
            //        SET ISACTIVE = " + isActive.ToString() + @"
            //           ,LOCAL_TD = " + localTd + @"
            //           ,PARENT_ID = " + parentId + @"
            //           ,FIRM_NAME = '" + firmName + @"'
            //      WHERE FIRM_GUID = '" + myGuid + @"'
            //    end  ";
            //*/



            return s;

            #region
            /*
            begin transaction 
              if ( Select count(*) ADET from [HP_FIRM] where 0 = 0 
                 and [FIRM_GUID] = null 
                 ) = 0 
                 begin 
                 insert into [HP_FIRM] ( 
                       ISACTIVE, 
                       REC_DATE, 
                       FIRM_GUID, 
                       LOCAL_TD, 
                       LOCAL_ID, 
                       PARENT_ID, 
                       FIRM_NAME, 
                       HP_CODE, 
                       HP_NAME, 
                       HP_LONG_NAME1, 
                       HP_LONG_NAME2, 
                       OLD_ID ) values 
                 ( 0,   null,   null,   0,   0,   0,   null,   null,   null,   null,   null,   0 ) 
                  select MAX(ID) as ID from HP_FIRM 
                 end else 
                   Select 0 as ID 
            commit transaction 
            */
            #endregion

        }

        private string Sql_HP_UPDATES_Insert(
              string updateDate
            , string exeName
            , Int16 updateTd
            , int updateNo
            , string versionNo
            , string packetName
            , string about
            )
        {
            string s = "";

            if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                s = @"
                if ( Select count(*) ADET from [HP_UPDATES] where 0 = 0 
                     and [COMP_KEY] = '" + v.tComputer.Network_MACAddress + @"' 
                     and [UPDATE_TD] = "+ updateTd + @"
                     and [UPDATE_NO] = "+ updateNo + @"
                ) = 0 
                begin 
                  insert into [dbo].[HP_UPDATES]
                   ([COMP_KEY]
                   ,[ISACTIVE]
                   ,[REC_DATE]
                   ,[UPDATE_DATE]
                   ,[EXE_NAME]
                   ,[UPDATE_TD]
                   ,[UPDATE_NO]
                   ,[VERSION_NO]
                   ,[PACKET_NAME]
                   ,[ABOUT]) values ( 
                        '" + v.tComputer.Network_MACAddress + @"'
                      , 0 -- ISACTIVE 
                      , getdate() 
                      , " + Tarih_Formati(Convert.ToDateTime(updateDate)) + @"
                      , '" + exeName + @"'
                      , " + updateTd + @"
                      , " + updateNo + @"
                      , '" + versionNo + @"'
                      , '" + packetName + @"'
                      , '" + about + @"' ) 
                end 

                Select top 1 * from [HP_UPDATES] 
                where ISACTIVE = 0 
                and [COMP_KEY] = '" + v.tComputer.Network_MACAddress + @"' 
                and [UPDATE_TD] = " + updateTd + @"
                and [UPDATE_NO] = " + updateNo + @" ";

            return s;
        }

        public void SYS_Glyph_Read()
        {
            tSQLs sql = new tSQLs();

            string Sql = sql.SQL_MS_GLYPH_LIST();

            if (IsNotNull(Sql))
                SQL_Read_Execute(v.dBaseNo.Manager, v.ds_Icons, ref Sql, "MS_GLYPH", "");
        }

        // LKP_ olan fieldler ve onlara ait listeler
        public void SYS_Types_List(DataSet ds, string List_Name)
        {
            tSQLs sql = new tSQLs();
            
            string Sql = sql.SQL_Types_List(List_Name);
            
            if (IsNotNull(Sql))
            {
                SQL_Read_Execute(v.dBaseNo.Manager, ds, ref Sql, "SYS_TYPES_" + List_Name, "");

                if (IsNotNull(ds))
                {
                    string s = "";

                    s = ds.Tables[0].Rows[0]["VALUE_INT"].ToString();

                    // Eğer -9 ise Data tablolarından okunması gerekiyor
                    if (s == "-9")
                    {
                        Sql = SQL_Data_Types(ds, List_Name);
                        SQL_Read_Execute(v.dBaseNo.Project, ds, ref Sql, "LKP_TABLE_" + List_Name, "");
                    }
                }

            }

        }

        private string SQL_Data_Types(DataSet ds, string Type_Name) // İPTAL
        {
            // İPTAL
            string s = string.Empty;

            /*
            tToolBox t = new tToolBox();

            
            string val_type_ = t.Set(ds.Tables[0].Rows[0]["VALUE_TYPE"].ToString(), "", "");
            string str_fname = t.Set(ds.Tables[0].Rows[0]["VALUE_STR"].ToString(), "", "null");
            string int_fname = t.Set(ds.Tables[0].Rows[0]["VALUE_INT_FNAME"].ToString(), "", "null");
            string cap_fname = t.Set(ds.Tables[0].Rows[0]["VALUE_CAPTION"].ToString(), "", "");
            string where = t.Set(ds.Tables[0].Rows[0]["WHERE_SQL"].ToString(), "", "");
            if (t.IsNotNull(where)) where = " where " + where;
            string tableName = Type_Name;

            s = " select "
            + "   " + val_type_ + "  VALUE_TYPE "
            + " , " + int_fname + "  VALUE_INT "
            + " , " + str_fname + "  VALUE_STR "
            + " , " + cap_fname + "  VALUE_CAPTION "
            + " from " + tableName + " a "
            + where 
            ;

            //+ " select "
            //+ "   VALUE_TYPE "
            //+ " , -9  VALUE_INT "
            //+ " , STR_FNAME      VALUE_STR "
            //+ " , CAPTION_FNAME  VALUE_CAPTION "
            //+ " , INT_FNAME      VALUE_INT_FNAME "
            //+ " from SYS_TYPES_T a "
            //+ " where a.TABLE_NAME = @type_name "
            */
            return s;
        }

        #endregion SYS_Types_List

        #region Computer MOS

        public void Find_MOS_MacAddr()
        {
            String macAddr = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();

            //MessageBox.Show(macAddr.ToString());

            /*
            v.SQL = "";
            string id = "";
            ManagementObjectSearcher MOS = new ManagementObjectSearcher("root\\CIMV2", "Select * From Win32_NetworkAdapter");
            foreach (ManagementObject obj in MOS.Get())
            {
                foreach (PropertyData data in obj.Properties)
                {
                    if ((data.Value != null) && 
                        ((data.Name == "MACAddress") |
                         (data.Name == "InterfaceIndex") |
                         (data.Name == "Caption")
                        ))
                    {
                        id = id + v.ENTER + "Name : " + data.Name + "    Value : " + data.Value.ToString();
                        v.SQL = v.SQL + v.ENTER + "Name : " + data.Name.PadLeft(26) + " : " + data.Value.ToString();
                    }
                }
            }
            MOS.Dispose();

            MessageBox.Show(id);
            */
        }

        public void Find_MOS(string hwclass, string propertName)
        {
            
            string id = "";

            v.SQL = "";
            
            ManagementObjectSearcher MOS = new ManagementObjectSearcher("root\\CIMV2", "Select * From " + hwclass);//Win32_BaseBoard");
            foreach (ManagementObject obj in MOS.Get())
            {
                id = id + v.ENTER + obj.ToString();
                //Convert.ToString(obj[propertName]);

                foreach (PropertyData data in obj.Properties)
                {
                    if (data.Value != null)
                    {
                        id = id + v.ENTER + "Name : " + data.Name + "    Value : " + data.Value.ToString();
                        v.SQL = v.SQL + v.ENTER + "Name : " + data.Name.PadLeft(26) + " : " + data.Value.ToString();
                    }
                }

            }
            MOS.Dispose();

            /*
            var macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up

                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            */


            /// https://msdn.microsoft.com/en-us/library/aa389273(v=vs.85).aspx

            ///Win32_Processor
            ///
            ///string Name 
            ///string ProcessorId
            ///string SystemName
            ///
            ///Win32_DiskDrive
            ///
            ///string   SerialNumber : 5VEKCDC2
            ///string   Model        : ST9500325AS
            ///string   Name         : \\.\PHYSICALDRIVE0

        }

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

        #endregion Computer MOS

        #region TableFind

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public void tTableFind(string tableName)
        {
            String Sql = "";

            // Gerekli olan verileri topla
            DataSet ds = new DataSet();
            vTable vt = new vTable();
            
            if (v.active_DB.runDBaseNo == v.dBaseNo.Manager)
            {
                vt.DBaseNo = v.active_DB.managerDBaseNo;
                vt.DBaseType = v.active_DB.managerDBType;
                vt.DBaseName = v.active_DB.managerDBName;
            }

            if (v.active_DB.runDBaseNo == v.dBaseNo.Project)
            {
                vt.DBaseNo = v.active_DB.projectDBaseNo;
                vt.DBaseType = v.active_DB.projectDBType;
                vt.DBaseName = v.active_DB.projectDBName;
            }
            
            if (vt.DBaseType == v.dBaseType.MSSQL)
            {
                Sql = string.Format(" select * from {0}.sys.tables where name = '{1}' ", vt.DBaseName, tableName);

                vt.msSqlConnection = v.active_DB.projectMSSQLConn;
            }

            if (vt.DBaseType == v.dBaseType.MySQL)
            {
                Sql = string.Format(" SHOW FULL TABLES FROM {0} where Tables_in_{1} = '{2}'", vt.DBaseName, vt.DBaseName, tableName);

                vt.mySqlConnection = v.active_DB.projectMySQLConn;
            }

            SQL_Read_Execute(vt.DBaseNo, ds, ref Sql, "", "Table Find");

            if (IsNotNull(ds) == false)
            {
                MessageBox.Show(tableName + " tablosu yok, oluşturulacak...");
                                
                tTableCreate(tableName, vt);
            }
            //else MessageBox.Show(tableName + " tablosu mevcut");

            ds.Dispose();
        }

        private void tTableCreate(string tableName, vTable vt)
        {
            string fname = string.Empty;
            Boolean filenotfound = false;

            try
            {
                fname = v.EXE_PATH + "\\VT_MSSQL\\" + tableName + ".txt";
                if (File.Exists(@"" + fname))
                {
                    //FileStream fileStream = new FileStream(@"" + fname, FileMode.Open, FileAccess.Read);

                    DataSet ds = new DataSet();
                    string Sql = null;
                    System.IO.TextReader readFile = new StreamReader(@"" + fname);
                    Sql = readFile.ReadToEnd();

                    if (Sql_ExecuteNon(ds, ref Sql, vt))
                    {
                        MessageBox.Show("Başarıyla tablo açıldı : " + tableName);
                    }

                    readFile.Close();
                    readFile = null;
                    filenotfound = false;
                    ds.Dispose();
                }
                else filenotfound = true;

                if (filenotfound == true)
                {
                    MessageBox.Show("File not found : " + fname);
                }
            }
            catch
            { }

        }
        
        public void tTableUpdate(string DatabaseName, string FileName, SqlConnection SqlConn)
        {
            string fname = v.EXE_PATH + "\\VT_MSSQL\\" + FileName + ".txt";

            try
            {
                if (File.Exists(@"" + fname))
                {
                    //FileStream fileStream = new FileStream(@"" + fname, FileMode.Open, FileAccess.Read);
                    
                    DataSet ds = new DataSet();
                    string Sql = null;
                    System.IO.TextReader readFile = new StreamReader(@"" + fname);
                    Sql = readFile.ReadToEnd();
                    if (Sql.Length > 20)
                    {
                        //if (db.SQL_ExecuteNon(VTConnect, ref s, "TableUpdate"))
                        if (SQL_ExecuteNon(SqlConn, ds, ref Sql, "tTableUpdate"))
                        {
                            //MessageBox.Show("Update başarıyla gerçekleşti : " + FileName);
                        }
                    }
                    readFile.Close();
                    readFile = null;
                    ds.Dispose();
                }
                else
                {
                    MessageBox.Show("File not found : " + fname);
                }
            }
            catch
            { }
        }

        public void tTable_FieldAdd(string DatabaseName, string TableName, string FieldName, string FieldType, SqlConnection SqlConn)
        {
            string Sql =
            @" IF not EXISTS (     
                 select  
                 a.column_id, a.name, 
                 convert(smallInt, a.system_type_id) system_type_id, 
                 convert(smallInt, a.user_type_id) user_type_id, 
                 a.max_length, a.precision, a.scale, a.is_nullable, a.is_identity 
                 from 
                 [" + DatabaseName + @"].sys.columns a,  
                 [" + DatabaseName + @"].sys.tables b 
                 where b.object_id = a.object_id 
                 and   b.name = '" + TableName + @"'
                 and   a.name = '" + FieldName + @"' 
                 )
                 begin
                   ALTER TABLE " + TableName + @" ADD " + FieldName + @" " + FieldType + @" NULL 
                 end
                 ";

            DataSet ds = new DataSet();

            SQL_ExecuteNon(SqlConn, ds, ref Sql, "tTable_FieldAdd");

            ds.Dispose();

        }

        public void tTable_FieldNameChange(string DatabaseName, string Table_Name, 
                    string OldFieldName, string NewFieldName, SqlConnection SqlConn)
        {
            string Sql =
            @"
            IF EXISTS (
              select a.column_id, a.name, 
                     convert(smallInt, a.system_type_id) system_type_id, 
                     convert(smallInt, a.user_type_id) user_type_id, 
                     a.max_length, a.precision, a.scale, a.is_nullable, a.is_identity 
                     from 
                     [" + DatabaseName + @"].sys.columns a,  
                     [" + DatabaseName + @"].sys.tables b 
                     where b.object_id = a.object_id 
                     and   b.name = '" + Table_Name + @"' 
                     and   a.name = '" + OldFieldName + @"'
            )  
            begin
              USE " + DatabaseName + @"
              EXEC sp_rename '" + Table_Name + @"." + OldFieldName + @"', '" + NewFieldName + @"', 'COLUMN';
            end
             ";

            DataSet ds = new DataSet();

            SQL_ExecuteNon(SqlConn, ds, ref Sql, "tTable_FieldNameChange");

            ds.Dispose();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedureName"></param>
        public void tStoredProceduresFind(string procedureName)
        {
            String Sql = "";

            // Gerekli olan verileri topla
            DataSet ds = new DataSet();
            vTable vt = new vTable();

            if (v.active_DB.runDBaseNo == v.dBaseNo.Manager)
            {
                vt.DBaseNo = v.active_DB.managerDBaseNo;
                vt.DBaseType = v.active_DB.managerDBType;
                vt.DBaseName = v.active_DB.managerDBName;
            }

            if (v.active_DB.runDBaseNo == v.dBaseNo.Project)
            {
                vt.DBaseNo = v.active_DB.projectDBaseNo;
                vt.DBaseType = v.active_DB.projectDBType;
                vt.DBaseName = v.active_DB.projectDBName;
            }

            if (vt.DBaseType == v.dBaseType.MSSQL)
            {
                Sql = string.Format(" select * from {0}.sys.procedures where name = '{1}' ", vt.DBaseName, procedureName);

                vt.msSqlConnection = v.active_DB.projectMSSQLConn;
            }

            if (vt.DBaseType == v.dBaseType.MySQL)
            {
                Sql = string.Format(" SHOW PROCEDURE STATUS Where Name = '{0}' ", procedureName);

                vt.mySqlConnection = v.active_DB.projectMySQLConn;
            }

            SQL_Read_Execute(vt.DBaseNo, ds, ref Sql, "", "Procedure Find");

            if (IsNotNull(ds) == false)
            {
                MessageBox.Show(procedureName + " Stored Procedure yok, oluşturulacak...");

                tStoredProcedureCreate(procedureName, vt);
            }
            //else MessageBox.Show(tableName + " tablosu mevcut");

            ds.Dispose();

        }

        private void tStoredProcedureCreate(string procedureName, vTable vt)
        {
            string fname = string.Empty;
            Boolean filenotfound = false;

            try
            {
                fname = v.EXE_PATH + "\\VT_MSSQL\\" + procedureName + ".txt";
                if (File.Exists(@"" + fname))
                {
                    //FileStream fileStream = new FileStream(@"" + fname, FileMode.Open, FileAccess.Read);

                    DataSet ds = new DataSet();
                    string Sql = null;
                    System.IO.TextReader readFile = new StreamReader(@"" + fname);
                    Sql = readFile.ReadToEnd();

                    int bgn = 0;
                    int end = 0;

                    if (vt.DBaseType == v.dBaseType.MSSQL)
                    {
                        bgn = Sql.IndexOf("CREATE PROCEDURE");
                        end = Sql.IndexOf("END;");
                        Sql = Sql.Substring(bgn, (end - bgn) + 3);
                    }

                    if (vt.DBaseType == v.dBaseType.MySQL)
                    {
                        bgn = Sql.IndexOf("DELIMITER $$");
                        end = Sql.IndexOf("END $$;");
                        Sql = Sql.Substring(bgn, (end - bgn) + 7);
                    }



                    if (Sql_ExecuteNon(ds, ref Sql, vt))
                    {
                        MessageBox.Show("Başarıyla Stored Procedure açıldı : " + procedureName);
                    }

                    readFile.Close();
                    readFile = null;
                    filenotfound = false;
                    ds.Dispose();
                }
                else filenotfound = true;

                if (filenotfound == true)
                {
                    MessageBox.Show("File not found : " + fname);
                }
            }
            catch
            { }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="triggerName"></param>
        public void tTriggerFind(string triggerName)
        {
            String Sql = "";

            // Gerekli olan verileri topla
            DataSet ds = new DataSet();
            vTable vt = new vTable();

            if (v.active_DB.runDBaseNo == v.dBaseNo.Manager)
            {
                vt.DBaseNo = v.active_DB.managerDBaseNo;
                vt.DBaseType = v.active_DB.managerDBType;
                vt.DBaseName = v.active_DB.managerDBName;
            }

            if (v.active_DB.runDBaseNo == v.dBaseNo.Project)
            {
                vt.DBaseNo = v.active_DB.projectDBaseNo;
                vt.DBaseType = v.active_DB.projectDBType;
                vt.DBaseName = v.active_DB.projectDBName;
            }

            if (vt.DBaseType == v.dBaseType.MSSQL)
            {
                Sql = string.Format(" select * from {0}.sys.triggers where name = '{1}' ", vt.DBaseName, triggerName);

                vt.msSqlConnection = v.active_DB.projectMSSQLConn;
            }

            if (vt.DBaseType == v.dBaseType.MySQL)
            {
                Sql = string.Format(" SHOW TRIGGERS like = '%{0}%' ", triggerName);

                vt.mySqlConnection = v.active_DB.projectMySQLConn;
            }

            SQL_Read_Execute(vt.DBaseNo, ds, ref Sql, "", "Trigger Find");

            if (IsNotNull(ds) == false)
            {
                MessageBox.Show(triggerName + " Trigger yok, oluşturulacak...");

                tTriggerCreate(triggerName, vt);
            }
            //else MessageBox.Show(tableName + " tablosu mevcut");

            ds.Dispose();

        }

        private void tTriggerCreate(string triggerName, vTable vt)
        {
            string fname = string.Empty;
            Boolean filenotfound = false;

            try
            {
                fname = v.EXE_PATH + "\\VT_MSSQL\\" + triggerName + ".txt";
                if (File.Exists(@"" + fname))
                {
                    //FileStream fileStream = new FileStream(@"" + fname, FileMode.Open, FileAccess.Read);

                    DataSet ds = new DataSet();
                    string Sql = null;
                    System.IO.TextReader readFile = new StreamReader(@"" + fname);
                    Sql = readFile.ReadToEnd();

                    int bgn = 0;
                    int end = 0;

                    if (vt.DBaseType == v.dBaseType.MSSQL)
                    {
                        bgn = Sql.IndexOf("CREATE TRIGGER");
                        end = Sql.IndexOf("END;");
                        Sql = Sql.Substring(bgn, (end - bgn) + 3);
                    }

                    if (vt.DBaseType == v.dBaseType.MySQL)
                    {
                        bgn = Sql.IndexOf("DELIMITER $$");
                        end = Sql.IndexOf("END $$;");
                        Sql = Sql.Substring(bgn, (end - bgn) + 7);
                    }



                    if (Sql_ExecuteNon(ds, ref Sql, vt))
                    {
                        MessageBox.Show("Başarıyla Trigger açıldı : " + triggerName);
                    }

                    readFile.Close();
                    readFile = null;
                    filenotfound = false;
                    ds.Dispose();
                }
                else filenotfound = true;

                if (filenotfound == true)
                {
                    MessageBox.Show("File not found : " + fname);
                }
            }
            catch
            { }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public void tPreparingData(string tableName)
        {
            String Sql = "";

            // Gerekli olan verileri topla
            DataSet ds = new DataSet();
            vTable vt = new vTable();

            tableName = tableName.Replace("data_", "");

            if (v.active_DB.runDBaseNo == v.dBaseNo.Project)
            {
                vt.DBaseNo = v.active_DB.projectDBaseNo;
                vt.DBaseType = v.active_DB.projectDBType;
                vt.DBaseName = v.active_DB.projectDBName;
            }

            if (vt.DBaseType == v.dBaseType.MSSQL)
            {
                Sql = string.Format(" select count(*) ADET from {0}.dbo.{1} ", vt.DBaseName, tableName);
                
                if (tableName == "SYS_TYPES_L")
                    Sql = @" select count(ID) ADET from  SYS_TYPES_L where TYPES_NAME not like 'ICRA_%' and TYPES_NAME not like 'AVK_%' ";

                if (tableName == "SYS_TYPES_L_AVK")
                    Sql = @" select count(ID) ADET from  SYS_TYPES_L where TYPES_NAME like 'ICRA_%' or TYPES_NAME like 'AVK_%' ";
                                
                vt.msSqlConnection = v.active_DB.projectMSSQLConn;
            }


            SQL_Read_Execute(vt.DBaseNo, ds, ref Sql, "", "Preparing Data");

            if (IsNotNull(ds))
            {
                if (ds.Tables[0].Rows[0][0].ToString() == "0")
                {
                    MessageBox.Show(tableName + " tablosunda DATA yok, eklenecek...");

                    tDataInsert(tableName, vt);
                }
            }
            //else MessageBox.Show("olmadı");

            ds.Dispose();
        }

        private void tDataInsert(string tableName, vTable vt)
        {
            string fname = string.Empty;
            Boolean filenotfound = false;

            try
            {
                fname = v.EXE_PATH + "\\VT_MSSQL\\" + "data_" + tableName + ".txt";
                if (File.Exists(@"" + fname))
                {
                    //FileStream fileStream = new FileStream(@"" + fname, FileMode.Open, FileAccess.Read);

                    DataSet ds = new DataSet();
                    string Sql = null;
                    //StreamReader reader = new StreamReader(fileStream, Encoding.GetEncoding("iso-8859-9"), false);
                    // türkçe sorunu çözüldü
                    System.IO.TextReader readFile = new StreamReader(@"" + fname, Encoding.GetEncoding("iso-8859-9"), false);
                    //System.IO.TextReader readFile = new StreamReader(@"" + fname);

                    Sql = readFile.ReadToEnd();

                    int bgn = 0;
                    int end = 0;

                    if (vt.DBaseType == v.dBaseType.MSSQL)
                    {
                        bgn = Sql.IndexOf("-- INSERT_BEGIN;");
                        end = Sql.IndexOf("-- INSERT_END;");
                        Sql = Sql.Substring(bgn, (end - bgn) + 3);
                    }

                    if (vt.DBaseType == v.dBaseType.MySQL)
                    {
                        bgn = Sql.IndexOf("DELIMITER $$");
                        end = Sql.IndexOf("END $$;");
                        Sql = Sql.Substring(bgn, (end - bgn) + 7);
                    }


                    
                    if (Sql_ExecuteNon(ds, ref Sql, vt))
                    {
                        MessageBox.Show("Data başarıyla eklendi : " + tableName);
                    }
                    
                    readFile.Close();
                    readFile = null;
                    filenotfound = false;
                    ds.Dispose();
                }
                else filenotfound = true;

                if (filenotfound == true)
                {
                    MessageBox.Show("File not found : " + fname);
                }
            }
            catch
            { }

        }



        #endregion

        #region  Create_PropertiesEdit_Model_JSON
        public string Create_PropertiesEdit_Model_JSON(string TableName, string Main_FieldName)
        {
            string value = string.Empty;

            #region PROP_SUBVIEW
            if (Main_FieldName == "PROP_SUBVIEW")
            {
                PROP_SUBVIEW obj = new PROP_SUBVIEW();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "SV_LIST")
            {
                SV_LIST obj = new SV_LIST();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            #endregion

            #region PROP_JOINTABLE
            if (Main_FieldName == "PROP_JOINTABLE")
            {
                PROP_JOINTABLE obj = new PROP_JOINTABLE();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "J_TABLE")
            {
                J_TABLE obj = new J_TABLE();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "J_WHERE")
            {
                J_WHERE obj = new J_WHERE();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "J_STN_FIELDS")
            {
                J_STN_FIELDS obj = new J_STN_FIELDS();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "J_CASE_FIELDS")
            {
                J_CASE_FIELDS obj = new J_CASE_FIELDS();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            #endregion
            
            #region PROP_VIEWS - MS_TABLES_IP
            if ((Main_FieldName == "PROP_VIEWS") && (TableName == "MS_TABLES_IP"))
            {
                PROP_VIEWS_IP obj = new PROP_VIEWS_IP();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "ALLPROP")
            {
                ALLPROP obj = new ALLPROP();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "GRID")
            {
                GRID obj = new GRID();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "TILE")
            {
                TILE obj = new TILE();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            #endregion
            
            #region PROP_VIEWS - MS_ITEMS
            if ((Main_FieldName == "PROP_VIEWS") && (TableName == "MS_ITEMS"))
            {
                PROP_VIEWS_ITEMS obj = new PROP_VIEWS_ITEMS();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "NAVBAR")
            {
                NAVBAR obj = new NAVBAR();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "NAVIGATIONPANE")
            {
                NAVIGATIONPANE obj = new NAVIGATIONPANE();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "ACCORDION")
            {
                ACCORDION obj = new ACCORDION();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            #endregion

            #region PROP_VIEWS - MS_LAYOUT - MS_GROUPS
            if ((Main_FieldName == "PROP_VIEWS") && (TableName == "MS_LAYOUT"))
            {
                PROP_VIEWS_LAYOUT obj = new PROP_VIEWS_LAYOUT();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if ((Main_FieldName == "PROP_VIEWS") && (TableName == "MS_GROUPS"))
            {
                PROP_VIEWS_GROUPS obj = new PROP_VIEWS_GROUPS();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if ((Main_FieldName == "PROP_VIEWS") && (TableName == "MS_LAYOUT"))
            {
                PROP_VIEWS_LAYOUT obj = new PROP_VIEWS_LAYOUT();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "DOCKPANEL")
            {
                DOCKPANEL obj = new DOCKPANEL();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "SPLIT")
            {
                SPLIT obj = new SPLIT();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "EDITPANEL")
            {
                EDITPANEL obj = new EDITPANEL();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "TLP")
            {
                TLP obj = new TLP();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "TLP_COLUMNS")
            {
                TLP_COLUMNS obj = new TLP_COLUMNS();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "TLP_ROWS")
            {
                TLP_ROWS obj = new TLP_ROWS();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            #endregion
            
            #region PROP_RUNTIME
            if (Main_FieldName == "PROP_RUNTIME")
            {
                PROP_RUNTIME obj = new PROP_RUNTIME();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "DRAGDROP")
            {
                DRAGDROP obj = new DRAGDROP();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "PRL_KRT")
            {
                PRL_KRT obj = new PRL_KRT();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "AUTO_LST")
            {
                AUTO_LST obj = new AUTO_LST();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "TABLEIPCODE_LIST2")
            {
                TABLEIPCODE_LIST2 obj = new TABLEIPCODE_LIST2();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            
            #endregion

            #region PROP_NAVIGATOR and 2
            if (Main_FieldName == "PROP_NAVIGATOR")
            {
                PROP_NAVIGATOR obj = new PROP_NAVIGATOR();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
                //var customer1 = JsonConvert.DeserializeAnonymousType(value, obj);
            }
            if (Main_FieldName == "PROP_NAVIGATOR")
            {
                PROP_NAVIGATOR obj = new PROP_NAVIGATOR();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "TABLEIPCODE_LIST")
            {
                TABLEIPCODE_LIST obj = new TABLEIPCODE_LIST();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            #endregion
            
            #region PROP_SEARCH
            if (Main_FieldName == "PROP_SEARCH")
            {
                PROP_SEARCH obj = new PROP_SEARCH();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            if (Main_FieldName == "GET_FIELD_LIST")
            {
                GET_FIELD_LIST obj = new GET_FIELD_LIST();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            #endregion

            #region PROP_EXPRESSION
            if (Main_FieldName == "PROP_EXPRESSION")
            {
                PROP_EXPRESSION obj = new PROP_EXPRESSION();
                value = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            }
            #endregion

            ////var definition = new { Name = "" };
            //PROP_SUBVIEW definition = new PROP_SUBVIEW();
            //string json1 = thisValue;
            //json1 = json1.Replace((char)34, (char)39);
            //var customer1 = JsonConvert.DeserializeAnonymousType(json1, definition);   


            /// {  "CAPTION": null,  "SV_VALUE": null,  "TABLEIPCODE": null }
            /// şeklinde gelen yapıyı
            /// {  "CAPTION": "null",  "SV_VALUE": "null",  "TABLEIPCODE": "null" }
            /// şekline çeviriyoruz

            string s = ": " + (char)34 + "null" + (char)34;
            Str_Replace(ref value, ": null", s);

            /// sonucu gönder
            return value;
        }
        #endregion  Create_PropertiesEdit_Model_JSON

        #endregion Diğer

        #region *Find Functions

        #region Form

        public Form Find_Form(string FormCode)
        {
            // Eğer formu formcode ile bulamazsan ismiylede bulabilirsin
            //Form tForm = Application.OpenForms["tForm_" + Name];
            
            Form tForm = null;

            int t = Application.OpenForms.Count;

            for (int i = 0; i < t; i++)
            {
                if (Application.OpenForms[i].AccessibleDescription == FormCode)
                {
                    return Application.OpenForms[i];
                }
            }

            return tForm;
        }

        public Form Find_Form(object sender)
        {
            string function_name = "Find_Form(object)";
            Takipci(function_name, "", '{');

            // Eğer formu object ile bulamazsan ismiylede bulabilirsin
            //Form tForm = Application.OpenForms["tForm_" + Name];
                        
            string MyObjectName = sender.ToString();

            Form tForm = null;

            if (MyObjectName.IndexOf("DevExpress.XtraWizard.WizardControl") > -1)
            {
                tForm = ((DevExpress.XtraWizard.WizardControl)sender).FindForm();
                return tForm;
            }
            if (MyObjectName.IndexOf("DevExpress.XtraBars.BarManager") > -1)
            {
                tForm = ((DevExpress.XtraBars.BarManager)sender).Form.FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraBars.Ribbon.RibbonBarManager")
            {
                tForm = ((DevExpress.XtraBars.Ribbon.RibbonBarManager)sender).Form.FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraBars.BarButtonItem")
            {
                tForm = (((DevExpress.XtraBars.BarButtonItem)sender).Manager).Form.FindForm();
               //tForm = ((DevExpress.XtraBars.BarButtonItem)sender).Manager.Form.FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraBars.BarLargeButtonItem")
            {
                tForm = (((DevExpress.XtraBars.BarLargeButtonItem)sender).Manager).Form.FindForm();
                return tForm;
            }
            

            if (MyObjectName == "DevExpress.XtraEditors.ImageComboBoxEdit")
            {
                tForm = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraEditors.CheckEdit")
            {
                tForm = ((DevExpress.XtraEditors.CheckEdit)sender).FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraEditors.DateEdit")
            {
                tForm = ((DevExpress.XtraEditors.DateEdit)sender).FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraEditors.SimpleButton")
            {
                tForm = ((DevExpress.XtraEditors.SimpleButton)sender).FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraEditors.ButtonEdit")
            {
                tForm = ((DevExpress.XtraEditors.ButtonEdit)sender).FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraEditors.TextEdit")
            {
                tForm = ((DevExpress.XtraEditors.TextEdit)sender).FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraEditors.DataNavigator")
            {
                tForm = ((DevExpress.XtraEditors.DataNavigator)sender).FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraGrid.GridControl")
            {
                tForm = ((GridControl)sender).FindForm();
                return tForm;
            }
            if (MyObjectName.IndexOf("DevExpress.XtraEditors.ZoomTrackBarControl") > -1)
            {
                tForm = ((DevExpress.XtraEditors.ZoomTrackBarControl)sender).FindForm();
                return tForm;
            }
            
            if (MyObjectName == "DevExpress.XtraEditors.ListBoxControl")
            {
                tForm = ((DevExpress.XtraEditors.ListBoxControl)sender).FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraEditors.CheckButton")
            {
                tForm = ((DevExpress.XtraEditors.CheckButton)sender).FindForm();
                return tForm;
            }            
            if (MyObjectName == "DevExpress.XtraEditors.GroupControl")
            {
                tForm = ((DevExpress.XtraEditors.GroupControl)sender).FindForm();
                return tForm;
            }

            if (MyObjectName == "DevExpress.XtraTreeList.TreeList")
            {
                tForm = ((DevExpress.XtraTreeList.TreeList)sender).FindForm();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView")
            {
                tForm = ((DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView)sender).GridControl.FindForm();
                return tForm;
            }

            //DevExpress.XtraGrid.Views.WinExplorer.WinExplorerView

            if (MyObjectName == "DevExpress.XtraEditors.TileItem")
            {
                //    tForm = ((DevExpress.XtraEditors.TileItem)sender). // FindForm();
            }

            if (tForm == null)
            {
                MessageBox.Show("DİKKAT : " + MyObjectName + " ile tForm tespiti başarısız oldu ...", function_name);
            }

            Takipci(function_name, "", '}');

            return tForm;
        }

        public Form Find_TableIPCode_XtraEditors(object sender, ref string TableIPCode, 
               ref string FieldName, ref string Value, ref string Tag)
        {
            string function_name = "Find TableIPCode";
            Takipci(function_name, "", '{');

            Form tForm = null;

            string MyObjectName = sender.ToString();
            
            //tEdit.Name = "Column_" + tFieldName;
            //tEdit.Properties.AccessibleName = TableIPCode;

            if (MyObjectName == "DevExpress.XtraEditors.ImageComboBoxEdit")
            {
                tForm = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).FindForm();
                TableIPCode = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).Properties.AccessibleName;
                FieldName = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).Name.ToString();
                FieldName = FieldName.Substring(7, FieldName.Length - 7); // = "Column_" + tFieldName;
                Value = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).EditValue.ToString();
                if (((DevExpress.XtraEditors.ImageComboBoxEdit)sender).Tag != null)
                    Tag = ((DevExpress.XtraEditors.ImageComboBoxEdit)sender).Tag.ToString();
                return tForm;
            }
            if (MyObjectName == "DevExpress.XtraEditors.CheckEdit")
            {
                tForm = ((DevExpress.XtraEditors.CheckEdit)sender).FindForm();
                TableIPCode = ((DevExpress.XtraEditors.CheckEdit)sender).Properties.AccessibleName;
                FieldName = ((DevExpress.XtraEditors.CheckEdit)sender).Name.ToString();
                FieldName = FieldName.Substring(7, FieldName.Length - 7); // = "Column_" + tFieldName;
                Value = ((DevExpress.XtraEditors.CheckEdit)sender).EditValue.ToString();
                if (((DevExpress.XtraEditors.CheckEdit)sender).Tag != null)
                    Tag = ((DevExpress.XtraEditors.CheckEdit)sender).Tag.ToString();
                return tForm;
            }

            if (MyObjectName == "DevExpress.XtraEditors.CalcEdit")
            {
                tForm = ((DevExpress.XtraEditors.CalcEdit)sender).FindForm();
                TableIPCode = ((DevExpress.XtraEditors.CalcEdit)sender).Properties.AccessibleName;
                FieldName = ((DevExpress.XtraEditors.CalcEdit)sender).Name.ToString();
                FieldName = FieldName.Substring(7, FieldName.Length - 7); 
                Value = ((DevExpress.XtraEditors.CalcEdit)sender).EditValue.ToString();
                if (((DevExpress.XtraEditors.CalcEdit)sender).Tag != null)
                    Tag = ((DevExpress.XtraEditors.CalcEdit)sender).Tag.ToString();
                return tForm;
            }
            
            if (MyObjectName == "DevExpress.XtraEditors.TextEdit")
            {
                tForm = ((DevExpress.XtraEditors.TextEdit)sender).FindForm();
                TableIPCode = ((DevExpress.XtraEditors.TextEdit)sender).Properties.AccessibleName;
                FieldName = ((DevExpress.XtraEditors.TextEdit)sender).Name.ToString();
                FieldName = FieldName.Substring(7, FieldName.Length - 7); // = "Column_" + tFieldName;
                Value = ((DevExpress.XtraEditors.TextEdit)sender).EditValue.ToString();
                if (((DevExpress.XtraEditors.TextEdit)sender).Properties.Tag != null)
                    Tag = ((DevExpress.XtraEditors.TextEdit)sender).Properties.Tag.ToString();
                return tForm;
            }

            if (MyObjectName == "DevExpress.XtraEditors.DateEdit")
            {
                tForm = ((DevExpress.XtraEditors.DateEdit)sender).FindForm();
                TableIPCode = ((DevExpress.XtraEditors.DateEdit)sender).Properties.AccessibleName;
                FieldName = ((DevExpress.XtraEditors.DateEdit)sender).Name.ToString();
                FieldName = FieldName.Substring(7, FieldName.Length - 7); // = "Column_" + tFieldName;
                Value = ((DevExpress.XtraEditors.DateEdit)sender).EditValue.ToString();
                if (((DevExpress.XtraEditors.DateEdit)sender).Tag != null)
                    Tag = ((DevExpress.XtraEditors.DateEdit)sender).Tag.ToString();
                return tForm;
            }

            if (MyObjectName == "DevExpress.XtraEditors.ToggleSwitch")
            {
                tForm = ((DevExpress.XtraEditors.ToggleSwitch)sender).FindForm();
                TableIPCode = ((DevExpress.XtraEditors.ToggleSwitch)sender).Properties.AccessibleName;
                FieldName = ((DevExpress.XtraEditors.ToggleSwitch)sender).Name.ToString();
                FieldName = FieldName.Substring(7, FieldName.Length - 7); // = "Column_" + tFieldName;
                Value = ((DevExpress.XtraEditors.ToggleSwitch)sender).EditValue.ToString();
                if (((DevExpress.XtraEditors.ToggleSwitch)sender).Tag != null)
                    Tag = ((DevExpress.XtraEditors.ToggleSwitch)sender).Tag.ToString();
                return tForm;
            }
         
            
            Takipci(function_name, "", '}');

            return tForm;
        }

        #endregion Form

        #region Control

        public string Find_TableIPCode(Control cntrl)
        {
            string code = string.Empty;

            if (cntrl != null)
                if (cntrl.AccessibleName != null)
                    code = cntrl.AccessibleName.ToString();

            return code;
        }

        public Control Find_Control(Form tForm, string controlName)
        {
            string[] cs = new string[] { };
            Control c = Find_Control(tForm, controlName, "", cs);
            return c;
        }

        public Control Find_Control(Form tForm, string Name, string AccessibleName, string[] ControlType)
        {
            if (tForm == null)
            {
                //MessageBox.Show("DİKKAT : Aranan nesne için önce -Form-un tespit edilmesi gerekir ( Form tanımsız, belirsiz ) ...", "Find_Control");
                return null;
            }

            if (Name == "tMyFormBox")
                Name = "tMyFormBox_" + tForm.Name.ToString();
            if (Name == "tMyMemo")
                Name = "tMyMemo_" + tForm.Name.ToString();
            if (Name == "tMyGroup")
                Name = "tMyGroup_" + tForm.Name.ToString();


            byte i = 0;
            int x = ControlType.Length;

            if ((Name != "") && (AccessibleName == "") && (x == 0)) i = 1;
            if ((Name == "") && (AccessibleName != "") && (x == 0)) i = 2;
            if ((Name == "") && (AccessibleName == "") && (x != 0)) i = 3;

            if ((Name != "") && (AccessibleName != "") && (x == 0)) i = 4;
            if ((Name == "") && (AccessibleName != "") && (x != 0)) i = 5;
            if ((Name != "") && (AccessibleName == "") && (x != 0)) i = 6;

            if ((Name != "") && (AccessibleName != "") && (x != 0)) i = 7;

            // FieldName göre control tespiti
            if (AccessibleName == "FIELDNAME") i = 8;

            // controlun TAG üzerindeki MASTEITEM_NO kontrol edilecek
            if (AccessibleName == "TAG") i = 9;

            if (AccessibleName == "CR.CR_OMARA_L01")
            {
                v.Kullaniciya_Mesaj_Var = AccessibleName;
            }

            #region Control1

            if (v.ControlListView)
            {
                v.ControlList = string.Empty;
                v.ControlList = v.ControlList + "[ *** " + tForm.Name.ToString() + " *** ]" + v.ENTER2;
            }

            foreach (Control c in tForm.Controls)
            {
                
                if (Control_(c, Name, AccessibleName, ControlType, i)) return c;

                if (c.Controls.Count > 0)
                {
                    #region Control2
                    foreach (Control c2 in c.Controls)
                    {
                        if (Control_(c2, Name, AccessibleName, ControlType, i)) return c2;

                        if (c2.Controls.Count > 0)
                        {
                            #region Control3
                            foreach (Control c3 in c2.Controls)
                            {
                                if (Control_(c3, Name, AccessibleName, ControlType, i)) return c3;

                                if (c3.Controls.Count > 0)
                                {
                                    #region Control4
                                    foreach (Control c4 in c3.Controls)
                                    {
                                        if (Control_(c4, Name, AccessibleName, ControlType, i)) return c4;

                                        if (c4.Controls.Count > 0)
                                        {
                                            #region Control5
                                            foreach (Control c5 in c4.Controls)
                                            {
                                                if (Control_(c5, Name, AccessibleName, ControlType, i)) return c5;

                                                if (c5.Controls.Count > 0)
                                                {
                                                    #region Control6
                                                    foreach (Control c6 in c5.Controls)
                                                    {
                                                        if (Control_(c6, Name, AccessibleName, ControlType, i)) return c6;

                                                        if (c6.Controls.Count > 0)
                                                        {
                                                            #region Control7
                                                            foreach (Control c7 in c6.Controls)
                                                            {
                                                                if (Control_(c7, Name, AccessibleName, ControlType, i)) return c7;

                                                                if (c7.Controls.Count > 0)
                                                                {
                                                                    #region Control8
                                                                    foreach (Control c8 in c7.Controls)
                                                                    {
                                                                        if (Control_(c8, Name, AccessibleName, ControlType, i)) return c8;

                                                                        if (c8.Controls.Count > 0)
                                                                        {
                                                                            #region Control9
                                                                            foreach (Control c9 in c8.Controls)
                                                                            {
                                                                                if (Control_(c9, Name, AccessibleName, ControlType, i)) return c9;

                                                                                if (c9.Controls.Count > 0)
                                                                                {
                                                                                    #region Control10
                                                                                    foreach (Control c10 in c9.Controls)
                                                                                    {
                                                                                        if (Control_(c10, Name, AccessibleName, ControlType, i)) return c10;

                                                                                        if (c10.Controls.Count > 0)
                                                                                        {
                                                                                            #region Control11
                                                                                            foreach (Control c11 in c10.Controls)
                                                                                            {
                                                                                                if (Control_(c11, Name, AccessibleName, ControlType, i)) return c11;

                                                                                                if (c11.Controls.Count > 0)
                                                                                                {
                                                                                                    #region Control12
                                                                                                    foreach (Control c12 in c11.Controls)
                                                                                                    {
                                                                                                        if (Control_(c12, Name, AccessibleName, ControlType, i)) return c12;

                                                                                                        if (c12.Controls.Count > 0)
                                                                                                        {
                                                                                                            #region Control13
                                                                                                            foreach (Control c13 in c12.Controls)
                                                                                                            {
                                                                                                                if (Control_(c13, Name, AccessibleName, ControlType, i)) return c13;

                                                                                                                if (c13.Controls.Count > 0)
                                                                                                                {
                                                                                                                    #region Control14
                                                                                                                    foreach (Control c14 in c13.Controls)
                                                                                                                    {
                                                                                                                        if (Control_(c14, Name, AccessibleName, ControlType, i)) return c14;

                                                                                                                        if (c14.Controls.Count > 0)
                                                                                                                        {
                                                                                                                            #region Control15
                                                                                                                            foreach (Control c15 in c14.Controls)
                                                                                                                            {
                                                                                                                                if (Control_(c15, Name, AccessibleName, ControlType, i)) return c15;

                                                                                                                                if (c15.Controls.Count > 0)
                                                                                                                                {
                                                                                                                                    //...
                                                                                                                                }
                                                                                                                            }
                                                                                                                            #endregion // Control13
                                                                                                                        }
                                                                                                                    }
                                                                                                                    #endregion // Control14
                                                                                                                }
                                                                                                            }
                                                                                                            #endregion // Control13
                                                                                                        }
                                                                                                    }
                                                                                                    #endregion // Control12
                                                                                                }
                                                                                            }
                                                                                            #endregion // Control11
                                                                                        }
                                                                                    }
                                                                                    #endregion // Control10
                                                                                }
                                                                            }
                                                                            #endregion // Control9
                                                                        }
                                                                    }
                                                                    #endregion // Control8
                                                                }

                                                            }
                                                            #endregion // Control7
                                                        }
                                                    }
                                                    #endregion // Control6
                                                }
                                            }
                                            #endregion // Control5
                                        }
                                    }
                                    #endregion // Control4
                                }

                            }
                            #endregion // Control3
                        }
                    }
                    #endregion // Control2
                }
            }
            #endregion // Control1

            if (v.ControlListView)
                v.SQL = v.ControlList + v.SQL;

            return null;
        }

        public void External_Controls_Enabled(Form tForm, DataSet dsData, Control cntrl)
        {
            /// ismi aynı olmayan yalnız aynı dataset i kullanan diğer cnrtl ise
            /// EXTERNAL_TABLE_IP_CODE kulanan cntrl leride enabled özelliğini değiştirmek için
            ///
            string myProp = dsData.Namespace.ToString();

            /// kendine bağlı IP yok ise geri dönsün
            /// 
            if (myProp.IndexOf("External_IP:True") == -1) return;

            string Name = cntrl.Name;
            // TableIPCode
            string Extrenal_TableIPCode = cntrl.AccessibleName;

            #region Control1
            foreach (Control c in tForm.Controls)
            {
                if (c.AccessibleDefaultActionDescription != null)
                {
                    if ((c.Name != Name) && (c.AccessibleDefaultActionDescription == Extrenal_TableIPCode))
                    {
                        c.Enabled = IsNotNull(dsData);
                    }
                }

                if (c.Controls.Count > 0)
                {
                    #region Control2
                    foreach (Control c2 in c.Controls)
                    {
                        if (c2.AccessibleDefaultActionDescription != null)
                        {
                            if ((c2.Name != Name) && (c2.AccessibleDefaultActionDescription == Extrenal_TableIPCode))
                            {
                                c2.Enabled = IsNotNull(dsData);
                            }
                        }

                        if (c2.Controls.Count > 0)
                        {
                            #region Control3
                            foreach (Control c3 in c2.Controls)
                            {
                                if (c3.AccessibleDefaultActionDescription != null)
                                {
                                    if ((c3.Name != Name) && (c3.AccessibleDefaultActionDescription == Extrenal_TableIPCode))
                                    {
                                        c3.Enabled = IsNotNull(dsData);
                                    }
                                }

                                if (c3.Controls.Count > 0)
                                {
                                    #region Control4
                                    foreach (Control c4 in c3.Controls)
                                    {
                                        if (c4.AccessibleDefaultActionDescription != null)
                                        {
                                            if ((c4.Name != Name) && (c4.AccessibleDefaultActionDescription == Extrenal_TableIPCode))
                                            {
                                                c4.Enabled = IsNotNull(dsData);
                                            }
                                        }

                                        if (c4.Controls.Count > 0)
                                        {
                                            #region Control5
                                            foreach (Control c5 in c4.Controls)
                                            {
                                                if (c5.AccessibleDefaultActionDescription != null)
                                                {
                                                    if ((c5.Name != Name) && (c5.AccessibleDefaultActionDescription == Extrenal_TableIPCode))
                                                    {
                                                        c5.Enabled = IsNotNull(dsData);
                                                    }
                                                }

                                                if (c5.Controls.Count > 0)
                                                {
                                                    #region Control6
                                                    foreach (Control c6 in c5.Controls)
                                                    {
                                                        if (c6.AccessibleDefaultActionDescription != null)
                                                        {
                                                            if ((c6.Name != Name) && (c6.AccessibleDefaultActionDescription == Extrenal_TableIPCode))
                                                            {
                                                                c6.Enabled = IsNotNull(dsData);
                                                            }
                                                        }

                                                        if (c6.Controls.Count > 0)
                                                        {
                                                            #region Control7
                                                            foreach (Control c7 in c6.Controls)
                                                            {
                                                                if (c7.AccessibleDefaultActionDescription != null)
                                                                {
                                                                    if ((c7.Name != Name) && (c7.AccessibleDefaultActionDescription == Extrenal_TableIPCode))
                                                                    {
                                                                        c7.Enabled = IsNotNull(dsData);
                                                                    }
                                                                }

                                                                if (c7.Controls.Count > 0)
                                                                {
                                                                    #region Control8
                                                                    foreach (Control c8 in c7.Controls)
                                                                    {
                                                                        if (c8.AccessibleDefaultActionDescription != null)
                                                                        {
                                                                            if ((c8.Name != Name) && (c8.AccessibleDefaultActionDescription == Extrenal_TableIPCode))
                                                                            {
                                                                                c8.Enabled = IsNotNull(dsData);
                                                                            }
                                                                        }

                                                                        if (c8.Controls.Count > 0)
                                                                        {
                                                                            #region Control9
                                                                            foreach (Control c9 in c8.Controls)
                                                                            {
                                                                                if (c9.AccessibleDefaultActionDescription != null)
                                                                                {
                                                                                    if ((c9.Name != Name) && (c9.AccessibleDefaultActionDescription == Extrenal_TableIPCode))
                                                                                    {
                                                                                        c9.Enabled = IsNotNull(dsData);
                                                                                    }
                                                                                }

                                                                                if (c9.Controls.Count > 0)
                                                                                {


                                                                                }

                                                                            }
                                                                            #endregion // Control9

                                                                        }

                                                                    }
                                                                    #endregion // Control8

                                                                }

                                                            }
                                                            #endregion // Control7

                                                        }

                                                    }
                                                    #endregion // Control6

                                                }

                                            }
                                            #endregion // Control5

                                        }

                                    }
                                    #endregion // Control4

                                }

                            }
                            #endregion // Control3
                        }

                    }
                    #endregion // Control2
                }
            }
            #endregion Control1
        }

        private Boolean Control_(Control c, string Name, string AccessibleName, string[] ControlType, int i)
        {
            Boolean sonuc = false;

            //if (Name.ToUpper() == "TEST")
            //    MessageBox.Show(c.Name + "//" + c.AccessibleName + "//" + c.ToString() + "//" + c.GetType());

            //if (v.Takip_Find == true)
            //    v.SQL = v.SQL + 
            //        (c.Name + "//" + c.AccessibleName + "//" + c.ToString() + "//" + c.GetType()+v.ENTER);

            if (v.ControlListView)
            {
                v.ControlList = v.ControlList + c.ToString() + " ( " + c.Name.ToString() + " | " + c.Text + " )" + v.ENTER;

                //if (c.Name.IndexOf("tDataNavigator_") > -1)
                //{
                //    v.Kullaniciya_Mesaj_Var = c.Name;  
                //}

                if (AccessibleName == "CR.CR_OMARA_L01")
                {
                    v.Kullaniciya_Mesaj_Var = AccessibleName;
                }

            }

            Boolean controls = false;
            if ((i == 3) || (i == 5) || (i == 6) || (i == 7))
            {
                for (int t = 0; t < ControlType.Length; t++)
                {
                    if (c.ToString() == ControlType[t])
                    { controls = true; break; }
                }
            }

            if (i == 1) 
                if (c.Name == Name) sonuc = true;
            if (i == 2) if (c.AccessibleName == AccessibleName) sonuc = true;
            if (i == 3) if (controls == true) sonuc = true;
            if (i == 4) if ((c.Name == Name) && (c.AccessibleName == AccessibleName)) sonuc = true;
            if (i == 5)
                if ((c.AccessibleName == AccessibleName) && (controls == true)) sonuc = true;
            if (i == 6) if ((c.Name == Name) && (controls == true)) sonuc = true;
            if (i == 7) if ((c.Name == Name) && (c.AccessibleName == AccessibleName) && (controls == true)) sonuc = true;

            // Controlun FieldName bakarak control tespiti, 
            // FieldName de controlun Name ne veriliyor   
            // tEdit.Name = "Column_" + tfieldname;    şeklinde
            if (i == 8)
            {
                if ((c.Name.IndexOf(Name) > -1) &&
                    (c.Name.IndexOf("Panel_") == -1))
                    sonuc = true;
                // MessageBox.Show(c.Name.IndexOf(Name).ToString());
            }

            // Controlun Tag üzerindeki MASTER_ITEM_NO kontrol edilecek
            if (i == 9)
                if (c.Tag != null)
                    if (c.Tag.ToString() == Name) sonuc = true;

            return sonuc;
        }

        public Control Find_Control_Tag(Form tForm, string Tag)
        {
            string[] ControlType = { };

            return Find_Control(tForm, Tag, "TAG", ControlType);
        }

        public Control Find_Control_View(Form tForm, string TableIPCode)
        {
            
            Control cntrl = new Control();
            string[] controls = new string[] { "DevExpress.XtraGrid.GridControl", 
                                               "DevExpress.XtraVerticalGrid.VGridControl",
                                               "DevExpress.XtraDataLayout.DataLayoutControl",
                                               "DevExpress.XtraTreeList.TreeList",
                                               "DevExpress.XtraGrid.Views.Tile.TileView"
                                               //"DevExpress.XtraEditors.DataNavigator"
                                             }; //"DevExpress.XtraWizard.WizardControl" };

            cntrl = Find_Control(tForm, "", TableIPCode, controls);
            if (cntrl != null)
            {
                return cntrl;
            }
            return null;
        }

        public int Find_Control_Type(Control cntrl)
        {
            int i = 0;

            if (cntrl.ToString().IndexOf("GridControl") > -1) // "DevExpress.XtraGrid.GridControl")
                i = v.obj_vw_GridView;

            if (cntrl.ToString().IndexOf("BandedGridView") > -1)  // "DevExpress.XtraGrid.Views.BandedGrid.BandedGridView")
                i = v.obj_vw_BandedGridView;

            if (cntrl.ToString().IndexOf("AdvBandedGridView") > -1) // "DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView")
                i = v.obj_vw_AdvBandedGridView;

            if (cntrl.ToString().IndexOf("LayoutView") > -1)
                i = v.obj_vw_GridLayoutView;

            if (cntrl.ToString().IndexOf("DataLayoutControl") > -1) // "DevExpress.XtraDataLayout.DataLayoutControl")
                i = v.obj_vw_DataLayoutView;

            if (cntrl.ToString().IndexOf("VGridControl") > -1) // "DevExpress.XtraVerticalGrid.VGridControl")
            {
                if (((DevExpress.XtraVerticalGrid.VGridControl)cntrl).LayoutStyle == LayoutViewStyle.SingleRecordView)
                      i = v.obj_vw_VGridSingle;

                if (((DevExpress.XtraVerticalGrid.VGridControl)cntrl).LayoutStyle == LayoutViewStyle.MultiRecordView)
                    i = v.obj_vw_VGridMulti;
            }

            if (cntrl.ToString().IndexOf("TreeList") > -1) // "DevExpress.XtraTreeList.TreeList")
                i = v.obj_vw_TreeListView;

            if (cntrl.ToString().IndexOf("DataNavigator") > -1) // "DevExpress.XtraEditors.DataNavigator")
                i = v.obj_DataNavigator;

            if (cntrl.ToString().IndexOf("WizardControl") > -1) // "DevExpress.XtraWizard.WizardControl")
                i = v.obj_vw_WizardControl;

            if (i == 0)
            {
                MessageBox.Show(cntrl.ToString() + v.ENTER + "Bu control tanımlı değil...","Find_Control_Type");
            }
            return i;
        }

        public Control Find_PopupContainerEdit(PopupContainerEdit popupContainerEdit, string Name)
        {
            int i2 = popupContainerEdit.Properties.PopupControl.Controls.Count;
            Control c = null;
            for (int i = 0; i < i2; i++)
            {
                c = popupContainerEdit.Properties.PopupControl.Controls[i];
                
                //v.ControlList = v.ControlList + c.ToString() + " ( " + c.Name.ToString() + " )" + v.ENTER;

                if (c.Name.ToString() == Name)
                {
                    return c;
                }
            }
            return null;
        }

        /// <summary>
        /// İstenen isimde klasör açar.Aynı isimde klasör var ise işlem yapmaz.
        /// </summary>
        /// <param name="KlasorIsim">Yazılacak olan klasör ismi C:\\ altında kontrol edilir.Klasör yok ise oluşturulur, var ise işlem yapılmaz.</param>
        public string Find_Path(string PathName)
        {
            if ((Directory.Exists(v.EXE_PATH + "\\" + PathName) == false))
            {
                Directory.CreateDirectory(v.EXE_PATH + "\\" + PathName);
            }
            return v.EXE_PATH + "\\" + PathName + "\\";
        }

        #endregion Control

        #region DataSet

        // Nedenini almadım ! ama daha önce bu çalışıyordu
        // An unhandled exception of type 'System.StackOverflowException' occurred in Project Manager V3.exe
        //
        //public Boolean Find_DataSet(Form tForm, ref DataSet dsData, ref DataNavigator tDataNavigator, string TableIPCode)
        //{
        //    Boolean sonuc = false;
        //    sonuc = Find_DataSet(tForm, ref dsData, ref tDataNavigator, TableIPCode);
        //    return sonuc;
        //}

        //public Boolean Find_DataSet(Form tForm, ref DataSet dsData, ref DataNavigator tDataNavigator, string TableIPCode, string Kim)


        public Boolean Find_DataSet(Form tForm, ref DataSet dsData, ref DataNavigator tDataNavigator, string TableIPCode)
        {
            Boolean sonuc = false;

            // daha önce tespit edilmiş demektir
            if (tDataNavigator != null) return false;

            tDataNavigator = Find_DataNavigator(tForm, TableIPCode);//, Kim);

            if (tDataNavigator != null)
            {
                object tDataTable = tDataNavigator.DataSource;
                if (tDataTable != null)
                {
                    dsData = ((DataTable)tDataTable).DataSet;
                    sonuc = true;
                }
            }
            return sonuc;
        }

        public DataSet Find_DataSet(Form tForm, string CmpName, string TableIPCode, string Arayan)
        {
            // Dikkat : Direk olarak DataSet bulunamıyor
            // Ya CompanentName ile yada TableIPCode uyan bir Control tespit ediliyor
            // Bu Control ün önce DataTable na sonrada onun DataSet ine ulaşılıyor

            string function_name = "Find_DataSet";
            Takipci(function_name, "", '{');

            Control cntrl = new Control();
            string[] controls = new string[] { "DevExpress.XtraGrid.GridControl", 
                                               "DevExpress.XtraVerticalGrid.VGridControl",
                                               "DevExpress.XtraDataLayout.DataLayoutControl",
                                               "DevExpress.XtraTreeList.TreeList",
                                               "DevExpress.XtraEditors.DataNavigator",
                                               "DevExpress.XtraTab.XtraTabPage",
                                               "DevExpress.XtraTab.XtraTabControl"

                                             }; //"DevExpress.XtraWizard.WizardControl" };

            cntrl = Find_Control(tForm, CmpName, TableIPCode, controls);

            Takipci(function_name, "", '}');

            if (cntrl != null)
            {
                object tDataTable = new object();
                
                if (cntrl.ToString() == "DevExpress.XtraGrid.GridControl")
                    tDataTable = ((DevExpress.XtraGrid.GridControl)cntrl).DataSource;

                if (cntrl.ToString() == "DevExpress.XtraVerticalGrid.VGridControl")
                    tDataTable = ((DevExpress.XtraVerticalGrid.VGridControl)cntrl).DataSource;

                if (cntrl.ToString() == "DevExpress.XtraDataLayout.DataLayoutControl")
                    tDataTable = ((DevExpress.XtraDataLayout.DataLayoutControl)cntrl).DataSource;

                if (cntrl.ToString() == "DevExpress.XtraTreeList.TreeList")
                    tDataTable = ((DevExpress.XtraTreeList.TreeList)cntrl).DataSource;

                if (cntrl.ToString() == "DevExpress.XtraEditors.DataNavigator")
                    tDataTable = ((DevExpress.XtraEditors.DataNavigator)cntrl).DataSource;

                if (cntrl.ToString() == "DevExpress.XtraWizard.WizardControl")
                {
                    // üzerindeki DevExpress.XtraDataLayout.DataLayoutControl ait datasource ulaşılıyor 
                    MessageBox.Show("DevExpress.XtraWizard.WizardControl  için datasource metodu ???");
                }

                ////DevExpress.XtraGrid.Views.Layout.LayoutView
                if (tDataTable != null)
                {
                    if (tDataTable.ToString() != "System.Object")
                    {
                        DataSet tdsData = ((DataTable)tDataTable).DataSet;

                        //************************************************************
                        // gerek yok zaten kendisi herşeyi atıyormuş
                        // eğer bilgiler uçuyorsa başka bir yerde Namespace = "" vardır 
                        //************************************************************
                        
                        // üzerine yamadığımız bilgilerde transfer olsun

                        //tdsData.DataSetName = ((DataTable)tDataTable).DataSet.DataSetName;
                        //tdsData.Namespace = ((DataTable)tDataTable).DataSet.Namespace;

                        //if (tdsData.Tables.Count > 0)
                        //{
                        //    tdsData.Tables[0].Namespace = ((DataTable)tDataTable).DataSet.Tables[0].Namespace;
                        //}

                        return tdsData;
                    }
                    else return null;
                }
                else return null;
            }
            else
            {
                if ((CmpName.IndexOf("tMyDataNavigator_") == -1) && (Arayan != ""))
                {
                    if (TableIPCode != "null")
                        MessageBox.Show("DİKKAT : [ " + TableIPCode + " ] için DataControl tespit edilemedi ..." + v.ENTER +
                                        "Kim Arıyor : " + Arayan, function_name);
                    if (TableIPCode == "")
                        MessageBox.Show("DİKKAT : DataControl tespit yapılacak fakat TableIPCode parametresi boş geldi ..." + v.ENTER +
                                        "Kim Arıyor : " + Arayan, function_name);
                }

                return null;
            }

        }

        public void Find_DataSet_List(Form tForm, DataSet dsIPList)
        {
            tToolBox t = new tToolBox();

            Control cntrl = new Control();
            string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };
            List<string> list = new List<string>();
            t.Find_Control_List(tForm, list, controls);

            string myProp = string.Empty;
            string TableIPCode = string.Empty;
            string TableCaption = string.Empty;
            string TableCode = string.Empty;
            string IPCode = string.Empty;

            TableRowDelete(dsIPList, 0);

            int i = 1;
            foreach (string value in list)
            {
                //s = s + value + v.ENTER;
                cntrl = t.Find_Control(tForm, value, "", controls);

                if (cntrl != null)
                {
                    if (((DevExpress.XtraEditors.DataNavigator)cntrl).AccessibleName != null)
                    {
                        object tDataTable = ((DevExpress.XtraEditors.DataNavigator)cntrl).DataSource;
                        DataSet dsData = ((DataTable)tDataTable).DataSet;

                        if (dsData != null)
                        {
                            myProp = dsData.Namespace;
                            TableIPCode = t.MyProperties_Get(myProp, "TableIPCode:");
                            TableCaption = t.MyProperties_Get(myProp, "TableCaption:");

                            TableIPCode_Get(TableIPCode, ref TableCode, ref IPCode);

                            DataRow row = dsIPList.Tables[0].NewRow();
                            row["ID"] = i;
                            row["TABLECODE"] = TableCode;
                            row["IPCODE"] = IPCode;
                            row["TABLEIPCODE"] = TableIPCode;
                            row["TABLECAPTION"] = TableCaption;

                            dsIPList.Tables[0].Rows.Add(row);
                            i++;
                        }
                    }
                }
            }
        }
        
        public string Find_TableState(DataSet ds, DataNavigator dN, string TableName, Int16 TableNo)
        {
            string id_value = string.Empty;
            string State = string.Empty;
            try
            {
                // sıfırın column her zaman ID veya REF_ID kabul edildiği için
                if (IsNotNull(TableName))
                    id_value = ds.Tables[TableName].Rows[dN.Position][0].ToString();
                if ((IsNotNull(TableName) == false) &&
                    (TableNo > -1))
                    id_value = ds.Tables[TableNo].Rows[dN.Position][0].ToString();
            }
            catch
            {
                return ""; // dataset üzerinde veri yoksa burada hata olarak yakalanır
            }

            if ((id_value == "") || (id_value == "-999"))
                State = "dsInsert";
            else State = "dsEdit";

            return State;
        }


        #endregion DataSet

        #region DataNavigator
        //public DataNavigator Find_DataNavigator(Form tForm, string TableIPCode, string Kim)
        public DataNavigator Find_DataNavigator(Form tForm, string TableIPCode)
        {
            string function_name = "tDataNavigator_Find";
            Takipci(function_name, TableIPCode, '{');

            //v.Takip_Find = true;
            Control cntrl = new Control();
            string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };
            cntrl = Find_Control(tForm, "", TableIPCode, controls);

            Takipci(function_name, "", '}');

            if (cntrl != null)
            {
                DataNavigator tDataNavigator = ((DevExpress.XtraEditors.DataNavigator)cntrl);
                return tDataNavigator;
            }
            else
            {
                //if ((TableIPCode != "null") &&
                //    (TableIPCode != "tDataNavigator_CatList") && 
                //    (Kim != "tEkle_Butonu"))
                //    MessageBox.Show("DİKKAT : [ {" + Kim + "} için gerekli olan " + TableIPCode + " ] DataNavigator tespit edilemedi ...", Kim + " >> " + function_name);
                return null;
            }

        }
        #endregion DataNavigator

        #region DataNavigator_Position
        
        public int Find_DataNavigator_Position(Form tForm, string TableIPCode)
        {
            string function_name = "tDataNavigator_Position_Find";
            Takipci(function_name, "", '{');

            DataNavigator tDataNavigator = null;

            tDataNavigator = Find_DataNavigator(tForm, TableIPCode);//, function_name);

            Takipci(function_name, "", '}');

            if (tDataNavigator != null)
            {
                int i = -1;

                try
                {
                    object x = tDataNavigator.Tag;
                    i = ((int)x);
                }
                catch
                {
                    i = -1;
                }

                if (i == -1) i = tDataNavigator.Position;

                return i;
            }
            else
            {
                return -1;
            }

        }
        
        public int Find_DataNavigator_Position(Form tForm, DataSet dsData, string TableIPCode)
        {
            //string function_name = "Find_DataNavigator_Position-2";
            int snc = -1;
            if (dsData != null)
            {
                if (dsData.Tables[0].Rows.Count == 1)
                {
                    snc = 0;
                }
                else
                {
                    DataNavigator tDataNavigator =  Find_DataNavigator(tForm, TableIPCode);//, function_name);
                    if (tDataNavigator != null)
                    {
                        snc = tDataNavigator.Position;
                    }
                }
            }

            return snc;
        }
        
        #endregion DataNavigator_Position

        #region DBaseNo
        public byte Find_DBaseNo(string DatabaseName)
        {
            byte dbNo = 0;

            // "master"
            if ((DatabaseName.ToUpper() == "MASTER") || (DatabaseName == "1"))
                dbNo = (byte)v.dBaseNo.Master;

            //  "MANAGERSERVER"
            if ((DatabaseName.ToUpper() == v.active_DB.managerDBName.ToUpper()) || (DatabaseName == "2"))
                dbNo = (byte)v.dBaseNo.Manager;

            //  "SystemMS"
            if ((DatabaseName.ToUpper() == v.db_MAINMANAGER_DBNAME.ToUpper()) || (DatabaseName == "4"))
                dbNo = (byte)v.dBaseNo.MainManager;

            //  projenin adı
            if ((DatabaseName.ToUpper() == v.active_DB.projectDBName.ToUpper()) || (DatabaseName == "3") ||
                (DatabaseName == ""))
                dbNo = (byte)v.dBaseNo.Project;
            /*
            insert into MS_TYPES values( 1, 0, 'DB_TYPE',  0, '', 'none');
            insert into MS_TYPES values( 1, 0, 'DB_TYPE',  1, '', 'master');
            insert into MS_TYPES values( 1, 0, 'DB_TYPE',  2, '', 'ManagerServer');
            insert into MS_TYPES values( 1, 0, 'DB_TYPE',  3, '', 'Proje');
            insert into MS_TYPES values( 1, 0, 'DB_TYPE',  4, '', 'MainManagerServer');
            */

            if (dbNo != 0)
            {
                return dbNo;
            }
            else
            {
                if (DatabaseName != "null")
                    MessageBox.Show("DİKKAT : [ " + DatabaseName + " ] için Database Connection tespit edilemedi ...");

                return 0;
            }
        }
        #endregion DBaseNo

        #region Find_dBLongName
        public string Find_dBLongName(string DatabaseName)
        {
            string s = string.Empty;

            // "master"
            if ((DatabaseName.ToUpper() == "MASTER") || (DatabaseName == v.dBaseNo.Master.ToString()) || (DatabaseName == "1"))
                s = v.db_MASTER_DBNAME;

            //  "MANAGERSERVER"
            if ((DatabaseName.ToUpper() == v.active_DB.managerDBName.ToUpper()) || (DatabaseName == v.dBaseNo.Manager.ToString()) || (DatabaseName == "2"))
                s = v.active_DB.managerDBName;

            //  proje adı
            if ((DatabaseName.ToUpper() == v.active_DB.projectDBName.ToUpper()) || (DatabaseName == v.dBaseNo.Project.ToString()) || (DatabaseName == "4") ||
                (DatabaseName == ""))
                s = v.active_DB.projectDBName;

            //  "MAINMANAGERSERVER"
            if ((DatabaseName.ToUpper() == v.db_MAINMANAGER_DBNAME.ToUpper()) || (DatabaseName == "3"))
                s = v.db_MAINMANAGER_DBNAME;
            
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

        #region Find_Record
        public Boolean Find_Record(SqlConnection SqlConn, string SQL)
        {
            // SqlConnection SqlConn buna gerek kalmadı silebilirsiniz

            Boolean sonuc = false;

            DataSet ds = new DataSet();

            if (IsNotNull(SQL))
            {
                SQL_Read_Execute(v.dBaseNo.None, ds, ref SQL, "TABLE1", "");

                sonuc = IsNotNull(ds);
            }

            ds.Dispose();

            // false dönerse aranın kayıt yok
            // true  dönerse aranan kayıt mevcut
            return sonuc;
        }
        #endregion Find_Record

        #region Diğerleri >>

        public Image Find_Glyph(string glyphName)
        {
            if (IsNotNull(v.ds_Icons) == false)
                return null;

            //ImageName

            if (glyphName == "KAYDET16") glyphName = "30_341_Save_16x16";

            if (glyphName == "SAVE16") glyphName = "40_424_Save_16x16";
            if (glyphName == "SAVE32") glyphName = "40_424_Save_32x32";
            if (glyphName == "CLOSE16") glyphName = "40_401_Close_16x16";
            if (glyphName == "CLOSE32") glyphName = "40_401_Close_32x32";

            if (glyphName == "YENIHESAP16") glyphName = "40_401_AddFile_16x16";
            if (glyphName == "YENIVAZGEC16") glyphName = "40_401_DeleteList2_16x16";


            if (glyphName == "DELETE16") glyphName = "40_408_Delete_16x16";
            if (glyphName == "DELETE32") glyphName = "40_408_Delete_32x32";

            if (glyphName == "SIHIRBAZGERI16") glyphName = "40_418_Backward_16x16";
            if (glyphName == "SIHIRBAZDEVAM16") glyphName = "40_418_Forward_16x16";

            if (glyphName == "NAVGERI16") glyphName = "40_418_Backward_16x16";
            if (glyphName == "NAVILERI16") glyphName = "40_418_Forward_16x16";


            if (glyphName == "") glyphName = "";
            if (glyphName == "") glyphName = "";

            if (glyphName == "") glyphName = "";
            if (glyphName == "") glyphName = "";

            int i2 = v.ds_Icons.Tables[0].Rows.Count;
            int i3 = -1;
            for (int i = 0; i < i2; i++)
            {
                if (v.ds_Icons.Tables[0].Rows[i]["GLYPH_NAME"].ToString() == glyphName)
                {
                    i3 = i;
                    break;
                }
            }

            byte[] img32 = (byte[])v.ds_Icons.Tables[0].Rows[i3]["GLYPH"];
            if ((img32 != null) && (i3 > -1))
            {
                MemoryStream ms = new MemoryStream(img32);
                return Image.FromStream(ms);
            }

            return null;
        }

        public void Find_DataNavigator_List(Form tForm, ref List<string> list)
        {
            Control cntrl = new Control();
            string[] controls = new string[] { "DevExpress.XtraEditors.DataNavigator" };
            //List<string> list = new List<string>();
            Find_Control_List(tForm, list, controls);
        }

        public void Find_DataNavigator_ButtonLink(Form tForm, string TableIPCode, string Button_Click_Type)
        {
            tToolBox t = new tToolBox();
            string function_name = "Find_DataNavigator_ButtonLink";
            t.Takipci(function_name, "", '{');

            string ButtonName = string.Empty;

            ButtonName = Find_ButtonName(tForm, TableIPCode, Button_Click_Type);

            if (ButtonName != string.Empty)
            {
                //tButton bt = new tButton();
                //bt.tButtonClick(tForm, ButtonName);
            }
            else
            {
                
                if (Button_Click_Type != "11") // Çıkış değilse
                {
                    MessageBox.Show(
                        "DİKKAT : Butona ait bağlantı Link'i bulunamadı ..." + v.ENTER2 +
                        "1. Buton tanımlanmamış olabilir." + v.ENTER +
                        "2. Buton üzerindeki tanımlar eksik olabilir." + v.ENTER +
                        "3. ExternelTableIPCode olabilir. "
                        );
                }

                if (Button_Click_Type == "11") // Çıkış
                {
                    tForm.Close();
                }
            }

            t.Takipci(function_name, "", '}');
        }
        
        public string Find_ButtonName(Form tForm, string TableIPCode, string Button_Click_Type)
        {
            string function_name = "Find_ButtonName";
            Takipci(function_name, "", '{');

            string snc = string.Empty;
            string btn = string.Empty;
            string ButtonName = string.Empty;

            // 1. formun üzerinde create sırasında atanmış olan button hakkındaki bilgiler okunur
            snc = Find_myFormBox_Value(tForm, "");

            int i1 = 0;
            int i2 = 0;

            string s1 = "TableIPCode1:" + TableIPCode;
            string s2 = "=ButtonClickType:" + Button_Click_Type;

            while (snc.IndexOf("//<btn_1>") > -1)
            {
                btn = BeforeGet_And_AfterClear(ref snc, "//<btn_1>", true); 

                i1 = btn.IndexOf(s1);
                i2 = btn.IndexOf(s2);

                if ((i1 > -1) && (i2 > -1))
                {
                    BeforeGet_And_AfterClear(ref btn, "//<btn_0>", false);
                    ButtonName = BeforeGet_And_AfterClear(ref btn, "=Caption:",false);
                    break;
                }
            }

            Takipci(function_name, "", '}');

            return ButtonName;

            /*
            //<btn_0>
            tBarButtonItem_84=Caption:Fiş Aç;
            tBarButtonItem_84=FormCode:22;
            tBarButtonItem_84=SourceTableIPCode1:TABLE_1.t11;
            tBarButtonItem_84=SourceFieldName11:HAREKET_ID;
            tBarButtonItem_84=SourceFieldValue11:null;
            tBarButtonItem_84=TargetTableIPCode1:TABLE_1.t12;
            tBarButtonItem_84=TargetFieldName11:HAREKET_ID;
            tBarButtonItem_84=TargetFieldValue11:null;
            tBarButtonItem_84=ButtonClickType:17;
            tBarButtonItem_84=ProcedureApproval:null;
            tBarButtonItem_84=LookupCode:null;
            tBarButtonItem_84=DefaultValue:null;
            tBarButtonItem_84=END:
            //<btn_1>
            */
        }

        public void myFormBox_Set(Form tForm, string AddValue)
        {
            string[] controls = new string[] { };
            Control c = Find_Control(tForm, "tMyFormBox", "", controls);

            if (c != null)
            {
                ((DevExpress.XtraEditors.MemoEdit)c).EditValue =
                    ((DevExpress.XtraEditors.MemoEdit)c).EditValue + v.ENTER + AddValue;
            }
        }

        public string Find_myFormBox_Value(Form tForm, string ButtonName)
        {
            string function_name = "Find_tMyFormBox_Value(" + ButtonName + ")"; ;
            Takipci(function_name, "", '{');

            string[] controls = new string[] { };
            Control c = Find_Control(tForm, "tMyFormBox", "", controls);
            string s = string.Empty;

            // ButtonName boş ise tMyFormBox üzerindeki tüm bilgiler gönderilecek
            //            boş değilse sadece ButtonName ait bilgiler gönderilecek

            if (c != null)
            {
                if (((DevExpress.XtraEditors.MemoEdit)c).EditValue != null)
                {
                    s = ((DevExpress.XtraEditors.MemoEdit)c).EditValue.ToString();

                    int i1 = 0;
                    int i2 = 0;
                    if ((ButtonName != string.Empty) || (ButtonName != ""))
                    {
                        i1 = s.IndexOf(ButtonName);
                        i2 = s.LastIndexOf(ButtonName + "=END:");

                        if ((i1 > -1) && (i2 > -1))
                            s = s.Substring(i1, i2 - i1);
                    }
                }
            }

            Takipci(function_name, "", '}');

            return s;
        }
                
        public void Find_TableIPCode(string snc, 
                                     ref string myTargetTableIPCode, ref string mySourceTableIPCode)
        {
            // bir form üzerinde birden fazla TableIP olduğu zaman 
            // örnek birden fazla detail tablo olduğu zaman hangisi seçecek ????

            string SourceTableIPCode1 = string.Empty;
            string TargetTableIPCode1 = string.Empty;
            string DetailTableIPCode1 = string.Empty;

            string TargetFieldName11 = string.Empty;
            string TargetFieldName12 = string.Empty;
            string TargetValue1 = string.Empty;
            string TargetValue2 = string.Empty;

            if (snc.Length > 0)
            {
                SourceTableIPCode1 = MyProperties_Get(snc, "=SourceTableIPCode1:");
                TargetTableIPCode1 = MyProperties_Get(snc, "=TargetTableIPCode1:");
                DetailTableIPCode1 = MyProperties_Get(snc, "=DetailTableIPCode1:");
            }
            
            if ((SourceTableIPCode1 == "null") && (SourceTableIPCode1 == "")) SourceTableIPCode1 = string.Empty;
            if ((TargetTableIPCode1 == "null") && (TargetTableIPCode1 == "")) TargetTableIPCode1 = string.Empty;
            if ((DetailTableIPCode1 == "null") && (DetailTableIPCode1 == "")) DetailTableIPCode1 = string.Empty;


            // my ile başlayan değerler 
            if ((TargetTableIPCode1 == string.Empty) &&
                (snc.LastIndexOf("//<myStart>") > -1))
            {
                TargetTableIPCode1 = MyProperties_Get(snc, "myTargetTableIPCode1=");
                TargetFieldName11 = MyProperties_Get(snc, "myTargetFieldName11=");
                TargetFieldName12 = MyProperties_Get(snc, "myTargetFieldName12=");
                TargetValue1 = MyProperties_Get(snc, "myTargetValue1=");
                TargetValue2 = MyProperties_Get(snc, "myTargetValue2=");
            }

            mySourceTableIPCode = string.Empty;

            if ((SourceTableIPCode1 != "") && (TargetTableIPCode1 == "") && (DetailTableIPCode1 == ""))
                myTargetTableIPCode = SourceTableIPCode1;

            if ((SourceTableIPCode1 == "") && (TargetTableIPCode1 != "") && (DetailTableIPCode1 == ""))
                myTargetTableIPCode = TargetTableIPCode1;

            if ((SourceTableIPCode1 == "") && (TargetTableIPCode1 == "") && (DetailTableIPCode1 != ""))
                myTargetTableIPCode = DetailTableIPCode1;

            if ((SourceTableIPCode1 != "") && (TargetTableIPCode1 != ""))
            {
                myTargetTableIPCode = TargetTableIPCode1;
                mySourceTableIPCode = SourceTableIPCode1;
            }

            if ((TargetTableIPCode1 != "") && (DetailTableIPCode1 != ""))
            {
                myTargetTableIPCode = DetailTableIPCode1;
                mySourceTableIPCode = TargetTableIPCode1;
            }
        }

        public string Find_Table_Ref_FieldName(DataSet dsIPFields)
        {
            string fname = string.Empty;

            foreach (DataRow row in dsIPFields.Tables[0].Rows)
            {
                //if (row["LKP_FIELD_NO"].ToString() == "1")
                if ((row["LKP_FAUTOINC"].ToString() == "True") ||
                    (row["LKP_FAUTOINC"].ToString() == "1"))
                {
                    fname = row["LKP_FIELD_NAME"].ToString();
                    break;
                }
            }

            return fname;
        }
        
        

        public bool Find_TableFields(DataSet dsData, SqlConnection SqlConn)
        {
            
            return true;
            /*
            bool onay = false;

            int i1 = dsData.Tables.Count;
            int i2 = 0;
            int i3 = 0;
            string fields = "_FIELDS";
            string myProp = dsData.Namespace.ToString();
            string TableIPCode = Set(MyProperties_Get(myProp, "=TableIPCode:"), "", "");
            string Table_Name = Set(MyProperties_Get(myProp, "=TableName:"), "", "TABLE1");
            
            if (Table_Name.IndexOf("SNL_") > -1)
                return onay;

            if (i1 == 1)
            {
                string FieldsListSQL = Set(MyProperties_Get(myProp, "=FieldsListSQL:"), "", "");
                string sqlA = string.Empty;
                string sqlB = string.Empty;

                if (FieldsListSQL.IndexOf("|ds|") > 0)
                    String_Parcala(FieldsListSQL, ref sqlA, ref sqlB, "|ds|");
                
                if (SqlConn == null)
                {
                    byte DBaseNo = Set(MyProperties_Get(myProp, "=DBaseNo:"), "", (byte)3);
                    MessageBox.Show("eksik : Find_TableFields");
                    SqlConn = null; //Find_DBConn(DBaseNo.ToString());
                }

                if ((FieldsListSQL != "") &&
                    (FieldsListSQL != "null") &&
                    (FieldsListSQL != "report") &&
                    (FieldsListSQL != "param"))
                {
                    SqlDataAdapter adapter = null;

                    if (sqlA != string.Empty)
                    {
                        adapter = new SqlDataAdapter(sqlA, SqlConn);
                        adapter.Fill(dsData, Table_Name + fields);
                    }

                    if (sqlB != string.Empty)
                    {
                        //= TableIPCode:3S_MSTBL.3S_MSTBL_02;

                        //if (TableIPCode.IndexOf("3S_") == -1)
                            adapter = new SqlDataAdapter(sqlB, v.SP_Conn_Manager_MSSQL);
                        //else adapter = new SqlDataAdapter(sqlB, v.SP_Conn_MainManager_MSSQL);

                        adapter.Fill(dsData, Table_Name + fields + "2");
                    }

                    if ((sqlA == string.Empty) &&
                        (sqlB == string.Empty))
                    {
                        adapter = new SqlDataAdapter(FieldsListSQL, SqlConn);
                        adapter.Fill(dsData, Table_Name + fields);
                    }

                    adapter.Dispose();
                }

            }
            
            if (i1 > 1)
            {
                string tname = dsData.Tables[1].TableName;
                // field tablosu mevcut mu
                i2 = tname.IndexOf(fields);
                // field listesi mevcut mu
                i3 = dsData.Tables[1].Rows.Count;
                
                if (i3 == 0)
                    MessageBox.Show("DİKKAT : " + Table_Name + " isimli tablonun field listesi bulunamadı ... " + v.ENTER2 +
                                    "( dsData["+ Table_Name + "_FIELDS].Rows.Count = 0 )", "");

                if ((i2 > 0) && (i3 > 0)) onay = true;
            }

            return onay;
            */
        }

        public int Find_Field_Type_Id(DataSet dsData, string fieldName, ref string displayFormat)
        {
            int ftype = 0;
            
            // fields tablsou varsa
            if (dsData != null)
            {
                if (dsData.Namespace != null)
                {
                    int TableCount = dsData.Tables.Count;

                    if (TableCount > 1)
                    {
                        int i2 = 0;
                        string function_name = "";
                        string myProp = dsData.Namespace.ToString();
                        string Table_Name = Set(MyProperties_Get(myProp, "=TableName:"), "", "TABLE1");
                        string TableFields = Table_Name + "_FIELDS";

                        try
                        {
                            i2 = dsData.Tables[TableFields].Rows.Count;

                            if (i2 == 0)
                                MessageBox.Show("DİKKAT : " + Table_Name + " isimli tablonun field listesi bulunamadı ... " + v.ENTER2 +
                                                "( Tables[xxxx_FIELDS].Rows.Count = 0 )", function_name);
                        }
                        catch
                        {
                            MessageBox.Show("DİKKAT : " + Table_Name + " isimli tablonun field listesi bulunamadı ... " + v.ENTER2 +
                                            "( Tables[xxxx_FIELDS].Rows.Count = 0 )", function_name);
                            i2 = 0;
                        }

                        string ValidationInsert = "";
                        string fForeing = "";
                        string fTrigger = "";
                        //string displayFormat = "";

                        string fname = "";
                        for (int i = 0; i < i2; i++)
                        {
                            fname = dsData.Tables[TableFields].Rows[i]["name"].ToString();
                            if (fname == fieldName)
                            {
                                ftype = myInt32(dsData.Tables[TableFields].Rows[i]["user_type_id"].ToString());

                                OtherValues_Get(dsData, TableFields + "2", fname, 
                                    ref ValidationInsert, ref fForeing, ref fTrigger, ref displayFormat);
                                
                                break;
                            }
                        }

                    }
                }
            }
            return ftype; 
        }

        public void OtherValues_Get(DataSet ds, string tableName, string fName,
            ref string ValidationInsert,
            ref string fForeing,
            ref string fTrigger,
            ref string displayFormat)
        {
            int j = ds.Tables[tableName].Rows.Count;

            for (int i = 0; i < j; i++)
            {
                if (ds.Tables[tableName].Rows[i]["FIELD_NAME"].ToString() == fName)
                {
                    ValidationInsert = Set(ds.Tables[tableName].Rows[i]["VALIDATION_INSERT"].ToString(), "", "False");
                    fForeing = Set(ds.Tables[tableName].Rows[i]["FFOREING"].ToString(), "", "False");
                    fTrigger = Set(ds.Tables[tableName].Rows[i]["FTRIGGER"].ToString(), "", "False");
                    displayFormat = Set(ds.Tables[tableName].Rows[i]["CMP_DISPLAY_FORMAT"].ToString(),"","");

                    break;
                }
            }
        }


        public VGridControl Find_VGridControl(Form tForm, string VGridName, string TableIPCode)
        {
            Control cntrl = new Control();
            string[] controls = new string[] { };

            if (IsNotNull(VGridName))
                cntrl = Find_Control(tForm, VGridName, "", controls);
            if (IsNotNull(TableIPCode))
                cntrl = Find_Control(tForm, "", TableIPCode, controls);
            
            if (cntrl != null)
            {
                VGridControl tVGrid = ((DevExpress.XtraVerticalGrid.VGridControl)cntrl);
                return tVGrid;
            }
            else
            {
                if (IsNotNull(VGridName))
                    MessageBox.Show("DİKKAT : [ " + VGridName + " ] VGrid tespit edilemedi ...", "Find_VGridControl");
                return null;
            }
        }

        public int Find_GotoRecord(DataSet dsData, string FieldName, string Value)
        {
            if (dsData == null)
                return -1;

            if ((IsNotNull(FieldName) == false) || 
                (IsNotNull(Value) == false) || 
                (dsData.Tables.Count < 1))
            {
                return -1;
            }

            // position tespit edilecek 
            int s = -1;
            int rc = dsData.Tables[0].Rows.Count;
            for (int i = 0; i < rc; i++)
            {
                if (dsData.Tables[0].Rows[i][FieldName].ToString() == Value)
                {
                    s = i;
                    break;
                }
            }

            // position döndürülüyor 
            return s;
        }

        public string Find_Kriter_Value(Form tForm, string TableIPCode, string FieldName, byte default_type)
        {
            tToolBox t = new tToolBox();

            string tValue = string.Empty;
            // default_type
            // 51,  'Kriter READ (Even.Bas)' 
            // 52,  'Kriter READ (Even.Bit)' 
            // 53,  'Kriter READ (Odd)'

            // VgridControl tespit ediliyor
            DevExpress.XtraVerticalGrid.VGridControl VGrid = t.Find_VGridControl(tForm, "", "KRITER_" + TableIPCode);


            if (VGrid != null)
            {
                string fullname = string.Empty;
                string ttable_alias = string.Empty;
                string fname = string.Empty;

                string fvalue = string.Empty;
                string fvalue2 = string.Empty;
                //string tmpkriter = string.Empty;
                string fkriter = v.ENTER;
                string foperand = string.Empty;
                string foperand_id = string.Empty;
                int ffieldtype = 0;
                int t1 = VGrid.Rows.Count;
                int t2 = 0;
                
                #region Vgrid row döngüsü
                // Vgrid üzerindeki category sayısı
                for (int i1 = 0; i1 < t1; i1++)
                {
                    // category altındaki edittext sayısı
                    t2 = VGrid.Rows[i1].ChildRows.Count;

                    // t2 = 4 ise sorgu tipi even  (başlangıç, bitiş)
                    // t2 = 2 ise sorgu tipi odd   (tek sorgu)

                    #region categori altı
                    for (int i2 = 0; i2 < t2; i2++)
                    {
                        fullname = VGrid.Rows[i1].ChildRows[i2].Properties.FieldName.ToString();

                        TableIPCode_Get(fullname, ref ttable_alias, ref fname);

                        fvalue = string.Empty;
                        fvalue2 = string.Empty;
                        foperand = string.Empty;
                        foperand_id = string.Empty;
                        ffieldtype = 0;

                        #region // fieldin tipi nedir, tespiti
                        if (VGrid.Rows[i1].ChildRows[i2].Tag != null)
                            ffieldtype = System.Convert.ToInt32(VGrid.Rows[i1].ChildRows[i2].Tag);

                        // field tipine göre veri hazırlnacak 
                        if (VGrid.Rows[i1].ChildRows[i2].Properties.Value != null)
                        {
                            fvalue = VGrid.Rows[i1].ChildRows[i2].Properties.Value.ToString();

                            //fvalue = t.tData_Convert(fvalue, ffieldtype);
                            // bu ikinci veriye Like lar yüzünden ihtiyaç duyuldu.
                            fvalue2 = t.Str_Check(VGrid.Rows[i1].ChildRows[i2].Properties.Value.ToString());
                        }
                        #endregion

                        #region // Vgridin satırı sorgu çeşitleri değilse ve elle girilmiş veri var  ise

                        if ((fname.IndexOf("bas_sorgu_") == -1) &&
                            (fname.IndexOf("bit_sorgu_") == -1) &&
                            (fvalue != ""))
                        {
                            if (FieldName == fname)
                            {
                                //MessageBox.Show(fullname);
                                if ((default_type == 51) && (i2 == 0))
                                {
                                    tValue = fvalue;
                                    break;
                                }
                                if ((default_type == 52) && (i2 == 1))
                                {
                                    tValue = fvalue;
                                    break;
                                }
                                if ((default_type == 53) && (i2 == 0))
                                {
                                    tValue = fvalue;
                                    break;
                                }
                            }

                            /*
                            // operand tipi belirlenecek
                            if (t2 == 4) // even ise (çift)
                            {
                                if (VGrid.Rows[i1].ChildRows[i2 + 2].Properties.Value != null)
                                    foperand_id = VGrid.Rows[i1].ChildRows[i2 + 2].Properties.Value.ToString();
                            }

                            if (t2 == 2) // odd ise (tek)
                            {
                                if (VGrid.Rows[i1].ChildRows[i2 + 1].Properties.Value != null)
                                    foperand_id = VGrid.Rows[i1].ChildRows[i2 + 1].Properties.Value.ToString();
                            }
                            */
                            /*
                            foperand = Kriter_Operand_Find(foperand_id);
                            //---

                            // parçalar bir araya geldi, şimdi and ile başlayan yeni bir where parçası oluştulacak

                            if (foperand == "%%")
                                tmpkriter = tmpkriter + " and " + fname + " like '%" + fvalue2 + "%' " + v.ENTER;
                            else if (foperand == "%")
                                tmpkriter = tmpkriter + " and " + fname + " like '" + fvalue2 + "%' " + v.ENTER;
                            else if (foperand != string.Empty)
                                tmpkriter = tmpkriter + " and " + fname + " " + foperand + " " + fvalue + v.ENTER;

                            // ------

                            if (t.IsNotNull(tmpkriter))
                            {
                                if (t.IsNotNull(tkrt_alias))
                                {
                                    aSQL = t.SQLWhereAdd(aSQL, tkrt_alias, tmpkriter, "KRITERLER");
                                }
                                else
                                {
                                    fkriter = fkriter + tmpkriter;
                                }

                                tmpkriter = string.Empty;
                            }
                            */
                        }
                        #endregion
                        
                    }
                    #endregion
                }
                #endregion
            }

            return tValue;
        }
        
        public string Find_TableIPCode_Value(Form tForm, string TableIPCode, string FieldName)
        {
            string tValue = string.Empty;
                        
            DataSet dS = null;
            DataNavigator dN = null;
            Find_DataSet(tForm, ref dS, ref dN, TableIPCode);

            if ((IsNotNull(dS)) &&
                (dN.Position > -1) &&
                IsNotNull(FieldName))
            {
                tValue = dS.Tables[0].Rows[dN.Position][FieldName].ToString();
            }

            return tValue;
        }

        public string Find_Properies_Get_RowBlock(string thisBlocks, string MainFName, string FindValues)
        {
            /*
            3=ROW_PROP_NAVIGATOR:3;
            3=CAPTION:ccccccc;
     >>     3=BUTTONTYPE:56;
            3=TABLEIPCODE_LIST:null;
            3=FORMNAME:null;
            3=FORMCODE:null;
            3=FORMTYPE:null;
            3=FORMSTATE:null;
            3=ROWE_PROP_NAVIGATOR:3;
            PROP_NAVIGATOR=}
            */

            string s = thisBlocks; 
            string find_block = string.Empty;

            if (thisBlocks.IndexOf(FindValues) > -1)
            {
                Get_And_Clear(ref s, FindValues);
                string block = MyProperties_Get(s, "=ROWE_" + MainFName + ":");
                
                string block_basi = block + "=ROW_" + MainFName;
                string block_sonu = block + "=ROWE_" + MainFName + ":" + block;

                int i1 = thisBlocks.IndexOf(block_basi);
                int i2 = thisBlocks.IndexOf(block_sonu) + block_sonu.Length;

                if ((i1 > -1) && (i2 > i1))
                    find_block = thisBlocks.Substring(i1, i2 - i1);
            }

            return find_block;
        }

        public string Find_Properies_Get_RowBlock(ref string thisBlocks, string MainFName)
        {
            #region  
            /*
            0=J_WHERE:J_WHERE={
            1=ROW_J_WHERE:1;      <<<<< row başı
            1=CAPTION:MS_FIELDS.TABLE_CODE;
            1=J_TABLE_ALIAS:B;
            1=MT_FNAME:TABLE_CODE;
            1=J_FNAME:TABLE_CODE;
            1=FVALUE:null;
            1=ROWE_J_WHERE:1;     <<<<< row sonu
            2=ROW_J_WHERE:2;
            2=CAPTION:MS_FIELDS.FIELD_NO;
            2=J_TABLE_ALIAS:B;
            2=MT_FNAME:FIELD_NO;
            2=J_FNAME:FIELD_NO;
            2=FVALUE:null;
            2=ROWE_J_WHERE:2;
            J_WHERE=};
            */
            #endregion 

            string row_block = string.Empty;

            string row_basi = "=ROW_" + MainFName; 
            string row_sonu = "=ROWE_" + MainFName + ":";

            int i1 = 0;
            int i2 = 0;

            if (thisBlocks.IndexOf(row_sonu) > -1)
            {
                i1 = thisBlocks.IndexOf(row_basi);
                i2 = thisBlocks.IndexOf(row_sonu) + row_sonu.Length;

                if ((i1 > -1) && (i2 > i1))
                    row_block = thisBlocks.Substring(i1, i2 - i1);

                thisBlocks = thisBlocks.Remove(i1, i2 - i1);
            }

            return row_block;
        }

        public string Find_Properies_Get_FieldBlock(string thisBlocks, string MainFName)
        {
            #region
            /*
            PROP_JOINTABLE={
            0=ROW_PROP_JOINTABLE:0;           <<<< buradan 
            0=J_TABLE:J_TABLE={
            1=ROW_J_TABLE:1;
            1=CAPTION:MS_FIELDS;
            1=J_FORMAT:STANDART;
            1=J_TYPE:LEFT;
            1=J_TABLE_NAME:MS_FIELDS;
            1=J_TABLE_ALIAS:B;
            1=ROWE_J_TABLE:1;
            J_TABLE=};                        <<<< arası isteniyor
            0=J_WHERE:J_WHERE={
            1=ROW_J_WHERE:1;
            1=CAPTION:MS_FIELDS.TABLE_CODE;
            1=J_TABLE_ALIAS:B;
            1=MT_FNAME:TABLE_CODE;
            1=J_FNAME:TABLE_CODE;
            1=FVALUE:null;
            1=ROWE_J_WHERE:1;
            2=ROW_J_WHERE:2;
            2=CAPTION:MS_FIELDS.FIELD_NO;
            2=J_TABLE_ALIAS:B;
            2=MT_FNAME:FIELD_NO;
            2=J_FNAME:FIELD_NO;
            2=FVALUE:null;
            2=ROWE_J_WHERE:2;
            J_WHERE=};
            0=J_STN_FIELDS:J_STN_FIELDS={
            1=ROW_J_STN_FIELDS:1;
            1=CAPTION:FIELD NAME;
            1=J_TABLE_ALIAS:B;
            1=J_FNAME:FIELD_NAME;
            1=FNL_FNAME:LKP_FIELD_NAME;
            1=ROWE_J_STN_FIELDS:1;
            2=ROW_J_STN_FIELDS:2;
            2=CAPTION:FCAPTION;
            2=J_TABLE_ALIAS:B;
            2=J_FNAME:FCAPTION;
            2=FNL_FNAME:LKP_FCAPTION;
            2=ROWE_J_STN_FIELDS:2;
            J_STN_FIELDS=};
            0=J_CASE_FIELDS:null;
            0=ROWE_PROP_JOINTABLE:0;
            PROP_JOINTABLE=}
                         
             */
            #endregion

            int i1 = 0;
            int i2 = 0;
            string field_block = string.Empty;
            string paket_basi = MainFName + "={";
            string paket_sonu = MainFName + "=};";

            
            if (thisBlocks.IndexOf(paket_basi) > -1)
            {
                //Str_Remove(ref thisFullBlockValues, paket_basi);
                //Str_Remove(ref thisFullBlockValues, paket_sonu);
                //thisBlocks = thisBlocks.Trim();

                i1 = thisBlocks.IndexOf(paket_basi);
                i2 = thisBlocks.IndexOf(paket_sonu) + paket_sonu.Length;

                if ((i1 > -1) && (i2 > i1))
                    field_block = thisBlocks.Substring(i1, i2 - i1);

            }

            return field_block;
        }

        public string Get_Properties_Value(v.myProperties type, string fullBlocks, string FieldName, ref int Position)
        {
            string values = string.Empty;
            if (Position < 0) Position = 0;

            int i1 = 0;
            int i2 = 0;
            string paket_basi = string.Empty;
            string paket_sonu = string.Empty;

            if (type == v.myProperties.Column)
            {
                values = MyProperties_Get(fullBlocks, FieldName + ":");
                Position = -1;
                return values;
            }

            if (type == v.myProperties.Block)
            {
                paket_basi = FieldName + "={" + v.ENTER;
                paket_sonu = FieldName + "=}";
            }

            if (type == v.myProperties.Row)
            {
                paket_basi = "=ROW_" + FieldName;
                paket_sonu = "=ROWE_" + FieldName + ":";
            }

            i1 = fullBlocks.IndexOf(paket_basi, Position);
            i2 = fullBlocks.IndexOf(paket_sonu, Position);

            if ((i1 > -1) && (i2 > i1))
            {
                Position = i2 + paket_sonu.Length;
                i1 = i1 + paket_basi.Length;
                //i2 = i2 - paket_basi.Length;
                values = fullBlocks.Substring(i1, (i2 - i1));
            }
            else Position = -1;

            return values;
        }
        
        public string TableIPCodeList_Get_Values(Form tForm, string TableIPCode_RowBlock)
        {
            #region örnek
            /*
             
            1=TABLEIPCODE_LIST:TABLEIPCODE_LIST={   <<<<< field bloğu        <<< okunacak TABLEIPCODE ler
            1=ROW_TABLEIPCODE_LIST:1;               <<<<< row bloğu
            1=CAPTION:Kart aç;
            1=TABLEIPCODE:TSTDTL.TSTDTL_03;
            1=TABLEALIAS:[TSTDTL];
            1=KEYFNAME:MSTR_REF_ID;
            1=RTABLEIPCODE:TSTMSTR.TSTMSTR_01;
            1=RKEYFNAME:REF_ID;
            1=MSETVALUE:null;
            1=WORKTYPE:READ;
            1=ROWE_TABLEIPCODE_LIST:1;
            2=ROW_TABLEIPCODE_LIST:2;
            2=CAPTION:Cari Hesabı Oku;
            2=TABLEIPCODE:HP_CARI.HP_CARI_02;
            2=TABLEALIAS:[HPCARI];
            2=KEYFNAME:REF_ID;
            2=RTABLEIPCODE:TSTMSTR.TSTMSTR_01;
            2=RKEYFNAME:CARI_ID;
            2=MSETVALUE:null;
            2=WORKTYPE:READ;
            2=ROWE_TABLEIPCODE_LIST:2;
            TABLEIPCODE_LIST=};                 <<<<< 
            
            */
            #endregion örnek

            string s = TableIPCode_RowBlock;
            string RTABLEIPCODE = string.Empty;
            string RKEYFNAME = string.Empty;
            string WORKTYPE = string.Empty;
            string ReadValue = string.Empty;

            string row_block = string.Empty;
            string old_row_block = string.Empty;
            string lockE = "=ROWE_";
            string find_row = "MSETVALUE:";
                                
            int i1 = 0;

            while (s.IndexOf(lockE) > -1)
            {
                ReadValue = string.Empty;
                row_block = Find_Properies_Get_RowBlock(ref s, "TABLEIPCODE_LIST");
                old_row_block = row_block;

                RTABLEIPCODE = MyProperties_Get(row_block, "RTABLEIPCODE:");
                RKEYFNAME = MyProperties_Get(row_block, "RKEYFNAME:");
                WORKTYPE = MyProperties_Get(row_block, "WORKTYPE:");

                DataSet ds = null;
                DataNavigator tDataNavigator = null;

                if (WORKTYPE == "NEW")
                {
                    ReadValue = "0";
                }

                if (IsNotNull(RTABLEIPCODE) &&
                    IsNotNull(RKEYFNAME) &&
                    ((WORKTYPE == "READ") || 
                     (WORKTYPE == "SVIEW") || 
                     (WORKTYPE == "SVIEWVALUE") ||
                     (WORKTYPE == "CREATEVIEW"))
                    )
                {
                    ds = Find_DataSet(tForm, "", RTABLEIPCODE, "");

                    if (IsNotNull(ds))
                    {
                        tDataNavigator = Find_DataNavigator(tForm, RTABLEIPCODE);//, "");

                        if (tDataNavigator != null)
                        {
                            ReadValue = ds.Tables[0].Rows[tDataNavigator.Position][RKEYFNAME].ToString();
                        }
                    }
                }

                if (IsNotNull(ReadValue))
                {
                    i1 = row_block.IndexOf(find_row);
                    i1 = i1 + find_row.Length;

                    if (row_block.IndexOf("MSETVALUE:0") > -1)
                    {
                        row_block = row_block.Remove(i1, 1); // silinecek ifade null dur.   MSETVALUE: > null <;
                        row_block = row_block.Insert(i1, ReadValue);

                        Str_Replace(ref TableIPCode_RowBlock, old_row_block, row_block);
                    }

                    if (row_block.IndexOf("MSETVALUE:null") > -1)
                    {
                        row_block = row_block.Remove(i1, 4); // silinecek ifade null dur.   MSETVALUE: > null <;
                        row_block = row_block.Insert(i1, ReadValue);

                        Str_Replace(ref TableIPCode_RowBlock, old_row_block, row_block);
                    }
                }
            }

            return TableIPCode_RowBlock;
        }

        public void Find_Button_AddClick(Form tForm, string menuName, string buttonName,
                                         DevExpress.XtraBars.ItemClickEventHandler myClick)
        {
            string[] controls = new string[] { };
            Control c = Find_Control(tForm, menuName, "", controls);

            if (c != null)
            {
                if (c.ToString() == "DevExpress.XtraBars.Ribbon.RibbonControl")
                {
                    int i2 = ((DevExpress.XtraBars.Ribbon.RibbonControl)c).Items.Count;
                    for (int i = 0; i < i2; i++)
                    {
                        if ((((DevExpress.XtraBars.Ribbon.RibbonControl)c).Items[i].Name.ToString() == buttonName) ||
                             (buttonName == "ALLBUTTONS"))
                        {
                            ((DevExpress.XtraBars.Ribbon.RibbonControl)c).Items[i].ItemClick += null;
                            ((DevExpress.XtraBars.Ribbon.RibbonControl)c).Items[i].ItemClick += myClick;
                            
                            if (buttonName != "ALLBUTTONS") break;
                        }
                    }
                }
            }
        }

        public void Find_Button_AddClick(Form tForm, string menuName, string buttonName,
                                         DevExpress.XtraBars.Navigation.NavElementClickEventHandler myClick)
        {
            string[] controls = new string[] { };
            Control c = Find_Control(tForm, menuName, "", controls);

            if (c != null)
            {
                if (c.ToString() == "DevExpress.XtraBars.Navigation.TileNavPane")
                {
                    DevExpress.XtraBars.Navigation.TileNavPane tnPane = null;
                    tnPane = c as DevExpress.XtraBars.Navigation.TileNavPane;
                    string bname = string.Empty;
                    string iname = string.Empty;

                    int i3 = tnPane.Buttons.Count;
                    int i5 = 0;
                    for (int i2 = 0; i2 < i3; i2++)
                    {
                        bname = tnPane.Buttons[i2].Element.Name.ToString();
                        if (bname == buttonName)
                        {
                            tnPane.Buttons[i2].Element.ElementClick += null;
                            tnPane.Buttons[i2].Element.ElementClick += myClick;
                            break;
                        }

                        /// category / sub menu altındakiler
                        ///
                        i5 = 0;
                        if (tnPane.Buttons[i2].Element.ToString() == "DevExpress.XtraBars.Navigation.TileNavCategory")
                        {
                            i5 = ((DevExpress.XtraBars.Navigation.TileNavCategory)tnPane.Buttons[i2].Element).Items.Count;

                            for (int i4 = 0; i4 < i5; i4++)
                            {
                                iname = ((DevExpress.XtraBars.Navigation.TileNavCategory)tnPane.Buttons[i2].Element).Items[i4].Name.ToString();
                                if (iname == buttonName)
                                {
                                    ((DevExpress.XtraBars.Navigation.TileNavCategory)tnPane.Buttons[i2].Element).Items[i4].ElementClick += null;
                                    ((DevExpress.XtraBars.Navigation.TileNavCategory)tnPane.Buttons[i2].Element).Items[i4].ElementClick += myClick;
                                    break;
                                }
                            }
                        }
                    }
                    
                }
            }

        }


        public string Find_Properties_Value(string SelectBlockValue, string tRowFName)
        {

            #region örnek
            /*------------------------
            {
            "SV_ENABLED": "TRUE",
            "SV_KEYFNAME": "ID",
            "SV_CAPTION_FNAME": "BAŞLIK",
            "SV_CMP_TYPE": "TEXT",
            "SV_CMP_LOCATION": "0,0",
            "SV_VIEW_TYPE": "TabPage",
            "SV_LIST": [
            {
            "CAPTION": "Anull",
            "SV_VALUE": "null",
            "TABLEIPCODE": "null"
            },
            {
            "CAPTION": "Bnull",
            "SV_VALUE": "null",
            "TABLEIPCODE": "null"
            },
            {
            "CAPTION": "Cnull",
            "SV_VALUE": "null",
            "TABLEIPCODE": "null"
            },
            {
            "CAPTION": "Dnull",
            "SV_VALUE": "null",
            "TABLEIPCODE": "null"
            }
            ]
            }
            ----------------------*/
            #endregion örnek
            
            

            string tValue = string.Empty;
            string s = SelectBlockValue;
            int j1 = -1;
            int j2 = -1;


            if (tRowFName == "TLP")
            {
                // iki properties içinde TLP mevcut
                // bu nedenle hangisi geldi onu bulmak gerekiyor
                int i = SelectBlockValue.IndexOf("DOCKPANEL");
                if (i >= 0)
                {
                    PROP_VIEWS_LAYOUT packet = new PROP_VIEWS_LAYOUT();
                    SelectBlockValue = SelectBlockValue.Replace((char)34, (char)39);
                    var prop_ = JsonConvert.DeserializeAnonymousType(SelectBlockValue, packet);
                    tValue = JsonConvert.SerializeObject(prop_.TLP, Newtonsoft.Json.Formatting.Indented);
                    return tValue;
                }
                if (i == -1)
                {
                    PROP_VIEWS_GROUPS packet = new PROP_VIEWS_GROUPS();
                    SelectBlockValue = SelectBlockValue.Replace((char)34, (char)39);
                    var prop_ = JsonConvert.DeserializeAnonymousType(SelectBlockValue, packet);
                    tValue = JsonConvert.SerializeObject(prop_.TLP, Newtonsoft.Json.Formatting.Indented);
                    return tValue;
                }
            }


            /// "SV_LIST": [
            string paket_basi_A = (char)34 + tRowFName + (char)34 + ": [";
            string paket_sonu_A = "]";

            /// "SV_LIST": {
            string paket_basi_B = (char)34 + tRowFName + (char)34 + ": {";
            string paket_basi_B2 = (char)34 + tRowFName + (char)34 + ": ";
            string paket_sonu_B = "}";

            /// başka bir blok, paket varsa
            /// "SV_LIST": [
            j1 = s.IndexOf(paket_basi_A);
            if (j1 > -1)
            {
                j2 = s.IndexOf(paket_sonu_A, j1) + paket_sonu_A.Length;
                if (j2 > j1)
                {
                    tValue = s.Substring(j1, j2 - j1);
                    tValue = tValue.Trim();
                }
            }

            /// başka bir blok, paket varsa
            /// "SV_LIST": {
            if (j1 == -1)
            {
                j1 = s.IndexOf(paket_basi_B);
                if (j1 > -1)
                {
                    j1 = j1 + paket_basi_B2.Length;
                    j2 = s.IndexOf(paket_sonu_B, j1) + paket_sonu_B.Length;
                    tValue = s.Substring(j1, j2 - j1);
                    tValue = tValue.Trim();
                }
            }

            if (j1 == -1)
            {
                /// sadece fname ve value değeri var ise
                /// "SV_ENABLED": "TRUE"
                tValue = MyProperties_Get(s, (char)34 + tRowFName + (char)34 + ": " + (char)34); //"
                //tValue = t.MyProperties_Get(s, (char)39 + tRowFName + (char)39 + ": " + (char)39); //'
                tValue = tValue.Trim();
                if (tValue.IndexOf("null") == 0)
                    tValue = "";
            }

            return tValue;
        }
        /*
{
  "ALLPROP": {
    "GROUPFNAME1": "null",
    "NAVBUTTONCAPTION": "null",
    "GROUPFNAME2": "null",
    "GROUPFNAME3": "null"
  },
  "GRID": {
    "ALLOWCELLMERGE": "null",
    "INVERTSELECTION": "null",
    ...
    "SVERTICALLINES": "null",
    "SVIEWCAPTION": "null"
  },
  "GRIDADB": "null",
  "GRIDLYT": "null",
  "WINEXP": "null",
  "VGRID": "null",
  "TREE": "null",
  "PIVOT": "null",
  "DATALYT": "null",
  "TILE": {
    "TL_ITEMWIDTH": "null",
    "TL_ITEMHEIGHT": "null",
    "TL_ORIENTATION": "null"
  }
}
        */
        #endregion Diğerleri <<

        #endregion Find Functions

        #region *Tarih Fonksiyonları

        #region Tarih_Formati

        public string Tarih_Formati(DateTime Tarih)
        {
            string s = "";
            string t = "";
            if (Tarih.Year > 0)
            {
                t = Tarih.Date.ToString().Substring(0,10);
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

        #region *mySoru

        public DialogResult mySoru(string Soru)
        {
            string Question = Soru;

            if ((Soru.ToUpper() == "VAZGEÇ") ||
                (Soru.ToUpper() == "SİL") ||
                (Soru.ToUpper() == "SIL"))
                Question = "İşlemden vazgeçmek ister misiniz?";

            if ((Soru.ToUpper() == "DEVAM") ||
                (Soru.ToUpper() == "ONAY")) 
                Question = "İşleme devam etmek istediğinize emin misiniz?";
                        
            DialogResult answer = MessageBox.Show(Question, "Onay İşlemi", MessageBoxButtons.YesNoCancel);
            
            switch (answer)
            {
                case DialogResult.Yes:
                    {
                        break; // break ifadesini sakın silme
                    }
                case DialogResult.No:
                    {
                        break; // break ifadesini sakın silme
                    }
                case DialogResult.Cancel:
                    {
                        break; // break ifadesini sakın silme
                    }
            }

            return answer; 
        }


        #endregion mySoru

        #region *Grid Fonksiyonları

        public void GridGroupRefresh(GridControl grid)
        {
            if (grid != null)
            {
                if (grid.MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                {
                    string GROUPFNAME1 = string.Empty;
                    string GROUPFNAME2 = string.Empty;
                    string GROUPFNAME3 = string.Empty;

                    int i1 = ((GridView)grid.MainView).GroupedColumns.Count;
                    if (i1 > 0) GROUPFNAME1 = ((GridView)grid.MainView).GroupedColumns.View.GroupedColumns[0].FieldName;
                    if (i1 > 1) GROUPFNAME2 = ((GridView)grid.MainView).GroupedColumns.View.GroupedColumns[1].FieldName;
                    if (i1 > 2) GROUPFNAME3 = ((GridView)grid.MainView).GroupedColumns.View.GroupedColumns[2].FieldName;

                    if (IsNotNull(GROUPFNAME3))
                        ((GridView)grid.MainView).Columns[GROUPFNAME3].GroupIndex = -1;
                    if (IsNotNull(GROUPFNAME2))
                        ((GridView)grid.MainView).Columns[GROUPFNAME2].GroupIndex = -1;
                    if (IsNotNull(GROUPFNAME1))
                        ((GridView)grid.MainView).Columns[GROUPFNAME1].GroupIndex = -1;

                    if (IsNotNull(GROUPFNAME1))
                        ((GridView)grid.MainView).Columns[GROUPFNAME1].GroupIndex = 0;
                    if (IsNotNull(GROUPFNAME2))
                        ((GridView)grid.MainView).Columns[GROUPFNAME2].GroupIndex = 1;
                    if (IsNotNull(GROUPFNAME3))
                        ((GridView)grid.MainView).Columns[GROUPFNAME3].GroupIndex = 2;



                }
            }
        }

        public void Gridi_Grupla(GridControl grid,
                                 string Group0_Name, string Group1_Name, string Group2_Name,
                                 ref string Group0_Old, ref string Group1_Old, ref string Group2_Old)
        {
            if (grid != null)
            {
                if (grid.MainView.ToString() == "DevExpress.XtraGrid.Views.Grid.GridView")
                {
                    //GridView tGridView = grid.MainView as GridView;
                    //GridView tGridView = (DevExpress.XtraGrid.Views.Grid.GridView)grid.MainView;
                    //GridView tGridView = new GridView(grid);

                    if (IsNotNull(Group0_Old))
                        ((GridView)grid.MainView).Columns[Group0_Old].GroupIndex = -1;
                    if (IsNotNull(Group1_Old))
                        ((GridView)grid.MainView).Columns[Group1_Old].GroupIndex = -1;
                    if (IsNotNull(Group2_Old))
                        ((GridView)grid.MainView).Columns[Group2_Old].GroupIndex = -1;

                    if (IsNotNull(Group0_Name))
                        ((GridView)grid.MainView).Columns[Group0_Name].GroupIndex = 0;
                    if (IsNotNull(Group1_Name))
                        ((GridView)grid.MainView).Columns[Group1_Name].GroupIndex = 1;
                    if (IsNotNull(Group2_Name))
                        ((GridView)grid.MainView).Columns[Group2_Name].GroupIndex = 2;

                    Group0_Old = Group0_Name;
                    Group1_Old = Group1_Name;
                    Group2_Old = Group2_Name;

                    ((GridView)grid.MainView).ExpandAllGroups();
                }
            }
        }
     
        #endregion Grid Fonksiyonları

        #region *InputBox
        //public DialogResult InputBox(string title, string promptText, ref string value, int ftype, string displayFormat)
        public DialogResult UserInpuBox(vUserInputBox vUIBox)
        {
            Form form = new DevExpress.XtraEditors.XtraForm();//new Form();
            Label label = new Label();
            //TextBox textBox = new TextBox();
            DevExpress.XtraEditors.TextEdit textBox = new DevExpress.XtraEditors.TextEdit();
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
                textBox.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            }

            //* rakam  56, 48, 127, 52, 60, 62, 59, 106, 108
            if ((vUIBox.fieldType == 56) | (vUIBox.fieldType == 48) | 
                (vUIBox.fieldType == 59) | (vUIBox.fieldType == 52) |
                (vUIBox.fieldType == 60) | (vUIBox.fieldType == 62) | 
                (vUIBox.fieldType == 127) | (vUIBox.fieldType == 106) | 
                (vUIBox.fieldType == 108))
            {
                textBox.RightToLeft = RightToLeft.Yes;
                textBox.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;

                if (vUIBox.value == "") textBox.Text = "0";
            }
            
            if ((vUIBox.displayFormat != "") ||
                (vUIBox.displayFormat != null))
            {
                textBox.Properties.Mask.EditMask = vUIBox.displayFormat;
                textBox.Properties.Mask.SaveLiteral = true;
                textBox.Properties.Mask.ShowPlaceHolders = true;
                textBox.Properties.Mask.UseMaskAsDisplayFormat = true;
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

        #region Report.Select/TableIPCODE işlemleri

        public void Preparing_Select_ReportData_DataSet(Form tForm,
                    ref DataSet dsData,
                    byte DBaseNo, string TableIPCode, string Kriterler)
        {
            if (dsData.Namespace.ToString() == "") 
            {
                DataSet ds_Table = new DataSet();
                DataSet ds_Fields = new DataSet();

                tTablesRead tr = new tTablesRead();

                tr.MS_Tables_IP_Read(ds_Table, TableIPCode);
                tr.MS_Fields_IP_Read(ds_Fields, TableIPCode);

                if ((IsNotNull(ds_Table) == false) ||
                    (IsNotNull(ds_Fields) == false)) return;

                DataRow row_Table = ds_Table.Tables[0].Rows[0] as DataRow;

                vTableAbout vTA = new vTableAbout();
                Table_About(vTA, ds_Fields);

                tSQLs sql = new tSQLs();
                sql.Preparing_dsData(tForm, row_Table, ds_Fields, ref dsData, "", vTA);
            }

            string myProp = dsData.Namespace.ToString();
            string tTableName = MyProperties_Get(myProp, "TableName:");
            string tTableCaption = MyProperties_Get(myProp, "TableCaption:");
            
            string SqlF = MyProperties_Get(myProp, "SqlFirst:");
            //string SqlS = MyProperties_Get(myProp, "SqlSecond:");

            string block = string.Empty;
            string and_block = string.Empty;
            string alias = string.Empty;

            if (Kriterler != null)
            {
                while (Kriterler.IndexOf("=ROWE_") > -1)
                {
                    block = Get_And_Clear(ref Kriterler, "=ROWE_");
                    alias = MyProperties_Get(block, "ALIAS:");
                    and_block = MyProperties_Get(block, "_PARAMS:");

                    if (and_block.Length > 0)
                    {
                        tKriter_Ekle(ref SqlF, alias, and_block);
                    }
                }
            }
            /*
            =ROW_[VRSNL_10]:null;
            =[VRSNL_10]_PARAMS: and [VRSNL_10].PART_NAME like  '%aaa%' 
             and [VRSNL_10].BAS_TARIH =   CONVERT(SMALLDATETIME, '01.01.2015 00:00:00', 101)  
             and [VRSNL_10].GIRIS_YOK = 2
             and [VRSNL_10].GIRIS_YAPTI = 3
            ;
            =ROWE_[VRSNL_10]:null;
            */

            if (Kriterler != "")
            {
                try
                {
                    SqlF = MySQL_Clear(SqlF);
                    Data_Read_Execute(dsData, ref SqlF, tTableName, null);
                    dsData.Tables[tTableName].Namespace = tTableCaption;
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    throw;
                }
            }

        }

        public void Preparing_Select_Param_DataSet(Form tForm,
                    ref DataSet dsKrtr,
                    byte DBaseNo, string TableIPCode)
        {
            tSQLs sql = new tSQLs();
                        
            string sqlKrtr_Kist = string.Empty;
            string sqlKrtr_Full = string.Empty;
            string tTOP_Count = string.Empty;

            DataSet ds_Table = new DataSet();
            
            tTablesRead tr = new tTablesRead();

            tr.MS_Tables_IP_Read(ds_Table, TableIPCode); //TableIPCode

            string tTableCaption = ds_Table.Tables[0].Rows[0]["IP_CAPTION"].ToString();
            string tTableCode = ds_Table.Tables[0].Rows[0]["TABLE_CODE"].ToString();
            string tTableName = ds_Table.Tables[0].Rows[0]["LKP_TABLE_NAME"].ToString();
            //string tKey_FName = ds_Table.Tables[0].Rows[0]["KEY_FNAME"].ToString();

            // tTableType = 6  Select / TableIPCode  ise
            sql.SQL_MSFieldsIPKrtr(TableIPCode, ref sqlKrtr_Kist, ref sqlKrtr_Full);

            try
            {
                Data_Read_Execute(dsKrtr, ref sqlKrtr_Kist, tTableName + "_KIST", null);
                Data_Read_Execute(dsKrtr, ref sqlKrtr_Full, tTableName + "_FULL", null);
                dsKrtr.Tables[tTableName + "_KIST"].Namespace = tTableCaption;
                dsKrtr.Tables[tTableName + "_FULL"].Namespace = tTableCaption;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }
                    

        #endregion Report.Select/TableIPCODE işlemleri

        #region Report.Table işlemleri

        public void Preparing_Param_DataSet(Form tForm,
                    ref DataSet dsKrtr,
                    byte DBaseNo, 
                    string TableName, 
                    string KonuOlanTableCode,
                    string KonuOlanFieldName,  
                    string KonuOlanValue,
                    string KonuOlanaAit_Liste_TableIPCode)
        {

            string myProp = string.Empty;
            string DBaseName = Find_dBLongName(DBaseNo.ToString());
            
            MyProperties_Set(ref myProp, "DBaseNo", DBaseNo.ToString());
            MyProperties_Set(ref myProp, "TableName", TableName);
            MyProperties_Set(ref myProp, "Cargo", "param"); // cargo = data, param, report

            DataSet dsData = null;
            
            // Form tForm işi şimdilik askıya alındı, form bir defa oluşsun bir tekrar create etmeye
            // kalmasın diye düşünüldü bir an için ama ????

            //dsKrtr = Find_dsParam(tForm, TableName);_

            dsKrtr = new DataSet();
            dsKrtr.Namespace = myProp;

            ReportOrParam_Preparing_DataSet(tForm, 
                ref dsData, ref dsKrtr,
                DBaseName, TableName,
                KonuOlanTableCode, KonuOlanFieldName, KonuOlanValue, 
                KonuOlanaAit_Liste_TableIPCode, "", true);

        }

        public void Preparing_ReportData(Form tForm, 
                    ref DataSet dsData, 
                    byte DBaseNo, 
                    string TableName,
                    string KonuOlanTableCode,
                    string KonuOlanFieldName,
                    string KonuOlanValue,
                    string KonuOlanaAit_Liste_TableIPCode,
                    string UserParam)
        {
            
            string myProp = string.Empty;
            string DBaseName = Find_dBLongName(DBaseNo.ToString());
            
            MyProperties_Set(ref myProp, "DBaseNo", DBaseNo.ToString());
            MyProperties_Set(ref myProp, "TableName", TableName);
            MyProperties_Set(ref myProp, "Cargo", "report"); // cargo = data, param, report

            
            dsData = new DataSet();
            DataSet dsKrtr = null;

            dsData.Namespace = myProp;

            ReportOrParam_Preparing_DataSet(tForm, 
                ref dsData, ref dsKrtr, 
                DBaseName, TableName,
                KonuOlanTableCode, KonuOlanFieldName, KonuOlanValue, 
                KonuOlanaAit_Liste_TableIPCode, UserParam, false);

            // Firma Tablosu Okunuyor
            ReportOrParam_Preparing_DataSet(tForm, 
                ref dsData, ref dsKrtr,
                DBaseName, v.SP_FIRM_TABLENAME,
                KonuOlanTableCode, KonuOlanFieldName, v.SP_FIRM_ID.ToString(),
                KonuOlanaAit_Liste_TableIPCode, UserParam, false);


        }


        public void ReportOrParam_Preparing_DataSet(Form tForm, 
                            ref DataSet dsData, ref DataSet dsKrtr,
                            string DBName, string TableName,
                            string KonuOlanTableCode,
                            string KonuOlanFieldName,
                            string KonuOlanValue,
                            string KonuOlanaAit_Liste_TableIPCode,
                            string UserParam, Boolean tParam)
        {
            tSQLs sql = new tSQLs();

            // Burada MS_TABLES e bakarak Master_Table_Name sayesinde birbirine bağlı olan tablolar tespit ediliyor
            // burada hem dsData hemde dsKrtr hazırlanıyor 
            // fakat farklı zamanlarda

            string tBasamak = string.Empty;
            string tTableCaption = string.Empty;
            string tTableCode = string.Empty;
            string uTableCode = string.Empty;
            string tTableName = string.Empty;
            string tKey_FName = string.Empty;
            string tMaster_TableName = string.Empty;
            string tMaster_Key_FName = string.Empty;
            string tForeing_FName = string.Empty;
            string and_in = string.Empty;
            string and_select = string.Empty;
            string in__select = string.Empty;
            string and_full = string.Empty;

            string tFrom = string.Empty;
            string tWhereKeys = string.Empty;
            string KOA_Liste_KonuOlanValue = string.Empty;

            int TableNo = 0;

            // Value
            string s = sql.RSQL_MasterTable(DBName, TableName);

            DataSet dsMSTables = new DataSet();

            try
            {
                Data_Read_Execute(dsMSTables, ref s, TableName, null);
            }
            catch (Exception)
            {
                //
                throw;
            }

            if (IsNotNull(dsMSTables))
            {

                if (TableName != v.SP_FIRM_TABLENAME)
                {
                    TableRemove(dsData);
                }

                // master ise
                // grp      // and [GRP].ID in ( 
                // grp      //    select [GRP].ID from GRUP [GRP] where 0 = 0   
                // detail ise
                // krsyr >> // and [KRSYR].GRUP_ID in ( 
                // krsyr    //    select [KRSYR].GRUP_ID from KURSIYER [KRSYR] where 0 = 0

                
                #region // yani Rapor Datası okunacaksa extra alttaki bu işlemler yapılacak
                if (tParam == false)
                {
                    //and_full =

                    MyProperties_Set(ref and_full, "and_full", "BEGIN");
                                        
                    foreach (DataRow MSTable_Row in dsMSTables.Tables[0].Rows)
                    {
                        //basamak ile tablanun kaçıncı union katmanından geldiğini tespit ediyoruz
 
                        tBasamak = MSTable_Row["BASAMAK"].ToString();
                        tTableCaption = MSTable_Row["TB_CAPTION"].ToString();
                        tTableCode = MSTable_Row["TABLE_CODE"].ToString();
                        tTableName = MSTable_Row["TABLE_NAME"].ToString();
                        tKey_FName = MSTable_Row["KEY_FNAME"].ToString();
                        tMaster_TableName = MSTable_Row["MASTER_TABLE_NAME"].ToString();
                        tMaster_Key_FName = MSTable_Row["MASTER_KEY_FNAME"].ToString();
                        tForeing_FName = MSTable_Row["FOREING_FNAME"].ToString();
                        
                        if (TableNo == 0)
                        {
                            and_in = " and [" + tTableCode + "]." + tKey_FName + " in ( ";
                            and_select = " select [" + tTableCode + "]." + tKey_FName + " from " + tTableName + " [" + tTableCode + "] where 0 = 0 ";
                            in__select = "";
                            //if (IsNotNull(KonuOlanValue))
                            //    tWhereKeys = " [" + uTableCode + "]." + tKey_FName + " = " + KonuOlanValue + " ";
                        }
                        else
                        {
                            and_in = " and [" + tTableCode + "]." + tForeing_FName + " in ( ";
                            and_select = " select [" + tTableCode + "]." + tForeing_FName + " from " + tTableName + " [" + tTableCode + "] where 0 = 0 ";
                            //in__select = " select [" + tTableCode + "]." + tKey_FName + " from " + tTableName + " [" + tTableCode + "] where [" + tTableCode + "]." + tForeing_FName + " = ";
                            in__select = Preparing_in_select(dsMSTables, tMaster_TableName);
                            //=KRSYR_and_in: and [KRSYR].GRUP_ID in ( ;
                            //=KRSYR_and_select: select [KRSYR].GRUP_ID from KURSIYER [KRSYR] where 0 = 0 ;
                            //=KRSYR_in__select: Select [KRSYR].ID      from KURSIYER [KRSYR] where [KRSYR].GRUP_ID  = ;

                        }

                        MyProperties_Set(ref and_full, tTableCode + "_basamak", tBasamak);
                        MyProperties_Set(ref and_full, tTableCode + "_and_in", and_in);
                        MyProperties_Set(ref and_full, tTableCode + "_and_select", and_select);
                        MyProperties_Set(ref and_full, tTableCode + "_in__select", in__select);
            

                        TableNo++;
                    }

                    MyProperties_Set(ref and_full, "and_full", "END");


                    if (IsNotNull(UserParam))
                        UserParam = and_full + UserParam;
                    else UserParam = and_full;

                }
                #endregion 
                
                // --------------------------------------------------------------
                // bu döngü hem params lar, hemde Okunacak Data için gerekli
                // eğer dsData null değilse dsKrtr null dur
                // eğer dsKrtr null değilse dsData null dur
                // ikisi aynı anda gelmiyor

                // listedeki datanın ıdsi
                if (IsNotNull(KonuOlanaAit_Liste_TableIPCode) &&
                     IsNotNull(KOA_Liste_KonuOlanValue) == false)
                {
                    DataSet dsKonuOlanList = null;
                    DataNavigator tDataNavigator_KonuOlanList = null;

                    dsKonuOlanList = Find_DataSet(tForm, "", KonuOlanaAit_Liste_TableIPCode, "");
                    tDataNavigator_KonuOlanList = Find_DataNavigator(tForm, KonuOlanaAit_Liste_TableIPCode);//, "");

                    if (tDataNavigator_KonuOlanList != null)
                    {
                        if (tDataNavigator_KonuOlanList.Position > -1)
                        {
                            string myProp = dsKonuOlanList.Namespace.ToString();
                            string read_tablename = MyProperties_Get(myProp, "=TableName:");
                            string read_keyfname = MyProperties_Get(myProp, "=KeyFName:");

                            KOA_Liste_KonuOlanValue = dsKonuOlanList.Tables[read_tablename].Rows[tDataNavigator_KonuOlanList.Position][read_keyfname].ToString();
                        }
                        else
                        {
                            KOA_Liste_KonuOlanValue = "-1";
                        }
                    }

                }
                

                TableNo = 0;
                foreach (DataRow MSTable_Row in dsMSTables.Tables[0].Rows)
                {
                    //--------------
                    tTableCaption = MSTable_Row["TB_CAPTION"].ToString();
                    tTableCode = MSTable_Row["TABLE_CODE"].ToString();
                    tTableName = MSTable_Row["TABLE_NAME"].ToString();
                    tKey_FName = MSTable_Row["KEY_FNAME"].ToString();
                    tMaster_TableName = MSTable_Row["MASTER_TABLE_NAME"].ToString();
                    tMaster_Key_FName = MSTable_Row["MASTER_KEY_FNAME"].ToString();
                    tForeing_FName = MSTable_Row["FOREING_FNAME"].ToString();

                    tFrom = "[" + tTableName + "] [" + tTableCode + "]";
                    tWhereKeys = " 0 = 0 " + v.ENTER;
                    //if (IsNotNull(KonuOlanValue))
                    //    tWhereKeys = " [" + TableCode + "]." + tKey_FName + " = " + KonuOlanValue + " ";
                    //--------
                    
                    if (IsNotNull(KOA_Liste_KonuOlanValue) == false)
                    {
                        RTable_Read(tForm, ref dsData, ref dsKrtr, MSTable_Row, DBName,
                                KonuOlanTableCode, KonuOlanFieldName, KonuOlanValue,
                                TableNo, tParam, UserParam);
                    }
                    else
                    {
                        RTable_Read(tForm, ref dsData, ref dsKrtr, MSTable_Row, DBName,
                                KonuOlanTableCode, KonuOlanFieldName, KOA_Liste_KonuOlanValue,
                                TableNo, tParam, UserParam);
                    }

                    TableNo++;
                }

            }

            dsMSTables.Dispose();
        }

        private string Preparing_in_select(DataSet dsMSTables, string MasterTablename)
        {
            string tTableCode = string.Empty;
            string tTableName = string.Empty;
            string tKey_FName = string.Empty;
            string tForeing_FName = string.Empty;
            string in_select = string.Empty;

            int i2 = dsMSTables.Tables[0].Rows.Count;
            for (int i = 0; i < i2; i++)
            {
                tTableName = dsMSTables.Tables[0].Rows[i]["TABLE_NAME"].ToString();

                if (tTableName == MasterTablename)
                {
                    tTableCode = dsMSTables.Tables[0].Rows[i]["TABLE_CODE"].ToString();
                    tKey_FName = dsMSTables.Tables[0].Rows[i]["KEY_FNAME"].ToString();
                    tForeing_FName = dsMSTables.Tables[0].Rows[i]["FOREING_FNAME"].ToString();
                    
                    in_select = " select [" + tTableCode + "]." + tKey_FName + " from " + tTableName + " [" + tTableCode + "] where [" + tTableCode + "]." + tForeing_FName + " = ";
                    break;
                }          
            }

            return in_select;
        }

        public string Read_REPORT_TEMP(int ID)
        {
            DataSet ds = new DataSet();

            string Sql =
               " Select REPORT_TEMP from MS_REPORTS where REF_ID = " + ID.ToString();

            SQL_Read_Execute(v.dBaseNo.Manager, ds, ref Sql, "", "Read_REPORT_TEMP");

            string temp = string.Empty;

            if (IsNotNull(ds))
            {
                temp = ds.Tables[0].Rows[0]["REPORT_TEMP"].ToString();
            }

            ds.Dispose();

            return temp;
        }

        public bool Save_REPORT_TEMP(int ID, string temp)
        {
            tSave sv = new tSave();
            DataSet dsData = new DataSet();
            
            // REPORT_TEMP save
            string TableName = "MS_REPORTS";
            string Sql =
               " Select * from MS_REPORTS where REF_ID = " + ID.ToString();

            // önce update olacak veriyi oku
            SQL_Read_Execute(v.dBaseNo.Manager, dsData, ref Sql, TableName, "Save_REPORT_TEMP");
            // tabloya ait field listesini oku

            // bu iptal oldu 
            //Load_Field_List(v.active_DB.managerDBName, TableName, dsData, "", "T15_MSRPR.T15_MSRPR_04");


            // sorun yok ise
            if (IsNotNull(dsData))
            {
                // atamayı yap
                dsData.Tables[0].Rows[0]["REPORT_TEMP"] = temp;

                MessageBox.Show("Burasıda MySQL den dolayı düzenleme istiyor  : Save_REPORT_TEMP() ");

                /*
                // kaydet
                v.Kullaniciya_Mesaj_Var =
                        sv.MyRecord(dsData, TableName, 0,
                                    (byte)v.Save.KAYDET, v.SP_Conn_Manager_MSSQL);
                */
            }
            
            dsData.Dispose();

            return true;
        }
        
        public void RTable_Read(Form tForm, ref DataSet dsData, ref DataSet dsKrtr, DataRow MSTable_Row,
                                string DBName, 
                                string KonuOlanTableCode, 
                                string KonuOlanFieldName, 
                                string KonuOlanValue, 
                                int TableNo, 
                                Boolean tParam, string UserParam) 
        {
            tSQLs sql = new tSQLs();

            DataSet dsMSFields = new DataSet();

            string function_name = "RTable Read";
            string tTableCaption = string.Empty;
            string tTableCode = string.Empty;
            string tTableName = string.Empty;
            string tKey_FName = string.Empty;
            string tMaster_TableName = string.Empty;
            string tMaster_Key_FName = string.Empty;
            string tForeing_FName = string.Empty;
            
            string fieldList = string.Empty;
            string foreing_where = string.Empty;
            string where_key_fname = string.Empty;

            string sqlKrtr_Kist = string.Empty;
            string sqlKrtr_Full = string.Empty;
            string tTOP_Count = string.Empty;
            
            tTableCaption = MSTable_Row["TB_CAPTION"].ToString();
            tTableCode = MSTable_Row["TABLE_CODE"].ToString();
            tTableName = MSTable_Row["TABLE_NAME"].ToString();
            tKey_FName = MSTable_Row["KEY_FNAME"].ToString();
            tMaster_TableName = MSTable_Row["MASTER_TABLE_NAME"].ToString();
            tMaster_Key_FName = MSTable_Row["MASTER_KEY_FNAME"].ToString();
            tForeing_FName = MSTable_Row["FOREING_FNAME"].ToString();
            foreing_where = "";// MSTable_Row["FOREING_WHERE"].ToString();  tabloya eklemeyi UNUTMA

            if (TableNo == 0) where_key_fname = tKey_FName;
            
            if ((TableNo > 0) && (IsNotNull(tForeing_FName)))
                where_key_fname = tForeing_FName;

            #region Read Param/Krtr

            if (tParam) 
            {
                // tTableType = 1 Table ise
                sql.SQL_MSFieldsKrtr(tTableCode, ref sqlKrtr_Kist, ref sqlKrtr_Full);

                try
                {
                    Data_Read_Execute(dsKrtr, ref sqlKrtr_Kist, tTableName + "_KIST", null);
                    Data_Read_Execute(dsKrtr, ref sqlKrtr_Full, tTableName + "_FULL", null);
                    dsKrtr.Tables[tTableName + "_KIST"].Namespace = tTableCaption;
                    dsKrtr.Tables[tTableName + "_FULL"].Namespace = tTableCaption;

                }
                catch (Exception)
                {
                    //
                    throw;
                }
            }

            #endregion Read Krtr

            #region Read Data
            
            if (tParam == false)
            {
                /// dikkat mysql yüzünden bu yapı çalışmaz 
                /// MYSQL UNUTMA
                /// 
                fieldList = sql.SQL_MSTable_FieldsList(DBName, tTableName);

                try
                {
                    Data_Read_Execute(dsMSFields, ref fieldList, tTableName, null);
                }
                catch (Exception)
                {
                    //
                    throw;
                }

                #region Fields and ReadData
                if (IsNotNull(dsMSFields))
                {
                    string Report_FieldsName = string.Empty;
                    string fieldname = string.Empty;
                    string fcaption = string.Empty;
                    string new_Param = string.Empty;
                    string fforeing = string.Empty;
                    string tl_types_name = string.Empty;
                    string tl_value_type = string.Empty;
                    string tl_where_fname1 = string.Empty;
                    string tl_where_fname2 = string.Empty;
                    string tl_table_name = string.Empty;
                    string tl_caption_fname = string.Empty;

                    string sn = string.Empty;
                    string leftjoin = string.Empty;

                    int i = 1;

                    //d.LIST_TYPES_NAME, tl.CAPTION_FNAME, tl.VALUE_TYPE, tl.WHERE_FNAME1, tl.WHERE_FNAME2, tl.TABLE_NAME 
                    
                    foreach (DataRow Row in dsMSFields.Tables[0].Rows)
                    {
                        sn = i.ToString(); 
                        fieldname = Row["name"].ToString();
                        fcaption = Set(Row["FCAPTION"].ToString(), fieldname, fieldname);
                        tl_types_name = Set(Row["LIST_TYPES_NAME"].ToString(), "", "");
                        fforeing = Row["FFOREING"].ToString();

                        if (fforeing == "True")
                            Report_FieldsName = Report_FieldsName + " , [" + tTableCode + "]." + fieldname + "  [" + fieldname + "] " + v.ENTER;

                        if (Report_FieldsName.Length == 0)
                        {
                            Report_FieldsName = Report_FieldsName + "   [" + tTableCode + "]." + fieldname + "  [" + fcaption + "] " + v.ENTER;
                        }
                        else
                        {
                            if (IsNotNull(tl_types_name) == false)
                            {
                                Report_FieldsName = Report_FieldsName + " , [" + tTableCode + "]." + fieldname + "  [" + fcaption + "] " + v.ENTER;
                            }
                            else
                            {

                                tl_caption_fname = Set(Row["CAPTION_FNAME"].ToString(), "", "");
                                tl_where_fname1 = Set(Row["WHERE_FNAME1"].ToString(), "", "");
                                tl_where_fname2 = Set(Row["WHERE_FNAME2"].ToString(), "", "");
                                tl_table_name = Set(Row["TABLE_NAME"].ToString(), "", "");

                                if ((IsNotNull(tl_caption_fname)) &&
                                    (IsNotNull(tl_where_fname1)) &&
                                    (IsNotNull(tl_table_name)))
                                {
                                    
                                    Report_FieldsName = Report_FieldsName + " , [tL" + sn + "]." + tl_caption_fname + " [" + fcaption + "] " + v.ENTER;

                                    leftjoin = leftjoin + " left outer join dbo.[" + tl_table_name + "] [tL" + sn + "] on ( [" + tTableCode + "]." + fieldname + " = "
                                      + "[tL" + sn + "]." + tl_where_fname1 + " ";

                                    if (IsNotNull(tl_where_fname2))
                                        leftjoin = leftjoin +
                                            " and " + " [tL" + sn + "]." + tl_where_fname2 + " = '" + tl_types_name + "'";

                                    leftjoin = leftjoin + " ) " + v.ENTER;
                                }
                                else
                                {
                                    Report_FieldsName = Report_FieldsName + " , [" + tTableCode + "]." + fieldname + "  [" + fcaption + "] " + v.ENTER;
                                }

                            }
                        }
                        i++;
                    }
                    
                    tTOP_Count = "";
                    //if ((IsNotNull(UserParam) == false) && (IsNotNull(KonuOlanValue) == false))
                    if ( (UserParam.IndexOf("_PARAMS:") < 0) && 
                         (KonuOlanValue == "")
                       )
                            tTOP_Count = " TOP 20 ";
                    
                    new_Param = Preparing_UserParam(UserParam, tTableCode, KonuOlanTableCode, KonuOlanFieldName, KonuOlanValue);
                    
                    
                    string table_sql =
                          " Select " + tTOP_Count + v.ENTER 
                        + Report_FieldsName
                        + " from [" + tTableName + "] [" + tTableCode + "]" + v.ENTER
                        + leftjoin
                        + " where 0 = 0 " + v.ENTER
                        + " /*KRITERLER*/ " + v.ENTER
                        + new_Param;
                    
                    try
                    {
                        Data_Read_Execute(dsData, ref table_sql, tTableName, null);

                        if ((TableNo > 0) &&
                            (IsNotNull(tMaster_TableName)) &&
                            (IsNotNull(tMaster_Key_FName)) &&
                            (IsNotNull(tForeing_FName)))
                        {
                            // Master-Detail Bağlantısı
                            // Set up a master-detail relationship between the DataTables
                            DataColumn keyColumn = dsData.Tables[tMaster_TableName].Columns[tMaster_Key_FName];
                            DataColumn foreignKeyColumn = dsData.Tables[tTableName].Columns[tForeing_FName];

                            if ((keyColumn != null) && (foreignKeyColumn != null))
                            {
                                dsData.Relations.Add(tMaster_TableName + "_" + tTableName, keyColumn, foreignKeyColumn, false);
                            }
                            else
                            {
                                string s = 
                                " DİKKAT : Raporlar için tablolar arası Master-Detail bağlantısı kurulamıyor. " + v.ENTER +
                                " Master Table : " + tMaster_TableName + v.ENTER +
                                " Master keyFieldName : " + tMaster_Key_FName + v.ENTER +
                                " Detail Table : " + tTableName + v.ENTER +
                                " Foreing FieldName : " + tForeing_FName + v.ENTER2;

                                if ((keyColumn == null) && (foreignKeyColumn != null))
                                    s = s + " Master tablosunun keyFieldName tespit edilemiyor..." + v.ENTER;
                                if ((keyColumn != null) && (foreignKeyColumn == null))
                                    s = s + " Detail tablosunun foringkeyFieldName tespit edilemiyor..." + v.ENTER;
                                if ((keyColumn == null) && (foreignKeyColumn == null))
                                    s = s + " Master tablosunun keyFieldName ve Detail tablosunun foringkeyFieldName tespit edilemiyor..." + v.ENTER2;

                                s = s +
                                    " 1. Database üzerinde aşağıdaki tanımla olmayabilir." + v.ENTER +
                                    "   CONSTRAINT FK_detailTableName_masterTableName FOREIGN KEY (foringkeyFieldName)  REFERENCES masterTableName (keyFieldName)" + v.ENTER +
                                    " 2. MS_TABLES tablosunda detailTableName fieldleri üzerinde -Foreing Field- tanımlaması yapılmamış olabilir..." + v.ENTER2 +
                                    " Eğer bunlar tamam ve yinede çalışmıyor ise yardım isteyin...";

                                MessageBox.Show(s, function_name);

                            }
                        }
                    }
                    catch (Exception)
                    {
                        //
                        throw;
                    }

                    dsMSFields.Dispose();

                } // (IsNotNull(dsMSFields)) 
                #endregion Fields and ReadData

            } // IsNotNull(dsData)

            #endregion Read Data
            
            #region Constraint örnekleri
            /* Constraint örnekleri


             DataTable customersTable = dsData.Tables["KURSIYER"];
             DataTable ordersTable = dsData.Tables["ADRES"];

             // Create unique and foreign key constraints.
             UniqueConstraint uniqueConstraintX = new
                 UniqueConstraint(customersTable.Columns["ID"]);
                        
             ForeignKeyConstraint fkConstraint = new
                     ForeignKeyConstraint("FK_ADRES_KURSIYER",
                         customersTable.Columns["ID"],
                             ordersTable.Columns["CARI_ID"]);

             fkConstraint.DeleteRule = Rule.None;
                         
             // Add the constraints.  EN SON ÇALIŞAN BU ADD OLDU
             dsData.Tables["ADRES"].Constraints.Add(fkConstraint);
                        
             // BİR TÜRLÜ ÇALIŞMADI
             //ordersTable.Constraints.AddRange(new Constraint[] { uniqueConstraintX, fkConstraint });
                        
             * 
             * --------------------------------------------------------------
             * 
             * * 
               ForeignKeyConstraint custOrderFK = new ForeignKeyConstraint("CustOrderFK",
               custDS.Tables["CustTable"].Columns["CustomerID"], 
               custDS.Tables["OrdersTable"].Columns["CustomerID"]);
               custOrderFK.DeleteRule = Rule.None;  
               // Cannot delete a customer value that has associated existing orders.
               custDS.Tables["OrdersTable"].Constraints.Add(custOrderFK);
            
            try
            {
                // Reference the tables from the DataSet.
                DataTable customersTable = dataSet.Tables["Customers"];
                DataTable ordersTable = dataSet.Tables["Orders"];

                // Create unique and foreign key constraints.
                UniqueConstraint uniqueConstraint = new 
                    UniqueConstraint(customersTable.Columns["CustomerID"]);
             
                ForeignKeyConstraint fkConstraint = new 
                    ForeignKeyConstraint("CustOrdersConstraint",
                    customersTable.Columns["CustomerID"],
                    ordersTable.Columns["CustomerID"]);

                // Add the constraints.
                customersTable.Constraints.AddRange(new Constraint[] 
                    {uniqueConstraint, fkConstraint});
            }
            catch(Exception ex)
            {
                // Process exception and return.
                Console.WriteLine("Exception of type {0} occurred.", 
                    ex.GetType());
            }

            //-------------------------------
            this.dataTable1.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "Column1"}, false)});
            

            this.dataTable2.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("Constraint1", "Table1", new string[] {
                        "Column1"}, new string[] {
                        "Column5"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
            this.dataTable2.TableName = "Table2";
            
            */
            #endregion Constraint örnekleri
        }

        private string Preparing_UserParam(string UserParam, string tTableCode, 
                          string KonuOlanTableCode, string KonuOlanFName, string KonuOlanValue)
        {
            string tParam1 = string.Empty;
            string tParam2 = string.Empty;

            string basamak = MyProperties_Get(UserParam, tTableCode + "_basamak:");
            string and_in = MyProperties_Get(UserParam, tTableCode + "_and_in:");
            string and_select = MyProperties_Get(UserParam, tTableCode + "_and_select:");
            string in__select = MyProperties_Get(UserParam, tTableCode + "_in__select:");

            string block = string.Empty;
            string and_block = string.Empty;
            string UserParam2 = UserParam;

            string s1 = string.Empty;
            string s2 = string.Empty;
            int i1 = 0;
            int i2 = 0;

            tTableCode = "[" + tTableCode + "]";

            if (KonuOlanTableCode.IndexOf('[') < 0)
                KonuOlanTableCode = "[" + KonuOlanTableCode + "]";

            if (((IsNotNull(KonuOlanValue)) && (tTableCode == KonuOlanTableCode)))
            {
                // tParam1 =  and [KRSYR].ID = 100  
                tParam1 = " and " + KonuOlanTableCode + "." + KonuOlanFName + " = " + KonuOlanValue + " " + v.ENTER;
            }

            if (((IsNotNull(KonuOlanValue)) && (tTableCode != KonuOlanTableCode)))
            {
                // and_in  =  and [ADRS].CARI_ID in (                 
                //     s1  =  in ( 
                //     s2  =  and [ADRS].CARI_ID 
                // tParam2 =  and [ADRS].CARI_ID = xxx 

                if ((basamak == "1") || (basamak == "2"))
                {
                    s1 = "in ( ";
                    s2 = and_in.Substring(0, and_in.Length - s1.Length);
                    tParam2 = s2 + " = " + KonuOlanValue + " " + v.ENTER;
                }

                if (basamak == "3")
                {
                    s1 = "in ( ";
                    s2 = and_in.Substring(0, and_in.Length - s1.Length);
                    tParam2 = and_in + in__select + " " + KonuOlanValue + " ) " + v.ENTER;
                    //ADRS_and_in    :   and [ADRS].CARI_ID in (
                    //ADRS_in__select:   select [KRSYR].ID from KURSIYER [KRSYR] where [KRSYR].GRUP_ID = ;   
                }

                s1 = string.Empty;
                s2 = string.Empty;
            }

            while (UserParam2.IndexOf("=ROWE_") > -1)
            {
                block = Get_And_Clear(ref UserParam2, "=ROWE_");
                and_block = MyProperties_Get(block, "_PARAMS:");

                if (and_block.Length > 0)
                {
                    if (and_block.IndexOf(tTableCode) > 0)
                    {
                        // aşağıdaki format hazırlanıyor
                        // and [GRP].TEORIK_BAS_TARIHI >=   CONVERT(SMALLDATETIME, '01.01.2015 00:00:00', 101)  
                        // and [GRP].TEORIK_BAS_TARIHI <=   CONVERT(SMALLDATETIME, '06.03.2015 00:00:00', 101) 
                        tParam1 = tParam1 + and_block;
                    }
                    else
                    {
                        // aşağıdaki formatlar hazırlanıyor
                        //=ADRS_basamak:3;
                        //=ADRS_and_in: and [ADRS].CARI_ID in ( ;
                        //=ADRS_and_select: select [ADRS].CARI_ID from ADRES [ADRS] where 0 = 0 ;
                        //=ADRS_in__select: select [KRSYR].ID from KURSIYER [KRSYR] where [KRSYR].GRUP_ID = ;
            

                        // ----
                        // and [GRP].ID in (
                        //    select [KRSYR].GRUP_ID from KURSIYER [KRSYR] where 0 = 0  )
                        // ---
                        i1 = and_block.IndexOf("[");
                        i2 = and_block.IndexOf("]");
                        s1 = and_block.Substring(i1 + 1, (i2 - i1) - 1);
                        s2 = s1 + "_and_select:";
                        s1 = MyProperties_Get(UserParam, s2);

                        tParam2 = tParam2 +
                            and_in + v.ENTER +            //    and [GRP].ID in (
                            s1 + v.ENTER +                //    select [KRSYR].GRUP_ID from KURSIYER [KRSYR] where 0 = 0 
                            and_block + " ) " + v.ENTER;  //    and [KRSYR].TAMADI like  '%tekin%'  ) 
                    }
                }
            }

            #region Açıklama
            /* gelen UserParam
             
            =BEGIN:
            =GRP_basamak:1;
            =GRP_and_in: and [GRP].ID in ( ;
            =GRP_and_select: select [GRP].ID from GRUP [GRP] where 0 = 0 ;
            =GRP_in__select:null;
            =KRSYR_basamak:2;
            =KRSYR_and_in: and [KRSYR].GRUP_ID in ( ;
            =KRSYR_and_select: select [KRSYR].GRUP_ID from KURSIYER [KRSYR] where 0 = 0 ;
            =KRSYR_in__select: select [GRP].ID from GRUP [GRP] where [GRP]. = ;
            =ADRS_basamak:3;
            =ADRS_and_in: and [ADRS].CARI_ID in ( ;
            =ADRS_and_select: select [ADRS].CARI_ID from ADRES [ADRS] where 0 = 0 ;
            =ADRS_in__select: select [KRSYR].ID from KURSIYER [KRSYR] where [KRSYR].GRUP_ID = ;
            =KMLK_basamak:3;
            =KMLK_and_in: and [KMLK].CARI_ID in ( ;
            =KMLK_and_select: select [KMLK].CARI_ID from KIMLIK [KMLK] where 0 = 0 ;
            =KMLK_in__select: select [KRSYR].ID from KURSIYER [KRSYR] where [KRSYR].GRUP_ID = ;
            =KRSEK_basamak:3;
            =KRSEK_and_in: and [KRSEK].KURSIYER_ID in ( ;
            =KRSEK_and_select: select [KRSEK].KURSIYER_ID from KURSIYER_EK [KRSEK] where 0 = 0 ;
            =KRSEK_in__select: select [KRSYR].ID from KURSIYER [KRSYR] where [KRSYR].GRUP_ID = ;
            =END:

              
            =ROW_[GRP]:null;
            =[GRP]_PARAMS: and [GRP].TEORIK_BAS_TARIHI >=   CONVERT(SMALLDATETIME, '01.01.2015 00:00:00', 101)  
             and [GRP].TEORIK_BAS_TARIHI <=   CONVERT(SMALLDATETIME, '06.03.2015 00:00:00', 101)  
            ;
            =ROWE_[GRP]:null;
            =ROW_[KRSYR]:null;
            =[KRSYR]_PARAMS: and [KRSYR].TAMADI like  '%tekin%' 
             and [KRSYR].CEP_TEL like  '%532%' 
            ;
            =ROWE_[KRSYR]:null;
  
            */

            // gelen tTableCode ise ( ya  GRP  yada  KRSYR ) şeklindeki alias
            
            //---------------------------------------------------------

            // ortaya çıkması gereken sonuclar

            // 1. GRP için
            // main table için kriterler

            // and [GRP].TEORIK_BAS_TARIHI >=   CONVERT(SMALLDATETIME, '01.01.2015 00:00:00', 101)  
            // and [GRP].TEORIK_BAS_TARIHI <=   CONVERT(SMALLDATETIME, '06.03.2015 00:00:00', 101)  

            // and [GRP].ID in ( 
            //    select [KRSYR].GRUP_ID from KURSIYER [KRSYR] where 0 = 0
            //    and [KRSYR].AD like  '%tekin%' 
            //    )
            
            // 2. KRSYR için
            
            // detail için kriterler 
            
            // and [KRSYR].AD like  '%tekin%' 

            // and [KRSYR].GRUP_ID in ( 
            //    select [GRP].ID from GRUP [GRP] where 0 = 0   
            //    and [GRP].TEORIK_BAS_TARIHI >=   CONVERT(SMALLDATETIME, '01.01.2015 00:00:00', 101)  
            //    and [GRP].TEORIK_BAS_TARIHI <=   CONVERT(SMALLDATETIME, '06.03.2015 00:00:00', 101)  
            //    )

            // gelen param hazırlanırken
            // master ise
            // grp      // and [GRP].ID in ( 
            // grp      //    select [GRP].ID from GRUP [GRP] where 0 = 0   
            // detail ise
            // krsyr >> // and [KRSYR].GRUP_ID in ( 
            // krsyr    //    select [KRSYR].GRUP_ID from KURSIYER [KRSYR] where 0 = 0
            #endregion Açıklama

            return (tParam1 + v.ENTER + tParam2);
        }

        public string RSQL_ParamFields(DataSet dsKrtr, int tTableType)
        {
            string s = string.Empty;

            string tTableName = string.Empty;
            string FindTable = "_FULL";
            string sFields = string.Empty;
            string sTableName = string.Empty;
            string sWhere = string.Empty;
            string idFieldName = string.Empty;

            int i2 = dsKrtr.Tables.Count; 
            int i3 = 0;
            
            #region Table ise
            if (tTableType == 1)
            {
                                
                for (int i = 0; i < i2; i++)
                {
                    tTableName = dsKrtr.Tables[i].TableName.ToString();

                    i3 = tTableName.IndexOf(FindTable);

                    if (i3 > 0)
                    {
                        tTableName = tTableName.Substring(0, i3);

                        // ilk fieldi tablonun id kabul ediyoruz
                        idFieldName = Set(dsKrtr.Tables[i].Rows[0]["FIELD_NAME"].ToString(), "-1", "-1");

                        if (sTableName == "")
                        {
                            sFields = " tbl" + i.ToString() + ".* ";
                            sTableName = tTableName + " tbl" + i.ToString();
                            sWhere = " tbl" + i.ToString() + "." + idFieldName + " = -1 ";
                        }
                        else
                        {
                            sFields = sFields + " , tbl" + i.ToString() + ".* ";
                            sTableName = sTableName + " , " + tTableName + " tbl" + i.ToString();
                            sWhere = sWhere + v.ENTER + " and tbl" + i.ToString() + "." + idFieldName + " = -1 ";
                        }
                    }
                }

                if (sFields.IndexOf("LKP_") == -1)
                {
                    s = " Select " + sFields + v.ENTER
                      + " from " + sTableName + v.ENTER
                      + " where " + sWhere + v.ENTER;
                }
                /* şeklinde sql hazırlanıyor
                @" select t1.* , t2.* , t3.*
                from KURSIYER t1, ADRES t2, KIMLIK t3
                where t1.ID = -1  
                and t2.ID = -1  
                and t3.ID = -1 ";
                */
            }
            #endregion Table ise

            #region Select/TableIPCode ise
            if (tTableType == 6)
            {
                for (int i = 0; i < i2; i++)
                {
                    tTableName = dsKrtr.Tables[i].TableName.ToString();
                    if (tTableName.IndexOf("_FULL") > 0)
                    {
                        string declare = string.Empty;
                        string fname = string.Empty;
                        string field_type = string.Empty;
                        string field_length = string.Empty;

                        int i5 = dsKrtr.Tables[i].Rows.Count;

                        for (int i4 = 0; i4 < i5; i4++)
                        {
                            fname = Set(dsKrtr.Tables[i].Rows[i4]["LKP_FIELD_NAME"].ToString(), "", "");
                            field_type = Set(dsKrtr.Tables[i].Rows[i4]["LKP_FIELD_TYPE"].ToString(), "0", "0");
                            field_length = Set(dsKrtr.Tables[i].Rows[i4]["LKP_FIELD_LENGTH"].ToString(), "0", "0");

                            if (field_type == "0")
                            {
                                MessageBox.Show("DİKKAT : " + fname + "  için Field Type tanımlı değil...");
                                field_type = "167";
                            }

                            declare = declare +
                                "  declare @" + fname + " " + Get_FieldTypeName(Convert.ToInt16(field_type), field_length) + v.ENTER;
                            sFields = sFields +
                                "  , " + "@" + fname + " " + fname + v.ENTER;

                        }

                        if (sFields.Length > 4)
                        {
                            // en baştaki virgülü temizle
                            sFields = sFields.Remove(0, 4);
                            sFields = "    " + sFields;
                        }

                        s = declare + v.ENTER +
                            " Select " + v.ENTER + sFields + v.ENTER;

                        break;
                    }
                }
            }

            #endregion Select/TableIPCode ise

            return s;
        }

        public string Read_ParamsValue(Form tForm, VGridControl tVGrid, string TableName)
        {
            
            DataSet dsKrtr = Find_DataSet(tForm, "", TableName, "");
            DataSet dsParamValue = Find_DataSet(tForm, "", "tPARAMS_" + TableName, "");
            
            #region Tanımlar

            string fullname = string.Empty;
            string fname = string.Empty;
            string fvalue1 = string.Empty;
            string fvalue2 = string.Empty;
            string tmpkriter = string.Empty;
            string fkriter = string.Empty;
            string foperand1 = string.Empty;
            string foperand2 = string.Empty;
            
            string ttable_alias = string.Empty;
            string utable_alias = string.Empty;
            string tkrt_alias = string.Empty;
            string s = string.Empty;
            string myProp = string.Empty;
            string editorType = string.Empty;

            bool DeclareField = false;
            bool FirstDeclareField = false;

            int ffieldtype = 0;
            
            #endregion Tanımlar

            /*
              
            =ROW_[GRP]:null;
            =[GRP]_PARAMS: and [GRP].TEORIK_BAS_TARIHI >=   CONVERT(SMALLDATETIME, '01.01.2015 00:00:00', 101)  
             and [GRP].TEORIK_BAS_TARIHI <=   CONVERT(SMALLDATETIME, '06.03.2015 00:00:00', 101)  
            ;
            =ROWE_[GRP]:null;
            =ROW_[KRSYR]:null;
            =[KRSYR]_PARAMS: and [KRSYR].TAMADI like  '%tekin%' 
             and [KRSYR].CEP_TEL like  '%532%' 
            ;
            =ROWE_[KRSYR]:null;
  
            */
            int t1 = tVGrid.Rows.Count;
            int t2 = 0;

            for (int i1 = 0; i1 < t1; i1++)
            {
                t2 = tVGrid.Rows[i1].ChildRows.Count;
                
                #region categori altı
                for (int i2 = 0; i2 < t2; i2++)
                {
                    fvalue1 = string.Empty;
                    fvalue2 = string.Empty;
                    ffieldtype = 0;
                    foperand1 = string.Empty;
                    foperand2 = string.Empty;
                                        
                    fullname = tVGrid.Rows[i1].ChildRows[i2].Name.ToString();
                    fname = tVGrid.Rows[i1].ChildRows[i2].Properties.FieldName.ToString();
                    // new CheckedComboBoxEdit
                    editorType = tVGrid.Rows[i1].ChildRows[i2].Properties.RowEdit.EditorTypeName.ToString();

                    TableIPCode_Get(fullname, ref ttable_alias, ref s);

                    if (utable_alias != ttable_alias)
                    {
                        /*
                        if (utable_alias.Length > 0)
                            fkriter = fkriter + utable_alias + "_END:" + v.ENTER;
                        
                        fkriter = fkriter + ttable_alias + "_START:" + v.ENTER;
                        */

                        if (utable_alias.Length > 0)
                        {
                            MyProperties_Set(ref myProp, "ROW_" + utable_alias, "");
                            MyProperties_Set(ref myProp, "ALIAS" , utable_alias);  
                            MyProperties_Set(ref myProp, utable_alias + "_PARAMS", fkriter);
                            MyProperties_Set(ref myProp, "ROWE_" + utable_alias, "");  
                            
                            fkriter = string.Empty;
                        }

                        utable_alias = ttable_alias;
                    }
                    
                    if (tVGrid.Rows[i1].ChildRows[i2].Tag != null)
                        ffieldtype = Convert.ToInt32(tVGrid.Rows[i1].ChildRows[i2].Tag);
                    
                    if (dsParamValue.Tables[0].Rows.Count == 2)
                    {
                        fvalue1 = dsParamValue.Tables[0].Rows[0][fname].ToString(); // başlangıç kolonu
                        fvalue2 = dsParamValue.Tables[0].Rows[1][fname].ToString(); // bitiş kolonu 
                    }

                    #region 
                    /*
                    1, '', '>='
                    2, '', '>'
                    3, '', '='
                    4, '', '<='
                    5, '', '<'
                    6, '', 'Benzerleri (%abc%)'
                    7, '', 'Benzerleri (abc%)'
                    8, '', '<>'
                    */
                    #endregion

                    if (IsNotNull(fvalue1) && IsNotNull(fvalue2))
                    {
                        //* text = (char)175, (varchar)167, (ntext)99, (text)35, (nchar)239
                        if ((ffieldtype == 175) | (ffieldtype == 167) | (ffieldtype == 99) | (ffieldtype == 35) | (ffieldtype == 239))
                        {
                            foperand1 = " like ";
                            foperand2 = " like ";
                        }
                        else
                        {
                            foperand1 = " >= ";
                            foperand2 = " <= ";
                        }
                    }

                    if (IsNotNull(fvalue1) && IsNotNull(fvalue2)==false)
                    {
                        
                        //* text = (char)175, (varchar)167, (ntext)99, (text)35, (nchar)239
                        if ((ffieldtype == 175) | (ffieldtype == 167) | (ffieldtype == 99) | (ffieldtype == 35) | (ffieldtype == 239))
                             foperand1 = " like ";
                        else foperand1 = " = ";
                        
                    }

                    // new CheckedComboBoxEdit
                    if (editorType == "CheckedComboBoxEdit")
                    {
                        foperand1 = " in ";
                    }

                    if (IsNotNull(fvalue1))
                    {
                        DeclareField = DeclarePreparingField(fullname, fvalue1, ffieldtype);
                        FirstDeclareField = DeclareField;

                        if (DeclareField == false)
                        {
                            if (foperand1 != " like ")
                            {
                                fvalue1 = tData_Convert(fvalue1, ffieldtype);
                                fullname = tField_Convert(fullname, ffieldtype);
                                fkriter = fkriter + " and " + fullname + foperand1 + fvalue1 + v.ENTER;
                            }
                            if (foperand1 == " like ")
                            {
                                fvalue1 = tData_Convert("%" + fvalue1 + "%", ffieldtype);
                                fkriter = fkriter + " and " + fullname + foperand1 + fvalue1 + v.ENTER;
                            }
                            // new CheckedComboBoxEdit
                            if (foperand1 == " in ")
                            {
                                //* text = (char)175, (varchar)167, (ntext)99, (text)35, (nchar)239
                                if ((ffieldtype == 175) | (ffieldtype == 167) | (ffieldtype == 99) | (ffieldtype == 35) | (ffieldtype == 239))
                                    fvalue1 = tData_Convert(fvalue1, 1672);

                                //* rakam  56, 48, 127, 52, 60, 62, 59, 106, 108
                                if ((ffieldtype == 56) | (ffieldtype == 48) | (ffieldtype == 127) | (ffieldtype == 52) |
                                    (ffieldtype == 60) | (ffieldtype == 62) | (ffieldtype == 59) | (ffieldtype == 106) | (ffieldtype == 108))
                                    fvalue1 = tData_Convert(fvalue1, 562);

                                fkriter = fkriter + " and " + fullname + foperand1 + fvalue1 + v.ENTER;
                            }
                        }
                        else
                        {
                            // DeclareField yüzünden boş döndüğünde DataRead çalışmıyor
                            // çalışsın diye
                            fkriter = fkriter + "  ";
                        }
                    }

                    if (IsNotNull(fvalue2) && (FirstDeclareField == false))
                    {
                        if (foperand2 != " like ")
                        {
                            fvalue2 = tData_Convert(fvalue2, ffieldtype);
                            fullname = tField_Convert(fullname, ffieldtype);
                            fkriter = fkriter + " and " + fullname + foperand2 + fvalue2 + v.ENTER;
                        }
                        if (foperand2 == " like ")
                        {
                            fvalue2 = tData_Convert("%" + fvalue2 + "%", ffieldtype);
                            fkriter = fkriter + " and " + fullname + foperand2 + fvalue2 + v.ENTER;
                        }
                    }
                                        
                    // --------------------------

                }
                #endregion categori altı 
               
            }

            if (fkriter.Length > 0)
            {
                MyProperties_Set(ref myProp, "ROW_" + utable_alias, "");
                MyProperties_Set(ref myProp, "ALIAS", utable_alias); 
                MyProperties_Set(ref myProp, ttable_alias + "_PARAMS", fkriter);
                MyProperties_Set(ref myProp, "ROWE_" + ttable_alias, "");   
                            
                //    fkriter = fkriter + utable_alias + "_END:" + v.ENTER;
            }

            return myProp; // fkriter;
        }

        public void Clear_ParamsValue(Form tForm, string TableName)
        {
            DataSet dsParamValue = Find_DataSet(tForm, "", "tPARAMS_" + TableName, "");
            if (IsNotNull(dsParamValue))
            {
                dsParamValue.Tables[0].Rows.Clear();
                dsParamValue.Tables[0].Rows.Add();
                dsParamValue.Tables[0].Rows.Add();
            }
        }


        #endregion Report.Table işlemleri

        #region AllFormsClose, WaitForm

        public void AllFormsClose()
        {
            int i2 = Application.OpenForms.Count;

            for (int i = i2 - 1; i > 0; i--)
            {
                Application.OpenForms[i].Dispose();
            }
        }

        public void WaitFormOpen(Form tForm, string Mesaj)
        {
            if (Mesaj == "") Mesaj = "İşlmeniz yapılıyor ...";
            Mesaj = "  " + Mesaj.PadRight(100);

            if (v.SP_OpenApplication == false)
            {
                SplashScreenManager.ShowForm(tForm, typeof(DevExpress.XtraWaitForm.AutoLayoutDemoWaitForm), true, true, false);
                //SplashScreenManager.ShowForm(tForm, typeof(DevExpress.XtraWaitForm.ManualLayoutDemoWaitForm), true, true, false);
                //SplashScreenManager.ShowForm(tForm, typeof(DevExpress.XtraWaitForm.DemoWaitForm), true, true, false);

                //SplashScreenManager.ShowForm(tForm, typeof(DevExpress.XtraSplashScreen.DemoProgressSplashScreen), true, true, false);
                //SplashScreenManager.ShowForm(tForm, typeof(), true, true, false);
            }

            //SplashScreenManager.ShowDefaultProgressSplashScreen(Mesaj);

            SplashScreenManager.Default.SetWaitFormCaption(v.Wait_Caption);
            SplashScreenManager.Default.SetWaitFormDescription(Mesaj);
        }

        public void WaitFormClose()
        {
            SplashScreenManager.CloseForm(false);
        }

        #endregion AllFormsClose


        #region WebBrowser

        /// <summary>
        /// WebSet kullanımı
        /// </summary>
        /// tSetAttribute ws = new tSetAttribute();
        /// ---
        /// ws._01_Caption = "Mevcut Ehliyet Sınıfı";
        /// ws._02_ElemanName = "cmbMevcutEhliyetSinifi";
        /// ws._03_SetValue = MevcutSertifikaValue;
        /// ws._04_InvokeMember = v.tInvokeMember.onchange;
        /// t.WebSet(webBrowser2, ws);
        /// ---
        /// ws._01_Caption = "Mevcut Ehliyet Belge No";
        /// ws._02_ElemanName = "txtEhliyetBelgeNo";
        /// ws._03_SetValue = EskiSertifikaBelgeSayisi;
        /// t.WebSet(webBrowser2, ws);
        
        public void WebSet(WebBrowser wb, tSetAttribute ws)
        {

            // Value atama işlemi 

            //if (webBrowser2.Document.GetElementById("txtCepTelefonNo") != null)
            //{
            //    webBrowser2.Document.GetElementById("txtCepTelefonNo").SetAttribute("value", CepTelefon);
            //    while (webBrowser2.ReadyState != WebBrowserReadyState.Complete)
            //           Application.DoEvents();
            //}

            // Atanan valueden sonra nesnenin tetiklenmesi gerekirse

            //webBrowser2.Document.GetElementById("cmbEgitimDonemi").InvokeMember("onchange");
            //while (webBrowser2.ReadyState != WebBrowserReadyState.Complete)
            //    Application.DoEvents();

            if (wb.Document == null) return;

            string inner = string.Empty;

            if (ws._03_SetValue == "")
            {
                MessageBox.Show("DİKKAT [error 1000] : [ " + ws._01_Caption + " ] için veri/bilgi yok...");
                return;
            }

            //
            // mevcut value ile yeni gelen value eşit değilse işlem yapılcak
            //
            if (wb.Document.GetElementById(ws._02_ElemanName) != null)
            {
                if (wb.Document.GetElementById(ws._02_ElemanName).GetAttribute("value") == ws._03_SetValue) return;
            }

            if ((ws._03_SetValue != null) &&
                (wb.Document.GetElementById(ws._02_ElemanName) != null))
            {
                //
                // Value atama işlemi 
                //
                #region
                try
                {
                    wb.Document.GetElementById(ws._02_ElemanName).SetAttribute("value", ws._03_SetValue);
                    WebReadyComplate(wb);
                }
                catch (Exception exc1)
                {
                    inner = (exc1.InnerException != null ? exc1.InnerException.ToString() : exc1.Message.ToString());

                    WebReadyComplate(wb);
                    MessageBox.Show("DİKKAT [error 1001] : [ " + ws._01_Caption + " (" + ws._03_SetValue + ") ] veri ataması sırasında sorun oluştu ..." +
                        v.ENTER2 + inner);
                }
                #endregion
            }

            //
            // Atanan valueden sonra nesnenin tetiklenmesi gerekirse
            //
            #region
            if (ws._04_InvokeMember > v.tInvokeMember.none)
            {
                string invoke = string.Empty;
                if (ws._04_InvokeMember == v.tInvokeMember.click) invoke = "click";
                if (ws._04_InvokeMember == v.tInvokeMember.onchange) invoke = "onchange";
                if (ws._04_InvokeMember == v.tInvokeMember.submit) invoke = "submit";

                try
                {
                    WebReadyComplate(wb);
                    wb.Document.GetElementById(ws._02_ElemanName).InvokeMember(invoke);
                    WebReadyComplate(wb);
                }
                catch (Exception exc2)
                {
                    inner = (exc2.InnerException != null ? exc2.InnerException.ToString() : exc2.Message.ToString());

                    WebReadyComplate(wb);

                    MessageBox.Show("DİKKAT [error 1002] : [ " + ws._01_Caption + " (" + ws._03_SetValue + "), (" + invoke + ") ] verinin çalıştırılması sırasında sorun oluştu ..." +
                        v.ENTER2 + inner);
                }
            }
            #endregion

            //
            // Bir sonraki kullanım için değişkenleri temizle
            //
            ws._01_Caption = string.Empty;
            ws._02_ElemanName = string.Empty;
            ws._03_SetValue = string.Empty;
            ws._04_InvokeMember = v.tInvokeMember.none;

        }

        public void WebReadyComplate(WebBrowser wb)
        {
            while (wb.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
        }

        /// <summary>
        /// WebBrowser scroll position change 
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="ScrollPoint"></param>
        /// param = 1  Sayfa başı
        /// param = 2  Sayfa sonu
        /// param > 2  istenilen pixel    
        public void WebScrollPosition(WebBrowser wb, int ScrollPoint)
        {
            if (wb.Document != null)
            {
                while (wb.ReadyState != WebBrowserReadyState.Complete)
                    Application.DoEvents();

                if (ScrollPoint > 2)
                {
                    Point scrollPoint = new Point(0, ScrollPoint);
                    wb.Document.Window.ScrollTo(scrollPoint);
                }

                if (ScrollPoint == 1)
                {
                    wb.Document.Body.ScrollIntoView(true);
                }

                if (ScrollPoint == 2)
                {
                    wb.Document.Body.ScrollIntoView(false);
                }
            }
        }

        #endregion WebBrowser
        
        #region Open Exe / Execute
        public void OpenExe(string ExeName, string file)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = ExeName; // "GONDER.EXE";
            startInfo.Arguments = file;
            Process.Start(startInfo);
        }
        #endregion Open Exe / Execute

        #region Order

        private void newExeUpdate(DataSet ds)
        {
            if (IsNotNull(ds) == false) return;

            /*
             * [COMP_KEY]
                   ,[ISACTIVE]
                   ,[REC_DATE]
                   ,[UPDATE_DATE]
                   ,[EXE_NAME]
                   ,[UPDATE_TD]
                   ,[UPDATE_NO]
                   ,[VERSION_NO]
                   ,[PACKET_NAME]
                   ,[ABOUT]
            */

            
            string text =
            @" <b>" + ds.Tables[0].Rows[0]["EXE_NAME"].ToString() + @"</b><br> Mevcut Versiyon : " + Application.ProductVersion.ToString() + @"<br> Yeni Versiyon : " + ds.Tables[0].Rows[0]["VERSION_NO"].ToString() + v.ENTER + @" ";

            AlertMessage("Güncelleme var... ", text);

            /// ftp : //ustadyazilim.com
            ///u8094836@edisonhost.com

            tFtp ftpClient = new tFtp(@"ftp://ustadyazilim.com", "u8094836@edisonhost.com", "CanBerk98");
            //using ftpClient do

            //string[] simpleDirectoryListing = ftpClient.directoryListDetailed("/public");

            /* Download a File */
            ftpClient.download("public/YesiLdefter_201806201.rar", @"C:\download\YesiLdefter_201806201.rar");


            ftpClient = null;
        }

        public void AlertMessage(string caption, string text)
        {
            AlertInfo info = new AlertInfo(caption, text);
            
            AlertControl control = new AlertControl();
            //control.
            control.AllowHtmlText = true;
            control.FormLocation = AlertFormLocation.BottomRight;
            control.Show(v.mainForm, info);
            //control.Show(v.mainForm, caption, text);
        }

        public void SelectPage(Form tForm, string mainControl, string pageName, int pageIndex)
        {
            Control c = Find_Control(tForm, mainControl);

            int i2 = 0;

            if (c != null)
            {
                #region TabPane
                if (c.ToString() == "DevExpress.XtraBars.Navigation.TabPane")
                {
                    if (IsNotNull(pageName))
                    {
                        i2 = ((DevExpress.XtraBars.Navigation.TabPane)c).Pages.Count;

                        for (int i = 0; i < i2; i++)
                        {
                            if (((DevExpress.XtraBars.Navigation.TabPane)c).Pages[i].Name == pageName)
                            {
                                ((DevExpress.XtraBars.Navigation.TabPane)c).SelectedPageIndex = i;
                                break;
                            }
                        }
                    }

                    if ((IsNotNull(pageName) == false) && (pageIndex > -1))
                    {
                        ((DevExpress.XtraBars.Navigation.TabPane)c).SelectedPageIndex = pageIndex;
                    }
                }
                #endregion TabPane

                #region NavigationPane
                if (c.ToString() == "DevExpress.XtraBars.Navigation.NavigationPane")
                {
                    if (IsNotNull(pageName))
                    {
                        i2 = ((DevExpress.XtraBars.Navigation.NavigationPane)c).Pages.Count;

                        for (int i = 0; i < i2; i++)
                        {
                            if (((DevExpress.XtraBars.Navigation.NavigationPane)c).Pages[i].Name == pageName)
                            {
                                ((DevExpress.XtraBars.Navigation.NavigationPane)c).SelectedPageIndex = i;
                                break;
                            }
                        }
                    }

                    if ((IsNotNull(pageName) == false) && (pageIndex > -1))
                    {
                        ((DevExpress.XtraBars.Navigation.NavigationPane)c).SelectedPageIndex = pageIndex;
                    }
                }
                #endregion NavigationPane

                #region XtraTabControl
                if (c.ToString() == "DevExpress.XtraTab.XtraTabControl")
                {
                    if (IsNotNull(pageName))
                    {
                        i2 = ((DevExpress.XtraTab.XtraTabControl)c).TabPages.Count;

                        for (int i = 0; i < i2; i++)
                        {
                            if (((DevExpress.XtraTab.XtraTabControl)c).TabPages[i].Name == pageName)
                            {
                                ((DevExpress.XtraTab.XtraTabControl)c).TabIndex = i;
                                break;
                            }
                        }
                    }

                    if ((IsNotNull(pageName) == false) && (pageIndex > -1))
                    {
                        ((DevExpress.XtraTab.XtraTabControl)c).TabIndex = pageIndex;
                    }
                }
                #endregion XtraTabControl

                #region BackstageViewControl
                if (c.ToString() == "DevExpress.XtraBars.Ribbon.BackstageViewControl")
                {
                    if (IsNotNull(pageName))
                    {
                        i2 = ((DevExpress.XtraBars.Ribbon.BackstageViewControl)c).Controls.Count;

                        for (int i = 0; i < i2; i++)
                        {
                            if (((DevExpress.XtraBars.Ribbon.BackstageViewControl)c).Controls[i].Name == pageName)
                            {
                                // sıfırda ekranda görmediğimiz bir nesne daha mevcut
                                ((DevExpress.XtraBars.Ribbon.BackstageViewControl)c).SelectedTabIndex = (i - 1);
                                break;
                            }
                        }
                    }

                    if ((IsNotNull(pageName) == false) && (pageIndex > -1))
                    {
                        ((DevExpress.XtraBars.Ribbon.BackstageViewControl)c).SelectedTabIndex = pageIndex;
                    }
                }
                #endregion BackstageViewControl
            }
        }

        public void Table_FieldsGroups_Count(vTableAbout vTA, DataSet dsFields)
        {
            // tableCount ? 1 = sadece fields bilgileri var, 
            //              2 = MS_Groups bilgileride var
            if (dsFields == null) return;

            int tableCount = 0;
            int fieldCount = 0;
            int groupCount = 0;

            /// ----- tespit yeri -----
            tableCount = dsFields.Tables.Count;
            if (tableCount > 0)
            {
                fieldCount = dsFields.Tables[0].Rows.Count;
                if (tableCount == 2)
                    groupCount = dsFields.Tables[1].Rows.Count;

            }// tableCount

            vTA.TablesCount = tableCount;
            vTA.FieldsCount = fieldCount;
            vTA.GroupsCount = groupCount;
        }

        public void Table_About(vTableAbout vTA, DataSet dsFields)
        {
            // tableCount ? 1 = sadece fields bilgileri var, 
            //              2 = MS_Groups bilgileride var
            if (dsFields == null) return;

            int tableCount = 0;
            int rowCount = 0;
            int groupCount = 0;
            int tGroupPanelCount = 0;
            int tDLTabPageCount = 0;
            int tTabPagesCount = 0;


            /// ----- TESPİT YERİ -----

            tableCount = dsFields.Tables.Count;
            if (tableCount > 0)
            {
                rowCount = dsFields.Tables[0].Rows.Count;
                if (tableCount == 2)
                    groupCount = dsFields.Tables[1].Rows.Count;

                #region groupCount
                if (groupCount > 0)  
                {
                    if (dsFields.Tables[1].TableName == "GROUPS")
                    {
                        Int16 group_types = 0;
                        bool fvisible = false;

                        for (int i = 0; i < groupCount; i++)
                        {
                            /// GROUP_TYPES', 1, '', 'GroupPanel');
                            /// GROUP_TYPES', 2, '', 'DataLayoutTabPage');
                            /// GROUP_TYPES', 3, '', 'WelcomeWizard');
                            /// GROUP_TYPES', 4, '', 'WizardPage');
                            /// GROUP_TYPES', 5, '', 'CompletionWizard');
                            /// GROUP_TYPES', 6, '', 'TabPages');

                            group_types = Set(dsFields.Tables[1].Rows[i]["GROUP_TYPES"].ToString(), "", (Int16)0);
                            fvisible = Set(dsFields.Tables[1].Rows[i]["FVISIBLE"].ToString(), "", (bool)false);
                            if (fvisible)
                            {
                                if (group_types == 1) tGroupPanelCount++;
                                if (group_types == 2) tDLTabPageCount++;
                                if (group_types == 6) tTabPagesCount++;
                            }
                        } // for
                    } //if (dsFields.Tables[1].TableName == "GROUPS")
                }
                #endregion groupCount

            }// tableCount

            vTA.TablesCount = tableCount;
            vTA.FieldsCount = rowCount;
            vTA.RecordCount = rowCount;
            vTA.GroupsCount = groupCount;
            vTA.groupPanelCount = tGroupPanelCount;
            vTA.dLTabPageCount = tDLTabPageCount;
            vTA.dWTabPageCount = tTabPagesCount;
        }


        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            // Eşzamansız işlem için benzersiz tanımlayıcı edinin. 
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            v.mailSent = true;
        }
        public void eMailSend(string[] args)
        {
            // Command line argument must the the SMTP host.
            // Komut satırı argümanı SMTP ana bilgisayar olmalıdır. 
            SmtpClient client = new SmtpClient(args[0]);

            // Specify the e-mail sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            // E-posta gönderenini belirtin. 
            // UTF8 karakteri içeren bir posta adresi oluşturun . 
            // Görünen adda 
            MailAddress from = new MailAddress("jane@contoso.com", "Jane " + (char)0xD8 + " Clayton", System.Text.Encoding.UTF8);

            // Set destinations for the e-mail message.
            // E-posta mesajının hedeflerini ayarlayın.
            MailAddress to = new MailAddress("ben@contoso.com");

            // Specify the message content.
            // Mesaj içeriğini belirtin.
            MailMessage message = new MailMessage(from, to);
            //message.Body = "This is a test e-mail message sent by an application. ";
            message.Body = "Bu, bir uygulama tarafından gönderilen bir test e-postası mesajıdır.";

            // Include some non-ASCII characters in body and subject.
            // Gövde ve öznede bazı ASCII olmayan karakterler ekleyin.
            string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
            message.Body += Environment.NewLine + someArrows;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Subject = "test message 1" + someArrows;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            // Set the method that is called back when the send operation ends.
            // Gönderme işlemi sona erdiğinde geri çağrılan yöntemi ayarlayın.
            client.SendCompleted += new
            SendCompletedEventHandler(SendCompletedCallback);

            // The userState can be any object that allows your callback 
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            // UserState, geri arama 
            // yönteminizin bu gönderme işlemini tanımlamasına izin veren herhangi bir nesne olabilir . 
            // Bu örnekte, userToken bir dize sabitidir. 

            string userState = "test message1";
            client.SendAsync(message, userState);
            //Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
            Console.WriteLine("Mesaj gönderme ... posta iptal etmek için c tuşuna basın, çıkmak için herhangi bir tuşa basın.");
            string answer = Console.ReadLine();

            // If the user canceled the send, and mail hasn't been sent yet,
            // then cancel the pending operation.
            // Kullanıcı gönderimi iptal etmişse ve posta henüz gönderilmemişse 
            // bekleyen işlemi iptal et. 

            if (answer.StartsWith("c") && v.mailSent == false)
            {
                client.SendAsyncCancel();
            }

            // Clean up.
            // Temizlemek.
            message.Dispose();
            Console.WriteLine("Goodbye.");
        }

        public void eMailSend2()
        {
            var message = new MailMessage("admin@bendytree.com", "someone@example.com");
            message.Subject = "What Up, Dog?";
            message.Body = "Why you gotta be all up in my grill?";
            SmtpClient mailer = new SmtpClient("smtp.gmail.com", 587);
            mailer.Credentials = new NetworkCredential("admin@bendytree.com", "YourPasswordHere");
            mailer.EnableSsl = true;
            mailer.Send(message);
        }

        public void eMailSend3(eMail eMail)
        {
            string userName = "tekinucar70@gmail.com";
            string userPass = "canberk98";
            string smtp = "smtp.gmail.com";
            string port = "587";
            string toMail = eMail.toMailAddress;     // gideceği adres
            string toCCMail = eMail.toCCMailAddress; // bilgi verilen mail
            string subject = eMail.subject;          // konu başlığı
            string bodyMessage = eMail.message;      // gönderilecek mesaj

            NetworkCredential login = new NetworkCredential(userName, userPass);

            SmtpClient client = new SmtpClient(smtp);
            client.Port = Convert.ToInt32(port);
            client.EnableSsl = true;
            client.Credentials = login;

            //MailMessage msg = new MailMessage { From = new MailAddress(userName + smtp.Replace("smtp.", "@"), "Ustad Bilişim", Encoding.UTF8) };
            MailMessage msg = new MailMessage { From = new MailAddress(userName, "Üstad Bilişim", Encoding.UTF8) };
            msg.To.Add(new MailAddress(toMail));

            if (!string.IsNullOrEmpty(toCCMail))
                msg.CC.Add(toCCMail);

            msg.Subject = subject;
            msg.Body = bodyMessage;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.Normal;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback3);

            string userState = "Sending...";
            client.SendAsync(msg, userState);

        }

        private void SendCompletedCallback3(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show(string.Format("{0} send canceled.",e.UserState), "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (e.Error != null)
                MessageBox.Show(string.Format("{0} {1}", e.UserState, e.Error), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Mesaj başarıyla gönderilmiştir.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void eMailGet()
        {
            /*
            Almak biraz daha karmaşıktır. Üçüncü taraf bir kütüphaneye ihtiyaç duyuyor. 
            Google'ın GData API'sı ile çalışmaya çalıştım, ancak elimden gelen en iyi şey, gelen kutumun bir xml özet akışıydı. 
            Xml ayrıştırma yalnızca zamanı boşa harcamak isteyen insanlar içindir.

            3-4 başarısız kütüphaneden sonra OpenPOP'u çalıştım . GMail'i kullanıyorsanız, 
            gmail'in klasörleri ve POP'u olmadığı için IMAP'ı POP'dan daha iyi bulabilirsiniz. 

            Gelen kutunuzdaki en son e-postanın konusunu edinmek için gerekli olan kod:

            E-posta aldığımda daha fazla arama yapabilir ve sorgulayabilirim, ancak bu işi tamamlar.

            Bir sır bilmek ister misin? İşte, e-postayla almak istedim. Selenyum ile çalışan bazı otomasyonum var ve bazen bir captcha açılıyor. 
            
            Bu durumda bir ekran görüntüsü alıp kendim e-postayla gönderiyorum. Daha sonra, elverişli bir şekilde, e-postaya, captcha'da gördüğüm metinle cevap veriyorum. 
            
            Selenyumu içine koyar ve sürer. Kemik için epey kötü, ha?

            var client = new POPClient();
            client.Connect("pop.gmail.com", 995, true);
            client.Authenticate("admin@bendytree.com", "YourPasswordHere");
            var count = client.GetMessageCount();
            Message message = client.GetMessage(count);
            Console.WriteLine(message.Headers.Subject); 
              
            */

            
        }

        #endregion Order
    }


    #region 
    /*


Select a.[name]
 , a.[object_id]
 , b.[name] [param_name]
 , b.[parameter_id]
 , b.[system_type_id]
 , b.[user_type_id]
 , b.[max_length]

from [sys].[procedures] a
, [sys].[parameters] b
where a.[object_id] = b.[object_id] 


// Shows the open file dialog, and opens the file(s) specified by the user, if any.
    private void Open()
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Filter = "Xml Documents (*.xml)|*.xml|All Files (*.*)|*.*";
            dialog.Multiselect = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.Open(dialog.FileNames);
            }
        }
    }

bilgisayar üzerinde çalışan diğer programların listesi

            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                //process
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    Console.WriteLine("Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                }
            }





*/
    #endregion

}
