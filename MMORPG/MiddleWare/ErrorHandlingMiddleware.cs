using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MMORPG.Help;

namespace MMORPG.MiddleWare
    {
        public class ErrorHandlingMiddleware
        {
            private readonly RequestDelegate _next;

            public ErrorHandlingMiddleware(RequestDelegate next)
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

                    switch(error)
                    {
                        case NotFoundException e:
                            // custom application error
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                        default:
                            // unhandled error
                            response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            break;
                    }
                    var result = JsonSerializer.Serialize(new {message = error?.Message});
                    await response.WriteAsync(result);
                }
            }
        }
    }