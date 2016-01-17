using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                    CreateIOStockNewProperies();
                    ApplyModifiedInventoryProperties();
                    await DbAdapter.GetInstance().InsertAsync(Format);
                    await Inventory.SyncDataFromServer();
                    result = new ObservableIOStock(Format);
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(result);
                    break;
                case Mode.MODIFY:
                    ApplyModifiedIOStockProperties();
                    ApplyModifiedInventoryProperties();
                    await DbAdapter.GetInstance().UpdateAsync(Format);
                    await Inventory.SyncDataFromServer();
                    _originSource.Format = Format;
                    result = _originSource;
                    break;
            }
            await RefreshDataGridItems();
            return result;
        }

        /// <summary>
        /// 새로 추가할 텍스트 필드들을 Observable<T>객체로 초기화하여 생성
        /// </summary>
        private void CreateIOStockNewProperies()
        {
            switch (StockType)
            {
                case IOStockType.INCOMING:
                    if (Client == null && !string.IsNullOrEmpty(ClientText))
                        Supplier = new Observable<Supplier>() { Name = ClientText };
                    if (Warehouse == null && !string.IsNullOrEmpty(WarehouseText))
                        Warehouse = new Observable<Warehouse>() { Name = WarehouseText };
                    break;
                case IOStockType.OUTGOING:
                    if (Client == null && !string.IsNullOrEmpty(ClientText))
                        Customer = new Observable<Customer>() { Name = ClientText };
                    if (Project == null && !string.IsNullOrEmpty(ProjectText))
                        Project = new Observable<Project>() { Name = ProjectText };
                    break;
            }
            if (Employee == null && !string.IsNullOrEmpty(EmployeeText))
                Employee = new Observable<Employee>() { Name = EmployeeText };
            if (Maker == null && !string.IsNullOrEmpty(MakerText))
                Maker = new Observable<Maker>() { Name = MakerText };
            if (Measure == null && !string.IsNullOrEmpty(MeasureText))
                Measure = new Observable<Measure>() { Name = MeasureText };
        }

        /// <summary>
        /// 이름이 변경된 객체를 수정
        /// </summary>
        private void ApplyModifiedIOStockProperties()
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
            CreateIOStockNewProperies();
        }

        /// <summary>
        /// 수정 또는 새로운 재고 데이터를 생성하여 데이터베이스에 이를 저장한다.
        /// </summary>
        private void ApplyModifiedInventoryProperties()
        {
            ObservableInventory inventory = null;
            if (Inventory == null)
            {
                Observable<Product> product = Product;
                if (product == null)
                {
                    product = new Observable<Product>() { Name = ProductText };
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(product);
                }
                inventory = new ObservableInventory()
                {
                    Specification = SpecificationText,
                    Memo = SpecificationMemo,
                    Maker = Maker,
                    Measure = Measure,
                    Product = product,
                };
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(inventory);
            }
            else
            {
                inventory = ObservableInventoryDirector.GetInstance().Search(Inventory.ID);
                inventory.Format = Inventory.Format;
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
                SortedDictionary<string, IOStockDataGridItem> backupSource = _ioStockStatusViewModel.BackupSource;
                foreach (var src in backupSource)
                {
                    if (src.Value.Inventory.ID == Inventory.ID && src.Value.Date > Date)
                        await src.Value.SyncDataFromServer();
                }
            }
        }
    }
}