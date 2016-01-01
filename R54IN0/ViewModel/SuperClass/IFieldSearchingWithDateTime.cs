using System;
using System.Collections.Generic;

namespace R54IN0
{
    public interface IFieldSearchingWithDateTime
    {
        string Keyword { get; set; }

        CommandHandler TodayCommand { get; set; }
        CommandHandler YesterdayCommand { get; set; }
        CommandHandler ThisWorkCommand { get; set; }
        CommandHandler ThisMonthCommand { get; set; }
        CommandHandler SearchCommand { get; set; }

        DateTime FromDateTime { get; set; } //시작
        DateTime ToDateTime { get; set; } //끝

        IEnumerable<Type> SearchTypes { get; set; }
        Type SelectedSearchType { get; set; }

        void SearchKeyword<T>();
    }
}