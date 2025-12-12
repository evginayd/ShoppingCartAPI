using System.Net;
using System.Text.Json;
using ShoppingCartAPI.Exceptions;

namespace ShoppingCartAPI.Middleware
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = context.Response;

            object errorResponse;

            switch (ex)
            {
                case InvalidQuantityException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { error = ex.Message };
                    break;

                case ProductNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new { error = ex.Message };
                    break;
                    
                case CartItemNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new { error = ex.Message };
                    break;

                case UnauthorizedCartAccessException:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse = new { error = ex.Message };
                    break;

                case InvalidCartQuantityException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { error = ex.Message };
                    break;

                case QuantityExceedsCartItemException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { error = ex.Message };
                    break;

                case CustomerAlreadyExistsException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { error = ex.Message };
                    break;

                case InvalidBudgetValueException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { error = ex.Message };
                    break;

                case CustomerNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new { error = ex.Message };
                    break;

                case InvalidCredentialsException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new { error = ex.Message };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new { error = "An unexpected error occurred." };
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
