

export interface Resource {
    platformid: string;
    tenantid: string;
    platformtype: string;
    resourcemodelversion: string;
    status?: string;
    resourcedetails?: Resourcedetail[];
    observablesandremediationplans?: Observablesandremediationplan[];
    logdetails?: Logdetails;
}

export class Resourcedetail {
    resourcename?: string;
    resourceid?: string;
    resourcetypename?: string;
    resourcetypeid?: string;
    dontmonitor?:boolean;
    cascadetochild?:boolean
    source?:string;
    startdate?:string;
    portfolioid?:string;
    enddate?:string;
    parentdetails?: Parentdetail[];
    logdetails?: any;
    resourceattribute?: Resourceattribute[];
    childdetails?: ChildDetails[];
    observablesandremediations?: Observablesandremediation[];
}

export class ChildDetails {
    resourcename?: string;
    resourceid?: string;
    resourcetypename?: string;
    resourcetypeid?: string;
    dontmonitor?:boolean;
    source?:string;
    startdate?:string;
    enddate?:string;
    portfolioid?:string;
    resourceattribute?: Resourceattribute[];
    observablesandremediations?: Observablesandremediation[];
}

export interface Observablesandremediation {
    name?: string;
    ObservableId?: string;
    ObservableName?: string;
    ObservableActionId?: string;
    ObservableActionName?: string;
    RemediationPlanId?: string;
    RemediationPlanName?: string;
    isObsSelected?:boolean;
    isRemSelected?:boolean;
    ismodified?:boolean;
}

export class Observablesandremediationplan {
    resourcetypename?: string;
    resourcetypeid?: string;
    observablesandremediations?: Observablesandremediation[];
}

export class Parentdetail {
    resourceid: string;
    resourcename: string;
    resourcetypename: string;
    resourcetypeid: string;
}

export class Resourceattribute {
    attributename: string;
    attributevalue: string;
    displayname?: string;
    description?:string
    IsSecret?:boolean
 

}





export class Logdetails {
    CreatedBy: string;
    ModifiedBy: string;
    CreateDate: string;
    ModifiedDate: string;
    ValidityStart: string;
    ValidityEnd: string;
}

