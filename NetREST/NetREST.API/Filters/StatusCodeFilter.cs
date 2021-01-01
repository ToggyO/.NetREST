using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NetREST.Common.Response;

namespace NetREST.API.Filters
{
	public class StatusCodeFilter : IResultFilter
	{
		private readonly ILogger<StatusCodeFilter> _logger;
		
		public StatusCodeFilter(ILogger<StatusCodeFilter> logger)
		{
			_logger = logger;
		}

		public void OnResultExecuting(ResultExecutingContext context)
		{
			try {
				if (context.Result is ObjectResult objectResult)
				{
					if (objectResult.Value is Response response)
					{
						context.HttpContext.Response.StatusCode = (int) response.HttpStatusCode;
					}
				}
			} catch (Exception ex)
			{
					_logger.LogError(ex, ex.Message);
			}
		}

		public void OnResultExecuted(ResultExecutedContext context)
		{

		}
	}
}