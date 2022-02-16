import { Component, OnInit, OnChanges, ChangeDetectorRef, DoCheck, AfterViewInit, AfterViewChecked, ViewChild, ElementRef, Input } from '@angular/core';
import { FormGroup, FormControl, NgForm } from '@angular/forms';

import { Configuration, ResourceTypes } from '../model/configuration.model';
import { Resource, Resourcedetail, Parentdetail, Observablesandremediation, ChildDetails, Resourceattribute, Observablesandremediationplan } from '../model/resource.model';
import { ConfigurationService } from '../services/configuration.service';
import { ToastrManager } from 'ng6-toastr-notifications';


declare var $: any;
@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.css']
})
export class ConfigurationComponent implements OnInit {
  @ViewChild('node', { static: false }) node: ElementRef;
  @ViewChild('param', { static: false }) param: ElementRef;
  @ViewChild('nodeSelect', { static: false }) nodeSelect: ElementRef;
  @ViewChild('includeDefault', { static: false }) includeDefault: ElementRef;
  @ViewChild('includeDefaultDiv', { static: false }) includeDefaultDiv: ElementRef;
  @ViewChild('paramdesc', { static: false }) paramDesc: ElementRef;
  @ViewChild('paramDisplay', { static: false }) paramDisplay: ElementRef;
  
  @Input() PlatformIdChild;
  @Input() ResourceTypeNameChild;
  fromDate: Date = new Date();
  toDate: Date = new Date();
  fromDateChild: Date = new Date();
  toDateChild: Date = new Date();
  settings = {
    bigBanner: false,
    timePicker: false,
    format: 'dd-MM-yyyy',
    defaultOpen: false,
    closeOnSelect: true
  }


  constructor(public toastr: ToastrManager, private confService: ConfigurationService, private ref: ChangeDetectorRef) {

  }


  confJson: Configuration[] = [{}];

  resourceJosn: Resource[];
  resourceJsontmp: Resource[];
  saveResourceJson: Resource;

  // defautlmetadata: Attributes = null;
  hasParent: boolean = false;
  ResourceType: ResourceTypes;


  resourceCombo: any[] = [];
  parentCombo: any[] = [];
  resourceComboChild: any[] = [];
  Resourcedetail: Resourcedetail;
  parentDetails: Parentdetail[] = [];
  childDetiils: ChildDetails[];
  childDetailsTmp: ChildDetails[];
  childDetailJson: ChildDetails;
  // childAttributes:Resourcedetail;
  observablesRemediationPlan: Observablesandremediationplan[];
  SaveobservablesRemediationPlan: Observablesandremediationplan;
  mainObsRemediation: Observablesandremediationplan;
  parentObsRem: Observablesandremediation[];
  childObsRem: Observablesandremediation[];
  childObsRemediation: Observablesandremediationplan;
  activeObsRemediation: Observablesandremediationplan;
  activeObsRemediationTmp: Observablesandremediationplan;
  tempObsRemediation: Observablesandremediationplan;
  temobservablesandremediations: Observablesandremediation[];
  isParentNode: boolean;
  isResouceChanged: boolean;
  tempObs: Observablesandremediation[];
  resourceIdCombo: string;
  resourceIdComboChild: string;
  showTable: boolean;
  childResId: string;
  portfolios: any;


  ngOnInit() {
    console.log("hai strat")
    console.log(this.PlatformIdChild);
    console.log(this.ResourceTypeNameChild);
    console.log("end");
    this.childObsRem = [];
    this.childDetailJson = {};
    if (this.Resourcedetail == undefined) {
      this.Resourcedetail = {}
      this.Resourcedetail.startdate = new Date().toString();
      this.Resourcedetail.enddate = new Date().toString();
    }
    this.childResId = "";

    this.isResouceChanged = false;
    this.showTable = true;
    this.isLoader = false;
    this.nodeParameters = [];
    this.isParentNode = true;
    this.resourceCombo = [];
    this.resourceComboChild = []
    this.resourceTypeId = "0";;
    this.parentCombo = [];

    this.PlatformInstanceId = "1";
    this.TenantId = "1";

    let d: Date = new Date();
    let mnth: number;
    let year: number;

    if (d.getMonth() === 0) {
      mnth = 12;
      year = d.getFullYear() - 1;
    } else {
      mnth = d.getMonth();
      year = d.getFullYear();
    }


    // this.fromDate = year + "-" + (mnth) + "-" + d.getDate() + " " + d.getHours() + ":" + d.getMinutes();
    // this.toDate = d.getFullYear() + "-" + (d.getMonth() ) + "-" + d.getDate() + " " + d.getHours() + ":" + d.getMinutes();

    this.getResourceConfiguration();
    this.getPortfolioDetails();
  }
  getPortfolioDetails() {
    this.confService.getPortfolioDetails("1").subscribe(res => {
      this.portfolios = res;
    })
  }
  onClickResource(index: string, resourceId: string, typename: string) {
    this.isFromSummary=false;
    this.childResId = "";
    this.assignValues(index, resourceId, typename);
  }
  onChangeResourceChild(id: string) {
    // 
    if (this.childResId != "") {
      id = this.childResId;
    }
    try {
      this.resourceIdComboChild = id;
      this.childDetiils.forEach((obj) => {
        if (id === obj.resourceid) {

          this.childDetailJson = <ChildDetails>obj;
          this.childObsRem = this.childDetailJson.observablesandremediations;
        }
      });
    } catch (e) {

    }


  }
  onChangeParentHTML(id: string) {
    this.parentComboId=id;
    var firstItr = true;
    var resouceidtemp;
    this.resourceCombo = [];
    this.resourceJosn.forEach((element) => {

      element.resourcedetails.forEach((obj) => {

        if (obj.parentdetails != undefined && obj.parentdetails[0] != undefined) {

          if (id == obj.parentdetails[0].resourceid) {

            if (firstItr) {
              
              resouceidtemp = obj.resourceid;
              firstItr = false;
            }
            this.resourceCombo.push({ 'key': obj.resourcename, 'value': obj.resourceid });
          }
        }
      });
    });
    this.resourceIdCombo = resouceidtemp;
    $("#resouceComboSelect").val(this.resourceIdCombo);
    this.onChangeResource(resouceidtemp);

  }

