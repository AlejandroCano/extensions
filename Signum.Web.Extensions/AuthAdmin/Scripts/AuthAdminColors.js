/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
define(["require", "exports"], function (require, exports) {
    function authAdminColor(nodes, key) {
        return {
            getFill: function (t) { return t.extra[key + "-db"] == null ? "white" : "url(#" + t.extra[key + "-db"] + ")"; },
            getStroke: function (t) { return t.extra[key + "-ui"] == null ? "white" : "url(#" + t.extra[key + "-ui"] + ")"; },
            getTooltip: function (t) { return t.extra[key + "-tooltip"] == null ? null : t.extra[key + "-tooltip"]; }
        };
    }
    exports.authAdminColor = authAdminColor;
});
//# sourceMappingURL=AuthAdminColors.js.map