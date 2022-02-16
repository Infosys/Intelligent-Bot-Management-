export class AcionScript {
    ArgString?: string;
    BelongsToAccount?: string;
    BelongsToOrg?: string;
    BelongsToTrack?: string;
    CallMethod?: any;
    CategoryId?: number;
    CreatedBy?: string;
    CreatedOn?: Date;
    Description?: string;
    ExternalReferences?: string;
    IfeaScriptName?: any;
    IsDeleted?: boolean;
    LicenseType?: any;
    ModifiedBy?: string;
    ModifiedOn?: Date;
    Name?: string;
    Params?: Params[];
    RunAsAdmin: boolean;
    ScriptContent?: any;
    ScriptFileVersion?: number;
    ScriptId?: number;
    ScriptType?: string;
    ScriptURL?: string;
    SourceUrl?: any;
    StorageBaseUrl?: any;
    Tags?: string;
    TaskCmd?: any;
    TaskType?: string;
    UsesUIAutomation?: boolean;
    WorkingDir?: string;
}
export class Params {
    AllowedValues: string;
    CreatedBy: string;
    DataType?: any;
    DefaultValue: string;
    IsDeleted: boolean;
    IsMandatory: boolean;
    IsReferenceKey: boolean;
    IsSecret: boolean;
    IsUnnamed: boolean;
    ModifiedBy: string;
    Name: string;
    ParamId: number;
    ParamType: number;
    ScriptId: number;
}