using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ntech.NetStandard.Utilities
{
    public static class StringExtension
    {
        public static string ToIdentifier(this string src)
        {
            if (src.IsEmpty())
                return src;

            src = src.ToLower();

            //Replace contiguous invalid chars with hyphen
            src = Regex.Replace(src, @"[^a-z0-9_]+", "-", RegexOptions.None);

            //Remove leading/trailing hypen
            src = Regex.Replace(src, @"^[-_]+|[-_]+$", "");

            //remove special characters, leaving only alphanumeric characters
            src = Regex.Replace(src, @"[^a-z0-9-]+", "", RegexOptions.Compiled);

            return src;
        }

        public static string ConvertToValidFileName(this string name)
        {
            StringBuilder result = new StringBuilder();
            foreach (var str in name)
            {
                if (Char.IsLetterOrDigit(str))
                    result.Append(str);
            }

            return result.ToString();
        }

        public static string GetGuidFromUrl(this string url)
        {
            return Regex.Match(url, @"([a-fA-F\d]{8}-([a-fA-F\d]{4}-){3}[a-fA-F\d]{12})").Groups[1].Value;
        }

        /// Example Url: design/testsection/testsection-pk-uia/details
        /// Return: "testsection-pk-uia"
        public static string GetIdentifierFromUrl(this string url)
        {
            int lastSlash = url.LastIndexOf('/');
            int secondLastSlash = url.LastIndexOf('/', lastSlash - 1);
            var s = url.Substring(secondLastSlash + 1, lastSlash - secondLastSlash - 1);
            return s;
        }

        public static bool IsEmpty(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static bool HasValue(this string value)
        {
            return !IsEmpty(value);
        }

        public static string AddSpaces(this string text, bool preserveAcronyms = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static int AsInteger(this string text)
        {
            try
            {
                return int.Parse(text);
            }
            catch (Exception)
            {
                //Log the exception here
                return 0;
            }
        }
 
        public static int? AsIntegerNullable(this string text)
        {
            try
            {
                return int.Parse(text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static long AsLong(this string text)
        {
            try
            {
                return long.Parse(text);
            }
            catch (Exception)
            {
                //Log the exception here
                return 0;
            }
        }

        public static Guid AsGuid(this string text)
        {
            if (text.IsEmpty())
                throw new Exception("Cannot convert a empty string to guid");

            if (!text.IsGuidString())
                throw new Exception("Cannot convert string to guid as it does not match a guid format : " + text);

            return new Guid(text);
        }

        public static bool AsBool(this string text)
        {
            if (text.IsEmpty())
                return false;

            if (text.EqualsIgnoreCase("yes") || text.EqualsIgnoreCase("true") || text.Equals("1"))
                return true;

            return false;
        }

        public static DateTime AsDateTime(this string text)
        {
            if (text.IsEmpty())
                return DateTime.MinValue;

            DateTime result = DateTime.MinValue;
            DateTime.TryParse(text, out result);
            return result;
        }

        public static dynamic AsJson(this string text, bool exposeError = false)
        {
            try
            {
                return JsonConvert.DeserializeObject<dynamic>(text);
            }
            catch (Exception)
            {
                if (exposeError)
                {
                    throw;
                }
                else
                {
                    return null;
                }
            }
        }

        public static bool IsGuidString(this string guidStr)
        {
            Guid guid = Guid.Empty;
            return Guid.TryParse(guidStr.Trim(new char[] { '{', '}' }), out guid);
        }

        public static bool Contains(this string haystack, string pin, StringComparison comparisonOptions)
        {
            return haystack.IndexOf(pin, comparisonOptions) >= 0;
        }

        public static bool ContainsIgnoreCase(this string data, string value) => data.Contains(value, StringComparison.InvariantCultureIgnoreCase);

        public static bool ContainsIgnoreCase(this string data, string[] value)
        {
            foreach (var item in value)
            {
                if (data.Contains(item, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        public static string EmptyIfNull(this string value)
        {
            return value ?? String.Empty;
        }

        public static string WithEnvironmentNewLine(this string value)
        {
            return value.EmptyIfNull().Replace(@"\r\n", Environment.NewLine);
        }

        public static DateTime HourMinutesAsDateTime(this string value)
        {
            var splits = value.Split(':');
            return new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day,
                             splits[0].AsInteger(), splits[1].AsInteger(), 0);
        }

        public static string ConvertToDbFormatColumnName(this string value)
        {
            return "_" + Regex.Replace(value, @"[^\w]+", "").ToLower();
        }

        public static bool EqualsIgnoreCase(this string value, string compareValue)
        {
            return value.EmptyIfNull().Equals(compareValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool NotEquals(this string value, string compareVallue) => !value.Equals(compareVallue);

        public static string StripBasicHtml(this string htmlText)
        {
            return Regex.Replace(htmlText, "<.*?>", string.Empty);
        }

        public static string SplitOnCaps(this string str)
        {
            System.Text.StringBuilder output = new System.Text.StringBuilder(str.Substring(0, 1));

            for (int i = 1; i < str.Length; i++)
            {
                if (Char.IsUpper(str[i]) && (!char.IsUpper(str[i - 1]) || (i + 1 < str.Length && char.IsLower(str[i + 1]))))
                {
                    output.Append(" " + str[i]);
                }
                else
                {
                    output.Append(str[i]);
                }
            }
            return output.ToString();
        }
        public static IEnumerable<string> SplitInParts(this string str, int partLength)
        {
            if (str.IsEmpty()) throw new ArgumentNullException("SplitInParts - str");
            if (partLength <= 0) throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < str.Length; i += partLength)
                yield return str.Substring(i, Math.Min(partLength, str.Length - i));
        }

        public static string SurroundWithQuotes(this string str)
        {
            if (!str.StartsWith(@""""))
            {
                str = $@"""{str}";
            }
            if (!str.EndsWith(@""""))
            {
                str = $@"{str}""";
            }

            return str;
        }

        public static string SurroundWith(this string str, string surroundString)
        {
            if (!str.StartsWith(surroundString))
            {
                str = $@"{surroundString}{str}";
            }
            if (!str.EndsWith(surroundString))
            {
                str = $@"{str}{surroundString}";
            }

            return str;
        }
    }
}
