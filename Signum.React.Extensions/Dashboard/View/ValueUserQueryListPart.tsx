﻿
import * as React from 'react'
import { Link } from 'react-router'
import { FormGroup, FormControlStatic, ValueLine, ValueLineType, EntityLine, EntityCombo, EntityList, EntityRepeater, RenderEntity} from '../../../../Framework/Signum.React/Scripts/Lines'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import { QueryDescription, SubTokensOptions, FindOptions } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { getQueryNiceName, PropertyRoute, getTypeInfos } from '../../../../Framework/Signum.React/Scripts/Reflection'
import { ModifiableEntity, EntityControlMessage, Entity, parseLite, getToString, Lite, is, JavascriptMessage } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import * as Constructor from '../../../../Framework/Signum.React/Scripts/Constructor'
import { ValueSearchControlLine } from '../../../../Framework/Signum.React/Scripts/Search'
import { TypeContext, FormGroupStyle, mlistItemContext } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import QueryTokenEntityBuilder from '../../UserAssets/Templates/QueryTokenEntityBuilder'
import * as UserQueryClient from '../../UserQueries/UserQueryClient'
import { ValueUserQueryListPartEntity, ValueUserQueryElementEmbedded } from '../Signum.Entities.Dashboard'

export default class ValueUserQueryListPart extends React.Component<{ part: ValueUserQueryListPartEntity; entity: Lite<Entity>}, void> {
    
    render() {

        const entity = this.props.part;
        const ctx = TypeContext.root(entity, { formGroupStyle: "None" });
        return (
            <div>
                {
                    mlistItemContext(ctx.subCtx(a => a.userQueries))
                        .map((ctx, i) =>                             
                            <div key={i} >    
                                <ValueUserQueryElement ctx={ctx} entity={this.props.entity}/>    
                            </div>)
                }
            </div>
        );
    }
}

export interface ValueUserQueryElementProps {
    ctx: TypeContext<ValueUserQueryElementEmbedded>
    entity?: Lite<Entity>; 
}

export class ValueUserQueryElement extends React.Component<ValueUserQueryElementProps, {fo?: FindOptions }> {
    
    state = { fo: undefined } as { fo?: FindOptions };

    componentWillMount(){
        this.loadFindOptions(this.props);
    }

    componentWillReceiveProps(newProps : ValueUserQueryElementProps ){

        if(is(this.props.ctx.value.userQuery, newProps.ctx.value.userQuery))
            return;

        this.loadFindOptions(newProps);
    }

    loadFindOptions(props: ValueUserQueryElementProps) {

        UserQueryClient.Converter.toFindOptions(props.ctx.value.userQuery!, this.props.entity)
            .then(fo => this.setState({ fo: fo }))
            .done();
    }

    render(){

        const ctx = this.props.ctx;

        if (!this.state.fo)
            return <span>{ JavascriptMessage.loading.niceToString() }</span>;

        return (
            <div>
                <span>{ctx.value.label || getQueryNiceName(this.state.fo.queryName)}</span>&nbsp;
                <ValueSearchControlLine ctx={ctx} findOptions={this.state.fo} />
            </div>             
        );
    }   
} 



