using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;

namespace R54IN0
{
    public class IOStockWrapper : RecordWrapper<InOutStock>
    {
        AccountWrapper _account;
        FieldWrapper<Employee> _eeployee;

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
                Inven.Save<InOutStock>();
                OnPropertyChanged("Date");
            }
        }

        public AccountWrapper Account
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
                Inven.EnterpriseUUID = value.UUID;
                Inven.Save<InOutStock>();
                OnPropertyChanged("Account");
            }
        }

        public FieldWrapper<Employee> Employee
        {
            get
            {
                return _eeployee;
            }
            set
            {
                _eeployee = value;
                Inven.EmployeeUUID = value.UUID;
                Inven.Save<InOutStock>();
                OnPropertyChanged("Employee");
            }
        }

        public StockType StockType
        {
            get
            {
                return Inven.StockType;
            }
            set
            {
                Inven.StockType = value;
                Inven.Save<InOutStock>();
                OnPropertyChanged("StockType");
            }
        }

        public IOStockWrapper(InOutStock ioStock)
            : base(ioStock)
        {
        }

        protected override void LoadProperies(InOutStock ioStock)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            _account = fwd.CreateCollection<Account, AccountWrapper>().Where(x => x.UUID == ioStock.EnterpriseUUID).SingleOrDefault();
            _eeployee = fwd.CreateCollection<Employee, FieldWrapper<Employee>>().Where(x => x.UUID == ioStock.EmployeeUUID).SingleOrDefault();
            base.LoadProperies(ioStock);
        }
    }
}