## 수정 내용

이 PR은 빌드 시 발생하던 컴파일 에러와 null 참조 경고를 모두 수정합니다.

### 주요 수정 사항

1. **AuthService.cs**
   - `User.Id`에 `string` 타입의 `userId`를 할당하던 타입 에러 수정
   - `UserId` 속성에 올바르게 할당하도록 변경
   - `Id`는 `DateTime.Now.Ticks`로 설정

2. **AlarmService.cs**
   - API 호출 결과에 null 체크 추가 (`?? Enumerable.Empty<Alarm>()`)
   - `AcknowledgeAlarmAsync` 메서드의 null 인자 경고 수정

3. **QualityService.cs**
   - 모든 API 호출 결과에 null 체크 추가
   - `RecordInspectionAsync`, `GetQualitySummaryAsync`, `GetDefectStatisticsAsync` 메서드의 null 반환 경고 수정

4. **ProductionService.cs**
   - API 호출 결과에 null 체크 추가
   - `RecordProductionAsync`, `GetTodaySummaryAsync` 메서드의 null 반환 경고 수정

### 테스트 결과

- ✅ 모든 컴파일 에러 해결
- ✅ null 참조 경고 해결
- ✅ Infrastructure 프로젝트 빌드 성공

### 참고

- `AlarmService.AlarmOccurred` 이벤트 미사용 경고는 인터페이스에 정의된 이벤트이므로 정상입니다.

