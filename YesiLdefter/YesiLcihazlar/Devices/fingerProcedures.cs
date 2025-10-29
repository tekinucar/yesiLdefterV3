using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DevExpress.XtraEditors.Filtering.Templates;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.Utils.DirectXPaint;

namespace YesiLcihazlar
{
    internal class fingerProcedures
    {
        /// user / kişi bilgisi set etme, okuma
        /// 
        tToolBox t = new tToolBox();
        cihazSqls Sql = new cihazSqls();

        #region orjinal fonksiyonlar

        #region User / Kullanıcı bilgileri

        /*
        /// sadece userId ve UserName cihaza set ediliyor
        public void tSetYeniUser(DataTable table)
        {
            int iMachineNumber = 0;
            string userId = "";
            string userName = "";
            int iPrivilege = 0;
            string cardNumber = "";
            string password = "";

            int cihazId = 0;
            int cihazId2 = 0;

            fingerK50_SDKHelper SDK = null;

            foreach (DataRow row in table.Rows)
            {
                cihazId = t.myInt32(row["LkpCihazId"].ToString());

                if (cihazId2 != cihazId)
                {
                    // bitiş işlem
                    if (cihazId2 > 0)
                    {
                        iMachineNumber = cihazId2;
                        //upload all the information in the memory
                        SDK.BatchUpdate(iMachineNumber);
                        //the data in the device should be refreshed
                        SDK.RefreshData(iMachineNumber);
                        SDK.EnableDevice(iMachineNumber, true);

                        disConnectCihazSdk(iMachineNumber);

                        iMachineNumber = cihazId;
                    }

                    SDK = getCihazSdk(cihazId);
                    SDK.EnableDevice(iMachineNumber, false);

                    cihazId2 = cihazId;
                }


                if (SDK != null)
                {
                    iMachineNumber = t.myInt32(row["LkpCihazId"].ToString());
                    userId = row["CariId"].ToString();
                    userName = row["TamAdi"].ToString();
                    iPrivilege = Convert.ToInt32(v.tCihazPrivilage.prOrtakKulanici); //row[""];

                    tSetUserInfo_(SDK, iMachineNumber, userId, userName, iPrivilege, cardNumber, password);
                }

            }

            iMachineNumber = cihazId;
            //upload all the information in the memory
            SDK.BatchUpdate(iMachineNumber);
            //the data in the device should be refreshed
            SDK.RefreshData(iMachineNumber);
            SDK.EnableDevice(iMachineNumber, true);

            disConnectCihazSdk(iMachineNumber);
        }
        */





        /*
        /// Liste : user ve parmak izi kaydı
        public int tSetAllUserFPInfo(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataTable dT
            )
        {
            string sEnrollNumber = "";
            string sEnabled = "";
            bool bEnabled = false;

            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            string sFPTmpData = "";
            string sCardnumber = "";
            int idwFingerIndex = 0;
            string sdwFingerIndex = "";
            int iFlag = 0;
            string sFlag = "";
            int num = 0;
            int idwErrorCode = 0;

            //prgSta.Value = 0;

            SDK.EnableDevice(iMachineNumber, false);
            
            int count = dT.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                sEnrollNumber = dT.Rows[i]["EnrollNumber"].ToString();
                sEnabled = dT.Rows[i]["Enabled"].ToString();
                if (sEnabled == "true")
                {
                    bEnabled = true;
                }
                else
                {
                    bEnabled = false;
                }
                sName = dT.Rows[i]["Name"].ToString();
                sCardnumber = dT.Rows[i]["CardNumber"].ToString();
                sPassword = dT.Rows[i]["Password"].ToString();
                sdwFingerIndex = dT.Rows[i]["FingerIndex"].ToString();
                sFlag = dT.Rows[i]["iFlag"].ToString();
                sFPTmpData = dT.Rows[i]["TmpData"].ToString();
                iPrivilege = Convert.ToInt32(dT.Rows[i]["Privelage"].ToString());

                if (sCardnumber != "" && sCardnumber != "0")
                {
                    SDK.SetStrCardNumber(sCardnumber);
                }
                //upload user information to the device
                if (SDK.SSR_SetUserInfo(iMachineNumber, sEnrollNumber, sName, sPassword, iPrivilege, bEnabled))
                {
                    if (sdwFingerIndex != "" && sFlag != "" && sFPTmpData != "")
                    {
                        idwFingerIndex = Convert.ToInt32(sdwFingerIndex);
                        iFlag = Convert.ToInt32(sFlag);
                        //upload templates information to the device
                        SDK.SetUserTmpExStr(iMachineNumber, sEnrollNumber, idwFingerIndex, iFlag, sFPTmpData);
                    }
                    num++;
                    //prgSta.Value = num % 100;
                }
                else
                {
                    SDK.GetLastError(ref idwErrorCode);
                    //lblOutputInfo.Items.Add("*Upload user " + sEnrollNumber + " error,ErrorCode=!" + idwErrorCode.ToString());
                    //axCZKEM1.EnableDevice(iMachineNumber, true);
                    //return -1022;
                }

            }
            //prgSta.Value = 100;

            //the data in the device should be refreshed
            SDK.RefreshData(iMachineNumber);
            SDK.EnableDevice(iMachineNumber, true);
            //lblOutputInfo.Items.Add("Upload user successfully");

            return 1;
        }
        */

        /*
        /// Liste : hızlı user ve parmak izi kaydı // bu kullanılmadı
        public int tBatch_SetAllUserFPInfo(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataTable dT
            )
        {
            string sLastEnrollNumber = "";
            string sEnrollNumber = "";
            string sEnabled = "";
            bool bEnabled = false;

            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            string sFPTmpData = "";
            string sCardnumber = "";
            int idwFingerIndex = 0;
            string sdwFingerIndex = "";
            int iFlag = 0;
            string sFlag = "";
            int num = 0;
            int idwErrorCode = 0;

            //prgSta.Value = 0;
            SDK.EnableDevice(iMachineNumber, false);

            //create memory space for batching data
            if (SDK.BeginBatchUpdate(iMachineNumber, 1))
            {
                int count = dT.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    sEnrollNumber = dT.Rows[i]["EnrollNumber"].ToString();
                    sEnabled = dT.Rows[i]["Enabled"].ToString();
                    if (sEnabled == "true")
                    {
                        bEnabled = true;
                    }
                    else
                    {
                        bEnabled = false;
                    }
                    sName = dT.Rows[i]["Name"].ToString();
                    sCardnumber = dT.Rows[i]["CardNumber"].ToString();
                    sPassword = dT.Rows[i]["Password"].ToString();
                    sdwFingerIndex = dT.Rows[i]["FingerIndex"].ToString();
                    sFlag = dT.Rows[i]["iFlag"].ToString();
                    sFPTmpData = dT.Rows[i]["TmpData"].ToString();
                    iPrivilege = Convert.ToInt32(dT.Rows[i]["Privelage"].ToString());
                                                         
                    if (sCardnumber != "" && sCardnumber != "0")
                    {
                        SDK.SetStrCardNumber(sCardnumber);
                    }

                    // kullanıcı ismi bir defa kaydedilecek
                    // kullanıcının birden fazla FP varsa o datalar ayrı gönderilir 
                    if (sEnrollNumber != sLastEnrollNumber)
                    {
                        //upload user information to the device
                        if (SDK.SSR_SetUserInfo(iMachineNumber, sEnrollNumber, sName, sPassword, iPrivilege, bEnabled))
                        {
                            if (sdwFingerIndex != "" && sFlag != "" && sFPTmpData != "")
                            {
                                idwFingerIndex = Convert.ToInt32(sdwFingerIndex);
                                iFlag = Convert.ToInt32(sFlag);
                                //upload templates information to the device
                                SDK.SetUserTmpExStr(iMachineNumber, sEnrollNumber, idwFingerIndex, iFlag, sFPTmpData);
                            }
                            num++;
                            //prgSta.Value = num % 100;
                        }
                        else SDK.GetLastError(ref idwErrorCode);
                    }
                    else
                    {
                        // birden fazla olan FP burada cihaza gönderiliyor
                        if (sdwFingerIndex != "" && sFlag != "" && sFPTmpData != "")
                        {
                            idwFingerIndex = Convert.ToInt32(sdwFingerIndex);
                            iFlag = Convert.ToInt32(sFlag);
                            //upload templates information to the device
                            SDK.SetUserTmpExStr(iMachineNumber, sEnrollNumber, idwFingerIndex, iFlag, sFPTmpData);
                        }
                        num++;
                        //prgSta.Value = num % 100;
                    }
                    sLastEnrollNumber = sEnrollNumber;
                }
            }
            //prgSta.Value = 100;

            //upload all the information in the memory
            SDK.BatchUpdate(iMachineNumber);
            //the data in the device should be refreshed
            SDK.RefreshData(iMachineNumber);
            SDK.EnableDevice(iMachineNumber, true);
            //lblOutputInfo.Items.Add("Upload user successfully in batch");
            return 1;
        }
        */

        /// liste halindeki kullanıcı isimleri ve FP leri istenilen cihazlara gönder
        /// Sadece bir cihaza tüm listeyi gönderir
        public bool tBatch_SetAllUserFPInfo(int cihazId, DataSet ds, v.cihazTalepTipi talep)
        {
            fingerK50_SDKHelper SDK = null;
            int rowCount = ds.Tables[0].Rows.Count;
            int idwErrorCode = 0;
            int iMachineNumber = 0;

            bool onay = false;
            string myLog = talepNameGet(talep);
            string sLastEnrollNumber = "";

            if (v.listBox1 != null)
                v.listBox1.Items.Add(myLog + " : işlemi başladı ...");

            v.progressBar1.Value = 0;
            Application.DoEvents();
                        
            iMachineNumber = cihazId;
            // cihaz için  başlangıç : cihaz için SDK hazırla
            SDK = getCihazSdk(cihazId);

            // cihazı kullanılmaz hazle geti
            if (v.CihazIsActive) 
                SDK.EnableDevice(iMachineNumber, false);

            // create memory space for batching data
            if (v.CihazIsActive)
                onay = SDK.BeginBatchUpdate(iMachineNumber, 1);
            else onay = true;
                        
            v.progressBar1.Maximum = rowCount;
            
            for (int i = 0; i < rowCount; i++)
            {
                if (onay)
                {
                    // fp olan tüm kullanıcıları cihaza gönder (hangi cihaz isteniyorsa)
                    if (talep == v.cihazTalepTipi.chSetAllUserAndFPs)
                    {
                        //if (v.CihazIsActive)
                        idwErrorCode = tBatch_SetUserFPInfo_(SDK, iMachineNumber, ds.Tables[0], i, ref sLastEnrollNumber);
                    }
                }

                v.progressBar1.Value = i;
                Application.DoEvents();
            }

            if (v.CihazIsActive)
            {
                //upload all the information in the memory
                SDK.BatchUpdate(iMachineNumber);
                //the data in the device should be refreshed
                SDK.RefreshData(iMachineNumber);
                SDK.EnableDevice(iMachineNumber, true);
            }

            v.progressBar1.Value = 0;
            Application.DoEvents();

            if (v.listBox1 != null)
                v.listBox1.Items.Add(myLog + " : işlemi bitti ...");

            return onay;
        }
        
