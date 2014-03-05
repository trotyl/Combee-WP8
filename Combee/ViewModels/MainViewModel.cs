using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Combee.Resources;

namespace Combee.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.AllReceiptsItems = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// ItemViewModel 对象的集合。
        /// </summary>
        public ObservableCollection<ItemViewModel> AllReceiptsItems { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// 返回本地化字符串的示例属性
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建一些 ItemViewModel 对象并将其添加到 Items 集合中。
        /// </summary>
        public void LoadData()
        {
            // 示例数据；替换为实际数据
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime one", AuthorName = "Maecenas praesent accumsan bibendum", Body = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime two", AuthorName = "Dictumst eleifend facilisi faucibus", Body = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime three", AuthorName = "Habitant inceptos interdum lobortis", Body = "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime four", AuthorName = "Nascetur pharetra placerat pulvinar", Body = "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime five", AuthorName = "Maecenas praesent accumsan bibendum", Body = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime six", AuthorName = "Dictumst eleifend facilisi faucibus", Body = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime seven", AuthorName = "Habitant inceptos interdum lobortis", Body = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime eight", AuthorName = "Nascetur pharetra placerat pulvinar", Body = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime nine", AuthorName = "Maecenas praesent accumsan bibendum", Body = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime ten", AuthorName = "Dictumst eleifend facilisi faucibus", Body = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime eleven", AuthorName = "Habitant inceptos interdum lobortis", Body = "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime twelve", AuthorName = "Nascetur pharetra placerat pulvinar", Body = "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime thirteen", AuthorName = "Maecenas praesent accumsan bibendum", Body = "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime fourteen", AuthorName = "Dictumst eleifend facilisi faucibus", Body = "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime fifteen", AuthorName = "Habitant inceptos interdum lobortis", Body = "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat" });
            this.AllReceiptsItems.Add(new ItemViewModel() { Title = "runtime sixteen", AuthorName = "Nascetur pharetra placerat pulvinar", Body = "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum" });

            this.IsDataLoaded = true;
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