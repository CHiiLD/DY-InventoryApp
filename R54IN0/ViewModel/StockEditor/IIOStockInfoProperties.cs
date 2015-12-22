using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IIOStockInfoProperties : IInventoryInfoProperties
    {
        string Remark { get; set; }
        FieldWrapper<Employee> Employee { get; set; }
        StockType StockType { get; set; }
        DateTime Date { get; set; }
        AccountWrapper Account { get; set; }
    }
}