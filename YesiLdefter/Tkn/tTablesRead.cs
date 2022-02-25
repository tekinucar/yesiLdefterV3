using System.Data;
using Tkn_SQLs;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_TablesRead
{
    public class tTablesRead : tBase
    {
        #region System Tables Read 

        public void MS_Tables_IP_Read(DataSet ds, string TableIPCode)
        {
            string function_name = "MS_Tables_IP_Read";
            string tSql = string.Empty;

            tToolBox t = new tToolBox();
            tSQLs sql = new tSQLs();

            string softCode = "";
            string projectCode = "";
            string TableCode = string.Empty;
            string IPCode = string.Empty;

            t.TableIPCode_Get(TableIPCode, ref softCode, ref projectCode, ref TableCode, ref IPCode);

            tSql = sql.SQL_MS_TABLES_IP_LIST(softCode, projectCode, TableCode, IPCode);

            if (ds == null)
                t.SQL_Read_Execute(v.dBaseNo.Manager, v.dsMS_Tables_IP, ref tSql, "MS_TABLES_IP", function_name);
            if (ds != null)
                t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "MS_TABLES_IP", function_name);

            //int i1 = TableIPCode.IndexOf("3S_");
            //if ( i1 == -1) // Tablo ismi 3S_ ile başlamıyorsa
            //{
            //    if (ds == null)
            //        t.SQL_Read_Execute(v.dBName.Manager, v.dsMS_Tables_IP, ref tSql, "MS_TABLES_IP", function_name);
            //    if (ds != null)
            //        t.SQL_Read_Execute(v.dBName.Manager, ds, ref tSql, "MS_TABLES_IP", function_name);
            //}
            //else // Tablo ismi 3S_ ile başlıyorsa
            //{
            //    if (ds == null)
            //        t.SQL_Read_Execute(v.dBName.MainManager, v.dsMS_Tables_IP, ref tSql, "MS_TABLES_IP", function_name);
            //    if (ds != null)
            //        t.SQL_Read_Execute(v.dBName.MainManager, ds, ref tSql, "MS_TABLES_IP", function_name);
            //}

        }

        public void MS_Fields_IP_Read(DataSet ds, string TableIPCode)
        {
            string function_name = "MS_Fields_IP_Read";
            string tSql = string.Empty;

            tToolBox t = new tToolBox();
            tSQLs sql = new tSQLs();

            tSql = sql.SQL_MS_FIELDS_IP_LIST(TableIPCode);
            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "MS_FIELDS_IP", function_name);

            tSql = sql.SQL_MS_GROUPS(TableIPCode);
            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "GROUPS", function_name);

            //if (TableIPCode.IndexOf("3S_") == -1)
            //{
            //    tSql = sql.SQL_MS_FIELDS_IP_LIST(TableIPCode);
            //    t.SQL_Read_Execute(v.dBName.Manager, ds, ref tSql, "MS_FIELDS_IP", function_name);
            //    tSql = sql.SQL_MS_GROUPS(TableIPCode);
            //    t.SQL_Read_Execute(v.dBName.Manager, ds, ref tSql, "GROUPS", function_name);
            //}
            //else
            //{
            //    // bu özel bir durum MS_TABLES e bak 
            //    if (TableIPCode != "3S_MSTBLIP_VWJ.3S_MSTBLIP_VWJ_L01")
            //    {
            //        tSql = sql.SQL_MS_FIELDS_IP_LIST(TableIPCode);
            //        t.SQL_Read_Execute(v.dBName.MainManager, ds, ref tSql, "MS_FIELDS_IP", function_name);
            //        tSql = sql.SQL_MS_GROUPS(TableIPCode);
            //        t.SQL_Read_Execute(v.dBName.MainManager, ds, ref tSql, "GROUPS", function_name);
            //    }
            //    else
            //    {  // 3S_MSTBLIP_VWJ.3S_MSTBLIP_VWJ_L01 için özel durum 
            //        tSql = sql.SQL_MS_FIELDS_IP_LIST(TableIPCode);
            //        t.SQL_Read_Execute(v.dBName.Manager, ds, ref tSql, "MS_FIELDS_IP", function_name);
            //        tSql = sql.SQL_MS_GROUPS(TableIPCode);
            //        t.SQL_Read_Execute(v.dBName.Manager, ds, ref tSql, "GROUPS", function_name);
            //    }
            //}

            //tSql = string.Empty;
        }

        public void MS_Properties_Read(DataSet ds, string TableName, string FieldName)
        {

            string function_name = "MS_Properties_Read";
            string tSql = string.Empty;

            tToolBox t = new tToolBox();
            tSQLs sql = new tSQLs();

            tSql = sql.SQL_MS_PROPERTIES_LIST(TableName, FieldName);
            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "MS_PROPERTIES", function_name);

        }

        public void MS_LayoutOrItems_Read(DataSet ds, string MasterCode, byte MasterItemType)
        {
            string tSql = string.Empty;

            tToolBox t = new tToolBox();
            tSQLs sql = new tSQLs();

            // MasterItemType
            // 1 = Form
            // 2 = UserControl
            // 3 = Menu

            if (MasterItemType < 3)
            {
                tSql = sql.SQL_MS_LAYOUT_LIST(MasterCode, MasterItemType);
            }

            if (MasterItemType == 3)
            {
                tSql = sql.SQL_MS_ITEMS_LIST(MasterCode, MasterItemType);
            }


            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "", "MS_LayoutOrItems Read");
        }



        #endregion System Tables Read

    }
}
