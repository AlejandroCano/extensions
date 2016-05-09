﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using System.ComponentModel;
using Signum.Entities.Dashboard;
using System.Xml.Linq;
using Signum.Entities.UserAssets;

namespace Signum.Entities.Omnibox
{
    public static class OmniboxUtils
    {
        public static bool IsPascalCasePattern(string ident)
        {
            if (string.IsNullOrEmpty(ident))
                return false;

            for (int i = 0; i < ident.Length; i++)
            {
                if (!char.IsUpper(ident[i]))
                    return false;
            }

            return true;
        }

        public static OmniboxMatch SubsequencePascal(object value, string identifier, string pattern)
        {
            bool[] indices = new bool[identifier.Length];
            int j = 0;
            for (int i = 0; i < pattern.Length; i++)
            {
                var pc = pattern[i];
                for (; j < identifier.Length; j++)
                {
                    var ic = identifier[j];

                    if (char.IsUpper(ic))
                    {
                        if (ic == pc)
                        {
                            indices[j] = true;

                            break;
                        }
                    }
                }

                if (j == identifier.Length)
                    return null;

                j++;
            }

            return new OmniboxMatch(value,
                remaining: identifier.Count(char.IsUpper) - pattern.Length,
                choosenString: identifier,
                boldIndices: indices);
        }

        public static IEnumerable<OmniboxMatch> Matches<T>(Dictionary<string, T> values, Func<T, bool> filter, string pattern, bool isPascalCase)
        {
            T val;
            if (values.TryGetValue(pattern, out val) && filter(val))
            {
                yield return new OmniboxMatch(val, 0, pattern, Enumerable.Repeat(true, pattern.Length).ToArray());
            }
            else
            {
                foreach (var kvp in values.Where(kvp => filter(kvp.Value)))
                {
                    OmniboxMatch result;
                    if (isPascalCase)
                    {
                        result = SubsequencePascal(kvp.Value, kvp.Key, pattern);

                        if (result != null)
                        {
                            yield return result;
                            continue;
                        }
                    }

                    result = Contains(kvp.Value, kvp.Key, pattern);
                    if (result != null)
                    {
                        yield return result;
                        continue;
                    }
                }
            }
        }

        public static OmniboxMatch Contains(object value, string identifier, string pattern)
        {
            var parts = pattern.SplitNoEmpty(' ' );

            bool[] indices = null;

            foreach (var p in parts)
	        {
                int index = identifier.IndexOf(p, StringComparison.InvariantCultureIgnoreCase);
                if (index == -1)
                    return null;

                if(indices == null)
                    indices = new bool[identifier.Length];

                for (int i = 0; i < p.Length; i++)
                    indices[index + i] = true;
	        }

            return new OmniboxMatch(value,
                remaining: identifier.Length - pattern.Length,
                choosenString: identifier,
                boldIndices: indices ?? new bool[identifier.Length]);
        }

        public static string CleanCommas(string str)
        {
            return str.Trim('\'', '"');;
        }
    }

    public class OmniboxMatch
    {
        public OmniboxMatch(object value, int remaining, string choosenString, bool[] boldIndices)
        {
            if (choosenString.Length != boldIndices.Length)
                throw new ArgumentException("choosenString '{0}' is {1} long but boldIndices is {2}".FormatWith(choosenString, choosenString.Length, boldIndices.Length));

            this.Value = value;

            this.Text = choosenString;
            this.BoldIndices = boldIndices;

            this.Distance = remaining;

            if (boldIndices.Length > 0 && boldIndices[0])
                this.Distance /= 2f;
        }

        public object Value; 

        public float Distance;
        public string Text;
        public bool[] BoldIndices;

        public IEnumerable<Tuple<string, bool>> BoldSpans()
        {
            return this.Text.ZipStrict(BoldIndices)
                .GroupWhenChange(a => a.Item2)
                .Select(gr => Tuple.Create(new string(gr.Select(a => a.Item1).ToArray()), gr.Key));
        }
    }


    public enum OmniboxMessage
    {
        [Description("no")]
        No,
        [Description("[Not found]")]
        NotFound,
        [Description("Searching between 'apostrophe' will make queries to the database")]
        Omnibox_DatabaseAccess,
        [Description("With [Tab] you disambiguate you query")]
        Omnibox_Disambiguate,
        [Description("Field")]
        Omnibox_Field,
        [Description("Help")]
        Omnibox_Help,
        [Description("You can match results by (st)art, mid(dle) or (U)pper(C)ase")]
        Omnibox_MatchingOptions,
        [Description("Query")]
        Omnibox_Query,
        [Description("Type")]
        Omnibox_Type,
        [Description("UserChart")]
        Omnibox_UserChart,
        [Description("UserQuery")]
        Omnibox_UserQuery,
        [Description("Dashboard")]
        Omnibox_Dashboard,
        [Description("Value")]
        Omnibox_Value,
        Unknown,
        [Description("yes")]
        Yes,
        [Description(@"\b(the|of) ")]
        ComplementWordsRegex,
        [Description("Search...")]
        Search,
    }

    [Serializable, EntityKind(EntityKind.Part, EntityData.Master)]
    public class OmniboxPanelPartEntity : Entity, IPartEntity
    {
        bool requiresTitle;
        public bool RequiresTitle
        {
            get { return requiresTitle; }
            set { Set(ref requiresTitle, value); }
        }

        public IPartEntity Clone()
        {
            return new OmniboxPanelPartEntity();
        }

        public XElement ToXml(IToXmlContext ctx)
        {
            return new XElement("OmniboxPanelPartEntity", this);
        }

        public void FromXml(XElement element, IFromXmlContext ctx)
        { }
    }
}
