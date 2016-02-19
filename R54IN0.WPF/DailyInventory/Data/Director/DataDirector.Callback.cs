using System;
using System.Collections.Generic;
using System.Linq;

namespace R54IN0.WPF
{
    public partial class DataDirector
    {
        private void OnDataInserted(object obj, SQLInsertEventArgs e)
        {
            IID iID = e.IID;
            Console.WriteLine("{0}(TYPE: {1}, ID: {2})", nameof(OnDataInserted), typeof(IID).Name, iID.ID);

            switch (iID.GetType().Name)
            {
                case nameof(InventoryFormat):
                    InventoryFormat invf = iID as InventoryFormat;
                    ObservableInventory oinv = new ObservableInventory(invf);
                    _inventory.Add(oinv);
                    _subject.NotifyNewItemAdded(oinv);
                    break;

                case nameof(IOStockFormat):
                    IOStockFormat stof = iID as IOStockFormat; //TODO 여기 부분은 DataGridItem ?
                    IOStockDataGridItem osto = new IOStockDataGridItem(stof);
                    _subject.NotifyNewItemAdded(osto);
                    break;

                case nameof(Product):
                    Product prod = iID as Product;
                    Observable<Product> oprod = new Observable<Product>(prod);
                    _field.Add<Product>(oprod);
                    _subject.NotifyNewItemAdded(oprod);
                    break;

                case nameof(Maker):
                    Maker maker = iID as Maker;
                    Observable<Maker> omaker = new Observable<Maker>(maker);
                    _field.Add<Maker>(omaker);
                    _subject.NotifyNewItemAdded(omaker);
                    break;

                case nameof(Measure):
                    Measure meas = iID as Measure;
                    Observable<Measure> omeas = new Observable<Measure>(meas);
                    _field.Add<Measure>(omeas);
                    _subject.NotifyNewItemAdded(omeas);
                    break;

                case nameof(Customer):
                    Customer cust = iID as Customer;
                    Observable<Customer> ocust = new Observable<Customer>(cust);
                    _field.Add<Customer>(ocust);
                    _subject.NotifyNewItemAdded(ocust);
                    break;

                case nameof(Supplier):
                    Supplier supp = iID as Supplier;
                    Observable<Supplier> osupp = new Observable<Supplier>(supp);
                    _field.Add<Supplier>(osupp);
                    _subject.NotifyNewItemAdded(osupp);
                    break;

                case nameof(Project):
                    Project proj = iID as Project;
                    Observable<Project> oproj = new Observable<Project>(proj);
                    _field.Add<Project>(oproj);
                    _subject.NotifyNewItemAdded(oproj);
                    break;

                case nameof(Warehouse):
                    Warehouse ware = iID as Warehouse;
                    Observable<Warehouse> oware = new Observable<Warehouse>(ware);
                    _field.Add<Warehouse>(oware);
                    _subject.NotifyNewItemAdded(oware);
                    break;

                case nameof(Employee):
                    Employee emp = iID as Employee;
                    Observable<Employee> oemp = new Observable<Employee>(emp);
                    _field.Add<Employee>(oemp);
                    _subject.NotifyNewItemAdded(oemp);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void OnDataUpdated(object obj, SQLUpdateEventArgs e)
        {
            Console.WriteLine("{0}(TYPE: {1}, PROPERTY: {2}, VALUE: {3})", nameof(OnDataUpdated),
                e.Type.Name, e.Content.First().Key, e.Content.First().Value);

            //업데이트 되는 건 Quantity 밖에 없음
            if (e.Type == typeof(InventoryFormat))
            {
                string id = e.ID;
                var qtyPropertyName = e.Content.First().Key;
                var value = e.Content.First().Value;
                ObservableInventory inv = SearchInventory(id);
                if (inv != null)
                {
                    using (UpdateLocker locker = new UpdateLocker(inv))
                        inv.GetType().GetProperty(qtyPropertyName).SetValue(inv, value);
                }
            }
        }

        private void OnDataDeleted(object obj, SQLDeleteEventArgs e)
        {
            Type type = e.Type;
            List<string> idList = e.IDList;

            foreach (string id in idList)
            {
                Console.WriteLine("{0}(TYPE: {1}, id: {2})", nameof(OnDataDeleted), e.Type.Name, id);
                switch (type.Name)
                {
                    case nameof(InventoryFormat):
                        ObservableInventory inv = SearchInventory(id);
                        if (inv != null)
                        {
                            List<IOStockDataGridItem> stos = StockCollection.Where(x => x.Inventory.ID == id).ToList();
                            if (stos.Count() != 0)
                                OnDataDeleted(obj, new SQLDeleteEventArgs(typeof(IOStockFormat), stos.Select(x => x.ID).ToList()));

                            _subject.NotifyItemDeleted(inv);
                            _inventory.Remove(inv.ID);
                        }
                        break;

                    case nameof(IOStockFormat):
                        IOStockDataGridItem stock = StockCollection.Where(x => x.ID == id).SingleOrDefault();
                        if (stock != null)
                            _subject.NotifyItemDeleted(stock);
                        break;

                    case nameof(Product):
                        Observable<Product> prod = SearchField<Product>(id);
                        if (prod != null)
                        {
                            var invs = SearchInventories(prod.ID); //inven 삭제
                            OnDataDeleted(obj, new SQLDeleteEventArgs(typeof(InventoryFormat), invs.Select(x => x.ID).ToList()));

                            _subject.NotifyItemDeleted(prod);
                            _field.Delete<Product>(prod.ID);
                        }
                        break;

                    case nameof(Maker):
                        Observable<Maker> maker = SearchField<Maker>(id);
                        if (maker != null)
                        {
                            CopyInventories().ForEach(x => { if (x.MakerID == id) x.MakerID = null; });
                            _subject.NotifyItemDeleted(maker);
                            _field.Delete<Maker>(maker.ID);
                        }
                        break;

                    case nameof(Measure):
                        Observable<Measure> meas = SearchField<Measure>(id);
                        if (meas != null)
                        {
                            CopyInventories().ForEach(x => { if (x.MeasureID == id) x.MakerID = null; });
                            _subject.NotifyItemDeleted(meas);
                            _field.Delete<Measure>(meas.ID);
                        }
                        break;

                    case nameof(Customer):
                        Observable<Customer> cust = SearchField<Customer>(id);
                        if (cust != null)
                        {
                            StockCollection.ForEach(x => { if (x.CustomerID == id) x.CustomerID = null; });
                            _subject.NotifyItemDeleted(cust);
                            _field.Delete<Customer>(cust.ID);
                        }
                        break;

                    case nameof(Supplier):
                        Observable<Supplier> supp = SearchField<Supplier>(id);
                        if (supp != null)
                        {
                            StockCollection.ForEach(x => { if (x.SupplierID == id) x.SupplierID = null; });
                            _subject.NotifyItemDeleted(supp);
                            _field.Delete<Supplier>(supp.ID);
                        }
                        break;

                    case nameof(Project):
                        Observable<Project> proj = SearchField<Project>(id);
                        if (proj != null)
                        {
                            StockCollection.ForEach(x => { if (x.ProjectID == id) x.ProjectID = null; });
                            _subject.NotifyItemDeleted(proj);
                            _field.Delete<Project>(proj.ID);
                        }
                        break;

                    case nameof(Warehouse):
                        Observable<Warehouse> ware = SearchField<Warehouse>(id);
                        if (ware != null)
                        {
                            StockCollection.ForEach(x => { if (x.WarehouseID == id) x.WarehouseID = null; });
                            _subject.NotifyItemDeleted(ware);
                            _field.Delete<Warehouse>(ware.ID);
                        }
                        break;

                    case nameof(Employee):
                        Observable<Employee> emp = SearchField<Employee>(id);
                        if (emp != null)
                        {
                            StockCollection.ForEach(x => { if (x.EmployeeID == id) x.Employee = null; });
                            _subject.NotifyItemDeleted(emp);
                            _field.Delete<Employee>(emp.ID);
                        }
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}