﻿
import * as React from 'react'
import { Route } from 'react-router'
import { Dic, classes } from '../../../Framework/Signum.React/Scripts/Globals';
import { Button, OverlayTrigger, Tooltip, MenuItem } from "react-bootstrap"
import { ajaxPost, ajaxGet } from '../../../Framework/Signum.React/Scripts/Services';
import { EntitySettings } from '../../../Framework/Signum.React/Scripts/Navigator'
import * as Navigator from '../../../Framework/Signum.React/Scripts/Navigator'
import { Lite, Entity, EntityPack, ExecuteSymbol, DeleteSymbol, ConstructSymbol_From } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import { EntityOperationSettings } from '../../../Framework/Signum.React/Scripts/Operations'
import { PseudoType, QueryKey, GraphExplorer, OperationType  } from '../../../Framework/Signum.React/Scripts/Reflection'
import * as Operations from '../../../Framework/Signum.React/Scripts/Operations'
import * as ContextualOperations from '../../../Framework/Signum.React/Scripts/Operations/ContextualOperations'
import { ProcessState, ProcessEntity, ProcessPermission, PackageLineEntity, PackageEntity, PackageOperationEntity } from './Signum.Entities.Processes'
import * as OmniboxClient from '../Omnibox/OmniboxClient'
import * as AuthClient from '../Authorization/AuthClient'

require("!style!css!./Processes.css");

export function start(options: { routes: JSX.Element[], packages: boolean, packageOperations: boolean }) {
    options.routes.push(<Route path="processes">
        <Route path="view" getComponent={(loc, cb) => require(["./ProcessPanelPage"], (Comp) => cb(null, Comp.default))}/>
    </Route>);

    Navigator.addSettings(new EntitySettings(ProcessEntity, e => new Promise(resolve => require(['./Templates/Process'], resolve))));

    if (options.packages || options.packageOperations) {
        Navigator.addSettings(new EntitySettings(PackageLineEntity, e => new Promise(resolve => require(['./Templates/PackageLine'], resolve))));
    }

    if (options.packages) {
        Navigator.addSettings(new EntitySettings(PackageEntity, e => new Promise(resolve => require(['./Templates/Package'], resolve))));
    }

    if (options.packageOperations) {
        Navigator.addSettings(new EntitySettings(PackageOperationEntity, e => new Promise(resolve => require(['./Templates/PackageOperation'], resolve))));
    }

    
    OmniboxClient.registerSpecialAction({
        allowed: () => AuthClient.isPermissionAuthorized(ProcessPermission.ViewProcessPanel),
        key: "ProcessPanel",
        onClick: () => Promise.resolve(Navigator.currentHistory.createHref("/processes/view"))
    });

    monkeyPatchCreateContextualMenuItem()

}

export var processOperationSettings :{ [key: string]: Operations.ContextualOperationSettings<any> } = {}; 
export function register<T extends Entity>(...settings : Operations.ContextualOperationSettings<T>[]){
    settings.forEach(s => Dic.addOrThrow(processOperationSettings, s.operationSymbol.key, s));
}

function monkeyPatchCreateContextualMenuItem(){

    var base = ContextualOperations.MenuItemConstructor.createContextualMenuItem;

    ContextualOperations.MenuItemConstructor.createContextualMenuItem = (coc: Operations.ContextualOperationContext<Entity>, defaultClick: (coc: Operations.ContextualOperationContext<Entity>, event: React.MouseEvent) => void, key: any) => {
        
        if(!Navigator.isViewable(PackageOperationEntity) )
            return base(coc, defaultClick, key);

        if(coc.operationInfo.operationType == OperationType.Constructor ||
            coc.operationInfo.operationType == OperationType.ConstructorFromMany)
            return base(coc, defaultClick, key);

        if(coc.context.lites.length <= 1)
            return base(coc, defaultClick, key);

        var settings = processOperationSettings[coc.operationInfo.key];

        if(settings != null){
            if(settings.isVisible && !settings.isVisible(coc))
                return base(coc, defaultClick, key);

            if(settings.hideOnCanExecute && coc.canExecute != null)
                return base(coc, defaultClick, key);
        }


         var text = coc.settings && coc.settings.text ? coc.settings.text() :
        coc.entityOperationSettings && coc.entityOperationSettings.text ? coc.entityOperationSettings.text() :
            coc.operationInfo.niceName;

        var bsStyle = coc.settings && coc.settings.style || Operations.autoStyleFunction(coc.operationInfo);

        var disabled = !!coc.canExecute;

        var onClick = coc.settings && coc.settings.onClick ?
            (me: React.MouseEvent) => coc.settings.onClick(coc, me) :
            (me: React.MouseEvent) => defaultClick(coc, me);

        var menuItem = <MenuItem
            className={classes("btn-" + bsStyle, disabled ? "disabled" : null) }
            onClick={disabled ? null : onClick}
            data-operation={coc.operationInfo.key}
            key={key}>
            {text}
            <span className="glyphicon glyphicon-cog process-contextual-icon" aria-hidden={true} onClick={me =>defaultConstructFromMany(coc, me)}></span>
            </MenuItem>;

        if (!coc.canExecute)
            return menuItem;

        const tooltip = <Tooltip id={"tooltip_" + coc.operationInfo.key.replace(".", "_") }>{coc.canExecute}</Tooltip>;

        return <OverlayTrigger placement="right" overlay={tooltip} >{menuItem}</OverlayTrigger>;


    };
}

function defaultConstructFromMany(coc: Operations.ContextualOperationContext<Entity>, event: React.MouseEvent) {

    event.preventDefault();
    event.stopPropagation();

    if (!ContextualOperations.confirmInNecessary(coc))
        return;

    API.processFromMany<Entity>(coc.context.lites, coc.operationInfo.key).then(pack => {

        if (!pack || !pack.entity)
            return;

        var es = Navigator.getSettings(pack.entity.Type);
        if (es.avoidPopup || event.ctrlKey || event.button == 1) {
            Navigator.currentHistory.pushState(pack, '/Create/');
            return;
        }
        else {
            Navigator.navigate({
                entityOrPack: pack,
            });
        }
    }).done();
}

export module API {

    export function processFromMany<T extends Entity>(lites: Lite<T>[], operationKey: string | ExecuteSymbol<T> | DeleteSymbol<T> | ConstructSymbol_From<any, T>, args?: any[]): Promise<EntityPack<ProcessEntity>> {
        GraphExplorer.propagateAll(lites, args);
        return ajaxPost<EntityPack<ProcessEntity>>({ url: "/api/processes/constructFromMany" }, { lites: lites, operationKey: Operations.API.getOperationKey(operationKey), args: args } as Operations.API.MultiOperationRequest);
    }

    export function start(): Promise<void> {
        return ajaxPost<void>({ url: "/api/processes/start" }, null);
    }

    export function stop(): Promise<void> {
        return ajaxPost<void>({ url: "/api/processes/stop" }, null);
    }

    export function view(): Promise<ProcessLogicState> {
        return ajaxGet<ProcessLogicState>({ url: "/api/processes/view" });
    }
}


export interface ProcessLogicState {
    MaxDegreeOfParallelism: number;
    InitialDelayMiliseconds: number;
    Running: boolean;
    MachineName: string;
    JustMyProcesses: boolean;
    NextPlannedExecution: string;
    Executing: ExecutionState[];
}

export interface ExecutionState {
    Process: Lite<ProcessEntity>;
    State: ProcessState;
    IsCancellationRequested: boolean;
    Progress: number;
    MachineName: string;
    ApplicationName: string;
}