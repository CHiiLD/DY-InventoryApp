using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class TreeViewNodeJsonFormat
    {
        public string ID { get; set; }
        public string Data { get; set; }

        public TreeViewNodeJsonFormat()
        { }

        public TreeViewNodeJsonFormat(string id, string data)
        {
            ID = id;
            Data = data;
        }
    }
}