  onChangeResourceHTML(id: string) {
    if (this.isResouceChanged) {
      if (window.confirm("Do you want to change the resource? Changes you made may not be saved ?")) {
        this.onChangeResource(id);
      } else {
        $("#resouceComboSelect").val(this.resourceIdCombo);
      }
    } else {
      this.onChangeResource(id);
    }

  }
  onKeyUp() {
    this.isResouceChanged = true;
  }
  onChangeResource(id: string) {
    this.isResouceChanged = false;
    this.childObsRem = [];
    this.resourceIdCombo = id;
    this.resourceComboChild.length = 0;
    this.resourceComboChild = [];
    this.childDetailsTmp = this.childDetiils;
    this.resourceJosn.forEach((element) => {
      this.observablesRemediationPlan = element.observablesandremediationplans;
      element.resourcedetails.forEach((obj) => {
        if (id === obj.resourceid) {
          this.Resourcedetail = obj;
          this.parentDetails = <Parentdetail[]>obj.parentdetails;

          this.childDetiils = obj.childdetails;
          this.parentObsRem = obj.observablesandremediations;
          if (this.childDetiils != undefined)
            this.childObsRem = this.childDetiils[0].observablesandremediations;
        }
      });
    });

    if (this.childDetiils != null) {

      this.childDetiils.forEach(obj => {
        this.resourceComboChild.push({ 'key': obj.resourcename, 'value': obj.resourceid })
      });

    }
    // test -1 
    // try {
    //   this.observablesRemediationPlan.forEach((resource) => {
    //     if (this.childDetiils == undefined) {
    //       this.mainObsRemediation = resource;
    //     } else {
    //       if (resource.resourcetypeid == this.childDetiils[0].resourcetypeid) {
    //         this.childObsRemediation = resource;
    //       } else {
    //         this.mainObsRemediation = resource;
    //       }
    //     }

    //   });
    //   this.assignDefaults();
    // } catch (e) {
    // }



  }




  checkValueChild(Obsid: any, remId: any, Obstype: string, ischecked: boolean) {
    this.isResouceChanged = true;
    try {
      this.Resourcedetail.childdetails.forEach(child => {
        if (child.resourceid == this.resourceIdComboChild) {
          child.observablesandremediations.forEach(obsrem => {
            if (obsrem.ObservableId == Obsid && obsrem.RemediationPlanId == remId) {

              if (Obstype == 'obs' && obsrem.ObservableId == Obsid) {
                obsrem.isObsSelected = ischecked;
                // obs.isRemSelected = ischecked;
                if (!ischecked) {
                  obsrem.isRemSelected = ischecked;
                }

              } else if (obsrem.ObservableId == Obsid && Obstype == 'rem' && obsrem.RemediationPlanId == remId) {
                obsrem.isRemSelected = ischecked;

              }
              //is modified?
              var isFound = false;
              this.resourceJsontmp[0].resourcedetails.forEach(obj => {
                if (obj.resourceid == this.Resourcedetail.resourceid) {

                  obj.childdetails.forEach(child => {

                    if (child.resourceid == this.resourceIdComboChild) {

                      child.observablesandremediations.forEach(element => {

                        if ((element.ObservableId == Obsid) && (element.RemediationPlanId == remId)) {
                          isFound = true;
                          if ((element.isObsSelected == obsrem.isObsSelected) && (element.isRemSelected == obsrem.isRemSelected)) {
                            obsrem.ismodified = false;
                          } else {
                            obsrem.ismodified = true;
                          }
                        }


                      })
                    }
                  })
                }
              });

              if (!isFound) {
                console.log(this.activeObsRemediation);

                if (this.activeObsRemediationTmp != undefined) {
                  this.activeObsRemediationTmp.observablesandremediations.forEach((element) => {
                    if ((element.ObservableId == Obsid) && (element.RemediationPlanId == remId)) {
                      isFound = true;
                      if ((element.isObsSelected == obsrem.isObsSelected) && (element.isRemSelected == obsrem.isRemSelected)) {
                        obsrem.ismodified = false;
                      } else {
                        obsrem.ismodified = true;
                      }
                    }
                  })
                }
              }


            }



          })
        }

      })





    } catch (e) {
      console.log(e)
    }



    //  this.childDetiils.forEach(child=>{

    //   if(child.resourceid == this.resourceIdComboChild){

    //      child.observablesandremediations.forEach(obsrem=>{
    //       if( (obsrem.ObservableId == observablesandremediation.ObservableId) && (obsrem.RemediationPlanId == observablesandremediation.RemediationPlanId)  ){

    //                  if( (obsrem.isObsSelected == observablesandremediation.isObsSelected) && (obsrem.isRemSelected == observablesandremediation.isRemSelected)  ){
    //                    observablesandremediation.isModified= false;
    //                  }else{
    //                    observablesandremediation.isModified=true;
    //                  }
    //             }
    //      })
    //   }
    // })




  }

