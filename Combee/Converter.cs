using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace Combee
{
    public class StringToImageSource : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                BitmapImage ret = new BitmapImage();
                string filePath = Storage.GetSmallImage((string)value);

                if (filePath.Substring(0, 4) == "http")
                {
                    //return filePath;
                    return null;
                }

                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (iso.FileExists(filePath))
                    { // 使用隔离存储中的文件
                        using (var source = iso.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                        {
                            ret.SetSource(source);
                        }
                    }
                    else
                    { // 使用资源文件(注意将BuildAction设置为Content)
                        Uri uri = new Uri(filePath, UriKind.RelativeOrAbsolute);
                        StreamResourceInfo sri = Application.GetResourceStream(uri);
                        ret.SetSource(sri.Stream);
                    }

                    return ret;
                }
            }
            catch (Exception)
            {
                return null; // 如果发生异常，让图片位置为空
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }

    public class DisplayTime : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DateTime dt = (DateTime)value;
                if(dt.Date == DateTime.Now.Date)
                {
                    return dt.ToShortTimeString();
                }
                else
                {
                    return dt.ToShortDateString();
                }
            }
            catch (Exception)
            {
                return null; 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}

