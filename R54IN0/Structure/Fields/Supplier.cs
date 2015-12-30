using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    /// <summary>
    /// 납품처, 공급회사
    /// </summary>
    public class Supplier : IField
    {
        public string ID { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
    }
}