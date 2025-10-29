using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ustad.API.Models
{
    public class UstadFirm
    {
        public int FirmId { get; set; }
        public string FirmLongName { get; set; }
        public string FirmShortName { get; set; }
        public string FirmGuid { get; set; }
        public string MenuCode { get; set; }
        public string MenuCodeOld { get; set; }
        public Int16 SectorTypeId { get; set; }
        public string DatabaseType { get; set; }
        public string DatabaseName { get; set; }
        public string ServerNameIP { get; set; }
        public string DbAuthentication { get; set; }
        public string DbLoginName { get; set; }
        public string DbPassword { get; set; }
        public string MebbisCode { get; set; }
        public string MebbisPass { get; set; }
        public void Clear()
        {
            FirmId = 0;
            FirmLongName = "";
            FirmShortName = "";
            FirmGuid = "";
            MenuCode = "";
            MenuCodeOld = "";
            SectorTypeId = 0;
            MebbisCode = "";
            MebbisPass = "";
        }
    }
}

