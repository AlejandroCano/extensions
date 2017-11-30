﻿using Signum.Entities;
using Signum.Entities.Basics;
using Signum.Entities.UserAssets;
using Signum.Entities.UserQueries;
using Signum.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Signum.Entities.DynamicQuery;
using System.Reflection;
using Signum.Entities.Files;
using Signum.Entities.Authorization;
using System.Xml.Linq;
using System.ComponentModel;

namespace Signum.Entities.MachineLearning
{
    [Serializable, EntityKind(EntityKind.Main, EntityData.Transactional)]
    public class PredictorEntity : Entity
    {
        public PredictorEntity()
        {
            RebindEvents();
        }

        [SqlDbType(Size = 100),]
        [StringLengthValidator(AllowNulls = true, Min = 3, Max = 100)]
        public string Name { get; set; }

        [NotNullable]
        [NotNullValidator]
        public PredictorSettingsEmbedded Settings { get; set; }

        [NotNullable, NotNullValidator]
        public PredictorAlgorithmSymbol Algorithm { get; set; }

        public PredictorResultSaverSymbol ResultSaver { get; set; }

        public Lite<ExceptionEntity> TrainingException { get; set; }

        [ImplementedBy(typeof(UserEntity))]
        public Lite<IUserEntity> User { get; set; }

        [ImplementedBy(typeof(NeuralNetworkSettingsEntity))]
        public IPredictorAlgorithmSettings AlgorithmSettings { get; set; }

        public PredictorState State { get; set; }

        [NotNullable]
        [NotNullValidator, InTypeScript(Undefined = false, Null = false), NotifyChildProperty]
        public PredictorMainQueryEmbedded MainQuery { get; set; }

        [Ignore, NotifyChildProperty, NotifyCollectionChanged] //virtual Mlist
        public MList<PredictorSubQueryEntity> SubQueries { get; set; } = new MList<PredictorSubQueryEntity>();
        
        [NotNullable, PreserveOrder]
        [NotNullValidator, NoRepeatValidator]
        public MList<FilePathEmbedded> Files { get; set; } = new MList<FilePathEmbedded>();

        public PredictorClassificationMetricsEmbedded ClassificationTraining { get; set; }
        public PredictorClassificationMetricsEmbedded ClassificationValidation { get; set; }
        public PredictorRegressionMetricsEmbedded RegressionTraining { get; set; }
        public PredictorRegressionMetricsEmbedded RegressionValidation { get; set; }


        static Expression<Func<PredictorEntity, string>> ToStringExpression = @this => @this.Name;
        [ExpressionField]
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    [Serializable]
    public class PredictorMainQueryEmbedded : EmbeddedEntity
    {
        public PredictorMainQueryEmbedded()
        {
            RebindEvents();
        }

        [Ignore]
        internal object queryName;

        [NotNullValidator]
        public QueryEntity Query { get; set; }

        [NotNullable, PreserveOrder]
        public MList<QueryFilterEmbedded> Filters { get; set; } = new MList<QueryFilterEmbedded>();

        [NotNullable, PreserveOrder]
        [NotNullValidator, NoRepeatValidator, NotifyChildProperty, NotifyCollectionChanged]
        public MList<PredictorColumnEmbedded> Columns { get; set; } = new MList<PredictorColumnEmbedded>();

        internal void ParseData(QueryDescription qd)
        {
            if (Filters != null)
                foreach (var f in Filters)
                    f.ParseData(this, qd, SubTokensOptions.CanAnyAll | SubTokensOptions.CanElement);

            if (Columns != null)
                foreach (var c in Columns)
                    c.ParseData(this, qd, SubTokensOptions.CanElement);
        }

        internal PredictorMainQueryEmbedded Clone() => new PredictorMainQueryEmbedded
        {
            Query = Query,
            Filters = Filters.Select(f => f.Clone()).ToMList(),
            Columns = Columns.Select(a => a.Clone()).ToMList(),
        };
    }

    [Serializable]
    public class PredictorClassificationMetricsEmbedded : EmbeddedEntity
    {
        public int TotalCount { get; set; }
        public int MissCount { get; set; }
        [Format("p2")]
        public double? MissRate { get; set; }

