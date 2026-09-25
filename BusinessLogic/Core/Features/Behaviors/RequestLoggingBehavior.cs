using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Core.Features.Behaviors
{
    public class RequestLoggingBehavior
    {
        /// <summary>
        /// Логирует начало и завершение обработки запроса и её длительность.
        /// Ничего не глотает и не логирует ошибку — это делает ExceptionHandlingBehavior.
        /// </summary>
        public class RequestsLoggingBehavior<TRequest, TResponse>
            : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            private readonly ILogger<RequestsLoggingBehavior<TRequest, TResponse>> _logger;

            public RequestsLoggingBehavior(ILogger<RequestsLoggingBehavior<TRequest, TResponse>> logger)
            {
                _logger = logger;
            }

            public async Task<TResponse> Handle(TRequest request,
                CancellationToken cancellationToken,
                RequestHandlerDelegate<TResponse> next)
            {
                var requestName = typeof(TRequest).Name;
                var stopwatch = Stopwatch.StartNew();
                var success = false;

                _logger.LogInformation("--> {RequestName} начат", requestName);

                try
                {
                    var response = await next();
                    success = true;
                    return response;
                }
                finally
                {
                    stopwatch.Stop();
                    _logger.LogInformation(
                        "<-- {RequestName} завершён за {ElapsedMilliseconds} мс (успех: {Success})",
                        requestName,
                        stopwatch.ElapsedMilliseconds,
                        success);
                }
            }
        }
    }
}
