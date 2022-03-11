export class Logdetails {
    CreatedBy: string;
    ModifiedBy: string;
    CreateDate: string;
    ModifiedDate: string;
    ValidityStart: string;
    ValidityEnd: string;
}

export class AnomolyConfig {
    ResourceId?: string;
    ResourceName?: string;
    ResourceTypeId?: string;
    ResourceTypeName?: string;
    ObservableId?: string;
    ObservableName?: string;
    OperatorId?: string;
    Operator?: string;
    LowerThreshold?: any;
    UpperThreshold?: string;
    logdetails?: Logdetails;
}