  checkValue(observablesandremediation: Observablesandremediation) {
    debugger
    this.isResouceChanged = true;
    if (!observablesandremediation.isObsSelected) {
      observablesandremediation.isRemSelected = false;
    }
    //is modified??

    console.log(this.Resourcedetail);
    var isFound = false;
    this.resourceJsontmp[0].resourcedetails.forEach(obj => {

      if (obj.observablesandremediations != undefined) {
        if (obj.resourceid == this.Resourcedetail.resourceid) {

          obj.observablesandremediations.forEach(obs => {

            if ((obs.ObservableId == observablesandremediation.ObservableId) && (obs.RemediationPlanId == observablesandremediation.RemediationPlanId)) {
              isFound = true;
              if ((obs.isObsSelected == observablesandremediation.isObsSelected) && (obs.isRemSelected == observablesandremediation.isRemSelected)) {
                observablesandremediation.ismodified = false;
              } else {
                observablesandremediation.ismodified = true;
              }
            }
          })
        }
      }


    })

    if (!isFound) {
      console.log(this.activeObsRemediation);

      if (this.activeObsRemediationTmp != undefined) {
        this.activeObsRemediationTmp.observablesandremediations.forEach((obs) => {
          if ((obs.ObservableId == observablesandremediation.ObservableId) && (obs.RemediationPlanId == observablesandremediation.RemediationPlanId)) {

            if ((obs.isObsSelected == observablesandremediation.isObsSelected) && (obs.isRemSelected == observablesandremediation.isRemSelected)) {
              observablesandremediation.ismodified = false;
            } else {
              observablesandremediation.ismodified = true;
            }
          }
        })
      }
    }

    // this.isResouceChanged = true;


    // if (type == "main") {
    //   this.tempObs = JSON.parse(JSON.stringify(this.parentObsRem));
    // } else {
    //   this.tempObs = JSON.parse(JSON.stringify(this.childObsRemediation));
    // }
    // if (this.tempObs != undefined) {
    //   this.tempObs.forEach((obs) => {

    //     if (Obstype == 'obs' && obs.ObservableId == Obsid) {
    //       obs.isObsSelected = ischecked;
    //       // obs.isRemSelected = ischecked;
    //       if (!ischecked) {
    //         obs.isRemSelected = ischecked;
    //       }

    //     } else if (obs.ObservableId == Obsid && Obstype == 'rem' && obs.RemediationPlanId == remId) {
    //       obs.isRemSelected = ischecked;

    //     }

    //   })
    // }
    // if (type == "main") {
    //   this.parentObsRem = this.tempObs;
    // } else {
    //   // this.childObsRemediation = this.tempObs;
    // }


  }
  onReset() {
    try {
      this.assignValues(this.resourceJosn[0].resourcedetails[0].resourcetypeid, this.resourceIdCombo, this.resourceJosn[0].resourcedetails[0].resourcetypename);
    } catch (e) {

    }
  }

