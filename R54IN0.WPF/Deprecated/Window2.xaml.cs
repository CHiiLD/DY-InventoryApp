using MahApps.Metro.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace R54IN0.WPF
{
    public class Employee_
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Image { get; set; }
    }

    /// <summary>
    /// Window2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window2 : MetroWindow
    {
        public Window2()
        {
            InitializeComponent();

            // Create some test data
            var employees =
                new ObservableCollection<Employee_>
                    {
                    new Employee_ {FirstName = "Mohammed", LastName = "Fadil", Street = "A B C", ZipCode = "123", City = "London", Country = "UK", Image = "/Images/globe.png"},
                    new Employee_ {FirstName = "Siraj", LastName = "Hussam", Street = "A B C", ZipCode = "123", City = "London", Country = "UK", Image = "/Images/globe.png"},
                    new Employee_ {FirstName = "Ayman", LastName = "Tariq", Street = "A B C", ZipCode = "123", City = "London", Country = "UK", Image = "/Images/globe.png"},
                    new Employee_ {FirstName = "Khalid", LastName = "Sheik", Street = "X Y Z", ZipCode = "234", City = "Paris", Country = "France", Image = "/Images/monitor.png"},
                    new Employee_ {FirstName = "Hassan", LastName = "Ali", Street = "Q W E R", ZipCode = "544", City = "NY", Country = "USA", Image = "/Images/star.png"},
                    new Employee_ {FirstName = "Ehsan", LastName = "Mahmoud", Street = "A B C", ZipCode = "123", City = "London", Country = "UK", Image = "/Images/globe.png"},
                    new Employee_ {FirstName = "Idris", LastName = "Sheik", Street = "X Y Z", ZipCode = "234", City = "Paris", Country = "France", Image = "/Images/monitor.png"},
                    new Employee_ {FirstName = "Khalil", LastName = "Ali", Street = "Q W E R", ZipCode = "544", City = "NY", Country = "USA", Image = "/Images/star.png"}
                    };

            ICollectionView employeesView =
                CollectionViewSource.GetDefaultView(employees);

            // Set the grouping by city proprty
            employeesView.GroupDescriptions.Add(new PropertyGroupDescription("City"));

            // Set the view as the DataContext for the DataGrid
            EmployeesDataGrid.DataContext = employeesView;
        }
    }
}