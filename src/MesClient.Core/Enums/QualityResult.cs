namespace MesClient.Core.Enums;

/// <summary>
/// 품질 검사 결과
/// </summary>
public enum QualityResult
{
    /// <summary>미검사</summary>
    NotInspected = 0,
    
    /// <summary>합격</summary>
    Pass = 1,
    
    /// <summary>불합격</summary>
    Fail = 2,
    
    /// <summary>조건부 합격</summary>
    ConditionalPass = 3,
    
    /// <summary>재검사 필요</summary>
    NeedReInspection = 4
}
