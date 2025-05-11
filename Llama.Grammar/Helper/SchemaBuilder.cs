using System.Text.Json;

namespace Llama.Grammar.Helper
{
    public class SchemaBuilder
    {
        protected readonly Dictionary<string, object> _schema = new();

        public SchemaBuilder Type(string type)
        {
            _schema["type"] = type;
            return this;
        }

        public SchemaBuilder Type(params string[] types)
        {
            _schema["type"] = types;
            return this;
        }

        public SchemaBuilder Enum(params object[] values)
        {
            _schema["enum"] = values;
            return this;
        }

        public SchemaBuilder Const(object value)
        {
            _schema["const"] = value;
            return this;
        }

        public SchemaBuilder Pattern(string regex)
        {
            _schema["pattern"] = regex;
            return this;
        }

        public SchemaBuilder Nullable()
        {
            // turn "type":"string" into ["string","null"], or add nullable:true
            if (_schema.TryGetValue("type", out var t) && t is string s)
                _schema["type"] = new[] { s, "null" };
            else
                _schema["nullable"] = true;
            return this;
        }

        public SchemaBuilder MinLength(int min)
        {
            _schema["minLength"] = min;
            return this;
        }

        public SchemaBuilder MaxLength(int max)
        {
            _schema["maxLength"] = max;
            return this;
        }

        public SchemaBuilder MinItems(int min)
        {
            _schema["minItems"] = min;
            return this;
        }

        public SchemaBuilder MaxItems(int max)
        {
            _schema["maxItems"] = max;
            return this;
        }

        public SchemaBuilder Items(Action<SchemaBuilder> configure)
        {
            var itemBuilder = new SchemaBuilder();
            configure(itemBuilder);
            _schema["items"] = itemBuilder._schema;
            return this;
        }

        public SchemaBuilder Properties(Action<PropertiesBuilder> configure)
        {
            var pb = new PropertiesBuilder();
            configure(pb);
            _schema["properties"] = pb.Build();
            return this;
        }

        public SchemaBuilder Required(params string[] names)
        {
            _schema["required"] = names;
            return this;
        }

        public string ToJson() => JsonSerializer.Serialize(_schema, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        public override string ToString() => ToJson();

        public class PropertiesBuilder
        {
            private readonly Dictionary<string, object> _props = new();

            public PropertiesBuilder Add(string name, Action<SchemaBuilder> configure)
            {
                var sb = new SchemaBuilder();
                configure(sb);
                _props[name] = sb._schema;
                return this;
            }

            public Dictionary<string, object> Build() => _props;
        }
    }
}
