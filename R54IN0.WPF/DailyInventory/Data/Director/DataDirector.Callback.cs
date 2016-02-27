using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R54IN0.WPF
{
    public partial class DataDirector
    {
        private void OnDataInserted(object obj, SQLInsertEventArgs e)
        {
            IID format = e.Format;
            log.DebugFormat("{0}(TYPE: {1}, ID: {2})", nameof(OnDataInserted), format.GetType().Name, e.Format.ID);

            switch (format.GetType().Name)
            {
                case nameof(InventoryFormat):
                    InventoryFormat invf = format as InventoryFormat;
                    ObservableInventory oinv = new ObservableInventory(invf);
                    _inventory.Add(oinv);
                    _subject.NotifyNewItemAdded(oinv);
                    break;

                case nameof(IOStockFormat):
                    IOStockFormat stof = format as IOStockFormat;
                    IOStockDataGridItem osto = new IOStockDataGridItem(stof);
                    _subject.NotifyNewItemAdded(osto);
                    break;

                case nameof(Product):
                    Product prod = format as Product;
                    Observable<Product> oprod = new Observable<Product>(prod);
                    _field.Add<Product>(oprod);
                    _subject.NotifyNewItemAdded(oprod);
                    break;

                case nameof(Maker):
                    Maker maker = format as Maker;
                    Observable<Maker> omaker = new Observable<Maker>(maker);
                    _field.Add<Maker>(omaker);
                    _subject.NotifyNewItemAdded(omaker);
                    break;

                case nameof(Measure):
                    Measure meas = format as Measure;
                    Observable<Measure> omeas = new Observable<Measure>(meas);
                    _field.Add<Measure>(omeas);
                    _subject.NotifyNewItemAdded(omeas);
                    break;

                case nameof(Customer):
                    Customer cust = format as Customer;
                    Observable<Customer> ocust = new Observable<Customer>(cust);
                    _field.Add<Customer>(ocust);
                    _subject.NotifyNewItemAdded(ocust);
                    break;

                case nameof(Supplier):
                    Supplier supp = format as Supplier;
                    Observable<Supplier> osupp = new Observable<Supplier>(supp);
                    _field.Add<Supplier>(osupp);
                    _subject.NotifyNewItemAdded(osupp);
                    break;

                case nameof(Project):
                    Project proj = format as Project;
                    Observable<Project> oproj = new Observable<Project>(proj);
                    _field.Add<Project>(oproj);
                    _subject.NotifyNewItemAdded(oproj);
                    break;

                case nameof(Warehouse):
                    Warehouse ware = format as Warehouse;
                    Observable<Warehouse> oware = new Observable<Warehouse>(ware);
                    _field.Add<Warehouse>(oware);
                    _subject.NotifyNewItemAdded(oware);
                    break;

                case nameof(Employee):
                    Employee emp = format as Employee;
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
            IID format = e.Format;
            Type type = format.GetType();
            IUpdateLock target = null;
            log.DebugFormat("{0}(TYPE: {1}, ID: {2})", nameof(OnDataUpdated), type.Name, format.ID);

            switch (type.Name)
            {
                case nameof(InventoryFormat):
                    target = SearchInventory(format.ID); break;
                case nameof(IOStockFormat):
                    target = StockList.Where(x => x.ID == format.ID).SingleOrDefault(); break;
                case nameof(Product):
                    target = SearchField<Product>(format.ID); break;
                case nameof(Maker):
                    target = SearchField<Maker>(format.ID); break;
                case nameof(Measure):
                    target = SearchField<Measure>(format.ID); break;
                case nameof(Customer):
                    target = SearchField<Customer>(format.ID); break;
                case nameof(Supplier):
                    target = SearchField<Supplier>(format.ID); break;
                case nameof(Project):
                    target = SearchField<Project>(format.ID); break;
                case nameof(Warehouse):
                    target = SearchField<Warehouse>(format.ID); break;
                case nameof(Employee):
                    target = SearchField<Employee>(format.ID); break;
                default:
                    throw new NotSupportedException();
            }

            PropertyInfo[] properties = type.GetProperties();
            Type targetType = target.GetType();

            using (UpdateLocker ul = new UpdateLocker(target))
            {
                foreach (PropertyInfo property in properties)
                {
                    string name = property.Name;
                    object value1 = targetType.GetProperty(name).GetValue(target);
                    object value2 = type.GetProperty(name).GetValue(format);

                    if (value1 == null ^ value2 == null)
                    {
                        targetType.GetProperty(name).SetValue(target, value2);
                        log.DebugFormat("update ID: {0}  {1} => {2}", format.ID, value1, value2);
                    }
                    else if (value1 != null && value2 != null && value1.ToString() != value2.ToString())
                    {
                        targetType.GetProperty(name).SetValue(target, value2);
                        log.DebugFormat("update ID: {0}  {1} => {2}", format.ID, value1, value2);
                    }
                }
            }
        }

        private void OnDataDeleted(object obj, SQLDeleteEventArgs e)
        {
            Type type = e.Type;
            string id = e.ID;

            log.DebugFormat("{0}(TYPE: {1}, id: {2})", nameof(OnDataDeleted), e.Type.Name, id);

            switch (type.Name)
            {
                case nameof(InventoryFormat):
                    ObservableInventory inv = SearchInventory(id);
                    if (inv != null)
                    {
                        List<IOStockDataGridItem> stos = StockList.Where(x => x.Inventory.ID == id).ToList();
                        stos.ForEach(x => OnDataDeleted(obj, new SQLDeleteEventArgs(typeof(IOStockFormat), x.ID)));
                        _subject.NotifyItemDeleted(inv);
                        _inventory.Remove(inv.ID);
                    }
                    break;

                case nameof(IOStockFormat):
                    IOStockDataGridItem stock = StockList.Where(x => x.ID == id).SingleOrDefault();
                    if (stock != null)
                        _subject.NotifyItemDeleted(stock);
                    break;

                case nameof(Product):
                    Observable<Product> prod = SearchField<Product>(id);
                    if (prod != null)
                    {
                        var invs = SearchInventories(prod.ID); //inven 삭제
                        invs.ForEach(x => OnDataDeleted(obj, new SQLDeleteEventArgs(typeof(InventoryFormat), x.ID)));
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
                        StockList.ForEach(x => { if (x.CustomerID == id) x.CustomerID = null; });
                        _subject.NotifyItemDeleted(cust);
                        _field.Delete<Customer>(cust.ID);
                    }
                    break;

                case nameof(Supplier):
                    Observable<Supplier> supp = SearchField<Supplier>(id);
                    if (supp != null)
                    {
                        StockList.ForEach(x => { if (x.SupplierID == id) x.SupplierID = null; });
                        _subject.NotifyItemDeleted(supp);
                        _field.Delete<Supplier>(supp.ID);
                    }
                    break;

                case nameof(Project):
                    Observable<Project> proj = SearchField<Project>(id);
                    if (proj != null)
                    {
                        StockList.ForEach(x => { if (x.ProjectID == id) x.ProjectID = null; });
                        _subject.NotifyItemDeleted(proj);
                        _field.Delete<Project>(proj.ID);
                    }
                    break;

                case nameof(Warehouse):
                    Observable<Warehouse> ware = SearchField<Warehouse>(id);
                    if (ware != null)
                    {
                        StockList.ForEach(x => { if (x.WarehouseID == id) x.WarehouseID = null; });
                        _subject.NotifyItemDeleted(ware);
                        _field.Delete<Warehouse>(ware.ID);
                    }
                    break;

                case nameof(Employee):
                    Observable<Employee> emp = SearchField<Employee>(id);
                    if (emp != null)
                    {
                        StockList.ForEach(x => { if (x.EmployeeID == id) x.Employee = null; });
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