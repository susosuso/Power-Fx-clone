using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Syntax;

namespace PowerFXBenchmark
{
    public class ParsedContext
    {
        public ParseResult ParseResult { get; set; }

        public IList<string> ReferencedPaths { get; set; }

        public ParsedContext(ParseResult parseResult)
        {
            ParseResult = parseResult;
            ReferencedPaths = new List<string>();
            FindReferencePaths(ParseResult.Root);
        }

        private void FindReferencePaths(TexlNode node)
        {
            if (node is NameNode nn)
            {
                switch (node.Kind)
                {
                    case NodeKind.Parent:
                    case NodeKind.DottedName:
                    case NodeKind.FirstName:
                        ReferencedPaths.Add(nn.ToString());
                        break;
                }
            }
            if (node is BinaryOpNode bin)
            {
                FindReferencePaths(bin.Left);
                FindReferencePaths(bin.Right);
            }
            if (node is UnaryOpNode uno)
            {
                FindReferencePaths(uno.Child);
            }
            if (node is ListNode list)
            {
                foreach (var listNode in list.ChildNodes)
                {
                    FindReferencePaths(listNode);
                }
            }
            if (node is CallNode call)
            {
                FindReferencePaths(call.Args);
            }
        }
    }
}