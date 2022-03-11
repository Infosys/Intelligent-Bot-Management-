export class ActList {
        actionid: number;
        actionname: string;
        actiontypeid: number;
        endpointuri: string;
        scriptid: number;
        categoryid: number;
        automationengineid: number;
        createdby: string;
        ModifiedBy: string;
        CreateDate: Date;
        ModifiedDate: Date;
        ValidityStart: Date;
        ValidityEnd: Date;
    }

    export class actionModel {
        tenantID: number;
        actList: ActList[];
    }



