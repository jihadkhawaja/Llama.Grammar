Here's a `README.md` file you can use for your C# class library **Llama.Grammar**, referencing the original project it builds upon:

---

# Llama.Grammar

**Llama.Grammar** is a C# class library that allows you to define JSON Schemas, C# objects dynamically and convert them to [GBNF (Grammar-Based Next-Token Format)](https://github.com/adrienbrault/json-schema-to-gbnf) grammars. It is useful for working with structured outputs in AI models like LLaMA, Mistral, or GPT when paired with inference runtimes that support GBNF grammars.

## Features

- 🧱 Fluent builder API for creating JSON Schema objects in C#
- 🧠 Converts JSON Schema to GBNF using Adrien Brault’s original grammar logic
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

```csharp
using Llama.Grammar;

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

string gbnf = JsonSchemaToGbnf.Convert(json);
Console.WriteLine(gbnf);
```

## Based on

This project is based on the excellent work by [Adrien Brault](https://github.com/adrienbrault/json-schema-to-gbnf), porting and wrapping his TypeScript/JavaScript logic for use in .NET.

GitHub: [https://github.com/adrienbrault/json-schema-to-gbnf](https://github.com/adrienbrault/json-schema-to-gbnf)

## License

MIT License. See [LICENSE](LICENSE) for details.

```

---

Would you like a badge set (NuGet, license, GitHub stars, etc.) added at the top?
```
