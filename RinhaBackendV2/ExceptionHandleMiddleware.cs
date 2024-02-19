namespace RinhaBackendV2
{
    public class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandleMiddleware(RequestDelegate next)
        {
            _next= next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }catch(Exception e)
            {
                await HandleException(e, httpContext);
            }
        }

        private async Task HandleException(Exception ex, HttpContext httpContext)
        {


            if (ex is InvalidOperationException)
            {
                if(ex.Message == "cliente nao existe")
                    Results.NotFound(ex);
            }
        }
    }

    public static class ExceptionHandleMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandleMiddleware>();
        }
    }
}
