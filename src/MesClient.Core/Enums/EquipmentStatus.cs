namespace MesClient.Core.Enums;

/// <summary>
/// 설비 상태
/// </summary>
public enum EquipmentStatus
{
    /// <summary>가동중</summary>
    Running = 0,
    
    /// <summary>대기중</summary>
    Idle = 1,
    
    /// <summary>고장</summary>
    Down = 2,
    
    /// <summary>정비중</summary>
    Maintenance = 3,
    
    /// <summary>셋업중</summary>
    Setup = 4,
    
    /// <summary>오프라인</summary>
    Offline = 5
}
