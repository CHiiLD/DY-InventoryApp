using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace R54IN0
{
    public class FinderTreeNodeJsonRecord
    {
        public string UUID { get; set; }
        public string Data { get; set; }

        public FinderTreeNodeJsonRecord()
        { }

        public FinderTreeNodeJsonRecord(string uuid, string data)
        {
            UUID = uuid;
            Data = data;
        }
    }

    public class FinderDirector
    {
        const string JSON_TREE_KEY = "JSON_TREE_KEY";
        static FinderDirector _thiz;
        ObservableCollection<FinderNode> _nodes;

        FinderDirector()
        {
            LoadTree();
            Refresh();
        }

        public ObservableCollection<FinderNode> Collection
        {
            get
            {
                return _nodes;
            }
        }

        public static FinderDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new FinderDirector();
            return _thiz;
        }

        public static void Distroy()
        {
            if (_thiz != null)
            {
                _thiz.SaveTree();
                _thiz._nodes = null;
                _thiz = null;
            }
        }

        public void Add(FinderNode node)
        {
            if (!Contains(node))
                _nodes.Add(node);
        }

        public void Add(FinderNode parent, FinderNode child)
        {
            if (!Contains(child) && Contains(parent))
                parent.Nodes.Add(child);
        }

        public bool Contains(FinderNode node)
        {
            return _nodes.Any(x => x.Descendants().Contains(node)) || _nodes.Contains(node);
        }

        public bool Remove(FinderNode node)
        {
            if (!Contains(node))
                return false;

            ObservableCollection<FinderNode> copy = new ObservableCollection<FinderNode>(_nodes);
            FinderNode parent = copy.SelectMany(x => x.Descendants()).Where(x => x.Nodes.Contains(node)).SingleOrDefault();
            if (parent != null)
                return parent.Nodes.Remove(node);
            else
                return _nodes.Remove(node); //부모를 못 찾으면 마스터 루트에 있는 것.
        }

        public bool Remove(string itemUuid)
        {
            ObservableCollection<FinderNode> copy = new ObservableCollection<FinderNode>(_nodes);
            FinderNode node = copy.SelectMany(x => x.Descendants()).Where(x => x.Type == NodeType.ITEM && x.ItemUUID == itemUuid).SingleOrDefault();
            return Remove(node);
        }

        public void SaveTree()
        {
            string json = JsonConvert.SerializeObject(_nodes);
            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Save(new FinderTreeNodeJsonRecord(JSON_TREE_KEY, json));
            }
        }

        public void Refresh()
        {
            var itemws = FieldWrapperDirector.GetInstance().CreateFieldWrapperCollection<Item, ItemWrapper>().Where(x => !x.IsDeleted); //FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Item>();
            foreach (ItemWrapper itemw in itemws)
            {
                bool result = _nodes.Any(n => n.Descendants().Where(x => x.Type == NodeType.ITEM).Any(x => x.ItemUUID == itemw.Field.UUID));
                if (!result)
                    _nodes.Add(new FinderNode(NodeType.ITEM) { ItemUUID = itemw.Field.UUID });
            }
        }

        void LoadTree()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                var ssf = db.LoadByKey<FinderTreeNodeJsonRecord>(JSON_TREE_KEY);
                if (ssf != null)
                    _nodes = JsonConvert.DeserializeObject<ObservableCollection<FinderNode>>(ssf.Data);
                else
                    _nodes = new ObservableCollection<FinderNode>();
            }
        }
    }
}
