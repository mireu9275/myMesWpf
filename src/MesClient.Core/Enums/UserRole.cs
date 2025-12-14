namespace MesClient.Core.Enums;

/// <summary>
/// 사용자 역할
/// </summary>
[Flags]
public enum UserRole
{
    /// <summary>없음</summary>
    None = 0,
    
    /// <summary>작업자</summary>
    Operator = 1,
    
    /// <summary>품질관리자</summary>
    QualityController = 2,
    
    /// <summary>현장관리자</summary>
    Supervisor = 4,
    
    /// <summary>공장장</summary>
    Manager = 8,
    
    /// <summary>시스템관리자</summary>
    Admin = 16
}