        protected override void PreSaving(ref bool graphModified)
        {
            MissRate = TotalCount == 0 ? (double?)null : Math.Round(MissCount / (double)TotalCount, 2);

            base.PreSaving(ref graphModified);
        }
    }

    [Serializable]
    public class PredictorRegressionMetricsEmbedded : EmbeddedEntity
    {
        public double? Signed { get; set; }

        [Unit("±")]
        public double? Absolute { get; set; }

        [Unit("±")]
        public double? Deviation { get; set; }

        [Format("p2")]
        public double? PercentageSigned { get; set; }

        [Format("p2"), Unit("±")]
        public double? PercentageAbsolute { get; set; }

        [Format("p2"), Unit("±")]
        public double? PercentageDeviation { get; set; }

    }

    [Serializable]
    public class PredictorSettingsEmbedded : EmbeddedEntity
    {
        [Format("p")]
        public double TestPercentage { get; set; } = 0.2;

        public int? Seed { get; set; }

        internal PredictorSettingsEmbedded Clone() => new PredictorSettingsEmbedded
        {
            TestPercentage = TestPercentage,
            Seed = Seed
        };
    }

    [AutoInit]
    public static class PredictorFileType
    {
        public static readonly FileTypeSymbol PredictorFile;
    }


    public interface IPredictorAlgorithmSettings : IEntity
    {
        IPredictorAlgorithmSettings Clone();
    }

    [AutoInit]
    public static class PredictorOperation
    {
        public static readonly ExecuteSymbol<PredictorEntity> Save;
        public static readonly ExecuteSymbol<PredictorEntity> Train;
        public static readonly ExecuteSymbol<PredictorEntity> CancelTraining;
        public static readonly ExecuteSymbol<PredictorEntity> StopTraining;
        public static readonly ExecuteSymbol<PredictorEntity> Untrain;
        public static readonly DeleteSymbol<PredictorEntity> Delete;
        public static readonly ConstructSymbol<PredictorEntity>.From<PredictorEntity> Clone;
    }

    [Serializable]
    public class PredictorColumnEmbedded : EmbeddedEntity
    {
        public PredictorColumnUsage Usage { get; set; }

        [NotNullable]
        [NotNullValidator]
        public QueryTokenEmbedded Token { get; set; }

        public PredictorColumnEncoding Encoding { get; set; }

        public PredictorColumnNullHandling NullHandling { get; set; }

        public void ParseData(ModifiableEntity context, QueryDescription description, SubTokensOptions options)
        {
            if (Token != null)
                Token.ParseData(context, description, options);
        }

        protected override string PropertyValidation(PropertyInfo pi)
        {
            return base.PropertyValidation(pi);
        }
        
        internal PredictorColumnEmbedded Clone() => new PredictorColumnEmbedded
        {
            Usage = Usage, 
            Token = Token.Clone(),
            Encoding = Encoding,
            NullHandling = NullHandling
        };

        public override string ToString() => $"{Usage} {Token} {Encoding}";
    }

    public enum PredictorColumnNullHandling
    {
        Zero,
        Error,
        Mean,
    }

    public enum PredictorColumnEncoding
    {
        None,
        OneHot,
        Codified,
        NormalizeZScore,
    }

    public enum PredictorState
    {
        Draft,
        Training, 
        Trained,
        Error,
    }

    public enum PredictorColumnUsage
    {
        Input,
        Output
    }


    [Serializable, EntityKind(EntityKind.Part, EntityData.Transactional)]
    public class PredictorSubQueryEntity : Entity
    {
        public PredictorSubQueryEntity()
        {
            RebindEvents();
        }

        [NotNullable]
        public Lite<PredictorEntity> Predictor { get; set; }

        [NotNullable, SqlDbType(Size = 100)]
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string Name { get; set; }

        [NotNullable, NotNullValidator]
        public QueryEntity Query { get; set; }

        [NotNullable, PreserveOrder]
        public MList<QueryFilterEmbedded> Filters { get; set; } = new MList<QueryFilterEmbedded>();
        
        [NotNullable, PreserveOrder]
        [NotNullValidator, NoRepeatValidator, NotifyChildProperty, NotifyCollectionChanged]
        public MList<PredictorSubQueryColumnEmbedded> Columns { get; set; } = new MList<PredictorSubQueryColumnEmbedded>();

