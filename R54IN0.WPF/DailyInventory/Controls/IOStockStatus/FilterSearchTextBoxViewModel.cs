using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace R54IN0.WPF
{
    public class FilterSearchTextBoxViewModel : InventorySearchTextBoxViewModel, ICollectionViewModel<string>
    {
        public const string FILTER_PRODUCT = "제품";
        public const string FILTER_SPECIFICATION = "규격";
        public const string FILTER_MAKER = "제조사";
        public const string FILTER_SUPPLIER = "구입처";
        public const string FILTER_WAREHOUSE = "보관장소";
        public const string FILTER_CUSTOMER = "출고처";
        public const string FILTER_EMPLOYEE = "담당자";

        public FilterSearchTextBoxViewModel()
        {
            Items = new ObservableCollection<string>()
            {
                FILTER_PRODUCT,
                FILTER_SPECIFICATION,
                FILTER_MAKER,
                FILTER_SUPPLIER,
                FILTER_WAREHOUSE,
                FILTER_CUSTOMER,
                FILTER_EMPLOYEE,
            };
            SelectedItem = Items.First();
        }

        public ObservableCollection<string> Items
        {
            get; set;
        }

        public string SelectedItem
        {
            get; set;
        }

        public string SearchAsFilter()
        {
            string[] keywords = Text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> lowerKeywords = keywords.Select(x => x.ToLower());

            string column = string.Empty;
            StringBuilder sb = new StringBuilder();
            string sql = null;

            if (SelectedItem == FILTER_PRODUCT || SelectedItem == FILTER_SPECIFICATION || SelectedItem == FILTER_MAKER)
            {
                IEnumerable<ObservableInventory> invs = DataDirector.GetInstance().CopyInventories();
                IEnumerable<ObservableInventory> match = null;
                switch (SelectedItem)
                {
                    case FILTER_PRODUCT:
                        match = lowerKeywords.SelectMany(word => invs.Where(inven => inven.Product.Name.ToLower().Contains(word)));
                        break;

                    case FILTER_SPECIFICATION:
                        match = lowerKeywords.SelectMany(word => invs.Where(inven => inven.Specification.ToLower().Contains(word)));
                        break;

                    case FILTER_MAKER:
                        match = lowerKeywords.SelectMany(word => invs.Where(inven => inven.Maker != null && inven.Maker.Name.ToLower().Contains(word)));
                        break;
                }
                if (match.Count() == 0)
                    return null;

                foreach (var inventory in match.Distinct())
                {
                    sb.Append('\'');
                    sb.Append(inventory.ID);
                    sb.Append("', ");
                }
                sb.Remove(sb.Length - 2, 2);
                sql = string.Format("select * from {0} where {1} in ({2});", typeof(IOStockFormat).Name, "InventoryID", sb.ToString());
            }
            else if (SelectedItem == FILTER_SUPPLIER || SelectedItem == FILTER_WAREHOUSE || SelectedItem == FILTER_CUSTOMER || SelectedItem == FILTER_EMPLOYEE)
            {
                IEnumerable<IObservableField> fields = null;
                switch (SelectedItem)
                {
                    case FILTER_SUPPLIER:
                        fields = DataDirector.GetInstance().CopyFields<Supplier>();
                        column = "SupplierID";
                        break;

                    case FILTER_WAREHOUSE:
                        fields = DataDirector.GetInstance().CopyFields<Warehouse>();
                        column = "WarehouseID";
                        break;

                    case FILTER_CUSTOMER:
                        fields = DataDirector.GetInstance().CopyFields<Customer>();
                        column = "CustomerID";
                        break;

                    case FILTER_EMPLOYEE:
                        fields = DataDirector.GetInstance().CopyFields<Employee>();
                        column = "EmployeeID";
                        break;
                }
                IEnumerable<IObservableField> match = lowerKeywords.SelectMany(
                    word => fields.Where(x => x.Name != null && x.Name.ToLower().Contains(word)));
                if (match.Count() == 0)
                    return null;

                foreach (var field in match.Distinct())
                {
                    sb.Append('\'');
                    sb.Append(field.ID);
                    sb.Append("', ");
                }
                sb.Remove(sb.Length - 2, 2);
                sql = string.Format("select * from {0} where {1} in ({2});", typeof(IOStockFormat).Name, column, sb.ToString());
            }
            return sql;
        }
    }
}