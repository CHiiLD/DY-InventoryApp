using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public partial class ClientAdapter
    {
#if false
        public const string HOST = "";
        public const int READ_SERVER_PORT = 4000;
        public const int WRITE_SERVER_PORT = 4001;

        public const int BUFFER_SIZE = 1024 * 4;
        public byte[] _buffer = new byte[BUFFER_SIZE];
        public int _bufIndex;

        AsyncTcpSession _writeSession;
        TcpClient _readSession;

        public void Connect()
        {
            _writeSession = CreateWriteSession(new IPEndPoint(IPAddress.Parse(HOST), READ_SERVER_PORT));
            _readSession = new TcpClient();
        }

        public AsyncTcpSession CreateWriteSession(IPEndPoint localEP)
        {
            var writeSession = new AsyncTcpSession(localEP);
            writeSession.DataReceived += OnDataReceived;
            writeSession.Error += OnErrorWriteSesson;
            return writeSession;
        }

        private void OnErrorWriteSesson(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
        }

        private void OnDataReceived(object sender, DataEventArgs e)
        {

        }
#endif
    }
}