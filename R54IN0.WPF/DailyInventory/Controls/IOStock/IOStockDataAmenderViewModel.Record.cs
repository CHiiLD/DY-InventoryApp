using System;
using System.Collections.Generic;
using System.Linq;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel : ObservableIOStock
    {
        public IObservableIOStockProperties Record()
        {
            if (Product == null && string.IsNullOrEmpty(ProductText))
                throw new Exception("제품의 이름을 입력해주세요.");
            if (Inventory == null && string.IsNullOrEmpty(SpecificationText))
                throw new Exception("규격의 이름을 입력해주세요.");

            CreateObservableFields();
            ApplyModifiedInventoryProperties();
            UpdateModifiedRemainingQuantity();

            IObservableIOStockProperties result = null;
            switch (_mode)
            {
                case Mode.ADD:
                    result = new ObservableIOStock(Format.Save<IOStockFormat>());
                    CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(result);
                    break;

                case Mode.MODIFY:
                    result = _originObservableIOStock;
                    result.Format = Format;
                    break;
            }
            result.Format.Save<IOStockFormat>();
            return result;
        }

        /// <summary>
        /// 수정 또는 새로운 재고 데이터를 생성하여 데이터베이스에 이를 저장한다.
        /// </summary>
        private void ApplyModifiedInventoryProperties()
        {
            var ofd = ObservableFieldDirector.GetInstance();
            var oid = ObservableInventoryDirector.GetInstance();
            if (Inventory == null)
            {
                ObservableInventory obInventory = new ObservableInventory(new InventoryFormat().Save<InventoryFormat>());
                if (Product != null)
                {
                    obInventory.Product = Product;
                }
                else
                {
                    Observable<Product> newProduct = new Observable<Product>() { Name = ProductText };
                    ObservableFieldDirector.GetInstance().Add<Product>(newProduct);
                    TreeViewNodeDirector.GetInstance().Add(new TreeViewNode(NodeType.PRODUCT, newProduct.ID));
                    obInventory.Product = newProduct;
                }
                obInventory.Specification = SpecificationText;
                obInventory.Memo = _specificationMemo;
                if (!string.IsNullOrEmpty(MakerText) && Maker == null)
                {
                    obInventory.Maker = new Observable<Maker>() { Name = MakerText };
                    ofd.Add<Maker>(obInventory.Maker);
                }
                else if (Maker != null)
                {
                    obInventory.Maker = Maker;
                }

                if (!string.IsNullOrEmpty(MeasureText) && Measure == null)
                {
                    obInventory.Measure = new Observable<Measure>() { Name = MeasureText };
                    ofd.Add<Measure>(obInventory.Measure);
                }
                else if (Measure != null)
                {
                    obInventory.Measure = Measure;
                }
                obInventory.Quantity = InventoryQuantity;
                oid.Add(obInventory as ObservableInventory);
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(obInventory);
                Inventory = obInventory;
            }
            else
            {
                ObservableInventory originInventory = oid.Search(Inventory.ID);
                if (!string.IsNullOrEmpty(MakerText) && Maker == null)
                {
                    originInventory.Maker = new Observable<Maker>() { Name = MakerText };
                    ofd.Add<Maker>(originInventory.Maker);
                }
                else if (originInventory.Maker != Maker)
                {
                    originInventory.Maker = Maker;
                }
                if (!string.IsNullOrEmpty(MeasureText) && Measure == null)
                {
                    originInventory.Measure = new Observable<Measure>() { Name = MeasureText };
                    ofd.Add<Measure>(originInventory.Measure);
                }
                else if (originInventory.Measure != Measure)
                {
                    originInventory.Measure = Measure;
                }
                if (originInventory.Memo != SpecificationMemo)
                    originInventory.Memo = SpecificationMemo;
                originInventory.Quantity = InventoryQuantity;
                Inventory = originInventory;
            }
        }

        /// <summary>
        /// 새로 추가할 텍스트 필드들을 Observable<T>객체로 초기화하여 생성
        /// </summary>
        private void CreateObservableFields()
        {
            var ofd = ObservableFieldDirector.GetInstance();
            if (Client == null && !string.IsNullOrEmpty(ClientText)) //거래처
            {
                switch (StockType)
                {
                    case IOStockType.INCOMING:
                        Supplier = new Observable<Supplier>() { Name = ClientText };
                        ofd.Add<Supplier>(Supplier);
                        break;

                    case IOStockType.OUTGOING:
                        Customer = new Observable<Customer>() { Name = ClientText };
                        ofd.Add<Customer>(Customer);
                        break;
                }
            }
            if (Warehouse == null && !string.IsNullOrEmpty(WarehouseText))
            {
                Warehouse = new Observable<Warehouse>() { Name = WarehouseText };
                ofd.Add<Warehouse>(Warehouse);
            }
            if (Project == null && !string.IsNullOrEmpty(ProjectText))
            {
                Project = new Observable<Project>() { Name = ProjectText };
                ofd.Add<Project>(Project);
            }
            if (Employee == null && !string.IsNullOrEmpty(EmployeeText))
            {
                Employee = new Observable<Employee>() { Name = EmployeeText };
                ofd.Add<Employee>(Employee);
            }
        }

        /// <summary>
        /// 입출고 데이터를 새로 추가하는 경우 또는 과거의 데이터를 수정할 경우 입출고 수량에 변화가 있다면
        /// 관련 IOStock 데이터들의 잔여수량 및 재고수량을 다시 계산하여 전부 업데이트하고 Owner의 DataGridItems 역시 변화된 값들을 반영하게 한다.
        /// TODO
        /// </summary>
        private void UpdateModifiedRemainingQuantity()
        {
            using (var db = LexDb.GetDbInstance())
            {
                List<IOStockFormat> formats = db.Table<IOStockFormat>().IndexQueryByKey("InventoryID", Inventory.ID).ToList();
                if (formats.Count() == 0)
                    return;
                var orderedFormats = formats.Where(x => x.Date > Date).OrderBy(x => x.Date);
                foreach (var fmt in orderedFormats)
                {
                    int qty = 0;
                    switch (_mode)
                    {
                        case Mode.ADD:
                            if (_nearIOStockFormat != null)
                                qty = StockType == IOStockType.INCOMING ? RemainingQuantity : -RemainingQuantity;
                            else
                                qty = RemainingQuantity;
                            break;

                        case Mode.MODIFY:
                            qty = RemainingQuantity - _originObservableIOStock.RemainingQuantity;
                            break;
                    }
                    if (qty == 0)
                        return;

                    var backupSource = _ioStockStatusViewModel.BackupSource;
                    if (backupSource != null && backupSource.ContainsKey(fmt.ID))
                    {
                        backupSource[fmt.ID].RemainingQuantity += qty;
                    }
                    else
                    {
                        fmt.RemainingQuantity += qty;
                        fmt.Save<IOStockFormat>();
                    }
                }
            }
        }
    }
}