  buildJosn(resourceId: string) {
    // debugger
    this.resourceComboChild.length = 0;
    this.resourceComboChild = [];
    var firstItr = true;
    this.parentCombo = [];
    this.childDetailsTmp = this.childDetiils;
    var parentSet = new Set();
    var parentId = "";
    if (this.resourceJosn != null) {
      this.resourceJosn.forEach((element) => {
        this.observablesRemediationPlan = element.observablesandremediationplans;

        element.resourcedetails.forEach((res) => {
          if (resourceId == res.resourceid) {
            this.Resourcedetail = res;
            this.parentDetails = res.parentdetails;
            var childarray = res.childdetails;
            this.childDetailsTmp = childarray;
            this.childDetiils = childarray;
            this.parentObsRem = res.observablesandremediations;


            try {
              parentId = res.parentdetails[0].resourceid;
            } catch (error) {

            }
          }
          // firstItr = false;


          // this.resourceCombo.push({ 'key': res.resourcename, 'value': res.resourceid });

          if (res.parentdetails != undefined && res.parentdetails[0] != undefined) {

            this.parentCombo.push({ 'key': res.parentdetails[0].resourcename, 'value': res.parentdetails[0].resourceid })
          }

        });
      });



      try {
        this.parentCombo = Array.from(new Set(this.parentCombo.map(obj => obj.value))).map(id => {

          return {
            value: id,
            key: this.parentCombo.find(ele => ele.value === id).key
          }
        });


        try {

          if (this.parentCombo == undefined || this.parentCombo.length == 0) {

           
            this.resourceJosn.forEach((element) => {
              element.resourcedetails.forEach((res) => {

                this.resourceCombo.push({ 'key': res.resourcename, 'value': res.resourceid });

              })

            })
          } else {
            try {
              this.parentComboId=this.parentCombo[0].value;
            } catch (error) {
              
            }
           
            this.resourceJosn.forEach((element) => {
              element.resourcedetails.forEach((res) => {
                if (parentId == res.parentdetails[0].resourceid) {
                  this.resourceCombo.push({ 'key': res.resourcename, 'value': res.resourceid });
                }
              })

            })
          }

        } catch (error) {

        }


      } catch (e) {

      }

      if (this.childDetiils != null) {
        this.childObsRem = this.childDetiils[0].observablesandremediations;
        this.childDetiils.forEach((obj) => {
          this.resourceComboChild.push({ 'key': obj.resourcename, 'value': obj.resourceid })
        });


        this.onChangeResourceChild(this.childDetiils[0].resourceid);
      } else {
        this.childObsRem = [];
      }


if(this.isFromSummary){
  try {
        var parentid = "";
        this.resourceJsontmp.forEach((element) => {
          element.resourcedetails.forEach(res => {
            if (res.resourceid == this.resourceIdCombo) {
              if (res.parentdetails[0] != undefined) {
                parentid = res.parentdetails[0].resourceid;
              }
    
            }
          })
        })
        if (parentid != "") {
  this.parentComboId=parentid;

          // this.onChangeParentHTML(parentid);
          $("#resouceParentSelect").val(parentid);
          $("#resouceParentSelect").val(parentid).change();
        }
      } catch (error) {
        
      }
}

      // this.parentObsRem = this.Resourcedetail.observablesandremediations; commented last change

      // commentted change remobs
      // try {

      //   this.observablesRemediationPlan.forEach((resource) => {
      //     if (this.childDetiils == undefined) {
      //       this.mainObsRemediation = resource;
      //     } else {
      //       if (resource.resourcetypeid == this.childDetiils[0].resourcetypeid) {
      //         this.childObsRemediation = resource;
      //       } else {
      //         this.mainObsRemediation = resource;
      //       }
      //     }
      //   });
      //   // this.assignDefaults();

      // } catch (e) {
      // }
    }
  }


  resourceTypeId: string = "0";
  PlatformInstanceId: string;
  TenantId: string;
  ResourceTypeName: string;
  nodeParameters: any[] = [];
  isLoader: boolean = false;

  isShowTable(flag: boolean) {
    try {
      this.showTable = flag;
      this.nodeParameters = [];
      this.node.nativeElement.value = "";
      this.nodeSelect.nativeElement.value = "0"
      this.paramDesc.nativeElement.value = "";
      this.paramDisplay.nativeElement.value="";
    } catch (e) {

    }
  }

  summary: any = "";
  getSummaryDetails() {



  }


  getResourceConfiguration() {
    this.isLoader = true;
    this.confService.getResourceConfiguration(this.PlatformIdChild, this.TenantId).subscribe(
      (
        res => {
          this.isLoader = false;

          this.confJson = <Configuration[]>res;
          this.childObsRemediation = null;
          this.mainObsRemediation = null;
          this.resourceComboChild = [];
          try {
            if (this.confJson[0] != undefined)
              this.assignValues(this.confJson[0].resourcetypedetails[0].resourcetypeid, null, this.ResourceTypeNameChild)
            // this.assignValues(this.confJson[0].resourcetypedetails[0].resourcetypeid, null, this.confJson[0].resourcetypedetails[0].resourcetypename)
          } catch (e) {

          }
        }
      ), error => {
        console.log(error);
        this.isLoader = false;
      }

    )
  }
  getResourceModelConfiguration(resourceTypeId, resourceId: string) {
    // debugger

    // test code start

    // test code end



    // this.resourceJosn = <Resource[]>JSON.parse(JSON.stringify(this.confService.getsampleJson()));
    // this.resourceJsontmp = <Resource[]>JSON.parse(JSON.stringify(this.confService.getsampleJson()));
    // this.resourceComboChild.length = 0;
    // this.resourceComboChild = [];
    // this.Resourcedetail = {};
    // this.parentDetails = [];
    // this.childDetiils = [];
    // this.childDetailJson = {};
    // try {
    //   if (this.resourceJosn[0].resourcedetails[0].resourceid != undefined) {
    //     this.resourceIdCombo = this.resourceJosn[0].resourcedetails[0].resourceid
    //     if (resourceId == null) {
    //       this.buildJosn(this.resourceJosn[0].resourcedetails[0].resourceid);

    //     } else {
    //       this.resourceIdCombo = resourceId
    //       this.buildJosn(resourceId);
    //     }
    //   }
    // } catch (e) {
    //   console.log(e);
    // }



    // to-do un comment this
    this.isLoader = true;
    this.confService.getResourceModelConfiguration(this.PlatformInstanceId, this.TenantId, resourceTypeId).subscribe((res => {

      this.isLoader = false;
      this.resourceJosn = <Resource[]>res;
      var jsontmp = <Resource[]>res;
      this.resourceJsontmp = JSON.parse(JSON.stringify(jsontmp));
      this.resourceComboChild.length = 0;
      this.resourceComboChild = [];
      this.resourceCombo = [];
      this.Resourcedetail = {};
      this.parentDetails = [];
      this.childDetiils = [];
      this.childDetailJson = {};
      try {
        if (this.resourceJosn[0].resourcedetails[0].resourceid != undefined) {
          if (this.resourceJosn[0].resourcedetails[0].observablesandremediations != undefined) {
            this.Resourcedetail.observablesandremediations = this.resourceJosn[0].resourcedetails[0].observablesandremediations;
          }

          if (resourceId == null) {
            this.buildJosn(this.resourceJosn[0].resourcedetails[0].resourceid);
            this.resourceIdCombo = this.resourceJosn[0].resourcedetails[0].resourceid
          } else {
            this.resourceIdCombo = resourceId
            this.buildJosn(resourceId);
          }
        }
      } catch (e) {
        console.log(e);
      }


    }), error => {
      console.log(error);
      this.isLoader = false;
    }
    )
  }

