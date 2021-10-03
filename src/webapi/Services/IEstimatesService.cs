using System.Threading.Tasks;
using BodyShopBoosterTest.Exceptions;
using BodyShopBoosterTest.Models;

namespace BodyShopBoosterTest.Services
{
	/// <summary>A service providing access to operations related to <see cref="Estimate"/> model instances.</summary>
	public interface IEstimatesService
	{
		/// <summary>Creates a new <see cref="Estimate"/> entry in the database.</summary>
		/// <param name="estimate">An object containing the data to be registered in the database.</param>
		/// <returns>
		///     Returns a <see cref="Task"/> representing the asynchronous operation, and wrapping an updated version containing the data of
		///     the saved <see cref="Estimate"/> object (including its database ID).
		/// </returns>
		/// <exception cref="ServiceException">
		///     Thrown for any unexpected error during the method's execution, such as in cases of invalid input data or database communication errors.
		/// </exception>
		Task<Estimate> CreateEstimateAsync(Estimate estimate);
	}
}