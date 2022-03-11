import { Component, OnInit,Output ,EventEmitter} from '@angular/core';
import { RemediationDetails, ActionDetail } from '../model/remediation-config.model';
import { ConfigurationService } from '../services/configuration.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { ActionConfig, Actiondetail, Actionparam } from '../model/action-config.model';
import { debug } from 'util';
import { Action } from 'rxjs/internal/scheduler/Action';
@Component({
  selector: 'app-remediation-config',
  templateUrl: './remediation-config.component.html',
  styleUrls: ['./remediation-config.component.css']
})
export class RemediationConfigComponent implements OnInit {

  constructor(public toastr: ToastrManager, private confService: ConfigurationService) { }
  isUpdateType: boolean;
  isLoader: boolean = false;
  remediationDetails: RemediationDetails;
  actionDetails: ActionDetail;
  remediationPlans: any[];
  plans: any[];
  actionDetail: ActionDetail;
  actionConfig: ActionConfig;
  actions: any[];
  @Output() messageEvent = new EventEmitter<any>();
  ngOnInit() {
    this.isUpdateType = true;
    this.isLoader = false;
    this.plans = [];
    this.getRemediationPlanDetails();
    this.remediationDetails = {
      remediationId: 0,
      IsUserDefined: false,
      remediationPlanName: "",
      RemediationPlanDescription: "",

    }
    this.actionDetail = {
      ActionId: 0,
      ActionName: "",
      ActionSequence: "",
      ActionStageId: "",
      isDeleted: false,
      RemediationPlanActionId: 0
    }
    this.getActionDetails();
  }
  getActionDetails() {

    this.confService.getActionConfigDetails("1").subscribe(res => {
      this.actionConfig = <ActionConfig>res;
      this.actions = [];
      this.actionConfig.actiondetails.forEach(action => {

        this.actions.push({ "actionid": action.actionid, "actionname": action.actionname })
      })


    })
  }

  getRemediationPlanDetails() {
    this.plans = [];
    this.confService.getRemediationConfigDetails("1").subscribe(res => {
      this.remediationPlans = JSON.parse(JSON.stringify(res)).RemediationPlans;
      this.remediationPlans.forEach(plan => {
        this.plans.push({ "remediationId": plan.remediationId, "remediationPlanName": plan.remediationPlanName });
      })
    })
  }

  onDelete() {

    if (this.remediationDetails.remediationId == 0) {
      this.toastr.errorToastr("Please select Remediation Plan !", 'Error!');
    } else {
      this.isLoader = true;
      var remJson = {
        "tenantid": 1,
        RemediationPlans: []
      }
      remJson.RemediationPlans = [];
      remJson.RemediationPlans.push(this.remediationDetails);
      console.log(JSON.stringify(remJson));
      this.confService.deleteRemediationConfigDetails(JSON.stringify(remJson)).subscribe(res => {
        this.isLoader = false;
        this.reset();
        this.toastr.successToastr('Remediation Plan details deleted successfully', 'Success!');
      }, error => {
        console.log(error);
        this.toastr.errorToastr("Error occured !", 'Error!');
        this.isLoader = false;
      })
    }

  }
  onChangeAction(action: any) {

    try {

      if (action != "" && action != undefined) {
        action = JSON.parse(action);
        this.actionDetail.ActionId = action.actionid;
        this.actionDetail.ActionName = action.actionname;
      } else {
        this.actionDetail.ActionId = 0;
        this.actionDetail.ActionName = "";
      }

    } catch (e) {

    }
  }
  onChangePlan(id: any) {

    if (id == "") {
      this.remediationDetails = {
        remediationId: 0,
        IsUserDefined: false,
        remediationPlanName: "",
        RemediationPlanDescription: "",

      }
    } else {
      this.remediationPlans.forEach(plan => {
        if (plan.remediationId == id) {
          this.remediationDetails = plan;
          try {
            this.remediationDetails.ActionDetails.forEach(action => {
              action.isDeleted = false;
            })
          } catch (e) {

          }
        }
      })

    }

  }

  reset() {
    this.remediationDetails = {
      remediationId: 0,
      IsUserDefined: false,
      remediationPlanName: "",
      RemediationPlanDescription: "",
    }
    this.isUpdateType = true;
    this.getRemediationPlanDetails();
    this.getActionDetails();
  }

