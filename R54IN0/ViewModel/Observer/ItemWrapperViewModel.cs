using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace R54IN0
{
    public class ItemWrapperViewModel : FieldWrapperViewModel<Item, ItemWrapper>, IFinderViewModelCreatation
    {
        private SpecificationWrapperViewModel _specViewModel;

        public ItemWrapperViewModel(CollectionViewModelObserverSubject sub) : base(sub)
        {
            _specViewModel = new SpecificationWrapperViewModel(sub, this);
        }

        /// <summary>
        /// 새로운 규격 데이터 추가
        /// </summary>
        public CommandHandler AddNewSpecCommand
        {
            get
            {
                return _specViewModel.AddNewItemCommand;
            }
        }

        /// <summary>
        /// 선택된 규격 데이터 삭제
        /// </summary>
        public CommandHandler RemoveSpecCommand
        {
            get
            {
                return _specViewModel.DeleteItemCommand;
            }
        }

        public ObservableCollection<SpecificationWrapper> Specifications
        {
            get
            {
                return _specViewModel.Items;
            }
            set
            {
                _specViewModel.Items = value;
                OnPropertyChanged("Specifications");
            }
        }

        public override ItemWrapper SelectedItem
        {
            get
            {
                return base.SelectedItem;
            }
            set
            {
                base.SelectedItem = value;
                //선택된 아이템에 대한 규격 데이터를 불러와 Specifications컬렉션에 저장합니다.
                if (base.SelectedItem != null && _specViewModel != null)
                {
                    FieldWrapperDirector fwd = FieldWrapperDirector.GetInstance();
                    var collection = fwd.CreateCollection<Specification, SpecificationWrapper>().
                        Where(x => !x.IsDeleted && x.Field.ItemID == base.SelectedItem.ID);
                    Specifications = new ObservableCollection<SpecificationWrapper>(collection);
                }
                if (_specViewModel != null)
                    _specViewModel.AddNewItemCommand.UpdateCanExecute();
            }
        }

        public SpecificationWrapper SelectedSpecification
        {
            get
            {
                return _specViewModel.SelectedItem;
            }
            set
            {
                _specViewModel.SelectedItem = value;
                OnPropertyChanged("SelectedSpecification");
                RemoveSpecCommand.UpdateCanExecute();
            }
        }

        public FinderViewModel FinderViewModel
        {
            get;
            set;
        }

        public override void Add(ItemWrapper item)
        {
            base.Add(item);
            FinderDirector.GetInstance().Add(new FinderNode(NodeType.ITEM) { ItemID = item.ID });
        }

        public override void Remove(ItemWrapper item)
        {
            base.Remove(item);
            FinderDirector.GetInstance().Remove(item.ID);
        }

        public override void ExecuteNewItemAddition(object parameter)
        {
            base.ExecuteNewItemAddition(parameter);
            // 새로 아이템을 등록할 시, 새로운 규격을 등록한다. 규격 리스트는 최소 하나 이상을 가져야 한며, 그 이하로는 삭제를 할 수 없다.
            _specViewModel.AddNewItemCommand.Execute(null);

            if (FinderViewModel != null) //새롭게 생성한 품목 데이터에 포커스와 Select 상황을 준다.
            {
                FinderViewModel.SelectedNodes.Clear();
                var newNode = FinderViewModel.Nodes.Last();
                FinderViewModel.OnNodeSelected(null, new SelectionChangedCancelEventArgs(new List<FinderNode>() { newNode }, new List<FinderNode>()));
            }
        }

        public void OnFinderViewSelectItemChanged(object sender, EventArgs e)
        {
            var _finderViewModel = sender as ItemFinderViewModel;
            if (_finderViewModel != null)
            {
                //Finder에서 품목들을 선택하였을 때 그 품목들의 리스트와 품목에 속한 규격 리스트를 업데이트한다.
                List<ItemWrapper> itemwTemp = new List<ItemWrapper>();
                IEnumerable<FinderNode> itemNodes = _finderViewModel.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
                var itemws = fieldWrapperDirector.CreateCollection<Item, ItemWrapper>();
                foreach (var itemNode in itemNodes)
                    itemwTemp.AddRange(itemws.Where(x => x.ID == itemNode.ItemID && !x.IsDeleted));
                Items = new ObservableCollection<ItemWrapper>(itemwTemp);

                List<SpecificationWrapper> specwTemp = new List<SpecificationWrapper>();
                foreach (var item in Items)
                {
                    var collection = fieldWrapperDirector.CreateCollection<Specification, SpecificationWrapper>().
                    Where(x => !x.IsDeleted && x.Field.ItemID == item.ID);
                    specwTemp.AddRange(collection);
                }
                Specifications = new ObservableCollection<SpecificationWrapper>(specwTemp);
            }
        }

        public FinderViewModel CreateFinderViewModel(TreeViewEx treeView)
        {
            FinderViewModel = new ItemFinderViewModel(treeView);
            FinderViewModel.SelectItemsChanged += OnFinderViewSelectItemChanged;
            return FinderViewModel;
        }

        public override void ExecuteSelectedItemDeletion(object parameter)
        {
            //품목을 삭제할 땐, 품목의 규격들도 같이 삭제한다.
            foreach (var specw in new List<SpecificationWrapper>(Specifications))
            {
                SelectedSpecification = specw;
                _specViewModel.DeleteItemCommand.Execute(null);
            }
            base.ExecuteSelectedItemDeletion(parameter);
        }
    }
}