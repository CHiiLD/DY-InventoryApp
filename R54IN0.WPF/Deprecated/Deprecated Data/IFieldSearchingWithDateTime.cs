using System;
using System.Collections.Generic;

namespace R54IN0
{
    public interface IFieldSearchingWithDateTime
    {
        string Keyword { get; set; }

        RelayCommand<object> TodayCommand { get; set; }
        RelayCommand<object> YesterdayCommand { get; set; }
        RelayCommand<object> ThisWorkCommand { get; set; }
        RelayCommand<object> ThisMonthCommand { get; set; }
        RelayCommand<object> SearchCommand { get; set; }

        DateTime FromDateTime { get; set; } //시작
        DateTime ToDateTime { get; set; } //끝

        IEnumerable<Type> SearchTypes { get; set; }
        Type SelectedSearchType { get; set; }

        void SearchKeyword<T>();
    }
}