  togglePageType(flag: boolean) {
    this.getActionDetails();
    this.isUpdateType = flag;

    //for update screen
    if (flag) {

    } else {
      this.remediationDetails = {
        remediationId: 0,
        IsUserDefined: false,
        remediationPlanName: "",
        RemediationPlanDescription: "",

      }
    }
  }

  addAction() {
    if (this.actionDetail.ActionName != undefined && this.actionDetail.ActionName.length > 0) {
      // console.log(this.actionParam);
      if (this.remediationDetails.ActionDetails == undefined)
        this.remediationDetails.ActionDetails = [];

      this.actionDetail.isDeleted = false;

      var isduplicate = false;
      var isaciondup = false;
      var actionjsonid = null;
      this.remediationDetails.ActionDetails.forEach(action => {

        if (action.ActionSequence == this.actionDetail.ActionSequence && (action.ActionId != this.actionDetail.ActionId)) {
          isduplicate = true;
        }
        if (action.ActionId == this.actionDetail.ActionId) {
          isaciondup = true;

          if (action.isDeleted) {
            // action.isDeleted = false;
            actionjsonid = action.ActionId;
            // action.ActionStageId = this.actionDetail.ActionStageId;
            // action.ActionSequence = this.actionDetail.ActionSequence;
            // isaciondup = false;
          }
        }
      })

      
      if (isduplicate || isaciondup) {

        if (isduplicate == false &&     actionjsonid != null && actionjsonid == this.actionDetail.ActionId) {
          this.remediationDetails.ActionDetails.forEach(action => {
            if (action.ActionId == actionjsonid) {
              if (action.isDeleted) {
              isaciondup = false;
              action.isDeleted = false;
              action.ActionStageId = this.actionDetail.ActionStageId;
              action.ActionSequence = this.actionDetail.ActionSequence;
              // if(isduplicate && (action.ActionSequence == this.actionDetail.ActionSequence)){
              //   isduplicate=false;
              // }

              }
            }
          })
        }
        if (isaciondup) {

          this.toastr.errorToastr('Action already Exists', "Error!")
        } else if(isduplicate) {
          this.toastr.errorToastr('Action Sequence already Exists', "Error!")
        }

      } else {
        this.remediationDetails.ActionDetails.push(this.actionDetail);
        this.actionDetail = {
          ActionId: 0,
          ActionName: "",
          ActionSequence: "",
          ActionStageId: "",
          isDeleted: false,
          RemediationPlanActionId: 0
        }
      }


      // console.log(this.actiondetail);
    }

  }

  deleteParam(action: ActionDetail) {
    action.isDeleted = true;

  }

  saveRemediationConfig() {
    if (this.isUpdateType) {
      this.updateRemediationConfig();

    } else {
      this.addRemediationConfig();
    }
  }

  addRemediationConfig() {
    var remJson = {
      "tenantid": 1,
      RemediationPlans: []
    }
    remJson.RemediationPlans = [];
    remJson.RemediationPlans.push(this.remediationDetails);
    console.log(JSON.stringify(remJson));
    this.confService.addRemediationConfigDetails(JSON.stringify(remJson)).subscribe(res => {
      this.isLoader = false;
      this.reset();
     
      this.toastr.successToastr('Remediation Plan details Added successfully', 'Success!');
    }, error => {
      console.log(error);
      this.toastr.errorToastr("Error occured !", 'Error!');
      this.isLoader = false;
    }
    )
  }

  updateRemediationConfig() {

    if (this.remediationDetails.remediationId == 0) {
      this.toastr.errorToastr("Please select Remediation Plan !", 'Error!');
    } else {
      var remJson = {
        "tenantid": 1,
        RemediationPlans: []
      }
      remJson.RemediationPlans = [];
      remJson.RemediationPlans.push(this.remediationDetails);
      console.log(JSON.stringify(remJson));
      this.confService.updateRemediationConfigDetails(JSON.stringify(remJson)).subscribe(res => {
        this.isLoader = false;
        this.reset();
        this.toastr.successToastr('Remediation Configuration details updated successfully', 'Success!');
      }, error => {
        console.log(error);
        this.toastr.errorToastr("Error occured !", 'Error!');
        this.isLoader = false;
      }
      )
    }


  }
navigateToAction(ActionId:any){
  this.messageEvent.emit(ActionId);
}
}

