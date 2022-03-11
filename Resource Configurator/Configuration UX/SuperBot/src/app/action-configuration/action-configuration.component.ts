import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { ActionConfig, Actiondetail, Actionparam } from '../model/action-config.model';
import { ConfigurationService } from '../services/configuration.service';
import { AcionScript, Params } from '../model/action-script.model';
import { ToastrManager } from 'ng6-toastr-notifications';
import { LocalDataSource } from '../ng2-smart-table';
import { environment } from '../../environments/environment';
import { FormGroup, FormControl, NgForm } from '@angular/forms';
declare var $: any;

@Component({
  selector: 'app-action-configuration',
  templateUrl: './action-configuration.component.html',
  styleUrls: ['./action-configuration.component.css']
})

export class ActionConfigurationComponent implements OnInit {

  constructor(private ref: ChangeDetectorRef, public toastr: ToastrManager, private confService: ConfigurationService) { }
  @Input() ActionId;
  searchScriptVal: string;
  searchBotVal: string;
  isUpdateType: boolean;
  actionConfig: ActionConfig;
  actiondetail: Actiondetail;
  isLoader: boolean = false;
  categories: any;
  actionTypes: any;
  actions: any[];
  actionsVisible: any[];
  actionJson: any;
  aeJson: any[];
  csJson: any[];
  sdJson: any[];
  botDetails: any[];
  botDetailsVisible: any[];
  settings = {
    bigBanner: false,
    timePicker: false,
    format: 'dd-MM-yyyy',
    defaultOpen: false,
    closeOnSelect: true
  }
  actionParam: Actionparam;
  actionParamDef: Actionparam;
  configDetails: ConfigDetails;
  actionScriptsVisible: any;
  actionScripts: any;

  source: LocalDataSource;

  onDeleteConfirm(event) {
    console.log("Edit Event In Console")
    console.log(event);
    event.confirm.resolve();
  }
  onSaveConfirm(event) {
    console.log("Edit Event In Console")
    console.log(event);
    event.confirm.resolve();
  }

  ngOnInit() {
    this.aeJson = JSON.parse(JSON.stringify(this.confService.automationEngineJson));
    this.csJson = JSON.parse(JSON.stringify(this.confService.categoryService));
    this.sdJson = JSON.parse(JSON.stringify(this.confService.scriptDetailsvcJson))
    console.log(this.ActionId);
    this.isUpdateType = true;
    this.configDetails = {

    }
    this.actionParam = {


    };
    this.actions = [];
    this.actionTypes = [];
    this.isLoader = false;
    // debuggee
    this.getActionDetails(true);
    this.getationTypes();
    this.actiondetail = {
      "actionid": 0,
      "actionname": "",
      "actiontypeid": null,
      "actiontype": "",
      "endpointuri": null,
      "scriptid": null,
      "categoryid": null,
      "automationenginename": "",
      "createdby": "",
      "ModifiedBy": "",
      "CreateDate": "",
      "ModifiedDate": "",
      "ValidityStart": new Date(),
      "ValidityEnd": new Date(),
      "actionparams": []
    }
    this.actionScriptsVisible = {
      Scripts: []
    };


  }


