using System;
using System.Windows;

namespace R54IN0.WPF
{
    /// <summary>
    /// Window3.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window3 : Window
    {
        string[] _items = new
            string[] { "123", "321", "345" };
        string _selected;
        string _text;

        public string[] Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        public string SelectedItem
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                Console.WriteLine("selected " + _selected);
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                Console.WriteLine(_text);
            }
        }

        public Window3()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}