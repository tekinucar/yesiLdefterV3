using System.Windows.Forms;

using Tkn_CreateObject;
using Tkn_Events;
using Tkn_InputPanel;
using Tkn_ToolBox;
using Tkn_Variable;

namespace Tkn_SubView
{
    public class tSubViews : tBase
    {
        /// <summary>
        ///  şimdilik iptal 
        ///  
        /// </summary>
        /// <param name="tForm"></param>
        /// <param name="Prop_Navigator"></param>
        /// <param name="selectItemValue"></param>
        /// <param name="caption"></param>
        public void tSubView_(Form tForm, string Prop_Navigator, string selectItemValue, string caption)
        {
            tToolBox t = new tToolBox();

            string TableIPCode = t.MyProperties_Get(Prop_Navigator, "=TABLEIPCODE:");
            string TableAlias = t.MyProperties_Get(Prop_Navigator, "=TABLEALIAS:");
            string KeyFName = t.MyProperties_Get(Prop_Navigator, "=KEYFNAME:");

            //-----
            // 1. Tablo
            string myFormLoadValue = string.Empty;
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "BEGIN", string.Empty);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetTableIPCode", TableIPCode);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetTableAlias", TableAlias);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetFieldName", KeyFName);
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "TargetValue", selectItemValue);
            // 2. Tablo
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondTableIPCode", TableIPCode);
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondTableAlias", TableAlias);
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondFieldName", "AHESAP_ID");
            //t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "SecondValue", "1");
            t.MyProperties_Set(ref myFormLoadValue, v.FormLoad, "END", string.Empty);

            v.con_FormLoadValue = myFormLoadValue;

            //subViewExec(tForm, "TabPage", "", TableIPCode, selectItemValue, caption, "");
        }

    }
}
