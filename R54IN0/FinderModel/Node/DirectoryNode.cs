using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace R54IN0
{
    public class DirectoryNode
    {
        public ObservableCollection<DirectoryNode> Nodes { get; set; }

        public virtual string Name { get; set; }

        public bool AllowDrag { get { return !IsInEditMode; } }

        public virtual bool AllowDrop { get; set; }

        public virtual bool AllowInsert { get; set; }

        public virtual Brush Color { get { return Brushes.Tan; } }

        //public virtual bool IsEditable
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        public bool IsInEditMode { get; set; }

        public string UUID { get; set; }

        public DirectoryNode()
        {
            Nodes = new ObservableCollection<DirectoryNode>();
            AllowDrop = true;
            AllowInsert = true;
            UUID = Guid.NewGuid().ToString();
        }

        public DirectoryNode(DirectoryNode thiz) : this()
        {
            AllowDrop = thiz.AllowDrop;
            AllowInsert = thiz.AllowInsert;
            Name = thiz.Name;
            UUID = thiz.UUID;
            foreach (DirectoryNode i in thiz.Nodes)
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