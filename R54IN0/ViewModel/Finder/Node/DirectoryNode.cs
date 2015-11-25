using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace R54IN0
{
    public class DirectoryNode : IFinderNode
    {
        public ObservableCollection<IFinderNode> Nodes { get; set; }

        public virtual string Name { get; set; }

        public virtual bool AllowDrag { get; set; }

        public virtual bool AllowDrop { get; set; }

        public virtual bool AllowInsert { get; set; }

        public virtual Brush Color
        {
            get
            {
                return Brushes.Tan;
            }
            set
            {

            }
        }

        public DirectoryNode()
        {
            Nodes = new ObservableCollection<IFinderNode>();
            AllowDrag = true;
            AllowDrop = true;
            AllowInsert = true;
            UUID = Guid.NewGuid().ToString();
        }

        public string UUID { get; set; }

        public DirectoryNode(DirectoryNode thiz) : this()
        {
            AllowDrag = thiz.AllowDrag;
            AllowDrop = thiz.AllowDrop;
            AllowInsert = thiz.AllowInsert;
            Name = thiz.Name;
            UUID = thiz.UUID;
            foreach (IFinderNode i in thiz.Nodes)
            {
                if (i.GetType() == typeof(DirectoryNode))
                    Nodes.Add(new DirectoryNode(i as DirectoryNode));
                else if (i.GetType() == typeof(ItemNode))
                    Nodes.Add(new ItemNode(i as ItemNode));
            }
        }

        public DirectoryNode(string name) : this()
        {
            Name = name;
        }
    }
}
