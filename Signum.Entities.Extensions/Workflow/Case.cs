﻿using Signum.Entities;
using Signum.Entities.Authorization;
using Signum.Entities.Basics;
using Signum.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Signum.Entities.Workflow
{
    [Serializable, EntityKind(EntityKind.System, EntityData.Transactional), InTypeScript(Undefined = false)]
    public class CaseEntity : Entity
    {
        [NotNullable]
        [NotNullValidator]
        public WorkflowEntity Workflow { get; set; }

        public CaseEntity ParentCase { get; set; }

        [NotNullable, SqlDbType(Size = 100)]
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string Description { get; set; }

        [NotNullable, ImplementedByAll]
        [NotNullValidator]
        public ICaseMainEntity MainEntity { get; set; }

        public DateTime StartDate { get; set; } = TimeZoneManager.Now;
        public DateTime? FinishDate { get; set; }

        static Expression<Func<CaseEntity, string>> ToStringExpression = @this => @this.Description;
        [ExpressionField]
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    [AutoInit]
    public static class CaseOperation
    {
        public static readonly ExecuteSymbol<CaseEntity> SetTags;
    }

    public interface ICaseMainEntity : IEntity
    {

    }

    [Serializable]
    public class CaseTagsModel : ModelEntity
    {
        [NotNullable, PreserveOrder]
        [NotNullValidator, NoRepeatValidator]
        public MList<CaseTagTypeEntity> CaseTags { get; set; } = new MList<CaseTagTypeEntity>();

        [NotNullable, PreserveOrder]
        [NotNullValidator, NoRepeatValidator]
        public MList<CaseTagTypeEntity> OldCaseTags { get; set; } = new MList<CaseTagTypeEntity>();
    }


    [Serializable, EntityKind(EntityKind.System, EntityData.Transactional)]
    public class CaseTagEntity : Entity
    {
        public DateTime CreationDate { get; private set; } = TimeZoneManager.Now;

        [NotNullable]
        [NotNullValidator]
        public Lite<CaseEntity> Case { get; set; }

        [NotNullable]
        [NotNullValidator]
        public CaseTagTypeEntity TagType { get; set; }

        [NotNullable]
        [NotNullValidator, ImplementedBy(typeof(UserEntity))]
        public Lite<IUserEntity> CreatedBy { get; set; }
    }

    [Serializable, EntityKind(EntityKind.Main, EntityData.Master)]
    public class CaseTagTypeEntity : Entity
    {
        [NotNullable, SqlDbType(Size = 100), UniqueIndex]
        [StringLengthValidator(AllowNulls = false, Min = 2, Max = 100)]
        public string Name { get; set; }

        [NotNullable, SqlDbType(Size = 12)]
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 12)]
        public string Color { get; set; }

        static Expression<Func<CaseTagTypeEntity, string>> ToStringExpression = @this => @this.Name;
        [ExpressionField]
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    [AutoInit]
    public static class CaseTagTypeOperation
    {
        public static readonly ExecuteSymbol<CaseTagTypeEntity> Save;
    }

    [InTypeScript(true), DescriptionOptions(DescriptionOptions.Members)]
    public enum CaseFlowColor
    {
        CaseMaxDuration,
        AverageDuration,
        EstimatedDuration
    }
}
