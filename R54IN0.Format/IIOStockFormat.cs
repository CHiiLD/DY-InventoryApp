namespace R54IN0
{
    public interface IIOStockFormat
    {
        string CustomerID { get; set; }
        string EmployeeID { get; set; }
        string InventoryID { get; set; }
        string ProjectID { get; set; }
        string SupplierID { get; set; }
        string WarehouseID { get; set; }
    }
}