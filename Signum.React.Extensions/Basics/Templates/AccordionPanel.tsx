﻿import * as React from 'react'
import { Panel, PanelGroup } from "react-bootstrap";
import { classes } from '../../../../Framework/Signum.React/Scripts/Globals'
import { BsStyle } from "../../../../Framework/Signum.React/Scripts/Operations";

export interface AccordionPanelProps {
    panels: Panel[];
}

export interface AccordionPanelState {
    activeIndex?: number;
}

export default class AccordionPanel extends React.Component<AccordionPanelProps, AccordionPanelState> {

    constructor(props: AccordionPanelProps) {
        super(props);
        this.state = { activeIndex: 0 };
    }

    handleSelect = (activeIndex: number) => {
        this.setState({ activeIndex });
    }

    render() {
        return (
            <div>
                <PanelGroup activeKey={this.state.activeIndex} onSelect={this.handleSelect as any} accordion>
                    {
                        React.Children.map(this.props.panels,
                            (p, i) => React.cloneElement((p as React.ReactElement<any>), { eventKey: i, key: i }))
                    }
                </PanelGroup>
            </div>);
    }
}

