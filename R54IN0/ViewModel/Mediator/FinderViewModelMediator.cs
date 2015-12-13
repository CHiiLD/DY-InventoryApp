using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class FinderViewModelMediator
    {
        static FinderViewModelMediator _mediator;
        List<AFinderViewModelMediatorColleague> _colleagues;

        FinderViewModelMediator()
        {
            _colleagues = new List<AFinderViewModelMediatorColleague>();
        }

        public static FinderViewModelMediator GetInstance()
        {
            if (_mediator == null)
                _mediator = new FinderViewModelMediator();
            return _mediator;
        }

        public void register(AFinderViewModelMediatorColleague colleague)
        {
            if (!_colleagues.Any(x => x == colleague))
                _colleagues.Add(colleague);
        }

        /// <summary>
        /// 아이템 필드의 삭제 및 추가를 할 시 변경점을 파인더의 리스트에 업
        /// </summary>
        /// <param name="itemFieldViewModel"></param>
        public void OnItemPipeCollectionChanged(ItemPipe item, bool isAddAction)
        {
            var finders = _colleagues.OfType<InventoryFinderViewModel>();
            foreach (var finder in finders)
            {
                if (isAddAction)
                    finder.AddNewItemInNodes(item.Field.UUID);
                else
                    finder.RemoveItemInNodes(item.Field.UUID);
            }
        }

        /// <summary>
        /// 파인더에서 디렉토리 선택시 데이터그리드에 해당 아이템의 재고 목록 표시
        /// </summary>
        /// <param name="finderViewModel"></param>
        public void OnFinderNodesSelected(InventoryFinderViewModel finderViewModel)
        {
            IEnumerable<FinderNode> itemNodes = finderViewModel.SelectedNodes.
                SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
            itemNodes = itemNodes.Distinct();
            List<InventoryPipe> newList = new List<InventoryPipe>();
            ObservableCollection<InventoryPipe> pipes = InventoryPipeCollectionDirector.GetInstance().LoadPipe();
            foreach (var itemNode in itemNodes)
            {
                var inventoryPipes = pipes.Where(x => x.Inven.ItemUUID == itemNode.ItemUUID);
                newList.AddRange(inventoryPipes);
            }
            var inventoryDataGridViewModels = _colleagues.OfType<InventoryDataGridViewModel>();
            foreach (var vm in inventoryDataGridViewModels)
                vm.ChangeInventoryItems(newList);
        }
    }
}