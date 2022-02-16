export class MetaConfig {
    platformid?: string;
    tenantid?: string;
    platformtype?: string;
    resourcetypedetails?: Resourcetypedetail[];
}

export class Parentdetail {
    resourcetypename: string;
    resourcetypeid: string;
}

export class Childdetail {
    resourcetypename: string;
    resourcetypeid: string;
}

export class Resourcetypemetadata {
    attributename: string;
    attributetype: string;
    description: string;
    displayname: string;
    ismandatory: boolean;
    Sequence: string;
    DefaultValue?:string;
    isSecret?:boolean
}

export class Logdetails {
    CreatedBy: string;
    ModifiedBy: string;
    CreateDate: Date;
    ModifiedDate: Date;
    ValidityStart: Date;
    ValidityEnd: Date;
}

export class Resourcetypedetail {

    isnewmapping?:boolean;
    resourcetypename?: string;
    resourcetypeid?: string;
    ismainentry?: boolean;
    portfolioid?:string;
    ismappingdeleted?:boolean;
    isVisible?:boolean;
    parentdetails?: Parentdetail[];
    childdetails?: Childdetail[];
    resourcetypemetadata?: Resourcetypemetadata[];
    logdetails?: Logdetails;
}

