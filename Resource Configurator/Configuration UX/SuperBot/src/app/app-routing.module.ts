import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ConfigurationComponent } from './configuration/configuration.component';
import { SidebarComponent } from '../app/sidebar/sidebar.component';
import { MetaConfigurationComponent } from '../app/meta-configuration/meta-configuration.component';
const routes: Routes = [

{ path: '', redirectTo: '/home', pathMatch: 'full' },
{ path: 'configuration', component:ConfigurationComponent  },
{ path: 'meta', component:MetaConfigurationComponent  },
{ path: 'home', component:SidebarComponent  },
{ path: '**', redirectTo: 'home' }
  
];



@NgModule({
  imports: [RouterModule.forRoot(routes,{useHash: true})],
  exports: [RouterModule]
})
export class AppRoutingModule {


}