        /// eylem listesindeki kullanıcı isimleri ve FP leri gerekli cihazlara gönderir
        /// birden fazla cihaza sadece eylem planında olanları gönderir
        public bool tBatch_SetEPUserFPInfo(DataSet ds, v.cihazTalepTipi talep)
        {
            fingerK50_SDKHelper SDK = null;
            int cihazId = 0;
            int cihazId2 = 0;
            int eylemPlaniId = 0;
            Int16 isDelete = 0;
            int rowCount = ds.Tables[0].Rows.Count;
            int idwErrorCode = 0;

            int iMachineNumber = 0;
            string userId = "";
            string userName = "";
            int iPrivilege = 0;
            string cardNumber = "";
            string password = "";

            bool onay = false;
            string sLastEnrollNumber = "";
            
            string myLog = talepNameGet(talep);

            if (v.listBox1 != null)
                v.listBox1.Items.Add("... " + myLog + " : işlemi başladı ...");

            v.progressBar1.Maximum = rowCount;
            v.progressBar1.Value = 0;
            Application.DoEvents();

            //test
            //rowCount = 3; 

            for (int i = 0; i < rowCount; i++)
            {
                cihazId = t.myInt32(ds.Tables[0].Rows[i]["LkpCihazId"].ToString());
                eylemPlaniId = t.myInt32(ds.Tables[0].Rows[i]["LkpEylemId"].ToString());
                isDelete = t.myInt16(ds.Tables[0].Rows[i]["IsDelete"].ToString());

                //test
                //cihazId = 2;

                iMachineNumber = cihazId;
                
                // Cihaz SDK tespiti, Connect ve Disconnect işlemleri 
                // gelene sorguda birden fazla cihaz için liste oluyor
                // cihaz değişince SDK larıda cihaza göre ayarlamak gerekiyor  
                //
                if (cihazId != cihazId2)
                {
                    // bitiş işlemi : satırlarda yeni bir cihaza geçmişse
                    if (cihazId2 > 0)
                    {
                        iMachineNumber = cihazId2;

                        if ((talep == v.cihazTalepTipi.chDelGorev) ||
                            (talep == v.cihazTalepTipi.chDelGorus) ||
                            (talep == v.cihazTalepTipi.chDelTahliye))
                        {
                            // silme işleminde BeginBatchUpdate in çalışmaması gerekiyor
                            onay = true;
                        }
                        else
                        {
                            // create memory space for batching data
                            if (v.CihazIsActive)
                                onay = SDK.BeginBatchUpdate(iMachineNumber, 1);
                            else onay = true;
                        }

                        //the data in the device should be refreshed
                        if (v.CihazIsActive)
                        {
                            SDK.RefreshData(iMachineNumber);
                            SDK.EnableDevice(iMachineNumber, true);
                        }
                        iMachineNumber = cihazId;
                    }

                    // cihaz için  başlangıç : cihaz için SDK hazırla
                    SDK = getCihazSdk(cihazId);

                    // cihazı kullanılmaz hazle getir
                    if (v.CihazIsActive)
                        SDK.EnableDevice(iMachineNumber, false);

                    if ((talep == v.cihazTalepTipi.chDelGorev) ||
                        (talep == v.cihazTalepTipi.chDelGorus) ||
                        (talep == v.cihazTalepTipi.chDelTahliye))
                    {
                        // silme işleminde BeginBatchUpdate in çalışmaması gerekiyor
                        onay = true; 
                    }
                    else
                    {
                        // create memory space for batching data
                        if (v.CihazIsActive)
                            onay = SDK.BeginBatchUpdate(iMachineNumber, 1);
                        else onay = true;
                    }
                    // hafıza sakla
                    cihazId2 = cihazId;
                }
                                
                if (onay)
                {
                    idwErrorCode = 0;

                    if (talep == v.cihazTalepTipi.chSetNewUser)
                    {
                        userId = ds.Tables[0].Rows[i]["CariId"].ToString();
                        userName = ds.Tables[0].Rows[i]["TamAdi"].ToString();
                        iPrivilege = Convert.ToInt32(v.tCihazPrivilage.prOrtakKulanici);

                        onay = tSetUserInfo_(SDK, iMachineNumber, userId, userName, iPrivilege, cardNumber, password);
                    }
                    
                    if ((talep == v.cihazTalepTipi.chSetSayim) ||
                        (talep == v.cihazTalepTipi.chSetTahliye) ||
                        (talep == v.cihazTalepTipi.chSetGorev) ||
                        (talep == v.cihazTalepTipi.chSetGorus) 
                        )
                    {
                        //if (v.CihazIsActive) 
                        idwErrorCode = tBatch_SetUserFPInfo_(SDK, iMachineNumber, ds.Tables[0], i, ref sLastEnrollNumber);

                        // eğer hata dönmediyse işlem başarılı
                        if (idwErrorCode == 0)
                        {
                            // yapılan atamaların kaydı tutuluyor
                            Sql.prc_CihazLogSet(cihazId, eylemPlaniId, isDelete);
                        }
                        else onay = false;
                    }

                    if ((talep == v.cihazTalepTipi.chDelGorev) ||
                        (talep == v.cihazTalepTipi.chDelGorus) ||
                        (talep == v.cihazTalepTipi.chDelTahliye)
                        )
                    {
                        if (v.CihazIsActive)
                            idwErrorCode = tBatch_DelUserFPInfo_(SDK, iMachineNumber, ds.Tables[0], i, ref sLastEnrollNumber);

                        // eğer hata dönmediyse işlem başarılı
                        if (idwErrorCode == 0)
                        {
                            // yapılan atamaların kaydı tutuluyor
                            Sql.prc_CihazLogSet(cihazId, eylemPlaniId, isDelete);
                        }
                        else onay = false;
                    }
                }

                v.progressBar1.Value = i;
                Application.DoEvents();
            }

            iMachineNumber = cihazId2;
            
            if ((talep == v.cihazTalepTipi.chDelGorev) ||
                (talep == v.cihazTalepTipi.chDelGorus) ||
                (talep == v.cihazTalepTipi.chDelTahliye))
            {
                // silme işleminde BeginBatchUpdate in çalışmaması gerekiyor
            }
            else
            {
                //upload all the information in the memory
                if (v.CihazIsActive)
                    SDK.BatchUpdate(iMachineNumber);
            }

            //the data in the device should be refreshed
            if (v.CihazIsActive)
            {
                SDK.RefreshData(iMachineNumber);
                SDK.EnableDevice(iMachineNumber, true);
            }

            if (v.listBox1 != null)
                v.listBox1.Items.Add("... " + myLog + " : işlemi bitti ...");

            return onay;
        }

        /// Sadece Kullanıcı hakkındaki bilginin kaydı 
        /// v.cihazTalepTipi.chSetYeniUser
        public bool tSetUserInfo_(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            string userId,
            string userName,
            int iPrivilege,
            string cardNumber,
            string password)
        {
            #region
            /*
            //lblOutputInfo.Items.Add("[func IsUserDefRoleEnable]Temporarily unsupported");

            int iPIN2Width = 0;
            int iIsABCPinEnable = 0;
            int iT9FunOn = 0;
            string strTemp = "";
            axCZKEM1.GetSysOption(GetMachineNumber(), "~PIN2Width", out strTemp);
            iPIN2Width = Convert.ToInt32(strTemp);
            axCZKEM1.GetSysOption(GetMachineNumber(), "~IsABCPinEnable", out strTemp);
            iIsABCPinEnable = Convert.ToInt32(strTemp);
            axCZKEM1.GetSysOption(GetMachineNumber(), "~T9FunOn", out strTemp);
            iT9FunOn = Convert.ToInt32(strTemp);
            /*
            axCZKEM1.GetDeviceInfo(iMachineNumber, 76, ref iPIN2Width);
            axCZKEM1.GetDeviceInfo(iMachineNumber, 77, ref iIsABCPinEnable);
            axCZKEM1.GetDeviceInfo(iMachineNumber, 78, ref iT9FunOn);
            *+/

            if (txtUserID.Text.Length > iPIN2Width)
            {
                lblOutputInfo.Items.Add("*User ID error! The max length is " + iPIN2Width.ToString());
                return -1022;
            }

            if (iIsABCPinEnable == 0 || iT9FunOn == 0)
            {
                if (txtUserID.Text.Substring(0, 1) == "0")
                {
                    lblOutputInfo.Items.Add("*User ID error! The first letter can not be as 0");
                    return -1022;
                }

                foreach (char tempchar in txtUserID.Text.ToCharArray())
                {
                    if (!(char.IsDigit(tempchar)))
                    {
                        lblOutputInfo.Items.Add("*User ID error! User ID only support digital");
                        return -1022;
                    }
                }
            }
            */
            #endregion

            bool onay = false;
            int idwErrorCode = 0;
            string sEnrollNumber = userId;
            string sName = userName;
            string sCardNumber = cardNumber;
            string sPassword = password;
            bool bEnabled = true;

            /// Dikkat Privilege
            /// 1.Privilege parametresi kullanıcı ayrıcalığını belirtir. 
            ///   0 : ortak kullanıcı, 
            ///   1 : kayıt memuru
            ///   2 : yönetici
            ///   3 : süper yöneticiyi belirtir. 
            /// 2.Enable parametresi bir kullanıcı hesabının etkin olup olmadığını belirtir. 
            ///   1 : kullanıcı hesabının etkinleştirildiğini 
            ///   0 : kullanıcı hesabının devre dışı bırakıldığını gösterir.


            //SDK.EnableDevice(iMachineNumber, false);
            // Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
            // SetUserInfo işlevini kullanmadan önce, cihaza yükleyebileceğinizden emin olmak için kart numarasını ayarlayın

            //??????
            if (sCardNumber == "")
                sCardNumber = "0";
            //SDK.SetStrCardNumber(sCardNumber);

            if (v.CihazIsActive)
            {
                // upload the user's information(card number included)
                // kullanıcı bilgilerini yükle (kart numarası dahil)
                if (SDK.SSR_SetUserInfo(iMachineNumber, sEnrollNumber, sName, sPassword, iPrivilege, bEnabled))
                {
                    //lblOutputInfo.Items.Add("Set user information successfully");
                }
                else
                {
                    SDK.GetLastError(ref idwErrorCode);
                    //lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
                }

                if (idwErrorCode != 0)
                    onay = false;
            }
            else
            {
                // test tablosuna ekle
                onay = Sql.prc_CihazTestUser(iMachineNumber, t.myInt32(sEnrollNumber), sName, 0, "");
            }

            // the data in the device should be refreshed
            // cihazdaki veriler yenilenmeli
            //SDK.RefreshData(iMachineNumber);
            //SDK.EnableDevice(iMachineNumber, true);

            return onay;
        }

