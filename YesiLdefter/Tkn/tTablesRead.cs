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
            string tSql = string.Empty;

            tToolBox t = new tToolBox();

            //tSQLs sql = new tSQLs();
            //string softCode = "";
            //string projectCode = "";
            //string TableCode = string.Empty;
            //string IPCode = string.Empty;
            //t.TableIPCode_Get(TableIPCode, ref softCode, ref projectCode, ref TableCode, ref IPCode);
            //tSql = sql.SQL_MS_TABLES_IP_LIST(softCode, projectCode, TableCode, IPCode);

            //tSql = t.msTableIPCodeTableList_SQL(TableIPCode);

            //if (ds != null)
            //    t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "MS_TABLES_IP", function_name);

            t.preparing_TableIPCodeTableList(TableIPCode);

            DataTable dt = v.ds_TableIPCodeTable.Tables[TableIPCode];
            if (dt == null) return;
            ds.Tables.Add(dt.Copy());
            dt.Dispose();
        }

        public void MS_Fields_IP_Read(DataSet ds, string TableIPCode)
        {
            string function_name = "MS_Fields_IP_Read";
            string tSql = string.Empty;

            tToolBox t = new tToolBox();
            tSQLs sql = new tSQLs();

            //tSql = sql.SQL_MS_FIELDS_IP_LIST(TableIPCode);
            //tSql = t.msTableIPCodeFieldsList_SQL(TableIPCode);
            //t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "MS_FIELDS_IP", function_name);

            t.preparing_TableIPCodeFieldsList(TableIPCode);

            DataTable dt = v.ds_TableIPCodeFields.Tables[TableIPCode];
            if (dt == null) return;
            ds.Tables.Add(dt.Copy());
            dt.Dispose();

            //tSql = sql.SQL_MS_GROUPS(TableIPCode);
            //t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "GROUPS", function_name);

            t.preparing_TableIPCodeGroupsList(TableIPCode);

            DataTable dtg = v.ds_TableIPCodeGroups.Tables[TableIPCode + "_GROUPS"];
            if (dtg == null) return;
            ds.Tables.Add(dtg.Copy());
            dtg.Dispose();



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
            //tSQLs sql = new tSQLs();

            // MasterItemType
            // 1 = Form
            // 2 = UserControl
            // 3 = Menu

            if (MasterItemType < 3)
            {
                //tSql = sql.SQL_MS_LAYOUT_LIST(MasterCode, MasterItemType);
                tSql = t.msLayoutItemsList_SQL(MasterCode);
            }

            if (MasterItemType == 3)
            {
                //tSql = sql.SQL_MS_ITEMS_LIST(MasterCode, MasterItemType);
                tSql = t.msMenuItemsList_SQL(MasterCode);
            }

            t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "", "MS_LayoutOrItems Read");
        }

        public void MS_Layout_Read(DataSet ds, string MasterCode)
        {
            string tSql = string.Empty;

            tToolBox t = new tToolBox();
            //tSQLs sql = new tSQLs();

            // MasterItemType
            // 1 = Form
            // 2 = UserControl
            // 3 = Menu

            ////tSql = sql.SQL_MS_LAYOUT_LIST(MasterCode, MasterItemType);
            //tSql = t.msLayoutItemsList_SQL(MasterCode);
            //t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "", "MS_LayoutOrItems Read");

            t.preparing_LayoutItemsList(MasterCode);

            DataTable dt = v.ds_MsLayoutItems.Tables[MasterCode];
            if (dt == null) return;
            ds.Tables.Add(dt.Copy());
            dt.Dispose();
        }

        public void MS_Menu_Read(DataSet ds, string MasterCode)
        {
            string tSql = string.Empty;

            tToolBox t = new tToolBox();
            //tSQLs sql = new tSQLs();

            // MasterItemType
            // 1 = Form
            // 2 = UserControl
            // 3 = Menu

            ////tSql = sql.SQL_MS_ITEMS_LIST(MasterCode, MasterItemType);
            //tSql = t.msMenuItemsList_SQL(MasterCode);
            //t.SQL_Read_Execute(v.dBaseNo.Manager, ds, ref tSql, "", "MS_LayoutOrItems Read");

            t.preparing_MenuItemsList(MasterCode);

            DataTable dt = v.ds_MsMenuItems.Tables[MasterCode];
            if (dt == null) return;
            ds.Tables.Add(dt.Copy());
            dt.Dispose();
        }



        #endregion System Tables Read

    }
}
