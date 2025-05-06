using Microsoft.AspNetCore.Diagnostics;

namespace CurrencyAPI.Handlers
{
    public class ExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var error = new { message = exception.Message };
            await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);
            return true;
        }
    }
}
