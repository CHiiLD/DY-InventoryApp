using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class SimpleStringFormat
    {
        public string UUID { get; set; }
        public string Data { get; set; }

        public SimpleStringFormat()
        { }

        public SimpleStringFormat(string uuid, string data)
        {
            UUID = uuid;
            Data = data;
        }
    }
}
