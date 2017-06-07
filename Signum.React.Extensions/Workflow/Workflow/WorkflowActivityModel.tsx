﻿import * as React from 'react'
import {
    WorkflowActivityEntity, WorkflowActivityModel, WorkflowActivityValidationEmbedded, WorkflowMessage, WorkflowActivityMessage, WorkflowConditionEntity, WorkflowActionEntity,
    WorkflowJumpEmbedded, WorkflowTimeoutEmbedded, IWorkflowNodeEntity, SubWorkflowEmbedded, SubEntitiesEval, WorkflowScriptEntity, WorkflowScriptPartEmbedded, WorkflowScriptEval, WorkflowEntity
} from '../Signum.Entities.Workflow'
import * as WorkflowClient from '../WorkflowClient'
import * as DynamicViewClient from '../../../../Extensions/Signum.React.Extensions/Dynamic/DynamicViewClient'
import { TypeContext, ValueLine, ValueLineType, EntityLine, EntityTable, EntityDetail, FormGroup, LiteAutocompleteConfig, RenderEntity } from '../../../../Framework/Signum.React/Scripts/Lines'
import { is, JavascriptMessage, Lite } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { TypeEntity } from '../../../../Framework/Signum.React/Scripts/Signum.Entities.Basics'
import { DynamicValidationEntity } from '../../../../Extensions/Signum.React.Extensions/Dynamic/Signum.Entities.Dynamic'
import { Dic } from '../../../../Framework/Signum.React/Scripts/Globals';
import { Binding } from '../../../../Framework/Signum.React/Scripts/Reflection';
import CSharpCodeMirror from '../../../../Extensions/Signum.React.Extensions/Codemirror/CSharpCodeMirror'
import TypeHelpComponent from '../../Dynamic/Help/TypeHelpComponent'
import HtmlEditor from '../../../../Extensions/Signum.React.Extensions/HtmlEditor/HtmlEditor'
import Typeahead from '../../../../Framework/Signum.React/Scripts/Lines/Typeahead'
import { API } from '../WorkflowClient'

interface WorkflowActivityModelComponentProps {
    ctx: TypeContext<WorkflowActivityModel>;
}

interface WorkflowActivityModelComponentState {
    viewInfo: { [name: string]: "Static" | "Dynamic" };

}

export default class WorkflowActivityModelComponent extends React.Component<WorkflowActivityModelComponentProps, WorkflowActivityModelComponentState> {

    constructor(props: WorkflowActivityModelComponentProps) {
        super(props);

        this.state = { viewInfo: {} };
    }

    componentWillMount() {

        if (this.props.ctx.value.mainEntityType) {

            const typeName = this.props.ctx.value.mainEntityType.cleanName;

            const registeredViews = WorkflowClient.getViewNames(typeName).toObject(k => k, v => "Static") as { [name: string]: "Static" | "Dynamic" };

            DynamicViewClient.API.getDynamicViewNames(typeName)
                .then(dynamicViews => {
                    dynamicViews.forEach(dv => {
                        if (registeredViews[dv])
                            throw Error(WorkflowActivityMessage.DuplicateViewNameFound0.niceToString(`"${dv}"`));
                        else
                            registeredViews[dv] = "Dynamic";
                    });

                    this.setState({ viewInfo: registeredViews });
                }).done();
        }

        this.handleTypeChange();
    }

    getViewNameColor(viewName: string) {

        if (this.state.viewInfo[viewName] == "Dynamic")
            return { color: "blue" };

        return { color: "black" };
    }

    handleViewNameChange = (e: React.SyntheticEvent<HTMLSelectElement>) => {
        this.props.ctx.value.viewName = (e.currentTarget as HTMLSelectElement).value;
        this.props.ctx.value.modified = true;
        this.forceUpdate();
    };

