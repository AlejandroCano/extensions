﻿
import * as React from 'react'
import { RouteComponentProps } from 'react-router'
import { DropdownButton, MenuItem } from 'react-bootstrap'
import { Dic, classes } from '../../../Framework/Signum.React/Scripts/Globals'
import * as Finder from '../../../Framework/Signum.React/Scripts/Finder'
import { ResultTable, FindOptions, FilterOption, QueryDescription } from '../../../Framework/Signum.React/Scripts/FindOptions'
import { SearchMessage, JavascriptMessage, parseLite, is, Lite, toLite } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import * as Navigator from '../../../Framework/Signum.React/Scripts/Navigator'
import SearchControlLoaded from '../../../Framework/Signum.React/Scripts/SearchControl/SearchControlLoaded'
import { ExcelReportEntity, ExcelMessage } from './Signum.Entities.Excel'
import * as ExcelClient from './ExcelClient'

export interface ExcelMenuProps {
    searchControl: SearchControlLoaded;

    plainExcel: boolean;
    excelReport: boolean;
}

export default class ExcelMenu extends React.Component<ExcelMenuProps, { excelReport?: Lite<ExcelReportEntity>[] }> {

    constructor(props: ExcelMenuProps) {
        super(props);
        this.state = { };
    }

    handleSelectedToggle = (isOpen: boolean) => {

        if (isOpen && this.state.excelReport == undefined)
            this.reloadList().done();
    }

    reloadList(): Promise<void> {
        return ExcelClient.API.forQuery(this.props.searchControl.props.findOptions.queryKey)
            .then(list => this.setState({ excelReport: list }));
    }


    handlePlainExcel = () => {
        ExcelClient.API.generatePlanExcel(this.props.searchControl.getQueryRequest());
    }


    handleSelect = (er: Lite<ExcelReportEntity>) => {
        ExcelClient.API.generateExcelReport(this.props.searchControl.getQueryRequest(), er);
    }

    handleCreate = () => {
        Finder.API.fetchQueryEntity(this.props.searchControl.props.findOptions.queryKey)
            .then(qe => ExcelReportEntity.New({ query: qe }))
            .then(er => Navigator.view(er))
            .then(() => this.reloadList())
            .done();
    }

    handleAdmnister = () => {
        Finder.explore({ queryName: ExcelReportEntity, parentColumn: "Query.Key", parentValue: this.props.searchControl.props.findOptions.queryKey })
            .then(() => this.reloadList())
            .done();
    }

    render() {
        const label = <span><i className="fa fa-file-excel-o"></i> &nbsp; {ExcelMessage.ExcelReport.niceToString()}</span>;

        if (this.props.plainExcel && !this.props.excelReport)
            return <button className={"sf-query-button sf-search btn btn-default"} onClick={this.handlePlainExcel}>{label} </button>;

        const excelReports = this.state.excelReport;
        return (
            <DropdownButton title={label as any} id="userQueriesDropDown" className="sf-userquery-dropdown"
                onToggle={this.handleSelectedToggle}>
                { this.props.plainExcel && <MenuItem onSelect={this.handlePlainExcel} >{label }</MenuItem> }
                { this.props.plainExcel && excelReports && excelReports.length > 0 && <MenuItem divider/> }
                {
                    excelReports && excelReports.map((uq, i) =>
                        <MenuItem key={i}
                            onSelect={() => this.handleSelect(uq) }>
                            { uq.toStr }
                        </MenuItem>)
                }
                {  (this.props.plainExcel || excelReports && excelReports.length > 0) && <MenuItem divider/> }
                <MenuItem onSelect={this.handleAdmnister}>{ExcelMessage.Administer.niceToString() }</MenuItem>
                <MenuItem onSelect={this.handleCreate}>{ExcelMessage.CreateNew.niceToString() }</MenuItem>
            </DropdownButton>
        );
    }
 
}



