using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public interface IDbAction : IDisposable
    {
        EventHandler<SQLInsertEventArgs> DataInsertEventHandler { get; set; }
        EventHandler<SQLUpdateEventArgs> DataUpdateEventHandler { get; set; }
        EventHandler<SQLDeleteEventArgs> DataDeleteEventHandler { get; set; }

        void Delete<TableT>(string id) where TableT : class, IID, new();
        void Insert<TableT>(TableT item) where TableT : IID;
        void Insert<TableT>(object item) where TableT : class, IID;
        Task<List<TableT>> QueryAsync<TableT>(string sql, params object[] args) where TableT : class, IID, new();
        Task<List<Tuple<T1>>> QueryReturnTupleAsync<T1>(string sql, params object[] args);
        Task<List<TableT>> SelectAsync<TableT>() where TableT : class, IID, new();
        Task<TableT> SelectAsync<TableT>(string id) where TableT : class, IID, new();
        void Update<TableT>(TableT item) where TableT : class, IID, new();
    }
}