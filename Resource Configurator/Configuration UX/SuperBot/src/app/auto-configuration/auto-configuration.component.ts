import { Component, OnInit, Output, EventEmitter, ElementRef, ViewChild } from '@angular/core';
import { MetaConfig, Parentdetail, Childdetail, Resourcetypemetadata, Resourcetypedetail } from '../model/meta-config.model'
import { ConfigurationService } from '../services/configuration.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { FormGroup, FormControl, NgForm } from '@angular/forms';
import { ResolveStart } from '@angular/router';
import { createInject } from '@angular/compiler/src/core';
import { Description } from '../model/description.model';
import { resource } from '../model/resource-input.model';
import { config } from 'rxjs';

@Component({
  selector: 'app-auto-configuration',
  templateUrl: './auto-configuration.component.html',
  styleUrls: ['./auto-configuration.component.css']
})
export class AutoConfigurationComponent implements OnInit {
  @ViewChild('actionNameSelect', { static: false }) actionNameSelect: ElementRef;

  @Output() messageEvent = new EventEmitter<any>();
  constructor(private confService: ConfigurationService, public toastr: ToastrManager) { }
  des: Description;
  resource_model: resource;
  isLoader: boolean = false;
  childElements: any[];
  isUpdateAttr: boolean;
  selectedAttrId: string;
  attributename: string;
  displayname: string;
  resourcetypeid: any;
  discoveryJson: any[];
  platformJson: any;
  ngOnInit() {
    this.attributename = ""
    this.displayname = "";
    this.isLoader = false;
    this.childElements = [];
    this.isUpdateAttr = false;
    this.selectedAttrId = "";
    this.discoveryJson = this.confService.getDiscoveryDetails();


    console.log(this.discoveryJson);
    this.des = {
      "Tenantid": 1,
      "Platformname": "",
      "Platformtype": "",
      "HostName": "",
      "IPAddress": "",
      "Database_HostName": "",
      "Database_Type": "",
      "Database_IPaddress": "",
      "Database_Name": "",
      "Database_UserName": "",
      "Database_Password": "",
      "API_Password": "",
      "API_UserName": "",
      "API_URL": "",
      "Service_UserName": "",
      "Service_Password": ""

    }

  }
  onChangePlatform(name: string) {

    this.discoveryJson.forEach(discovery => {
      if (discovery.name == name) {
        this.platformJson = discovery;
      }
    })
  }
  onReset() {
    this.des = {
      "Tenantid": 1,
      "Platformname": "",
      "Platformtype": "",
      "HostName": "",
      "IPAddress": "",
      "Database_HostName": "",
      "Database_Type": "",
      "Database_IPaddress": "",
      "Database_Name": "",
      "Database_UserName": "",
      "Database_Password": "",
      "API_Password": "",
      "API_UserName": "",
      "API_URL": "",
      "Service_UserName": "",
      "Service_Password": ""

    }
    try {
      this.actionNameSelect.nativeElement.value = "";
    } catch (e) {

    }
    // actionnameselect

  }
  getResourceModelUipathOnPremise(inputJson: any) {
    this.isLoader = true;
    this.confService.getResourceModelUipathOnPremise(JSON.stringify(this.platformJson)).subscribe(res => {
      this.resource_model = <resource>res;
      console.log(this.resource_model);
      try {
        this.isLoader = false;

        this.viewPlatform(this.resource_model.PlatformId.toString(), this.resource_model.ResourceTypeName);
        // {"Tenantid":1,"PlatformId":1,"ResourceTypeName":"Platform"} 
      } catch (e) {

      }
    }), error => {
      console.log(error);
      this.isLoader = false;
      this.toastr.errorToastr("Error occured !", 'Error!');
    }
  }
  getResourceModelUipath(inputJson: any) {
    this.isLoader = true;
    this.confService.getResourceModelUipath(JSON.stringify(this.platformJson)).subscribe(res => {
      this.resource_model = <resource>res;
      console.log(this.resource_model);
      try {
        this.isLoader = false;

        this.viewPlatform(this.resource_model.PlatformId.toString(), this.resource_model.ResourceTypeName);
        // {"Tenantid":1,"PlatformId":1,"ResourceTypeName":"Platform"} 
      } catch (e) {

      }
    }), error => {
      console.log(error);
      this.isLoader = false;
      this.toastr.errorToastr("Error occured !", 'Error!');
    }


  }

  onDiscover(validForm: NgForm) {
    // debugger
    if (validForm.valid) {

      try {
        this.platformJson.tenantId = this.confService.tenantId;
      } catch (error) {

      }
      console.log(JSON.stringify(this.platformJson))

      // this.viewPlatform("3","Platform");
      if (this.platformJson.name == 'UiPath on-Premise') {
        this.platformJson.name = 'UiPath';
        this.getResourceModelUipath(this.platformJson);
       

      } else
        if (this.platformJson.name == 'UiPath') {
          this.platformJson.name = 'UiPath';
          this.getResourceModelUipath(this.platformJson);
        } else if (this.platformJson.name == 'Automation Anywhere Enterprise') {
          this.isLoader = true;
          this.confService.getResourceModelScreen(JSON.stringify(this.platformJson)).subscribe(res => {
            this.resource_model = <resource>res;
            console.log(this.resource_model);
            try {
              this.isLoader = false;

              this.viewPlatform(this.resource_model.PlatformId.toString(), this.resource_model.ResourceTypeName);
              // {"Tenantid":1,"PlatformId":1,"ResourceTypeName":"Platform"} 
            } catch (e) {

            }
          }), error => {
            console.log(error);
            this.isLoader = false;
            this.toastr.errorToastr("Error occured !", 'Error!');
          }

        }

    }
  }
  viewPlatform(PlatFormId: string, ResourceTypeName: string) {
    this.messageEvent.emit({ "PlatFormId": PlatFormId, "ResourceTypeName": ResourceTypeName, "count": 1 })
  }

}
