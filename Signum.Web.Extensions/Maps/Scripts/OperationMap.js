/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
/// <reference path="../../../../Framework/Signum.Web/Signum/Headers/d3/d3.d.ts"/>
define(["require", "exports", "d3", "Extensions/Signum.Web.Extensions/Maps/Scripts/SchemaMap"], function (require, exports, d3, SchemaMap) {
    function getOperation(line) {
        return line.isFrom ? line.source : line.target;
    }
    function similarLinks(line) {
        return line.isFrom ?
            line.target.fromStates.length :
            line.source.toStates.length;
    }
    function createMap(mapId, svgMapId, colorId, map) {
        var div = mapId.get();
        var colorCombo = colorId.get();
        //div.closest(".container").removeClass("container").addClass("container-fluid");
        div.css("width", "100%");
        div.css("height", (window.innerHeight - 200) + "px");
        var width = div.width(), height = div.height();
        var nodes = map.operations.concat(map.states);
        var nodesDic = map.states.toObject(function (g) { return g.key; });
        var fromRelationships = map.operations.filter(function (op) { return op.fromStates != null; })
            .flatMap(function (op) { return op.fromStates.map(function (s) { return { source: nodesDic[s], target: op, isFrom: true }; }); });
        var toRelationships = map.operations.filter(function (op) { return op.toStates != null; })
            .flatMap(function (op) { return op.toStates.map(function (s) { return { source: op, target: nodesDic[s], isFrom: false }; }); });
        var links = fromRelationships.concat(toRelationships);
        var opacities = [1, .5, .3, .2, .1];
        var fanOut = map.operations.flatMap(function (a) { return a.fromStates.map(function (s) { return ({ s: s, weight: 1.0 / a.fromStates.length }); }); }).groupByObject(function (a) { return a.s; });
        var fanIn = map.operations.flatMap(function (a) { return a.toStates.map(function (s) { return ({ s: s, weight: 1.0 / a.toStates.length }); }); }).groupByObject(function (a) { return a.s; });
        map.states.forEach(function (m) {
            m.fanOut = (fanOut[m.key] ? fanOut[m.key].reduce(function (acum, e) { return acum + e.weight; }, 0) : 0);
            m.fanIn = (fanIn[m.key] ? fanIn[m.key].reduce(function (acum, e) { return acum + e.weight; }, 0) : 0);
        });
        var force = d3.layout.force()
            .gravity(0)
            .linkDistance(80)
            .charge(-600)
            .size([width, height]);
        var colorStates = SchemaMap.colorScale(map.states.map(function (a) { return a.count; }).max());
        var colorOperations = SchemaMap.colorScale(map.operations.map(function (a) { return a.count; }).max());
        force
            .nodes(nodes)
            .links(links)
            .start();
        var svg = d3.select("#" + svgMapId)
            .attr("width", width)
            .attr("height", height);
        var link = svg.append("svg:g").attr("class", "links").selectAll(".link")
            .data(links)
            .enter().append("line")
            .attr("class", "link")
            .style("stroke", "black")
            .attr("marker-end", function (d) { return d.isFrom ? null : "url(#normal_arrow)"; });
        var selectedTable;
        function selectedLinks() {
            link.style("stroke-width", function (d) { return d.source == selectedTable || d.target == selectedTable ? 1.5 : 1; })
                .style("opacity", function (d) { return d.source == selectedTable || d.target == selectedTable ? 1 :
                Math.max(.1, opacities[Math.min(similarLinks(d), opacities.length - 1)]); });
        }
        ;
        selectedLinks();
        var statesGroup = svg.append("svg:g").attr("class", "states")
            .selectAll(".stateGroup")
            .data(map.states)
            .enter()
            .append("svg:g").attr("class", "stateGroup")
            .style("cursor", function (d) { return d.link ? "pointer" : null; })
            .on("click", function (d) {
            selectedTable = selectedTable == d ? null : d;
            selectedLinks();
            selectedNode();
            var event = d3.event;
            if (event.defaultPrevented)
                return;
            if (event.ctrlKey && d.link) {
                window.open(d.link);
                d3.event.preventDefault();
                return false;
            }
        }).call(force.drag);
        var labelStates;
        {
            var nodeStates = statesGroup.append("rect")
                .attr("class", function (d) { return "state " + (d.key == "DefaultState.Start" ? "start" :
                d.key == "DefaultState.All" ? "all" :
                    d.key == "DefaultState.End" ? "end" :
                        d.ignored ? "ignore" : null); })
                .attr("rx", 5)
                .attr('fill-opacity', 0.1);
            //.style("fill", "white")
            //.style("stroke", "black");
            function onStateColorChange() {
                function color(d) {
                    if (colorCombo.val() == "rows")
                        return colorStates(d.count);
                    else
                        return d.color || (d.key.startsWith("DefaltState") ? null : "lightgray");
                }
                nodeStates
                    .attr('stroke', color)
                    .attr('fill', color);
            }
            colorCombo.change(onStateColorChange);
            onStateColorChange();
            var margin = 3;
            labelStates = statesGroup.append("text")
                .attr("class", "state")
                .style("cursor", function (d) { return d.link ? "pointer" : null; })
                .text(function (d) { return d.niceName; })
                .each(function (d) {
                SchemaMap.wrap(this, 60);
                var b = this.getBBox();
                d.width = b.width + margin * 2;
                d.height = b.height + margin * 2;
            });
            nodeStates.attr("width", function (d) { return d.width; })
                .attr("height", function (d) { return d.height; });
            labelStates.attr("transform", function (d) { return "translate(" + d.width / 2 + ", 0)"; });
            labelStates.append('svg:title')
                .text(function (t) { return t.niceName + " (" + t.count + ")"; });
        }
        var operationsGroup = svg.append("svg:g").attr("class", "operations")
            .selectAll(".operation")
            .data(map.operations)
            .enter()
            .append("svg:g").attr("class", "operation")
            .style("cursor", function (d) { return d.link ? "pointer" : null; })
            .on("click", function (d) {
            selectedTable = selectedTable == d ? null : d;
            selectedLinks();
            selectedNode();
            var event = d3.event;
            if (event.defaultPrevented)
                return;
            if (event.ctrlKey && d.link) {
                window.open(d.link);
                d3.event.preventDefault();
                return false;
            }
        }).call(force.drag);
        var labelOperations;
        {
            var nodeOperations = operationsGroup.append("rect")
                .attr("class", "operation");
            var margin = 1;
            labelOperations = operationsGroup.append("text")
                .attr("class", "operation")
                .style("cursor", function (d) { return d.link ? "pointer" : null; })
                .text(function (d) { return d.niceName; })
                .each(function (d) {
                SchemaMap.wrap(this, 60);
                var b = this.getBBox();
                d.width = b.width + margin * 2;
                d.height = b.height + margin * 2;
            });
            function onOperationColorChange() {
                function color(d) {
                    if (colorCombo.val() == "rows")
                        return colorOperations(d.count);
                    else
                        return "transparent";
                }
                nodeOperations
                    .attr('stroke', color)
                    .attr('fill', color);
            }
            colorCombo.change(onOperationColorChange);
            onOperationColorChange();
            nodeOperations.attr("width", function (d) { return d.width; })
                .attr("height", function (d) { return d.height; });
            labelOperations.attr("transform", function (d) { return "translate(" + d.width / 2 + ", 0)"; });
            labelOperations.append('svg:title')
                .text(function (t) { return t.niceName + " (" + t.count + ")"; });
        }
        function selectedNode() {
            labelStates.style("font-weight", function (d) { return d == selectedTable ? "bold" : null; });
            labelOperations.style("font-weight", function (d) { return d == selectedTable ? "bold" : null; });
        }
        force.on("tick", function () {
            nodes.forEach(function (d) {
                d.nx = d.x;
                d.ny = d.y;
            });
            gravity();
            nodes.forEach(function (d) {
                d.x = d.nx;
                d.y = d.ny;
            });
            fanInOut();
            link.each(function (rel) {
                rel.sourcePoint = SchemaMap.calculatePoint(rel.source, rel.target);
                rel.targetPoint = SchemaMap.calculatePoint(rel.target, rel.source);
            });
            link.attr("x1", function (l) { return l.sourcePoint.x; })
                .attr("y1", function (l) { return l.sourcePoint.y; })
                .attr("x2", function (l) { return l.targetPoint.x; })
                .attr("y2", function (l) { return l.targetPoint.y; });
            statesGroup.attr("transform", function (d) { return "translate(" + (d.x - d.width / 2) + ", " + (d.y - d.height / 2) + ")"; });
            operationsGroup.attr("transform", function (d) { return "translate(" + (d.x - d.width / 2) + ", " + (d.y - d.height / 2) + ")"; });
        });
        var fanInConstant = 0.05;
        function fanInOut() {
            map.states.forEach(function (d) {
                if (d.fanOut > 0)
                    d.y -= d.y * d.fanOut * fanInConstant * force.alpha();
                if (d.fanIn > 0)
                    d.y += (height - d.y) * d.fanIn * fanInConstant * force.alpha();
            });
        }
        function gravity() {
            function gravityDim(v, min, max) {
                var minF = min + 100;
                var maxF = max - 100;
                var dist = maxF < v ? maxF - v :
                    v < minF ? minF - v : 0;
                return dist * force.alpha() * 0.4;
            }
            nodes.forEach(function (n) {
                n.nx += gravityDim(n.x, 0, width);
                n.ny += gravityDim(n.y, 0, height);
            });
        }
    }
    exports.createMap = createMap;
});
//# sourceMappingURL=OperationMap.js.map