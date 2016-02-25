using R54IN0.Format;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Server
{
    public class ProtocolFormatReceiveFilter : FixedHeaderReceiveFilter<BinaryRequestInfo>
    {
        public ProtocolFormatReceiveFilter() : base(ReceiveName.HEADER_SIZE)
        {

        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            int len = 0;
            string lens = null;
            try
            {
                lens = Encoding.UTF8.GetString(header, offset + ReceiveName.NAME_SIZE, ReceiveName.BODYLEN_SIZE);
                len = Convert.ToInt32(lens);
            }
            catch(Exception e)
            {
                
            }
            return len;
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            return new BinaryRequestInfo(Encoding.UTF8.GetString(header.Array, header.Offset, ReceiveName.NAME_SIZE), bodyBuffer.CloneRange(offset, length));
        }
    }
}