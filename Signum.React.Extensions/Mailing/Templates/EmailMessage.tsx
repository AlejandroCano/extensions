﻿import * as React from 'react'
import { Tab, Tabs } from 'react-bootstrap'
import { classes } from '../../../../Framework/Signum.React/Scripts/Globals'
import { FormGroup, FormControlStatic, ValueLine, ValueLineType, EntityLine, EntityCombo, EntityDetail, EntityList, EntityRepeater, EntityTabRepeater } from '../../../../Framework/Signum.React/Scripts/Lines'
import { SearchControl } from '../../../../Framework/Signum.React/Scripts/Search'
import { getToString, getMixin } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { TypeContext, FormGroupStyle } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import {
    EmailMessageEntity, EmailAddressEmbedded, EmailRecipientEntity, EmailAttachmentEmbedded,
    EmailReceptionMixin, EmailFileType
} from '../Signum.Entities.Mailing'
import { EmailTemplateEntity, EmailTemplateContactEmbedded, EmailTemplateRecipientEntity, EmailTemplateMessageEmbedded, EmailTemplateViewMessage, EmailTemplateMessage } from '../Signum.Entities.Mailing'
import FileLine from '../../Files/FileLine'
import IFrameRenderer from './IFrameRenderer'
import HtmlCodemirror from '../../Codemirror/HtmlCodemirror'


export default class EmailMessage extends React.Component<{ ctx: TypeContext<EmailMessageEntity> }, void> {

    render() {

        let e = this.props.ctx;

        if (e.value.state != "Created")
            e = e.subCtx({ readOnly: true });

        const sc4 = e.subCtx({ labelColumns: { sm: 4 } });
        const sc1 = e.subCtx({ labelColumns: { sm: 1 } });

        return (
            <Tabs id="newsletterTabs">
                <Tab title={EmailMessageEntity.niceName()}>
                    <fieldset>
                        <legend>Properties</legend>
                        <div className="row">
                            <div className="col-sm-5">
                                <ValueLine ctx={sc4.subCtx(f => f.state)} />
                                <ValueLine ctx={sc4.subCtx(f => f.sent)} />
                                <ValueLine ctx={sc4.subCtx(f => f.bodyHash)} />
                            </div>
                            <div className="col-sm-7">
                                <EntityLine ctx={e.subCtx(f => f.template)} />
                                <EntityLine ctx={e.subCtx(f => f.package)} />
                                <EntityLine ctx={e.subCtx(f => f.exception)} />
                            </div>
                        </div>
                    </fieldset>


                    <div className="form-inline repeater-inline">
                        <EntityDetail ctx={e.subCtx(f => f.from)} />
                        <EntityRepeater ctx={e.subCtx(f => f.recipients)} />
                        <EntityRepeater ctx={e.subCtx(f => f.attachments)} getComponent={this.renderAttachment} />
                    </div>

                    <EntityLine ctx={sc1.subCtx(f => f.target)} />
                    <ValueLine ctx={sc1.subCtx(f => f.subject)} />
                    <ValueLine ctx={sc1.subCtx(f => f.isBodyHtml)} inlineCheckbox={true} onChange={() => this.forceUpdate()} />
                    {sc1.value.state != "Created" ? <IFrameRenderer style={{ width: "100%" }} html={e.value.body} /> :
                        sc1.value.isBodyHtml ? <div className="code-container"><HtmlCodemirror ctx={e.subCtx(f => f.body)} /></div> :
                            <div className="form-vertical">
                                <ValueLine ctx={e.subCtx(f => f.body)} valueLineType="TextArea" valueHtmlAttributes={{ style: { height: "180px" } }} formGroupStyle="SrOnly" />
                            </div>
                    }
                    <EmailMessageComponent ctx={e} invalidate={() => this.forceUpdate()} />
                </Tab>
                {getMixin(e.value, EmailReceptionMixin) && getMixin(e.value, EmailReceptionMixin).receptionInfo && this.renderEmailReceptionMixin()}
            </Tabs>
        );
    }


    renderEmailReceptionMixin = () => {

        const ri = this.props.ctx.subCtx(a => getMixin(a, EmailReceptionMixin).receptionInfo!);

        return <Tab title={EmailReceptionMixin.niceName()}>
            <fieldset>
                <legend>Properties</legend>

                <EntityLine ctx={ri.subCtx(f => f.reception)} />
                <ValueLine ctx={ri.subCtx(f => f.uniqueId)} />
                <ValueLine ctx={ri.subCtx(f => f.sentDate)} />
                <ValueLine ctx={ri.subCtx(f => f.receivedDate)} />
                <ValueLine ctx={ri.subCtx(f => f.deletionDate)} />

            </fieldset>

            <pre>{ri.value.rawContent}</pre>
        </Tab>;
    };


    renderAttachment = (ec: TypeContext<EmailAttachmentEmbedded>) => {
        const sc = ec.subCtx({ formGroupStyle: "SrOnly" });
        return (
            <div>
                <FileLine ctx={ec.subCtx(a => a.file)} remove={false}
                    fileType={EmailFileType.Attachment} />
            </div>
        );
    };
}

export interface EmailMessageComponentProps {
    ctx: TypeContext<EmailMessageEntity>;
    invalidate: () => void;
}

export class EmailMessageComponent extends React.Component<EmailMessageComponentProps, { showPreview: boolean }>{
    constructor(props: EmailMessageComponentProps) {
        super(props);
        this.state = { showPreview: false }
    }

    handlePreviewClick = (e: React.FormEvent<any>) => {
        e.preventDefault();
        this.setState({
            showPreview: !this.state.showPreview
        });
    }

    handleCodeMirrorChange = () => {
        if (this.state.showPreview)
            this.forceUpdate();
    }


    render() {
        const ec = this.props.ctx.subCtx({ labelColumns: { sm: 2 } });
        return (
            <div className="sf-email-template-message">
                <div className="form-vertical">
                    <br />
                    <a href="#" onClick={this.handlePreviewClick}>
                        {this.state.showPreview ?
                            EmailTemplateMessage.HidePreview.niceToString() :
                            EmailTemplateMessage.ShowPreview.niceToString()}
                    </a>
                    {this.state.showPreview && <IFrameRenderer style={{ width: "100%", height: "150px" }} html={ec.value.body} />}
                </div>
            </div>
        );
    }
}
