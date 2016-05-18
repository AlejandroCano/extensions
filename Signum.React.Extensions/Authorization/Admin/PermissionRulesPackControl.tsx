﻿import * as React from 'react'
import { Button } from 'react-bootstrap'
import { Link } from 'react-router'
import * as numbro from 'numbro'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import EntityLink from '../../../../Framework/Signum.React/Scripts/SearchControl/EntityLink'
import { TypeContext, ButtonsContext, RenderButtonsComponent } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import { EntityLine, ValueLine } from '../../../../Framework/Signum.React/Scripts/Lines'

import { QueryDescription, SubTokensOptions } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { getQueryNiceName, PropertyRoute, getTypeInfos } from '../../../../Framework/Signum.React/Scripts/Reflection'
import { ModifiableEntity, EntityControlMessage, Entity, parseLite, getToString, JavascriptMessage } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { Api } from '../AuthClient'
import { PermissionRulePack, AuthAdminMessage, PermissionSymbol, AuthMessage } from '../Signum.Entities.Authorization'




export default class PermissionRulesPackControl extends React.Component<{ ctx: TypeContext<PermissionRulePack> }, void> implements RenderButtonsComponent {
    
    handleSaveClick = (bc: ButtonsContext) => {
        var pack = this.props.ctx.value;

        Api.savePermissionRulePack(pack)
            .then(() => Api.fetchPermissionRulePack(pack.role.id))
            .then(newPack => bc.frame.onReload({ entity: newPack, canExecute: null }))
            .done();
    }

    renderButtons(bc: ButtonsContext) {
        return [
            <Button bsStyle="primary" onClick={() => this.handleSaveClick(bc) }>{AuthMessage.Save.niceToString() }</Button>
        ];
    }


    render() {

        var ctx = this.props.ctx;

        return (
            <div>
                <div className="form-compact">
                    <EntityLine ctx={ctx.subCtx(f => f.role) }  />
                    <ValueLine ctx={ctx.subCtx(f => f.strategy) }  />
                </div>
                <table className="sf-auth-rules">
                    <thead>
                        <tr>
                            <th>
                                { PermissionSymbol.niceName()}
                            </th>
                            <th>
                                {AuthAdminMessage.Allow.niceToString() }
                            </th>
                            <th>
                                {AuthAdminMessage.Deny.niceToString() }
                            </th>
                            <th>
                                {AuthAdminMessage.Overriden.niceToString() }
                            </th>
                        </tr>
                    </thead>
                    { ctx.mlistItemCtxs(a => a.rules).map(c => 
                        <tr>
                            <td>
                                {c.value.resource.key}                                
                            </td>
                            <td>
                                <ColorRadio checked={c.value.allowed} color="green" onClicked={a => { c.value.allowed = true; this.forceUpdate() } }/>                     
                            </td>
                            <td>
                                <ColorRadio checked={!c.value.allowed} color="red" onClicked={a => { c.value.allowed = false; this.forceUpdate() } }/>  
                            </td>
                            <td>
                                <GrayCheckbox checked={c.value.allowed == c.value.allowedBase}/>
                            </td>
                        </tr>
                        )
                    }
                </table>

            </div>
        );
    }
}

class ColorRadio extends React.Component<{ checked: boolean, onClicked: (e: React.MouseEvent) => void, color: string }, void>{

    render() {
        return (
            <a href="#" onClick={e => { e.preventDefault(); this.props.onClicked(e); } }
                className={this.props.checked ? "fa fa-dot-circle-o" : "fa fa-circle-o"}
                style={{ color: this.props.checked ? this.props.color : "#eee" }}>
            </a>
        );
    }
}

class GrayCheckbox extends React.Component<{ checked: boolean }, void>{

    render() {
        return (
            <i>
                className={this.props.checked ? "fa fa-check-square-o" : "fa fa-square-o"}
                style={{ color: "#eee" }}>
            </i>
        );
    }
}




