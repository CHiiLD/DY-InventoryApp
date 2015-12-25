using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lex.Db;
using System.Diagnostics;

namespace R54IN0
{
    public class StockWrapper : ProductWrapper<InOutStock>
    {
        ClientWrapper _client;
        FieldWrapper<Employee> _eeployee;

        public StockWrapper()
           : base()
        {
        }

        public StockWrapper(InOutStock ioStock)
            : base(ioStock)
        {
        }

        public string Code
        {
            get
            {
                return Record.ItemUUID.Substring(0, 6).ToUpper();
            }
        }

        public DateTime Date
        {
            get
            {
                return Record.Date;
            }
            set
            {
                Record.Date = value;
                Record.Save<InOutStock>();
                OnPropertyChanged("Date");
            }
        }

        public ClientWrapper Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                Record.EnterpriseUUID = (_client != null ? _client.UUID : null);
                Record.Save<InOutStock>();
                OnPropertyChanged("Client");
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
                Record.EmployeeUUID = (_eeployee != null ? _eeployee.UUID : null);
                Record.Save<InOutStock>();
                OnPropertyChanged("Employee");
            }
        }

        public StockType StockType
        {
            get
            {
                return Record.StockType;
            }
            set
            {
                Record.StockType = value;
                Record.Save<InOutStock>();
                OnPropertyChanged("StockType");
            }
        }

        public override FieldWrapper<Warehouse> Warehouse
        {
            get
            {
                var iwd = InventoryWrapperDirector.GetInstance();
                var inven = InventoryWrapperDirector.GetInstance().BinSearch(Record.InventoryUUID); //iwd.CreateCollection().Where(x => x.UUID == Record.InventoryUUID).SingleOrDefault();
                return inven != null ? inven.Warehouse : null;
            }
            set
            {
                throw new NotSupportedException();
                //var iwd = InventoryWrapperDirector.GetInstance();
                //var inven = iwd.CreateCollection().Where(x => x.UUID == Record.InventoryUUID).SingleOrDefault();
                //inven.Warehouse  = inven != null ? value : null;
            }
        }

        protected override void LoadProperies(InOutStock ioStock)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            Client = fwd.CreateCollection<Client, ClientWrapper>().Where(x => x.UUID == ioStock.EnterpriseUUID).SingleOrDefault();
            Employee = fwd.CreateCollection<Employee, FieldWrapper<Employee>>().Where(x => x.UUID == ioStock.EmployeeUUID).SingleOrDefault();
            base.LoadProperies(ioStock);
        }
    }
}