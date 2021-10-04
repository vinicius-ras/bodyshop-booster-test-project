using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BodyShopBoosterTest.Attributes;
using BodyShopBoosterTest.Data;
using BodyShopBoosterTest.Enumerations;
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
		/// <summary>Constructor.</summary>
		/// <param name="estimatesService">Container-injected service for accessing <see cref="Estimate"/>-related operations.</param>
		public EstimatesController(IEstimatesService estimatesService) =>
			_estimatesService = estimatesService;


		/// <summary>Registers a new estimate in the database.</summary>
		/// <param name="estimate">
		///     <para>The data for the estimate to be registered in the database.</para>
		///     <para>
		///         This data should not contain an <see cref="Estimate.Id"/> field,
		///         and its <see cref="Estimate.Status"/> field should always be set to <see cref="EstimateStatus.Pending"/>.
		///         Otherwise, this endpoint will return an HTTP 400 Bad Request status code.
		///     </para>
		/// </param>
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
		[ProducesResponseType(typeof(Estimate), (int)HttpStatusCode.Created)]
		[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
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
					return StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to update the database: {svcEx.Message?.TrimEnd('.') ?? "no details were provided"}.");
				else if (svcEx.ValidationErrors?.Any() ?? false)
					return ValidationProblem(svcEx.ValidationErrors);
				return BadRequest($"Error while saving data: {svcEx.Message}");
			}
		}


		/// <summary>Retrieves an existing estimate's data.</summary>
		/// <param name="estimateId">The unique identifier for the estimate to be retrieved.</param>
		/// <returns>Returns a <see cref="Task"/> representing the asynchronous operation, and wrapping the result of this action's execution.</returns>
		/// <response code="200">
		///     Indicates the operation was successful. The response's payload will contain the corresponding <see cref="Estimate"/>'s data.
		/// </response>
		/// <response code="400">
		///     Indicates the specified estimate's ID has been specified with an invalid format.
		///     The response body will contain a <see cref="ValidationProblemDetails"/> instance describing the errors.
		/// </response>
		/// <response code="404">Indicates the specified estimate could not be found on the database.</response>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(Estimate), (int)(HttpStatusCode.OK))]
		[ProducesResponseType(typeof(ValidationProblemDetails), (int)(HttpStatusCode.BadRequest))]
		public async Task<ActionResult<Estimate>> GetEstimateById([FromRoute(Name = "id")] Guid estimateId)
		{
			var foundEstimate = await _estimatesService.GetEstimateByIdAsync(estimateId);
			return foundEstimate == null
				? NotFound()
				: Ok(foundEstimate);
		}


		/// <summary>Updates an existing estimate's data.</summary>
		/// <param name="estimateId">
		///     The unique identifier for the estimate to be updated.
		///     This identifier must match the HTTP Request's payload ID.
		///     Otherwise, this endpoint will return an HTTP 400 Bad Request status code.
		/// </param>
		/// <param name="estimateData">
		///     <para>The data for the estimate to be updated in the database.</para>
		///     <para>
		///         This data should contain an <see cref="Estimate.Id"/> field, and this field must match the URL-provided ID.
		///         Otherwise, this endpoint will return an HTTP 400 Bad Request status code.
		///     </para>
		/// </param>
		/// <returns>Returns a <see cref="Task"/> representing the asynchronous operation, and wrapping the result of this action's execution.</returns>
		/// <response code="200">
		///     Indicates the operation was successful. The response's payload will contain the updated <see cref="Estimate"/>'s data.
		/// </response>
		/// <response code="400">
		///     Indicates a validation failure on the sent data. Check if the Estimate's ID matches in both the URL-provided ID and the HTTP Response payload.
		///     Also, verify if all of the payload's contents are valid.
		///     The response body will contain a <see cref="ValidationProblemDetails"/> instance describing the errors.
		/// </response>
		/// <response code="404">Indicates the specified estimate could not be found on the database.</response>
		[HttpPut("{id}")]
		[ProducesResponseType(typeof(Estimate), (int)(HttpStatusCode.OK))]
		[ProducesResponseType(typeof(ValidationProblemDetails), (int)(HttpStatusCode.BadRequest))]
		[ProducesResponseType(typeof(void), (int)(HttpStatusCode.NotFound))]
		public async Task<ActionResult<Estimate>> UpdateEstimate(
			[FromRoute(Name = "id")] Guid estimateId,
			[FromBody] Estimate estimateData)
		{
			// Perform extra validations
			if (estimateId != estimateData.Id)
				ModelState.AddModelError(nameof(Estimate.Id), $"The URL-provided ID ({estimateId}) does not match the HTTP Request's payload ID ({estimateData.Id}).");

			if (ModelState.Any(entry => entry.Value.Errors.Any()))
				return ValidationProblem(ModelState);

			// Try to update the estimate
			try
			{
				var updatedEstimate = await _estimatesService.UpdateEstimateAsync(estimateData);
				return Ok(updatedEstimate);
			}
			catch (ServiceException svcEx)
			{
				if (svcEx.AppErrorCode == AppExceptionErrorCodes.DatabaseUpdateError)
					return StatusCode((int)HttpStatusCode.InternalServerError, $"Failed to update the database: {svcEx.Message?.TrimEnd('.') ?? "no details were provided"}.");
				else if (svcEx.ValidationErrors?.Any() ?? false)
					return ValidationProblem(svcEx.ValidationErrors);
				return BadRequest($"Error while saving data: {svcEx.Message}");
			}
		}
	}
}