
export class ActionConfig {
    tenantid: string;
    actiondetails: Actiondetail[];
}
export class Actionparam {
    name?: string;
    fieldtomap?: string;
    ismandatory?: string;
    defaultvalue?: any;
    automationengineparamid?: string;
    isdeleted?:boolean;
    
}

export class Actiondetail {
    actionid?: number;
    automationenginename?:string;
    actionname?: string;
    actiontypeid?: number;
    actiontype?: string;
    endpointuri?: any;
    scriptid?: number;
    categoryid?: number;
    categoryname?:string;
    automationengineid?: number;
    createdby?: string;
    ModifiedBy?: string;
    CreateDate?: string;
    ModifiedDate?: string;
    ValidityStart?: Date;
    ValidityEnd?: Date;
    actionparams?: Actionparam[];
}

