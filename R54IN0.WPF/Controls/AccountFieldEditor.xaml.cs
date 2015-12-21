using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace R54IN0.WPF
{
    public partial class AccountFieldEditor : UserControl
    {
        FieldWrapperViewModel<Account, AccountWrapper> _viewModel;

        public AccountFieldEditor()
        {
            InitializeComponent();
            ViewModelObserverSubject subject = ViewModelObserverSubject.GetInstance();
            _viewModel = new FieldWrapperViewModel<Account, AccountWrapper>(subject);
            DataContext = _viewModel;
        }
    }
}