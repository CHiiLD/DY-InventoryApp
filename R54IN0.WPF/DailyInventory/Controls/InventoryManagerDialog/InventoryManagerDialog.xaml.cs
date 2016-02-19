using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace R54IN0.WPF
{
    /// <summary>
    /// NewInventoryAddDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InventoryManagerDialog : BaseMetroDialog
    {
        public InventoryManagerDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        public InventoryManagerDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
        }
    }
}