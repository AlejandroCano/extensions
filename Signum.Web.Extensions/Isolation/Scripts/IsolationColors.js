/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
define(["require", "exports"], function (require, exports) {
    function isolationColors(nodes) {
        return {
            getFill: function (t) { return t.extra["isolation"] == null ? "white" :
                t.extra["isolation"] == "Isolated" ? "#CC0099" :
                    t.extra["isolation"] == "Optional" ? "#9966FF" :
                        t.extra["isolation"] == "None" ? "#00CCFF" : "black"; },
            getTooltip: function (t) { return t.extra["isolation"] == null ? null : t.extra["isolation"]; }
        };
    }
    exports.isolationColors = isolationColors;
});
//# sourceMappingURL=IsolationColors.js.map