﻿using Signum.Entities;
using Signum.Entities.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Windows.Operations;
using System.Reflection;

namespace Signum.Windows.Alerts
{
    public static class AlertClient
    {
        public static Func<IEnumerable<Lite<IIdentifiable>>> LoadAlertType;

        public static void Start(Func<IEnumerable<Lite<IIdentifiable>>> loadAlertType = null, params Type[] types)
        {
            if (Navigator.Manager.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                if (types == null)
                    throw new ArgumentNullException("types");

                Navigator.AddSettings(new List<EntitySettings>
                {
                    new EntitySettings<AlertTypeDN> { View = e => new AlertType() },
                    new EntitySettings<AlertDN>
                    {
                        View = e => new Alert(),
                        IsCreable = EntityWhen.Never,
                        Icon = ExtensionsImageLoader.GetImageSortName("alert.png"),
                    }   
                });

                Server.SetSemiSymbolIds<AlertTypeDN>();
                LoadAlertType += loadAlertType;

                OperationClient.AddSettings(new List<OperationSettings> 
                {
                    new EntityOperationSettings<Entity>(AlertOperation.CreateAlertFromEntity){ IsVisible = a => false },
                    new EntityOperationSettings<AlertDN>(AlertOperation.SaveNew){ IsVisible = a => a.Entity.IsNew },
                    new EntityOperationSettings<AlertDN>(AlertOperation.Save){ IsVisible = a => !a.Entity.IsNew }
                });

                WidgetPanel.GetWidgets += (obj, mainControl) =>
                    (obj is Entity && types.Contains(obj.GetType()) && !((Entity)obj).IsNew) &&
                    Finder.IsFindable(typeof(AlertDN)) ? new AlertsWidget() : null;
            }
        }
    }
}
