using Azure;
using System.Collections.Concurrent;

namespace Magic.Guangdong.Exam.Tools
{
    public class SseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ConcurrentDictionary<Guid, HttpResponse> _clients = new();

        public SseMiddleware(RequestDelegate next)
        {
            _next = next;
            //_httpClient = httpClientFactory.CreateClient();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/api/sse")
            {
                var clientId = Guid.NewGuid();
                _clients[clientId] = context.Response;

                context.Response.Headers["Content-Type"] = "text/event-stream";
                context.Response.Headers["Cache-Control"] = "no-cache";
                //context.Response.Headers["Connection"] = "keep-alive";
                if (context.Request.Protocol.StartsWith("HTTP/1.1"))
                {
                    context.Response.Headers["Connection"] = "keep-alive";
                }
                var cancellationToken = context.RequestAborted;

                try
                {
                    await HandleSseConnection(clientId, cancellationToken);
                }
                finally
                {
                    _clients.TryRemove(clientId, out _);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task HandleSseConnection(Guid clientId, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // 等待消息或取消
                await Task.Delay(1000, cancellationToken);
            }
        }

        public async Task BroadcastMessage(string message)
        {
            foreach (var client in _clients.Values)
            {
                var sseMessage = $"data: {message}\n\n";
                _ = await client.WriteAsync(sseMessage).ContinueWith(t => client.Body.FlushAsync());
            }
        }
    }

    public static class SseMiddlewareExtensions
    {
        public static IApplicationBuilder UseSse(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SseMiddleware>();
        }
    }

}
