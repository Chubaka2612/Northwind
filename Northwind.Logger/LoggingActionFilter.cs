using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Northwind.Logger
{
    public class LoggingActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;
        private readonly bool _logParameters;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger, bool logParameters = false)
        {
            _logger = logger;
            _logParameters = logParameters;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionName = context.ActionDescriptor.DisplayName;

            _logger.LogInformation("Action {ActionName} started.", actionName);

            if (_logParameters)
            {
                var parameters = string.Join(", ", context.ActionArguments.Select(p => $"{p.Key}: {p.Value}"));
                _logger.LogInformation("Action {ActionName} parameters: {Parameters}", actionName, parameters);
            }

            var executedContext = await next();

            _logger.LogInformation("Action {ActionName} ended.", actionName);

            if (executedContext.Exception != null)
            {
                _logger.LogError(executedContext.Exception, "Action {ActionName} threw an exception.", actionName);
            }
        }
    }
}