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
        public ProtocolFormatReceiveFilter() : base(8)
        {

        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            byte[] dest = new byte[4];
            Array.Copy(header, offset, dest, 0, 4);

            string lens = Encoding.Default.GetString(dest);
            int len = Convert.ToInt32(lens, 10);
            return len;
        }

        protected override BinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            return new BinaryRequestInfo(Encoding.UTF8.GetString(header.Array, header.Offset, 4), bodyBuffer.CloneRange(offset, length));
        }
    }
}