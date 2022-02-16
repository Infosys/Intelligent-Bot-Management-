/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using IE = Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;
using Infosys.Solutions.Ainauto.ConfigurationManager.BusinessComponent;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Controllers
{
    public class ObservablePlanController : ApiController
    {

        [Route("GetActions/{TenantId=TenantId}")]
        public IE.actionModelMap GetActions( int tenantID)
        {
            try
            {
                ObservablePlanBuilder obsBuilder = new ObservablePlanBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.actionModelMap actModel = Translator.ResourceModel_BE_IE.actionModel_BEtoIE(obsBuilder.GetActions(tenantID));
                return actModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [Route("GetResourceType/{TenantId=TenantId}")]
        public IE.resourceTypeModelMap GetResourceType( int tenantID)
        {
            try
            {
                ObservablePlanBuilder obsBuilder = new ObservablePlanBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.resourceTypeModelMap resTypeModel = Translator.ResourceModel_BE_IE.resourceType_BEtoIE(obsBuilder.GetResourceType(tenantID));
                return resTypeModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [Route("GetResourceType/{PlatformId=PlatformId}/{TenantId=TenantId}")]
        public IE.resourceTypeModelMap GetResourceType(int platformId, int tenantID)
        {
            try
            {
                ObservablePlanBuilder obsBuilder = new ObservablePlanBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.resourceTypeModelMap resTypeModel = Translator.ResourceModel_BE_IE.resourceType_BEtoIE(obsBuilder.GetResourceType(platformId, tenantID));
                return resTypeModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [Route("GetobservableModel/{TenantId=TenantId}")]
        public IE.observablemodelmap GetobservableModel(int tenantID)
        {
            try
            {
                ObservablePlanBuilder obsBuilder = new ObservablePlanBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.observablemodelmap obsModel = Translator.ResourceModel_BE_IE.observablemodel_BEtoIE(obsBuilder.GetobservableModel(tenantID));
                return obsModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [Route("GetActionTypeDteails/{TenantId=TenantId}")]
       public IE.actionTypeDetails GetActionTypeDteails(int tenantId)
        {
            try
            {
                ObservablePlanBuilder obsBuilder = new ObservablePlanBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.actionTypeDetails obsModel = Translator.ResourceModel_BE_IE.actionTypeDetails_BEtoIE(obsBuilder.getActionTypeDetails(tenantId));
                return obsModel;
            }

            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
