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
    public class ProtocolFormat : Commands
    {
        public const int BUFFER_SIZE = 1024 * 4;

        public string Name { get; set; }
        public string Table { get; set; }
        public string ID { get; set; }
        public string SQL { get; set; }
        public string Ping { get; set; }
        public object Value { get; set; }
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

            byte[] lenBytes = new byte[BODYLEN_SIZE];
            Array.Copy(buffer, offset + NAME_SIZE, lenBytes, 0, BODYLEN_SIZE);
            string jsonLenStr = Encoding.UTF8.GetString(lenBytes);
            int length = Convert.ToInt32(jsonLenStr, 16);

            return length == count - HEADER_SIZE;
        }

        /// <summary>
        /// ProtocolFormat 프로퍼티를 Json데이터로 변환 후 byte[] 데이터로 변환
        /// </summary>
        /// <param name="receiveName"></param>
        /// <returns></returns>
        public byte[] ToBytes(string receiveName)
        {
            if (!IsCommands(receiveName))
                throw new Exception();

            Name = receiveName;
            string json = JsonConvert.SerializeObject(this);
            int jsonLen = Encoding.UTF8.GetByteCount(json);
            string jsonLenStr = string.Format("{0:X8}", jsonLen);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] headerBytes = Encoding.UTF8.GetBytes(receiveName + jsonLenStr);
            byte[] protocolData = new byte[jsonBytes.Length + headerBytes.Length];
            Array.Copy(headerBytes, protocolData, headerBytes.Length);
            Array.Copy(jsonBytes, 0, protocolData, headerBytes.Length, jsonBytes.Length);

            return protocolData;
        }

        public ArraySegment<byte> ToArraySegment(string receiveName)
        {
            byte[] bytes = ToBytes(receiveName);
            return new ArraySegment<byte>(bytes);
        }

        /// <summary>
        /// 서버에서 클라이언트 측 데이터를 분석
        /// </summary>
        /// <param name="receiveName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static ProtocolFormat ToProtocolFormat(string receiveName, byte[] body)
        {
            byte[] jsonBytes = body;
            string name = receiveName;
            if (!IsCommands(name))
                throw new Exception(string.Format("Name을 알 수 없습니다. {0}", name));

            string json = Encoding.UTF8.GetString(jsonBytes);
            ProtocolFormat format = JsonConvert.DeserializeObject<ProtocolFormat>(json);
            return format;
        }

        /// <summary>
        /// 클라이언트에서 서버측 데이터를 분석
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ProtocolFormat ToProtocolFormat(byte[] buffer, int offset, int count)
        {
            byte[] nameBytes = new byte[NAME_SIZE];
            byte[] lenBytes = new byte[BODYLEN_SIZE];
            byte[] jsonBytes = new byte[buffer.Length - HEADER_SIZE];
            Array.Copy(buffer, offset, nameBytes, 0, NAME_SIZE);
            Array.Copy(buffer, offset + NAME_SIZE, lenBytes, 0, BODYLEN_SIZE);

            string receiveName = Encoding.UTF8.GetString(nameBytes);
            if (!IsCommands(receiveName))
                throw new Exception(string.Format("Name을 알 수 없습니다. {0}", receiveName));

            string jsonLenStr = Encoding.UTF8.GetString(lenBytes);
            int length = Convert.ToInt32(jsonLenStr, 16);

            if (length != count - HEADER_SIZE)
                throw new Exception(string.Format("bodyLength와 실제 Json string Length가 불일치합니다."));

            Array.Copy(buffer, offset + HEADER_SIZE, jsonBytes, 0, count - HEADER_SIZE);
            string json = Encoding.UTF8.GetString(jsonBytes);

            ProtocolFormat format = JsonConvert.DeserializeObject<ProtocolFormat>(json);
            return format;
        }
    }
}