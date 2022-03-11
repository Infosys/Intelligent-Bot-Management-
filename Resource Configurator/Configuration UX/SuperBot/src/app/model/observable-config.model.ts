export class ObservableConfig {
    tenantid: string;
    observableDetails: observableDetails[];
    logdetails: Logdetails;
}

export class observableDetails {
    observableid: string;
    observablename: string;
    unitofmeasure: string;
    datatype: string;
    ValidityStart: Date;
    ValidityEnd: Date;
    createdby: string;
    ModifiedBy: string;
    CreateDate: string;
    ModifiedDate: string;
}

export class Logdetails {
    ValidityStart?: any;
    ValidityEnd?: any;
}





