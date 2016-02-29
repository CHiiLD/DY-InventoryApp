using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Format
{
    public class ProtocolFormat : ProtocolCommand
    {
        public const int BUFFER_SIZE = 1024 * 2;

        public string Name { get; set; }
        public string Table { get; set; }
        public string ID { get; set; }
        public string SQL { get; set; }
        public string Ping { get; set; }
        public object Value { get; set; }
        public int Offset { get; set; }
        public int RowCount { get; set; }
        public List<object> ValueList { get; set; }

        public ProtocolFormat()
        {

        }

        public ProtocolFormat(Type type)
        {
            Table = type.Name;
        }

        public ProtocolFormat(string type)
        {
            Table = type;
        }

        public ProtocolFormat SetID(string id)
        {
            ID = id;
            return this;
        }

        public ProtocolFormat SetSQL(string sql)
        {
            SQL = sql;
            return this;
        }

        public ProtocolFormat SetPing(string ping)
        {
            Ping = ping;
            return this;
        }

        public ProtocolFormat SetLimitParameter(int offset, int rowCount)
        {
            Offset = offset;
            RowCount = rowCount;
            return this;
        }

        public ProtocolFormat SetValueList(List<object> formats)
        {
            ValueList = formats;
            return this;
        }

        public List<TableT> ConvertToFormat<TableT>() where TableT : class, IID, new()
        {
            if (ValueList == null)
                return null;

            List<TableT> result = new List<TableT>();
            PropertyInfo[] properties = typeof(TableT).GetProperties();
            foreach (JObject jobj in ValueList)
            {
                TableT t = jobj.ToObject<TableT>();
                result.Add(t);
            }
            return result;
        }

        public ProtocolFormat SetInstance(object instance)
        {
            Value = instance;
            return this;
        }

        public static bool IsCommands(string header)
        {
            bool ret = false;
            switch (header)
            {
                case SELECT_ALL:
                case SELECT_ONE:
                case QUERY_FORMAT:
                case QUERY_VALUE:
                case INSERT:
                case DELETE:
                case UPDATE:
                case PING:
                case PONG:
                    ret = true;
                    break;
            }
            return ret;
        }

        public static bool IsReceivedCompletely(byte[] buffer, int offset, int count)
        {
            if (count <= HEADER_SIZE)
                return false;

            string cr = Encoding.UTF8.GetString(buffer, offset + count - END_STRING.Length, END_STRING.Length);
            if (cr != END_STRING)
                return false;

            string command = Encoding.UTF8.GetString(buffer, offset, NAME_SIZE);
            if (!IsCommands(command))
                return false;

            string lenstr = Encoding.UTF8.GetString(buffer, offset + NAME_SIZE, BODYLEN_SIZE);
            int len = Convert.ToInt32(lenstr, 16);

            return len == count - HEADER_SIZE;
        }

        /// <summary>
        /// ProtocolFormat 프로퍼티를 Json데이터로 변환 후 byte[] 데이터로 변환
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public byte[] ToBytes(string command)
        {
            string str = ToString(command);
            byte[] data = Encoding.UTF8.GetBytes(str);
            return data;
        }

        public string ToString(string command)
        {
            if (!IsCommands(command))
                throw new Exception();

            Name = command;
            string json = JsonConvert.SerializeObject(this);
            int jsonLen = Encoding.UTF8.GetByteCount(json) + Encoding.UTF8.GetByteCount(END_STRING);
            string jsonLenStr = string.Format("{0:X4}", jsonLen);

            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(jsonLenStr);
            sb.Append(json);
            sb.Append(END_STRING);
            return sb.ToString();
        }

        public ArraySegment<byte> ToArraySegment(string command)
        {
            byte[] bytes = ToBytes(command);
            return new ArraySegment<byte>(bytes);
        }

        /// <summary>
        /// 서버에서 클라이언트 측 데이터를 분석
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static ProtocolFormat ToProtocolFormat(string command, byte[] body)
        {
            if (!IsCommands(command))
                throw new Exception(string.Format("Header의 Command를 알 수 없습니다.({0})", command));

            string json = Encoding.UTF8.GetString(body);
            ProtocolFormat pfmt = JsonConvert.DeserializeObject<ProtocolFormat>(json);
            return pfmt;
        }

        public static ProtocolFormat ToProtocolFormat(byte[] buffer, int offset, int count)
        {
            if (!IsReceivedCompletely(buffer, offset, count))
                throw new Exception("올바른 형태의 버퍼 데이터가 아닙니다.");

            string json = Encoding.UTF8.GetString(buffer, offset + HEADER_SIZE, count - HEADER_SIZE);
            ProtocolFormat pfmt = JsonConvert.DeserializeObject<ProtocolFormat>(json);
            return pfmt;
        }

        /// <summary>
        /// 클라이언트에서 서버측 데이터를 분석
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<ProtocolFormat> ToProtocolFormats(byte[] buffer, int offset, int count)
        {
            List<ProtocolFormat> fmts = new List<ProtocolFormat>();
            string utf8string = Encoding.UTF8.GetString(buffer, offset, count);
            string[] split = utf8string.Split(new string[] { END_STRING }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in split)
            {
                string name = str.Substring(0, NAME_SIZE);
                if (!IsCommands(name))
                    throw new Exception(string.Format("Header의 Command를 알 수 없습니다.({0})", name));
                string jsonLen = str.Substring(NAME_SIZE, BODYLEN_SIZE);
                string json = str.Substring(HEADER_SIZE, str.Length - HEADER_SIZE);
                ProtocolFormat pfmt = JsonConvert.DeserializeObject<ProtocolFormat>(json);
                fmts.Add(pfmt);
            }
#if false
            byte[] nameBytes = new byte[NAME_SIZE];
            byte[] lenBytes = new byte[BODYLEN_SIZE];
            byte[] jsonBytes = new byte[buffer.Length - HEADER_SIZE];
            Array.Copy(buffer, offset, nameBytes, 0, NAME_SIZE);
            Array.Copy(buffer, offset + NAME_SIZE, lenBytes, 0, BODYLEN_SIZE);

            string receiveName = Encoding.UTF8.GetString(nameBytes);
            if (!IsCommands(receiveName))
                throw new Exception(string.Format("Header의 Command를 알 수 없습니다.({0})", receiveName));

            string jsonLenStr = Encoding.UTF8.GetString(lenBytes);
            int length = Convert.ToInt32(jsonLenStr, 16);

            if (length != count - HEADER_SIZE)
                throw new Exception(string.Format("Header정보의 길이와 본체의 데이터 길이가 같지 않습니다."));
            
            Array.Copy(buffer, offset + HEADER_SIZE, jsonBytes, 0, count - HEADER_SIZE);
            string json = Encoding.UTF8.GetString(jsonBytes);

            ProtocolFormat format = JsonConvert.DeserializeObject<ProtocolFormat>(json);
            return format;
#endif
            return fmts;
        }
    }
}