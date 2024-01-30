using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_ExeUpdate
{
    public class tExeUpdate:tBase
    {
        tToolBox t = new tToolBox();

        #region versionChecked
        public bool versionChecked(Form tForm)
        {
            bool onay = false;

            if (t.IsNotNull(v.tExeAbout.ftpVersionNo))
            {
                string activeVer = v.tExeAbout.activeVersionNo.Replace("_", "");
                string ftpVer = v.tExeAbout.ftpVersionNo.Replace("_", "");

                if (t.myLong(activeVer) < t.myLong(ftpVer))
                {
                    //barButtonGuncelleme.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    t.FlyoutMessage(tForm, "Güncelleme", "Yeni exe güncelemesi mevcut, güncelleme başlayacak lütfen bekleyiniz...");
                    onay = RunExeUpdate();
                }
            }
            return onay;
        }
        private bool RunExeUpdate()
        {
            bool onay = false;
            //
            onay = t.ftpDownload(v.tExeAbout.ftpPacketName);
            //
            if (onay)
                onay = ExtractFile();
            //
            if (onay)
                onay = fileNameChange();

            if (onay)
            {
                LaunchCommandLineApp();
            }
            return onay;
        }
        private void LaunchCommandLineApp()
        {
            // For the example
            //const string ex1 = "C:\\";
            //const string ex2 = "C:\\Dir";

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = v.tExeAbout.activeExeName;   //"dcm2jpg.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.Arguments = "-f j -o \"" + ex1 + "\" -z 1.0 -s y " + ex2;
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    //exeProcess.WaitForExit();
                    Application.Exit();
                }
            }
            catch
            {
                // Log error.
            }
        }


        #region Extract

        public bool ExtractFile()
        {

            bool onay = false;

            DirectoryInfo di = new DirectoryInfo(v.tExeAbout.activePath);

            foreach (FileInfo fi in di.GetFiles())
            {
                //for specific file 
                if (fi.ToString() == v.tExeAbout.ftpPacketName)
                {
                    onay = Extract(fi);
                    break;
                }
            }

            return onay;
        }

        public bool Extract(FileInfo fi)
        {
            bool onay = false;

            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                string currentFileName = fi.FullName;
                string newFileName = fi.FullName.Remove(fi.FullName.IndexOf(fi.Extension), fi.Extension.Length) + ".exe";

                using (FileStream deCompressedFile = File.Create(newFileName))
                {
                    using (GZipStream deCompress = new GZipStream(inFile, CompressionMode.Decompress))
                    {
                        if (fi.Extension == ".gz")
                        {
                            deCompress.CopyTo(deCompressedFile);
                            onay = true;
                        }
                    }
                }
            }

            return onay;
        }

        public bool fileNameChange()
        {
            bool onay = false;

            try
            {
                // mevcut exe yi yedekleyelim

                // YesiLdefter.exe  >>>  YesiLdefter_20190328_2205.exe  ismine çevrilecek
                string newFileName = v.tExeAbout.activeExeName.Remove(v.tExeAbout.activeExeName.IndexOf(".exe"), 4) + "_" +
                                     v.tExeAbout.activeVersionNo + ".exe";
                // newFileName varsa sil
                File.Delete(newFileName);
                // YesiLdefter.exe  >>>  YesiLdefter_20190328_2205.exe
                File.Move(v.tExeAbout.activeExeName, newFileName);

                // ftp den inen exe nin adını değiştirelim

                // YesiLdefter_20190331_1843.gz   >>> YesiLdefter_20190331_1843.exe
                newFileName = v.tExeAbout.ftpPacketName.Remove(v.tExeAbout.ftpPacketName.IndexOf(".gz"), 3) + ".exe";

                // YesiLdefter_20190331_1843.exe  >>> YesiLdefter.exe
                File.Move(newFileName, v.tExeAbout.activeExeName);

                onay = true;
            }
            catch (Exception e1)
            {
                MessageBox.Show("Hata : Dosya isimleri değiştirilirken problem oluştu ..." + v.ENTER2 + e1.Message);
                throw;
            }
            return onay;
        }

        #endregion


        #endregion

    }
}
