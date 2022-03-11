export class Configuration{
   
    platformid?:string;
    tenantid?:string;
    platformtype?:string;
    resourcetypedetails?:ResourceTypes[];
   
}



// export class Attributes{
//     attributename?:string
//     attributetype?:string	
// }

export class ResourceTypes{
    resourcetypename?:string
    resourcetypeid?:string           
    details: ResourceDetails[];
}
export class ResourceDetails{
    resourcename?:string
    resourceid?:string 
    
}
// export class childdetails{
//     resourcetypename?:string
//     resourcetypeid?:string           
//     resourcetypemetadata?:Attributes[];
// }



// export interface Childdetail {
//     resourcetypename: string;
//     resourcetypeid: string;
//     childdetails?: any;
//     resourcetypemetadata: Attributes[];
// }

// export interface Attributes {
//     attributename: string;
//     attributetype: string;
// }

// export interface Resourcetypedetail {
//     resourcetypename: string;
//     resourcetypeid: string;
//     childdetails: Childdetail[];
//     resourcetypemetadata: Attributes[];
//     parentdetails?: any;
// }

// export interface RootObject {
//     platformid: string;
//     tenantid: string;
//     platformtype: string;
//     defaultmetadata: Attributes[];
//     resourcetypedetails: Resourcetypedetail[];
// }
