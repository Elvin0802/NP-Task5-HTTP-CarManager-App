using HTTPCarManager.Models;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace HTTPCarManagerClient;

public partial class MainWindow : Window
{
	public IPAddress Ip { get; } = IPAddress.Loopback;
	public int Port { get; } = 27001;
	public TcpClient Client { get; set; }
	public NetworkStream Stream { get; set; }
	public BinaryReader BReader { get; set; }
	public BinaryWriter BWriter { get; set; }
	public HttpCommand? Command { get; set; }
	public string? Responce { get; set; }
	public Car? CurrentCar { get; set; }

	public MainWindow()
	{
		InitializeComponent();
		DataContext = this;

		Client = new();
		Client.Connect(Ip, Port);

		Stream = Client.GetStream();
		BReader = new BinaryReader(Stream);
		BWriter = new BinaryWriter(Stream);

		Command = null;
		Responce = null;
		CurrentCar = null;
	}

	public void GetBtnClick(object sender, RoutedEventArgs e)
	{
		try
		{
			GetProcess(null);
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
			PostProcess(null);
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
			PutProcess(null);
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
			DeleteProcess(null);
		}
		catch (Exception ex)
		{
			ShowMessage(ex);
		}
	}

	public void ShowMessage(Exception ex) =>
		MessageBox.Show($"Error occoured in {ex.Source}", "Server Result : Error", MessageBoxButton.OK, MessageBoxImage.Error);

	public void GetProcess(HttpCommand command)
	{
		Command = new(HttpCommandMethods.GET);

		BWriter.Write(JsonSerializer.Serialize(Command));
		BWriter.Flush();

		Responce = BReader.ReadString();

		var l = JsonSerializer.Deserialize<List<Car>>(Responce);

		CarsLB.Items.Clear();

		foreach (var c in l)
			CarsLB.Items.Add(c);
	}

	public void PostProcess(HttpCommand command)
	{
		var win = new CarDetailWindow();

		win.Main = this;

		CurrentCar = new();

		win.ShowDialog();

		Command = new(HttpCommandMethods.POST, CurrentCar);

		BWriter.Write(JsonSerializer.Serialize(Command));
		BWriter.Flush();

		Responce = BReader.ReadString();

		MessageBox.Show(Responce, "Server Result : POST", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	public void PutProcess(HttpCommand command)
	{
		var win = new CarDetailWindow();

		win.Main = this;

		CurrentCar = CarsLB.SelectedItem as Car;

		win.CarModelTB.Text = CurrentCar.Model;
		win.CarOwnerTB.Text = CurrentCar.Owner;
		win.CarVendorTB.Text = CurrentCar.Vendor;
		win.CarYearTB.Text = CurrentCar.ReleaseYear.ToString();
		
		win.ShowDialog();

		Command = new(HttpCommandMethods.PUT, CurrentCar);

		BWriter.Write(JsonSerializer.Serialize(Command));
		BWriter.Flush();

		Responce = BReader.ReadString();

		MessageBox.Show(Responce, "Server Result : POST", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	public void DeleteProcess(HttpCommand command)
	{
		CurrentCar = CarsLB.SelectedItem as Car;

		Command = new(HttpCommandMethods.DELETE, CurrentCar!);

		BWriter.Write(JsonSerializer.Serialize(Command));
		BWriter.Flush();

		Responce = BReader.ReadString();

		MessageBox.Show(Responce, "Server Result : POST", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private void WinExit(object sender, System.ComponentModel.CancelEventArgs e)
	{
		BReader.Close();
		BWriter.Close();
		Stream.Close();
		Client.Close();
	}
}