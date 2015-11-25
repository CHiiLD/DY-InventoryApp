using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public static class IFinderNodeExtention
    {
        public static IEnumerable<IFinderNode> Descendants(this IFinderNode root)
        {
            var nodes = new Stack<IFinderNode>(new[] { root });
            while (nodes.Any())
            {
                IFinderNode node = nodes.Pop();
                yield return node;
                foreach (var n in node.Nodes)
                    nodes.Push(n);
            }
        }
    }
}
