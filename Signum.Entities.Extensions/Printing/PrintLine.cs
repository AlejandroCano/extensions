﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using Signum.Utilities;
using Signum.Entities.Processes;
using Signum.Entities.Basics;
using Signum.Entities;
using Signum.Entities.Files;
using System.Reflection;
using Signum.Entities.Authorization;

namespace Signum.Entities.Printing
{
    [Serializable, EntityKind(EntityKind.System, EntityData.Transactional)]
    public class PrintLineEntity : Entity, IProcessLineDataEntity
    {
        public DateTime CreationDate { get; private set; } = TimeZoneManager.Now;

        [NotNullable]
        [NotNullValidator]
        public EmbeddedFilePathEntity File { get; set; }

        public Lite<PrintPackageEntity> Package { get; set; }

        public DateTime? PrintedOn { get; set; }

        [ImplementedBy()]
        public Lite<Entity> Referred { get; set; }

        public PrintLineState State { get; set; }

        static StateValidator<PrintLineEntity, PrintLineState> stateValidator =
            new StateValidator<PrintLineEntity, PrintLineState>
            (n => n.State, n => n.PrintedOn)
            {
                { PrintLineState.ReadyToPrint,  false  },
                { PrintLineState.Printed,       true   },
                { PrintLineState.Error,         false  },
                { PrintLineState.Cancelled,     false  }   
            };
        protected override string PropertyValidation(PropertyInfo pi)
        {
            return stateValidator.Validate(this, pi) ?? base.PropertyValidation(pi);
        }
    }
    public enum PrintLineState
    {
        ReadyToPrint,
        Printed,
        Cancelled,
        Error
    }

    [AutoInit]
    public static class PrintLineOperation
    {
        public static ExecuteSymbol<PrintLineEntity> Print;
        public static ExecuteSymbol<PrintLineEntity> Retry;
        public static ExecuteSymbol<PrintLineEntity> Cancel;
    }

    [AutoInit]
    public static class PrintPackageProcess
    {
        public static readonly ProcessAlgorithmSymbol PrintPackage;
    }

    [AutoInit]
    public static class PrintPermission
    {
        public static PermissionSymbol ViewPrintPanel;
    }

}
