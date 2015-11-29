using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class AccountFieldEditorViewModel : IFieldEditorViewModel
    {
        public AccountPipe SelectedItem { get; set; }
        public ObservableCollection<AccountPipe> Items { get; set; }

        public AccountFieldEditorViewModel()
        {
            Account[] items = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                items = db.LoadAll<Account>();
            }
            IEnumerable<AccountPipe> fieldPipes = items.Where(x => !x.IsDeleted).Select(x => new AccountPipe(x));
            Items = new ObservableCollection<AccountPipe>(fieldPipes);
            SelectedItem = Items.FirstOrDefault();
        }

        public virtual void AddNewItem()
        {
            Items.Add(new AccountPipe(new Account().Save<Account>()));
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
                field.Field.Save<Account>();
        }
    }
}