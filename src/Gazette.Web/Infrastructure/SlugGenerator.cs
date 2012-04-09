// // Copyright 2012, Smoothfriction
// // Author: Erik van Brakel
// // Licensed under the BSD 3-Clause License, see license.txt for details, or go to // http://www.opensource.org/licenses/BSD-3-Clause
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gazette.Controllers
{
    // generate url slugs. built on http://github.com/ayende/RaccoonBlog and http://blogs.msdn.com/michkap/archive/2007/05/14/2629747.aspx
    public static class SlugGenerator
    {
        private static string RemoveDiacritics(this string input)
        {
            var stFormD = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            for (var i = 0; i < stFormD.Length; i++)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[i]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        public static string Generate(string input)
        {
            return input
                .ToLowerInvariant()
                .RemoveDiacritics()
                .ReplaceWhitespace();
        }

        private static string ReplaceWhitespace(this string input)
        {
            // Remove special characters
            input = Regex.Replace(input, "[’'“”\"&]{1,}", "");

            // change all non-alphanumeric into spaces
            var chars = input.Select(c => char.IsLetterOrDigit(c) ? c : ' ').ToArray();
            input = new string(chars);
            return Regex.Replace(input, @"[ ]+", "-");
        }
    }
}