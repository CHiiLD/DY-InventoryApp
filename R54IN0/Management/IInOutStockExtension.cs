using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.Inven
{
    public static class IInOutStockExtension
    {
        public static Seller TraceSeller(this IInOutStock iios)
        {
            using (var db = DatabaseDirector.GetBase().GetDbInstance())
            {
                Seller seller = db.LoadByKey<Seller>(iios.EnterpriseUUID);
                return seller;
            }
        }

        public static Employee TraceEmployee(this IInOutStock iios)
        {
            using (var db = DatabaseDirector.GetBase().GetDbInstance())
            {
                Employee eeployee = db.LoadByKey<Employee>(iios.EmployeeUUID);
                return eeployee;
            }
        }

        public static Warehouse TraceWarehouse(this IInOutStock iios)
        {
            using (var db = DatabaseDirector.GetBase().GetDbInstance())
            {
                Warehouse warehouse = db.LoadByKey<Warehouse>(iios.WarehouseUUID);
                return warehouse;
            }
        }
    }
}
