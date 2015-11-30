using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace R54IN0
{
    public class AccountFieldEditorViewModel : IFieldEditorViewModel
    {
        public AccountPipe SelectedItem { get; set; }
        public ObservableCollection<AccountPipe> Items { get; set; }

        public CommandHandler AddNewItemCommand { get; set; }
        public CommandHandler RemoveItemCommand { get; set; }

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

            AddNewItemCommand = new CommandHandler(AddNewItem, CanAddNewItem);
            RemoveItemCommand = new CommandHandler(RemoveSelectedItem, CanRemoveSelectedItem);
        }

        public bool CanAddNewItem(object parameter)
        {
            return true;
        }

        public bool CanRemoveSelectedItem(object parameter)
        {
            return SelectedItem != null ? true : false;
        }

        public void AddNewItem(object parameter)
        {
            Items.Add(new AccountPipe(new Account() { Name = "new account" }.Save<Account>()));
            SelectedItem = Items.LastOrDefault();
            RemoveItemCommand.UpdateCanExecute();
        }

        public void RemoveSelectedItem(object parameter)
        {
            SelectedItem.IsDeleted = true;
            Items.Remove(SelectedItem);
            SelectedItem = Items.FirstOrDefault();
            RemoveItemCommand.UpdateCanExecute();
        }
    }
}