  selectBotBtn() {
    this.actiondetail.actionparams = [];
    if (this.actiondetail.automationengineid != undefined && this.actiondetail.automationengineid != 0) {
      if (!this.isUpdateType) {

        $("#ActionBotmodel").modal("show");

        this.isLoader = true;
        this.confService.getAABotDetails().subscribe(res => {
          this.isLoader = false;
          this.botDetails = <any[]>res;
          var detailsjson = <any[]>res;
          this.botDetailsVisible = JSON.parse(JSON.stringify(detailsjson));
        })
        // this.actionScriptsVisible = {};
        // this.actionScripts = {};
      }

    }
  }
  onSelectBot(botDetail: BotDetail) {
    $("#ActionBotmodel").modal("hide");
    try {
      this.actiondetail.actionname = botDetail.Name;
      // this.actiondetail.categoryid=acionScript.CategoryId;
      this.actiondetail.scriptid = parseInt(botDetail.botid.trim());
      this.actiondetail.actionparams = null;
    } catch (error) {
      console.log(error);
    }

  }
  selectCategory() {
    this.actiondetail.actionparams = [];
    if (this.actiondetail.automationengineid != undefined && this.actiondetail.automationengineid != 0) {
      if (!this.isUpdateType) {
        $("#ActionScriptmodel").modal("show");



        var inputJson = {
          "SEEBaseUrl": environment.server_address,
          "ServiceName": "WEMScriptExecutor_Superbot/WEMCommonService.svc/GetAllCategoriesByCompany?companyId=1&module=2"

        }
        this.isLoader = true;

        console.log(JSON.stringify(inputJson))
        this.confService.getSEECategories(JSON.stringify(inputJson)).subscribe(res => {
          this.isLoader = false;
          this.categories = <any>res;
        })
        this.actionScriptsVisible = {};
        this.actionScripts = {};
      }

    }

  }
  onSelectCategory(event) {
    this.actiondetail.categoryname = event.node.data.path;
    console.log()
    this.actiondetail.categoryid = event.node.id;
    console.log(event);
    this.actionScriptsVisible = {};
    this.actionScripts = {};
    this.getActionScriptdetails(event.node.id);
  }
  searchScript(value: string) {
    var tempScripts = JSON.parse(JSON.stringify(this.actionScripts)).Scripts.slice(0);
    if (value != "") {
      this.actionScriptsVisible.Scripts = JSON.parse(JSON.stringify(tempScripts)).filter(script => {
        var nameHasFilterText;
        nameHasFilterText = script.Name.toLowerCase().indexOf(value.toLowerCase()) >= 0
        if (nameHasFilterText) {
          return nameHasFilterText
        }
      })
      this.ref.detectChanges();
    }
    else {
      this.actionScriptsVisible.Scripts = JSON.parse(JSON.stringify(tempScripts)).slice(0);
      this.ref.detectChanges();
    }
  }

  searchBot(value: string) {
    var tempScripts = JSON.parse(JSON.stringify(this.botDetails)).slice(0);
    if (value != "") {
      this.botDetailsVisible = JSON.parse(JSON.stringify(tempScripts)).filter(script => {
        var nameHasFilterText;
        nameHasFilterText = script.Name.toLowerCase().indexOf(value.toLowerCase()) >= 0
        if (nameHasFilterText) {
          return nameHasFilterText
        }
      })
      this.ref.detectChanges();
    }
    else {
      this.botDetailsVisible = JSON.parse(JSON.stringify(tempScripts)).slice(0);
      this.ref.detectChanges();
    }
  }
  resetScriptModel() {
    this.actionScriptsVisible = {};
    this.actionScripts = {};
  }
  cancelScriptModel() {
    $("#ActionScriptmodel").modal("hide");
    // this.nodes=[];
    this.resetScriptModel();
  }
  onSelectScript(acionScript: AcionScript) {

    $("#ActionScriptmodel").modal("hide");

    this.actiondetail.actionname = acionScript.Name;
    // this.actiondetail.categoryid=acionScript.CategoryId;
    this.actiondetail.scriptid = acionScript.ScriptId;
    this.actiondetail.actionparams = []
    // this.actiondetail.actionparams=acionScript.Parameters;

    if (acionScript.Params != undefined && acionScript.Params.length > 0) {

      acionScript.Params.forEach(script => {
        this.actiondetail.actionparams.push({
          "name": script.Name,
          "fieldtomap": "",
          "ismandatory": script.IsMandatory.toString(),
          "defaultvalue": script.DefaultValue,
          "automationengineparamid": "",
          "isdeleted": false,

        });
      })

    }



  }
  changeFieldType(value: string) {
    this.actionParam.fieldtomap = value;
  }
  changeFieldTypeTable(value: string, actionParam: Actionparam) {
    actionParam.fieldtomap = value;
  }
  getActionScriptdetails(categoryid: string) {
    // $("#ActionScriptmodel")
    var inputJson = {
      "CategoryId": categoryid,
      "SEEBaseUrl": environment.server_address,
      "ServiceName": "WEMScriptExecutor_Superbot/WEMScriptService.svc/GetAllScriptDetails/"
    }

    this.isLoader = true;
    this.confService.getSEEScriptDetails(JSON.stringify(inputJson)).subscribe(res => {
      this.isLoader = false;
      this.actionScriptsVisible = res;
      this.actionScripts = JSON.parse(JSON.stringify(res));
    }, error => {
      console.log(error);
      this.isLoader = false;
      // this.toastr.errorToastr("Error occured !", 'Error!');
    })
  }
  getationTypes() {

    this.confService.getActionTypes("1").subscribe(res => {

      try {
        this.actionJson = res;
        this.actionTypes = JSON.parse(JSON.stringify(res)).actionTypeList;
      } catch (e) {

      }
    }, error => {
      console.log(error);
      this.isLoader = false;
      // this.toastr.errorToastr("Error occured !", 'Error!');
    })
  }
  onChangeAction(id: string) {
    this.actiondetail = {};
    this.actiondetail.actionparams = [];
    if (id != undefined && id != "") {


      this.actionConfig.actiondetails.forEach(action => {

        if (action.actionid.toString() === id.toString()) {
          this.actiondetail = action;
          action.actionparams.forEach(param => {
            param.isdeleted = false;

          })
          this.actiondetail.actionparams = JSON.parse(JSON.stringify(action.actionparams));
          // this.source= new LocalDataSource(this.actiondetail.actionparams);
        }
      })
    }
  }
  onChangeAutomationEngine(id: string) {
    if (!this.isUpdateType) {
      this.actiondetail.automationengineid = parseInt(id);
    }
    this.actionsVisible = []
    this.actionConfig.actiondetails.forEach(action => {
      if (action.automationengineid.toString() == id) {
        this.actionsVisible.push({ "actionid": action.actionid, "actionname": action.actionname })
      }

    })

  }
  getActionDetails(isOnLoad: boolean) {

    this.confService.getActionConfigDetails("1").subscribe(res => {
      this.actionConfig = <ActionConfig>res;
      this.actions = [];
      this.actionsVisible = []
      this.actionConfig.actiondetails.forEach(action => {

        this.actionsVisible.push({ "actionid": action.actionid, "actionname": action.actionname })
      })

      this.actiondetail = {};
      if (isOnLoad) {
        if (this.ActionId != undefined) {
          this.onChangeAction(this.ActionId);
          try {
            // $("#actionnamesel").val(this.ActionId).change();
          } catch (e) {

          }
        }
      }

    }, error => {
      console.log(error);
      this.isLoader = false;
      // this.toastr.errorToastr("Error occured !", 'Error!');
    })
  }

