using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public enum CollectionAction
    {
        NONE,
        ADD,
        REMOVE,
    }

    public class FinderViewModelMediator
    {
        static FinderViewModelMediator _mediator;
        List<FinderViewModelMediatorColleague> _colleagues;
        Dictionary<FinderViewModel, IUpdateNewItems> _finderDataGridPair;

        FinderViewModelMediator()
        {
            _colleagues = new List<FinderViewModelMediatorColleague>();
            _finderDataGridPair = new Dictionary<FinderViewModel, IUpdateNewItems>();
        }

        public static FinderViewModelMediator GetInstance()
        {
            if (_mediator == null)
                _mediator = new FinderViewModelMediator();
            return _mediator;
        }

        public void Register(FinderViewModelMediatorColleague colleague)
        {
            if (!this._colleagues.Contains(colleague))
                this._colleagues.Add(colleague);
        }

        public void RegisterControlPair(FinderViewModel viewModel, IUpdateNewItems iUpdate)
        {
            if(!_finderDataGridPair.ContainsKey(viewModel))
                _finderDataGridPair.Add(viewModel, iUpdate);
        }

        public void Cancellation(FinderViewModelMediatorColleague colleague)
        {
            if (this._colleagues.Contains(colleague))
                this._colleagues.Remove(colleague);

            if (colleague is FinderViewModel)
            {
                FinderViewModel fvm = colleague as FinderViewModel;
                if (this._finderDataGridPair.ContainsKey(fvm))
                    this._finderDataGridPair.Remove(fvm);
            }
        }

        /// <summary>
        /// 아이템 필드의 삭제 및 추가를 할 시 변경점을 파인더의 리스트에 업
        /// </summary>
        /// <param name="itemFieldViewModel"></param>
        public void OnItemPipeCollectionChanged(ItemWrapper item, CollectionAction action)
        {
            var finders = _colleagues.OfType<FinderViewModel>();
            foreach (var finder in finders)
            {
                if (action == CollectionAction.ADD)
                    finder.AddNewItemInNodes(item.Field.UUID);
                else if (action == CollectionAction.REMOVE)
                    finder.RemoveItemInNodes(item.Field.UUID);
            }
        }

        /// <summary>
        /// 파인더에서 디렉토리 선택시 데이터그리드에 해당 아이템의 재고 목록 표시
        /// </summary>
        /// <param name="finderViewModel"></param>
        public void OnFinderNodesSelected(FinderViewModel finderViewModel)
        {
            if (!_finderDataGridPair.ContainsKey(finderViewModel))
                return;
            IEnumerable<FinderNode> itemNodes = finderViewModel.SelectedNodes.
                SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
            itemNodes = itemNodes.Distinct();
            List<object> newList = new List<object>();
            IEnumerable<object> pipes = _finderDataGridPair[finderViewModel].LoadPipe(); // InventoryPipeCollectionDirector.GetInstance().LoadPipe();
            foreach (var itemNode in itemNodes)
            {
                var inventoryPipes = pipes.Where(x => ((IRecordWrapper)x).Inven.ItemUUID == itemNode.ItemUUID);
                newList.AddRange(inventoryPipes);
            }
            _finderDataGridPair[finderViewModel].UpdateNewItems(newList);
        }
    }
}