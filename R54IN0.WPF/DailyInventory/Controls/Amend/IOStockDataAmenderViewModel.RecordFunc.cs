using System;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel
    {
        public async Task<IObservableIOStockProperties> RecordAsync()
        {
            if (Product == null && string.IsNullOrEmpty(ProductText))
                throw new Exception("제품의 이름을 입력해주세요.");
            if (Inventory == null && string.IsNullOrEmpty(SpecificationText))
                throw new Exception("규격의 이름을 입력해주세요.");

            IObservableIOStockProperties result = null;

            switch (_mode)
            {
                case Mode.ADD:
                    await CreateIOStockNewProperies();
                    await ApplyModifiedInventoryProperties();
                    await DbAdapter.GetInstance().InsertAsync(Format);
                    result = new ObservableIOStock(Format);
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(result);
                    break;

                case Mode.MODIFY:
                    await ApplyModifiedIOStockProperties();
                    await ApplyModifiedInventoryProperties();
                    await DbAdapter.GetInstance().UpdateAsync(Format);
                    _originSource.Format = Format;
                    result = _originSource;
                    break;
            }
            await RefreshDataGridItems();
            return result;
        }

        /// <summary>
        /// 새로 추가할 텍스트 필드들을 Observable<T>객체로 초기화하여 생성
        /// </sumary>
        private async Task CreateIOStockNewProperies()
        {
            switch (StockType)
            {
                case IOStockType.INCOMING:
                    if (Client == null && !string.IsNullOrEmpty(ClientText))
                    {
                        var supplier = new Observable<Supplier>(ClientText);
                        await InventoryDataCommander.GetInstance().AddObservableField(supplier);
                        Supplier = supplier;
                        Console.WriteLine("recorded Supplier.Name property: {0}", Supplier.Name);
                    }
                    if (Warehouse == null && !string.IsNullOrEmpty(WarehouseText))
                    {
                        warehouse = new Observable<Warehouse>(WarehouseText);
                        await InventoryDataCommander.GetInstance().AddObservableField(warehouse);
                        Warehouse = warehouse;
                    }
                    break;

                case IOStockType.OUTGOING:
                    if (Client == null && !string.IsNullOrEmpty(ClientText))
                    {
                        var customer = new Observable<Customer>(ClientText);
                        await InventoryDataCommander.GetInstance().AddObservableField(customer);
                        Customer = customer;
                    }
                    if (Project == null && !string.IsNullOrEmpty(ProjectText))
                    {
                        var project = new Observable<Project>(ProjectText);
                        await InventoryDataCommander.GetInstance().AddObservableField(project);
                        Project = project;
                    }
                    break;
            }
            if (Employee == null && !string.IsNullOrEmpty(EmployeeText))
            {
                var employee = new Observable<Employee>(EmployeeText);
                await InventoryDataCommander.GetInstance().AddObservableField(employee);
                Employee = employee;
            }
            if (Maker == null && !string.IsNullOrEmpty(MakerText))
            {
                var maker = new Observable<Maker>(MakerText);
                await InventoryDataCommander.GetInstance().AddObservableField(maker);
                Maker = maker;
            }
            if (Measure == null && !string.IsNullOrEmpty(MeasureText))
            {
                var measure = new Observable<Measure>(MeasureText);
                await InventoryDataCommander.GetInstance().AddObservableField(measure);
                Measure = measure;
            }
        }

        /// <summary>
        /// 이름이 변경된 객체를 수정
        /// </summary>
        private async Task ApplyModifiedIOStockProperties()
        {
            switch (StockType)
            {
                case IOStockType.INCOMING:
                    if (_originSource.Supplier != null && Client == null)
                    {
                        _originSource.Supplier.Name = ClientText;
                        Client = _originSource.Supplier;
                    }
                    if (_originSource.Warehouse != null && Warehouse == null)
                    {
                        _originSource.Warehouse.Name = WarehouseText;
                        Warehouse = _originSource.Warehouse;
                    }
                    break;

                case IOStockType.OUTGOING:
                    if (_originSource.Customer != null && Client == null)
                    {
                        _originSource.Customer.Name = ClientText;
                        Client = _originSource.Customer;
                    }
                    if (_originSource.Project != null && Project == null)
                    {
                        _originSource.Project.Name = ProjectText;
                        Project = _originSource.Project;
                    }
                    break;
            }
            if (_originSource.Employee != null && Employee == null)
            {
                _originSource.Employee.Name = EmployeeText;
                Employee = _originSource.Employee;
            }
            if (_originSource.Inventory.Maker != null && Maker == null)
            {
                _originSource.Inventory.Maker.Name = MakerText;
                Maker = _originSource.Inventory.Maker;
            }
            if (_originSource.Inventory.Measure != null && Measure == null)
            {
                _originSource.Inventory.Measure.Name = MeasureText;
                Measure = _originSource.Inventory.Measure;
            }
            await CreateIOStockNewProperies();
        }

        /// <summary>
        /// 수정 또는 새로운 재고 데이터를 생성하여 데이터베이스에 이를 저장한다.
        /// </summary>
        private async Task ApplyModifiedInventoryProperties()
        {
            ObservableInventory inventory = null;
            if (Inventory == null)
            {
                if (Product == null)
                {
                    Observable<Product> product = new Observable<Product>(ProductText);
                    await InventoryDataCommander.GetInstance().AddObservableField(product);
                    Product = product;
                }
                inventory = new ObservableInventory(Product, SpecificationText, InventoryQuantity, SpecificationMemo, Maker, Measure);
                await InventoryDataCommander.GetInstance().AddObservableInventory(inventory);
            }
            else
            {
                inventory = InventoryDataCommander.GetInstance().SearchObservableInventory(Inventory.ID);
                inventory.Format = Inventory.Format;
                inventory.Quantity = InventoryQuantity;
            }
            Inventory = inventory;
        }

        /// <summary>
        /// 입출고 데이터를 새로 추가하는 경우 또는 과거의 데이터를 수정할 경우 입출고 수량에 변화가 있다면
        /// 관련 IOStock 데이터들의 잔여수량 및 재고수량을 다시 계산하여 전부 업데이트하고 Owner의 DataGridItems 역시 변화된 값들을 반영하게 한다.
        /// TODO
        /// </summary>
        private async Task RefreshDataGridItems()
        {
            if (_ioStockStatusViewModel != null && _ioStockStatusViewModel.BackupSource != null)
            {
                var backupSource = _ioStockStatusViewModel.BackupSource;
                foreach (var src in backupSource)
                {
                    if (src.Inventory.ID == Inventory.ID && src.Date > Date)
                        await src.SyncDataFromServer();
                }
            }
        }
    }
}