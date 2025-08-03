
<p align="center">
  <img src="docs/llama_grammar_icon.png" alt="Llama.Grammar Icon" width="120" />
</p>

# Llama.Grammar

**Llama.Grammar** is a C# class library that allows you to define JSON Schemas, C# objects dynamically and convert them to [GBNF (Grammar-Based Next-Token Format)](https://github.com/ggml-org/llama.cpp/blob/master/grammars/README.md) grammars. It is useful for working with structured outputs in AI models like LLaMA, Mistral, or GPT when paired with inference runtimes that support GBNF grammars.

## Features

- 🧱 Fluent builder API for creating JSON Schema objects in C#
- 🧠 Converts JSON Schema to GBNF
- ✅ Supports complex schema features like:
  - Nested objects
  - Arrays and array constraints
  - Enums and constants
  - Required fields
  - Pattern matching
  - Nullable types

## Installation

Add the library to your project:

```bash
dotnet add package Llama.Grammar
````

## Usage

### Generating GBNF using fluent API

You can create JSON Schemas using the fluent API and convert them to GBNF grammars. Here's an example of how to do this


```csharp
using Llama.Grammar.Helper;
using Llama.Grammar.Service;

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
Console.WriteLine(gbnf);
```

### Generating GBNF from C# Types

You can generate GBNF grammars directly from C# types, including nested objects and collections


```csharp
public class TestPerson
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public List<string> Nicknames { get; set; } = new();
    public Address? HomeAddress { get; set; }
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
}

IGbnfGrammar grammar = new GbnfGrammar();
var gbnf = grammar.ConvertTypeToGbnf<TestPerson>();
Console.WriteLine(gbnf);
```

## Try it Online

🌐 **[Interactive Web App](https://jihadkhawaja.github.io/Llama.Grammar/)** - Try the library functionality directly in your browser!

The web app provides an easy-to-use interface for:
- Converting JSON schemas to GBNF
- Converting C# types to GBNF 
- Converting regular expressions to GBNF
- Interactive examples and documentation

## Based on

This project is based on the excellent work by [Adrien Brault](https://github.com/adrienbrault/json-schema-to-gbnf), porting and wrapping his TypeScript logic for use in .NET.

## License

MIT License. See [LICENSE](https://github.com/jihadkhawaja/Llama.Grammar?tab=License-1-ov-file#readme). for details.
