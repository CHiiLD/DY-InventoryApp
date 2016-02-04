using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R54IN0
{
    public class MysqlDbAdapter : IDbAdapter
    {
        private MySqlConnection _conn;

        public async Task<bool> ConnectAsync()
        {
            _conn = new MySqlConnection(@"Data Source=localhost;Database=inventory;User ID=root;Password=213");
            await _conn.OpenAsync();
            return true;
        }

        public async Task DisconnectAsync()
        {
            await _conn.CloseAsync();
        }

        public Task<bool> DeleteAsync<TableT>(TableT item) where TableT : class, IID
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync<TableT>(TableT item) where TableT : class, IID
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TableT>> QueryAsync<TableT>(params object[] commandOptions) where TableT : class, IID
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TableT>> SelectAllAsync<TableT>() where TableT : class, IID
        {
            throw new NotImplementedException();
        }

        public Task<TableT> SelectAsync<TableT>(string id) where TableT : class, IID
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<TableT>(TableT item, params string[] properties) where TableT : class, IID
        {
            throw new NotImplementedException();
        }
    }
}