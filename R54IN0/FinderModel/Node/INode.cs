using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace R54IN0
{
    public interface INode
    {
        NodeType Type { get; set; }

        ObservableCollection<INode> Nodes { get; set; }

        string ItemUUID { get; set; }

        string Name { get; set; }

        bool AllowDrag { get; set; }

        bool AllowDrop { get; set; }

        bool AllowInsert { get; set; }

        Brush Color { get; set; }

        bool IsEditable { get; set; }

        bool IsInEditMode { get; set; }
    }
}