        public void ParseData(QueryDescription description)
        {
            if (Filters != null)
                foreach (var f in Filters)
                    f.ParseData(this, description, SubTokensOptions.CanAnyAll | SubTokensOptions.CanElement | SubTokensOptions.CanAggregate);
            
            if (Columns != null)
                foreach (var a in Columns)
                    a.ParseData(this, description, SubTokensOptions.CanElement | SubTokensOptions.CanAggregate);
        }

        public PredictorSubQueryEntity Clone() => new PredictorSubQueryEntity
        {
            Name = Name,
            Query = Query,
            Filters = Filters.Select(f => f.Clone()).ToMList(),
            Columns = Columns.Select(f => f.Clone()).ToMList(),
        };

        static Expression<Func<PredictorSubQueryEntity, string>> ToStringExpression = @this => @this.Name;
        [ExpressionField]
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    [Serializable]
    public class PredictorSubQueryColumnEmbedded : EmbeddedEntity
    {
        public PredictorSubQueryColumnUsage Usage { get; set; }

        [NotNullable]
        [NotNullValidator]
        public QueryTokenEmbedded Token { get; set; }

        public PredictorColumnEncoding? Encoding { get; set; }

        public PredictorColumnNullHandling? NullHandling { get; set; }

        public void ParseData(ModifiableEntity context, QueryDescription description, SubTokensOptions options)
        {
            if (Token != null)
                Token.ParseData(context, description, options);
        }

        protected override string PropertyValidation(PropertyInfo pi)
        {
            stateValidator.Validate(this, pi);

            return base.PropertyValidation(pi);
        }

        public static StateValidator<PredictorSubQueryColumnEmbedded, PredictorSubQueryColumnUsage> stateValidator = 
            new StateValidator<PredictorSubQueryColumnEmbedded, PredictorSubQueryColumnUsage>
            (a => a.Usage, a => a.Encoding, a => a.NullHandling)
        {
            { PredictorSubQueryColumnUsage.Input, true, true },
            { PredictorSubQueryColumnUsage.Output,true, true },
            { PredictorSubQueryColumnUsage.SplitBy, false, false},
            { PredictorSubQueryColumnUsage.ParentKey, false, false },
        };

        internal PredictorSubQueryColumnEmbedded Clone() => new PredictorSubQueryColumnEmbedded
        {
            Usage = Usage,
            Token = Token.Clone(),
            Encoding = Encoding,
            NullHandling = NullHandling
        };

        public override string ToString() => $"{Usage} {Token} {Encoding}";
    }

    public enum PredictorSubQueryColumnUsage
    {
        ParentKey,
        SplitBy,
        Input,
        Output
    }

    public static class PredictorColumnUsageExtensions
    {
        public static PredictorColumnUsage ToPredictorColumnUsage(this PredictorSubQueryColumnUsage usage)
        {
            return usage == PredictorSubQueryColumnUsage.Input ? PredictorColumnUsage.Input :
                usage == PredictorSubQueryColumnUsage.Output ? PredictorColumnUsage.Output :
                throw new InvalidOperationException("Unexcpected " + nameof(PredictorSubQueryColumnUsage));
        }
    }

    [Serializable]
    public class PredictorAlgorithmSymbol : Symbol
    {
        private PredictorAlgorithmSymbol() { }

        public PredictorAlgorithmSymbol(Type declaringType, string fieldName) :
            base(declaringType, fieldName)
        {
        }
    }

    [Serializable]
    public class PredictorResultSaverSymbol : Symbol
    {
        private PredictorResultSaverSymbol() { }

        public PredictorResultSaverSymbol(Type declaringType, string fieldName) :
            base(declaringType, fieldName)
        {
        }
    }

    [AutoInit]
    public static class AccordPredictorAlgorithm
    {
        public static PredictorAlgorithmSymbol DiscreteNaiveBayes;
    }

    [AutoInit]
    public static class CNTKPredictorAlgorithm
    {
        public static PredictorAlgorithmSymbol NeuralNetwork;
    }

    [AutoInit]
    public static class PredictorResultSaver
    {
        public static PredictorResultSaverSymbol SimpleRegression;
        public static PredictorResultSaverSymbol SimpleClassification;
    }
}
