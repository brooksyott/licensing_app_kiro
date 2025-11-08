using Microsoft.AspNetCore.Mvc;
using LicenseManagementApi.Models;

namespace LicenseManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult ToActionResult<T>(ServiceResult<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        // Handle validation errors (400 Bad Request)
        if (result.ValidationErrors != null && result.ValidationErrors.Any())
        {
            return BadRequest(new
            {
                errors = result.ValidationErrors
            });
        }

        // Handle not found errors (404 Not Found)
        if (result.ErrorCode == "NOT_FOUND")
        {
            return NotFound(new
            {
                message = result.ErrorMessage,
                errorCode = result.ErrorCode
            });
        }

        // Handle validation/business logic errors (400 Bad Request)
        if (result.ErrorCode == "VALIDATION_ERROR" || result.ErrorCode == "BUSINESS_RULE_VIOLATION")
        {
            return BadRequest(new
            {
                message = result.ErrorMessage,
                errorCode = result.ErrorCode
            });
        }

        // Handle all other errors as internal server errors (500)
        return StatusCode(500, new
        {
            message = result.ErrorMessage ?? "An internal server error occurred",
            errorCode = result.ErrorCode ?? "INTERNAL_ERROR"
        });
    }

    protected IActionResult ToActionResult<T>(ServiceResult<PagedResult<T>> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return ToActionResult((ServiceResult<object>)new ServiceResult<object>
        {
            IsSuccess = result.IsSuccess,
            ErrorMessage = result.ErrorMessage,
            ErrorCode = result.ErrorCode,
            ValidationErrors = result.ValidationErrors
        });
    }
}
