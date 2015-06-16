/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
define(["require", "exports", "Extensions/Signum.Web.Extensions/Maps/Scripts/SchemaMap"], function (require, exports, Map) {
    function cacheColors(nodes, title, key) {
        var max = nodes.map(function (a) { return a.extra[key]; }).filter(function (n) { return n != undefined; }).max();
        var color = Map.colorScale(max);
        return {
            getFill: function (t) { return t.extra["cache-semi"] == undefined ? "lightgray" : color(t.extra[key]); },
            getMask: function (t) { return t.extra["cache-semi"] == undefined ? null :
                t.extra["cache-semi"] ? "url(#mask-stripe)" : null; },
            getTooltip: function (t) { return t.extra["cache-semi"] == undefined ? "NO Cached" :
                t.extra["cache-semi"] == true ? ("SEMI Cached - " + t.extra[key] + "  " + title) :
                    ("Cached - " + t.extra[key] + "  " + title); }
        };
    }
    exports.cacheColors = cacheColors;
});
//# sourceMappingURL=CacheColors.js.map