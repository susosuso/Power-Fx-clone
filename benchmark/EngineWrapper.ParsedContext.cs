using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;
using Newtonsoft.Json.Linq;
using PowerFXBenchmark.Builders;
using PowerFXBenchmark.Inputs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PowerFXBenchmark
{
    public partial class EngineWrapper
    {
        public ParsedContext ParseExpressionAndExtractPaths(string expression)
        {
            var parsed = engine.Parse(expression);
            return new ParsedContext(parsed);
        }

        public async Task<object> EvaluateExpressionAsync(ParsedContext parsedContext, TestObject testObject, JsonElement telemetry, CancellationToken cancellationToken = default)
        {
            var input = new RecordValueBuilder()
                .WithTypedTestObject(testObject)
                .WithTypedEventJson(telemetry, parsedContext.ReferencedPaths.Where(s => s.StartsWith("event")).Select(s => s.Remove(0, 6)))
                .Build();
            var checkedResult = engine.Check(parsedContext.ParseResult, input.Type);
            checkedResult.ThrowOnErrors();
            var result = await checkedResult.Expression.EvalAsync(input, cancellationToken).ConfigureAwait(false);
            return result.ToObject();
        }
    }
}
