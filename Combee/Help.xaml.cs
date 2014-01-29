using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Combee
{
    public partial class Help : PhoneApplicationPage
    {
        public Help()
        {
            InitializeComponent();
        }

        private void HelpLink_1_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri(@"http://mp.weixin.qq.com/mp/appmsg/show?__biz=MzA4ODAyMzgzNQ==&appmsgid=10000009&itemidx=1&sign=00db43db77cf5cb974e4c38ac9b59260#wechat_redirect", UriKind.Absolute);

            webBrowserTask.Show();
        }

        private void HelpLink_2_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri(@"http://mp.weixin.qq.com/mp/appmsg/show?__biz=MzA4ODAyMzgzNQ==&appmsgid=10000011&itemidx=1&sign=a30e9aea28d6512541a12db894170bb0#wechat_redirect", UriKind.Absolute);

            webBrowserTask.Show();

        }

        private void HelpLink_3_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();

            webBrowserTask.Uri = new Uri(@"http://mp.weixin.qq.com/mp/appmsg/show?__biz=MzA4ODAyMzgzNQ==&appmsgid=10000013&itemidx=1&sign=589646f49c2c2c0e6df845011714af1f#wechat_redirect", UriKind.Absolute);

            webBrowserTask.Show();

        }
    }
}