using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;

namespace R54IN0
{
    public class InOutStockWrapper : RecordWrapper<InOutStock>
    {
        IFieldWrapper _account;
        IFieldWrapper _eeployee;

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

        public IFieldWrapper Account
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
                Inven.EnterpriseUUID = _account.Field.UUID;
                Inven.Save<InOutStock>();
                OnPropertyChanged("Account");
            }
        }

        public IFieldWrapper Employee
        {
            get
            {
                return _eeployee;
            }
            set
            {
                _eeployee = value;
                Inven.EmployeeUUID = _eeployee.Field.UUID;
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

        public InOutStockWrapper(InOutStock ioStock)
            : base(ioStock)
        {
            _account = FieldPipeCollectionDirector.GetInstance().LoadPipe<Account>().
                Where(x => x.Field.UUID == ioStock.EnterpriseUUID).SingleOrDefault();
            _eeployee = FieldPipeCollectionDirector.GetInstance().LoadPipe<Employee>().
                Where(x => x.Field.UUID == ioStock.EmployeeUUID).SingleOrDefault();
        }
    }
}