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
#if DEBUG
            _mysql = new MySqlConnection("Server=localhost;Database=test_inventory;Uid=child;Pwd=f54645464");
#else
            _mySqlConn = new MySqlConnection("Server=localhost;Database=inventory;Uid=child;Pwd=f54645464");
#endif
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