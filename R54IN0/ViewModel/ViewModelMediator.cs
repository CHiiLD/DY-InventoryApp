using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class ViewModelMediator
    {
        private static ViewModelMediator _mediator;
        private List<AViewModelMediatorColleague> _colleagues;

        private ViewModelMediator()
        {
            _colleagues = new List<AViewModelMediatorColleague>();
        }

        public static ViewModelMediator GetInstance()
        {
            if (_mediator == null)
                _mediator = new ViewModelMediator();
            return _mediator;
        }

        public void register(AViewModelMediatorColleague colleague)
        {
            if (!_colleagues.Any(x => x == colleague))
                _colleagues.Add(colleague);
        }

        public void OnViewModelChanged(AViewModelMediatorColleague colleague, object args)
        {
            Type type = colleague.GetType();
            if (type == typeof(InventoryFinderViewModel))
                OnFinderSelectedItemsChanged(colleague as InventoryFinderViewModel, args);
            else if (type == typeof(ItemFieldEditorViewModel))
                OnItemFieldEditorItemChanged(colleague as ItemFieldEditorViewModel, args);
        }

        /// <summary>
        /// 아이템 필드의 삭제 및 추가를 할 시 변경점을 파인더의 리스트에 업
        /// </summary>
        /// <param name="itemFieldViewModel"></param>
        void OnItemFieldEditorItemChanged(ItemFieldEditorViewModel itemFieldViewModel, object args)
        {
            if (!(args is Item))
                return;
            var finders = _colleagues.OfType<InventoryFinderViewModel>();
            Item item = args as Item;
            ObservableCollection<ItemPipe> items = itemFieldViewModel.Items;
            bool result = items.Any(x => x.Field.UUID == item.UUID);
            foreach (var f in finders)
            {
                if (result)
                    f.AddNewItemInNodes(item.UUID);
                else
                    f.RemoveItemInNodes(item.UUID);
            }
        }

        /// <summary>
        /// 파인더에서 디렉토리 선택시 데이터그리드에 해당 아이템의 재고 목록 표시
        /// </summary>
        /// <param name="finderViewModel"></param>
        void OnFinderSelectedItemsChanged(InventoryFinderViewModel finderViewModel, object args)
        {
            IEnumerable<FinderNode> itemNodes = finderViewModel.SelectedNodes.SelectMany(x =>
            x.Descendants().Where(y => y.Type == NodeType.ITEM));
            itemNodes = itemNodes.Distinct();
            List<InventoryPipe> pipeList = new List<InventoryPipe>();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Inventory[] invens = db.LoadAll<Inventory>();
                foreach (var itemNode in itemNodes)
                {
                    IEnumerable<Inventory> inventory = invens.Where(x => x.ItemUUID == itemNode.ItemUUID);
                    pipeList.AddRange(inventory.Select(x => new InventoryPipe(x)));
                }
            }
            var datagrids = _colleagues.OfType<InventoryDataGridViewModel>();
            foreach (var vm in datagrids)
                vm.ChangeInventoryItems(pipeList);
        }
    }
}