        /// satırdaki kullanıcı isimni ve FP ler cihaza gönder
        /// v.cihazTalepTipi.chSetYeni
        public int tBatch_SetUserFPInfo_(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataTable dT,
            int rowId,
            ref string sLastEnrollNumber
            )
        {
            int i = rowId;
            string sEnrollNumber = "";
            string sEnabled = "";
            bool bEnabled = false;
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            string sFPTmpData = "";
            string sCardnumber = "";
            int idwFingerIndex = 0;
            string sdwFingerIndex = "";
            int iFlag = 0;
            string sFlag = "";
            int num = 0;
            int idwErrorCode = 0;
            
            //sEnrollNumber = dT.Rows[i]["EnrollNumber"].ToString();
            sEnrollNumber = dT.Rows[i]["CariId"].ToString();

            // default olsun
            //sEnabled = dT.Rows[i]["Enabled"].ToString();
            sEnabled = "true"; 
            if (sEnabled == "true")
                 bEnabled = true;
            else bEnabled = false;
                        
            //sName = dT.Rows[i]["Name"].ToString();
            sName = dT.Rows[i]["TamAdi"].ToString();

            //sCardnumber = dT.Rows[i]["CardNumber"].ToString();
            sCardnumber = "";
                        
            //sPassword = dT.Rows[i]["Password"].ToString();
            sPassword = "";
                        
            //sdwFingerIndex = dT.Rows[i]["FingerIndex"].ToString();
            sdwFingerIndex = dT.Rows[i]["BioIndex"].ToString();
            if (sdwFingerIndex != "")
                idwFingerIndex = Convert.ToInt32(sdwFingerIndex);

            //sFlag = dT.Rows[i]["iFlag"].ToString();
            sFlag = "1";
            iFlag = Convert.ToInt32(sFlag);

            //sFPTmpData = dT.Rows[i]["TmpData"].ToString();
            sFPTmpData = dT.Rows[i]["BioPrint"].ToString();

            //iPrivilege = Convert.ToInt32(dT.Rows[i]["Privelage"].ToString());
            iPrivilege = Convert.ToInt32(v.tCihazPrivilage.prOrtakKulanici);

            if (sCardnumber != "" && sCardnumber != "0")
            {
                if (v.CihazIsActive)
                    SDK.SetStrCardNumber(sCardnumber);
            }

            // kullanıcı ismi bir defa kaydedilecek
            // kullanıcının birden fazla FP varsa o datalar ayrı gönderilir 
            if (sEnrollNumber != sLastEnrollNumber)
            {
                if (v.CihazIsActive)
                {
                    //upload user information to the device
                    if (SDK.SSR_SetUserInfo(iMachineNumber, sEnrollNumber, sName, sPassword, iPrivilege, bEnabled))
                    {
                        if (sdwFingerIndex != "" && sFlag != "" && sFPTmpData != "")
                        {
                            //upload templates information to the device
                            SDK.SetUserTmpExStr(iMachineNumber, sEnrollNumber, idwFingerIndex, iFlag, sFPTmpData);
                        }
                        num++;
                        //prgSta.Value = num % 100;
                    }
                    else SDK.GetLastError(ref idwErrorCode);
                }
                else
                {
                    // test tablosuna ekle
                    Sql.prc_CihazTestUser(iMachineNumber, t.myInt32(sEnrollNumber), sName, idwFingerIndex, sFPTmpData);
                }
            }
            else
            {
                // birden fazla olan FP burada cihaza gönderiliyor
                if (sdwFingerIndex != "" && sFlag != "" && sFPTmpData != "")
                {
                    if (v.CihazIsActive)
                    {
                        //upload templates information to the device
                        SDK.SetUserTmpExStr(iMachineNumber, sEnrollNumber, idwFingerIndex, iFlag, sFPTmpData);
                    }
                    else
                    {
                        // test tablosuna ekle
                        Sql.prc_CihazTestUser(iMachineNumber, t.myInt32(sEnrollNumber), sName, idwFingerIndex, sFPTmpData);
                    }
                }
                num++;
                //prgSta.Value = num % 100;
            }
            sLastEnrollNumber = sEnrollNumber;

            return idwErrorCode;
        }

        public int tBatch_DelUserFPInfo_(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataTable dT,
            int rowId,
            ref string sLastEnrollNumber
            )
        {
            int i = rowId;
            int idwErrorCode = 0;
            string sEnrollNumber = "";

            sEnrollNumber = dT.Rows[i]["CariId"].ToString();
            //sEnrollNumber = dT.Rows[i]["EnrollNumber"].ToString(); << test

            /// Delete enrolled data according to bakcupnumber:
            /// 0 - 9 : Fingerprint index, delete the specified fingerprint template.
            ///         If there is no any other fingerprints and password is empty,
            ///         then delete the user.
            /// 10 : Clear password. If there is no fingerprint, delete the user.
            /// 11 : Clear the user's all fingerprint templates.
            /// 12 : Delete the user.

            int iBackupNumber = 12;

            if (sEnrollNumber != sLastEnrollNumber)
            {
                if (SDK.SSR_DeleteEnrollData(iMachineNumber, sEnrollNumber, iBackupNumber))
                {
                    //SDK.RefreshData(iMachineNumber);//the data in the device should be refreshed
                    //lblOutputInfo.Items.Add("SSR_DeleteEnrollData,UserID=" + sUserID + " BackupNumber=" + iBackupNumber.ToString());
                }
                else
                {
                    SDK.GetLastError(ref idwErrorCode);
                    //if (idwErrorCode == 0 && iBackupNumber == 11)
                    //    lblOutputInfo.Items.Add("SSR_DeleteEnrollData,UserID=" + sUserID + " BackupNumber=" + iBackupNumber.ToString());
                    //else
                    //    lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
                }
            }

            sLastEnrollNumber = sEnrollNumber;
            return idwErrorCode;
        }

