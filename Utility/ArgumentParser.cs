using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Keeper.Garrett.ScrewTurn.Utility
{
    public class ArgumentParser
    {
        /// <summary>
        /// (\s)? - Optional start with whitespace,/n,/r,/t
        /// (/)? - Optional only for the looks /key=value
        /// (?<key>(\w+)) - Required, key name only chars
        /// = - Required
        /// (('(?<value>(.*?))')|(?<value>(.*?))(\s|$)) - Required either ('(?<value>(.*?))') or (?<value>(.*?))(\s|$)
        ///     ('(?<value>(.*?))') - Value must start with ' and end with '
        ///     (?<value>(.*?))(\s|$) - Value ends with whitespace,/n,/r,/t OR $ endofline
        /// </summary>
        private static readonly Regex m_ArgumentRegex = new Regex(@"(\s)?(/)?(?<key>(\w+))=(('(?<value>(.*?))')|(?<value>(.*?))(\s|$))", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        public Dictionary<string, string> Parse(string _line)
        {
            var retval = new Dictionary<string,string>();

            var matches = m_ArgumentRegex.Matches(_line);

            foreach (Match match in matches)
            {
                if (match != null && match.Groups.Count > 1)
                {
                    var key = match.Groups["key"].Value.ToLower();
                    var value = match.Groups["value"].Value.Trim();

                    //Add key if's not allready there
                    //Add only if value is not null
                    if (retval.ContainsKey(key) == false && string.IsNullOrEmpty(value) == false)
                    {
                        retval.Add(key, value);
                    }
                }
            }

            return retval;
        }
    }
}
