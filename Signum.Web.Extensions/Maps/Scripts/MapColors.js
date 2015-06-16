/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
/// <reference path="../../../../Framework/Signum.Web/Signum/Headers/d3/d3.d.ts"/>
define(["require", "exports", "d3", "Extensions/Signum.Web.Extensions/Maps/Scripts/SchemaMap"], function (require, exports, d3, Map) {
    function namespace(nodes) {
        var color = d3.scale.category20();
        return {
            getFill: function (t) { return color(t.namespace); },
            getTooltip: function (t) { return t.namespace; }
        };
    }
    exports.namespace = namespace;
    function tableSize(nodes) {
        var color = Map.colorScale(nodes.map(function (a) { return a.total_size_kb; }).max());
        return {
            getFill: function (t) { return color(t.total_size_kb); },
            getTooltip: function (t) { return t.total_size_kb + " KB"; }
        };
    }
    exports.tableSize = tableSize;
    function rows(nodes, rowsText) {
        var color = Map.colorScale(nodes.map(function (a) { return a.rows; }).max());
        return {
            getFill: function (t) { return color(t.rows); },
            getTooltip: function (t) { return t.rows + " " + rowsText; }
        };
    }
    exports.rows = rows;
    function columns(nodes, columnsText) {
        var color = Map.colorScale(nodes.map(function (a) { return a.columns; }).max());
        return {
            getFill: function (t) { return color(t.columns); },
            getTooltip: function (t) { return t.columns + " " + columnsText; }
        };
    }
    exports.columns = columns;
    function entityKind(nodes) {
        var f = {};
        f[Map.EntityKind.SystemString] = "#8c564b";
        f[Map.EntityKind.System] = "#7f7f7f";
        f[Map.EntityKind.Relational] = "#17becf";
        f[Map.EntityKind.String] = "#e377c2";
        f[Map.EntityKind.Shared] = "#2ca02c";
        f[Map.EntityKind.Main] = "#d62728";
        f[Map.EntityKind.Part] = "#ff7f0e";
        f[Map.EntityKind.SharedPart] = "#bcbd22";
        return {
            getFill: function (t) { return f[t.entityKind]; },
            getTooltip: function (t) { return Map.EntityKind[t.entityKind]; }
        };
    }
    exports.entityKind = entityKind;
    function entityData(nodes) {
        return {
            getFill: function (t) {
                return t.entityData == Map.EntityData.Master ? "#2ca02c" :
                    t.entityData == Map.EntityData.Transactional ? "#d62728" : "black";
            },
            getTooltip: function (t) { return Map.EntityData[t.entityData]; }
        };
    }
    exports.entityData = entityData;
});
//# sourceMappingURL=MapColors.js.map