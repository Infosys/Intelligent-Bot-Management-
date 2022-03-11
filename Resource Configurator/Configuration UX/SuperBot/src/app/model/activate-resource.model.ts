export interface AcctivateResource {
    resourcetypename: string;
    resourcetypeid: string;
    resourcedetails: Resourcedetail[];
}

export interface Resourcedetail {
    resourcename: string;
    resourceid: string;
    isactive: boolean;
    parentdetails: Parentdetail[];
    childdetails: Childdetail[];
}
export interface Parentdetail {
    resourcetypename: string;
    resourcetypeid: string;
    resourcename: string;
    resourceid: string;
}

export interface Childdetail {
    resourcename: string;
    resourceid: string;
    isactive: boolean;
    resourcetypename: string;
    resourcetypeid: string;
}



