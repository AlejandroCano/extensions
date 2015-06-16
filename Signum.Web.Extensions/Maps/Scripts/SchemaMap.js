/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
/// <reference path="../../../../Framework/Signum.Web/Signum/Headers/d3/d3.d.ts"/>
define(["require", "exports", "d3"], function (require, exports, d3) {
    (function (EntityKind) {
        EntityKind[EntityKind["SystemString"] = 0] = "SystemString";
        EntityKind[EntityKind["System"] = 1] = "System";
        EntityKind[EntityKind["Relational"] = 2] = "Relational";
        EntityKind[EntityKind["String"] = 3] = "String";
        EntityKind[EntityKind["Shared"] = 4] = "Shared";
        EntityKind[EntityKind["Main"] = 5] = "Main";
        EntityKind[EntityKind["Part"] = 6] = "Part";
        EntityKind[EntityKind["SharedPart"] = 7] = "SharedPart";
    })(exports.EntityKind || (exports.EntityKind = {}));
    var EntityKind = exports.EntityKind;
    (function (EntityData) {
        EntityData[EntityData["Master"] = 0] = "Master";
        EntityData[EntityData["Transactional"] = 1] = "Transactional";
    })(exports.EntityData || (exports.EntityData = {}));
    var EntityData = exports.EntityData;
    (function (EntityBaseType) {
        EntityBaseType[EntityBaseType["EnumEntity"] = 0] = "EnumEntity";
        EntityBaseType[EntityBaseType["Symbol"] = 1] = "Symbol";
        EntityBaseType[EntityBaseType["SemiSymbol"] = 2] = "SemiSymbol";
        EntityBaseType[EntityBaseType["Entity"] = 3] = "Entity";
        EntityBaseType[EntityBaseType["MList"] = 4] = "MList";
        EntityBaseType[EntityBaseType["Part"] = 5] = "Part";
    })(exports.EntityBaseType || (exports.EntityBaseType = {}));
    var EntityBaseType = exports.EntityBaseType;
    function createMap(mapId, svgMapId, filterId, colorId, map) {
        var getProvider = window["getProvider"];
        var div = mapId.get();
        var filter = filterId.get();
        var colorCombo = colorId.get();
        div.closest(".container").removeClass("container").addClass("container-fluid");
        div.css("width", "100%");
        div.css("height", (window.innerHeight - 200) + "px");
        var width = div.width(), height = div.height();
        map.tables.forEach(function (t) { return t.mlistTables.forEach(function (ml) {
            ml.entityKind = t.entityKind;
            ml.entityData = t.entityData;
            ml.entityBaseType = EntityBaseType.MList;
            ml.namespace = t.namespace;
        }); });
        var allNodes = map.tables.concat(map.tables.flatMap(function (t) { return t.mlistTables; }));
        var nodesDic = allNodes.toObject(function (g) { return g.tableName; });
        map.relations.forEach(function (a) {
            a.source = nodesDic[a.fromTable];
            a.target = nodesDic[a.toTable];
        });
        var allLinks = map.relations.map(function (a) { return a; })
            .concat(map.tables.flatMap(function (t) { return t.mlistTables.map(function (tm) { return { source: t, target: tm, isMList: true }; }); }));
        var fanIn = map.relations.groupByObject(function (a) { return a.toTable; });
        var opacities = [1, .9, .8, .7, .6, .5, .4, .3, .25, .2, .15, .1, .07, .05, .03, .02];
        var force = d3.layout.force()
            .gravity(0)
            .charge(0)
            .linkDistance(function (d) { return d.isMList ? 30 : 60; })
            .linkStrength(function (d) { return 0.7 * (d.isMList ? 1 : opacities[Math.min(fanIn[d.toTable].length, opacities.length - 1)]); })
            .size([width, height]);
        var nodes;
        var links;
        function restart() {
            var val = filter.val().toLowerCase();
            nodes = allNodes.filter(function (n, i) { return val == null ||
                n.namespace.toLowerCase().contains(val) ||
                n.tableName.toLowerCase().contains(val) ||
                n.niceName.toLowerCase().contains(val); });
            links = allLinks.filter(function (l) {
                return nodes.indexOf(l.source) != -1 &&
                    nodes.indexOf(l.target) != -1;
            });
            force
                .nodes(nodes)
                .links(links)
                .start();
        }
        restart();
        var selectedTable;
        var svg = d3.select("#" + svgMapId)
            .attr("width", width)
            .attr("height", height);
        var link = svg.append("svg:g").attr("class", "links").selectAll(".link")
            .data(links)
            .enter().append("line")
            .attr("class", "link")
            .style("stroke-dasharray", function (d) { return d.lite ? "2, 2" : null; })
            .style("stroke", "black")
            .attr("marker-end", function (d) { return "url(#" + (d.isMList ? "mlist_arrow" : d.lite ? "lite_arrow" : "normal_arrow") + ")"; });
        function selectedLinks() {
            link.style("stroke-width", function (d) { return d.source == selectedTable || d.target == selectedTable ? 1.5 : d.isMList ? 1.5 : 1; })
                .style("opacity", function (d) { return d.source == selectedTable || d.target == selectedTable ? 1 : d.isMList ? 0.8 :
                Math.max(.1, opacities[Math.min(fanIn[d.toTable].length, opacities.length - 1)]); });
        }
        ;
        selectedLinks();
        var nodesG = svg.append("svg:g").attr("class", "nodes");
        var nodeGroup = nodesG.selectAll(".nodeGroup")
            .data(nodes)
            .enter()
            .append("svg:g").attr("class", "nodeGroup")
            .style("cursor", function (d) { return d.findUrl ? "pointer" : null; })
            .on("click", function (d) {
            selectedTable = selectedTable == d ? null : d;
            selectedLinks();
            selectedNode();
            var event = d3.event;
            if (event.defaultPrevented)
                return;
            if (event.ctrlKey && d.findUrl) {
                window.open(d.findUrl);
                d3.event.preventDefault();
                return false;
            }
        }).call(force.drag);
        var node = nodeGroup.append("rect")
            .attr("class", function (d) { return "node " + EntityBaseType[d.entityBaseType]; })
            .attr("rx", function (n) {
            return n.entityBaseType == EntityBaseType.Entity ? 7 :
                n.entityBaseType == EntityBaseType.Part ? 4 :
                    n.entityBaseType == EntityBaseType.SemiSymbol ? 5 :
                        n.entityBaseType == EntityBaseType.Symbol ? 4 :
                            n.entityBaseType == EntityBaseType.EnumEntity ? 3 : 0;
        });
        var margin = 3;
        var label = nodeGroup.append("text")
            .attr("class", function (d) { return "node " + EntityBaseType[d.entityBaseType]; })
            .style("cursor", function (d) { return d.findUrl ? "pointer" : null; })
            .text(function (d) { return d.niceName; })
            .each(function (d) {
            wrap(this, 60);
            var b = this.getBBox();
            d.width = b.width + margin * 2;
            d.height = b.height + margin * 2;
        });
        node.attr("width", function (d) { return d.width; })
            .attr("height", function (d) { return d.height; });
        function selectedNode() {
            label.style("font-weight", function (d) { return d == selectedTable ? "bold" : null; });
        }
        filter.keypress(function () {
            restart();
            nodeGroup.style("display", function (n) { return nodes.indexOf(n) == -1 ? "none" : "inline"; });
            link.style("display", function (r) { return links.indexOf(r) == -1 ? "none" : "inline"; });
        });
        label.attr("transform", function (d) { return "translate(" + d.width / 2 + ", 0)"; });
        var titles = label.append('svg:title');
        function drawColor() {
            var colorVal = colorCombo.val();
            getProvider(colorVal, nodes).then(function (cp) {
                node.style("fill", cp.getFill)
                    .style("stroke", cp.getStroke || cp.getFill)
                    .style("mask", cp.getMask);
                titles.text(function (t) { return cp.getTooltip(t) + " (" + EntityBaseType[t.entityBaseType] + ")"; });
            });
        }
        ;
        drawColor();
        colorCombo.change(function () { return drawColor(); });
        force.on("tick", function () {
            nodes.forEach(function (d) {
                d.nx = d.x;
                d.ny = d.y;
            });
            namespaceClustering();
            gravity();
            nodes.forEach(function (d) {
                d.x = d.nx;
                d.y = d.ny;
            });
            link.each(function (rel) {
                rel.sourcePoint = calculatePoint(rel.source, rel.target);
                rel.targetPoint = calculatePoint(rel.target, rel.source);
            });
            link.attr("x1", function (l) { return l.sourcePoint.x; })
                .attr("y1", function (l) { return l.sourcePoint.y; })
                .attr("x2", function (l) { return l.targetPoint.x; })
                .attr("y2", function (l) { return l.targetPoint.y; });
            nodeGroup.attr("transform", function (d) { return "translate(" + (d.x - d.width / 2) + ", " + (d.y - d.height / 2) + ")"; });
        });
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
        function namespaceClustering() {
            var quadtree = d3.geom.quadtree(nodes, width, height);
            var constant = nodes.length < 10 ? 100 :
                nodes.length < 20 ? 50 :
                    nodes.length < 50 ? 30 :
                        nodes.length < 100 ? 20 :
                            nodes.length < 200 ? 15 : 10;
            nodes.forEach(function (d) {
                quadtree.visit(function (quad, x1, y1, x2, y2) {
                    if (quad.point && quad.point != d) {
                        var x = d.x - quad.point.x, y = d.y - quad.point.y, l = Math.sqrt(x * x + y * y);
                        var lx = x / l;
                        var ly = y / l;
                        var ratio = l / 30;
                        var f = constant * force.alpha() / Math.max(ratio * ratio, 0.1);
                        if (d.namespace != quad.point.namespace)
                            f *= 4;
                        d.nx += lx * f;
                        d.ny += ly * f;
                    }
                    var dx = distance(d.x, x1, x2);
                    var dy = distance(d.y, y1, y2);
                    var dist = Math.sqrt(dx * dx + dy * dy);
                    return dist > 400;
                    return false;
                });
            });
        }
    }
    exports.createMap = createMap;
    function distance(v, min, max) {
        if (v < min)
            return min - v;
        if (max < v)
            return v - max;
        return 0;
    }
    function wrap(textElement, width) {
        var text = d3.select(textElement);
        var words = text.text().split(/\s+/).reverse();
        var word;
        var line = [];
        var tspan = text.text(null).append("tspan")
            .attr("x", 0)
            .attr("dy", "1.2em");
        while (word = words.pop()) {
            line.push(word);
            tspan.text(line.join(" "));
            if (tspan.node().getComputedTextLength() > width && line.length > 1) {
                line.pop();
                tspan.text(line.join(" "));
                line = [word];
                tspan = text.append("tspan")
                    .attr("x", 0)
                    .attr("dy", "1.2em").text(word);
            }
        }
    }
    exports.wrap = wrap;
    function colorScale(max) {
        return d3.scale.linear()
            .domain([0, max / 4, max])
            .range(["green", "gold", "red"]);
    }
    exports.colorScale = colorScale;
    function calculatePoint(rectangle, point) {
        var vector = { x: point.x - rectangle.x, y: point.y - rectangle.y };
        var v2 = { x: rectangle.width / 2, y: rectangle.height / 2 };
        var ratio = getRatio(vector, v2);
        return { x: rectangle.x + vector.x * ratio, y: rectangle.y + vector.y * ratio };
    }
    exports.calculatePoint = calculatePoint;
    function getRatio(vOut, vIn) {
        var vOut2 = { x: vOut.x, y: vOut.y };
        if (vOut2.x < 0)
            vOut2.x = -vOut2.x;
        if (vOut2.y < 0)
            vOut2.y = -vOut2.y;
        if (vOut2.x == 0 && vOut2.y == 0)
            return null;
        if (vOut2.x == 0)
            return vIn.y / vOut2.y;
        if (vOut2.y == 0)
            return vIn.x / vOut2.x;
        return Math.min(vIn.x / vOut2.x, vIn.y / vOut2.y);
    }
});
//# sourceMappingURL=SchemaMap.js.map