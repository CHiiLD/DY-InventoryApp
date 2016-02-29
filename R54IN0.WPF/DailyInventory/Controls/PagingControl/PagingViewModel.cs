using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    public class PagingViewModel : INotifyPropertyChanged
    {
        public const int MAX_PAGE_SIZE = 9;

        private int _curSelectedIndex;
        private int _rowCount;
        private Action<int, int, object> _callback;
        private object _state;

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

        public PagingViewModel()
        {
            FirstMoveCommand = new RelayCommand(ExecuteFirstMoveCommand);
            LastMoveCommand = new RelayCommand(ExecuteLastMoveCommand);
            PreviousCommand = new RelayCommand(ExecutePreviousCommand);
            NextCommand = new RelayCommand(ExecuteNextCommand);

            CurrentPageNumber = -1;
            RowCount = 50;

            NumericButtons = new ObservableCollection<Button>();
            for(int i = 0; i < MAX_PAGE_SIZE; i++)
            {
                Button button = new Button();
                button.Click += OnNumericButtonClicked;
                button.Content = (i + 1).ToString();
                button.Visibility = Visibility.Collapsed;
                NumericButtons.Add(button);
            }
        }

        public int CurrentPageNumber
        {
            get
            {
                return _curSelectedIndex;
            }
            private set
            {
                _curSelectedIndex = value;
                if (value == -1)
                    return;

                int idx = (_curSelectedIndex - 1) % MAX_PAGE_SIZE;
                foreach (Button button in NumericButtons.Where(x => x != NumericButtons[idx]))
                    button.FontWeight = FontWeights.Normal;
                NumericButtons[idx].FontWeight = FontWeights.UltraBold;

                if (_callback != null)
                    _callback(Offset, RowCount, _state);
            }
        }

        public int PageCount
        {
            get
            {
                return (Count / RowCount) + (Count % RowCount == 0 ? 0 : 1);
            }
        }

        public int RowCount
        {
            get
            {
                return _rowCount;
            }
            private set
            {
                if (value > 0)
                    _rowCount = value;
                else
                    throw new ArgumentOutOfRangeException("Row Count는 항상 0보다 커야 합니다.");
            }
        }

        /// <summary>
        /// 1 ~ 9까지의 페이지를 한 챕터로 생각한다.
        /// </summary>
        public int CurrentChapterIndex
        {
            get;
            private set;
        }

        public int ChapterCount
        {
            get
            {
                return PageCount / MAX_PAGE_SIZE + (PageCount % MAX_PAGE_SIZE == 0 ? 0 : 1);
            }
        }

        public int Offset
        {
            get
            {
                return (CurrentPageNumber - 1) * RowCount;
            }
        }

        public int Count
        {
            get;
            private set;
        }

        public RelayCommand FirstMoveCommand
        {
            get;
            private set;
        }

        public RelayCommand LastMoveCommand
        {
            get;
            private set;
        }

        public RelayCommand PreviousCommand
        {
            get;
            private set;
        }

        public RelayCommand NextCommand
        {
            get;
            private set;
        }

        public ObservableCollection<Button> NumericButtons
        {
            get;
            private set;
        }

        public void SetNavigation(int rowCount, int count)
        {
            RowCount = rowCount;
            Count = count;
            CurrentPageNumber = 1;
            CurrentChapterIndex = 0;
            int showingPageCount = PageCount > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : PageCount;
            InitializeNumericButtons();
            for (int i = 0; i < showingPageCount; i++)
            {
                NumericButtons[i].Visibility = Visibility.Visible;
                NumericButtons[i].Content = (i + 1).ToString();
            }
        }

        public void SetNavigation(int rowCount, int count, Action<int, int, object> callback, object state)
        {
            _callback = callback;
            _state = state;
            SetNavigation(rowCount, count);
        }

        public void InitializeNumericButtons()
        {
            int i = 1;
            foreach (Button button in NumericButtons)
            {
                button.Visibility = Visibility.Collapsed;
                button.Content = i.ToString();
                i++;
            }
        }

        private void OnNumericButtonClicked(object obj, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            string content = button.Content as string;
            CurrentPageNumber = int.Parse(content);
        }

        private void ExecuteFirstMoveCommand()
        {
            if (CurrentChapterIndex == 0)
                return;
            InitializeNumericButtons();

            CurrentChapterIndex = 0;
            CurrentPageNumber = -1;
            SetButtonProperty();
        }

        private void ExecutePreviousCommand()
        {
            if (CurrentChapterIndex - 1 < 0)
                return;
            InitializeNumericButtons();

            CurrentChapterIndex -= 1;
            CurrentPageNumber = -1;
            SetButtonProperty();
        }

        private void ExecuteLastMoveCommand()
        {
            if (CurrentChapterIndex == ChapterCount - 1)
                return;
            InitializeNumericButtons();

            CurrentChapterIndex = ChapterCount - 1;
            CurrentPageNumber = -1;
            SetButtonProperty();
        }

        private void ExecuteNextCommand()
        {
            if (CurrentChapterIndex >= ChapterCount - 1)
                return;
            InitializeNumericButtons();

            CurrentChapterIndex += 1;
            CurrentPageNumber = -1;
            SetButtonProperty();
        }

        private void SetButtonProperty()
        {
            int startPageNum = (CurrentChapterIndex * MAX_PAGE_SIZE) + 1;
            int lastPageNum = startPageNum + MAX_PAGE_SIZE > PageCount ? PageCount + 1 : MAX_PAGE_SIZE + startPageNum;
            for (int i = startPageNum; i < lastPageNum; i++)
            {
                int idx = (i - 1) % MAX_PAGE_SIZE;
                NumericButtons[idx].Visibility = Visibility.Visible;
                NumericButtons[idx].Content = i.ToString();
            }
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
