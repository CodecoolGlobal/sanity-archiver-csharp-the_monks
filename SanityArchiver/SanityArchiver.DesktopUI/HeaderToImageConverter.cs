﻿using System;
using System.Globalization;
using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF_Explorer_Tree
{
    #region HeaderToImageConverter

    [ValueConversion(typeof(string), typeof(bool))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as string).Contains(@"\"))
            {
                var x = Environment.CurrentDirectory.ToString();
                
/*                var path = Path.GetFullPath("pack://application:,,,/Images/diskdrive.png");
                var dir = AppDomain.CurrentDomain.BaseDirectory.ToString();*/
                Uri uri = new Uri(@"pack://application:,,,/Images/diskdrive.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else
            {
                Uri uri = new Uri(@"pack://application:,,,/Images/folder.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }

    #endregion // DoubleToIntegerConverter


}
