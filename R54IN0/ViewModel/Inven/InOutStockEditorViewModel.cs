using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class InOutStockEditorViewModel : EditorViewModel<InOutStock>
    {
        Account _account;
        Employee _employee;

        public DateTime Date
        {
            get
            {
                return Inventory.Date;
            }
            set
            {
                Inventory.Date = value;
                OnPropertyChanged("Date");
            }
        }

        Account SelectedAccount
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
                Inventory.EnterpriseUUID = _account.UUID;
                OnPropertyChanged("SelectedAccount");
            }
        }

        Employee SelectedEmployee
        {
            get
            {
                return _employee;
            }
            set
            {
                _employee = value;
                Inventory.EmployeeUUID = _employee.UUID;
                OnPropertyChanged("SelectedEmployee");
            }
        }

        IEnumerable<Account> AllAccount
        {
            get
            {
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    return db.LoadAll<Account>().Where(x => !x.IsDeleted);
                }
            }
        }

        IEnumerable<Employee> AllEmployee
        {
            get
            {
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    return db.LoadAll<Employee>().Where(x => !x.IsDeleted);
                }
            }
        }


        public InOutStockEditorViewModel()
            : base()
        {

        }

        public InOutStockEditorViewModel(InOutStock ioStock)
            : base(ioStock)
        {
            _account = ioStock.TraceAccount();
            _employee = ioStock.TraceEmployee();
        }
    }
}