        public int sta_DelUserTmp(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataTable dT,
            int rowId,
            int iFingerIndex)
        {
            int i = rowId;
            int idwErrorCode = 0;
            string sUserID = "";

            sUserID = dT.Rows[i]["CariId"].ToString();

            // 0 - 9 : Fingerprint index, delete the specified fingerprint template.
            // 13    : Delete the user's all fingerprint templates.

            if (SDK.SSR_DelUserTmpExt(iMachineNumber, sUserID, iFingerIndex))
            {
                //SDK.RefreshData(iMachineNumber);//the data in the device should be refreshed
                //lblOutputInfo.Items.Add("SSR_DelUserTmpExt,UserID:" + sUserID + " FingerIndex:" + iFingerIndex.ToString());
            }
            else
            {
                SDK.GetLastError(ref idwErrorCode);
                //lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            }

            return 1;
        }


        /// user / Kullanıcı bilgi ve parmak izlerini okuma
        public bool tGetAllUserFPInfo(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataSet dT,
            bool dbSave
            )
        {
            bool onay = false;
            string sEnrollNumber = "";
            bool bEnabled = false;
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            string sFPTmpData = "";
            string sCardnumber = "";
            int idwFingerIndex = 0;
            int iFlag = 0;
            int iFPTmpLength = 0;
            int i = 0;
            int num = 0;
            int iFpCount = 0;
            int index = 0;
            int xx = 1;
            int count = 0;
            DataRow dRow = null;

            if (dT.Tables[0].Rows.Count > 0) 
                dT.Tables[0].Rows.Clear();

            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            v.progressBar1.Value = 20;
            Application.DoEvents();
            
            SDK.EnableDevice(iMachineNumber, false);


            v.progressBar1.Value = 40;
            Application.DoEvents();
            
            // read all the user information to the memory  except fingerprint Templates
            // parmak izi Şablonları hariç tüm kullanıcı bilgilerini belleğe okuyun
            SDK.ReadAllUserID(iMachineNumber);
            count = getUserCount_(SDK, iMachineNumber);
            SDK.ReadAllUserID(iMachineNumber);


            v.progressBar1.Value = 75;
            Application.DoEvents();

            // read all the users' fingerprint templates to the memory
            // tüm kullanıcıların parmak izi şablonlarını belleğe okuyun
            SDK.ReadAllTemplate(iMachineNumber);

            v.progressBar1.Value = 100;
            Application.DoEvents();
            
            v.progressBar1.Maximum = count;

            // get all the users' information from the memory
            // tüm kullanıcıların bilgilerini bellekten al

            while (SDK.SSR_GetAllUserInfo(iMachineNumber, out sEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
            {
                // ???????
                // get the card number from the memory  
                // kart numarasını hafızadan al
                //SDK.GetStrCardNumber(out sCardnumber);          
                
                dRow = dT.Tables[0].NewRow();
                dRow["Id"] = num;
                dRow["MachineNumber"] = iMachineNumber;
                dRow["EnrollNumber"] = sEnrollNumber;
                if (bEnabled == true)
                    dRow["Enabled"] = true;
                else dRow["Enabled"] = false;
                dRow["Name"] = sName;
                dRow["CardNumber"] = sCardnumber;
                dRow["Password"] = sPassword;
                
                i = 0;
                xx = 1;
                
                for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                {
                    // get the corresponding templates string and length from the memory
                    // ilgili şablonlar dizesini ve uzunluğunu bellekten al
                    if (SDK.GetUserTmpExStr(iMachineNumber, sEnrollNumber, idwFingerIndex, out iFlag, out sFPTmpData, out iFPTmpLength))
                    {
                        if (xx == 1)
                        {
                            dRow["FingerIndex"] = idwFingerIndex;
                            dRow["iFlag"] = iFlag.ToString();
                            dRow["TmpData"] = sFPTmpData;
                            dRow["Privelage"] = iPrivilege;
                        }
                        else
                        {
                            dRow = dT.Tables[0].NewRow();
                            dRow["MachineNumber"] = iMachineNumber;
                            dRow["EnrollNumber"] = sEnrollNumber;
                            if (bEnabled == true)
                                 dRow["Enabled"] = true;
                            else dRow["Enabled"] = false;
                            dRow["Name"] = sName;
                            dRow["CardNumber"] = sCardnumber;
                            dRow["Password"] = sPassword;
                            dRow["FingerIndex"] = idwFingerIndex;
                            dRow["iFlag"] = iFlag.ToString();
                            dRow["TmpData"] = sFPTmpData;
                            dRow["Privelage"] = iPrivilege;
                        }
                        dT.Tables[0].Rows.Add(dRow);

                        // database kaydet
                        if (dbSave)
                        {
                            // bu geçic procedure
                            Sql.prc_SetCariHesap(t.myInt32(sEnrollNumber), sName);

                            Sql.prc_SetCariBio(t.myInt32(sEnrollNumber), idwFingerIndex, sFPTmpData);
                        }
                        index++;
                        xx = 0;
                        iFpCount++;
                        onay = true;
                    }
                    else
                    {
                        i++;
                    }
                }
                
                if (i == 10)
                {
                    // parmak izi olmayanlarıda ekliyor
                    // eğer parmak izleri olmayanları istemiyorsan ya burayı kapat yada parametreye bağl
                    dT.Tables[0].Rows.Add(dRow);
                    index++;
                }
                
                num++;
                //prgSta.Value = num % 100;
                v.progressBar1.Value = num;// % count;
                Application.DoEvents();
            }

            //MessageBox.Show(count.ToString() + "; num : " + num.ToString());
            //prgSta.Value = 100;
            //lblOutputInfo.Items.Add("Download user count : " + num.ToString() + " ,  fingerprint count : " + iFpCount.ToString());

            SDK.EnableDevice(iMachineNumber, true);

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            v.progressBar1.Value = 0;

            return onay;
        }



        public int tCountAllUserFPInfo(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            ref int fpCount
            )
        {
            string sEnrollNumber = "";
            bool bEnabled = false;
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            string sFPTmpData = "";
            int idwFingerIndex = 0;
            int iFlag = 0;
            int iFPTmpLength = 0;
            int iFpCount = 0;
            int iUserCount = 0;

            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            if (v.CihazIsActive)
            {
                SDK.EnableDevice(iMachineNumber, false);

                // read all the user information to the memory  except fingerprint Templates
                // parmak izi Şablonları hariç tüm kullanıcı bilgilerini belleğe okuyun
                SDK.ReadAllUserID(iMachineNumber);

                // read all the users' fingerprint templates to the memory
                // tüm kullanıcıların parmak izi şablonlarını belleğe okuyun
                SDK.ReadAllTemplate(iMachineNumber);

                // get all the users' information from the memory
                // tüm kullanıcıların bilgilerini bellekten al
                while (SDK.SSR_GetAllUserInfo(iMachineNumber, out sEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
                {
                    for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                    {
                        // get the corresponding templates string and length from the memory
                        // ilgili şablonlar dizesini ve uzunluğunu bellekten al
                        if (SDK.GetUserTmpExStr(iMachineNumber, sEnrollNumber, idwFingerIndex, out iFlag, out sFPTmpData, out iFPTmpLength))
                        {
                            iFpCount++;
                        }
                    }
                    iUserCount++;
                }

                SDK.EnableDevice(iMachineNumber, true);
            }
            else
            {
                // test tablosunu oku
                Sql.cihazTestUserCountSql(iMachineNumber, ref iUserCount, ref iFpCount);
            }

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            fpCount = iFpCount;
            return iUserCount;
        }

        /// cihaz üzerindeki kullanıcı sayısı
        public int tGetAllUserCount(
            fingerK50_SDKHelper SDK,
            int iMachineNumber)
        {
            int count = 0;
            
            SDK.EnableDevice(iMachineNumber, false);

            // read all the user information to the memory  except fingerprint Templates
            // parmak izi Şablonları hariç tüm kullanıcı bilgilerini belleğe okuyun
            SDK.ReadAllUserID(iMachineNumber);

            // read all the users' fingerprint templates to the memory
            // tüm kullanıcıların parmak izi şablonlarını belleğe okuyun
            // SDK.ReadAllTemplate(iMachineNumber);

            count = getUserCount_(SDK, iMachineNumber);

            SDK.EnableDevice(iMachineNumber, true);

            return count;
        }

        public int getUserCount_(
            fingerK50_SDKHelper SDK,
            int iMachineNumber)
        {
            int count = 0;
            string sEnrollNumber = "";
            bool bEnabled = false;
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;

            // get all the users' information from the memory
            // tüm kullanıcıların bilgilerini bellekten al
            while (SDK.SSR_GetAllUserInfo(iMachineNumber, out sEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
            {
                count++;
            }
            //SDK.RefreshData(iMachineNumber);
            return count;
        }


        #endregion User / Kullanıcı bilgileri

        #region Data bilgisi okuma

        public void tGetAllUserAndFPs(int cihazId, DataSet table)
        {
            fingerK50_SDKHelper SDK = null;

            SDK = getCihazSdk(cihazId);
            if (SDK != null)
            {
                if (v.CihazIsActive)
                {
                    tGetAllUserFPInfo(SDK, cihazId, table, false);
                }
                else
                {
                    tGetAllUserFPInfoCihazTest(SDK, cihazId, table, false);
                }
                disConnectCihazSdk(cihazId);
            }

        }

        public void tGetAllLogs(int cihazId, DataSet table, IslemTarihi islemTarihi)
        {
            fingerK50_SDKHelper SDK = null;

            SDK = getCihazSdk(cihazId);
            if (SDK != null)
            {
                if (v.CihazIsActive)
                {
                    tReadAttLog(SDK, cihazId, table, islemTarihi);
                }
                else
                {
                    tReadCihazTestLog(SDK, cihazId, table, islemTarihi);
                }
                disConnectCihazSdk(cihazId);
            }    
        }


        public bool talepRun(
            DataSet cihazTable, 
            v.cihazCalismaTipi calismaTipi,
            DataSet table, 
            v.cihazTalepTipi talep,
            IslemTarihi islemTarihi)
        {
            fingerK50_SDKHelper SDK = null;

            bool onay = false;
            int cihazId = 0;
            int cihazCalismaTipiId = 0;
            string myLog = talepNameGet(talep);

            if (v.listBox1 != null)
                v.listBox1.Items.Add(".... " + myLog + " : işlemi başladı ...");

            v.progressBar1.Value = 0;

            v.cihazCalismaTipi value = v.cihazCalismaTipi.None;

            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;


            /*  silme 
             *  trigger yapması gerekenleri işler için trigger Enable olduktan sonra 
                update çaılşması gerekiyor.
                ? : Hangi kayıtlar update olacak

            if ((talep == v.cihazTalepTipi.chGetGorev) ||
                (talep == v.cihazTalepTipi.chGetGorus) ||
                (talep == v.cihazTalepTipi.chGetSayim))
                t.tTriggerOnOff("CihazKaydi", "trg_CihazKaydi", v.tEnabled.Disable);
            */

            foreach (DataRow row in cihazTable.Tables[0].Rows)
            {
                cihazCalismaTipiId = t.myInt32(row["CalismaTipiId"].ToString());

                if (cihazCalismaTipiId == 11) value = v.cihazCalismaTipi.Kayit;
                
                ///if (cihazCalismaTipiId == 12) value = v.cihazCalismaTipi.Giris;
                ///if (cihazCalismaTipiId == 13) value = v.cihazCalismaTipi.Cikis;
                ///if (cihazCalismaTipiId == 14) value = v.cihazCalismaTipi.GirisCikis;
                if ((cihazCalismaTipiId == 12) || 
                    (cihazCalismaTipiId == 13) ||
                    (cihazCalismaTipiId == 14)) value = v.cihazCalismaTipi.GirisCikis;

                if (cihazCalismaTipiId == 15) value = v.cihazCalismaTipi.Gorus;
                if (cihazCalismaTipiId == 16) value = v.cihazCalismaTipi.Sayim;

                if (calismaTipi == value)
                {
                    //cihazId = t.myInt32(row["LkpCihazId"].ToString());
                    cihazId = t.myInt32(row["CihazId"].ToString());
                    SDK = getCihazSdk(cihazId);
                    if (SDK != null)
                    {
                        if (v.CihazIsActive)
                        {
                            // kayıt cihazındaki bio tanımlama kayıtlarını oku database yaz
                            if (talep == v.cihazTalepTipi.chGetNewUserFP)
                                onay = tGetAllUserFPInfo(SDK, cihazId, table, true);

                            // kayıt cihazını komple boşalt / sil
                            if (talep == v.cihazTalepTipi.chDelUserFPLog)
                                onay = sta_ClearAllData(cihazId, ref myLog);

                            if ((talep == v.cihazTalepTipi.chGetGorev) ||
                                (talep == v.cihazTalepTipi.chGetGorus) ||
                                (talep == v.cihazTalepTipi.chGetSayim))
                                onay = tReadAttLog(SDK, cihazId, table, islemTarihi);
                        }
                        else
                        {
                            // yeni test tabloları eklenecek

                            if (talep == v.cihazTalepTipi.chGetNewUserFP)
                                onay = tGetAllUserFPInfoCihazTest(SDK, cihazId, table, true);

                            if (talep == v.cihazTalepTipi.chDelUserFPLog)
                                onay = Sql.cihazTestAllClearSql(cihazId, ref myLog);

                            if ((talep == v.cihazTalepTipi.chGetGorev) ||
                                (talep == v.cihazTalepTipi.chGetGorus) ||
                                (talep == v.cihazTalepTipi.chGetSayim))
                                onay = tReadCihazTestLog(SDK, cihazId, table, islemTarihi);
                        }
                        disConnectCihazSdk(cihazId);
                    }
                }

            }

            /* silme
             * trigger yapması gerekenleri işler için trigger Enable olduktan sonra 
               update çalışması gerekiyor.
             ? : Hangi kayıtlar update olacak
             * 
            if ((talep == v.cihazTalepTipi.chGetGorev) ||
                (talep == v.cihazTalepTipi.chGetGorus) ||
                (talep == v.cihazTalepTipi.chGetSayim))
                t.tTriggerOnOff("CihazKaydi", "trg_CihazKaydi", v.tEnabled.Enable);
             */

            v.progressBar1.Value = 0;

            if (v.listBox1 != null)
                v.listBox1.Items.Add(".... " + myLog + " : işlemi bitti ...");

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            return onay;
        }


        /// iki saat arasındaki datayı okumak
        public int tReadLogByPeriod(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataTable dt_logPeriod,
            string fromTime, 
            string toTime)
        {
            /*
            if (GetConnectState() == false)
            {
                lblOutputInfo.Items.Add("*Please connect first!");
                return -1024;
            }
            */

            int ret = 0;

            //disable the device
            SDK.EnableDevice(iMachineNumber, false); // (GetMachineNumber(), false);

            string sdwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            int idwErrorCode = 0;

            // bu da çalışmıyor
            if (SDK.ReadTimeGLogData(iMachineNumber, fromTime, toTime))
            {
                //get records from the memory
                while (SDK.SSR_GetGeneralLogData(iMachineNumber, 
                    out sdwEnrollNumber, 
                    out idwVerifyMode,
                    out idwInOutMode, 
                    out idwYear, 
                    out idwMonth, 
                    out idwDay, 
                    out idwHour, 
                    out idwMinute, 
                    out idwSecond, 
                    ref idwWorkcode))
                {
                    DataRow dr = dt_logPeriod.NewRow();
                    dr["MachineNumber"] = iMachineNumber;
                    dr["UserID"] = sdwEnrollNumber;
                    dr["VerifyDate"] = idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond;
                    dr["VerifyType"] = idwVerifyMode;
                    dr["VerifyState"] = idwInOutMode;
                    dr["WorkCode"] = idwWorkcode;
                    dt_logPeriod.Rows.Add(dr);
                }
                ret = 1;
            }
            else
            {
                SDK.GetLastError(ref idwErrorCode);
                ret = idwErrorCode;

                if (idwErrorCode != 0)
                {
                    //lblOutputInfo.Items.Add("*Read attlog by period failed,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
            }


            //lblOutputInfo.Items.Add("[func ReadTimeGLogData]Temporarily unsupported");
            SDK.EnableDevice(iMachineNumber, true);//enable the device

            return ret;
        }



        public int tReadAttLogCount(
            fingerK50_SDKHelper SDK,
            int iMachineNumber
            )
        {
            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;
            
            int count = 0;

            if (v.CihazIsActive)
            {
                //disable the device
                SDK.EnableDevice(iMachineNumber, false);

                string sdwEnrollNumber = "";
                int idwVerifyMode = 0;
                int idwInOutMode = 0;
                int idwYear = 0;
                int idwMonth = 0;
                int idwDay = 0;
                int idwHour = 0;
                int idwMinute = 0;
                int idwSecond = 0;
                int idwWorkcode = 0;
                
                //if (SDK.ReadGeneralLogData(iMachineNumber))
                if (SDK.ReadAllGLogData(iMachineNumber))
                {

                    //get records from the memory
                    while (SDK.SSR_GetGeneralLogData(iMachineNumber,
                        out sdwEnrollNumber,
                        out idwVerifyMode,
                        out idwInOutMode,
                        out idwYear,
                        out idwMonth,
                        out idwDay,
                        out idwHour,
                        out idwMinute,
                        out idwSecond,
                        ref idwWorkcode))
                    {
                        count++;
                    }

                }

                // enable the device
                SDK.EnableDevice(iMachineNumber, true);
            }
            else
            {
                /// test tablosunu oku
                Sql.cihazTestLogCountSql(iMachineNumber, ref count);
            }

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            return count;
        }



        /// tüm datayı oku
        public bool tReadAttLog(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataSet dt_log,
            IslemTarihi islemTarihi
            )
        {
            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            //disable the device
            SDK.EnableDevice(iMachineNumber, false);

            bool onay = false;
            //int ret = 0;
            int num = 0;
            string sdwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            int idwErrorCode = 0;

            if (dt_log.Tables[0].Rows.Count > 0)
                dt_log.Tables[0].Rows.Clear();

            v.progressBar1.Maximum = 100;
            v.progressBar1.Value = 50;
            Application.DoEvents();

            int islemGun = (islemTarihi.Yil * 365) + (islemTarihi.Ay * 30) + islemTarihi.Gun;
            int okunanGun = 0;

            if (v.listBox1 != null)
                v.listBox1.Items.Add("Cihaz üzerindeki Log datası okunuyor ....");

            //if (SDK.ReadGeneralLogData(iMachineNumber))
            if (SDK.ReadAllGLogData(iMachineNumber))
            {

                //get records from the memory
                while (SDK.SSR_GetGeneralLogData(iMachineNumber, 
                    out sdwEnrollNumber, 
                    out idwVerifyMode,
                    out idwInOutMode, 
                    out idwYear, 
                    out idwMonth, 
                    out idwDay, 
                    out idwHour, 
                    out idwMinute, 
                    out idwSecond, 
                    ref idwWorkcode))
                {
                    okunanGun = (idwYear * 365) + (idwMonth * 30) + idwDay;

                    //if ((idwYear >= islemTarihi.Yil) &&
                    //    (idwMonth >= islemTarihi.Ay) &&
                    //    (idwDay >= islemTarihi.Gun))
                    if (okunanGun >= islemGun)
                    {
                        DataRow dr = dt_log.Tables[0].NewRow();
                        dr["MachineNumber"] = iMachineNumber;
                        dr["UserID"] = sdwEnrollNumber;
                        dr["VerifyDate"] = idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond;
                        //dr["VerifyType"] = idwVerifyMode;
                        //dr["VerifyState"] = idwInOutMode;
                        //dr["WorkCode"] = idwWorkcode;
                        dt_log.Tables[0].Rows.Add(dr);

                        // okunan datanın kaydı tutuluyor
                        Sql.prc_CihazKaydiSet(iMachineNumber, Convert.ToInt32(sdwEnrollNumber), (idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond));
                    }
                    num++;
                    v.progressBar1.Value = num % 100;
                    Application.DoEvents();

                    //if (num == 200) break;  

                }
                onay = true;
            }
            else
            {
                SDK.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    onay = false;
                    //lblOutputInfo.Items.Add("*Read attlog failed,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    onay = true;
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
            }

            v.progressBar1.Value = 0;

            // enable the device
            SDK.EnableDevice(iMachineNumber, true);

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            return onay;
        }

        /// sadece yeni datayı oku
        public int tReadNewAttLog(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataTable dt_logNew
            )
        {
            /*
            if (GetConnectState() == false)
            {
                lblOutputInfo.Items.Add("Please connect first!");
                return -1024;
            }
            */

            int ret = 0;

            SDK.EnableDevice(iMachineNumber, false);//disable the device

            string sdwEnrollNumber = "";
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            int idwErrorCode = 0;

            // bu çalışmıyor

            //if (SDK.ReadNewGLogData(iMachineNumber))
            if (SDK.ReadNewGLogData(iMachineNumber))
            {
                // get records from the memory
                while (SDK.SSR_GetGeneralLogData(iMachineNumber, 
                    out sdwEnrollNumber, 
                    out idwVerifyMode, 
                    out idwInOutMode, 
                    out idwYear, 
                    out idwMonth, 
                    out idwDay, 
                    out idwHour, 
                    out idwMinute, 
                    out idwSecond, 
                    ref idwWorkcode))
                {
                    DataRow dr = dt_logNew.NewRow();
                    dr["MachineNumber"] = iMachineNumber;
                    dr["UserID"] = sdwEnrollNumber;
                    dr["VerifyDate"] = idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond;
                    dr["VerifyType"] = idwVerifyMode;
                    dr["VerifyState"] = idwInOutMode;
                    dr["WorkCode"] = idwWorkcode;
                    dt_logNew.Rows.Add(dr);
                }
                ret = 1;
            }
            else
            {
                SDK.GetLastError(ref idwErrorCode);
                ret = idwErrorCode;

                if (idwErrorCode != 0)
                {
                    //lblOutputInfo.Items.Add("*Read attlog by period failed,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
            }

            //lblOutputInfo.Items.Add("[func ReadNewGLogData]Temporarily unsupported");
            
            //enable the device
            SDK.EnableDevice(iMachineNumber, true);

            return ret;
        }


        public bool tReadCihazTestLog(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataSet dt_log,
            IslemTarihi islemTarihi
            )
        {
            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            //disable the device
            //SDK.EnableDevice(iMachineNumber, false);

            bool onay = false;
            int num = 0;
            string sdwEnrollNumber = "";
            //int idwVerifyMode = 0;
            //int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            //int idwWorkcode = 0;

            //int idwErrorCode = 0;

            if (dt_log.Tables[0].Rows.Count > 0)
                dt_log.Tables[0].Rows.Clear();

            v.progressBar1.Maximum = 100;
            v.progressBar1.Value = 50;
            Application.DoEvents();

            int islemGun = (islemTarihi.Yil * 365) + (islemTarihi.Ay * 30) + islemTarihi.Gun;
            
            int okunanGun = 0;
            DateTime okunanTarih = DateTime.Now.Date;

            if (v.listBox1 != null)
                v.listBox1.Items.Add("Cihaz üzerindeki Log datası okunuyor ....");

            //if (SDK.ReadGeneralLogData(iMachineNumber))
            if (ReadAllCihazTestLogData(iMachineNumber))//(SDK.ReadAllGLogData(iMachineNumber))
            {
                foreach (DataRow row in v.dsCihazTest.Tables[0].Rows)
                {
                    okunanTarih = Convert.ToDateTime(row["TarihSaat"].ToString());

                    idwYear = okunanTarih.Year;
                    idwMonth = okunanTarih.Month;
                    idwDay = okunanTarih.Day;

                    okunanGun = (idwYear * 365) + (idwMonth * 30) + idwDay;

                    if (okunanGun >= islemGun)
                    {
                        sdwEnrollNumber = row["CariId"].ToString();
                        idwHour = okunanTarih.Hour;
                        idwMinute = okunanTarih.Minute;
                        idwSecond = okunanTarih.Second;


                        DataRow dr = dt_log.Tables[0].NewRow();
                        dr["MachineNumber"] = iMachineNumber;
                        dr["UserID"] = sdwEnrollNumber;
                        dr["VerifyDate"] = idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond;
                        //dr["VerifyType"] = idwVerifyMode;
                        //dr["VerifyState"] = idwInOutMode;
                        //dr["WorkCode"] = idwWorkcode;
                        dt_log.Tables[0].Rows.Add(dr);

                        // okunan datanın kaydı tutuluyor
                        Sql.prc_CihazKaydiSet(iMachineNumber, Convert.ToInt32(sdwEnrollNumber), (idwYear + "-" + idwMonth + "-" + idwDay + " " + idwHour + ":" + idwMinute + ":" + idwSecond));

                    }
                    num++;
                    v.progressBar1.Value = num % 100;
                    Application.DoEvents();

                }
                                
                //ret = 1;
                onay = true;
            }
            else
            {
                //SDK.GetLastError(ref idwErrorCode);
                //return = = idwErrorCode;
                onay = false;
            }

            v.progressBar1.Value = 0;

            // enable the device
            //SDK.EnableDevice(iMachineNumber, true);
            
            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;
            
            return onay;
        }
        
        private bool ReadAllCihazTestLogData(int iMachineNumber)
        {
            /// bu okuma tablosunu view etme
            /// 
            bool onay = true;
            string tSql = Sql.cihazTestLogSql(ref v.dsCihazTest, iMachineNumber);

            try
            {
                if (v.dsCihazTest.Tables.Count > 0)
                {
                    if (v.dsCihazTest.Tables[0].Rows.Count > 0)
                        v.dsCihazTest.Tables[0].Clear();
                }

                t.Data_Read_Execute(v.dsCihazTest, ref tSql, "CihazTestLog", null);
            }
            catch (Exception)
            {
                onay = false;
                throw;
            }
            return onay;
        }

        public bool tGetAllUserFPInfoCihazTest(
            fingerK50_SDKHelper SDK,
            int iMachineNumber,
            DataSet dT,
            bool dbSave
            )
        {
            bool onay = false;
            string sEnrollNumber = "";
            //bool bEnabled = false;
            string sName = "";
            //string sPassword = "";
            //int iPrivilege = 0;
            string sFPTmpData = "";
            //string sCardnumber = "";
            int idwFingerIndex = 0;
            int iFlag = 0;
            //int iFPTmpLength = 0;
            //int i = 0;
            int num = 0;
            //int iFpCount = 0;
            //int index = 0;
            //int xx = 1;
            int count = 0;
            DataRow dRow = null;

            if (dT.Tables[0].Rows.Count > 0)
                dT.Tables[0].Rows.Clear();

            if (Cursor.Current == Cursors.Default)
                Cursor.Current = Cursors.WaitCursor;

            v.progressBar1.Value = 20;
            Application.DoEvents();

            //SDK.EnableDevice(iMachineNumber, false);

            v.progressBar1.Value = 40;
            Application.DoEvents();

            // read all the user information to the memory  except fingerprint Templates
            // parmak izi Şablonları hariç tüm kullanıcı bilgilerini belleğe okuyun
            //SDK.ReadAllUserID(iMachineNumber);
            //count = getUserCount_(SDK, iMachineNumber);
            //SDK.ReadAllUserID(iMachineNumber);
                        
            v.progressBar1.Value = 75;
            Application.DoEvents();

            // read all the users' fingerprint templates to the memory
            // tüm kullanıcıların parmak izi şablonlarını belleğe okuyun
            //SDK.ReadAllTemplate(iMachineNumber);

            count = 0;
            GetAllCihazTestUserData(iMachineNumber);
            if (v.dsCihazTest.Tables.Count > 0)
                count = v.dsCihazTest.Tables[0].Rows.Count;

            v.progressBar1.Value = 100;
            Application.DoEvents();

            v.progressBar1.Maximum = count;

            // get all the users' information from the memory
            // tüm kullanıcıların bilgilerini bellekten al

            if (v.listBox1 != null)
                v.listBox1.Items.Add("Cihaz üzerindeki kullanıcı ve bio datası okunuyor ....");

            foreach (DataRow row in v.dsCihazTest.Tables[0].Rows)
            {
                sEnrollNumber = row["CariId"].ToString();
                sName = row["UserName"].ToString();
                idwFingerIndex = t.myInt32(row["BioIndex"].ToString());
                iFlag = 0;
                sFPTmpData = row["BioPrint"].ToString();

                dRow = dT.Tables[0].NewRow();
                dRow["Id"] = num;
                dRow["MachineNumber"] = iMachineNumber;
                dRow["EnrollNumber"] = sEnrollNumber;
                dRow["Enabled"] = true;
                dRow["FingerIndex"] = idwFingerIndex;
                dRow["iFlag"] = iFlag.ToString();
                dRow["TmpData"] = sFPTmpData;

                dT.Tables[0].Rows.Add(dRow);

                // database kaydet
                if (dbSave)
                {
                    // bu geçic procedure
                    Sql.prc_SetCariHesap(t.myInt32(sEnrollNumber), sName);

                    Sql.prc_SetCariBio(t.myInt32(sEnrollNumber), idwFingerIndex, sFPTmpData);
                }

                num++;
                //prgSta.Value = num % 100;
                v.progressBar1.Value = num;// % count;
                Application.DoEvents();

                onay = true;
            }

            if (Cursor.Current == Cursors.WaitCursor)
                Cursor.Current = Cursors.Default;

            v.progressBar1.Value = 0;

            return onay;
        }

        private bool GetAllCihazTestUserData(int iMachineNumber)
        {
            /// bu okuma tablosunu view etme
            /// 
            bool onay = true;
            string tSql = Sql.cihazTestUserSql(ref v.dsCihazTest, iMachineNumber);

            try
            {
                if (v.dsCihazTest.Tables.Count > 0)
                {
                    if (v.dsCihazTest.Tables[0].Rows.Count > 0)
                        v.dsCihazTest.Tables[0].Clear();
                }

                t.Data_Read_Execute(v.dsCihazTest, ref tSql, "CihazTestUser", null);
            }
            catch (Exception)
            {
                onay = false;
                throw;
            }
            return onay;
        }

        #endregion data okuma

        #region Data Silme 

        /// iki saat arasındaki datayı sil
        public int tDeleteAttLogByPeriod(fingerK50_SDKHelper SDK,
            int iMachineNumber,
            string fromTime,
            string toTime)
        {
            /*
            if (GetConnectState() == false)
            {
                lblOutputInfo.Items.Add("*Please connect first!");
                return -1024;
            }
            */

            int ret = 0;
            int idwErrorCode = 0;

            //disable the device
            SDK.EnableDevice(iMachineNumber, false);
            
            if (SDK.DeleteAttlogBetweenTheDate(iMachineNumber, fromTime, toTime))
            {
                SDK.RefreshData(iMachineNumber);
                ret = 1;
            }
            else
            {
                SDK.GetLastError(ref idwErrorCode);
                ret = idwErrorCode;

                if (idwErrorCode != 0)
                {
                    //lblOutputInfo.Items.Add("*Delete attlog by period failed,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
            }

            //lblOutputInfo.Items.Add("[func DeleteAttlogBetweenTheDate]Temporarily unsupported");

            //enable the device
            SDK.EnableDevice(iMachineNumber, true);

            return ret;
        }

        /// tüm datayı sil
        public int tDeleteAttLog(fingerK50_SDKHelper SDK,
            int iMachineNumber)
        {
            /*
            if (GetConnectState() == false)
            {
                lblOutputInfo.Items.Add("*Please connect first!");
                return -1024;
            }
            */
            int ret = 0;
            int idwErrorCode = 0;

            //disable the device
            SDK.EnableDevice(iMachineNumber, false);


            if (SDK.ClearGLog(iMachineNumber))
            {
                SDK.RefreshData(iMachineNumber);
                ret = 1;
            }
            else
            {
                SDK.GetLastError(ref idwErrorCode);
                ret = idwErrorCode;

                if (idwErrorCode != 0)
                {
                    //lblOutputInfo.Items.Add("*Delete attlog, ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
            }
            // enable the device
            SDK.EnableDevice(iMachineNumber, true);

            return ret;
        }

        /// belirtilen saat öncesi datayı sil
        public int tDelOldAttLogFromTime(fingerK50_SDKHelper SDK,
            int iMachineNumber,
            string fromTime)
        {
            /*
            if (GetConnectState() == false)
            {
                lblOutputInfo.Items.Add("Please connect first!");
                return -1024;
            }
            */
            int ret = 0;
            int idwErrorCode = 0;

            //disable the device
            SDK.EnableDevice(iMachineNumber, false);
            
            if (SDK.DeleteAttlogByTime(iMachineNumber, fromTime))
            {
                SDK.RefreshData(iMachineNumber);
                ret = 1;
            }
            else
            {
                SDK.GetLastError(ref idwErrorCode);
                ret = idwErrorCode;

                if (idwErrorCode != 0)
                {
                    //lblOutputInfo.Items.Add("*Delete old attlog from time failed,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
            }

            //lblOutputInfo.Items.Add("[func DeleteAttlogByTime]Temporarily unsupported");

            //enable the device
            SDK.EnableDevice(iMachineNumber, true);

            return ret;
        }

        #endregion Data Silme

        #region silme işlemleri

        #region ClearData
        // cihazdan sadece bir kullanıcıyı sil
        public bool sta_DeleteUserFromMachine(fingerK50_SDKHelper objZkeeper, int iMachineNumber, int userId)
        {
            return objZkeeper.SSR_DeleteEnrollData(iMachineNumber, userId.ToString(), 0);
        }

        public int sta_ClearAdmin(int iMachineNumber)
        {
            int ret = 0;
            int idwErrorCode = 0;

            fingerK50_SDKHelper axCZKEM1 = getCihazSdk(iMachineNumber);

            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (axCZKEM1.ClearAdministrators(iMachineNumber))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                //lblOutputInfo.Items.Add("All administrator have been cleared from teiminal!");
                ret = 1;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                if (idwErrorCode != 0)
                {
                    //lblOutputInfo.Items.Add("*ClearAdmin failed,ErrorCode=" + idwErrorCode.ToString());
                }
                else
                {
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
                ret = idwErrorCode;
            }

            axCZKEM1.EnableDevice(iMachineNumber, true);

            disConnectCihazSdk(iMachineNumber);

            return ret;
        }

        public bool sta_ClearAllLogs(int iMachineNumber, ref string log)
        {
            bool onay = false;
            int ret = 0;
            int idwErrorCode = 0;

            fingerK50_SDKHelper axCZKEM1 = getCihazSdk(iMachineNumber);

            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (axCZKEM1.ClearData(iMachineNumber, 1))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                //lblOutputInfo.Items.Add("All AttLogs have been cleared from teiminal!");
                log = iMachineNumber.ToString() + " nolu cihazın Log datası başarıyla silindi ...";
                onay = true;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                if (idwErrorCode != 0)
                {
                    log = iMachineNumber.ToString() + " nolu cihazın Log datası silinirken hata oluştu ...( ErrorCode = " + idwErrorCode.ToString();
                    onay = true;
                    //lblOutputInfo.Items.Add("*ClearAllLogs failed,ErrorCode=" + idwErrorCode.ToString());
                }
                else
                {
                    log = iMachineNumber.ToString() + " nolu cihazda veri yok ";
                    onay = false;
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
                ret = idwErrorCode;
            }

            axCZKEM1.EnableDevice(iMachineNumber, true);

            disConnectCihazSdk(iMachineNumber);

            return onay;
        }

        public bool sta_ClearAllFps(int iMachineNumber, ref string log)
        {
            bool onay = false;
            int ret = 0;
            int idwErrorCode = 0;

            fingerK50_SDKHelper axCZKEM1 = getCihazSdk(iMachineNumber);

            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (axCZKEM1.ClearData(iMachineNumber, 2))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                //lblOutputInfo.Items.Add("All fp templates have been cleared from teiminal!");
                log = iMachineNumber.ToString() + " nolu cihazın Kullanıcılara ait Bio datası başarıyla silindi ...";
                onay = true;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                if (idwErrorCode != 0)
                {
                    log = iMachineNumber.ToString() + " nolu cihazın Bio datası silinirken hata oluştu ...( ErrorCode = " + idwErrorCode.ToString();
                    onay = false;
                    //lblOutputInfo.Items.Add("*ClearAllFps failed,ErrorCode=" + idwErrorCode.ToString());
                }
                else
                {
                    log = iMachineNumber.ToString() + " nolu cihazda kullanıcıya ait bio verisi yok ";
                    onay = true;
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
                ret = idwErrorCode;
            }

            axCZKEM1.EnableDevice(iMachineNumber, true);

            disConnectCihazSdk(iMachineNumber);

            return onay;
        }

        public bool sta_ClearAllUsers(int iMachineNumber, ref string log)
        {
            bool onay = false;
            int ret = 0;
            int idwErrorCode = 0;

            fingerK50_SDKHelper axCZKEM1 = getCihazSdk(iMachineNumber);

            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (axCZKEM1.ClearData(iMachineNumber, 5))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                //lblOutputInfo.Items.Add("All users have been cleared from teiminal!");
                log = iMachineNumber.ToString() + " nolu cihazın Kullanıcı datası başarıyla silindi ...";
                onay = true;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                if (idwErrorCode != 0)
                {
                    //lblOutputInfo.Items.Add("*ClearAllUsers failed,ErrorCode=" + idwErrorCode.ToString());
                    log = iMachineNumber.ToString() + " nolu cihazın Kullanıcı datası silinirken hata oluştu ...( ErrorCode = " + idwErrorCode.ToString();
                    onay = false;
                }
                else
                {
                    log = iMachineNumber.ToString() + " nolu cihazda kullanıcı verisi yok ";
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                    onay = true;
                }
                ret = idwErrorCode;
            }

            axCZKEM1.EnableDevice(iMachineNumber, true);

            disConnectCihazSdk(iMachineNumber);

            return onay;
        }
                
        public bool sta_ClearAllData(int iMachineNumber, ref string log)
        {
            bool onay = false;
            string log_ = "";
            onay = sta_ClearAllLogs(iMachineNumber, ref log_);
            log = log + log_ + v.ENTER;
            onay = sta_ClearAllFps(iMachineNumber, ref log_);
            log = log + log_ + v.ENTER;
            onay = sta_ClearAllUsers(iMachineNumber, ref log_);
            log = log + log_ + v.ENTER;

            /*
            int ret = 0;
            int idwErrorCode = 0;

            fingerK50_SDKHelper axCZKEM1 = getCihazSdk(iMachineNumber);

            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (axCZKEM1.ClearKeeperData(iMachineNumber))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                //lblOutputInfo.Items.Add("All Data have been cleared from teiminal!");
                ret = 1;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                if (idwErrorCode != 0)
                {
                    //lblOutputInfo.Items.Add("*ClearAllData failed,ErrorCode=" + idwErrorCode.ToString());
                }
                else
                {
                    //lblOutputInfo.Items.Add("No data from terminal returns!");
                }
                ret = idwErrorCode;
            }

            axCZKEM1.EnableDevice(iMachineNumber, true);

            disConnectCihazSdk(iMachineNumber);
            */
            return onay;
        }

        #endregion

        #endregion silme işlemleri

        #endregion orjinal fonksiyonlar

        public void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        //ShowStatusBar("The device is switched off", true);
                        //DisplayEmpty();
                        //btnConnect.Text = "Connect";
                        //ToggleControls(false);
                        break;
                    }

                default:
                    break;
            }
        }

        public fingerK50_SDKHelper getCihazSdk(int cihazId)
        {
            if (v.progressBar1.Value == 0)
            {
                v.progressBar1.Maximum = 100;
                v.progressBar1.Value = 10;
                Application.DoEvents();
            }

            fingerK50_SDKHelper sdk = null;

            foreach (cihazHesap ch in v.tCihazHesapList)
            {
                if (ch.CihazId == cihazId)
                {
                    if (ch.Connnect == false)
                    {
                        if (v.CihazIsActive)
                            ch.Connnect = ch.Sdk.Connect_Net(ch.CihazIp, t.myInt32(ch.CihazPort)); // cihazlar networkde gerçekten var ise
                        else ch.Connnect = true; // cihazsız test için kullanılıyor

                        if (v.listBox1 != null)
                            v.listBox1.Items.Add("Cihaz Id : " + ch.CihazId + ", " + ch.CihazAdi + " Connect ... ");
                    }

                    if (ch.Connnect)
                    {
                        sdk = ch.Sdk;
                        return sdk;
                    }
                    else return null;
                }
            }
            return null;
        }

        public cihazHesap getCihazHesap(int cihazId)
        {
            foreach (cihazHesap ch in v.tCihazHesapList)
            {
                if (ch.CihazId == cihazId)
                {
                    if (ch.Connnect == false)
                    {
                        if (v.CihazIsActive)
                            ch.Connnect = ch.Sdk.Connect_Net(ch.CihazIp, t.myInt32(ch.CihazPort)); // cihazlar networkde gerçekten var ise
                        else ch.Connnect = true; // cihazsız test için kullanılıyor

                        if (v.listBox1 != null)
                            v.listBox1.Items.Add("Cihaz Id : " + ch.CihazId + ", " + ch.CihazAdi + " Connect ... ");
                    }

                    if (ch.Connnect)
                    {
                        return ch;
                    }
                    else return null;
                }
            }
            return null;
        }


        public void disConnectCihazSdk(int cihazId)
        {
            foreach (cihazHesap ch in v.tCihazHesapList)
            {
                if (ch.CihazId == cihazId)
                {
                    if (ch.Connnect)
                    {
                        ch.Connnect = false;
                        ch.Sdk.Disconnect();

                        if (v.listBox1 != null)
                            v.listBox1.Items.Add("Cihaz Id : " + ch.CihazId + ", " + ch.CihazAdi + " Disconnect ... ");
                        return;
                    }
                    else return;
                }
            }
        }

        public bool manuelTalepRunStart()
        {
            bool onay = true;
            if (v.autoTalepRun)
            {
                onay = false;
                MessageBox.Show("DİKKAT : Üzgünüm şu anda otomatik cihaz işlemleri gerçekleştiriliyor ..." + v.ENTER2 +
                    "Lütfen otomatik işlemlerin bitmesini bekleyin ve bu işlemlerden sonra tekrar deneyin...");
            }
            else v.manuelTalepRun = true;

            return onay;
        }

        public string deviceWork(int cihazId, v.cihazTalepTipi talep)
        {
            string s = "";

            //fingerK50_SDKHelper SDK = null;

            cihazHesap ch = getCihazHesap(cihazId);
            
            if (ch == null)
            {
                s = "DİKKAT : Cihaza ulaşılamıyor.. CihazId = " + cihazId.ToString();
                return s;
            }

            if (manuelTalepRunStart() == false) return s;

            if (ch.Connnect)
            {
                s = ch.CihazAdi + " : " + ch.CihazIp;

                #region cihaz işlemleri
                if (talep == v.cihazTalepTipi.chConnect)
                {
                    s = s + " : Connection ...";
                }
                if (talep == v.cihazTalepTipi.chDisconnnect)
                {
                    if (v.CihazIsActive)
                        ch.Sdk.Disconnect();
                    ch.Connnect = false;
                    s = s + " : Disconnect ";
                }
                if (talep == v.cihazTalepTipi.chTest)
                {
                    //int count = tGetAllUserCount(ch.Sdk, ch.CihazId);
                    int fpCount = 0;
                    int userCount = tCountAllUserFPInfo(ch.Sdk, ch.CihazId, ref fpCount);
                    s = s + " : Test  : " + userCount.ToString() + " kişi, "+ fpCount.ToString() + " bio izi mevcut";
                }
                if (talep == v.cihazTalepTipi.chLogCount)
                {
                    int logCount = tReadAttLogCount(ch.Sdk, ch.CihazId);
                    s = s + " : Test  : " + logCount.ToString() + " adet okutulmuş bio iz mevcut";
                }
                if (talep == v.cihazTalepTipi.chGetTarihSaat)
                {
                    s = s + " : GetTarihSaat : " + tGetDeviceTime(ch.Sdk, ch.CihazId);
                }
                if (talep == v.cihazTalepTipi.chSetTarihSaat)
                {
                    s = s + " : SetTarihSaat : " + tSetDeviceTime(ch.Sdk, ch.CihazId);
                }
                if (talep == v.cihazTalepTipi.chReset)
                {
                    if (v.CihazIsActive)
                        ch.Sdk.RestartDevice(ch.CihazId);
                    
                    ch.Connnect = false;
                    s = s + " : Reset ";
                }

                #endregion cihaz işlemleri
            }
            else
            {
                s = ch.CihazAdi + " : " + ch.CihazIp + " : not connetion ( Bağlantı kurulumadı ...) ";
            }

            v.manuelTalepRun = false;

            return s;
        }

        public string talepNameGet(v.cihazTalepTipi talep)
        {
            string name = "";
                        
            if (talep == v.cihazTalepTipi.chConnect) name = "Connect";
            if (talep == v.cihazTalepTipi.chDisconnnect) name = "Disconnnect";
            if (talep == v.cihazTalepTipi.chTest) name = "Test";
            if (talep == v.cihazTalepTipi.chLogCount) name = "LogCount";
            if (talep == v.cihazTalepTipi.chGetTarihSaat) name = "GetTarihSaat";
            if (talep == v.cihazTalepTipi.chSetTarihSaat) name = "SetTarihSaat";
            if (talep == v.cihazTalepTipi.chGetAllUserAndFPs) name = "GetAllUserAndFPs";
            if (talep == v.cihazTalepTipi.chGetAllLogs) name = "GetAllLogs";
            if (talep == v.cihazTalepTipi.chSetAllUserAndFPs) name = "SetAllUserAndFPs";
            if (talep == v.cihazTalepTipi.chReset) name = "Reset";

            if (talep == v.cihazTalepTipi.chSetNewUser) name = "SetNewUser";
            if (talep == v.cihazTalepTipi.chGetNewUserFP) name = "GetNewUserFP";
            if (talep == v.cihazTalepTipi.chSetNewUserSayim) name = "SetNewUserSayim";

            if (talep == v.cihazTalepTipi.chSetOldUser) name = "SetOldUser";
            if (talep == v.cihazTalepTipi.chGetOldUserFP) name = "GetOldUserFP";
            if (talep == v.cihazTalepTipi.chSetOldUserSayim) name = "SetOldUserSayim";

            if (talep == v.cihazTalepTipi.chSetSayim) name = "SetSayim";
            if (talep == v.cihazTalepTipi.chSetTahliye) name = "SetTahliye";
            if (talep == v.cihazTalepTipi.chSetGorev) name = "SetGorev";
            if (talep == v.cihazTalepTipi.chSetGorus) name = "SetGorus";

            if (talep == v.cihazTalepTipi.chGetGorev) name = "GetGorev";
            if (talep == v.cihazTalepTipi.chGetGorus) name = "GetGorus";
            if (talep == v.cihazTalepTipi.chGetSayim) name = "GetSayim";

            if (talep == v.cihazTalepTipi.chDelGorev) name = "DelGorev";
            if (talep == v.cihazTalepTipi.chDelGorus) name = "DelGorus";
            if (talep == v.cihazTalepTipi.chDelTahliye) name = "DelTahliye";

            if (talep == v.cihazTalepTipi.chDelUserFPLog) name = "DelUserFPLog"; // "DelNewUser"; tüm bilgileri sil

            if (talep == v.cihazTalepTipi.chIcmal) name = "Icmal";
            if (talep == v.cihazTalepTipi.chIcmalNewUser) name = "IcmalNewUser";
            if (talep == v.cihazTalepTipi.chIcmalOldUser) name = "IcmalOldUser";
            if (talep == v.cihazTalepTipi.chIcmalTahliye) name = "IcmalTahliye";

            return name;
        }

        public v.cihazTalepTipi talepNameGet(string name)
        {
            v.cihazTalepTipi talep = v.cihazTalepTipi.chNull;

            /*
            if (talep == v.cihazTalepTipi.chConnect) name = "Connect";
            if (talep == v.cihazTalepTipi.chDisconnnect) name = "Disconnnect";
            if (talep == v.cihazTalepTipi.chTest) name = "Test";
            if (talep == v.cihazTalepTipi.chLogCount) name = "LogCount";
            if (talep == v.cihazTalepTipi.chGetTarihSaat) name = "GetTarihSaat";
            if (talep == v.cihazTalepTipi.chSetTarihSaat) name = "SetTarihSaat";
            if (talep == v.cihazTalepTipi.chReset) name = "Reset";
            */
            
            if (name == "GetAllUserAndFPs") talep = v.cihazTalepTipi.chGetAllUserAndFPs;
            if (name == "GetAllLogs") talep = v.cihazTalepTipi.chGetAllLogs;
            if (name == "SetAllUserAndFPs") talep = v.cihazTalepTipi.chSetAllUserAndFPs;
            
            if (name == "SetNewUser") talep = v.cihazTalepTipi.chSetNewUser;
            if (name == "GetNewUserFP") talep = v.cihazTalepTipi.chGetNewUserFP;
            if (name == "SetNewUserSayim") talep = v.cihazTalepTipi.chSetNewUserSayim;
            
            if (name == "SetOldUser") talep = v.cihazTalepTipi.chSetOldUser;
            if (name == "GetOldUserFP") talep = v.cihazTalepTipi.chGetOldUserFP;
            if (name == "SetOldUserSayim") talep = v.cihazTalepTipi.chSetOldUserSayim;

            if (name == "SetSayim") talep = v.cihazTalepTipi.chSetSayim;
            if (name == "SetTahliye") talep = v.cihazTalepTipi.chSetTahliye;
            if (name == "SetGorev") talep = v.cihazTalepTipi.chSetGorev;
            if (name == "SetGorus") talep = v.cihazTalepTipi.chSetGorus;

            if (name == "GetGorev") talep = v.cihazTalepTipi.chGetGorev;
            if (name == "GetGorus") talep = v.cihazTalepTipi.chGetGorus;
            if (name == "GetSayim") talep = v.cihazTalepTipi.chGetSayim;

            if (name == "DelGorev") talep = v.cihazTalepTipi.chDelGorev;
            if (name == "DelGorus") talep = v.cihazTalepTipi.chDelGorus;
            if (name == "DelTahliye") talep = v.cihazTalepTipi.chDelTahliye;

            if (name == "DelUserFPLog") talep = v.cihazTalepTipi.chDelUserFPLog; // chDelNewUser;  -- Tüm bilgileri sil

            if (name == "Icmal") talep = v.cihazTalepTipi.chIcmal;
            if (name == "IcmalNewUser") talep = v.cihazTalepTipi.chIcmalNewUser;
            if (name == "IcmalOldUser") talep = v.cihazTalepTipi.chIcmalOldUser;
            if (name == "IcmalTahliye") talep = v.cihazTalepTipi.chIcmalTahliye;


            return talep;
        }

        #region SYNCTime / DateTime functions 
        public void tSYNCTime(fingerK50_SDKHelper SDK, int iMachineNumber)
        {
            Cursor.Current = Cursors.WaitCursor;

            int idwErrorCode = 0;

            if (SDK.SetDeviceTime(iMachineNumber))
            {
                SDK.RefreshData(iMachineNumber);//the data in the device should be refreshed
                //lblOutputInfo.Items.Add("Successfully SYNC the PC's time to device!");
            }
            else
            {
                SDK.GetLastError(ref idwErrorCode);
                //lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
            }

            Cursor.Current = Cursors.Default;
        }

        public string tGetDeviceTime(fingerK50_SDKHelper SDK, int iMachineNumber)
        {
            Cursor.Current = Cursors.WaitCursor;

            int idwErrorCode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            string sonuc = "";

            if (v.CihazIsActive)
            {
                if (SDK.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond))//show the time
                {
                    sonuc = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
                    //lblOutputInfo.Items.Add("Get devie time successfully");
                }
                else
                {
                    SDK.GetLastError(ref idwErrorCode);
                    //lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
                }
            }
            else
            {
                sonuc = DateTime.Now.ToString() +  " (Get Test)";
            }

            Cursor.Current = Cursors.Default;

            return sonuc;
        }

        public string tSetDeviceTime(fingerK50_SDKHelper SDK, int iMachineNumber)
        {
            Cursor.Current = Cursors.WaitCursor;

            DateTime date = DateTime.Now; //DateTime.Parse(dtDeviceTime.Text);
            string sonuc = date.ToString();

            if (v.CihazIsActive)
            {
                int idwErrorCode = 0;
                int idwYear = Convert.ToInt32(date.Year.ToString());
                int idwMonth = Convert.ToInt32(date.Month.ToString());
                int idwDay = Convert.ToInt32(date.Day.ToString());
                int idwHour = Convert.ToInt32(date.Hour.ToString());
                int idwMinute = Convert.ToInt32(date.Minute.ToString());
                int idwSecond = Convert.ToInt32(date.Second.ToString());

                if (SDK.SetDeviceTime2(iMachineNumber, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond))
                {
                    SDK.RefreshData(iMachineNumber);//the data in the device should be refreshed
                                                    //lblOutputInfo.Items.Add("Successfully set the time");
                }
                else
                {
                    SDK.GetLastError(ref idwErrorCode);
                    //lblOutputInfo.Items.Add("*Operation failed,ErrorCode=" + idwErrorCode.ToString());
                }
            }
            else
            {
                sonuc = sonuc + " (Set Test)";
            }

            Cursor.Current = Cursors.Default;
            return sonuc; 
        }
        
        #endregion


        #region İzzet Kodları
        // ------------------------------------------------
        // bir önceki projenin kodları (izzet)

        public string FetchDeviceInfo(fingerK50_SDKHelper objZkeeper, int machineNumber)
        {
            StringBuilder sb = new StringBuilder();

            string returnValue = string.Empty;


            objZkeeper.GetFirmwareVersion(machineNumber, ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Firmware V: ");
                sb.Append(returnValue);
                sb.Append(",");
            }


            returnValue = string.Empty;
            objZkeeper.GetVendor(ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Vendor: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            string sWiegandFmt = string.Empty;
            objZkeeper.GetWiegandFmt(machineNumber, ref sWiegandFmt);

            returnValue = string.Empty;
            objZkeeper.GetSDKVersion(ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("SDK V: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            returnValue = string.Empty;
            objZkeeper.GetSerialNumber(machineNumber, out returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Serial No: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            returnValue = string.Empty;
            objZkeeper.GetDeviceMAC(machineNumber, ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Device MAC: ");
                sb.Append(returnValue);
            }

            return sb.ToString();
        }


        // user bilgisini set et
        public bool SetUserDataToDevice(fingerK50_SDKHelper objZkeeper, int machineNumber, string userId, string userName)
        {
            string password = string.Empty;
            int privelage = 0;

            //return objZkeeper.SSR_SetUserInfo(machineNumber, enrollNo, userName, password, privelage, true);
            return objZkeeper.SSR_SetUserInfo(machineNumber, userId, userName, password, privelage, true);

            // dwMachineNumber LONG[in] Machine ID
            // dwEnrollNumber  BSTR[in] User ID
            // Name            BSTR[in] User name
            // Password        BSTR[in] User password
            // Privilege       LONG[in] User privilege
            // Enabled         BOOL[in]
            // Flag that indicates whether a user account is enabled

            // Attention 
            // 1.The Privilege parameter specifies the user privilege. 
            // The value 0 indicates common user, 1 registrar, 2 administrator, and 3 super administrator. 
            // 2.The Enable parameter specifies whether a user account is enabled.
            // The value 1 indicates that the user account is enabled and 0 indicates that the user account is disabled.

            // Dikkat 
            // 1.Privilege parametresi kullanıcı ayrıcalığını belirtir. 
            // 0 değeri, ortak kullanıcı, 1 kayıt memuru, 2 yönetici ve 3 süper yöneticiyi belirtir. 
            // 2.Enable parametresi bir kullanıcı hesabının etkin olup olmadığını belirtir. 
            // 1 değeri, kullanıcı hesabının etkinleştirildiğini ve 0 değeri, kullanıcı hesabının devre dışı bırakıldığını gösterir.

        }

        // user sil
        public bool DeleteUserFromMachine(fingerK50_SDKHelper objZkeeper, int machineNumber, int userId)
        {
            return objZkeeper.SSR_DeleteEnrollData(machineNumber, userId.ToString(), 0);
        }

        // tüm user bilgisini getir
        public ICollection<fingerUserInfo> GetAllUsers(fingerK50_SDKHelper objZkeeper, int machineNumber)
        {
            string sdwEnrollNumber = string.Empty;
            string sName = string.Empty;
            string sPassword = string.Empty;
            int iPrivilege = 0;
            bool bEnabled = false;

            string sTmpData = string.Empty;

            ICollection<fingerUserInfo> lstFPTemplates = new List<fingerUserInfo>();

            objZkeeper.ReadAllUserID(machineNumber);
            objZkeeper.ReadAllTemplate(machineNumber);

            while (objZkeeper.SSR_GetAllUserInfo(machineNumber,
                              out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
            {
                //for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                //{
                //   if (objZkeeper.GetUserInfo(machineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))
                //    {

                fingerUserInfo fpInfo = new fingerUserInfo();

                fpInfo.MachineNumber = machineNumber;
                fpInfo.EnrollNumber = sdwEnrollNumber;
                fpInfo.Name = sName;
                fpInfo.FingerIndex = 0;// idwFingerIndex;
                fpInfo.TmpData = sTmpData;  // << parmakİzi datası yok
                fpInfo.Privelage = iPrivilege;
                fpInfo.Password = sPassword;
                fpInfo.Enabled = bEnabled;
                fpInfo.iFlag = "";// iFlag.ToString();

                lstFPTemplates.Add(fpInfo);

                //    }
                //}

            }
            return lstFPTemplates;
        }

        // sadece parmak izi kayıtlı olanları getir
        public ICollection<fingerUserInfo> GetAllUserFPInfo(fingerK50_SDKHelper objZkeeper, int machineNumber)
        {
            string sdwEnrollNumber = string.Empty;
            string sName = string.Empty;
            string sPassword = string.Empty;
            string sTmpData = string.Empty;
            int iPrivilege = 0;
            int iTmpLength = 0;
            int iFlag = 0;
            int idwFingerIndex = 0;
            bool bEnabled = false;

            ICollection<fingerUserInfo> lstFPTemplates = new List<fingerUserInfo>();

            objZkeeper.ReadAllUserID(machineNumber);
            objZkeeper.ReadAllTemplate(machineNumber);

            while (objZkeeper.SSR_GetAllUserInfo(machineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
            {
                for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                {
                    if (objZkeeper.GetUserTmpExStr(machineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))
                    {
                        fingerUserInfo fpInfo = new fingerUserInfo();
                        fpInfo.MachineNumber = machineNumber;
                        fpInfo.EnrollNumber = sdwEnrollNumber;
                        fpInfo.Name = sName;
                        fpInfo.FingerIndex = idwFingerIndex;
                        fpInfo.TmpData = sTmpData;
                        fpInfo.Privelage = iPrivilege;
                        fpInfo.Password = sPassword;
                        fpInfo.Enabled = bEnabled;
                        fpInfo.iFlag = iFlag.ToString();

                        lstFPTemplates.Add(fpInfo);
                    }
                }

            }
            return lstFPTemplates;
        }

        // cihazdaki datayı oku
        public ICollection<fingerLogData> GetLogData(fingerK50_SDKHelper objZkeeper, int machineNumber)
        {
            string dwEnrollNumber1 = "";
            int dwVerifyMode = 0;
            int dwInOutMode = 0;
            int dwYear = 0;
            int dwMonth = 0;
            int dwDay = 0;
            int dwHour = 0;
            int dwMinute = 0;
            int dwSecond = 0;
            int dwWorkCode = 0;

            ICollection<fingerLogData> lstEnrollData = new List<fingerLogData>();

            objZkeeper.ReadAllGLogData(machineNumber);

            while (objZkeeper.SSR_GetGeneralLogData(machineNumber,
                   out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode,
                   out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
            {
                string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();

                fingerLogData objInfo = new fingerLogData();
                objInfo.MachineNumber = machineNumber;
                objInfo.IndRegID = int.Parse(dwEnrollNumber1);
                objInfo.DateTimeRecord = inputDate;

                lstEnrollData.Add(objInfo);
            }

            return lstEnrollData;
        }

        // user hakkındaki bilgileri cihazlara gönder
        public bool UploadFTPTemplate(fingerK50_SDKHelper objZkeeper, int machineNumber, List<fingerUserInfo> lstUserInfo)
        {
            string sdwEnrollNumber = string.Empty;
            string sName = string.Empty;
            string sTmpData = string.Empty;
            int idwFingerIndex = 0;
            int iPrivilege = 0;
            int iFlag = 1;
            int iUpdateFlag = 1;

            string sPassword = "";
            string sEnabled = "";
            bool bEnabled = false;

            if (objZkeeper.BeginBatchUpdate(machineNumber, iUpdateFlag))
            {
                string sLastEnrollNumber = "";

                for (int i = 0; i < lstUserInfo.Count; i++)
                {
                    sdwEnrollNumber = lstUserInfo[i].EnrollNumber;
                    sName = lstUserInfo[i].Name;
                    idwFingerIndex = lstUserInfo[i].FingerIndex;
                    sTmpData = lstUserInfo[i].TmpData;
                    iPrivilege = lstUserInfo[i].Privelage;
                    sPassword = lstUserInfo[i].Password;
                    sEnabled = lstUserInfo[i].Enabled.ToString();
                    iFlag = Convert.ToInt32(lstUserInfo[i].iFlag);
                    bEnabled = true;

                    // [ Identify whether the user 
                    //   information(except fingerprint templates) has been uploaded 

                    // Kullanıcının bilgi (parmak izi şablonları hariç) yüklendi

                    if (sdwEnrollNumber != sLastEnrollNumber)
                    {
                        // upload user information to the memory
                        // kullanıcı bilgilerini belleğe yükle
                        if (objZkeeper.SSR_SetUserInfo(machineNumber,
                                sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))
                        {
                            // upload templates information to the memory
                            // şablon bilgilerini belleğe yükle
                            if (!string.IsNullOrEmpty(sTmpData))
                                objZkeeper.SetUserTmpExStr(machineNumber,
                                    sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData.Trim());
                        }
                        else
                            return false;
                    }
                    else
                    {
                        // [ The current fingerprint and the former one belongs the same user,
                        //   i.e one user has more than one template ] 

                        // [ Geçerli parmak izi ve bir önceki parmak izi aynı kullanıcıya ait,
                        //   yani bir kullanıcının birden fazla şablonu var]

                        objZkeeper.SetUserTmpExStr(machineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);
                    }

                    sLastEnrollNumber = sdwEnrollNumber;

                }

                // upload all the information in the memory
                // bellekteki tüm bilgileri yükle
                //
                objZkeeper.BatchUpdate(machineNumber);
                objZkeeper.RefreshData(machineNumber);

                return true;
            }
            else
                return false;
        }

        public object ClearData(fingerK50_SDKHelper objZkeeper, int machineNumber, ClearFlag clearFlag)
        {
            int iDataFlag = (int)clearFlag;

            if (objZkeeper.ClearData(machineNumber, iDataFlag))
                return objZkeeper.RefreshData(machineNumber);
            else
            {
                int idwErrorCode = 0;
                objZkeeper.GetLastError(ref idwErrorCode);
                return idwErrorCode;
            }
        }

        //-------------------------------------------------------------------------------------------
        #endregion İzzet Kodları



    }




}
