using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _pvlnsnv { 
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CosmosService _cosmosService;

        public MainWindow()
        {
            InitializeComponent();
            _cosmosService = new CosmosService();

        }

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var customer = new Customer
        //    {
        //        id = Guid.NewGuid().ToString(),
        //        Name = "John"
        //    };

        //    await _cosmosService.AddCustomerAsync(customer);
        //}

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //string customerName = txtCustomerName.Text;

            MessageBox.Show("Test test");

        }
    }
}