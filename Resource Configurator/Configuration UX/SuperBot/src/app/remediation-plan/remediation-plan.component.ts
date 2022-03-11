import { Component, OnInit } from '@angular/core';
import { RemediationPlan } from '../model/remediation-plan.model';
import { ConfigurationService } from '../services/configuration.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { debug } from 'util';
import { LocalDataSource} from '../ng2-smart-table';
@Component({
  selector: 'app-remediation-plan',
  templateUrl: './remediation-plan.component.html',
  styleUrls: ['./remediation-plan.component.css']
})
export class RemediationPlanComponent implements OnInit {

  constructor(public toastr: ToastrManager, private confService: ConfigurationService) { }
  isUpdateType: boolean;
  isLoader: boolean = false;
  remediationPlan: RemediationPlan;
  remediationPlans: RemediationPlan[];

  remPlans: any[];
  remediationPlanId:any;
  restypelist: any;
  restype: any;
  restypearr: any;
  obs: any;
  obslist: any;
  obsarray: any;
  settings = {
    bigBanner: false,
    timePicker: false,
    format: 'dd-MM-yyyy',
    defaultOpen: false,
    closeOnSelect: true
  }
  source:LocalDataSource;

  settings_table = {
   
    columns: {
     
      resourcetypename: {
        title: 'Resource Type',
        width: '13%'
      },
      ObservableName: {
        title: 'Monitoring Plan',
        width: '15%'
      },
      RemediationPlanName: {
        title: 'Remediation Plan',
        type: 'html',
        width: '12%',
        editable:true
      },

      ValidityStart: {
        title: 'Validity Start',
        width: '15%'
      },


      ValidityEnd: {
        title: 'Validity End',

        width: '10%'

      },
      


    },
    mode: "internal",
    // pager: { perPageSelect: [10, 20, 30] }
    actions: {
      edit: false, //as an example
      custom: [{ name: 'routeToAPage', title: `<a href="#top" ><img src="/superbotapp/assets/images/edit.png"></a>` }]
    }
  };
  ngOnInit() {

    this.remediationPlanId=0;
    this.isUpdateType = false;
    this.isLoader = false;
    this.remediationPlan = {
      ValidityEnd: new Date(),
      ValidityStart: new Date(),
      ObservableId: 0,
      ObservableName: "",
      RemediationPlanId: 0,
      RemediationPlanName: "",
      resourcetypeid: 0,
      resourcetypename: ""

    }
  
    this.getRemediatioConfigPlanDetails();
    this.getRemediationPlanDetails();

    
  }
  onEditRowSelect(event) {
    console.log(event);
    try {
     var isExst=false;
    
      // this.configuration = event.data;
      console.log(event.data);
      this.remediationPlans.forEach(plan => {
if (plan.RemediationPlanId == event.data.RemediationPlanId && plan.ObservableId == event.data.ObservableId && plan.resourcetypeid ==   event.data.resourcetypeid ) {
          this.remediationPlan = plan;
          isExst=true;
        }
      })
      if(!isExst){
        this.isUpdateType = false;
  
      }else{
        this.isUpdateType = true;
      }
    } catch (e) {

    }


    // $("#resouceModel").modal("show");
  }
  onChangeRes(id: any) {
    this.remediationPlan.resourcetypeid = id;

    if( this.remediationPlan.ObservableId != 0){
      var isExst=false;
      this.remediationPlans.forEach(plan => {
        if (plan.RemediationPlanId == this.remediationPlanId && plan.ObservableId == this.remediationPlan.ObservableId && plan.resourcetypeid ==   id ) {
          this.remediationPlan = plan;
          isExst=true;
        }
      })
      if(!isExst){
        this.isUpdateType = false;
  
      }else{
        this.isUpdateType = true;
      }
    }
  }