  addParameter() {


    if (this.actionParam.name != undefined && this.actionParam.name.length > 0) {
      // console.log(this.actionParam);
      if (this.actiondetail.actionparams == undefined)
        this.actiondetail.actionparams = [];

      this.actionParam.isdeleted = false;
      // this.actionParam.ismandatory = "false";
      this.actionParam.automationengineparamid = "";
      this.actiondetail.actionparams.push(this.actionParam);
      this.actionParam = {

        "name": "",
        "fieldtomap": "",
        "ismandatory": "false",
        "defaultvalue": "",
        "automationengineparamid": "",
        "isdeleted": false
      };
      // console.log(this.actiondetail);
    }

  }

  reset() {
    this.isUpdateType = true;
    this.actiondetail = {
      "actionid": 0,
      "actionname": "",
      "actiontypeid": null,
      "actiontype": "",
      "endpointuri": null,
      "scriptid": null,
      "categoryid": null,
      "automationenginename": "",
      "createdby": "",
      "ModifiedBy": "",
      "CreateDate": "",
      "ModifiedDate": "",
      "ValidityStart": new Date(),
      "ValidityEnd": new Date(),
      "actionparams": []
    }
    this.getActionDetails(false);
  }

  togglePageType(flag: boolean) {
    this.isUpdateType = flag;

    //for update screen
    if (flag) {

    } else {
      this.actiondetail = {
        "actionid": 0,
        "actionname": "",
        "actiontypeid": null,
        "actiontype": "",
        "endpointuri": null,
        "scriptid": null,
        "categoryid": null,
        "automationenginename": "",
        "createdby": "",
        "ModifiedBy": "",
        "CreateDate": "",
        "ModifiedDate": "",
        "ValidityStart": new Date(),
        "ValidityEnd": new Date(),
        "actionparams": []
      }
    }
  }
  saveActionDetails(validForm: NgForm) {

    if (validForm.valid) {
      if (this.isUpdateType) {
        this.updateActionDetails();
      } else {
        this.addActionDetails();
      }
    } else {
      this.toastr.errorToastr("Please fill all required paramter values!", 'Error!')

    }

  }
  updateActionDetails() {



    // this.actionJson.actiondetail = [];
    var actnJson = {
      "tenantid": 1,
      actiondetails: []
    }
    actnJson.actiondetails = [];
    actnJson.actiondetails.push(this.actiondetail);
    // console.log(JSON.stringify(this.actiondetail))
    console.log(JSON.stringify(actnJson));
    this.isLoader = true;
    this.confService.updateActionDetails(JSON.stringify(actnJson)).subscribe(res => {
      this.isLoader = false;
      this.reset();
      this.toastr.successToastr('Action Configuration details Updated successfully', 'Success!');
    }, error => {
      console.log(error);
      this.toastr.errorToastr("Error occured !", 'Error!');
      this.isLoader = false;
    }
    )

  }

