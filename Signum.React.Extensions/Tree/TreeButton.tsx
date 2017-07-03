﻿
import * as React from 'react'
import { Button, MenuItem, } from 'react-bootstrap'
import { Dic, classes } from '../../../Framework/Signum.React/Scripts/Globals'
import { getQueryKey } from '../../../Framework/Signum.React/Scripts/Reflection'
import * as Finder from '../../../Framework/Signum.React/Scripts/Finder'
import { Lite, toLite } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import { ResultTable, FindOptions, FilterOption, QueryDescription } from '../../../Framework/Signum.React/Scripts/FindOptions'
import { SearchMessage, JavascriptMessage, parseLite, is } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import * as Navigator from '../../../Framework/Signum.React/Scripts/Navigator'
import { default as SearchControlLoaded } from '../../../Framework/Signum.React/Scripts/SearchControl/SearchControlLoaded'
import { TreeMessage } from './Signum.Entities.Tree'
import * as TreeClient from './TreeClient'

export interface TreeButtonProps {
    searchControl: SearchControlLoaded;
}

export default class TreeButton extends React.Component<TreeButtonProps, void> {

    handleClick = (e: React.MouseEvent<any>) => {

        const fo = this.props.searchControl.props.findOptions;

        const path = TreeClient.treePath(fo.queryKey, Finder.toFilterOptions(fo.filterOptions));

        Navigator.pushOrOpenInTab(path, e);
    }
    
    render() {
        return (
            <Button onClick={this.handleClick}><i className="fa fa-sitemap"></i> &nbsp; {TreeMessage.Tree.niceToString()}</Button>
        );
    }
}



