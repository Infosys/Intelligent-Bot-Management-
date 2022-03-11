$MethodsForEachComponent
#region ServiceComponent code for ##ServiceName##
        public bool Test##ServiceName##() 
        {
            try
            {
                ServiceComponent component = new ServiceComponent();
                ##DataEntity## dataEntity = Build##DataEntity##();
                dataEntity = component.Execute##ServiceName##(dataEntity);
                bool isValid = Validate##DataEntity##(dataEntity);
                return isValid;
            }
            catch(Exception exc)
            {
                return false;
            }
        }
        
        ##DataEntity## Build##DataEntity##()
        {
            ##DataEntity## dataEntity= new ##DataEntity##();
            /// The developer should build the data Entity here.
            return dataEntity;

        }
        bool Validate##DataEntity##(##DataEntity## dataEntity)
        {
            try
            {
                bool isValid = false;
                /// The developer should build the data Entity here.
                return isValid;
            }
            catch(Exception exc)
            {
                return false;
            }
        }

#endregion ServiceComponent code for ##ServiceName##
$NameSpaceOfEachDataEntity
using ##DataEntityNameSpace##.##DataEntity##;
$end


using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

using LifGenerated.ServiceComponents;

##NameSpaceOfDataEntities##


namespace LifGenerated.ServiceComponents.TestCases
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    public partial class TestCases {

        public TestCases() {

        }
##GeneratedCodeForAllComponents##

    }
}