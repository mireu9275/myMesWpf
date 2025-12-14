# MES Client - WPF 기반 제조실행시스템 클라이언트

현대적인 WPF(.NET 8) 기반의 MES(Manufacturing Execution System) 클라이언트 애플리케이션입니다.

## 📋 목차

- [주요 기능](#-주요-기능)
- [기술 스택](#-기술-스택)
- [프로젝트 구조](#-프로젝트-구조)
- [시작하기](#-시작하기)
- [아키텍처](#-아키텍처)
- [개발 가이드](#-개발-가이드)

---

## ✨ 주요 기능

### 🏭 생산관리
- 작업지시 조회 및 관리
- 실시간 생산 실적 모니터링
- 작업지시 시작/일시정지/완료 처리

### ⚙️ 설비관리
- 설비 현황 실시간 모니터링
- 설비 상태 변경 (가동/대기/고장/정비)
- 설비 가동률 분석

### 📊 품질관리
- 품질 검사 이력 조회
- 불량 유형별 통계
- LOT 추적

### 📈 대시보드
- 일일 생산 KPI (목표/실적/달성률/불량률)
- 진행중 작업지시 현황
- 설비 상태 현황
- 실시간 알람 모니터링

---

## 🛠 기술 스택

| 구분 | 기술 |
|------|------|
| **Framework** | .NET 8.0 (Windows) |
| **UI** | WPF + Material Design in XAML |
| **Architecture** | MVVM (Model-View-ViewModel) |
| **DI Container** | Microsoft.Extensions.DependencyInjection |
| **MVVM Toolkit** | CommunityToolkit.Mvvm |
| **차트** | LiveCharts2 |
| **HTTP Client** | HttpClient + Polly (Resilience) |
| **ORM** | Entity Framework Core 8 |
| **Logging** | Serilog |
| **Testing** | xUnit + Moq + FluentAssertions |

---

## 📁 프로젝트 구조

```
MesClient/
├── src/
│   ├── MesClient.Core/              # 핵심 도메인 레이어
│   │   ├── Models/                  # 도메인 모델
│   │   ├── Interfaces/              # 서비스 인터페이스
│   │   ├── Enums/                   # 열거형
│   │   └── Services/                # 비즈니스 로직
│   │
│   ├── MesClient.Infrastructure/    # 인프라 레이어
│   │   ├── Api/                     # API 클라이언트
│   │   ├── Data/                    # 데이터베이스 컨텍스트
│   │   ├── Repositories/            # 리포지토리 구현
│   │   ├── Services/                # 서비스 구현
│   │   └── Configuration/           # 설정
│   │
│   └── MesClient.WPF/               # WPF 애플리케이션
│       ├── Views/                   # XAML 뷰
│       │   ├── Dashboard/
│       │   ├── Production/
│       │   ├── Equipment/
│       │   ├── Quality/
│       │   └── Settings/
│       ├── ViewModels/              # 뷰모델
│       ├── Converters/              # 값 변환기
│       ├── Controls/                # 커스텀 컨트롤
│       ├── Resources/               # 리소스 (스타일, 색상)
│       ├── Services/                # WPF 전용 서비스
│       └── Helpers/                 # 헬퍼 클래스
│
├── tests/
│   └── MesClient.Tests/             # 단위 테스트
│
├── MesClient.sln                    # 솔루션 파일
├── Directory.Build.props            # 공통 빌드 설정
└── README.md
```

---

## 🚀 시작하기

### 필수 요구사항

- **Windows 10/11** (WPF는 Windows 전용)
- **Visual Studio 2022** (17.8 이상 권장)
- **.NET 8.0 SDK**
- **SQL Server** (로컬 또는 원격)

### 설치 및 실행

1. **저장소 클론**
   ```bash
   git clone <repository-url>
   cd MesClient
   ```

2. **Visual Studio에서 솔루션 열기**
   ```
   MesClient.sln 더블클릭
   ```

3. **NuGet 패키지 복원**
   ```
   Visual Studio에서 자동 복원 또는
   dotnet restore
   ```

4. **빌드 및 실행**
   ```bash
   dotnet build
   dotnet run --project src/MesClient.WPF
   ```

### 설정 파일

`appsettings.json`을 생성하여 서버 연결 정보를 설정하세요:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5000",
    "TimeoutSeconds": 30,
    "RetryCount": 3
  },
  "Database": {
    "ConnectionString": "Server=.;Database=MES;Trusted_Connection=True;"
  },
  "Logging": {
    "LogPath": "logs",
    "MinimumLevel": "Information"
  }
}
```

---

## 🏗 아키텍처

### Clean Architecture 적용

```
┌─────────────────────────────────────────┐
│              Presentation               │
│           (MesClient.WPF)               │
│    Views, ViewModels, Converters        │
└────────────────────┬────────────────────┘
                     │
┌────────────────────▼────────────────────┐
│              Application                │
│           (MesClient.Core)              │
│   Interfaces, Models, Business Logic    │
└────────────────────┬────────────────────┘
                     │
┌────────────────────▼────────────────────┐
│            Infrastructure               │
│       (MesClient.Infrastructure)        │
│   API Client, DB Context, Services      │
└─────────────────────────────────────────┘
```

### MVVM 패턴

```
┌─────────┐     ┌───────────┐     ┌─────────┐
│  View   │◄────│ ViewModel │◄────│  Model  │
│ (XAML)  │     │ (C#)      │     │ (C#)    │
└─────────┘     └───────────┘     └─────────┘
    │                │                  │
    │  Data Binding  │    Services      │
    └────────────────┴──────────────────┘
```

---

## 📖 개발 가이드

### WinForm에서 WPF로 전환 시 알아야 할 것들

#### 1. 데이터 바인딩 (가장 중요!)

**WinForm 방식:**
```csharp
// 직접 컨트롤 값 설정
textBox1.Text = user.Name;
label1.Text = $"Total: {count}";
```

**WPF 방식:**
```xml
<!-- XAML에서 바인딩 선언 -->
<TextBox Text="{Binding UserName}" />
<TextBlock Text="{Binding TotalCount, StringFormat=Total: {0}}" />
```

```csharp
// ViewModel에서 프로퍼티 정의
[ObservableProperty]
private string _userName;

[ObservableProperty]
private int _totalCount;
```

#### 2. 이벤트 처리 → Command

**WinForm 방식:**
```csharp
private void btnSave_Click(object sender, EventArgs e)
{
    SaveData();
}
```

**WPF 방식:**
```xml
<Button Content="저장" Command="{Binding SaveCommand}" />
```

```csharp
[RelayCommand]
private async Task SaveAsync()
{
    await _service.SaveDataAsync();
}
```

#### 3. 리스트 표시

**WinForm 방식:**
```csharp
dataGridView1.DataSource = GetWorkOrders();
```

**WPF 방식:**
```xml
<DataGrid ItemsSource="{Binding WorkOrders}" 
          SelectedItem="{Binding SelectedWorkOrder}" />
```

```csharp
[ObservableProperty]
private ObservableCollection<WorkOrder> _workOrders = new();
```

### ViewModel 작성 패턴

```csharp
public partial class MyViewModel : ViewModelBase
{
    private readonly IMyService _service;
    private readonly IDialogService _dialogService;

    // 생성자 주입
    public MyViewModel(IMyService service, IDialogService dialogService)
    {
        _service = service;
        _dialogService = dialogService;
        Title = "내 화면";
    }

    // 바인딩 프로퍼티
    [ObservableProperty]
    private string _searchKeyword = string.Empty;

    [ObservableProperty]
    private ObservableCollection<MyItem> _items = new();

    [ObservableProperty]
    private MyItem? _selectedItem;

    // 초기화
    public override async Task InitializeAsync()
    {
        await LoadDataAsync();
    }

    // Command
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var data = await _service.GetItemsAsync();
            Items = new ObservableCollection<MyItem>(data);
        });
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedItem == null) return;
        
        var confirm = await _dialogService.ShowConfirmAsync("삭제하시겠습니까?");
        if (confirm)
        {
            await _service.DeleteAsync(SelectedItem.Id);
            await LoadDataAsync();
        }
    }
}
```

### XAML 스타일 가이드

```xml
<!-- 페이지 기본 구조 -->
<UserControl>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />  <!-- 헤더 -->
            <RowDefinition Height="Auto" />  <!-- 검색 조건 -->
            <RowDefinition Height="*" />     <!-- 메인 컨텐츠 -->
        </Grid.RowDefinitions>

        <!-- 헤더 -->
        <TextBlock Text="페이지 제목" Style="{StaticResource PageTitle}" />

        <!-- 검색 조건 -->
        <materialDesign:Card Grid.Row="1" Padding="16" Margin="0,0,0,16">
            <!-- 검색 컨트롤들 -->
        </materialDesign:Card>

        <!-- 메인 컨텐츠 -->
        <materialDesign:Card Grid.Row="2">
            <DataGrid ItemsSource="{Binding Items}" />
        </materialDesign:Card>
    </Grid>
</UserControl>
```

### 새 화면 추가 방법

1. **ViewModel 생성** (`ViewModels/NewPageViewModel.cs`)
2. **View 생성** (`Views/NewPage/NewPageView.xaml`)
3. **App.xaml.cs에 DI 등록**
4. **NavigationService에 추가**
5. **MainWindow에 DataTemplate 추가**

---

## 🎨 UI 컴포넌트

### 사용 가능한 스타일

| 스타일 | 설명 |
|--------|------|
| `PageTitle` | 페이지 제목 |
| `SectionTitle` | 섹션 제목 |
| `CardTitle` | 카드 타이틀 |
| `KpiValue` | KPI 큰 숫자 |
| `MesCard` | 기본 카드 |
| `KpiCard` | KPI 카드 |
| `MesDataGrid` | 데이터그리드 |
| `SearchTextBox` | 검색 텍스트박스 |
| `NavButton` | 네비게이션 버튼 |

### 색상

| 색상 | 용도 |
|------|------|
| `PrimaryBrush` | 주요 강조색 (파랑) |
| `SuccessBrush` | 성공/완료 (초록) |
| `WarningBrush` | 경고 (노랑) |
| `ErrorBrush` | 오류/불량 (빨강) |
| `RunningBrush` | 설비 가동중 |
| `IdleBrush` | 설비 대기중 |
| `DownBrush` | 설비 고장 |

---

## 📝 라이선스

MIT License

---

## 🤝 기여

버그 리포트, 기능 제안, PR 환영합니다!
