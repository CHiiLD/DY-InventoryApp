using System;
using System.ComponentModel;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class InoutStockDatePickerViewModel : INotifyPropertyChanged
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

        public InoutStockDatePickerViewModel()
        {
            TodayCommand = new CommandHandler(ExecuteTodayCommand, Can);
            YesterdayCommand = new CommandHandler(ExecuteYesterdayCommand, Can);
            TheDayBeforeYesterday = new CommandHandler(ExecuteTheDayBeforeYesterdayCommand, Can);
            ThisWeekCommand = new CommandHandler(ExecuteThisWeekCommand, Can);
            LastWeekCommand = new CommandHandler(ExecuteLastWeekCommand, Can);
            FromLastWeekToTodayCommand = new CommandHandler(ExecuteFromLastWeekToTodayCommand, Can);
            ThisMonthCommand = new CommandHandler(ExecuteThisMonthCommand, Can);
            LastMonthCommand = new CommandHandler(ExecuteLastMonthCommand, Can);
            FirstQuarterCommand = new CommandHandler(ExecuteFirstQuarterCommand, Can);
            SecondQuarterCommand = new CommandHandler(ExecuteSecondQuarterCommand, Can);
            ThirdQuarterCommand = new CommandHandler(ExecuteThirdQuarterCommand, Can);
            FourthQuarterCommand = new CommandHandler(ExecuteFourthQuarterCommand, Can);
            FirstHalfYearCommand = new CommandHandler(ExecuteFirstHalfYearCommand, Can);
            SecondHalfYearCommand = new CommandHandler(ExecuteSecondHalfYearCommand, Can);
            ThisYearCommand = new CommandHandler(ExecuteThisYearCommand, Can);
            LastYearCommand = new CommandHandler(ExecuteLastYearCommand, Can);
            TheYearBeforeLastCommand = new CommandHandler(ExecuteTheYearBeforeLastCommand, Can);

            //베이스
            ExecuteThisWeekCommand(null);
        }

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

        private void ExecuteTheYearBeforeLastCommand(object obj)
        {
            FromDate = new DateTime(DateTime.Now.Year - 2, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 11, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 11), 23, 59, 59, 999, DateTimeKind.Local); //12.31
        }

        private void ExecuteLastYearCommand(object obj)
        {
            FromDate = new DateTime(DateTime.Now.Year - 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 11, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 11), 23, 59, 59, 999, DateTimeKind.Local); //12.31
        }

        private void ExecuteThisYearCommand(object obj)
        {
            FromDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 11, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 11), 23, 59, 59, 999, DateTimeKind.Local); //12.31
        }

        private void ExecuteSecondHalfYearCommand(object obj)
        {
            FromDate = new DateTime(DateTime.Now.Year, 7, 1, 0, 0, 0, 0, DateTimeKind.Local); //7.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 5, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 5), 23, 59, 59, 999, DateTimeKind.Local); //12.31
        }

        private void ExecuteFirstHalfYearCommand(object obj)
        {
            FromDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 5, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 5), 23, 59, 59, 999, DateTimeKind.Local); //6.30
        }

        private void ExecuteFourthQuarterCommand(object obj)
        {
            FromDate = new DateTime(DateTime.Now.Year, 10, 1, 0, 0, 0, 0, DateTimeKind.Local); //10.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 2, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 2), 23, 59, 59, 999, DateTimeKind.Local); //12.31
        }

        private void ExecuteThirdQuarterCommand(object obj)
        {
            FromDate = new DateTime(DateTime.Now.Year, 7, 1, 0, 0, 0, 0, DateTimeKind.Local); //7.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 2, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 2), 23, 59, 59, 999, DateTimeKind.Local); //10.31
        }

        private void ExecuteSecondQuarterCommand(object obj)
        {
            FromDate = new DateTime(DateTime.Now.Year, 4, 1, 0, 0, 0, 0, DateTimeKind.Local); //4.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 2, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 2), 23, 59, 59, 999, DateTimeKind.Local); //6.30
        }

        private void ExecuteFirstQuarterCommand(object obj)
        {
            FromDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, 0, DateTimeKind.Local); //1.1
            ToDate = new DateTime(FromDate.Year, FromDate.Month + 2, DateTime.DaysInMonth(FromDate.Year, FromDate.Month + 2), 23, 59, 59, 999, DateTimeKind.Local); //3.31
        }

        private void ExecuteLastMonthCommand(object obj)
        {
            var date = DateTime.Now.AddMonths(-1);
            FromDate = new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59, 999, DateTimeKind.Local);
        }

        private void ExecuteThisMonthCommand(object obj)
        {
            var now = DateTime.Now;
            FromDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59, 999, DateTimeKind.Local);
        }

        private void ExecuteFromLastWeekToTodayCommand(object obj)
        {
            var now = DateTime.Now;
            var from = now.AddDays(-(int)now.DayOfWeek - 7);
            FromDate = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        private void ExecuteLastWeekCommand(object obj)
        {
            var now = DateTime.Now;
            var from = now.AddDays(-(int)now.DayOfWeek - 7);
            var to = now.AddDays((int)DayOfWeek.Saturday - (int)now.DayOfWeek - 7);
            FromDate = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        private void ExecuteThisWeekCommand(object obj)
        {
            var now = DateTime.Now;
            var from = now.AddDays(-(int)now.DayOfWeek);
            var to = now.AddDays((int)DayOfWeek.Saturday - (int)now.DayOfWeek);
            FromDate = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        private void ExecuteTheDayBeforeYesterdayCommand(object obj)
        {
            var date = DateTime.Now.AddDays(-2);
            FromDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        private void ExecuteYesterdayCommand(object obj)
        {
            var date = DateTime.Now.AddDays(-1);
            FromDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        private void ExecuteTodayCommand(object obj)
        {
            var now = DateTime.Now;
            FromDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        public bool Can(object parameter)
        {
            return true;
        }
    }
}