namespace HTTPCarManager.Models;

public class Car
{
	public int Id { get; set; }
	public string Model { get; set; }
	public string Vendor { get; set; }
	public int ReleaseYear { get; set; }
	public string Owner { get; set; }

	public Car()
	{
		Id = IdCounter.GetNewId();
	}

	public Car(string model, string vendor, int releaseYear, string owner)
		: this()
	{
		Model=model;
		Vendor=vendor;
		ReleaseYear=releaseYear;
		Owner=owner;
	}

	public override string ToString() =>
		$" Id: {Id}. The {Vendor} {Model} {ReleaseYear} owned by {Owner}.";
}
public static class IdCounter
{
	private static int _counter = 1;
	public static int GetNewId() => _counter++;
}