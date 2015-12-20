using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public static class FinderNodeExtension
    {
        public static IEnumerable<FinderNode> Descendants(this FinderNode root)
        {
            var nodes = new Stack<FinderNode>(new[] { root });
            while (nodes.Any())
            {
                FinderNode node = nodes.Pop();
                yield return node;
                foreach (var n in node.Nodes)
                    nodes.Push(n);
            }
        }
    }
}
