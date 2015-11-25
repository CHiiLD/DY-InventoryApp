using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace R54IN0
{
    public interface IFinderNode
    {
        ObservableCollection<IFinderNode> Nodes { get; set; }
        Brush Color { get; set; }
        string Name { get; set; }
        bool AllowDrag { get; set; }
        bool AllowDrop { get; set; }
        bool AllowInsert { get; set; }
        string UUID { get; set; }
    }
}
