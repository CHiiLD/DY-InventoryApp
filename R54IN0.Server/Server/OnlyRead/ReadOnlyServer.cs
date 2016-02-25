using MySql.Data.MySqlClient;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Server
{
    public class ReadOnlyServer : AppServer<ReadOnlySession, BinaryRequestInfo>
    {
        private MySqlConnection _mysql;

        public ReadOnlyServer() : base(new DefaultReceiveFilterFactory<ProtocolFormatReceiveFilter, BinaryRequestInfo>())
        {
            string connectionStr = MySQLConfig.ConnectionString(@"mysql.json");
            _mysql = new MySqlConnection(connectionStr);
            _mysql.Open();
        }

        public MySqlConnection MySQL
        {
            get
            {
                return _mysql;
            }
        }
    }
}