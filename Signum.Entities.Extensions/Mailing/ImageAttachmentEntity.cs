﻿using Signum.Entities.Files;
using Signum.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Signum.Entities.Mailing
{
    [Serializable, EntityKind(EntityKind.Part, EntityData.Master)]
    public class ImageAttachmentEntity : Entity, IAttachmentGeneratorEntity
    {
        [Ignore]
        internal object FileNameNode;

        [SqlDbType(Size = 100)]
        string fileName;
        [StringLengthValidator(AllowNulls = true, Min = 3, Max = 100), FileNameValidator]
        public string FileName
        {
            get { return fileName; }
            set
            {
                if (Set(ref fileName, value))
                    FileNameNode = null;
            }
        }

        [NotNullable, SqlDbType(Size = 300)]
        [StringLengthValidator(AllowNulls = false, Min = 1, Max = 300)]
        public string ContentId { get; set; }
        
        [NotNullable]
        [NotNullValidator]
        public FileEmbedded File { get; set; }

        static Expression<Func<ImageAttachmentEntity, string>> ToStringExpression = @this => @this.FileName ?? @this.File.FileName;
        [ExpressionField]
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }
}
