using Llama.Grammar.Helper;
using Llama.Grammar.Service;
using System.Text.RegularExpressions;

namespace Llama.Grammar.Test
{
    [TestClass]
    public sealed class GrammarGeneratorTest
    {
        [TestMethod]
        public void ValidateGeneratedGBNF()
        {
            var schemaBuilder = new SchemaBuilder()
                .Type("object")
                .Properties(p => p
                    .Add("name", s => s.Type("string"))
                    .Add("age", s => s.Type("integer"))
                    .Add("nicknames", s => s.Type("array")
                                             .MinItems(1)
                                             .MaxItems(3)
                                             .Items(i => i.Type("string"))))
                .Required("name", "age");

            string json = schemaBuilder.ToJson();

            IGbnfGrammar grammar = new GbnfGrammar();
            var gbnf = grammar.ConvertJsonSchemaToGbnf(json);

            // Validate GBNF format (example using regex)
            Assert.IsTrue(IsValidGbnf(gbnf), "Generated GBNF is invalid.");
        }

        [TestMethod]
        public void ValidateTypeToGBNF()
        {
            IGbnfGrammar grammar = new GbnfGrammar();
            var gbnf = grammar.ConvertTypeToGbnf<TestPerson>();
            Assert.IsTrue(IsValidGbnf(gbnf), "Generated GBNF from C# type is invalid.");
        }

        private class TestPerson
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
            public List<string> Nicknames { get; set; } = new();
            public Address? HomeAddress { get; set; }
        }

        private class Address
        {
            public string Street { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string? ZipCode { get; set; }
        }

        private bool IsValidGbnf(string gbnf)
        {
            //  - allow all printable ASCII (0x20–0x7E)
            //  - plus newline (\r, \n) and horizontal tab (\t)
            //  - require at least one ::= so we didn’t output an empty or non‐grammar string
            const string pattern = @"^(?=.*::=)[\u0020-\u007E\r\n\t]+$";
            return Regex.IsMatch(gbnf, pattern, RegexOptions.Multiline);
        }
    }
}
