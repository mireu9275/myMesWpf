namespace MesClient.Core.Enums;

/// <summary>
/// 작업지시 상태
/// </summary>
public enum WorkOrderStatus
{
    /// <summary>계획됨</summary>
    Planned = 0,
    
    /// <summary>대기중</summary>
    Waiting = 1,
    
    /// <summary>진행중</summary>
    InProgress = 2,
    
    /// <summary>일시정지</summary>
    Paused = 3,
    
    /// <summary>완료</summary>
    Completed = 4,
    
    /// <summary>취소</summary>
    Cancelled = 5
}
