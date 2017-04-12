﻿/// <reference path="../bpmn-js.d.ts" />
import Modeler = require("bpmn-js/lib/Modeler");
import BpmnReplaceMenuProvider = require("bpmn-js/lib/features/popup-menu/ReplaceMenuProvider");
import {  } from '../Signum.Entities.Workflow'
import * as BpmnUtils from './BpmnUtils'
import { Lite, liteKey } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'

interface ReplaceOptions {
    actionName: string;
    className: string;
    label: string;
    target: BPMN.DiElement;
}

export class CustomReplaceMenuProvider extends BpmnReplaceMenuProvider {

    static $inject = ['popupMenu', 'modeling', 'moddle', 'bpmnReplace', 'rules', 'translate'];
    constructor(popupMenu: any, modeling: any, moddle: BPMN.ModdleElement, bpmnReplace: any, rules: any, translate: any) {
        super(popupMenu, modeling, moddle, bpmnReplace, rules, translate);
    }

    getHeaderEntries(element: BPMN.DiElement) {
        return [];
    }

    _createEntries(element: BPMN.DiElement, replaceOptions: ReplaceOptions[]) {

        debugger;
        if (BpmnUtils.isGatewayAnyKind(element.type))
            return super._createEntries(element, replaceOptions.filter(a =>
                a.actionName == "replace-with-parallel-gateway" ||
                a.actionName == "replace-with-inclusive-gateway" ||
                a.actionName == "replace-with-exclusive-gateway"));

        return [];
    }
}

export var __init__ = ['customReplaceMenuProvider'];
export var customReplaceMenuProvider = ['type', CustomReplaceMenuProvider];