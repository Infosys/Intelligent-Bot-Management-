import { Component,Output,Input, OnInit,EventEmitter } from '@angular/core';
import { ResourceSummary, Resourcemodeldetail, Childdetail } from '../../model/summary.model';
import { ConfigurationService } from '../../services/configuration.service';
import { debug } from 'util';
declare var $;
@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.css']
})
export class SummaryComponent implements OnInit {

  constructor(private confService: ConfigurationService) { }
  summary: ResourceSummary;
  resourcedetails: Resourcemodeldetail;
  resourceCombo: any[];
  childdetails: Childdetail;
  @Input() PlatformId ;
  @Output() messageEvent = new EventEmitter<string>();
  ngOnInit() {
    this.childdetails = {};
    this.getSummaryDetails();

  }
  onChangeParent(name: any) {
    // 
    this.summary[0].resourcemodeldetails.forEach(res => {
      if (res.resourcename == name) {
        this.resourcedetails = res.resourcedetails;
        this.childdetails = res.childdetails;
      }
    });
  }
  sendMessage(tyepeId:any,resourceId:any,typeName:any,child:any) {
    
   console.log("tyepeId"+tyepeId);
   console.log("resourceId"+resourceId);
   console.log("typeName"+typeName);
    this.messageEvent.emit(JSON.stringify({"tyepeId":tyepeId,"resourceId":resourceId,"typeName":typeName,"child":child}));
  }
  getSummaryDetails() {

    this.confService.getSummaryDetails(this.PlatformId, 1).subscribe(res => {
      try {
        this.summary = <ResourceSummary>res;
        this.resourcedetails = this.summary[0].resourcemodeldetails[0].resourcedetails;
        this.childdetails = this.summary[0].resourcemodeldetails[0].childdetails;
        this.resourceCombo = [];
        this.summary[0].resourcemodeldetails.forEach(obj => {
          this.resourceCombo.push({ "name": obj.resourcetypename, "id": obj.resourcetypeid })

        });
      } catch (e) {

      }

    }
    )

    console.log(this.summary);


  }
  toggledetails(res, child) {
    var image = document.getElementById(res + "_" + child + 'i');
    image.classList.toggle("minus-list");

    var element = document.getElementById(res + "_" + child);

    if (element.style.display === "none") {
      element.style.display = "block";
    } else {
      element.style.display = "none";
    }

  }
}
