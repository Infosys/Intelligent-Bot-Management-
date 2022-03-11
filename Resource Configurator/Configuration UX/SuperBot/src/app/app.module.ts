import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { AppRoutingModule } from './app-routing.module';
import { ConfigurationComponent } from './configuration/configuration.component';
import {ConfigurationService} from '../app/services/configuration.service';
import {PropertiesService} from '../app/services/properties.service';
import { SidebarComponent } from './sidebar/sidebar.component';
import { HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from './loader/loader.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularDateTimePickerModule } from 'angular2-datetimepicker';
import { ToastrModule } from 'ng6-toastr-notifications';
import { MetaConfigurationComponent } from './meta-configuration/meta-configuration.component';
import { SummaryComponent } from './configuration/summary/summary.component';
import { ObservableplanComponent } from './observableplan/observableplan.component';
import { ActionConfigurationComponent } from './action-configuration/action-configuration.component';
import { ObservableConfigComponent } from './observable-config/observable-config.component';
import { RemediationConfigComponent } from './remediation-config/remediation-config.component';
import { RemediationPlanComponent } from './remediation-plan/remediation-plan.component';
import { AnomalyConfigComponent } from './anomaly-config/anomaly-config.component';


import { Ng2SmartTableModule } from './ng2-smart-table/ng2-smart-table.module';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { PlatformDetailsComponent } from './platform-details/platform-details.component';
import { AutoConfigurationComponent } from './auto-configuration/auto-configuration.component';
import { TreeModule } from 'angular-tree-component';
import { ActivateResourceComponent } from './configuration/activate-resource/activate-resource.component';
@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    ConfigurationComponent,
    SidebarComponent,
    LoaderComponent,
    MetaConfigurationComponent,
    SummaryComponent,
    ObservableplanComponent,
    ActionConfigurationComponent,
    ObservableConfigComponent,
    RemediationConfigComponent,
    RemediationPlanComponent,
    AnomalyConfigComponent,
    PlatformDetailsComponent,
    AutoConfigurationComponent,
    ActivateResourceComponent,
    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
     Ng2SmartTableModule,
    ToastrModule.forRoot(),
    AngularDateTimePickerModule,
    NgMultiSelectDropDownModule.forRoot(),
    TreeModule.forRoot()


  ],
  providers: [{
    provide:APP_INITIALIZER,
    useFactory:(setting:PropertiesService) =>  function(){
      return  setting.getProperties()
    },
    deps:[PropertiesService],
    multi:true
  }],
  // providers:[PropertiesService],
  bootstrap: [AppComponent]
})
export class AppModule { }
