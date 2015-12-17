using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace R54IN0
{
    public class SimpleStringFormat
    {
        public string UUID { get; set; }
        public string Data { get; set; }

        public SimpleStringFormat()
        { }

        public SimpleStringFormat(string uuid, string data)
        {
            UUID = uuid;
            Data = data;
        }
    }

    public class FinderNodeCollectionDirector
    {
        static FinderNodeCollectionDirector _thiz;
        ObservableCollection<FinderNode> _nodes;
        const string JSON_TREE_KEY = "JSON_TREE_KEY";

        public ObservableCollection<FinderNode> Collection
        {
            get
            {
                return _nodes;
            }
        }

        FinderNodeCollectionDirector()
        {
            LoadTree();
            Refresh();
        }

        public static FinderNodeCollectionDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new FinderNodeCollectionDirector();
            return _thiz;
        }

        public static void Distroy()
        {
            if (_thiz != null)
            {
                _thiz.SaveTree();
                _thiz = null;
            }
        }

        void LoadTree()
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                var ssf = db.LoadByKey<SimpleStringFormat>(JSON_TREE_KEY);
                if (ssf != null)
                    _nodes = JsonConvert.DeserializeObject<ObservableCollection<FinderNode>>(ssf.Data);
                else
                    _nodes = new ObservableCollection<FinderNode>();
            }
        }

        public void SaveTree()
        {
            string json = JsonConvert.SerializeObject(_nodes);
            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Save(new SimpleStringFormat(JSON_TREE_KEY, json));
            }
        }

        public void Refresh()
        {
            var items = FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Item>();
            foreach (var item in items)
            {
                bool result = _nodes.Any(n => n.Descendants().Where(x => x.Type == NodeType.ITEM).Any(x => x.ItemUUID == item.Field.UUID));
                if (!result)
                    _nodes.Add(new FinderNode(NodeType.ITEM) { ItemUUID = item.Field.UUID });
            }
        }
    }
}
