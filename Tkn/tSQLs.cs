using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

using Tkn_DefaultValue;
using Tkn_Events;
using Tkn_Variable;
using Tkn_ToolBox;

using DevExpress.XtraEditors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tkn_SQLs
{
    public class tSQLs:tBase
    {
        tToolBox t = new tToolBox();

        #region Report SQLs

        public string RSQL_MasterTable(string DatabaseName, string Table_Name)
        {
            // , int Value
            // declare @value int
            // set @value = " + Value + @"
            //, @value LKP_VALUE 
            return
            @" 
            declare @table_name varchar(50)
        
            set @table_name = '" + Table_Name + @"'
        
            SELECT 1 BASAMAK, * FROM [" + v.active_DB.managerDBName + @"].dbo.MS_TABLES 
            WHERE TABLE_NAME = @table_name
        
            UNION ALL
        
            SELECT 2 BASAMAK, * FROM [" + v.active_DB.managerDBName + @"].dbo.MS_TABLES 
            WHERE MASTER_TABLE_NAME = @table_name 
                             
            UNION ALL
        
            SELECT 3 BASAMAK, * FROM [" + v.active_DB.managerDBName + @"].dbo.MS_TABLES 
            WHERE MASTER_TABLE_NAME in (
              SELECT TABLE_NAME FROM [" + v.active_DB.managerDBName + @"].dbo.MS_TABLES 
              WHERE MASTER_TABLE_NAME = @table_name
            )                             
                             
            ";

        }
                
        #endregion Report SQLs

        #region System SQLs

        public string SQL_Navigator()
        {
            string s =  
            @"
            declare @onay bit 
            declare @newCaption varchar(100)
            declare @newWidth int

            set @onay = 0
            set @newWidth = 0

            select 
              TYPES_VALUE_INT ID
            , @onay LKP_ONAY 
            , '[' + convert(varchar(5),TYPES_VALUE_INT) + '] ' + TYPES_CAPTION LKP_CAPTION
            , @newCaption LKP_NEW_CAPTION
            , @newWidth LKP_NEW_WIDTH
 
            from MS_TYPES
            where TYPES_NAME = 'NAVIGATOR'
            order by TYPES_VALUE_INT 
            ";

            return s;
        }

        public void SQL_MSFieldsIPKrtr(string TableIPCode, ref string tSqlKist, ref string tSqlFull)
        {
            string TableCode = string.Empty;
            string IPCode = string.Empty;

            t.TableIPCode_Get(TableIPCode, ref TableCode, ref IPCode);

            string MsFieldList = MS_FIELDS_LIST("b");

            // and   isnull(b.FPICTURE, '') = ''

            // and   b.KRT_OPERAND_TYPE <> 9   Visible=False değilse

            tSqlKist =
            @" select a.* " + MsFieldList + @"
               from [" + v.active_DB.managerDBName + @"].dbo.MS_FIELDS_IP  a
                  left outer join [" + v.active_DB.managerDBName + @"].dbo.MS_FIELDS b on ( a.TABLE_CODE = b.TABLE_CODE and a.FIELD_NO = b.FIELD_NO ) 
                           where a.TABLE_CODE = '" + TableCode + @"'
               and   a.IP_CODE = '" + IPCode + @"'
               and   a.KRT_LINE_NO > 0
               and   isnull(a.KRT_OPERAND_TYPE,0) <> 9
               order by a.KRT_LINE_NO, a.GROUP_LINE_NO, a.FIELD_NO  ";

            tSqlFull =
            @" select a.* " + MsFieldList + @"
               from [" + v.active_DB.managerDBName + @"].dbo.MS_FIELDS_IP a
                  left outer join [" + v.active_DB.managerDBName + @"].dbo.MS_FIELDS b on ( a.TABLE_CODE = b.TABLE_CODE and a.FIELD_NO = b.FIELD_NO ) 
               where a.TABLE_CODE = '" + TableCode + @"'
               and   a.IP_CODE = '" + IPCode + @"'
               and   isnull(a.KRT_OPERAND_TYPE,0) <> 9    
               order by a.KRT_LINE_NO, a.GROUP_LINE_NO, a.FIELD_NO      ";
        }

        public void SQL_MSFieldsKrtr(string TableCode, ref string tSqlKist, ref string tSqlFull)
        {
            // and   isnull(b.FPICTURE, '') = ''

            // and   b.KRT_OPERAND_TYPE <> 9   Visible=False değilse
            tSqlKist =
            @" select a.* 
               from [" + v.active_DB.managerDBName + @"].dbo.MS_FIELDS  a
               where a.TABLE_CODE = '" + TableCode + @"'
               and   a.KRT_LINE_NO > 0
               and   isnull(a.KRT_OPERAND_TYPE,0) <> 9
               order by a.KRT_LINE_NO, a.GROUP_LINE_NO, a.FIELD_NO  ";

            tSqlFull =
            @" select a.* 
               from [" + v.active_DB.managerDBName + @"].dbo.MS_FIELDS  a
               where a.TABLE_CODE = '" + TableCode + @"'
               and   isnull(a.KRT_OPERAND_TYPE,0) <> 9 
               order by a.KRT_LINE_NO, a.GROUP_LINE_NO, a.FIELD_NO  ";
        }

        public string SQL_ProcedureParamList(string ProcedureName)
        {
            return @"
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
              and   a.[name] = '" + ProcedureName + @"' "; 
        }
                
        public string SQL_Table_FieldsList(string DatabaseName, string Table_Name, string IPCode)
        {
            #region bulut ve mysql öncesi yapı
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

            string s1 =
            @" select  
               a.column_id, a.name 
             , convert(smallInt, a.system_type_id) system_type_id 
             , convert(smallInt, a.user_type_id)   user_type_id 
             , a.max_length, a.precision, a.scale, a.is_nullable, a.is_identity  
             from [" + DatabaseName + @"].sys.columns a 
               left outer join [" + DatabaseName + @"].sys.tables b on (a.object_id = b.object_id ) 
             where b.name = '" + Table_Name + @"' 
             order by a.column_id ";


            /// FFOREING Bit                 NULL,
            /// FTRIGGER Bit                 NULL,
            /// PROP_EXPRESSION VarChar(4000)          NULL,              
            /// VALIDATION_INSERT Bit                NULL,
            /// XML_FIELD_NAME VarChar(30)            NULL,
            /// CMP_DISPLAY_FORMAT VarChar(50) 		NULL,
            
            string s2 =
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
                 and e.IP_CODE = '" + IPCode + @"' 
                 )  
             where c.TABLE_NAME = '" + Table_Name + @"' 
             order by d.FIELD_NO 
            ";

            if ((Table_Name.IndexOf("_KIST") > -1) ||
                (Table_Name.IndexOf("_FULL") > -1))
                return "param";

            return s1 + "|ds|" + s2;
        }

        public string SQL_MSTable_FieldsList(string DatabaseName, string Table_Name)
        {
            return
            @" select  
               a.column_id, a.name 
             , convert(smallInt, a.system_type_id) system_type_id 
             , convert(smallInt, a.user_type_id) user_type_id 
             , a.max_length, a.precision, a.scale, a.is_nullable, a.is_identity  
             , d.FFOREING, d.TABLE_CODE, d.FCAPTION, d.LIST_TYPES_NAME  
             , tl.VALUE_TYPE, tl.CAPTION_FNAME, tl.WHERE_FNAME1, tl.WHERE_FNAME2, tl.TABLE_NAME 
             from [" + DatabaseName + @"].sys.columns a 
              left outer join [" + DatabaseName + @"].sys.tables b on (a.object_id = b.object_id )
              left outer join [" + v.active_DB.managerDBName + @"].dbo.MS_TABLES c on (b.name = c.TABLE_NAME)
              left outer join [" + v.active_DB.managerDBName + @"].dbo.MS_FIELDS d on (
                 a.name = d.FIELD_NAME 
                 and c.TABLE_CODE = d.TABLE_CODE
                 and a.system_type_id = d.FIELD_TYPE
                 )
              left outer join [" + DatabaseName + @"].dbo.VW_TYPES_LIST tl on ( d.LIST_TYPES_NAME = tl.TYPES_NAME )
             where 0 = 0 
             and   b.name = '" + Table_Name + @"' 
             order by a.column_id ";
        }

        public string SQL_Types_List(string Type_Name)
        {
            string s = string.Empty;

            // eğer buraya önce Type_Name isim listesi eklenirse geresiz vt sorgusu oluşmaz, 
            // onun için isim listesi ekle

            s =
              " declare @type_name varchar(100) "
            + " set @type_name = '" + Type_Name + "' "

            + " select "
            + "   'MS_TYPES' TABLE_NAME "  
            + " , a.VALUE_TYPE "
            + " , a.IMAGE_ID "
            + " , a.TYPES_VALUE_INT  VALUE_INT "
            + " , a.TYPES_VALUE_STR  VALUE_STR "
            + " , a.TYPES_CAPTION    VALUE_NAME "
            + " , ''                 VALUE_INT_FNAME "
            + " from " + v.active_DB.managerDBName + ".dbo.MS_TYPES a "
            + " where a.TYPES_NAME = @type_name "

            + " union all "

            + " select "
            + "   'MS_GROUPS' TABLE_NAME "
            + " , -1          VALUE_TYPE "
            + " , -1          IMAGE_ID "
            + " , a.FGROUPNO  VALUE_INT "
            + " , ''          VALUE_STR "
            + " , a.FCAPTION  VALUE_NAME "
            + " , ''          VALUE_INT_FNAME "
            + " from " + v.active_DB.managerDBName + ".dbo.MS_GROUPS a "
            + " where a.TABLE_CODE = @type_name "

            + " union all "

            + " select "
            + "   'SYS_TYPES_L' TABLE_NAME "  
            + " , a.VALUE_TYPE "
            + " ,  -1                IMAGE_ID "
            + " , a.TYPES_VALUE_INT  VALUE_INT "
            + " , a.TYPES_VALUE_STR  VALUE_STR "
            + " , a.TYPES_CAPTION    VALUE_NAME "
            + " , ''                 VALUE_INT_FNAME "
            + " from " + v.active_DB.projectDBName + ".dbo.SYS_TYPES_L a "
            + " where a.TYPES_NAME = @type_name "

            + " union all "

            /* Bu Select ile Sadece aranın nerede olduğunu tespit ediyoruz   */
            + " select "
            + "   TABLE_NAME "  
            + " , VALUE_TYPE "
            + " , -9             IMAGE_ID "
            + " , -9             VALUE_INT "
            + " , STR_FNAME      VALUE_STR "
            + " , CAPTION_FNAME  VALUE_NAME "
            + " , INT_FNAME      VALUE_INT_FNAME "
            + " from " + v.active_DB.projectDBName + ".dbo.SYS_TYPES_T a "
            + " where a.TYPES_NAME = @type_name "
            //+ " where a.TABLE_NAME = @type_name "

            ;

            return s;
        }

        public void SQL_Types_List(ref string SysTypesL, ref string MsTypes)
        {
            // eğer buraya önce Type_Name isim listesi eklenirse geresiz vt sorgusu oluşmaz, 
            // onun için isim listesi ekle

            /// BULUTA geçince bu şekilde ulaşım olmuyor
            /// onun için MS_TYPES proje DB yede eklendi
            ///
            ///select 'MS_TYPES' TABLE_NAME
            ///from " + v.active_DB.managerDBName + @".dbo.MS_TYPES a
            ///
            /// MS_GROUPS ta devreden çıkarıldı 
            /// gerekli olunca ona göre çare bulunacak
            /// 
            
            MsTypes = @"
            select 
             'MS_TYPES' TABLE_NAME 
            , a.TYPES_NAME 
            , a.VALUE_TYPE 
            , a.TYPES_VALUE_INT  VALUE_INT 
            , a.TYPES_VALUE_STR  VALUE_STR 
            , a.TYPES_CAPTION    VALUE_CAPTION
            , ''                 VALUE_INT_FNAME
            , ''                 WHERE_SQL 
            , ''                 ORDER_BY 
                
            from MS_TYPES a 

            union all

            select
              'MS_GROUPS'  TABLE_NAME
            , 'MS_GROUPS'  TYPES_NAME
            ,  2           VALUE_TYPE
            , a.FGROUPNO   VALUE_INT
            , ''           VALUE_STR
            , a.FCAPTION   VALUE_CAPTION
            , ''           VALUE_INT_FNAME
            , ''           WHERE_SQL
            , ''           ORDER_BY

            from MS_GROUPS a  "; 

            
            SysTypesL =
             @" Select * from ( 
                select 
                  'SYS_TYPES_L'  LKP_TABLE_NAME 
                , a.TYPES_NAME 
                , a.VALUE_TYPE 
                , a.TYPES_VALUE_INT  VALUE_INT 
                , a.TYPES_VALUE_STR  VALUE_STR 
                , a.TYPES_CAPTION    VALUE_CAPTION 
                , ''                 VALUE_INT_FNAME 
                , ''                 WHERE_SQL 
                , ''                 ORDER_BY
                
                from SYS_TYPES_L a 
                union all 
                select 
                  a.TABLE_NAME  LKP_TABLE_NAME
                , a.TYPES_NAME 
                , a.VALUE_TYPE 
                , -9               VALUE_INT 
                , a.STR_FNAME      VALUE_STR 
                , a.CAPTION_FNAME  VALUE_CAPTION 
                , a.INT_FNAME      VALUE_INT_FNAME
                , a.WHERE_SQL      
                , a.ORDER_BY
                
                from SYS_TYPES_T a 
                ) x order by TYPES_NAME  
                ";
        }

        public string SQL_SYS_Variables_List(v.dBaseType dbtype)
        {
            /// Bu SQL in doğru çalışması için 
            /// buraya sırayla FIRM_ID, SHOP_ID, PART_ID, DEPT_ID, PERSONEL_ID, COMP_ID
            /// gelmesi gerekiyor
            string s = "";

            if (dbtype == v.dBaseType.MySQL)
                s = @" CALL prc_SYS_VARIABLES_FULL ("+ v.SP_FIRM_ID + "); ";


            if (dbtype == v.dBaseType.MSSQL)
                s = @" EXEC prc_SYS_VARIABLES_FULL " + v.SP_FIRM_ID;

            return s;

            #region
            /*
            @" 
            Select [3S_SYSVAR].* 
               , ( CASE [MSVR].FJOIN_TABLE_NAME 
                   WHEN 'HP_FIRM'   THEN [HPFIRM].HP_NAME 
                   WHEN 'HP_CARI'   THEN [HPCARI].TAMADI 
                   WHEN 'HP_FINANS' THEN [HPFIN].HP_TAMADI 
                   WHEN 'HP_IL'     THEN [HPIL].IL_ADI
                   END ) LKP_CLIENT_VALUE_CAPTION

               from (
                   select 
                        [SYSVAR].[VARIABLE_CODE]
                   ,max([SYSVAR].[CLIENT_TYPE])  [CLIENT_TYPE]
                   ,max([SYSVAR].[CLIENT_ID])    [CLIENT_ID]
                   ,max([SYSVAR].[CLIENT_VALUE]) [CLIENT_VALUE] 
                   from VW_CLIENT_LIST [CLNT] 
                     left outer join [dbo].[MS_VARIABLES] [MSVR] on ( 0 = 0 )
                     left outer join [dbo].[SYS_VARIABLES] [SYSVAR] on 
                     (      [MSVR].[VARIABLE_CODE] = [SYSVAR].[VARIABLE_CODE] 
                        and [SYSVAR].[CLIENT_TYPE]   = [CLNT].CLIENT_TYPE 
                        and [SYSVAR].[CLIENT_ID]     = [CLNT].CLIENT_ID
                     )
                   where 0 = 0
                   -- and [MSVR].FJOIN_TYPE > 0
                   and [SYSVAR].ID > 0
                   and (
                       ( [SYSVAR].[CLIENT_TYPE] = 1 and [SYSVAR].CLIENT_ID = 1 )   -- firm
                    or ( [SYSVAR].[CLIENT_TYPE] = 2 and [SYSVAR].CLIENT_ID = 10 )  -- shop
                    or ( [SYSVAR].[CLIENT_TYPE] = 3 and [SYSVAR].CLIENT_ID = 0 )   -- part
                    or ( [SYSVAR].[CLIENT_TYPE] = 4 and [SYSVAR].CLIENT_ID = 0 )   -- departman
                    or ( [SYSVAR].[CLIENT_TYPE] = 5 and [SYSVAR].CLIENT_ID = 0 )   -- personel
                    or ( [SYSVAR].[CLIENT_TYPE] = 6 and [SYSVAR].CLIENT_ID = 0 )   -- computer
               ) 

               group by
                   [SYSVAR].[MODUL_CODE]
                  ,[SYSVAR].[VARIABLE_CODE]

               ) [3S_SYSVAR] 

              left outer join [dbo].[MS_VARIABLES] [MSVR] on ( 
                [3S_SYSVAR].[VARIABLE_CODE] = [MSVR].[VARIABLE_CODE] 
                )
              left outer join [dbo].[HP_FIRM] [HPFIRM] on ( 
                [MSVR].FJOIN_TABLE_NAME = 'HP_FIRM' 
                and Convert(Int, isnull([3S_SYSVAR].CLIENT_VALUE,0)) = [HPFIRM].ID 
                ) 
              left outer join [dbo].[HP_CARI] [HPCARI] on ( 
                [MSVR].FJOIN_TABLE_NAME = 'HP_CARI' 
                and Convert(Int, isnull([3S_SYSVAR].CLIENT_VALUE,0)) = [HPCARI].ID 
                ) 
              left outer join [dbo].[HP_FINANS] [HPFIN] on ( 
                [MSVR].FJOIN_TABLE_NAME = 'HP_FINANS' 
                and Convert(Int, isnull([3S_SYSVAR].CLIENT_VALUE,0)) = [HPFIN].ID 
                ) 
              left outer join [dbo].[HP_IL] [HPIL] on ( 
                [MSVR].FJOIN_TABLE_NAME = 'HP_IL' 
                and Convert(Int, isnull([3S_SYSVAR].CLIENT_VALUE, 0)) = [HPIL].IL_KODU
                ) ";
            */

            /*
Select [3S_SYSVAR].* 
 , ( CASE [MSVR].FJOIN_TABLE_NAME 
     WHEN 'HP_FIRM' THEN [HPFIRM].HP_NAME 
     WHEN 'HP_CARI' THEN [HPCARI].TAMADI 
     WHEN 'HP_FINANS' THEN [HPFIN].HP_TAMADI 
     END ) LKP_CLIENT_VALUE_CAPTION
     
 from (
   select 
           [SYSVAR].[VARIABLE_CODE]
      ,max([SYSVAR].[CLIENT_TYPE])  [CLIENT_TYPE]
      ,max([SYSVAR].[CLIENT_ID])    [CLIENT_ID]
      ,max([SYSVAR].[CLIENT_VALUE]) [CLIENT_VALUE] 
   from VW_CLIENT_LIST [CLNT] 
     left outer join [MSV3DFTR].[dbo].[MS_VARIABLES] [MSVR] on ( 0 = 0 )
     left outer join [dbo].[SYS_VARIABLES] [SYSVAR] on 
      (          [MSVR].[VARIABLE_CODE] = [SYSVAR].[VARIABLE_CODE] 
           and [SYSVAR].[CLIENT_TYPE]   = [CLNT].CLIENT_TYPE 
           and [SYSVAR].[CLIENT_ID]     = [CLNT].CLIENT_ID
      )

   where 0 = 0
   and [MSVR].FJOIN_TYPE > 0
   and [SYSVAR].ID > 0
   and (
       ( [SYSVAR].[CLIENT_TYPE] = 1 and [SYSVAR].CLIENT_ID = 1 )   -- firm
    or ( [SYSVAR].[CLIENT_TYPE] = 2 and [SYSVAR].CLIENT_ID = 10 )  -- shop
    or ( [SYSVAR].[CLIENT_TYPE] = 3 and [SYSVAR].CLIENT_ID = 0 )   -- part
    or ( [SYSVAR].[CLIENT_TYPE] = 4 and [SYSVAR].CLIENT_ID = 0 )   -- departman
    or ( [SYSVAR].[CLIENT_TYPE] = 5 and [SYSVAR].CLIENT_ID = 0 )   -- personel
    or ( [SYSVAR].[CLIENT_TYPE] = 6 and [SYSVAR].CLIENT_ID = 0 )   -- computer
       ) 

   group by
       [SYSVAR].[MODUL_CODE]
      ,[SYSVAR].[VARIABLE_CODE]

 ) [3S_SYSVAR] 
 
  left outer join [MSV3DFTR].[dbo].[MS_VARIABLES] [MSVR] on ( 
    [3S_SYSVAR].[VARIABLE_CODE] = [MSVR].[VARIABLE_CODE] 
    )
    
  left outer join HP_FIRM [HPFIRM] on ( 
    [MSVR].FJOIN_TABLE_NAME = 'HP_FIRM' 
    and Convert(Int, isnull([3S_SYSVAR].CLIENT_VALUE,0)) = [HPFIRM].ID 
    ) 
  left outer join HP_CARI [HPCARI] on ( 
    [MSVR].FJOIN_TABLE_NAME = 'HP_CARI' 
    and Convert(Int, isnull([3S_SYSVAR].CLIENT_VALUE,0)) = [HPCARI].ID 
    ) 
  left outer join HP_FINANS [HPFIN] on ( 
    [MSVR].FJOIN_TABLE_NAME = 'HP_FINANS' 
    and Convert(Int, isnull([3S_SYSVAR].CLIENT_VALUE,0)) = [HPFIN].ID 
    ) 
  
  
            */
            #endregion
        }

        public string SQL_SYS_FIRM_List(string myGuid, v.tFirmListType flt)
        {
            string tSql = "";

            #region Only Select Firm List
            
            /// kendisi ve kendisine bağlı alt firma, şubeleride getirir
            if (flt == v.tFirmListType.OnlySelect)
            tSql = @" 
             declare @_firmGUID varchar(50) 
             declare @_XID int
 
             set @_firmGUID = '" + myGuid + @"'
 
             Select @_XID = ID from SYS_FIRMS
             where FIRM_GUID = @_firmGUID

             -- Select @_XID 
 
             ;WITH CTE
             AS
             (
                SELECT M1.ID, M1.ID U_ID
        	    FROM SYS_FIRMS M1
        	    WHERE M1.PARENT_ID = @_XID
		
                UNION ALL
	
                SELECT M1.ID, M1.ID U_ID
        	    FROM SYS_FIRMS M1
        	    WHERE M1.ID = @_XID
	
        	    /* 
	            -- BUNU AÇINCA 
	            -- GRUBUN HERHANGİ BİR ÜYESİNİ İSTEYİNCE
	            -- GRUBUN TÜM ELEMANLARI GELİYOR 
	
	            UNION ALL
	
                SELECT M1.ID, M1.PARENT_ID U_ID
	            FROM SYS_FIRMS M1
	            WHERE M1.ID = @_XID
	            AND M1.PARENT_ID > 0
        	    */
	
        	    /*
	            UNION ALL
		
	            SELECT C.U_ID, M.PARENT_ID 
                FROM CTE C
                    JOIN SYS_FIRMS M ON C.U_ID = M.ID
                */

	            UNION ALL
		
        	    SELECT C.U_ID, M.ID 
                FROM CTE C
                    JOIN SYS_FIRMS M ON C.U_ID = M.PARENT_ID
		        
             )
              -- Select DISTINCT U_ID, f.* 
              Select distinct f.[ID]
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
                 left outer join SYS_FIRMS f on ( c.U_ID = f.ID ) ";
            
            #endregion

            #region All Firm List
            if (flt == v.tFirmListType.AllFirm)
            tSql = @" 
             declare @_firmGUID varchar(50) 
             declare @_XID int

             set @_firmGUID = '" + myGuid + @"'

             Select @_XID = ID from SYS_FIRMS
             where FIRM_GUID = @_firmGUID

             -- Select @_XID

             ; WITH CTE
             AS
             (
                SELECT M1.ID, M1.ID U_ID
                FROM SYS_FIRMS M1
                WHERE M1.PARENT_ID = @_XID

                UNION ALL

                SELECT M1.ID, M1.ID U_ID
                FROM SYS_FIRMS M1
                WHERE M1.ID = @_XID

	            -- BUNU AÇINCA 
	            -- GRUBUN HERHANGİ BİR ÜYESİNİ İSTEYİNCE
	            -- GRUBUN TÜM ELEMANLARI GELİYOR 
	
	            UNION ALL
	
                SELECT M1.ID, M1.PARENT_ID U_ID
	            FROM SYS_FIRMS M1
	            WHERE M1.ID = @_XID
	            AND M1.PARENT_ID > 0

                /*
	            UNION ALL
		
	            SELECT C.U_ID, M.PARENT_ID 
                FROM CTE C
                    JOIN SYS_FIRMS M ON C.U_ID = M.ID
                */
                UNION ALL

                SELECT C.U_ID, M.ID
                FROM CTE C
                    JOIN SYS_FIRMS M ON C.U_ID = M.PARENT_ID

             )
              Select distinct f.[ID]
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
            ";


            #endregion

            #region guid = TEST

            if (myGuid == "TEST")
            tSql = @" 
                Select  
                   f.[ID]
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
                from  SYS_FIRMS f 
                where f.OPERATION_MODE_TD = 1 ";

            #endregion 
            
            return tSql;
        }

        public string SQL_SYS_UPDATES()
        {
            string tSql = "";

            tSql = @"
            Select TOP 1 * from SYS_UPDATES 
            where ISACTIVE = 1 
            order by REC_DATE desc 
            ";

            return tSql;
        }

        #endregion System SQLs

        #region ManagerServer Tables SQLs Preparing

        public string SQL_MS_TABLES_IP_LIST(string TableCode, string IPCode)
        {
            string s = 
                @" Select a.*  
               , b.DBASE_TYPE          LKP_DBASE_NAME 
               , b.MODUL_CODE          LKP_MODUL_CODE 
               , b.PARENT_TABLE_CODE   LKP_PARENT_TABLE_CODE 
               , b.TABLE_CODE          LKP_TABLE_CODE 
               , b.TABLE_NAME          LKP_TABLE_NAME 
               , b.KEY_FNAME           LKP_KEY_FNAME  
               , b.TABLE_TYPE          LKP_TABLE_TYPE 

               , b.MASTER_TABLEIPCODE  LKP_MASTER_TABLEIPCODE 
               , b.MASTER_TABLE_NAME   LKP_MASTER_TABLE_NAME 
               , b.MASTER_KEY_FNAME    LKP_MASTER_KEY_FNAME 
              
               , b.FOREING_FNAME       LKP_FOREING_FNAME 
               , b.PARENT_FNAME        LKP_PARENT_FNAME 
               , b.TB_CAPTION          LKP_TB_CAPTION 
               , b.TB_SELECT_SQL       LKP_TB_SELECT_SQL 
               , b.TB_WHERE_SQL        LKP_TB_WHERE_SQL 
               , b.TB_ORDER_BY         LKP_TB_ORDER_BY 
               , b.PROP_SUBVIEW        LKP_PROP_SUBVIEW 
               , b.PROP_JOINTABLE      LKP_PROP_JOINTABLE 
              
               from MS_TABLES_IP a 
                 left outer join MS_TABLES b on ( a.TABLE_CODE = b.TABLE_CODE ) 
               where 0 = 0 " ;

            if ((TableCode != "") && (TableCode != "null"))
                s = s + " and a.TABLE_CODE = '" + TableCode + "' ";
            if ((IPCode != "") && (IPCode != "null"))
                s = s + " and a.IP_CODE = '" + IPCode + "' ";

            return s;
        }

        private string MS_FIELDS_LIST(string Alias)
        {

            string s = v.ENTER
           + " , " + Alias + ".TABLE_CODE               LKP_TABLE_CODE " + v.ENTER
           + " , " + Alias + ".FIELD_NO                 LKP_FIELD_NO " + v.ENTER
           + " , " + Alias + ".FIELD_NAME               LKP_FIELD_NAME " + v.ENTER
           + " , " + Alias + ".FIELD_TYPE               LKP_FIELD_TYPE " + v.ENTER
           + " , " + Alias + ".FIELD_LENGTH             LKP_FIELD_LENGTH " + v.ENTER
           + " , " + Alias + ".FAUTOINC                 LKP_FAUTOINC " + v.ENTER
           + " , " + Alias + ".FNOTNULL                 LKP_FNOTNULL " + v.ENTER
           + " , " + Alias + ".FREADONLY                LKP_FREADONLY " + v.ENTER
           + " , " + Alias + ".FENABLED                 LKP_FENABLED " + v.ENTER
           + " , " + Alias + ".FVISIBLE                 LKP_FVISIBLE " + v.ENTER
           + " , " + Alias + ".FINDEX                   LKP_FINDEX " + v.ENTER
           + " , " + Alias + ".FPICTURE                 LKP_FPICTURE " + v.ENTER
           + " , " + Alias + ".FFOREING                 LKP_FFOREING " + v.ENTER
           + " , " + Alias + ".FMEMORY_FIELD            LKP_FMEMORY_FIELD " + v.ENTER
           + " , " + Alias + ".FCAPTION                 LKP_FCAPTION " + v.ENTER
           + " , " + Alias + ".FHINT                    LKP_FHINT " + v.ENTER

           + " , " + Alias + ".DEFAULT_TYPE             LKP_DEFAULT_TYPE " + v.ENTER
           //+ " , " + Alias + ".DEFAULT_TYPE2            LKP_DEFAULT_TYPE2 " + v.ENTER
           + " , " + Alias + ".DEFAULT_NUMERIC          LKP_DEFAULT_NUMERIC " + v.ENTER
           + " , " + Alias + ".DEFAULT_TEXT             LKP_DEFAULT_TEXT " + v.ENTER
           + " , " + Alias + ".DEFAULT_INT              LKP_DEFAULT_INT " + v.ENTER
           + " , " + Alias + ".DEFAULT_SP               LKP_DEFAULT_SP " + v.ENTER
           + " , " + Alias + ".DEFAULT_SETUP            LKP_DEFAULT_SETUP " + v.ENTER

           + " , " + Alias + ".CMP_COLUMN_TYPE          LKP_CMP_COLUMN_TYPE " + v.ENTER
           + " , " + Alias + ".CMP_WIDTH                LKP_CMP_WIDTH " + v.ENTER
           + " , " + Alias + ".CMP_SORT_TYPE            LKP_CMP_SORT_TYPE " + v.ENTER
           + " , " + Alias + ".CMP_SUMMARY_TYPE         LKP_CMP_SUMMARY_TYPE " + v.ENTER
           + " , " + Alias + ".CMP_FORMAT_TYPE          LKP_CMP_FORMAT_TYPE " + v.ENTER
           + " , " + Alias + ".CMP_DISPLAY_FORMAT       LKP_CMP_DISPLAY_FORMAT " + v.ENTER
           + " , " + Alias + ".LIST_TYPES_NAME          LKP_LIST_TYPES_NAME " + v.ENTER
           + " , " + Alias + ".VALIDATION_OPERATOR      LKP_VALIDATION_OPERATOR " + v.ENTER
           + " , " + Alias + ".VALIDATION_VALUE1        LKP_VALIDATION_VALUE1 " + v.ENTER
           + " , " + Alias + ".VALIDATION_VALUE2        LKP_VALIDATION_VALUE2 " + v.ENTER
           + " , " + Alias + ".VALIDATION_ERRORTEXT     LKP_VALIDATION_ERRORTEXT " + v.ENTER
           + " , " + Alias + ".VALIDATION_ERRORTYPE     LKP_VALIDATION_ERRORTYPE " + v.ENTER

           + " , " + Alias + ".KRT_LINE_NO              LKP_KRT_LINE_NO " + v.ENTER
           + " , " + Alias + ".KRT_CAPTION              LKP_KRT_CAPTION " + v.ENTER // Kriter FieldName olara kullnaılıyor
           + " , " + Alias + ".KRT_OPERAND_TYPE         LKP_KRT_OPERAND_TYPE " + v.ENTER
           + " , " + Alias + ".KRT_LIKE                 LKP_KRT_LIKE " + v.ENTER
           + " , " + Alias + ".KRT_DEFAULT1             LKP_KRT_DEFAULT1 " + v.ENTER
           + " , " + Alias + ".KRT_DEFAULT2             LKP_KRT_DEFAULT2 " + v.ENTER
           + " , " + Alias + ".KRT_ALIAS                LKP_KRT_ALIAS " + v.ENTER
           + " , " + Alias + ".KRT_TABLE_ALIAS          LKP_KRT_TABLE_ALIAS " + v.ENTER

           + " , " + Alias + ".MASTER_TABLEIPCODE       LKP_MASTER_TABLEIPCODE " + v.ENTER
           + " , " + Alias + ".SEARCH_TABLEIPCODE       LKP_MASTER_TABLE_NAME " + v.ENTER
           + " , " + Alias + ".MASTER_KEY_FNAME         LKP_MASTER_KEY_FNAME " + v.ENTER
           + " , " + Alias + ".MASTER_CHECK_FNAME       LKP_MASTER_CHECK_FNAME " + v.ENTER
           + " , " + Alias + ".MASTER_CHECK_VALUE       LKP_MASTER_CHECK_VALUE " + v.ENTER

           + " , " + Alias + ".GROUP_NO                 LKP_GROUP_NO " + v.ENTER
           + " , " + Alias + ".GROUP_LINE_NO            LKP_GROUP_LINE_NO " + v.ENTER

           + " , " + Alias + ".EXPRESSION_TYPE          LKP_EXPRESSION_TYPE " + v.ENTER
           + " , " + Alias + ".PROP_EXPRESSION          LKP_PROP_EXPRESSION " + v.ENTER; // LKP_EXPRESSION

            return s;
        }

        public string SQL_MS_FIELDS_IP_LIST(string Table_IP_Code)
        {
            string Table_Code = string.Empty;
            string IP_Code = string.Empty;

            tToolBox t = new tToolBox();
            t.TableIPCode_Get(Table_IP_Code, ref Table_Code, ref IP_Code);

            string msfields_list = MS_FIELDS_LIST("b");

            return
              " Select a.* " + v.ENTER
            + " , c.TABLE_NAME               LKP_TABLE_NAME " + v.ENTER
            + msfields_list
            + " from MS_FIELDS_IP a " + v.ENTER
            + "   left outer join MS_FIELDS b on ( a.TABLE_CODE = b.TABLE_CODE and a.FIELD_NO = b.FIELD_NO ) "
            + "   left outer join MS_TABLES c on ( a.TABLE_CODE = c.TABLE_CODE ) "
            + " where a.TABLE_CODE = '" + Table_Code + "' "
            + " and   a.IP_CODE = '" + IP_Code + "' "
            + " order by "
            + " isnull(a.GROUP_NO,0), isnull(a.GROUP_LINE_NO,0), "
            + " isnull(b.GROUP_NO,0), isnull(b.GROUP_LINE_NO,0), "
            + " isnull(a.FIELD_NO,0) ";

        }

        public string SQL_MS_PROPERTIES_LIST(string TableName, string FieldName)
        {
            return 
                " select a.* from MS_PROPERTIES a "
              + " where a.TABLENAME = '" + TableName + "' "
              + " and a.FIELDNAME = '" + FieldName + "' "
              + " order by a.ROW_CATEGORY_NO, a.ROW_TYPE, a.ROW_LINE_NO ";
        }

        public string SQL_MS_ITEMS_LIST(string MasterCode, byte MasterItemType)
        {
            return
                " select a.* "
              + " , g16.GLYPH LKP_GLYPH16 "
              + " , g32.GLYPH LKP_GLYPH32 "

              + " from MS_ITEMS a "
              + "   left outer join MS_GLYPH g16 on (a.GYLPH_16 = g16.GLYPH_NAME) "
              + "   left outer join MS_GLYPH g32 on (a.GYLPH_32 = g32.GLYPH_NAME) "
              + " where a.MASTER_CODE = '" + MasterCode + "' "
              + " and a.MASTER_ITEM_TYPE = " + MasterItemType.ToString() + " "
              + " order by a.MASTER_CODE, a.ITEM_CODE ";
              //+" order by a.MASTER_CODE, a.LINE_NO, a.ITEM_CODE ";
        }

        public string SQL_MS_GLYPH_LIST()
        {

            return @" select 
              [3S_MSGLY].[REF_ID]
            , [3S_MSGLY].[MAIN_GROUP_ID]
            , [3S_MSGLY].[GROUP_ID]
            , [3S_MSGLY].[GFILE_NAME]
            , [3S_MSGLY].[GSIZE]
            , [3S_MSGLY].[GLYPH_NAME]
            , [3S_MSGLY].[GLYPH]
            , CONVERT(varchar(10),[3S_MSGLY].MAIN_GROUP_ID) + '_' +
              CONVERT(varchar(10),[3S_MSGLY].GROUP_ID) + '_' + 
              [3S_MSGLY].GFILE_NAME LKP_FULL_NAME

            from MS_GLYPH [3S_MSGLY]
            
            where 0 = 0
            and   [3S_MSGLY].MAIN_GROUP_ID <> 20
            and   isnull([3S_MSGLY].GSIZE,'') <> 'DIR'
            order by [3S_MSGLY].[MAIN_GROUP_ID]  
            ";
            //-- where [3S_MSGLY].MAIN_GROUP_ID<> 20 

            /*           
                       return  @"
                       Select[3S_MSGLY].*
                       , [GRP].GFILE_NAME LKP_GROUP_NAME
                       , CONVERT(varchar(10),[3S_MSGLY].MAIN_GROUP_ID) + '_' +
                         CONVERT(varchar(10),[3S_MSGLY].GROUP_ID) + '_' + 
                         [3S_MSGLY].GFILE_NAME LKP_FULL_NAME

                       from MS_GLYPH[3S_MSGLY]
                         left outer join MS_GLYPH[GRP] on(
                           [3S_MSGLY].MAIN_GROUP_ID = [GRP].MAIN_GROUP_ID and
                           [3S_MSGLY].GROUP_ID = [GRP].GROUP_ID  and
                           [GRP].GSIZE = 'DIR'
                         )

                       where [3S_MSGLY].MAIN_GROUP_ID<> 20 
                       and   isnull([3S_MSGLY].GSIZE,'') <> 'DIR'

                       ";

           */
        }

        public string SQL_MS_LAYOUT_LIST(string MasterCode, byte MasterItemType)
        {
            return
                " select a.* from MS_LAYOUT a "
              + " where a.MASTER_CODE = '" + MasterCode + "' "
              //+ " and a.MASTER_ITEM_TYPE = " + MasterItemType.ToString() + " "
              + " order by a.MASTER_CODE, isnull(a.GROUP_LINE_NO,0), isnull(a.LAYOUT_CODE,0) ";
        }

        public string SQL_MS_DC(string DC_Code)
        {
            return
              @" select a.* from [" + v.active_DB.managerDBName + @"].[dbo].MS_DC a  where a.DC_CODE = '" + DC_Code + @"' ";
            
            /*
            return
              @" select a.* from [" + v.active_DB.managerDBName + @"].[dbo].MS_DC a  where a.DC_CODE = '" + DC_Code + @"'  
                 union all
                 select a.* from [" + v.db_MAINMANAGER_DBNAME + @"].[dbo].MS_DC a where a.DC_CODE = '" + DC_Code + @"'  
               ";
            */
        }

        public string SQL_MS_DC_LINE(string DC_Code)
        {
            return
              @" select a.* from [" + v.active_DB.managerDBName + @"].[dbo].MS_DC_LINE a 
                 where  a.DC_CODE = '" + DC_Code + @"' 
                 order by a.LINE_NO ";

            /*
            return
              @"
               Select x.* from (
               select a.* from [" + v.active_DB.managerDBName + @"].[dbo].MS_DC_LINE a where a.DC_CODE = '" + DC_Code + @"' 
               union all
               select a.* from [" + v.db_MAINMANAGER_DBNAME + @"].[dbo].MS_DC_LINE a where a.DC_CODE = '" + DC_Code + @"' 
               ) x
               order by x.LINE_NO ";
        */
        
        }

        public string SQL_MS_GROUPS(string Table_IP_Code)
        {
            string Table_Code = string.Empty;
            string IP_Code = string.Empty;

            tToolBox t = new tToolBox();
            t.TableIPCode_Get(Table_IP_Code, ref Table_Code, ref IP_Code);

            return 
               @" Select x.* from ( 
               select 
                 a.MAIN_TABLE_NAME 
               , a.TABLE_CODE 
               , a.GROUP_TYPES 
               , a.FCAPTION 
               , a.FGROUPNO 
               , a.FVISIBLE 
               , a.FIXED 
               , a.CMP_WIDTH 
               , a.MOVE_ITEM_TYPE 
               , a.MOVE_ITEM_NAME 
               , a.MOVE_TYPE 
               , a.MOVE_LOCATION 
               , a.MOVE_LAYOUT_TYPE 
               , a.PROP_VIEWS 
               , 1 TABLE_NO 

               from MS_GROUPS a 
               where a.MAIN_TABLE_NAME = 'MS_FIELDS_IP' 
               and a.TABLE_CODE = '" + IP_Code + @"' 
              
               union all 

               select 
                 a.MAIN_TABLE_NAME 
               , a.TABLE_CODE 
               , a.GROUP_TYPES 
               , a.FCAPTION 
               , a.FGROUPNO 
               , a.FVISIBLE 
               , a.FIXED 
               , a.CMP_WIDTH 
               , a.MOVE_ITEM_TYPE 
               , a.MOVE_ITEM_NAME 
               , a.MOVE_TYPE 
               , a.MOVE_LOCATION 
               , a.MOVE_LAYOUT_TYPE 
               , a.PROP_VIEWS
               , 2 TABLE_NO 

               from MS_GROUPS a 
               where a.MAIN_TABLE_NAME = 'MS_FIELDS' 
               and a.TABLE_CODE = '" + Table_Code + @"' 
               ) x 

              order by x.TABLE_NO, x.FGROUPNO ";
        }
        
        #endregion ManagerServer Tables SQLs Preparing

        #region Preparing dsData

        public void Preparing_dsData(Form tForm, 
            DataRow row, DataSet dsFields, ref DataSet dsData,
            string MultiPageID, vTableAbout vTA)
        {
            tToolBox t = new tToolBox();

            #region Tanımlar
            string function_name = "Preparing_dsData";
            string snc = string.Empty;
            string NewSQL = string.Empty;
            //string RefID_Target = string.Empty;
            string RefID_Second = string.Empty;
            string RefID_SubView = string.Empty;
            string REF_CALL = string.Empty;
            
            byte   dBaseNo = t.Set(row["LKP_DBASE_NAME"].ToString(), "", (byte)3);
            string DBaseName = t.Find_dBLongName(dBaseNo.ToString());
            string IP_Caption = t.Set(row["IP_CAPTION"].ToString(), "", "");
            string TableName = t.Set(row["LKP_TABLE_NAME"].ToString(), "", "null");
            string TableLabel = "[" + t.Set(row["TABLE_CODE"].ToString(), "", "") + "]";
            string Where_IP_Add = t.Set(row["IP_WHERE_SQL"].ToString(), row["LKP_TB_WHERE_SQL"].ToString(), "");
            string OrderBy = t.Set(row["IP_ORDER_BY"].ToString(), row["LKP_TB_ORDER_BY"].ToString(), "");
            string Block_Field = t.Set(row["BLOCK_PRIMARY_FNAME"].ToString(), "", "");
            
            string KeyFName = t.Set(row["LKP_KEY_FNAME"].ToString(), "", "null");
            if (t.IsNotNull(KeyFName) == false)
                KeyFName = t.Find_Table_Ref_FieldName(dsFields);
            
            // for know-how 
            string TableCode = t.Set(row["TABLE_CODE"].ToString(), "", "");
            string IPCode = t.Set(row["IP_CODE"].ToString(), "", "");
            string TableIPCode = TableCode + "." + IPCode;
            string Data_Find = t.Set(row["DATA_FIND"].ToString(), "", ""); /* 0= Find yok, 1. standart, 2. List&Data   */
            string Find_FName = t.Set(row["FIND_FNAME"].ToString(), "", "");
            string AutoInsert = t.Set(row["AUTO_INSERT"].ToString(), "", "");
            
            string Prop_SubView = t.Set(row["PROP_SUBVIEW"].ToString(), row["LKP_PROP_SUBVIEW"].ToString(), "");
            #region Prop_SubView (Kendi altında SUBVIEW var ise)
                        
            // old
            if (Prop_SubView.IndexOf("=SV_ENABLED:TRUE;") > -1)
            {
                tForm.IsAccessible = true;
            } // json
            else if (t.Find_Properties_Value(Prop_SubView, "SV_ENABLED") == "TRUE")
            {
                tForm.IsAccessible = true;
            }
            else
            {
                Prop_SubView = string.Empty;
            }
            #endregion Prop_SubView

            // Navigator Buton bilgileri
            // Kart butonları,  51.Kart Aç, 52.Yeni Kart Aç  
            // Fiş  butonları,  56.Fişi Aç, 56.Yeni Fiş Aç
            string Prop_Navigator = t.Set(row["PROP_NAVIGATOR"].ToString(), "", "");

            // Tespite gitmeden önce RunTime varmı yokmu kontrol için 
            string Prop_Runtime = t.Set(row["PROP_RUNTIME"].ToString(), "", "");
            #region Prop_Runtime var ise
            if (t.IsNotNull(Prop_Runtime))
                 Prop_Runtime = "True";
            else Prop_Runtime = "False";
            #endregion Prop_Runtime var ise
            
            string ForeingID = string.Empty;
            string Kisitlama = string.Empty;
            string join_Fields = string.Empty;
            string join_Tables = string.Empty;
            string About_Detail_SubDetail = string.Empty;
            string Where_Lines = string.Empty;
            string declare = string.Empty;
            string setlist = string.Empty;
            string TargetValue = string.Empty;
            string Report_FieldsName = string.Empty;
            string Lkp_Memory_Fields = string.Empty;
            string DataWizardTabPage = string.Empty;
            string DataCopyCode = string.Empty;

            /// TABLE_TYPE  
            /// 1, 'Table'
            /// 2, 'View' 
            /// 3, 'Stored Procedure'
            /// 4, 'Function'
            /// 5, 'Trigger'
            /// 6, 'Select'

            int Table_Type = t.Set(row["TABLE_TYPE"].ToString(), row["LKP_TABLE_TYPE"].ToString(), (byte)1);

            /// DATA_READ_TYPE
            /// 1, '', 'Not Read Data'
            /// 2, '', 'Read RefID'
            /// 3, '', 'Detail Table'
            /// 4, '', 'SubDetail Table'
            /// 5, '', 'Data Collection'
            /// 6, '', 'Read SubView'

            int Data_Read_Type = t.Set(row["DATA_READ"].ToString(), "", (byte)1);
            
            // TableType.Select
            if (Table_Type == 6)
            {
                NewSQL = t.Set(row["IP_SELECT_SQL"].ToString(), row["LKP_TB_SELECT_SQL"].ToString(), "null");

                if (t.IsNotNull(NewSQL))
                    NewSQL = NewSQL + "     ";
            }
                        
            #endregion Tanımlar

            #region Read Properties Values on Form
                        
            string form_prp = t.Set( t.myFormBox_Values(tForm, v.FormLoad),  // Formun üzerindeki MemoEdit ten alınıyor
                                     v.con_FormLoadValue,                    // Gecici Hafızaya aktarılan bilgi
                                     string.Empty );

            // AddIP_ ile Form_Shown sırasında burada belirtilen TableIPCode da Create_InputPanel de oluşturulsun

            // RaporView de Konu Olan kısmını doldurmak için kullanılmakta
            // çünkü önceden neyin Konu Olan belli değil

            if (form_prp.IndexOf("AddIP_TableIPCode") > 0) v.con_AddIP = true;

            #endregion Read Properties Values on Form
            
            #region Not Read Data // Kısıtlama
            
            if ( //(Table_Type == 1) &&       // v.TableType.Table
                 (Data_Read_Type == 1) &&   // v.DataReadType.NotReadData
                 (t.IsNotNull(KeyFName)) )  // KefFName biliniyorsa
            {
                // tüm datanında okunmaması için geçiçi bir kısıtlama eklenmekte   
                Kisitlama = 
                  " /*KISITLAMALAR|1|*/ " + v.ENTER +
                  " and " + TableLabel + "." + KeyFName + " = -1 " + v.ENTER +
                  " /*KISITLAMALAR|2|*/ " + v.ENTER;
            }

            #endregion Not Raed Data
            
            #region Detail Table (Master-Detail Bağlantısı)
            if (Data_Read_Type == 3)
            {
                //LKP_MASTER_TABLE_NAME
                //LKP_MASTER_KEY_FNAME
                //LKP_FOREING_FNAME

                string Master_TableIPCode = t.Set(row["MASTER_TABLEIPCODE"].ToString(), row["LKP_MASTER_TABLEIPCODE"].ToString(),"null");
                string Master_TableName = t.Set(row["MASTER_TABLE_NAME"].ToString(), row["LKP_MASTER_TABLE_NAME"].ToString(), "null"); 
                string Master_KeyFName = t.Set(row["MASTER_KEY_FNAME"].ToString(), row["LKP_MASTER_KEY_FNAME"].ToString(),"null");
                string Foreing_FName = t.Set(row["FOREING_FNAME"].ToString(), row["LKP_FOREING_FNAME"].ToString(), "null");

                string MasterValue1 = string.Empty;

                if (t.IsNotNull(Master_TableIPCode))
                    MasterValue1 = t.Find_TableIPCode_Value(tForm, Master_TableIPCode, Master_KeyFName);

                if (t.IsNotNull(MasterValue1) == false)
                {
                    MasterValue1 = "-1";
                    TargetValue = "NewRecord";
                }
                
                if ((t.IsNotNull(Foreing_FName)) && (t.IsNotNull(MasterValue1)))
                {
                    //  BASLIK_ID = xxx         veya
                    //  BASLIK_ID = -1          vaya
                    //  BASLIK_ADI = 'xxx'      vaya
                    //  BASLIK_ADI = ''         gibi sonuclar döner  

                    snc = t.Set_FieldName_Value(dsFields, Foreing_FName, MasterValue1, "and", "=");

                    if ((snc != string.Empty) && (MasterValue1 != "-1"))
                    {
                        ForeingID = " and " + TableLabel + "." + snc + v.ENTER;
                    }
                }

                
            }
            #endregion Detail Table

            #region SubDetail Table / Data Collection
            if ((Data_Read_Type == 4) || // v.DataReadType.SubDetailTable
                (Data_Read_Type == 5))   // v.DataReadType.DataCollection 
            {
               // form_shown sırasında üzerinde Detail-SubDetail bağlıtısı olan 
               // tabloların olduğunu işaret için bu özellik true ediliyor
               // bakınız [ myForm_Shown ] a 
               tForm.IsAccessible = true;
            }
            #endregion SubDetail Table

            #region v.DataReadType.ReadSubView
            if (Data_Read_Type == 6)
            {
                ////if ((form_prp.IndexOf("SVIEWVALUE") > -1) &&
                ////    (form_prp.IndexOf("TABLEIPCODE_LIST") > -1))
                ////{
                ////    //WORKTYPE:READ
                ////    Preparing_RefID(dsFields, ref NewSQL, ref TargetValue,
                ////                    TableIPCode, TableLabel, form_prp, Data_Read_Type);
                ////}

                if (form_prp.IndexOf("SVIEWVALUE") > -1)
                {
                    if (form_prp.IndexOf("TABLEIPCODE_LIST") > -1)
                    {
                        Preparing_RefID(dsFields, ref NewSQL, ref TargetValue,
                                            TableIPCode, TableLabel, form_prp, Data_Read_Type);
                    }
                    else if (form_prp.IndexOf("[{") > -1)
                    {
                        Preparing_RefID_JSON(dsFields, 
                                             ref NewSQL, ref TargetValue, ref DataCopyCode,
                                             TableIPCode, TableLabel, form_prp, Data_Read_Type);
                    }
                }

            }
            #endregion v.DataReadType.ReadSubView

            #region Block Table

            if (t.IsNotNull(Block_Field))
                Block_Field = " and " + TableLabel + "." + Block_Field + v.ENTER;

            #endregion Block Table

            #region DataWizardTabPage find, ( True or False )
            DataWizardTabPage = "False";
            if (vTA.dWTabPageCount > 0)
                DataWizardTabPage = "True";
            #endregion

            #region Preparing SubDetail, DataCollection, Join Tables and Fields

            Preparing_Join_Fields_Tables(tForm, row, dsFields, 
                          dBaseNo, 
                          TableIPCode, 
                      ref join_Tables, 
                      ref join_Fields,
                      ref Lkp_Memory_Fields,
                      ref About_Detail_SubDetail, 
                      ref Where_Lines,
                      ref declare, 
                      ref setlist,
                      ref NewSQL,
                      ref REF_CALL,
                      ref Report_FieldsName);

            #endregion Preparing Join Tables and Fields

            #region Preparing SQL
            //ViewData_SQL =
            //    String.Format(" Select {0} * from [{1}].[dbo].[{2}] ", Adet, DatabaseName, TableName);
            
            #region Table için SQL hazırlanıyor
            if (Table_Type == 1)  // v.TableType.Table
            {
                NewSQL = 
                     " Select " + TableLabel + ".* " + v.ENTER +
                       join_Fields + v.ENTER +
                       Lkp_Memory_Fields + 
                     " from " + TableName + " " + TableLabel + v.ENTER +
                       join_Tables + v.ENTER +
                     " where 0 = 0 " + v.ENTER +
                     " /*" + TableLabel + ".DEFAULT_VALUE*/" + v.ENTER +
                       REF_CALL + // iptal kontrolü ( false = normal kayıt,  true = iptal edilmiş kayıt  )
                       Block_Field +
                       ForeingID +
                       //RefID_Target +
                       //Kisitlama +
                       //Where_IP_Add +
                       //Where_Lines +
                     " /*" + TableLabel + ".KRITERLER*/ " + v.ENTER;
            }
            #endregion Table

            #region Stored Procedure için SQL hazırlanıyor
            if (Table_Type == 3)  
            {
                string Master_TableName = t.Set(row["MASTER_TABLE_NAME"].ToString(), row["LKP_MASTER_TABLE_NAME"].ToString(), "null");
                // 
                //if ((dBaseNo == 4) && (v.dBTypes.ProjectDBType == v.dBaseType.MSSQL))
                if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                {
                    //NewSQL =
                    //    //"-- D.SD.DCL --" + v.ENTER +
                    //    //"-- D.SD.SET --" + v.ENTER +
                    //    " EXEC [dbo].[" + Master_TableName + "]" + v.ENTER +
                    //           Where_Lines;

                    string s = " EXEC [dbo].[" + Master_TableName + "]" + v.ENTER;
                    Where_Lines = Where_Lines.Replace("/*executeBegin*/", s);

                    NewSQL = Where_Lines;
                     
                }

                if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                {
                    NewSQL =
                        " CALL " + Master_TableName + " (" + v.ENTER + Where_Lines + v.ENTER + ")";
                }
            }
            #endregion Stored Procedure
            
            #region Select Cümlesine Ekler

            if (t.IsNotNull(NewSQL)) // select var ise
            {
                #region // Kisitlama add
                if (t.IsNotNull(Kisitlama))
                {
                    NewSQL = t.SQLWhereAdd(NewSQL, TableLabel, Kisitlama + v.ENTER, "DEFAULT");
                }
                #endregion // Kisitlama

                #region // Where_IP_Add add
                if (t.IsNotNull(Where_IP_Add))
                {
                    NewSQL = t.SQLWhereAdd(NewSQL, TableLabel, Where_IP_Add + v.ENTER, "DEFAULT");
                }
                #endregion // Where_IP_Add add

                #region // Where_Lines add
                if (t.IsNotNull(Where_Lines))
                {
                    NewSQL = t.SQLWhereAdd(NewSQL, TableLabel, Where_Lines, "DEFAULT");
                }
                #endregion // Where_Lines add

                #region // RefID add
                if (t.IsNotNull(form_prp) && (form_prp != "[]"))
                {
                    if (form_prp.IndexOf("TABLEIPCODE_LIST") > -1)
                    {
                        Preparing_RefID_OLD(dsFields, ref NewSQL, ref TargetValue,
                                            TableIPCode, TableLabel, form_prp, Data_Read_Type);
                    }
                    else if (form_prp.IndexOf("[{") > -1)
                    {
                        if (form_prp.IndexOf(TableIPCode) > -1)
                        {
                            Preparing_RefID_JSON(dsFields,
                                                 ref NewSQL, ref TargetValue, ref DataCopyCode,
                                                 TableIPCode, TableLabel, form_prp, Data_Read_Type);
                        }
                    }
                    else
                    {
                        Preparing_Manuel_RefID(dsFields, ref NewSQL, ref TargetValue, 
                            TableIPCode, TableLabel, KeyFName, form_prp);
                    }
                }
                #endregion // RefID add

                #region // SQL içinde Declare ve Set ibareleri kullanıldıysa
                if ((t.IsNotNull(declare)) && 
                    (Table_Type == 6))   // Select ise,  Stored Procedure değilse
                {
                    // Detail-SubDetail bağlantısı sırasında 
                    // master_fname önüne @ ibaresi konularak devreye giriyor

                    string str_dcl = "-- D.SD.DCL --";
                    string str_set = "-- D.SD.SET --";
                    int i_dcl = NewSQL.IndexOf(str_dcl) + str_dcl.Length + 1;

                    if (i_dcl > 13)
                        NewSQL = NewSQL.Insert(i_dcl, v.ENTER + declare);
                    else MessageBox.Show("DİKKAT : SQL içinde aranan ibare bulunamadı ...(" + str_dcl + ")", function_name);

                    int i_set = NewSQL.IndexOf(str_set) + str_set.Length + 1;

                    if (i_set > 13)
                        NewSQL = NewSQL.Insert(i_set, v.ENTER + setlist);
                    else MessageBox.Show("DİKKAT : SQL içinde aranan ibare bulunamadı ...(" + str_set + ")", function_name);
                }
                #endregion // Declare
            }

            #endregion // Select Cümlesine Ekler
            
            #region OrderBy

            if ( (t.IsNotNull(NewSQL)) && 
                 (t.IsNotNull(OrderBy)) )
                NewSQL = NewSQL + " order by " + OrderBy + v.ENTER;
            
            #endregion OrderBy
            
            #endregion Preparing SQL

            #region Know-how Preparing (Bilgiler Hazırlanıyor)

            string myProp = string.Empty;
            string FieldsListSQL = SQL_Table_FieldsList(DBaseName, TableName, IPCode);

            //DBaseType >> DBase_Type >> DBaseName DBaseNo
            t.MyProperties_Set(ref myProp, "DBaseNo", dBaseNo.ToString());
            t.MyProperties_Set(ref myProp, "DBaseName", DBaseName.ToString());
            t.MyProperties_Set(ref myProp, "TableIPCode", TableIPCode);
            t.MyProperties_Set(ref myProp, "TableName", TableName);
            t.MyProperties_Set(ref myProp, "TableLabel", TableLabel);
            t.MyProperties_Set(ref myProp, "TableCaption", IP_Caption);
            t.MyProperties_Set(ref myProp, "TableType", Table_Type.ToString());
            t.MyProperties_Set(ref myProp, "Cargo", "data"); // cargo = data, param, report
            t.MyProperties_Set(ref myProp, "MultiPageID", MultiPageID);
            t.MyProperties_Set(ref myProp, "DataFind", Data_Find);
            t.MyProperties_Set(ref myProp, "FindFName", Find_FName);
            t.MyProperties_Set(ref myProp, "AutoInsert", AutoInsert);
            t.MyProperties_Set(ref myProp, "OrderBy", OrderBy);
            t.MyProperties_Set(ref myProp, "JoinFields", join_Fields); // "LkpFields", Lkp_Fields);
            t.MyProperties_Set(ref myProp, "JoinTables", join_Tables); // "LkpTables", Lkp_Tables);
            t.MyProperties_Set(ref myProp, "KeyFName", KeyFName);
            t.MyProperties_Set(ref myProp, "KeyIDValue", "0");
            t.MyProperties_Set(ref myProp, "Kisitlama", Kisitlama);
            t.MyProperties_Set(ref myProp, "ForeingID", ForeingID);
            t.MyProperties_Set(ref myProp, "Where_IP_Add", Where_IP_Add);
            t.MyProperties_Set(ref myProp, "Prop_Runtime", Prop_Runtime);
            t.MyProperties_Set(ref myProp, "Prop_SubView", Prop_SubView);
            t.MyProperties_Set(ref myProp, "DataReadType", Data_Read_Type.ToString());
            t.MyProperties_Set(ref myProp, "DataWizardTabPage", DataWizardTabPage);
            t.MyProperties_Set(ref myProp, "DataCopyCode", DataCopyCode);
            //t.MyProperties_Set(ref myProp, "FieldsListSQL", FieldsListSQL);
            
            if (t.IsNotNull(About_Detail_SubDetail))
            {
                //  About_Detail_SubDetail:MasterTableIPCode||MasterTableName||MasterKeyFName||DetailFName;
                t.MyProperties_Set(ref myProp, "DetailSubDetail", "True");
                myProp = myProp + About_Detail_SubDetail;
                
                // IsAccessible == true  ise formun üzerinde 
                // kendisine (detail'e) bağlı alt tablolar(subdetail) vardır 
                tForm.IsAccessible = true;
            }

            t.MyProperties_Set(ref myProp, "SqlFirst", NewSQL);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
                        
            // bu atamaylada dsDatanın işlenmemiş SQL ele alınarak üzerine kriterler eklenerek 
            // dsData kriterli yeniden çalıştırılacak
            dsData.DataSetName = TableIPCode;
            dsData.Namespace = myProp;
            
            v.con_SQL = NewSQL;

            //v.SQL = v.ENTER2 + NewSQL + v.SQL; 
            //v.SQL = v.ENTER +
            //   "[ " + IP_Caption + " - " + TableIPCode + " ]" + v.ENTER +
            //   "===========================================================" + v.ENTER + v.SQL;

            #endregion Know-how

            #region Read dsData

            t.Data_Read_Execute(dsData, ref NewSQL, TableName, null);
            
            if ((TargetValue == "NewRecord") ||
                (AutoInsert == "True"))
            {
                if (dsData.Tables[0].Rows.Count == 0)
                    v.con_AutoNewRecords = true;
            }
            
            #endregion Read dsData
            
        }

        #region RefID Preparing

        private void Preparing_RefID_JSON(DataSet dsFields, 
                     ref string NewSQL, 
                     ref string TargetValue,
                     ref string DataCopyCode,
                     string Main_TableIPCode, string Main_TableLabel,
                     string form_prp,
                     int Data_Read_Type)
        {
            // kendinden önceki formdan gelen bilgiler

            // önceki formun Navigator Butonlarından gelen ve 
            // açılan yeni formun mymemo sundaki bilgileri analiz edeceğiz
            // Kart butonları,  51.Kart Aç, 52.Yeni Kart Aç  
            // Fiş  butonları,  56.Fişi Aç, 56.Yeni Fiş Aç

            //'WORKTYPE', 0, '', 'none');
            //'WORKTYPE', 0, 'READ', 'Read Data');
            //'WORKTYPE', 0, 'NEW', 'New Data');
            //'WORKTYPE', 0, 'GOTO', 'Goto Record');
            //'WORKTYPE', 0, 'SVIEW', 'Sub View');
            //'WORKTYPE', 0, 'SVIEWVALUE', 'Sub View Value');
            //'WORKTYPE', 0, 'CREATEVIEW', 'Create View');
            
            string TABLEIPCODE = string.Empty;
            string TABLEALIAS = string.Empty;
            string KEYFNAME = string.Empty;
            string MSETVALUE = string.Empty;
            string WORKTYPE = string.Empty;
            string RefID = string.Empty;
            
            List<TABLEIPCODE_LIST> prop_ = JsonConvert.DeserializeObject<List<TABLEIPCODE_LIST>>(form_prp);

            #region
            foreach (var item in prop_)
            {
                TABLEIPCODE = item.TABLEIPCODE.ToString();

                #region 1
                if ((item.WORKTYPE.ToString() == "SVIEWVALUE") &&
                    (TABLEIPCODE == "") &&
                    (Data_Read_Type == 6))
                {
                    TABLEIPCODE = Main_TableIPCode;
                }
                #endregion

                #region 2
                if (item.WORKTYPE.ToString() == "CREATEVIEW")
                {
                    v.con_FormAfterCreateView = true;
                }
                #endregion

                #region 3
                if (Main_TableIPCode == TABLEIPCODE)
                {
                    TABLEALIAS = t.Set(item.TABLEALIAS.ToString(), Main_TableLabel, "");
                    KEYFNAME =  t.Set(item.KEYFNAME.ToString(),"","");
                    MSETVALUE = t.Set(item.MSETVALUE.ToString(), "0", "0");
                    WORKTYPE = item.WORKTYPE.ToString();

                    if (t.IsNotNull(item.DCCODE))
                        DataCopyCode = item.DCCODE.ToString();

                    if (TABLEALIAS.IndexOf("[") == -1)
                        TABLEALIAS = "[" + TABLEALIAS + "]";

                    if ((WORKTYPE == "READ") ||
                        (WORKTYPE == "CREATEVIEW") ||
                        (WORKTYPE == "SVIEWVALUE") ||
                        (WORKTYPE == "SVIEW"))
                        Preparing_RefID_Set(dsFields, ref RefID, TABLEALIAS, KEYFNAME, MSETVALUE);

                    if (WORKTYPE == "NEW")
                    {
                        if (t.IsNotNull(KEYFNAME) == false)
                        {
                            MessageBox.Show("DİKKAT : " + TABLEIPCODE + " için NEW DATA tanımlarında " + v.ENTER2 + "[ Target Key FName ] tanımı eksik...");
                        }
                        else
                        {
                            Preparing_RefID_Set(dsFields, ref RefID, TABLEALIAS, KEYFNAME, "0");
                        }
                    }

                    if ((WORKTYPE == "GOTO") || (WORKTYPE == "SVIEW"))
                    {
                        TargetValue = "GotoRecord";
                        v.con_GotoRecord = "ON";
                        v.con_GotoRecord_TableIPCode = TABLEIPCODE;
                        v.con_GotoRecord_FName = KEYFNAME;
                        v.con_GotoRecord_Value = MSETVALUE;
                    }

                    if (t.IsNotNull(RefID))
                    {
                        NewSQL = t.SQLWhereAdd(NewSQL, TABLEALIAS, RefID, "DEFAULT");

                        if ((MSETVALUE == "0") || (WORKTYPE == "NEW"))
                            TargetValue = "NewRecord";
                    }
                    
                }// if (Main_TableIPCode == TABLEIPCODE)
                #endregion

            }
            #endregion

        }

        private void Preparing_RefID(DataSet dsFields, ref string NewSQL, ref string TargetValue,
                     string Main_TableIPCode, string Main_TableLabel,
                     string form_prp,
                     int Data_Read_Type)
        {
            MessageBox.Show("DİKKAT : Preparing_RefID yi bekliyordun ");
            Preparing_RefID_OLD(dsFields, ref NewSQL, ref TargetValue,
                     Main_TableIPCode, Main_TableLabel,
                     form_prp,
                     Data_Read_Type);
        }


        private void Preparing_RefID_OLD(DataSet dsFields, ref string NewSQL, ref string TargetValue,
                     string Main_TableIPCode, string Main_TableLabel,
                     string form_prp,
                     int Data_Read_Type)
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

            // kendinden önceki formdan gelen bilgiler

            // önceki formun Navigator Butonlarından gelen ve 
            // açılan yeni formun mymemo sundaki bilgileri analiz edeceğiz
            // Kart butonları,  51.Kart Aç, 52.Yeni Kart Aç  
            // Fiş  butonları,  56.Fişi Aç, 56.Yeni Fiş Aç

            //'WORKTYPE', 0, '', 'none');
            //'WORKTYPE', 0, 'READ', 'Read Data');
            //'WORKTYPE', 0, 'NEW', 'New Data');
            //'WORKTYPE', 0, 'GOTO', 'Goto Record');
            //'WORKTYPE', 0, 'SVIEW', 'Sub View');
            //'WORKTYPE', 0, 'SVIEWVALUE', 'Sub View Value');
            //'WORKTYPE', 0, 'CREATEVIEW', 'Create View');
            
            string RefID = string.Empty;
            string s = form_prp;

            #region 
            /*
            1=ROW_TABLEIPCODE_LIST:1;
            1=CAPTION:Fiş Aç - Cari Oku;
            1=TABLEIPCODE:PERSNL_03.PERSNL_03_02;
            1=TABLEALIAS:[HPCARI];
            1=KEYFNAME:REF_ID;
            1=RTABLEIPCODE:PERMAZRT.PERMAZRT_01;
            1=RKEYFNAME:CARI_ID;
            1=MSETVALUE:null;
            1=WORKTYPE:READ;
            1=ROWE_TABLEIPCODE_LIST:1;

            2=ROW_TABLEIPCODE_LIST:2;
            2=CAPTION:Fiş Aç - SubView Oku;
            2=TABLEIPCODE:T13_TYPESL.T13_TYPESL_IZIN_TIPI;
            2=TABLEALIAS:[T13_TYPESL];
            2=KEYFNAME:TYPES_VALUE_INT;
            2=RTABLEIPCODE:PERMAZRT.PERMAZRT_01;
            2=RKEYFNAME:IZIN_TIPI;
            2=MSETVALUE:null;
            2=WORKTYPE:SVIEW;
            2=ROWE_TABLEIPCODE_LIST:2;

            3=ROW_TABLEIPCODE_LIST:3;
            3=CAPTION:Fiş Aç - Mazeret Fişi;
            3=TABLEIPCODE:PERMAZRT.PERMAZRT_03;
            3=TABLEALIAS:[PERMAZRT];
            3=KEYFNAME:REF_ID;
            3=RTABLEIPCODE:PERMAZRT.PERMAZRT_01;
            3=RKEYFNAME:REF_ID;
            3=MSETVALUE:null;
            3=WORKTYPE:READ;
            3=ROWE_TABLEIPCODE_LIST:3;

            */
            #endregion

            if (s.IndexOf("TABLEIPCODE_LIST") > -1)
            {
                string TABLEIPCODE = string.Empty;
                string TABLEALIAS = string.Empty;
                string KEYFNAME = string.Empty;
                string MSETVALUE = string.Empty;
                string WORKTYPE = string.Empty;
                                
                string row_block = string.Empty;
                string lockE = "=ROWE_";
                
                while (s.IndexOf(lockE) > -1)
                {
                    row_block = t.Find_Properies_Get_RowBlock(ref s, "TABLEIPCODE_LIST");

                    TABLEIPCODE = t.MyProperties_Get(row_block, "TABLEIPCODE:");

                    /*
                    =ROW_TABLEIPCODE_LIST:3;
                    3=CAPTION:Fiş Aç - Mazeret Fişi;
                    3=TABLEIPCODE:null;
                    3=TABLEALIAS:[PERMAZRT];
                    3=KEYFNAME:REF_ID;
                    3=RTABLEIPCODE:PERMAZRT.PERMAZRT_01;
                    3=RKEYFNAME:REF_ID;
                    3=MSETVALUE:3;
                    3=WORKTYPE:SVIEWVALUE;
                    3=ROWE_TABLEIPCODE_LIST:
                    */

                    if ((row_block.IndexOf("SVIEWVALUE") > -1) &&
                        (TABLEIPCODE == "") &&
                        (Data_Read_Type == 6))
                    {
                        TABLEIPCODE = Main_TableIPCode;
                    }

                    if (row_block.IndexOf("CREATEVIEW") > -1)
                    {
                        v.con_FormAfterCreateView = true;
                        /*
                        1=ROW_TABLEIPCODE_LIST:1;
                        1=CAPTION:Icra Dosyasını Aç;
                        1=TABLEIPCODE:AVI_DOS.AVI_DOS_03;
                        1=TABLEALIAS:[AVI_DOS];
                        1=KEYFNAME:ID;
                        1=RTABLEIPCODE:AVI_DOS.AVI_DOS_02;
                        1=RKEYFNAME:ID;
                        1=MSETVALUE:splitContainer2.Panel1;
                        1=WORKTYPE:CREATEVIEW;
                        1=ROWE_TABLEIPCODE_LIST:1;
                        */
                    }

                    if (Main_TableIPCode == TABLEIPCODE)
                    {
                        TABLEALIAS = t.Set(t.MyProperties_Get(row_block, "TABLEALIAS:"), Main_TableLabel, "");
                        KEYFNAME = t.MyProperties_Get(row_block, "KEYFNAME:");
                        MSETVALUE = t.Set(t.MyProperties_Get(row_block, "MSETVALUE:"), "0", "0");
                        WORKTYPE = t.MyProperties_Get(row_block, "WORKTYPE:");

                        if ((WORKTYPE == "READ") ||
                            (WORKTYPE == "CREATEVIEW") ||
                            (WORKTYPE == "SVIEWVALUE") ||
                            (WORKTYPE == "SVIEW") && (MSETVALUE == "0"))
                           Preparing_RefID_Set(dsFields, ref RefID, TABLEALIAS, KEYFNAME, MSETVALUE);
                        
                        if (WORKTYPE == "NEW")
                        {
                            if (t.IsNotNull(KEYFNAME) == false)
                            {
                                MessageBox.Show("DİKKAT : " + TABLEIPCODE + " için NEW DATA tanımlarında " + v.ENTER2 + "[ Target Key FName ] tanımı eksik...");
                            }
                            else
                            {
                                Preparing_RefID_Set(dsFields, ref RefID, TABLEALIAS, KEYFNAME, "0");
                            }
                        }

                        if ((WORKTYPE == "GOTO") || (WORKTYPE == "SVIEW"))
                        {
                            TargetValue = "GotoRecord";
                            v.con_GotoRecord = "ON";
                            v.con_GotoRecord_TableIPCode = TABLEIPCODE;
                            v.con_GotoRecord_FName = KEYFNAME;
                            v.con_GotoRecord_Value = MSETVALUE;
                        }
                    }
                  }

                if (t.IsNotNull(RefID))
                {
                    NewSQL = t.SQLWhereAdd(NewSQL, TABLEALIAS, RefID, "DEFAULT");

                    if ((MSETVALUE == "0") || (WORKTYPE == "NEW")) TargetValue = "NewRecord";
                }
            }
        }
        
        private void Preparing_RefID_Set(DataSet dsFields, ref string RefID, 
                     string TableAlias, string FieldName, string Value )
        {
            string snc1 = string.Empty;
            string snc2 = string.Empty;

            //  and [xxx].REF_ID = xxx   veya  
            //  and [xxx].REF_ID = -1    
            snc1 = t.Set_FieldName_Value(dsFields, FieldName, Value, "and","=");
            snc2 = " and " + TableAlias + "." + snc1 + v.ENTER;
            if (snc1 != "") RefID = RefID + snc2; 
        }

        private void Preparing_Manuel_RefID(DataSet dsFields, ref string NewSQL, ref string Main_TargetValue,
                     string Main_TableIPCode, string Main_TableLabel, string Main_KeyFName,
                     string form_prp)
        {
            
            // DİKKAT : Manuel form açılışları sırasında kullanılıyor
            // zamanla gerek kalmaz ise silinebilir bir fonsiyon

            // açılan yeni formun mymemo MANUEL set edilen diğer bilgileri analiz edeceğiz

            #region Tanımlar
            // 1. Tablo
            string TargetTableIPCode = string.Empty;
            string TargetTableAlias = string.Empty;
            string TargetFieldName = string.Empty;
            string TargetValue = string.Empty;
            string TargetLikeValue = string.Empty;
            // 2. Tablo
            string SecondTableIPCode = string.Empty;
            string SecondTableAlias = string.Empty;
            string SecondFieldName = string.Empty;
            string SecondValue = string.Empty;
            // 3. Tablo
            string ThirdTableIPCode = string.Empty;
            string ThirdTableAlias = string.Empty;
            string ThirdFieldName = string.Empty;
            string ThirdValue = string.Empty;
            
            // Read SubView için value
            string SubViewTargetValue = string.Empty;
            string SubViewTargetTableAlias = string.Empty;
            string SubViewTargetFieldName = string.Empty;
            // AddIP 
            string AddIP_TableIPCode = string.Empty;
            string AddIP_Value = string.Empty;
                        
            string RefID_Target = string.Empty;
            string RefID_Second = string.Empty;
            string RefID_Third = string.Empty;
            string RefID_AddIP  = string.Empty;
            string RefID_SubView = string.Empty;

            string snc = string.Empty;

            // 1. Tablonun verileri
            TargetTableIPCode = t.MyProperties_Get(form_prp, "TargetTableIPCode:");
            TargetTableAlias = t.MyProperties_Get(form_prp, "TargetTableAlias:");
            TargetFieldName = t.MyProperties_Get(form_prp, "TargetFieldName:");
            /// sıfır ataması butona SubView özelliği eklenirken iptal edildi
            ///
            /// TargetValue = t.Set(t.MyProperties_Get(form_prp, "TargetValue:"), "", "0");
            TargetValue = t.Set(t.MyProperties_Get(form_prp, "TargetValue:"), "", "");
            TargetLikeValue = t.Set(t.MyProperties_Get(form_prp, "TargetLikeValue:"), "", "");
            // 2. Tablonun verileri
            SecondTableIPCode = t.MyProperties_Get(form_prp, "SecondTableIPCode:");
            SecondTableAlias = t.MyProperties_Get(form_prp, "SecondTableAlias:");
            SecondFieldName = t.MyProperties_Get(form_prp, "SecondFieldName:");
            SecondValue = t.Set(t.MyProperties_Get(form_prp, "SecondValue:"), "", "");
            ///SecondValue = t.Set(t.MyProperties_Get(form_prp, "SecondValue:"), "", "0");
            // 3. Tablonun verileri
            ThirdTableIPCode = t.MyProperties_Get(form_prp, "ThirdTableIPCode:");
            ThirdTableAlias = t.MyProperties_Get(form_prp, "ThirdTableAlias:");
            ThirdFieldName = t.MyProperties_Get(form_prp, "ThirdFieldName:");
            ThirdValue = t.Set(t.MyProperties_Get(form_prp, "ThirdValue:"), "", "");
            ///ThirdValue = t.Set(t.MyProperties_Get(form_prp, "ThirdValue:"), "", "0");

            // AddIP
            AddIP_TableIPCode = t.MyProperties_Get(form_prp, "AddIP_TableIPCode:");
            AddIP_Value = t.Set(t.MyProperties_Get(form_prp, "AddIP_Value:"), "", "0");
            
            // Read SubView için value
            SubViewTargetTableAlias = t.MyProperties_Get(form_prp, "SubViewTargetTableAlias:");
            SubViewTargetFieldName = t.MyProperties_Get(form_prp, "SubViewTargetFieldName:");
            SubViewTargetValue = t.Set(t.MyProperties_Get(form_prp, "SubViewTargetValue:"), "", "-1");

            #endregion Tanımlar

            #region // 1. Tablonun veriler set ediliyor
            if ((t.IsNotNull(TargetTableIPCode)) && (TargetTableIPCode == Main_TableIPCode))
            {
                if (t.IsNotNull(TargetFieldName) == false) TargetFieldName = Main_KeyFName;

                if ((t.IsNotNull(TargetValue)) && (TargetFieldName.IndexOf("{0}") == -1))
                {
                    //  and [xxx].REF_ID = xxx   veya  
                    //  and [xxx].REF_ID = -1    
                    snc = t.Set_FieldName_Value(dsFields, TargetFieldName, TargetValue, "and","=");
                    if (snc != string.Empty)
                        RefID_Target = " and " + Main_TableLabel + "." + snc + v.ENTER;
                    if (TargetTableAlias != string.Empty)
                        RefID_Target = " and " + TargetTableAlias + "." + snc + v.ENTER;
                }

                // KARIŞIK/parametreli ATAMALAR YAPILDIĞI ZAMANLAR
                // 
                if ((t.IsNotNull(TargetValue)) && (TargetFieldName.IndexOf("{0}") > -1))
                {
                    // TargetFieldName in içeriği
                    // AND ( [FNLNVOL].BHESAP_ID = {0} OR [FNLNVOL].AHESAP_ID = {0} )
                    RefID_Target = String.Format(TargetFieldName, TargetValue);
                }

                if (t.IsNotNull(TargetLikeValue))
                {
                    if (TargetLikeValue.ToUpper() != "FULL")
                    {
                        //  and [xxx].REF_NAME like xxx%   veya  
                        snc = t.Set_FieldName_Value(dsFields, TargetFieldName, TargetLikeValue, "andlike","=");
                        if (snc != string.Empty)
                            RefID_Target = " and " + Main_TableLabel + "." + snc + v.ENTER;
                        if (TargetTableAlias != string.Empty)
                            RefID_Target = " and " + TargetTableAlias + "." + snc + v.ENTER;
                    }
                    else
                    {
                        // üst tarafta dolan atamanın iptal edilmesi gerekiyor
                        // if (t.IsNotNull(TargetValue)) { ... }
                        RefID_Target = "";
                    }
                }

                if (t.IsNotNull(RefID_Target))
                {
                    if (TargetTableIPCode == Main_TableIPCode)
                    {
                        if (t.IsNotNull(TargetTableAlias))
                        {
                            NewSQL = t.SQLWhereAdd(NewSQL, TargetTableAlias, RefID_Target, "DEFAULT");
                        }
                        else
                        {
                            NewSQL = t.SQLWhereAdd(NewSQL, Main_TableLabel, RefID_Target, "DEFAULT");
                        }

                        if ((TargetValue == "0") ||
                            (TargetValue == "-1") ||
                            (TargetValue == "NEW")) Main_TargetValue = "NewRecord";
                    }
                }

            }
            #endregion // 1. Tablonun veriler set ediliyor

            #region // 2. Tablonun veriler set ediliyor
            if ((t.IsNotNull(SecondTableIPCode)) && (SecondTableIPCode == Main_TableIPCode))
            {
                if (t.IsNotNull(SecondFieldName) == false) SecondFieldName = Main_KeyFName;

                if ((t.IsNotNull(SecondValue)) && (SecondFieldName.IndexOf("{0}") == -1))
                {
                    //  and [xxx].REF_ID = xxx   veya  
                    //  and [xxx].REF_ID = -1    
                    snc = t.Set_FieldName_Value(dsFields, SecondFieldName, SecondValue, "and", "=");
                    if (snc != string.Empty)
                        RefID_Second = " and " + Main_TableLabel + "." + snc + v.ENTER;
                    if (SecondTableAlias != string.Empty)
                        RefID_Second = " and " + SecondTableAlias + "." + snc + v.ENTER;
                }

                // KARIŞIK/parametreli ATAMALAR YAPILDIĞI ZAMANLAR
                // 
                if ((t.IsNotNull(SecondValue)) && (SecondFieldName.IndexOf("{0}") > -1))
                {
                    // SecondFieldName in içeriği
                    // AND ( [FNLNVOL].BHESAP_ID = {0} OR [FNLNVOL].AHESAP_ID = {0} )
                    RefID_Target = String.Format(SecondFieldName, SecondValue);
                }

                if (t.IsNotNull(RefID_Second))
                {
                    if (SecondTableIPCode == Main_TableIPCode)
                    {
                        if (t.IsNotNull(SecondTableIPCode))
                        {
                            NewSQL = t.SQLWhereAdd(NewSQL, SecondTableAlias, RefID_Second, "DEFAULT");
                        }

                        if ((SecondValue == "0") ||
                            (SecondValue == "-1") ||
                            (SecondValue == "NEW")) Main_TargetValue = "NewRecord";
                    }
                }
            }
            #endregion // 2. Tablonun veriler set ediliyor

            #region // 3. Tablonun veriler set ediliyor
            if ((t.IsNotNull(ThirdTableIPCode)) && (ThirdTableIPCode == Main_TableIPCode))
            {
                if (t.IsNotNull(ThirdFieldName) == false) ThirdFieldName = Main_KeyFName;

                if (t.IsNotNull(ThirdValue))
                {
                    //  and [xxx].REF_ID = xxx   veya  
                    //  and [xxx].REF_ID = -1    
                    snc = t.Set_FieldName_Value(dsFields, ThirdFieldName, ThirdValue, "and", "=");
                    if (snc != string.Empty)
                        RefID_Third = " and " + Main_TableLabel + "." + snc + v.ENTER;
                    if (ThirdTableAlias != string.Empty)
                        RefID_Third = " and " + ThirdTableAlias + "." + snc + v.ENTER;
                }

                if (t.IsNotNull(RefID_Third))
                {
                    if (ThirdTableIPCode == Main_TableIPCode)
                    {
                        if (t.IsNotNull(ThirdTableIPCode))
                        {
                            NewSQL = t.SQLWhereAdd(NewSQL, ThirdTableAlias, RefID_Third, "DEFAULT");
                        }

                        if ((ThirdValue == "0") ||
                            (ThirdValue == "-1") ||
                            (ThirdValue == "NEW")) Main_TargetValue = "NewRecord";
                    }
                }
            }
            #endregion // 3. Tablonun veriler set ediliyor

            #region // AddIP Tablonun veriler set ediliyor
            if ((t.IsNotNull(AddIP_TableIPCode)) && (AddIP_TableIPCode == Main_TableIPCode))
            {
                if (t.IsNotNull(AddIP_Value))
                {
                    //  and [xxx].REF_ID = xxx   veya  
                    //  and [xxx].REF_ID = -1    
                    snc = t.Set_FieldName_Value(dsFields, Main_KeyFName, AddIP_Value, "and", "=");
                    if (snc != string.Empty)
                        RefID_AddIP = " and " + Main_TableLabel + "." + snc + v.ENTER;

                    if (t.IsNotNull(RefID_AddIP))
                    {
                        if (AddIP_TableIPCode == Main_TableIPCode)
                        {
                            if (t.IsNotNull(AddIP_TableIPCode))
                            {
                                NewSQL = t.SQLWhereAdd(NewSQL, Main_TableLabel, RefID_AddIP, "DEFAULT");
                            }

                            if ((AddIP_Value == "0") ||
                                (AddIP_Value == "-1") ||
                                (AddIP_Value == "NEW")) Main_TargetValue = "NewRecord";
                        }
                    }
                
                }

            }
            #endregion // AddIP Tablonun veriler set ediliyor
            
            #region Read_SubView ( kendisi bir subview olup READ olması isteniyor)
            if (t.IsNotNull(SubViewTargetValue)) //&& ( ???TableIPCode == Main_TableIPCode))
            {
                //  and [xxx].REF_ID = xxx   veya  
                //  and [xxx].REF_ID = -1    
                //snc = t.Set_FieldName_Value(dsFields, SubViewTargetFieldName, SubViewTargetValue, "and");
                
                //if (snc != string.Empty)
                //    RefID_SubView = " and " + Main_TableLabel + "." + snc + v.ENTER;
                //if (SubViewTargetTableAlias != string.Empty)
                //    RefID_SubView = " and " + SubViewTargetTableAlias + "." + snc + v.ENTER;

                if (SubViewTargetTableAlias == Main_TableLabel)
                {
                    //  and [xxx].REF_ID = xxx   veya  
                    //  and [xxx].REF_ID = -1    
                    snc = t.Set_FieldName_Value(dsFields, SubViewTargetFieldName, SubViewTargetValue, "and", "=");

                    if (snc != string.Empty)
                    {
                        RefID_SubView = " and " + SubViewTargetTableAlias + "." + snc + v.ENTER;

                        if (t.IsNotNull(RefID_SubView))
                        {
                            if (t.IsNotNull(SubViewTargetValue))
                            {
                                NewSQL = t.SQLWhereAdd(NewSQL, SubViewTargetTableAlias, RefID_SubView, "DEFAULT");

                                if (SubViewTargetValue == "0") Main_TargetValue = "NewRecord";
                            }
                        }
                    }
                }
            }
            #endregion Read_SubView

        }

        #endregion RefID Preparing
        
        #region Preparing_Join_Fields_Tables

        private void Preparing_Join_Fields_Tables(
                          Form tForm, 
                          DataRow row,
                          DataSet dsFields,
                          byte dBaseNo,
                          string Create_TableIPCode,
                          ref string joinTables, 
                          ref string joinFields,
                          ref string Lkp_Memory_Fields,
                          ref string About_Detail_SubDetail, 
                          ref string Where_Lines,
                          ref string declare,
                          ref string setlist,
                          ref string NewSQL,
                          ref string REF_CALL,
                          ref string Report_FieldsName
                          )
        {
            
            //string function_name = "Join_Fields_Tables_Preparing";
            
            tToolBox t = new tToolBox();

            #region Tanımlar
           
            Int16 field_type = 0;
            int RefId = 0;
            string s = string.Empty;
            string fieldname = string.Empty;
            string fcaption = string.Empty;
            string fname = string.Empty;
            string jtable = string.Empty;
            string jfield = string.Empty;
            string jwhere = string.Empty;
            string jlabel = string.Empty;

            byte default_type = 0;
            string mst_TableList = string.Empty;
            string mst_TableIPCode = string.Empty;
            //string mst_TableName = string.Empty;
            string mst_FName = string.Empty;
            string mst_CheckFName = string.Empty;
            string mst_CheckValue = string.Empty;

            // TABLE_TYPE  
            // 1, 'Table'            // 2, 'View'            // 3, 'Stored Procedure'
            // 4, 'Function'         // 5, 'Trigger'         // 6, 'Select' 
            
            byte Table_Type = t.Set(row["TABLE_TYPE"].ToString(), row["LKP_TABLE_TYPE"].ToString(), (byte)1);
            string TableCode = t.Set(row["TABLE_CODE"].ToString(), "", "");
            string IPCode = t.Set(row["IP_CODE"].ToString(), "", "");
            string TableIPCode = TableCode + "." + IPCode;
            string tLabel = "[" + t.Set(row["LKP_TABLE_CODE"].ToString(), "", "") + "]";

            byte Data_Read_Type = t.Set(row["DATA_READ"].ToString(), "", (byte)1); // 'DATA_READ_TYPE'
            /*
            DATA_READ_TYPE
             * 1, '', 'Not Read Data'
             * 2, '', 'Read RefID'
             * 3, '', 'Detail Table'
             * 4, '', 'SubDetail Table'
             * 5, '', 'Data Collection');
            */
            
            int ga = 0; // güzel a veya @
            int find = 0;
            string virgul = "";
            string MasterValue1 = string.Empty;
            string default_value = string.Empty;
            string tkrt_alias = string.Empty;
            string tkrt_table_alias = string.Empty;
            string fforeing = string.Empty;

            string prop_jointable = string.Empty;
            string J_TABLE = string.Empty;
            string J_WHERE = string.Empty;
            string J_STN_FIELDS = string.Empty;
            string J_CASE_FIELDS = string.Empty;

            Boolean tvisible = false;
            Boolean tLkpMemoryField = false;
            byte toperand_type = 0;

            #endregion Tanımlar

            #region join bağlantısı
            
            prop_jointable = t.Set(row["PROP_JOINTABLE"].ToString(), row["LKP_PROP_JOINTABLE"].ToString(), "");

            if (t.IsNotNull(prop_jointable))
            {
                string s1 = "=ROW_PROP_JOINTABLE:";
                string s2 = (char)34 + "J_TABLE" + (char)34 + ": [";

                /// old
                if (prop_jointable.IndexOf(s1) > -1)
                {
                    J_TABLE = t.Find_Properies_Get_FieldBlock(prop_jointable, "J_TABLE");
                    J_WHERE = t.Find_Properies_Get_FieldBlock(prop_jointable, "J_WHERE");
                    J_STN_FIELDS = t.Find_Properies_Get_FieldBlock(prop_jointable, "J_STN_FIELDS");
                    J_CASE_FIELDS = t.Find_Properies_Get_FieldBlock(prop_jointable, "J_CASE_FIELDS");

                    Preparing_JoinTable(J_TABLE, J_WHERE, J_STN_FIELDS, J_CASE_FIELDS,
                                        tLabel, ref joinTables, ref joinFields);
                }
                /// JSON
                if (prop_jointable.IndexOf(s2) > -1)
                {
                    Preparing_JoinTable_JSON(prop_jointable, tLabel, ref joinTables, ref joinFields);
                }
            }

            #endregion join bağlantısı



            // DİKKAT : Bu döngüde sadece join bağlantılar kurulmuyor !
            //          Hazır döngü var iken diğer field işlemleride yapılıyor
            //          Nasıl iyi fikir değil mi ?
            #region foreach
            foreach (DataRow Row in dsFields.Tables[0].Rows)
            {
                MasterValue1 = "";
                default_value = "";

                fieldname = t.Set(Row["LKP_FIELD_NAME"].ToString(), "", "");

                /// 'DEFAULT_TYPE'
                /// 21,  'Source TableIPCode READ'
                /// 31,  'Master=Detail'
                /// 32,  'Master=Detail Multi'
                /// 41,  'Line No'
                /// 51,  'Kriter READ (Even.Bas)' 
                /// 52,  'Kriter READ (Even.Bit)' 
                /// 53,  'Kriter READ (Odd)'

                default_type = t.Set(Row["DEFAULT_TYPE"].ToString(), Row["LKP_DEFAULT_TYPE"].ToString(), (byte)0);
                                
                ///  toperand_type
                ///  0,  '' yani =  
                ///  1,  'Even (Double)'
                ///  2,  'Odd  (Single)'
                ///  3,  'Speed'
                ///  4,  'On/Off'
                ///  9,  'Visible=False'
                /// 11,  '>='
                /// 12,  '>'
                /// 13,  '<='
                /// 14,  '<'
                /// 15,  '<>'
                /// 16,  'Benzerleri (%abc%)'
                /// 17,  'Benzerleri (abc%)'
                                
                toperand_type = t.Set(Row["KRT_OPERAND_TYPE"].ToString(), "", (byte)0);

                tLkpMemoryField = t.Set(Row["LKP_FMEMORY_FIELD"].ToString(), "", false);

                tvisible = t.Set(Row["CMP_VISIBLE"].ToString(), Row["LKP_FVISIBLE"].ToString(), true);
                
                if (tvisible)
                {
                    fcaption = t.Set(Row["FCAPTION"].ToString(), Row["LKP_FCAPTION"].ToString(), fieldname);

                    fforeing = t.Set(Row["LKP_FFOREING"].ToString(), "", ""); 
                    
                    //if (Report_FieldsName.Length == 0)
                    //     Report_FieldsName = Report_FieldsName + "   " + fieldname + "  [" + fcaption + "] " + v.ENTER;
                    //else Report_FieldsName = Report_FieldsName + " , " + fieldname + "  [" + fcaption + "] " + v.ENTER;
                    if (fforeing == "True")
                        Report_FieldsName = Report_FieldsName + " , " + fieldname + "  [" + fieldname + "] " + v.ENTER;

                    Report_FieldsName = Report_FieldsName + " , " + fieldname + "  [" + fcaption + "] " + v.ENTER;
                }

                if ((tLkpMemoryField) ||    // LKP_ MemoryField  ise
                    (toperand_type == 2) || // Odd Single 
                    (toperand_type == 3))   // SpeedKriter ise
                {
                    // Tespit edilen Default Type Göre -------------------
                    ///'DEFAULT_TYPE'
                    /// 1, '', 'Var (Integer)');
                    /// 2, '', 'Var (Numeric)');
                    /// 3, '', 'Var (Text)');
                    /// 4, '', 'Var (SP)');
                    /// 5, '', 'Var (Setup/Value)');
                    /// 6, '', 'Var (Setup/Caption)');

                    #region default_type > Var ( 1, 2, 3, 4 )
                    if ((default_type >= 1) &&
                        (default_type <= 5))
                    {
                        if (default_type == 1) // Var (Integer)
                        {
                            default_value = t.Set(Row["DEFAULT_INT"].ToString(), Row["LKP_DEFAULT_INT"].ToString(), "0");
                        }

                        if (default_type == 2) // Var (Numeric)
                        {
                            default_value = t.Set(Row["DEFAULT_NUMERIC"].ToString(), Row["LKP_DEFAULT_NUMERIC"].ToString(), "0");
                        }

                        if (default_type == 3) // Var (Text)
                        {
                            default_value = t.Set(Row["DEFAULT_TEXT"].ToString(), Row["LKP_DEFAULT_TEXT"].ToString(), "");
                        }

                        if (default_type == 4) // Var (Standart) SP_xxxxx
                        {
                            tDefaultValue dv = new tDefaultValue();
                            s = t.Set(Row["DEFAULT_SP"].ToString(), Row["LKP_DEFAULT_SP"].ToString(), "0");
                            if (s != "0")
                                default_value = dv.tSP_Value_Load(s);
                        }

                        if (default_type == 5) // Var SETUP_VALUE
                        {
                            tDefaultValue dv = new tDefaultValue();
                            s = t.Set(Row["DEFAULT_SETUP"].ToString(), Row["LKP_DEFAULT_SETUP"].ToString(), "0");
                            if (s != "0")
                                default_value = dv.tSETUP_Load(s, (byte)default_type);

                            if (toperand_type == 2)
                                MasterValue1 = default_value;
                        }
                        

                    }
                    #endregion Var
                }

                #region LkpMemoryField

                if (tLkpMemoryField)
                {
                    field_type = t.Set(Row["LKP_FIELD_TYPE"].ToString(), "", (Int16)0);
                    
                    Lkp_Memory_Fields = Lkp_Memory_Fields +
                        " , " + t.Set_FieldName_Value_(field_type, fieldname, default_value, "as", toperand_type.ToString()) + v.ENTER;
                }

                #endregion LkpMemoryField

                #region Detail-SubDetail ve Detail-SubDetail-Multi bağlantısı

                // bu satırların yerini değiştirme
                ga = -1;
                mst_FName = t.Set(Row["MASTER_KEY_FNAME"].ToString(), Row["LKP_MASTER_KEY_FNAME"].ToString(),"");
                if (mst_FName != "")
                {
                    ga = mst_FName.IndexOf("@");  // declare değişkeni mi?

                    if (ga > -1)
                    {
                        //mst_FName = mst_FName.Substring(1, mst_FName.Length - 1);
                    }
                }
                // ***

                /// 'DEFAULT_TYPE'
                /// 21,  'Source TableIPCode READ'
                /// 31,  'Master=Detail'
                /// 32,  'Master=Detail Multi'
                /// 41,  'Line No'
                /// 51,  'Kriter READ (Even.Bas)' 
                /// 52,  'Kriter READ (Even.Bit)' 
                /// 53,  'Kriter READ (Odd)'

                if ((default_type == 31) || 
                    (default_type == 32) || 
                    (default_type == 51) ||
                    (default_type == 52) ||
                    (default_type == 53) ||
                    (toperand_type == 2) || // odd single
                    (toperand_type == 3) || // SpeedKriter Double
                    (toperand_type == 4) || // SpeedKriter Single
                    (ga > -1))      
                {
                    // Master Table bağlantısı
 
                    #region Tanımlar

                    RefId = t.Set(Row["REF_ID"].ToString(), "", (int)0);
                    fname = t.Set(Row["KRT_CAPTION"].ToString(), Row["LKP_FIELD_NAME"].ToString(), "");
                    field_type = t.Set(Row["LKP_FIELD_TYPE"].ToString(), "", (Int16)0);
                    
                    // farklı bir where aliasına gitmesi gerekebilir 
                    tkrt_alias = t.Set(Row["KRT_ALIAS"].ToString(), tLabel, "");
                    // farklı aliası var ise esas table aliasına ulaşılıyor
                    tkrt_table_alias = t.Set(Row["KRT_TABLE_ALIAS"].ToString(), tLabel, "");
                                        
                    mst_TableIPCode = t.Set(Row["MASTER_TABLEIPCODE"].ToString(),Row["LKP_MASTER_TABLEIPCODE"].ToString(), "");
                    //src_TableIPCode = t.Set(Row["SEARCH_TABLEIPCODE"].ToString(), Row["LKP_SEARCH_TABLEIPCODE"].ToString(), "");
                    mst_CheckFName = t.Set(Row["MASTER_CHECK_FNAME"].ToString(), Row["LKP_MASTER_CHECK_FNAME"].ToString(), "");
                    mst_CheckValue = t.Set(Row["MASTER_CHECK_VALUE"].ToString(), Row["LKP_MASTER_CHECK_VALUE"].ToString(), "");

                    /// 3. SpeedKriter Double 
                    /// 4. SpeedKriter Single 
                    if ((toperand_type == 3) ||
                        (toperand_type == 4))
                    {
                        mst_TableIPCode = TableIPCode;
                        mst_CheckFName = fieldname;
                        mst_CheckValue = "";
                    }
                    
                    #endregion Tanımlar
                    
                    //  About_Detail_SubDetail:MasterTableIPCode||MasterTableName||MasterKeyFName||DetailFName;

                    #region About_Detail_SubDetail

                    /// 51,  'Kriter READ (Even.Bas)' 
                    /// 52,  'Kriter READ (Even.Bit)' 
                    /// 53,  'Kriter READ (Odd)'

                    if ((default_type != 51) &&
                        (default_type != 52) &&
                        (default_type != 53))
                    {
                        About_Detail_SubDetail =  About_Detail_SubDetail + 
                            "=Detail_SubDetail:" +
                            mst_TableIPCode + "||" +
                            mst_FName + "||" +
                            tkrt_table_alias + "." + fname + "||" +
                            field_type.ToString() + "||" +
                            default_type.ToString() + "||" +
                            toperand_type.ToString() + "||" +
                            mst_CheckFName + "||" +
                            mst_CheckValue + "||" +
                            RefId.ToString() + "|ds|" + v.ENTER;
                    }

                    if ((default_type == 51) ||
                        (default_type == 52) ||
                        (default_type == 53))
                    {
                        About_Detail_SubDetail = About_Detail_SubDetail + 
                            "=Detail_SubDetail:" +
                            mst_TableIPCode + "||" +
                            mst_FName + "||" +
                            //tkrt_table_alias + "." + fname + "||" +
                            fname + "||" +
                            field_type.ToString() + "||" +
                            default_type.ToString() + "||" +
                            toperand_type.ToString() + "||" +
                            mst_CheckFName + "||" +
                            mst_CheckValue + "||" +
                            RefId.ToString() + "|ds|" + v.ENTER;
                    }


                    #endregion About_Detail_SubDetail
                    
                    #region Where_Lines

                    //  and  a.GCB_ID = xxx  --Detail_SubDetail:MasterTableIPCode||MasterKeyFName;--

                    // 'DEFAULT_TYPE'
                    // 21,  'Source TableIPCode READ'
                    // 31,  'Master=Detail'
                    // 32,  'Master=Detail Multi'
                    // 41,  'Line No'
                    // 51,  'Kriter READ (Even.Bas)' 
                    // 52,  'Kriter READ (Even.Bit)' 
                    // 53,  'Kriter READ (Odd)'

                    // declare field değil ise, @ yok ise
                    if (
                        ((default_type == 31) ||   // Master-Detail       >> ID = x
                         (default_type == 32) ||   // master-Detail Multi >> ID in ( .... )  
                         (default_type == 51) ||
                         (default_type == 52) ||
                         (default_type == 53) ||
                         (toperand_type == 3) ||   // SpeedKriter Double
                         (toperand_type == 4) ||   // SpeedKriter Single
                        ((toperand_type == 2) && (Table_Type == 3)) // odd single and stored procedre 
                        ) &&
                        //(ga < 0) &&   Table_Type = 3 için ve Where_Lines oluşmuyor 
                        (Data_Read_Type != 2) &&   // RefId        değil ise 
                        (Data_Read_Type != 6)      // Read SubView değil ise 
                        )
                    {
                        #region SpeedKriter 

                        // Odd Single
                        // Stored Procedure hazırlanırken
                        // eğer kritername yoksa bu parametre atlaması için
                        // 
                        if ((toperand_type == 2) && (Table_Type == 3))
                        {
                            fname = t.Set(Row["KRT_CAPTION"].ToString(), "", "");
                        }

                        // Double
                        if (toperand_type == 3)
                        {
                            Where_Lines =
                                    Where_Lines +
                                    " and " + t.Set_FieldName_Value_(field_type, tkrt_table_alias + "." + fname, default_value, "and", ">=") +
                                    "   -- :D.SD." + RefId.ToString() + ": -->=" + v.ENTER +
                                    " and " + t.Set_FieldName_Value_(field_type, tkrt_table_alias + "." + fname, default_value, "and", "<=") +
                                    "   -- :D.SD." + RefId.ToString() + ": --<=" + v.ENTER;

                        }
                        // Single
                        if (toperand_type == 4)
                        {
                            Where_Lines =
                                    Where_Lines +
                                    " and " + t.Set_FieldName_Value_(field_type, tkrt_table_alias + "." + fname, default_value, "and", "=") +
                                    "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                        }
                        #endregion SpeedKriter

                        #region (default_type == 31) || (default_type == 51,52,53)
                        // önce bilgiyi oku
                        if (default_type == 31)
                        {
                            // master bilgiye ulaşırsa (dsData) direk bu bağlantı sağlansın
                            MasterValue1 = t.Find_TableIPCode_Value(tForm, mst_TableIPCode, mst_FName);
                        }
                        
                        if ((default_type == 51) ||
                            (default_type == 52) ||
                            (default_type == 53))
                        {
                            // kriterler nesnesine ulaşırsa (VGrid KRITER_TableIPCode) direk buradan bilgiyi alsın
                            MasterValue1 = t.Find_Kriter_Value(tForm, mst_TableIPCode, mst_FName, default_type);
                        }
                        
                        // aldığın bilgiyi set et
                        if ((toperand_type == 2) ||  // odd single  
                            (default_type == 31) ||
                            (default_type == 51) ||
                            (default_type == 52) ||
                            (default_type == 53))
                        {
                            #region // Table ise
                            if (Table_Type == 1)
                            {
                                if (MasterValue1 == "")
                                {
                                    Where_Lines =
                                        Where_Lines +
                                        " and " + t.Set_FieldName_Value_(field_type, tkrt_table_alias + "." + fname, "first", "and", toperand_type.ToString()) +
                                        "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                                }
                                
                                if (MasterValue1 != "")
                                {
                                    Where_Lines =
                                        Where_Lines +
                                        " and " + t.Set_FieldName_Value_(field_type, tkrt_table_alias + "." + fname, MasterValue1, "and", toperand_type.ToString()) +
                                        "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                                }
                            
                            }
                            #endregion

                            #region // Stored Procedure
                            if ((Table_Type == 3) && t.IsNotNull(fname))
                            {
                                #region ** eski hali
                                /*
                                 ** eski hali                                

                                 EXEC [dbo].[prc_STK_FIYAT_GET]
                                  /*prm* / @URUN_ID = 2-- :D.SD.6596: --
                                , /*prm* / @BIRIM_NO = 2-- :D.SD.6597: --
                                , /*prm* / @GECERLI_BAS_TARIH = Convert(Date, '13.04.2018', 103)-- :D.SD.6606: --
                                , /*prm* / @GECERLI_BAS_SAAT = '16:36'-- :D.SD.6607: --

                                ** yeni hali

                                DECLARE @_URUN_ID int
                                DECLARE @_BIRIM_NO smallint
                                DECLARE @_ISLEM_TARIHI date
                                DECLARE @_ISLEM_SAAT varchar(8)

                                set @_URUN_ID = 2
                                set @_BIRIM_NO = 2
                                set @_ISLEM_TARIHI = CONVERT(date, '13.04.2018',104)
                                set @_ISLEM_SAAT = '16:05'

                                EXECUTE [dbo].[prc_STK_FIYAT_GET] 
                                   @_URUN_ID
                                  ,@_BIRIM_NO
                                  ,@_ISLEM_TARIHI
                                  ,@_ISLEM_SAAT
  
                                */
                                #endregion

                                if (Where_Lines.Length == 0)
                                    Where_Lines = "" +
                                          "/*declareEnd*/" + v.ENTER +
                                          "/*setEnd*/" + v.ENTER +
                                          "/*executeBegin*/" + //v.ENTER +
                                          "/*paramEnd*/" + v.ENTER;

                                /// virgul ilk geldiğinde ="" olduğu için önce boşluk oluyor  ( virgul = "   " )
                                /// birinci döngüden sonra virgul gerçekten virgül değerine dönüşüyor ( virgul = " , " )  
                                /// aşağıdak, sıralamayı değiştirme
                                /// 
                                if (virgul == "   ") virgul = " , ";
                                if (virgul == "") virgul = "   ";
                                

                                //if ((dBaseNo == 4) && (v.dBTypes.ProjectDBType == v.dBaseType.MSSQL))
                                if (v.active_DB.projectDBType == v.dBaseType.MSSQL)
                                {
                                    if ((MasterValue1 == "") && (default_value == ""))
                                    {
                                        //Where_Lines =
                                        //    Where_Lines +
                                        //    "/*prm*/ " + t.Set_FieldName_Value_(field_type, "@" + fname, "first", "@", toperand_type.ToString()) +
                                        //    "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                                        find = Where_Lines.IndexOf("/*declareEnd*/");
                                        Where_Lines = Where_Lines.Insert(find,
                                            "declare @" + fname + " " + t.Get_FieldTypeName(Convert.ToInt16(field_type), "800") + v.ENTER);

                                        find = Where_Lines.IndexOf("/*setEnd*/");
                                        Where_Lines = Where_Lines.Insert(find,
                                            "/*prm*/ set " + t.Set_FieldName_Value_(field_type, "@" + fname, "first", "@", toperand_type.ToString()) +
                                            "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER);

                                        find = Where_Lines.IndexOf("/*paramEnd*/");
                                        Where_Lines = Where_Lines.Insert(find, virgul + " @" + fname + v.ENTER);
                                    }

                                    /// default value varsa onu set edelim
                                    /// 
                                    if ((MasterValue1 == "") && (default_value != ""))
                                        MasterValue1 = default_value;

                                    if (MasterValue1 != "")
                                    {
                                        //Where_Lines =
                                        //    Where_Lines +
                                        //    "/*prm*/ " + t.Set_FieldName_Value_(field_type, "@" + fname, MasterValue1, "@", toperand_type.ToString()) +
                                        //    "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;

                                        find = Where_Lines.IndexOf("/*declareEnd*/");
                                        Where_Lines = Where_Lines.Insert(find,
                                            "declare @" + fname + " " + t.Get_FieldTypeName(Convert.ToInt16(field_type), "800") + v.ENTER);

                                        find = Where_Lines.IndexOf("/*setEnd*/");
                                        Where_Lines = Where_Lines.Insert(find,
                                            "/*prm*/ set " + t.Set_FieldName_Value_(field_type, "@" + fname, MasterValue1, "@", toperand_type.ToString()) +
                                            "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER);

                                        find = Where_Lines.IndexOf("/*paramEnd*/");
                                        Where_Lines = Where_Lines.Insert(find, virgul + " @" + fname + v.ENTER);
                                    }

                                }

                                //if ((dBaseNo == 4) && (v.dBTypes.ProjectDBType == v.dBaseType.MySQL))
                                if (v.active_DB.projectDBType == v.dBaseType.MySQL)
                                {
                                    if (MasterValue1 == "")
                                    {
                                        Where_Lines =
                                            Where_Lines +
                                            "/*prm.@" + fname + "*/ " + t.Set_FieldName_Value_(field_type, "", "first", "@", "null") +
                                            "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                                    }

                                    if (MasterValue1 != "")
                                    {
                                        Where_Lines =
                                            Where_Lines +
                                            "/*prm.@" + fname + "*/ " + t.Set_FieldName_Value_(field_type, "", MasterValue1, "@", "null") +
                                            "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                                    }
                                }
                            }
                            #endregion // sp
                            
                            #region // Select ise
                            if (Table_Type == 6)
                            {
                                // burada tek tek sql in içine set ediliyor
                                // her bir bağlantı farklı ailas olabilir
                                //Where_Lines =
                                if (MasterValue1 == "")
                                {
                                    s = " and " + t.Set_FieldName_Value_(field_type, tkrt_table_alias + "." + fname, "first", "and", toperand_type.ToString()) +
                                    "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                                }
                                if (MasterValue1 != "")
                                {
                                    s = " and " + t.Set_FieldName_Value_(field_type, tkrt_table_alias + "." + fname, MasterValue1, "and", toperand_type.ToString()) +
                                    "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                                }

                                NewSQL = t.SQLWhereAdd(NewSQL, tkrt_alias, s, "DEFAULT");

                                s = string.Empty;
                            }
                            #endregion 
                        } // 31
                        #endregion (31) || (51,52,53)
                        
                        #region (default_type == 32)
                        if (default_type == 32)
                        {

                            Where_Lines =
                                    Where_Lines +
                                    " and " + t.Set_FieldName_Value_(field_type, tkrt_table_alias + "." + fname, "first", "in", toperand_type.ToString()) +
                                    "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                            /*
                             and [K].GRUP_ID in (   -- :D.SD.2774: --
                             / *[K].GRUP_ID_INLIST* /
                               2990
                             , 2983
                             / *[K].GRUP_ID_INEND* /
                             )
                            */

                        } // 32
                        #endregion (default_type == 32)
                                                
                    }

                    #endregion Where_Lines
    
                    // SubDetail_TableIPCode:TableIPCode;
                    
                    #region tDataNavigator.IsAccessible = true

                    // Master-Detail ilişki sırasında bazen Tablonun kendisine 
                    // join le bağlı olan tablosundan LKP_XXXX li bir fieldin value değeri gerekebiliyor
                    // bu bilgiler Default_Value de Master-Detail sırasında lağzım olabiliyor
                    // ama burada bu ilgiyi kullanamamız gerekiyor
                    if (TableIPCode == mst_TableIPCode)
                    {
                        mst_TableList = mst_TableList + mst_TableIPCode + v.ENTER;
                    }
                    
                    if (mst_TableList.IndexOf(mst_TableIPCode) == -1)
                    {
                        // MasterTable/DetailTable tespit edilecek ve üzerine 
                        // TableIPCode/SubDetailTable (lar) listesi yazılacak
                    
                        mst_TableList = mst_TableList + mst_TableIPCode + v.ENTER;

                        // Master Table tespit ediliyor 
                        DataNavigator tDataNavigator = null;
                        tDataNavigator = t.Find_DataNavigator(tForm, mst_TableIPCode);//, function_name);// + "/" + Create_TableIPCode);

                        // Master/Detail Table Tespit edildiyse
                        if (tDataNavigator != null)
                        {
                            // gerekli bilgiler toplandıysa  
                            // MasterTable/DetailTable ın üzerine kendisine bağlı olan SubDetail_TableIPCode nin bilgiler işleniyor
                            s = "";
                            t.MyProperties_Set(ref s, "SubDetail_TableIPCode", TableIPCode);
                            if (tDataNavigator.Text.IndexOf(TableIPCode) == -1)
                            {
                                tDataNavigator.IsAccessible = true;
                                tDataNavigator.Text = tDataNavigator.Text + s;
                                
                                //object tDataTable = tDataNavigator.DataSource;
                                //DataSet tdsData = ((DataTable)tDataTable).DataSet;
                            }
                        }
                    } // if (mst_TableList.IndexOf(mst_TableIPCode) == -1)

                    #endregion tDataNavigator.IsAccessible = true

                    // declare ve set  parametreleri kullanılıyor ise

                    #region Declare & Set
                                        
                    if (ga > -1) // @ var ise
                    {
                        string mst_FName2 = mst_FName.Substring(1, mst_FName.Length - 1);
                        
                        MasterValue1 = "";
                        default_value = t.Set(Row["DEFAULT_TEXT"].ToString(), Row["LKP_DEFAULT_TEXT"].ToString(), "");

                        if (default_value == "")
                        {
                            // master bilgiye ulaşırsa direk bu bağlantı sağlansın
                            MasterValue1 = t.Find_TableIPCode_Value(tForm, mst_TableIPCode, mst_FName2);
                        }

                        declare = declare +  
                            "declare @" + fname + " " + t.Get_FieldTypeName(Convert.ToInt16(field_type), "800") + v.ENTER;

                        if (MasterValue1 == "")
                        {
                            setlist = setlist +
                                "set " + t.Set_FieldName_Value_(field_type, fname, default_value, "@", toperand_type.ToString())
                                + "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                        }
                        else
                        {
                            setlist = setlist +
                                "set " + t.Set_FieldName_Value_(field_type, fname, MasterValue1, "@", toperand_type.ToString())
                                + "   -- :D.SD." + RefId.ToString() + ": --" + v.ENTER;
                        }
                    }

                    #endregion Declare & Set
                }

                #endregion Detail-SubDetail bağlantısı
                
                #region // REF_CALL field varmı ?
                if (fieldname == "REF_CALL")
                {
                    REF_CALL = " and " + tLabel + "." + fieldname + " = "
                        + t.Set(Row["DEFAULT_INT"].ToString(), Row["LKP_DEFAULT_INT"].ToString(), "0")
                        + v.ENTER;
                }
                #endregion 
                        
            }
            #endregion foreach

            if (Report_FieldsName.Length > 3) 
            {
                // en baştaki virgülü temizle
                Report_FieldsName = Report_FieldsName.Remove(0, 3);
                Report_FieldsName = "   " + Report_FieldsName;
            }  
         
            #region About_Detail_SubDetail
            
            if (t.IsNotNull(About_Detail_SubDetail))
                About_Detail_SubDetail =
                    "About_Detail_SubDetail:" + v.ENTER + 
                     About_Detail_SubDetail + ";" + v.ENTER;

            #endregion About_Detail_SubDetail

        }
                
        private void Preparing_JoinTable_JSON(
                     string Prop_JoinTable, 
                     string MasterTableAlias,
                     ref string joinTables,
                     ref string joinFields)
        {
            PROP_JOINTABLE packet = new PROP_JOINTABLE();
            Prop_JoinTable = Prop_JoinTable.Replace((char)34, (char)39);
            var prop_ = JsonConvert.DeserializeAnonymousType(Prop_JoinTable, packet);
            
            string j_format = string.Empty;
            string j_type = string.Empty;
            string j_table_name = string.Empty;
            string j_table_alias = string.Empty;
            string join = string.Empty;

            foreach (var item in prop_.J_TABLE)
            {
                j_format = item.J_FORMAT.ToString();
                j_type = item.J_TYPE.ToString();
                j_table_name = item.J_TABLE_NAME.ToString();
                j_table_alias = item.J_TABLE_ALIAS.ToString();

                if (t.IsNotNull(j_table_name))
                {
                    if (joinTables.IndexOf(j_table_name + " " + j_table_alias) == -1)
                    {
                        if (j_type == "LEFT") join = "   left outer join ";
                        if (j_type == "RIGHT") join = "   right outer join ";
                        if (j_type == "INNER") join = "   inner join ";

                        // J_TABLE + J_WHERE
                        joinTables = joinTables +
                            join + j_table_name + " [" + j_table_alias + "] on ( " +
                              Preparing_JoinWhere_JSON(prop_.J_WHERE, j_table_alias, MasterTableAlias) + " ) " + v.ENTER;

                        // J_STN_FIELDS
                        if (j_format == "STANDART")
                            Preparing_Select_Standart_Fields_JSON(prop_.J_STN_FIELDS, ref joinFields, j_table_alias);
                    }
                }

            }

            //J_CASE_FIELDS    (j_format == "CASE")
            Preparing_Select_Case_Fields_JSON(prop_.J_CASE_FIELDS, ref joinFields, j_table_alias, MasterTableAlias);

        }

        private void Preparing_JoinTable(
                     string J_TABLE, string J_WHERE, 
                     string J_STN_FIELDS, string J_CASE_FIELDS, 
                     string MasterTableAlias, 
                     ref string joinTables,
                     ref string joinFields )
        {
            tToolBox t = new tToolBox();

            string j_format = string.Empty;
            string j_type = string.Empty;
            string j_table_name = string.Empty;
            string j_table_alias = string.Empty;
            string join = string.Empty;
            string row_block = string.Empty;
            string lockE = string.Empty;
            
            // J_TABLE
            //lockE = "=ROWE_" + "J_TABLE" + ":"; 

            lockE = "=ROWE_";
            while (J_TABLE.IndexOf(lockE) > -1)
            {
                row_block = t.Find_Properies_Get_RowBlock(ref J_TABLE, "J_TABLE");

                j_format = t.MyProperties_Get(row_block, "J_FORMAT:");
                j_type = t.MyProperties_Get(row_block, "J_TYPE:");
                j_table_name = t.MyProperties_Get(row_block, "J_TABLE_NAME:");
                j_table_alias = t.MyProperties_Get(row_block, "J_TABLE_ALIAS:");

                if (t.IsNotNull(j_table_name))
                {
                    if (joinTables.IndexOf(j_table_name + " " + j_table_alias) == -1)
                    {
                        if (j_type == "LEFT")  join = "   left outer join ";
                        if (j_type == "RIGHT") join = "   right outer join ";
                        if (j_type == "INNER") join = "   inner join ";

                        // J_TABLE + J_WHERE
                        joinTables = joinTables +
                            join + j_table_name + " " + j_table_alias + " on ( " +
                              Preparing_JoinWhere(J_WHERE, j_table_alias, MasterTableAlias) + " ) " + v.ENTER;

                        // J_STN_FIELDS
                        if (j_format == "STANDART")
                            Preparing_Select_Standart_Fields(J_STN_FIELDS, ref joinFields, j_table_alias);
                    }
                }           
            }

            //J_CASE_FIELDS    (j_format == "CASE")
            Preparing_Select_Case_Fields(J_CASE_FIELDS, ref joinFields, j_table_alias, MasterTableAlias);

            #region J_TABLE
            //PROP_JOINTABLE={
            //0=ROW_PROP_JOINTABLE:0;           <<<< buradan 
            //0=J_TABLE:J_TABLE={
            //1=ROW_J_TABLE:1;
            //1=CAPTION:MS_FIELDS;
            //1=J_FORMAT:STANDART;
            //1=J_TYPE:LEFT;
            //1=J_TABLE_NAME:MS_FIELDS;
            //1=J_TABLE_ALIAS:B;
            //1=ROWE_J_TABLE:1;
            //J_TABLE=};                        <<<< arası isteniyor
            #endregion J_TABLE
        }

        private string Preparing_JoinWhere(string J_WHERE, string j_table_alias, string MasterTableAlias)
        {
            //tToolBox t = new tToolBox();
            string s = string.Empty;
            string j_alias = string.Empty;
            string m_alias = string.Empty;
            string mst_alias = string.Empty;
            string mst_fname = string.Empty;
            string j_fname = string.Empty;
            string fvalue = string.Empty;
            string row_block = string.Empty;
            string lockE = string.Empty;

            lockE = "=ROWE_"; // +"J_WHERE" + ":"; 

            #region J_WHERE
            while (J_WHERE.IndexOf(lockE) > -1)
            {
                //row_block = t.Get_And_Clear(ref J_WHERE, lockE);
                row_block = t.Find_Properies_Get_RowBlock(ref J_WHERE, "J_WHERE");

                j_alias = t.MyProperties_Get(row_block, "J_TABLE_ALIAS:");
                
                if (j_alias == j_table_alias)
                {
                    mst_alias = t.MyProperties_Get(row_block, "M_TABLE_ALIAS:");
                    mst_fname = t.MyProperties_Get(row_block, "MT_FNAME:");
                    j_fname = t.MyProperties_Get(row_block, "J_FNAME:");
                    fvalue = t.MyProperties_Get(row_block, "FVALUE:");

                    m_alias = t.Set(mst_alias, MasterTableAlias, "");


                    if (s == "")
                    {
                        // a.mst_FieldName = b.FieldName
                        if (t.IsNotNull(mst_fname) &&
                           (t.IsNotNull(j_fname)) &&
                           (t.IsNotNull(fvalue) == false))
                            s = m_alias + "." + mst_fname + " = " + j_table_alias + "." + j_fname + " ";

                        // a.mst_FieldName = value
                        if (t.IsNotNull(mst_fname) &&
                           (t.IsNotNull(j_fname) == false) &&
                           (t.IsNotNull(fvalue)))
                            s = m_alias + "." + mst_fname + " = " + fvalue + " ";

                        // b.join_FieldName = value
                        if ((t.IsNotNull(mst_fname) == false) &&
                            (t.IsNotNull(j_fname)) &&
                            (t.IsNotNull(fvalue)))
                            s = j_table_alias + "." + j_fname + " = " + fvalue + " ";

                        // eğer üç değerde dolu ise value olduğu gibi eklenecek
                        if ((t.IsNotNull(mst_fname)) &&
                            (t.IsNotNull(j_fname)) &&
                            (t.IsNotNull(fvalue)))
                            s = s + " and  " + fvalue + " ";
                    }
                    else
                    {
                        // and a.mst_FieldName = b.join_FieldName
                        if (t.IsNotNull(mst_fname) &&
                           (t.IsNotNull(j_fname)) &&
                           (t.IsNotNull(fvalue) == false))
                            s = s + v.ENTER + " and  " + m_alias + "." + mst_fname + " = " + j_table_alias + "." + j_fname + " ";
                        // and a.mst_FieldName = value
                        if (t.IsNotNull(mst_fname) &&
                           (t.IsNotNull(j_fname) == false) &&
                           (t.IsNotNull(fvalue)))
                            s = s + v.ENTER + " and  " + m_alias + "." + mst_fname + " = " + fvalue + " ";
                        // and b.join_FieldName = value
                        if ((t.IsNotNull(mst_fname) == false) &&
                            (t.IsNotNull(j_fname)) &&
                            (t.IsNotNull(fvalue)))
                            s = s + v.ENTER + " and  " + j_table_alias + "." + j_fname + " = " + fvalue + " ";
                        // eğer üç değerde dolu ise value olduğu gibi eklenecek
                        if ((t.IsNotNull(mst_fname)) &&
                            (t.IsNotNull(j_fname)) &&
                            (t.IsNotNull(fvalue)))
                            s = s + " and  " + fvalue + " ";
                    }
                }
            }
            #endregion J_WHERE

            return s;

            #region J_WHERE örnek
            /*
            0=J_WHERE:J_WHERE={       <<<<<  field block
            1=ROW_J_WHERE:1;          <<<<<  row block   
            1=CAPTION:MS_FIELDS.TABLE_CODE;
            1=J_TABLE_ALIAS:B;
            1=MT_FNAME:TABLE_CODE;
            1=J_FNAME:TABLE_CODE;
            1=FVALUE:null;
            1=ROWE_J_WHERE:1;
            2=ROW_J_WHERE:2;          <<<<<  row block
            2=CAPTION:MS_FIELDS.FIELD_NO;
            2=J_TABLE_ALIAS:B;
            2=MT_FNAME:FIELD_NO;
            2=J_FNAME:FIELD_NO;
            2=FVALUE:null;
            2=ROWE_J_WHERE:2;
            J_WHERE=};
            */
            #endregion J_WHERE

        }

        private void Preparing_Select_Standart_Fields(string J_STN_FIELDS, ref string joinFields, string j_table_alias)
        {
            //tToolBox t = new tToolBox();
            
            string j_alias = string.Empty;
            string j_fname = string.Empty;
            string fnl_fname = string.Empty;
            string row_block = string.Empty;
            string lockE = string.Empty;

            lockE = "=ROWE_";
            
            // birden fazla row var ise
            while (J_STN_FIELDS.IndexOf(lockE) > -1)
            {
                row_block = t.Find_Properies_Get_RowBlock(ref J_STN_FIELDS, "J_STN_FIELDS");

                j_alias = t.MyProperties_Get(row_block, "J_TABLE_ALIAS:");
                if (j_alias == j_table_alias)
                {
                    j_fname = t.MyProperties_Get(row_block, "J_FNAME:");
                    fnl_fname = t.MyProperties_Get(row_block, "FNL_FNAME:");
                    joinFields = joinFields + " , " + j_alias + "." + j_fname.PadRight(35) + " " + fnl_fname + v.ENTER;
                }
            }

            #region J_STN_FIELDS
            /*
            0=J_STN_FIELDS:J_STN_FIELDS={      <<<< field block
            1=ROW_J_STN_FIELDS:1;              <<<< row block 
            1=CAPTION:FIELD NAME;
            1=J_TABLE_ALIAS:B;
            1=J_FNAME:FIELD_NAME;
            1=FNL_FNAME:LKP_FIELD_NAME;
            1=ROWE_J_STN_FIELDS:1;
             *
            2=ROW_J_STN_FIELDS:2;              <<<< row block
            2=CAPTION:FCAPTION;
            2=J_TABLE_ALIAS:B;
            2=J_FNAME:FCAPTION;
            2=FNL_FNAME:LKP_FCAPTION;
            2=ROWE_J_STN_FIELDS:2;
            J_STN_FIELDS=};
            */
            #endregion J_STN_FIELDS
        }

        private void Preparing_Select_Case_Fields(string J_CASE_FIELDS, ref string joinFields, string j_table_alias, string MasterTableAlias)
        {

            //tToolBox t = new tToolBox();
            string s = string.Empty;
            string case_for_fname = string.Empty;
            string where_value = string.Empty;
            string then_alias = string.Empty;
            string then_fname = string.Empty;
            string final_fname = string.Empty;
            string row_block = string.Empty;
            string lockE = string.Empty;

            string satir = string.Empty;
            string satir_sonu = string.Empty;
            string nsatir = string.Empty;           
            int i1 = 0;

            lockE = "=ROWE_"; // +"J_CASE_FIELDS" + ":"; 

            while (J_CASE_FIELDS.IndexOf(lockE) > -1)
            {
                //blok = t.Get_And_Clear(ref J_CASE_FIELDS, lockE);
                row_block = t.Find_Properies_Get_RowBlock(ref J_CASE_FIELDS, "J_CASE_FIELDS");

                case_for_fname = t.MyProperties_Get(row_block, "CASE_FOR_FNAME:");
                where_value = t.MyProperties_Get(row_block, "WHERE_VALUE:");
                then_alias = t.MyProperties_Get(row_block, "THEN_ALIAS:");
                then_fname = t.MyProperties_Get(row_block, "THEN_FNAME:");
                final_fname = t.MyProperties_Get(row_block, "FINAL_FNAME:");

                satir_sonu = "  else '' end ) " + final_fname + "  ";
                
                if (nsatir.IndexOf(final_fname) == -1)
                {
                    if ((MasterTableAlias != "") && (case_for_fname != ""))
                    {
                        satir = " , ( case " + MasterTableAlias + "." + case_for_fname + satir_sonu + v.ENTER;
                        nsatir = nsatir + satir;
                    }
                }

                if ((where_value != "") && (then_alias != "") && (then_fname != ""))
                    satir = " when " + where_value + " then " + then_alias + "." + then_fname;

                if (nsatir.IndexOf(satir) == -1)
                {
                    i1 = nsatir.IndexOf(satir_sonu);
                    nsatir = nsatir.Insert(i1, satir);
                }
            }

            if (nsatir.Length > 0)
                joinFields = joinFields + nsatir;

            #region J_CASE_FIELDS
            /*
             , ( CASE A.TABLO_ID 
                WHERE 10 THEN B.KASA_KODU
                WHERE 20 THEN C.BANKA_KODU
                ELSE 'None' END )  LKP_HESAP_KODU

            , ( CASE A.TABLO_ID 
                WHERE 10 THEN B.KASA_ADI
                WHERE 20 THEN C.BANKA_ADI
                ELSE 'None' END )  LKP_HESAP_ADI
             *  
             * 
             * 
            1=CAPTION:Kasa Kodu;
            1=CASE_FOR_FNAME:TABLO_ID;
            1=WHERE_VALUE:10;
            1=THEN_ALIAS:B;
            1=THEN_FNAME:KASA_KODU;
            1=FINAL_FNAME:LKP_HESAP_KODU;
             * 
             *
            2=CAPTION:Kasa Adı;
            2=CASE_FOR_FNAME:TABLO_ID;
            2=WHERE_VALUE:10;
            2=THEN_ALIAS:B;
            2=THEN_FNAME:KASA_ADI;
            2=FINAL_FNAME:LKP_HESAP_ADI;
            
             */
            #endregion J_CASE_FIELDS
        }


        private string Preparing_JoinWhere_JSON(List<J_WHERE> J_WHERE, 
            string j_table_alias, string MasterTableAlias)
        {
            string s = string.Empty;
            string m_alias = string.Empty;
            string mst_alias = string.Empty;
            string mst_fname = string.Empty;
            string j_fname = string.Empty;
            string fvalue = string.Empty;
                        
            #region J_WHERE
            foreach (var item in J_WHERE)
            {
                if ((item.J_TABLE_ALIAS == j_table_alias) ||
                    ("["+ item.J_TABLE_ALIAS +"]" == j_table_alias))
                {
                    mst_alias = item.M_TABLE_ALIAS.ToString();
                    mst_fname = item.MT_FNAME.ToString();
                    j_fname = item.J_FNAME.ToString();
                    fvalue = item.FVALUE.ToString();

                    m_alias = t.Set(mst_alias, MasterTableAlias, "");

                    if (m_alias.IndexOf("[") == -1)
                        m_alias = "[" + m_alias + "]";
                    if (j_table_alias.IndexOf("[") == -1)
                        j_table_alias = "[" + j_table_alias + "]";

                    if (s == "")
                    {
                        // a.mst_FieldName = b.FieldName
                        if (t.IsNotNull(mst_fname) &&
                           (t.IsNotNull(j_fname)) &&
                           (t.IsNotNull(fvalue) == false))
                            s = m_alias + "." + mst_fname + " = " + j_table_alias + "." + j_fname + " ";

                        // a.mst_FieldName = value
                        if (t.IsNotNull(mst_fname) &&
                           (t.IsNotNull(j_fname) == false) &&
                           (t.IsNotNull(fvalue)))
                            s = m_alias + "." + mst_fname + " = " + fvalue + " ";

                        // b.join_FieldName = value
                        if ((t.IsNotNull(mst_fname) == false) &&
                            (t.IsNotNull(j_fname)) &&
                            (t.IsNotNull(fvalue)))
                        {
                            try
                            {
                                /// burada value nin tipini bilmiyoruz
                                /// eğer bu çevrim hata vermezse int değerindedir
                                var x = Convert.ToInt64(fvalue);

                                s = j_table_alias + "." + j_fname + " = " + fvalue + " ";
                            }
                            catch (Exception)
                            {
                                /// eğer hata verdiyse string kabul ediyoruz 
                                s = j_table_alias + "." + j_fname + " = '" + fvalue + "' ";
                            }

                        }
                        // eğer üç değerde dolu ise value olduğu gibi eklenecek
                        if ((t.IsNotNull(mst_fname)) &&
                            (t.IsNotNull(j_fname)) &&
                            (t.IsNotNull(fvalue)))
                            s = s + " and  " + fvalue + " ";
                    }
                    else
                    {
                        // and a.mst_FieldName = b.join_FieldName
                        if (t.IsNotNull(mst_fname) &&
                           (t.IsNotNull(j_fname)) &&
                           (t.IsNotNull(fvalue) == false))
                            s = s + v.ENTER + " and  " + m_alias + "." + mst_fname + " = " + j_table_alias + "." + j_fname + " ";
                        // and a.mst_FieldName = value
                        if (t.IsNotNull(mst_fname) &&
                           (t.IsNotNull(j_fname) == false) &&
                           (t.IsNotNull(fvalue)))
                            s = s + v.ENTER + " and  " + m_alias + "." + mst_fname + " = " + fvalue + " ";
                        // and b.join_FieldName = value
                        if ((t.IsNotNull(mst_fname) == false) &&
                            (t.IsNotNull(j_fname)) &&
                            (t.IsNotNull(fvalue)))
                        //s = s + v.ENTER + " and  " + j_table_alias + "." + j_fname + " = " + fvalue + " ";
                        {
                            try
                            {
                                /// burada value nin tipini bilmiyoruz
                                /// eğer bu çevrim hata vermezse int değerindedir
                                var x = Convert.ToInt64(fvalue);

                                s = s + v.ENTER + " and  " + j_table_alias + "." + j_fname + " = " + fvalue + " ";
                            }
                            catch (Exception)
                            {
                                /// eğer hata verdiyse string kabul ediyoruz 
                                s = s + v.ENTER + " and  " + j_table_alias + "." + j_fname + " = '" + fvalue + "' ";
                            }

                        }
                        // eğer üç değerde dolu ise value olduğu gibi eklenecek
                        if ((t.IsNotNull(mst_fname)) &&
                            (t.IsNotNull(j_fname)) &&
                            (t.IsNotNull(fvalue)))
                            s = s + " and  " + fvalue + " ";
                    }
                }
            }
            #endregion J_WHERE

            return s;
        }

        private void Preparing_Select_Standart_Fields_JSON(List<J_STN_FIELDS> J_STN_FIELDS, 
            ref string joinFields, string j_table_alias)
        {
            string j_alias = j_table_alias;
            string j_fname = string.Empty;
            string fnl_fname = string.Empty;

            if (j_alias.IndexOf("[") == -1)
                j_alias = "[" + j_alias + "]";

            #region J_STN_FIELDS
            foreach (var item in J_STN_FIELDS)
            {
                if (item.J_TABLE_ALIAS == j_table_alias)
                {
                    j_fname = item.J_FNAME.ToString();
                    fnl_fname = item.FNL_FNAME.ToString();
                    
                    joinFields = joinFields + " , " + j_alias + "." + j_fname.PadRight(35) + " " + fnl_fname + v.ENTER;
                }
            }
            #endregion J_STN_FIELDS
        }

        private void Preparing_Select_Case_Fields_JSON(List<J_CASE_FIELDS> J_CASE_FIELDS, 
            ref string joinFields, string j_table_alias, string MasterTableAlias)
        {
            string s = string.Empty;
            string case_for_fname = string.Empty;
            string where_value = string.Empty;
            string then_alias = string.Empty;
            string then_fname = string.Empty;
            string final_fname = string.Empty;

            string satir = string.Empty;
            string satir_sonu = string.Empty;
            string nsatir = string.Empty;
            int i1 = 0;
            
            foreach (var item in J_CASE_FIELDS)
            {
                case_for_fname = item.CASE_FOR_FNAME.ToString();
                where_value = item.WHERE_VALUE.ToString();
                then_alias = item.THEN_ALIAS.ToString();
                then_fname = item.THEN_FNAME.ToString();
                final_fname = item.FINAL_FNAME.ToString();

                satir_sonu = "  else '' end ) " + final_fname + "  ";

                if (nsatir.IndexOf(final_fname) == -1)
                {
                    if ((MasterTableAlias != "") && (case_for_fname != ""))
                    {
                        satir = " , ( case " + MasterTableAlias + "." + case_for_fname + satir_sonu + v.ENTER;
                        nsatir = nsatir + satir;
                    }
                }

                if ((where_value != "") && (then_alias != "") && (then_fname != ""))
                    satir = " when " + where_value + " then " + then_alias + "." + then_fname;

                if (nsatir.IndexOf(satir) == -1)
                {
                    i1 = nsatir.IndexOf(satir_sonu);
                    nsatir = nsatir.Insert(i1, satir);
                }
            }
            
            if (nsatir.Length > 0)
                joinFields = joinFields + nsatir;
        }

        #endregion Preparing_Join_Fields_Tables

        #endregion Preparing_dsData

        #region CategoryList
        public string CategoryList(string TableCode)
        {
            // burası artık çalışmıyor
            // çünkü artık bazı fieldler yok
            //
            string s =
               " select a.LOOKUP_CODE, a.TLKP_TYPE, "
             + " b.DATA_ABOUT CAPTION, a.FIELD_NAME, a.FIELD_TYPE "
             + " from "
             + " [" + v.active_DB.managerDBName + "].dbo.[MS_FIELDS] a, " + v.ENTER
             + " [" + v.active_DB.projectDBName + "].dbo.[APP_LKP_TABLES] b " + v.ENTER
             + " where a.TLKP_TYPE = 107 " + v.ENTER
             + " and not a.LOOKUP_CODE is null " + v.ENTER
             + " and a.TABLE_CODE = '" + TableCode + "' " + v.ENTER
             + " and a.LOOKUP_CODE = b.TYPES_NAME " + v.ENTER

             + " union all " + v.ENTER

             + " select a.LOOKUP_CODE, a.TLKP_TYPE, "
             + " a.FCAPTION  CAPTION, "
                //+ " a.FCAPTION + ' : ' + b.TYPES_CAPTION CAPTION, "
             + " a.FIELD_NAME, a.FIELD_TYPE "
             + " from "
             + " [" + v.active_DB.managerDBName + "].dbo.[MS_FIELDS] a, " + v.ENTER
             + " [" + v.active_DB.projectDBName + "].dbo.[APP_TYPES_LIST] b " + v.ENTER
             + " where a.TLKP_TYPE >= 101 and a.TLKP_TYPE <= 105 " + v.ENTER
             + " and not a.LOOKUP_CODE is null " + v.ENTER
             + " and a.TABLE_CODE = '" + TableCode + "' " + v.ENTER
             + " and a.LOOKUP_CODE = b.TYPES_CODE " + v.ENTER;

            return s;
        }
        #endregion CategoryList

    }
}
