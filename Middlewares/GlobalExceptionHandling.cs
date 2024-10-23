using System.Text.Json;

namespace Reddit.Middlewares
    {
        public class GlobalExceptionHandling
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<GlobalExceptionHandling> _logger;

            public GlobalExceptionHandling(RequestDelegate next, ILogger<GlobalExceptionHandling> logger)
            {
                this._next = next;
                this._logger = logger;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "dude, go fix this mess");

                    var errorResponseModel = new ErrorResponse
                    {
                        Message = "Unexpected error occured on server",
                        Description = ex.Message
                    };

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json"; 

                    var jsonResponse = JsonSerializer.Serialize(errorResponseModel);

                    await context.Response.WriteAsync(jsonResponse);
                }
            }

        }
    }
