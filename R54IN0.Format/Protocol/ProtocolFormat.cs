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
    public class ProtocolFormat : ReceiveName
    {
        public string Name { get; set; }
        public string Table { get; set; }
        public string ID { get; set; }
        public string SQL { get; set; }
        public string Ping { get; set; }
        public object Value { get; set; }
        public List<object> JFormatList { get; set; }

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

        public ProtocolFormat SetFormats(List<object> formats)
        {
            JFormatList = formats;
            return this;
        }

        public List<TableT> ConvertJFormatList<TableT>() where TableT : class, IID, new()
        {
            if (JFormatList == null)
                return null;

            List<TableT> result = new List<TableT>();
            PropertyInfo[] properties = typeof(TableT).GetProperties();
            foreach (JObject jobj in JFormatList)
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

        public ProtocolFormat SetQueryValueResult(object value)
        {
            Value = value;
            return this;
        }

        public static bool IsRequestName(string header)
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

        /// <summary>
        /// ProtocolFormat 프로퍼티를 Json데이터로 변환 후 byte[] 데이터로 변환
        /// </summary>
        /// <param name="receiveName"></param>
        /// <returns></returns>
        public byte[] ToBytes(string receiveName)
        {
            if (!IsRequestName(receiveName))
                throw new Exception();

            Name = receiveName;
            string json = JsonConvert.SerializeObject(this);
            int jsonLen = Encoding.UTF8.GetByteCount(json);
            string jsonLenStr = string.Format("{0:D4}", jsonLen);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] headerBytes = Encoding.UTF8.GetBytes(receiveName + jsonLenStr);
            byte[] protocolData = new byte[jsonBytes.Length + headerBytes.Length];
            Array.Copy(headerBytes, protocolData, headerBytes.Length);
            Array.Copy(jsonBytes, 0, protocolData, headerBytes.Length, jsonBytes.Length);

            return protocolData;
        }

        /// <summary>
        /// 서버에서 클라이언트 측 데이터를 분석
        /// </summary>
        /// <param name="receiveName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static ProtocolFormat ToFormat(string receiveName, byte[] body)
        {
            byte[] jsonBytes = body;
            string name = receiveName;
            if (!IsRequestName(name))
                throw new Exception(string.Format("Name을 알 수 없습니다. {0}", name));

            string json = Encoding.UTF8.GetString(jsonBytes);
            ProtocolFormat format = JsonConvert.DeserializeObject<ProtocolFormat>(json);
            return format;
        }

        /// <summary>
        /// 클라이언트에서 서버측 데이터를 분석
        /// </summary>
        /// <param name="readBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static ProtocolFormat ToFormat(byte[] readBuffer, int offset, int length)
        {
            byte[] nameBytes = new byte[NAME_SIZE];
            byte[] bodyLenBytes = new byte[BODYLEN_SIZE];
            byte[] jsonBytes = new byte[readBuffer.Length - HEADER_SIZE];
            Array.Copy(readBuffer, offset, nameBytes, 0, NAME_SIZE);
            Array.Copy(readBuffer, offset + NAME_SIZE, bodyLenBytes, 0, BODYLEN_SIZE);

            string name = Encoding.UTF8.GetString(nameBytes);
            if (!IsRequestName(name))
                throw new Exception(string.Format("Name을 알 수 없습니다. {0}", name));

            string jsonLens = Encoding.UTF8.GetString(bodyLenBytes);
            int jsonLen = int.Parse(jsonLens);

            if (jsonLen != length - HEADER_SIZE)
                throw new Exception(string.Format("bodyLength와 실제 Json string Length가 불일치합니다."));

            Array.Copy(readBuffer, offset + HEADER_SIZE, jsonBytes, 0, length - HEADER_SIZE);
            string json = Encoding.UTF8.GetString(jsonBytes);

            ProtocolFormat format = JsonConvert.DeserializeObject<ProtocolFormat>(json);
            return format;
        }
    }
}