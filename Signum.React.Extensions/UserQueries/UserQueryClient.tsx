﻿import * as React from 'react'
import { Route } from 'react-router'
import { ajaxPost, ajaxGet } from '../../../Framework/Signum.React/Scripts/Services';
import { EntitySettings, ViewPromise } from '../../../Framework/Signum.React/Scripts/Navigator'
import * as Navigator from '../../../Framework/Signum.React/Scripts/Navigator'
import * as Finder from '../../../Framework/Signum.React/Scripts/Finder'
import { EntityOperationSettings } from '../../../Framework/Signum.React/Scripts/Operations'
import { Entity, Lite, liteKey } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import * as Constructor from '../../../Framework/Signum.React/Scripts/Constructor'
import * as Operations from '../../../Framework/Signum.React/Scripts/Operations'
import * as QuickLinks from '../../../Framework/Signum.React/Scripts/QuickLinks'
import { FindOptionsParsed, FindOptions, FilterOption, FilterOperation, OrderOption, ColumnOption, FilterRequest, QueryRequest, Pagination } from '../../../Framework/Signum.React/Scripts/FindOptions'
import * as AuthClient  from '../../../Extensions/Signum.React.Extensions/Authorization/AuthClient'
import { UserQueryEntity, UserQueryPermission, UserQueryMessage,
    QueryFilterEmbedded, QueryColumnEmbedded, QueryOrderEmbedded } from './Signum.Entities.UserQueries'
import { QueryTokenEmbedded } from '../UserAssets/Signum.Entities.UserAssets'
import UserQueryMenu from './UserQueryMenu'
import * as UserAssetsClient from '../UserAssets/UserAssetClient'
import { ImportRoute } from "../../../Framework/Signum.React/Scripts/AsyncImport";

export function start(options: { routes: JSX.Element[] }) {

    UserAssetsClient.start({ routes: options.routes });
    UserAssetsClient.registerExportAssertLink(UserQueryEntity);

    options.routes.push(<ImportRoute path="~/userQuery/:userQueryId/:entity?" onImportModule={() => _import("./Templates/UserQueryPage")} />);

    Finder.ButtonBarQuery.onButtonBarElements.push(ctx => {
        if (!ctx.searchControl.props.showBarExtension || !AuthClient.isPermissionAuthorized(UserQueryPermission.ViewUserQuery))
            return undefined;

        return <UserQueryMenu searchControl={ctx.searchControl}/>;
    }); 

    QuickLinks.registerGlobalQuickLink(ctx => {
        if (!AuthClient.isPermissionAuthorized(UserQueryPermission.ViewUserQuery))
            return undefined;

        return API.forEntityType(ctx.lite.EntityType).then(uqs =>
            uqs.map(uq => new QuickLinks.QuickLinkAction(liteKey(uq), uq.toStr || "", e => {
                window.open(Navigator.toAbsoluteUrl(`~/userQuery/${uq.id}/${liteKey(ctx.lite)}`));
            }, { icon: "glyphicon glyphicon-list-alt", iconColor: "dodgerblue" })));
    });

    QuickLinks.registerQuickLink(UserQueryEntity, ctx => new QuickLinks.QuickLinkAction("preview", UserQueryMessage.Preview.niceToString(),
        e => {
            Navigator.API.fetchAndRemember(ctx.lite).then(uq => {
                if (uq.entityType == undefined)
                    window.open(Navigator.toAbsoluteUrl(`~/userQuery/${uq.id}`));
                else
                    Navigator.API.fetchAndForget(uq.entityType)
                        .then(t => Finder.find({ queryName: t.cleanName }))
                        .then(lite => {
                            if (!lite)
                                return;

                            window.open(Navigator.toAbsoluteUrl(`~/userQuery/${uq.id}/${liteKey(lite)}`));
                        })
                        .done();
            }).done();
        }, { isVisible: AuthClient.isPermissionAuthorized(UserQueryPermission.ViewUserQuery) }));

    Constructor.registerConstructor<QueryFilterEmbedded>(QueryFilterEmbedded, () => QueryFilterEmbedded.New({ token: QueryTokenEmbedded.New() }));
    Constructor.registerConstructor<QueryOrderEmbedded>(QueryOrderEmbedded, () => QueryOrderEmbedded.New({token : QueryTokenEmbedded.New() }));
    Constructor.registerConstructor<QueryColumnEmbedded>(QueryColumnEmbedded, () => QueryColumnEmbedded.New({ token : QueryTokenEmbedded.New() }));

    Navigator.addSettings(new EntitySettings(UserQueryEntity, e => _import('./Templates/UserQuery'), { isCreable: "Never" }));
}