    handleTypeChange = () => {

        var wa = this.props.ctx.value;
        if (wa.type != "Task")
            wa.timeout = null;

        if (wa.type == "Script") {
            if (!wa.script)
                wa.script = WorkflowScriptPartEmbedded.New({
                });
            wa.subWorkflow = null;
        }

        if (wa.type == "DecompositionWorkflow" || wa.type == "CallWorkflow") {
            if (!wa.subWorkflow)
                wa.subWorkflow = SubWorkflowEmbedded.New({
                    subEntitiesEval: SubEntitiesEval.New()
                });
            wa.script = null;
        }

        if (wa.type == "DecompositionWorkflow" || wa.type == "CallWorkflow" || wa.type == "Script") {
            wa.viewName = null;
            wa.requiresOpen = false;
            wa.reject = null;
            wa.validationRules = [];
        }
        else {
            wa.subWorkflow = null;
            wa.script = null;
        }

        wa.modified = true;

        this.forceUpdate();
    }

    render() {
        var ctx = this.props.ctx;

        const mainEntityType = this.props.ctx.value.mainEntityType;

        return (
            <div>
                <ValueLine ctx={ctx.subCtx(d => d.name)} onChange={() => this.forceUpdate()} />
                <ValueLine ctx={ctx.subCtx(d => d.type)} onChange={this.handleTypeChange} />
                <ValueLine ctx={ctx.subCtx(a => a.estimatedDuration)} />

                {ctx.value.type != "DecompositionWorkflow" && ctx.value.type != "CallWorkflow" && ctx.value.type != "Script" &&
                    <div>
                    {ctx.value.mainEntityType ?
                        <FormGroup ctx={ctx.subCtx(d => d.viewName)} labelText={ctx.niceName(d => d.viewName)}>
                            {
                                <select value={ctx.value.viewName ? ctx.value.viewName : ""} className="form-control" onChange={this.handleViewNameChange} style={this.getViewNameColor(ctx.value.viewName || "")} >
                                    {!ctx.value.viewName && <option value="">{" - "}</option>}
                                    {Dic.getKeys(this.state.viewInfo).map((v, i) => <option key={i} value={v} style={this.getViewNameColor(v)}>{v}</option>)}
                                </select>
                            }
                        </FormGroup>
                        : <div className="alert alert-warning">{WorkflowMessage.ToUse0YouSouldSetTheWorkflow1.niceToString(ctx.niceName(e => e.viewName), ctx.niceName(e => e.mainEntityType))}</div>}


                        <ValueLine ctx={ctx.subCtx(a => a.requiresOpen)} />
                        {ctx.value.mainEntityType ?
                            <EntityTable ctx={ctx.subCtx(d => d.validationRules)} columns={EntityTable.typedColumns<WorkflowActivityValidationEmbedded>([
                            {
                                property: wav => wav.rule,
                                headerHtmlAttributes: { style: { width: "100%" } },
                                template: ctx => <EntityLine ctx={ctx.subCtx(wav => wav.rule)} findOptions={{
                                    queryName: DynamicValidationEntity,
                                    filterOptions: [
                                        { columnName: "Entity.EntityType", value: mainEntityType },
                                        { columnName: "Entity.IsGlobalyEnabled", value: false },
                                    ]
                                }} />
                            },
                            ctx.value.type == "Decision" ? {
                                property: wav => wav.onAccept,
                                cellHtmlAttributes: ctx => ({ style: { verticalAlign: "middle" } }),
                                template: ctx => <ValueLine ctx={ctx.subCtx(wav => wav.onAccept)} formGroupStyle="None" valueHtmlAttributes={{ style: { margin: "0 auto" } }} />,
                            } : null,
                            ctx.value.type == "Decision" ? {
                                property: wav => wav.onDecline,
                                cellHtmlAttributes: ctx => ({ style: { verticalAlign: "middle" } }),
                                template: ctx => <ValueLine ctx={ctx.subCtx(wav => wav.onDecline)} formGroupStyle="None" valueHtmlAttributes={{ style: { margin: "0 auto" } }} />,
                            } : null,
                        ])} />
                            : <div className="alert alert-warning">{WorkflowMessage.ToUse0YouSouldSetTheWorkflow1.niceToString(ctx.niceName(e => e.validationRules), ctx.niceName(e => e.mainEntityType))}</div>}

                        <EntityDetail ctx={ctx.subCtx(a => a.reject)} />

                        {ctx.value.type == "Task" ? ctx.value.workflow ?
                            <EntityDetail ctx={ctx.subCtx(a => a.timeout)} getComponent={(tctx: TypeContext<WorkflowTimeoutEmbedded>) =>
                                <div>
                                    <FormGroup ctx={tctx.subCtx(t => t.timeout)} >
                                        <RenderEntity ctx={tctx.subCtx(t => t.timeout)} />
                                    </FormGroup>
                                    <EntityLine
                                        ctx={tctx.subCtx(t => t.to)}
                                        autoComplete={new LiteAutocompleteConfig((ac,str) => API.findNode({ workflowId: ctx.value.workflow!.id, subString: str, count: 5, excludes: this.getCurrentJumpsTo() }, ac), false)}
                                        find={false}
                                        helpBlock={WorkflowMessage.ToUseNewNodesOnJumpsYouSouldSaveWorkflow.niceToString()} />
                                    <EntityLine ctx={tctx.subCtx(t => t.action)} findOptions={{
                                        queryName: WorkflowActionEntity,
                                        parentColumn: "Entity.MainEntityType",
                                        parentValue: ctx.value.mainEntityType
                                    }} />
                            </div>
                        } /> : <div className="alert alert-warning">{WorkflowMessage.ToUse0YouSouldSaveWorkflow.niceToString(ctx.niceName(e => e.timeout))}</div>
                            : undefined}

                        {ctx.value.workflow ?
                            <EntityTable
                                labelText={<div>{ctx.niceName(d => d.jumps)} : <small style={{ fontWeight: "normal" }}>{WorkflowMessage.ToUseNewNodesOnJumpsYouSouldSaveWorkflow.niceToString()}</small></div>}
                                ctx={ctx.subCtx(d => d.jumps)}
                                columns={EntityTable.typedColumns<WorkflowJumpEmbedded>([
                                    {
                                        property: wj => wj.to,
                                        template: (jctx, row, state) => {
                                            return <EntityLine
                                                ctx={jctx.subCtx(wj => wj.to)}
                                                autoComplete={new LiteAutocompleteConfig((ac, str) => API.findNode({ workflowId: ctx.value.workflow!.id, subString: str, count: 5, excludes: this.getCurrentJumpsTo() }, ac), false)}
                                                find={false} />
                                        },
                                        headerHtmlAttributes: { width: "40%" }
                                    },
                                    {
                                        property: wj => wj.action,
                                        headerHtmlAttributes: { width: "30%" },
                                        template: (jctx, row, state) => {
                                            return <EntityLine ctx={jctx.subCtx(wj => wj.action)} findOptions={{
                                                queryName: WorkflowActionEntity,
                                                parentColumn: "Entity.MainEntityType",
                                                parentValue: ctx.value.mainEntityType
                                            }} />
                                        },
                                    },
                                    {
                                        property: wj => wj.condition,
                                        headerHtmlAttributes: { width: "20%" },
                                        template: (jctx, row, state) => {
                                            return <EntityLine ctx={jctx.subCtx(wj => wj.condition)} findOptions={{
                                                queryName: WorkflowConditionEntity,
                                                parentColumn: "Entity.MainEntityType",
                                                parentValue: ctx.value.mainEntityType
                                            }} />
                                        },
                                    },
                                ])} /> :
                            <div className="alert alert-warning">{WorkflowMessage.ToUse0YouSouldSaveWorkflow.niceToString(ctx.niceName(e => e.jumps))}</div>
                        }

                        <fieldset>
                            <legend>{WorkflowActivityModel.nicePropertyName(a => a.userHelp)}</legend>
                            <HtmlEditor binding={Binding.create(ctx.value, a => a.userHelp)} />
                        </fieldset>
                        <ValueLine ctx={ctx.subCtx(d => d.comments)} />
                    </div>
                }

                {ctx.value.script ?
                    ctx.value.workflow ? <ScriptComponent ctx={ctx.subCtx(a => a.script!)} mainEntityType={ctx.value.mainEntityType} workflow={ctx.value.workflow!} />
                        : <div className="alert alert-warning">{WorkflowMessage.ToUse0YouSouldSaveWorkflow.niceToString(ctx.niceName(e => e.script))}</div>
                    : undefined
                }

                {ctx.value.subWorkflow ?
                    ctx.value.mainEntityType ? <DecompositionComponent ctx={ctx.subCtx(a => a.subWorkflow!)} mainEntityType={ctx.value.mainEntityType} />
                        : <div className="alert alert-warning">{WorkflowMessage.ToUse0YouSouldSetTheWorkflow1.niceToString(ctx.niceName(e => e.subWorkflow), ctx.niceName(e => e.mainEntityType))}</div>
                    : undefined}
            </div>
        );
    }

