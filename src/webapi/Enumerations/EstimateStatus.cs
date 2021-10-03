namespace BodyShopBoosterTest.Enumerations
{
	/// <summary>The status of a given <see cref="BodyShopBoosterTest.Models.Estimate"/> object.</summary>
	public enum EstimateStatus
	{
		/// <summary>Indicates the estimate's evaluation is currently pending.</summary>
		Pending,
		/// <summary>Indicates the estimate has already been sent to the client.</summary>
		/// <remarks>
		///     This state indicates the estimate has been sent to the client, but the client still hasn't
		///     confirmed/booked the services.
		/// </remarks>
		Sent,
		/// <summary>Indicates the client has received the estimate and booked the services.</summary>
		BookConfirmed,
	}
}