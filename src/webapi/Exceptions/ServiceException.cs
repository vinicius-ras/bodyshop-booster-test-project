using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BodyShopBoosterTest.Exceptions
{
	/// <summary>An exception type which can be fired by operations belonging to the application's services layer.</summary>
	public class ServiceException : Exception
	{
		// INSTANCE PROPERTIES
		/// <summary>An application-specific error code to be associated with the exception.</summary>
		/// <value>
		///     Values for this property are usually either created as GUIDs in the <see cref="BodyShopBoosterTest.Data.AppExceptionErrorCodes"/> class,
		///     or defined at the service level.
		/// </value>
		public string AppErrorCode { get; init; }
		/// <summary>A dictionary containing any validation errors which caused the service to fire an exception.</summary>
		/// <value>
		///     A model state dictionary which can be used by controllers to return errors to client applications (e.g., via
		///     the <see cref="ControllerBase.ValidationProblem(ModelStateDictionary)"/> method).
		/// </value>
		public ModelStateDictionary ValidationErrors { get; init; }





		// INSTANCE METHODS
		/// <summary>Constructor.</summary>
		public ServiceException()
		{
		}


		/// <summary>Constructor.</summary>
		/// <param name="message">The message to be associated with the exception that is being created.</param>
		public ServiceException(string message)
			: base(message)
		{
		}


		/// <summary>Constructor.</summary>
		/// <param name="message">The message to be associated with the exception that is being created.</param>
		/// <param name="innerException">The exception which needs to be wrapped as the "cause" of the new exception.</param>
		public ServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}