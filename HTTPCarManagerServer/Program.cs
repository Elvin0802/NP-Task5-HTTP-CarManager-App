using HTTPCarManager.Models;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

List<Car> Cars = [
new Car("320","BMW",2019,"Germany"),
new Car("430","BMW",2021,"Italy"),
new Car("540","BMW",2017,"France"),
new Car("750","BMW",2023,"Spain")];

var ip = IPAddress.Loopback;
var port = 27001;
var listener = new TcpListener(ip, port);
listener.Start();

Console.WriteLine("Server is listening...");

while (true)
{
	var client = listener.AcceptTcpClient();
	var stream = client.GetStream();
	var br = new BinaryReader(stream);
	var bw = new BinaryWriter(stream);

	try
	{
		while (true)
		{
			var input = br.ReadString();
			var command = JsonSerializer.Deserialize<HttpCommand>(input);
			string result = "";

			Console.WriteLine(command.Method);

			switch (command.Method)
			{
				case HttpCommandMethods.GET:
					result = JsonSerializer.Serialize(Cars);
					break;
				case HttpCommandMethods.POST:
					try {
						Cars.Add(command.Value);
						result = "Car adding sucess.";
					}
					catch (Exception) {
						result = "Car adding failed.";
					}
					break;
				case HttpCommandMethods.PUT:
					try
					{
						var c = Cars.FirstOrDefault(c => c.Id == command.Value.Id);

						c.Vendor = command.Value.Vendor;
						c.Model = command.Value.Model;
						c.ReleaseYear = command.Value.ReleaseYear;
						c.Owner = command.Value.Owner;

						result = "Car updating sucess.";
					}
					catch (Exception) {
						result = "Car updating failed.";
					}

					break;
				case HttpCommandMethods.DELETE:
					try
					{
						Cars.RemoveAll(c => c.Id == command.Value.Id);
						result = "Car deleting sucess.";
					}
					catch (Exception)
					{
						result = "Car deleting failed.";
					}

					break;
				default: break;
			}

			bw.Write(result);
			bw.Flush();
		}
	}
	catch (IOException)
	{
		Console.WriteLine("\tClient disconnected.\n");
	}
	finally
	{
		client.Close();
	}
}