  addActionDetails() {
    var actnJson = {
      "tenantid": 1,
      actiondetails: []
    }
    actnJson.actiondetails = [];
    actnJson.actiondetails.push(this.actiondetail);
    console.log(JSON.stringify(actnJson));
    this.isLoader = true;
    this.confService.addActionDetails(JSON.stringify(actnJson)).subscribe(res => {
      this.isLoader = false;
      this.reset();
      this.toastr.successToastr('Action Configuration details Added successfully', 'Success!');
    }, error => {
      console.log(error);
      this.toastr.errorToastr("Error occured !", 'Error!');
      this.isLoader = false;
    }
    )

  }

  onDateSelect(date: any, obs: Actiondetail, type: string) {
    if (type == "start") {
      obs.ValidityStart = new Date(date);
    } else {
      obs.ValidityEnd = new Date(date);
    }

  }
  editParam(param: Actionparam) {
    this.actionParamDef = param;
    this.actionParam = JSON.parse(JSON.stringify(param));
  }
  updateParam() {
    // this.actionParamDef=this.actionParam;

    try {
      this.actiondetail.actionparams.forEach(param => {

        if (param.name == this.actionParam.name) {
          // param = this.actionParam
          param.fieldtomap = this.actionParam.fieldtomap;
          param.defaultvalue = this.actionParam.defaultvalue;
        }
      })
      this.ref.detectChanges();
      this.actionParam = {};
    } catch (e) {

    }
  }
  deleteParam(param: Actionparam) {
    param.isdeleted = true;

  }
  onBlurConfig(lable: any) {
    if (lable == "") {

    }

    switch (lable) {
      case "AutomationEngineNameDd": {
        this.configDetails.AutomationEngineNameTxt = undefined;
        break;
      }
      case "AutomationEngineNameTxt": {
        this.configDetails.AutomationEngineNameDd = undefined;
        break;
      }
      case "CategoryServiceNameDd": {
        this.configDetails.CategoryServiceNameTxt = undefined;
        break;
      }
      case "CategoryServiceNameTxt": {
        this.configDetails.CategoryServiceNameDd = undefined;
        break;
      }

      case "ScriptDetailServiceNameDd": {
        this.configDetails.ScriptDetailServiceNameTxt = undefined;
        break;
      }
      case "ScriptDetailServiceNameTxt": {
        this.configDetails.ScriptDetailServiceNameDd = undefined;
        break;
      }
    }
  }
  closeConfigure() {
    this.resetConfig();
    $("#preconfigmodel").modal("hide");
  }
  ConfigureActions() {
    // debugger
    var configobj = {

      "TenantId": "",
      "AutomationEngineName": "",
      "SEEBaseUrl": "-04",
      "CategoryServiceName": "",
      "ScriptDetailServiceName": ""


    }
    // this.confService.configureActions()
    var valid = true;
    configobj.TenantId = this.confService.tenantId;
    if (this.configDetails.AutomationEngineNameDd == undefined && this.configDetails.AutomationEngineNameTxt == undefined) {
      valid = false;
    } else {
      configobj.AutomationEngineName = (this.configDetails.AutomationEngineNameDd != undefined) ? this.configDetails.AutomationEngineNameDd : this.configDetails.AutomationEngineNameTxt
    }
    if (this.configDetails.BaseUrl == undefined) {
      valid = false;
    } else {
      configobj.SEEBaseUrl = this.configDetails.BaseUrl;
    }
    if (this.configDetails.CategoryServiceNameDd == undefined && this.configDetails.CategoryServiceNameTxt == undefined) {
      valid = false;
    } else {
      configobj.CategoryServiceName = (this.configDetails.CategoryServiceNameDd != undefined) ? this.configDetails.CategoryServiceNameDd : this.configDetails.CategoryServiceNameTxt;
    }
    if (this.configDetails.ScriptDetailServiceNameDd == undefined && this.configDetails.CategoryServiceNameTxt == undefined) {
      valid = false;
    } else {
      configobj.ScriptDetailServiceName = (this.configDetails.ScriptDetailServiceNameDd != undefined) ? this.configDetails.ScriptDetailServiceNameDd : this.configDetails.ScriptDetailServiceNameTxt;
    }

    if (valid) {
      this.isLoader = true;
      this.confService.configureActions(JSON.stringify(configobj)).subscribe(res => {
        this.isLoader = false;
        $("#preconfigmodel").modal("hide");
        this.resetConfig();
        this.toastr.successToastr('Configuration updated successfully', 'Success!');

        this.isUpdateType = true;
        this.configDetails = {

        }
        this.actionParam = {


        };
        this.actions = [];
        this.actionTypes = [];
        this.isLoader = false;
        // debuggee
        this.getActionDetails(true);
        this.getationTypes();
        this.actiondetail = {
          "actionid": 0,
          "actionname": "",
          "actiontypeid": null,
          "actiontype": "",
          "endpointuri": null,
          "scriptid": null,
          "categoryid": null,
          "automationenginename": "",
          "createdby": "",
          "ModifiedBy": "",
          "CreateDate": "",
          "ModifiedDate": "",
          "ValidityStart": new Date(),
          "ValidityEnd": new Date(),
          "actionparams": []
        }
      }, error => {
        console.log(error);
        this.toastr.errorToastr("Error occured !", 'Error!');
        this.isLoader = false;
      }
      )

    }
    console.log(this.configDetails)
    console.log(configobj);

  }
  resetConfig() {
    this.configDetails = {
      "TenantId": "",
      "AutomationEngineNameTxt": "",
      "AutomationEngineNameDd": "",
      "BaseUrl": "",
      "CategoryServiceNameTxt": "",
      "CategoryServiceNameDd": "",
      "ScriptDetailServiceNameTxt": "",
      "ScriptDetailServiceNameDd": ""
    }
  }


