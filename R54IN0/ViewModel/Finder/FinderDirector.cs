using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0
{
    public class FinderTreeNodeJsonRecord
    {
        public string ID { get; set; }
        public string Data { get; set; }

        public FinderTreeNodeJsonRecord()
        { }

        public FinderTreeNodeJsonRecord(string id, string data)
        {
            ID = id;
            Data = data;
        }
    }

    public class FinderDirector
    {
        private const string JSON_TREE_KEY = "JSON_TREE_KEY";
        private static FinderDirector _thiz;
        private ObservableCollection<FinderNode> _nodes;

        private FinderDirector()
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
            if (node.Type == NodeType.PRODUCT && string.IsNullOrEmpty(node.ItemID))
                throw new ArgumentException();

            return
                _nodes.Any(x => x.Descendants().Contains(node)) || //이미 자식루트에서 가지고 있는 경우
                _nodes.Contains(node) || //ROOT에서 가지고 있을 경우
               _nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Any(x => x.ItemID == node.ItemID); //동일한 item 유니크키를 가지고 있는 경우
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

        public bool Remove(string itemID)
        {
            ObservableCollection<FinderNode> copy = new ObservableCollection<FinderNode>(_nodes);
            FinderNode node = copy.SelectMany(x => x.Descendants()).Where(x => x.Type == NodeType.PRODUCT && x.ItemID == itemID).SingleOrDefault();
            return Remove(node);
        }

        public void SaveTree()
        {
            string json = JsonConvert.SerializeObject(_nodes);
            using (var db = LexDb.GetDbInstance())
            {
                db.Save(new FinderTreeNodeJsonRecord(JSON_TREE_KEY, json));
            }
        }

        public void Refresh()
        {
            var fwd = FieldWrapperDirector.GetInstance();
            var itemws = fwd.CreateCollection<Item, ItemWrapper>().Where(x => !x.IsDeleted);

            var itemNodes = _nodes.SelectMany(x => x.Descendants()).Where(x => x.Type == NodeType.PRODUCT);
            foreach (FinderNode node in new List<FinderNode>(itemNodes)) //없는 Item은 삭제
            {
                if (!itemws.Any(x => x.ID == node.ItemID))
                    Remove(node);
            }

            itemNodes = _nodes.SelectMany(x => x.Descendants()).Where(x => x.Type == NodeType.PRODUCT);
            foreach (ItemWrapper itemw in itemws)//Item 목록에는 존재하지만 Finder에는 없는 경우
            {
                if (!itemNodes.Any(x => x.ItemID == itemw.Field.ID))
                    Add(new FinderNode(NodeType.PRODUCT) { ItemID = itemw.Field.ID });
            }
        }

        private void LoadTree()
        {
            using (var db = LexDb.GetDbInstance())
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