  onChangeObs(id: any) {
    this.remediationPlan.ObservableId = id;
    var isExst=false;
    if( this.remediationPlan.resourcetypeid != 0){
      this.remediationPlans.forEach(plan => {
        if (plan.RemediationPlanId == this.remediationPlanId && plan.ObservableId == id && plan.resourcetypeid ==  this.remediationPlan.resourcetypeid ) {
          this.remediationPlan = plan;
          isExst=true;
        }
      })
      if(!isExst){
        this.isUpdateType = false;
  
      }else{
        this.isUpdateType = true;
      }
    }
    
    
  }
  onChangePlan(id: any) {
    this.remediationPlanId=id;
    try {


      var isExst=false;
      this.remediationPlans.forEach(plan => {
        if (plan.RemediationPlanId == id && plan.ObservableId == this.remediationPlan.ObservableId && plan.resourcetypeid ==   this.remediationPlan.resourcetypeid ) {
          this.remediationPlan = plan;
          isExst=true;
        }
      })
      if(!isExst){
        this.isUpdateType = false;
  
      }else{
        this.isUpdateType = true;
      }
    
      if (id == "") {
        this.remediationPlan = {
          ValidityEnd: new Date(),
          ValidityStart: new Date(),
          ObservableId: 0,
          ObservableName: "",
          RemediationPlanId: this.remediationPlanId,
          RemediationPlanName: "",
          resourcetypeid: 0,
          resourcetypename: ""

        }
      } else {
        // id = JSON.parse(id);
        // this.remediationPlan = id;
       
      }

    } catch (e) {

    }
  }
  getRemediatioConfigPlanDetails() {
    this.remPlans = [];
    this.confService.getRemediationConfigDetails("1").subscribe(res => {
       var remediationPlans = JSON.parse(JSON.stringify(res)).RemediationPlans;
         remediationPlans.forEach(plan => {
        this.remPlans.push({ "remediationId": plan.remediationId, "remediationPlanName": plan.remediationPlanName });
      })
    })
  }
  getRemediationPlanDetails() {
    this.remPlans = [];
    this.confService.getRemediationPlanDetails("1", "1").subscribe(res => {
      this.remediationPlans = JSON.parse(JSON.stringify(res)).RemediationPlanObservableAndResourceTypeList;
      this.source= new LocalDataSource(this.remediationPlans);
      // try {
      //   this.remediationPlans.forEach(plan => {
      //     this.remPlans.push({ "RemediationPlanId": plan.RemediationPlanId, "RemediationPlanName": plan.RemediationPlanName });
      //   })
      // } catch (e) {

      // }

      this.obsarray = [];
      this.confService.getObservableModelList("1").subscribe(res => {
        this.obs = res;

        this.obs.obsList.forEach(ob => {
          this.obsarray.push({ "id": ob.observableid, "observablename": ob.observablename });
        })
      });

      this.restypearr = [];
      this.confService.getResourceTypeModelList( "1").subscribe(res => {
        this.restype = res;

        this.restype.resTypeList.forEach(res => {
          if (res.resourcetypename.trim() != 'Platform') {
            this.restypearr.push({ "id": res.resourcetypeid, "resourcetypename": res.resourcetypename })
          }

        })
      });

    })
  }
  reset() {
    this.isUpdateType = true;
    this.remediationPlan = {
      ValidityEnd: new Date(),
      ValidityStart: new Date(),
      ObservableId: 0,
      ObservableName: "",
      RemediationPlanId: 0,
      RemediationPlanName: "",
      resourcetypeid: 0,
      resourcetypename: ""

    }
     this.getRemediatioConfigPlanDetails();
    this.getRemediationPlanDetails();
  }
  togglePageType(flag: boolean) {
   
    this.isUpdateType = flag;

    //for update screen
    if (flag) {

    } else {
      // this.getRemediationPlanDetails();
      this.remediationPlan = {
        ValidityEnd: new Date(),
        ValidityStart: new Date(),
        ObservableId:  this.remediationPlan.ObservableId,
        ObservableName: "",
        RemediationPlanId: this.remediationPlanId,
        RemediationPlanName: "",
        resourcetypeid: this.remediationPlan.resourcetypeid,
        resourcetypename: ""

      }
    }
    this.getRemediatioConfigPlanDetails();
    this.getRemediationPlanDetails();
  }
  saveRemediationPlan() {
    if (this.isUpdateType) {
      this.updateRemediationPlan();
    } else {
      this.addRemediationPlan();
    }
  }

  addRemediationPlan() {
    var remJson = {
      "tenantid": 1,
      RemediationPlanObservableAndResourceTypeList: []
    }
    remJson.RemediationPlanObservableAndResourceTypeList = [];
    remJson.RemediationPlanObservableAndResourceTypeList.push(this.remediationPlan);
    console.log(JSON.stringify(remJson));
    this.confService.addRemediationPlanDetails(JSON.stringify(remJson)).subscribe(res => {
      this.isLoader = false;
      this.togglePageType(false);
      this.toastr.successToastr('Remediation Plan Configuration details Added successfully', 'Success!');
    }, error => {
      console.log(error);
      this.toastr.errorToastr("Error occured !", 'Error!');
      this.isLoader = false;
    }
    )
  }
  onDelete() {
    
    if(this.remediationPlan.RemediationPlanId == 0){
      this.toastr.errorToastr("Please select Remediation Plan !", 'Error!');
    }else{
      var remJson = {
        "tenantid": 1,
        RemediationPlanObservableAndResourceTypeList: []
      }
      remJson.RemediationPlanObservableAndResourceTypeList = [];
      remJson.RemediationPlanObservableAndResourceTypeList.push(this.remediationPlan);
      console.log(JSON.stringify(remJson));
      this.confService.deleteRemediationPlanDetails(JSON.stringify(remJson)).subscribe(res => {
        this.isLoader = false;
        this.reset();
        this.toastr.successToastr('Remediation Plan Configuration details Deleted successfully', 'Success!');
      }, error => {
        console.log(error);
        this.toastr.errorToastr("Error occured !", 'Error!');
        this.isLoader = false;
      }
      )
    }

    
  }

  updateRemediationPlan() {
    if(this.remediationPlan.RemediationPlanId == 0){
      this.toastr.errorToastr("Please select Remediation Plan !", 'Error!');
    }else{
      var remJson = {
        "tenantid": 1,
        RemediationPlanObservableAndResourceTypeList: []
      }
      remJson.RemediationPlanObservableAndResourceTypeList = [];
      remJson.RemediationPlanObservableAndResourceTypeList.push(this.remediationPlan);
      console.log(JSON.stringify(remJson));
      this.confService.updateRemediationPlanDetails(JSON.stringify(remJson)).subscribe(res => {
        this.isLoader = false;
        this.reset();
        this.toastr.successToastr('Remediation Plan Configuration details Updated successfully', 'Success!');
      }, error => {
        console.log(error);
        this.toastr.errorToastr("Error occured !", 'Error!');
        this.isLoader = false;
      }
      )
    }
   
  }

  onDateSelect(date: any, rem: RemediationPlan, type: string) {
    if (type == "start") {
      rem.ValidityStart = new Date(date);
    } else {
      rem.ValidityEnd = new Date(date);
    }


  }
}
