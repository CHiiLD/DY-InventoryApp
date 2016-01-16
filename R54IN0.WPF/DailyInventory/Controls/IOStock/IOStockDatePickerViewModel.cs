using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class IOStockDatePickerViewModel : INotifyPropertyChanged
    {
        private DateTime _fromDate;

        private DateTime _toDate;

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

        public EventHandler<EventArgs> CommandExecuted
        {
            get;
            set;
        }

        public IOStockDatePickerViewModel()
        {
            SearchCommand = new RelayCommand(ExecuteSearchCommand, Can);
            TodayCommand = new RelayCommand(ExecuteTodayCommand, Can);
            YesterdayCommand = new RelayCommand(ExecuteYesterdayCommand, Can);
            TheDayBeforeYesterday = new RelayCommand(ExecuteTheDayBeforeYesterdayCommand, Can);
            ThisWeekCommand = new RelayCommand(ExecuteThisWeekCommand, Can);
            LastWeekCommand = new RelayCommand(ExecuteLastWeekCommand, Can);
            FromLastWeekToTodayCommand = new RelayCommand(ExecuteFromLastWeekToTodayCommand, Can);
            ThisMonthCommand = new RelayCommand(ExecuteThisMonthCommand, Can);
            LastMonthCommand = new RelayCommand(ExecuteLastMonthCommand, Can);
            FirstQuarterCommand = new RelayCommand(ExecuteFirstQuarterCommand, Can);
            SecondQuarterCommand = new RelayCommand(ExecuteSecondQuarterCommand, Can);
            ThirdQuarterCommand = new RelayCommand(ExecuteThirdQuarterCommand, Can);
            FourthQuarterCommand = new RelayCommand(ExecuteFourthQuarterCommand, Can);
            FirstHalfYearCommand = new RelayCommand(ExecuteFirstHalfYearCommand, Can);
            SecondHalfYearCommand = new RelayCommand(ExecuteSecondHalfYearCommand, Can);
            ThisYearCommand = new RelayCommand(ExecuteThisYearCommand, Can);
            LastYearCommand = new RelayCommand(ExecuteLastYearCommand, Can);
            TheYearBeforeLastCommand = new RelayCommand(ExecuteTheYearBeforeLastCommand, Can);

            //베이스
            ExecuteThisWeekCommand();
        }

        public ICommand SearchCommand { get; set; }

        public DateTime FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = value;
                NotifyPropertyChanged("FromDate");
            }
        }

        public DateTime ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
                NotifyPropertyChanged("ToDate");
            }
        }

        public ICommand TodayCommand { get; set; }

        public ICommand YesterdayCommand { get; set; }

        public ICommand TheDayBeforeYesterday { get; set; }

        public ICommand ThisWeekCommand { get; set; }

        public ICommand LastWeekCommand { get; set; }

        public ICommand FromLastWeekToTodayCommand { get; set; }

        public ICommand ThisMonthCommand { get; set; }

        public ICommand LastMonthCommand { get; set; }

        public ICommand FirstQuarterCommand { get; set; }

        public ICommand SecondQuarterCommand { get; set; }

        public ICommand ThirdQuarterCommand { get; set; }

        public ICommand FourthQuarterCommand { get; set; }

        public ICommand FirstHalfYearCommand { get; set; }

        public ICommand SecondHalfYearCommand { get; set; }

        public ICommand ThisYearCommand { get; set; }

        public ICommand LastYearCommand { get; set; }

        public ICommand TheYearBeforeLastCommand { get; set; }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void CallCommandExecutedHandler()
        {
            if (CommandExecuted != null)
                CommandExecuted(this, EventArgs.Empty);
        }

        /// <summary>
        /// 조회 버튼을 누를 경우
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteSearchCommand()
        {
            var now = DateTime.Now;
            FromDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, 23, 59, 59, 999, DateTimeKind.Local);
            CallCommandExecutedHandler();
        }

        private void ExecuteTheYearBeforeLastCommand()
        {
            FromDate = new DateTime(DateTime.Now.Year - 2, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 11, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 11), 23, 59, 59, 999, DateTimeKind.Local); //12.31
            CallCommandExecutedHandler();
        }

        private void ExecuteLastYearCommand()
        {
            FromDate = new DateTime(DateTime.Now.Year - 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 11, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 11), 23, 59, 59, 999, DateTimeKind.Local); //12.31
            CallCommandExecutedHandler();
        }

        private void ExecuteThisYearCommand()
        {
            FromDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 11, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 11), 23, 59, 59, 999, DateTimeKind.Local); //12.31
            CallCommandExecutedHandler();
        }

        private void ExecuteSecondHalfYearCommand()
        {
            FromDate = new DateTime(DateTime.Now.Year, 7, 1, 0, 0, 0, 0, DateTimeKind.Local); //7.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 5, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 5), 23, 59, 59, 999, DateTimeKind.Local); //12.31
            CallCommandExecutedHandler();
        }

        private void ExecuteFirstHalfYearCommand()
        {
            FromDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 5, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 5), 23, 59, 59, 999, DateTimeKind.Local); //6.30
            CallCommandExecutedHandler();
        }

        private void ExecuteFourthQuarterCommand()
        {
            FromDate = new DateTime(DateTime.Now.Year, 10, 1, 0, 0, 0, 0, DateTimeKind.Local); //10.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 2, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 2), 23, 59, 59, 999, DateTimeKind.Local); //12.31
            CallCommandExecutedHandler();
        }

        private void ExecuteThirdQuarterCommand()
        {
            FromDate = new DateTime(DateTime.Now.Year, 7, 1, 0, 0, 0, 0, DateTimeKind.Local); //7.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 2, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 2), 23, 59, 59, 999, DateTimeKind.Local); //10.31
            CallCommandExecutedHandler();
        }

        private void ExecuteSecondQuarterCommand()
        {
            FromDate = new DateTime(DateTime.Now.Year, 4, 1, 0, 0, 0, 0, DateTimeKind.Local); //4.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 2, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 2), 23, 59, 59, 999, DateTimeKind.Local); //6.30
            CallCommandExecutedHandler();
        }

        private void ExecuteFirstQuarterCommand()
        {
            FromDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 2, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 2), 23, 59, 59, 999, DateTimeKind.Local); //3.31
            CallCommandExecutedHandler();
        }

        private void ExecuteLastMonthCommand()
        {
            var date = DateTime.Now.AddMonths(-1);
            FromDate = new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59, 999, DateTimeKind.Local);
            CallCommandExecutedHandler();
        }

        private void ExecuteThisMonthCommand()
        {
            var now = DateTime.Now;
            FromDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59, 999, DateTimeKind.Local);
            CallCommandExecutedHandler();
        }

        private void ExecuteFromLastWeekToTodayCommand()
        {
            var now = DateTime.Now;
            var from = now.AddDays(-(int)now.DayOfWeek - 7);
            FromDate = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
            CallCommandExecutedHandler();
        }

        private void ExecuteLastWeekCommand()
        {
            var now = DateTime.Now;
            var from = now.AddDays(-(int)now.DayOfWeek - 7);
            var to = now.AddDays((int)DayOfWeek.Saturday - (int)now.DayOfWeek - 7);
            FromDate = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59, 999, DateTimeKind.Local);
            CallCommandExecutedHandler();
        }

        private void ExecuteThisWeekCommand()
        {
            var now = DateTime.Now;
            var from = now.AddDays(-(int)now.DayOfWeek);
            var to = now.AddDays((int)DayOfWeek.Saturday - (int)now.DayOfWeek);
            FromDate = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59, 999, DateTimeKind.Local);
            CallCommandExecutedHandler();
        }

        private void ExecuteTheDayBeforeYesterdayCommand()
        {
            var date = DateTime.Now.AddDays(-2);
            FromDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, DateTimeKind.Local);
            CallCommandExecutedHandler();
        }

        private void ExecuteYesterdayCommand()
        {
            var date = DateTime.Now.AddDays(-1);
            FromDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, DateTimeKind.Local);
            CallCommandExecutedHandler();
        }

        private void ExecuteTodayCommand()
        {
            var now = DateTime.Now;
            FromDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
            CallCommandExecutedHandler();
        }

        public bool Can()
        {
            return true;
        }
    }
}