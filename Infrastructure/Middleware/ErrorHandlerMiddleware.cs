using System.Net;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Infrastructure.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = error switch
                {
                    AppException => (int)HttpStatusCode.BadRequest,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    ForbiddenException => (int)HttpStatusCode.Forbidden,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var errorDetails = new ErrorDetails
                {
                    Message = error.Message,
                    Instance = context.Request.Path,
                    Status = response.StatusCode,
                };

                var result = JsonConvert.SerializeObject(errorDetails);
                await response.WriteAsync(result);
            }
        }

        private class ErrorDetails
        {
            public string Message { get; set; }
            public string Instance { get; set; }
            public int Status { get; set; }
        }
    }
}