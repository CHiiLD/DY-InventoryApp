using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public class MySqlConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Uid { get; set; }
        public string Pwd { get; set; }

        public static string ConnectionString(string mysqlConfigPath)
        {
            string json = System.IO.File.ReadAllText(mysqlConfigPath);
            MySqlConfig config = JsonConvert.DeserializeObject<MySqlConfig>(json);
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] properties = typeof(MySqlConfig).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                sb.Append(property.Name);
                sb.Append('=');
                object value = property.GetValue(config);
                sb.Append(value);
                sb.Append(';');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}