using System.Collections.Generic;

using YesiLcihazlar;
//using Tkn_Variable;

namespace Tkn_Registry
{
    class tRegistry : tBase
    {

        public void SetUstadRegistry(string caption, string value)
        {
            var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + v.registryPath);
            regUser.SetValue(caption, value, Microsoft.Win32.RegistryValueKind.String);
        }

        public List<object> GetUstadEMailList()
        {
            var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + v.registryPath);

            List<object> itemList = new List<object>();

            if (regUser != null)
            {
                foreach (var item in regUser.GetValueNames())
                {
                    if (item.IndexOf("userEMail") > -1)
                    {
                        //((DevExpress.XtraEditors.ComboBoxEdit)cmb_EMail).Properties.Items.Add(regUser.GetValue(item.ToString()));
                        itemList.Add(regUser.GetValue(item.ToString()));
                    }
                }
            }

            return itemList;
        }

        public object getRegistryValue(string caption)
        {
            var regUser = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"" + v.registryPath);

            object value = null;

            value = regUser.GetValue(caption);

            return value;
        }

    }
}
