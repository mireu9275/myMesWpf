# WinForm 개발자를 위한 WPF 가이드

WinForm에서 WPF로 전환하는 개발자를 위한 상세 가이드입니다.

## 목차

1. [WinForm vs WPF 핵심 차이점](#1-winform-vs-wpf-핵심-차이점)
2. [MVVM 패턴 이해하기](#2-mvvm-패턴-이해하기)
3. [데이터 바인딩 마스터하기](#3-데이터-바인딩-마스터하기)
4. [Command 패턴](#4-command-패턴)
5. [스타일과 템플릿](#5-스타일과-템플릿)
6. [자주 하는 실수와 해결방법](#6-자주-하는-실수와-해결방법)

---

## 1. WinForm vs WPF 핵심 차이점

### UI 정의 방식

| WinForm | WPF |
|---------|-----|
| Designer + Code-behind | XAML 선언적 정의 |
| 픽셀 기반 위치 지정 | 레이아웃 패널 기반 |
| 컨트롤 직접 조작 | 데이터 바인딩 |

### 마인드셋 변화

**WinForm 사고방식:**
```
"버튼이 클릭되면 → 데이터를 가져와서 → 그리드에 넣는다"
```

**WPF 사고방식:**
```
"데이터가 변경되면 → UI가 자동으로 업데이트된다"
```

---

## 2. MVVM 패턴 이해하기

### 구조

```
┌─────────────────────────────────────────────────────┐
│                      View                           │
│  ┌─────────────────────────────────────────────┐   │
│  │  <TextBox Text="{Binding UserName}" />      │   │
│  │  <Button Command="{Binding SaveCommand}" /> │   │
│  └─────────────────────────────────────────────┘   │
│                         ▲                           │
│                         │ DataBinding               │
│                         ▼                           │
│  ┌─────────────────────────────────────────────┐   │
│  │              ViewModel                       │   │
│  │  public string UserName { get; set; }        │   │
│  │  public ICommand SaveCommand { get; }        │   │
│  └─────────────────────────────────────────────┘   │
│                         ▲                           │
│                         │ Uses                      │
│                         ▼                           │
│  ┌─────────────────────────────────────────────┐   │
│  │              Model / Service                 │   │
│  │  public class User { ... }                   │   │
│  │  public interface IUserService { ... }       │   │
│  └─────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

### 각 레이어의 역할

#### View (XAML)
- UI 레이아웃과 외관 정의
- 데이터 바인딩 선언
- 비즈니스 로직 **없음**

#### ViewModel (C#)
- View에 표시할 데이터 제공
- 사용자 액션 처리 (Command)
- 서비스 호출
- UI 컨트롤 직접 참조 **없음**

#### Model
- 비즈니스 데이터 구조
- 도메인 로직

---

## 3. 데이터 바인딩 마스터하기

### 기본 바인딩

```xml
<!-- 단방향: ViewModel → View -->
<TextBlock Text="{Binding UserName}" />

<!-- 양방향: View ↔ ViewModel -->
<TextBox Text="{Binding UserName, Mode=TwoWay}" />

<!-- 실시간 업데이트 -->
<TextBox Text="{Binding SearchKeyword, UpdateSourceTrigger=PropertyChanged}" />
```

### 바인딩이 작동하려면?

ViewModel이 `INotifyPropertyChanged`를 구현해야 합니다:

**CommunityToolkit.Mvvm 사용 시 (권장):**
```csharp
public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]  // 자동으로 변경 알림 생성!
    private string _userName;
}
```

**수동 구현 시:**
```csharp
public class MyViewModel : INotifyPropertyChanged
{
    private string _userName;
    public string UserName
    {
        get => _userName;
        set
        {
            if (_userName != value)
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

### 컬렉션 바인딩

```csharp
// ObservableCollection 사용 (항목 추가/삭제 시 UI 자동 업데이트)
[ObservableProperty]
private ObservableCollection<WorkOrder> _workOrders = new();

// 데이터 로드
public async Task LoadDataAsync()
{
    var data = await _service.GetWorkOrdersAsync();
    WorkOrders = new ObservableCollection<WorkOrder>(data);
}
```

```xml
<DataGrid ItemsSource="{Binding WorkOrders}" 
          SelectedItem="{Binding SelectedWorkOrder}">
    <DataGrid.Columns>
        <DataGridTextColumn Header="작업번호" Binding="{Binding WorkOrderNo}" />
        <DataGridTextColumn Header="제품명" Binding="{Binding ProductName}" />
    </DataGrid.Columns>
</DataGrid>
```

### StringFormat 사용

```xml
<!-- 숫자 포맷 -->
<TextBlock Text="{Binding Quantity, StringFormat=N0}" />           <!-- 1,234 -->
<TextBlock Text="{Binding Rate, StringFormat={}{0:F2}%}" />        <!-- 85.50% -->
<TextBlock Text="{Binding Price, StringFormat=\{0:C\}}" />         <!-- ₩10,000 -->

<!-- 날짜 포맷 -->
<TextBlock Text="{Binding CreatedAt, StringFormat=yyyy-MM-dd HH:mm}" />

<!-- 문자열 조합 -->
<TextBlock Text="{Binding Count, StringFormat=총 {0}건}" />
```

### Converter 사용

바인딩 값을 변환해야 할 때:

```csharp
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && b ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

```xml
<Window.Resources>
    <local:BoolToVisibilityConverter x:Key="BoolToVisConverter" />
</Window.Resources>

<ProgressBar Visibility="{Binding IsBusy, Converter={StaticResource BoolToVisConverter}}" />
```

---

## 4. Command 패턴

### WinForm의 이벤트 핸들러를 Command로 변환

**WinForm:**
```csharp
private async void btnSearch_Click(object sender, EventArgs e)
{
    btnSearch.Enabled = false;
    try
    {
        var data = await LoadDataAsync();
        dataGridView1.DataSource = data;
    }
    finally
    {
        btnSearch.Enabled = true;
    }
}
```

**WPF with CommunityToolkit.Mvvm:**
```csharp
[RelayCommand]
private async Task SearchAsync()
{
    IsBusy = true;
    try
    {
        var data = await _service.GetDataAsync();
        Items = new ObservableCollection<Item>(data);
    }
    finally
    {
        IsBusy = false;
    }
}
```

```xml
<Button Content="조회" 
        Command="{Binding SearchCommand}" 
        IsEnabled="{Binding IsNotBusy}" />
```

### CanExecute로 버튼 활성화 제어

```csharp
[RelayCommand(CanExecute = nameof(CanSave))]
private async Task SaveAsync()
{
    await _service.SaveAsync(CurrentItem);
}

private bool CanSave()
{
    return CurrentItem != null && 
           !string.IsNullOrEmpty(CurrentItem.Name) &&
           !IsBusy;
}

// 조건 변경 시 CanExecute 재평가
partial void OnCurrentItemChanged(Item? value)
{
    SaveCommand.NotifyCanExecuteChanged();
}
```

### CommandParameter 전달

```xml
<Button Content="삭제" 
        Command="{Binding DeleteCommand}" 
        CommandParameter="{Binding SelectedItem}" />
```

```csharp
[RelayCommand]
private async Task DeleteAsync(Item item)
{
    if (item == null) return;
    await _service.DeleteAsync(item.Id);
}
```

---

## 5. 스타일과 템플릿

### 스타일 정의

```xml
<!-- App.xaml 또는 리소스 파일에 정의 -->
<Style x:Key="PrimaryButton" TargetType="Button">
    <Setter Property="Background" Value="#1976D2" />
    <Setter Property="Foreground" Value="White" />
    <Setter Property="FontWeight" Value="Bold" />
    <Setter Property="Padding" Value="16,8" />
    <Setter Property="Cursor" Value="Hand" />
</Style>

<!-- 사용 -->
<Button Content="저장" Style="{StaticResource PrimaryButton}" />
```

### 기본 스타일 (TargetType만)

```xml
<!-- 모든 TextBlock에 적용 -->
<Style TargetType="TextBlock">
    <Setter Property="FontFamily" Value="맑은 고딕" />
    <Setter Property="FontSize" Value="13" />
</Style>
```

### DataTemplate

컬렉션 항목의 표시 방식 정의:

```xml
<ListBox ItemsSource="{Binding Alarms}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <Ellipse Width="10" Height="10" 
                         Fill="{Binding Level, Converter={StaticResource LevelToBrushConverter}}" />
                <TextBlock Text="{Binding Message}" Margin="8,0,0,0" />
                <TextBlock Text="{Binding OccurredAt, StringFormat=HH:mm:ss}" 
                           Foreground="Gray" Margin="8,0,0,0" />
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

### ControlTemplate (컨트롤 외관 완전 재정의)

```xml
<ControlTemplate x:Key="CircleButton" TargetType="Button">
    <Grid>
        <Ellipse Fill="{TemplateBinding Background}" />
        <ContentPresenter HorizontalAlignment="Center" 
                          VerticalAlignment="Center" />
    </Grid>
</ControlTemplate>

<Button Template="{StaticResource CircleButton}" 
        Background="Red" 
        Content="!" 
        Width="40" Height="40" />
```

---

## 6. 자주 하는 실수와 해결방법

### ❌ 실수 1: View에서 직접 컨트롤 조작

**잘못된 방식:**
```csharp
// Code-behind에서
private void Button_Click(object sender, RoutedEventArgs e)
{
    textBox1.Text = "Hello";
    listBox1.Items.Clear();
}
```

**올바른 방식:**
```csharp
// ViewModel에서
[RelayCommand]
private void DoSomething()
{
    Message = "Hello";
    Items.Clear();
}
```

### ❌ 실수 2: ObservableCollection 대신 List 사용

**잘못된 방식:**
```csharp
public List<Item> Items { get; set; } = new();

// 항목 추가해도 UI 업데이트 안됨!
Items.Add(newItem);
```

**올바른 방식:**
```csharp
[ObservableProperty]
private ObservableCollection<Item> _items = new();

// 항목 추가 시 UI 자동 업데이트
Items.Add(newItem);
```

### ❌ 실수 3: 비동기 초기화 누락

**잘못된 방식:**
```csharp
public MyViewModel()
{
    LoadDataAsync(); // await 없이 호출 - 문제 발생 가능
}
```

**올바른 방식:**
```csharp
public override async Task InitializeAsync()
{
    await LoadDataAsync();
}

// View에서 호출
Loaded += async (s, e) => await ViewModel.InitializeAsync();
```

### ❌ 실수 4: UI 스레드 문제

**잘못된 방식:**
```csharp
// 백그라운드 스레드에서 UI 업데이트
await Task.Run(() =>
{
    Items = new ObservableCollection<Item>(data); // 예외 발생!
});
```

**올바른 방식:**
```csharp
var data = await Task.Run(() => _service.GetData());

// UI 스레드에서 업데이트
Application.Current.Dispatcher.Invoke(() =>
{
    Items = new ObservableCollection<Item>(data);
});

// 또는 async/await 패턴 활용 (자동으로 UI 스레드 복귀)
var data = await _service.GetDataAsync();
Items = new ObservableCollection<Item>(data);
```

### ❌ 실수 5: 바인딩 오류 무시

XAML 바인딩 오류는 예외를 발생시키지 않고 조용히 실패합니다.

**디버깅 방법:**
1. Visual Studio 출력 창에서 바인딩 오류 확인
2. XAML에서 `d:DataContext` 설정으로 디자인 타임 지원
3. PresentationTraceSources로 상세 로그

```xml
<TextBlock Text="{Binding UserName, 
           PresentationTraceSources.TraceLevel=High}" />
```

---

## 핵심 정리

| WinForm | WPF |
|---------|-----|
| `textBox1.Text = "값"` | `Property = "값"` + 바인딩 |
| `button1_Click` 이벤트 | `Command` 바인딩 |
| `dataGridView.DataSource` | `ItemsSource` 바인딩 |
| `Controls.Add()` | `ObservableCollection` |
| 픽셀 단위 배치 | Grid, StackPanel 레이아웃 |
| Designer | XAML 코드 |

**기억하세요:**
> WPF에서는 "데이터가 변경되면 UI가 따라온다"는 철학으로 개발하세요!
