$MethodsForEachComponent
#region ServiceComponent code for ##ServiceName##
        public ##DataEntity## Execute##ServiceName##(##DataEntity## dataEntity) 
        {
            LegacyEvent eventHandler = new LegacyEvent();
            eventHandler.ResponseReceived += new RequestDelegate(RetrieveResponseOf##ServiceName##);
            RequestParameters requestParameters = new RequestParameters();
            requestParameters.RequestCollection.Add("RequestType", "S");
            ServiceManager.Execute(eventHandler, ServiceManager.ProcessMode.WaitForAll, "##ServiceName##", dataEntity, requestParameters);
            return returnObject;
        }
        
        ##DataEntity## returnObject = null;

        void RetrieveResponseOf##ServiceName##(object sender, RequestDelegateArgs responses)
        {
            returnObject = (##DataEntity##)responses.Response["##ServiceName##"];
        }
#endregion ServiceComponent code for ##ServiceName##
$NameSpaceOfEachDataEntity
using ##DataEntityNameSpace##.##DataEntity##;
$end

using System;
using System.Web;
using System.Collections;


using Infosys.Lif.LegacyFacade;
using Infosys.Lif.LegacyParameters;
##NameSpaceOfDataEntities##

namespace LifGenerated.ServiceComponents
{
    /// <summary>
    /// Summary description for ServiceComponent
    /// </summary>
    public partial class ServiceComponent {

        public ServiceComponent() {

        }
##GeneratedCodeForAllComponents##

    }

}