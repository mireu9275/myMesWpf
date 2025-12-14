using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;

namespace MesClient.WPF.Models;

/// <summary>
/// 메뉴 아이템
/// </summary>
public partial class MenuItem : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _viewName = string.Empty;

    [ObservableProperty]
    private PackIconKind _icon = PackIconKind.None;

    [ObservableProperty]
    private bool _isSelected;

    public MenuItem(string title, string viewName, PackIconKind icon = PackIconKind.None)
    {
        Title = title;
        ViewName = viewName;
        Icon = icon;
    }
}

/// <summary>
/// 메뉴 그룹 (1단계 메뉴)
/// </summary>
public partial class MenuGroup : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private PackIconKind _icon = PackIconKind.None;

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<MenuItem> _items = new();

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isSelected;

    public MenuGroup(string title, PackIconKind icon = PackIconKind.None)
    {
        Title = title;
        Icon = icon;
    }
}

