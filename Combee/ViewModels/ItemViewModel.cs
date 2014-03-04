using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Combee.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string _title;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _authorName;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string AuthorName
        {
            get
            {
                return _authorName;
            }
            set
            {
                if (value != _authorName)
                {
                    _authorName = value;
                    NotifyPropertyChanged("AuthorName");
                }
            }
        }

        private DateTime? _createdAt;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public DateTime? CreatedAt
        {
            get
            {
                return _createdAt;
            }
            set
            {
                if (value != _createdAt)
                {
                    _createdAt = value;
                    NotifyPropertyChanged("CreatedAt");
                }
            }
        }

        private string _body;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                if (value != _body)
                {
                    _body = value;
                    NotifyPropertyChanged("Body");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}