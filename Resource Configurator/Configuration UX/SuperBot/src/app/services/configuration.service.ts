import { Injectable, OnInit } from '@angular/core';
import { HttpHeaders, HttpClient, HttpParams,HttpErrorResponse } from '@angular/common/http';
// import { HttpClient,HttpErrorResponse  } from '@angular/common/http';
// import { HttpHeaders, HttpParams } from '@angular/common/http';
import {  throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { EventEmitter } from '@angular/core';
import {PropertiesService} from '../services/properties.service';
import { environment } from '../../environments/environment';
import   properties  from  '../../assets/properties.json';
// import *  as properties   from  '../../assets/config/properties.json';  
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class ConfigurationService implements OnInit{

  private serverAddress = environment.server_address;
  private services_app_name = environment.services_app_name;
  public tenantId = environment.tenantId;
  public automationEngineJson = environment.automation_engine;
  public categoryService = environment.category_service;
settingsJson:any;

  constructor(private propServ:PropertiesService,private _HttpClient: HttpClient) {
    console.log(properties);
    // this.getProperties();
    
    // this.settingsJson=<any>sessionStorage.getItem('settingsJson');
    //  this.serverAddress = this.settingsJson.server_address;
    //  this.services_app_name = this.settingsJson.services_app_name;
    //  this.tenantId = this.settingsJson.tenantId;
    //  this.automationEngineJson = this.settingsJson.automation_engine;
    //  this.categoryService = this.settingsJson.category_service;
   }
ngOnInit(){
  // this.settingsJson=localStorage.getItem('settingsJson');
  // this.serverAddress = this.settingsJson.server_address;
  // this.services_app_name = this.settingsJson.services_app_name;
  // this.tenantId = this.settingsJson.tenantId;
  // this.automationEngineJson = this.settingsJson.automation_engine;
  // this.categoryService = this.settingsJson.category_service;
}
 

  public scriptDetailsvcJson = environment.script_detail_service;
  private REST_SERVICE_URI_GET_RESOURCE_CONFIGURATION = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GetResourceTypeConfiguration';
  private REST_SERVICE_URI_GET_RESOURCE_MODEL_CONFIGURATION = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GetActiveResourceModelConfiguration';
  private REST_SERVICE_URI_POST_UPADTE_RESOURCE_MODEL_CONFIGURATION = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/updateResourceModelConfiguration';
  private REST_SERVICE_URI_GET_OBSERVABLE_REMEDIATION_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/getObservablesandRemediationDetails';
  private REST_SERVICE_URI_GET_META_CONFIGURATION_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/Metadata/getResourceTypeMetaData';
  private REST_SERVICE_URI_POST_ADD_ATTRIBUTES = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/Metadata/AddResourceTypeAttributes';
  private REST_SERVICE_URI_POST_DELETE_ATTRIBUTES = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/Metadata/DeleteResourceTypeAttributes';
  private REST_SERVICE_URI_POST_UPDATE_METADATA = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/Metadata/UpdateResourceTypeMetaData';
  private REST_SERVICE_URI_GET_OBSERVABL_CONFIG = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/Observable/GetObservableDetails';
  private REST_SERVICE_URI_POST_ADD_OBSERVABL_CONFIG = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/Observable/AddObservableDetails';
  private REST_SERVICE_URI_POST_UPDATE_OBSERVABL_CONFIG = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/Observable/UpdateObservableDetails ';
  private REST_SERVICE_URI_GET_OBSERVABL_RESOURCETYPEMAP = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ObservableResourceTypeModel/GetObservableResourceTypeActionMapDetails';
  private REST_SERVICE_URI_UPDATE_OBSERVABL_RESOURCETYPEMAP = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ObservableResourceTypeModel/UpdateResourceTypeActionMap  ';
  private REST_SERVICE_URI_ADD_OBSERVABL_RESOURCETYPEMAP = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ObservableResourceTypeModel/AddResourceTypeActionMap ';
  private REST_SERVICE_URI_GET_ACTION_LIST = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ObservablePlan/GetActions';
  private REST_SERVICE_URI_GET_OBSERVABLE_LIST = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ObservablePlan/GetobservableModel';
  private REST_SERVICE_URI_GET_OBSERVABLE_RESOURCETYPE_LIST = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ObservablePlan/GetResourceType';
  private REST_SERVICE_URI_GET_RESOURCE_SUMMARY = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GetAllResourceModelConfiguration';
  private REST_SERVICE_URI_GET_ALL_PORTFOLIOS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GetAllPortfolios';
  private REST_SERVICE_URI_GET_ACTION_CONFIG_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ActionModel/GetActionDetails';
  private REST_SERVICE_URI_GET_ACTION_TYPES = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ObservablePlan/GetActionTypeDteails';
  private REST_SERVICE_URI_UPDATE_ACTION_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ActionModel/updateActionDetails';
  private REST_SERVICE_URI_ADD_ACTION_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ActionModel/addActionDetails';
  private REST_SERVICE_URI_GET_REMEDIATION_CONFIG_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/RemediationPlan/GetRemediationPlanDetails';
  private REST_SERVICE_URI_ADD_REMEDIATION_CONFIG_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/RemediationPlan/InsertRemediationPlanDetails';
  private REST_SERVICE_URI_UPADTE_REMEDIATION_CONFIG_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/RemediationPlan/UpdateRemediationPlanDetails';
  private REST_SERVICE_URI_DELETE_REMEDIATION_CONFIG_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/RemediationPlan/DeleteRemediationPlanDetails';
  private REST_SERVICE_URI_GET_REMEDIATION_PLAN_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/RemediationPlanObservableAndResourceType/GetRemediationPlanObservableAndResourceTypeDetails';
  private REST_SERVICE_URI_ADD_REMEDIATION_PLAN_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/RemediationPlanObservableAndResourceType/InsertRemediationPlanObservableAndResourceTypeDetails';
  private REST_SERVICE_URI_UPDATE_REMEDIATION_PLAN_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/RemediationPlanObservableAndResourceType/UpdateRemediationPlanObservableAndResourceTypeDetails';
  private REST_SERVICE_URI_DELETE_REMEDIATION_PLAN_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/RemediationPlanObservableAndResourceType/DeleteRemediationPlanObservableAndResourceTypeDetails';
  private REST_SERVICE_URI_GET_ALL_PLATFORM_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GetAllPlatformDetails';
  private REST_SERVICE_URI_GET_ANOMALY_RULES_CONFIGURATION = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/AnomalyRulesConfiguration/GetAnomalyRulesConfiguration';
  private REST_SERVICE_URI_UPDATE_ANOMALY_RULES_CONFIGURATION = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/AnomalyRulesConfiguration/UpdateAnomalyRulesConfiguration';
  private REST_SERVICE_URI_ADD_ANOMALY_RULES_CONFIGURATION = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/AnomalyRulesConfiguration/InsertAnomalyRulesConfiguration';
  private REST_SERVICE_URI_DELETE_ANOMALY_RULES_CONFIGURATION = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/AnomalyRulesConfiguration/DeleteAnomalyRulesConfiguration';
  private REST_SERVICE_URI_GET_RESOURCE_MODEL_SCREEN = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GenerateResourceModel';

  private REST_SERVICE_URI_GET_RESOURCE_TYPES = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ObservablePlan/GetResourceType';
  private REST_SERVICE_URI_POST_CONFIGURATION_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/UpdateCategoryandScriptId';
  private REST_SERVICE_GET_ACTION_SCRIPT_DETAILS = 'http://' + this.serverAddress + '/' + '/WEMScriptExecutor_Superbot/WEMScriptService.svc/GetAllScriptDetails/';

  private REST_SERVICE_GET_AA_BOT_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GetAADetails';
  private REST_SERVICE_GET_SEE_CATEGORIES = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/LoadSEECategories';

  private REST_SERVICE_GET_SEE_SCRIPT_DETAILS = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/LoadSEEScripts';
  private REST_SERVICE_GET_RESOURCE_SUMMARY = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GetSummaryViewConfiguration';
  private REST_SERVICE_UPDATE_RESOURCE_SUMMARY = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/UpdateSummaryViewConfiguration';
  private REST_SERVICE_URI_GET_RESOURCE_MODEL_UI_PATH = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GenerateResourceModelUiPath';
  private REST_SERVICE_URI_GET_RESOURCE_MODEL_UI_PATH_ON_PREMISE = 'http://' + this.serverAddress + '/' + this.services_app_name + '/api/ResourceModel/GenerateResourceModelUiPath';

 

  // private serverAddress = environment.server_address;
  // private services_app_name = environment.services_app_name;
  // public tenantId = environment.tenantId;
  // public automationEngineJson = environment.automation_engine;
  // public categoryService = environment.category_service;
  errorHandler(error: HttpErrorResponse) {
   console.error(error);
   return  throwError(error.message  ||  "Server Error");
 } 
  getJSON():Observable<any>{
    return  this._HttpClient.get("../../assets/properties.json");
  }
  public  getProperties(){
    this.getJSON().subscribe(res=>{
      this.serverAddress = res.server_address;
      this.services_app_name = res.services_app_name;
      this.tenantId = res.tenantId;
      this.automationEngineJson = res.automation_engine;
      this.categoryService = res.category_service;
    })
  }
  // http://vmgfpdstp-04/ConfigManagement/api/ResourceModel/GetSummaryViewConfiguration?PlatformInstanceId=1&TenantId=1&ResourceTypeName=Bot Runner
  // http://vmgfpdstp-04/ConfigManagement/api/ResourceModel/UpdateSummaryViewConfiguration 
  getResourceSummary(ResourceTypeName: any,PlatformInstanceId:string) {
    let params = new HttpParams().set('PlatformInstanceId', PlatformInstanceId).set('TenantId', this.tenantId).set('ResourceTypeName', ResourceTypeName);
    return this._HttpClient.get(this.REST_SERVICE_GET_RESOURCE_SUMMARY, {params:params})

  }
  updateSummaryDetails(inputjson: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.post(this.REST_SERVICE_UPDATE_RESOURCE_SUMMARY, inputjson, httpOptions)

  }
  getSEECategories(inputjson: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_GET_SEE_CATEGORIES, inputjson, httpOptions)

  }

  getSEEScriptDetails(inputjson: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_GET_SEE_SCRIPT_DETAILS, inputjson, httpOptions)

  }

  getAABotDetails() {
    return this._HttpClient.get(this.REST_SERVICE_GET_AA_BOT_DETAILS);
  }

  getActionScriptDetails(categoryid: string) {
    // let params = new HttpParams().set('PlatformId', platformId).set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_GET_ACTION_SCRIPT_DETAILS + categoryid);
  }



  configureActions(json: any) {
    // console.log(JSON.parse(JSON.stringify(json)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_POST_CONFIGURATION_DETAILS, json, httpOptions)
  }
  getResourceTypes(tenantId: string, platformId: string) {
    let params = new HttpParams().set('PlatformId', platformId).set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_RESOURCE_TYPES, { params: params })
  }

