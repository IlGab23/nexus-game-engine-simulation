using System.Diagnostics;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Application.Behavior;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> log)
: IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        if (log.IsEnabled(LogLevel.Information))
        {
            string requestPayload = JsonSerializer.Serialize(request);
            log.LogInformation("Processing: {RequestType}\nPayload: {RequestString}", requestName, requestPayload);
        }

        long startTimeStamp = Stopwatch.GetTimestamp();
        var response = await next(cancellationToken);
        TimeSpan elapsed = Stopwatch.GetElapsedTime(startTimeStamp);

        if (log.IsEnabled(LogLevel.Information))
        {
            bool result = false;

            if (response is ResultBase resultResponse)
            {
                result = resultResponse.IsSuccess;
            }

            log.LogInformation("Terminated: {RequestType} ---> Estimated Time: {Elapsed}\nResult: {Result}", requestName, elapsed, result);
        }

        return response;
    }

}
