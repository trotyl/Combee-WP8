using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;

namespace Combee
{
    public partial class Post : PhoneApplicationPage
    {
        public Post()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //加载正文信息
            string id = NavigationContext.QueryString["id"];


            WebClient newWebClient = new WebClient();
            newWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedPost);
            Uri uri = new Uri(Json.host + "posts" + @"/" + id + Json.rear + ThisUser.private_token);

            newWebClient.DownloadStringAsync(uri);
        }

        private void RetrievedPost(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message.ToString());
            }
            else
            {
                JObject o = JObject.Parse(e.Result);
                ContentBrowser.IsEnabled = true;
                ContentBrowser.IsHitTestVisible = true;
                ContentBrowser.IsScriptEnabled = true;
                ContentBrowser.NavigateToString("<html><head></head><body>Silverlight Developer Site" + "<iframe src=\"http://www.silverlight.net\" WIDTH=300 HEIGHT=200>" + "</iframe></body></html>");
                TitleTextBlock.Text = (string)o["title"];
                FromTextBlock.Text = (string)o["author"]["name"];
            }

        }

    }
}