getResourceModelUipath(inputJson: any){
  
  console.log(JSON.parse(JSON.stringify(inputJson)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.post(this.REST_SERVICE_URI_GET_RESOURCE_MODEL_UI_PATH, inputJson, httpOptions).pipe(catchError(this.errorHandler));;

}
getResourceModelUipathOnPremise(inputJson: any){
  
   console.log(JSON.parse(JSON.stringify(inputJson)))
     const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
     return this._HttpClient.post(this.REST_SERVICE_URI_GET_RESOURCE_MODEL_UI_PATH_ON_PREMISE, inputJson, httpOptions);
 
 }

  getResourceModelScreen(inputJson: any) {
    console.log(JSON.parse(JSON.stringify(inputJson)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.post(this.REST_SERVICE_URI_GET_RESOURCE_MODEL_SCREEN, inputJson, httpOptions);


  }

  deleteAnomalyRulesConfiguration(json: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_DELETE_ANOMALY_RULES_CONFIGURATION, json, httpOptions)
  }
  addAnomalyRulesConfiguration(json: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.post(this.REST_SERVICE_URI_ADD_ANOMALY_RULES_CONFIGURATION, json, httpOptions)
  }
  updateAnomalyRulesConfiguration(json: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_UPDATE_ANOMALY_RULES_CONFIGURATION, json, httpOptions)
  }

  getAnomalyRulesConfiguration(tenantId: string, observableId: string, resourcetypeId: string, platformId: string) {
    let params = new HttpParams().set('observableId', observableId).set('resourcetypeId', resourcetypeId).set('platformId', platformId).set('tenantId', this.tenantId);;
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_ANOMALY_RULES_CONFIGURATION, { params: params })
  }

  getAllPlatforms(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId);
    // let params = new HttpParams().set('TenantId', TenantId).set('PlatformId',PlatformId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_ALL_PLATFORM_DETAILS, { params: params }).pipe(catchError(this.errorHandler));
  }
  deleteRemediationPlanDetails(json: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_DELETE_REMEDIATION_PLAN_DETAILS, JSON.parse(JSON.stringify(json)), httpOptions)

  }

  deleteRemediationConfigDetails(json: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_DELETE_REMEDIATION_CONFIG_DETAILS, JSON.parse(JSON.stringify(json)), httpOptions)

  }
  updateRemediationPlanDetails(json: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_UPDATE_REMEDIATION_PLAN_DETAILS, JSON.parse(JSON.stringify(json)), httpOptions)
  }
  addRemediationPlanDetails(json: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_ADD_REMEDIATION_PLAN_DETAILS, JSON.parse(JSON.stringify(json)), httpOptions)
  }
  getRemediationPlanDetails(PlatformId: string, TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId);
    // let params = new HttpParams().set('TenantId', TenantId).set('PlatformId',PlatformId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_REMEDIATION_PLAN_DETAILS, { params: params })
  }

  getRemediationConfigDetails(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_REMEDIATION_CONFIG_DETAILS, { params: params })
  }
  getActionTypes(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_ACTION_TYPES, { params: params })
  }
  addRemediationConfigDetails(json: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_ADD_REMEDIATION_CONFIG_DETAILS, JSON.parse(JSON.stringify(json)), httpOptions)
  }
  updateRemediationConfigDetails(json: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_UPADTE_REMEDIATION_CONFIG_DETAILS, JSON.parse(JSON.stringify(json)), httpOptions)
  }
  addActionDetails(json: any) {
    console.log(JSON.parse(JSON.stringify(json)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_ADD_ACTION_DETAILS, JSON.parse(JSON.stringify(json)), httpOptions)
  }
  updateActionDetails(json: any) {
    console.log(JSON.parse(JSON.stringify(json)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_UPDATE_ACTION_DETAILS, JSON.parse(JSON.stringify(json)), httpOptions)
  }
  getActionConfigDetails(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_ACTION_CONFIG_DETAILS, { params: params })
  }
  getPortfolioDetails(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_ALL_PORTFOLIOS, { params: params })
  }
  getResourceConfiguration(PlatformInstanceId: string, TenantId: string) {
    let params = new HttpParams().set('PlatformInstanceId', PlatformInstanceId).set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_RESOURCE_CONFIGURATION, { params: params })
  }

  getResourceModelConfiguration(PlatformInstanceId: string, TenantId: string, ResourceTypeName: string) {
    let params = new HttpParams().set('PlatformInstanceId', PlatformInstanceId).set('TenantId', this.tenantId).set('ResourceTypeName', ResourceTypeName);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_RESOURCE_MODEL_CONFIGURATION, { params: params })
  }

  updateResourceModelConfiguration(inputResources: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.post(this.REST_SERVICE_URI_POST_UPADTE_RESOURCE_MODEL_CONFIGURATION, inputResources, httpOptions);
  }
  getObservableConfiguration(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_OBSERVABL_CONFIG, { params: params })

  }
  addObservableConfiguration(inputResources: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_POST_ADD_OBSERVABL_CONFIG, inputResources, httpOptions);

  }
  updateObservableConfiguration(inputResources: any) {
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_POST_UPDATE_OBSERVABL_CONFIG, inputResources, httpOptions);

  }
  getObservablesAndRemediationDetails(PlatformId: string, TenantId: string, ResourceTypeName: string) {
    let params = new HttpParams().set('PlatformId', PlatformId).set('TenantId', this.tenantId).set('ResourceTypeName', ResourceTypeName);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_OBSERVABLE_REMEDIATION_DETAILS, { params: params })
  }

  getMetaData(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId)
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_META_CONFIGURATION_DETAILS, { params: params })
  }

  addAttributes(json: any) {
    console.log(JSON.parse(JSON.stringify(json)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_POST_ADD_ATTRIBUTES, JSON.parse(JSON.stringify(json)), httpOptions)
  }

  deleteAttributes(json: any) {
    console.log(JSON.parse(JSON.stringify(json)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.post(this.REST_SERVICE_URI_POST_DELETE_ATTRIBUTES, JSON.parse(JSON.stringify(json)), httpOptions)
  }


  updateResourceTypeMetaData(inputJson: any) {
    console.log(JSON.parse(JSON.stringify(inputJson)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.post(this.REST_SERVICE_URI_POST_UPDATE_METADATA, inputJson, httpOptions);
  }
  getObservableResourceTypeActionMap(TenantId: string) {
    // let params = new HttpParams().set('PlatformId', PlatformId).set('TenantId', TenantId);
    let params = new HttpParams().set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_OBSERVABL_RESOURCETYPEMAP, { params: params })
  }
  updateObservableResourceTypeActionMap(inputJson: any) {
    console.log(JSON.parse(JSON.stringify(inputJson)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_UPDATE_OBSERVABL_RESOURCETYPEMAP, inputJson, httpOptions);
  }
  addObservableResourceTypeActionMap(inputJson: any) {
    console.log(JSON.parse(JSON.stringify(inputJson)))
    const httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
    return this._HttpClient.put(this.REST_SERVICE_URI_ADD_OBSERVABL_RESOURCETYPEMAP, inputJson, httpOptions);
  }
  getActionModelList(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId)
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_ACTION_LIST, { params: params })
  }

  getObservableModelList(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId)
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_OBSERVABLE_LIST, { params: params })
  }
  getResourceTypeModelList(TenantId: string) {
    let params = new HttpParams().set('TenantId', this.tenantId);

    // let params = new HttpParams().set('PlatformId', PlatformId).set('TenantId', TenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_OBSERVABLE_RESOURCETYPE_LIST, { params: params })
  }




  getSummaryDetails(PlatformInstanceId: any, TenantId: any) {
    let params = new HttpParams().set('PlatformInstanceId', PlatformInstanceId).set('TenantId', this.tenantId);
    return this._HttpClient.get(this.REST_SERVICE_URI_GET_RESOURCE_SUMMARY, { params: params })

  }


  getDiscoveryDetails() {

    return [
      {
         "name":"Automation Anywhere Enterprise",
         "tenantId":"",
         "labels":[
            {
               "name":"Platform Name",
               "type":"text",
               "value":""
            },
            {
               "name":"Host Name",
               "type":"text",
               "value":""
            }
         ],
         "sections":[
            {
               "name":"Control Tower Details",
               "labels":[
                  {
                     "name":"Control Tower URL",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Control Tower User Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Control Tower Password",
                     "type":"password",
                     "value":""
                  },
                  {
                     "name":"IP Address",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Service User Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Service Password",
                     "type":"password",
                     "value":""
                  }
               ]
            },
            {
               "name":"Database Details",
               "labels":[
                  {
                     "name":"Database Host Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Database Type",
                     "type":"dropdown",
                     "value":"",
                     "values":[
                        {
                           "name":"Oracle",
                           "value":"Oracle"
                        },
                        {
                           "name":"SQL Sever",
                           "value":"SQL Sever"
                        }
                     ]
                  },
                  {
                     "name":"Database IP Address",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Database Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Database User Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Database Password",
                     "type":"password",
                     "value":""
                  }
               ]
            }
         ]
      },
      {
         "name":"UiPath",
         "tenantId":"",
         "labels":[
            {
               "name":"Platform Name",
               "type":"text",
               "value":""
            }
         ],
         "sections":[
            {
               "name":"Orchestrator Details",
               "labels":[
                  {
                     "name":"Orchestrator URL",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Authentication URL",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Account Logical Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Tenant Logical Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Client Id",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Refresh Token",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"UiPath TenantName",
                     "type":"text",
                     "value":""
                  }
               ]
            }
         ]
      },
      {
         "name":"UiPath on-Premise",
         "tenantId":"",
         "labels":[
            {
               "name":"Platform Name",
               "type":"text",
               "value":""
            }
            
         ],
         "sections":[
            {
               "name":"Orchestrator Details",
               "labels":[
                  {
                     "name":"Orchestrator URL",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Orchestrator User Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Orchestrator Password",
                     "type":"password",
                     "value":""
                  },
                  
                  {
                     "name":"IP Address",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Service User Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Service Password",
                     "type":"password",
                     "value":""
                  },{
                    "name":"Authentication URL",
                    "type":"text",
                    "value":""
                 },
                  {
                     "name":"UiPath Tenant Name",
                     "type":"text",
                     "value":""
                  }
               ]
            },
            {
               "name":"Database Details",
               "labels":[
                  {
                     "name":"Database Host Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Database Type",
                     "type":"dropdown",
                     "value":"",
                     "values":[
                        {
                           "name":"Oracle",
                           "value":"Oracle"
                        },
                        {
                           "name":"SQL Sever",
                           "value":"SQL Sever"
                        }
                     ]
                  },
                  {
                     "name":"Database IP Address",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Database Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Database User Name",
                     "type":"text",
                     "value":""
                  },
                  {
                     "name":"Database Password",
                     "type":"password",
                     "value":""
                  }
               ]
            }
         ]
      }
   ]
  }
  downloadFile(data, filename='data',header:any[]) {
    // ['name','age', 'average', 'approved', 'description']
    let csvData = this.ConvertToCSV(data, header);
    console.log(csvData)
    let blob = new Blob(['\ufeff' + csvData], { type: 'text/csv;charset=utf-8;' });
    let dwldLink = document.createElement("a");
    let url = URL.createObjectURL(blob);
    let isSafariBrowser = navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1;
    if (isSafariBrowser) {  //if Safari open in new window to save file with random filename.
        dwldLink.setAttribute("target", "_blank");
    }
    // filename="osdata";
    dwldLink.setAttribute("href", url);
    dwldLink.setAttribute("download", filename + ".csv");
    dwldLink.style.visibility = "hidden";
    document.body.appendChild(dwldLink);
    dwldLink.click();
    document.body.removeChild(dwldLink);
}

ConvertToCSV(objArray, headerList) {
     let array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
     let str = '';
     let row = 'S.No,';

     for (let index in headerList) {
         row += headerList[index] + ',';
     }
     row = row.slice(0, -1);
     str += row + '\r\n';
     for (let i = 0; i < array.length; i++) {
         let line = (i+1)+'';
         for (let index in headerList) {
            let head = headerList[index];

             line += ',' + array[i][head];
         }
         str += line + '\r\n';
     }
     return str;
 }
}








