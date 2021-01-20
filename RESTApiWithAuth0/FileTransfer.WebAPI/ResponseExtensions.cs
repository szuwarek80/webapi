using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FileTransfer.WebAPI.Definitions
{
    public static class ResponseExtensions
    {
        public static IActionResult ToHttpResponse(this IResponse response)
            => new ObjectResult(response)
            {
                StatusCode = (int)(response.HasError ? HttpStatusCode.InternalServerError : HttpStatusCode.OK)
            };

        public static IActionResult ToHttpResponse<TModel>(this ISingleResponse<TModel> response)
        {
            var status = HttpStatusCode.OK;

            if (response.HasError)
                status = HttpStatusCode.InternalServerError;
            else if (response.Model == null)
                status = HttpStatusCode.NotFound;

            return new ObjectResult(response)
            {
                StatusCode = (int)status
            };
        }

        public static IActionResult ToHttpCreatedResponse<TModel>(this ISingleResponse<TModel> response)
        {
            var status = HttpStatusCode.Created;

            if (response.HasError)
                status = HttpStatusCode.InternalServerError;
            else if (response.Model == null)
                status = HttpStatusCode.NotFound;

            return new ObjectResult(response)
            {
                StatusCode = (int)status
            };
        }

        public static IActionResult ToHttpResponse<TModel>(this IListResponse<TModel> response)
        {
            var status = HttpStatusCode.OK;

            if (response.HasError)
                status = HttpStatusCode.InternalServerError;
            else if (response.Model == null)
                status = HttpStatusCode.NoContent;

            return new ObjectResult(response)
            {
                StatusCode = (int)status
            };
        }
    }
}
