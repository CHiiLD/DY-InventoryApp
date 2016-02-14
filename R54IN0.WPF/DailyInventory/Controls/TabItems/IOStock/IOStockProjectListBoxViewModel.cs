using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace R54IN0.WPF
{
    public class IOStockProjectListBoxViewModel : ICollectionViewModel<Observable<Project>>, INotifyPropertyChanged, ICollectionViewModelObserver
    {
        private ObservableCollection<Observable<Project>> _items;
        private Observable<Project> _selectedItem;

        private event PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged -= value;
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        public IOStockProjectListBoxViewModel()
        {
            var ofd = DataDirector.GetInstance();
            var list = ofd.CopyFields<Project>();
            Items = new ObservableCollection<Observable<Project>>(list);
            CollectionViewModelObserverSubject.GetInstance().Attach(this);
            ProjectDeletionCommand = new RelayCommand(ExecuteProjectDeletionCommand, IsSelected);
        }

        private bool IsSelected()
        {
            return SelectedItem != null;
        }

        private void ExecuteProjectDeletionCommand()
        {
            if (SelectedItem != null)
            {
                DataDirector.GetInstance().RemoveField(SelectedItem);
                SelectedItem = null;
            }
        }

        ~IOStockProjectListBoxViewModel()
        {
            CollectionViewModelObserverSubject.GetInstance().Detach(this);
        }

        public ObservableCollection<Observable<Project>> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public Observable<Project> SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        public RelayCommand ProjectDeletionCommand { get; set; }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void UpdateNewItem(object item)
        {
            if (item is Observable<Project>)
            {
                var project = item as Observable<Project>;
                if (!Items.Contains(project))
                    Items.Add(project);
            }
        }

        public void UpdateDelItem(object item)
        {
            if (item is Observable<Project>)
            {
                var project = item as Observable<Project>;
                if (Items.Contains(project))
                    Items.Remove(project);
            }
        }
    }
}