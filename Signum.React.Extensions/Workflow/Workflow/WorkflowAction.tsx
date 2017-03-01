﻿import * as React from 'react'
import { ValueLine, EntityLine, TypeContext, FormGroup, ValueLineType, LiteAutocompleteConfig } from '../../../../Framework/Signum.React/Scripts/Lines'
import { PropertyRoute, Binding } from '../../../../Framework/Signum.React/Scripts/Reflection'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import CSharpCodeMirror from '../../../../Extensions/Signum.React.Extensions/Codemirror/CSharpCodeMirror'
import { WorkflowActionEntity } from '../Signum.Entities.Workflow'
import { WorkflowConditionTestResponse, API, DecisionResultValues } from '../WorkflowClient'
import TypeHelpComponent from '../../Dynamic/Help/TypeHelpComponent'
import ValueLineModal from '../../../../Framework/Signum.React/Scripts/ValueLineModal'

interface WorkflowConditionComponentProps {
    ctx: TypeContext<WorkflowActionEntity>;
}

export default class WorkflowConditionComponent extends React.Component<WorkflowConditionComponentProps, void> {

    handleMainEntityTypeChange = () => {
        this.props.ctx.value.eval!.script = "";
        this.forceUpdate();
    }

    handleCodeChange = (newScript: string) => {
        const evalEntity = this.props.ctx.value.eval!;
        evalEntity.script = newScript;
        evalEntity.modified = true;
        this.forceUpdate();
    }

    render() {
        var ctx = this.props.ctx;

        return (
            <div>
                <ValueLine ctx={ctx.subCtx(wc => wc.name)} />
                <EntityLine ctx={ctx.subCtx(wc => wc.mainEntityType)}
                    onChange={this.handleMainEntityTypeChange}
                    autoComplete={new LiteAutocompleteConfig(str => API.findMainEntityType({ subString: str, count: 5 }), false)}
                    find={false} />
                {ctx.value.mainEntityType &&
                    <div>
                        <br />
                        <div className="row">
                            <div className="col-sm-7">
                                <div className="code-container">
                                    <pre style={{ border: "0px", margin: "0px" }}>{"void Action(" + ctx.value.mainEntityType.cleanName + " e, WorkflowTransitionContext ctx)\n{"}</pre>
                                    <CSharpCodeMirror script={ctx.value.eval!.script || ""} onChange={this.handleCodeChange} />
                                    <pre style={{ border: "0px", margin: "0px" }}>{"}"}</pre>
                                </div>
                            </div>
                            <div className="col-sm-5">
                                <TypeHelpComponent initialType={ctx.value.mainEntityType.cleanName} mode="CSharp" onMemberClick={this.handleTypeHelpClick} />
                            </div>
                        </div>
                    </div>}
            </div>
        );
    }

    handleTypeHelpClick = (pr: PropertyRoute | undefined) => {
        if (!pr)
            return;

        ValueLineModal.show({
            type: { name: "string" },
            initialValue: TypeHelpComponent.getExpression("e", pr, "CSharp"),
            valueLineType: "TextArea",
            title: "Property Template",
            message: "Copy to clipboard: Ctrl+C, ESC",
            initiallyFocused: true,
        });
    }
}

