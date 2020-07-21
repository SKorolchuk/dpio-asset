using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Deeproxio.Asset.API.Infrastructure
{
    public class LoggerInterceptor : Interceptor
    {
        private readonly ILogger<LoggerInterceptor> _logger;

        public LoggerInterceptor(ILogger<LoggerInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            LogCall(context);

            try
            {
                return await continuation(request, context);
            }
            catch (RpcException exception)
            {
                _logger.LogError(exception, $"An error occured when calling {context.Method}");

                throw exception;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occured when calling {context.Method}");

                throw new RpcException(new Status(StatusCode.Internal, e.Message));
            }
        }

        private void LogCall(ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();

            _logger.LogDebug($"Starting call. Request: {httpContext.Request.Path}");
        }
    }
}
