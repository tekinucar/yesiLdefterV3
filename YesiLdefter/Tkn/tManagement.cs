using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Tkn_Management
{
    class tManagement
    {

        private string yalinPrint = string.Empty;
        private string fingerPrint = string.Empty;
        private string mailPrint = "tekinucar70@hotmail.com";// string.Empty;

        public string Value()
        {
            if (string.IsNullOrEmpty(fingerPrint))
            {
                yalinPrint =
                    "CPU >> " + cpuId()
                    + "\nBIOS >> " + biosId()
                    + "\nBASE >> " + baseId()
                    + "\nVIDEO >> " + videoId()
                    + "\nMAC >> " + macId()
                    + "\nDISK >> " + diskId()
                    + "\nNAME >> " + Environment.MachineName
                    + "\nOSVERSION >> " + Environment.OSVersion
                    + "\nMAIL >> " + mailPrint;

                fingerPrint = GetHash(yalinPrint);

                MessageBox.Show(
                      yalinPrint + "\r\n" + "\r\n"
                    + fingerPrint + "\r\n" + "\r\n"
                    + GetHash(mailPrint)
                    );

            }
            return fingerPrint;
        }

        private string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }

        private string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }


        #region Original Device ID Getting Code

        //Return a hardware identifier
        private string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    //Only get the first one
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }

        //Return a hardware identifier
        private string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }

        #endregion


        private string cpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") //If no ProcessorId, use Name
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "") //If no Name, use Manufacturer
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    //Add clock speed for extra security
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }
        //BIOS Identifier
        private string biosId()
        {
            return
                identifier("Win32_BIOS", "Manufacturer")
              + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
              + identifier("Win32_BIOS", "IdentificationCode")
              + identifier("Win32_BIOS", "SerialNumber")
              + identifier("Win32_BIOS", "ReleaseDate")
              + identifier("Win32_BIOS", "Version");
        }
        //Main physical hard drive ID
        private string diskId()
        {
            return
                identifier("Win32_DiskDrive", "Model")
              + identifier("Win32_DiskDrive", "Manufacturer")
              + identifier("Win32_DiskDrive", "Signature")
              + identifier("Win32_DiskDrive", "TotalHeads");
        }
        //Motherboard ID
        private string baseId()
        {
            return
                identifier("Win32_BaseBoard", "Model")
              + identifier("Win32_BaseBoard", "Manufacturer")
              + identifier("Win32_BaseBoard", "Name")
              + identifier("Win32_BaseBoard", "SerialNumber");
        }
        //Primary video controller ID
        private string videoId()
        {
            return
                identifier("Win32_VideoController", "DriverVersion")
              + identifier("Win32_VideoController", "Name");
        }
        //First enabled network card ID
        private string macId()
        {
            return
                identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }

        public string EncodeBase64(string standartText)
        {
            if (standartText == null)
            {
                return null;
            }

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(standartText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        //public string EncodeBase64(this System.Text.Encoding encoding, string text)
        //{
        //    if (text == null)
        //    {
        //        return null;
        //    }

        //    byte[] textAsBytes = encoding.GetBytes(text);
        //    return System.Convert.ToBase64String(textAsBytes);
        //}

        public string DecodeBase64(string sifreliText)
        {
            if (sifreliText == null)
            {
                return null;
            }

            var base64EncodedBytes = System.Convert.FromBase64String(sifreliText);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        //public string DecodeBase64(this System.Text.Encoding encoding, string encodedText)
        //{
        //    if (encodedText == null)
        //    {
        //        return null;
        //    }

        //    byte[] textAsBytes = System.Convert.FromBase64String(encodedText);
        //    return encoding.GetString(textAsBytes);
        //}


    }

    /*

    */

    /*
    //// Retrieve the enumerator instance, and then retrieve the data sources.
            /*
            SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
            DataTable dtDatabaseSources = instance.GetDataSources();

            string s = "";
            foreach (DataRow row in dtDatabaseSources.Rows)
            {
                s = s + row["ServerName"].ToString()
                            + "\\" + row["InstanceName"].ToString() + v.ENTER;
            }

            MessageBox.Show(s);
            */

    /*
    //// Populate the data sources into DropDownList.            
    foreach (DataRow row in dtDatabaseSources.Rows)
        if (!string.IsNullOrWhiteSpace(row["InstanceName"].ToString()))
            Model.DatabaseDataSourceNameList.Add(new ExportWizardChooseDestinationModel
            {
                DatabaseDataSourceListItem = row["ServerName"].ToString()
                    + "\\" + row["InstanceName"].ToString()
            });
    */


    /*
    string myServer = Environment.MachineName;
    string ss = "";
    DataTable servers = SqlDataSourceEnumerator.Instance.GetDataSources();
    for (int i = 0; i < servers.Rows.Count; i++)
    {
        if (myServer == servers.Rows[i]["ServerName"].ToString()) ///// used to get the servers in the local machine////
        {
            if ((servers.Rows[i]["InstanceName"] as string) != null)
                ss = ss + servers.Rows[i]["ServerName"] + "\\" + servers.Rows[i]["InstanceName"] + v.ENTER;
            else
                ss = ss + servers.Rows[i]["ServerName"] + v.ENTER;
        }
    }

    MessageBox.Show(ss);
    */

    /*
    MessageBox.Show(
       Environment.MachineName + v.ENTER +
       Environment.OSVersion + v.ENTER +
       Environment.ProcessorCount + v.ENTER +
       Environment.UserDomainName + v.ENTER +
       Environment.UserName + v.ENTER +
       Environment.Version.ToString() + v.ENTER +
       Environment.CurrentManagedThreadId.ToString() + v.ENTER +
       Environment.CurrentDirectory + v.ENTER
       );

    */

    /*
        string cpuInfo = string.Empty;
        ManagementClass mc = new ManagementClass("win32_processor");
        ManagementObjectCollection moc = mc.GetInstances();

        foreach (ManagementObject mo in moc)
        {

             cpuInfo = mo.Properties["processorID"].Value.ToString();
             break;
        }

        string drive = "C";
        ManagementObject dsk = new ManagementObject(
        @"win32_logicaldisk.deviceid=""" + drive + @":""");
        dsk.Get();
        string volumeSerial = dsk["VolumeSerialNumber"].ToString();


MessageBox.Show(cpuInfo + "/" + volumeSerial);
*/
}
