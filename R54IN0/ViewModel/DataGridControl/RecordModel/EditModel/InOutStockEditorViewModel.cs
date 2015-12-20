﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class InOutStockEditorViewModel : EditorViewModel<InOutStock>
    {
        IFieldWrapper _account;
        IFieldWrapper _employee;

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

        public StockType SelectedType
        {
            get
            {
                return Inventory.StockType;
            }
            set
            {
                Inventory.StockType = value;
                OnPropertyChanged("SelectedType");
            }
        }

        public IFieldWrapper SelectedAccount
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
                Inventory.EnterpriseUUID = _account.Field.UUID;
                OnPropertyChanged("SelectedAccount");
            }
        }

        public IFieldWrapper SelectedEmployee
        {
            get
            {
                return _employee;
            }
            set
            {
                _employee = value;
                Inventory.EmployeeUUID = _employee.Field.UUID;
                OnPropertyChanged("SelectedEmployee");
            }
        }

        public IEnumerable<IFieldWrapper> AllAccount
        {
            get
            {
                return FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Account>();
            }
        }

        public IEnumerable<IFieldWrapper> AllEmployee
        {
            get
            {
                return FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Employee>();
            }
        }

        public IEnumerable<StockType> AllStockType
        {
            get
            {
                return new StockType[] { StockType.IN, StockType.OUT };
            }
        }

        public InOutStockEditorViewModel()
            : base()
        {
            Date = DateTime.Now;
            SelectedType = StockType.IN;
        }

        public InOutStockEditorViewModel(StockType type)
            : this()
        {
            SelectedType = type;
        }

        public InOutStockEditorViewModel(InOutStock ioStock)
            : base(ioStock)
        {
            var fcd = FieldPipeCollectionDirector.GetInstance();
            _account = fcd.LoadPipe<Account>().Where(x => x.Field.UUID == ioStock.EnterpriseUUID).SingleOrDefault();
            _employee = fcd.LoadPipe<Employee>().Where(x => x.Field.UUID == ioStock.EmployeeUUID).SingleOrDefault();
        }
    }
}
