export class RemediationDetails {
    remediationId: number;
    remediationPlanName: string;
    RemediationPlanDescription: string;
    IsUserDefined: boolean;
    ActionDetails?: ActionDetail[];
}


export class ActionDetail {
    ActionId: number;
    ActionSequence: string;
    ActionStageId: string;
    ActionName: string;
    RemediationPlanActionId: number;
    isDeleted?:boolean;
}




