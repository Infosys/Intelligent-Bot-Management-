using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using System.Configuration;
using Infosys.Lif.LegacyWorkbench.Configurations.Retrievers;


namespace Infosys.Lif.LegacyWorkbench.Configurations
{
    class RetrieversHandler : IConfigurationSectionHandler
    {

        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            Retrievers.Retrievers readRetrievers = new Retrievers.Retrievers();
            readRetrievers.defaultRetriever = section.Attributes["defaultRetriever"].Value;
            XmlNodeList xmlNodes = section.SelectNodes("//Retriever");
            for (int nodeLooper = 0; nodeLooper < xmlNodes.Count; nodeLooper++)
            {
                readRetrievers.Add(BuildRetriever(xmlNodes[nodeLooper]));
            }
            return readRetrievers;
        }

        private Retriever BuildRetriever(XmlNode xmlNode)
        {
            Retriever retriever = new Retriever();
            retriever.name = xmlNode.Attributes["name"].Value;
            retriever.type = xmlNode.Attributes["type"].Value;
            retriever.fileExtension = xmlNode.Attributes["fileExtension"].Value;
            retriever.assemblyPath = xmlNode.Attributes["assemblyPath"].Value;
            return retriever;
        }
        #endregion
    }
}
