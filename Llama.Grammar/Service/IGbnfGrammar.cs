namespace Llama.Grammar.Service
{
    public interface IGbnfGrammar
    {
        /// <summary>
        /// Converts a JSON Schema to GBNF grammar.
        /// </summary>
        /// <param name="jsonSchema">The JSON Schema string.</param>
        /// <returns>The GBNF grammar string.</returns>
        string ConvertJsonSchemaToGbnf(string jsonSchema);

        /// <summary>
        /// Converts a regular expression to GBNF grammar.
        /// </summary>
        /// <param name="regexp">The regular expression string.</param>
        /// <returns>The GBNF grammar string.</returns>
        string ConvertRegexpToGbnf(string regexp);
    }
}
