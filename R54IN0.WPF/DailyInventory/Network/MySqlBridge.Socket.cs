using Dawn.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R54IN0.Format;
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
    public partial class MySqlBridge
    {
        private AsyncTcpSession _session;
        private BlockingBufferManager _bufManager;
        private SocketAwaitablePool _pool;
        private Socket _socket;

        public Socket Socket
        {
            get
            {
                return _socket;
            }
        }

        public AsyncTcpSession Session
        {
            get
            {
                return _session;
            }
        }

        public void Connect()
        {
            string json = System.IO.File.ReadAllText("ipconfig.json");
            IPConfigJsonFormat config = JsonConvert.DeserializeObject<IPConfigJsonFormat>(json);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(config.WriteServerHost), config.WriteServerPort);
            _session = new AsyncTcpSession(iep);
            _session.Connected += ConnectCallback;
            _session.Connect();

            //TODO Timeout 설정하기 (config에서 로드해서 실제로 await에서 먹히는지 테스트)
            iep = new IPEndPoint(IPAddress.Parse(config.ReadServerHost), config.ReadServerPort);
            _socket = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(iep, new AsyncCallback(ConnectCallback), _socket);
        }

        public async Task ConnectAsync()
        {
            string json = System.IO.File.ReadAllText("ipconfig.json");
            IPConfigJsonFormat config = JsonConvert.DeserializeObject<IPConfigJsonFormat>(json);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(config.WriteServerHost), config.WriteServerPort);
            _session = new AsyncTcpSession(iep);
            _session.Connected += ConnectCallback;
            _session.Connect();

            iep = new IPEndPoint(IPAddress.Parse(config.ReadServerHost), config.ReadServerPort);
            _socket = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await Task.Factory.FromAsync(_socket.BeginConnect(iep, new AsyncCallback(ConnectCallback), _socket), ConnectCallback);
        }

        public void Disconnect()
        {
            if (_session != null && _session.IsConnected)
                _session.Close();
            if (_socket != null && _socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
        }

        public void Dispose()
        {
            Disconnect();

            if (_pool != null)
                _pool.Dispose();
            if (_bufManager != null)
                _bufManager.Dispose();

            _socket = null;
            _pool = null;
            _bufManager = null;
            _session = null;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndConnect(ar);
            _bufManager = new BlockingBufferManager(ProtocolFormat.BUFFER_SIZE, 10000);
            _pool = new SocketAwaitablePool(100);
            log.Debug("ReadOnlyServer connection success");
        }

        private void ConnectCallback(object sender, EventArgs e)
        {
            AsyncTcpSession session = sender as AsyncTcpSession;
            session.DataReceived += OnDataReceived;

            log.Debug("WriteOnlyServer connection success");
        }

        private void OnDataReceived(object sender, DataEventArgs e)
        {
            ProtocolFormat pfmt = ProtocolFormat.ToProtocolFormat(e.Data, e.Offset, e.Length);
            Type type = Type.GetType("R54IN0.Format." + pfmt.Table + ", R54IN0.Format"); //TODO 여기 부분도 체크해야함
            switch (pfmt.Name)
            {
                case Commands.INSERT:
                    JObject jobj = pfmt.Value as JObject;
                    DataInsertEventHandler(this, new SQLInsertEventArgs(ToFormat(pfmt.Table, jobj)));
                    break;
                case Commands.UPDATE:
                    jobj = pfmt.Value as JObject;
                    DataUpdateEventHandler(this, new SQLUpdateEventArgs(ToFormat(pfmt.Table, jobj)));
                    break;
                case Commands.DELETE:
                    DataDeleteEventHandler(this, new SQLDeleteEventArgs(type, pfmt.ID));
                    break;
            }
        }

        private async Task<ProtocolFormat> SendAsync(string receiveName, ProtocolFormat sendingFmt)
        {
            SocketAwaitable at = _pool.Take();
            List<ArraySegment<byte>> segments = new List<ArraySegment<byte>>();
            ArraySegment<byte> segment = sendingFmt.ToArraySegment(receiveName);
            at.Buffer = segment;
            ProtocolFormat resultFmt = null;
            try
            {
                if (SocketError.Success != await _socket.SendAsync(at))
                    throw new Exception("send to readserver fail");
                at.Buffer = _bufManager.GetBuffer();
                int offset = at.Buffer.Offset;
                int count = 0;
                while (SocketError.Success == await _socket.ReceiveAsync(at) && at.Transferred.Count > 0)
                {
                    segments.Add(at.Buffer);
                    count += at.Transferred.Count;
                    if (ProtocolFormat.IsReceivedCompletely(at.Transferred.Array, offset, count))
                        break;
                    at.Buffer = _bufManager.GetBuffer();
                }
                resultFmt = ProtocolFormat.ToProtocolFormat(at.Transferred.Array, offset, count);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                at.Clear();
                _pool.Add(at);
                segments.ForEach(x => _bufManager.ReleaseBuffer(x));
            }
            return resultFmt;
        }

        private void Send(string receiveName, ProtocolFormat sendingFmt)
        {
            ArraySegment<byte> segment = sendingFmt.ToArraySegment(receiveName);
            _session.Send(segment);
        }

        private IID ToFormat(string formatName, JObject jobj)
        {
            IID instance = null;
            switch (formatName)
            {
                case nameof(Maker):
                    instance = jobj.ToObject<Maker>();
                    break;
                case nameof(Measure):
                    instance = jobj.ToObject<Measure>();
                    break;
                case nameof(Customer):
                    instance = jobj.ToObject<Customer>();
                    break;
                case nameof(Supplier):
                    instance = jobj.ToObject<Supplier>();
                    break;
                case nameof(Project):
                    instance = jobj.ToObject<Project>();
                    break;
                case nameof(Product):
                    instance = jobj.ToObject<Product>();
                    break;
                case nameof(Warehouse):
                    instance = jobj.ToObject<Warehouse>();
                    break;
                case nameof(Employee):
                    instance = jobj.ToObject<Employee>();
                    break;
                case nameof(InventoryFormat):
                    instance = jobj.ToObject<InventoryFormat>();
                    break;
                case nameof(IOStockFormat):
                    instance = jobj.ToObject<IOStockFormat>();
                    break;
                default:
                    throw new NotSupportedException();
            }
            return instance;
        }
    }
}