  options = {};

  jsonData = [

    {
      MetricName: "SoftwareDetails",
      details: [
        {
          oldversion: "1.0",
          oldinstalleddate: "02-02-2020",
          oldpublisher: "xxx",
          newversion: "2.0",
          newinstalleddate: "04-04-2020",
          newpublisher: "abcd",
          status: "added"
        },
        {
          oldversion: "1.1",
          oldinstalleddate: "02-02-2020",
          oldpublisher: "yyy",
          newversion: "2.1",
          newinstalleddate: "04-04-2020",
          newpublisher: "ccd",
          status: "modified"
        }

      ],
    },
 
    {
      MetricName: "OSDetails",
      details: [
    
  
      {
        osname: "572*968",
        oldsystemtype: "xxx",
        oldversion: "2.0",
        newsystemtype: "",
        newversion: "",
        newbuildnuber: "",
        status: "modified"
      }

    ]
  },
  {
    MetricName: "OSDetails",
   
      details: [
      {
        oldversion: "1.1",
        newversion: "",
        status: "modified"
      }

    ]
  }
  ]

download() {

  var softwareHeader: any[];
  softwareHeader = []
  for (var name in this.jsonData[0].details[0]) {
    console.log(name);
    softwareHeader.push(name);
  }


  this.confService.downloadFile(this.jsonData[0].details, 'software', softwareHeader);
  var osHeader: any[];
  osHeader = []
  for (var name in this.jsonData[1].details[0]) {
    console.log(name);
    osHeader.push(name);
  }
  this.confService.downloadFile(this.jsonData[1].details, 'os', osHeader);
  var resolutionHeader: any[];
  resolutionHeader = []
  for (var name in this.jsonData[2].details[0]) {
    console.log(name);
    resolutionHeader.push(name);
  }
  this.confService.downloadFile(this.jsonData[0].details, 'Screenresolution', resolutionHeader);
}
}

interface ConfigDetails {

  TenantId?: string,
  AutomationEngineNameTxt?: string,
  AutomationEngineNameDd?: string,
  BaseUrl?: string,
  CategoryServiceNameTxt?: string,
  CategoryServiceNameDd?: string,
  ScriptDetailServiceNameTxt?: string
  ScriptDetailServiceNameDd?: string


}
interface BotDetail {
  Name?: string;
  botid?: string;
}