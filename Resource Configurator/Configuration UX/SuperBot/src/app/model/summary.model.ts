
export class ResourceSummary {
    tenantid: string;
    platformid: string;
    platformname: string;
    resourcemodeldetails?: Resourcemodeldetail[];
}
export class Resourcemodeldetail {
    resourcetypename: string;
    resourcetypeid: string;
    resourceid: string;
    resourcename: string;
    childdetails: Childdetail[];
    resourcedetails: Resourcedetail[];
}

export class Resourcedetail {
    resourcetypename: string;
    resourcetypeid: string;
    details: Detail[];
}

export class Detail {
    resourceid: string;
    resourcename: string;
    childdetails?: Childdetail[];
}

export class Childdetail {
    resourcetypename?: string;
    resourcetypeid?: string;
    resourcetypedetails?: Resourcetypedetail[];
}


export class Resourcetypedetail {
    resourceid: string;
    resourcename: string;
}







