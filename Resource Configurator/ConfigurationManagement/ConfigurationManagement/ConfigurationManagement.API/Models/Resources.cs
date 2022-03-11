using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models
{
    public class Resources
    {
        public string platformid { get; set; }        
        public int tenantid { get; set; }
        public string platformtype { get; set; }
        public string resourcemodelversion { get; set; }
        public string status { get; set; }       
       
        public List<ResourceDetails> resourcedetails { get; set; }
        //public List<observablesandremediationplan> observablesandremediationplans { get; set; }
        public List<LogDetails> logdetails { get; set; }

    }
    public class SummaryResourceInfo
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public List<SummaryResource> resourcedetails { get; set; }

    }
    public class SummaryResource
    {
        public string resourcename { get; set; }
        public string resourceid { get; set; }
        public bool isactive { get; set; }
        public List<SummaryResourceParent> parentdetails { get; set; }
        public List<SummaryResourceChild> childdetails { get; set; }
    }

    public class SummaryResourceParent
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public string resourcename { get; set; }
        public string resourceid { get; set; }
    }
    public class SummaryResourceChild
    {
        public string resourcename { get; set; }
        public string resourceid { get; set; }
        public bool isactive { get; set; }
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
    }
    public class ResourceDetails
    {
        public string resourcename { get; set; }
        public string resourceid { get; set; }
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public string portfolioid { get; set; }
        public bool dontmonitor { get; set; }
        public bool cascadetochild { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public List<ParentDetails> parentdetails { get; set; }
        public List<LogDetails> logdetails { get; set; }
        public List<ResourceAttribute> resourceattribute { get; set; }
        public List<ChildResource> childdetails { get; set; }
        public List<observablesandremediation> observablesandremediations { get; set; }
        public string source { get; set; }

    }
    public class ParentDetails
    {
        public string resourcename { get; set; }
        public string resourceid { get; set; }
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }

    }
    public class LogDetails
    {
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }

    }
    public class ResourceAttribute
    {
        public string attributename { get; set; }
        public string attributevalue { get; set; }
        public string displayname { get; set; }
        public string description { get; set; }
        public bool IsSecret { get; set; }
        public string attributeCategory { get; set; }

        public string remarks { get; set; }

        public bool Ishiddeninui { get; set; }
    }

    public class ChildResource
    {
        public string resourcename { get; set; }
        public string resourceid { get; set; }
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public string portfolioid { get; set; }
        public bool dontmonitor { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }

        public List<ResourceAttribute> resourceattribute { get; set; }
        public List<observablesandremediation> observablesandremediations { get; set; }
        public string source { get; set; }

    }

    public class observablesandremediationplan
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }

        public List<observablesandremediation> observablesandremediations { get; set; }
    }

    public class observablesandremediation
    {
        public bool ismodified { get; set; }
        public string name { get; set; }
        public string ObservableId { get; set; }
        public string ObservableName { get; set; }
        public string ObservableActionId { get; set; }
        public string ObservableActionName { get; set; }
        public string RemediationPlanId { get; set; }
        public string RemediationPlanName { get; set; }
        public bool isObsSelected { get; set; }
        public bool isRemSelected { get; set; }
    }

    public class ObservableResourceTypeActionMap
    {
        public int PlatformID { get; set; }
        public int tenantID { get; set; }
        public List<ObservableResourceTypeAction> observableresourcetypeactions { get; set; }

    }

    public class ObservableResourceTypeAction
    {
        public string name { get; set; }
        public string resourcetypename { get; set; }
        public int resourcetypeid { get; set; }
        public int observableid { get; set; }
        public string observablename { get; set; }
        public int actionid { get; set; }
        public string actionname { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }

    }

    public class addResourceTypeObservableActionMap
    {
        public string Message { get; set; }
    }

    public class dummy
    {
        public string name { get; set; }
        public int id { get; set; }

    }

    public class observable
    {

        public int tenantId { get; set; }
        public List<observableDetails> observableDetails { get; set; }

    }
    public class observableDetails
    {
        public int observableid { get; set; }
        public string observablename { get; set; }
        public string unitofmeasure { get; set; }
        public string createdby { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
        
        public string datatype { get; set; }

    }

    public class addObservableDetails
    {
        public string Message { get; set; }
    }
    public class addRemediationPlanDetails
    {
        public string Message { get; set; }
    }

    public class addActionMessageDetails
    {
        public string Message { get; set; }
    }

    public class addRemePlanObsAndResourceTypeMap
    {
        public string Message { get; set; }
    }

    public class action
    {
        public int tenantid { get; set; }
        public List<actiondetails> actiondetails { get; set; }
    }

    public class actiondetails
    {
        public int actionid { get; set; }
        public string actionname { get; set; }
        public int actiontypeid { get; set; }
        public string actiontype { get; set; }
        public string endpointuri { get; set; }
        public int scriptid { get; set; }
        public int categoryid { get; set; }
        public int automationengineid { get; set; }
        public string automationenginename { get; set; }
        public string createdby { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ValidityStart { get; set; }
        public string ValidityEnd { get; set; }
        public List<actionParams> actionparams { get; set; }
        public string categoryname { get; set; }

    }
    public class actionParams
    {
        public string name { get; set; }
        public string fieldtomap { get; set; }
        public bool ismandatory { get; set; }
        public string defaultvalue { get; set; }
        public int automationengineparamid { get; set; }
        public bool isDeleted { get; set; }

    }
    #region Models for getResourceTypeConfiguration service

    public class ResourceTypeConfiguration
    {
        public string platformid { get; set; }
        public int tenantid { get; set; }
        public string platformtype { get; set; }
        public List<ResourceTypeDetails> resourcetypedetails { get; set; }
    }

    public class ResourceTypeDetails
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public List<ResDetails> details { get; set; }
    }
    public class ResDetails
    {
        public string resourceid { get; set; }
        public string resourcename { get; set; }
    }

    #endregion

    #region Models for getObservationRemediationDetails
    public class ObservablesandRemediationPlanDetails
    {
        public int platformid { get; set; }
        public int tenantid { get; set; }
        public List<ObrandRemDetails> observablesandremediationplans { get; set; }
    }

    public class ObrandRemDetails
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }

        public List<ObrandRem> observablesandremediations { get; set; }
    }

    public class ObrandRem
    {
        public string name { get; set; }
        public string ObservableId { get; set; }
        public string ObservableName { get; set; }
        public string ObservableActionId { get; set; }
        public string ObservableActionName { get; set; }
        public string RemediationPlanId { get; set; }
        public string RemediationPlanName { get; set; }
        public bool isObsSelected { get; set; }
        public bool isRemSelected { get; set; }
    }
    #endregion

    #region Model for updateResourceModelConfiguration
    public class ResourceModelUpdate
    {
        public string Message { get; set; }
        
    }

    #endregion


    #region Model for getresourcemodelView
    public class Resourcetypedetail
    {
        public string resourceid { get; set; }
        public string resourcename { get; set; }
    }

    public class Childdetail
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public bool ismainentiy { get; set; }
        public List<Resourcetypedetail> resourcetypedetails { get; set; }
    }

    /*public class Resourcetypedetail2
    {
        public string resourceid { get; set; }
        public string resourcename { get; set; }
    }

    public class Childdetail2
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public List<Resourcetypedetail2> resourcetypedetails { get; set; }
    }*/

    public class Detail
    {
        public string resourceid { get; set; }
        public string resourcename { get; set; }
        public List<Childdetail> childdetails { get; set; }
    }

    public class Resourcedetail
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public bool ismainentiy { get; set; }
        public List<Detail> details { get; set; }
    }

    public class Resourcemodeldetail
    {
        public string resourcetypename { get; set; }
        public string resourcetypeid { get; set; }
        public string resourceid { get; set; }
        public string resourcename { get; set; }
        public bool ismainentiy { get; set; }
        public List<Childdetail> childdetails { get; set; }
        public List<Resourcedetail> resourcedetails { get; set; }
    }

    public class ResourceModel
    {
        public string tenantid { get; set; }
        public string platformid { get; set; }
        public string platformname { get; set; }
        public List<Resourcemodeldetail> resourcemodeldetails { get; set; }
    }
    #endregion

    public class RemediationPlanDetails
    {
        public int tenantId { get; set; }
        public List<RemediationPlan> RemediationPlans { get; set; }

    }

    public class RemediationPlan
    {
        public int remediationId { get; set; }
        public string remediationPlanName { get; set; }
        public string RemediationPlanDescription { get; set; }
        public bool IsUserDefined { get; set; }
        public List<ActionRemdiation> ActionDetails { get; set; }




    }

    public class ActionRemdiation
    {
        public int ActionId { get; set; }
        public short ActionSequence { get; set; }
        public string ActionStageId { get; set; }
        public string ActionName { get; set; }
        public int RemediationPlanActionId { get; set; }
        public bool isDeleted { get; set; }
    }

    public class RemediationPlanObservableAndResourceType
    {
        public string resourcetypename { get; set; }
        public int resourcetypeid { get; set; }
        public int ObservableId { get; set; }
        public string ObservableName { get; set; }
        public int RemediationPlanId { get; set; }
        public string RemediationPlanName { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }




    }

    public class RemediationPlanObservableAndResourceTypeMap
    {
        public int platformid { get; set; }
        public int tenantid { get; set; }
        public List<RemediationPlanObservableAndResourceType> RemediationPlanObservableAndResourceTypeList { get; set; }


    }
    public class actionModelMap
    {
        public int tenantID { get; set; }
        public List<actionModel> actList { get; set; }
    }
    public class actionModel
    {
        public int actionid { get; set; }
        public string actionname { get; set; }
        public int actiontypeid { get; set; }
        
        public string endpointuri { get; set; }
        public int scriptid { get; set; }
        public int categoryid { get; set; }
        public int automationengineid { get; set; }
        public string createdby { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
    }

    public class resourceTypeModelMap
    {
        public int tenantid { get; set; }
        public int platformid { get; set; }
        public List<resourceTypeModel> resTypeList { get; set; }

    }
    public class resourceTypeModel
    {
        public string resourcetypename { get; set; }
        public int resourcetypeid { get; set; }
        
    }

    public class observablemodelmap
    {
        public int tenentId { get; set; }
        public List<observableModel> obsList { get; set; }
    }

    public class observableModel
    {
        public int observableid { get; set; }
        public string observablename { get; set; }
        public string unitofmeasure { get; set; }
        public string createdby { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }

        public string datatype { get; set; }

    }
    public class actiontype
    {
        public int ActionTypeId { get; set; }
        public string ActionType { get; set; }
        public string createdby { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
    public class actionTypeDetails
    {
        public List<actiontype> actionTypeList { get; set; }
        public int TenantId { get; set; }

    }
    #region portfolios
    public class PortfolioInfo
    {
        public string portfolioid { get; set; }
        public string portfolioname { get; set; }
    }
    #endregion

    #region Generate Resource Model  
    public class ResourceModelGenerationReqMsg
    {
        public int Tenantid { get; set; }
        public string Platformname { get; set; }
        public string Platformtype { get; set; }
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string Database_HostName { get; set; }
        public string Database_Type { get; set; }
        public string Database_IPaddress { get; set; }
        public string Database_Name { get; set; }
        public string Database_UserName { get; set; }
        public string Database_Password { get; set; }
        public string API_Password { get; set; }
        public string API_UserName { get; set; }
        public string API_URL { get; set; }
        public string Service_UserName { get; set; }
        public string Service_Password { get; set; }

    }
    public class ResourceModelGenerationResMsg
    {
        public int Tenantid { get; set; }
        public int PlatformId { get; set; }
        public string ResourceTypeName { get; set; }

    }

    #endregion

    #region DynamicAutomationTypes

    public class Label
    {
        public string name { get; set; }
        public string type { get; set; }
        public List<Value> values { get; set; }
    }
    public class Section
    {
        public string name { get; set; }
        public List<Label> labels { get; set; }
    }
    public class Value
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class RPAType
    {
        public string name { get; set; }
        public List<Label> labels { get; set; }
        public List<Section> sections { get; set; }
    }
    #endregion   

}
