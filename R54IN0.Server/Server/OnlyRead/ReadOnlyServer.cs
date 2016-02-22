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
        private MySqlConnection _mySqlConn;

        public ReadOnlyServer() : base(new DefaultReceiveFilterFactory<ProtocolFormatReceiveFilter, BinaryRequestInfo>())
        {
            _mySqlConn = new MySqlConnection(
                "Host=child_home.gonetis.com;Port=3306;Server=child_home.gonetis.com;Database=test_inventory;Uid=child;Pwd=f54645464");
        }

        public MySqlConnection MySQL
        {
            get
            {
                return _mySqlConn;
            }
        }

        protected override void OnStarted()
        {
            Logger.Debug(Name + " 서버 시작 ...");
            _mySqlConn.Open();
            Logger.Debug(Name + " 데이터베이스 접속 성공");
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            Logger.Debug(Name + " 서버 종료 ...");
            base.OnStopped();
            Logger.Debug(Name + " 데이터베이스 접속 해제");
            _mySqlConn.Close();
        }
    }
}