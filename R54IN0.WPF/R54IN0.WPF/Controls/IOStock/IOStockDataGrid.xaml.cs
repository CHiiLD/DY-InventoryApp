using System.Reflection;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    /// <summary>
    /// InoutStockDataGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IOStockDataGrid : UserControl
    {
        public IOStockDataGrid()
        {
            InitializeComponent();
        }

        //private void DataGrid_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        //{
        //    var datagrid = sender as DataGrid;
        //    if(datagrid != null)
        //    {
        //        IOStockDataGridItem item = datagrid.CurrentItem as IOStockDataGridItem;
        //        DataGridColumn column = datagrid.CurrentColumn;
        //        if (column.SortMemberPath.Contains("Name"))
        //        {
        //            string propertyPath = column.SortMemberPath.Replace(".Name", "");
        //            string[] paths = propertyPath.Split('.');
        //            object property = item;
        //            foreach(var path in paths)
        //            {
        //                property = property.GetType().GetProperty(path).GetValue(property, null);
        //            }
        //            if (property == null)
        //                e.Handled = true;
        //        }
        //    }
        //}
    }
}