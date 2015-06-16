/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
define(["require", "exports", "Framework/Signum.Web/Signum/Scripts/Finder", "Framework/Signum.Web/Signum/Scripts/Operations"], function (require, exports, Finder, Operations) {
    function initReplacements() {
        var self = this;
        $(".sf-word-template-container").on("click", ".sf-word-inserttoken", function () {
            var tokenName = Finder.QueryTokenBuilder.constructTokenName($(this).data("prefix"));
            if (SF.isEmpty(tokenName)) {
                return;
            }
            var tokenTag = constructTokenTag(tokenName, $(this).data("block"));
            window.prompt("Copy to clipboard: Ctrl+C, Enter", tokenTag);
        });
        $(".sf-word-template-container").on("sf-new-subtokens-combo", "select", function (event) {
            var idSelectedCombo = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                idSelectedCombo[_i - 1] = arguments[_i];
            }
            newSubTokensComboAdded.call(self, $("#" + idSelectedCombo[0]));
        });
    }
    exports.initReplacements = initReplacements;
    function constructTokenTag(tokenName, block) {
        if (typeof block == "undefined") {
            return "@[" + tokenName + "]";
        }
        else if (block === "if") {
            return "@if[" + tokenName + "]\r\n@else\r\n@endif";
        }
        else if (block === "foreach") {
            return "@foreach[" + tokenName + "]\r\n@endforeach";
        }
        else if (block === "any") {
            return "@any[" + tokenName + "=value]\r\n@notany\r\n@endany";
        }
        else {
            throw "invalid block name";
        }
    }
    ;
    function newSubTokensComboAdded($selectedCombo) {
        var $btnInsertBasic = $(".sf-word-inserttoken-basic");
        var $btnInsertIf = $(".sf-word-inserttoken-if");
        var $btnInsertForeach = $(".sf-word-inserttoken-foreach");
        var $btnInsertAny = $(".sf-word-inserttoken-any");
        var $selectedOption = $selectedCombo.children("option:selected");
        $selectedCombo.attr("title", $selectedOption.attr("title"));
        $selectedCombo.attr("style", $selectedOption.attr("style"));
        if ($selectedOption.val() == "") {
            var $prevSelect = $selectedCombo.prev("select");
            if ($prevSelect.length == 0) {
                changeButtonState($btnInsertBasic, lang.signum.selectToken);
                changeButtonState($btnInsertIf, lang.signum.selectToken);
                changeButtonState($btnInsertForeach, lang.signum.selectToken);
                changeButtonState($btnInsertAny, lang.signum.selectToken);
            }
            else {
                changeButtonState($btnInsertBasic);
                var $prevSelectedOption = $prevSelect.find("option:selected");
                changeButtonState($btnInsertIf, $prevSelectedOption.attr("data-if"));
                changeButtonState($btnInsertForeach, $prevSelectedOption.attr("data-foreach"));
                changeButtonState($btnInsertAny, $prevSelectedOption.attr("data-any"));
            }
            return;
        }
        changeButtonState($btnInsertBasic);
        changeButtonState($btnInsertIf, $selectedOption.attr("data-if"));
        changeButtonState($btnInsertForeach, $selectedOption.attr("data-foreach"));
        changeButtonState($btnInsertAny, $selectedOption.attr("data-any"));
    }
    exports.newSubTokensComboAdded = newSubTokensComboAdded;
    ;
    function changeButtonState($button, disablingMessage) {
        var hiddenId = $button.attr("id") + "temp";
        if (typeof disablingMessage != "undefined") {
            $button.attr("disabled", "disabled").attr("title", disablingMessage);
            $button.unbind('click').bind('click', function (e) { e.preventDefault(); return false; });
        }
        else {
            $button.attr("disabled", null).attr("title", "");
            $button.unbind('click');
        }
    }
    function createWordReportFromTemplate(options, event, findOptions, url, contextual) {
        Finder.find(findOptions).then(function (entity) {
            if (entity == null)
                return;
            options.requestExtraJsonData = { keys: entity.runtimeInfo.key() };
            SF.submit(url, contextual ? Operations.contextualRequestData(options) :
                Operations.entityRequestData(options), $("<form method='post'></form>"));
        });
    }
    exports.createWordReportFromTemplate = createWordReportFromTemplate;
});
//# sourceMappingURL=Word.js.map