﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Signum.Engine.Authorization;
using Signum.Entities;
using Signum.Entities.Authorization;
using Signum.Services;
using Signum.Utilities;
using Signum.React.Facades;
using Signum.React.Authorization;
using Signum.Engine.Cache;
using Signum.Engine;
using Signum.Entities.Cache;
using Signum.Utilities.ExpressionTrees;

namespace Signum.React.Cache
{
    public class CacheController : ApiController
    {
        [Route("api/cache/view"), HttpGet]
        public CacheStateTS View()
        {
            CachePermission.ViewCache.AssertAuthorized();

            var tables = CacheLogic.Statistics().Select(ctb => new CacheTableTS(ctb)).ToList();

            return new CacheStateTS
            {
                isEnabled = !CacheLogic.GloballyDisabled,
                tables = tables
            };
        }

        [Route("api/cache/enable"), HttpPost]
        public void Enable()
        {
            CachePermission.ViewCache.AssertAuthorized();

            CacheLogic.GloballyDisabled = false;

        }

        [Route("api/cache/disable"), HttpPost]
        public void Disable()
        {
            CachePermission.ViewCache.AssertAuthorized();

            CacheLogic.GloballyDisabled = true;

        }

        [Route("api/cache/clear"), HttpPost]
        public void Clear()
        {
            CachePermission.InvalidateCache.AssertAuthorized();

            CacheLogic.ForceReset();
            GlobalLazy.ResetAll();
            GC.Collect(2);
        }
    }

    public class CacheStateTS
    {
        public bool isEnabled;
        public List<CacheTableTS> tables;
    }

    public class CacheTableTS
    {
        public string tableName;
        public string typeName;
        public int? count;
        public int hits;
        public int invalidations;
        public int loads;
        public string sumLoadTime;
        public List<CacheTableTS> subTables;

        public CacheTableTS(CachedTableBase ct)
        {
            this.tableName = ct.Table.Name.Name;
            this.typeName = ct.Type.TypeName();
            this.count = ct.Count;
            this.hits = ct.Hits;
            this.invalidations = ct.Invalidations;
            this.loads = ct.Loads;
            this.sumLoadTime = ct.SumLoadTime.NiceToString();

            if (ct.SubTables != null)
                this.subTables = ct.SubTables.Select(ctv => new CacheTableTS(ctv)).ToList();
        }
    }
}