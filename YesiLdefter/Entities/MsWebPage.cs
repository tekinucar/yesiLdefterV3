using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YesiLdefter.Entities
{
    public class MsWebPage
    {
        public int Id { get; set; }
	    public int ParentId { get; set; }
        public bool IsActive { get; set; }
        public int LineNumber { get; set; }
        public string About { get; set; }
        public string PageCode { get; set; }
        public string PageUrl { get; set; }     
	    public string BeforePageUrl { get; set; }
        public bool LoginPage { get; set; } 
        public Int16 PageLeft { get; set; }
	    public Int16 PageTop { get; set; }
        public string ErrorPageUrl { get; set; }
        public string ModulCode { get; set; }
        public string SoftwareCode { get; set; }
        public string ProjectCode { get; set; }
        public string SchemasCode { get; set; }
    }
}
