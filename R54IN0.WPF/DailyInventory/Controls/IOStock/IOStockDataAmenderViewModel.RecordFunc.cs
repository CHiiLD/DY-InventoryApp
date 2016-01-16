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
                    await UpdateModifiedRemainingQuantity();
                    result = new ObservableIOStock(Format);
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(result);
                    break;
                case Mode.MODIFY:
                    ApplyModifiedIOStockProperties();
                    ApplyModifiedInventoryProperties();
                    await DbAdapter.GetInstance().UpdateAsync(Format);
                    await UpdateModifiedRemainingQuantity();
                    result = _originSource;
                    result.Format = Format;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 새로 추가할 텍스트 필드들을 Observable<T>객체로 초기화하여 생성
        /// </summary>
        private void CreateIOStockNewProperies()
        {
            var ofd = ObservableFieldDirector.GetInstance();
            switch (StockType)
            {
                case IOStockType.INCOMING:
                    if (Client == null)
                    {
                        Supplier = new Observable<Supplier>() { Name = ClientText };
                        ofd.Add<Supplier>(Supplier);
                    }
                    if (Warehouse == null)
                    {
                        Warehouse = new Observable<Warehouse>() { Name = WarehouseText };
                        ofd.Add<Warehouse>(Warehouse);
                    }
                    break;
                case IOStockType.OUTGOING:
                    if (Client == null)
                    {
                        Customer = new Observable<Customer>() { Name = ClientText };
                        ofd.Add<Customer>(Customer);
                    }
                    if (Project == null)
                    {
                        Project = new Observable<Project>() { Name = ProjectText };
                        ofd.Add<Project>(Project);
                    }
                    break;
            }
            if (Employee == null)
            {
                Employee = new Observable<Employee>() { Name = EmployeeText };
                ofd.Add<Employee>(Employee);
            }
        }

        /// <summary>
        /// 이름이 변경된 객체를 수정
        /// </summary>
        private void ApplyModifiedIOStockProperties()
        {
            switch (StockType)
            {
                case IOStockType.INCOMING:
                    if (Client == null) //거래처
                    {
                        Client = _originSource.Supplier;
                        Client.Name = ClientText;
                    }
                    if (Warehouse == null)
                    {
                        Warehouse = _originSource.Warehouse;
                        Warehouse.Name = WarehouseText;
                    }
                    break;
                case IOStockType.OUTGOING:
                    if (Client == null) //거래처
                    {
                        Client = _originSource.Customer;
                        Client.Name = ClientText;
                    }
                    if (Project == null)
                    {
                        Project = _originSource.Project;
                        Project.Name = ProjectText;
                    }
                    break;
            }
            if (Employee == null)
            {
                Employee = _originSource.Employee;
                Employee.Name = EmployeeText;
            }
        }

        /// <summary>
        /// 수정 또는 새로운 재고 데이터를 생성하여 데이터베이스에 이를 저장한다.
        /// </summary>
        private void ApplyModifiedInventoryProperties()
        {
            var ofd = ObservableFieldDirector.GetInstance();
            var oid = ObservableInventoryDirector.GetInstance();
            ObservableInventory inventory = null;
            if (Inventory == null)
            {
                inventory = new ObservableInventory(new InventoryFormat())
                {
                    Product = this.Product,
                    Maker = this.Maker,
                    Measure = this.Measure,
                    Specification = SpecificationText,
                    Memo = _specificationMemo
                };
                if (inventory.Product == null)
                {
                    Observable<Product> newProduct = new Observable<Product>() { Name = ProductText };
                    ObservableFieldDirector.GetInstance().Add<Product>(newProduct);
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(newProduct);
                    inventory.Product = newProduct;
                }
                if (inventory.Maker == null)
                {
                    inventory.Maker = new Observable<Maker>() { Name = MakerText };
                    ofd.Add<Maker>(inventory.Maker);
                }
                if (inventory.Measure == null)
                {
                    inventory.Measure = new Observable<Measure>() { Name = MeasureText };
                    ofd.Add<Measure>(inventory.Measure);
                }
                oid.Add(inventory);
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(inventory);
            }
            else
            {
                inventory = oid.Search(Inventory.ID);
                if (Maker == null)
                    inventory.Maker.Name = MakerText;
                else if (inventory.Maker != Maker)
                    inventory.Maker = Maker;
                if (Measure == null)
                    inventory.Measure.Name = MeasureText;
                else if (inventory.Measure != Measure)
                    inventory.Measure = Measure;
                if (inventory.Memo != SpecificationMemo)
                    inventory.Memo = SpecificationMemo;
            }
            inventory.Quantity = InventoryQuantity;
            Inventory = inventory;
        }

        /// <summary>
        /// 입출고 데이터를 새로 추가하는 경우 또는 과거의 데이터를 수정할 경우 입출고 수량에 변화가 있다면
        /// 관련 IOStock 데이터들의 잔여수량 및 재고수량을 다시 계산하여 전부 업데이트하고 Owner의 DataGridItems 역시 변화된 값들을 반영하게 한다.
        /// TODO
        /// </summary>
        private async Task UpdateModifiedRemainingQuantity()
        {
            if (_ioStockStatusViewModel != null && _ioStockStatusViewModel.BackupSource != null)
            {
                SortedDictionary<string, IOStockDataGridItem> backupSource = _ioStockStatusViewModel.BackupSource;
                foreach (var src in backupSource)
                {
                    if (src.Value.Inventory.ID == Inventory.ID && src.Value.Date > Date)
                        await src.Value.LoadFromServer();
                }
            }
        }
    }
}