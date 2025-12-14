using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MesClient.Core.Enums;

namespace MesClient.WPF.Converters;

/// <summary>
/// Boolean 반전 컨버터
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }
}

/// <summary>
/// Boolean 반전 -> Visibility 컨버터
/// </summary>
public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Null -> Visibility 컨버터
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var invert = parameter?.ToString() == "Invert";
        var isVisible = value != null;
        if (invert) isVisible = !isVisible;
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Null -> Boolean 컨버터
/// </summary>
public class NullToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var invert = parameter?.ToString() == "Invert";
        var result = value != null;
        if (invert) result = !result;
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 설비 상태 -> 색상 컨버터
/// </summary>
public class EquipmentStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not EquipmentStatus status)
            return Brushes.Gray;

        return status switch
        {
            EquipmentStatus.Running => new SolidColorBrush(Color.FromRgb(76, 175, 80)),     // Green
            EquipmentStatus.Idle => new SolidColorBrush(Color.FromRgb(158, 158, 158)),     // Gray
            EquipmentStatus.Down => new SolidColorBrush(Color.FromRgb(244, 67, 54)),       // Red
            EquipmentStatus.Maintenance => new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
            EquipmentStatus.Setup => new SolidColorBrush(Color.FromRgb(33, 150, 243)),     // Blue
            EquipmentStatus.Offline => new SolidColorBrush(Color.FromRgb(96, 125, 139)),   // BlueGray
            _ => Brushes.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 작업지시 상태 -> 색상 컨버터
/// </summary>
public class WorkOrderStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not WorkOrderStatus status)
            return Brushes.Gray;

        return status switch
        {
            WorkOrderStatus.Planned => new SolidColorBrush(Color.FromRgb(158, 158, 158)),   // Gray
            WorkOrderStatus.Waiting => new SolidColorBrush(Color.FromRgb(255, 193, 7)),    // Amber
            WorkOrderStatus.InProgress => new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Blue
            WorkOrderStatus.Paused => new SolidColorBrush(Color.FromRgb(255, 152, 0)),     // Orange
            WorkOrderStatus.Completed => new SolidColorBrush(Color.FromRgb(76, 175, 80)),  // Green
            WorkOrderStatus.Cancelled => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
            _ => Brushes.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 퍼센트 포맷 컨버터
/// </summary>
public class PercentageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            var format = parameter?.ToString() ?? "F1";
            return $"{d.ToString(format)}%";
        }
        return "0%";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// DateTime 포맷 컨버터
/// </summary>
public class DateTimeFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
        {
            var format = parameter?.ToString() ?? "yyyy-MM-dd HH:mm:ss";
            return dt.ToString(format);
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Enum Description 컨버터
/// </summary>
public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return string.Empty;

        var enumType = value.GetType();
        if (!enumType.IsEnum) return value.ToString() ?? string.Empty;

        var memberInfo = enumType.GetMember(value.ToString()!);
        if (memberInfo.Length > 0)
        {
            var attrs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attrs.Length > 0)
            {
                return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
            }
        }

        return value.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// EquipmentStatus -> 문자열 컨버터
/// </summary>
public class EquipmentStatusToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return "전체";
        
        if (value is EquipmentStatus status)
        {
            return status switch
            {
                EquipmentStatus.Running => "가동중",
                EquipmentStatus.Idle => "대기중",
                EquipmentStatus.Down => "고장",
                EquipmentStatus.Maintenance => "정비중",
                EquipmentStatus.Setup => "셋업중",
                EquipmentStatus.Offline => "오프라인",
                _ => status.ToString()
            };
        }
        
        return "전체";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
