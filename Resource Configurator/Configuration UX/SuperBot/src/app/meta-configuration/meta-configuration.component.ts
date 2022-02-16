import { Component, OnInit, ChangeDetectorRef, ElementRef, ViewChild } from '@angular/core';
import { MetaConfig, Parentdetail, Childdetail, Resourcetypemetadata, Resourcetypedetail } from '../model/meta-config.model'
import { ConfigurationService } from '../services/configuration.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { FormGroup, FormControl, NgForm } from '@angular/forms';
import { ResolveStart } from '@angular/router';
import { createInject } from '@angular/compiler/src/core';

@Component({
  selector: 'app-meta-configuration',
  templateUrl: './meta-configuration.component.html',
  styleUrls: ['./meta-configuration.component.css']
})
export class MetaConfigurationComponent implements OnInit {
  @ViewChild('closeModal', { static: false }) closeModal: ElementRef;
  @ViewChild('attrForm', { static: false }) attrForm: NgForm;
  @ViewChild('defaultvalue', { static: false }) defaultValue: ElementRef;
  @ViewChild('description', { static: false }) description: ElementRef;
  
  constructor(private confService: ConfigurationService, public toastr: ToastrManager, private ref: ChangeDetectorRef) { }
  metaConfig: MetaConfig[];
  metaConfigJson: MetaConfig;
  resourcetypeJson: Resourcetypedetail;
  resourceTypes: any[];
  attributes: any;
  isLoader: boolean = false;
  childElements: any[];
  isUpdateAttr: boolean;
  selectedAttrId: string;
  attributename: string;
  displayname: string;
  resourcetypeid: any;
  portfolios:any;
  ngOnInit() {

    this.attributename = ""
    this.displayname = "";
    this.isLoader = false;
    this.childElements = [];
    this.isUpdateAttr = false;
    this.selectedAttrId = "";
    this.metaConfigJson = {};
    this.assignDefaults();
    this.getPortfolioDetails();
    // this.mappingform.reset();
    // (<HTMLFormElement>document.getElementById("mappingform")).reset();
  }
  getPortfolioDetails() {
    this.confService.getPortfolioDetails("1").subscribe(res => {
      this.portfolios = res;
    })
  }
  onChangeType(type: string) {

    this.metaConfig.forEach(config => {
      if (config.platformtype == type) {
        this.metaConfigJson =config;
        this.addDefaultTypes();
      }
    })
  }


  platfromType: any[];
  assignDefaults() {
    
    this.platfromType = [];
    this.isLoader = true;
    this.confService.getMetaData('1').subscribe(res => {
      this.isLoader = false;
      this.metaConfig = <any[]>res;
      // this.metaConfig=JSON.parse(JSON.stringify(this.metaConfig));
      console.log(this.metaConfig);
      this.resourceTypes = [];
      // this.metaConfigJson=this.metaConfig[0];

      this.metaConfig.forEach(type => {
        this.platfromType.push({ "type": type.platformtype });
      })
      // this.addDefaultTypes();
      this.onChangeType(this.metaConfig[0].platformtype)
      this.ref.detectChanges();
    }, error => {
      console.log(error);
      this.isLoader = false;
    }
    );

  }
  addDefaultTypes() {

    this.resourceTypes=[];

    this.metaConfigJson.resourcetypedetails.forEach(resource => {

      if ((resource.parentdetails == null || resource.parentdetails.length == 0) && (resource.childdetails == null || resource.childdetails.length == 0)) {
        resource.isVisible = false;
      } else {
        resource.isVisible = true;
      }
      this.resourceTypes.push({ 'key': resource.resourcetypeid, 'value': resource.resourcetypename });
    });
    this.ref.detectChanges();

    (<HTMLInputElement>document.getElementById('updatebtn')).style.display = 'none';
    (<HTMLInputElement>document.getElementById('deletebtn')).style.display = 'none';
    (<HTMLInputElement>document.getElementById('addbtn')).style.display = '';
    this.resourcetypeid = -1;
    (<HTMLSelectElement>document.getElementById("resourcetypeidsel")).value = "";
    (<HTMLSelectElement>document.getElementById('resourcetypeidsel')).selectedIndex = -1;
  }
  addChild(resourcetypeid: any) {


    var resourcetypemetadata: any = (<HTMLSelectElement>document.getElementById('resourcetypeidparent-' + resourcetypeid)).value;
    resourcetypemetadata = JSON.parse(resourcetypemetadata);

    // var typeid = (<HTMLSelectElement>document.getElementById('resourcetypeidparent-' + resourcetypeid)).selectedIndex;
    var value = "";
    try {
      // this.resourceTypes.forEach(ele => {

      //   if (ele.key == resourcetypemetadata.key) {
      //     value = ele.value;

      //   }

      // })

      var isexists: boolean = false;
      this.metaConfigJson.resourcetypedetails.forEach(resource => {

        if (resource.resourcetypeid == resourcetypeid) {

          if (resource.childdetails == null) {
            resource.childdetails = []
          } else {
            if (resource.childdetails.length >= 1) {
              resource.childdetails.forEach(child => {

                if (child.resourcetypeid == resourcetypemetadata.key) {
                  isexists = true;
                }
              })
              if (!isexists) {
                resource.childdetails.push({ "resourcetypename": resourcetypemetadata.value, "resourcetypeid": resourcetypemetadata.key.toString() })


              }
            } else {

              resource.childdetails.push({ "resourcetypename": resourcetypemetadata.value, "resourcetypeid": resourcetypemetadata.key.toString() });
            }

          }



        }
      });
    } catch (e) {

    }


  }
  onChangeMainMenu(resourcetypedetail: any, flag: boolean) {

    resourcetypedetail.ismainentry = flag;
  }
  updateMapping() {

    console.log(JSON.stringify(this.metaConfig[0]));

    this.confService.updateResourceTypeMetaData(JSON.stringify(this.metaConfigJson)).subscribe(res => {

      this.toastr.successToastr('Configuration updated successfully', 'Success!');
      this.assignDefaults();
    }, error => {
      this.toastr.errorToastr("Error occured !", 'Error!');

    })
  }

