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
using BindingData.Model;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;

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

            var query = from Receipts rpt in App.NewViewModel.myDB.ReceiptsTable
                        where rpt.PostId == id
                        select rpt;
            if(query.Count() != 0)
            {
                Receipts r = query.First();
                TitleTextBlock.Text = r.Title;
                FromTextBlock.Text = r.AuthorName;

                IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream fileStream = isoFile.OpenFile(Storage.GetSmallImage(r.AuthorAvatar), FileMode.Open, FileAccess.Read);
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(fileStream);
                AvatarImage.Source = bitmap;

                string rawHtml = string.Empty;
                rawHtml += "<html><body bgcolor=\"#34495E\"><p>";
                rawHtml += "<font color=\"#FFFFFF\">";
                rawHtml += r.BodyHtml;
                rawHtml += "</p></font></body></html>";
                ContentBrowser.NavigateToString(rawHtml);

            }

            else
            {
                WebClient newWebClient = new WebClient();
                newWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RetrievedPost);
                Uri uri = new Uri(Json.host + "posts" + @"/" + id + Json.rear + ThisUser.private_token);

                newWebClient.DownloadStringAsync(uri);
            }
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
                string rawHtml = string.Empty;
                rawHtml += "<html><body bgcolor=\"#34495E\"><p>";
                rawHtml += "<font color=\"#FFFFFF\">";
                rawHtml += (string)o["body_html"];
                rawHtml += "</p></font></body></html>";
                ContentBrowser.NavigateToString(rawHtml);
                TitleTextBlock.Text = (string)o["title"];
                FromTextBlock.Text = (string)o["author"]["name"];
            }

        }

        private void ContentBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ContentBrowser.Opacity = 1;
        }

    }
}