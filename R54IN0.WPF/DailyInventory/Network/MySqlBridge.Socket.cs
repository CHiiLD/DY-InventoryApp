using Dawn.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R54IN0.Format;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace R54IN0.WPF
{
    public partial class MySqlBridge
    {
        private AsyncTcpSession _session;
        private Socket _socket;
        private readonly BlockingBufferManager _bufferManager = new BlockingBufferManager(ProtocolFormat.BUFFER_SIZE * 10, 100);

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

        /// <summary>
        /// Re/Write서버에 접속합니다.
        /// </summary>
        /// <returns>read session BeginConnect 실행 후 반환값</returns>
        public IAsyncResult Connect()
        {
            string json = System.IO.File.ReadAllText("ipconfig.json");
            IPConfigJsonFormat config = JsonConvert.DeserializeObject<IPConfigJsonFormat>(json);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(config.WriteServerHost), config.WriteServerPort);
            _session = new AsyncTcpSession(iep);
            _session.ReceiveBufferSize = ProtocolFormat.BUFFER_SIZE * 100;
            _session.Connected += ConnectCallback;
            _session.Connect();

            iep = new IPEndPoint(IPAddress.Parse(config.ReadServerHost), config.ReadServerPort);
            _socket = new Socket(iep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            return _socket.BeginConnect(iep, new AsyncCallback(ConnectCallback), _socket);
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
            _socket = null;
            _session = null;
            _bufferManager.Dispose();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndConnect(ar);
            log.Debug("쿼리 서버와의 연결에 성공하였습니다.");
        }

        private void ConnectCallback(object sender, EventArgs e)
        {
            AsyncTcpSession session = sender as AsyncTcpSession;
            session.DataReceived += OnDataReceived;
            log.Debug("갱신 서버와의 연결에 성공하였습니다.");
        }

        private void OnDataReceived(object sender, DataEventArgs e)
        {
            log.DebugFormat("갱신 서버로부터 데이터 수신(BYTE: {0})", e.Length - e.Offset + 1);

            List<ProtocolFormat> fmtList = ProtocolFormat.ToProtocolFormats(e.Data, e.Offset, e.Length);

            if (Application.Current != null)
                fmtList.ForEach(x => Application.Current.Dispatcher.Invoke(new Action<ProtocolFormat>(DataReceiveHandler), x));
            else
                fmtList.ForEach(x => Dispatcher.CurrentDispatcher.Invoke(new Action<ProtocolFormat>(DataReceiveHandler), x));
        }

        private void DataReceiveHandler(ProtocolFormat pfmt)
        {
            log.DebugFormat("갱신서버로부터 데이터를 수신받았습니다.(CMD: {0}, TYPE: {1})", pfmt.Name, pfmt.Table);
            switch (pfmt.Name)
            {
                case ProtocolCommand.INSERT:
                    JObject jobj = pfmt.Value as JObject;
                    DataInsertEventHandler(this, new SQLInsertEventArgs(ToFormat(pfmt.Table, jobj)));
                    break;

                case ProtocolCommand.UPDATE:
                    jobj = pfmt.Value as JObject;
                    DataUpdateEventHandler(this, new SQLUpdateEventArgs(ToFormat(pfmt.Table, jobj)));
                    break;

                case ProtocolCommand.DELETE:
                    DataDeleteEventHandler(this, new SQLDeleteEventArgs(ToType(pfmt.Table), pfmt.ID));
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private async Task<ProtocolFormat> SendAsync(string cmd, ProtocolFormat sfmt)
        {
            ProtocolFormat rfmt = null;
            SocketError error = SocketError.Success;
            ArraySegment<byte> segment = _bufferManager.GetBuffer();
            int position = segment.Offset;
            int offset = position;
            int remainSize = _bufferManager.BufferSize;
            byte[] array = segment.Array;
            int receivedSize = 0;
            try
            {
                byte[] sendbytes = sfmt.ToBytes(cmd);
                IAsyncResult sendBeginAr = _socket.BeginSend(sendbytes, 0, sendbytes.Length, SocketFlags.None, out error, null, _socket);
                await Task.Factory.FromAsync(sendBeginAr, EndSendCallback);
                log.DebugFormat("쿼리 서버로 데이터를 전송합니다.(CMD: {0}, TYPE: {1})", sfmt.Name, sfmt.Table);
                if (error != SocketError.Success)
                    throw new Exception("소켓 에러가 발생하였습니다. " + error.ToString());
                do
                {
                    remainSize -= receivedSize;
                    position += receivedSize;
                    IAsyncResult receiveBeginAr = _socket.BeginReceive(array, position, remainSize, SocketFlags.None, out error, null, _socket);
                    int size = await Task.Factory.FromAsync<int>(receiveBeginAr, EndReceiveCallback);
                    if (error != SocketError.Success)
                        throw new Exception("소켓 에러가 발생하였습니다. " + error.ToString());
                    if (size == 0)
                        throw new Exception("read server에서 연결을 해제하였습니다.");
                    receivedSize += size;
                }
                while (!ProtocolFormat.IsReceivedCompletely(array, offset, receivedSize));
                rfmt = ProtocolFormat.ToProtocolFormat(array, offset, receivedSize);
                log.DebugFormat("쿼리 서버로 데이터를 수신받았습니다.(CMD: {0}, TYPE: {1})", rfmt.Name, rfmt.Table);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _bufferManager.ReleaseBuffer(segment);
            }
            return rfmt;
        }

        private void EndSendCallback(IAsyncResult ar)
        {
            Socket skt = ar.AsyncState as Socket;
            skt.EndSend(ar);
        }

        private int EndReceiveCallback(IAsyncResult ar)
        {
            Socket skt = ar.AsyncState as Socket;
            return skt.EndReceive(ar); //TODO 접속이 안되거나 서버에서 연결을 끊으면 여기서 에러가 남
        }

        private void Send(string cmd, ProtocolFormat sendingFmt)
        {
            log.DebugFormat("갱신 서버로 데이터를 전송합니다.(CMD: {0}, TYPE:{1})", cmd, sendingFmt.Table);
            ArraySegment<byte> segment = sendingFmt.ToArraySegment(cmd);
            _session.Send(segment);
        }

        private Type ToType(string formatName)
        {
            Type type = null;
            switch (formatName)
            {
                case nameof(Maker):
                    type = typeof(Maker);
                    break;

                case nameof(Measure):
                    type = typeof(Measure);
                    break;

                case nameof(Customer):
                    type = typeof(Customer);
                    break;

                case nameof(Supplier):
                    type = typeof(Supplier);
                    break;

                case nameof(Project):
                    type = typeof(Project);
                    break;

                case nameof(Product):
                    type = typeof(Product);
                    break;

                case nameof(Warehouse):
                    type = typeof(Warehouse);
                    break;

                case nameof(Employee):
                    type = typeof(Employee);
                    break;

                case nameof(InventoryFormat):
                    type = typeof(InventoryFormat);
                    break;

                case nameof(IOStockFormat):
                    type = typeof(IOStockFormat);
                    break;

                default:
                    throw new NotSupportedException();
            }
            return type;
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