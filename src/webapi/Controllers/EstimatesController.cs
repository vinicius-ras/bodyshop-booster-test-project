using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BodyShopBoosterTest.Attributes;
using BodyShopBoosterTest.Data;
using BodyShopBoosterTest.Exceptions;
using BodyShopBoosterTest.Models;
using BodyShopBoosterTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;

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
		[HttpGet("{id}")]
		public async Task<ActionResult<Estimate>> GetEstimateById() => throw new System.NotImplementedException();


		/// <summary>Constructor.</summary>
		/// <param name="estimatesService">Container-injected service for accessing <see cref="Estimate"/>-related operations.</param>
		public EstimatesController(IEstimatesService estimatesService) =>
			_estimatesService = estimatesService;


		/// <summary>Endpoint for registering a new estimate in the database.</summary>
		/// <param name="estimate">The data for the estimate to be registered in the database.</param>
		/// <returns>Returns a <see cref="Task"/> representing the asynchronous operation, and wrapping the result of this action's execution.</returns>
		/// <response code="201">
		///     Indicates the operation was successful. An <see cref="Estimate"/> containing the newly created client's data
		///     will be returned in the response's body. The response will also contain a "Location" HTTP Header indicating the URI where
		///     API clients can retrieve the created client's data.
		/// </response>
		/// <response code="400">
		///     Indicates validation errors in the request body's payload data.
		///     The response body will contain a <see cref="ValidationProblemDetails"/> instance describing the errors.
		/// </response>
		[HttpPost]
		[ProducesResponseType(typeof(Estimate), 201)]
		[ProducesResponseType(typeof(ValidationProblemDetails), 400)]
		[CustomResponseHeader(StatusCode = HttpStatusCode.Created, HeaderName = nameof(HeaderNames.Location), Description = "URI to the newly registered Client Application's data.")]
		public async Task<IActionResult> CreateEstimateAsync([FromBody] Estimate estimate)
		{
			try
			{
				var createdEstimate = await _estimatesService.CreateEstimateAsync(estimate);
				return CreatedAtAction(
					nameof(GetEstimateById),
					new RouteValueDictionary {
						["id"] = estimate.Id,
					},
					createdEstimate);
			}
			catch (ServiceException svcEx)
			{
				if (svcEx.AppErrorCode == AppExceptionErrorCodes.DatabaseUpdateError)
					return StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to update the database: {svcEx.Message}");
				else if (svcEx.ValidationErrors?.Any() ?? false)
					return ValidationProblem(svcEx.ValidationErrors);
				return BadRequest($"Error while saving data: {svcEx.Message}");
			}
		}
	}
}