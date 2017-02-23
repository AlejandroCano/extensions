﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using W = DocumentFormat.OpenXml.Wordprocessing;
using D = DocumentFormat.OpenXml.Drawing;
using M = DocumentFormat.OpenXml.Math;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Templating;
using Signum.Entities;
using Signum.Entities.DynamicQuery;
using Signum.Utilities;
using Signum.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Signum.Entities.Word;

namespace Signum.Engine.Word
{
    public class TemplateParser
    {
        public List<TemplateError> Errors = new List<TemplateError>();
        QueryDescription queryDescription;
        ScopedDictionary<string, ValueProviderBase> variables = new ScopedDictionary<string, ValueProviderBase>(null);
        public readonly Type SystemWordTemplateType;
        OpenXmlPackage document;
        WordTemplateEntity template;

        public TemplateParser(OpenXmlPackage document, QueryDescription queryDescription, Type systemWordTemplateType, WordTemplateEntity template)
        {
            this.queryDescription = queryDescription;
            this.SystemWordTemplateType = systemWordTemplateType;
            this.document = document;
            this.template = template;
        }

        public void ParseDocument()
        {
            foreach (var p in document.AllRootElements())
            {
                foreach (var item in p.Descendants())
                {
                    if (item is W.Paragraph)
                        ReplaceRuns((W.Paragraph)item, new WordprocessingNodeProvider());

                    if (item is D.Paragraph)
                        ReplaceRuns((D.Paragraph)item, new DrawingNodeProvider());
                }
            }
        }

        private void ReplaceRuns(OpenXmlCompositeElement par, INodeProvider nodeProvider)
        {
            string text = par.ChildElements.Where(a => nodeProvider.IsRun(a)).ToString(r => nodeProvider.GetText(r), "");

            var matches = TemplateUtils.KeywordsRegex.Matches(text).Cast<Match>().ToList();

            if (matches.Any())
            {
                List<ElementInfo> infos = GetElementInfos(par.ChildElements, nodeProvider);

                par.RemoveAllChildren();

                var stack = new Stack<ElementInfo>(infos.AsEnumerable().Reverse());

                foreach (var m in matches)
                {
                    var interval = new Interval<int>(m.Index, m.Index + m.Length);

                    //  [Before][Start][Ignore][Ignore][End]...[Remaining]
                    //              [        Match       ]

                    ElementInfo start = stack.Pop(); //Start
                    while (start.Interval.Max <= interval.Min) //Before
                    {
                        par.Append(start.Element);
                        start = stack.Pop();
                    }

                    var startRun = (OpenXmlCompositeElement)nodeProvider.CastRun(start.Element);

                    if (start.Interval.Min < interval.Min)
                    {
                        var firstRunPart = nodeProvider.NewRun(
                            (OpenXmlCompositeElement)nodeProvider.GetRunProperties(startRun)?.CloneNode(true),
                             start.Text.Substring(0, m.Index - start.Interval.Min),
                             SpaceProcessingModeValues.Preserve
                            );
                        par.Append(firstRunPart);
                    }

                    par.Append(new MatchNode(nodeProvider, m) { RunProperties = (OpenXmlCompositeElement)nodeProvider.GetRunProperties(startRun)?.CloneNode(true) });

                    ElementInfo end = start;
                    while (end.Interval.Max < interval.Max) //Ignore
                        end = stack.Pop();

                    if (interval.Max < end.Interval.Max) //End
                    {
                        var endRun = (OpenXmlCompositeElement)end.Element;

                        var textPart = end.Text.Substring(interval.Max - end.Interval.Min);
                        var endRunPart = nodeProvider.NewRun(
                            nodeProvider.GetRunProperties(startRun)?.Let(r => (OpenXmlCompositeElement)r.CloneNode(true)),
                            textPart,
                             SpaceProcessingModeValues.Preserve
                            );

                        stack.Push(new ElementInfo
                        {
                            Element = endRunPart,
                            Text = textPart,
                            Interval = new Interval<int>(interval.Max, end.Interval.Max)
                        });
                    }
                }

                while (!stack.IsEmpty()) //Remaining
                {
                    var pop = stack.Pop();
                    par.Append(pop.Element);
                }
            }
        }

