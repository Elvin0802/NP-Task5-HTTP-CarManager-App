using HTTPCarManager.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;

namespace HTTPCarManagerClient;

public partial class MainWindow : Window
{
	public Car? CurrentCar { get; set; }
	public HttpClient? Client { get; set; }
	public Uri? LocalUri { get; init; }

	public MainWindow()
	{
		InitializeComponent();
		DataContext = this;

		LocalUri = new Uri(@"http://localhost:27001/");

		CurrentCar = null;
		Client = new HttpClient();
	}

	public void GetBtnClick(object sender, RoutedEventArgs e)
	{
		try
		{
			GetProcessAsync().Wait(5);
		}
		catch (Exception ex)
		{
			ShowMessage(ex);
		}
	}
	public void PostBtnClick(object sender, RoutedEventArgs e)
	{
		try
		{
			PostProcessAsync().Wait(5);
		}
		catch (Exception ex)
		{
			ShowMessage(ex);
		}
	}
	public void PutBtnClick(object sender, RoutedEventArgs e)
	{
		try
		{
			PutProcessAsync().Wait(5);
		}
		catch (Exception ex)
		{
			ShowMessage(ex);
		}
	}
	public void DeleteBtnClick(object sender, RoutedEventArgs e)
	{
		try
		{
			DeleteProcessAsync().Wait(5);
		}
		catch (Exception ex)
		{
			ShowMessage(ex);
		}
	}

	public void ShowMessage(Exception ex) =>
		MessageBox.Show($"Error occoured in {ex.Source}", "Server Result.", MessageBoxButton.OK, MessageBoxImage.Error);

	public async Task GetProcessAsync()
	{
		var message = new HttpRequestMessage
		{
			Method = HttpMethod.Get,
			RequestUri = LocalUri
		};

		message.Headers.Add("Accept", "application/json");

		var response = await Client!.SendAsync(message);

		var json = await response.Content.ReadAsStringAsync();

		var l = JsonSerializer.Deserialize<List<Car>>(json);

		Dispatcher.Invoke(() =>
		{
			CarsLB.Items.Clear();

			foreach (var c in l)
				CarsLB.Items.Add(c);
		});
	}

	public async Task PostProcessAsync()
	{
		CurrentCar = new("", "", 0, "");

		var win = new CarDetailWindow();

		win.Main = this;

		win.ShowDialog();

		var content = JsonContent.Create(CurrentCar);

		var response = await Client!.PostAsync(LocalUri, content);

		var result = await response.Content.ReadAsStringAsync();

		Dispatcher.Invoke(() =>
		{
			MessageBox.Show(result, "Server Result.", MessageBoxButton.OK, MessageBoxImage.Information);
		});
	}

	public async Task PutProcessAsync()
	{
		var win = new CarDetailWindow();

		win.Main = this;

		CurrentCar = CarsLB.SelectedItem as Car;

		win.CarModelTB.Text = CurrentCar.Model;
		win.CarOwnerTB.Text = CurrentCar.Owner;
		win.CarVendorTB.Text = CurrentCar.Vendor;
		win.CarYearTB.Text = CurrentCar.ReleaseYear.ToString();

		win.ShowDialog();

		var content = JsonContent.Create(CurrentCar);

		var response = await Client!.PutAsync(LocalUri, content);

		var result = await response.Content.ReadAsStringAsync();

		Dispatcher.Invoke(() =>
		{
			MessageBox.Show(result, "Server Result.", MessageBoxButton.OK, MessageBoxImage.Information);
		});
	}

	public async Task DeleteProcessAsync()
	{
		CurrentCar = CarsLB.SelectedItem as Car;

		var message = new HttpRequestMessage
		{
			Method = HttpMethod.Delete,
			RequestUri = LocalUri,
			Content = JsonContent.Create(CurrentCar)
		};

		message.Headers.Add("Delete", "application/json");

		var response = await Client!.SendAsync(message);

		var result = await response.Content.ReadAsStringAsync();

		Dispatcher.Invoke(() =>
		{
			MessageBox.Show(result, "Server Result.", MessageBoxButton.OK, MessageBoxImage.Information);
		});
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		App.Current.Shutdown();
	}
}