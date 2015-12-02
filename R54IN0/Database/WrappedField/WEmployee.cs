using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class WEmployee : WField<Employee>
    {
        public WEmployee(Employee employee)
            : base(employee)
        {

        }
    }
}