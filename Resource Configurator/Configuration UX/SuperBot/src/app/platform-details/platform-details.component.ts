import { Component,ChangeDetectorRef, OnInit,EventEmitter,Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationService } from '../services/configuration.service';
import { PropertiesService } from '../services/properties.service';
import { ToastrManager } from 'ng6-toastr-notifications';
// import   properties  from  '/superbotapp/assets/properties.json';
declare var $: any;
@Component({
  selector: 'app-platform-details',
  templateUrl: './platform-details.component.html',
  styleUrls: ['./platform-details.component.css']
})
export class PlatformDetailsComponent implements OnInit {

  constructor(
    private  toastr: ToastrManager,
    private propSvc:PropertiesService,
    private ref: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router, private configsvc:ConfigurationService) {}
    @Output() messageEvent = new EventEmitter<any>();
    platformJson:PlatformDetails[];
    isLoader: boolean = false;
    loadPage:boolean;
    PlatformIdChild:string;
  ngOnInit() {
    // this.ref.detectChanges();
    this.propSvc.getJSON().subscribe(res=>{

      console.log(res.server_address);
      console.log( res.services_app_name);
      console.log(res.category_service);
      
    })
    // console.log(properties);
    // this.router.navigate(['configuration'], { });
    // this.PlatformIdChild="1";
    this.loadPage=false;
    this.sendEvent();
  }
  activateDeactivateResource(PlatFormId:string,ResourceTypeName:string){
    ;
    this.PlatformIdChild=PlatFormId;
    this.ref.detectChanges();
    $("#summaryActivateModel").modal("show");
    this.ref.detectChanges();
  }
  viewPlatform(PlatFormId:string,ResourceTypeName:string){
    this.messageEvent.emit({"PlatFormId":PlatFormId,"ResourceTypeName":ResourceTypeName,"count":1})
  }
  sendEvent() {

    // tAlltAll
    this.isLoader=true;
    this.configsvc.getAllPlatforms("1").subscribe(res=>{
      this.platformJson=JSON.parse(JSON.stringify(res));
      this.loadPage=true;
      this.isLoader=false;
    
      if( this.platformJson.length ==1){
        this.messageEvent.emit({"id":12,"name":"namwe","count":0})
      }
    
    }, error => {
      console.log(error);
      this.isLoader = false;
      // this.toa
      this.toastr.errorToastr("Server Error !", 'Error!');
    }
    
    )

   ;
  }
  
}
export interface PlatformDetails {
  PlatformId: string;
  PlatformTypeName: string;
  PlatformInstanceName: string;
  ResourceTypeId: string;
  ResourceTypeName: string;
    }