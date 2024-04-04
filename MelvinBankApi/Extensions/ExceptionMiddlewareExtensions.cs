using Contracts;
using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace MelvinBankApi.Extensions;

public static class ExceptionMiddlewareExtensions
{
	public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
	{
		app.UseExceptionHandler(appError =>
		{
			appError.Run(async context =>
			{
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				context.Response.ContentType = "application/json";

				var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
				if (contextFeature != null)
				{
					context.Response.StatusCode = contextFeature.Error switch
					{
						EntityExistsException => StatusCodes.Status400BadRequest,
						BadRequestException => StatusCodes.Status400BadRequest,
						EntityNotExistException => StatusCodes.Status404NotFound,	
						PinsNotMatchException => StatusCodes.Status400BadRequest,	
						_ => StatusCodes.Status500InternalServerError
						

					};
					logger.LogError($"Something went wrong: {contextFeature.Error}");

					await context.Response.WriteAsync(new ErrorDetails()
					{
						StatusCode = context.Response.StatusCode,
						Message = contextFeature.Error.Message,
					}.ToString());
				}
			});
		});

	}
}
