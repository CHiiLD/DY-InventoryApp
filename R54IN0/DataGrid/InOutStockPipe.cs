using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;

namespace R54IN0
{
    public class InOutStockPipe : InvenPipe<InOutStock>
    {
        Account _account;
        Employee _eeployee;
        public string Code
        {
            get
            {
                return Inven.ItemUUID.Substring(0, 6).ToUpper();
            }
        }

        public DateTime Date
        {
            get
            {
                return Inven.Date;
            }
            set
            {
                Inven.Date = value;
                OnPropertyChanged("Date");
            }
        }

        public Account Account
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
                Inven.EnterpriseUUID = _account.UUID;
                OnPropertyChanged("Account");
            }
        }

        public Employee Employee
        {
            get
            {
                return _eeployee;
            }
            set
            {
                _eeployee = value;
                Inven.EmployeeUUID = _eeployee.UUID;
                OnPropertyChanged("Employee");
            }
        }

        public InOutStockPipe(InOutStock ioStock)
            : base(ioStock)
        {
            using (var db = DatabaseDirector.GetDbInstance())
            {
                _account = db.LoadByKey<Account>(ioStock.EnterpriseUUID);
                _eeployee = db.LoadByKey<Employee>(ioStock.EmployeeUUID);
            }
        }
    }
}