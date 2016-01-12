using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace R54IN0.WPF
{
    /// <summary>
    /// MultiSelectTreeView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MultiSelectTreeView : UserControl
    {
        public MultiSelectTreeView()
        {
            InitializeComponent();
            TreeView.MouseRightButtonDown += OnMouseRightButtonDown;
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition((UIElement)sender);
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(TreeView, pt);
            if (hitTestResult == null || hitTestResult.VisualHit == null)
                return;
            FrameworkElement child = hitTestResult.VisualHit as FrameworkElement;
            do
            {
                if (child is TreeViewExItem)
                {
                    TreeViewNode node = child.DataContext as TreeViewNode;
                    MultiSelectTreeViewModelView treeViewViewModel = this.DataContext as MultiSelectTreeViewModelView;
                    if (node != null && treeViewViewModel != null)
                    {
                        treeViewViewModel.SelectedNodes.Clear();
                        treeViewViewModel.SelectedNodes.Add(node);
                    }
                    break;
                }
                else if (child is TreeViewEx)
                {
                    break;
                }
                else
                {
                    child = VisualTreeHelper.GetParent(child) as FrameworkElement;
                }
            } while (child != null);
        }
    }
}