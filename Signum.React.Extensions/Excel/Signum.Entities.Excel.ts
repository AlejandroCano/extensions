//////////////////////////////////
//Auto-generated. Do NOT modify!//
//////////////////////////////////
import { MessageKey, QueryKey, Type, EnumType, registerSymbol } from '../../../Framework/Signum.React/Scripts/Reflection' 

import * as Entities from '../../../Framework/Signum.React/Scripts/Signum.Entities' 

import * as Files from '../Files/Signum.Entities.Files' 

import * as UserQueries from '../UserQueries/Signum.Entities.UserQueries' 

import * as Mailing from '../Mailing/Signum.Entities.Mailing' 


export const ExcelAttachmentEntity_Type = new Type<ExcelAttachmentEntity>("ExcelAttachment");
export interface ExcelAttachmentEntity extends Entities.Entity, Mailing.IAttachmentGeneratorEntity {
    fileName?: string;
    userQuery?: Entities.Lite<UserQueries.UserQueryEntity>;
    related?: Entities.Lite<Entities.Entity>;
}

export module ExcelAttachmentOperation {
    export const Save : Entities.ExecuteSymbol<ExcelAttachmentEntity> = registerSymbol({ Type: "Operation", key: "ExcelAttachmentOperation.Save" });
}

export module ExcelMessage {
    export const Data = new MessageKey("ExcelMessage", "Data");
    export const Download = new MessageKey("ExcelMessage", "Download");
    export const Excel2007Spreadsheet = new MessageKey("ExcelMessage", "Excel2007Spreadsheet");
    export const Administer = new MessageKey("ExcelMessage", "Administer");
    export const ExcelReport = new MessageKey("ExcelMessage", "ExcelReport");
    export const ExcelTemplateMustHaveExtensionXLSXandCurrentOneHas0 = new MessageKey("ExcelMessage", "ExcelTemplateMustHaveExtensionXLSXandCurrentOneHas0");
    export const FindLocationFoExcelReport = new MessageKey("ExcelMessage", "FindLocationFoExcelReport");
    export const Reports = new MessageKey("ExcelMessage", "Reports");
    export const TheExcelTemplateHasAColumn0NotPresentInTheFindWindow = new MessageKey("ExcelMessage", "TheExcelTemplateHasAColumn0NotPresentInTheFindWindow");
    export const ThereAreNoResultsToWrite = new MessageKey("ExcelMessage", "ThereAreNoResultsToWrite");
    export const CreateNew = new MessageKey("ExcelMessage", "CreateNew");
}

export const ExcelReportEntity_Type = new Type<ExcelReportEntity>("ExcelReport");
export interface ExcelReportEntity extends Entities.Entity {
    query?: Entities.Basics.QueryEntity;
    displayName?: string;
    file?: Files.EmbeddedFileEntity;
}

export module ExcelReportOperation {
    export const Save : Entities.ExecuteSymbol<ExcelReportEntity> = registerSymbol({ Type: "Operation", key: "ExcelReportOperation.Save" });
    export const Delete : Entities.DeleteSymbol<ExcelReportEntity> = registerSymbol({ Type: "Operation", key: "ExcelReportOperation.Delete" });
}
