using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace R54IN0.Server
{
    public class ReadOnlySession : AppSession<ReadOnlySession, BinaryRequestInfo>
    {
        public ReadOnlySession()
        {
            
        }

        protected override void OnSessionStarted()
        {
            Logger.Debug(SessionID + " 세션 시작 ...");
            base.OnSessionStarted();
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
            Logger.Debug(SessionID + " 세션 종료 ...");
        }
    }
}