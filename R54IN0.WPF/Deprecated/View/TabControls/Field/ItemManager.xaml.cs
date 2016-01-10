using System.Windows.Controls;

namespace R54IN0.WPF
{
    /// <summary>
    /// ItemManager.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ItemManager : UserControl
    {
        public ItemManager()
        {
            InitializeComponent();
            Finder.ViewModel = ItemDataGrid.ViewModel.CreateFinderViewModel(Finder.FinderTreeView) as ItemFinderViewModel;
        }
    }
}