using Llama.Grammar.Service;
using Llama.Grammar.Helper;

public class TestApp
{
    public static void Main()
    {
        var grammar = new GbnfGrammar();
        
        // Test JSON Schema conversion
        var schemaBuilder = new SchemaBuilder()
            .Type("object")
            .Properties(p => p
                .Add("name", s => s.Type("string"))
                .Add("age", s => s.Type("integer")))
            .Required("name", "age");
        
        string json = schemaBuilder.ToJson();
        string gbnf = grammar.ConvertJsonSchemaToGbnf(json);
        
        Console.WriteLine("JSON Schema:");
        Console.WriteLine(json);
        Console.WriteLine("\nGBNF Output:");
        Console.WriteLine(gbnf);
        
        // Test regex conversion
        string regexGbnf = grammar.ConvertRegexpToGbnf(@"^[a-z]+@[a-z]+\.[a-z]{2,}$");
        Console.WriteLine("\nRegex to GBNF:");
        Console.WriteLine(regexGbnf);
    }
}

public class TestPerson
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}
