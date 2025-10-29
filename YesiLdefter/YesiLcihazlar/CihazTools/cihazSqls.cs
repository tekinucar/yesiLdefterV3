using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace YesiLcihazlar
{
    public class cihazSqls : tBase
    {
        tToolBox t = new tToolBox();
        public string SQL_SYS_FIRM_List(string myGuid, v.tFirmListType flt)
        {
            string tSql = "";
            string selectFields =
              @"
              Select distinct f.[ID]
                  ,f.[PARENT_ID]
                  ,f.[REF_ID]
                  ,f.[PARENT_REF_ID]
                  ,f.[ISACTIVE]
                  ,f.LOCAL_TD
                  ,f.[USE_PACKAGE]
                  ,f.[FIRM_GUID]
                  ,f.[FIRM_CODE]
                  ,f.[FIRM_NAME]
                  ,f.[FIRM_LONG_NAME1]
                  ,f.[FIRM_LONG_NAME2]
                  ,f.[FIRM_DB_TYPE]
                  ,f.[FIRM_DB_NAME]
                  ,f.[FIRM_MAINDB_FORMAT]
                  ,f.[FIRM_PERIODDB_FORMAT]
                  ,f.[FIRM_SERVER_TYPE]
                  ,f.[FIRM_SERVER_NAME]
                  ,f.[FIRM_AUTHENTICATION]
                  ,f.[FIRM_LOGIN]
                  ,f.[FIRM_PASSWORD]
                  ,f.[FIRM_MAINDB_CONNTEXT]
                  ,f.[FIRM_PERIODDB_CONNTEXT]
            ";

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
             " + selectFields + @"
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
             " + selectFields + @"
              from CTE c
                 left outer join SYS_FIRMS f on(c.U_ID = f.ID)
            ";


            #endregion

            #region guid = TEST

            if (myGuid == "TEST")
                tSql = selectFields + @"
                from  SYS_FIRMS f 
                where f.OPERATION_MODE_TD = 1 ";

            #endregion

            return tSql;
        }

        public string CihazHesapListSql(ref DataSet ds)
        {

            string sql =
        @" Select
   [CihazHesap].Id as CihazId
 , 'none' as Baglanti
 , 0 as Adet 
 , [CihazHesapCalismaTipi].CalismaTipi
 , [CihazHesap].CihazAdi
 , [CihazHesap].CihazIP
 , [CihazHesap].CihazPort
 , [CihazHesap].CalismaTipiId
 From [dbo].[CihazHesap] 
   left outer join [Lkp].[CihazHesapCalismaTipi] on ([CihazHesap].CalismaTipiId = [CihazHesapCalismaTipi].Id)
 Where IsActive = 1   
 And   FirmId = :FIRM_ID
 And   isnull(CihazIP,'''') <> '''' ";

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazHesap");
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
            ds.Namespace = myProp;

            return sql;
        }

        public string prc_CihazLogGetSql(string Param, ref DataSet ds)
        {
            string sql =
        @" 
DECLARE @FirmId int
DECLARE @Param varchar(20)
Set @FirmId = :FIRM_ID
Set @Param = ':PARAM'
EXECUTE [dbo].[prc_CihazLogGet] @FirmId, @Param
        ";
            sql = sql.Replace(":PARAM", Param);

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazLog" + Param);
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
            ds.Namespace = myProp;
            return sql;
        }

        public string CihazEmirYenilerSql(ref DataSet ds)
        {
            string sql = @"
 SELECT [Id]
       ,[TalepTipiId]
       ,[TarihSaat]
 FROM[dbo].[CihazEmri]
 Where FirmId = :FIRM_ID
 And IsRun = 0
 order by TarihSaat ";

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazEmirYeni");
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
            ds.Namespace = myProp;

            return sql;
        }

        public string CihazEmirEskilerSql(ref DataSet ds)
        {
            string sql = @"
 SELECT TOP 100 
        [Id]
       ,[TalepTipiId]
       ,[TarihSaat]
       ,[RunTarihSaat]
 FROM[dbo].[CihazEmri]
 Where FirmId = :FIRM_ID
 And IsRun = 1
 Order by TarihSaat desc ";

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazEmirYeni");
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
            ds.Namespace = myProp;

            return sql;
        }

        public void prc_CihazEmir(int Id)
        {
            string sql =
        @" 
DECLARE @Id int
DECLARE @FirmId int
DECLARE @TalepTipiId smallint

Set @Id = :CihazEmirId
Set @FirmId = :FIRM_ID
Set @TalepTipiId = ':TalepTipiId'
EXECUTE [dbo].[prc_CihazEmir] @Id, @FirmId, @TalepTipiId
        ";

            // buraya Id her zaman geliyor bu sayede her zaman update çalışmakta
            sql = sql.Replace(":CihazEmirId", Id.ToString());
            sql = sql.Replace(":TalepTipiId", "0");// TalepTipiIdGet(Param).ToString()); gerek yok

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazEmir");
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");

            DataSet ds = new DataSet();
            ds.Namespace = myProp;

            try
            {
                t.Data_Read_Execute(ds, ref sql, "CihazEmir", null);
            }
            catch (Exception)
            {
                throw;
            }
            ds.Dispose();
        }


        public void prc_SetCariBio(int cariId, int bioIndex, string bioPrint)
        {
            if (cariId <= 0) return;

            try
            {
                SqlCommand cmd = new SqlCommand(@"
DECLARE @FirmId int
DECLARE @CariId int
DECLARE @BioTipiId smallint
DECLARE @BioIndex smallint
DECLARE @BioPrint nvarchar(max)

set @FirmId = @_FirmId
set @CariId = @_CariId
set @BioTipiId = @_BioTipiId
set @BioIndex = @_BioIndex 
set @BioPrint = @_BioPrint

EXECUTE[dbo].[prc_CariBio]
    @FirmId
  , @CariId
  , @BioTipiId
  , @BioIndex
  , @BioPrint
");
                cmd.Parameters.AddWithValue("@_FirmId", v.SP_FIRM_ID);
                cmd.Parameters.AddWithValue("@_CariId", cariId);
                cmd.Parameters.AddWithValue("@_BioTipiId", 1);
                cmd.Parameters.AddWithValue("@_BioIndex", bioIndex);
                cmd.Parameters.AddWithValue("@_BioPrint", bioPrint);

                string myProp = string.Empty;
                t.MyProperties_Set(ref myProp, "DBaseNo", "4");
                t.MyProperties_Set(ref myProp, "TableName", "CariBio");
                t.MyProperties_Set(ref myProp, "SqlFirst", "null");
                t.MyProperties_Set(ref myProp, "SqlSecond", "null");

                vTable vt = new vTable();
                t.Preparing_DataSet_(myProp, vt, 0);
                t.Sql_ExecuteNon(cmd, vt);

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
        }

        // geçici prpcedure işi bitince sil
        public void prc_SetCariHesap(int cariId, string cariTamAdi)
        {
            if (cariId <= 0) return;

            try
            {
                SqlCommand cmd = new SqlCommand(@"
DECLARE @FirmId int
DECLARE @CariId int
DECLARE @CariTamAdi nvarchar(200)

set @FirmId = @_FirmId
set @CariId = @_CariId
set @CariTamAdi = @_CariTamAdi

EXECUTE [dbo].[prc_CariHesap] @FirmId, @CariId, @CariTamAdi ");

                cmd.Parameters.AddWithValue("@_FirmId", v.SP_FIRM_ID);
                cmd.Parameters.AddWithValue("@_CariId", cariId);
                cmd.Parameters.AddWithValue("@_CariTamAdi", cariTamAdi);

                string myProp = string.Empty;
                t.MyProperties_Set(ref myProp, "DBaseNo", "4");
                t.MyProperties_Set(ref myProp, "TableName", "CariHesap");
                t.MyProperties_Set(ref myProp, "SqlFirst", "null");
                t.MyProperties_Set(ref myProp, "SqlSecond", "null");

                vTable vt = new vTable();
                t.Preparing_DataSet_(myProp, vt, 0);
                t.Sql_ExecuteNon(cmd, vt);

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
        }


        public void prc_CihazLogSet(int CihazId, int EylemPlaniId, Int16 IsDelete)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(@" 
DECLARE @FirmId int
DECLARE @CihazId int
DECLARE @EylemPlaniId int
DECLARE @IsDelete smallint

set @FirmId = @_FirmId
set @CihazId = @_CihazId
set @EylemPlaniId = @_EylemPlaniId
set @IsDelete = @_IsDelete

EXECUTE [dbo].[prc_CihazLogSet] @FirmId, @CihazId, @EylemPlaniId, @IsDelete ");

                cmd.Parameters.AddWithValue("@_FirmId", v.SP_FIRM_ID);
                cmd.Parameters.AddWithValue("@_CihazId", CihazId);
                cmd.Parameters.AddWithValue("@_EylemPlaniId", EylemPlaniId);
                cmd.Parameters.AddWithValue("@_IsDelete", IsDelete);

                string myProp = string.Empty;
                t.MyProperties_Set(ref myProp, "DBaseNo", "4");
                t.MyProperties_Set(ref myProp, "TableName", "CihazLog");
                t.MyProperties_Set(ref myProp, "SqlFirst", "null");
                t.MyProperties_Set(ref myProp, "SqlSecond", "null");

                vTable vt = new vTable();
                t.Preparing_DataSet_(myProp, vt, 0);
                t.Sql_ExecuteNon(cmd, vt);

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
        }


        public void prc_CihazKaydiSet(int CihazId, int CariId, string TarihSaat)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(@" 
DECLARE @FirmId int
DECLARE @CihazId int
DECLARE @CariId int
DECLARE @TarihSaat datetime

set @FirmId = @_FirmId
set @CihazId = @_CihazId
set @CariId = @_CariId
set @TarihSaat = convert(datetime, @_TarihSaat, 101)

EXECUTE [dbo].[prc_CihazKaydiSet] @FirmId, @CihazId, @CariId, @TarihSaat ");

                cmd.Parameters.AddWithValue("@_FirmId", v.SP_FIRM_ID);
                cmd.Parameters.AddWithValue("@_CihazId", CihazId);
                cmd.Parameters.AddWithValue("@_CariId", CariId);
                cmd.Parameters.AddWithValue("@_TarihSaat", TarihSaat);

                string myProp = string.Empty;
                t.MyProperties_Set(ref myProp, "DBaseNo", "4");
                t.MyProperties_Set(ref myProp, "TableName", "CihazKaydi");
                t.MyProperties_Set(ref myProp, "SqlFirst", "null");
                t.MyProperties_Set(ref myProp, "SqlSecond", "null");

                vTable vt = new vTable();
                t.Preparing_DataSet_(myProp, vt, 0);
                t.Sql_ExecuteNon(cmd, vt);

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
        }



        public bool prc_CihazTestUser(int machineNumber, int cariId, string userName, int bioIndex, string bioPrint)
        {
            if (cariId <= 0) return false;
            
            bool onay = false;

            try
            {
                SqlCommand cmd = new SqlCommand(@"
DECLARE @FirmId int
DECLARE @MachineNumber int
DECLARE @CariId int
DECLARE @UserName nvarchar(100)
DECLARE @BioIndex smallint
DECLARE @BioPrint nvarchar(max)

set @FirmId = @_FirmId
set @MachineNumber = @_MachineNumber
set @CariId = @_CariId
set @UserName = @_UserName
set @BioIndex = @_BioIndex 
set @BioPrint = @_BioPrint

EXECUTE[dbo].[prc_CihazTestUser]
    @FirmId
  , @MachineNumber
  , @CariId
  , @UserName
  , @BioIndex
  , @BioPrint
");
                cmd.Parameters.AddWithValue("@_FirmId", v.SP_FIRM_ID);
                cmd.Parameters.AddWithValue("@_MachineNumber", machineNumber);
                cmd.Parameters.AddWithValue("@_CariId", cariId);
                cmd.Parameters.AddWithValue("@_UserName", userName);
                cmd.Parameters.AddWithValue("@_BioIndex", bioIndex);
                cmd.Parameters.AddWithValue("@_BioPrint", bioPrint);

                string myProp = string.Empty;
                t.MyProperties_Set(ref myProp, "DBaseNo", "4");
                t.MyProperties_Set(ref myProp, "TableName", "CihazTestUser");
                t.MyProperties_Set(ref myProp, "SqlFirst", "null");
                t.MyProperties_Set(ref myProp, "SqlSecond", "null");

                vTable vt = new vTable();
                t.Preparing_DataSet_(myProp, vt, 0);
                t.Sql_ExecuteNon(cmd, vt);
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

        public string cihazTestLogSql(ref DataSet ds, int machineNumber)
        {
            string sql = @"
 SELECT [Id]
      ,[FirmId]
      ,[MachineNumber]
      ,[CariId]
      ,[TarihSaat]
 FROM [dbo].[CihazTestLog]
 Where [FirmId] = :FIRM_ID
 And [MachineNumber] = :MachineNumber ";

            sql = sql.Replace(":MachineNumber", machineNumber.ToString());

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazTestLog");
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
            ds.Namespace = myProp;

            try
            {
                t.Data_Read_Execute(ds, ref sql, "CihazTestLog", null);
            }
            catch (Exception)
            {
                throw;
            }

            return sql;
        }

        public string cihazTestUserSql(ref DataSet ds, int machineNumber)
        {
            string sql = @"
 SELECT [Id]
      ,[FirmId]
      ,[MachineNumber]
      ,[CariId]
      ,[UserName]
      ,[BioIndex]
      ,[BioPrint]
 FROM [dbo].[CihazTestUser]
 Where [FirmId] = :FIRM_ID
 And [MachineNumber] = :MachineNumber ";

            sql = sql.Replace(":MachineNumber", machineNumber.ToString());

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazTestUser");
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");
            ds.Namespace = myProp;

            try
            {
                t.Data_Read_Execute(ds, ref sql, "CihazTestUser", null);
            }
            catch (Exception)
            {
                throw;
            }

            return sql;
        }

        public string cihazTestUserCountSql(int machineNumber, ref int iUserCount, ref int iFpCount)
        {
            string sql = @"
 Select sum( UserCount ) as UserCount
      , sum( BioPrintCount ) as BioPrintCount from (
 Select  count( distinct CariId ) UserCount
 , 0 BioPrintCount
 From [dbo].[CihazTestUser]
 Where [FirmId] = :FIRM_ID
 And [MachineNumber] = :MachineNumber 
 Union all
 Select  0 UserCount
 , count(CariId) BioPrintCount
 From[dbo].[CihazTestUser]
 Where [FirmId] = :FIRM_ID
 And   [MachineNumber] = :MachineNumber
 And   isnull([BioPrint],'''') <> ''''
 ) topla ";

            sql = sql.Replace(":MachineNumber", machineNumber.ToString());

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazTestUser");
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");

            DataSet ds = new DataSet();
            ds.Namespace = myProp;

            iUserCount = 0;
            iFpCount = 0;

            try
            {
                t.Data_Read_Execute(ds, ref sql, "CihazTestUserCount", null);
                if (ds.Tables.Count > 0)
                {
                    iUserCount = t.myInt32(ds.Tables[0].Rows[0]["UserCount"].ToString());
                    iFpCount = t.myInt32(ds.Tables[0].Rows[0]["BioPrintCount"].ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }

            ds.Dispose();

            return sql;
        }

        public string cihazTestLogCountSql(int machineNumber, ref int bioLogCount)
        {
            string sql = @"
 Select  count( Id ) BioLogCount
 From [dbo].[CihazTestLog]
 Where [FirmId] = :FIRM_ID
 And [MachineNumber] = :MachineNumber ";

            sql = sql.Replace(":MachineNumber", machineNumber.ToString());

            string myProp = string.Empty;
            t.MyProperties_Set(ref myProp, "DBaseNo", "4");
            t.MyProperties_Set(ref myProp, "TableName", "CihazTestLog");
            t.MyProperties_Set(ref myProp, "SqlFirst", sql);
            t.MyProperties_Set(ref myProp, "SqlSecond", "null");

            DataSet ds = new DataSet();
            ds.Namespace = myProp;

            bioLogCount = 0;
            
            try
            {
                t.Data_Read_Execute(ds, ref sql, "CihazTestLogCount", null);
                if (ds.Tables.Count > 0)
                {
                    bioLogCount = t.myInt32(ds.Tables[0].Rows[0]["BioLogCount"].ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }

            ds.Dispose();

            return sql;
        }

        public string cihazTestLogClearSql(int machineNumber)
        {
            string sonuc = "";
            try
            {
                SqlCommand cmd = new SqlCommand(@"
 Delete From [dbo].[CihazTestLog]
 Where [FirmId] = @_FirmId
 And [MachineNumber] = @_MachineNumber ");

                cmd.Parameters.AddWithValue("@_FirmId", v.SP_FIRM_ID);
                cmd.Parameters.AddWithValue("@_MachineNumber", machineNumber);
                
                string myProp = string.Empty;
                t.MyProperties_Set(ref myProp, "DBaseNo", "4");
                t.MyProperties_Set(ref myProp, "TableName", "CihazTestLog");
                t.MyProperties_Set(ref myProp, "SqlFirst", "null");
                t.MyProperties_Set(ref myProp, "SqlSecond", "null");

                vTable vt = new vTable();
                t.Preparing_DataSet_(myProp, vt, 0);
                t.Sql_ExecuteNon(cmd, vt);

                cmd.Dispose();

                sonuc = machineNumber.ToString() + " nolu cihazın Log datası başarıyla silindi ..."; 
            }
            catch (SqlException se)
            {
                throw se;
            }
            catch (Exception ex)
            {
                sonuc = machineNumber.ToString() + " nolu cihazın Log datası silinirken hata oluştu ..." + v.ENTER + ex.Message;
                throw ex;
            }

            return sonuc;
        }

        public string cihazTestUserClearFpsSql(int machineNumber)
        {
            string sonuc = "";
            try
            {
                SqlCommand cmd = new SqlCommand(@"
 Update From [dbo].[CihazTestUser] set BioPrint = null 
 Where [FirmId] = @_FirmId
 And [MachineNumber] = @_MachineNumber ");

                cmd.Parameters.AddWithValue("@_FirmId", v.SP_FIRM_ID);
                cmd.Parameters.AddWithValue("@_MachineNumber", machineNumber);

                string myProp = string.Empty;
                t.MyProperties_Set(ref myProp, "DBaseNo", "4");
                t.MyProperties_Set(ref myProp, "TableName", "CihazTestUser");
                t.MyProperties_Set(ref myProp, "SqlFirst", "null");
                t.MyProperties_Set(ref myProp, "SqlSecond", "null");

                vTable vt = new vTable();
                t.Preparing_DataSet_(myProp, vt, 0);
                t.Sql_ExecuteNon(cmd, vt);

                cmd.Dispose();

                sonuc = machineNumber.ToString() + " nolu cihazın kullanıcının Bio datası başarıyla silindi ...";
            }
            catch (SqlException se)
            {
                throw se;
            }
            catch (Exception ex)
            {
                sonuc = machineNumber.ToString() + " nolu cihazın kullanıcının Bio datası silinirken hata oluştu ..." + v.ENTER + ex.Message;
                throw ex;
            }

            return sonuc;
        }

        public string cihazTestUserClearUsersSql(int machineNumber)
        {
            string sonuc = "";
            try
            {
                SqlCommand cmd = new SqlCommand(@"
 Delete From [dbo].[CihazTestUser]
 Where [FirmId] = @_FirmId
 And [MachineNumber] = @_MachineNumber ");

                cmd.Parameters.AddWithValue("@_FirmId", v.SP_FIRM_ID);
                cmd.Parameters.AddWithValue("@_MachineNumber", machineNumber);

                string myProp = string.Empty;
                t.MyProperties_Set(ref myProp, "DBaseNo", "4");
                t.MyProperties_Set(ref myProp, "TableName", "CihazTestLog");
                t.MyProperties_Set(ref myProp, "SqlFirst", "null");
                t.MyProperties_Set(ref myProp, "SqlSecond", "null");

                vTable vt = new vTable();
                t.Preparing_DataSet_(myProp, vt, 0);
                t.Sql_ExecuteNon(cmd, vt);

                cmd.Dispose();

                sonuc = machineNumber.ToString() + " nolu cihazın Kullanıcı datası başarıyla silindi ...";
            }
            catch (SqlException se)
            {
                throw se;
            }
            catch (Exception ex)
            {
                sonuc = machineNumber.ToString() + " nolu cihazın Kullanıcı datası silinirken hata oluştu ..." + v.ENTER + ex.Message;
                throw ex;
            }

            return sonuc;
        }

        public bool cihazTestAllClearSql(int machineNumber, ref string log)
        {
            bool onay = false;
            
            string log_ = "";
            log_ = cihazTestLogClearSql(machineNumber) + v.ENTER;
            //log_ = log_ + cihazTestUserClearFpsSql(machineNumber) + v.ENTER;
            log_ = log_ + cihazTestUserClearUsersSql(machineNumber) + v.ENTER;
            log = log_;
            
            onay = true;

            return onay;
        }

    }
}
