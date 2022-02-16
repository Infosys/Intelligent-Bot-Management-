/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IE = Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;
using BE = Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity;
using Newtonsoft.Json;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Translator
{
    public class ResouceTypeMetaDataBE_IE
    {
        public static List<IE.Platform_Info> Platform_BEtoIE(List<BE.Platform_Info> platformsBE)
        {
            List < IE.Platform_Info> platforms = new List<IE.Platform_Info>();
            var json = JsonConvert.SerializeObject(platformsBE);
            platforms = JsonConvert.DeserializeObject< List<IE.Platform_Info>>(json);
            return platforms;
        }

        public static List<IE.ResourceTypeMetadata> metadata_BEListtoIEList(List<BE.ResourceTypeMetadata> responseBE)
        {
            List<IE.ResourceTypeMetadata> resourceMetadata = new List<IE.ResourceTypeMetadata>();
            var json = JsonConvert.SerializeObject(responseBE);
            resourceMetadata = JsonConvert.DeserializeObject<List<IE.ResourceTypeMetadata>>(json);
            return resourceMetadata;
        }

        public static BE.ResourceTypeMetadata metadata_IEtoBE(IE.ResourceTypeMetadata responseIE)
        {
            BE.ResourceTypeMetadata resourceMetadata = new BE.ResourceTypeMetadata();
            var json = JsonConvert.SerializeObject(responseIE);
            resourceMetadata = JsonConvert.DeserializeObject<BE.ResourceTypeMetadata>(json);
            return resourceMetadata;
        }

        public static IE.ResourceTypeMetadata metadata_BEtoIE(BE.ResourceTypeMetadata responseBE)
        {
            IE.ResourceTypeMetadata resourceMetadata = new IE.ResourceTypeMetadata();
            var json = JsonConvert.SerializeObject(responseBE);
            resourceMetadata = JsonConvert.DeserializeObject<IE.ResourceTypeMetadata>(json);
            return resourceMetadata;
        }

        public static BE.ResourceType_Attribute ResourceTypeAttributesIE_BE(IE.ResourceType_Attribute attributeIE)
        {
            BE.ResourceType_Attribute attributeBE = new BE.ResourceType_Attribute();
            var json = JsonConvert.SerializeObject(attributeIE);
            attributeBE = JsonConvert.DeserializeObject<BE.ResourceType_Attribute>(json);
            return attributeBE;
        }
    }
}