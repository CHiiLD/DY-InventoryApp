//UNIT TEST
//TODO ReadServer의 스트레스 UnitTest가 필요함 수 많은 데이터를 보낼 경우 부하를 견딜 수 있는지 검사해야함
//TODO IDbAction이 실제로 작동되는지 검사가 필요함(테스트 코드)

//SERVER
//TODO WRITE 서버에서 수정 시 에러에 대비해서 트랜잭션 코드를 만든다. 
//TODO PING PONG을 read/write 서버용으로 command를 하나씩 만들어서 1분씩 서버가 클라이언트를 검사하게 함

//CLIENT
//TODO 서버에 접속할 때 Dialog로 접속 경과를 알려주는 프로그레스 다이얼로그를 만들어야함 실패 시 (닫기, 재접속을 행하는 버튼도 같이) 
//TODO 서버에 접근하는 모든 행위에 대해서 사전에 Connected, Ping 검사를 하고 아닐 경우 접속 불가를 알려주는 다이얼로그를 만들어야함 (닫기, 재접속)
//TODO Paging DataGrid 만들어야함
//TODO IOStockStatus 에서 종류 -> 필터로 변경 
//TODO 검색 시 그룹화 이름을 ""로 변경하기
//TODO 접속 부분 예외처리로 커버링

//STUDY
//TODO Invoke, BeginInvoke 차이점 알기

//ERROR
//TODO IOStock에서 Invoke로 Value 읽어들이는 통신부분에서 에러 발생
//TODO 기본정보관리에서 삭제가 안되는 현상

//private async Task<ProtocolFormat> SendAsync(string cmdName, ProtocolFormat sfmt)
//{
//#if false
//            SocketAwaitable at = _pool.Take();
//            List<ArraySegment<byte>> segments = new List<ArraySegment<byte>>();
//            ArraySegment<byte> segment = sendingFmt.ToArraySegment(receiveName);
//            at.Buffer = segment;
//            ProtocolFormat resultFmt = null;
//            try
//            {
//                if (SocketError.Success != await _socket.SendAsync(at))
//                    throw new Exception("send to readserver fail");
//                at.Buffer = _bufManager.GetBuffer();
//                int offset = at.Buffer.Offset;
//                int count = 0;
//                while (SocketError.Success == await _socket.ReceiveAsync(at) && at.Transferred.Count > 0)
//                {
//                    segments.Add(at.Buffer);
//                    count += at.Transferred.Count;
//                    if (ProtocolFormat.IsReceivedCompletely(at.Transferred.Array, offset, count))
//                        break;
//                    at.Buffer = _bufManager.GetBuffer();
//                }
//                if (count == 0)
//                    throw new Exception("read server에서 연결을 해제하였습니다.");
//                resultFmt = ProtocolFormat.ToProtocolFormat(at.Transferred.Array, offset, count);
//                log.DebugFormat("SEND TO READ SERVER({0})", resultFmt.Name);
//            }
//            catch (Exception e)
//            {
//                throw e;
//            }
//            finally
//            {
//                at.Clear();
//                _pool.Add(at);
//                segments.ForEach(x => _bufManager.ReleaseBuffer(x));
//            }
//            return resultFmt;
//#endif
//    ProtocolFormat rfmt = null;
//    int offset = 0;
//    int receivedSize = 0;
//    SocketError error = SocketError.Success;
//    byte[] sendbytes = sfmt.ToBytes(cmdName);
//    try
//    {
//        IAsyncResult sendBeginAr = _socket.BeginSend(sendbytes, 0, sendbytes.Length, SocketFlags.None, out error, null, _socket);
//        await Task.Factory.FromAsync(sendBeginAr, EndSendCallback);
//        if (error != SocketError.Success)
//            throw new Exception("소켓 에러가 발생하였습니다. " + error.ToString());

//        while (true)
//        {
//            IAsyncResult receiveBeginAr = _socket.BeginReceive(_buffer, offset, _buffer.Length - offset, SocketFlags.None, out error, null, _socket);
//            receivedSize = await Task.Factory.FromAsync<int>(receiveBeginAr, EndReceiveCallback);

//            if (error != SocketError.Success)
//                throw new Exception("소켓 에러가 발생하였습니다. " + error.ToString());
//            if (receivedSize == 0)
//                throw new Exception("read server에서 연결을 해제하였습니다.");

//            if (ProtocolFormat.IsReceivedCompletely(_buffer, 0, offset + receivedSize))
//                break;
//            offset += receivedSize;
//        }
//        rfmt = ProtocolFormat.ToProtocolFormat(_buffer, 0, offset + receivedSize);
//        log.DebugFormat("SEND TO READ SERVER({0})", rfmt.Name);
//    }
//    catch (Exception e)
//    {
//        throw e;
//    }
//    return rfmt;
//}

//private void EndSendCallback(IAsyncResult ar)
//{
//    Socket skt = ar.AsyncState as Socket;
//    skt.EndSend(ar);
//}

//private int EndReceiveCallback(IAsyncResult ar)
//{
//    Socket skt = ar.AsyncState as Socket;
//    return skt.EndReceive(ar);
//}