using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.Inven
{
    public static class DatabaseDirector
    {
        private static Base _base = new Base();
        private static Stock _stock = new Stock();

        public static Base GetBase()
        {
            return _base;
        }

        public static Stock GetStock()
        {
            return _stock;
        }
    }
}
