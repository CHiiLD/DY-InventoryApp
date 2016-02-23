﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Dictionary<string, object> KeyValues { get; set; }
        public List<object> Formats { get; set; }

        public ProtocolFormat()
        {

        }

        public ProtocolFormat(Type type)
        {
            Table = type.Name;
        }

        public ProtocolFormat(Type type, Dictionary<string, object> data)
        {
            Table = type.Name;
            KeyValues = data;
        }

        public ProtocolFormat(string type, List<object> formats)
        {
            Table = type;
            Formats = formats;
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

        public static bool IsRequestName(string header)
        {
            bool result = false;
            switch (header)
            {
                case SELECT_ALL:
                case SELECT_ONE:
                case QUERY_FORMAT:
                case QUERY_VALUE:
                case INSERT:
                case DELETE:
                case UPDATE:
                    result = true;
                    break;
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// ProtocolFormat 프로퍼티를 Json데이터로 변환 후 byte[] 데이터로 변환
        /// </summary>
        /// <param name="requestName"></param>
        /// <returns></returns>
        public byte[] ToByteArray(string requestName)
        {
            if (requestName.Length != NAME_SIZE && !IsRequestName(requestName))
                throw new Exception();

            Name = requestName;

            string json = JsonConvert.SerializeObject(this);
            int jsonLen = Encoding.UTF8.GetByteCount(json);
            string lenstr = string.Format("{0:D4}", jsonLen);
            if (lenstr.Length != BODYLEN_SIZE)
                throw new Exception();

            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] headerBytes = Encoding.UTF8.GetBytes(requestName + lenstr);
            byte[] protocolData = new byte[jsonBytes.Length + headerBytes.Length];
            Array.Copy(headerBytes, protocolData, headerBytes.Length);
            Array.Copy(jsonBytes, 0, protocolData, headerBytes.Length, jsonBytes.Length);

            return protocolData;
        }

        /// <summary>
        /// 서버에서 클라이언트 측 데이터를 분석
        /// </summary>
        /// <param name="requestName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static ProtocolFormat ToFormat(string requestName, byte[] body)
        {
            byte[] jsonBytes = body;
            string name = requestName;
            if (!IsRequestName(name))
            {
                throw new Exception();
            }
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
            Array.Copy(readBuffer, offset + NAME_SIZE, nameBytes, 0, BODYLEN_SIZE);

            string name = Encoding.UTF8.GetString(nameBytes);
            if (!IsRequestName(name))
            {
                throw new Exception();
            }

            string jsonLens = Encoding.UTF8.GetString(bodyLenBytes);
            int jsonLen = int.Parse(jsonLens);
            if (jsonLen != length - HEADER_SIZE)
            {
                throw new Exception();
            }

            Array.Copy(readBuffer, offset + HEADER_SIZE, jsonBytes, 0, length - HEADER_SIZE);
            string json = Encoding.UTF8.GetString(jsonBytes);

            ProtocolFormat format = JsonConvert.DeserializeObject<ProtocolFormat>(json);
            return format;
        }
    }
}