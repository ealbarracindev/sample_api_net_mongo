using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using web_api.Core.Exceptions;
using web_api.Infrastructure.Diagnostics;
using web_api.Infrastructure.Responses;

namespace web_api.Infrastructure.Middlewares
{
    public static class ErrorHandlerMiddlewareExtensions
    {
        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }

    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
                var responseModel = new ApiResponse<string>()
                {
                    traceId = string.IsNullOrEmpty(context.TraceIdentifier) ? Guid.NewGuid().ToString() : context.TraceIdentifier,
                    isSuccess = false,
                    message = error?.Message,
                    instance = context.Request.Path
                };

                switch (error)
                {
                    case BusinessException e:
                        // custom application error
                        //types logs:
                        //Log.Information("Log: Log.Information");
                        //Log.Warning("Log: Log.Warning");
                        //Log.Error("Log: Log.Error");
                        //Log.Fatal("Log: Log.Fatal");
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogInformationCustom($"[{response.StatusCode}]: {e?.Message}");
                        break;
                    case ApiException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogWarningCustom($"[{response.StatusCode}]: {e?.Message}");
                        break;
                    case ValidationCustomException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.errors = e.Errors;
                        _logger.LogInformationCustom($"[{response.StatusCode}]: {e.Errors}");
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        _logger.LogWarningCustom($"[{response.StatusCode}]: {e?.Message}");
                        break;
                    case InvalidStatusChangeException e:
                        // custom status error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogWarningCustom($"[{response.StatusCode}]: {e?.Message}");
                        break;
                    case EntityNotFoundException e:
                        // custom status error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogWarningCustom($"[{response.StatusCode}]: {e?.Message}");
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        _logger.LogErrorCustom($"[{response.StatusCode}]: {error?.Message}", error);
                        break;
                }
                responseModel.status = response.StatusCode;
                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
