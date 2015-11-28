using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class ItemFieldEditorViewModel : IFieldEditorViewModel
    {
        ItemPipe _selectedItem;
        SpecificationPipe _selectedSpecification { get; set; }
        SortedDictionary<string, List<SpecificationPipe>> _awaters;

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
                _awaters[_selectedItem.Field.UUID] = Specifications.ToList();
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

        public ItemFieldEditorViewModel()
        {
            Item[] items = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                items = db.LoadAll<Item>();
            }
            IEnumerable<ItemPipe> itemPipes = items.Where(x => !x.IsDeleted).Select(x => new ItemPipe(x));
            Items = new ObservableCollection<ItemPipe>(itemPipes);
            Specifications = new ObservableCollection<SpecificationPipe>();
            _awaters = new SortedDictionary<string, List<SpecificationPipe>>();
            SelectedItem = Items.FirstOrDefault();
        }

        public void AddNewItem()
        {
            Items.Add(new ItemPipe(new Item() { UUID = Guid.NewGuid().ToString() }));
            SelectedItem = Items.LastOrDefault();
        }

        public void AddNewSpecification()
        {
            if (SelectedItem != null)
            {
                var spec = new Specification() { ItemUUID = SelectedItem.Field.UUID };
                var specPipe = new SpecificationPipe(spec);
                Specifications.Add(specPipe);
                SelectedSpecification = Specifications.LastOrDefault();
                if (_awaters.ContainsKey(spec.ItemUUID))
                    _awaters[spec.ItemUUID].Add(specPipe);
            }
        }

        public void RemoveSelectedItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Field.IsDeleted = true;
                Items.Remove(SelectedItem);
                SelectedItem = Items.FirstOrDefault();
            }
        }

        public void RemoveSelectedSpecification()
        {
            if (SelectedSpecification != null)
            {
                SelectedSpecification.Field.IsDeleted = true;
                Specifications.Remove(SelectedSpecification);
                SelectedSpecification = Specifications.FirstOrDefault();
            }
        }

        public void Save()
        {
            foreach (var field in Items)
                field.Field.Save<Item>();
            foreach (var awaiter in _awaters)
                foreach (var spec in awaiter.Value)
                    spec.Field.Save<Specification>();
        }
    }
}