    getCurrentJumpsTo() {
        var result: Lite<IWorkflowNodeEntity>[] = [];
        var ctx = this.props.ctx;
        if (ctx.value.workflowActivity)
            result.push(ctx.value.workflowActivity);
        ctx.value.jumps.forEach(j => j.element.to && result.push(j.element.to));
        return result;
    }
}

class ScriptComponent extends React.Component<{ ctx: TypeContext<WorkflowScriptPartEmbedded>, mainEntityType: TypeEntity, workflow: WorkflowEntity }, void>{


    render() {
        const ctx = this.props.ctx;
        const mainEntityName = this.props.workflow.mainEntityType!.cleanName;
        return (
            <fieldset>
                <legend>{ctx.niceName()}</legend>
                <EntityLine ctx={ctx.subCtx(p => p.script)} findOptions={{
                    queryName: WorkflowScriptEntity,
                    parentColumn: "Entity.MainEntityType",
                    parentValue: this.props.mainEntityType
                }} />
                <EntityLine ctx={ctx.subCtx(s => s.retryStrategy)} />
                <EntityLine
                    ctx={ctx.subCtx(s => s.onFailureJump)}
                    autoComplete={new LiteAutocompleteConfig((ac, str) => API.findNode({ workflowId: this.props.workflow.id, subString: str, count: 5 }, ac), false)}
                    find={false}
                    helpBlock={WorkflowMessage.ToUseNewNodesOnJumpsYouSouldSaveWorkflow.niceToString()} />
             
            </fieldset>
        );
    }
}

