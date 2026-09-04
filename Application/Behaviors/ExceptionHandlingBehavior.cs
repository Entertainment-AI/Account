using System.Collections.Concurrent;
using System.Reflection;
using Account.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> _failureMethods = new();
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(ex, "Unhandled exception for {RequestName} {@Request}", requestName, request);

            var responseType = typeof(TResponse);
            var error = new Error("INTERNAL_SERVER_ERROR", "An unexpected error occurred: " + ex.Message);

            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var innerType = responseType.GetGenericArguments()[0];
                var resultType = typeof(Result<>).MakeGenericType(innerType);

                var failureMethod = _failureMethods.GetOrAdd(resultType, type =>
                    type.GetMethod(
                        nameof(Result<object>.Failure),
                        BindingFlags.Public | BindingFlags.Static,
                        binder: null,
                        types: [typeof(Error)],
                        modifiers: null)!);

                var result = failureMethod.Invoke(null, [error]);
                return (TResponse)result!;
            }

            if (responseType == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(error);
            }

            throw;
        }
    }
}