export module Converter {

    export function toFindOptions(uq: UserQueryEntity, entity: Lite<Entity> | undefined): Promise<FindOptions> {

        var query = uq.query!;

        var fo = { queryName: query.key } as FindOptions;

        const convertedFilters = uq.withoutFilters ? Promise.resolve([] as FilterRequest[]) : UserAssetsClient.API.parseFilters({
            queryKey: query.key,
            canAggregate: false,
            entity: entity,
            filters: uq.filters!.map(mle => mle.element).map(f => ({
                tokenString: f.token!.tokenString,
                operation: f.operation,
                valueString: f.valueString
            }) as UserAssetsClient.API.ParseFilterRequest)
        });

        return convertedFilters.then(filters => {

            if (!uq.withoutFilters && filters) {
                fo.filterOptions = (fo.filterOptions || []).filter(f => f.frozen);
                fo.filterOptions.push(...filters.map(f => ({
                    columnName: f.token,
                    operation: f.operation,
                    value: f.value,
                    frozen: false
                }) as FilterOption));
            }

            fo.columnOptionsMode = uq.columnsMode;

            fo.columnOptions = (uq.columns || []).map(f => ({
                columnName: f.element.token!.tokenString,
                displayName: f.element.displayName
            }) as ColumnOption);

            fo.orderOptions = (uq.orders || []).map(f => ({
                columnName: f.element.token!.tokenString,
                orderType: f.element.orderType
            }) as OrderOption);


            const qs = Finder.querySettings[query.key];

            fo.pagination = uq.paginationMode == undefined ?
                ((qs && qs.pagination) || Finder.defaultPagination) : {
                    mode: uq.paginationMode,
                    currentPage: uq.paginationMode == "Paginate" ? 1 : undefined,
                    elementsPerPage: uq.paginationMode == "All" ? undefined : uq.elementsPerPage,
                } as Pagination;

            return fo;
        });
    }

    export function applyUserQuery(fop: FindOptionsParsed, uq: UserQueryEntity, entity: Lite<Entity> | undefined): Promise<FindOptionsParsed> {
        return toFindOptions(uq, entity)
            .then(fo => Finder.getQueryDescription(fo.queryName).then(qd => Finder.parseFindOptions(fo, qd)))
            .then(fop2 => {
                fop.filterOptions = fop.filterOptions.filter(a => a.frozen);
                fop.filterOptions.push(...fop2.filterOptions);
                fop.orderOptions = fop2.orderOptions;
                fop.columnOptions = fop2.columnOptions;
                fop.pagination = fop2.pagination;
                return fop;
            });
    }
}

export module API {
    export function forEntityType(type: string): Promise<Lite<UserQueryEntity>[]> {
        return ajaxGet<Lite<UserQueryEntity>[]>({ url: "~/api/userQueries/forEntityType/" + type });
    }

    export function forQuery(queryKey: string): Promise<Lite<UserQueryEntity>[]> {
        return ajaxGet<Lite<UserQueryEntity>[]>({ url: "~/api/userQueries/forQuery/" + queryKey });
    }

    export function fromQueryRequest(request: { queryRequest: QueryRequest; defaultPagination: Pagination}): Promise<UserQueryEntity> {
        return ajaxPost<UserQueryEntity>({ url: "~/api/userQueries/fromQueryRequest/" }, request);
    }
}
