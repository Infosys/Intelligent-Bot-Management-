import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ObservableConfig, observableDetails } from '../model/observable-config.model';
import { ConfigurationService } from '../services/configuration.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { debug } from 'util';
@Component({
  selector: 'app-observable-config',
  templateUrl: './observable-config.component.html',
  styleUrls: ['./observable-config.component.css']
})
export class ObservableConfigComponent implements OnInit {

  constructor(public toastr: ToastrManager, private confService: ConfigurationService) { }
  @ViewChild('actionNameSelect', { static: false }) actionNameSelect: ElementRef;
  fromDate: Date = new Date();
  toDate: Date = new Date();
  settings = {
    bigBanner: false,
    timePicker: false,
    format: 'dd-MM-yyyy',
    defaultOpen: false,
    closeOnSelect: true
  }
  observableConfig: ObservableConfig;
  observable: observableDetails;
  observableArr: any;
  isUpdatePage: boolean;
  isLoader: boolean;
  ngOnInit() {
    this.isUpdatePage = true;
    this.isLoader = false;
    this.observableArr = [];
    this.getObsDetails();
    this.observable = {
      "observableid": "0",
      "observablename": "",
      "unitofmeasure": "",
      "datatype": "",
      "ValidityStart": new Date(),
      "ValidityEnd": new Date(),
      "createdby": "",
      "ModifiedBy": "",
      "CreateDate": "",
      "ModifiedDate": ""
    }

  }
  onChangeDataType(type: any) {
    this.observable.datatype = type;
  }
  getObsDetails() {
    this.observableArr = [];
    this.confService.getObservableConfiguration("1").subscribe(res => {
      this.observableConfig = <ObservableConfig>res;
      this.observableConfig.observableDetails.forEach(obs => {
        try {
          obs.unitofmeasure = obs.unitofmeasure.trim();
        } catch (e) {

        }
        this.observableArr.push({ "id": obs.observableid, "name": obs.observablename });
      })
    })


  }
  onChangeObservable(id: any) {
    this.observableConfig.observableDetails.forEach(obs => {
      if (obs.observableid == id) {
        this.observable = obs;
      }
    })
  }
  changePageType() {
    this.isUpdatePage = false;
    this.observable = {
      "observableid": "0",
      "observablename": "",
      "unitofmeasure": "",
      "datatype": "",
      "ValidityStart": new Date(),
      "ValidityEnd": new Date(),
      "createdby": "",
      "ModifiedBy": "",
      "CreateDate": "",
      "ModifiedDate": ""
    }

  }

  onReset() {
    this.observable = {
      "observableid": "0",
      "observablename": "",
      "unitofmeasure": "",
      "datatype": "",
      "ValidityStart": new Date(),
      "ValidityEnd": new Date(),
      "createdby": "",
      "ModifiedBy": "",
      "CreateDate": "",
      "ModifiedDate": ""
    }
    // actionnameselect
    try {
      this.actionNameSelect.nativeElement.value = "";
    } catch (e) {

    }

    this.isUpdatePage = true;
    this.getObsDetails();
  }

  
  onUpdate() {

    var json: any = JSON.stringify(this.observable)
    console.log(this.isUpdatePage)

    if (this.isUpdatePage) {
      this.isLoader = true;

      var inputJson = {
        "tenantId": 1,
        "observableDetails": []
      }
      inputJson.observableDetails = [];
      inputJson.observableDetails.push(this.observable);
      console.log(inputJson);
      console.log(JSON.stringify(inputJson));
      this.confService.updateObservableConfiguration(JSON.stringify(inputJson)).subscribe(res => {
        this.isLoader = false;
        this.onReset();
        this.toastr.successToastr('Configuration details updated successfully', 'Success!');

      }, error => {
        this.isLoader = false;
        this.toastr.errorToastr("Error occured !", 'Error!');
      }
      )
    } else {
      this.isLoader = true;
      var inputJson = {
        "tenantId": 1,
        "observableDetails": []
      }
      inputJson.observableDetails = [];
      inputJson.observableDetails.push(this.observable);
      console.log(inputJson);
      console.log(JSON.stringify(inputJson));
      this.confService.addObservableConfiguration(JSON.stringify(inputJson)).subscribe(res => {
        this.isLoader = false;
        this.changePageType();
        this.toastr.successToastr('Configuration details added successfully', 'Success!');
      }, error => {
        this.isLoader = false;
        this.toastr.errorToastr("Error occured !", 'Error!');
      }
      )
    }


  }
  onDateSelect(date: any, obs: observableDetails, type: string) {
    if (type == "start") {
      obs.ValidityStart = new Date(date);
    } else {
      obs.ValidityEnd = new Date(date);
    }


  }

}
