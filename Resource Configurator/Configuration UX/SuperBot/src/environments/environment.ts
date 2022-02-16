// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  server_address: 'superbot_serverName',
  services_app_name: 'ConfigManagement',
  perPage: 5,
  tenantId:"1",
  automation_engine:[{"key":"Script Execution engine","value":"Script Execution engine"}],
  category_service:[{"key":"iapwemservices/WEMCommonService.svc/GetAllCategoriesByCompany?companyId=1&module=2","value":"iapwemservices/WEMCommonService.svc/GetAllCategoriesByCompany?companyId=1&module=2"}],
  script_detail_service:[{"key":"iapwemservices/WEMScriptService.svc/GetAllScriptDetails/","value":"iapwemservices/WEMScriptService.svc/GetAllScriptDetails/"}]

};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
