using Ecommerce.SharedLirary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Ecommerce.SharedLirary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declare default variables
            string message = "Sorry, internal server error occured. Kindly try again";
            int statusCode = (int)StatusCodes.Status500InternalServerError;
            string title = "Error";

            try
            {
                await next(context);
                // checking if the response is too many requests // 429 status
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many requests made";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not Authorized to access";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }

                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Acess";
                    message = "You are not allowed to access";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }


            }
            catch (Exception ex)
            {
                // logging original exception to file,console and debugger
                LogException.LogExceptions(ex);

                // check if exception is timeout || 408 request timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "Request timeout... try again";
                    statusCode = (int)StatusCodes.Status408RequestTimeout;
                }

                await ModifyHeader(context, title, message, statusCode);

            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // display scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
        }
    }
}
