using Llama.Grammar.Core;

namespace Llama.Grammar.Service
{
    public class GbnfGrammar : IGbnfGrammar
    {
        public string ConvertJsonSchemaToGbnf(string jsonSchema)
            => JsonSchemaToGbnf.Convert(jsonSchema);

        public string ConvertRegexpToGbnf(string regexp)
            => RegexToGbnf.Convert(regexp);

        public string ConvertTypeToGbnf<T>()
        {
            var jsonSchema = TypeToJsonSchema.Convert<T>();
            return ConvertJsonSchemaToGbnf(jsonSchema);
        }
    }
}
