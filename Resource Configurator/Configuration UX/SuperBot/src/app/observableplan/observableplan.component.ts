import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { Observableresourcetypeaction, ObservablePlan,  } from '../model/observableplan.model';
import { actionModel, ActList  } from '../model/action.model';
import {ObsList,Obs} from '../model/observable.model';
import { ResTypeList,ResType} from '../model/resourcetype.model';
import { ConfigurationService } from '../services/configuration.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { Configuration, ResourceTypes } from '../model/configuration.model';
//import {MatDatepickerModule} from '@angular/material/datepicker';
import { FormGroup, FormControl, NgForm } from '@angular/forms';
import { ResolveStart } from '@angular/router';
import { createInject } from '@angular/compiler/src/core';
import { NgModule } from '@angular/core';
import { DatePicker } from 'angular2-datetimepicker';
import { Resource, Resourcedetail, Parentdetail, Observablesandremediation, ChildDetails, Resourceattribute, Observablesandremediationplan } from '../model/resource.model';
import { Action } from '../../../node_modules/rxjs/internal/scheduler/Action';

@Component({
  selector: 'app-observableplan',
  templateUrl: './observableplan.component.html',
  styleUrls: ['./observableplan.component.css']
})
export class ObservableplanComponent implements OnInit {
  @ViewChild('node', { static: false }) node: ElementRef;
  @ViewChild('param', { static: false }) param: ElementRef;
  @ViewChild('nodeSelect', { static: false }) nodeSelect: ElementRef;
  @ViewChild('includeDefault', { static: false }) includeDefault: ElementRef;
  @ViewChild('includeDefaultDiv', { static: false }) includeDefaultDiv: ElementRef;
  @ViewChild('paramdesc', { static: false }) paramDesc: ElementRef;
  constructor(public toastr: ToastrManager, private confService: ConfigurationService) { 

    
  }
  isLoader : boolean;
  confJson: Configuration[] = [{}];
  fromDate: Date=new Date() ;
  toDate: Date =new Date() ;
  // fromDateChild: Date = new Date();
  // toDateChild: Date = new Date();
  observableresourcetypeaction:Observableresourcetypeaction;
  actlist:ActList;
  Action:actionModel;
  obsPlansArr:any;
  actArray:any;
  restypelist:ResTypeList;
  restype:ResType;
  restypearr:any;
  obs:Obs;
  obslist:ObsList;
  obsarray:any;
  resultMessage:any;
  sample:any;
  settings = {
    bigBanner: false,
    timePicker: false,
    format: 'dd-MM-yyyy',
    defaultOpen: false,
    closeOnSelect: true
    } 
  observablePlan:ObservablePlan;
  isUpdate:boolean;
  disableAddNewButton:boolean;
  ngOnInit() {
    // debugger
    this.isUpdate=true;
    this.getDetails();
  }