class DecompositionComponent extends React.Component<{ ctx: TypeContext<SubWorkflowEmbedded>, mainEntityType: TypeEntity }, void>{

    handleCodeChange = (newScript: string) => {
        const subEntitiesEval = this.props.ctx.value.subEntitiesEval!;
        subEntitiesEval.script = newScript;
        subEntitiesEval.modified = true;
        this.forceUpdate();
    }

    render() {
        const ctx = this.props.ctx;
        const mainEntityName = this.props.mainEntityType.cleanName;
        return (
            <fieldset>
                <legend>{ctx.niceName()}</legend>
                <EntityLine ctx={ctx.subCtx(a => a.workflow)} onChange={() => this.forceUpdate()} />
                {ctx.value.workflow &&
                    <div>
                        <br />
                        <div className="row">
                            <div className="col-sm-7">
                                <div className="code-container">
                                    <pre style={{ border: "0px", margin: "0px" }}>{`IEnumerable<${ctx.value.workflow.mainEntityType!.cleanName}Entity> SubEntities(${mainEntityName}Entity e, WorkflowTransitionContext ctx)\n{`}</pre>
                                    <CSharpCodeMirror script={ctx.value.subEntitiesEval!.script || ""} onChange={this.handleCodeChange} />
                                    <pre style={{ border: "0px", margin: "0px" }}>{"}"}</pre>
                                </div>
                            </div>
                            <div className="col-sm-5">
                                <TypeHelpComponent initialType={mainEntityName} mode="CSharp" />
                            </div>
                        </div>
                    </div>}
            </fieldset>
        );
    }
}
