using System;
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
		/// <summary>Retrieves an <see cref="Estimate"/> object from the database, given its unique ID.</summary>
		/// <param name="estimateId">The unique identifier for the <see cref="Estimate"/> entry to be retrieved from the database.</param>
		/// <returns>
		///     Returns a <see cref="Task{TResult}"/> representing the asynchronous operation, and wrapping the found instance's data.
		///     The <see cref="Task{TResult}"/> will have a <c>null</c> result if the entry does not exist in the database.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown if the given estimate's ID parameter is set to <c>null</c>.</exception>
		Task<Estimate> GetEstimateByIdAsync(Guid estimateId);
		/// <summary>Updates an existing <see cref="Estimate"/> entry in the database.</summary>
		/// <param name="estimate">
		///     An object containing the data to be saved (updated) in the database.
		///     This object should have its <see cref="Estimate.Id"/> set to the unique ID of the entry that needs to be updated in the database.
		/// </param>
		/// <returns>
		///     Returns a <see cref="Task"/> representing the asynchronous operation, and wrapping an updated version containing the data of
		///     the saved <see cref="Estimate"/> object (including its database ID).
		/// </returns>
		/// <exception cref="ServiceException">
		///     Thrown for any unexpected error during the method's execution, such as in cases of invalid input data or database communication errors.
		/// </exception>
		Task<Estimate> UpdateEstimateAsync(Estimate estimate);
	}
}