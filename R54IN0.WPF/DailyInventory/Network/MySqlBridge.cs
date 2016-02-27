using Dawn.Net.Sockets;
using log4net;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using R54IN0.Format;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public partial class MySqlBridge : IDisposable, IDbAction
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public EventHandler<SQLInsertEventArgs> DataInsertEventHandler { get; set; }
        public EventHandler<SQLUpdateEventArgs> DataUpdateEventHandler { get; set; }
        public EventHandler<SQLDeleteEventArgs> DataDeleteEventHandler { get; set; }

        public MySqlBridge()
        {
            DataInsertEventHandler += OnDataInserted;
            DataUpdateEventHandler += OnDataUpdated;
            DataDeleteEventHandler += OnDataDeleted;
        }

        /// <summary>
        /// reqt: type, sql
        /// recv: JFormatList(value type)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<List<Tuple<T1>>> QueryReturnTupleAsync<T1>(string sql, params object[] args)
        {
            ProtocolFormat pfmt = await SendAsync(ProtocolCommand.QUERY_VALUE, new ProtocolFormat(typeof(T1)).SetSQL(sql));
            IEnumerable<T1> t1s =  pfmt.ValueList.Cast<T1>();
            List<Tuple<T1>> tuples = new List<Tuple<T1>>();
            foreach (T1 t1 in t1s)
                tuples.Add(new Tuple<T1>(t1));
            return tuples;
        }
        
        /// <summary>
        /// reqt: type, instance
        /// recv: type, instance (except me)
        /// </summary>
        /// <typeparam name="TableT"></typeparam>
        /// <param name="item"></param>
        public void Update<TableT>(TableT item) where TableT : class, IID, new()
        {
            Send(ProtocolCommand.UPDATE, new ProtocolFormat(typeof(TableT)).SetInstance(item));
        }
        
        /// <summary>
        /// reqt: type, id
        /// recv: type, id
        /// </summary>
        /// <typeparam name="TableT"></typeparam>
        /// <param name="id"></param>
        public void Delete<TableT>(string id) where TableT : class, IID, new()
        {
            Send(ProtocolCommand.DELETE, new ProtocolFormat(typeof(TableT)).SetID(id));
        }

        public void Insert<TableT>(object item) where TableT : class, IID
        {
            Insert<TableT>(item as TableT);
        }

        /// <summary>
        /// reqt: type, id
        /// recv: type, instance
        /// </summary>
        /// <typeparam name="TableT"></typeparam>
        /// <param name="item"></param>
        public void Insert<TableT>(TableT item) where TableT : IID
        {
            if (item.ID == null)
                item.ID = Guid.NewGuid().ToString();
            Send(ProtocolCommand.INSERT, new ProtocolFormat(typeof(TableT)).SetInstance(item));
        }

        /// <summary>
        /// reqt: type,
        /// recv: type, JObjectList
        /// </summary>
        /// <typeparam name="TableT"></typeparam>
        /// <returns></returns>
        public async Task<List<TableT>> SelectAsync<TableT>() where TableT : class, IID, new()
        {
            ProtocolFormat pfmt = await SendAsync(ProtocolCommand.SELECT_ALL, new ProtocolFormat(typeof(TableT)));
            List<TableT> tables = new List<TableT>();
            foreach (JObject jobject in pfmt.ValueList)
            {
                TableT table = jobject.ToObject<TableT>();
                tables.Add(table);
            }
            return tables;
        }

        /// <summary>
        /// reqt: type, id
        /// recv: type, JObjectList(1)
        /// </summary>
        /// <typeparam name="TableT"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TableT> SelectAsync<TableT>(string id) where TableT : class, IID, new()
        {
            TableT table = null;
            ProtocolFormat pfmt = await SendAsync(ProtocolCommand.SELECT_ONE, new ProtocolFormat(typeof(TableT)).SetID(id));
            object value = pfmt.ValueList.SingleOrDefault();
            if(value != null)
            {
                JObject jobject = value as JObject;
                table = jobject.ToObject<TableT>();
            }
            return table;
        }

        /// <summary>
        /// reqt: type, sql
        /// recv: type, JObjectList
        /// </summary>
        /// <typeparam name="TableT"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<List<TableT>> QueryAsync<TableT>(string sql, params object[] args) where TableT : class, IID, new()
        {
            ProtocolFormat pfmt = await SendAsync(ProtocolCommand.QUERY_FORMAT, new ProtocolFormat(typeof(TableT)).SetSQL(string.Format(sql, args)));
            List<TableT> tables = new List<TableT>();
            foreach (JObject jobject in pfmt.ValueList)
            {
                TableT table = jobject.ToObject<TableT>();
                tables.Add(table);
            }
            return tables;
        }

        private void OnDataDeleted(object sender, SQLDeleteEventArgs e)
        {
        }

        private void OnDataUpdated(object sender, SQLUpdateEventArgs e)
        {
        }

        private void OnDataInserted(object sender, SQLInsertEventArgs e)
        {
        }
    }
}