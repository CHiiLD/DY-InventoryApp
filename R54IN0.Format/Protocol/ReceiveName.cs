using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Format
{
    public class ReceiveName
    {
        public const int NAME_SIZE = 4;
        public const int BODYLEN_SIZE = 4;
        public const int HEADER_SIZE = NAME_SIZE + BODYLEN_SIZE;

        public const string SELECT_ALL = "SELA";
        public const string SELECT_ONE = "SELO";
        public const string QUERY_FORMAT = "QUEF";
        public const string QUERY_VALUE = "QUEV";

        public const string INSERT = "INST";
        public const string DELETE = "DELT";
        public const string UPDATE = "UPDT";

        public const string ERROR = "ER00";

        public const string PING = "PING";
        public const string PONG = "PONG";
    }
}