using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public static class InOutStockExtension
    {
        public static Account TraceAccount(this InOutStock iios)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                Account seller = db.LoadByKey<Account>(iios.EnterpriseUUID);
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