  getDetails(){
    this.restypearr=[];
      this.actArray=[];
      this.obsPlansArr=[];
      this.obsarray=[];
      this.observableresourcetypeaction= {
        name: "",
        resourcetypename: "",
        resourcetypeid: 0,
        observableid: 0,
        observablename: "",
        actionid: 0,
        actionname: "",
        CreatedBy: "",
        ModifiedBy: null,
        CreateDate: null,
      ModifiedDate: null,
        ValidityStart: new Date(),
        ValidityEnd: new Date()
      }
      
      this.confService.getObservableResourceTypeActionMap("1").subscribe(res=>{
        this.observablePlan =<ObservablePlan> res;
        // debugger
        this.obsPlansArr=[];
        this.observablePlan.observableresourcetypeactions.forEach(obs => {
          // if(obs.name.trim() != 'Platform'){
            this.obsPlansArr.push({ "id": obs.observableid, "name": obs.name });
          // }
          
        })
      });
      this.actArray=[]; 
      this.confService.getActionModelList("1").subscribe(res=>{
        this.Action =<actionModel> res;
        
        this.Action.actList.forEach(act => {
          this.actArray.push({ "id": act.actionid, "name":act.actionname});
        })
      });
      this.obsarray=[];
      this.confService.getObservableModelList("1").subscribe(res=>{
        this.obs =<Obs> res;
       
        this.obs.obsList.forEach(ob => {
          this.obsarray.push({ "id":ob.observableid,"observablename":ob.observablename});
        })
      });
      this.restypearr=[];
      this.confService.getResourceTypeModelList("1").subscribe(res=>{
        this.restype =<ResType> res;
        
        this.restype.resTypeList.forEach(res => {
          if(res.resourcetypename.trim() != 'Platform'){
            this.restypearr.push({"id":res.resourcetypeid,"resourcetypename":res.resourcetypename})
          }
         
        })
      });
  
  }
  OnChangesObservable(id:number,observableresourcetypeaction:Observableresourcetypeaction){
    this.observableresourcetypeaction.observableid=id;
  }
  OnChangesObs(name:string){
    
    this.observablePlan.observableresourcetypeactions.forEach(obs => {
      if(name == obs.name){
        this.observableresourcetypeaction=obs;
      }

    });
  }
  OnChangesRes(id:number){
    this.observableresourcetypeaction.resourcetypeid=id;
  }
  OnChangesAction(id:number){
    this.observableresourcetypeaction.actionid=id;
  }
  createNew(attrForm: NgForm){
    this.isUpdate=false;
    this.disableAddNewButton=true;
  //this.sample ="sample string";
  
    console.log(this.observableresourcetypeaction.name)
    console.log(this.observableresourcetypeaction.observablename)
    let addInputJson = {
      "PlatformID":"1",
      "tenantID": "1",
  
      "observableresourcetypeactions": [{"name": this.observableresourcetypeaction.name,
      "resourcetypename": this.observableresourcetypeaction.resourcetypename,
      "resourcetypeid": this.observableresourcetypeaction.resourcetypeid,
      "observableid": this.observableresourcetypeaction.observableid,
      "observablename": this.observableresourcetypeaction.observablename,
      "actionid": this.observableresourcetypeaction.actionid,
      "actionname": this.observableresourcetypeaction.actionname,
      "CreatedBy": this.observableresourcetypeaction.CreatedBy,
      "ModifiedBy": this.observableresourcetypeaction.ModifiedBy,
      "CreateDate": this.observableresourcetypeaction.CreateDate,
      "ModifiedDate": this.observableresourcetypeaction.ModifiedDate,
      "ValidityStart":this.observableresourcetypeaction.ValidityStart,
      "ValidityEnd": this.observableresourcetypeaction.ValidityEnd
    } ]
  
    
    }
  
    this.confService.addObservableResourceTypeActionMap(JSON.stringify(addInputJson)).subscribe(res => {
     try{
      this.sample = <string>res;
      if(this.sample.search("Exception") == -1){
        console.log("Exception occured!")
        this.toastr.errorToastr("Exception occured !", 'Error!');
      }
      else {
        this.toastr.successToastr('Plan added successfully', 'Success!');  
      }
        
     }catch(e){
      this.toastr.successToastr('Plan added successfully', 'Success!');  
     }
        this.onCancel();
      
      }, error => {
        this.toastr.errorToastr("Error Occured - Please enter valid combination of Resource Type , Observable and Action name", 'Error!');
      })
  }
  
onCancel() {
  this.getDetails();
  this.isUpdate = false;
  this.disableAddNewButton = true;
  this.observableresourcetypeaction = {
    name: "",
    resourcetypename: "",
    resourcetypeid: 0,
    observableid: 0,
    observablename: "",
    actionid: 0,
    actionname: "",
    CreatedBy: "",
    ModifiedBy: null,
    CreateDate: null,
    ModifiedDate: null,
    ValidityStart: new Date(),
    ValidityEnd: new Date()
  }
}

reset() {
  // form.reset()
  this.isUpdate = true;
  this.getDetails();
  (<HTMLInputElement>document.getElementById('resourcetype')).value = "";
  (<HTMLInputElement>document.getElementById('actionname')).value = "";
  (<HTMLInputElement>document.getElementById('observablename')).value = "";
  (<HTMLSelectElement>document.getElementById('attrselect')).selectedIndex = 0;
  (<HTMLSelectElement>document.getElementById('attrselect1')).selectedIndex = 0;
  (<HTMLSelectElement>document.getElementById('attrselect2')).selectedIndex = 0;
  (<HTMLSelectElement>document.getElementById('attrselect3')).selectedIndex = 0;
}
 
  convertTodate(date1: string) {
    var date = new Date(date1);

    return date.getDate() + '-' + date.getMonth() + 1 + '-' + date.getFullYear() + ' ' + date.getHours() + ':' + date.getMinutes();

  }
  PlatformInstanceId: string;
  TenantId: string;

  updateAttribute(attrForm: NgForm){
    var json : any = JSON.stringify(this.observableresourcetypeaction)
    console.log("validity start date" +this.observableresourcetypeaction.ValidityStart)
  let inputJson = {
    "PlatformID":"1",
    "tenantID": "1",

    "observableresourcetypeactions": [{
      "name": this.observableresourcetypeaction.name,
      "resourcetypename": this.observableresourcetypeaction.resourcetypename,
      "resourcetypeid": this.observableresourcetypeaction.resourcetypeid,
      "observableid": this.observableresourcetypeaction.observableid,
      "observablename": this.observableresourcetypeaction.observablename,
      "actionid": this.observableresourcetypeaction.actionid,
      "actionname": this.observableresourcetypeaction.actionname,
      "CreatedBy": this.observableresourcetypeaction.CreatedBy,
      "ModifiedBy": this.observableresourcetypeaction.ModifiedBy,
      "CreateDate": this.observableresourcetypeaction.CreateDate,
      "ModifiedDate": this.observableresourcetypeaction.ModifiedDate,
      "ValidityStart":this.observableresourcetypeaction.ValidityStart,
      "ValidityEnd": this.observableresourcetypeaction.ValidityEnd,
      }]

  }
  console.log(JSON.stringify(inputJson))
  
  this.confService.updateObservableResourceTypeActionMap(JSON.stringify(inputJson)).subscribe(res => {
  this.toastr.successToastr('Plan updated successfully', 'Success!');

  }, error => {
    this.toastr.errorToastr("Error occured !", 'Error!');
  })
 
  

  }
  onDateSelect(date: any, action: Observableresourcetypeaction,type:string) {
    if(type == "start"){
      action.ValidityStart = new Date(date);
    }else{
      action.ValidityEnd = new Date(date);
    }
   

  }

}
