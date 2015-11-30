﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace R54IN0
{
    public class ItemFieldEditorViewModel : AViewModelMediatorColleague, IFieldEditorViewModel
    {
        ItemPipe _selectedItem;
        SpecificationPipe _selectedSpecification { get; set; }

        public CommandHandler AddNewItemCommand { get; set; }
        public CommandHandler RemoveItemCommand { get; set; }

        public CommandHandler AddNewSpecCommand { get; set; }
        public CommandHandler RemoveSpecCommand { get; set; }

        public ObservableCollection<ItemPipe> Items { get; set; }

        public ItemPipe SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                Specifications.Clear();
                if (_selectedItem != null)
                {
                    IEnumerable<Specification> result = null;
                    using (var db = DatabaseDirector.GetDbInstance())
                    {
                        result = db.Table<Specification>().IndexQueryByKey("ItemUUID", _selectedItem.Field.UUID).ToList().Where(x => !x.IsDeleted);
                    }
                    foreach (var i in result)
                        Specifications.Add(new SpecificationPipe(i));
                }
                SelectedSpecification = Specifications.FirstOrDefault();
            }
        }

        public ObservableCollection<SpecificationPipe> Specifications { get; set; }

        public SpecificationPipe SelectedSpecification
        {
            get
            {
                return _selectedSpecification;
            }
            set
            {
                _selectedSpecification = value;
            }
        }

        public ItemFieldEditorViewModel() : base(ViewModelMediator.GetInstance())
        {
            Item[] items = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                items = db.LoadAll<Item>();
            }
            IEnumerable<ItemPipe> itemPipes = items.Where(x => !x.IsDeleted).Select(x => new ItemPipe(x));
            Items = new ObservableCollection<ItemPipe>(itemPipes);
            Specifications = new ObservableCollection<SpecificationPipe>();
            SelectedItem = Items.FirstOrDefault();

            AddNewItemCommand = new CommandHandler(AddNewItem, CanAddNewItem);
            RemoveItemCommand = new CommandHandler(RemoveSelectedItem, CanRemoveSelectedItem);

            AddNewSpecCommand = new CommandHandler(AddNewSpecification, CanAddNewSpecficiation);
            RemoveSpecCommand = new CommandHandler(RemoveSelectedSpecification, CanRemoveSelectedSpecfication);
        }

        public bool CanAddNewItem(object parameter)
        {
            return true;
        }

        public bool CanRemoveSelectedItem(object parameter)
        {
            return SelectedItem != null ? true : false;
        }

        public bool CanAddNewSpecficiation(object parameter)
        {
            return true;
        }

        public bool CanRemoveSelectedSpecfication(object parameter)
        {
            return SelectedSpecification != null ? true : false;
        }

        public void AddNewItem(object parameter)
        {
            var item = new Item() { Name = "new item", UUID = Guid.NewGuid().ToString() }.Save<Item>();
            Items.Add(new ItemPipe(item));
            SelectedItem = Items.LastOrDefault();
            /// 새로 아이템을 등록할 시 베이스 규격을 등록, 규격 리스트는 최소 하나 이상을 가져야 한다.
            AddNewSpecification(null);
            Changed(item);
            RemoveItemCommand.UpdateCanExecute();
        }

        public void RemoveSelectedItem(object parameter)
        {
            var item = SelectedItem.Field;
            SelectedItem.IsDeleted = true;
            Items.Remove(SelectedItem);
            SelectedItem = Items.FirstOrDefault();
            Changed(item);
            RemoveItemCommand.UpdateCanExecute();
        }

        public void AddNewSpecification(object parameter)
        {
            var newSpecification = new Specification() { Name = "new specification", ItemUUID = SelectedItem.Field.UUID }.Save<Specification>();
            var newSpecificationPipe = new SpecificationPipe(newSpecification);
            Specifications.Add(newSpecificationPipe);
            SelectedSpecification = Specifications.LastOrDefault();
            RemoveSpecCommand.UpdateCanExecute();
        }

        public void RemoveSelectedSpecification(object parameter)
        {
            if (SelectedSpecification != null && Specifications.Count > 1)
                SelectedSpecification.IsDeleted = true;
            Specifications.Remove(SelectedSpecification);
            SelectedSpecification = Specifications.FirstOrDefault();
            RemoveSpecCommand.UpdateCanExecute();
        }
    }
}