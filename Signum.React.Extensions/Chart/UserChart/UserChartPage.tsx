﻿import * as React from 'react'
import { DropdownButton, MenuItem, Tabs, Tab} from 'react-bootstrap'
import { Dic, classes, ifError } from '../../../../Framework/Signum.React/Scripts/Globals'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import { Lite, toLite } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { ResultTable, FindOptions, FilterOption, QueryDescription, SubTokensOptions, QueryToken, QueryTokenType, ColumnOption } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { TypeContext, FormGroupSize, FormGroupStyle, StyleOptions, StyleContext, mlistItemContext } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import { SearchMessage, JavascriptMessage, parseLite, is, liteKey } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { PropertyRoute, getQueryNiceName, getTypeInfo, Binding, GraphExplorer }  from '../../../../Framework/Signum.React/Scripts/Reflection'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import { ChartColumnEntity, ChartScriptColumnEntity, ChartScriptParameterEntity, ChartRequest, GroupByChart, ChartMessage,
    ChartColorEntity, ChartScriptEntity, ChartParameterEntity, ChartParameterType, UserChartEntity } from '../Signum.Entities.Chart'
import * as ChartClient from '../ChartClient'
import * as UserChartClient from './UserChartClient'
import ChartRequestView from '../Templates/ChartRequestView'


interface UserChartPageProps extends ReactRouter.RouteComponentProps<{}, { userChartId: string; entity?: string }> {

}


export default class UserChartPage extends React.Component<UserChartPageProps, { userChart?: UserChartEntity; chartRequest?: ChartRequest }> {

    constructor(props: UserChartPageProps) {
        super(props);
        this.state = {};
    }

    componentWillMount() {
        this.load(this.props);
    }

    componentWillReceiveProps(nextProps: UserChartPageProps) {
        this.state = {};
        this.forceUpdate();
        this.load(nextProps);
    }

    load(props: UserChartPageProps) {

        const {userChartId, entity } = this.props.routeParams!;

        const lite = entity == undefined ? undefined : parseLite(entity);

        Navigator.API.fillToStrings(lite ? [lite]: [])
            .then(()=> Navigator.API.fetchEntity(UserChartEntity, userChartId))
            .then(uc => {
                this.setState({ userChart : uc });
                return UserChartClient.Converter.toChartRequest(uc, lite)
            })
            .then(cr => this.setState({ chartRequest: cr}))
            .done();
    }


    render() {
        return <ChartRequestView
            chartRequest={this.state.chartRequest}
            userChart={this.state.userChart}
            onChange={cr => this.setState({ chartRequest: cr }) }/>;
    }
}