  assignValues(index: string, resourceId: string, typename: string) {
    // debugger
    this.childObsRem = [];
    this.childDetailJson = {};
    this.ResourceTypeName = typename;
    this.resourceTypeId = index;

    try {
      this.PlatformInstanceId = this.confJson[0].platformid;
      this.TenantId = this.confJson[0].tenantid;
    } catch (e) {
      console.log(e);
    }

    this.childObsRemediation = null;

    this.mainObsRemediation = null;
    this.resourceComboChild = [];
    this.resourceCombo = [];
    //  this.resourceJosn = this.getResourceJson(index);
    this.getResourceModelConfiguration(typename, resourceId);
    this.resourceCombo = [];
    try {
      // this.onChangeParentHTML(this.parentCombo[0].value);
      this.onChangeParentHTML(this.parentCombo[0].value)
    } catch (e) {

    }


  }
  openSummary() {
    this.childResId = "";
  }
  isFromSummary:boolean;
  parentComboId:string;
  receiveActivateMsg(isclosed:string){
    this.getResourceConfiguration();
    this.getPortfolioDetails();
  }
  receiveMessage(inputData: any) {
    this.isFromSummary=true;
   inputData = JSON.parse(inputData);
   


    
    $("#summaryModel").modal("hide");




    this.assignValues(inputData.tyepeId, inputData.resourceId, inputData.typeName)

    
    try {
      window.scrollTo(0, 0);

    } catch (e) {

    }
    this.childResId = inputData.child;
    // var selfref=this;
    // window.setTimeout(function () { 
    //   selfref.onChangeResourceChild(inputData.child);
    //   }, 1100);


  }
  onUpdate() {

    var isconfirm = confirm(" Are you sure that you want to update");
    if (isconfirm) {
      this.saveResourceJson = {
        platformid: this.resourceJosn[0].platformid,
        tenantid: this.resourceJosn[0].tenantid,
        platformtype: this.resourceJosn[0].platformtype,
        resourcemodelversion: this.resourceJosn[0].resourcemodelversion,
        resourcedetails: [{

          childdetails: []
        }],

      };
      this.saveResourceJson.resourcedetails[0] = this.Resourcedetail;
      if(this.Resourcedetail.childdetails == undefined ||(this.Resourcedetail.childdetails.length == 0 && (this.childDetiils  != undefined  && this.childDetiils.length>0)) ){
        this.saveResourceJson.resourcedetails[0].childdetails=[];
        this.saveResourceJson.resourcedetails[0].childdetails=this.childDetiils
      }else{
        this.saveResourceJson.resourcedetails[0].childdetails = this.Resourcedetail.childdetails;
      }
      
      // this.saveResourceJson.resourcedetails[0].childdetails[1]={}
      // this.saveResourceJson.observablesandremediationplans = [{
      //   resourcetypeid: "0",
      //   observablesandremediations: [{}]
      // },
      // {
      //   resourcetypeid: "0",
      //   observablesandremediations: [{}]
      // }
      // ];



      // this.saveResourceJson.observablesandremediationplans[0] = this.mainObsRemediation;

      // if (this.childObsRemediation != undefined) {
      //   this.saveResourceJson.observablesandremediationplans[1] = this.childObsRemediation
      // }

      console.log(JSON.stringify(this.saveResourceJson));



      this.isLoader = true;
      this.confService.updateResourceModelConfiguration(JSON.stringify(this.saveResourceJson)).subscribe(
        (
          res => {
            this.isLoader = false;
            console.log("updated");
            this.toastr.successToastr('Configuration details updated successfully', 'Success!');
            if (this.saveResourceJson.resourcedetails[0].resourceid == this.saveResourceJson.resourcedetails[0].resourcename) {
              this.assignValues(this.saveResourceJson.resourcedetails[0].resourcetypeid, undefined, this.saveResourceJson.resourcedetails[0].resourcetypename)
            } else {
              this.assignValues(this.saveResourceJson.resourcedetails[0].resourcetypeid, this.saveResourceJson.resourcedetails[0].resourceid, this.saveResourceJson.resourcedetails[0].resourcetypename)
            }

          }
        ), error => {
          console.log(error);
          this.assignValues(this.saveResourceJson.resourcedetails[0].resourcetypeid, this.saveResourceJson.resourcedetails[0].resourceid, this.saveResourceJson.resourcedetails[0].resourcetypename)
          this.isLoader = false;
          this.toastr.errorToastr("Error occured !", 'Error!');
        }

      )
    } else {

    }


  }
  savePlan() {
    // debugger
    this.isResouceChanged = true;
    if (this.tempObsRemediation.observablesandremediations == null) {
      this.tempObsRemediation.observablesandremediations = [];

    } else {

    }
    if (this.parentObsRem == null) {
      this.parentObsRem = []

    }
    var count = 0;
    if (this.isParentNode) {
      if (this.tempObsRemediation.observablesandremediations != undefined) {
        try {
          this.tempObsRemediation.observablesandremediations.forEach((ele => {

            var ispresent = false;

            this.parentObsRem.forEach((plan => {
              if ((plan.ObservableId == ele.ObservableId) && (plan.ObservableActionId == ele.ObservableActionId) && (plan.RemediationPlanId == ele.RemediationPlanId)) {
                ispresent = true;
                count++;
              }

            }))

            if (!ispresent) {

              this.parentObsRem.push(ele);
              var existplan = false;

              if (this.Resourcedetail.observablesandremediations == undefined) {
                this.Resourcedetail.observablesandremediations = [];
                this.Resourcedetail.observablesandremediations.push(ele);
              } else {
                this.Resourcedetail.observablesandremediations.forEach(obj => {
                  if ((obj.ObservableId == ele.ObservableId) && (obj.ObservableActionId == ele.ObservableActionId) && (obj.RemediationPlanId == ele.RemediationPlanId)) {
                    existplan = true;
                  }
                })
                if (!existplan) {
                  this.Resourcedetail.observablesandremediations.push(ele);
                }
              }
              // TO_DO commented as duplicates are added




            }


          }))
        } catch (e) {

        }
      }
      if (count > 0) {
        this.toastr.infoToastr('Existing plans cannot be added', 'Info!');
      }
      count = 0;
      $("#AddNodeModal").modal("hide");



    } else {
      count = 0;
      if (this.childObsRem == null) {
        this.childObsRem = [];
      }
      // if (this.childObsRemediation.observablesandremediations == undefined) {

      //   this.childObsRemediation.observablesandremediations = [];
      // }


      if (this.tempObsRemediation.observablesandremediations != undefined) {
        this.tempObsRemediation.observablesandremediations.forEach((ele => {
          var ispresent = false;
          this.childObsRem.forEach((plan => {
            if (plan.ObservableId == ele.ObservableId) {
              ispresent = true;
              count++;
            }
          }))

          if (!ispresent) {


            this.childObsRem.push(ele);
            this.childDetiils.forEach((child => {
              if (child.resourceid == this.resourceIdComboChild) {
                if (child.observablesandremediations == undefined) {
                  child.observablesandremediations = [];
                }
                child.observablesandremediations.push(ele);
              }
            }))

            if (this.Resourcedetail.childdetails == undefined || this.Resourcedetail.childdetails.length == 0) {
              this.Resourcedetail.childdetails = [];
              this.Resourcedetail.childdetails.push(this.childDetiils[0]);
            }
            // var Obsremplan: Observablesandremediationplan;
            // if (this.childDetailsTmp[0]! = undefined) {
            //   Obsremplan = {
            //     resourcetypeid: this.childDetailsTmp[0].resourcetypeid,
            //     resourcetypename: this.childDetailsTmp[0].resourcetypename,
            //     observablesandremediations: []
            //   }
            // } else {
            //   Obsremplan = {
            //     resourcetypeid: "",
            //     resourcetypename: "",
            //     observablesandremediations: []
            //   }
            // }

            // Obsremplan.observablesandremediations[0] = ele;
            // // this.childObsRemediation[0].push(Obsremplan);
            // this.childObsRemediation.resourcetypeid = this.tempObsRemediation.resourcetypeid
            // this.childObsRemediation.resourcetypename = this.tempObsRemediation.resourcetypename
            // this.childObsRemediation.observablesandremediations.push(ele);
          }


        }))
      }
      if (count > 0) {
        this.toastr.infoToastr('Existing plans cannot be added', 'Info!');
      }
      count = 0;
      $("#AddNodeModal").modal("hide");
    }
  }
  selectPlan(flag: boolean, obsRem: Observablesandremediation) {
    // this.
    // debugger

    // this.tempObsRemediation.observablesandremediations = [];
    try {
      if (!flag) {
        this.tempObsRemediation.observablesandremediations = this.tempObsRemediation.observablesandremediations.filter(function (el) { return el.ObservableId != obsRem.ObservableId; });
      } else {

        this.tempObsRemediation.observablesandremediations.push(obsRem);
      }
    } catch (e) {

    }


  }

