import { Component, OnInit, ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {

  constructor(private ref: ChangeDetectorRef) {

  }
  liParent: string;
  lichild: string;
  ResourceTypeName: string;
  PlatformId: string;
  count: number;
  showAutoConfig: boolean;
  actionId:string;
  ngOnInit() {
    this.showAutoConfig = true;
    this.count = 0;
    this.actionId="";
    this.liParent = "Resource Model";
    this.lichild = "Resource Model"
  }
  receiveMessage(event: any) {
    console.log(event);
    this.count = event.count;
    this.PlatformId = event.PlatFormId;
    this.ResourceTypeName = event.ResourceTypeName;
    console.log(event.count)
    console.log(this.count);
    setInterval(() => {
      this.ref.detectChanges();
    }, 5);

  }
loadAction(event: any){
  this.actionId=event;
  this.liParent = "Configuration";
  this.lichild = 'Action Configuration';

}
  platformDetails(event: any) {
    this.liParent = 'Resource Model';
    this.lichild = 'Resource Model';
    this.showAutoConfig = false;
    console.log(event);
    this.count = event.count;
    this.PlatformId = event.PlatFormId;
    this.ResourceTypeName = event.ResourceTypeName;
    console.log(event.count)
    console.log(this.count);
    setInterval(() => {
      this.ref.detectChanges();
    }, 5);

  }
  onSelect(parent: string, child: string) {
    this.count = 0;
    this.liParent = parent;
    this.lichild = child
    if(this.lichild == "Auto Configuration"){
      this.showAutoConfig = true;
    }
  }
}
