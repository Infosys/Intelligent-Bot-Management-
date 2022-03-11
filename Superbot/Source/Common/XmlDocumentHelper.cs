/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System.Xml;

namespace Infosys.Solutions.Superbot.Infrastructure.Common
{
    public class XmlDocumentHelper : XmlDocument
    {
        private XmlReader xmlReader;
        private XmlNamespaceManager xmlNameSpaceManager;

        public XmlDocumentHelper(string url)
            : base()
        {
            xmlReader = new XmlTextReader(url);
            xmlNameSpaceManager = new XmlNamespaceManager(xmlReader.NameTable);
            this.Load(xmlReader);
        }

        public override XmlElement this[string id]
        {
            get
            {
                return this.SelectSingleNode(string.Format("//*[@id='{0}']", id)) as XmlElement;
            }
        }
    }
}