  deleteMapping(resourcetypeid: any) {
    this.metaConfigJson.resourcetypedetails.forEach(resource => {

      if (resource.resourcetypeid == resourcetypeid) {
        resource.ismappingdeleted = true;
        resource.isVisible = false;
      }
    });

  }
  onChangeResourceType(resourcetypeid: string) {
    (<HTMLInputElement>document.getElementById('description')).value = "";
    // (<HTMLInputElement>document.getElementById('defaultvalue')).value = "";
    this.defaultValue.nativeElement.value="";
    this.description.nativeElement.value="";
    (<HTMLInputElement>document.getElementById('attributename')).value = "";
    (<HTMLInputElement>document.getElementById('displayname')).value = "";

    this.attributes = [];
    this.metaConfigJson.resourcetypedetails.forEach(resource => {

      if (resource.resourcetypeid == resourcetypeid) {
        this.attributes = resource.resourcetypemetadata;
      }
    });
  }
  onSelectAttribute(resourcetypemetadata: Resourcetypemetadata, typeid: any, index: any) {

    (<HTMLSelectElement>document.getElementById("resourcetypeidsel")).selectedIndex = (parseInt(typeid) + 1);
    (<HTMLSelectElement>document.getElementById("resourcetypeidsel")).value = typeid;
    this.resourcetypeid = typeid
    // resourcetypemetadata = JSON.parse(resourcetypemetadata);
    this.onChangeResourceType(typeid);
    this.selectedAttrId = resourcetypemetadata.attributename;

    this.onChangeAttribute(JSON.stringify(resourcetypemetadata));
    window.setTimeout(function () {
      (<HTMLSelectElement>document.getElementById('attrselect')).selectedIndex = (parseInt(index) + 1);
    }, 50);


  }