  isParentModel(flag: boolean) {
    try {
      this.tempObsRemediation = {

      };


      var Obsremplan: Observablesandremediationplan;
      if (this.isParentNode) {
        Obsremplan = {
          resourcetypeid: this.resourceTypeId,
          resourcetypename: this.ResourceTypeName,
          observablesandremediations: []
        }
      } else {
        Obsremplan = {
          resourcetypeid: this.childDetailsTmp[0].resourcetypeid,
          resourcetypename: this.childDetailsTmp[0].resourcetypename,
          observablesandremediations: []
        }
      }

      this.tempObsRemediation = Obsremplan;
    } catch (e) {

    }
    // if( (this.Resourcedetail.resourcetypename != 'Platform')  ){

    //   if( ( (flag == false) && this.Resourcedetail.resourcetypename == 'BOT' )){


    //   }else{
    // this.tempObsRemediation = {};
    this.isParentNode = flag;
    try {
      this.node.nativeElement.value = "";
      this.nodeSelect.nativeElement.value = "0";
    } catch (e) {

    }

    this.nodeParameters = [];
    // this.ResourceTypeName = "";
    try {
      this.ResourceTypeName = this.Resourcedetail.resourcetypename;

    } catch (e) {

    }
    try {
      this.param.nativeElement.value = "";

    } catch (e) {

    }




    this.isLoader = true;
    var resouceName = "";
    if (flag) {
      resouceName = this.Resourcedetail.resourcetypename;
    } else {
      try {
        resouceName = this.childDetailsTmp[0].resourcetypename;
      } catch (e) {

      }
    }
    this.confService.getObservablesAndRemediationDetails(this.PlatformInstanceId, this.TenantId, resouceName).subscribe(
      (
        res => {

          try {
            this.activeObsRemediation = JSON.parse(JSON.stringify(res[0].observablesandremediationplans[0]));
          } catch (e) {
            this.activeObsRemediation = undefined;
          }

          this.isLoader = false;
          var obj = res[0].observablesandremediationplans[0];
          try {
            // this.tempObsRemediation = JSON.parse(JSON.stringify(obj));
          } catch (e) {

          }
          console.log("activeObsRemediation");

          if (this.activeObsRemediation != undefined) {
            this.activeObsRemediation.observablesandremediations.forEach((obs) => {
              obs.isObsSelected = true;
              obs.isRemSelected = true;
              obs.ismodified = true;
            })
          }
          this.activeObsRemediationTmp = JSON.parse(JSON.stringify(this.activeObsRemediation));
        }
      ), error => {
        console.log(error);
        this.isLoader = false;
      }

    )
    //   }

    // }



  }
  onBlurCombo() {
    // this.includeDefaultDiv.nativeElement.style.display = 'none';
    this.node.nativeElement.value = "";
    this.paramDesc.nativeElement.value = "";

  }
  onBlurCreate() {
    // this.includeDefaultDiv.nativeElement.style.display = '';
    this.nodeSelect.nativeElement.value = "0"
    this.paramDesc.nativeElement.value = "";
    this.paramDisplay.nativeElement.value="";
  }
  addParameters(param: HTMLInputElement, node: HTMLInputElement, nodeSelect: HTMLSelectElement) {
    if (this.paramDesc.nativeElement.value == undefined) {
      this.paramDesc.nativeElement.value = ""
    }

    if(this.paramDisplay.nativeElement.value == undefined || this.paramDisplay.nativeElement.value == ""){
      this.paramDisplay.nativeElement.value= param.value;
    }

    if (((param.value != undefined && param.value.trim().length > 0) && (node.value != undefined && node.value.length > 0)) || (this.nodeSelect.nativeElement.value != undefined && this.nodeSelect.nativeElement.value != "0") && (param.value != undefined && param.value.trim().length > 0)) {
      this.nodeParameters.push({ 'value': param.value, 'desc': this.paramDesc.nativeElement.value,'displayname':this.paramDisplay.nativeElement.value });
      //  var ispresent=false;
      //      try{
      //       this.nodeParameters.forEach(function(value){
      //         if (value.indexOf(param.value) == -1){
      //           ispresent= true;
      //         }

      //       });
      //      }catch(e){

      //      }
      //      if(!ispresent){
      //       this.nodeParameters.push(param.value);
      //      }
      param.value = "";
      this.paramDesc.nativeElement.value = "";
      this.paramDisplay.nativeElement.value= "";
    }
  }


