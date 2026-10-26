using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.SharedLirary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract specific headers from request
            var signedHeader = context.Request.Headers["Api-Gateway"];

            //NULL means, Request is not comming from api-gateway // service unavilabe to user
            if (signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, Service is unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }

    }
}