  onChangeAttribute(resourcetypemetadata: any) {

    this.isUpdateAttr = false;
    this.resourcetypeid = (<HTMLSelectElement>document.getElementById("resourcetypeidsel")).value;
    console.log((<HTMLSelectElement>document.getElementById('attrselect')).selectedIndex)
    // if ((<HTMLSelectElement>document.getElementById('attrselect')).value != "") {
    if (this.selectedAttrId != "") {
      this.isUpdateAttr = true;

      resourcetypemetadata = JSON.parse(resourcetypemetadata);
      this.defaultValue.nativeElement.value=resourcetypemetadata.DefaultValue;
      this.description.nativeElement.value=resourcetypemetadata.description;
      (<HTMLInputElement>document.getElementById('description')).value = resourcetypemetadata.description;
      // (<HTMLInputElement>document.getElementById('defaultvalue')).value=resourcetypemetadata.DefaultValue;
      (<HTMLInputElement>document.getElementById('updatebtn')).style.display = '';
      (<HTMLInputElement>document.getElementById('deletebtn')).style.display = '';
      (<HTMLInputElement>document.getElementById('addbtn')).style.display = 'none';
      // this.resourcetypeid=resourcetypemetadata.resourcetypeid
      try {
        this.displayname = resourcetypemetadata.displayname;
        this.attributename = resourcetypemetadata.attributename;
        (<HTMLInputElement>document.getElementById('attributename')).value = resourcetypemetadata.attributename;
        (<HTMLInputElement>document.getElementById('displayname')).value = resourcetypemetadata.displayname;
      } catch (e) {

      }
    } else {
      this.resetButtons();
      this.isUpdateAttr = false;
    }



  }
  resetButtons() {
    // this.mappingform.reset();
    (<HTMLSelectElement>document.getElementById('attrselect')).selectedIndex = 0;
    (<HTMLInputElement>document.getElementById('addbtn')).style.display = '';
    (<HTMLInputElement>document.getElementById('updatebtn')).style.display = 'none';
    (<HTMLInputElement>document.getElementById('deletebtn')).style.display = 'none';
    (<HTMLInputElement>document.getElementById('description')).value = "";
    (<HTMLInputElement>document.getElementById('defaultvalue')).value = "";
    (<HTMLInputElement>document.getElementById('attributename')).value = "";
    (<HTMLInputElement>document.getElementById('displayname')).value = "";
    this.assignDefaults();
    // (<HTMLSelectElement>document.getElementById('resourcetypeidsel')).selectedIndex = 0;
    this.attrForm.resetForm();
  }
  addAttribute(attrForm: NgForm) {

    let inputJson = {
      "resourcetypeid": attrForm.value.resourcetypeid,
      "attributename": this.attributename,
      "description": attrForm.value.description,
      "DefaultValue":attrForm.value.defaultvalue,
      "attributetype": "varchar",
      "displayname": this.displayname
    }

    this.confService.addAttributes(JSON.stringify(inputJson)).subscribe(res => {

      this.toastr.successToastr('Attribute added successfully', 'Success!');
      (<HTMLSelectElement>document.getElementById('attrselect')).selectedIndex = 0;


      this.resetButtons();
      this.assignDefaults();
    }, error => {
      this.toastr.errorToastr("Error occured !", 'Error!');
      this.resetButtons();
    })
  }
  updateAttribute(attrForm: NgForm) {

    let inputJson = {
      "resourcetypeid": this.resourcetypeid,
      "attributename": this.attributename,
      "description": this.description.nativeElement.value,
      "DefaultValue":this.defaultValue.nativeElement.value,
      "attributetype": "varchar",
      "displayname": this.displayname
    }
    this.confService.addAttributes(JSON.stringify(inputJson)).subscribe(res => {

      this.toastr.successToastr('Attribute updated successfully', 'Success!');
      (<HTMLSelectElement>document.getElementById('attrselect')).selectedIndex = 0;
      this.resetButtons();
      this.assignDefaults();
    }, error => {
      this.toastr.errorToastr("Error occured !", 'Error!');
      // (<HTMLSelectElement>document.getElementById('attrselect')).selectedIndex = -1;

    }
    )
  }
  deleteAttribute(attrForm: NgForm) {
    let inputJson = {
      "resourcetypeid": attrForm.value.resourcetypeid,
      "attributename": (<HTMLInputElement>document.getElementById('attributename')).value,

    }
    this.confService.deleteAttributes(JSON.stringify(inputJson)).subscribe(res => {

      this.toastr.successToastr('Attribute deleted successfully', 'Success!');
      (<HTMLSelectElement>document.getElementById('attrselect')).selectedIndex = 0;

      this.resetButtons();
      this.assignDefaults();

    }, error => {
      this.toastr.errorToastr("Error occured !", 'Error!');
    })
  }
  resetAttribute(attrForm: NgForm) {

    (<HTMLSelectElement>document.getElementById('attrselect')).selectedIndex = 0;
    this.resetButtons();
    attrForm.reset();
  }
  openPopup() {
    this.childElements = [];
  }
  onChangeResPopup() {
    var resourcetypemetadata: any = (<HTMLSelectElement>document.getElementById('resourcetypeid_popup')).value;
    resourcetypemetadata = JSON.parse(resourcetypemetadata);

    try {
      this.metaConfigJson.resourcetypedetails.forEach(resource => {
        if (resource.resourcetypeid == resourcetypemetadata.key) {
          if (resource.ismainentry) {
            (<HTMLInputElement>document.getElementById("styled-checkbox-chk_ismainentryp")).checked = true
          } else {
            (<HTMLInputElement>document.getElementById("styled-checkbox-chk_ismainentryp")).checked = false
          }
        }

      });
    } catch (e) {

    }

  }
  addNewChildElements() {

    var resourcetypemetadata: any = (<HTMLSelectElement>document.getElementById('childresource_popup')).value;
    resourcetypemetadata = JSON.parse(resourcetypemetadata);

    var isExists = false;
    this.childElements.forEach(element => {
      if (element.resourcetypename == resourcetypemetadata.value) {
        isExists = true
      }
    });
    if (!isExists) {
      this.childElements.push({ "resourcetypename": resourcetypemetadata.value, "resourcetypeid": resourcetypemetadata.key })
    }

    console.log(this.childElements)
  }

