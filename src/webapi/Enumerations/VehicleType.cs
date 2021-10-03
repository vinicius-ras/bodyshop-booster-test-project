namespace BodyShopBoosterTest.Enumerations
{
	/// <summary>Identifies a type of a vehicle for which estimates can be requested.</summary>
	public enum VehicleType
	{
		/// <summary>Identifier for the other types of vehicles which do not fall into any of the other more specific types.</summary>
		Other,
		/// <summary>Identifier for the "Car" vehicles.</summary>
		Car,
		/// <summary>Identifier for the "Truck" vehicles.</summary>
		Truck,
		/// <summary>Identifier for the "SUV" vehicles.</summary>
		Suv,
	}
}