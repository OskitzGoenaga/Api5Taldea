using Microsoft.AspNetCore.Mvc;
using WebApi.Responses;

namespace WebApi.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult Success<T>(T data, int code, string message = "OK")
            => Ok(ApiResponse<T>.Ok(data, code, message));
        protected IActionResult SuccessList<T>(List<T> datalist, int code, string message = "OK")
            => Ok(ApiResponse<T>.Ok(datalist, code, message));
        protected IActionResult Created<T>(T data, int code, string message = "Zuzen sortu da")
            => StatusCode(201, ApiResponse<T>.Ok(data, code, message));

        protected IActionResult Error(int code, string type, string message, string detail = null)
            => StatusCode(code, ApiError.Create(code, type, message, detail));

        protected IActionResult NotFoundError(string detail)
            => Error(404, "NOT_FOUND", "Eskatutakoa ez da aurkitu", detail);
    }
}