  deleteParams(param: string) {
    this.nodeParameters = this.nodeParameters.filter(item => item !== param)
  }

  saveNode(type: string) {
    debugger
    this.isResouceChanged = true;
    if (type == "node") {

      if (this.nodeSelect.nativeElement.value != "0") {

        if (this.isParentNode) {
          this.nodeParameters.forEach((element => {

            this.Resourcedetail.resourceattribute.push({
              "attributename": element.value,
              "attributevalue": "",
              
              "description": element.desc,
              "displayname":element.displayname,
              "IsSecret":false
            })
          }))


        } else {
          this.nodeParameters.forEach((element => {

            this.childDetiils[0].resourceattribute.push({
              "attributename": element.value,
              "attributevalue": "",
              "displayname": element.displayname,
              "description": element.desc,
              "IsSecret":false
            })
          }))
        }

        $("#AddNodeModal").modal("hide");

      } else if (this.node.nativeElement.value != "") {

        var resource: Resourcedetail = {};
        resource.resourceid = this.node.nativeElement.value

        resource.resourcename = this.node.nativeElement.value;
        resource.resourcetypeid = this.resourceTypeId;
        resource.resourcetypename = this.ResourceTypeName;
        resource.childdetails = null;
        resource.logdetails = null;
        resource.parentdetails = this.parentDetails
        resource.resourceattribute = [];
        resource.dontmonitor = true;
        resource.portfolioid="";
        this.parentObsRem=[];

        if (this.isParentNode) {
          this.resourceIdCombo = this.node.nativeElement.value
          this.nodeParameters.forEach((element => {

            resource.resourceattribute.push({
              "attributename": element.value,
              "attributevalue": "",
              "displayname": element.displayname,
              "description": element.desc,
              "IsSecret":false
            })
          }))
          resource.parentdetails = this.Resourcedetail.parentdetails;
          if (this.includeDefault.nativeElement.checked) {
            this.Resourcedetail.resourceattribute.forEach((ele => {
              var attrobj = ele;
              attrobj.attributevalue = "";
              resource.resourceattribute.push(attrobj);

            }))
          }
          this.resourceJosn[0].resourcedetails.push(resource);
          this.resourceCombo.push({ 'key': this.node.nativeElement.value, 'value': this.node.nativeElement.value });
          this.onChangeResource(this.node.nativeElement.value);
          if (this.nodeSelect.nativeElement.value == "0") {
            this.childObsRemediation = {}

            this.childObsRem = [];
          }

          $("#AddNodeModal").modal("hide");
        } else {
          // this.resourceJosn[0].resourcedetails
          if (this.childDetiils == null) {
            this.childDetiils = [];
          }
          resource.childdetails = [];
          var childDetails: ChildDetails = {};
          childDetails.resourceid = this.node.nativeElement.value
          childDetails.resourcename = this.node.nativeElement.value;
          if (this.childDetailsTmp[0] != undefined && this.childDetailsTmp[0].resourcetypename != undefined) {
            childDetails.resourcetypename = this.childDetailsTmp[0].resourcetypename;
            childDetails.resourcetypeid = this.childDetailsTmp[0].resourcetypeid;
          } else {
            childDetails.resourcetypename = "";
            childDetails.resourcetypeid = "";
          }

          childDetails.dontmonitor = this.Resourcedetail.dontmonitor;
          childDetails.portfolioid="";

          childDetails.resourceattribute = [];

          this.nodeParameters.forEach((element => {

            childDetails.resourceattribute.push({
              "attributename": element.value,
              "attributevalue": "",
              "displayname": element.displayname,
              "description": element.desc,
              "IsSecret":false
            })
          }))
          console.log(this.childDetailJson);
          if (this.includeDefault.nativeElement.checked) {
            try {
              this.childDetailJson.resourceattribute.forEach((ele => {
                var attrobj = ele;
                attrobj.attributevalue = "";
                childDetails.resourceattribute.push(attrobj);

              }))
            } catch (e) {

            }
          }
          if (this.Resourcedetail.childdetails == null) {
            this.Resourcedetail.childdetails = [];
          }

          // this.Resourcedetail.childdetails.push(childDetails);
          this.childDetiils.push(childDetails);
          // this.childDetiils[0] = childDetails;
          this.resourceIdComboChild = this.node.nativeElement.value;
          if (this.resourceComboChild == undefined) {
            this.resourceComboChild = [];
          }
          this.resourceComboChild.push({ 'key': this.node.nativeElement.value, 'value': this.node.nativeElement.value });
          this.onChangeResourceChild(this.node.nativeElement.value);
          $("#AddNodeModal").modal("hide");
        }

      }
    }
  }

