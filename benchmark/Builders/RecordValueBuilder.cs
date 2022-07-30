#pragma warning disable CA1051
namespace PowerFXBenchmark.Builders
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using Microsoft.PowerFx.Types;
    using Newtonsoft.Json.Linq;
    using PowerFXBenchmark.Inputs.Models;
    using PowerFXBenchmark.UntypedObjects;

    public class RecordValueBuilder
    {
        protected IList<NamedValue> fields;


        public RecordValueBuilder()
        {
            fields = new List<NamedValue>();
        }

        public RecordValueBuilder WithUntypedTestObject(TestObject testObj)
        {
            fields.Add(new NamedValue("testObj", TestObjectUntypedObject.New(testObj)));
            return this;
        }

        public RecordValueBuilder WithUntypedEventJson(JsonElement json)
        {
            if (json.ValueKind != JsonValueKind.Null)
            {
                fields.Add(new NamedValue("event", FormulaValue.New(new JsonUntypedObject(json))));
            }

            return this;
        }

        public RecordValueBuilder WithUntypedEventJson(string json)
        {
            JsonElement result;
            using (var document = JsonDocument.Parse(json))
            {
                // Clone must be used here because the original element will be disposed
                result = document.RootElement.Clone();
            }

            if (result.ValueKind != JsonValueKind.Null)
            {
                fields.Add(new NamedValue("event", FormulaValue.New(new JsonUntypedObject(result))));
            }
            return this;
        }

        public RecordValueBuilder WithTypedEventJson(JsonElement json, IEnumerable<string> referencedPaths)
        {
            var result = new JsonObject();
            foreach (var referencedPath in referencedPaths)
            {
                var dottedNames = referencedPath.Split('.');
                TraversePath(dottedNames, 0, json, result);
            }
            fields.Add(new NamedValue("event", FormulaValue.FromJson(JsonSerializer.Deserialize<JsonElement>(result))));
            return this;
        }

        public RecordValueBuilder WithTypedTestObject(TestObject testObj)
        {
            var testObjFields = new List<NamedValue>
            {
                new NamedValue("$id", testObj.Id == null ? FormulaValue.NewBlank() : FormulaValue.New(testObj.Id)),
                new NamedValue("$testSessionId", testObj.TestSessionId == null ? FormulaValue.NewBlank() : FormulaValue.New(testObj.TestSessionId)),
                new NamedValue("$metadata", ConvertMetadataToFormulaValue(testObj.GetMetadata()))
            };

            foreach (var prop in testObj.JTokenBag)
            {
                FormulaValue formulaValue = FormulaValue.NewBlank();
                formulaValue = prop.Value.Type switch
                {
                    JTokenType.String => FormulaValue.New(prop.Value.ToString()),
                    JTokenType.Boolean => FormulaValue.New((bool)prop.Value),
                    JTokenType.Float => FormulaValue.New((double)prop.Value),
                    JTokenType.Integer => FormulaValue.New((int)prop.Value),
                    _ => FormulaValue.NewBlank(),
                };
                testObjFields.Add(new NamedValue(prop.Key, formulaValue));
            }

            fields.Add(new NamedValue("testObj", FormulaValue.NewRecordFromFields(testObjFields)));
            return this;
        }

        private static FormulaValue ConvertMetadataToFormulaValue(RootMetadata metadata)
        {
            var fields = new List<NamedValue>
            {
                new NamedValue("type", metadata.Type == null ? FormulaValue.NewBlank() : FormulaValue.New(metadata.Type)),
                new NamedValue("time", metadata.Time == null ? FormulaValue.NewBlank() : FormulaValue.New(metadata.Time))
            };

            foreach (var propMetadata in metadata.PropertyMetadata)
            {
                fields.Add(new NamedValue(propMetadata.Key, RecordValue.NewRecordFromFields(
                    new NamedValue("type", propMetadata.Value.Type == null ? FormulaValue.NewBlank() : FormulaValue.New(propMetadata.Value.Type)),
                    new NamedValue("time", propMetadata.Value.Time == null ? FormulaValue.NewBlank() : FormulaValue.New(propMetadata.Value.Time)))));
            }

            return RecordValue.NewRecordFromFields(fields);
        }

        private static void TraversePath(string[] path, int pos, JsonElement originalJson, JsonObject output)
        {
            if (path.Length <= pos)
            {
                return;
            }

            var propertyName = path[pos];
            if (originalJson.TryGetProperty(propertyName, out JsonElement prop))
            {
                switch (prop.ValueKind)
                {
                    case JsonValueKind.String:
                        output.TryAdd(propertyName, prop.ToString());
                        break;
                    case JsonValueKind.Number:
                        output.TryAdd(propertyName, prop.GetDouble());
                        break;
                    case JsonValueKind.Null:
                        output.TryAdd(propertyName, null);
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        output.TryAdd(propertyName, prop.GetBoolean());
                        break;
                    case JsonValueKind.Object:
                        JsonObject outputProp;
                        if (output.TryGetPropertyValue(propertyName, out JsonNode? outputPropTmp))
                        {
                            outputProp = outputPropTmp.AsObject();
                        }
                        else
                        {
                            outputProp = new JsonObject();
                            output.TryAdd(propertyName, outputProp);
                        }

                        TraversePath(path, pos + 1, prop, outputProp);
                        break;
                }
            }
        }

        public RecordValue Build() => FormulaValue.NewRecordFromFields(fields);
    }
}
