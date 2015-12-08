using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public static class IDirectoryNodeExtention
    {
        public static IEnumerable<DirectoryNode> Descendants(this DirectoryNode root)
        {
            //yield return root;
            var nodes = new Stack<DirectoryNode>(new[] { root });
            while (nodes.Any())
            {
                DirectoryNode node = nodes.Pop();
                yield return node;
                foreach (var n in node.Nodes)
                    nodes.Push(n);
            }
        }
    }
}
