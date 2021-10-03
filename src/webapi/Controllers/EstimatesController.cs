using System.Net;
using System.Threading.Tasks;
using BodyShopBoosterTest.Data;
using BodyShopBoosterTest.Exceptions;
using BodyShopBoosterTest.Models;
using BodyShopBoosterTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace BodyShopBoosterTest
{
	/// <summary>A controller providing estimate-related endpoints.</summary>
	[Route("/estimates")]
	[ApiController]
	public class EstimatesController : ControllerBase
	{
		// INSTANCE FIELDS
		/// <summary>Container-injected service for accessing <see cref="Estimate"/>-related operations.</summary>
		private readonly IEstimatesService _estimatesService;





		// INSTANCE METHODS
		/// <summary>Constructor.</summary>
		/// <param name="estimatesService">Container-injected service for accessing <see cref="Estimate"/>-related operations.</param>
		public EstimatesController(IEstimatesService estimatesService) =>
			_estimatesService = estimatesService;


		/// <summary>Endpoint for registering a new estimate in the database.</summary>
		/// <param name="estimate">The data for the estimate to be registered in the database.</param>
		/// <returns>Returns a <see cref="Task"/> representing the asynchronous operation, and wrapping the result of this action's execution.</returns>
		[HttpPost]
		public async Task<IActionResult> CreateEstimateAsync([FromBody] Estimate estimate)
		{
			try
			{
				await _estimatesService.CreateEstimateAsync(estimate);
				return Ok();
			}
			catch (ServiceException svcEx)
			{
				if (svcEx.AppErrorCode == AppExceptionErrorCodes.DatabaseUpdateError)
					return StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to update the database: {svcEx.Message}");
				return BadRequest($"Error while saving data: {svcEx.Message}");
			}
		}
	}
}