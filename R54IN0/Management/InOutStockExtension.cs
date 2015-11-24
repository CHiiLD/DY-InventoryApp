using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.Lib
{
    public static class InOutStockExtension
    {
        public static Seller TraceSeller(this InOutStock iios)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Seller seller = db.LoadByKey<Seller>(iios.EnterpriseUUID);
                return seller;
            }
        }

        public static Employee TraceEmployee(this InOutStock iios)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Employee eeployee = db.LoadByKey<Employee>(iios.EmployeeUUID);
                return eeployee;
            }
        }
    }
}
