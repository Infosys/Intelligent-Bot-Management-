import { Component, Output, Input, OnInit, EventEmitter } from '@angular/core';
import { ResourceSummary, Resourcemodeldetail, Childdetail } from '../../model/summary.model';
import { ConfigurationService } from '../../services/configuration.service';
import { Configuration, ResourceTypes } from '../../model/configuration.model';
import { ToastrManager } from 'ng6-toastr-notifications';
@Component({
  selector: 'app-activate-resource',
  templateUrl: './activate-resource.component.html',
  styleUrls: ['./activate-resource.component.css']
})
export class ActivateResourceComponent implements OnInit {

  constructor(private confService: ConfigurationService, public toastr: ToastrManager) { }
  isLoader: boolean = false;
  @Input() PlatformId;
  @Output() messageEvent = new EventEmitter<string>();
  // PlatformIdChild:any;
  confJson: Configuration[] = [{}];
  searchResourceVal:string;
  activateJson: any;
  activateJsonVisible: any;
  searchJson:any;
  parentResource: any[];
  resourceId: string;
  ngOnChanges() {
    this.activateJson = {};
    this.resourceId = "";
    this.activateJson.resourcedetails = [];
    this.parentResource = [];
    this.activateJsonVisible={};
    this.activateJsonVisible.resourcedetails = [];
    this.getResourceConfiguration();
}
  ngOnInit() {
    
    this.activateJson = {};
    this.resourceId = "";
    this.activateJson.resourcedetails = [];
    this.parentResource = [];
    this.activateJsonVisible={};
    this.activateJsonVisible.resourcedetails = [];
    this.getResourceConfiguration();


  }
  sendMessage() {
    
  
     this.messageEvent.emit("closed");
   }
  selectAll(flag: boolean) {
    
    try {
      this.activateJsonVisible.resourcedetails.forEach(resource => {
        resource.isactive = flag;
        resource.childdetails.forEach(child => {
          child.isactive = flag;
        })

      });
    } catch (error) {

    }
  }
  onReset() {
    this.onChangeMainResource(this.resourceId);
  }
  onChangeMainResource(resourceId: string) {
    if(resourceId != ""){

    
    this.searchResourceVal="";
    this.searchJson={};
    
    this.resourceId = resourceId;
    this.activateJson = {};
    this.activateJsonVisible={};
    this.activateJsonVisible.resourcedetails = [];
    this.activateJson.resourcedetails = [];
    this.isLoader = true
    this.confService.getResourceSummary(resourceId, this.PlatformId).subscribe(res => {
      this.isLoader = false
      this.activateJson = JSON.parse(JSON.stringify(res));
      var result=res
      this.parentResource = [];
      this.activateJsonVisible  =JSON.parse(JSON.stringify(result));
      this.searchJson=JSON.parse(JSON.stringify(result));
      this.activateJson.resourcedetails.forEach(resource => {
        this.parentResource.push({ "key": resource.parentdetails[0].resourcename, "value": resource.parentdetails[0].resourceid });

      });

      try {
        this.parentResource = Array.from(new Set(this.parentResource.map(obj => obj.value))).map(id => {

          return {
            value: id,
            key: this.parentResource.find(ele => ele.value === id).key
          }
        });
      }
      catch(e){
        console.log(e);
      }
    })
  }

  }
  onChangeParentResource(value:string){
    this.searchResourceVal="";
    this.searchJson={};
    
    // this.activateJsonVisible={};
    this.activateJsonVisible.resourcedetails = [];

    if(value  ==  "all"){
      

      this.activateJsonVisible.resourcedetails=[];
      var tempJson=JSON.parse(JSON.stringify(this.activateJson))
      tempJson.resourcedetails.forEach(resource => {
       
        // if(resource.parentdetails[0].resourceid == value ){
          this.activateJsonVisible.resourcedetails.push(resource);
        // }

      });
      this.searchJson=JSON.parse(JSON.stringify(this.activateJsonVisible))
      // this.activateJsonVisible.resourcedetails=this.activateJson.resourcedetails.slice(0);
    }else{
      this.activateJsonVisible.resourcedetails=[];
      var tempJson=JSON.parse(JSON.stringify(this.activateJson))
      tempJson.resourcedetails.forEach(resource => {
       
        if(resource.parentdetails[0].resourceid == value ){
          this.activateJsonVisible.resourcedetails.push(resource);
        }

      });
      this.searchJson=JSON.parse(JSON.stringify(this.activateJsonVisible))
    }

  }
  searchResource(value:string){

    var tempJson=JSON.parse(JSON.stringify(this.searchJson));
    if(value  ==  ""  ||  value.trim().length  ==  0){
      this.activateJsonVisible=JSON.parse(JSON.stringify(tempJson));
    }else{
      this.activateJsonVisible.resourcedetails=[];
      tempJson.resourcedetails.forEach(resource => {
       
        if(resource.resourcename.toLowerCase().indexOf(value.toLowerCase()) >= 0 ){
          this.activateJsonVisible.resourcedetails.push(resource);
        }
  
      });
    }
   
  }
  updateConfiguration() {
    console.log(JSON.stringify(this.activateJsonVisible))
    this.isLoader=true;
    this.confService.updateSummaryDetails(JSON.stringify(this.activateJsonVisible)).subscribe(res => {
      // this.
      this.isLoader=false;
      this.toastr.successToastr("Configuration  Updated Sucedfully", "SUCCESS");
      this.activateJson = {};
      this.activateJson.resourcedetails = [];
      this.activateJsonVisible={};
      this.activateJsonVisible.resourcedetails = [];
      this.getResourceConfiguration();

    })
  }
  onChangeResource(resourceobj: any) {
    try {
      resourceobj.childdetails.forEach(child => {
        child.isactive = resourceobj.isactive;
      })
    } catch (error) {

    }

  }
  getResourceConfiguration() {
    this.isLoader = true;
    this.confService.getResourceConfiguration(this.PlatformId, "1").subscribe(
      (
        res => {
          this.isLoader = false;

          this.confJson = <Configuration[]>res;


        }
      ), error => {
        console.log(error);
        this.isLoader = false;
      }

    )
  }
  onChangeParent(name: any) {
    // 
    // this.summary[0].resourcemodeldetails.forEach(res => {
    //   if (res.resourcename == name) {
    //     this.resourcedetails = res.resourcedetails;
    //     this.childdetails = res.childdetails;
    //   }
    // });
  }
  // sendMessage(tyepeId: any, resourceId: any, typeName: any, child: any) {

  //   console.log("tyepeId" + tyepeId);
  //   console.log("resourceId" + resourceId);
  //   console.log("typeName" + typeName);
  //   this.messageEvent.emit(JSON.stringify({ "tyepeId": tyepeId, "resourceId": resourceId, "typeName": typeName, "child": child }));
  // }



}
