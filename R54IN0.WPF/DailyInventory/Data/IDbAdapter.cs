using System.Collections.Generic;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IDbAdapter
    {
        Task<bool> ConnectAsync();
        Task<bool> DeleteAsync<TableT>(TableT item) where TableT : class, IID;
        Task DisconnectAsync();
        Task InsertAsync<TableT>(TableT item) where TableT : class, IID;
        Task<IEnumerable<TableT>> QueryAsync<TableT>(params object[] commandOptions) where TableT : class, IID;
        Task<IEnumerable<TableT>> SelectAllAsync<TableT>() where TableT : class, IID;
        Task<TableT> SelectAsync<TableT>(string id) where TableT : class, IID;
        Task UpdateAsync<TableT>(TableT item, params string[] properties) where TableT : class, IID;
    }
}