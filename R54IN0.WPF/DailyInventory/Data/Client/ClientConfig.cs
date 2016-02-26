using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public class ClientConfig
    {
        public string ReadServerHost { get; set; }
        public string WriteServerHost { get; set; }
        public int ReadServerPort { get; set; }
        public int WriteServerPort { get; set; }
    }
}