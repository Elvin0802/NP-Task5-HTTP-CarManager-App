using System.Windows;

namespace HTTPCarManagerClient;

public partial class CarDetailWindow : Window
{
	public MainWindow Main { get; set; }

	public CarDetailWindow()
	{
		InitializeComponent();
		DataContext = this;
	}

	public void SubmitBtnClick(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(CarModelTB.Text) ||
			string.IsNullOrEmpty(CarOwnerTB.Text) ||
			string.IsNullOrEmpty(CarVendorTB.Text) ||
			string.IsNullOrEmpty(CarYearTB.Text))
		{
			MessageBox.Show("All lines must be full, please, fill all lines.");
			return;
		}

		Main.CurrentCar!.Vendor = CarVendorTB.Text;
		Main.CurrentCar.Model = CarModelTB.Text;
		Main.CurrentCar.Owner = CarOwnerTB.Text;

		int.TryParse(CarYearTB.Text, out int y);

		Main.CurrentCar.ReleaseYear = y;

		Close();
	}
}
