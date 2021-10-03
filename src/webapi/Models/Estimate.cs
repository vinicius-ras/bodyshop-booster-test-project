using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BodyShopBoosterTest.Enumerations;

namespace BodyShopBoosterTest.Models
{
	/// <summary>A model representing service estimates which are sent to clients.</summary>
	public class Estimate
	{
		// INSTANCE PROPERTIES
		/// <summary>Unique identifier for the estimate in the database.</summary>
		public Guid Id { get; set; }
		/// <summary>The first name of the client who requested the estimate.</summary>
		[Required, StringLength(maximumLength: 100, MinimumLength = 1)]
		public string FirstName { get; set; }
		/// <summary>The last name of the client who requested the estimate.</summary>
		[Required, StringLength(maximumLength: 100, MinimumLength = 1)]
		public string LastName { get; set; }
		/// <summary>The type of the car for which the estimate was requested.</summary>
		[Required, Column(TypeName = "nvarchar(20)")]
		public VehicleType? CarType { get; set; }
		/// <summary>Identifies the year of the vehicle for which an estimate has been requested.</summary>
		[Required, StringLength(maximumLength: 20, MinimumLength = 1)]
		public string Year { get; set; }
		/// <summary>Identifies the model of the vehicle for which an estimate has been requested.</summary>
		[Required, StringLength(maximumLength: 100, MinimumLength = 1)]
		public string Model { get; set; }
		/// <summary>The license plate of the vehicle for which an estimate has been requested.</summary>
		[Required, StringLength(maximumLength: 30, MinimumLength = 1)]
		public string LicensePlate { get; set; }
		/// <summary>The current status of the given estimate.</summary>
		[Required, Column(TypeName = "nvarchar(20)")]
		public EstimateStatus? Status { get; set; }
	}
}