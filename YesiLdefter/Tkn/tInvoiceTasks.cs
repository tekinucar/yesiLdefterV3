using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tkn_Variable;
using YesiLdefter.ServiceReferenceUyumTest;

namespace Tkn_InvoiceTasks
{
    public class tInvoiceTasks
    {
        public static tInvoiceTasks Instance = new tInvoiceTasks();

        private tInvoiceTasks()
        {

        }

        public BasicIntegrationClient CreateClient()
        {
            var username = "";
            var password = "";
            var serviceuri = "";

            v.tInvoiceUser.Clear();

            if (v.tInvoiceUser.UseTestEnvironment)
            {
                username = v.tInvoiceUser.WebServiceTestUsername;
                password = v.tInvoiceUser.WebServiceTestPassword;
                serviceuri = v.tInvoiceUser.WebServiceTestUri;
            }
            if (!v.tInvoiceUser.UseTestEnvironment)
            {
                username = v.tInvoiceUser.WebServiceLiveUsername;
                password = v.tInvoiceUser.WebServiceLivePassword;
                serviceuri = v.tInvoiceUser.WebServiceLiveUri;
            }

            var client = new BasicIntegrationClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(serviceuri);
            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = password;
            return client;
        }
        public UserInformation GetUserInfo()
        {
            UserInformation userInfo = new UserInformation();

            if (v.tInvoiceUser.UseTestEnvironment)
            {
                userInfo.Username = v.tInvoiceUser.WebServiceTestUsername;
                userInfo.Password = v.tInvoiceUser.WebServiceTestPassword;
            }
            if (!v.tInvoiceUser.UseTestEnvironment)
            {
                userInfo.Username = v.tInvoiceUser.WebServiceLiveUsername;
                userInfo.Password = v.tInvoiceUser.WebServiceLivePassword;
            }
            return userInfo;
        }


    }
}
