import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { ConfigurationService } from '../services/configuration.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Resource, Resourcedetail, Parentdetail, Observablesandremediation, ChildDetails, Resourceattribute, Observablesandremediationplan } from '../model/resource.model';
import { AnomolyConfig } from '../model/anomaly-config.model';
import { LocalDataSource } from '../ng2-smart-table';
declare var $: any;
import { IDropdownSettings } from 'ng-multiselect-dropdown';
@Component({
  selector: 'app-anomaly-config',
  templateUrl: './anomaly-config.component.html',
  styleUrls: ['./anomaly-config.component.css']
})
export class AnomalyConfigComponent implements OnInit {

  constructor(public toastr: ToastrManager, private confService: ConfigurationService) { }
  isUpdateType: boolean;
  dropdownList = [];
  selectedItems = [];
  dropdownSettings: IDropdownSettings = {};
  isLoader: boolean = false;
  obs: any;
  obslist: any;
  obsarray: any;
  restypelist: any;
  restype: any;
  restypearr: any;
  PlatformInstanceId: string;
  TenantId: string;
  anomolyConfig: AnomolyConfig;
  configuration: AnomolyConfig;
  addConfig: AnomolyConfig;
  source: LocalDataSource;
  anomalyAddJson: AnomolyConfig[];
  platformJson: PlatformDetails[];
  anomalyConfig: AnomolyConfig[] = [];
  ngOnInit() {
    // this.source= new LocalDataSource(this.anomalyConfig);
    this.PlatformInstanceId = "0";
    this.TenantId = "1";
    this.configuration = {}
    this.getDetails();
    this.addConfig = {
      LowerThreshold: "",
      UpperThreshold: "",
      Operator: "",
      ResourceId: "",
      ResourceName: ""

    }
    this.dropdownList = [

    ];
    this.selectedItems = [

    ];
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      selectAllText: 'Select All',
      unSelectAllText: 'UnSelect All',
      itemsShowLimit: 0,
      allowSearchFilter: true
    };
    this.getPlatformDetails();
  }

  addConfiguration() {
    // debugger
    console.log(this.saveConfig)

    this.saveConfig.forEach(config => {
      // this.anomalyConfig.push(config);
      config.UpperThreshold = this.addConfig.UpperThreshold;
      config.LowerThreshold = this.addConfig.LowerThreshold;
      config.Operator = this.addConfig.Operator;
      // config.OperatorId="0";
      if (config.Operator == "=") {
        config.OperatorId = "1";
      } else if (config.Operator == ">") {
        config.OperatorId = "2";
      } else if (config.Operator == "&lt;") {
        config.OperatorId = "3";
      } else if (config.Operator == "Between") {
        config.OperatorId = "4";
      } else if (config.Operator == "!=") {
        config.OperatorId = "5";
      }
      // config.ObservableName
    })
    // console.log(JSON.stringify(this.anomalyConfig));

    this.addAnomalyRulesConfiguration();
    // this.source.load(this.anomalyConfig)
    // this.anomalyConfig.push(this.tempConfig);
  }
  saveConfig: AnomolyConfig[] = []

  onItemSelect(item: any) {
    // this.saveConfig = [];
    console.log(item);
    var tempConfig: AnomolyConfig = {}

    tempConfig.ResourceId = item.id,
      tempConfig.ResourceName = item.name,
      tempConfig.ResourceTypeId = this.addConfig.ResourceTypeId;
    tempConfig.ResourceTypeName = this.addConfig.ResourceTypeName;
    tempConfig.ObservableId = this.addConfig.ObservableId;
    tempConfig.ObservableName = this.addConfig.ObservableName;

    tempConfig.Operator = this.addConfig.Operator;
    if (tempConfig.Operator == "=") {
      tempConfig.OperatorId = "1";
    } else if (tempConfig.Operator == ">") {
      tempConfig.OperatorId = "2";
    } else if (tempConfig.Operator == "&lt;") {
      tempConfig.OperatorId = "3";
    } else if (tempConfig.Operator == "Between") {
      tempConfig.OperatorId = "4";
    } else if (tempConfig.Operator == "!=") {
      tempConfig.OperatorId = "5";
    }
    tempConfig.LowerThreshold = this.addConfig.LowerThreshold,
      tempConfig.UpperThreshold = this.addConfig.UpperThreshold,
      tempConfig.logdetails = {
        "CreatedBy": "admin@123",
        "ModifiedBy": "admin@123",
        "CreateDate": "2019-08-01 00:00:00.000000",
        "ModifiedDate": "2019-08-01 00:00:00.000000",
        "ValidityStart": "2019-08-01 00:00:00.000000",
        "ValidityEnd": "2099-08-01 00:00:00.000000"
      };
    this.saveConfig.push(tempConfig);
  }
  onSelectAll(items: any) {
    console.log(items);
    items.forEach(item => {
      var tempConfig: AnomolyConfig = {}
      tempConfig.ResourceId = item.id,
        tempConfig.ResourceName = item.name,
        tempConfig.ResourceTypeId = this.addConfig.ResourceTypeId;
      tempConfig.ResourceTypeName = this.addConfig.ResourceTypeName;
      tempConfig.ObservableId = this.addConfig.ObservableId;
      tempConfig.ObservableName = this.addConfig.ObservableName
      // tempConfig.OperatorId = "1";
      tempConfig.Operator = this.addConfig.Operator;
      if (tempConfig.Operator == "=") {
        tempConfig.OperatorId = "1";
      } else if (tempConfig.Operator == ">") {
        tempConfig.OperatorId = "2";
      } else if (tempConfig.Operator == "&lt;") {
        tempConfig.OperatorId = "3";
      } else if (tempConfig.Operator == "Between") {
        tempConfig.OperatorId = "4";
      } else if (tempConfig.Operator == "!=") {
        tempConfig.OperatorId = "5";
      }
      tempConfig.LowerThreshold = this.addConfig.LowerThreshold;
      tempConfig.UpperThreshold = this.addConfig.UpperThreshold;


      tempConfig.logdetails = {
        "CreatedBy": "admin@123",
        "ModifiedBy": "admin@123",
        "CreateDate": "2019-08-01 00:00:00.000000",
        "ModifiedDate": "2019-08-01 00:00:00.000000",
        "ValidityStart": "2019-08-01 00:00:00.000000",
        "ValidityEnd": "2099-08-01 00:00:00.000000"
      };
      this.saveConfig.push(tempConfig);
    })
  }
  selectionChanged(event) {
    console.log(event);
  }
  settings = {
    columns: {
      ResourceName: {
        title: 'ResourceName',
        type: 'html',
        width: '12%'
      },
      ResourceTypeName: {
        title: 'ResourceTypeName',
        width: '13%'
      },
      ObservableName: {
        title: 'Metric Name',
        width: '15%'
      },


      Operator: {
        title: 'Operator',
        width: '9%'
      },


      LowerThreshold: {
        title: 'LowerThreshold',

        width: '13%'

      },
      UpperThreshold: {
        title: 'UpperThreshold',

        width: '13%'

      },


    },
    mode: "internal",
    // pager: { perPageSelect: [10, 20, 30] }
    actions: {
      edit: false, //as an example
      custom: [{ name: 'routeToAPage', title: `<img style="cursor:pointer" src="/superbotapp/assets/images/edit.png">` }]
    }
  };

  onChangeObs(Obs: any) {
    try {
     
      this.addConfig.ResourceTypeId = "";
      this.anomalyConfig = [];
      this.source.load(this.anomalyConfig);
      // $("#resourcetype").val("0");
      // (<HTMLSelectElement>document.getElementById('remediationPlanName')).selectedIndex=1;
      this.clearAll();
      
      // $("#resourcetype").val("0").change();
    } catch (e) {

    }
    this.getResourceType();
    if (Obs != "") {
      Obs = JSON.parse(Obs);
      this.addConfig.ObservableId = Obs.id;
      this.addConfig.ObservableName = Obs.observablename;
    }

  }
  onEditRowSelect(event) {
    // debugger
    console.log(event);
    try {
      this.configuration = event.data;
    } catch (e) {

    }


    $("#resouceModel").modal("show");
  }

  onSelectionChanged(planId: string) {
    // debugger;
    console.log(planId);
    // TO_DO: un Comment
    // document.getElementById("openStepModalBtn").click();

    // this.getPlanHistoryStepDetails(planId);
  };
  platfromArray: any[];
  getPlatformDetails() {
    this.platfromArray=[];
    this.confService.getAllPlatforms("1").subscribe(res => {
      this.platformJson = JSON.parse(JSON.stringify(res));
      this.platformJson.forEach(platform => {
        this.platfromArray.push({ "id": platform.PlatformId, "name": platform.PlatformInstanceName })
      })

    },error => {
      console.log(error);
      this.isLoader = false;
      this.toastr.errorToastr("Server Error !", 'Error!');
    }
    );


  }

  getDetails() {

    this.obsarray = [];
    this.confService.getObservableModelList("1").subscribe(res => {
      this.obs = res;

      this.obs.obsList.forEach(ob => {
        this.obsarray.push({ "id": ob.observableid, "observablename": ob.observablename });
      })
    });
    this.getResourceType();


  }
  getResourceType() {
    this.restypearr = [];
    this.confService.getResourceTypes("1", this.PlatformInstanceId).subscribe(res => {
      this.restype = res;
      $("#resourcetype").val("0");
      // $('.resourcetype option[value=0]').attr('selected','selected');
      this.restype.resTypeList.forEach(res => {
        if (res.resourcetypename.trim() != 'Platform') {
          this.restypearr.push({ "id": res.resourcetypeid, "resourcetypename": res.resourcetypename })
        }

      })
    });
  }
  resourceList: any[];
  clearAll() {
    
    this.selectedItems = [

    ];
    this.saveConfig = [];
    this.addConfig.UpperThreshold = "";
    this.addConfig.LowerThreshold = "";
  }
  onChangePlatform(id:string){
 this.PlatformInstanceId=id;
 this.getResourceType();
 
  }
  onChangeType(res: any) {
    this.selectedItems = [

    ];
    this.saveConfig = [];
    this.addConfig.UpperThreshold = "";
    this.addConfig.LowerThreshold = "";
    if (res != "") {
      res = JSON.parse(res);
      this.addConfig.ResourceTypeId = res.id;
      this.addConfig.ResourceTypeName = res.resourcetypename;
    }


    // to-do un comment this
    
    this.isLoader = true;
    if( this.PlatformInstanceId != "0"  && res.resourcetypename != undefined){
    this.confService.getResourceModelConfiguration(this.PlatformInstanceId, this.TenantId, res.resourcetypename).subscribe((res => {
      // debugger
      this.isLoader = false;
      var resourceJson: Resource[] = <Resource[]>res;


      try {
        // debugger
        this.dropdownList = [];
        if (resourceJson[0].resourcedetails.length > 0) {
          resourceJson[0].resourcedetails.forEach(res => {
            this.dropdownList.push({ "id": res.resourceid, "name": res.resourcename })
          })
          // resourceList
          // debugger

        }
        this.getAnomalyRulesConfiguration();
      } catch (e) {
        console.log(e);
      }


    }), error => {
      console.log(error);
      this.isLoader = false;
    }
    )
  }
  }
  deleteAnomalyRulesConfiguration() {
    this.isLoader = true;
    var json = {
      "PlatformId": 1,
      "TenantId": 1,
      AnomalyDetectionRules: []
    }
    json.AnomalyDetectionRules = [];
    json.AnomalyDetectionRules.push(this.configuration);
    console.log(JSON.stringify(json));
    this.confService.deleteAnomalyRulesConfiguration(JSON.stringify(json)).subscribe((res => {

      this.isLoader = false;
      this.saveConfig = []
      this.onChangeType(JSON.stringify({ "id": this.addConfig.ResourceTypeId, "resourcetypename": this.addConfig.ResourceTypeName }));
      $("#resouceModel").modal("hide");
      this.toastr.successToastr('Configuration details deleted successfully', 'Success!');

    }), error => {
      console.log(error);
      this.isLoader = false;
      this.toastr.errorToastr("Error occured !", 'Error!');
    }
    )
  }
  addAnomalyRulesConfiguration() {
    // debugger

    if (this.saveConfig == undefined || this.saveConfig.length == 0) {
      this.toastr.errorToastr("Please Select Resource", 'Error!')
    } else {


      this.isLoader = true;
      var json = {
        "PlatformId": 1,
        "TenantId": 1,
        AnomalyDetectionRules: []
      }
      json.AnomalyDetectionRules = this.saveConfig;
      // json.AnomalyDetectionRules.push(this.configuration);

      console.log(JSON.stringify(json));
      this.confService.addAnomalyRulesConfiguration(JSON.stringify(json)).subscribe((res => {
        // debugger
        this.isLoader = false;
        this.onChangeType(JSON.stringify({ "id": this.addConfig.ResourceTypeId, "resourcetypename": this.addConfig.ResourceTypeName }));
        //  $("#resouceModel").modal("hide");
        this.clearAll();
        this.saveConfig = []
        this.toastr.successToastr('Configuration details added successfully', 'Success!');
      }), error => {
        console.log(error);
        this.isLoader = false;
        this.toastr.errorToastr("Error occured !", 'Error!');
      }
      )
    }
  }
  updateAnomalyRulesConfiguration() {
    // debugger
    this.isLoader = true;
    var json = {
      "PlatformId": 1,
      "TenantId": 1,
      AnomalyDetectionRules: []
    }
    json.AnomalyDetectionRules = [];
    json.AnomalyDetectionRules.push(this.configuration);
    console.log(JSON.stringify(json));
    this.confService.updateAnomalyRulesConfiguration(JSON.stringify(json)).subscribe((res => {
      // debugger
      this.isLoader = false;
      this.onChangeType(JSON.stringify({ "id": this.addConfig.ResourceTypeId, "resourcetypename": this.addConfig.ResourceTypeName }));
      $("#resouceModel").modal("hide");
      this.saveConfig = []
      this.toastr.successToastr('Configuration details updated successfully', 'Success!');
    }), error => {
      console.log(error);
      this.isLoader = false;
      this.toastr.errorToastr("Error occured !", 'Error!');
    }
    )

  }
  getAnomalyRulesConfiguration() {
    
    
    if( this.PlatformInstanceId != "0" &&  this.addConfig.ObservableId != undefined && this.addConfig.ResourceTypeId != "0"){

      this.isLoader = true;
    this.confService.getAnomalyRulesConfiguration(this.TenantId, this.addConfig.ObservableId, this.addConfig.ResourceTypeId, this.PlatformInstanceId).subscribe((res => {
      // debugger
      this.isLoader = false;
      try {
        this.anomalyConfig = <AnomolyConfig[]>JSON.parse(JSON.stringify(res)).AnomalyDetectionRules;

        this.source = new LocalDataSource(this.anomalyConfig);
        this.source.load(this.anomalyConfig);

      } catch (e) {
        console.log(e);
      }

    }), error => {
      console.log(error);
      this.isLoader = false;
    }
    )
  }
  }

}
export interface PlatformDetails {
  PlatformId: string;
  PlatformTypeName: string;
  PlatformInstanceName: string;
  ResourceTypeId: string;
  ResourceTypeName: string;
}