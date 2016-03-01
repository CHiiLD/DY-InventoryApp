using System.Windows.Controls;

namespace R54IN0.WPF
{
    /// <summary>
    /// FieldManagerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FieldManagerControl : UserControl
    {
        public FieldManagerControl()
        {
            DataContext = new FieldManagerViewModel();
            InitializeComponent();
        }
    }
}