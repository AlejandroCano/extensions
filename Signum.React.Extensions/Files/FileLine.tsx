﻿import * as React from 'react'
import { Link } from 'react-router'
import { classes, Dic } from '../../../Framework/Signum.React/Scripts/Globals'
import * as Services from '../../../Framework/Signum.React/Scripts/Services'
import * as Navigator from '../../../Framework/Signum.React/Scripts/Navigator'
import * as Constructor from '../../../Framework/Signum.React/Scripts/Constructor'
import * as Finder from '../../../Framework/Signum.React/Scripts/Finder'
import { FindOptions } from '../../../Framework/Signum.React/Scripts/FindOptions'
import { TypeContext, StyleContext, StyleOptions, FormGroupStyle } from '../../../Framework/Signum.React/Scripts/TypeContext'
import { PropertyRoute, PropertyRouteType, MemberInfo, getTypeInfo, getTypeInfos, TypeInfo, IsByAll, basicConstruct } from '../../../Framework/Signum.React/Scripts/Reflection'
import { LineBase, LineBaseProps, FormGroup, FormControlStatic, runTasks} from '../../../Framework/Signum.React/Scripts/Lines/LineBase'
import { ModifiableEntity, Lite, Entity, EntityControlMessage, JavascriptMessage, toLite, is, liteKey, getToString, } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import { IFile, IFilePath, FileMessage, FileTypeSymbol, FileEntity, FilePathEntity, EmbeddedFileEntity, EmbeddedFilePathEntity } from './Signum.Entities.Files'
import Typeahead from '../../../Framework/Signum.React/Scripts/Lines/Typeahead'
import { EntityBase, EntityBaseProps } from '../../../Framework/Signum.React/Scripts/Lines/EntityBase'
import { default as FileDownloader, FileDownloaderConfiguration, DownloadBehaviour } from './FileDownloader'
import FileUploader from './FileUploader'

require("!style!css!./Files.css");

export { FileTypeSymbol };

export interface FileLineProps extends EntityBaseProps {
    ctx: TypeContext<ModifiableEntity & IFile | Lite<IFile & Entity> | undefined | null>;
    download?: DownloadBehaviour;
    dragAndDrop?: boolean;
    dragAndDropMessage?: string;
    fileType?: FileTypeSymbol;
    accept?: string;
    configuration?: FileDownloaderConfiguration<IFile>;
}


export default class FileLine extends EntityBase<FileLineProps, FileLineProps> {

    static defaultProps = {
        download: "SaveAs",
        dragAndDrop: true
    }
   
    calculateDefaultState(state: FileLineProps) {
        super.calculateDefaultState(state);
    }

    handleFileLoaded = (file: IFile & ModifiableEntity) => {

        this.convert(file)
            .then(f => this.setValue(f))
            .done();
    }
    

    renderInternal() {

        const s = this.state;

        const hasValue = !!s.ctx.value;

        return (
            <FormGroup ctx={s.ctx} labelText={s.labelText} labelProps={s.labelHtmlProps} htmlProps={{ ...this.baseHtmlProps(), ...EntityBase.entityHtmlProps(s.ctx.value), ...s.formGroupHtmlProps }}>
                {hasValue ? this.renderFile() :
                    <FileUploader
                        accept={this.props.accept}
                        dragAndDrop={this.props.dragAndDrop}
                        dragAndDropMessage={this.props.dragAndDropMessage}
                        fileType={this.props.fileType}
                        onFileLoaded={this.handleFileLoaded}
                        typeName={this.props.ctx.propertyRoute.typeReference().name}
                        divHtmlProps={{ className: "sf-file-line-new" }}/>
                }
            </FormGroup>
        );
    }


    renderFile() {

        const val = this.state.ctx.value!;

        return (
            <div className="input-group">
                {
                    this.state.download == "None" ? <span className="form-control file-control">{val.toStr}</span> :
                        <FileDownloader
                            configuration={this.props.configuration}
                            download={this.props.download}
                            entityOrLite={val}
                            htmlProps={{ className: "form-control file-control" }} />
                }
                <span className="input-group-btn">
                    {this.renderRemoveButton(true, val) }
                </span>
            </div>
        );
    }
    
}

