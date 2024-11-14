using HTTPCarManager.Models;
using System.Net;
using System.Text.Json;

List<Car> Cars = [];

HttpListener listener = new();
listener.Prefixes.Add("http://localhost:27001/");
listener.Start();

Cars.Add(new Car("Honda", "Insight", 2017, "Brown Doe"));
Cars.Add(new Car("Honda", "Civic", 2018, "John Doe"));
Cars.Add(new Car("Honda", "Accord", 2019, "John Smith"));

Console.WriteLine("Server is listening...");

try
{
	while (true)
	{
		string result = "";

		HttpListenerContext context = listener.GetContext();
		HttpListenerRequest request = context.Request;

		string actionType = request.HttpMethod;

		using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
		{
			string content = reader.ReadToEnd();
			Console.WriteLine($"\t\tContent Received.\n");

			switch (actionType)
			{
				case "GET":
					result = JsonSerializer.Serialize(Cars);
					Console.WriteLine($"\n\n\t{result}\n");
					break;
				case "POST":
					try
					{
						using JsonDocument jsonDoc = JsonDocument.Parse(content);

						JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

						Car car = JsonSerializer.Deserialize<Car>(jsonDoc.RootElement, options);

						Cars.Add(car!);

						result = "Car adding sucess.";
					}
					catch (Exception)
					{
						result = "Car adding failed.";
					}
					break;
				case "PUT":
					try
					{
						using JsonDocument jsonDoc = JsonDocument.Parse(content);

						JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

						Car car = JsonSerializer.Deserialize<Car>(jsonDoc.RootElement, options);

						var c = Cars.FirstOrDefault(c => c.Id == car!.Id);

						c!.Vendor = car!.Vendor;
						c.Model = car.Model;
						c.ReleaseYear = car.ReleaseYear;
						c.Owner = car.Owner;

						result = "Car updating sucess.";
					}
					catch (Exception)
					{
						result = "Car updating failed.";
					}

					break;
				case "DELETE":
					try
					{
						using JsonDocument jsonDoc = JsonDocument.Parse(content);

						JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

						Car car = JsonSerializer.Deserialize<Car>(jsonDoc.RootElement, options);

						Cars.RemoveAll(c => c.Id == car!.Id);

						result = "Car deleting sucess.";
					}
					catch (Exception)
					{
						result = "Car deleting failed.";
					}

					break;
				default: break;
			}

			StreamWriter w = new StreamWriter(context.Response.OutputStream);
			w.Write(result);
			w.Flush();
			w.Close();
		}
	}
}
catch (IOException)
{
	Console.WriteLine("\tClient disconnected.\n");
}
finally
{
	listener.Close();
}
