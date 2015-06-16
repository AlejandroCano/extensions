/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
define(["require", "exports"], function (require, exports) {
    function disconnectedColors(nodes) {
        return {
            getFill: function (t) { return t.extra["disc-upload"] == null ? "white" : "url(#disconnected-" + t.extra["disc-upload"] + "-" + t.extra["disc-download"] + ")"; },
            getTooltip: function (t) { return t.extra["disc-upload"] == null ? "" : "Download " + t.extra["disc-download"] + " - " + "Upload " + t.extra["disc-upload"]; }
        };
    }
    exports.disconnectedColors = disconnectedColors;
});
//# sourceMappingURL=DisconnectedColors.js.map