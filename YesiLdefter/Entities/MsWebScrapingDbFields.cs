using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YesiLdefter.Entities
{
    public class MsWebScrapingDbFields
    {
        public string WebScrapingPageCode { get; set; }
        public int WebScrapingGetNodeId { get; set; }
        public Int16 WebScrapingGetColumnNo { get; set; }
        public int WebScrapingSetNodeId { get; set; }
        public Int16 WebScrapingSetColumnNo { get; set; }
        public Int16 KrtOperandType { get; set; } // KRT_OPERAND_TYPE
        public string TableIPCode { get; set; }
        public string FieldName { get; set; } //FIELD_NAME 
        public bool FLookUpField { get; set; } // FLOOKUP_FIELD
        public Int16  FieldType { get; set; } // FIELD_TYPE
}
}
