using MahApps.Metro.Controls;
using System;
using System.Diagnostics;

namespace R54IN0.WPF
{
    /// <summary>
    /// StartWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            DataContext = MainWindowViewModel.GetInstance();
            InitializeComponent();
        }
    }
}