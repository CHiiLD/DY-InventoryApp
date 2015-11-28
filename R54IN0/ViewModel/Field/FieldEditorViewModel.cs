using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class FieldEditorViewModel<T> : IFieldEditorViewModel where T : class, IField, new()
    {
        public FieldPipe<T> SelectedItem { get; set; }
        public ObservableCollection<FieldPipe<T>> Items { get; set; }

        public FieldEditorViewModel()
        {
            T[] items = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                items = db.LoadAll<T>();
            }
            IEnumerable<FieldPipe<T>> fieldPipes = items.Where(x => !x.IsDeleted).Select(x => new FieldPipe<T>(x));
            Items = new ObservableCollection<FieldPipe<T>>(fieldPipes);
            SelectedItem = Items.FirstOrDefault();
        }

        public virtual void AddNewItem()
        {
            Items.Add(new FieldPipe<T>(new T()));
            SelectedItem = Items.LastOrDefault();
        }

        public virtual void RemoveSelectedItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.IsDeleted = true;
                Items.Remove(SelectedItem);
                SelectedItem = Items.FirstOrDefault();
            }
        }

        public void Save()
        {
            foreach (var field in Items)
                field.Field.Save<T>();
        }
    }
}