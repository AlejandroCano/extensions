using System;
using Signum.Utilities;
using System.Reflection;
using Signum.Entities.Mailing;
using Signum.Entities.Basics;
using System.ComponentModel;

namespace Signum.Entities.Authorization
{
    [Serializable, EntityKind(EntityKind.Main, EntityData.Transactional)]
    public class UserEntity : Entity, IEmailOwnerEntity, IUserEntity
    {
        public static Func<string, string?> ValidatePassword = p =>
        {
            if (p.Length >= 5)
                return null;

            return LoginAuthMessage.ThePasswordMustHaveAtLeast0Characters.NiceToString(5);
        };

        public static string? OnValidatePassword(string password)
        {
            if (ValidatePassword != null)
                return ValidatePassword(password);

            return null;
        }

        [UniqueIndex(AvoidAttachToUniqueIndexes = true)]
        [StringLengthValidator(Min = 2, Max = 100)]
        public string UserName { get; set; }

        [DbType(Size = 128)]
        public byte[]? PasswordHash { get; set; }

        public Lite<RoleEntity> Role { get; set; }

        [StringLengthValidator(Max = 200), EMailValidator]
        public string? Email { get; set; }

        public CultureInfoEntity? CultureInfo { get; set; }

        public DateTime? DisabledOn { get; set; }

        public UserState State { get; set; } = UserState.New;

        protected override string? PropertyValidation(PropertyInfo pi)
        {
            if (pi.Name == nameof(State))
            {
                if (DisabledOn != null && State != UserState.Disabled)
                    return AuthAdminMessage.TheUserStateMustBeDisabled.NiceToString();
            }

            return base.PropertyValidation(pi);
        }

        public int LoginFailedCounter { get; set; }

        public static UserEntity Current
        {
            get { return (UserEntity)UserHolder.Current; }
            set { UserHolder.Current = value; }
        }

        [AutoExpressionField]
        public EmailOwnerData EmailOwnerData => As.Expression(() => new EmailOwnerData
        {
            Owner = this.ToLite(),
            CultureInfo = CultureInfo,
            DisplayName = UserName,
            Email = Email,
        });

        [AutoExpressionField]
        public override string ToString() => As.Expression(() => UserName);
    }

    public enum UserState
    {
        [Ignore]
        New = -1,
        Saved,
        Disabled,
    }

    [AutoInit]
    public static class UserOperation
    {
        public static ConstructSymbol<UserEntity>.Simple Create;
        public static ExecuteSymbol<UserEntity> Save;
        public static ExecuteSymbol<UserEntity> Enable;
        public static ExecuteSymbol<UserEntity> Disable;
        public static ExecuteSymbol<UserEntity> SetPassword;
        public static DeleteSymbol<UserEntity> Delete;
    }

    [Serializable]
    public class IncorrectUsernameException : ApplicationException
    {
        public IncorrectUsernameException() { }
        public IncorrectUsernameException(string message) : base(message) { }
        protected IncorrectUsernameException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }
    
    [Serializable]
    public class UserLockedException : ApplicationException
    {
        public UserLockedException() { }
        public UserLockedException(string message) : base(message) { }
        protected UserLockedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }

    [Serializable]
    public class IncorrectPasswordException : ApplicationException
    {
        public IncorrectPasswordException() { }
        public IncorrectPasswordException(string message) : base(message) { }
        protected IncorrectPasswordException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }


    [Serializable]
    public class UserOIDMixin : MixinEntity
    {
        public static bool AllowUsersWithPassswordAndOID = false;

        UserOIDMixin(ModifiableEntity mainEntity, MixinEntity? next)
            : base(mainEntity, next)
        {
        }

        [UniqueIndex(AllowMultipleNulls = true)]
        public Guid? OID { get; set; }

        protected override string? PropertyValidation(PropertyInfo pi)
        {
            if (pi.Name == nameof(OID) && OID != null && ((UserEntity)this.MainEntity).PasswordHash != null && !AllowUsersWithPassswordAndOID)
                return UserOIDMessage.TheUser0IsConnectedToActiveDirectoryAndCanNotHaveALocalPasswordSet.NiceToString(this.MainEntity);

            return base.PropertyValidation(pi);
        }
    }

    public enum UserOIDMessage
    {
        [Description("The user {0} is connected to Active Directory and can not have a local password set")]
        TheUser0IsConnectedToActiveDirectoryAndCanNotHaveALocalPasswordSet
    }
}
