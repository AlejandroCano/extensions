/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
define(["require", "exports"], function(require, exports) {
    function initDashboard(url) {
        var refreshCallback = function () {
            $.get(url, function (data) {
                $("div.emailAsyncMainDiv").replaceWith(data);
            });
        };

        var $processEnable = $("#sfEmailAsyncProcessEnable");
        var $processDisable = $("#sfEmailAsyncProcessDisable");

        $processEnable.click(function (e) {
            e.preventDefault();
            $.ajax({
                url: $(this).attr("href"),
                success: function () {
                    $processEnable.hide();
                    $processDisable.show();
                    refreshCallback();
                }
            });
        });

        $processDisable.click(function (e) {
            e.preventDefault();
            $.ajax({
                url: $(this).attr("href"),
                success: function () {
                    $processDisable.hide();
                    $processEnable.show();
                    refreshCallback();
                }
            });
        });
    }
    exports.initDashboard = initDashboard;
});
//# sourceMappingURL=AsyncEmailSender.js.map
