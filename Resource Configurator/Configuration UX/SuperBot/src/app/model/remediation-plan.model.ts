export class RemediationPlan {
    resourcetypename: string;
    resourcetypeid: number;
    ObservableId: number;
    ObservableName: string;
    RemediationPlanId: number;
    RemediationPlanName: string;
    CreatedBy?: string;
    ModifiedBy?: string;
    CreateDate?: Date;
    ModifiedDate?: Date;
    ValidityStart: Date;
    ValidityEnd: Date;
}