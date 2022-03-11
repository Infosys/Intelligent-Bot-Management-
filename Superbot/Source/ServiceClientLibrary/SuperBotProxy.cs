/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Configuration;

namespace Infosys.Solutions.Ainauto.Superbot.Infrastructure.ServiceClientLibrary
{
    public class SuperBotProxy
    {
        private static readonly WebHttpBinding _serviceBinding =
            new WebHttpBinding();

        static string _deploymentUrl = "";

        static SuperBotProxy()
        {
            _deploymentUrl = Convert.ToString(ConfigurationManager.AppSettings["ServiceBaseUrl"]);

            Uri _serviceAddress = new Uri(_deploymentUrl);

            if (_serviceAddress.Scheme.Equals(Uri.UriSchemeHttps))
                _serviceBinding.Security.Mode = WebHttpSecurityMode.Transport;
            else
                _serviceBinding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;

            _serviceBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            _serviceBinding.MaxReceivedMessageSize = 20000000;
            _serviceBinding.MaxBufferSize = 20000000;
            _serviceBinding.MaxBufferPoolSize = 20000000;
        }

        public static WebHttpBinding GetBinding()
        {
            return _serviceBinding;
        }

        public static Uri ServiceAddress(Services service)
        {
            string _serviceUrl 
                = string.Format(ServiceConstants.sSuperBotUrl, _deploymentUrl, service);

            return new Uri(_serviceUrl);
        }


    }
}
