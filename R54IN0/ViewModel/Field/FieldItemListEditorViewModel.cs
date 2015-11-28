using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class FieldItemListEditorViewModel
    {
        ItemPipe _selectedItem;
        SpecificationPipe _selectedSpecification { get; set; }

        public ObservableCollection<ItemPipe> Items { get; set; }

        public ItemPipe SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                Specifications.Clear();
                _selectedItem = value;
                if(_selectedItem != null)
                {
                    IEnumerable<Specification> specs = _selectedItem.GetSpecifications();
                    foreach (var i in specs)
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

        public FieldItemListEditorViewModel()
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
        }

        public void AddNewItem()
        {
            Items.Add(new ItemPipe(new Item().Save<Item>()));
            SelectedItem = Items.LastOrDefault();
        }

        public void AddNewSpecification()
        {
            if (SelectedItem != null)
            {
                var spec = new Specification() { ItemUUID = SelectedItem.Item.UUID }.Save<Specification>();
                Specifications.Add(new SpecificationPipe(spec));
                SelectedSpecification = Specifications.LastOrDefault();
            }
        }

        public void RemoveSelectedItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Item.IsDeleted = true;
                SelectedItem.Item.Save<Item>();
                Items.Remove(SelectedItem);
                SelectedItem = Items.FirstOrDefault();
            }
        }

        public void RemoveSelectedSpecification()
        {
            if (SelectedSpecification != null)
            {
                SelectedSpecification.Specification.IsDeleted = true;
                SelectedSpecification.Specification.Save<Specification>();
                Specifications.Remove(SelectedSpecification);
                SelectedSpecification = Specifications.FirstOrDefault();
            }
        }
    }
}