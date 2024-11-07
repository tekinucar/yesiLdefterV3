using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YesiLdefter.Entities
{
    public class MsWebNode
    {
		public int Id { get; set; }
        public int NodeId { get; set; }
		public int ParentId { get; set; }
		public string PageCode { get; set; }
		public bool IsActive { get; set; }
		public Int16 EventsType { get; set; }
		public int LineNumber { get; set; }

		public string About { get; set; }
		public string TagName { get; set; }
		public string AttId { get; set; }
		public string AttName { get; set; }
		public string AttClass { get; set; }
		public string AttType { get; set; }
		public string AttRole { get; set; }
		public string AttHRef { get; set; }
		public string AttSrc { get; set; }
		public string XPath { get; set; }

		public string InnerHtml { get; set; }
		public string InnerText { get; set; }
		public string OuterHtml { get; set; }
		public string OuterText { get; set; }

		public Int16 InjectType { get; set; } /* 1.get&set, 2.get, 3.set,  4.? */
		public Int16 InvokeMember { get; set; }
		public bool DontSave { get; set; }
		public bool GetSave { get; set; } /* get işleminin bitiminde otomatik database kayıt etmesi için */
		public bool DisplayNone { get; set; }
		public bool PageRefresh { get; set; }
		public string TestValue { get; set; }
		public string KrtOperandType { get; set; }
		public string CheckValue { get; set; }
		public int CheckNodeId { get; set; }

	}
}
