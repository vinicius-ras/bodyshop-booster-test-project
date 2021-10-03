using System;
using System.Threading.Tasks;
using BodyShopBoosterTest.Data;
using BodyShopBoosterTest.Enumerations;
using BodyShopBoosterTest.Exceptions;
using BodyShopBoosterTest.Models;

namespace BodyShopBoosterTest.Services
{
	/// <summary>Implementation for the <see cref="IEstimatesService"/> interface.</summary>
	public class EstimatesService : IEstimatesService
	{
		// CONSTANTS
		/// <summary>
		///     An error code indicating that the <see cref="CreateEstimateAsync(Estimate)"/> method has been called with an <see cref="Estimate"/> object
		///     that does not have the <see cref="EstimateStatus.Pending"/> status.
		/// </summary>
		private const string ErrorCodeCreatingNonPendingEstimate = "b1e0060ad9244382a057ce4ac38e84a0";





		// INSTANCE FIELDS
		/// <summary>Container-injected instance of a database context, for performing database operations.</summary>
		private readonly AppDbContext _appDbContext;





		// INSTANCE METHODS
		/// <summary>Constructor.</summary>
		/// <param name="appDbContext">Container-injected instance of a database context, for performing database operations.</param>
		public EstimatesService(AppDbContext appDbContext) =>
			_appDbContext = appDbContext;





		// INTERFACE IMPLEMENTATION: IEstimatesService
		/// <inheritdoc/>
		public async Task<Estimate> CreateEstimateAsync(Estimate estimate)
		{
			if (estimate.Status != EstimateStatus.Pending)
				throw new ServiceException($@"New estimates must be created with a ""{EstimateStatus.Pending}"" status.")
				{
					AppErrorCode = ErrorCodeCreatingNonPendingEstimate,
				};

			try
			{
				await _appDbContext.AddAsync(estimate);
				await _appDbContext.SaveChangesAsync();
				return estimate;
			}
			catch (Exception ex)
			{
				throw new ServiceException($"An error occurred while trying to save the {nameof(Estimate)} to the database.", ex)
				{
					AppErrorCode = AppExceptionErrorCodes.DatabaseUpdateError,
				};
			}
		}
	}
}