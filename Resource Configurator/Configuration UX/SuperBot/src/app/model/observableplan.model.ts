 export class Observableresourcetypeaction {
        name: string;
        resourcetypename: string;
        resourcetypeid: number;
        observableid: number;
        observablename: string;
        actionid: number;
        actionname: string;
        CreatedBy: string;
        ModifiedBy: string;
        CreateDate: Date;
        ModifiedDate: Date;
        ValidityStart: Date;
        ValidityEnd: Date;
    }

    export class ObservablePlan {
        PlatformID: number;
        tenantID: number;
        observableresourcetypeactions: Observableresourcetypeaction[];
    } 
 
