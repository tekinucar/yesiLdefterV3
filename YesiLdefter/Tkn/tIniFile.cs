using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Tkn_IniFile
{
    class tIniFile
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public tIniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName;
        }
        
        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
    /*
Bu nasıl kullanılır
INI dosyasını aşağıdaki 3 yoldan biriyle açın:

// Creates or loads an INI file in the same directory as your executable
// named EXE.ini (where EXE is the name of your executable)
var MyIni = new IniFile();

// Or specify a specific name in the current dir
var MyIni = new IniFile("Settings.ini");

// Or specify a specific name in a specific dir
var MyIni = new IniFile(@"C:\Settings.ini");
Bunun gibi bazı değerler yazabilirsiniz:

MyIni.Write("DefaultVolume", "100");
MyIni.Write("HomePage", "http://www.google.com");
Bunun gibi bir dosya oluşturmak için:

[MyProg]
DefaultVolume=100
HomePage=http://www.google.com
INI dosyasındaki değerleri okumak için:

var DefaultVolume = MyIni.Read("DefaultVolume");
var HomePage = MyIni.Read("HomePage");
İsteğe bağlı olarak şunları ayarlayabilirsiniz [Section]:

MyIni.Write("DefaultVolume", "100", "Audio");
MyIni.Write("HomePage", "http://www.google.com", "Web");
Bunun gibi bir dosya oluşturmak için:

[Audio]
DefaultVolume=100

[Web]
HomePage=http://www.google.com
Ayrıca şöyle bir anahtarın varlığını da kontrol edebilirsiniz:

if(!MyIni.KeyExists("DefaultVolume", "Audio"))
{
    MyIni.Write("DefaultVolume", "100", "Audio");
}
Bunun gibi bir anahtarı silebilirsiniz:

MyIni.DeleteKey("DefaultVolume", "Audio");
Ayrıca bir bölümün tamamını (tüm anahtarlar dahil) şu şekilde silebilirsiniz:

MyIni.DeleteSection("Web");




    */
}
