using System.Collections.Generic;
using System.Linq;

namespace R54IN0.WPF
{
    public static class TreeViewNodeExtension
    {
        public static IEnumerable<TreeViewNode> Descendants(this TreeViewNode root)
        {
            var nodes = new Stack<TreeViewNode>(new[] { root });
            while (nodes.Any())
            {
                TreeViewNode node = nodes.Pop();
                yield return node;
                foreach (var n in node.Root)
                    nodes.Push(n);
            }
        }
    }
}