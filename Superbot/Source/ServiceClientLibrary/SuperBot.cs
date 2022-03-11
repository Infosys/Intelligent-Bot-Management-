/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Ainauto.Services.Superbot.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Superbot.Infrastructure.ServiceClientLibrary
{
    public class SuperBot
    {
        string _serviceUrl; // to be used to created end point programmatically

        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { _serviceUrl = value; }
        }
        /// <summary>
        /// Constructor to establish communication channel to the ScriptExecute service.
        /// </summary>
        /// <param name="serviceUrl">The base url of the ScriptExecute service.
        /// if no url is passed then it would be constructed programmatically</param>
        public SuperBot(string serviceUrl = "")
        {
            _serviceUrl = serviceUrl;
        }

        /// <summary>
        /// Channel to call the operations on Script repository service
        /// </summary>
        public IResourceHandler ServiceChannel
        {
            get
            {

                var serviceBinding = SuperBotProxy.GetBinding();
                Uri _serviceAddress = null;

                if (!string.IsNullOrEmpty(_serviceUrl))
                    _serviceAddress = new Uri(_serviceUrl);
                else
                    _serviceAddress = SuperBotProxy.ServiceAddress(Services.ResourceHandler);

                WebChannelFactory<IResourceHandler> securityAccessChannel = new WebChannelFactory<IResourceHandler>(serviceBinding, _serviceAddress);
                return securityAccessChannel.CreateChannel();
            }
        }
    }
}
