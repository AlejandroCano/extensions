﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using Signum.Utilities;
using System.ComponentModel;

namespace Signum.Entities.Files
{
    public interface IFile
    {
        byte[] BinaryFile { get; set; }
        string FileName { get; set; }
        string FullWebPath { get; }
    }

    public enum FileMessage
    {
        [Description("Download File")]
        DownloadFile,
        ErrorSavingFile,
        [Description("FileTypes")]
        FileTypes,
        Open,
        [Description("Opening has no default implementation for {0}")]
        OpeningHasNotDefaultImplementationFor0,
        [Description("Download")]
        WebDownload,
        [Description("Image")]
        WebImage,
        Remove,
        [Description("Saving has no default implementation for {0}")]
        SavingHasNotDefaultImplementationFor0,
        [Description("Select File")]
        SelectFile,
        ViewFile,
        [Description("Viewing has no default implementation for {0}")]
        ViewingHasNotDefaultImplementationFor0,
        OnlyOneFileIsSupported
    }


    [Serializable, DescriptionOptions(DescriptionOptions.Description | DescriptionOptions.Members)]
    public class WebImage
    {
        public string FullWebPath;
    }

    [Serializable, DescriptionOptions(DescriptionOptions.Description | DescriptionOptions.Members)]
    public class WebDownload
    {
        public string FullWebPath;
    }

    [Serializable]
    public class EmbeddedFileEntity : EmbeddedEntity, IFile
    {
        [NotNullable]
        [StringLengthValidator(Min = 3)]
        public string FileName { get; set; }

        [NotNullable]
        [NotNullValidator]
        public byte[] BinaryFile { get; set; }

        public override string ToString()
        {
            return "{0} {1}".FormatWith(FileName, BinaryFile?.Let(bf => StringExtensions.ToComputerSize(bf.Length)) ?? "??");
        }


        public string FullWebPath
        {
            get { return null; }
        }
    }
}
