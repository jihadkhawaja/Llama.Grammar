using System.Text;
using System.Text.RegularExpressions;

namespace Llama.Grammar.Core
{
    internal static class RegexToGbnf
    {
        /// <summary>
        /// Port of the TS formatStringLength: given a single‐unit GBNF symbol (e.g. "a") and
        /// min/max counts, produce a sequence with exact/optional repetitions.
        /// </summary>
        internal static string FormatStringLength(string symbol, int? minLength, int? maxLength)
        {
            if (minLength.HasValue && maxLength.HasValue)
            {
                var mandatory = Enumerable.Repeat(symbol, minLength.Value);
                var optional = Enumerable.Repeat(symbol + "?", maxLength.Value - minLength.Value);
                return string.Join(" ", mandatory.Concat(optional));
            }
            if (minLength.HasValue)
            {
                var mandatory = Enumerable.Repeat(symbol, minLength.Value);
                return string.Join(" ", mandatory) + " " + symbol + "*";
            }
            if (maxLength.HasValue)
            {
                return string.Join(" ", Enumerable.Repeat(symbol + "?", maxLength.Value));
            }
            throw new ArgumentException("Either minLength or maxLength must be provided");
        }

        /// <summary>
        /// Port of convertRegexpToGbnf: handles ^/$ anchors, \w, ., simple classes, +*? and {m,n}.
        /// Anything else will be emitted as-is inside quotes (escaped).
        /// </summary>
        internal static string Convert(string pattern)
        {
            var hasStart = pattern.StartsWith("^");
            var hasEnd = pattern.EndsWith("$");
            if (hasStart) pattern = pattern[1..];
            if (hasEnd) pattern = pattern[..^1];

            pattern = Regex.Replace(pattern, @"\\w", "[0-9A-Za-z_]");
            pattern = pattern.Replace(".", "string-char");

            pattern = Regex.Replace(pattern,
                @"(.|\[.+?\])\{(\d+),(\d+)\}",
                m =>
                {
                    var sym = m.Groups[1].Value;
                    var min = int.Parse(m.Groups[2].Value);
                    var max = int.Parse(m.Groups[3].Value);
                    var unit = sym.StartsWith("[") ? sym : $"\"{EscapeForGbnf(sym)}\"";
                    return FormatStringLength(unit, min, max);
                });

            pattern = Regex.Replace(pattern,
                @"(.|\[.+?\])([*+?])",
                m =>
                {
                    var sym = m.Groups[1].Value;
                    var quant = m.Groups[2].Value;
                    var unit = sym.StartsWith("[") || sym.Length == 1
                        ? sym
                        : $"({sym})";
                    return unit + quant;
                });

            var parts = Regex.Split(pattern, @"(?<!\\)\|")
                             .Select(p => p.Trim())
                             .Where(p => p.Length > 0)
                             .Select(p =>
                             {
                                 var sb = new StringBuilder();
                                 foreach (var ch in p)
                                 {
                                     if (ch is '[' or ']' or '(' or ')' or '*' or '+' or '?' or ' ' || ch == '-')
                                         sb.Append(ch);
                                     else
                                         sb.Append($"\"{EscapeForGbnf(ch.ToString())}\" ");
                                 }
                                 return sb.ToString().Trim();
                             })
                             .ToArray();

            var core = parts.Length > 1
                ? "(" + string.Join(" | ", parts) + ")"
                : parts[0];

            if (!hasStart) core = "(string-char)* " + core;
            if (!hasEnd) core = core + " (string-char)*";

            return core;
        }

        /// <summary>
        /// Escape any GBNF-special chars inside a quoted literal.
        /// </summary>
        private static string EscapeForGbnf(string literal)
            => literal.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
