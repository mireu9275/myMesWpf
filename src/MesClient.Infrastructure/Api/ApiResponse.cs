namespace MesClient.Infrastructure.Api;

/// <summary>
/// API 응답 래퍼
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// 성공 여부
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 응답 데이터
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 오류 메시지
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 오류 코드
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 타임스탬프
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 성공 응답 생성
    /// </summary>
    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// 실패 응답 생성
    /// </summary>
    public static ApiResponse<T> Fail(string message, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode
        };
    }
}

/// <summary>
/// 페이징된 API 응답
/// </summary>
public class PagedApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    /// <summary>
    /// 전체 개수
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 페이지 번호
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 페이지 크기
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 전체 페이지 수
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// 다음 페이지 존재 여부
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// 이전 페이지 존재 여부
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
