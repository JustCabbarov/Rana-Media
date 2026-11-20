using System.Diagnostics;

namespace RanaMedia.Contract.DTOs.Common;

public record ErrorDetail(string? Field, string Message, string? Code = null);

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public IReadOnlyList<ErrorDetail>? Errors { get; set; }
    public string? TraceId { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null, string? traceId = null) => new()
    {
        Success = true,
        Data = data,
        Message = message,
        TraceId = traceId ?? Activity.Current?.Id
    };

    public static ApiResponse<T> Fail(
        string message,
        string? errorCode = null,
        IReadOnlyList<ErrorDetail>? errors = null,
        string? traceId = null) => new()
    {
        Success = false,
        Message = message,
        ErrorCode = errorCode,
        Errors = errors,
        TraceId = traceId ?? Activity.Current?.Id
    };
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}

public class CursorPagedResult<T>
{
    public List<T> Nodes { get; set; } = new();
    public bool HasNextPage { get; set; }
    public string? EndCursor { get; set; }
    public int? TotalCount { get; set; }
}

