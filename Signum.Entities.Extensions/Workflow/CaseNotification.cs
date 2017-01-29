﻿using Signum.Entities;
using Signum.Entities.Authorization;
using Signum.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Signum.Entities.Workflow
{
    [Serializable, EntityKind(EntityKind.System, EntityData.Transactional)]
    public class CaseNotificationEntity : Entity
    {
        [NotNullable]
        [NotNullValidator]
        public Lite<CaseActivityEntity> CaseActivity { get; set; }

        [NotNullable]
        [NotNullValidator]
        public Lite<UserEntity> User { get; set; }

        [NotNullable, ImplementedBy(typeof(UserEntity), typeof(RoleEntity))]
        [NotNullValidator]
        public Lite<Entity> Actor { get; internal set; }

        public CaseNotificationState State { get; set; }
    }

    public enum CaseNotificationState
    {
        New,
        Opened,
        InProgress,
        Done,
        DoneByOther,
    }

    [Serializable]
    public class InboxFilterModel : ModelEntity
    {
        public DateFilterRange Range { get; set; }
        public MList<CaseNotificationState> States { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

    }

    public enum InboxFilterModelMessage
    {
        Clear,
    }

    public enum DateFilterRange
    {
        All,
        LastWeek,
        LastMonth,
        CurrentYear,
    }
}
