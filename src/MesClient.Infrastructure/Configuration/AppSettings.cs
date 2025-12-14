namespace MesClient.Infrastructure.Configuration;

/// <summary>
/// 애플리케이션 설정
/// </summary>
public class AppSettings
{
    /// <summary>
    /// API 설정
    /// </summary>
    public ApiSettings Api { get; set; } = new();

    /// <summary>
    /// 데이터베이스 설정
    /// </summary>
    public DatabaseSettings Database { get; set; } = new();

    /// <summary>
    /// 로깅 설정
    /// </summary>
    public LoggingSettings Logging { get; set; } = new();

    /// <summary>
    /// UI 설정
    /// </summary>
    public UiSettings Ui { get; set; } = new();
}

/// <summary>
/// API 설정
/// </summary>
public class ApiSettings
{
    /// <summary>
    /// API 기본 URL
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5000";

    /// <summary>
    /// 요청 타임아웃 (초)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 재시도 횟수
    /// </summary>
    public int RetryCount { get; set; } = 3;
}

/// <summary>
/// 데이터베이스 설정
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// 연결 문자열
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 로컬 캐시 사용 여부
    /// </summary>
    public bool UseLocalCache { get; set; } = true;
}

/// <summary>
/// 로깅 설정
/// </summary>
public class LoggingSettings
{
    /// <summary>
    /// 로그 파일 경로
    /// </summary>
    public string LogPath { get; set; } = "logs";

    /// <summary>
    /// 로그 보관 일수
    /// </summary>
    public int RetainDays { get; set; } = 30;

    /// <summary>
    /// 최소 로그 레벨
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";
}

/// <summary>
/// UI 설정
/// </summary>
public class UiSettings
{
    /// <summary>
    /// 테마 (Light/Dark)
    /// </summary>
    public string Theme { get; set; } = "Light";

    /// <summary>
    /// 기본 언어
    /// </summary>
    public string Language { get; set; } = "ko-KR";

    /// <summary>
    /// 데이터 새로고침 간격 (초)
    /// </summary>
    public int RefreshIntervalSeconds { get; set; } = 10;

    /// <summary>
    /// 알림 표시 시간 (초)
    /// </summary>
    public int NotificationDurationSeconds { get; set; } = 5;
}
