using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ustad.API.Models
{
    public class UstadUser
    {
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserTcNo { get; set; }
        public string UserMobileNo { get; set; }
        public string UserEMail { get; set; }
        public string UserFirmGUID { get; set; }
        public string UserKey { get; set; }
        public string UserGUID { get; set; }
    }
}