  addMapping() {

    try {
      var resource: any = (<HTMLSelectElement>document.getElementById('resourcetypeid_popup')).value;
      resource = JSON.parse(resource);
      var isexists = false;


      if (resource.key != undefined) {

        var parent: any = (<HTMLSelectElement>document.getElementById('parentresource_popup')).value;
        parent = JSON.parse(parent);
        if (this.childElements.length != 0 || parent != "0") {
          if (parent != "0" && resource.value === parent.value) {
            console.log("invalid");
            this.toastr.errorToastr('Invalid mapping', 'Error!');
          } else {

            //already exists validation
            this.metaConfig[0].resourcetypedetails.forEach(obj => {

              // if ((obj.resourcetypeid == resource.key) && (obj.ismappingdeleted == false)) {

              // }

              if ((obj.resourcetypeid == resource.key) && ((obj.parentdetails != undefined && obj.parentdetails.length > 0) || (obj.childdetails != undefined && obj.childdetails.length > 0))) {
                isexists = true;
              }
            });

            if (!isexists) {

              if (this.childElements.length > 0 || parent.key != undefined) {

                var resourcetypedetail: Resourcetypedetail = {};
                resourcetypedetail.childdetails = this.childElements;
                if (parent.key != undefined) {
                  resourcetypedetail.parentdetails = [{
                    resourcetypeid: parent.key,
                    resourcetypename: parent.value
                  }]
                }
                resourcetypedetail.ismainentry = false;
                resourcetypedetail.ismappingdeleted = false;
                resourcetypedetail.resourcetypename = resource.value;
                resourcetypedetail.resourcetypeid = resource.key

              }

              // 
              var metadata: Resourcetypemetadata[] = [];
              // this.metaConfig[0].resourcetypedetails.forEach(obj => {

              //   if (obj.resourcetypeid == resource.key) {
              //     metadata = obj.resourcetypemetadata;
              //   }
              // });
              var ismainentry;
              if ((<HTMLInputElement>document.getElementById('styled-checkbox-chk_ismainentryp')).checked) {
                ismainentry = true;
              } else {
                ismainentry = false;
              }
              resourcetypedetail.resourcetypemetadata = metadata;
              // this.metaConfig[0].resourcetypedetails.push(resourcetypedetail);
              this.metaConfigJson.resourcetypedetails.forEach(obj => {

                if ((obj.resourcetypeid == resource.key)) {
                  if (this.childElements.length > 0 || parent.key != undefined) {
                    try {
                      obj.parentdetails = resourcetypedetail.parentdetails;

                    } catch (e) {

                    }
                    try {

                      obj.childdetails = resourcetypedetail.childdetails;
                    } catch (e) {

                    }
                    obj.ismainentry = ismainentry;
                    obj.ismappingdeleted = false;
                    obj.isnewmapping = true;
                    obj.isVisible = true;
                  } else {
                    obj.ismainentry = ismainentry;
                    obj.ismappingdeleted = false;
                    obj.isnewmapping = true;
                    obj.isVisible = true;
                  }



                  // obj.parentdetails = resourcetypedetail.parentdetails;
                  // obj.childdetails = resourcetypedetail.childdetails;
                }
              });

              this.closeModal.nativeElement.click();
            } else {
              this.toastr.errorToastr('Mapping already exists!', 'Error!');
            }
          }

        }





      }
    } catch (e) {

    }

  }
}

