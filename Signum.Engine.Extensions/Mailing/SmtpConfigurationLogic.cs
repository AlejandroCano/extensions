using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Engine.Maps;
using Signum.Utilities.Reflection;
using System.Reflection;
using Signum.Engine.DynamicQuery;
using Signum.Entities.Mailing;
using Signum.Entities;
using Signum.Utilities;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Signum.Engine.Operations;
using Signum.Services;

namespace Signum.Engine.Mailing
{
    public static class SmtpConfigurationLogic
    {
        public static ResetLazy<Dictionary<Lite<SmtpConfigurationEntity>, SmtpConfigurationEntity>> SmtpConfigCache;

        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                sb.Include<SmtpConfigurationEntity>()
                    .WithQuery(dqm, s => new
                    {
                        Entity = s,
                        s.Id,
                        s.DeliveryMethod,
                        s.Network.Host,
                        s.Network.Username,
                        s.PickupDirectoryLocation
                    });
                
                SmtpConfigCache = sb.GlobalLazy(() => Database.Query<SmtpConfigurationEntity>().ToDictionary(a => a.ToLite()),
                    new InvalidateWith(typeof(SmtpConfigurationEntity)));

                new Graph<SmtpConfigurationEntity>.Execute(SmtpConfigurationOperation.Save)
                {
                    AllowsNew = true,
                    Lite = false,
                    Execute = (sc, _) => 
                    {
                        if (sc.IsNew && sc.Network != null && sc.Network.Password.HasText())
                        {
                            var ce = new CryptorEngine(SmtpConfigurationEntity.GetCriptoKey());
                            sc.Network.Password = ce.Encrypt(sc.Network.Password);
                        }
                    },
                }.Register();

                new Graph<SmtpConfigurationEntity>.Execute(SmtpConfigurationOperation.SetPassword)
                {
                    AllowsNew = false,
                    Lite = true,
                    Execute = (sc, args) =>
                    {
                        var password = args.GetArg<string>();
                        var ce = new CryptorEngine(SmtpConfigurationEntity.GetCriptoKey());
                        sc.Network.Password = ce.Encrypt(password);
                    },
                }.Register();
            }
        }

        public static SmtpClient GenerateSmtpClient(this Lite<SmtpConfigurationEntity> config)
        {
            return config.RetrieveFromCache().GenerateSmtpClient();
        }

        public static SmtpConfigurationEntity RetrieveFromCache(this Lite<SmtpConfigurationEntity> config)
        {
            return SmtpConfigCache.Value.GetOrThrow(config);
        }

        public static SmtpClient GenerateSmtpClient(this SmtpConfigurationEntity config)
        {
            if (config.DeliveryMethod != SmtpDeliveryMethod.Network)
            {
                return new SmtpClient
                {
                    DeliveryFormat = config.DeliveryFormat,
                    DeliveryMethod = config.DeliveryMethod,
                    PickupDirectoryLocation = config.PickupDirectoryLocation,
                };
            }
            else
            {
                SmtpClient client = EmailLogic.SafeSmtpClient(config.Network.Host, config.Network.Port);
                client.DeliveryFormat = config.DeliveryFormat;
                client.UseDefaultCredentials = config.Network.UseDefaultCredentials;

                if (config.Network.Username.HasText())
                {
                    var ce = new CryptorEngine(SmtpConfigurationEntity.GetCriptoKey());
                    var password = ce.Decrypt(config.Network.Password);

                    client.Credentials = new NetworkCredential(config.Network.Username, password);
                }
                else
                {
                    client.Credentials = null;
                }

                client.EnableSsl = config.Network.EnableSSL;

                foreach (var cc in config.Network.ClientCertificationFiles)
                {
                    client.ClientCertificates.Add(cc.CertFileType == CertFileType.CertFile ?
                        X509Certificate.CreateFromCertFile(cc.FullFilePath)
                        : X509Certificate.CreateFromSignedFile(cc.FullFilePath));
                }

                return client;
            }
        }
    }
}
