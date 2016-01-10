using System.Windows.Controls;

namespace R54IN0.WPF
{
    /// <summary>
    /// MultiSelectTreeView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MultiSelectTreeView : UserControl
    {
        public ContextMenu CellContextMenu
        {
            get
            {
                return Resources["CellContextMenu"] as ContextMenu;
            }
        }

        public MultiSelectTreeView()
        {
            InitializeComponent();
        }
    }
}