  convertTodate(date1: string) {
    var date = new Date(date1);

    return date.getDate() + '-' + date.getMonth() + 1 + '-' + date.getFullYear() + ' ' + date.getHours() + ':' + date.getMinutes();

  }
  activateChildResource(flag) {
    // if (this.Resourcedetail.childdetails != undefined) {

    //   this.Resourcedetail.childdetails.forEach(child => {


    //     if(child.resourcetypeid == this.resourceIdComboChild){
    //       child.dontmonitor = flag;
    //     }

    //   })
    // }
    try {
      this.childDetailJson.dontmonitor = flag;
    } catch (e) {

    }

  }

  cascadeToChild() {
    try {
      // if (flag) {

      if (this.Resourcedetail.childdetails != undefined) {

        this.Resourcedetail.childdetails.forEach(child => {

          child.dontmonitor = this.Resourcedetail.dontmonitor;

        })
      }
      this.onChangeResourceChild(this.childDetailJson.resourceid)
      // }
    } catch (e) {

    }
  }


  dontIncludeChilds(flag: boolean) {

    try {
      // if (flag) {
      this.Resourcedetail.dontmonitor = flag;
      // if (this.Resourcedetail.childdetails != undefined) {

      //   this.Resourcedetail.childdetails.forEach(child => {

      //     child.dontmonitor = flag;

      //   })
      // }
      this.onChangeResourceChild(this.childDetailJson.resourceid)
      // }
    } catch (e) {

    }
  }


}