        private static List<ElementInfo> GetElementInfos(IEnumerable<OpenXmlElement> childrens, INodeProvider nodeProvider)
        {
            var infos = childrens.Select(c => new ElementInfo { Element = c, Text = nodeProvider.IsRun(c) ? nodeProvider.GetText(c) : null }).ToList();

            int currentPosition = 0;
            foreach (ElementInfo ri in infos)
            {
                ri.Interval = new Interval<int>(currentPosition, currentPosition + (ri.Text == null ? 0 : ri.Text.Length));
                currentPosition = ri.Interval.Max;
            }

            return infos;
        }

        class ElementInfo
        {
            public string Text;
            public OpenXmlElement Element;
            public Interval<int> Interval;

            public override string ToString()
            {
                return Interval + " " + Element.LocalName + (Text == null ? null : (": '" + Text + "'"));
            }
        }


        Stack<BlockContainerNode> stack = new Stack<BlockContainerNode>();

        public void CreateNodes()
        {
            foreach (var root in document.AllRootElements())
            {
                var lists = root.Descendants<MatchNode>().ToList();

                foreach (var matchNode in lists)
                {
                    var m = matchNode.Match;

                    var type = m.Groups["type"].Value;
                    var token = m.Groups["token"].Value;
                    var keyword = m.Groups["keyword"].Value;
                    var dec = m.Groups["dec"].Value;

                    switch (keyword)
                    {
                        case "":
                            var s = TemplateUtils.SplitToken(token);
                            if (s == null)
                                AddError(true, "{0} has invalid format".FormatWith(token));
                            else
                            {
                                var vp = TryParseValueProvider(type, s.Value.Token, dec);

                                matchNode.Parent.ReplaceChild(new TokenNode(matchNode.NodeProvider, vp, s.Value.Format)
                                {
                                    RunProperties = (OpenXmlCompositeElement)matchNode.RunProperties?.CloneNode(true)
                                }, matchNode);

                                DeclareVariable(vp);
                            }
                            break;
                        case "declare":
                            {
                                var vp = TryParseValueProvider(type, token, dec);

                                matchNode.Parent.ReplaceChild(new DeclareNode(matchNode.NodeProvider, vp, this.AddError)
                                {
                                    RunProperties = (OpenXmlCompositeElement)matchNode.RunProperties?.CloneNode(true)
                                }, matchNode);

                                DeclareVariable(vp);
                            }
                            break;
                        case "any":
                            {
                                AnyNode any;
                                ValueProviderBase vp;
                                var filter = TemplateUtils.TokenOperationValueRegex.Match(token);
                                if (!filter.Success)
                                {
                                    vp = TryParseValueProvider(type, token, dec);
                                    any = new AnyNode(matchNode.NodeProvider, vp) { AnyToken = new MatchNodePair(matchNode) };
                                }
                                else
                                {
                                    vp = TryParseValueProvider(type, filter.Groups["token"].Value, dec);
                                    var comparer = filter.Groups["comparer"].Value;
                                    var value = filter.Groups["value"].Value;
                                    any = new AnyNode(matchNode.NodeProvider, vp, comparer, value, this.AddError) { AnyToken = new MatchNodePair(matchNode) };
                                }

                                PushBlock(any);

                                DeclareVariable(vp);
                                break;
                            }
                        case "notany":
                            {
                                var an = PeekBlock<AnyNode>();
                                if (an != null)
                                {
                                    an.NotAnyToken = new MatchNodePair(matchNode);
                                }
                                break;
                            }
                        case "endany":
                            {
                                var an = PopBlock<AnyNode>();
                                if (an != null)
                                {
                                    an.EndAnyToken = new MatchNodePair(matchNode);

                                    an.ReplaceBlock();
                                }
                                break;
                            }
                        case "if":
                            {
                                IfNode ifn;
                                ValueProviderBase vpb;
                                var filter = TemplateUtils.TokenOperationValueRegex.Match(token);
                                if (!filter.Success)
                                {
                                    vpb = TryParseValueProvider(type, token, dec);
                                    ifn = new IfNode(matchNode.NodeProvider, vpb) { IfToken = new MatchNodePair(matchNode) };
                                }
                                else
                                {
                                    vpb = TryParseValueProvider(type, filter.Groups["token"].Value, dec);
                                    var comparer = filter.Groups["comparer"].Value;
                                    var value = filter.Groups["value"].Value;
                                    ifn = new IfNode(matchNode.NodeProvider, vpb, comparer, value, this.AddError) { IfToken = new MatchNodePair(matchNode) };
                                }

                                PushBlock(ifn);

                                DeclareVariable(vpb);

                                break;
                            }
                        case "else":
                            {
                                var an = PeekBlock<IfNode>();
                                if (an != null)
                                {
                                    an.ElseToken = new MatchNodePair(matchNode);
                                }
                                break;
                            }
                        case "endif":
                            {
                                var ifn = PopBlock<IfNode>();
                                if (ifn != null)
                                {
                                    ifn.EndIfToken = new MatchNodePair(matchNode);

                                    ifn.ReplaceBlock();
                                }
                                break;
                            }
                        case "foreach":
                            {
                                var vp = TryParseValueProvider(type, token, dec);
                                var fn = new ForeachNode(matchNode.NodeProvider, vp) { ForeachToken = new MatchNodePair(matchNode) };
                                PushBlock(fn);

                                DeclareVariable(vp);
                                break;
                            }
                        case "endforeach":
                            {
                                var fn = PopBlock<ForeachNode>();
                                if (fn != null)
                                {
                                    fn.EndForeachToken = new MatchNodePair(matchNode);

                                    fn.ReplaceBlock();
                                }
                                break;
                            }
                        default:
                            AddError(true, "'{0}' is deprecated".FormatWith(keyword));
                            break;
                    }
                }
            }
        }

