using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Llama.Grammar.Core
{
    internal static class JsonSchemaToGbnf
    {
        private static readonly string EbmpBase =
            """
            value  ::= (object | array | string | number | boolean | null) ws

            object ::=
              "{" ws (
                string ":" ws value
                ("," ws string ":" ws value)*
              )? "}"

            array  ::=
              "[" ws01 (
                        value
                ("," ws01 value)*
              )? "]"

            string ::=
              "\"" (string-char)* "\""

            string-char ::= [^"\\] | "\\" (["\\/bfnrt] | "u" [0-9a-fA-F] [0-9a-fA-F] [0-9a-fA-F] [0-9a-fA-F]) # escapes

            number ::= integer ("." [0-9]+)? ([eE] [-+]? [0-9]+)?
            integer ::= "-"? ([0-9] | [1-9] [0-9]*)
            boolean ::= "true" | "false"
            null ::= "null"

            # Optional space: by convention, applied in this grammar after literal chars when allowed
            ws ::= ([ \t\n] ws)?
            ws01 ::= ([ \t\n])?
            """;

        internal static string Convert(string jsonSchema)
        {
            using var doc = JsonDocument.Parse(jsonSchema);
            var root = doc.RootElement;
            var rules = new Dictionary<string, string>();
            var sb = new StringBuilder();

            void Traverse(JsonElement schema, string pointer, JsonElement? parent = null, string parentKey = null)
            {
                var name = JsonPointerToName(pointer);
                string FormatPropertyName(string key) =>
                    key is null ? "" : $"\"\\\"{key}\\\"\" ws01 \":\" ws01 ";

                bool IsNullable(JsonElement s) =>
                    s.TryGetProperty("nullable", out var n) && n.GetBoolean();

                string WrapNullable(string g) =>
                    g = IsNullable(schema) ? $"({g} | null)" : g;

                bool IsRequired(string prop) =>
                    schema.TryGetProperty("required", out var req) &&
                    req.EnumerateArray().Any(e => e.GetString() == prop);

                string Alt(IEnumerable<string> vs) => "(" + string.Join(" | ", vs) + ")";

                string Lit(JsonElement v) => v.ValueKind switch
                {
                    JsonValueKind.String => $"\"\\\"{v.GetString()}\\\"\"",
                    JsonValueKind.Number => $"\"{v.GetRawText()}\"",
                    JsonValueKind.True or JsonValueKind.False => $"\"{v.GetRawText()}\"",
                    _ => throw new InvalidOperationException()
                };

                string TryFormat(JsonElement s, string ptr)
                {
                    if (s.TryGetProperty("type", out var t))
                    {
                        if (t.ValueKind == JsonValueKind.String && t.GetString() == "object")
                        {
                            if (!s.TryGetProperty("properties", out var props))
                                return "object";

                            var parts = props.EnumerateObject()
                                .Select((p, i) =>
                                {
                                    var subPtr = ptr + "/properties/" + p.Name;
                                    var ruleName = JsonPointerToName(subPtr);
                                    Traverse(p.Value, subPtr, s, p.Name);
                                    var core = ruleName;
                                    if (!IsRequired(p.Name))
                                        core = $"({core})?";
                                    return (i == 0 ? "" : "\" ,\" ws01 ") + core;
                                });
                            return $"\"{{\" ws01 {string.Concat(parts)} \"}}\"";
                        }

                        if (t.ValueKind == JsonValueKind.String && t.GetString() == "array")
                        {
                            JsonElement items = s.GetProperty("items");
                            var itemPtr = ptr + "/items";
                            var itemName = JsonPointerToName(itemPtr);
                            Traverse(items, itemPtr);
                            var min = s.TryGetProperty("minItems", out var mi) ? mi.GetInt32() : (int?)null;
                            var max = s.TryGetProperty("maxItems", out var ma) ? ma.GetInt32() : (int?)null;

                            string Repeat()
                            {
                                if (min.HasValue && max.HasValue)
                                {
                                    var first = string.Join(" ,\" ws01 ",
                                        Enumerable.Repeat(itemName, min.Value));
                                    var opt = string.Join(" ",
                                        Enumerable.Repeat($"(\",\" ws01 {itemName})?", max.Value - min.Value));
                                    return $"{first} {opt}";
                                }
                                if (min.HasValue)
                                    return string.Join(" ,\" ws01 ",
                                        Enumerable.Repeat(itemName, min.Value))
                                        + " (\",\" ws01 " + itemName + ")*";
                                if (max.HasValue)
                                    return $"({itemName})? "
                                         + string.Join(" ",
                                             Enumerable.Repeat($"(\",\" ws01 {itemName})?", max.Value - 1));
                                // no constraints
                                return $"{itemName} ( ws01 \",\" ws01 {itemName})*";
                            }

                            return $"\"[\" ws01 {Repeat()} ws01 \"]\"";
                        }

                        if (t.ValueKind == JsonValueKind.String
                            && new[] { "string", "number", "integer", "boolean", "null" }.Contains(t.GetString()))
                        {
                            if (s.TryGetProperty("enum", out var enm))
                            {
                                var lits = enm.EnumerateArray().Select(Lit);
                                return Alt(lits);
                            }

                            if (s.TryGetProperty("pattern", out var pat))
                            {
                                return $"\"\\\"\" {ConvertRegexpToGbnf(pat.GetString())} \"\\\"\"";
                            }

                            if (t.GetString() == "string"
                                && (s.TryGetProperty("minLength", out var minL) || s.TryGetProperty("maxLength", out var maxL)))
                            {
                                return $"\"\\\"\" /* length logic here */ \"\\\"\"";
                            }

                            return t.GetString();
                        }
                    }

                    if (s.TryGetProperty("anyOf", out var anyOf))
                    {
                        var opts = anyOf.EnumerateArray()
                            .Select(x => x.GetProperty("type").GetString());
                        return Alt(opts);
                    }

                    if (s.TryGetProperty("const", out var cst))
                    {
                        return Lit(cst);
                    }

                    return null;
                }

                if (!rules.ContainsKey(name))
                {
                    var formatted = TryFormat(schema, pointer);
                    if (formatted != null)
                        rules[name] = FormatPropertyName(parentKey) + WrapNullable(formatted);
                }

                foreach (var prop in new[] { "properties", "items", "anyOf" })
                {
                    if (schema.TryGetProperty(prop, out var sub))
                    {
                        if (prop == "properties")
                        {
                            foreach (var p in sub.EnumerateObject())
                                Traverse(p.Value, pointer + "/properties/" + p.Name, schema, p.Name);
                        }
                        else if (prop == "items")
                        {
                            Traverse(sub, pointer + "/items", schema);
                        }
                        else
                        {
                            foreach (var (o, i) in sub.EnumerateArray().Select((x, i) => (x, i)))
                                Traverse(o, pointer + $"/anyOf/{i}", schema);
                        }
                    }
                }
            }

            Traverse(root, "");
            rules["root"] += " ws01";

            var outSb = new StringBuilder();
            foreach (var kv in rules)
                outSb.AppendLine($"{kv.Key} ::= {kv.Value}");
            outSb.AppendLine();
            outSb.Append(EbmpBase.TrimStart());

            return outSb.ToString();
        }

        private static string JsonPointerToName(string ptr)
        {
            if (string.IsNullOrEmpty(ptr))
                return "root";

            return "root" +
                   Regex.Replace(
                     ptr.Replace("/properties", ""),
                     @"[^a-zA-Z0-9-]+", "-");
        }

        private static string ConvertRegexpToGbnf(string pattern)
            => RegexToGbnf.Convert(pattern);
    }
}
