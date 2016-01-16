using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0.WPF
{
    public class TreeViewNodeDirector
    {
        private const string JSON_TREE_KEY = "10120-3902-9013902";
        private static TreeViewNodeDirector _thiz;
        private ObservableCollection<TreeViewNode> _nodes;

        protected TreeViewNodeDirector()
        {
            LoadTree();
            Refresh();
        }

        public ObservableCollection<TreeViewNode> Collection
        {
            get
            {
                return _nodes;
            }
        }

        public static TreeViewNodeDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new TreeViewNodeDirector();
            return _thiz;
        }

        public static void Destroy()
        {
            if (_thiz != null)
            {
                _thiz.SaveTree();
                _thiz._nodes = null;
                _thiz = null;
            }
        }

        public void Add(TreeViewNode node)
        {
            if (!Contains(node))
                _nodes.Add(node);
        }

        public void Add(TreeViewNode parent, TreeViewNode child)
        {
            if (!Contains(child) && Contains(parent))
                parent.Root.Add(child);
        }

        public bool Contains(TreeViewNode node)
        {
            if (node.Type == NodeType.PRODUCT && string.IsNullOrEmpty(node.ProductID))
                throw new ArgumentException();

            return
                _nodes.Any(x => x.Descendants().Contains(node)) || //이미 자식루트에서 가지고 있는 경우
                _nodes.Contains(node) || //ROOT에서 가지고 있을 경우
               _nodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Any(x => x.ProductID == node.ProductID); //동일한 item 유니크키를 가지고 있는 경우
        }

        public TreeViewNode SearchProductNode(string id)
        {
            var nodes = Collection.SelectMany(x => x.Descendants().Where(y => y.ProductID == id));
            if (nodes.Count() == 1)
                return nodes.Single();
            return null;
        }

        /// <summary>
        /// Node삭제
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Remove(TreeViewNode node)
        {
            if (!Contains(node))
                return false;

            ObservableCollection<TreeViewNode> copy = new ObservableCollection<TreeViewNode>(_nodes);
            TreeViewNode parent = copy.SelectMany(x => x.Descendants()).Where(x => x.Root.Contains(node)).SingleOrDefault();
            if (parent != null)
                return parent.Root.Remove(node);
            else
                return _nodes.Remove(node); //부모를 못 찾으면 마스터 루트에 있는 것.
        }

        /// <summary>
        /// Node삭제
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public bool Remove(string itemID)
        {
            ObservableCollection<TreeViewNode> copy = new ObservableCollection<TreeViewNode>(_nodes);
            TreeViewNode node = copy.SelectMany(x => x.Descendants()).Where(x => x.Type == NodeType.PRODUCT && x.ProductID == itemID).SingleOrDefault();
            return Remove(node);
        }

        /// <summary>
        /// 데이터 저장
        /// </summary>
        public void SaveTree()
        {
            string json = JsonConvert.SerializeObject(_nodes);
            using (var db = LexDb.GetDbInstance())
            {
                db.Save(new TreeViewNodeJsonFormat(JSON_TREE_KEY, json));
            }
        }

        /// <summary>
        /// 현재 Product List와 동기화
        /// </summary>
        public void Refresh()
        {
            //var fwd = FieldWrapperDirector.GetInstance();
            //var itemws = fwd.CreateCollection<Item, ItemWrapper>().Where(x => !x.IsDeleted);

            ObservableFieldDirector ofd = ObservableFieldDirector.GetInstance();

            IEnumerable<TreeViewNode> productNodes = _nodes.SelectMany(x => x.Descendants()).Where(x => x.Type == NodeType.PRODUCT);
            foreach (TreeViewNode node in new List<TreeViewNode>(productNodes)) //없는 Item은 삭제
            {
                if (ofd.Search<Product>(node.ProductID) == null)
                    Remove(node);
            }

            productNodes = _nodes.SelectMany(x => x.Descendants()).Where(x => x.Type == NodeType.PRODUCT);
            foreach (var product in ofd.CreateList<Product>()) //Item 목록에는 존재하지만 Finder에는 없는 경우
            {
                if (!productNodes.Any(x => x.ProductID == product.ID))
                    Add(new TreeViewNode(NodeType.PRODUCT, product.ID));
            }
        }

        /// <summary>
        /// 데이터 로드
        /// </summary>
        private void LoadTree()
        {
            using (var db = LexDb.GetDbInstance())
            {
                TreeViewNodeJsonFormat fmt = db.LoadByKey<TreeViewNodeJsonFormat>(JSON_TREE_KEY);
                if (fmt != null)
                    _nodes = JsonConvert.DeserializeObject<ObservableCollection<TreeViewNode>>(fmt.Data);
                else
                    _nodes = new ObservableCollection<TreeViewNode>();
            }
        }
    }
}