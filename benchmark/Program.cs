// See https://aka.ms/new-console-template for more information
using PowerFXBenchmark;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<Benchmark>();

//var bench = new Benchmark();
//var result = await bench.Evaluation_UntypedInput_ParseJSON().ConfigureAwait(false);
//Console.WriteLine(result);
//result = await bench.Evaluation_UntypedInput_CustomUntypedObject().ConfigureAwait(false);
//Console.WriteLine(result);
//result = await bench.Evaluation_StronglyTypedInput_FromJson().ConfigureAwait(false);
//Console.WriteLine(result);
//result = await bench.Evaluation_StronglyTypedInput_ChristianApproach().ConfigureAwait(false);
//Console.WriteLine(result);
//result = await bench.Evaluation_Expression_Complexity1().ConfigureAwait(false);
//Console.WriteLine(result);
//result = await bench.Evaluation_Expression_Complexity2().ConfigureAwait(false);
//Console.WriteLine(result);

//while (true)
//{
//    var expr = Console.ReadLine();
//    try
//    {
//        var parsedContext = bench.engine.ParseExpressionAndExtractPaths(expr);
//        var rs = await bench.EvalParsedContext(parsedContext).ConfigureAwait(false);
//        Console.WriteLine(rs);
//    }
//    catch (Exception ex)
//    {
//        Console.ForegroundColor = ConsoleColor.Red;
//        Console.WriteLine(ex.ToString());
//        Console.ResetColor();
//    }
//}
