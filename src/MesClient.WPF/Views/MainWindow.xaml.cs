using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MesClient.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MesClient.WPF.Views;

/// <summary>
/// MainWindow.xaml에 대한 상호작용 로직
/// </summary>
public partial class MainWindow : Window
{
    public static IValueConverter MenuWidthConverter { get; } = new MenuWidthValueConverter();

    public MainWindow()
    {
        InitializeComponent();
        
        var viewModel = App.Services.GetRequiredService<MainViewModel>();
        DataContext = viewModel;
        
        Loaded += async (s, e) => await viewModel.InitializeAsync();
    }

    private class MenuWidthValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isExpanded && isExpanded ? 240.0 : 64.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