        void PushBlock(BlockContainerNode node)
        {
            stack.Push(node);
            variables = new ScopedDictionary<string, ValueProviderBase>(variables);
        }

        T PopBlock<T>() where T : BlockContainerNode
        {
            if (stack.IsEmpty())
            {
                AddError(true, "No {0} has been opened".FormatWith(BlockContainerNode.UserString(typeof(T))));
                return null;
            }

            BlockContainerNode n = stack.Pop();
            if (n == null || !(n is T))
            {
                AddError(true, "Unexpected '{0}'".FormatWith(BlockContainerNode.UserString(n?.GetType())));
                return null;
            }

            variables = variables.Previous;
            return (T)n;
        }

        T PeekBlock<T>() where T : BlockContainerNode
        {
            if (stack.IsEmpty())
            {
                AddError(true, "No {0} has been opened".FormatWith(BlockContainerNode.UserString(typeof(T))));
                return null;
            }

            BlockContainerNode n = stack.Peek();
            if (n == null || !(n is T))
            {
                AddError(true, "Unexpected '{0}'".FormatWith(BlockContainerNode.UserString(n?.GetType())));
                return null;
            }


            variables = variables.Previous;
            variables = new ScopedDictionary<string, ValueProviderBase>(variables);
            return (T)n;
        }

        public ValueProviderBase TryParseValueProvider(string type, string token, string variable)
        {
            return ValueProviderBase.TryParse(type, token, variable, this.SystemWordTemplateType, this.queryDescription, this.variables, this.AddError);
        }


        internal void AddError(bool fatal, string message)
        {
            this.Errors.Add(new TemplateError(fatal, message));
        }


        void DeclareVariable(ValueProviderBase token)
        {
            if (token?.Variable.HasText() == true)
            {
                ValueProviderBase t;
                if (variables.TryGetValue(token.Variable, out t))
                {
                    if (!t.Equals(token))
                        AddError(true, "There's already a variable '{0}' defined in this scope".FormatWith(token.Variable));
                }
                else
                {
                    variables.Add(token.Variable, token);
                }
            }
        }

        public void AssertClean()
        {
            foreach (var root in this.document.AllRootElements())
            {
                var list = root.Descendants<MatchNode>().ToList();

                if (list.Any())
                    throw new InvalidOperationException("{0} unexpected MatchNode instances found: \r\n{1}".FormatWith(list.Count, list.ToString("\r\n").Indent(4)));
            }
        }
    }
}
