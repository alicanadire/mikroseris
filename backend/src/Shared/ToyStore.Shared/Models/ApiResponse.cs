using System.Net;

namespace ToyStore.Shared.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int StatusCode { get; set; } = (int)HttpStatusCode.OK;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? ErrorCode { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> SuccessResult(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = (int)HttpStatusCode.OK
        };
    }

    public static ApiResponse<T> ErrorResult(string message, int statusCode = 400, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            ErrorCode = errorCode
        };
    }

    public static ApiResponse<T> ErrorResult(List<string> errors, string message = "Validation failed", int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors
        };
    }
}

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public static PaginatedResponse<T> Create(List<T> data, int totalCount, int pageSize, int currentPage)
    {
        return new PaginatedResponse<T>
        {
            Data = data,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = currentPage,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
