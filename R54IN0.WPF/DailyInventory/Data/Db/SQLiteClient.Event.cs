﻿using System;

namespace R54IN0.WPF
{
    /// <summary>
    /// 만약 서버와 통신 중 insert, update, delete의 이벤트를 받은 경우 처리할 이벤트
    /// </summary>
    public partial class SQLiteClient
    {
        public EventHandler<SQLInsertEventArgs> DataInsertEventHandler;
        public EventHandler<SQLUpdateEventArgs> DataUpdateEventHandler;
        public EventHandler<SQLDeleteEventArgs> DataDeleteEventHandler;
    }
}