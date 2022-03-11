  export class ObsList {
        observableid: number;
        observablename: string;
        unitofmeasure: string;
        createdby: string;
        ModifiedBy: string;
        CreateDate: Date;
        ModifiedDate: Date;
        ValidityStart: Date;
        ValidityEnd: Date;
        datatype: string;
    }

    export class Obs {
        tenentId: number;
        obsList: ObsList[];
    }

